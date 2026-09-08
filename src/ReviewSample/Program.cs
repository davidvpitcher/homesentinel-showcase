using System.Text.Json;
using HomeSentinel.Showcase;

if (args.Length > 1)
{
    Console.Error.WriteLine("Usage: dotnet run -- [all|unreviewed|reviewed|blockers|expected]");
    return 2;
}
ReviewFilter? filter = (args.FirstOrDefault() ?? "all") switch
{
    "all" => new(),
    "unreviewed" => new(FilterMode.Unreviewed),
    "reviewed" => new(FilterMode.Reviewed),
    "blockers" => new(FilterMode.BaselineBlockers),
    "expected" => new(FilterMode.Status, ReviewStatus.Expected),
    _ => null,
};
if (filter is null)
{
    Console.Error.WriteLine("Unknown filter. Use all, unreviewed, reviewed, blockers or expected.");
    return 2;
}
var report = ReviewProjection.Build(SyntheticData.Create(), filter);
Console.WriteLine(JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true }));
return 0;
