using System;
using System.Runtime.Serialization;

namespace MultiplayerMod.Platform.Base.Network.Messaging.Surrogates;

public class SpiceGrinderSurrogate : ISerializationSurrogate, ISurrogateType {

    public Type Type => typeof(SpiceGrinder.Option);

    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
        var option = (SpiceGrinder.Option) obj;
        info.AddValue("id", option.Id);
    }

    public object SetObjectData(
        object obj,
        SerializationInfo info,
        StreamingContext context,
        ISurrogateSelector selector
    ) {
        var tag = (Tag) info.GetValue("id", typeof(Tag));
        return SpiceGrinder.SettingOptions[tag];
    }
}
