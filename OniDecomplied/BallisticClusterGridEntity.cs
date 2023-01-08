// Decompiled with JetBrains decompiler
// Type: BallisticClusterGridEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class BallisticClusterGridEntity : ClusterGridEntity
{
  [MyCmpReq]
  private ClusterDestinationSelector m_destionationSelector;
  [MyCmpReq]
  private ClusterTraveler m_clusterTraveler;
  [SerializeField]
  public string clusterAnimName;
  [SerializeField]
  public StringKey nameKey;
  private string clusterAnimSymbolSwapTarget;
  private string clusterAnimSymbolSwapSymbol;

  public override string Name => StringEntry.op_Implicit(Strings.Get(this.nameKey));

  public override EntityLayer Layer => EntityLayer.Payload;

  public override List<ClusterGridEntity.AnimConfig> AnimConfigs => new List<ClusterGridEntity.AnimConfig>()
  {
    new ClusterGridEntity.AnimConfig()
    {
      animFile = Assets.GetAnim(HashedString.op_Implicit(this.clusterAnimName)),
      initialAnim = "idle_loop",
      symbolSwapTarget = this.clusterAnimSymbolSwapTarget,
      symbolSwapSymbol = this.clusterAnimSymbolSwapSymbol
    }
  };

  public override bool IsVisible => !((Component) this).gameObject.HasTag(GameTags.ClusterEntityGrounded);

  public override ClusterRevealLevel IsVisibleInFOW => ClusterRevealLevel.Visible;

  public override bool SpaceOutInSameHex() => true;

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.m_clusterTraveler.getSpeedCB = new Func<float>(this.GetSpeed);
    this.m_clusterTraveler.getCanTravelCB = new Func<bool, bool>(this.CanTravel);
    this.m_clusterTraveler.onTravelCB = (System.Action) null;
  }

  private float GetSpeed() => 10f;

  private bool CanTravel(bool tryingToLand) => ((Component) this).HasTag(GameTags.EntityInSpace);

  public void Configure(AxialI source, AxialI destination)
  {
    this.m_location = source;
    this.m_destionationSelector.SetDestination(destination);
  }

  public override bool ShowPath() => this.m_selectable.IsSelected;

  public override bool ShowProgressBar() => this.m_selectable.IsSelected && this.m_clusterTraveler.IsTraveling();

  public override float GetProgress() => this.m_clusterTraveler.GetMoveProgress();

  public void SwapSymbolFromSameAnim(string targetSymbolName, string swappedSymbolName)
  {
    this.clusterAnimSymbolSwapTarget = targetSymbolName;
    this.clusterAnimSymbolSwapSymbol = swappedSymbolName;
  }
}
