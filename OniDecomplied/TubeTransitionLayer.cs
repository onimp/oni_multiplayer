// Decompiled with JetBrains decompiler
// Type: TubeTransitionLayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class TubeTransitionLayer : TransitionDriver.OverrideLayer
{
  private TubeTraveller.Instance tube_traveller;
  private TravelTubeEntrance entrance;

  public TubeTransitionLayer(Navigator navigator)
    : base(navigator)
  {
    this.tube_traveller = ((Component) navigator).GetSMI<TubeTraveller.Instance>();
    if (this.tube_traveller == null || navigator.CurrentNavType != NavType.Tube || this.tube_traveller.inTube)
      return;
    this.tube_traveller.OnTubeTransition(true);
  }

  public override void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
  {
    base.BeginTransition(navigator, transition);
    this.tube_traveller.OnPathAdvanced((object) null);
    if (transition.start != NavType.Tube && transition.end == NavType.Tube)
      this.entrance = this.GetEntrance(Grid.PosToCell((KMonoBehaviour) navigator));
    else
      this.entrance = (TravelTubeEntrance) null;
  }

  public override void EndTransition(Navigator navigator, Navigator.ActiveTransition transition)
  {
    base.EndTransition(navigator, transition);
    if (transition.start != NavType.Tube && transition.end == NavType.Tube && Object.op_Implicit((Object) this.entrance))
    {
      this.entrance.ConsumeCharge(((Component) navigator).gameObject);
      this.entrance = (TravelTubeEntrance) null;
    }
    this.tube_traveller.OnTubeTransition(transition.end == NavType.Tube);
  }

  private TravelTubeEntrance GetEntrance(int cell)
  {
    if (!Grid.HasUsableTubeEntrance(cell, this.tube_traveller.prefabInstanceID))
      return (TravelTubeEntrance) null;
    GameObject gameObject = Grid.Objects[cell, 1];
    if (Object.op_Inequality((Object) gameObject, (Object) null))
    {
      TravelTubeEntrance component = gameObject.GetComponent<TravelTubeEntrance>();
      if (Object.op_Inequality((Object) component, (Object) null) && component.isSpawned)
        return component;
    }
    return (TravelTubeEntrance) null;
  }
}
