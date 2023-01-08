// Decompiled with JetBrains decompiler
// Type: LonelyMinionHouseConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using TUNING;
using UnityEngine;

public class LonelyMinionHouseConfig : IBuildingConfig
{
  public const string ID = "LonelyMinionHouse";
  public const string LORE_UNLOCK_PREFIX = "story_trait_lonelyminion_";
  public const int FriendshipQuestCount = 3;
  public const string METER_TARGET = "meter_storage_target";
  public const string METER_ANIM = "meter";
  public static readonly string[] METER_SYMBOLS = new string[2]
  {
    "meter_storage",
    "meter_level"
  };
  public const string BLINDS_TARGET = "blinds_target";
  public const string BLINDS_PREFIX = "meter_blinds";
  public static readonly string[] BLINDS_SYMBOLS = new string[4]
  {
    "blinds_target",
    "blind",
    "blind_string",
    "blinds"
  };
  private const string LIGHTS_TARGET = "lights_target";
  private static readonly string[] LIGHTS_SYMBOLS = new string[5]
  {
    "lights_target",
    "festive_lights",
    "lights_wire",
    "light_bulb",
    "snapTo_light_locator"
  };
  public static readonly HashedString ANSWER = HashedString.op_Implicit("answer");
  public static readonly HashedString LIGHTS_OFF = HashedString.op_Implicit("meter_lights_off");
  public static readonly HashedString LIGHTS_ON = HashedString.op_Implicit("meter_lights_on_loop");
  public static readonly HashedString STORAGE = HashedString.op_Implicit("storage_off");
  public static readonly HashedString STORAGE_WORK_PST = HashedString.op_Implicit("working_pst");
  public static readonly HashedString[] STORAGE_WORKING = new HashedString[2]
  {
    HashedString.op_Implicit("working_pre"),
    HashedString.op_Implicit("working_loop")
  };
  public static readonly EffectorValues HOUSE_DECOR = new EffectorValues()
  {
    amount = -25,
    radius = 6
  };
  public static readonly EffectorValues STORAGE_DECOR = TUNING.DECOR.PENALTY.TIER1;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR5 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
    string[] construction_materials = new string[1]
    {
      SimHashes.Steel.ToString()
    };
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues houseDecor = LonelyMinionHouseConfig.HOUSE_DECOR;
    EffectorValues noise = none;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("LonelyMinionHouse", 4, 6, "lonely_dupe_home_kanim", 1000, 480f, tieR5, construction_materials, 9999f, BuildLocationRule.OnFloor, houseDecor, noise);
    buildingDef.DefaultAnimState = "on";
    buildingDef.ForegroundLayer = Grid.SceneLayer.BuildingFront;
    buildingDef.EnergyConsumptionWhenActive = 60f;
    buildingDef.AddLogicPowerPort = false;
    buildingDef.RequiresPowerInput = true;
    buildingDef.PowerInputOffset = new CellOffset(2, 1);
    buildingDef.ShowInBuildMenu = false;
    buildingDef.AudioCategory = "Metal";
    buildingDef.AudioSize = "large";
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.AddOrGet<NonEssentialEnergyConsumer>();
    go.GetComponent<Deconstructable>().allowDeconstruction = false;
    Prioritizable.AddRef(go);
    go.GetComponent<Prioritizable>().SetMasterPriority(new PrioritySetting(PriorityScreen.PriorityClass.high, 5));
    Storage storage = go.AddOrGet<Storage>();
    KnockKnock knockKnock = go.AddOrGet<KnockKnock>();
    LonelyMinionHouse.Def def = go.AddOrGetDef<LonelyMinionHouse.Def>();
    storage.allowItemRemoval = false;
    storage.capacityKg = 250000f;
    storage.storageFilters = STORAGEFILTERS.NOT_EDIBLE_SOLIDS;
    storage.storageFullMargin = TUNING.STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
    storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
    storage.showCapacityStatusItem = true;
    storage.showCapacityAsMainStatus = true;
    knockKnock.triggerWorkReactions = false;
    knockKnock.synchronizeAnims = false;
    knockKnock.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_doorknock_kanim"))
    };
    knockKnock.workAnims = new HashedString[2]
    {
      HashedString.op_Implicit("knocking_pre"),
      HashedString.op_Implicit("knocking_loop")
    };
    knockKnock.workingPstComplete = new HashedString[1]
    {
      HashedString.op_Implicit("knocking_pst")
    };
    knockKnock.workingPstFailed = (HashedString[]) null;
    knockKnock.SetButtonTextOverride(new ButtonMenuTextOverride()
    {
      Text = CODEX.STORY_TRAITS.LONELYMINION.KNOCK_KNOCK.TEXT,
      CancelText = CODEX.STORY_TRAITS.LONELYMINION.KNOCK_KNOCK.CANCELTEXT,
      ToolTip = CODEX.STORY_TRAITS.LONELYMINION.KNOCK_KNOCK.TOOLTIP,
      CancelToolTip = CODEX.STORY_TRAITS.LONELYMINION.KNOCK_KNOCK.CANCEL_TOOLTIP
    });
    def.Story = Db.Get().Stories.LonelyMinion;
    def.CompletionData = new StoryCompleteData()
    {
      KeepSakeSpawnOffset = new CellOffset(),
      CameraTargetOffset = new CellOffset(0, 3)
    };
    def.InitalLoreId = "story_trait_lonelyminion_initial";
    def.EventIntroInfo = new StoryManager.PopupInfo()
    {
      Title = (string) CODEX.STORY_TRAITS.LONELYMINION.BEGIN_POPUP.NAME,
      Description = (string) CODEX.STORY_TRAITS.LONELYMINION.BEGIN_POPUP.DESCRIPTION,
      CloseButtonText = (string) CODEX.STORY_TRAITS.CLOSE_BUTTON,
      TextureName = "minionhouseactivate_kanim",
      DisplayImmediate = true,
      PopupType = EventInfoDataHelper.PopupType.BEGIN
    };
    def.CompleteLoreId = "story_trait_lonelyminion_complete";
    def.EventCompleteInfo = new StoryManager.PopupInfo()
    {
      Title = (string) CODEX.STORY_TRAITS.LONELYMINION.END_POPUP.NAME,
      Description = (string) CODEX.STORY_TRAITS.LONELYMINION.END_POPUP.DESCRIPTION,
      CloseButtonText = (string) CODEX.STORY_TRAITS.LONELYMINION.END_POPUP.BUTTON,
      TextureName = "minionhousecomplete_kanim",
      PopupType = EventInfoDataHelper.PopupType.COMPLETE
    };
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    Object.Destroy((Object) go.GetComponent<BuildingEnabledButton>());
    go.GetComponent<RequireInputs>().visualizeRequirements = RequireInputs.Requirements.None;
    this.ConfigureLights(go);
  }

  private void ConfigureLights(GameObject go)
  {
    GameObject go1 = new GameObject("FestiveLights");
    go1.SetActive(false);
    go1.transform.SetParent(go.transform);
    go1.AddOrGet<Light2D>();
    KBatchedAnimController controller = go1.AddOrGet<KBatchedAnimController>();
    KBatchedAnimController component = go.GetComponent<KBatchedAnimController>();
    controller.AnimFiles = component.AnimFiles;
    controller.fgLayer = Grid.SceneLayer.NoLayer;
    controller.initialAnim = "meter_lights_off";
    controller.initialMode = (KAnim.PlayMode) 0;
    controller.isMovable = true;
    controller.FlipX = component.FlipX;
    controller.FlipY = component.FlipY;
    KBatchedAnimTracker kbatchedAnimTracker = go1.AddComponent<KBatchedAnimTracker>();
    kbatchedAnimTracker.SetAnimControllers(controller, component);
    kbatchedAnimTracker.symbol = HashedString.op_Implicit("lights_target");
    kbatchedAnimTracker.offset = Vector3.zero;
    for (int index = 0; index < LonelyMinionHouseConfig.LIGHTS_SYMBOLS.Length; ++index)
      component.SetSymbolVisiblity(KAnimHashedString.op_Implicit(LonelyMinionHouseConfig.LIGHTS_SYMBOLS[index]), false);
  }
}
