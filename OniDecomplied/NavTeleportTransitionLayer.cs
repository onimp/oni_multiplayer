// Decompiled with JetBrains decompiler
// Type: NavTeleportTransitionLayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class NavTeleportTransitionLayer : TransitionDriver.OverrideLayer
{
  public NavTeleportTransitionLayer(Navigator navigator)
    : base(navigator)
  {
  }

  public override void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
  {
    base.BeginTransition(navigator, transition);
    if (transition.start != NavType.Teleport)
      return;
    int cell = Grid.PosToCell((KMonoBehaviour) navigator);
    int x1;
    int y1;
    Grid.CellToXY(cell, out x1, out y1);
    int teleportTransition = navigator.NavGrid.teleportTransitions[cell];
    int x2;
    int y2;
    Grid.CellToXY(navigator.NavGrid.teleportTransitions[cell], out x2, out y2);
    transition.x = x2 - x1;
    transition.y = y2 - y1;
  }
}
