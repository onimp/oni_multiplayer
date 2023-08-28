using System;
using System.Runtime.Serialization;

namespace MultiplayerMod.Platform.Steam.Network.Messaging.Surrogates;

public class AssignmentGroupSurrogate : ISerializationSurrogate, ISurrogateType {

    public Type Type => typeof(AssignmentGroup);

    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
        var group = (AssignmentGroup) obj;
        info.AddValue("id", group.id);
    }

    public object? SetObjectData(
        object obj,
        SerializationInfo info,
        StreamingContext context,
        ISurrogateSelector selector
    ) {
        var id = info.GetString("id");
        return global::Game.Instance.assignmentManager.assignment_groups[id];
    }

}
