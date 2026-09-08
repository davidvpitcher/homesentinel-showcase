namespace HomeSentinel.Showcase;

public enum Severity { Unknown, Info, Low, Medium, High, Critical }
public enum Category { Network, Access, ScheduledTasks, Services, StartupRegistry, Firewall }
public enum ReviewStatus { Unreviewed, Expected, FalsePositive, NeedsReview, NeedsAction }
public enum FilterMode { All, Unreviewed, Reviewed, BaselineBlockers, Status }

// Synthetic source input. PrivateEvidence is deliberately absent from every public DTO.
public sealed record Finding(long Id, Category Category, Severity Severity,
    ReviewStatus ReviewStatus, string PrivateEvidence);

public sealed record ReviewFilter(FilterMode Mode = FilterMode.All, ReviewStatus? Status = null);

public sealed record FindingRow(long Id, string Category, string Severity,
    string ReviewStatus, bool BlocksBaselineRefresh);

public sealed record ReviewReport(int TotalFindings, int VisibleFindings,
    int TotalBaselineBlockers, IReadOnlyList<FindingRow> Rows, string Interpretation);

public static class SyntheticData
{
    public static Finding[] Create() =>
    [
        new(1, Category.Network, Severity.High, ReviewStatus.Expected, "SYNTHETIC-PRIVATE-NETWORK"),
        new(2, Category.StartupRegistry, Severity.Low, ReviewStatus.Unreviewed, "SYNTHETIC-PRIVATE-COMMAND"),
        new(3, Category.Services, Severity.Critical, ReviewStatus.NeedsAction, "SYNTHETIC-PRIVATE-SERVICE"),
        new(4, Category.Access, Severity.Medium, ReviewStatus.FalsePositive, "SYNTHETIC-PRIVATE-ACCOUNT"),
        new(5, Category.ScheduledTasks, Severity.Info, ReviewStatus.NeedsReview, "SYNTHETIC-PRIVATE-TASK"),
    ];
}
