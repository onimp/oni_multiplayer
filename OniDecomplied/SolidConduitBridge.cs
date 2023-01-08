// Decompiled with JetBrains decompiler
// Type: SolidConduitBridge
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SolidConduitBridge")]
public class SolidConduitBridge : ConduitBridgeBase
{
  [MyCmpGet]
  private Operational operational;
  private int inputCell;
  private int outputCell;
  private bool dispensing;

  public bool IsDispensing => this.dispensing;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    Building component = ((Component) this).GetComponent<Building>();
    this.inputCell = component.GetUtilityInputCell();
    this.outputCell = component.GetUtilityOutputCell();
    SolidConduit.GetFlowManager().AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.Default);
  }

  protected virtual void OnCleanUp()
  {
    SolidConduit.GetFlowManager().RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
    base.OnCleanUp();
  }

  private void ConduitUpdate(float dt)
  {
    this.dispensing = false;
    float mass = 0.0f;
    if (Object.op_Implicit((Object) this.operational) && !this.operational.IsOperational)
    {
      this.SendEmptyOnMassTransfer();
    }
    else
    {
      SolidConduitFlow flowManager = SolidConduit.GetFlowManager();
      if (!flowManager.HasConduit(this.inputCell) || !flowManager.HasConduit(this.outputCell))
      {
        this.SendEmptyOnMassTransfer();
      }
      else
      {
        if (flowManager.IsConduitFull(this.inputCell) && flowManager.IsConduitEmpty(this.outputCell))
        {
          Pickupable pickupable1 = flowManager.GetPickupable(flowManager.GetContents(this.inputCell).pickupableHandle);
          if (Object.op_Equality((Object) pickupable1, (Object) null))
          {
            flowManager.RemovePickupable(this.inputCell);
            this.SendEmptyOnMassTransfer();
            return;
          }
          float amount = pickupable1.PrimaryElement.Mass;
          if (this.desiredMassTransfer != null)
            amount = this.desiredMassTransfer(dt, pickupable1.PrimaryElement.Element.id, pickupable1.PrimaryElement.Mass, pickupable1.PrimaryElement.Temperature, pickupable1.PrimaryElement.DiseaseIdx, pickupable1.PrimaryElement.DiseaseCount, pickupable1);
          if ((double) amount == 0.0)
          {
            this.SendEmptyOnMassTransfer();
            return;
          }
          if ((double) amount < (double) pickupable1.PrimaryElement.Mass)
          {
            Pickupable pickupable2 = pickupable1.Take(amount);
            flowManager.AddPickupable(this.outputCell, pickupable2);
            this.dispensing = true;
            mass = pickupable2.PrimaryElement.Mass;
            if (this.OnMassTransfer != null)
              this.OnMassTransfer(pickupable2.PrimaryElement.ElementID, mass, pickupable2.PrimaryElement.Temperature, pickupable2.PrimaryElement.DiseaseIdx, pickupable2.PrimaryElement.DiseaseCount, pickupable2);
          }
          else
          {
            Pickupable pickupable3 = flowManager.RemovePickupable(this.inputCell);
            if (Object.op_Implicit((Object) pickupable3))
            {
              flowManager.AddPickupable(this.outputCell, pickupable3);
              this.dispensing = true;
              mass = pickupable3.PrimaryElement.Mass;
              if (this.OnMassTransfer != null)
                this.OnMassTransfer(pickupable3.PrimaryElement.ElementID, mass, pickupable3.PrimaryElement.Temperature, pickupable3.PrimaryElement.DiseaseIdx, pickupable3.PrimaryElement.DiseaseCount, pickupable3);
            }
          }
        }
        if ((double) mass != 0.0)
          return;
        this.SendEmptyOnMassTransfer();
      }
    }
  }
}
