// Decompiled with JetBrains decompiler
// Type: ElementSplitterComponents
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class ElementSplitterComponents : KGameObjectComponentManager<ElementSplitter>
{
  private const float MAX_STACK_SIZE = 25000f;

  public HandleVector<int>.Handle Add(GameObject go) => this.Add(go, new ElementSplitter(go));

  protected virtual void OnPrefabInit(HandleVector<int>.Handle handle)
  {
    ElementSplitter data = ((KCompactedVector<ElementSplitter>) this).GetData(handle);
    Pickupable component = ((Component) data.primaryElement).GetComponent<Pickupable>();
    Func<float, Pickupable> func1 = (Func<float, Pickupable>) (amount => ElementSplitterComponents.OnTake(handle, amount));
    component.OnTake += func1;
    Func<Pickupable, bool> func2 = (Func<Pickupable, bool>) (other => ElementSplitterComponents.CanFirstAbsorbSecond(handle, this.GetHandle(((Component) other).gameObject)));
    component.CanAbsorb += func2;
    component.absorbable = true;
    data.onTakeCB = func1;
    data.canAbsorbCB = func2;
    ((KCompactedVector<ElementSplitter>) this).SetData(handle, data);
  }

  protected virtual void OnSpawn(HandleVector<int>.Handle handle)
  {
  }

  protected virtual void OnCleanUp(HandleVector<int>.Handle handle)
  {
    ElementSplitter data = ((KCompactedVector<ElementSplitter>) this).GetData(handle);
    if (!Object.op_Inequality((Object) data.primaryElement, (Object) null))
      return;
    Pickupable component = ((Component) data.primaryElement).GetComponent<Pickupable>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.OnTake -= data.onTakeCB;
    component.CanAbsorb -= data.canAbsorbCB;
  }

  private static bool CanFirstAbsorbSecond(
    HandleVector<int>.Handle first,
    HandleVector<int>.Handle second)
  {
    if (HandleVector<int>.Handle.op_Equality(first, HandleVector<int>.InvalidHandle) || HandleVector<int>.Handle.op_Equality(second, HandleVector<int>.InvalidHandle))
      return false;
    ElementSplitter data1 = ((KCompactedVector<ElementSplitter>) GameComps.ElementSplitters).GetData(first);
    ElementSplitter data2 = ((KCompactedVector<ElementSplitter>) GameComps.ElementSplitters).GetData(second);
    return data1.primaryElement.ElementID == data2.primaryElement.ElementID && (double) data1.primaryElement.Units + (double) data2.primaryElement.Units < 25000.0;
  }

  private static Pickupable OnTake(HandleVector<int>.Handle handle, float amount)
  {
    ElementSplitter data = ((KCompactedVector<ElementSplitter>) GameComps.ElementSplitters).GetData(handle);
    Pickupable component1 = ((Component) data.primaryElement).GetComponent<Pickupable>();
    Storage storage = component1.storage;
    PrimaryElement component2 = ((Component) component1).GetComponent<PrimaryElement>();
    Pickupable component3 = component2.Element.substance.SpawnResource(TransformExtensions.GetPosition(component1.transform), amount, component2.Temperature, byte.MaxValue, 0, true).GetComponent<Pickupable>();
    component1.TotalAmount -= amount;
    component3.Trigger(1335436905, (object) component1);
    ElementSplitterComponents.CopyRenderSettings(((Component) component1).GetComponent<KBatchedAnimController>(), ((Component) component3).GetComponent<KBatchedAnimController>());
    if (Object.op_Inequality((Object) storage, (Object) null))
    {
      storage.Trigger(-1697596308, (object) ((Component) data.primaryElement).gameObject);
      storage.Trigger(-778359855, (object) storage);
    }
    return component3;
  }

  private static void CopyRenderSettings(KBatchedAnimController src, KBatchedAnimController dest)
  {
    if (!Object.op_Inequality((Object) src, (Object) null) || !Object.op_Inequality((Object) dest, (Object) null))
      return;
    dest.OverlayColour = src.OverlayColour;
  }
}
