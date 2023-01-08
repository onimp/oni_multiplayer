// Decompiled with JetBrains decompiler
// Type: CurryConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class CurryConfig : IEntityConfig
{
  public const string ID = "Curry";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab() => EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("Curry", (string) ITEMS.FOOD.CURRY.NAME, (string) ITEMS.FOOD.CURRY.DESC, 1f, false, Assets.GetAnim(HashedString.op_Implicit("curried_beans_kanim")), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.9f, 0.5f, true), TUNING.FOOD.FOOD_TYPES.CURRY);

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
