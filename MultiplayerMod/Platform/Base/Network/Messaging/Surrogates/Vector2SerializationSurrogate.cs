using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace MultiplayerMod.Platform.Base.Network.Messaging.Surrogates;

public class Vector2SerializationSurrogate : ISerializationSurrogate, ISurrogateType {

    public Type Type => typeof(Vector2);

    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
        var vector = (Vector2)obj;
        info.AddValue("x", vector.x);
        info.AddValue("y", vector.y);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) {
        var vector = (Vector2)obj;
        vector.x = info.GetSingle("x");
        vector.y = info.GetSingle("y");
        return vector;
    }

}
