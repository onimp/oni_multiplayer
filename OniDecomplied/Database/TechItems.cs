// Decompiled with JetBrains decompiler
// Type: Database.TechItems
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

namespace Database
{
  public class TechItems : ResourceSet<TechItem>
  {
    public const string AUTOMATION_OVERLAY_ID = "AutomationOverlay";
    public TechItem automationOverlay;
    public const string SUITS_OVERLAY_ID = "SuitsOverlay";
    public TechItem suitsOverlay;
    public const string JET_SUIT_ID = "JetSuit";
    public TechItem jetSuit;
    public const string ATMO_SUIT_ID = "AtmoSuit";
    public TechItem atmoSuit;
    public const string OXYGEN_MASK_ID = "OxygenMask";
    public TechItem oxygenMask;
    public const string LEAD_SUIT_ID = "LeadSuit";
    public TechItem leadSuit;
    public const string BETA_RESEARCH_POINT_ID = "BetaResearchPoint";
    public TechItem betaResearchPoint;
    public const string GAMMA_RESEARCH_POINT_ID = "GammaResearchPoint";
    public TechItem gammaResearchPoint;
    public const string DELTA_RESEARCH_POINT_ID = "DeltaResearchPoint";
    public TechItem deltaResearchPoint;
    public const string ORBITAL_RESEARCH_POINT_ID = "OrbitalResearchPoint";
    public TechItem orbitalResearchPoint;
    public const string CONVEYOR_OVERLAY_ID = "ConveyorOverlay";
    public TechItem conveyorOverlay;

    public TechItems(ResourceSet parent)
      : base(nameof (TechItems), parent)
    {
    }

    public void Init()
    {
      this.automationOverlay = this.AddTechItem("AutomationOverlay", (string) RESEARCH.OTHER_TECH_ITEMS.AUTOMATION_OVERLAY.NAME, (string) RESEARCH.OTHER_TECH_ITEMS.AUTOMATION_OVERLAY.DESC, this.GetSpriteFnBuilder("overlay_logic"), DlcManager.AVAILABLE_ALL_VERSIONS);
      this.suitsOverlay = this.AddTechItem("SuitsOverlay", (string) RESEARCH.OTHER_TECH_ITEMS.SUITS_OVERLAY.NAME, (string) RESEARCH.OTHER_TECH_ITEMS.SUITS_OVERLAY.DESC, this.GetSpriteFnBuilder("overlay_suit"), DlcManager.AVAILABLE_ALL_VERSIONS);
      this.betaResearchPoint = this.AddTechItem("BetaResearchPoint", (string) RESEARCH.OTHER_TECH_ITEMS.BETA_RESEARCH_POINT.NAME, (string) RESEARCH.OTHER_TECH_ITEMS.BETA_RESEARCH_POINT.DESC, this.GetSpriteFnBuilder("research_type_beta_icon"), DlcManager.AVAILABLE_ALL_VERSIONS);
      this.gammaResearchPoint = this.AddTechItem("GammaResearchPoint", (string) RESEARCH.OTHER_TECH_ITEMS.GAMMA_RESEARCH_POINT.NAME, (string) RESEARCH.OTHER_TECH_ITEMS.GAMMA_RESEARCH_POINT.DESC, this.GetSpriteFnBuilder("research_type_gamma_icon"), DlcManager.AVAILABLE_ALL_VERSIONS);
      this.orbitalResearchPoint = this.AddTechItem("OrbitalResearchPoint", (string) RESEARCH.OTHER_TECH_ITEMS.ORBITAL_RESEARCH_POINT.NAME, (string) RESEARCH.OTHER_TECH_ITEMS.ORBITAL_RESEARCH_POINT.DESC, this.GetSpriteFnBuilder("research_type_orbital_icon"), DlcManager.AVAILABLE_ALL_VERSIONS);
      this.conveyorOverlay = this.AddTechItem("ConveyorOverlay", (string) RESEARCH.OTHER_TECH_ITEMS.CONVEYOR_OVERLAY.NAME, (string) RESEARCH.OTHER_TECH_ITEMS.CONVEYOR_OVERLAY.DESC, this.GetSpriteFnBuilder("overlay_conveyor"), DlcManager.AVAILABLE_ALL_VERSIONS);
      this.jetSuit = this.AddTechItem("JetSuit", (string) RESEARCH.OTHER_TECH_ITEMS.JET_SUIT.NAME, (string) RESEARCH.OTHER_TECH_ITEMS.JET_SUIT.DESC, this.GetPrefabSpriteFnBuilder(TagExtensions.ToTag("Jet_Suit")), DlcManager.AVAILABLE_ALL_VERSIONS);
      this.atmoSuit = this.AddTechItem("AtmoSuit", (string) RESEARCH.OTHER_TECH_ITEMS.ATMO_SUIT.NAME, (string) RESEARCH.OTHER_TECH_ITEMS.ATMO_SUIT.DESC, this.GetPrefabSpriteFnBuilder(TagExtensions.ToTag("Atmo_Suit")), DlcManager.AVAILABLE_ALL_VERSIONS);
      this.oxygenMask = this.AddTechItem("OxygenMask", (string) RESEARCH.OTHER_TECH_ITEMS.OXYGEN_MASK.NAME, (string) RESEARCH.OTHER_TECH_ITEMS.OXYGEN_MASK.DESC, this.GetPrefabSpriteFnBuilder(TagExtensions.ToTag("Oxygen_Mask")), DlcManager.AVAILABLE_ALL_VERSIONS);
      this.deltaResearchPoint = this.AddTechItem("DeltaResearchPoint", (string) RESEARCH.OTHER_TECH_ITEMS.DELTA_RESEARCH_POINT.NAME, (string) RESEARCH.OTHER_TECH_ITEMS.DELTA_RESEARCH_POINT.DESC, this.GetSpriteFnBuilder("research_type_delta_icon"), DlcManager.AVAILABLE_EXPANSION1_ONLY);
      this.leadSuit = this.AddTechItem("LeadSuit", (string) RESEARCH.OTHER_TECH_ITEMS.LEAD_SUIT.NAME, (string) RESEARCH.OTHER_TECH_ITEMS.LEAD_SUIT.DESC, this.GetPrefabSpriteFnBuilder(TagExtensions.ToTag("Lead_Suit")), DlcManager.AVAILABLE_EXPANSION1_ONLY);
    }

    private Func<string, bool, Sprite> GetSpriteFnBuilder(string spriteName) => (Func<string, bool, Sprite>) ((anim, centered) => Assets.GetSprite(HashedString.op_Implicit(spriteName)));

    private Func<string, bool, Sprite> GetPrefabSpriteFnBuilder(Tag prefabTag) => (Func<string, bool, Sprite>) ((anim, centered) => Def.GetUISprite((object) prefabTag).first);

    public TechItem AddTechItem(
      string id,
      string name,
      string description,
      Func<string, bool, Sprite> getUISprite,
      string[] DLCIds)
    {
      if (!DlcManager.IsDlcListValidForCurrentContent(DLCIds))
        return (TechItem) null;
      if (this.TryGet(id) != null)
      {
        DebugUtil.LogWarningArgs(new object[4]
        {
          (object) "Tried adding a tech item called",
          (object) id,
          (object) name,
          (object) "but it was already added!"
        });
        return this.Get(id);
      }
      Tech techFromItemId = this.GetTechFromItemID(id);
      if (techFromItemId == null)
        return (TechItem) null;
      TechItem techItem = new TechItem(id, (ResourceSet) this, name, description, getUISprite, techFromItemId.Id, DLCIds);
      this.Add(techItem);
      techFromItemId.unlockedItems.Add(techItem);
      return techItem;
    }

    public bool IsTechItemComplete(string id)
    {
      bool flag = true;
      foreach (TechItem resource in this.resources)
      {
        if (resource.Id == id)
        {
          flag = resource.IsComplete();
          break;
        }
      }
      return flag;
    }

    private Tech GetTechFromItemID(string itemId) => Db.Get().Techs == null ? (Tech) null : Db.Get().Techs.TryGetTechForTechItem(itemId);

    public int GetTechTierForItem(string itemId)
    {
      Tech techFromItemId = this.GetTechFromItemID(itemId);
      return techFromItemId != null ? Techs.GetTier(techFromItemId) : 0;
    }
  }
}
