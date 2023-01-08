// Decompiled with JetBrains decompiler
// Type: DisabledCreatureStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;

public class DisabledCreatureStates : 
  GameStateMachine<DisabledCreatureStates, DisabledCreatureStates.Instance, IStateMachineTarget, DisabledCreatureStates.Def>
{
  public GameStateMachine<DisabledCreatureStates, DisabledCreatureStates.Instance, IStateMachineTarget, DisabledCreatureStates.Def>.State disableCreature;
  public GameStateMachine<DisabledCreatureStates, DisabledCreatureStates.Instance, IStateMachineTarget, DisabledCreatureStates.Def>.State behaviourcomplete;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.disableCreature;
    this.root.ToggleStatusItem((string) CREATURES.STATUSITEMS.DISABLED.NAME, (string) CREATURES.STATUSITEMS.DISABLED.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main).TagTransition(GameTags.Creatures.Behaviours.DisableCreature, this.behaviourcomplete, true);
    this.disableCreature.PlayAnim((Func<DisabledCreatureStates.Instance, string>) (smi => smi.def.disabledAnim));
    this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.Behaviours.DisableCreature);
  }

  public class Def : StateMachine.BaseDef
  {
    public string disabledAnim = "off";

    public Def(string anim) => this.disabledAnim = anim;
  }

  public new class Instance : 
    GameStateMachine<DisabledCreatureStates, DisabledCreatureStates.Instance, IStateMachineTarget, DisabledCreatureStates.Def>.GameInstance
  {
    public Instance(Chore<DisabledCreatureStates.Instance> chore, DisabledCreatureStates.Def def)
      : base((IStateMachineTarget) chore, def)
    {
      chore.AddPrecondition(ChorePreconditions.instance.HasTag, (object) GameTags.Creatures.Behaviours.DisableCreature);
    }
  }
}
