using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace MultiplayerMod.Platform.Steam.Network.Messaging.Surrogates;

public class Vector3SerializationSurrogate : ISerializationSurrogate, ISurrogateType {

    public Type Type => typeof(Vector3);

    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
        var vector = (Vector3)obj;
        info.AddValue("x", vector.x);
        info.AddValue("y", vector.y);
        info.AddValue("z", vector.z);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) {
        var vector = (Vector3)obj;
        vector.x = info.GetSingle("x");
        vector.y = info.GetSingle("y");
        vector.z = info.GetSingle("z");
        return vector;
    }

}
