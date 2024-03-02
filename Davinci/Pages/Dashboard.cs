using AbanoubNassem.Trinity.Components.Interfaces;
using AbanoubNassem.Trinity.Extensions;
using AbanoubNassem.Trinity.Pages;
using AbanoubNassem.Trinity.Widgets;
using SqlKata.Execution;

namespace Davinci.Pages;

public class Dashboard : TrinityPage
{
    public override string Slug => "dashboard";

    public override bool CanView => User.IsInRole("user");

    protected override List<ITrinityComponent> GetSchema()
    {
        using var conn = Configurations.ConnectionFactory();

        var stats = conn.Query()
            .From("projects")
            .SelectRaw("COUNT(*) as project_count")
            .SelectRaw("DATE(created_at) AS creation_date")
            .WhereRaw("created_at >= date('now', '-7 days')")
            .GroupBy("creation_date")
            .Get()
            .Cast<IDictionary<string, object?>>().ToList();

        if (!stats.Any()) return [];

        var total = stats.Sum(x => (long)(x["project_count"] ?? "0"));
        var lastDay = (long)(stats.Last()["project_count"] ?? 0);
        var inWeek = stats.Select(x => (long)(x["project_count"] ?? "0")).ToArray();

        return
        [
            Make<StatsWidget>("Projects", total.ToString())
                .SetDescription($"{lastDay} new since last day.", "green")
                .SetChart(inWeek)
        ];
    }
}