// Decompiled with JetBrains decompiler
// Type: Sculpture
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class Sculpture : Artable
{
  private static KAnimFile[] sculptureOverrides;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    if (Sculpture.sculptureOverrides == null)
      Sculpture.sculptureOverrides = new KAnimFile[1]
      {
        Assets.GetAnim(HashedString.op_Implicit("anim_interacts_sculpture_kanim"))
      };
    this.overrideAnims = Sculpture.sculptureOverrides;
    this.synchronizeAnims = false;
  }

  public override void SetStage(string stage_id, bool skip_effect)
  {
    base.SetStage(stage_id, skip_effect);
    if (skip_effect || !(this.CurrentStage != "Default"))
      return;
    KBatchedAnimController effect = FXHelpers.CreateEffect("sculpture_fx_kanim", TransformExtensions.GetPosition(this.transform), this.transform);
    effect.destroyOnAnimComplete = true;
    TransformExtensions.SetLocalPosition(((Component) effect).transform, Vector3.zero);
    effect.Play(HashedString.op_Implicit("poof"));
  }
}
