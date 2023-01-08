// Decompiled with JetBrains decompiler
// Type: ClusterGridEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using ProcGen;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClusterGridEntity : KMonoBehaviour
{
  [Serialize]
  protected AxialI m_location;
  public bool positionDirty;
  [MyCmpGet]
  protected KSelectable m_selectable;
  [MyCmpReq]
  private Transform m_transform;
  public bool isWorldEntity;

  public abstract string Name { get; }

  public abstract EntityLayer Layer { get; }

  public abstract List<ClusterGridEntity.AnimConfig> AnimConfigs { get; }

  public abstract bool IsVisible { get; }

  public virtual bool ShowName() => false;

  public virtual bool ShowProgressBar() => false;

  public virtual float GetProgress() => 0.0f;

  public virtual bool SpaceOutInSameHex() => false;

  public virtual bool ShowPath() => true;

  public abstract ClusterRevealLevel IsVisibleInFOW { get; }

  public AxialI Location
  {
    get => this.m_location;
    set
    {
      if (!AxialI.op_Inequality(value, this.m_location))
        return;
      AxialI location = this.m_location;
      this.m_location = value;
      if (((Component) this).gameObject.GetSMI<StateMachine.Instance>() == null)
        this.positionDirty = true;
      this.SendClusterLocationChangedEvent(location, this.m_location);
    }
  }

  protected virtual void OnSpawn()
  {
    ClusterGrid.Instance.RegisterEntity(this);
    if (Object.op_Inequality((Object) this.m_selectable, (Object) null))
      this.m_selectable.SetName(this.Name);
    if (!this.isWorldEntity)
      TransformExtensions.SetLocalPosition(this.m_transform, new Vector3(-1f, 0.0f, 0.0f));
    if (!Object.op_Inequality((Object) ClusterMapScreen.Instance, (Object) null))
      return;
    ((KMonoBehaviour) ClusterMapScreen.Instance).Trigger(1980521255, (object) null);
  }

  protected virtual void OnCleanUp() => ClusterGrid.Instance.UnregisterEntity(this);

  public virtual Sprite GetUISprite()
  {
    if (DlcManager.FeatureClusterSpaceEnabled())
    {
      List<ClusterGridEntity.AnimConfig> animConfigs = this.AnimConfigs;
      if (animConfigs.Count > 0)
        return Def.GetUISpriteFromMultiObjectAnim(animConfigs[0].animFile);
    }
    else
    {
      WorldContainer component = ((Component) this).GetComponent<WorldContainer>();
      if (Object.op_Inequality((Object) component, (Object) null))
      {
        World worldData = SettingsCache.worlds.GetWorldData(component.worldName);
        return worldData == null ? (Sprite) null : Assets.GetSprite(HashedString.op_Implicit(worldData.asteroidIcon));
      }
    }
    return (Sprite) null;
  }

  public void SendClusterLocationChangedEvent(AxialI oldLocation, AxialI newLocation)
  {
    ClusterLocationChangedEvent locationChangedEvent = new ClusterLocationChangedEvent()
    {
      entity = this,
      oldLocation = oldLocation,
      newLocation = newLocation
    };
    this.Trigger(-1298331547, (object) locationChangedEvent);
    Game.Instance.Trigger(-1298331547, (object) locationChangedEvent);
    if (!Object.op_Inequality((Object) this.m_selectable, (Object) null) || !this.m_selectable.IsSelected)
      return;
    DetailsScreen.Instance.Refresh(((Component) this).gameObject);
  }

  public struct AnimConfig
  {
    public KAnimFile animFile;
    public string initialAnim;
    public KAnim.PlayMode playMode;
    public string symbolSwapTarget;
    public string symbolSwapSymbol;
    public Vector3 animOffset;
  }
}
