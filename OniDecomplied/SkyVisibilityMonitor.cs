// Decompiled with JetBrains decompiler
// Type: SkyVisibilityMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

public class SkyVisibilityMonitor : 
  GameStateMachine<SkyVisibilityMonitor, SkyVisibilityMonitor.Instance, IStateMachineTarget, SkyVisibilityMonitor.Def>
{
  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.root;
    this.root.Update(new System.Action<SkyVisibilityMonitor.Instance, float>(SkyVisibilityMonitor.CheckSkyVisibility), (UpdateRate) 6);
  }

  public static void CheckSkyVisibility(SkyVisibilityMonitor.Instance smi, float dt)
  {
    int num1 = smi.HasSkyVisibility ? 1 : 0;
    Grid.IsRangeExposedToSunlight(Grid.OffsetCell(Grid.PosToCell((StateMachine.Instance) smi), smi.def.ScanOriginOffset), smi.def.ScanRadius, smi.def.ScanShape, out smi.NumClearCells);
    int num2 = smi.HasSkyVisibility ? 1 : 0;
    if (num1 == num2)
      return;
    smi.TriggerVisibilityChange();
  }

  public class Def : StateMachine.BaseDef
  {
    public Operational.State AffectedOperationalState;
    public string StatusItemId = "SPACE_VISIBILITY_NONE";
    public int ScanRadius = 15;
    public CellOffset ScanShape = new CellOffset(1, 0);
    public CellOffset ScanOriginOffset = new CellOffset(0, 0);
  }

  public new class Instance : 
    GameStateMachine<SkyVisibilityMonitor, SkyVisibilityMonitor.Instance, IStateMachineTarget, SkyVisibilityMonitor.Def>.GameInstance
  {
    public int NumClearCells;
    public System.Action SkyVisibilityChanged;
    private StatusItem visibilityStatusItem;
    private Operational.Flag skyVisibilityFlag;

    public bool HasSkyVisibility => (double) this.PercentClearSky > 0.0;

    public float PercentClearSky => (float) this.NumClearCells * (float) (this.def.ScanRadius + 1);

    public Instance(IStateMachineTarget master, SkyVisibilityMonitor.Def def)
      : base(master, def)
    {
      if (string.IsNullOrEmpty(def.StatusItemId))
        return;
      if (def.AffectedOperationalState != Operational.State.None)
        this.skyVisibilityFlag = new Operational.Flag("sky visibility", Operational.Flag.GetFlagType(def.AffectedOperationalState));
      this.visibilityStatusItem = new StatusItem(def.StatusItemId, "BUILDING", "status_item_no_sky", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, resolve_string_callback: new Func<string, object, string>(SkyVisibilityMonitor.Instance.GetStatusItemString));
    }

    public override void StartSM()
    {
      base.StartSM();
      SkyVisibilityMonitor.CheckSkyVisibility(this, 0.0f);
      this.TriggerVisibilityChange();
    }

    public void TriggerVisibilityChange()
    {
      if (this.visibilityStatusItem != null)
        this.smi.GetComponent<KSelectable>().ToggleStatusItem(this.visibilityStatusItem, !this.HasSkyVisibility, (object) this);
      if (this.def.AffectedOperationalState != Operational.State.None)
        this.smi.GetComponent<Operational>().SetFlag(this.skyVisibilityFlag, this.HasSkyVisibility);
      if (this.SkyVisibilityChanged == null)
        return;
      this.SkyVisibilityChanged();
    }

    private static string GetStatusItemString(string src_str, object data)
    {
      SkyVisibilityMonitor.Instance instance = (SkyVisibilityMonitor.Instance) data;
      return src_str.Replace("{VISIBILITY}", GameUtil.GetFormattedPercent(instance.PercentClearSky * 100f)).Replace("{RADIUS}", instance.def.ScanRadius.ToString());
    }
  }
}
