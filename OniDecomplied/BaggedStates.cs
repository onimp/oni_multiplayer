// Decompiled with JetBrains decompiler
// Type: BaggedStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using UnityEngine;

public class BaggedStates : 
  GameStateMachine<BaggedStates, BaggedStates.Instance, IStateMachineTarget, BaggedStates.Def>
{
  public GameStateMachine<BaggedStates, BaggedStates.Instance, IStateMachineTarget, BaggedStates.Def>.State bagged;
  public GameStateMachine<BaggedStates, BaggedStates.Instance, IStateMachineTarget, BaggedStates.Def>.State escape;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.bagged;
    this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
    this.root.ToggleStatusItem((string) CREATURES.STATUSITEMS.BAGGED.NAME, (string) CREATURES.STATUSITEMS.BAGGED.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main);
    this.bagged.Enter(new StateMachine<BaggedStates, BaggedStates.Instance, IStateMachineTarget, BaggedStates.Def>.State.Callback(BaggedStates.BagStart)).ToggleTag(GameTags.Creatures.Deliverable).PlayAnim("trussed", (KAnim.PlayMode) 0).TagTransition(GameTags.Creatures.Bagged, (GameStateMachine<BaggedStates, BaggedStates.Instance, IStateMachineTarget, BaggedStates.Def>.State) null, true).Transition(this.escape, new StateMachine<BaggedStates, BaggedStates.Instance, IStateMachineTarget, BaggedStates.Def>.Transition.ConditionCallback(BaggedStates.ShouldEscape), (UpdateRate) 7).EventHandler(GameHashes.OnStore, new StateMachine<BaggedStates, BaggedStates.Instance, IStateMachineTarget, BaggedStates.Def>.State.Callback(BaggedStates.OnStore)).Exit(new StateMachine<BaggedStates, BaggedStates.Instance, IStateMachineTarget, BaggedStates.Def>.State.Callback(BaggedStates.BagEnd));
    this.escape.Enter(new StateMachine<BaggedStates, BaggedStates.Instance, IStateMachineTarget, BaggedStates.Def>.State.Callback(BaggedStates.Unbag)).PlayAnim("escape").OnAnimQueueComplete((GameStateMachine<BaggedStates, BaggedStates.Instance, IStateMachineTarget, BaggedStates.Def>.State) null);
  }

  private static void BagStart(BaggedStates.Instance smi)
  {
    if ((double) smi.baggedTime == 0.0)
      smi.baggedTime = GameClock.Instance.GetTime();
    smi.UpdateFaller(true);
  }

  private static void BagEnd(BaggedStates.Instance smi)
  {
    smi.baggedTime = 0.0f;
    smi.UpdateFaller(false);
  }

  private static void Unbag(BaggedStates.Instance smi)
  {
    Baggable component = smi.gameObject.GetComponent<Baggable>();
    if (!Object.op_Implicit((Object) component))
      return;
    component.Free();
  }

  private static void OnStore(BaggedStates.Instance smi) => smi.UpdateFaller(true);

  private static bool ShouldEscape(BaggedStates.Instance smi) => !smi.gameObject.HasTag(GameTags.Stored) && (double) GameClock.Instance.GetTime() - (double) smi.baggedTime >= (double) smi.def.escapeTime;

  public class Def : StateMachine.BaseDef
  {
    public float escapeTime = 300f;
  }

  public new class Instance : 
    GameStateMachine<BaggedStates, BaggedStates.Instance, IStateMachineTarget, BaggedStates.Def>.GameInstance
  {
    [Serialize]
    public float baggedTime;
    public static readonly Chore.Precondition IsBagged = new Chore.Precondition()
    {
      id = nameof (IsBagged),
      fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) => context.consumerState.prefabid.HasTag(GameTags.Creatures.Bagged))
    };

    public Instance(Chore<BaggedStates.Instance> chore, BaggedStates.Def def)
      : base((IStateMachineTarget) chore, def)
    {
      chore.AddPrecondition(BaggedStates.Instance.IsBagged);
    }

    public void UpdateFaller(bool bagged)
    {
      bool flag1 = bagged && !this.gameObject.HasTag(GameTags.Stored);
      bool flag2 = ((KComponentManager<FallerComponent>) GameComps.Fallers).Has((object) this.gameObject);
      if (flag1 == flag2)
        return;
      if (flag1)
        GameComps.Fallers.Add(this.gameObject, Vector2.zero);
      else
        ((KGameObjectComponentManager<FallerComponent>) GameComps.Fallers).Remove(this.gameObject);
    }
  }
}
