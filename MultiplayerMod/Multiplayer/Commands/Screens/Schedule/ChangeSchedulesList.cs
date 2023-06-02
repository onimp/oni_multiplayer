using System;
using System.Collections.Generic;
using System.Linq;

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
        private List<int> assigned;
        private List<string> blocks;

        private static Dictionary<String, ScheduleGroup> groups =
            Db.Get().ScheduleGroups.allGroups.ToDictionary(
                a => a.Id,
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
            assigned = schedule.assigned.Select(@ref => @ref.id).ToList();
        }

        public List<ScheduleGroup> Groups => blocks.Select(block => groups[block]).ToList();
        public List<Ref<Schedulable>> Assigned => assigned.Select(id => new Ref<Schedulable>(Get(id))).ToList();

        public Schedulable Get(int id) {
            var instance = KPrefabIDTracker.Get().GetInstance(id);
            return instance?.GetComponent<Schedulable>();
        }
    }

}
