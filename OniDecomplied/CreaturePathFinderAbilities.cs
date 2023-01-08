// Decompiled with JetBrains decompiler
// Type: CreaturePathFinderAbilities
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class CreaturePathFinderAbilities : PathFinderAbilities
{
  public bool canTraverseSubmered;

  public CreaturePathFinderAbilities(Navigator navigator)
    : base(navigator)
  {
  }

  protected override void Refresh(Navigator navigator)
  {
    if (PathFinder.IsSubmerged(Grid.PosToCell((KMonoBehaviour) navigator)))
      this.canTraverseSubmered = true;
    else
      this.canTraverseSubmered = Db.Get().Attributes.MaxUnderwaterTravelCost.Lookup((Component) navigator) == null;
  }

  public override bool TraversePath(
    ref PathFinder.PotentialPath path,
    int from_cell,
    NavType from_nav_type,
    int cost,
    int transition_id,
    bool submerged)
  {
    return !submerged || this.canTraverseSubmered;
  }
}
