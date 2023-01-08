// Decompiled with JetBrains decompiler
// Type: EggShellConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class EggShellConfig : IEntityConfig
{
  public const string ID = "EggShell";
  public static readonly Tag TAG = TagManager.Create("EggShell");
  public const float EGG_TO_SHELL_RATIO = 0.5f;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject looseEntity = EntityTemplates.CreateLooseEntity("EggShell", (string) ITEMS.INDUSTRIAL_PRODUCTS.EGG_SHELL.NAME, (string) ITEMS.INDUSTRIAL_PRODUCTS.EGG_SHELL.DESC, 1f, false, Assets.GetAnim(HashedString.op_Implicit("eggshells_kanim")), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.9f, 0.6f, true);
    looseEntity.GetComponent<KPrefabID>().AddTag(GameTags.Organics, false);
    looseEntity.AddOrGet<EntitySplitter>();
    looseEntity.AddOrGet<SimpleMassStatusItem>();
    EntityTemplates.CreateAndRegisterCompostableFromPrefab(looseEntity);
    return looseEntity;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
