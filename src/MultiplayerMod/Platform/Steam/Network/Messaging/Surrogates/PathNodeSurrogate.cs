using System;
using System.Runtime.Serialization;

namespace MultiplayerMod.Platform.Steam.Network.Messaging.Surrogates;

public class PathNodeSurrogate : ISerializationSurrogate, ISurrogateType {

    public Type Type => typeof(BaseUtilityBuildTool.PathNode);

    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
        var node = (BaseUtilityBuildTool.PathNode) obj;
        info.AddValue("cell", node.cell);
        info.AddValue("valid", node.valid);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) {
        var node = (BaseUtilityBuildTool.PathNode) obj;
        node.cell = info.GetInt32("cell");
        node.valid = info.GetBoolean("valid");
        return node;
    }

}
