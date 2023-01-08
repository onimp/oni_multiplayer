// Decompiled with JetBrains decompiler
// Type: MachinePartsConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class MachinePartsConfig : IEntityConfig
{
  public const string ID = "MachineParts";
  public static readonly Tag TAG = TagManager.Create("MachineParts");
  public const float MASS = 5f;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab() => EntityTemplates.CreateLooseEntity("MachineParts", (string) ITEMS.INDUSTRIAL_PRODUCTS.MACHINE_PARTS.NAME, (string) ITEMS.INDUSTRIAL_PRODUCTS.MACHINE_PARTS.DESC, 5f, true, Assets.GetAnim(HashedString.op_Implicit("buildingrelocate_kanim")), "idle", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.CIRCLE, 0.35f, 0.35f, true);

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
