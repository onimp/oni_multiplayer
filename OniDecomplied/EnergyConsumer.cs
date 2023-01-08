// Decompiled with JetBrains decompiler
// Type: EnergyConsumer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using FMODUnity;
using KSerialization;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[SerializationConfig]
[DebuggerDisplay("{name} {WattsUsed}W")]
[AddComponentMenu("KMonoBehaviour/scripts/EnergyConsumer")]
public class EnergyConsumer : 
  KMonoBehaviour,
  ISaveLoadable,
  IEnergyConsumer,
  ICircuitConnected,
  IGameObjectEffectDescriptor
{
  [MyCmpReq]
  private Building building;
  [MyCmpGet]
  protected Operational operational;
  [MyCmpGet]
  private KSelectable selectable;
  [SerializeField]
  public int powerSortOrder;
  [Serialize]
  protected float circuitOverloadTime;
  public static readonly Operational.Flag PoweredFlag = new Operational.Flag("powered", Operational.Flag.Type.Requirement);
  private Dictionary<string, float> lastTimeSoundPlayed = new Dictionary<string, float>();
  private float soundDecayTime = 10f;
  private float _BaseWattageRating;

  public int PowerSortOrder => this.powerSortOrder;

  public int PowerCell { get; private set; }

  public bool HasWire => Object.op_Inequality((Object) Grid.Objects[this.PowerCell, 26], (Object) null);

  public virtual bool IsPowered
  {
    get => this.operational.GetFlag(EnergyConsumer.PoweredFlag);
    protected set => this.operational.SetFlag(EnergyConsumer.PoweredFlag, value);
  }

  public bool IsConnected => this.CircuitID != ushort.MaxValue;

  public string Name => this.selectable.GetName();

  public bool IsVirtual { get; private set; }

  public object VirtualCircuitKey { get; private set; }

  public ushort CircuitID { get; private set; }

  public float BaseWattageRating
  {
    get => this._BaseWattageRating;
    set => this._BaseWattageRating = value;
  }

  public float WattsUsed => this.operational.IsActive ? this.BaseWattageRating : 0.0f;

  public float WattsNeededWhenActive => this.building.Def.EnergyConsumptionWhenActive;

  protected virtual void OnPrefabInit()
  {
    this.CircuitID = ushort.MaxValue;
    this.IsPowered = false;
    this.BaseWattageRating = this.building.Def.EnergyConsumptionWhenActive;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    Components.EnergyConsumers.Add(this);
    this.PowerCell = ((Component) this).GetComponent<Building>().GetPowerInputCell();
    Game.Instance.circuitManager.Connect((IEnergyConsumer) this);
    Game.Instance.energySim.AddEnergyConsumer(this);
  }

  protected virtual void OnCleanUp()
  {
    Game.Instance.energySim.RemoveEnergyConsumer(this);
    Game.Instance.circuitManager.Disconnect((IEnergyConsumer) this, true);
    Components.EnergyConsumers.Remove(this);
    base.OnCleanUp();
  }

  public virtual void EnergySim200ms(float dt)
  {
    this.CircuitID = Game.Instance.circuitManager.GetCircuitID((ICircuitConnected) this);
    if (!this.IsConnected)
      this.IsPowered = false;
    this.circuitOverloadTime = Mathf.Max(0.0f, this.circuitOverloadTime - dt);
  }

  public virtual void SetConnectionStatus(CircuitManager.ConnectionStatus connection_status)
  {
    switch (connection_status)
    {
      case CircuitManager.ConnectionStatus.NotConnected:
        this.IsPowered = false;
        break;
      case CircuitManager.ConnectionStatus.Unpowered:
        if (!this.IsPowered || !Object.op_Equality((Object) ((Component) this).GetComponent<Battery>(), (Object) null))
          break;
        this.IsPowered = false;
        this.circuitOverloadTime = 6f;
        this.PlayCircuitSound("overdraw");
        break;
      case CircuitManager.ConnectionStatus.Powered:
        if (this.IsPowered || (double) this.circuitOverloadTime > 0.0)
          break;
        this.IsPowered = true;
        this.PlayCircuitSound("powered");
        break;
    }
  }

  protected void PlayCircuitSound(string state)
  {
    EventReference eventReference;
    switch (state)
    {
      case "powered":
        eventReference = Sounds.Instance.BuildingPowerOnMigrated;
        break;
      case "overdraw":
        eventReference = Sounds.Instance.ElectricGridOverloadMigrated;
        break;
      default:
        eventReference = new EventReference();
        Debug.Log((object) "Invalid state for sound in EnergyConsumer.");
        break;
    }
    if (!CameraController.Instance.IsAudibleSound(Vector2.op_Implicit(TransformExtensions.GetPosition(this.transform))))
      return;
    float num1;
    if (!this.lastTimeSoundPlayed.TryGetValue(state, out num1))
      num1 = 0.0f;
    float num2 = (Time.time - num1) / this.soundDecayTime;
    Vector3 position = TransformExtensions.GetPosition(this.transform);
    position.z = 0.0f;
    EventInstance eventInstance = KFMOD.BeginOneShot(eventReference, CameraController.Instance.GetVerticallyScaledPosition(position), 1f);
    ((EventInstance) ref eventInstance).setParameterByName("timeSinceLast", num2, false);
    KFMOD.EndOneShot(eventInstance);
    this.lastTimeSoundPlayed[state] = Time.time;
  }

  public List<Descriptor> GetDescriptors(GameObject go) => (List<Descriptor>) null;
}
