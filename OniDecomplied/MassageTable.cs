// Decompiled with JetBrains decompiler
// Type: MassageTable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MassageTable : RelaxationPoint, IGameObjectEffectDescriptor, IActivationRangeTarget
{
  [Serialize]
  private float activateValue = 50f;
  private static readonly string[] EffectsRemoved = new string[1]
  {
    "SoreBack"
  };
  private static readonly EventSystem.IntraObjectHandler<MassageTable> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<MassageTable>((Action<MassageTable, object>) ((component, data) => component.OnCopySettings(data)));
  private static readonly Chore.Precondition IsStressAboveActivationRange = new Chore.Precondition()
  {
    id = nameof (IsStressAboveActivationRange),
    description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_STRESS_ABOVE_ACTIVATION_RANGE,
    fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      IActivationRangeTarget activationRangeTarget = (IActivationRangeTarget) data;
      return (double) Db.Get().Amounts.Stress.Lookup(context.consumerState.gameObject).value >= (double) activationRangeTarget.ActivateValue;
    })
  };

  public string ActivateTooltip => (string) BUILDINGS.PREFABS.MASSAGETABLE.ACTIVATE_TOOLTIP;

  public string DeactivateTooltip => (string) BUILDINGS.PREFABS.MASSAGETABLE.DEACTIVATE_TOOLTIP;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<MassageTable>(-905833192, MassageTable.OnCopySettingsDelegate);
  }

  private void OnCopySettings(object data)
  {
    MassageTable component = ((GameObject) data).GetComponent<MassageTable>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    this.ActivateValue = component.ActivateValue;
    this.DeactivateValue = component.DeactivateValue;
  }

  protected override void OnCompleteWork(Worker worker)
  {
    base.OnCompleteWork(worker);
    Effects component = ((Component) worker).GetComponent<Effects>();
    for (int index = 0; index < MassageTable.EffectsRemoved.Length; ++index)
    {
      string effect_id = MassageTable.EffectsRemoved[index];
      component.Remove(effect_id);
    }
  }

  public new List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    Descriptor descriptor1 = new Descriptor();
    ((Descriptor) ref descriptor1).SetupDescriptor(string.Format((string) UI.BUILDINGEFFECTS.STRESSREDUCEDPERMINUTE, (object) GameUtil.GetFormattedPercent((float) ((double) this.stressModificationValue / 600.0 * 60.0))), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.STRESSREDUCEDPERMINUTE, (object) GameUtil.GetFormattedPercent((float) ((double) this.stressModificationValue / 600.0 * 60.0))), (Descriptor.DescriptorType) 1);
    descriptors.Add(descriptor1);
    if (MassageTable.EffectsRemoved.Length != 0)
    {
      Descriptor descriptor2 = new Descriptor();
      ((Descriptor) ref descriptor2).SetupDescriptor((string) UI.BUILDINGEFFECTS.REMOVESEFFECTSUBTITLE, (string) UI.BUILDINGEFFECTS.TOOLTIPS.REMOVESEFFECTSUBTITLE, (Descriptor.DescriptorType) 1);
      descriptors.Add(descriptor2);
      for (int index = 0; index < MassageTable.EffectsRemoved.Length; ++index)
      {
        string str1 = MassageTable.EffectsRemoved[index];
        string str2 = StringEntry.op_Implicit(Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + str1.ToUpper() + ".NAME"));
        string str3 = StringEntry.op_Implicit(Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + str1.ToUpper() + ".CAUSE"));
        Descriptor descriptor3 = new Descriptor();
        ((Descriptor) ref descriptor3).IncreaseIndent();
        ((Descriptor) ref descriptor3).SetupDescriptor("• " + string.Format((string) UI.BUILDINGEFFECTS.REMOVEDEFFECT, (object) str2), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.REMOVEDEFFECT, (object) str3), (Descriptor.DescriptorType) 1);
        descriptors.Add(descriptor3);
      }
    }
    return descriptors;
  }

  protected override WorkChore<RelaxationPoint> CreateWorkChore()
  {
    WorkChore<RelaxationPoint> workChore = new WorkChore<RelaxationPoint>(Db.Get().ChoreTypes.StressHeal, (IStateMachineTarget) this, allow_in_red_alert: false, ignore_schedule_block: true, allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.high);
    workChore.AddPrecondition(ChorePreconditions.instance.IsNotARobot, (object) this);
    workChore.AddPrecondition(MassageTable.IsStressAboveActivationRange, (object) this);
    return workChore;
  }

  public float ActivateValue
  {
    get => this.activateValue;
    set => this.activateValue = value;
  }

  public float DeactivateValue
  {
    get => this.stopStressingValue;
    set => this.stopStressingValue = value;
  }

  public bool UseWholeNumbers => true;

  public float MinValue => 0.0f;

  public float MaxValue => 100f;

  public string ActivationRangeTitleText => (string) UI.UISIDESCREENS.ACTIVATION_RANGE_SIDE_SCREEN.NAME;

  public string ActivateSliderLabelText => (string) UI.UISIDESCREENS.ACTIVATION_RANGE_SIDE_SCREEN.ACTIVATE;

  public string DeactivateSliderLabelText => (string) UI.UISIDESCREENS.ACTIVATION_RANGE_SIDE_SCREEN.DEACTIVATE;
}
