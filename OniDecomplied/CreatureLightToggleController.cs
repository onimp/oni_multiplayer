// Decompiled with JetBrains decompiler
// Type: CreatureLightToggleController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class CreatureLightToggleController : 
  GameStateMachine<CreatureLightToggleController, CreatureLightToggleController.Instance, IStateMachineTarget, CreatureLightToggleController.Def>
{
  private GameStateMachine<CreatureLightToggleController, CreatureLightToggleController.Instance, IStateMachineTarget, CreatureLightToggleController.Def>.State light_off;
  private GameStateMachine<CreatureLightToggleController, CreatureLightToggleController.Instance, IStateMachineTarget, CreatureLightToggleController.Def>.State turning_off;
  private GameStateMachine<CreatureLightToggleController, CreatureLightToggleController.Instance, IStateMachineTarget, CreatureLightToggleController.Def>.State light_on;
  private GameStateMachine<CreatureLightToggleController, CreatureLightToggleController.Instance, IStateMachineTarget, CreatureLightToggleController.Def>.State turning_on;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.light_on;
    this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
    this.light_off.Enter((StateMachine<CreatureLightToggleController, CreatureLightToggleController.Instance, IStateMachineTarget, CreatureLightToggleController.Def>.State.Callback) (smi => smi.SwitchLight(false))).TagTransition(GameTags.Creatures.Overcrowded, this.turning_on, true);
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    this.turning_off.BatchUpdate(CreatureLightToggleController.\u003C\u003Ec.\u003C\u003E9__5_1 ?? (CreatureLightToggleController.\u003C\u003Ec.\u003C\u003E9__5_1 = new UpdateBucketWithUpdater<CreatureLightToggleController.Instance>.BatchUpdateDelegate((object) CreatureLightToggleController.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CInitializeStates\u003Eb__5_1)))).Transition(this.light_off, (StateMachine<CreatureLightToggleController, CreatureLightToggleController.Instance, IStateMachineTarget, CreatureLightToggleController.Def>.Transition.ConditionCallback) (smi => smi.IsOff()));
    this.light_on.Enter((StateMachine<CreatureLightToggleController, CreatureLightToggleController.Instance, IStateMachineTarget, CreatureLightToggleController.Def>.State.Callback) (smi => smi.SwitchLight(true))).TagTransition(GameTags.Creatures.Overcrowded, this.turning_off);
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    this.turning_on.Enter((StateMachine<CreatureLightToggleController, CreatureLightToggleController.Instance, IStateMachineTarget, CreatureLightToggleController.Def>.State.Callback) (smi => smi.SwitchLight(true))).BatchUpdate(CreatureLightToggleController.\u003C\u003Ec.\u003C\u003E9__5_5 ?? (CreatureLightToggleController.\u003C\u003Ec.\u003C\u003E9__5_5 = new UpdateBucketWithUpdater<CreatureLightToggleController.Instance>.BatchUpdateDelegate((object) CreatureLightToggleController.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CInitializeStates\u003Eb__5_5)))).Transition(this.light_on, (StateMachine<CreatureLightToggleController, CreatureLightToggleController.Instance, IStateMachineTarget, CreatureLightToggleController.Def>.Transition.ConditionCallback) (smi => smi.IsOn()));
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<CreatureLightToggleController, CreatureLightToggleController.Instance, IStateMachineTarget, CreatureLightToggleController.Def>.GameInstance
  {
    private const float DIM_TIME = 25f;
    private const float GLOW_TIME = 15f;
    private int originalLux;
    private float originalRange;
    private Light2D light;
    private static WorkItemCollection<CreatureLightToggleController.Instance.ModifyBrightnessTask, object> modify_brightness_job = new WorkItemCollection<CreatureLightToggleController.Instance.ModifyBrightnessTask, object>();
    public static CreatureLightToggleController.Instance.ModifyLuxDelegate dim = (CreatureLightToggleController.Instance.ModifyLuxDelegate) ((instance, time_delta) =>
    {
      float num = (float) instance.originalLux / 25f;
      instance.light.Lux = Mathf.FloorToInt(Mathf.Max(0.0f, (float) instance.light.Lux - num * time_delta));
    });
    public static CreatureLightToggleController.Instance.ModifyLuxDelegate brighten = (CreatureLightToggleController.Instance.ModifyLuxDelegate) ((instance, time_delta) =>
    {
      float num = (float) instance.originalLux / 15f;
      instance.light.Lux = Mathf.CeilToInt(Mathf.Min((float) instance.originalLux, (float) instance.light.Lux + num * time_delta));
    });

    public Instance(IStateMachineTarget master, CreatureLightToggleController.Def def)
      : base(master, def)
    {
      this.light = master.GetComponent<Light2D>();
      this.originalLux = this.light.Lux;
      this.originalRange = this.light.Range;
    }

    public void SwitchLight(bool on) => ((Behaviour) this.light).enabled = on;

    public static void ModifyBrightness(
      List<UpdateBucketWithUpdater<CreatureLightToggleController.Instance>.Entry> instances,
      CreatureLightToggleController.Instance.ModifyLuxDelegate modify_lux,
      float time_delta)
    {
      CreatureLightToggleController.Instance.modify_brightness_job.Reset((object) null);
      for (int index = 0; index != instances.Count; ++index)
      {
        UpdateBucketWithUpdater<CreatureLightToggleController.Instance>.Entry instance = instances[index];
        instance.lastUpdateTime = 0.0f;
        instances[index] = instance;
        CreatureLightToggleController.Instance data = instance.data;
        modify_lux(data, time_delta);
        data.light.Range = data.originalRange * (float) data.light.Lux / (float) data.originalLux;
        int num = (int) data.light.RefreshShapeAndPosition();
        if (data.light.RefreshShapeAndPosition() != Light2D.RefreshResult.None)
          CreatureLightToggleController.Instance.modify_brightness_job.Add(new CreatureLightToggleController.Instance.ModifyBrightnessTask(data.light.emitter));
      }
      GlobalJobManager.Run((IWorkItemCollection) CreatureLightToggleController.Instance.modify_brightness_job);
      for (int index = 0; index != CreatureLightToggleController.Instance.modify_brightness_job.Count; ++index)
        CreatureLightToggleController.Instance.modify_brightness_job.GetWorkItem(index).Finish();
      CreatureLightToggleController.Instance.modify_brightness_job.Reset((object) null);
    }

    public bool IsOff() => this.light.Lux == 0;

    public bool IsOn() => this.light.Lux >= this.originalLux;

    private struct ModifyBrightnessTask : IWorkItem<object>
    {
      private LightGridManager.LightGridEmitter emitter;

      public ModifyBrightnessTask(LightGridManager.LightGridEmitter emitter)
      {
        this.emitter = emitter;
        emitter.RemoveFromGrid();
      }

      public void Run(object context) => this.emitter.UpdateLitCells();

      public void Finish() => this.emitter.AddToGrid(false);
    }

    public delegate void ModifyLuxDelegate(
      CreatureLightToggleController.Instance instance,
      float time_delta);
  }
}
