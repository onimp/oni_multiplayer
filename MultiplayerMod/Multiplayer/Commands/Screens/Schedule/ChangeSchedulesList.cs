using System;
using System.Collections.Generic;
using System.Linq;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Commands.Screens.Schedule;

[Serializable]
public class ChangeSchedulesList : IMultiplayerCommand {

    private readonly List<SerializableSchedule> serializableSchedules;

    public ChangeSchedulesList(List<global::Schedule> schedules) {
        serializableSchedules = schedules.Select(schedule => new SerializableSchedule(schedule)).ToList();
    }

    public void Execute() {
        ScheduleManager.Instance.schedules.Clear();
        foreach (var serializableSchedule in serializableSchedules) {
            var schedule = ScheduleManager.Instance.AddSchedule(
                serializableSchedule.Groups,
                serializableSchedule.name,
                serializableSchedule.alarmActivated
            );
            schedule.assigned = serializableSchedule.Assigned;
            schedule.Changed();
        }
    }

    [Serializable]
    private class SerializableSchedule {
        public string name;
        public bool alarmActivated;
        private List<GameObjectReference> assigned;
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
            name = schedule.name;
            alarmActivated = schedule.alarmActivated;
            blocks = schedule.blocks.Select(block => block.GroupId).ToList();
            assigned = schedule.assigned.Select(@ref => @ref.obj.gameObject.GetMultiplayerReference()).ToList();
        }

        public List<ScheduleGroup> Groups => blocks.Select(block => groups[block]).ToList();

        public List<Ref<Schedulable>> Assigned =>
            assigned.Select(reference => new Ref<Schedulable>(reference.GetComponent<Schedulable>())).ToList();
    }

}
