// Decompiled with JetBrains decompiler
// Type: Vent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/Vent")]
public class Vent : KMonoBehaviour, IGameObjectEffectDescriptor
{
  private int cell = -1;
  private int sortKey;
  [Serialize]
  public Dictionary<SimHashes, float> lifeTimeVentMass = new Dictionary<SimHashes, float>();
  private Vent.StatesInstance smi;
  [SerializeField]
  public ConduitType conduitType = ConduitType.Gas;
  [SerializeField]
  public Endpoint endpointType;
  [SerializeField]
  public float overpressureMass = 1f;
  [NonSerialized]
  public bool showConnectivityIcons = true;
  [MyCmpGet]
  [NonSerialized]
  public Structure structure;
  [MyCmpGet]
  [NonSerialized]
  public Operational operational;

  public int SortKey
  {
    get => this.sortKey;
    set => this.sortKey = value;
  }

  public void UpdateVentedMass(SimHashes element, float mass)
  {
    if (!this.lifeTimeVentMass.ContainsKey(element))
      this.lifeTimeVentMass.Add(element, mass);
    else
      this.lifeTimeVentMass[element] += mass;
  }

  public float GetVentedMass(SimHashes element) => this.lifeTimeVentMass.ContainsKey(element) ? this.lifeTimeVentMass[element] : 0.0f;

  public bool Closed()
  {
    bool flag = false;
    return this.operational.Flags.TryGetValue(LogicOperationalController.LogicOperationalFlag, out flag) && !flag || this.operational.Flags.TryGetValue(BuildingEnabledButton.EnabledFlag, out flag) && !flag;
  }

  protected virtual void OnSpawn()
  {
    this.cell = ((Component) this).GetComponent<Building>().GetUtilityOutputCell();
    this.smi = new Vent.StatesInstance(this);
    this.smi.StartSM();
  }

  public Vent.State GetEndPointState()
  {
    Vent.State endPointState = Vent.State.Invalid;
    switch (this.endpointType)
    {
      case Endpoint.Source:
        endPointState = this.IsConnected() ? Vent.State.Ready : Vent.State.Blocked;
        break;
      case Endpoint.Sink:
        endPointState = Vent.State.Ready;
        int cell = this.cell;
        if (!this.IsValidOutputCell(cell))
        {
          endPointState = Grid.Solid[cell] ? Vent.State.Blocked : Vent.State.OverPressure;
          break;
        }
        break;
    }
    return endPointState;
  }

  public bool IsConnected()
  {
    UtilityNetwork networkForCell = Conduit.GetNetworkManager(this.conduitType).GetNetworkForCell(this.cell);
    return networkForCell != null && (networkForCell as FlowUtilityNetwork).HasSinks;
  }

  public bool IsBlocked => this.GetEndPointState() != Vent.State.Ready;

  private bool IsValidOutputCell(int output_cell)
  {
    bool flag = false;
    if ((Object.op_Equality((Object) this.structure, (Object) null) || !this.structure.IsEntombed() || !this.Closed()) && !Grid.Solid[output_cell])
      flag = (double) Grid.Mass[output_cell] < (double) this.overpressureMass;
    return flag;
  }

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    string formattedMass = GameUtil.GetFormattedMass(this.overpressureMass);
    List<Descriptor> descriptors = new List<Descriptor>();
    descriptors.Add(new Descriptor(string.Format((string) UI.BUILDINGEFFECTS.OVER_PRESSURE_MASS, (object) formattedMass), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.OVER_PRESSURE_MASS, (object) formattedMass), (Descriptor.DescriptorType) 1, false));
    return descriptors;
  }

  public enum State
  {
    Invalid,
    Ready,
    Blocked,
    OverPressure,
    Closed,
  }

  public class StatesInstance : 
    GameStateMachine<Vent.States, Vent.StatesInstance, Vent, object>.GameInstance
  {
    private Exhaust exhaust;

    public StatesInstance(Vent master)
      : base(master)
    {
      this.exhaust = ((Component) master).GetComponent<Exhaust>();
    }

    public bool NeedsExhaust() => Object.op_Inequality((Object) this.exhaust, (Object) null) && this.master.GetEndPointState() != Vent.State.Ready && this.master.endpointType == Endpoint.Source;

    public bool Blocked() => this.master.GetEndPointState() == Vent.State.Blocked && this.master.endpointType != 0;

    public bool OverPressure() => Object.op_Inequality((Object) this.exhaust, (Object) null) && this.master.GetEndPointState() == Vent.State.OverPressure && this.master.endpointType != 0;

    public void CheckTransitions()
    {
      if (this.NeedsExhaust())
        this.smi.GoTo((StateMachine.BaseState) this.sm.needExhaust);
      else if (this.master.Closed())
        this.smi.GoTo((StateMachine.BaseState) this.sm.closed);
      else if (this.Blocked())
        this.smi.GoTo((StateMachine.BaseState) this.sm.open.blocked);
      else if (this.OverPressure())
        this.smi.GoTo((StateMachine.BaseState) this.sm.open.overPressure);
      else
        this.smi.GoTo((StateMachine.BaseState) this.sm.open.idle);
    }

    public StatusItem SelectStatusItem(StatusItem gas_status_item, StatusItem liquid_status_item) => this.master.conduitType != ConduitType.Gas ? liquid_status_item : gas_status_item;
  }

  public class States : GameStateMachine<Vent.States, Vent.StatesInstance, Vent>
  {
    public Vent.States.OpenState open;
    public GameStateMachine<Vent.States, Vent.StatesInstance, Vent, object>.State closed;
    public GameStateMachine<Vent.States, Vent.StatesInstance, Vent, object>.State needExhaust;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.open.idle;
      this.root.Update("CheckTransitions", (Action<Vent.StatesInstance, float>) ((smi, dt) => smi.CheckTransitions()));
      this.open.TriggerOnEnter(GameHashes.VentOpen);
      this.closed.TriggerOnEnter(GameHashes.VentClosed);
      this.open.blocked.ToggleStatusItem((Func<Vent.StatesInstance, StatusItem>) (smi => smi.SelectStatusItem(Db.Get().BuildingStatusItems.GasVentObstructed, Db.Get().BuildingStatusItems.LiquidVentObstructed)));
      this.open.overPressure.ToggleStatusItem((Func<Vent.StatesInstance, StatusItem>) (smi => smi.SelectStatusItem(Db.Get().BuildingStatusItems.GasVentOverPressure, Db.Get().BuildingStatusItems.LiquidVentOverPressure)));
    }

    public class OpenState : GameStateMachine<Vent.States, Vent.StatesInstance, Vent, object>.State
    {
      public GameStateMachine<Vent.States, Vent.StatesInstance, Vent, object>.State idle;
      public GameStateMachine<Vent.States, Vent.StatesInstance, Vent, object>.State blocked;
      public GameStateMachine<Vent.States, Vent.StatesInstance, Vent, object>.State overPressure;
    }
  }
}
