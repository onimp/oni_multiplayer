// Decompiled with JetBrains decompiler
// Type: ModularLaunchpadPortSolidConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class ModularLaunchpadPortSolidConfig : IBuildingConfig
{
  public const string ID = "ModularLaunchpadPortSolid";

  public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public override BuildingDef CreateBuildingDef() => BaseModularLaunchpadPortConfig.CreateBaseLaunchpadPort("ModularLaunchpadPortSolid", "conduit_port_solid_loader_kanim", ConduitType.Solid, true, height: 2);

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) => BaseModularLaunchpadPortConfig.ConfigureBuildingTemplate(go, prefab_tag, ConduitType.Solid, 20f, true);

  public override void DoPostConfigureComplete(GameObject go) => BaseModularLaunchpadPortConfig.DoPostConfigureComplete(go, true);
}
