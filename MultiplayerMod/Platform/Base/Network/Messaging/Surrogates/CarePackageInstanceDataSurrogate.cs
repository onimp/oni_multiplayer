using System;
using System.Runtime.Serialization;

namespace MultiplayerMod.Platform.Base.Network.Messaging.Surrogates;

public class CarePackageInstanceDataSurrogate : ISerializationSurrogate, ISurrogateType {

    public Type Type => typeof(CarePackageContainer.CarePackageInstanceData);

    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
        var packageInstanceData = (CarePackageContainer.CarePackageInstanceData) obj;
        info.AddValue("facadeID", packageInstanceData.facadeID);
        info.AddValue("info.id", packageInstanceData.info.id);
        info.AddValue("info.quantity", packageInstanceData.info.quantity);
        info.AddValue("info.facadeID", packageInstanceData.info.facadeID);
    }

    public object SetObjectData(
        object obj,
        SerializationInfo info,
        StreamingContext context,
        ISurrogateSelector selector
    ) {
        var packageInstanceData = (CarePackageContainer.CarePackageInstanceData) obj;
        packageInstanceData.info = new CarePackageInfo(
            info.GetString("info.id"),
            (float) info.GetDouble("info.quantity"),
            null,
            info.GetString("info.facadeID")
        );
        packageInstanceData.facadeID = info.GetString("facadeID");
        return packageInstanceData;
    }
}
