// Decompiled with JetBrains decompiler
// Type: ConduitElementSensor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using UnityEngine;

[SerializationConfig]
public class ConduitElementSensor : ConduitSensor
{
  [MyCmpGet]
  private Filterable filterable;

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.filterable.onFilterChanged += new Action<Tag>(this.OnFilterChanged);
    this.OnFilterChanged(this.filterable.SelectedTag);
  }

  private void OnFilterChanged(Tag tag)
  {
    if (!((Tag) ref tag).IsValid)
      return;
    bool on = Tag.op_Equality(tag, GameTags.Void);
    ((Component) this).GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.NoFilterElementSelected, on);
  }

  protected override void ConduitUpdate(float dt)
  {
    Tag element;
    bool hasMass;
    this.GetContentsElement(out element, out hasMass);
    if (!this.IsSwitchedOn)
    {
      if (!(Tag.op_Equality(element, this.filterable.SelectedTag) & hasMass))
        return;
      this.Toggle();
    }
    else
    {
      if (!Tag.op_Inequality(element, this.filterable.SelectedTag) && hasMass)
        return;
      this.Toggle();
    }
  }

  private void GetContentsElement(out Tag element, out bool hasMass)
  {
    int cell = Grid.PosToCell((KMonoBehaviour) this);
    if (this.conduitType == ConduitType.Liquid || this.conduitType == ConduitType.Gas)
    {
      ConduitFlow.ConduitContents contents = Conduit.GetFlowManager(this.conduitType).GetContents(cell);
      element = contents.element.CreateTag();
      hasMass = (double) contents.mass > 0.0;
    }
    else
    {
      SolidConduitFlow flowManager = SolidConduit.GetFlowManager();
      Pickupable pickupable = flowManager.GetPickupable(flowManager.GetContents(cell).pickupableHandle);
      KPrefabID kprefabId = Object.op_Inequality((Object) pickupable, (Object) null) ? ((Component) pickupable).GetComponent<KPrefabID>() : (KPrefabID) null;
      if (Object.op_Inequality((Object) kprefabId, (Object) null) && (double) pickupable.PrimaryElement.Mass > 0.0)
      {
        element = kprefabId.PrefabTag;
        hasMass = true;
      }
      else
      {
        element = GameTags.Void;
        hasMass = false;
      }
    }
  }
}
