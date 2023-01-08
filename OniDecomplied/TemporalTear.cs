// Decompiled with JetBrains decompiler
// Type: TemporalTear
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class TemporalTear : ClusterGridEntity
{
  [Serialize]
  private bool m_open;
  [Serialize]
  private bool m_hasConsumedCraft;

  public override string Name => Db.Get().SpaceDestinationTypes.Wormhole.typeName;

  public override EntityLayer Layer => EntityLayer.POI;

  public override List<ClusterGridEntity.AnimConfig> AnimConfigs => new List<ClusterGridEntity.AnimConfig>()
  {
    new ClusterGridEntity.AnimConfig()
    {
      animFile = Assets.GetAnim(HashedString.op_Implicit("temporal_tear_kanim")),
      initialAnim = "closed_loop"
    }
  };

  public override bool IsVisible => true;

  public override ClusterRevealLevel IsVisibleInFOW => ClusterRevealLevel.Peeked;

  protected override void OnSpawn()
  {
    base.OnSpawn();
    ((Component) ClusterManager.Instance).GetComponent<ClusterPOIManager>().RegisterTemporalTear(this);
    this.UpdateStatus();
  }

  public void UpdateStatus()
  {
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    ClusterMapVisualizer clusterMapVisualizer = (ClusterMapVisualizer) null;
    if (Object.op_Inequality((Object) ClusterMapScreen.Instance, (Object) null))
      clusterMapVisualizer = ClusterMapScreen.Instance.GetEntityVisAnim((ClusterGridEntity) this);
    if (this.IsOpen())
    {
      if (Object.op_Inequality((Object) clusterMapVisualizer, (Object) null))
        clusterMapVisualizer.PlayAnim("open_loop", (KAnim.PlayMode) 0);
      component.RemoveStatusItem(Db.Get().MiscStatusItems.TearClosed);
      component.AddStatusItem(Db.Get().MiscStatusItems.TearOpen);
    }
    else
    {
      if (Object.op_Inequality((Object) clusterMapVisualizer, (Object) null))
        clusterMapVisualizer.PlayAnim("closed_loop", (KAnim.PlayMode) 0);
      component.RemoveStatusItem(Db.Get().MiscStatusItems.TearOpen);
      ((Component) this).GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.TearClosed);
    }
  }

  protected override void OnCleanUp() => base.OnCleanUp();

  public void ConsumeCraft(Clustercraft craft)
  {
    if (!this.m_open || !AxialI.op_Equality(craft.Location, this.Location) || craft.IsFlightInProgress())
      return;
    for (int idx = 0; idx < Components.MinionIdentities.Count; ++idx)
    {
      MinionIdentity minionIdentity = Components.MinionIdentities[idx];
      if (minionIdentity.GetMyWorldId() == craft.ModuleInterface.GetInteriorWorld().id)
        Util.KDestroyGameObject(((Component) minionIdentity).gameObject);
    }
    craft.DestroyCraftAndModules();
    this.m_hasConsumedCraft = true;
  }

  public void Open()
  {
    this.m_open = true;
    this.UpdateStatus();
  }

  public bool IsOpen() => this.m_open;

  public bool HasConsumedCraft() => this.m_hasConsumedCraft;
}
