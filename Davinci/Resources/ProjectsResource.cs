using AbanoubNassem.Trinity.Columns;
using AbanoubNassem.Trinity.Components;
using AbanoubNassem.Trinity.Components.TrinityAction;
using AbanoubNassem.Trinity.Extensions;
using AbanoubNassem.Trinity.Fields;
using AbanoubNassem.Trinity.Notifications;
using AbanoubNassem.Trinity.Resources;
using FluentValidation;
using SqlKata;
using SqlKata.Execution;

namespace Davinci.Resources;

public class ProjectsResource : TrinityResource
{
    public override string TitleColumn => "title";
    public override bool CanView => User.IsInRole("user");
    public override bool CanCreate => User.IsInRole("user");
    public override bool CanUpdate => User.IsInRole("user");
    public override bool CanDelete => User.IsInRole("user");
    protected override bool WithDefaultEditAction => false;
    protected override bool WithDefaultDeleteAction => false;
    protected override bool WithDefaultBulkDeleteAction => false;

    public override bool CanUpdateRecord(IDictionary<string, object?>? record)
    {
        return record?["user_id"]?.ToString() == User.Id();
    }

    protected override void OnIndexQuery(ref Query query)
    {
        query.Select("t.user_id")
            .SelectRaw("CASE WHEN t.id IS NOT NULL THEN COUNT(up.project_id) ELSE NULL END AS in_project")
            .SelectRaw("CASE WHEN t.id IS NOT NULL THEN COUNT(tsk.project_id) ELSE NULL END AS tasks_count")
            .LeftJoin("users_projects AS up", (j) =>
            {
                j.On("up.project_id", "t.id");
                j.Where("up.user_id", "=", User.Id());
                return j;
            })
            .LeftJoin("tasks AS tsk", "t.id", "tsk.project_id")
            .GroupBy(["t.id"]);

    }


    protected override Task BeforeIndex(IDictionary<string, object?> record)
    {
        //before sending the record to front end we remove user id , so we don't leak it
        record.Remove("user_id");
        return Task.CompletedTask;
    }

    protected override void OnCreateQuery(ref Query query)
    {
        var c = query.GetOneComponent<InsertClause>("insert");
        c.Columns.Add("user_id");
        c.Values.Add(User.Id());
    }

    protected override void OnUpdateQuery(ref Query query)
    {
        query.Where("user_id", User.Id());
    }

    protected override void OnDeleteQuery(ref Query query)
    {
        query.Where("user_id", User.Id());
    }

    protected override TrinityForm GetTrinityForm()
    {
        return new TrinityForm()
            .SetSchema([

                    Make<TextField>("title")
                        .SetValidationRules(r => r.MinimumLength(8).MaximumLength(64)),
                Make<NumberField<int>>("tokens"),
                Make<DateTimeField>("created_at")
                        .SetFillUsing(_ => DateTime.Now)
                        .SetDefaultValue(DateTime.Now)
                        .SetAsHidden()
                        .SetOnlyOnCreate(),
                Make<DateTimeField>("updated_at")
                        .SetFillUsing(_ => DateTime.Now)
                        .SetDefaultValue(DateTime.Now)
                        .SetAsHidden()
                ]
            );
    }

    protected override TrinityTable GetTrinityTable()
    {
        return new TrinityTable()
            .SetColumns([
                Make<TextColumn>("title")
                    .SetAsSearchable(isIndividuallySearchable: true),
                Make<TextColumn>("tokens"),
                Make<TextColumn>("tasks_count")
                    .SetLabel("Tasks")
                    .SetAsUrl(record => $"/tasks?columnsSearch={{\"project_id\":\"{record["title"]}\"}}")
                    .SetFormatUsing(record => $"{record["tasks_count"]} Tasks")
                    .SetAsCustomColumn(),
                Make<TextColumn>("updated_at")
                    .SetAsDateTime()
                    .SetAsTimeAgo()
            ]).SetActions([
                DefaultEditAction
                    .SetAsHiddenUsing(record => record["user_id"]?.ToString() != User.Id()),
                DefaultDeleteAction
                    .SetAsHiddenUsing(record => record["user_id"]?.ToString() != User.Id()),

                Make<TrinityAction>("Join")
                    .SetLabel(Localizer["join"])
                    .SetIcon("fa-solid fa-right-to-bracket")
                    .SetRequiresConfirmation()
                    .SetConfirmText($"Are you sure you want to join this project?")
                    .SetHandleActionUsing((form, records) =>
                    {
                        var project = records.First();

                        if (project["user_id"]?.ToString() == User.Id())
                            return TrinityAction.Notification(NotificationSeverity.Error, "You can't join your own project!");

                        using var conn = ConnectionFactory();

                        var exist = conn.Query("users_projects")
                            .Where("user_id", User.Id())
                            .Where("project_id", project["id"])
                            .FirstOrDefault();

                        if (exist != null)
                        {
                            return TrinityAction.Notification(NotificationSeverity.Error, "You already in this Project!");
                        }

                        conn.Query("users_projects")
                            .Insert(new List<KeyValuePair<string, object?>>()
                            {
                                new("user_id", User.Id()),
                                new ("project_id", project["id"]),
                                new ("updated_at", DateTime.Now),
                                new ("created_at", DateTime.Now),
                            });

                        TrinityNotifications.NotifySuccess($"You have joined {project["name"]}.");
                        return TrinityAction.Redirect("");
                    })
                    .SetAsHiddenUsing(record => record["user_id"]?.ToString() == User.Id() || (long)(record["in_project"] ?? 0) >= 1),
            ]);
    }
}