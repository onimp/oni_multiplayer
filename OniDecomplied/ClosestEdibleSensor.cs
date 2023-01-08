// Decompiled with JetBrains decompiler
// Type: ClosestEdibleSensor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class ClosestEdibleSensor : Sensor
{
  private Edible edible;
  private bool hasEdible;
  public bool edibleInReachButNotPermitted;

  public ClosestEdibleSensor(Sensors sensors)
    : base(sensors)
  {
  }

  public override void Update()
  {
    HashSet<Tag> forbiddenTagSet = this.GetComponent<ConsumableConsumer>().forbiddenTagSet;
    Pickupable edibleFetchTarget = Game.Instance.fetchManager.FindEdibleFetchTarget(this.GetComponent<Storage>(), forbiddenTagSet, GameTags.Edible);
    bool reachButNotPermitted = this.edibleInReachButNotPermitted;
    Edible edible = (Edible) null;
    bool flag1 = false;
    bool flag2;
    if (Object.op_Inequality((Object) edibleFetchTarget, (Object) null))
    {
      edible = ((Component) edibleFetchTarget).GetComponent<Edible>();
      flag1 = true;
      flag2 = false;
    }
    else
      flag2 = Object.op_Inequality((Object) Game.Instance.fetchManager.FindEdibleFetchTarget(this.GetComponent<Storage>(), new HashSet<Tag>(), GameTags.Edible), (Object) null);
    if (!Object.op_Inequality((Object) edible, (Object) this.edible) && this.hasEdible == flag1)
      return;
    this.edible = edible;
    this.hasEdible = flag1;
    this.edibleInReachButNotPermitted = flag2;
    this.Trigger(86328522, (object) this.edible);
  }

  public Edible GetEdible() => this.edible;
}
