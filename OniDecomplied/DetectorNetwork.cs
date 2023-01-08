// Decompiled with JetBrains decompiler
// Type: DetectorNetwork
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DetectorNetwork : 
  GameStateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>
{
  public StateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>.FloatParameter selfQuality;
  public StateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>.FloatParameter networkQuality;
  public GameStateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>.State inoperational;
  public DetectorNetwork.SelfStates operational;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.inoperational;
    this.inoperational.EventTransition(GameHashes.OperationalChanged, (GameStateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>.State) this.operational, (StateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>.Transition.ConditionCallback) (smi => smi.GetComponent<Operational>().IsOperational));
    this.operational.DefaultState(this.operational.self_poor.poor).Update("CheckForInterference", (System.Action<DetectorNetwork.Instance, float>) ((smi, dt) => smi.Update(dt)), (UpdateRate) 6).EventTransition(GameHashes.OperationalChanged, this.inoperational, (StateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>.Transition.ConditionCallback) (smi => !smi.GetComponent<Operational>().IsOperational));
    this.operational.self_poor.InitializeStates(this).ToggleStatusItem((string) BUILDING.STATUSITEMS.DETECTORQUALITY.NAME, (string) BUILDING.STATUSITEMS.DETECTORQUALITY.TOOLTIP, "status_item_interference", StatusItem.IconType.Custom, NotificationType.BadMinor, false, new HashedString(), 129022, (Func<string, DetectorNetwork.Instance, string>) ((str, smi) => str.Replace("{Quality}", GameUtil.GetFormattedPercent(smi.GetDishQuality() * 100f))), (Func<string, DetectorNetwork.Instance, string>) null, (StatusItemCategory) null).ParamTransition<float>((StateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>.Parameter<float>) this.selfQuality, (GameStateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>.State) this.operational.self_good, (StateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>.Parameter<float>.Callback) ((smi, p) => (double) p >= 0.8));
    this.operational.self_good.InitializeStates(this).ToggleStatusItem((string) BUILDING.STATUSITEMS.DETECTORQUALITY.NAME, (string) BUILDING.STATUSITEMS.DETECTORQUALITY.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, new HashedString(), 129022, (Func<string, DetectorNetwork.Instance, string>) ((str, smi) => str.Replace("{Quality}", GameUtil.GetFormattedPercent(smi.GetDishQuality() * 100f))), (Func<string, DetectorNetwork.Instance, string>) null, (StatusItemCategory) null).ParamTransition<float>((StateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>.Parameter<float>) this.selfQuality, (GameStateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>.State) this.operational.self_poor, (StateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>.Parameter<float>.Callback) ((smi, p) => (double) p < 0.8));
  }

  public class Def : StateMachine.BaseDef
  {
    public int interferenceRadius;
    public float worstWarningTime;
    public float bestWarningTime;
    public int bestNetworkSize;
  }

  public class SelfStates : 
    GameStateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>.State
  {
    public DetectorNetwork.NetworkStates self_poor;
    public DetectorNetwork.NetworkStates self_good;
  }

  public class NetworkStates : 
    GameStateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>.State
  {
    public GameStateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>.State poor;
    public GameStateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>.State good;

    public DetectorNetwork.NetworkStates InitializeStates(DetectorNetwork parent)
    {
      this.DefaultState(this.poor);
      this.poor.ToggleStatusItem((string) BUILDING.STATUSITEMS.NETWORKQUALITY.NAME, (string) BUILDING.STATUSITEMS.NETWORKQUALITY.TOOLTIP, icon_type: StatusItem.IconType.Exclamation, notification_type: NotificationType.BadMinor, render_overlay: new HashedString(), resolve_string_callback: new Func<string, DetectorNetwork.Instance, string>(this.StringCallback)).ParamTransition<float>((StateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>.Parameter<float>) parent.networkQuality, this.good, (StateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>.Parameter<float>.Callback) ((smi, p) => (double) p >= 0.8));
      this.good.ToggleStatusItem((string) BUILDING.STATUSITEMS.NETWORKQUALITY.NAME, (string) BUILDING.STATUSITEMS.NETWORKQUALITY.TOOLTIP, render_overlay: new HashedString(), resolve_string_callback: new Func<string, DetectorNetwork.Instance, string>(this.StringCallback)).ParamTransition<float>((StateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>.Parameter<float>) parent.networkQuality, this.poor, (StateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>.Parameter<float>.Callback) ((smi, p) => (double) p < 0.8));
      return this;
    }

    private string StringCallback(string str, DetectorNetwork.Instance smi)
    {
      MathUtil.MinMax detectTimeRange = smi.GetDetectTimeRange();
      return str.Replace("{TotalQuality}", GameUtil.GetFormattedPercent(smi.ComputeTotalDishQuality() * 100f)).Replace("{WorstTime}", GameUtil.GetFormattedTime(((MathUtil.MinMax) ref detectTimeRange).min)).Replace("{BestTime}", GameUtil.GetFormattedTime(((MathUtil.MinMax) ref detectTimeRange).max));
    }
  }

  public new class Instance : 
    GameStateMachine<DetectorNetwork, DetectorNetwork.Instance, IStateMachineTarget, DetectorNetwork.Def>.GameInstance
  {
    private float closestMachinery = float.MaxValue;
    private int visibleSkyCells;

    public Instance(IStateMachineTarget master, DetectorNetwork.Def def)
      : base(master, def)
    {
    }

    public override void StartSM()
    {
      Components.DetectorNetworks.Add(this);
      base.StartSM();
    }

    public override void StopSM(string reason)
    {
      base.StopSM(reason);
      Components.DetectorNetworks.Remove(this);
    }

    public void Update(float dt)
    {
      this.CheckForVisibility();
      this.CheckForInterference();
      double num1 = (double) this.sm.selfQuality.Set(this.GetDishQuality(), this.smi);
      double num2 = (double) this.sm.networkQuality.Set(this.ComputeTotalDishQuality(), this.smi);
    }

    private void CheckForVisibility()
    {
      int cell = Grid.PosToCell((StateMachine.Instance) this);
      this.visibleSkyCells = 0 + DetectorNetwork.Instance.ScanVisiblityLine(cell, 1, 1, this.def.interferenceRadius) + DetectorNetwork.Instance.ScanVisiblityLine(cell, -1, 1, this.def.interferenceRadius);
    }

    public static int ScanVisiblityLine(int start_cell, int x_offset, int y_offset, int radius)
    {
      int num1 = 0;
      for (int index = 0; Mathf.Abs(index) <= radius; ++index)
      {
        int num2 = Grid.OffsetCell(start_cell, index * x_offset, index * y_offset);
        if (Grid.IsValidCell(num2))
        {
          if (Grid.ExposedToSunlight[num2] > (byte) 0)
            ++num1;
          else
            break;
        }
      }
      return num1;
    }

    private void CheckForInterference()
    {
      Extents extents = new Extents(Grid.PosToCell((StateMachine.Instance) this), this.def.interferenceRadius);
      List<ScenePartitionerEntry> gathered_entries = new List<ScenePartitionerEntry>();
      GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.industrialBuildings, gathered_entries);
      float num = float.MaxValue;
      foreach (ScenePartitionerEntry partitionerEntry in gathered_entries)
      {
        GameObject gameObject = (GameObject) partitionerEntry.obj;
        if (!Object.op_Equality((Object) gameObject, (Object) this.gameObject))
        {
          Vector3 vector3 = Vector3.op_Subtraction(TransformExtensions.GetPosition(this.gameObject.transform), TransformExtensions.GetPosition(gameObject.transform));
          float magnitude = ((Vector3) ref vector3).magnitude;
          num = Mathf.Min(num, magnitude);
        }
      }
      this.closestMachinery = num;
    }

    public float GetDishQuality() => !this.GetComponent<Operational>().IsOperational ? 0.0f : Mathf.Clamp01(this.closestMachinery / (float) this.def.interferenceRadius) * Mathf.Clamp01((float) this.visibleSkyCells / ((float) this.def.interferenceRadius * 2f));

    public float ComputeTotalDishQuality()
    {
      float num = 0.0f;
      foreach (DetectorNetwork.Instance instance in Components.DetectorNetworks.Items)
        num += instance.GetDishQuality();
      return num / (float) this.def.bestNetworkSize;
    }

    public MathUtil.MinMax GetDetectTimeRange() => new MathUtil.MinMax(Mathf.Lerp(this.def.worstWarningTime, this.def.bestWarningTime, this.ComputeTotalDishQuality()), this.def.bestWarningTime);
  }
}
