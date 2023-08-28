using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace MultiplayerMod.Platform.Steam.Network.Messaging.Surrogates;

public class MinionStartingStatsSurrogate : ISerializationSurrogate, ISurrogateType {

    public Type Type => typeof(MinionStartingStats);

    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
        var stats = (MinionStartingStats) obj;
        info.AddValue("Name", stats.Name);
        info.AddValue("NameStringKey", stats.NameStringKey);
        info.AddValue("GenderStringKey", stats.GenderStringKey);
        info.AddValue("stressTrait", stats.stressTrait?.Id);
        info.AddValue("joyTrait", stats.joyTrait?.Id);
        info.AddValue("congenitaltrait", stats.congenitaltrait?.Id);
        info.AddValue("stickerType", stats.stickerType);
        info.AddValue("voiceIdx", stats.voiceIdx);
        info.AddValue("personality", stats.personality?.Id);
        info.AddValue("Traits", stats.Traits.Select(trait => trait?.Id).ToArray());
        info.AddValue("StartingLevels.Keys", stats.StartingLevels.Keys.ToArray());
        info.AddValue("StartingLevels.Values", stats.StartingLevels.Values.ToArray());
        info.AddValue("skillAptitudes.Keys", stats.skillAptitudes.Keys.Select(skillGroup => skillGroup.Id).ToArray());
        info.AddValue("skillAptitudes.Values", stats.skillAptitudes.Values.ToArray());
    }

    public object SetObjectData(
        object obj,
        SerializationInfo info,
        StreamingContext context,
        ISurrogateSelector selector
    ) {
        var stats = (MinionStartingStats) obj;
        stats.Name = info.GetString("Name");
        stats.NameStringKey = info.GetString("NameStringKey");
        stats.GenderStringKey = info.GetString("GenderStringKey");
        stats.stressTrait = Db.Get().traits.Get(info.GetString("stressTrait"));
        stats.joyTrait = Db.Get().traits.Get(info.GetString("joyTrait"));
        stats.congenitaltrait = Db.Get().traits.TryGet(info.GetString("congenitaltrait"));
        stats.stickerType = info.GetString("stickerType");
        stats.voiceIdx = info.GetInt32("voiceIdx");
        stats.Traits = ((string[]) info.GetValue("Traits", typeof(string[]))).Select(id => Db.Get().traits.Get(id))
            .ToList();
        stats.personality = Db.Get().Personalities.Get(info.GetString("personality"));
        stats.StartingLevels =
            ToDictionary(
                (string[]) info.GetValue("StartingLevels.Keys", typeof(string[])),
                (int[]) info.GetValue("StartingLevels.Values", typeof(int[]))
            );
        stats.skillAptitudes =
            ToDictionary(
                ((string[]) info.GetValue("skillAptitudes.Keys", typeof(string[])))
                .Select(key => Db.Get().SkillGroups.Get(key)).ToArray(),
                (float[]) info.GetValue("skillAptitudes.Values", typeof(float[]))
            );
        stats.IsValid = true;
        return stats;
    }

    private static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(
        IReadOnlyList<TKey> keys,
        IReadOnlyList<TValue> values
    ) {
        var result = new Dictionary<TKey, TValue>();
        for (var i = 0; i < keys.Count; i++) {
            result[keys[i]] = values[i];
        }
        return result;
    }
}
