// Decompiled with JetBrains decompiler
// Type: Klei.AI.CustomSickEffectSickness
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

namespace Klei.AI
{
  public class CustomSickEffectSickness : Sickness.SicknessComponent
  {
    private string kanim;
    private string animName;

    public CustomSickEffectSickness(string effect_kanim, string effect_anim_name)
    {
      this.kanim = effect_kanim;
      this.animName = effect_anim_name;
    }

    public override object OnInfect(GameObject go, SicknessInstance diseaseInstance)
    {
      KBatchedAnimController effect = FXHelpers.CreateEffect(this.kanim, Vector3.op_Addition(TransformExtensions.GetPosition(go.transform), new Vector3(0.0f, 0.0f, -0.1f)), go.transform, true);
      effect.Play(HashedString.op_Implicit(this.animName), (KAnim.PlayMode) 0);
      return (object) effect;
    }

    public override void OnCure(GameObject go, object instance_data) => TracesExtesions.DeleteObject(((Component) instance_data).gameObject);
  }
}
