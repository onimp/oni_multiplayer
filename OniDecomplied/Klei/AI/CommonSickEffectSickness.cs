// Decompiled with JetBrains decompiler
// Type: Klei.AI.CommonSickEffectSickness
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

namespace Klei.AI
{
  public class CommonSickEffectSickness : Sickness.SicknessComponent
  {
    public override object OnInfect(GameObject go, SicknessInstance diseaseInstance)
    {
      KBatchedAnimController effect = FXHelpers.CreateEffect("contaminated_crew_fx_kanim", Vector3.op_Addition(TransformExtensions.GetPosition(go.transform), new Vector3(0.0f, 0.0f, -0.1f)), go.transform, true);
      effect.Play(HashedString.op_Implicit("fx_loop"), (KAnim.PlayMode) 0);
      return (object) effect;
    }

    public override void OnCure(GameObject go, object instance_data) => TracesExtesions.DeleteObject(((Component) instance_data).gameObject);
  }
}
