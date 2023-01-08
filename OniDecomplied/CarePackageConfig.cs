// Decompiled with JetBrains decompiler
// Type: CarePackageConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class CarePackageConfig : IEntityConfig
{
  public static readonly string ID = "CarePackage";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab() => EntityTemplates.CreateLooseEntity(CarePackageConfig.ID, (string) ITEMS.CARGO_CAPSULE.NAME, (string) ITEMS.CARGO_CAPSULE.DESC, 1f, true, Assets.GetAnim(HashedString.op_Implicit("portal_carepackage_kanim")), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE);

  public void OnPrefabInit(GameObject go) => go.AddOrGet<CarePackage>();

  public void OnSpawn(GameObject go)
  {
  }
}
