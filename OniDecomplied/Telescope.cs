// Decompiled with JetBrains decompiler
// Type: Telescope
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Telescope")]
public class Telescope : 
  Workable,
  OxygenBreather.IGasProvider,
  IGameObjectEffectDescriptor,
  ISim200ms
{
  public int clearScanCellRadius = 15;
  private OxygenBreather.IGasProvider workerGasProvider;
  private Operational operational;
  private float percentClear;
  private static readonly Operational.Flag visibleSkyFlag = new Operational.Flag("VisibleSky", Operational.Flag.Type.Requirement);
  private static StatusItem reducedVisibilityStatusItem;
  private static StatusItem noVisibilityStatusItem;
  private Storage storage;
  public static readonly Chore.Precondition ContainsOxygen = new Chore.Precondition()
  {
    id = nameof (ContainsOxygen),
    sortOrder = 1,
    description = (string) DUPLICANTS.CHORES.PRECONDITIONS.CONTAINS_OXYGEN,
    fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) => Object.op_Inequality((Object) context.chore.target.GetComponent<Storage>().FindFirstWithMass(GameTags.Oxygen), (Object) null))
  };
  private Chore chore;
  private Operational.Flag flag = new Operational.Flag("ValidTarget", Operational.Flag.Type.Requirement);

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.attributeConverter = Db.Get().AttributeConverters.ResearchSpeed;
    this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.ALL_DAY_EXPERIENCE;
    this.skillExperienceSkillGroup = Db.Get().SkillGroups.Research.Id;
    this.skillExperienceMultiplier = SKILLS.ALL_DAY_EXPERIENCE;
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    SpacecraftManager.instance.Subscribe(532901469, new Action<object>(this.UpdateWorkingState));
    Components.Telescopes.Add(this);
    if (Telescope.reducedVisibilityStatusItem == null)
    {
      Telescope.reducedVisibilityStatusItem = new StatusItem("SPACE_VISIBILITY_REDUCED", "BUILDING", "status_item_no_sky", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID);
      Telescope.reducedVisibilityStatusItem.resolveStringCallback = new Func<string, object, string>(Telescope.GetStatusItemString);
      Telescope.noVisibilityStatusItem = new StatusItem("SPACE_VISIBILITY_NONE", "BUILDING", "status_item_no_sky", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID);
      Telescope.noVisibilityStatusItem.resolveStringCallback = new Func<string, object, string>(Telescope.GetStatusItemString);
    }
    this.OnWorkableEventCB = this.OnWorkableEventCB + new Action<Workable, Workable.WorkableEvent>(this.OnWorkableEvent);
    this.operational = ((Component) this).GetComponent<Operational>();
    this.storage = ((Component) this).GetComponent<Storage>();
    this.UpdateWorkingState((object) null);
  }

  protected override void OnCleanUp()
  {
    Components.Telescopes.Remove(this);
    SpacecraftManager.instance.Unsubscribe(532901469, new Action<object>(this.UpdateWorkingState));
    base.OnCleanUp();
  }

  public void Sim200ms(float dt)
  {
    Extents extents = ((Component) this).GetComponent<Building>().GetExtents();
    int cellsClear;
    bool sunlight = Grid.IsRangeExposedToSunlight(Grid.XYToCell(extents.x, extents.y), this.clearScanCellRadius, new CellOffset(1, 0), out cellsClear);
    this.percentClear = (float) cellsClear / (float) (this.clearScanCellRadius * 2 + 1);
    KSelectable component1 = ((Component) this).GetComponent<KSelectable>();
    Operational component2 = ((Component) this).GetComponent<Operational>();
    component1.ToggleStatusItem(Telescope.noVisibilityStatusItem, !sunlight, (object) this);
    component1.ToggleStatusItem(Telescope.reducedVisibilityStatusItem, sunlight && (double) this.percentClear < 1.0, (object) this);
    component2.SetFlag(Telescope.visibleSkyFlag, sunlight);
    if (component2.IsActive || !component2.IsOperational || this.chore != null)
      return;
    this.chore = this.CreateChore();
    this.SetWorkTime(float.PositiveInfinity);
  }

  private static string GetStatusItemString(string src_str, object data)
  {
    Telescope telescope = (Telescope) data;
    return src_str.Replace("{VISIBILITY}", GameUtil.GetFormattedPercent(telescope.percentClear * 100f)).Replace("{RADIUS}", telescope.clearScanCellRadius.ToString());
  }

  private void OnWorkableEvent(Workable workable, Workable.WorkableEvent ev)
  {
    Worker worker = this.worker;
    if (Object.op_Equality((Object) worker, (Object) null))
      return;
    OxygenBreather component1 = ((Component) worker).GetComponent<OxygenBreather>();
    KPrefabID component2 = ((Component) worker).GetComponent<KPrefabID>();
    switch (ev)
    {
      case Workable.WorkableEvent.WorkStarted:
        this.ShowProgressBar(true);
        this.progressBar.SetUpdateFunc((Func<float>) (() => SpacecraftManager.instance.HasAnalysisTarget() ? SpacecraftManager.instance.GetDestinationAnalysisScore(SpacecraftManager.instance.GetStarmapAnalysisDestinationID()) / (float) TUNING.ROCKETRY.DESTINATION_ANALYSIS.COMPLETE : 0.0f));
        this.workerGasProvider = component1.GetGasProvider();
        component1.SetGasProvider((OxygenBreather.IGasProvider) this);
        ((Behaviour) ((Component) component1).GetComponent<CreatureSimTemperatureTransfer>()).enabled = false;
        component2.AddTag(GameTags.Shaded, false);
        break;
      case Workable.WorkableEvent.WorkStopped:
        component1.SetGasProvider(this.workerGasProvider);
        ((Behaviour) ((Component) component1).GetComponent<CreatureSimTemperatureTransfer>()).enabled = true;
        this.ShowProgressBar(false);
        component2.RemoveTag(GameTags.Shaded);
        break;
    }
  }

  protected override bool OnWorkTick(Worker worker, float dt)
  {
    if (SpacecraftManager.instance.HasAnalysisTarget())
    {
      int analysisDestinationId = SpacecraftManager.instance.GetStarmapAnalysisDestinationID();
      float num1 = 1f / (float) SpacecraftManager.instance.GetDestination(analysisDestinationId).OneBasedDistance;
      float num2 = (float) ((double) TUNING.ROCKETRY.DESTINATION_ANALYSIS.DISCOVERED / (double) TUNING.ROCKETRY.DESTINATION_ANALYSIS.DEFAULT_CYCLES_PER_DISCOVERY / 600.0);
      float points = dt * num1 * num2;
      SpacecraftManager.instance.EarnDestinationAnalysisPoints(analysisDestinationId, points);
    }
    return base.OnWorkTick(worker, dt);
  }

  public override List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = base.GetDescriptors(go);
    Element elementByHash = ElementLoader.FindElementByHash(SimHashes.Oxygen);
    Descriptor descriptor = new Descriptor();
    ((Descriptor) ref descriptor).SetupDescriptor(elementByHash.tag.ProperName(), string.Format((string) STRINGS.BUILDINGS.PREFABS.TELESCOPE.REQUIREMENT_TOOLTIP, (object) elementByHash.tag.ProperName()), (Descriptor.DescriptorType) 0);
    descriptors.Add(descriptor);
    return descriptors;
  }

  protected Chore CreateChore()
  {
    WorkChore<Telescope> chore = new WorkChore<Telescope>(Db.Get().ChoreTypes.Research, (IStateMachineTarget) this);
    chore.AddPrecondition(Telescope.ContainsOxygen);
    return (Chore) chore;
  }

  protected void UpdateWorkingState(object data)
  {
    bool flag1 = false;
    if (SpacecraftManager.instance.HasAnalysisTarget() && SpacecraftManager.instance.GetDestinationAnalysisState(SpacecraftManager.instance.GetDestination(SpacecraftManager.instance.GetStarmapAnalysisDestinationID())) != SpacecraftManager.DestinationAnalysisState.Complete)
      flag1 = true;
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    bool flag2 = !flag1 && !SpacecraftManager.instance.AreAllDestinationsAnalyzed();
    StatusItem analysisSelected = Db.Get().BuildingStatusItems.NoApplicableAnalysisSelected;
    int num = flag2 ? 1 : 0;
    component.ToggleStatusItem(analysisSelected, num != 0);
    this.operational.SetFlag(this.flag, flag1);
    if (flag1 || !Object.op_Implicit((Object) this.worker))
      return;
    this.StopWork(this.worker, true);
  }

  public void OnSetOxygenBreather(OxygenBreather oxygen_breather)
  {
  }

  public void OnClearOxygenBreather(OxygenBreather oxygen_breather)
  {
  }

  public bool ShouldEmitCO2() => false;

  public bool ShouldStoreCO2() => false;

  public bool ConsumeGas(OxygenBreather oxygen_breather, float amount)
  {
    if (this.storage.items.Count <= 0)
      return false;
    GameObject gameObject = this.storage.items[0];
    if (Object.op_Equality((Object) gameObject, (Object) null))
      return false;
    PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
    int num = (double) component.Mass >= (double) amount ? 1 : 0;
    component.Mass = Mathf.Max(0.0f, component.Mass - amount);
    return num != 0;
  }
}
