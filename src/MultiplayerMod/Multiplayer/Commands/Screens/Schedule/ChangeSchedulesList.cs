using System;
using System.Collections.Generic;
using System.Linq;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Commands.Screens.Schedule;

[Serializable]
public class ChangeSchedulesList : MultiplayerCommand {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger<ChangeSchedulesList>();

    private readonly List<SerializableSchedule> serializableSchedules;

    public ChangeSchedulesList(List<global::Schedule> schedules) {
        serializableSchedules = schedules.Select(schedule => new SerializableSchedule(schedule)).ToList();
    }

    public override void Execute(MultiplayerCommandContext context) {
        var manager = ScheduleManager.Instance;
        var schedules = manager.schedules;

        for (var i = 0; i < Math.Min(serializableSchedules.Count, schedules.Count); i++) {
            var schedule = schedules[i];
            var changedSchedule = serializableSchedules[i];
            schedule.name = changedSchedule.Name;
            schedule.alarmActivated = changedSchedule.AlarmActivated;
            schedule.assigned = changedSchedule.Assigned;
            schedule.SetBlocksToGroupDefaults(changedSchedule.Groups); // Triggers "Changed"
        }

        if (Math.Abs(serializableSchedules.Count - schedules.Count) > 1)
            log.Warning("Schedules update contains more than one schedule addition / removal");

        if (serializableSchedules.Count > schedules.Count) {
            // New schedules was added
            var newSchedule = serializableSchedules.Last();
            var schedule = manager.AddSchedule(newSchedule.Groups, newSchedule.Name, newSchedule.AlarmActivated);
            schedule.assigned = newSchedule.Assigned;
            schedule.Changed();
        } else if (schedules.Count > serializableSchedules.Count) {
            // A schedule was removed
            manager.DeleteSchedule(schedules.Last());
        }
    }

    [Serializable]
    private class SerializableSchedule {
        public string Name { get; }
        public bool AlarmActivated { get; }
        private List<ComponentReference<Schedulable>> assigned;
        private List<string> blocks;

        private static Dictionary<string, ScheduleGroup> groups =
            Db.Get().ScheduleGroups.allGroups.ToDictionary(
                a => a.Id,
                // It is a group for 1 hour, so it's important to change defaultSegments value to '1' from the default.
                a => new ScheduleGroup(
                    a.Id,
                    null,
                    1,
                    a.Name,
                    a.description,
                    a.notificationTooltip,
                    a.allowedTypes,
                    a.alarm
                )
            );

        public SerializableSchedule(global::Schedule schedule) {
            Name = schedule.name;
            AlarmActivated = schedule.alarmActivated;
            blocks = schedule.blocks.Select(block => block.GroupId).ToList();
            assigned = schedule.assigned
                .Select(@ref => @ref.obj.gameObject.GetComponent<Schedulable>().GetReference())
                .ToList();
        }

        public List<ScheduleGroup> Groups => blocks.Select(block => groups[block]).ToList();

        public List<Ref<Schedulable>> Assigned =>
            assigned.Select(reference => new Ref<Schedulable>(reference.GetComponent())).ToList();
    }

}
