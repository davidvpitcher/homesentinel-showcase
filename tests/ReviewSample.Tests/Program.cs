using System.Text.Json;
using HomeSentinel.Showcase;

int passed = 0, failed = 0;
void Equal<T>(T expected, T actual)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
        throw new InvalidOperationException($"Expected {expected}; got {actual}.");
}
void Throws<T>(Action action) where T : Exception
{
    try { action(); }
    catch (T) { return; }
    throw new InvalidOperationException($"Expected {typeof(T).Name}.");
}
void Test(string name, Action action)
{
    try { action(); passed++; Console.WriteLine($"PASS {name}"); }
    catch (Exception e) { failed++; Console.WriteLine($"FAIL {name}: {e.Message}"); }
}
string Ids(ReviewReport report) => string.Join(",", report.Rows.Select(f => f.Id));
var input = SyntheticData.Create();
Test("all findings sorted by severity", () => Equal("3,1,4,2,5", Ids(ReviewProjection.Build(input, new()))));
Test("unreviewed filter", () => Equal("2", Ids(ReviewProjection.Build(input, new(FilterMode.Unreviewed)))));
Test("reviewed filter", () => Equal("3,1,4,5", Ids(ReviewProjection.Build(input, new(FilterMode.Reviewed)))));
Test("blocker filter", () => Equal("3,2,5", Ids(ReviewProjection.Build(input, new(FilterMode.BaselineBlockers)))));
Test("exact status filter", () => Equal("1", Ids(ReviewProjection.Build(input, new(FilterMode.Status, ReviewStatus.Expected)))));
Test("counts remain distinct", () => {var r=ReviewProjection.Build(input,new(FilterMode.Status,ReviewStatus.Expected));Equal(5,r.TotalFindings);Equal(1,r.VisibleFindings);Equal(3,r.TotalBaselineBlockers);});
Test("severity is not approval", () => {Equal(false,ReviewProjection.BlocksBaselineRefresh(ReviewStatus.Expected));Equal(true,ReviewProjection.BlocksBaselineRefresh(ReviewStatus.Unreviewed));});
Test("all known blocker states", () => {
    foreach(var s in Enum.GetValues<ReviewStatus>()) Equal(s is ReviewStatus.Unreviewed or ReviewStatus.NeedsReview or ReviewStatus.NeedsAction,ReviewProjection.BlocksBaselineRefresh(s));
});
Test("source labels preserved", () => {Equal("false-positive",ReviewProjection.StatusLabel(ReviewStatus.FalsePositive));Equal("needs-review",ReviewProjection.StatusLabel(ReviewStatus.NeedsReview));Equal("needs-action",ReviewProjection.StatusLabel(ReviewStatus.NeedsAction));});
Test("deterministic category then id ordering", () => {
    Finding[] f=[new(9,Category.Network,Severity.Low,ReviewStatus.Expected,"fake"),new(7,Category.Access,Severity.Low,ReviewStatus.Expected,"fake"),new(6,Category.Access,Severity.Low,ReviewStatus.Expected,"fake")];
    Equal("6,7,9",Ids(ReviewProjection.Build(f,new())));
});
Test("input permutation does not change output", () => Equal(Ids(ReviewProjection.Build(input,new())),Ids(ReviewProjection.Build(input.Reverse().ToArray(),new()))));
Test("empty input", () => {var r=ReviewProjection.Build([],new());Equal(0,r.TotalFindings);Equal(0,r.TotalBaselineBlockers);});
Test("unknown severity is retained", () => Equal(1,ReviewProjection.Build([input[0] with {Severity=Severity.Unknown}],new()).VisibleFindings));
Test("null input rejected", () => Throws<ArgumentNullException>(()=>ReviewProjection.Build(null!,new())));
Test("null filter rejected", () => Throws<ArgumentNullException>(()=>ReviewProjection.Build(input,null!)));
Test("missing status rejected", () => Throws<ArgumentException>(()=>ReviewProjection.Build(input,new(FilterMode.Status))));
Test("conflicting filter rejected", () => Throws<ArgumentException>(()=>ReviewProjection.Build(input,new(FilterMode.All,ReviewStatus.Expected))));
Test("invalid filter mode rejected", () => Throws<ArgumentException>(()=>ReviewProjection.Build(input,new((FilterMode)999))));
Test("invalid status filter rejected", () => Throws<ArgumentException>(()=>ReviewProjection.Build(input,new(FilterMode.Status,(ReviewStatus)999))));
Test("duplicate identity rejected", () => Throws<ArgumentException>(()=>ReviewProjection.Build([input[0],input[0]],new())));
Test("nonpositive identity rejected", () => Throws<ArgumentException>(()=>ReviewProjection.Build([input[0] with {Id=0}],new())));
Test("unknown review state rejected", () => Throws<ArgumentException>(()=>ReviewProjection.Build([input[0] with {ReviewStatus=(ReviewStatus)999}],new())));
Test("unknown category rejected", () => Throws<ArgumentException>(()=>ReviewProjection.Build([input[0] with {Category=(Category)999}],new())));
Test("invalid severity rejected", () => Throws<ArgumentException>(()=>ReviewProjection.Build([input[0] with {Severity=(Severity)999}],new())));
Test("null row rejected", () => Throws<ArgumentException>(()=>ReviewProjection.Build([null!],new())));
Test("bounded input", () => Throws<ArgumentException>(()=>ReviewProjection.Build(Enumerable.Range(1,10001).Select(i=>input[0] with {Id=i}).ToArray(),new())));
Test("cancellation", () => Throws<OperationCanceledException>(()=>ReviewProjection.Build(input,new(),new CancellationToken(true))));
Test("no source mutation", () => {var before=JsonSerializer.Serialize(input);ReviewProjection.Build(input,new(FilterMode.BaselineBlockers));Equal(before,JsonSerializer.Serialize(input));});
Test("positive export projection", () => {
    var json=JsonSerializer.Serialize(ReviewProjection.Build(input,new()));
    Equal(false,json.Contains("SYNTHETIC-PRIVATE",StringComparison.Ordinal));Equal(false,json.Contains("PrivateEvidence",StringComparison.Ordinal));
    using var doc=JsonDocument.Parse(json);Equal("Id,Category,Severity,ReviewStatus,BlocksBaselineRefresh",string.Join(",",doc.RootElement.GetProperty("Rows")[0].EnumerateObject().Select(p=>p.Name)));
});
Test("returned collection is read only", () => {
    var r=ReviewProjection.Build(input,new());
    Throws<NotSupportedException>(()=>((IList<FindingRow>)r.Rows).Add(r.Rows[0]));
});
Console.WriteLine($"RESULT: {passed} passed, {failed} failed");
return failed == 0 ? 0 : 1;
