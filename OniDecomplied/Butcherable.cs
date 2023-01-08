// Decompiled with JetBrains decompiler
// Type: Butcherable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Butcherable")]
public class Butcherable : Workable, ISaveLoadable
{
  [MyCmpGet]
  private KAnimControllerBase controller;
  [MyCmpGet]
  private Harvestable harvestable;
  private bool readyToButcher;
  private bool butchered;
  public string[] drops;
  private Chore chore;
  private static readonly EventSystem.IntraObjectHandler<Butcherable> SetReadyToButcherDelegate = new EventSystem.IntraObjectHandler<Butcherable>((Action<Butcherable, object>) ((component, data) => component.SetReadyToButcher(data)));
  private static readonly EventSystem.IntraObjectHandler<Butcherable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Butcherable>((Action<Butcherable, object>) ((component, data) => component.OnRefreshUserMenu(data)));

  public void SetDrops(string[] drops) => this.drops = drops;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<Butcherable>(1272413801, Butcherable.SetReadyToButcherDelegate);
    this.Subscribe<Butcherable>(493375141, Butcherable.OnRefreshUserMenuDelegate);
    this.workTime = 3f;
    this.multitoolContext = HashedString.op_Implicit("harvest");
    this.multitoolHitEffectTag = Tag.op_Implicit("fx_harvest_splash");
  }

  public void SetReadyToButcher(object param) => this.readyToButcher = true;

  public void SetReadyToButcher(bool ready) => this.readyToButcher = ready;

  public void ActivateChore(object param)
  {
    if (this.chore != null)
      return;
    this.chore = (Chore) new WorkChore<Butcherable>(Db.Get().ChoreTypes.Harvest, (IStateMachineTarget) this);
    this.OnRefreshUserMenu((object) null);
  }

  public void CancelChore(object param)
  {
    if (this.chore == null)
      return;
    this.chore.Cancel("User cancelled");
    this.chore = (Chore) null;
  }

  private void OnClickCancel() => this.CancelChore((object) null);

  private void OnClickButcher()
  {
    if (DebugHandler.InstantBuildMode)
      this.OnButcherComplete();
    else
      this.ActivateChore((object) null);
  }

  private void OnRefreshUserMenu(object data)
  {
    if (!this.readyToButcher)
      return;
    Game.Instance.userMenu.AddButton(((Component) this).gameObject, this.chore != null ? new KIconButtonMenu.ButtonInfo("action_harvest", "Cancel Meatify", new System.Action(this.OnClickCancel)) : new KIconButtonMenu.ButtonInfo("action_harvest", "Meatify", new System.Action(this.OnClickButcher)));
  }

  protected override void OnCompleteWork(Worker worker) => this.OnButcherComplete();

  public void OnButcherComplete()
  {
    if (this.butchered)
      return;
    KSelectable component1 = ((Component) this).GetComponent<KSelectable>();
    if (Object.op_Implicit((Object) component1) && component1.IsSelected)
      SelectTool.Instance.Select((KSelectable) null);
    for (int index = 0; index < this.drops.Length; ++index)
    {
      GameObject go = Scenario.SpawnPrefab(this.GetDropSpawnLocation(), 0, 0, this.drops[index]);
      go.SetActive(true);
      Edible component2 = go.GetComponent<Edible>();
      if (Object.op_Implicit((Object) component2))
        ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, component2.Calories, StringFormatter.Replace((string) UI.ENDOFDAYREPORT.NOTES.BUTCHERED, "{0}", go.GetProperName()), (string) UI.ENDOFDAYREPORT.NOTES.BUTCHERED_CONTEXT);
    }
    this.chore = (Chore) null;
    this.butchered = true;
    this.readyToButcher = false;
    Game.Instance.userMenu.Refresh(((Component) this).gameObject);
    this.Trigger(395373363, (object) null);
  }

  private int GetDropSpawnLocation()
  {
    int cell = Grid.PosToCell(((Component) this).gameObject);
    int num = Grid.CellAbove(cell);
    return Grid.IsValidCell(num) && !Grid.Solid[num] ? num : cell;
  }
}
