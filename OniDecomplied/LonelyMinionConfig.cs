// Decompiled with JetBrains decompiler
// Type: LonelyMinionConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class LonelyMinionConfig : IEntityConfig
{
  public static string ID = "LonelyMinion";
  public const int VOICE_IDX = -2;
  public const int STARTING_SKILL_POINTS = 3;
  public const int BASE_ATTRIBUTE_LEVEL = 7;
  public const int AGE_MIN = 2190;
  public const int AGE_MAX = 3102;
  public const float MIN_IDLE_DELAY = 20f;
  public const float MAX_IDLE_DELAY = 40f;
  public const string IDLE_PREFIX = "idle_blinds";
  public static readonly HashedString GreetingCriteraId = HashedString.op_Implicit("Neighbor");
  public static readonly HashedString FoodCriteriaId = HashedString.op_Implicit("FoodQuality");
  public static readonly HashedString DecorCriteriaId = HashedString.op_Implicit("Decor");
  public static readonly HashedString PowerCriteriaId = HashedString.op_Implicit("SuppliedPower");
  public static readonly HashedString CHECK_MAIL = HashedString.op_Implicit("mail_pre");
  public static readonly HashedString CHECK_MAIL_SUCCESS = HashedString.op_Implicit("mail_success_pst");
  public static readonly HashedString CHECK_MAIL_FAILURE = HashedString.op_Implicit("mail_failure_pst");
  public static readonly HashedString CHECK_MAIL_DUPLICATE = HashedString.op_Implicit("mail_duplicate_pst");
  public static readonly HashedString FOOD_SUCCESS = HashedString.op_Implicit("food_like_loop");
  public static readonly HashedString FOOD_FAILURE = HashedString.op_Implicit("food_dislike_loop");
  public static readonly HashedString FOOD_DUPLICATE = HashedString.op_Implicit("food_duplicate_loop");
  public static readonly HashedString FOOD_IDLE = HashedString.op_Implicit("idle_food_quest");
  public static readonly HashedString DECOR_IDLE = HashedString.op_Implicit("idle_decor_quest");
  public static readonly HashedString POWER_IDLE = HashedString.op_Implicit("idle_power_quest");
  public static readonly HashedString BLINDS_IDLE_0 = HashedString.op_Implicit("idle_blinds_0");
  public static readonly HashedString PARCEL_SNAPTO = HashedString.op_Implicit("parcel_snapTo");

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    string name = (string) DUPLICANTS.MODIFIERS.BASEDUPLICANT.NAME;
    GameObject entity = EntityTemplates.CreateEntity(LonelyMinionConfig.ID, name);
    entity.AddComponent<Accessorizer>();
    entity.AddComponent<Storage>().doDiseaseTransfer = false;
    entity.AddComponent<StateMachineController>();
    LonelyMinion.Def def = entity.AddOrGetDef<LonelyMinion.Def>();
    Tag tag;
    // ISSUE: explicit constructor call
    ((Tag) ref tag).\u002Ector("Jorge");
    string upper = ((Tag) ref tag).Name.ToUpper();
    def.Personality = new Personality(upper, StringEntry.op_Implicit(Strings.Get(string.Format("STRINGS.DUPLICANTS.PERSONALITIES.{0}.NAME", (object) upper))), "Male", "Grumpy", "UglyCrier", "BalloonArtist", "", "", 5, 5, -1, 3, 45, ((Tag) ref tag).GetHash(), StringEntry.op_Implicit(Strings.Get(string.Format("STRINGS.DUPLICANTS.PERSONALITIES.{0}.DESC", (object) upper))), false);
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit("body_lonelyminion_kanim"));
    List<AccessorySlot> resources = Db.Get().AccessorySlots.resources;
    for (int index = 0; index < resources.Count; ++index)
    {
      int count1 = resources[index].accessories.Count;
      resources[index].AddAccessories(anim, (ResourceSet) null);
      int count2 = resources[index].accessories.Count;
      if (count1 != count2)
      {
        Resource accessory = (Resource) resources[index].accessories[resources[index].accessories.Count - 1];
        Db.Get().ResourceTable.Add(accessory);
      }
    }
    def.Personality.Disabled = true;
    Db.Get().Personalities.Add(def.Personality);
    KBatchedAnimController kbatchedAnimController = entity.AddOrGet<KBatchedAnimController>();
    kbatchedAnimController.defaultAnim = "idle_default";
    kbatchedAnimController.initialAnim = "idle_default";
    kbatchedAnimController.initialMode = (KAnim.PlayMode) 0;
    kbatchedAnimController.AnimFiles = new KAnimFile[3]
    {
      Assets.GetAnim(HashedString.op_Implicit("body_comp_default_kanim")),
      Assets.GetAnim(HashedString.op_Implicit("anim_idles_default_kanim")),
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_lonely_dupe_kanim"))
    };
    this.ConfigurePackageOverride(entity);
    SymbolOverrideController prefab = SymbolOverrideControllerUtil.AddToPrefab(entity);
    prefab.applySymbolOverridesEveryFrame = true;
    prefab.AddSymbolOverride(HashedString.op_Implicit("snapto_cheek"), Assets.GetAnim(HashedString.op_Implicit("head_swap_kanim")).GetData().build.GetSymbol(KAnimHashedString.op_Implicit(string.Format("cheek_00{0}", (object) def.Personality.headShape))), 1);
    MinionConfig.ConfigureSymbols(entity);
    return entity;
  }

  public void OnPrefabInit(GameObject go)
  {
  }

  public void OnSpawn(GameObject go)
  {
  }

  private void ConfigurePackageOverride(GameObject go)
  {
    GameObject go1 = new GameObject("PackageSnapPoint");
    go1.transform.SetParent(go.transform);
    KBatchedAnimController component = go.GetComponent<KBatchedAnimController>();
    KBatchedAnimController kbatchedAnimController = go1.AddOrGet<KBatchedAnimController>();
    ((Component) kbatchedAnimController).transform.position = Vector3.op_Multiply(Vector3.forward, -0.1f);
    kbatchedAnimController.AnimFiles = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("mushbar_kanim"))
    };
    kbatchedAnimController.initialAnim = "object";
    component.SetSymbolVisiblity(KAnimHashedString.op_Implicit(LonelyMinionConfig.PARCEL_SNAPTO), false);
    KBatchedAnimTracker kbatchedAnimTracker = go1.AddOrGet<KBatchedAnimTracker>();
    kbatchedAnimTracker.controller = component;
    kbatchedAnimTracker.symbol = LonelyMinionConfig.PARCEL_SNAPTO;
  }

  public static void ApplyAccessoryOverrides(Accessorizer accessorizer)
  {
    int num = Hash.SDBMLower("Jorge");
    accessorizer.RemoveAccessory(accessorizer.GetAccessory(Db.Get().AccessorySlots.Neck));
    accessorizer.RemoveAccessory(accessorizer.GetAccessory(Db.Get().AccessorySlots.Leg));
    accessorizer.RemoveAccessory(accessorizer.GetAccessory(Db.Get().AccessorySlots.Belt));
    accessorizer.RemoveAccessory(accessorizer.GetAccessory(Db.Get().AccessorySlots.Pelvis));
    accessorizer.RemoveAccessory(accessorizer.GetAccessory(Db.Get().AccessorySlots.Foot));
    accessorizer.RemoveAccessory(accessorizer.GetAccessory(Db.Get().AccessorySlots.Hand));
    accessorizer.RemoveAccessory(accessorizer.GetAccessory(Db.Get().AccessorySlots.Cuff));
    accessorizer.AddAccessory(Db.Get().AccessorySlots.Neck.Lookup(string.Format("neck_{0}", (object) num)));
    accessorizer.AddAccessory(Db.Get().AccessorySlots.Leg.Lookup(string.Format("leg_{0}", (object) num)));
    accessorizer.AddAccessory(Db.Get().AccessorySlots.Belt.Lookup(string.Format("belt_{0}", (object) num)));
    accessorizer.AddAccessory(Db.Get().AccessorySlots.Pelvis.Lookup(string.Format("pelvis_{0}", (object) num)));
    accessorizer.AddAccessory(Db.Get().AccessorySlots.Foot.Lookup(string.Format("foot_{0}", (object) num)));
    accessorizer.AddAccessory(Db.Get().AccessorySlots.Hand.Lookup(string.Format("hand_paint_{0}", (object) num)));
    accessorizer.AddAccessory(Db.Get().AccessorySlots.Cuff.Lookup(string.Format("cuff_{0}", (object) num)));
  }
}
