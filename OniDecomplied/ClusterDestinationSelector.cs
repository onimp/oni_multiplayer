// Decompiled with JetBrains decompiler
// Type: ClusterDestinationSelector
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;

public class ClusterDestinationSelector : KMonoBehaviour
{
  [Serialize]
  protected AxialI m_destination;
  public bool assignable;
  public bool requireAsteroidDestination;
  [Serialize]
  public bool canNavigateFogOfWar;
  public bool requireLaunchPadOnAsteroidDestination;
  public bool shouldPointTowardsPath;
  private EventSystem.IntraObjectHandler<ClusterDestinationSelector> OnClusterLocationChangedDelegate = new EventSystem.IntraObjectHandler<ClusterDestinationSelector>((Action<ClusterDestinationSelector, object>) ((cmp, data) => cmp.OnClusterLocationChanged(data)));

  protected virtual void OnPrefabInit() => this.Subscribe<ClusterDestinationSelector>(-1298331547, this.OnClusterLocationChangedDelegate);

  protected virtual void OnClusterLocationChanged(object data)
  {
    if (!AxialI.op_Equality(((ClusterLocationChangedEvent) data).newLocation, this.m_destination))
      return;
    this.Trigger(1796608350, data);
  }

  public int GetDestinationWorld() => ClusterUtil.GetAsteroidWorldIdAtLocation(this.m_destination);

  public AxialI GetDestination() => this.m_destination;

  public virtual void SetDestination(AxialI location)
  {
    if (this.requireAsteroidDestination)
      Debug.Assert(ClusterUtil.GetAsteroidWorldIdAtLocation(location) != -1, (object) string.Format("Cannot SetDestination to {0} as there is no world there", (object) location));
    this.m_destination = location;
    this.Trigger(543433792, (object) location);
  }

  public bool HasAsteroidDestination() => ClusterUtil.GetAsteroidWorldIdAtLocation(this.m_destination) != -1;

  public virtual bool IsAtDestination() => AxialI.op_Equality(this.GetMyWorldLocation(), this.m_destination);
}
