// Decompiled with JetBrains decompiler
// Type: ResearchDestination
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System.Collections.Generic;

[SerializationConfig]
public class ResearchDestination : ClusterGridEntity
{
  public override string Name => (string) UI.SPACEDESTINATIONS.RESEARCHDESTINATION.NAME;

  public override EntityLayer Layer => EntityLayer.POI;

  public override List<ClusterGridEntity.AnimConfig> AnimConfigs => new List<ClusterGridEntity.AnimConfig>();

  public override bool IsVisible => false;

  public override ClusterRevealLevel IsVisibleInFOW => ClusterRevealLevel.Peeked;

  public void Init(AxialI location) => this.m_location = location;
}
