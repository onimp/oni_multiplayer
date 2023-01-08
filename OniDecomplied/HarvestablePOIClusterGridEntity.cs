// Decompiled with JetBrains decompiler
// Type: HarvestablePOIClusterGridEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System.Collections.Generic;

[SerializationConfig]
public class HarvestablePOIClusterGridEntity : ClusterGridEntity
{
  public string m_name;
  public string m_Anim;

  public override string Name => this.m_name;

  public override EntityLayer Layer => EntityLayer.POI;

  public override List<ClusterGridEntity.AnimConfig> AnimConfigs => new List<ClusterGridEntity.AnimConfig>()
  {
    new ClusterGridEntity.AnimConfig()
    {
      animFile = Assets.GetAnim(HashedString.op_Implicit("harvestable_space_poi_kanim")),
      initialAnim = Util.IsNullOrWhiteSpace(this.m_Anim) ? "cloud" : this.m_Anim
    }
  };

  public override bool IsVisible => true;

  public override ClusterRevealLevel IsVisibleInFOW => ClusterRevealLevel.Peeked;

  public void Init(AxialI location) => this.Location = location;
}
