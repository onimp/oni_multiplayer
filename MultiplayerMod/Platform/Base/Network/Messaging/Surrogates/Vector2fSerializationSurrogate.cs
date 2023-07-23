using System;
using System.Runtime.Serialization;

namespace MultiplayerMod.Platform.Base.Network.Messaging.Surrogates;

public class Vector2fSerializationSurrogate : ISerializationSurrogate, ISurrogateType {

    public Type Type => typeof(Vector2f);

    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
        var vector = (Vector2f)obj;
        info.AddValue("x", vector.x);
        info.AddValue("y", vector.y);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) {
        var vector = (Vector2f)obj;
        vector.x = info.GetSingle("x");
        vector.y = info.GetSingle("y");
        return vector;
    }

}
