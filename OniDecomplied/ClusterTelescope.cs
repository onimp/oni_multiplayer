// Decompiled with JetBrains decompiler
// Type: ClusterTelescope
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using KSerialization;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class ClusterTelescope : 
  GameStateMachine<ClusterTelescope, ClusterTelescope.Instance, IStateMachineTarget, ClusterTelescope.Def>
{
  private static StatusItem noVisibilityStatusItem = new StatusItem("SPACE_VISIBILITY_NONE", "BUILDING", "status_item_no_sky", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, resolve_string_callback: new Func<string, object, string>(ClusterTelescope.GetStatusItemString));
  public GameStateMachine<ClusterTelescope, ClusterTelescope.Instance, IStateMachineTarget, ClusterTelescope.Def>.State all_work_complete;
  public ClusterTelescope.ReadyStates ready;

  private static string GetStatusItemString(string src_str, object data)
  {
    ClusterTelescope.Instance instance = (ClusterTelescope.Instance) data;
    return src_str.Replace("{VISIBILITY}", GameUtil.GetFormattedPercent(instance.PercentClear * 100f)).Replace("{RADIUS}", instance.def.clearScanCellRadius.ToString());
  }

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.ready.no_visibility;
    this.ready.EventTransition(GameHashes.ClusterFogOfWarRevealed, (Func<ClusterTelescope.Instance, KMonoBehaviour>) (smi => (KMonoBehaviour) Game.Instance), this.all_work_complete, (StateMachine<ClusterTelescope, ClusterTelescope.Instance, IStateMachineTarget, ClusterTelescope.Def>.Transition.ConditionCallback) (smi => !smi.CheckHasAnalyzeTarget()));
    this.ready.no_visibility.UpdateTransition(this.ready.ready_to_work, (Func<ClusterTelescope.Instance, float, bool>) ((smi, dt) => smi.HasSkyVisibility())).ToggleStatusItem(ClusterTelescope.noVisibilityStatusItem);
    this.ready.ready_to_work.UpdateTransition(this.ready.no_visibility, (Func<ClusterTelescope.Instance, float, bool>) ((smi, dt) => !smi.HasSkyVisibility())).ToggleChore((Func<ClusterTelescope.Instance, Chore>) (smi => smi.CreateChore()), this.ready.no_visibility);
    this.all_work_complete.ToggleMainStatusItem(Db.Get().BuildingStatusItems.ClusterTelescopeAllWorkComplete).EventTransition(GameHashes.ClusterLocationChanged, (Func<ClusterTelescope.Instance, KMonoBehaviour>) (smi => (KMonoBehaviour) Game.Instance), this.ready.no_visibility, (StateMachine<ClusterTelescope, ClusterTelescope.Instance, IStateMachineTarget, ClusterTelescope.Def>.Transition.ConditionCallback) (smi => smi.CheckHasAnalyzeTarget()));
  }

  public class Def : StateMachine.BaseDef
  {
    public int clearScanCellRadius = 15;
    public int analyzeClusterRadius = 3;
    public KAnimFile[] workableOverrideAnims;
    public bool providesOxygen;
  }

  public class ReadyStates : 
    GameStateMachine<ClusterTelescope, ClusterTelescope.Instance, IStateMachineTarget, ClusterTelescope.Def>.State
  {
    public GameStateMachine<ClusterTelescope, ClusterTelescope.Instance, IStateMachineTarget, ClusterTelescope.Def>.State no_visibility;
    public GameStateMachine<ClusterTelescope, ClusterTelescope.Instance, IStateMachineTarget, ClusterTelescope.Def>.State ready_to_work;
  }

  public new class Instance : 
    GameStateMachine<ClusterTelescope, ClusterTelescope.Instance, IStateMachineTarget, ClusterTelescope.Def>.GameInstance
  {
    private float m_percentClear;
    [Serialize]
    private bool m_hasAnalyzeTarget;
    [Serialize]
    private AxialI m_analyzeTarget;
    [MyCmpAdd]
    private ClusterTelescope.ClusterTelescopeWorkable m_workable;
    public KAnimFile[] workableOverrideAnims;
    public bool providesOxygen;

    public float PercentClear => this.m_percentClear;

    public Instance(IStateMachineTarget smi, ClusterTelescope.Def def)
      : base(smi, def)
    {
      this.workableOverrideAnims = def.workableOverrideAnims;
      this.providesOxygen = def.providesOxygen;
    }

    public bool CheckHasAnalyzeTarget()
    {
      ClusterFogOfWarManager.Instance smi = ((Component) SaveGame.Instance).GetSMI<ClusterFogOfWarManager.Instance>();
      if (this.m_hasAnalyzeTarget && !smi.IsLocationRevealed(this.m_analyzeTarget))
        return true;
      AxialI myWorldLocation = this.GetMyWorldLocation();
      this.m_hasAnalyzeTarget = smi.GetUnrevealedLocationWithinRadius(myWorldLocation, this.def.analyzeClusterRadius, out this.m_analyzeTarget);
      return this.m_hasAnalyzeTarget;
    }

    public Chore CreateChore()
    {
      WorkChore<ClusterTelescope.ClusterTelescopeWorkable> chore = new WorkChore<ClusterTelescope.ClusterTelescopeWorkable>(Db.Get().ChoreTypes.Research, (IStateMachineTarget) this.m_workable);
      if (this.providesOxygen)
        chore.AddPrecondition(Telescope.ContainsOxygen);
      return (Chore) chore;
    }

    public AxialI GetAnalyzeTarget()
    {
      Debug.Assert(this.m_hasAnalyzeTarget, (object) "GetAnalyzeTarget called but this telescope has no target assigned.");
      return this.m_analyzeTarget;
    }

    public bool HasSkyVisibility()
    {
      Extents extents = this.GetComponent<Building>().GetExtents();
      int cellsClear;
      int num = Grid.IsRangeExposedToSunlight(Grid.XYToCell(extents.x, extents.y), this.def.clearScanCellRadius, new CellOffset(1, 0), out cellsClear) ? 1 : 0;
      this.m_percentClear = (float) cellsClear / (float) (this.def.clearScanCellRadius * 2 + 1);
      return num != 0;
    }
  }

  public class ClusterTelescopeWorkable : Workable, OxygenBreather.IGasProvider
  {
    [MySmiReq]
    private ClusterTelescope.Instance m_telescope;
    private ClusterFogOfWarManager.Instance m_fowManager;
    private GameObject telescopeTargetMarker;
    private AxialI currentTarget;
    private OxygenBreather.IGasProvider workerGasProvider;
    [MyCmpGet]
    private Storage storage;
    private AttributeModifier radiationShielding;
    private float checkMarkerTimer;
    private float checkMarkerFrequency = 1f;

    protected override void OnPrefabInit()
    {
      base.OnPrefabInit();
      this.attributeConverter = Db.Get().AttributeConverters.ResearchSpeed;
      this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.ALL_DAY_EXPERIENCE;
      this.skillExperienceSkillGroup = Db.Get().SkillGroups.Research.Id;
      this.skillExperienceMultiplier = SKILLS.ALL_DAY_EXPERIENCE;
      this.requiredSkillPerk = Db.Get().SkillPerks.CanUseClusterTelescope.Id;
      this.workLayer = Grid.SceneLayer.BuildingUse;
      this.radiationShielding = new AttributeModifier(Db.Get().Attributes.RadiationResistance.Id, FIXEDTRAITS.COSMICRADIATION.TELESCOPE_RADIATION_SHIELDING, (string) STRINGS.BUILDINGS.PREFABS.CLUSTERTELESCOPEENCLOSED.NAME);
    }

    protected override void OnCleanUp()
    {
      if (Object.op_Inequality((Object) this.telescopeTargetMarker, (Object) null))
        Util.KDestroyGameObject(this.telescopeTargetMarker);
      base.OnCleanUp();
    }

    protected override void OnSpawn()
    {
      base.OnSpawn();
      this.OnWorkableEventCB = this.OnWorkableEventCB + new System.Action<Workable, Workable.WorkableEvent>(this.OnWorkableEvent);
      this.m_fowManager = ((Component) SaveGame.Instance).GetSMI<ClusterFogOfWarManager.Instance>();
      this.SetWorkTime(float.PositiveInfinity);
      this.overrideAnims = this.m_telescope.workableOverrideAnims;
    }

    private void OnWorkableEvent(Workable workable, Workable.WorkableEvent ev)
    {
      Worker worker = this.worker;
      if (Object.op_Equality((Object) worker, (Object) null))
        return;
      KPrefabID component1 = ((Component) worker).GetComponent<KPrefabID>();
      OxygenBreather component2 = ((Component) worker).GetComponent<OxygenBreather>();
      Attributes attributes = worker.GetAttributes();
      switch (ev)
      {
        case Workable.WorkableEvent.WorkStarted:
          this.ShowProgressBar(true);
          this.progressBar.SetUpdateFunc((Func<float>) (() => this.m_fowManager.GetRevealCompleteFraction(this.currentTarget)));
          this.currentTarget = this.m_telescope.GetAnalyzeTarget();
          if (!Object.op_Implicit((Object) ClusterGrid.Instance.GetEntityOfLayerAtCell(this.currentTarget, EntityLayer.Telescope)))
          {
            this.telescopeTargetMarker = GameUtil.KInstantiate(Assets.GetPrefab(Tag.op_Implicit("TelescopeTarget")), Grid.SceneLayer.Background);
            this.telescopeTargetMarker.SetActive(true);
            this.telescopeTargetMarker.GetComponent<TelescopeTarget>().Init(this.currentTarget);
          }
          if (this.m_telescope.providesOxygen)
          {
            attributes.Add(this.radiationShielding);
            this.workerGasProvider = component2.GetGasProvider();
            component2.SetGasProvider((OxygenBreather.IGasProvider) this);
            ((Behaviour) ((Component) component2).GetComponent<CreatureSimTemperatureTransfer>()).enabled = false;
            component1.AddTag(GameTags.Shaded, false);
          }
          ((Component) this).GetComponent<Operational>().SetActive(true);
          this.checkMarkerFrequency = Random.Range(2f, 5f);
          break;
        case Workable.WorkableEvent.WorkStopped:
          if (this.m_telescope.providesOxygen)
          {
            attributes.Remove(this.radiationShielding);
            component2.SetGasProvider(this.workerGasProvider);
            ((Behaviour) ((Component) component2).GetComponent<CreatureSimTemperatureTransfer>()).enabled = true;
            component1.RemoveTag(GameTags.Shaded);
          }
          ((Component) this).GetComponent<Operational>().SetActive(false);
          if (Object.op_Inequality((Object) this.telescopeTargetMarker, (Object) null))
            Util.KDestroyGameObject(this.telescopeTargetMarker);
          this.ShowProgressBar(false);
          break;
      }
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

    protected override bool OnWorkTick(Worker worker, float dt)
    {
      AxialI analyzeTarget = this.m_telescope.GetAnalyzeTarget();
      bool flag = false;
      if (AxialI.op_Inequality(analyzeTarget, this.currentTarget))
      {
        if (Object.op_Implicit((Object) this.telescopeTargetMarker))
          this.telescopeTargetMarker.GetComponent<TelescopeTarget>().Init(analyzeTarget);
        this.currentTarget = analyzeTarget;
        flag = true;
      }
      if (!flag && (double) this.checkMarkerTimer > (double) this.checkMarkerFrequency)
      {
        this.checkMarkerTimer = 0.0f;
        if (!Object.op_Implicit((Object) this.telescopeTargetMarker) && !Object.op_Implicit((Object) ClusterGrid.Instance.GetEntityOfLayerAtCell(this.currentTarget, EntityLayer.Telescope)))
        {
          this.telescopeTargetMarker = GameUtil.KInstantiate(Assets.GetPrefab(Tag.op_Implicit("TelescopeTarget")), Grid.SceneLayer.Background);
          this.telescopeTargetMarker.SetActive(true);
          this.telescopeTargetMarker.GetComponent<TelescopeTarget>().Init(this.currentTarget);
        }
      }
      this.checkMarkerTimer += dt;
      float num = (float) ((double) TUNING.ROCKETRY.CLUSTER_FOW.POINTS_TO_REVEAL / (double) TUNING.ROCKETRY.CLUSTER_FOW.DEFAULT_CYCLES_PER_REVEAL / 600.0);
      this.m_fowManager.EarnRevealPointsForLocation(this.currentTarget, dt * num);
      return base.OnWorkTick(worker, dt);
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
}
