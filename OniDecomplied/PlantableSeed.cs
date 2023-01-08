// Decompiled with JetBrains decompiler
// Type: PlantableSeed
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/PlantableSeed")]
public class PlantableSeed : KMonoBehaviour, IReceptacleDirection, IGameObjectEffectDescriptor
{
  public Tag PlantID;
  public Tag PreviewID;
  [Serialize]
  public float timeUntilSelfPlant;
  public Tag replantGroundTag;
  public string domesticatedDescription;
  public SingleEntityReceptacle.ReceptacleDirection direction;
  [MyCmpGet]
  private Pickupable pickupable;

  public SingleEntityReceptacle.ReceptacleDirection Direction => this.direction;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.timeUntilSelfPlant = Util.RandomVariance(2400f, 600f);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    Components.PlantableSeeds.Add(this);
  }

  protected virtual void OnCleanUp()
  {
    Components.PlantableSeeds.Remove(this);
    base.OnCleanUp();
  }

  public void TryPlant(bool allow_plant_from_storage = false)
  {
    this.timeUntilSelfPlant = Util.RandomVariance(2400f, 600f);
    if (!allow_plant_from_storage && ((Component) this).gameObject.HasTag(GameTags.Stored))
      return;
    int cell = Grid.PosToCell(((Component) this).gameObject);
    if (!this.TestSuitableGround(cell))
      return;
    Vector3 posCbc = Grid.CellToPosCBC(cell, Grid.SceneLayer.BuildingFront);
    GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(this.PlantID), posCbc, Grid.SceneLayer.BuildingFront);
    MutantPlant component = gameObject.GetComponent<MutantPlant>();
    if (Object.op_Inequality((Object) component, (Object) null))
      ((Component) this).GetComponent<MutantPlant>().CopyMutationsTo(component);
    gameObject.SetActive(true);
    Pickupable pickupable = this.pickupable.Take(1f);
    if (Object.op_Inequality((Object) pickupable, (Object) null))
    {
      Object.op_Inequality((Object) gameObject.GetComponent<Crop>(), (Object) null);
      Util.KDestroyGameObject(((Component) pickupable).gameObject);
    }
    else
      KCrashReporter.Assert(false, "Seed has fractional total amount < 1f");
  }

  public bool TestSuitableGround(int cell)
  {
    if (!Grid.IsValidCell(cell))
      return false;
    int index = this.Direction != SingleEntityReceptacle.ReceptacleDirection.Bottom ? Grid.CellBelow(cell) : Grid.CellAbove(cell);
    if (!Grid.IsValidCell(index) || Grid.Foundation[index] || Grid.Element[index].hardness >= (byte) 150 || ((Tag) ref this.replantGroundTag).IsValid && !Grid.Element[index].HasTag(this.replantGroundTag))
      return false;
    GameObject prefab = Assets.GetPrefab(this.PlantID);
    EntombVulnerable component1 = prefab.GetComponent<EntombVulnerable>();
    if (Object.op_Inequality((Object) component1, (Object) null) && !component1.IsCellSafe(cell))
      return false;
    DrowningMonitor component2 = prefab.GetComponent<DrowningMonitor>();
    if (Object.op_Inequality((Object) component2, (Object) null) && !component2.IsCellSafe(cell))
      return false;
    TemperatureVulnerable component3 = prefab.GetComponent<TemperatureVulnerable>();
    if (Object.op_Inequality((Object) component3, (Object) null) && !component3.IsCellSafe(cell))
      return false;
    UprootedMonitor component4 = prefab.GetComponent<UprootedMonitor>();
    if (Object.op_Inequality((Object) component4, (Object) null) && !component4.IsSuitableFoundation(cell))
      return false;
    OccupyArea component5 = prefab.GetComponent<OccupyArea>();
    return !Object.op_Inequality((Object) component5, (Object) null) || component5.CanOccupyArea(cell, ObjectLayer.Building);
  }

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    if (this.direction == SingleEntityReceptacle.ReceptacleDirection.Bottom)
    {
      Descriptor descriptor;
      // ISSUE: explicit constructor call
      ((Descriptor) ref descriptor).\u002Ector((string) UI.GAMEOBJECTEFFECTS.SEED_REQUIREMENT_CEILING, (string) UI.GAMEOBJECTEFFECTS.TOOLTIPS.SEED_REQUIREMENT_CEILING, (Descriptor.DescriptorType) 0, false);
      descriptors.Add(descriptor);
    }
    else if (this.direction == SingleEntityReceptacle.ReceptacleDirection.Side)
    {
      Descriptor descriptor;
      // ISSUE: explicit constructor call
      ((Descriptor) ref descriptor).\u002Ector((string) UI.GAMEOBJECTEFFECTS.SEED_REQUIREMENT_WALL, (string) UI.GAMEOBJECTEFFECTS.TOOLTIPS.SEED_REQUIREMENT_WALL, (Descriptor.DescriptorType) 0, false);
      descriptors.Add(descriptor);
    }
    return descriptors;
  }
}
