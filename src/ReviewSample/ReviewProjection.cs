namespace HomeSentinel.Showcase;

/// <summary>
/// A small adaptation of HomeSentinel's finding-filter semantics, not a scanner.
/// Known review states control baseline-refresh blocking, independently of severity.
/// </summary>
public static class ReviewProjection
{
    public static ReviewReport Build(IReadOnlyList<Finding> findings, ReviewFilter filter,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(findings);
        ArgumentNullException.ThrowIfNull(filter);
        cancellationToken.ThrowIfCancellationRequested();
        if (findings.Count > 10000)
            throw new ArgumentException("The showcase accepts at most 10,000 findings.", nameof(findings));
        if (!Enum.IsDefined(filter.Mode) ||
            (filter.Mode == FilterMode.Status) != filter.Status.HasValue ||
            (filter.Status.HasValue && !Enum.IsDefined(filter.Status.Value)))
            throw new ArgumentException("Use one supported filter; Status requires a review status.", nameof(filter));

        var ids = new HashSet<long>();
        var selected = new List<Finding>();
        int blockers = 0;
        foreach (var finding in findings)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (finding is null || finding.Id <= 0 || !ids.Add(finding.Id) ||
                !Enum.IsDefined(finding.Category) || !Enum.IsDefined(finding.Severity) ||
                !Enum.IsDefined(finding.ReviewStatus))
                throw new ArgumentException("Findings require unique positive IDs and known typed values.", nameof(findings));
            if (BlocksBaselineRefresh(finding.ReviewStatus)) blockers++;
            bool visible = filter.Mode switch
            {
                FilterMode.All => true,
                FilterMode.Unreviewed => finding.ReviewStatus == ReviewStatus.Unreviewed,
                FilterMode.Reviewed => finding.ReviewStatus != ReviewStatus.Unreviewed,
                FilterMode.BaselineBlockers => BlocksBaselineRefresh(finding.ReviewStatus),
                FilterMode.Status => finding.ReviewStatus == filter.Status,
                _ => throw new ArgumentException("Unknown filter.", nameof(filter)),
            };
            if (visible) selected.Add(finding);
        }
        // Same ranking principle as the source: severity desc, category, finding identity.
        var rows = selected.OrderByDescending(f => f.Severity)
            .ThenBy(f => f.Category.ToString(), StringComparer.OrdinalIgnoreCase)
            .ThenBy(f => f.Id)
            .Select(f => new FindingRow(f.Id, f.Category.ToString(), f.Severity.ToString(),
                StatusLabel(f.ReviewStatus), BlocksBaselineRefresh(f.ReviewStatus)))
            .ToArray();
        cancellationToken.ThrowIfCancellationRequested();
        return new ReviewReport(findings.Count, rows.Length, blockers, Array.AsReadOnly(rows),
            "Review state is not proof of safety. This synthetic projection does not scan, approve a baseline, or modify Windows.");
    }

    public static bool BlocksBaselineRefresh(ReviewStatus status) => status switch
    {
        ReviewStatus.Unreviewed or ReviewStatus.NeedsReview or ReviewStatus.NeedsAction => true,
        ReviewStatus.Expected or ReviewStatus.FalsePositive => false,
        _ => throw new ArgumentOutOfRangeException(nameof(status)),
    };

    public static string StatusLabel(ReviewStatus status) => status switch
    {
        ReviewStatus.Unreviewed => "unreviewed",
        ReviewStatus.Expected => "expected",
        ReviewStatus.FalsePositive => "false-positive",
        ReviewStatus.NeedsReview => "needs-review",
        ReviewStatus.NeedsAction => "needs-action",
        _ => throw new ArgumentOutOfRangeException(nameof(status)),
    };
}
