// Decompiled with JetBrains decompiler
// Type: JetSuitConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class JetSuitConfig : IEquipmentConfig
{
  public const string ID = "Jet_Suit";
  public const string WORN_ID = "Worn_Jet_Suit";
  public static ComplexRecipe recipe;
  private const PathFinder.PotentialPath.Flags suit_flags = PathFinder.PotentialPath.Flags.HasJetPack;
  private AttributeModifier expertAthleticsModifier;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public EquipmentDef CreateEquipmentDef()
  {
    Dictionary<string, float> dictionary = new Dictionary<string, float>()
    {
      {
        SimHashes.Steel.ToString(),
        200f
      },
      {
        SimHashes.Petroleum.ToString(),
        25f
      }
    };
    List<AttributeModifier> AttributeModifiers = new List<AttributeModifier>();
    AttributeModifiers.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.INSULATION, (float) TUNING.EQUIPMENT.SUITS.ATMOSUIT_INSULATION, (string) STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME));
    AttributeModifiers.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, (float) TUNING.EQUIPMENT.SUITS.ATMOSUIT_ATHLETICS, (string) STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME));
    AttributeModifiers.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.THERMAL_CONDUCTIVITY_BARRIER, TUNING.EQUIPMENT.SUITS.ATMOSUIT_THERMAL_CONDUCTIVITY_BARRIER, (string) STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME));
    AttributeModifiers.Add(new AttributeModifier(Db.Get().Attributes.Digging.Id, (float) TUNING.EQUIPMENT.SUITS.ATMOSUIT_DIGGING, (string) STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME));
    AttributeModifiers.Add(new AttributeModifier(Db.Get().Attributes.ScaldingThreshold.Id, (float) TUNING.EQUIPMENT.SUITS.ATMOSUIT_SCALDING, (string) STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME));
    this.expertAthleticsModifier = new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, (float) -TUNING.EQUIPMENT.SUITS.ATMOSUIT_ATHLETICS, Db.Get().Skills.Suits1.Name);
    EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef("Jet_Suit", TUNING.EQUIPMENT.SUITS.SLOT, SimHashes.Steel, (float) TUNING.EQUIPMENT.SUITS.ATMOSUIT_MASS, "suit_jetpack_kanim", "", "body_jetpack_kanim", 6, AttributeModifiers, IsBody: true, additional_tags: new Tag[2]
    {
      GameTags.Suit,
      GameTags.Clothes
    }, RecipeTechUnlock: "JetSuit");
    equipmentDef.wornID = "Worn_Jet_Suit";
    equipmentDef.RecipeDescription = (string) STRINGS.EQUIPMENT.PREFABS.JET_SUIT.RECIPE_DESC;
    equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("SoakingWet"));
    equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("WetFeet"));
    equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("PoppedEarDrums"));
    equipmentDef.OnEquipCallBack = (Action<Equippable>) (eq =>
    {
      Ownables soleOwner = eq.assignee.GetSoleOwner();
      if (!Object.op_Inequality((Object) soleOwner, (Object) null))
        return;
      GameObject targetGameObject = ((Component) soleOwner).GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
      Navigator component1 = targetGameObject.GetComponent<Navigator>();
      if (Object.op_Inequality((Object) component1, (Object) null))
        component1.SetFlags(PathFinder.PotentialPath.Flags.HasJetPack);
      MinionResume component2 = targetGameObject.GetComponent<MinionResume>();
      if (Object.op_Inequality((Object) component2, (Object) null) && component2.HasPerk(HashedString.op_Implicit(Db.Get().SkillPerks.ExosuitExpertise.Id)))
        targetGameObject.GetAttributes().Get(Db.Get().Attributes.Athletics).Add(this.expertAthleticsModifier);
      KAnimControllerBase component3 = targetGameObject.GetComponent<KAnimControllerBase>();
      if (!Object.op_Implicit((Object) component3))
        return;
      component3.AddAnimOverrides(Assets.GetAnim(HashedString.op_Implicit("anim_loco_hover_kanim")));
    });
    equipmentDef.OnUnequipCallBack = (Action<Equippable>) (eq =>
    {
      if (eq.assignee == null)
        return;
      Ownables soleOwner = eq.assignee.GetSoleOwner();
      if (!Object.op_Implicit((Object) soleOwner))
        return;
      GameObject targetGameObject = ((Component) soleOwner).GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
      if (Object.op_Implicit((Object) targetGameObject))
      {
        targetGameObject.GetAttributes()?.Get(Db.Get().Attributes.Athletics).Remove(this.expertAthleticsModifier);
        Navigator component4 = targetGameObject.GetComponent<Navigator>();
        if (Object.op_Inequality((Object) component4, (Object) null))
          component4.ClearFlags(PathFinder.PotentialPath.Flags.HasJetPack);
        KAnimControllerBase component5 = targetGameObject.GetComponent<KAnimControllerBase>();
        if (Object.op_Implicit((Object) component5))
          component5.RemoveAnimOverrides(Assets.GetAnim(HashedString.op_Implicit("anim_loco_hover_kanim")));
        Effects component6 = targetGameObject.GetComponent<Effects>();
        if (Object.op_Inequality((Object) component6, (Object) null) && component6.HasEffect("SoiledSuit"))
          component6.Remove("SoiledSuit");
      }
      Tag elementTag = ((Component) eq).GetComponent<SuitTank>().elementTag;
      ((Component) eq).GetComponent<Storage>().DropUnlessHasTag(elementTag);
    });
    GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "Jet_Suit");
    GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "Helmet");
    return equipmentDef;
  }

  public void DoPostConfigure(GameObject go)
  {
    SuitTank suitTank = go.AddComponent<SuitTank>();
    suitTank.element = "Oxygen";
    suitTank.capacity = 75f;
    suitTank.elementTag = GameTags.Breathable;
    go.AddComponent<JetSuitTank>();
    go.AddComponent<HelmetController>().has_jets = true;
    KPrefabID component = go.GetComponent<KPrefabID>();
    component.AddTag(GameTags.Clothes, false);
    component.AddTag(GameTags.PedestalDisplayable, false);
    component.AddTag(GameTags.AirtightSuit, false);
    Durability durability = go.AddComponent<Durability>();
    durability.wornEquipmentPrefabID = "Worn_Jet_Suit";
    durability.durabilityLossPerCycle = TUNING.EQUIPMENT.SUITS.ATMOSUIT_DECAY;
    Storage storage = go.AddOrGet<Storage>();
    storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
    storage.showInUI = true;
    go.AddOrGet<AtmoSuit>();
    go.AddComponent<SuitDiseaseHandler>();
  }
}
