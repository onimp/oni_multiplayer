// Decompiled with JetBrains decompiler
// Type: AsteroidGridEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidGridEntity : ClusterGridEntity
{
  public static string DEFAULT_ASTEROID_ICON_ANIM = "asteroid_sandstone_start_kanim";
  [MyCmpReq]
  private WorldContainer m_worldContainer;
  [Serialize]
  private string m_name;
  [Serialize]
  private string m_asteroidAnim;

  public override bool ShowName() => true;

  public override string Name => this.m_name;

  public override EntityLayer Layer => EntityLayer.Asteroid;

  public override List<ClusterGridEntity.AnimConfig> AnimConfigs
  {
    get
    {
      List<ClusterGridEntity.AnimConfig> animConfigs = new List<ClusterGridEntity.AnimConfig>();
      ClusterGridEntity.AnimConfig animConfig = new ClusterGridEntity.AnimConfig();
      animConfig.animFile = Assets.GetAnim(HashedString.op_Implicit(Util.IsNullOrWhiteSpace(this.m_asteroidAnim) ? AsteroidGridEntity.DEFAULT_ASTEROID_ICON_ANIM : this.m_asteroidAnim));
      animConfig.initialAnim = "idle_loop";
      animConfigs.Add(animConfig);
      animConfig = new ClusterGridEntity.AnimConfig();
      animConfig.animFile = Assets.GetAnim(HashedString.op_Implicit("orbit_kanim"));
      animConfig.initialAnim = "orbit";
      animConfigs.Add(animConfig);
      return animConfigs;
    }
  }

  public override bool IsVisible => true;

  public override ClusterRevealLevel IsVisibleInFOW => ClusterRevealLevel.Peeked;

  public void Init(string name, AxialI location, string asteroidTypeId)
  {
    this.m_name = name;
    this.m_location = location;
    this.m_asteroidAnim = asteroidTypeId;
  }

  protected override void OnSpawn()
  {
    Game.Instance.Subscribe(-1298331547, new Action<object>(this.OnClusterLocationChanged));
    Game.Instance.Subscribe(-1991583975, new Action<object>(this.OnFogOfWarRevealed));
    if (ClusterGrid.Instance.IsCellVisible(this.m_location))
      ((Component) SaveGame.Instance).GetSMI<ClusterFogOfWarManager.Instance>().RevealLocation(this.m_location, 1);
    base.OnSpawn();
  }

  protected override void OnCleanUp()
  {
    Game.Instance.Unsubscribe(-1298331547, new Action<object>(this.OnClusterLocationChanged));
    Game.Instance.Unsubscribe(-1991583975, new Action<object>(this.OnFogOfWarRevealed));
    base.OnCleanUp();
  }

  public void OnClusterLocationChanged(object data)
  {
    if (this.m_worldContainer.IsDiscovered || !ClusterGrid.Instance.IsCellVisible(this.Location))
      return;
    Clustercraft component = ((Component) ((ClusterLocationChangedEvent) data).entity).GetComponent<Clustercraft>();
    if (Object.op_Equality((Object) component, (Object) null) || !Object.op_Equality((Object) component.GetOrbitAsteroid(), (Object) this))
      return;
    this.m_worldContainer.SetDiscovered(true);
  }

  public void OnFogOfWarRevealed(object data = null)
  {
    if (data == null || AxialI.op_Inequality((AxialI) data, this.m_location) || !ClusterGrid.Instance.IsCellVisible(this.Location))
      return;
    WorldDetectedMessage worldDetectedMessage = new WorldDetectedMessage(this.m_worldContainer);
    MusicManager.instance.PlaySong("Stinger_WorldDetected");
    Messenger.Instance.QueueMessage((Message) worldDetectedMessage);
    if (this.m_worldContainer.IsDiscovered)
      return;
    foreach (Clustercraft clustercraft in Components.Clustercrafts)
    {
      if (Object.op_Equality((Object) clustercraft.GetOrbitAsteroid(), (Object) this))
      {
        this.m_worldContainer.SetDiscovered(true);
        break;
      }
    }
  }
}
