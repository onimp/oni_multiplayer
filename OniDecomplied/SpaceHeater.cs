// Decompiled with JetBrains decompiler
// Type: SpaceHeater
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class SpaceHeater : 
  StateMachineComponent<SpaceHeater.StatesInstance>,
  IGameObjectEffectDescriptor
{
  public float targetTemperature = 308.15f;
  public float minimumCellMass;
  public int radius = 2;
  [SerializeField]
  private bool heatLiquid;
  [MyCmpReq]
  private Operational operational;
  private List<int> monitorCells = new List<int>();

  public float TargetTemperature => this.targetTemperature;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    GameScheduler.Instance.Schedule("InsulationTutorial", 2f, (Action<object>) (obj => Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Insulation)), (object) null, (SchedulerGroup) null);
    this.smi.StartSM();
  }

  public void SetLiquidHeater() => this.heatLiquid = true;

  private SpaceHeater.MonitorState MonitorHeating(float dt)
  {
    this.monitorCells.Clear();
    GameUtil.GetNonSolidCells(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)), this.radius, this.monitorCells);
    int num1 = 0;
    float num2 = 0.0f;
    for (int index = 0; index < this.monitorCells.Count; ++index)
    {
      if ((double) Grid.Mass[this.monitorCells[index]] > (double) this.minimumCellMass && (Grid.Element[this.monitorCells[index]].IsGas && !this.heatLiquid || Grid.Element[this.monitorCells[index]].IsLiquid && this.heatLiquid))
      {
        ++num1;
        num2 += Grid.Temperature[this.monitorCells[index]];
      }
    }
    return num1 == 0 ? (!this.heatLiquid ? SpaceHeater.MonitorState.NotEnoughGas : SpaceHeater.MonitorState.NotEnoughLiquid) : ((double) num2 / (double) num1 >= (double) this.targetTemperature ? SpaceHeater.MonitorState.TooHot : SpaceHeater.MonitorState.ReadyToHeat);
  }

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    Descriptor descriptor = new Descriptor();
    ((Descriptor) ref descriptor).SetupDescriptor(string.Format((string) UI.BUILDINGEFFECTS.HEATER_TARGETTEMPERATURE, (object) GameUtil.GetFormattedTemperature(this.targetTemperature)), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.HEATER_TARGETTEMPERATURE, (object) GameUtil.GetFormattedTemperature(this.targetTemperature)), (Descriptor.DescriptorType) 1);
    descriptors.Add(descriptor);
    return descriptors;
  }

  public class StatesInstance : 
    GameStateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.GameInstance
  {
    public StatesInstance(SpaceHeater master)
      : base(master)
    {
    }
  }

  public class States : GameStateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater>
  {
    public GameStateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.State offline;
    public SpaceHeater.States.OnlineStates online;
    private StatusItem statusItemUnderMassLiquid;
    private StatusItem statusItemUnderMassGas;
    private StatusItem statusItemOverTemp;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.offline;
      this.serializable = StateMachine.SerializeType.Never;
      this.statusItemUnderMassLiquid = new StatusItem("statusItemUnderMassLiquid", (string) BUILDING.STATUSITEMS.HEATINGSTALLEDLOWMASS_LIQUID.NAME, (string) BUILDING.STATUSITEMS.HEATINGSTALLEDLOWMASS_LIQUID.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID);
      this.statusItemUnderMassGas = new StatusItem("statusItemUnderMassGas", (string) BUILDING.STATUSITEMS.HEATINGSTALLEDLOWMASS_GAS.NAME, (string) BUILDING.STATUSITEMS.HEATINGSTALLEDLOWMASS_GAS.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID);
      this.statusItemOverTemp = new StatusItem("statusItemOverTemp", (string) BUILDING.STATUSITEMS.HEATINGSTALLEDHOTENV.NAME, (string) BUILDING.STATUSITEMS.HEATINGSTALLEDHOTENV.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID);
      this.statusItemOverTemp.resolveStringCallback = (Func<string, object, string>) ((str, obj) =>
      {
        SpaceHeater.StatesInstance statesInstance = (SpaceHeater.StatesInstance) obj;
        return string.Format(str, (object) GameUtil.GetFormattedTemperature(statesInstance.master.TargetTemperature));
      });
      this.offline.EventTransition(GameHashes.OperationalChanged, (GameStateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.State) this.online, (StateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.Transition.ConditionCallback) (smi => smi.master.operational.IsOperational));
      this.online.EventTransition(GameHashes.OperationalChanged, this.offline, (StateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.Transition.ConditionCallback) (smi => !smi.master.operational.IsOperational)).DefaultState(this.online.heating).Update("spaceheater_online", (Action<SpaceHeater.StatesInstance, float>) ((smi, dt) =>
      {
        switch (smi.master.MonitorHeating(dt))
        {
          case SpaceHeater.MonitorState.ReadyToHeat:
            smi.GoTo((StateMachine.BaseState) this.online.heating);
            break;
          case SpaceHeater.MonitorState.TooHot:
            smi.GoTo((StateMachine.BaseState) this.online.overtemp);
            break;
          case SpaceHeater.MonitorState.NotEnoughLiquid:
            smi.GoTo((StateMachine.BaseState) this.online.undermassliquid);
            break;
          case SpaceHeater.MonitorState.NotEnoughGas:
            smi.GoTo((StateMachine.BaseState) this.online.undermassgas);
            break;
        }
      }), (UpdateRate) 7);
      this.online.heating.Enter((StateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.State.Callback) (smi => smi.master.operational.SetActive(true))).Exit((StateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.State.Callback) (smi => smi.master.operational.SetActive(false)));
      this.online.undermassliquid.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Heat, this.statusItemUnderMassLiquid);
      this.online.undermassgas.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Heat, this.statusItemUnderMassGas);
      this.online.overtemp.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Heat, this.statusItemOverTemp);
    }

    public class OnlineStates : 
      GameStateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.State
    {
      public GameStateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.State heating;
      public GameStateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.State overtemp;
      public GameStateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.State undermassliquid;
      public GameStateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.State undermassgas;
    }
  }

  private enum MonitorState
  {
    ReadyToHeat,
    TooHot,
    NotEnoughLiquid,
    NotEnoughGas,
  }
}
