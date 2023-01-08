// Decompiled with JetBrains decompiler
// Type: ArtifactPOIConfigurator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ArtifactPOIConfigurator")]
public class ArtifactPOIConfigurator : KMonoBehaviour
{
  private static List<ArtifactPOIConfigurator.ArtifactPOIType> _poiTypes;
  public static ArtifactPOIConfigurator.ArtifactPOIType defaultArtifactPoiType = new ArtifactPOIConfigurator.ArtifactPOIType("HarvestablePOIArtifacts");
  public HashedString presetType;
  public float presetMin;
  public float presetMax = 1f;

  public static ArtifactPOIConfigurator.ArtifactPOIType FindType(HashedString typeId)
  {
    ArtifactPOIConfigurator.ArtifactPOIType type = (ArtifactPOIConfigurator.ArtifactPOIType) null;
    if (HashedString.op_Inequality(typeId, HashedString.Invalid))
      type = ArtifactPOIConfigurator._poiTypes.Find((Predicate<ArtifactPOIConfigurator.ArtifactPOIType>) (t => HashedString.op_Equality(HashedString.op_Implicit(t.id), typeId)));
    if (type == null)
      Debug.LogError((object) string.Format("Tried finding a harvestable poi with id {0} but it doesn't exist!", (object) typeId.ToString()));
    return type;
  }

  public ArtifactPOIConfigurator.ArtifactPOIInstanceConfiguration MakeConfiguration() => this.CreateRandomInstance(this.presetType, this.presetMin, this.presetMax);

  private ArtifactPOIConfigurator.ArtifactPOIInstanceConfiguration CreateRandomInstance(
    HashedString typeId,
    float min,
    float max)
  {
    int globalWorldSeed = SaveLoader.Instance.clusterDetailSave.globalWorldSeed;
    ClusterGridEntity component = ((Component) this).GetComponent<ClusterGridEntity>();
    Vector3 position = ClusterGrid.Instance.GetPosition(component);
    int x = (int) position.x;
    KRandom randomSource = new KRandom(globalWorldSeed + x + (int) position.y);
    return new ArtifactPOIConfigurator.ArtifactPOIInstanceConfiguration()
    {
      typeId = typeId,
      rechargeRoll = this.Roll(randomSource, min, max)
    };
  }

  private float Roll(KRandom randomSource, float min, float max) => (float) (randomSource.NextDouble() * ((double) max - (double) min)) + min;

  public class ArtifactPOIType
  {
    public string id;
    public HashedString idHash;
    public string harvestableArtifactID;
    public bool destroyOnHarvest;
    public float poiRechargeTimeMin;
    public float poiRechargeTimeMax;
    public string dlcID;
    public List<string> orbitalObject = new List<string>()
    {
      Db.Get().OrbitalTypeCategories.gravitas.Id
    };

    public ArtifactPOIType(
      string id,
      string harvestableArtifactID = null,
      bool destroyOnHarvest = false,
      float poiRechargeTimeMin = 30000f,
      float poiRechargeTimeMax = 60000f,
      string dlcID = "EXPANSION1_ID")
    {
      this.id = id;
      this.idHash = HashedString.op_Implicit(id);
      this.harvestableArtifactID = harvestableArtifactID;
      this.destroyOnHarvest = destroyOnHarvest;
      this.poiRechargeTimeMin = poiRechargeTimeMin;
      this.poiRechargeTimeMax = poiRechargeTimeMax;
      this.dlcID = dlcID;
      if (ArtifactPOIConfigurator._poiTypes == null)
        ArtifactPOIConfigurator._poiTypes = new List<ArtifactPOIConfigurator.ArtifactPOIType>();
      ArtifactPOIConfigurator._poiTypes.Add(this);
    }
  }

  [Serializable]
  public class ArtifactPOIInstanceConfiguration
  {
    public HashedString typeId;
    private bool didInit;
    public float rechargeRoll;
    private float poiRechargeTime;

    private void Init()
    {
      if (this.didInit)
        return;
      this.didInit = true;
      this.poiRechargeTime = MathUtil.ReRange(this.rechargeRoll, 0.0f, 1f, this.poiType.poiRechargeTimeMin, this.poiType.poiRechargeTimeMax);
    }

    public ArtifactPOIConfigurator.ArtifactPOIType poiType => ArtifactPOIConfigurator.FindType(this.typeId);

    public bool DestroyOnHarvest()
    {
      this.Init();
      return this.poiType.destroyOnHarvest;
    }

    public string GetArtifactID()
    {
      this.Init();
      return this.poiType.harvestableArtifactID;
    }

    public float GetRechargeTime()
    {
      this.Init();
      return this.poiRechargeTime;
    }
  }
}
