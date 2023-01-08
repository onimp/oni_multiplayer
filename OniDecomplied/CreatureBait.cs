// Decompiled with JetBrains decompiler
// Type: CreatureBait
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using UnityEngine;

[SerializationConfig]
public class CreatureBait : StateMachineComponent<CreatureBait.StatesInstance>
{
  [Serialize]
  public Tag baitElement;

  protected virtual void OnPrefabInit() => base.OnPrefabInit();

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.baitElement = ((Component) this).GetComponent<Deconstructable>().constructionElements[1];
    ((Component) this).gameObject.GetSMI<Lure.Instance>().SetActiveLures(new Tag[1]
    {
      this.baitElement
    });
    this.smi.StartSM();
  }

  public class StatesInstance : 
    GameStateMachine<CreatureBait.States, CreatureBait.StatesInstance, CreatureBait, object>.GameInstance
  {
    public StatesInstance(CreatureBait master)
      : base(master)
    {
    }
  }

  public class States : 
    GameStateMachine<CreatureBait.States, CreatureBait.StatesInstance, CreatureBait>
  {
    public GameStateMachine<CreatureBait.States, CreatureBait.StatesInstance, CreatureBait, object>.State idle;
    public GameStateMachine<CreatureBait.States, CreatureBait.StatesInstance, CreatureBait, object>.State destroy;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.idle;
      this.idle.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Baited).Enter((StateMachine<CreatureBait.States, CreatureBait.StatesInstance, CreatureBait, object>.State.Callback) (smi =>
      {
        KAnim.Build build = ElementLoader.FindElementByName(smi.master.baitElement.ToString()).substance.anim.GetData().build;
        KAnim.Build.Symbol symbol = build.GetSymbol(new KAnimHashedString(build.name));
        HashedString target_symbol = HashedString.op_Implicit("snapTo_bait");
        smi.GetComponent<SymbolOverrideController>().AddSymbolOverride(target_symbol, symbol);
      })).TagTransition(GameTags.LureUsed, this.destroy);
      this.destroy.PlayAnim("use").EventHandler(GameHashes.AnimQueueComplete, (StateMachine<CreatureBait.States, CreatureBait.StatesInstance, CreatureBait, object>.State.Callback) (smi => Util.KDestroyGameObject(((Component) smi.master).gameObject)));
    }
  }
}
