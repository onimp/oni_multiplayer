// Decompiled with JetBrains decompiler
// Type: LadderDiseaseTransitionLayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using UnityEngine;

public class LadderDiseaseTransitionLayer : TransitionDriver.OverrideLayer
{
  public LadderDiseaseTransitionLayer(Navigator navigator)
    : base(navigator)
  {
  }

  public override void EndTransition(Navigator navigator, Navigator.ActiveTransition transition)
  {
    base.EndTransition(navigator, transition);
    if (transition.end != NavType.Ladder)
      return;
    int cell = Grid.PosToCell((KMonoBehaviour) navigator);
    GameObject gameObject = Grid.Objects[cell, 1];
    if (!Object.op_Inequality((Object) gameObject, (Object) null))
      return;
    PrimaryElement component1 = gameObject.GetComponent<PrimaryElement>();
    if (!Object.op_Inequality((Object) component1, (Object) null))
      return;
    PrimaryElement component2 = ((Component) navigator).GetComponent<PrimaryElement>();
    if (!Object.op_Inequality((Object) component2, (Object) null))
      return;
    SimUtil.DiseaseInfo invalid1 = SimUtil.DiseaseInfo.Invalid with
    {
      idx = component2.DiseaseIdx,
      count = (int) ((double) component2.DiseaseCount * 0.004999999888241291)
    };
    SimUtil.DiseaseInfo invalid2 = SimUtil.DiseaseInfo.Invalid with
    {
      idx = component1.DiseaseIdx,
      count = (int) ((double) component1.DiseaseCount * 0.004999999888241291)
    };
    component2.ModifyDiseaseCount(-invalid1.count, "Navigator.EndTransition");
    component1.ModifyDiseaseCount(-invalid2.count, "Navigator.EndTransition");
    if (invalid1.count > 0)
      component1.AddDisease(invalid1.idx, invalid1.count, "TransitionDriver.EndTransition");
    if (invalid2.count <= 0)
      return;
    component2.AddDisease(invalid2.idx, invalid2.count, "TransitionDriver.EndTransition");
  }
}
