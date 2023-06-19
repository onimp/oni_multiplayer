using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace MultiplayerMod.Platform.Steam.Network.Messaging.Surrogates;

public class MinionStartingStatsSurrogate : ISerializationSurrogate, ISurrogateType {

    public Type Type => typeof(MinionStartingStats);

    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
        var node = (MinionStartingStats) obj;
        info.AddValue("Name", node.Name);
        info.AddValue("NameStringKey", node.NameStringKey);
        info.AddValue("GenderStringKey", node.GenderStringKey);
        info.AddValue("stressTrait", node.stressTrait?.Id);
        info.AddValue("joyTrait", node.joyTrait?.Id);
        info.AddValue("congenitaltrait", node.congenitaltrait?.Id);
        info.AddValue("stickerType", node.stickerType);
        info.AddValue("voiceIdx", node.voiceIdx);
        info.AddValue("personality", node.personality?.Id);
        info.AddValue("Traits", node.Traits.Select(a => a?.Id).ToArray());
        info.AddValue("StartingLevels.Keys", node.StartingLevels.Keys.ToArray());
        info.AddValue("StartingLevels.Values", node.StartingLevels.Values.ToArray());
        info.AddValue("skillAptitudes.Keys", node.skillAptitudes.Keys.Select(a => a.Id).ToArray());
        info.AddValue("skillAptitudes.Values", node.skillAptitudes.Values.ToArray());
    }

    public object SetObjectData(
        object obj,
        SerializationInfo info,
        StreamingContext context,
        ISurrogateSelector selector
    ) {
        var node = (MinionStartingStats) obj;
        node.Name = info.GetString("Name");
        node.NameStringKey = info.GetString("NameStringKey");
        node.GenderStringKey = info.GetString("GenderStringKey");
        node.stressTrait = Db.Get().traits.Get(info.GetString("stressTrait"));
        node.joyTrait = Db.Get().traits.Get(info.GetString("joyTrait"));
        node.congenitaltrait = Db.Get().traits.TryGet(info.GetString("congenitaltrait"));
        node.stickerType = info.GetString("stickerType");
        node.voiceIdx = info.GetInt32("voiceIdx");
        node.Traits = ((string[]) info.GetValue("Traits", typeof(string[]))).Select(id => Db.Get().traits.Get(id))
            .ToList();
        node.personality = Db.Get().Personalities.Get(info.GetString("personality"));
        node.StartingLevels =
            ToDictionary(
                (string[]) info.GetValue("StartingLevels.Keys", typeof(string[])),
                (int[]) info.GetValue("StartingLevels.Values", typeof(int[]))
            );
        node.skillAptitudes =
            ToDictionary(
                ((string[]) info.GetValue("skillAptitudes.Keys", typeof(string[])))
                .Select(key => Db.Get().SkillGroups.Get(key)).ToArray(),
                (float[]) info.GetValue("skillAptitudes.Values", typeof(float[]))
            );
        node.IsValid = true;
        return node;
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
