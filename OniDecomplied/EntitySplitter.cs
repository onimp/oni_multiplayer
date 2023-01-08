// Decompiled with JetBrains decompiler
// Type: EntitySplitter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using System;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/EntitySplitter")]
public class EntitySplitter : KMonoBehaviour
{
  public float maxStackSize = PrimaryElement.MAX_MASS;
  private static readonly EventSystem.IntraObjectHandler<EntitySplitter> OnAbsorbDelegate = new EventSystem.IntraObjectHandler<EntitySplitter>((Action<EntitySplitter, object>) ((component, data) => component.OnAbsorb(data)));

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    Pickupable pickupable = ((Component) this).GetComponent<Pickupable>();
    if (Object.op_Equality((Object) pickupable, (Object) null))
      Debug.LogError((object) (((Object) this).name + " does not have a pickupable component!"));
    pickupable.OnTake += (Func<float, Pickupable>) (amount => EntitySplitter.Split(pickupable, amount));
    Rottable.Instance rottable = ((Component) this).gameObject.GetSMI<Rottable.Instance>();
    pickupable.absorbable = true;
    pickupable.CanAbsorb = (Func<Pickupable, bool>) (other => EntitySplitter.CanFirstAbsorbSecond(pickupable, rottable, other, this.maxStackSize));
    this.Subscribe<EntitySplitter>(-2064133523, EntitySplitter.OnAbsorbDelegate);
  }

  private static bool CanFirstAbsorbSecond(
    Pickupable pickupable,
    Rottable.Instance rottable,
    Pickupable other,
    float maxStackSize)
  {
    if (Object.op_Equality((Object) other, (Object) null))
      return false;
    KPrefabID component1 = ((Component) pickupable).GetComponent<KPrefabID>();
    KPrefabID component2 = ((Component) other).GetComponent<KPrefabID>();
    if (Object.op_Equality((Object) component1, (Object) null) || Object.op_Equality((Object) component2, (Object) null) || Tag.op_Inequality(component1.PrefabTag, component2.PrefabTag) || (double) pickupable.TotalAmount + (double) other.TotalAmount > (double) maxStackSize || (double) pickupable.PrimaryElement.Mass + (double) other.PrimaryElement.Mass > (double) maxStackSize)
      return false;
    if (rottable != null)
    {
      Rottable.Instance smi = ((Component) other).GetSMI<Rottable.Instance>();
      if (smi == null || !rottable.IsRotLevelStackable(smi))
        return false;
    }
    bool flag = component1.HasTag(GameTags.SpicedFood);
    if (flag != component2.HasTag(GameTags.SpicedFood))
      return false;
    Edible component3 = ((Component) component1).GetComponent<Edible>();
    Edible component4 = ((Component) component1).GetComponent<Edible>();
    if (flag && !component3.CanAbsorb(component4))
      return false;
    if (component1.HasTag(GameTags.Seed) || component1.HasTag(GameTags.CropSeed) || component1.HasTag(GameTags.Compostable))
    {
      MutantPlant component5 = ((Component) pickupable).GetComponent<MutantPlant>();
      MutantPlant component6 = ((Component) other).GetComponent<MutantPlant>();
      if ((Object.op_Inequality((Object) component5, (Object) null) || Object.op_Inequality((Object) component6, (Object) null)) && (Object.op_Equality((Object) component5, (Object) null) != Object.op_Equality((Object) component6, (Object) null) || component1.HasTag(GameTags.UnidentifiedSeed) != component2.HasTag(GameTags.UnidentifiedSeed) || Tag.op_Inequality(component5.SubSpeciesID, component6.SubSpeciesID)))
        return false;
    }
    return true;
  }

  public static Pickupable Split(Pickupable pickupable, float amount, GameObject prefab = null)
  {
    if ((double) amount >= (double) pickupable.TotalAmount && Object.op_Equality((Object) prefab, (Object) null))
      return pickupable;
    Storage storage = pickupable.storage;
    if (Object.op_Equality((Object) prefab, (Object) null))
      prefab = Assets.GetPrefab(((Component) pickupable).GetComponent<KPrefabID>().PrefabTag);
    GameObject parent = (GameObject) null;
    if (Object.op_Inequality((Object) pickupable.transform.parent, (Object) null))
      parent = ((Component) pickupable.transform.parent).gameObject;
    GameObject gameObject = GameUtil.KInstantiate(prefab, TransformExtensions.GetPosition(pickupable.transform), Grid.SceneLayer.Ore, parent);
    Debug.Assert(Object.op_Inequality((Object) gameObject, (Object) null), (object) "WTH, the GO is null, shouldn't happen on instantiate");
    Pickupable component = gameObject.GetComponent<Pickupable>();
    if (Object.op_Equality((Object) component, (Object) null))
      Debug.LogError((object) ("Edible::OnTake() No Pickupable component for " + ((Object) gameObject).name), (Object) gameObject);
    gameObject.SetActive(true);
    component.TotalAmount = Mathf.Min(amount, pickupable.TotalAmount);
    component.PrimaryElement.Temperature = pickupable.PrimaryElement.Temperature;
    bool keepZeroMassObject = pickupable.PrimaryElement.KeepZeroMassObject;
    pickupable.PrimaryElement.KeepZeroMassObject = true;
    pickupable.TotalAmount -= amount;
    component.Trigger(1335436905, (object) pickupable);
    pickupable.PrimaryElement.KeepZeroMassObject = keepZeroMassObject;
    pickupable.TotalAmount += 0.0f;
    if (Object.op_Inequality((Object) storage, (Object) null))
    {
      storage.Trigger(-1697596308, (object) ((Component) pickupable).gameObject);
      storage.Trigger(-778359855, (object) storage);
    }
    IExtendSplitting[] components = ((Component) pickupable).GetComponents<IExtendSplitting>();
    if (components != null)
    {
      for (int index = 0; index < components.Length; ++index)
        components[index].OnSplitTick(component);
    }
    return component;
  }

  private void OnAbsorb(object data)
  {
    Pickupable pickupable = (Pickupable) data;
    if (!Object.op_Inequality((Object) pickupable, (Object) null))
      return;
    PrimaryElement component = ((Component) this).GetComponent<PrimaryElement>();
    PrimaryElement primaryElement = pickupable.PrimaryElement;
    if (!Object.op_Inequality((Object) primaryElement, (Object) null))
      return;
    float temperature = 0.0f;
    float mass1 = component.Mass;
    float mass2 = primaryElement.Mass;
    if ((double) mass1 > 0.0 && (double) mass2 > 0.0)
      temperature = SimUtil.CalculateFinalTemperature(mass1, component.Temperature, mass2, primaryElement.Temperature);
    else if ((double) primaryElement.Mass > 0.0)
      temperature = primaryElement.Temperature;
    component.SetMassTemperature(mass1 + mass2, temperature);
    if (!Object.op_Inequality((Object) CameraController.Instance, (Object) null))
      return;
    string sound = GlobalAssets.GetSound("Ore_absorb");
    Vector3 position = TransformExtensions.GetPosition(pickupable.transform);
    position.z = 0.0f;
    if (sound == null || !CameraController.Instance.IsAudibleSound(position, HashedString.op_Implicit(sound)))
      return;
    KFMOD.PlayOneShot(sound, position, 1f);
  }
}
