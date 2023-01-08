// Decompiled with JetBrains decompiler
// Type: EquippableBalloonConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class EquippableBalloonConfig : IEquipmentConfig
{
  public const string ID = "EquippableBalloon";
  private BalloonFX.Instance fx;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public EquipmentDef CreateEquipmentDef()
  {
    List<AttributeModifier> AttributeModifiers = new List<AttributeModifier>();
    EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef("EquippableBalloon", EQUIPMENT.TOYS.SLOT, SimHashes.Carbon, EQUIPMENT.TOYS.BALLOON_MASS, EQUIPMENT.VESTS.COOL_VEST_ICON0, (string) null, (string) null, 0, AttributeModifiers, CollisionShape: EntityTemplates.CollisionShape.RECTANGLE, width: 0.75f, height: 0.4f);
    equipmentDef.OnEquipCallBack = new Action<Equippable>(this.OnEquipBalloon);
    equipmentDef.OnUnequipCallBack = new Action<Equippable>(this.OnUnequipBalloon);
    return equipmentDef;
  }

  private void OnEquipBalloon(Equippable eq)
  {
    if (!Object.op_Inequality((Object) eq, (Object) null) || eq.assignee == null)
      return;
    Ownables soleOwner = eq.assignee.GetSoleOwner();
    if (Object.op_Equality((Object) soleOwner, (Object) null))
      return;
    MinionAssignablesProxy component1 = ((Component) soleOwner).GetComponent<MinionAssignablesProxy>();
    Effects component2 = ((Component) (component1.target as KMonoBehaviour)).GetComponent<Effects>();
    if (!Object.op_Inequality((Object) component2, (Object) null))
      return;
    component2.Add("HasBalloon", false);
    this.fx = new BalloonFX.Instance((IStateMachineTarget) ((Component) (component1.target as KMonoBehaviour)).GetComponent<KMonoBehaviour>());
    this.fx.StartSM();
  }

  private void OnUnequipBalloon(Equippable eq)
  {
    if (Object.op_Inequality((Object) eq, (Object) null) && eq.assignee != null)
    {
      Ownables soleOwner = eq.assignee.GetSoleOwner();
      if (Object.op_Equality((Object) soleOwner, (Object) null))
        return;
      MinionAssignablesProxy component1 = ((Component) soleOwner).GetComponent<MinionAssignablesProxy>();
      if (!Util.IsNullOrDestroyed((object) component1.target))
      {
        Effects component2 = ((Component) (component1.target as KMonoBehaviour)).GetComponent<Effects>();
        if (Object.op_Inequality((Object) component2, (Object) null))
          component2.Remove("HasBalloon");
      }
    }
    if (this.fx != null)
      this.fx.StopSM("Unequipped");
    Util.KDestroyGameObject(((Component) eq).gameObject);
  }

  public void DoPostConfigure(GameObject go)
  {
    go.GetComponent<KPrefabID>().AddTag(GameTags.Clothes, false);
    Equippable equippable = go.GetComponent<Equippable>();
    if (Object.op_Equality((Object) equippable, (Object) null))
      equippable = go.AddComponent<Equippable>();
    equippable.hideInCodex = true;
    equippable.unequippable = false;
    go.AddOrGet<EquippableBalloon>();
  }
}
