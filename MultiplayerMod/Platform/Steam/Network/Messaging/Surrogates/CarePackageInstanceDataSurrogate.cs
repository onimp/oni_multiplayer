using System;
using System.Runtime.Serialization;

namespace MultiplayerMod.Platform.Steam.Network.Messaging.Surrogates;

public class CarePackageInstanceDataSurrogate : ISerializationSurrogate, ISurrogateType {

    public Type Type => typeof(CarePackageContainer.CarePackageInstanceData);

    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
        var node = (CarePackageContainer.CarePackageInstanceData) obj;
        info.AddValue("facadeID", node.facadeID);
        info.AddValue("info.id", node.info.id);
        info.AddValue("info.quantity", node.info.quantity);
        info.AddValue("info.facadeID", node.info.facadeID);
    }

    public object SetObjectData(
        object obj,
        SerializationInfo info,
        StreamingContext context,
        ISurrogateSelector selector
    ) {
        var node = (CarePackageContainer.CarePackageInstanceData) obj;
        node.info = new CarePackageInfo(
            info.GetString("info.id"),
            (float) info.GetDouble("info.quantity"),
            null,
            info.GetString("info.facadeID")
        );
        node.facadeID = info.GetString("facadeID");
        return node;
    }
}
