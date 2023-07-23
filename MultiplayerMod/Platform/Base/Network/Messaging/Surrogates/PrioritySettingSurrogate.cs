using System;
using System.Runtime.Serialization;

namespace MultiplayerMod.Platform.Base.Network.Messaging.Surrogates;

public class PrioritySettingSurrogate : ISerializationSurrogate, ISurrogateType {

    public Type Type => typeof(PrioritySetting);

    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
        var priority = (PrioritySetting) obj;
        info.AddValue("class", priority.priority_class);
        info.AddValue("value", priority.priority_value);
    }

    public object SetObjectData(
        object obj,
        SerializationInfo info,
        StreamingContext context,
        ISurrogateSelector selector
    ) {
        var priority = (PrioritySetting) obj;
        priority.priority_class = (PriorityScreen.PriorityClass) info.GetInt32("class");
        priority.priority_value = info.GetInt32("value");
        return priority;
    }

}
