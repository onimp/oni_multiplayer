// Decompiled with JetBrains decompiler
// Type: RotPile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RotPile : StateMachineComponent<RotPile.StatesInstance>
{
  private Notification notification;

  protected virtual void OnPrefabInit() => base.OnPrefabInit();

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
  }

  protected void ConvertToElement()
  {
    PrimaryElement component = ((Component) this.smi.master).GetComponent<PrimaryElement>();
    float mass = component.Mass;
    float temperature = component.Temperature;
    if ((double) mass <= 0.0)
    {
      Util.KDestroyGameObject(((Component) this).gameObject);
    }
    else
    {
      SimHashes hash = SimHashes.ToxicSand;
      GameObject gameObject = ElementLoader.FindElementByHash(hash).substance.SpawnResource(TransformExtensions.GetPosition(this.smi.master.transform), mass, temperature, byte.MaxValue, 0);
      PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, ElementLoader.FindElementByHash(hash).name, gameObject.transform);
      Util.KDestroyGameObject(this.smi.gameObject);
    }
  }

  private static string OnRottenTooltip(List<Notification> notifications, object data)
  {
    string str = "";
    foreach (Notification notification in notifications)
    {
      if (notification.tooltipData != null)
        str = str + "\n• " + (string) notification.tooltipData + " ";
    }
    return string.Format((string) MISC.NOTIFICATIONS.FOODROT.TOOLTIP, (object) str);
  }

  public void TryClearNotification()
  {
    if (this.notification == null)
      return;
    ((Component) this).gameObject.AddOrGet<Notifier>().Remove(this.notification);
  }

  public void TryCreateNotification()
  {
    WorldContainer myWorld = this.smi.master.GetMyWorld();
    if (!Object.op_Inequality((Object) myWorld, (Object) null) || !myWorld.worldInventory.IsReachable(((Component) this.smi.master).gameObject.GetComponent<Pickupable>()))
      return;
    this.notification = new Notification((string) MISC.NOTIFICATIONS.FOODROT.NAME, NotificationType.BadMinor, new Func<List<Notification>, object, string>(RotPile.OnRottenTooltip));
    this.notification.tooltipData = (object) ((Component) this.smi.master).gameObject.GetProperName();
    ((Component) this).gameObject.AddOrGet<Notifier>().Add(this.notification);
  }

  public class StatesInstance : 
    GameStateMachine<RotPile.States, RotPile.StatesInstance, RotPile, object>.GameInstance
  {
    public AttributeModifier baseDecomposeRate;

    public StatesInstance(RotPile master)
      : base(master)
    {
    }
  }

  public class States : GameStateMachine<RotPile.States, RotPile.StatesInstance, RotPile>
  {
    public GameStateMachine<RotPile.States, RotPile.StatesInstance, RotPile, object>.State decomposing;
    public GameStateMachine<RotPile.States, RotPile.StatesInstance, RotPile, object>.State convertDestroy;
    public StateMachine<RotPile.States, RotPile.StatesInstance, RotPile, object>.FloatParameter decompositionAmount;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.decomposing;
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      double num;
      this.decomposing.Enter((StateMachine<RotPile.States, RotPile.StatesInstance, RotPile, object>.State.Callback) (smi => smi.master.TryCreateNotification())).Exit((StateMachine<RotPile.States, RotPile.StatesInstance, RotPile, object>.State.Callback) (smi => smi.master.TryClearNotification())).ParamTransition<float>((StateMachine<RotPile.States, RotPile.StatesInstance, RotPile, object>.Parameter<float>) this.decompositionAmount, this.convertDestroy, (StateMachine<RotPile.States, RotPile.StatesInstance, RotPile, object>.Parameter<float>.Callback) ((smi, p) => (double) p >= 600.0)).Update("Decomposing", (Action<RotPile.StatesInstance, float>) ((smi, dt) => num = (double) this.decompositionAmount.Delta(dt, smi)));
      this.convertDestroy.Enter((StateMachine<RotPile.States, RotPile.StatesInstance, RotPile, object>.State.Callback) (smi => smi.master.ConvertToElement()));
    }
  }
}
