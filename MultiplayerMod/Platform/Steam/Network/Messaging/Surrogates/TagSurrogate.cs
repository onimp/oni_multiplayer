using System;
using System.Runtime.Serialization;

namespace MultiplayerMod.Platform.Steam.Network.Messaging.Surrogates;

public class TagSurrogate : ISerializationSurrogate, ISurrogateType {

    public Type Type => typeof(Tag);

    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
        var tag = (Tag) obj;
        info.AddValue("hash", tag.hash);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) {
        var tag = (Tag) obj;
        tag.hash = info.GetInt32("hash");
        tag.name = TagManager.GetProperName(tag, stripLink: true);
        return tag;
    }

}
