// Decompiled with JetBrains decompiler
// Type: MouthAnimation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class MouthAnimation : IEntityConfig
{
  public static string ID = nameof (MouthAnimation);

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject entity = EntityTemplates.CreateEntity(MouthAnimation.ID, MouthAnimation.ID, false);
    entity.AddOrGet<KBatchedAnimController>().AnimFiles = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_mouth_flap_kanim"))
    };
    return entity;
  }

  public void OnPrefabInit(GameObject go)
  {
  }

  public void OnSpawn(GameObject go)
  {
  }
}
