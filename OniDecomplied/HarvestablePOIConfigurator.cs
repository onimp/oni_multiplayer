// Decompiled with JetBrains decompiler
// Type: HarvestablePOIConfigurator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/HarvestablePOIConfigurator")]
public class HarvestablePOIConfigurator : KMonoBehaviour
{
  private static List<HarvestablePOIConfigurator.HarvestablePOIType> _poiTypes;
  public HashedString presetType;
  public float presetMin;
  public float presetMax = 1f;

  public static HarvestablePOIConfigurator.HarvestablePOIType FindType(HashedString typeId)
  {
    HarvestablePOIConfigurator.HarvestablePOIType type = (HarvestablePOIConfigurator.HarvestablePOIType) null;
    if (HashedString.op_Inequality(typeId, HashedString.Invalid))
      type = HarvestablePOIConfigurator._poiTypes.Find((Predicate<HarvestablePOIConfigurator.HarvestablePOIType>) (t => HashedString.op_Equality(HashedString.op_Implicit(t.id), typeId)));
    if (type == null)
      Debug.LogError((object) string.Format("Tried finding a harvestable poi with id {0} but it doesn't exist!", (object) typeId.ToString()));
    return type;
  }

  public HarvestablePOIConfigurator.HarvestablePOIInstanceConfiguration MakeConfiguration() => this.CreateRandomInstance(this.presetType, this.presetMin, this.presetMax);

  private HarvestablePOIConfigurator.HarvestablePOIInstanceConfiguration CreateRandomInstance(
    HashedString typeId,
    float min,
    float max)
  {
    int globalWorldSeed = SaveLoader.Instance.clusterDetailSave.globalWorldSeed;
    ClusterGridEntity component = ((Component) this).GetComponent<ClusterGridEntity>();
    Vector3 position = ClusterGrid.Instance.GetPosition(component);
    int x = (int) position.x;
    KRandom randomSource = new KRandom(globalWorldSeed + x + (int) position.y);
    return new HarvestablePOIConfigurator.HarvestablePOIInstanceConfiguration()
    {
      typeId = typeId,
      capacityRoll = this.Roll(randomSource, min, max),
      rechargeRoll = this.Roll(randomSource, min, max)
    };
  }

  private float Roll(KRandom randomSource, float min, float max) => (float) (randomSource.NextDouble() * ((double) max - (double) min)) + min;

  public class HarvestablePOIType
  {
    public string id;
    public HashedString idHash;
    public Dictionary<SimHashes, float> harvestableElements;
    public float poiCapacityMin;
    public float poiCapacityMax;
    public float poiRechargeMin;
    public float poiRechargeMax;
    public bool canProvideArtifacts;
    public string dlcID;
    public List<string> orbitalObject;
    public int maxNumOrbitingObjects;

    public HarvestablePOIType(
      string id,
      Dictionary<SimHashes, float> harvestableElements,
      float poiCapacityMin = 54000f,
      float poiCapacityMax = 81000f,
      float poiRechargeMin = 30000f,
      float poiRechargeMax = 60000f,
      bool canProvideArtifacts = true,
      List<string> orbitalObject = null,
      int maxNumOrbitingObjects = 20,
      string dlcID = "EXPANSION1_ID")
    {
      this.id = id;
      this.idHash = HashedString.op_Implicit(id);
      this.harvestableElements = harvestableElements;
      this.poiCapacityMin = poiCapacityMin;
      this.poiCapacityMax = poiCapacityMax;
      this.poiRechargeMin = poiRechargeMin;
      this.poiRechargeMax = poiRechargeMax;
      this.canProvideArtifacts = canProvideArtifacts;
      this.orbitalObject = orbitalObject;
      this.maxNumOrbitingObjects = maxNumOrbitingObjects;
      this.dlcID = dlcID;
      if (HarvestablePOIConfigurator._poiTypes == null)
        HarvestablePOIConfigurator._poiTypes = new List<HarvestablePOIConfigurator.HarvestablePOIType>();
      HarvestablePOIConfigurator._poiTypes.Add(this);
    }
  }

  [Serializable]
  public class HarvestablePOIInstanceConfiguration
  {
    public HashedString typeId;
    private bool didInit;
    public float capacityRoll;
    public float rechargeRoll;
    private float poiTotalCapacity;
    private float poiRecharge;

    private void Init()
    {
      if (this.didInit)
        return;
      this.didInit = true;
      this.poiTotalCapacity = MathUtil.ReRange(this.capacityRoll, 0.0f, 1f, this.poiType.poiCapacityMin, this.poiType.poiCapacityMax);
      this.poiRecharge = MathUtil.ReRange(this.rechargeRoll, 0.0f, 1f, this.poiType.poiRechargeMin, this.poiType.poiRechargeMax);
    }

    public HarvestablePOIConfigurator.HarvestablePOIType poiType => HarvestablePOIConfigurator.FindType(this.typeId);

    public Dictionary<SimHashes, float> GetElementsWithWeights()
    {
      this.Init();
      return this.poiType.harvestableElements;
    }

    public bool CanProvideArtifacts()
    {
      this.Init();
      return this.poiType.canProvideArtifacts;
    }

    public float GetMaxCapacity()
    {
      this.Init();
      return this.poiTotalCapacity;
    }

    public float GetRechargeTime()
    {
      this.Init();
      return this.poiRecharge;
    }
  }
}
