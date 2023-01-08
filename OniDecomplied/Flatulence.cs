// Decompiled with JetBrains decompiler
// Type: Flatulence
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

[SkipSaveFileSerialization]
public class Flatulence : StateMachineComponent<Flatulence.StatesInstance>
{
  private const float EmitMass = 0.1f;
  private const SimHashes EmitElement = SimHashes.Methane;
  private const float EmissionRadius = 1.5f;
  private const float MaxDistanceSq = 2.25f;
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
    float temperature = Db.Get().Amounts.Temperature.Lookup((Component) this).value;
    Equippable equippable = ((Component) this).GetComponent<SuitEquipper>().IsWearingAirtightSuit();
    if (Object.op_Inequality((Object) equippable, (Object) null))
    {
      ((Component) equippable).GetComponent<Storage>().AddGasChunk(SimHashes.Methane, 0.1f, temperature, byte.MaxValue, 0, false);
    }
    else
    {
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
            ((Component) minionIdentity).gameObject.GetSMI<ThoughtGraph.Instance>().AddThought(Db.Get().Thoughts.PutridOdour);
          }
        }
      }
      SimMessages.AddRemoveSubstance(Grid.PosToCell(TransformExtensions.GetPosition(gameObject.transform)), SimHashes.Methane, CellEventLogger.Instance.ElementConsumerSimUpdate, 0.1f, temperature, byte.MaxValue, 0);
      KBatchedAnimController effect = FXHelpers.CreateEffect("odor_fx_kanim", TransformExtensions.GetPosition(gameObject.transform), gameObject.transform, true);
      effect.Play(Flatulence.WorkLoopAnims);
      effect.destroyOnAnimComplete = true;
    }
    GameObject go = gameObject;
    bool objectIsSelectedAndVisible = SoundEvent.ObjectIsSelectedAndVisible(go);
    Vector3 sound_pos = TransformExtensions.GetPosition(go.GetComponent<Transform>());
    sound_pos.z = 0.0f;
    float num = 1f;
    if (objectIsSelectedAndVisible)
    {
      sound_pos = SoundEvent.AudioHighlightListenerPosition(sound_pos);
      num = SoundEvent.GetVolume(objectIsSelectedAndVisible);
    }
    else
      sound_pos.z = 0.0f;
    KFMOD.PlayOneShot(GlobalAssets.GetSound("Dupe_Flatulence"), sound_pos, num);
  }

  public class StatesInstance : 
    GameStateMachine<Flatulence.States, Flatulence.StatesInstance, Flatulence, object>.GameInstance
  {
    public StatesInstance(Flatulence master)
      : base(master)
    {
    }
  }

  public class States : GameStateMachine<Flatulence.States, Flatulence.StatesInstance, Flatulence>
  {
    public GameStateMachine<Flatulence.States, Flatulence.StatesInstance, Flatulence, object>.State idle;
    public GameStateMachine<Flatulence.States, Flatulence.StatesInstance, Flatulence, object>.State emit;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.idle;
      this.root.TagTransition(GameTags.Dead, (GameStateMachine<Flatulence.States, Flatulence.StatesInstance, Flatulence, object>.State) null);
      this.idle.Enter("ScheduleNextFart", (StateMachine<Flatulence.States, Flatulence.StatesInstance, Flatulence, object>.State.Callback) (smi => smi.ScheduleGoTo(this.GetNewInterval(), (StateMachine.BaseState) this.emit)));
      this.emit.Enter("Fart", (StateMachine<Flatulence.States, Flatulence.StatesInstance, Flatulence, object>.State.Callback) (smi => smi.master.Emit((object) ((Component) smi.master).gameObject))).ToggleExpression(Db.Get().Expressions.Relief).ScheduleGoTo(3f, (StateMachine.BaseState) this.idle);
    }

    private float GetNewInterval() => Mathf.Min(Mathf.Max(Util.GaussianRandom(TRAITS.FLATULENCE_EMIT_INTERVAL_MAX - TRAITS.FLATULENCE_EMIT_INTERVAL_MIN, 1f), TRAITS.FLATULENCE_EMIT_INTERVAL_MIN), TRAITS.FLATULENCE_EMIT_INTERVAL_MAX);
  }
}
