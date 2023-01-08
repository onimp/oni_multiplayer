// Decompiled with JetBrains decompiler
// Type: Stinky
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using TUNING;
using UnityEngine;

[SkipSaveFileSerialization]
public class Stinky : StateMachineComponent<Stinky.StatesInstance>
{
  private const float EmitMass = 0.00250000018f;
  private const SimHashes EmitElement = SimHashes.ContaminatedOxygen;
  private const float EmissionRadius = 1.5f;
  private const float MaxDistanceSq = 2.25f;
  private KBatchedAnimController stinkyController;
  private static readonly HashedString[] WorkLoopAnims = new HashedString[3]
  {
    HashedString.op_Implicit("working_pre"),
    HashedString.op_Implicit("working_loop"),
    HashedString.op_Implicit("working_pst")
  };

  protected virtual void OnSpawn() => this.smi.StartSM();

  private void Emit(object data)
  {
    GameObject gameObject = (GameObject) data;
    Components.Cmps<MinionIdentity> minionIdentities = Components.LiveMinionIdentities;
    Vector2 vector2_1 = Vector2.op_Implicit(TransformExtensions.GetPosition(gameObject.transform));
    for (int idx = 0; idx < minionIdentities.Count; ++idx)
    {
      MinionIdentity minionIdentity = minionIdentities[idx];
      if (Object.op_Inequality((Object) ((Component) minionIdentity).gameObject, (Object) gameObject.gameObject))
      {
        Vector2 vector2_2 = Vector2.op_Implicit(TransformExtensions.GetPosition(minionIdentity.transform));
        if ((double) Vector2.SqrMagnitude(Vector2.op_Subtraction(vector2_1, vector2_2)) <= 2.25)
        {
          minionIdentity.Trigger(508119890, (object) Strings.Get("STRINGS.DUPLICANTS.DISEASES.PUTRIDODOUR.CRINGE_EFFECT").String);
          ((Component) minionIdentity).GetComponent<Effects>().Add("SmelledStinky", true);
          ((Component) minionIdentity).gameObject.GetSMI<ThoughtGraph.Instance>().AddThought(Db.Get().Thoughts.PutridOdour);
        }
      }
    }
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(gameObject.transform));
    float num1 = Db.Get().Amounts.Temperature.Lookup((Component) this).value;
    CellAddRemoveSubstanceEvent consumerSimUpdate = CellEventLogger.Instance.ElementConsumerSimUpdate;
    double temperature = (double) num1;
    SimMessages.AddRemoveSubstance(cell, SimHashes.ContaminatedOxygen, consumerSimUpdate, 0.00250000018f, (float) temperature, byte.MaxValue, 0);
    GameObject go = gameObject;
    bool objectIsSelectedAndVisible = SoundEvent.ObjectIsSelectedAndVisible(go);
    Vector3 sound_pos = TransformExtensions.GetPosition(go.GetComponent<Transform>());
    float num2 = 1f;
    if (objectIsSelectedAndVisible)
    {
      sound_pos = SoundEvent.AudioHighlightListenerPosition(sound_pos);
      num2 = SoundEvent.GetVolume(objectIsSelectedAndVisible);
    }
    else
      sound_pos.z = 0.0f;
    KFMOD.PlayOneShot(GlobalAssets.GetSound("Dupe_Flatulence"), sound_pos, num2);
  }

  public class StatesInstance : 
    GameStateMachine<Stinky.States, Stinky.StatesInstance, Stinky, object>.GameInstance
  {
    public StatesInstance(Stinky master)
      : base(master)
    {
    }
  }

  public class States : GameStateMachine<Stinky.States, Stinky.StatesInstance, Stinky>
  {
    public GameStateMachine<Stinky.States, Stinky.StatesInstance, Stinky, object>.State idle;
    public GameStateMachine<Stinky.States, Stinky.StatesInstance, Stinky, object>.State emit;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.idle;
      this.root.TagTransition(GameTags.Dead, (GameStateMachine<Stinky.States, Stinky.StatesInstance, Stinky, object>.State) null).Enter((StateMachine<Stinky.States, Stinky.StatesInstance, Stinky, object>.State.Callback) (smi =>
      {
        KBatchedAnimController effect = FXHelpers.CreateEffect("odor_fx_kanim", TransformExtensions.GetPosition(((Component) smi.master).gameObject.transform), ((Component) smi.master).gameObject.transform, true);
        effect.Play(Stinky.WorkLoopAnims);
        smi.master.stinkyController = effect;
      })).Update("StinkyFX", (Action<Stinky.StatesInstance, float>) ((smi, dt) =>
      {
        if (!Object.op_Inequality((Object) smi.master.stinkyController, (Object) null))
          return;
        smi.master.stinkyController.Play(Stinky.WorkLoopAnims);
      }), (UpdateRate) 7);
      this.idle.Enter("ScheduleNextFart", (StateMachine<Stinky.States, Stinky.StatesInstance, Stinky, object>.State.Callback) (smi => smi.ScheduleGoTo(this.GetNewInterval(), (StateMachine.BaseState) this.emit)));
      this.emit.Enter("Fart", (StateMachine<Stinky.States, Stinky.StatesInstance, Stinky, object>.State.Callback) (smi => smi.master.Emit((object) ((Component) smi.master).gameObject))).ToggleExpression(Db.Get().Expressions.Relief).ScheduleGoTo(3f, (StateMachine.BaseState) this.idle);
    }

    private float GetNewInterval() => Mathf.Min(Mathf.Max(Util.GaussianRandom(TRAITS.STINKY_EMIT_INTERVAL_MAX - TRAITS.STINKY_EMIT_INTERVAL_MIN, 1f), TRAITS.STINKY_EMIT_INTERVAL_MIN), TRAITS.STINKY_EMIT_INTERVAL_MAX);
  }
}
