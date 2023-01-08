// Decompiled with JetBrains decompiler
// Type: BipedTransitionLayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using TUNING;
using UnityEngine;

public class BipedTransitionLayer : TransitionDriver.OverrideLayer
{
  private bool isWalking;
  private float floorSpeed;
  private float ladderSpeed;
  private float startTime;
  private float jetPackSpeed;
  private const float tubeSpeed = 18f;
  private const float downPoleSpeed = 15f;
  private const float WATER_SPEED_PENALTY = 0.5f;
  private AttributeConverterInstance movementSpeed;
  private AttributeLevels attributeLevels;

  public BipedTransitionLayer(Navigator navigator, float floor_speed, float ladder_speed)
    : base(navigator)
  {
    navigator.Subscribe(1773898642, (Action<object>) (data => this.isWalking = true));
    navigator.Subscribe(1597112836, (Action<object>) (data => this.isWalking = false));
    this.floorSpeed = floor_speed;
    this.ladderSpeed = ladder_speed;
    this.jetPackSpeed = floor_speed;
    this.movementSpeed = Db.Get().AttributeConverters.MovementSpeed.Lookup(((Component) navigator).gameObject);
    this.attributeLevels = ((Component) navigator).GetComponent<AttributeLevels>();
  }

  public override void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
  {
    base.BeginTransition(navigator, transition);
    float num1 = 1f;
    bool flag1 = (transition.start == NavType.Pole || transition.end == NavType.Pole) && transition.y < 0 && transition.x == 0;
    bool flag2 = transition.start == NavType.Tube || transition.end == NavType.Tube;
    bool flag3 = transition.start == NavType.Hover || transition.end == NavType.Hover;
    if ((flag1 || flag2 ? 0 : (!flag3 ? 1 : 0)) != 0)
    {
      if (this.isWalking)
        return;
      num1 = this.GetMovementSpeedMultiplier(navigator);
    }
    int cell = Grid.PosToCell((KMonoBehaviour) navigator);
    float num2 = 1f;
    bool flag4 = (navigator.flags & PathFinder.PotentialPath.Flags.HasAtmoSuit) != 0;
    if (!((navigator.flags & PathFinder.PotentialPath.Flags.HasJetPack) != 0 | flag4) && Grid.IsSubstantialLiquid(cell))
      num2 = 0.5f;
    float num3 = num1 * num2;
    if (transition.x == 0 && (transition.start == NavType.Ladder || transition.start == NavType.Pole) && transition.start == transition.end)
    {
      if (flag1)
      {
        transition.speed = 15f * num2;
      }
      else
      {
        transition.speed = this.ladderSpeed * num3;
        GameObject gameObject = Grid.Objects[cell, 1];
        if (Object.op_Inequality((Object) gameObject, (Object) null))
        {
          Ladder component = gameObject.GetComponent<Ladder>();
          if (Object.op_Inequality((Object) component, (Object) null))
          {
            float movementSpeedMultiplier = component.upwardsMovementSpeedMultiplier;
            if (transition.y < 0)
              movementSpeedMultiplier = component.downwardsMovementSpeedMultiplier;
            transition.speed *= movementSpeedMultiplier;
            transition.animSpeed *= movementSpeedMultiplier;
          }
        }
      }
    }
    else
      transition.speed = !flag2 ? (!flag3 ? this.floorSpeed * num3 : this.jetPackSpeed) : 18f;
    float num4 = num3 - 1f;
    transition.animSpeed += (float) ((double) transition.animSpeed * (double) num4 / 2.0);
    if (transition.start == NavType.Floor && transition.end == NavType.Floor)
    {
      int num5 = Grid.CellBelow(cell);
      if (Grid.Foundation[num5])
      {
        GameObject gameObject = Grid.Objects[num5, 1];
        if (Object.op_Inequality((Object) gameObject, (Object) null))
        {
          SimCellOccupier component = gameObject.GetComponent<SimCellOccupier>();
          if (Object.op_Inequality((Object) component, (Object) null))
          {
            transition.speed *= component.movementSpeedMultiplier;
            transition.animSpeed *= component.movementSpeedMultiplier;
          }
        }
      }
    }
    this.startTime = Time.time;
  }

  public override void EndTransition(Navigator navigator, Navigator.ActiveTransition transition)
  {
    base.EndTransition(navigator, transition);
    bool flag1 = (transition.start == NavType.Pole || transition.end == NavType.Pole) && transition.y < 0 && transition.x == 0;
    bool flag2 = transition.start == NavType.Tube || transition.end == NavType.Tube;
    if (this.isWalking || flag1 || flag2 || !Object.op_Inequality((Object) this.attributeLevels, (Object) null))
      return;
    this.attributeLevels.AddExperience(Db.Get().Attributes.Athletics.Id, Time.time - this.startTime, DUPLICANTSTATS.ATTRIBUTE_LEVELING.ALL_DAY_EXPERIENCE);
  }

  public float GetMovementSpeedMultiplier(Navigator navigator)
  {
    float num = 1f;
    if (this.movementSpeed != null)
      num += this.movementSpeed.Evaluate();
    return Mathf.Max(0.1f, num);
  }
}
