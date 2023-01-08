// Decompiled with JetBrains decompiler
// Type: FishFeederBotConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class FishFeederBotConfig : IEntityConfig
{
  public const string ID = "FishFeederBot";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject entity = EntityTemplates.CreateEntity("FishFeederBot", "FishFeederBot");
    KBatchedAnimController kbatchedAnimController = entity.AddOrGet<KBatchedAnimController>();
    kbatchedAnimController.AnimFiles = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("fishfeeder_kanim"))
    };
    kbatchedAnimController.sceneLayer = Grid.SceneLayer.BuildingBack;
    SymbolOverrideControllerUtil.AddToPrefab(((Component) kbatchedAnimController).gameObject);
    return entity;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
