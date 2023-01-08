// Decompiled with JetBrains decompiler
// Type: StickerBombConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using UnityEngine;

public class StickerBombConfig : IEntityConfig
{
  public const string ID = "StickerBomb";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject basicEntity = EntityTemplates.CreateBasicEntity("StickerBomb", (string) STRINGS.BUILDINGS.PREFABS.STICKERBOMB.NAME, (string) STRINGS.BUILDINGS.PREFABS.STICKERBOMB.DESC, 1f, true, Assets.GetAnim(HashedString.op_Implicit("sticker_a_kanim")), "off", Grid.SceneLayer.Backwall);
    EntityTemplates.AddCollision(basicEntity, EntityTemplates.CollisionShape.RECTANGLE, 1f, 1f);
    basicEntity.AddOrGet<StickerBomb>();
    return basicEntity;
  }

  public void OnPrefabInit(GameObject inst)
  {
    inst.AddOrGet<OccupyArea>().OccupiedCellsOffsets = new CellOffset[1];
    inst.AddComponent<Modifiers>();
    inst.AddOrGet<DecorProvider>().SetValues(TUNING.DECOR.BONUS.TIER2);
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
