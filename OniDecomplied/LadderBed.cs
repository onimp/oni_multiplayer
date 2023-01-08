// Decompiled with JetBrains decompiler
// Type: LadderBed
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using System.Collections.Generic;
using UnityEngine;

public class LadderBed : 
  GameStateMachine<LadderBed, LadderBed.Instance, IStateMachineTarget, LadderBed.Def>
{
  public static string lightBedShakeSoundPath = GlobalAssets.GetSound("LadderBed_LightShake");
  public static string noDupeBedShakeSoundPath = GlobalAssets.GetSound("LadderBed_Shake");
  public static string LADDER_BED_COUNT_BELOW_PARAMETER = "bed_count";

  public override void InitializeStates(out StateMachine.BaseState default_state) => default_state = (StateMachine.BaseState) this.root;

  public class Def : StateMachine.BaseDef
  {
    public CellOffset[] offsets;
  }

  public new class Instance : 
    GameStateMachine<LadderBed, LadderBed.Instance, IStateMachineTarget, LadderBed.Def>.GameInstance
  {
    private List<HandleVector<int>.Handle> m_partitionEntires = new List<HandleVector<int>.Handle>();
    private int m_cell;
    [MyCmpGet]
    private Ownable m_ownable;
    [MyCmpGet]
    private Sleepable m_sleepable;
    [MyCmpGet]
    private AttachableBuilding m_attachable;
    private int numBelow;

    public Instance(IStateMachineTarget master, LadderBed.Def def)
      : base(master, def)
    {
      ScenePartitionerLayer objectLayer = GameScenePartitioner.Instance.objectLayers[40];
      this.m_cell = Grid.PosToCell(master.gameObject);
      foreach (CellOffset offset in def.offsets)
      {
        int cell = Grid.OffsetCell(this.m_cell, offset);
        if (Grid.IsValidCell(this.m_cell) && Grid.IsValidCell(cell))
        {
          this.m_partitionEntires.Add(GameScenePartitioner.Instance.Add("LadderBed.Constructor", (object) this.gameObject, cell, GameScenePartitioner.Instance.pickupablesChangedLayer, new System.Action<object>(this.OnMoverChanged)));
          this.OnMoverChanged((object) null);
        }
      }
      this.m_attachable.onAttachmentNetworkChanged += new System.Action<object>(this.OnAttachmentChanged);
      this.OnAttachmentChanged((object) null);
      this.Subscribe(-717201811, new System.Action<object>(this.OnSleepDisturbedByMovement));
      master.GetComponent<KAnimControllerBase>().GetLayering().GetLink().syncTint = false;
    }

    private void OnSleepDisturbedByMovement(object obj)
    {
      this.GetComponent<KAnimControllerBase>().Play(HashedString.op_Implicit("interrupt_light"));
      EventInstance instance = SoundEvent.BeginOneShot(LadderBed.lightBedShakeSoundPath, TransformExtensions.GetPosition(this.smi.transform));
      ((EventInstance) ref instance).setParameterByName(LadderBed.LADDER_BED_COUNT_BELOW_PARAMETER, (float) this.numBelow, false);
      SoundEvent.EndOneShot(instance);
    }

    private void OnAttachmentChanged(object data) => this.numBelow = AttachableBuilding.CountAttachedBelow(this.m_attachable);

    private void OnMoverChanged(object obj)
    {
      Pickupable cmp = obj as Pickupable;
      if (!Object.op_Inequality((Object) cmp, (Object) null) || !Object.op_Inequality((Object) ((Component) cmp).gameObject, (Object) null) || !((Component) cmp).HasTag(GameTags.Minion) || ((Component) cmp).GetComponent<Navigator>().CurrentNavType != NavType.Ladder)
        return;
      if (Object.op_Equality((Object) this.m_sleepable.worker, (Object) null))
      {
        this.GetComponent<KAnimControllerBase>().Play(HashedString.op_Implicit("interrupt_light_nodupe"));
        EventInstance instance = SoundEvent.BeginOneShot(LadderBed.noDupeBedShakeSoundPath, TransformExtensions.GetPosition(this.smi.transform));
        ((EventInstance) ref instance).setParameterByName(LadderBed.LADDER_BED_COUNT_BELOW_PARAMETER, (float) this.numBelow, false);
        SoundEvent.EndOneShot(instance);
      }
      else
      {
        if (!Object.op_Inequality((Object) ((Component) cmp).gameObject, (Object) ((Component) this.m_sleepable.worker).gameObject))
          return;
        this.m_sleepable.worker.Trigger(-717201811, (object) null);
      }
    }

    protected override void OnCleanUp()
    {
      foreach (HandleVector<int>.Handle partitionEntire in this.m_partitionEntires)
        GameScenePartitioner.Instance.Free(ref partitionEntire);
      this.m_attachable.onAttachmentNetworkChanged -= new System.Action<object>(this.OnAttachmentChanged);
      base.OnCleanUp();
    }
  }
}
