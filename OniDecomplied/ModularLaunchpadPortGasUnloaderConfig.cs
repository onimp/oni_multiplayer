// Decompiled with JetBrains decompiler
// Type: ModularLaunchpadPortGasUnloaderConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class ModularLaunchpadPortGasUnloaderConfig : IBuildingConfig
{
  public const string ID = "ModularLaunchpadPortGasUnloader";

  public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public override BuildingDef CreateBuildingDef() => BaseModularLaunchpadPortConfig.CreateBaseLaunchpadPort("ModularLaunchpadPortGasUnloader", "conduit_port_gas_unloader_kanim", ConduitType.Gas, false);

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) => BaseModularLaunchpadPortConfig.ConfigureBuildingTemplate(go, prefab_tag, ConduitType.Gas, 1f, false);

  public override void DoPostConfigureComplete(GameObject go) => BaseModularLaunchpadPortConfig.DoPostConfigureComplete(go, false);
}
