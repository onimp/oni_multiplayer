// Decompiled with JetBrains decompiler
// Type: EmptyConduitWorkable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/EmptyConduitWorkable")]
public class EmptyConduitWorkable : Workable
{
  [MyCmpReq]
  private Conduit conduit;
  private static StatusItem emptyLiquidConduitStatusItem;
  private static StatusItem emptyGasConduitStatusItem;
  private Chore chore;
  private const float RECHECK_PIPE_INTERVAL = 2f;
  private const float TIME_TO_EMPTY_PIPE = 4f;
  private const float NO_EMPTY_SCHEDULED = -1f;
  [Serialize]
  private float elapsedTime = -1f;
  private bool emptiedPipe = true;
  private static readonly EventSystem.IntraObjectHandler<EmptyConduitWorkable> OnEmptyConduitCancelledDelegate = new EventSystem.IntraObjectHandler<EmptyConduitWorkable>((Action<EmptyConduitWorkable, object>) ((component, data) => component.OnEmptyConduitCancelled(data)));

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.SetOffsetTable(OffsetGroups.InvertedStandardTable);
    this.SetWorkTime(float.PositiveInfinity);
    this.faceTargetWhenWorking = true;
    this.multitoolContext = HashedString.op_Implicit("build");
    this.multitoolHitEffectTag = Tag.op_Implicit(EffectConfigs.BuildSplashId);
    this.Subscribe<EmptyConduitWorkable>(2127324410, EmptyConduitWorkable.OnEmptyConduitCancelledDelegate);
    if (EmptyConduitWorkable.emptyLiquidConduitStatusItem == null)
    {
      EmptyConduitWorkable.emptyLiquidConduitStatusItem = new StatusItem("EmptyLiquidConduit", (string) BUILDINGS.PREFABS.CONDUIT.STATUS_ITEM.NAME, (string) BUILDINGS.PREFABS.CONDUIT.STATUS_ITEM.TOOLTIP, "status_item_empty_pipe", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.LiquidConduits.ID, 66);
      EmptyConduitWorkable.emptyGasConduitStatusItem = new StatusItem("EmptyGasConduit", (string) BUILDINGS.PREFABS.CONDUIT.STATUS_ITEM.NAME, (string) BUILDINGS.PREFABS.CONDUIT.STATUS_ITEM.TOOLTIP, "status_item_empty_pipe", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.GasConduits.ID, 130);
    }
    this.requiredSkillPerk = Db.Get().SkillPerks.CanDoPlumbing.Id;
    this.shouldShowSkillPerkStatusItem = false;
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    if ((double) this.elapsedTime == -1.0)
      return;
    this.MarkForEmptying();
  }

  public void MarkForEmptying()
  {
    if (this.chore != null)
      return;
    StatusItem statusItem = this.GetStatusItem();
    ((Component) this).GetComponent<KSelectable>().ToggleStatusItem(statusItem, true);
    this.CreateWorkChore();
  }

  private void CancelEmptying()
  {
    this.CleanUpVisualization();
    if (this.chore == null)
      return;
    this.chore.Cancel("Cancel");
    this.chore = (Chore) null;
    this.shouldShowSkillPerkStatusItem = false;
    this.UpdateStatusItem();
  }

  private void CleanUpVisualization()
  {
    StatusItem statusItem = this.GetStatusItem();
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    if (Object.op_Inequality((Object) component, (Object) null))
      component.ToggleStatusItem(statusItem, false);
    this.elapsedTime = -1f;
    if (this.chore == null)
      return;
    ((Component) this).GetComponent<Prioritizable>().RemoveRef();
  }

  protected override void OnCleanUp()
  {
    this.CancelEmptying();
    base.OnCleanUp();
  }

  private ConduitFlow GetFlowManager() => this.conduit.type != ConduitType.Gas ? Game.Instance.liquidConduitFlow : Game.Instance.gasConduitFlow;

  private void OnEmptyConduitCancelled(object data) => this.CancelEmptying();

  private StatusItem GetStatusItem()
  {
    switch (this.conduit.type)
    {
      case ConduitType.Gas:
        return EmptyConduitWorkable.emptyGasConduitStatusItem;
      case ConduitType.Liquid:
        return EmptyConduitWorkable.emptyLiquidConduitStatusItem;
      default:
        throw new ArgumentException();
    }
  }

  private void CreateWorkChore()
  {
    ((Component) this).GetComponent<Prioritizable>().AddRef();
    this.chore = (Chore) new WorkChore<EmptyConduitWorkable>(Db.Get().ChoreTypes.EmptyStorage, (IStateMachineTarget) this, only_when_operational: false);
    this.chore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, (object) Db.Get().SkillPerks.CanDoPlumbing.Id);
    this.elapsedTime = 0.0f;
    this.emptiedPipe = false;
    this.shouldShowSkillPerkStatusItem = true;
    this.UpdateStatusItem();
  }

  protected override bool OnWorkTick(Worker worker, float dt)
  {
    if ((double) this.elapsedTime == -1.0)
      return true;
    bool flag = false;
    this.elapsedTime += dt;
    if (!this.emptiedPipe)
    {
      if ((double) this.elapsedTime > 4.0)
      {
        this.EmptyPipeContents();
        this.emptiedPipe = true;
        this.elapsedTime = 0.0f;
      }
    }
    else if ((double) this.elapsedTime > 2.0)
    {
      int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
      if ((double) this.GetFlowManager().GetContents(cell).mass > 0.0)
      {
        this.elapsedTime = 0.0f;
        this.emptiedPipe = false;
      }
      else
      {
        this.CleanUpVisualization();
        this.chore = (Chore) null;
        flag = true;
        this.shouldShowSkillPerkStatusItem = false;
        this.UpdateStatusItem();
      }
    }
    return flag;
  }

  public override bool InstantlyFinish(Worker worker)
  {
    int num = (int) worker.Work(4f);
    return true;
  }

  public void EmptyPipeContents()
  {
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
    ConduitFlow.ConduitContents conduitContents = this.GetFlowManager().RemoveElement(cell, float.PositiveInfinity);
    this.elapsedTime = 0.0f;
    if ((double) conduitContents.mass <= 0.0 || conduitContents.element == SimHashes.Vacuum)
      return;
    IChunkManager instance;
    switch (this.conduit.type)
    {
      case ConduitType.Gas:
        instance = (IChunkManager) GasSourceManager.Instance;
        break;
      case ConduitType.Liquid:
        instance = (IChunkManager) LiquidSourceManager.Instance;
        break;
      default:
        throw new ArgumentException();
    }
    instance.CreateChunk(conduitContents.element, conduitContents.mass, conduitContents.temperature, conduitContents.diseaseIdx, conduitContents.diseaseCount, Grid.CellToPosCCC(cell, Grid.SceneLayer.Ore));
  }

  public override float GetPercentComplete() => Mathf.Clamp01(this.elapsedTime / 4f);
}
