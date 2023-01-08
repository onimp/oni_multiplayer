// Decompiled with JetBrains decompiler
// Type: ModularLaunchpadPortSolidUnloaderConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class ModularLaunchpadPortSolidUnloaderConfig : IBuildingConfig
{
  public const string ID = "ModularLaunchpadPortSolidUnloader";

  public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public override BuildingDef CreateBuildingDef() => BaseModularLaunchpadPortConfig.CreateBaseLaunchpadPort("ModularLaunchpadPortSolidUnloader", "conduit_port_solid_unloader_kanim", ConduitType.Solid, false);

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) => BaseModularLaunchpadPortConfig.ConfigureBuildingTemplate(go, prefab_tag, ConduitType.Solid, 20f, false);

  public override void DoPostConfigureComplete(GameObject go) => BaseModularLaunchpadPortConfig.DoPostConfigureComplete(go, false);
}
