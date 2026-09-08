using System.Text.Json;
using HomeSentinel.Showcase;

int passed = 0;
int failed = 0;

void Equal<T>(T expected, T actual)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
    {
        throw new InvalidOperationException($"Expected {expected}; got {actual}.");
    }
}

void Throws<T>(Action action) where T : Exception
{
    try
    {
        action();
    }
    catch (T)
    {
        return;
    }
    throw new InvalidOperationException($"Expected {typeof(T).Name}.");
}

void Test(string name, Action action)
{
    try
    {
        action();
        passed++;
        Console.WriteLine($"PASS {name}");
    }
    catch (Exception error)
    {
        failed++;
        Console.WriteLine($"FAIL {name}: {error.Message}");
    }
}

string Ids(ReviewReport report) => string.Join(",", report.Rows.Select(row => row.Id));
var input = SyntheticData.Create();

// Each filter changes the view, not the underlying findings.
Test("all findings sorted by severity", () =>
    Equal("3,1,4,2,5", Ids(ReviewProjection.Build(input, new()))));
Test("unreviewed filter", () =>
    Equal("2", Ids(ReviewProjection.Build(input, new(FilterMode.Unreviewed)))));
Test("reviewed filter", () =>
    Equal("3,1,4,5", Ids(ReviewProjection.Build(input, new(FilterMode.Reviewed)))));
Test("blocker filter", () =>
    Equal("3,2,5", Ids(ReviewProjection.Build(input, new(FilterMode.BaselineBlockers)))));
Test("exact status filter", () =>
    Equal("1", Ids(ReviewProjection.Build(input, new(FilterMode.Status, ReviewStatus.Expected)))));

Test("counts remain distinct", () =>
{
    var report = ReviewProjection.Build(input, new(FilterMode.Status, ReviewStatus.Expected));
    Equal(5, report.TotalFindings);
    Equal(1, report.VisibleFindings);
    Equal(3, report.TotalBaselineBlockers);
});

Test("severity is not approval", () =>
{
    Equal(false, ReviewProjection.BlocksBaselineRefresh(ReviewStatus.Expected));
    Equal(true, ReviewProjection.BlocksBaselineRefresh(ReviewStatus.Unreviewed));
});

Test("all known blocker states", () =>
{
    foreach (var status in Enum.GetValues<ReviewStatus>())
    {
        bool expected = status is ReviewStatus.Unreviewed or ReviewStatus.NeedsReview or ReviewStatus.NeedsAction;
        Equal(expected, ReviewProjection.BlocksBaselineRefresh(status));
    }
});

Test("source labels preserved", () =>
{
    Equal("false-positive", ReviewProjection.StatusLabel(ReviewStatus.FalsePositive));
    Equal("needs-review", ReviewProjection.StatusLabel(ReviewStatus.NeedsReview));
    Equal("needs-action", ReviewProjection.StatusLabel(ReviewStatus.NeedsAction));
});

Test("deterministic category then id ordering", () =>
{
    Finding[] findings =
    [
        new(9, Category.Network, Severity.Low, ReviewStatus.Expected, "fake"),
        new(7, Category.Access, Severity.Low, ReviewStatus.Expected, "fake"),
        new(6, Category.Access, Severity.Low, ReviewStatus.Expected, "fake"),
    ];
    Equal("6,7,9", Ids(ReviewProjection.Build(findings, new())));
});

Test("input permutation does not change output", () =>
{
    string originalOrder = Ids(ReviewProjection.Build(input, new()));
    string reversedOrder = Ids(ReviewProjection.Build(input.Reverse().ToArray(), new()));
    Equal(originalOrder, reversedOrder);
});

Test("empty input", () =>
{
    var report = ReviewProjection.Build([], new());
    Equal(0, report.TotalFindings);
    Equal(0, report.TotalBaselineBlockers);
});

Test("unknown severity is retained", () =>
{
    var report = ReviewProjection.Build([input[0] with { Severity = Severity.Unknown }], new());
    Equal(1, report.VisibleFindings);
});

// Invalid inputs and conflicting filter options must fail explicitly.
Test("null input rejected", () =>
    Throws<ArgumentNullException>(() => ReviewProjection.Build(null!, new())));
Test("null filter rejected", () =>
    Throws<ArgumentNullException>(() => ReviewProjection.Build(input, null!)));
Test("missing status rejected", () =>
    Throws<ArgumentException>(() => ReviewProjection.Build(input, new(FilterMode.Status))));
Test("conflicting filter rejected", () =>
    Throws<ArgumentException>(() => ReviewProjection.Build(input, new(FilterMode.All, ReviewStatus.Expected))));
Test("invalid filter mode rejected", () =>
    Throws<ArgumentException>(() => ReviewProjection.Build(input, new((FilterMode)999))));
Test("invalid status filter rejected", () =>
    Throws<ArgumentException>(() => ReviewProjection.Build(input, new(FilterMode.Status, (ReviewStatus)999))));
Test("duplicate identity rejected", () =>
    Throws<ArgumentException>(() => ReviewProjection.Build([input[0], input[0]], new())));
Test("nonpositive identity rejected", () =>
    Throws<ArgumentException>(() => ReviewProjection.Build([input[0] with { Id = 0 }], new())));
Test("unknown review state rejected", () =>
    Throws<ArgumentException>(() => ReviewProjection.Build([input[0] with { ReviewStatus = (ReviewStatus)999 }], new())));
Test("unknown category rejected", () =>
    Throws<ArgumentException>(() => ReviewProjection.Build([input[0] with { Category = (Category)999 }], new())));
Test("invalid severity rejected", () =>
    Throws<ArgumentException>(() => ReviewProjection.Build([input[0] with { Severity = (Severity)999 }], new())));
Test("null row rejected", () =>
    Throws<ArgumentException>(() => ReviewProjection.Build([null!], new())));

Test("bounded input", () =>
{
    var findings = Enumerable.Range(1, 10001).Select(id => input[0] with { Id = id }).ToArray();
    Throws<ArgumentException>(() => ReviewProjection.Build(findings, new()));
});

Test("cancellation", () =>
    Throws<OperationCanceledException>(() => ReviewProjection.Build(input, new(), new CancellationToken(true))));

// Export and filtering must not expose private evidence or mutate source objects.
Test("no source mutation", () =>
{
    string before = JsonSerializer.Serialize(input);
    ReviewProjection.Build(input, new(FilterMode.BaselineBlockers));
    Equal(before, JsonSerializer.Serialize(input));
});

Test("positive export projection", () =>
{
    string json = JsonSerializer.Serialize(ReviewProjection.Build(input, new()));
    Equal(false, json.Contains("SYNTHETIC-PRIVATE", StringComparison.Ordinal));
    Equal(false, json.Contains("PrivateEvidence", StringComparison.Ordinal));

    using var document = JsonDocument.Parse(json);
    string fields = string.Join(",", document.RootElement.GetProperty("Rows")[0]
        .EnumerateObject().Select(property => property.Name));
    Equal("Id,Category,Severity,ReviewStatus,BlocksBaselineRefresh", fields);
});

Test("returned collection is read only", () =>
{
    var report = ReviewProjection.Build(input, new());
    Throws<NotSupportedException>(() => ((IList<FindingRow>)report.Rows).Add(report.Rows[0]));
});

Console.WriteLine($"RESULT: {passed} passed, {failed} failed");
return failed == 0 ? 0 : 1;
