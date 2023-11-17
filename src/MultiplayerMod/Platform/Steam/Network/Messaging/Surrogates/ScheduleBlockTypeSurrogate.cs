using System;
using System.Runtime.Serialization;

namespace MultiplayerMod.Platform.Steam.Network.Messaging.Surrogates;

public class ScheduleBlockTypeSurrogate : ISerializationSurrogate, ISurrogateType {

    public Type Type => typeof(ScheduleBlockType);

    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
        var scheduleBlockType = (ScheduleBlockType) obj;
        info.AddValue("id", scheduleBlockType.Id);
    }

    public object? SetObjectData(
        object obj,
        SerializationInfo info,
        StreamingContext context,
        ISurrogateSelector selector
    ) {
        var id = info.GetString("id");
        return Db.Get().ScheduleBlockTypes.Get(id);
    }
}
