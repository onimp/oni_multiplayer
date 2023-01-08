// Decompiled with JetBrains decompiler
// Type: PathFinderAbilities
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public abstract class PathFinderAbilities
{
  protected Navigator navigator;
  protected int prefabInstanceID;

  public PathFinderAbilities(Navigator navigator) => this.navigator = navigator;

  public void Refresh()
  {
    this.prefabInstanceID = ((Component) this.navigator).gameObject.GetComponent<KPrefabID>().InstanceID;
    this.Refresh(this.navigator);
  }

  protected abstract void Refresh(Navigator navigator);

  public abstract bool TraversePath(
    ref PathFinder.PotentialPath path,
    int from_cell,
    NavType from_nav_type,
    int cost,
    int transition_id,
    bool submerged);

  public virtual int GetSubmergedPathCostPenalty(PathFinder.PotentialPath path, NavGrid.Link link) => 0;
}
