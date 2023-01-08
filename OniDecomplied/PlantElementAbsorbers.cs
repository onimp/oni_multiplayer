// Decompiled with JetBrains decompiler
// Type: PlantElementAbsorbers
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class PlantElementAbsorbers : KCompactedVector<PlantElementAbsorber>
{
  private bool updating;
  private List<HandleVector<int>.Handle> queuedRemoves = new List<HandleVector<int>.Handle>();

  public HandleVector<int>.Handle Add(
    Storage storage,
    PlantElementAbsorber.ConsumeInfo[] consumed_elements)
  {
    if (consumed_elements == null || consumed_elements.Length == 0)
      return HandleVector<int>.InvalidHandle;
    HandleVector<int>.Handle[] handleArray = new HandleVector<int>.Handle[consumed_elements.Length];
    for (int index = 0; index < consumed_elements.Length; ++index)
      handleArray[index] = Game.Instance.accumulators.Add("ElementsConsumed", (KMonoBehaviour) storage);
    HandleVector<int>.Handle invalidHandle = HandleVector<int>.InvalidHandle;
    HandleVector<int>.Handle handle;
    if (consumed_elements.Length == 1)
      handle = this.Allocate(new PlantElementAbsorber()
      {
        storage = storage,
        consumedElements = (PlantElementAbsorber.ConsumeInfo[]) null,
        accumulators = handleArray,
        localInfo = new PlantElementAbsorber.LocalInfo()
        {
          tag = consumed_elements[0].tag,
          massConsumptionRate = consumed_elements[0].massConsumptionRate
        }
      });
    else
      handle = this.Allocate(new PlantElementAbsorber()
      {
        storage = storage,
        consumedElements = consumed_elements,
        accumulators = handleArray,
        localInfo = new PlantElementAbsorber.LocalInfo()
        {
          tag = Tag.Invalid,
          massConsumptionRate = 0.0f
        }
      });
    return handle;
  }

  public HandleVector<int>.Handle Remove(HandleVector<int>.Handle h)
  {
    if (this.updating)
      this.queuedRemoves.Add(h);
    else
      this.Free(h);
    return HandleVector<int>.InvalidHandle;
  }

  public void Sim200ms(float dt)
  {
    int count = this.data.Count;
    this.updating = true;
    for (int index1 = 0; index1 < count; ++index1)
    {
      PlantElementAbsorber plantElementAbsorber = this.data[index1];
      if (!Object.op_Equality((Object) plantElementAbsorber.storage, (Object) null))
      {
        if (plantElementAbsorber.consumedElements == null)
        {
          float num1 = plantElementAbsorber.localInfo.massConsumptionRate * dt;
          PrimaryElement firstWithMass = plantElementAbsorber.storage.FindFirstWithMass(plantElementAbsorber.localInfo.tag);
          if (Object.op_Inequality((Object) firstWithMass, (Object) null))
          {
            float amount = Mathf.Min(num1, firstWithMass.Mass);
            firstWithMass.Mass -= amount;
            float num2 = num1 - amount;
            Game.Instance.accumulators.Accumulate(plantElementAbsorber.accumulators[0], amount);
            plantElementAbsorber.storage.Trigger(-1697596308, (object) ((Component) firstWithMass).gameObject);
          }
        }
        else
        {
          for (int index2 = 0; index2 < plantElementAbsorber.consumedElements.Length; ++index2)
          {
            float num = plantElementAbsorber.consumedElements[index2].massConsumptionRate * dt;
            for (PrimaryElement firstWithMass = plantElementAbsorber.storage.FindFirstWithMass(plantElementAbsorber.consumedElements[index2].tag); Object.op_Inequality((Object) firstWithMass, (Object) null); firstWithMass = plantElementAbsorber.storage.FindFirstWithMass(plantElementAbsorber.consumedElements[index2].tag))
            {
              float amount = Mathf.Min(num, firstWithMass.Mass);
              firstWithMass.Mass -= amount;
              num -= amount;
              Game.Instance.accumulators.Accumulate(plantElementAbsorber.accumulators[index2], amount);
              plantElementAbsorber.storage.Trigger(-1697596308, (object) ((Component) firstWithMass).gameObject);
              if ((double) num <= 0.0)
                break;
            }
          }
        }
        this.data[index1] = plantElementAbsorber;
      }
    }
    this.updating = false;
    for (int index = 0; index < this.queuedRemoves.Count; ++index)
      this.Remove(this.queuedRemoves[index]);
    this.queuedRemoves.Clear();
  }

  public virtual void Clear()
  {
    base.Clear();
    for (int index = 0; index < this.data.Count; ++index)
      this.data[index].Clear();
    this.data.Clear();
    ((KCompactedVectorBase) this).handles.Clear();
  }

  public PlantElementAbsorbers()
    : base(0)
  {
  }
}
