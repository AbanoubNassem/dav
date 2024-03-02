using AbanoubNassem.Trinity.Columns;
using AbanoubNassem.Trinity.Components;
using AbanoubNassem.Trinity.Components.Interfaces;
using AbanoubNassem.Trinity.Components.TrinityAction;
using AbanoubNassem.Trinity.Extensions;
using AbanoubNassem.Trinity.Fields;
using AbanoubNassem.Trinity.Layouts;
using AbanoubNassem.Trinity.Notifications;
using AbanoubNassem.Trinity.Resources;
using FluentValidation;
using SqlKata;
using SqlKata.Execution;

namespace Davinci.Resources
{
    public class TasksResource : TrinityResource
    {
        public override string TitleColumn => "short_description";
        public override bool CanView => User.IsInRole("user");
        public override bool CanCreate => User.IsInRole("user");
        public override bool CanUpdate => User.IsInRole("user");
        public override bool CanDelete => User.IsInRole("user");
        protected override bool WithDefaultEditAction => false;
        protected override bool WithDefaultDeleteAction => false;
        protected override bool WithDefaultBulkDeleteAction => false;

        protected override void OnIndexQuery(ref Query query)
        {
            query
                .Select("project.user_id")
                .SelectRaw("CASE WHEN t.id IS NOT NULL THEN COUNT(up.project_id) ELSE NULL END AS in_project")
                .LeftJoin("users_projects AS up", (j) =>
                {
                    j.On("up.project_id", "t.id");
                    j.Where("up.user_id", "=", User.Id());
                    return j;
                })
                .LeftJoin("projects as project", "t.project_id", "project.id")
                .SelectRaw("CASE WHEN t.id IS NOT NULL THEN COUNT(us.task_id) ELSE NULL END AS in_task")
                .LeftJoin("users_tasks AS us", (j) =>
                {
                    j.On("us.task_id", "t.id");
                    j.Where("us.user_id", "=", User.Id());
                    return j;
                })
                .SelectRaw("CASE WHEN t.id IS NOT NULL THEN COUNT(prt.task_id) ELSE NULL END AS participants_count")
                .LeftJoin("users_tasks AS prt", "t.id", "prt.task_id")
                .GroupBy(["t.id"]);

        }

        protected override async Task BeforeCreate(Dictionary<string, object?> form)
        {
            var project = await Configurations.ConnectionFactory()
                .Query("projects")
                .Where("user_id", User.Id())
                .Where("id", form["project_id"])
                .FirstOrDefaultAsync();

            if (project == null)
            {
                throw new Exception("You can only create tasks on your own projects!");
            }
        }

        public override bool CanUpdateRecord(IDictionary<string, object?>? record)
        {
            var project = record?["project"] as IDictionary<string, object?>;

            return project?["user_id"]?.ToString() == User.Id() || record?["user_id"]?.ToString() == User.Id();
        }

        protected override TrinityForm GetTrinityForm()
        {
            return new TrinityForm()
                .SetSchema([
                        Make<BelongsToField>("project_id", "id", "projects", "title")
                            .SetAssociatesRelationshipQueryUsing(query =>
                            {
                                query.Where("user_id", User.Id());
                            })
                            .SetAsLazy(),
                    Make<TextField>("short_description")
                            .SetValidationRules(r => r.MinimumLength(8).MaximumLength(34)),
                    Make<GridLayout>(
                            new List<IFormComponent>
                            {
                                Make<EditorField>("long_description")
                                    .SetValidationRules(r => r.MinimumLength(8).MaximumLength(5000))
                                    .SetDefaultValue("")
                                    .SetColumnSpan(12),
                            }, 1
                        ),
                    Make<SwitchInputField>("archived"),
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
                    Make<BelongsToColumn>("project_id", "projects", "id", "title", "project")
                        .SetDefaultValue("")
                        .SetAsSearchable(true, true)
                        .SetCustomFilter(
                            new TextField("project_id").SetLabel("Project"),
                            (query, str) =>
                            {
                                query.Join("projects", "t.project_id", "projects.id")
                                    .Where("projects.title", str);
                            })
                        .SetLabel("Project"),
                    Make<TextColumn>("short_description"),
                    Make<TextColumn>("participants_count")
                        .SetLabel("Participants")
                        .SetFormatUsing(record => $"{record["participants_count"]} Participants")
                        .SetAsCustomColumn(),
                    Make<IconColumn>("archived")
                    .SetAsBoolean(),
                    Make<TextColumn>("updated_at")
                        .SetAsDateTime()
                        .SetAsTimeAgo()
                ])
                .SetActions([
                    DefaultEditAction
                        .SetAsHiddenUsing(record => !CanUpdateRecord(record)),

                    DefaultDeleteAction
                        .SetAsHiddenUsing(record => !CanUpdateRecord(record)),

                    Make<TrinityAction>("Join")
                    .SetLabel(Localizer["join"])
                    .SetIcon("fa-solid fa-right-to-bracket")
                    .SetRequiresConfirmation()
                    .SetConfirmText($"Are you sure you want to join this Task?")
                    .SetHandleActionUsing((form, records) =>
                    {
                        var task = records.First();

                        if (task["user_id"]?.ToString() == User.Id())
                            return TrinityAction.Notification(NotificationSeverity.Error, "You can't join your own project!");

                        using var conn = ConnectionFactory();

                        var exist = conn.Query("users_tasks")
                        .Where("user_id", User.Id())
                        .Where("task_id", task["id"])
                        .FirstOrDefault();

                        if (exist != null)
                        {
                            return TrinityAction.Notification(NotificationSeverity.Error, "You already in this Task!");
                        }

                        conn.Query("users_tasks")
                        .Insert(new List<KeyValuePair<string, object?>>()
                        {
                            new("user_id", User.Id()),
                            new ("task_id", task["id"]),
                        });

                        TrinityNotifications.NotifySuccess($"You have joined {task["name"]}.");
                        return TrinityAction.Redirect("");
                    })
                    .SetAsHiddenUsing(record => record["user_id"]?.ToString() == User.Id() || (long)(record["in_task"] ?? 0) >= 1 || (long)(record["in_project"] ?? 0) == 0),
                ]);
        }

    }
}
