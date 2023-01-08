// Decompiled with JetBrains decompiler
// Type: MinionIdentity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using Klei.CustomSettings;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/MinionIdentity")]
public class MinionIdentity : 
  KMonoBehaviour,
  ISaveLoadable,
  IAssignableIdentity,
  IListableOption,
  ISim1000ms
{
  public const string HairAlwaysSymbol = "snapto_hair_always";
  [MyCmpReq]
  private KSelectable selectable;
  public int femaleVoiceCount;
  public int maleVoiceCount;
  [Serialize]
  private string name;
  [Serialize]
  public string gender;
  [Serialize]
  public string stickerType;
  [Serialize]
  [ReadOnly]
  public float arrivalTime;
  [Serialize]
  public int voiceIdx;
  [Serialize]
  public Ref<MinionAssignablesProxy> assignableProxy;
  private Navigator navigator;
  private ChoreDriver choreDriver;
  public float timeLastSpoke;
  private string voiceId;
  private KAnimHashedString overrideExpression;
  private KAnimHashedString expression;
  public bool addToIdentityList = true;
  private static MinionIdentity.NameList maleNameList;
  private static MinionIdentity.NameList femaleNameList;
  private static readonly EventSystem.IntraObjectHandler<MinionIdentity> OnDeadTagAddedDelegate = GameUtil.CreateHasTagHandler<MinionIdentity>(GameTags.Dead, (Action<MinionIdentity, object>) ((component, data) => component.OnDied(data)));

  [Serialize]
  public string genderStringKey { get; set; }

  [Serialize]
  public string nameStringKey { get; set; }

  [Serialize]
  public HashedString personalityResourceId { get; set; }

  public static void DestroyStatics()
  {
    MinionIdentity.maleNameList = (MinionIdentity.NameList) null;
    MinionIdentity.femaleNameList = (MinionIdentity.NameList) null;
  }

  protected virtual void OnPrefabInit()
  {
    if (this.name == null)
      this.name = MinionIdentity.ChooseRandomName();
    if (Object.op_Inequality((Object) GameClock.Instance, (Object) null))
      this.arrivalTime = (float) GameClock.Instance.GetCycle();
    KAnimControllerBase component = ((Component) this).GetComponent<KAnimControllerBase>();
    if (Object.op_Inequality((Object) component, (Object) null))
      component.OnUpdateBounds += new Action<Bounds>(this.OnUpdateBounds);
    GameUtil.SubscribeToTags<MinionIdentity>(this, MinionIdentity.OnDeadTagAddedDelegate, true);
  }

  protected virtual void OnSpawn()
  {
    if (this.addToIdentityList)
    {
      this.ValidateProxy();
      this.CleanupLimboMinions();
    }
    PathProber component1 = ((Component) this).GetComponent<PathProber>();
    if (Object.op_Inequality((Object) component1, (Object) null))
      component1.SetGroupProber((IGroupProber) MinionGroupProber.Get());
    this.SetName(this.name);
    if (this.nameStringKey == null)
      this.nameStringKey = this.name;
    this.SetGender(this.gender);
    if (this.genderStringKey == null)
      this.genderStringKey = "NB";
    if (HashedString.op_Equality(this.personalityResourceId, HashedString.Invalid))
    {
      Personality fromNameStringKey = Db.Get().Personalities.GetPersonalityFromNameStringKey(this.nameStringKey);
      if (fromNameStringKey != null)
        this.personalityResourceId = HashedString.op_Implicit(fromNameStringKey.Id);
    }
    if (this.addToIdentityList)
    {
      Components.MinionIdentities.Add(this);
      if (!((Component) this).gameObject.HasTag(GameTags.Dead))
      {
        Components.LiveMinionIdentities.Add(this);
        Game.Instance.Trigger(2144209314, (object) this);
      }
    }
    SymbolOverrideController component2 = ((Component) this).GetComponent<SymbolOverrideController>();
    if (Object.op_Inequality((Object) component2, (Object) null))
    {
      Accessorizer component3 = ((Component) this).gameObject.GetComponent<Accessorizer>();
      if (Object.op_Inequality((Object) component3, (Object) null))
      {
        string str = HashCache.Get().Get(component3.GetAccessory(Db.Get().AccessorySlots.HeadShape).symbol.hash).Replace("headshape", "cheek");
        component2.AddSymbolOverride(HashedString.op_Implicit("snapto_cheek"), Assets.GetAnim(HashedString.op_Implicit("head_swap_kanim")).GetData().build.GetSymbol(KAnimHashedString.op_Implicit(str)), 1);
        component2.AddSymbolOverride(HashedString.op_Implicit("snapto_hair_always"), component3.GetAccessory(Db.Get().AccessorySlots.Hair).symbol, 1);
        component2.AddSymbolOverride(HashedString.op_Implicit(Db.Get().AccessorySlots.HatHair.targetSymbolId), Db.Get().AccessorySlots.HatHair.Lookup("hat_" + HashCache.Get().Get(component3.GetAccessory(Db.Get().AccessorySlots.Hair).symbol.hash)).symbol, 1);
      }
    }
    this.voiceId = (this.voiceIdx + 1).ToString("D2");
    Prioritizable component4 = ((Component) this).GetComponent<Prioritizable>();
    if (Object.op_Inequality((Object) component4, (Object) null))
      component4.showIcon = false;
    Pickupable component5 = ((Component) this).GetComponent<Pickupable>();
    if (Object.op_Inequality((Object) component5, (Object) null))
      component5.carryAnimOverride = Assets.GetAnim(HashedString.op_Implicit("anim_incapacitated_carrier_kanim"));
    this.ApplyCustomGameSettings();
  }

  public void ValidateProxy() => this.assignableProxy = MinionAssignablesProxy.InitAssignableProxy(this.assignableProxy, (IAssignableIdentity) this);

  private void CleanupLimboMinions()
  {
    KPrefabID component = ((Component) this).GetComponent<KPrefabID>();
    if (component.InstanceID == -1)
    {
      DebugUtil.LogWarningArgs(new object[2]
      {
        (object) "Minion with an invalid kpid! Attempting to recover...",
        (object) this.name
      });
      if (Object.op_Inequality((Object) KPrefabIDTracker.Get().GetInstance(component.InstanceID), (Object) null))
        KPrefabIDTracker.Get().Unregister(component);
      component.InstanceID = KPrefabID.GetUniqueID();
      KPrefabIDTracker.Get().Register(component);
      DebugUtil.LogWarningArgs(new object[2]
      {
        (object) "Restored as:",
        (object) component.InstanceID
      });
    }
    if (component.conflicted)
    {
      DebugUtil.LogWarningArgs(new object[3]
      {
        (object) "Minion with a conflicted kpid! Attempting to recover... ",
        (object) component.InstanceID,
        (object) this.name
      });
      if (Object.op_Inequality((Object) KPrefabIDTracker.Get().GetInstance(component.InstanceID), (Object) null))
        KPrefabIDTracker.Get().Unregister(component);
      component.InstanceID = KPrefabID.GetUniqueID();
      KPrefabIDTracker.Get().Register(component);
      DebugUtil.LogWarningArgs(new object[2]
      {
        (object) "Restored as:",
        (object) component.InstanceID
      });
    }
    this.assignableProxy.Get().SetTarget((IAssignableIdentity) this, ((Component) this).gameObject);
  }

  public string GetProperName() => ((Component) this).gameObject.GetProperName();

  public string GetVoiceId() => this.voiceId;

  public void SetName(string name)
  {
    this.name = name;
    if (Object.op_Inequality((Object) this.selectable, (Object) null))
      this.selectable.SetName(name);
    ((Object) ((Component) this).gameObject).name = name;
    NameDisplayScreen.Instance.UpdateName(((Component) this).gameObject);
  }

  public void SetStickerType(string stickerType) => this.stickerType = stickerType;

  public bool IsNull() => Object.op_Equality((Object) this, (Object) null);

  public void SetGender(string gender)
  {
    this.gender = gender;
    this.selectable.SetGender(gender);
  }

  public static string ChooseRandomName()
  {
    if (MinionIdentity.femaleNameList == null)
    {
      MinionIdentity.maleNameList = new MinionIdentity.NameList(Game.Instance.maleNamesFile);
      MinionIdentity.femaleNameList = new MinionIdentity.NameList(Game.Instance.femaleNamesFile);
    }
    return (double) Random.value > 0.5 ? MinionIdentity.maleNameList.Next() : MinionIdentity.femaleNameList.Next();
  }

  protected virtual void OnCleanUp()
  {
    if (this.assignableProxy != null)
    {
      MinionAssignablesProxy assignablesProxy = this.assignableProxy.Get();
      if (Object.op_Implicit((Object) assignablesProxy) && assignablesProxy.target == this)
        Util.KDestroyGameObject(((Component) assignablesProxy).gameObject);
    }
    Components.MinionIdentities.Remove(this);
    Components.LiveMinionIdentities.Remove(this);
    Game.Instance.Trigger(2144209314, (object) this);
  }

  private void OnUpdateBounds(Bounds bounds)
  {
    KBoxCollider2D component = ((Component) this).GetComponent<KBoxCollider2D>();
    component.offset = Vector2.op_Implicit(((Bounds) ref bounds).center);
    component.size = Vector2.op_Implicit(((Bounds) ref bounds).extents);
  }

  private void OnDied(object data)
  {
    this.GetSoleOwner().UnassignAll();
    this.GetEquipment().UnequipAll();
    Components.LiveMinionIdentities.Remove(this);
    Game.Instance.Trigger(2144209314, (object) this);
  }

  public List<Ownables> GetOwners() => this.assignableProxy.Get().ownables;

  public Ownables GetSoleOwner() => ((Component) this.assignableProxy.Get()).GetComponent<Ownables>();

  public bool HasOwner(Assignables owner) => this.GetOwners().Contains(owner as Ownables);

  public int NumOwners() => this.GetOwners().Count;

  public Equipment GetEquipment() => ((Component) this.assignableProxy.Get()).GetComponent<Equipment>();

  public void Sim1000ms(float dt)
  {
    if (Object.op_Equality((Object) this, (Object) null))
      return;
    if (Object.op_Equality((Object) this.navigator, (Object) null))
      this.navigator = ((Component) this).GetComponent<Navigator>();
    if (Object.op_Inequality((Object) this.navigator, (Object) null) && !this.navigator.IsMoving())
      return;
    if (Object.op_Equality((Object) this.choreDriver, (Object) null))
      this.choreDriver = ((Component) this).GetComponent<ChoreDriver>();
    if (!Object.op_Inequality((Object) this.choreDriver, (Object) null))
      return;
    Chore currentChore = this.choreDriver.GetCurrentChore();
    if (currentChore == null || !(currentChore is FetchAreaChore))
      return;
    MinionResume component = ((Component) this).GetComponent<MinionResume>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.AddExperienceWithAptitude(Db.Get().SkillGroups.Hauling.Id, dt, SKILLS.ALL_DAY_EXPERIENCE);
  }

  private void ApplyCustomGameSettings()
  {
    SettingLevel currentQualitySetting1 = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.ImmuneSystem);
    if (currentQualitySetting1.id == "Compromised")
    {
      Db.Get().Attributes.DiseaseCureSpeed.Lookup((Component) this).Add(new AttributeModifier(Db.Get().Attributes.DiseaseCureSpeed.Id, -0.3333f, (string) UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.COMPROMISED.ATTRIBUTE_MODIFIER_NAME));
      Db.Get().Attributes.GermResistance.Lookup((Component) this).Add(new AttributeModifier(Db.Get().Attributes.GermResistance.Id, -2f, (string) UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.COMPROMISED.ATTRIBUTE_MODIFIER_NAME));
    }
    else if (currentQualitySetting1.id == "Weak")
      Db.Get().Attributes.GermResistance.Lookup((Component) this).Add(new AttributeModifier(Db.Get().Attributes.GermResistance.Id, -1f, (string) UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.WEAK.ATTRIBUTE_MODIFIER_NAME));
    else if (currentQualitySetting1.id == "Strong")
    {
      Db.Get().Attributes.DiseaseCureSpeed.Lookup((Component) this).Add(new AttributeModifier(Db.Get().Attributes.DiseaseCureSpeed.Id, 2f, (string) UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.STRONG.ATTRIBUTE_MODIFIER_NAME));
      Db.Get().Attributes.GermResistance.Lookup((Component) this).Add(new AttributeModifier(Db.Get().Attributes.GermResistance.Id, 2f, (string) UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.STRONG.ATTRIBUTE_MODIFIER_NAME));
    }
    else if (currentQualitySetting1.id == "Invincible")
    {
      Db.Get().Attributes.DiseaseCureSpeed.Lookup((Component) this).Add(new AttributeModifier(Db.Get().Attributes.DiseaseCureSpeed.Id, 1E+08f, (string) UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.INVINCIBLE.ATTRIBUTE_MODIFIER_NAME));
      Db.Get().Attributes.GermResistance.Lookup((Component) this).Add(new AttributeModifier(Db.Get().Attributes.GermResistance.Id, 200f, (string) UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.INVINCIBLE.ATTRIBUTE_MODIFIER_NAME));
    }
    SettingLevel currentQualitySetting2 = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.Stress);
    if (currentQualitySetting2.id == "Doomed")
      Db.Get().Amounts.Stress.deltaAttribute.Lookup((Component) this).Add(new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0.0333333351f, (string) UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.DOOMED.ATTRIBUTE_MODIFIER_NAME));
    else if (currentQualitySetting2.id == "Pessimistic")
      Db.Get().Amounts.Stress.deltaAttribute.Lookup((Component) this).Add(new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0.0166666675f, (string) UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.PESSIMISTIC.ATTRIBUTE_MODIFIER_NAME));
    else if (currentQualitySetting2.id == "Optimistic")
      Db.Get().Amounts.Stress.deltaAttribute.Lookup((Component) this).Add(new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, -0.0166666675f, (string) UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.OPTIMISTIC.ATTRIBUTE_MODIFIER_NAME));
    else if (currentQualitySetting2.id == "Indomitable")
      Db.Get().Amounts.Stress.deltaAttribute.Lookup((Component) this).Add(new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, float.NegativeInfinity, (string) UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.INDOMITABLE.ATTRIBUTE_MODIFIER_NAME));
    SettingLevel currentQualitySetting3 = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.CalorieBurn);
    if (currentQualitySetting3.id == "VeryHard")
      Db.Get().Amounts.Calories.deltaAttribute.Lookup((Component) this).Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -1666.66663f, (string) UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.LEVELS.VERYHARD.ATTRIBUTE_MODIFIER_NAME));
    else if (currentQualitySetting3.id == "Hard")
      Db.Get().Amounts.Calories.deltaAttribute.Lookup((Component) this).Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -833.3333f, (string) UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.LEVELS.HARD.ATTRIBUTE_MODIFIER_NAME));
    else if (currentQualitySetting3.id == "Easy")
    {
      Db.Get().Amounts.Calories.deltaAttribute.Lookup((Component) this).Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, 833.3333f, (string) UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.LEVELS.EASY.ATTRIBUTE_MODIFIER_NAME));
    }
    else
    {
      if (!(currentQualitySetting3.id == "Disabled"))
        return;
      Db.Get().Amounts.Calories.deltaAttribute.Lookup((Component) this).Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, float.PositiveInfinity, (string) UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.LEVELS.DISABLED.ATTRIBUTE_MODIFIER_NAME));
    }
  }

  private class NameList
  {
    private List<string> names = new List<string>();
    private int idx;

    public NameList(TextAsset file)
    {
      string str1 = file.text.Replace("  ", " ").Replace("\r\n", "\n");
      char[] chArray1 = new char[1]{ '\n' };
      foreach (string str2 in str1.Split(chArray1))
      {
        char[] chArray2 = new char[1]{ ' ' };
        string[] strArray = str2.Split(chArray2);
        if (strArray[strArray.Length - 1] != "" && strArray[strArray.Length - 1] != null)
          this.names.Add(strArray[strArray.Length - 1]);
      }
      Util.Shuffle<string>((IList<string>) this.names);
    }

    public string Next() => this.names[this.idx++ % this.names.Count];
  }
}
