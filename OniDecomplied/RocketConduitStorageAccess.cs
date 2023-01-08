// Decompiled with JetBrains decompiler
// Type: RocketConduitStorageAccess
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class RocketConduitStorageAccess : KMonoBehaviour, ISim200ms
{
  [SerializeField]
  public Storage storage;
  [SerializeField]
  public float targetLevel;
  [SerializeField]
  public CargoBay.CargoType cargoType;
  [MyCmpGet]
  private Filterable filterable;
  [MyCmpGet]
  private Operational operational;
  private const float TOLERANCE = 0.01f;
  private CraftModuleInterface craftModuleInterface;

  protected virtual void OnSpawn() => this.craftModuleInterface = ((Component) this.GetMyWorld()).GetComponent<CraftModuleInterface>();

  public void Sim200ms(float dt)
  {
    if (Object.op_Inequality((Object) this.operational, (Object) null) && !this.operational.IsOperational)
      return;
    float num1 = this.storage.MassStored();
    if ((double) num1 >= (double) this.targetLevel - 0.0099999997764825821 && (double) num1 <= (double) this.targetLevel + 0.0099999997764825821)
      return;
    if (Object.op_Inequality((Object) this.operational, (Object) null))
      this.operational.SetActive(true);
    float amount = this.targetLevel - num1;
    foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>) this.craftModuleInterface.ClusterModules)
    {
      CargoBayCluster component = ((Component) clusterModule.Get()).GetComponent<CargoBayCluster>();
      if (Object.op_Inequality((Object) component, (Object) null) && component.storageType == this.cargoType)
      {
        if ((double) amount > 0.0 && (double) component.storage.MassStored() > 0.0)
        {
          for (int index = component.storage.items.Count - 1; index >= 0; --index)
          {
            GameObject go = component.storage.items[index];
            if (!Object.op_Inequality((Object) this.filterable, (Object) null) || !Tag.op_Inequality(this.filterable.SelectedTag, GameTags.Void) || !Tag.op_Inequality(go.PrefabID(), this.filterable.SelectedTag))
            {
              Pickupable pickupable = go.GetComponent<Pickupable>().Take(amount);
              if (Object.op_Inequality((Object) pickupable, (Object) null))
              {
                amount -= pickupable.PrimaryElement.Mass;
                this.storage.Store(((Component) pickupable).gameObject, true);
              }
              if ((double) amount <= 0.0)
                break;
            }
          }
          if ((double) amount <= 0.0)
            break;
        }
        if ((double) amount < 0.0 && (double) component.storage.RemainingCapacity() > 0.0)
        {
          double num2 = (double) Mathf.Min(-amount, component.storage.RemainingCapacity());
          for (int index = this.storage.items.Count - 1; index >= 0; --index)
          {
            Pickupable pickupable = this.storage.items[index].GetComponent<Pickupable>().Take(-amount);
            if (Object.op_Inequality((Object) pickupable, (Object) null))
            {
              amount += pickupable.PrimaryElement.Mass;
              component.storage.Store(((Component) pickupable).gameObject, true);
            }
            if ((double) amount >= 0.0)
              break;
          }
          if ((double) amount >= 0.0)
            break;
        }
      }
    }
  }
}
