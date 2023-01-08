// Decompiled with JetBrains decompiler
// Type: ElementChunk
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using System;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/ElementChunk")]
public class ElementChunk : KMonoBehaviour
{
  private static readonly EventSystem.IntraObjectHandler<ElementChunk> OnAbsorbDelegate = new EventSystem.IntraObjectHandler<ElementChunk>((Action<ElementChunk, object>) ((component, data) => component.OnAbsorb(data)));

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    GameComps.OreSizeVisualizers.Add(((Component) this).gameObject);
    GameComps.ElementSplitters.Add(((Component) this).gameObject);
    this.Subscribe<ElementChunk>(-2064133523, ElementChunk.OnAbsorbDelegate);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    Vector3 position = TransformExtensions.GetPosition(this.transform);
    position.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
    TransformExtensions.SetPosition(this.transform, position);
    Element element = ((Component) this).GetComponent<PrimaryElement>().Element;
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    Func<Element> data = (Func<Element>) (() => element);
    component.AddStatusItem(Db.Get().MiscStatusItems.ElementalCategory, (object) data);
    component.AddStatusItem(Db.Get().MiscStatusItems.OreMass, (object) ((Component) this).gameObject);
    component.AddStatusItem(Db.Get().MiscStatusItems.OreTemp, (object) ((Component) this).gameObject);
  }

  protected virtual void OnCleanUp()
  {
    GameComps.ElementSplitters.Remove(((Component) this).gameObject);
    GameComps.OreSizeVisualizers.Remove(((Component) this).gameObject);
    base.OnCleanUp();
  }

  private void OnAbsorb(object data)
  {
    Pickupable pickupable = (Pickupable) data;
    if (!Object.op_Inequality((Object) pickupable, (Object) null))
      return;
    PrimaryElement primaryElement = pickupable.PrimaryElement;
    if (!Object.op_Inequality((Object) primaryElement, (Object) null))
      return;
    float mass1 = primaryElement.Mass;
    if ((double) mass1 > 0.0)
    {
      PrimaryElement component = ((Component) this).GetComponent<PrimaryElement>();
      float mass2 = component.Mass;
      float temperature = (double) mass2 > 0.0 ? SimUtil.CalculateFinalTemperature(mass2, component.Temperature, mass1, primaryElement.Temperature) : primaryElement.Temperature;
      component.SetMassTemperature(mass2 + mass1, temperature);
    }
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
