// Decompiled with JetBrains decompiler
// Type: FullPuftTransitionLayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class FullPuftTransitionLayer : TransitionDriver.OverrideLayer
{
  public FullPuftTransitionLayer(Navigator navigator)
    : base(navigator)
  {
  }

  public override void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
  {
    base.BeginTransition(navigator, transition);
    CreatureCalorieMonitor.Instance smi = ((Component) navigator).GetSMI<CreatureCalorieMonitor.Instance>();
    if (smi == null || !smi.stomach.IsReadyToPoop())
      return;
    KBatchedAnimController component = ((Component) navigator).GetComponent<KBatchedAnimController>();
    string str = HashCache.Get().Get(((HashedString) ref transition.anim).HashValue) + "_full";
    HashedString anim_name = HashedString.op_Implicit(str);
    if (!component.HasAnimation(anim_name))
      return;
    transition.anim = HashedString.op_Implicit(str);
  }
}
