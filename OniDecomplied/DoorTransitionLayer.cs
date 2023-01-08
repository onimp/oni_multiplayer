// Decompiled with JetBrains decompiler
// Type: DoorTransitionLayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class DoorTransitionLayer : TransitionDriver.InterruptOverrideLayer
{
  private List<INavDoor> doors = new List<INavDoor>();

  public DoorTransitionLayer(Navigator navigator)
    : base(navigator)
  {
  }

  private bool AreAllDoorsOpen()
  {
    foreach (INavDoor door in this.doors)
    {
      if (door != null && !door.IsOpen())
        return false;
    }
    return true;
  }

  protected override bool IsOverrideComplete() => base.IsOverrideComplete() && this.AreAllDoorsOpen();

  public override void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
  {
    if (this.doors.Count > 0)
      return;
    int cell1 = Grid.PosToCell((KMonoBehaviour) navigator);
    int cell2 = Grid.OffsetCell(cell1, transition.x, transition.y);
    this.AddDoor(cell2);
    if (navigator.CurrentNavType != NavType.Tube)
      this.AddDoor(Grid.CellAbove(cell2));
    for (int index = 0; index < transition.navGridTransition.voidOffsets.Length; ++index)
      this.AddDoor(Grid.OffsetCell(cell1, transition.navGridTransition.voidOffsets[index]));
    if (this.doors.Count == 0)
      return;
    if (!this.AreAllDoorsOpen())
    {
      base.BeginTransition(navigator, transition);
      transition.anim = navigator.NavGrid.GetIdleAnim(navigator.CurrentNavType);
      transition.start = this.originalTransition.start;
      transition.end = this.originalTransition.start;
    }
    foreach (INavDoor door in this.doors)
      door.Open();
  }

  public override void EndTransition(Navigator navigator, Navigator.ActiveTransition transition)
  {
    base.EndTransition(navigator, transition);
    if (this.doors.Count == 0)
      return;
    foreach (INavDoor door in this.doors)
    {
      if (!Util.IsNullOrDestroyed((object) door))
        door.Close();
    }
    this.doors.Clear();
  }

  private void AddDoor(int cell)
  {
    INavDoor door = this.GetDoor(cell);
    if (Util.IsNullOrDestroyed((object) door) || this.doors.Contains(door))
      return;
    this.doors.Add(door);
  }

  private INavDoor GetDoor(int cell)
  {
    if (!Grid.HasDoor[cell])
      return (INavDoor) null;
    GameObject go = Grid.Objects[cell, 1];
    if (Object.op_Inequality((Object) go, (Object) null))
    {
      INavDoor door = go.GetComponent<INavDoor>() ?? go.GetSMI<INavDoor>();
      if (door != null && door.isSpawned)
        return door;
    }
    return (INavDoor) null;
  }
}
