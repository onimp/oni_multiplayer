// Decompiled with JetBrains decompiler
// Type: ClusterTraveler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ClusterTraveler : KMonoBehaviour, ISim200ms
{
  [MyCmpReq]
  private ClusterDestinationSelector m_destinationSelector;
  [MyCmpReq]
  private ClusterGridEntity m_clusterGridEntity;
  [Serialize]
  private float m_movePotential;
  public Func<float> getSpeedCB;
  public Func<bool, bool> getCanTravelCB;
  public Func<AxialI, bool> validateTravelCB;
  public System.Action onTravelCB;
  private AxialI m_cachedPathDestination;
  private List<AxialI> m_cachedPath;
  private bool m_isPathDirty;
  public bool stopAndNotifyWhenPathChanges;
  private static EventSystem.IntraObjectHandler<ClusterTraveler> ClusterDestinationChangedHandler = new EventSystem.IntraObjectHandler<ClusterTraveler>((Action<ClusterTraveler, object>) ((cmp, data) => cmp.OnClusterDestinationChanged(data)));

  public List<AxialI> CurrentPath
  {
    get
    {
      if (this.m_cachedPath == null || AxialI.op_Inequality(this.m_destinationSelector.GetDestination(), this.m_cachedPathDestination))
      {
        this.m_cachedPathDestination = this.m_destinationSelector.GetDestination();
        this.m_cachedPath = ClusterGrid.Instance.GetPath(this.m_clusterGridEntity.Location, this.m_cachedPathDestination, this.m_destinationSelector);
      }
      return this.m_cachedPath;
    }
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    Components.ClusterTravelers.Add(this);
  }

  protected virtual void OnCleanUp()
  {
    Components.ClusterTravelers.Remove(this);
    Game.Instance.Unsubscribe(-1991583975, new Action<object>(this.OnClusterFogOfWarRevealed));
    base.OnCleanUp();
  }

  private void ForceRevealLocation(AxialI location)
  {
    if (ClusterGrid.Instance.IsCellVisible(location))
      return;
    ((Component) SaveGame.Instance).GetSMI<ClusterFogOfWarManager.Instance>().RevealLocation(location);
  }

  protected virtual void OnSpawn()
  {
    this.Subscribe<ClusterTraveler>(543433792, ClusterTraveler.ClusterDestinationChangedHandler);
    Game.Instance.Subscribe(-1991583975, new Action<object>(this.OnClusterFogOfWarRevealed));
    this.UpdateAnimationTags();
    this.MarkPathDirty();
    this.RevalidatePath(false);
    this.ForceRevealLocation(this.m_clusterGridEntity.Location);
  }

  private void MarkPathDirty() => this.m_isPathDirty = true;

  private void OnClusterFogOfWarRevealed(object data) => this.MarkPathDirty();

  private void OnClusterDestinationChanged(object data)
  {
    if (this.m_destinationSelector.IsAtDestination())
    {
      this.m_movePotential = 0.0f;
      if (this.CurrentPath != null)
        this.CurrentPath.Clear();
    }
    this.MarkPathDirty();
  }

  public int GetDestinationWorldID() => this.m_destinationSelector.GetDestinationWorld();

  public float TravelETA() => !this.IsTraveling() || this.getSpeedCB == null ? 0.0f : this.RemainingTravelDistance() / this.getSpeedCB();

  public float RemainingTravelDistance()
  {
    int num = this.RemainingTravelNodes();
    if (this.GetDestinationWorldID() >= 0)
      num = Mathf.Max(num - 1, 0);
    return (float) num * 600f - this.m_movePotential;
  }

  public int RemainingTravelNodes() => Mathf.Max(0, this.CurrentPath.Count);

  public float GetMoveProgress() => this.m_movePotential / 600f;

  public bool IsTraveling() => !this.m_destinationSelector.IsAtDestination();

  public void Sim200ms(float dt)
  {
    if (!this.IsTraveling())
      return;
    bool flag1 = this.CurrentPath != null && this.CurrentPath.Count > 0;
    bool flag2 = this.m_destinationSelector.HasAsteroidDestination();
    bool flag3 = flag2 & flag1 && this.CurrentPath.Count == 1;
    if (this.getCanTravelCB != null && !this.getCanTravelCB(flag3))
      return;
    AxialI location = this.m_clusterGridEntity.Location;
    if (flag1)
    {
      int num = !flag2 ? 0 : (this.m_destinationSelector.requireLaunchPadOnAsteroidDestination ? 1 : 0);
      if (!flag2 || this.CurrentPath.Count > 1)
      {
        this.m_movePotential += dt * this.getSpeedCB();
        if ((double) this.m_movePotential >= 600.0)
        {
          this.m_movePotential = 600f;
          if (this.AdvancePathOneStep())
          {
            Debug.Assert(Object.op_Equality((Object) ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(this.m_clusterGridEntity.Location, EntityLayer.Asteroid), (Object) null), (object) string.Format("Somehow this clustercraft pathed through an asteroid at {0}", (object) this.m_clusterGridEntity.Location));
            this.m_movePotential -= 600f;
            if (this.onTravelCB != null)
              this.onTravelCB();
          }
        }
      }
      else
        this.AdvancePathOneStep();
    }
    this.RevalidatePath();
  }

  public bool AdvancePathOneStep()
  {
    if (this.validateTravelCB != null && !this.validateTravelCB(this.CurrentPath[0]))
      return false;
    AxialI location = this.CurrentPath[0];
    this.CurrentPath.RemoveAt(0);
    this.ForceRevealLocation(location);
    this.m_clusterGridEntity.Location = location;
    this.UpdateAnimationTags();
    return true;
  }

  private void UpdateAnimationTags()
  {
    if (this.CurrentPath == null)
    {
      ((Component) this.m_clusterGridEntity).RemoveTag(GameTags.BallisticEntityLaunching);
      ((Component) this.m_clusterGridEntity).RemoveTag(GameTags.BallisticEntityLanding);
      ((Component) this.m_clusterGridEntity).RemoveTag(GameTags.BallisticEntityMoving);
    }
    else if (Object.op_Inequality((Object) ClusterGrid.Instance.GetAsteroidAtCell(this.m_clusterGridEntity.Location), (Object) null))
    {
      if (this.CurrentPath.Count == 0 || AxialI.op_Equality(this.m_clusterGridEntity.Location, this.CurrentPath[this.CurrentPath.Count - 1]))
      {
        ((Component) this.m_clusterGridEntity).AddTag(GameTags.BallisticEntityLanding);
        ((Component) this.m_clusterGridEntity).RemoveTag(GameTags.BallisticEntityLaunching);
        ((Component) this.m_clusterGridEntity).RemoveTag(GameTags.BallisticEntityMoving);
      }
      else
      {
        ((Component) this.m_clusterGridEntity).AddTag(GameTags.BallisticEntityLaunching);
        ((Component) this.m_clusterGridEntity).RemoveTag(GameTags.BallisticEntityLanding);
        ((Component) this.m_clusterGridEntity).RemoveTag(GameTags.BallisticEntityMoving);
      }
    }
    else
    {
      ((Component) this.m_clusterGridEntity).AddTag(GameTags.BallisticEntityMoving);
      ((Component) this.m_clusterGridEntity).RemoveTag(GameTags.BallisticEntityLanding);
      ((Component) this.m_clusterGridEntity).RemoveTag(GameTags.BallisticEntityLaunching);
    }
  }

  public void RevalidatePath(bool react_to_change = true)
  {
    List<AxialI> updatedPath;
    string reason;
    if (!this.HasCurrentPathChanged(out reason, out updatedPath))
      return;
    if (this.stopAndNotifyWhenPathChanges & react_to_change)
    {
      this.m_destinationSelector.SetDestination(this.m_destinationSelector.GetMyWorldLocation());
      string message = (string) MISC.NOTIFICATIONS.BADROCKETPATH.TOOLTIP;
      Notification notification = new Notification((string) MISC.NOTIFICATIONS.BADROCKETPATH.NAME, NotificationType.BadMinor, (Func<List<Notification>, object, string>) ((notificationList, data) => message + notificationList.ReduceMessages(false) + "\n\n" + reason));
      ((Component) this).GetComponent<Notifier>().Add(notification);
    }
    else
      this.m_cachedPath = updatedPath;
  }

  private bool HasCurrentPathChanged(out string reason, out List<AxialI> updatedPath)
  {
    if (!this.m_isPathDirty)
    {
      reason = (string) null;
      updatedPath = (List<AxialI>) null;
      return false;
    }
    this.m_isPathDirty = false;
    updatedPath = ClusterGrid.Instance.GetPath(this.m_clusterGridEntity.Location, this.m_cachedPathDestination, this.m_destinationSelector, out reason);
    if (updatedPath == null || updatedPath.Count != this.m_cachedPath.Count)
      return true;
    for (int index = 0; index < this.m_cachedPath.Count; ++index)
    {
      if (AxialI.op_Inequality(this.m_cachedPath[index], updatedPath[index]))
        return true;
    }
    return false;
  }

  [ContextMenu("Fill Move Potential")]
  public void FillMovePotential() => this.m_movePotential = 600f;
}
