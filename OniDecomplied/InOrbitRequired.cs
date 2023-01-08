// Decompiled with JetBrains decompiler
// Type: InOrbitRequired
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/InOrbitRequired")]
public class InOrbitRequired : KMonoBehaviour, IGameObjectEffectDescriptor
{
  [MyCmpReq]
  private Building building;
  [MyCmpReq]
  private Operational operational;
  public static readonly Operational.Flag inOrbitFlag = new Operational.Flag("in_orbit", Operational.Flag.Type.Requirement);
  private CraftModuleInterface craftModuleInterface;

  protected virtual void OnSpawn()
  {
    this.craftModuleInterface = ((Component) this.GetMyWorld()).GetComponent<CraftModuleInterface>();
    base.OnSpawn();
    this.UpdateFlag(((Component) this.craftModuleInterface).HasTag(GameTags.RocketNotOnGround));
    this.craftModuleInterface.Subscribe(-1582839653, new Action<object>(this.OnTagsChanged));
  }

  protected virtual void OnCleanUp()
  {
    if (!Object.op_Inequality((Object) this.craftModuleInterface, (Object) null))
      return;
    this.craftModuleInterface.Unsubscribe(-1582839653, new Action<object>(this.OnTagsChanged));
  }

  private void OnTagsChanged(object data)
  {
    TagChangedEventData changedEventData = (TagChangedEventData) data;
    if (!Tag.op_Equality(changedEventData.tag, GameTags.RocketNotOnGround))
      return;
    this.UpdateFlag(changedEventData.added);
  }

  private void UpdateFlag(bool newInOrbit)
  {
    this.operational.SetFlag(InOrbitRequired.inOrbitFlag, newInOrbit);
    ((Component) this).GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.InOrbitRequired, !newInOrbit, (object) this);
  }

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    descriptors.Add(new Descriptor((string) UI.BUILDINGEFFECTS.IN_ORBIT_REQUIRED, (string) UI.BUILDINGEFFECTS.TOOLTIPS.IN_ORBIT_REQUIRED, (Descriptor.DescriptorType) 0, false));
    return descriptors;
  }
}
