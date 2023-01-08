// Decompiled with JetBrains decompiler
// Type: Overheatable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
public class Overheatable : 
  StateMachineComponent<Overheatable.StatesInstance>,
  IGameObjectEffectDescriptor
{
  private bool modifiersInitialized;
  private AttributeInstance overheatTemp;
  private AttributeInstance fatalTemp;
  public float baseOverheatTemp;
  public float baseFatalTemp;

  public void ResetTemperature() => ((Component) this).GetComponent<PrimaryElement>().Temperature = 293.15f;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.overheatTemp = this.GetAttributes().Add(Db.Get().BuildingAttributes.OverheatTemperature);
    this.fatalTemp = this.GetAttributes().Add(Db.Get().BuildingAttributes.FatalTemperature);
  }

  private void InitializeModifiers()
  {
    if (this.modifiersInitialized)
      return;
    this.modifiersInitialized = true;
    AttributeModifier modifier1 = new AttributeModifier(this.overheatTemp.Id, this.baseOverheatTemp, (string) UI.TOOLTIPS.BASE_VALUE);
    AttributeModifier modifier2 = new AttributeModifier(this.fatalTemp.Id, this.baseFatalTemp, (string) UI.TOOLTIPS.BASE_VALUE);
    this.GetAttributes().Add(modifier1);
    this.GetAttributes().Add(modifier2);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.InitializeModifiers();
    HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(((Component) this).gameObject);
    if (handle.IsValid() && GameComps.StructureTemperatures.IsEnabled(handle))
    {
      GameComps.StructureTemperatures.Disable(handle);
      GameComps.StructureTemperatures.Enable(handle);
    }
    this.smi.StartSM();
  }

  public float OverheatTemperature
  {
    get
    {
      this.InitializeModifiers();
      return this.overheatTemp == null ? 10000f : this.overheatTemp.GetTotalValue();
    }
  }

  public Notification CreateOverheatedNotification()
  {
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    return new Notification((string) MISC.NOTIFICATIONS.BUILDINGOVERHEATED.NAME, NotificationType.BadMinor, (Func<List<Notification>, object, string>) ((notificationList, data) => (string) MISC.NOTIFICATIONS.BUILDINGOVERHEATED.TOOLTIP + notificationList.ReduceMessages(false)), (object) ("/t• " + component.GetProperName()), false);
  }

  private static string ToolTipResolver(List<Notification> notificationList, object data)
  {
    string str = "";
    for (int index = 0; index < notificationList.Count; ++index)
    {
      Notification notification = notificationList[index];
      str += (string) notification.tooltipData;
      if (index < notificationList.Count - 1)
        str += "\n";
    }
    return string.Format((string) MISC.NOTIFICATIONS.BUILDINGOVERHEATED.TOOLTIP, (object) str);
  }

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    if (this.overheatTemp != null && this.fatalTemp != null)
    {
      string formattedValue1 = this.overheatTemp.GetFormattedValue();
      string formattedValue2 = this.fatalTemp.GetFormattedValue();
      string format = (string) UI.BUILDINGEFFECTS.TOOLTIPS.OVERHEAT_TEMP + "\n\n" + this.overheatTemp.GetAttributeValueTooltip();
      Descriptor descriptor;
      // ISSUE: explicit constructor call
      ((Descriptor) ref descriptor).\u002Ector(string.Format((string) UI.BUILDINGEFFECTS.OVERHEAT_TEMP, (object) formattedValue1, (object) formattedValue2), string.Format(format, (object) formattedValue1, (object) formattedValue2), (Descriptor.DescriptorType) 1, false);
      descriptors.Add(descriptor);
    }
    else if ((double) this.baseOverheatTemp != 0.0)
    {
      string formattedTemperature1 = GameUtil.GetFormattedTemperature(this.baseOverheatTemp);
      string formattedTemperature2 = GameUtil.GetFormattedTemperature(this.baseFatalTemp);
      string overheatTemp = (string) UI.BUILDINGEFFECTS.TOOLTIPS.OVERHEAT_TEMP;
      Descriptor descriptor;
      // ISSUE: explicit constructor call
      ((Descriptor) ref descriptor).\u002Ector(string.Format((string) UI.BUILDINGEFFECTS.OVERHEAT_TEMP, (object) formattedTemperature1, (object) formattedTemperature2), string.Format(overheatTemp, (object) formattedTemperature1, (object) formattedTemperature2), (Descriptor.DescriptorType) 1, false);
      descriptors.Add(descriptor);
    }
    return descriptors;
  }

  public class StatesInstance : 
    GameStateMachine<Overheatable.States, Overheatable.StatesInstance, Overheatable, object>.GameInstance
  {
    public float lastOverheatDamageTime;

    public StatesInstance(Overheatable smi)
      : base(smi)
    {
    }

    public void TryDoOverheatDamage()
    {
      if ((double) Time.time - (double) this.lastOverheatDamageTime < 7.5)
        return;
      this.lastOverheatDamageTime += 7.5f;
      this.master.Trigger(-794517298, (object) new BuildingHP.DamageSourceInfo()
      {
        damage = 1,
        source = (string) BUILDINGS.DAMAGESOURCES.BUILDING_OVERHEATED,
        popString = (string) UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.OVERHEAT,
        fullDamageEffectName = "smoke_damage_kanim"
      });
    }
  }

  public class States : 
    GameStateMachine<Overheatable.States, Overheatable.StatesInstance, Overheatable>
  {
    public GameStateMachine<Overheatable.States, Overheatable.StatesInstance, Overheatable, object>.State invulnerable;
    public GameStateMachine<Overheatable.States, Overheatable.StatesInstance, Overheatable, object>.State safeTemperature;
    public GameStateMachine<Overheatable.States, Overheatable.StatesInstance, Overheatable, object>.State overheated;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.safeTemperature;
      this.root.EventTransition(GameHashes.BuildingBroken, this.invulnerable);
      this.invulnerable.EventHandler(GameHashes.BuildingPartiallyRepaired, (StateMachine<Overheatable.States, Overheatable.StatesInstance, Overheatable, object>.State.Callback) (smi => smi.master.ResetTemperature())).EventTransition(GameHashes.BuildingPartiallyRepaired, this.safeTemperature);
      this.safeTemperature.TriggerOnEnter(GameHashes.OptimalTemperatureAchieved).EventTransition(GameHashes.BuildingOverheated, this.overheated);
      this.overheated.Enter((StateMachine<Overheatable.States, Overheatable.StatesInstance, Overheatable, object>.State.Callback) (smi => Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_OverheatingBuildings))).EventTransition(GameHashes.BuildingNoLongerOverheated, this.safeTemperature).ToggleStatusItem(Db.Get().BuildingStatusItems.Overheated).ToggleNotification((Func<Overheatable.StatesInstance, Notification>) (smi => smi.master.CreateOverheatedNotification())).TriggerOnEnter(GameHashes.TooHotWarning).Enter("InitOverheatTime", (StateMachine<Overheatable.States, Overheatable.StatesInstance, Overheatable, object>.State.Callback) (smi => smi.lastOverheatDamageTime = Time.time)).Update("OverheatDamage", (Action<Overheatable.StatesInstance, float>) ((smi, dt) => smi.TryDoOverheatDamage()), (UpdateRate) 7);
    }
  }
}
