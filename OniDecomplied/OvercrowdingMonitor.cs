// Decompiled with JetBrains decompiler
// Type: OvercrowdingMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;

public class OvercrowdingMonitor : 
  GameStateMachine<OvercrowdingMonitor, OvercrowdingMonitor.Instance, IStateMachineTarget, OvercrowdingMonitor.Def>
{
  public const float OVERCROWDED_FERTILITY_DEBUFF = -1f;
  public static Effect futureOvercrowdedEffect;
  public static Effect overcrowdedEffect;
  public static Effect fishOvercrowdedEffect;
  public static Effect stuckEffect;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.root;
    this.root.Update(new System.Action<OvercrowdingMonitor.Instance, float>(OvercrowdingMonitor.UpdateState), (UpdateRate) 6, true);
    OvercrowdingMonitor.futureOvercrowdedEffect = new Effect("FutureOvercrowded", (string) CREATURES.MODIFIERS.FUTURE_OVERCROWDED.NAME, (string) CREATURES.MODIFIERS.FUTURE_OVERCROWDED.TOOLTIP, 0.0f, true, false, true);
    OvercrowdingMonitor.futureOvercrowdedEffect.Add(new AttributeModifier(Db.Get().Amounts.Fertility.deltaAttribute.Id, -1f, (string) CREATURES.MODIFIERS.FUTURE_OVERCROWDED.NAME, true));
    OvercrowdingMonitor.overcrowdedEffect = new Effect("Overcrowded", (string) CREATURES.MODIFIERS.OVERCROWDED.NAME, (string) CREATURES.MODIFIERS.OVERCROWDED.TOOLTIP, 0.0f, true, false, true);
    OvercrowdingMonitor.overcrowdedEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -5f, (string) CREATURES.MODIFIERS.OVERCROWDED.NAME));
    OvercrowdingMonitor.fishOvercrowdedEffect = new Effect("Overcrowded", (string) CREATURES.MODIFIERS.OVERCROWDED.NAME, (string) CREATURES.MODIFIERS.OVERCROWDED.FISHTOOLTIP, 0.0f, true, false, true);
    OvercrowdingMonitor.fishOvercrowdedEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -5f, (string) CREATURES.MODIFIERS.OVERCROWDED.NAME));
    OvercrowdingMonitor.stuckEffect = new Effect("Confined", (string) CREATURES.MODIFIERS.CONFINED.NAME, (string) CREATURES.MODIFIERS.CONFINED.TOOLTIP, 0.0f, true, false, true);
    OvercrowdingMonitor.stuckEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -10f, (string) CREATURES.MODIFIERS.CONFINED.NAME));
  }

  private static bool IsConfined(OvercrowdingMonitor.Instance smi) => !smi.HasTag(GameTags.Creatures.Burrowed) && !smi.HasTag(GameTags.Creatures.Digger) && (smi.cavity == null || smi.cavity.numCells < smi.def.spaceRequiredPerCreature);

  private static bool IsFutureOvercrowded(OvercrowdingMonitor.Instance smi)
  {
    if (smi.def.spaceRequiredPerCreature == 0 || smi.cavity == null)
      return false;
    int num = smi.cavity.creatures.Count + smi.cavity.eggs.Count;
    return num != 0 && smi.cavity.eggs.Count != 0 && smi.cavity.numCells / num < smi.def.spaceRequiredPerCreature;
  }

  private static bool IsOvercrowded(OvercrowdingMonitor.Instance smi)
  {
    if (smi.def.spaceRequiredPerCreature == 0)
      return false;
    FishOvercrowdingMonitor.Instance smi1 = smi.GetSMI<FishOvercrowdingMonitor.Instance>();
    if (smi1 != null)
    {
      int fishCount = smi1.fishCount;
      return fishCount > 0 ? smi1.cellCount / fishCount < smi.def.spaceRequiredPerCreature : !Grid.IsLiquid(Grid.PosToCell((StateMachine.Instance) smi));
    }
    return smi.cavity != null && smi.cavity.creatures.Count > 1 && smi.cavity.numCells / smi.cavity.creatures.Count < smi.def.spaceRequiredPerCreature;
  }

  private static void UpdateState(OvercrowdingMonitor.Instance smi, float dt)
  {
    OvercrowdingMonitor.UpdateCavity(smi, dt);
    bool set = OvercrowdingMonitor.IsConfined(smi);
    bool flag1 = OvercrowdingMonitor.IsOvercrowded(smi);
    bool flag2 = !smi.isBaby && OvercrowdingMonitor.IsFutureOvercrowded(smi);
    KPrefabID component = smi.gameObject.GetComponent<KPrefabID>();
    Effect effect = smi.isFish ? OvercrowdingMonitor.fishOvercrowdedEffect : OvercrowdingMonitor.overcrowdedEffect;
    component.SetTag(GameTags.Creatures.Confined, set);
    component.SetTag(GameTags.Creatures.Overcrowded, flag1);
    component.SetTag(GameTags.Creatures.Expecting, flag2);
    OvercrowdingMonitor.SetEffect(smi, OvercrowdingMonitor.stuckEffect, set);
    OvercrowdingMonitor.SetEffect(smi, effect, !set & flag1);
    OvercrowdingMonitor.SetEffect(smi, OvercrowdingMonitor.futureOvercrowdedEffect, !set & flag2);
  }

  private static void SetEffect(OvercrowdingMonitor.Instance smi, Effect effect, bool set)
  {
    Effects component = smi.GetComponent<Effects>();
    if (set)
      component.Add(effect, false);
    else
      component.Remove(effect);
  }

  private static void UpdateCavity(OvercrowdingMonitor.Instance smi, float dt)
  {
    CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell((StateMachine.Instance) smi));
    if (cavityForCell == smi.cavity)
      return;
    KPrefabID component = smi.GetComponent<KPrefabID>();
    if (smi.cavity != null)
    {
      if (smi.HasTag(GameTags.Egg))
        smi.cavity.RemoveFromCavity(component, smi.cavity.eggs);
      else
        smi.cavity.RemoveFromCavity(component, smi.cavity.creatures);
      Game.Instance.roomProber.UpdateRoom(cavityForCell);
    }
    smi.cavity = cavityForCell;
    if (smi.cavity == null)
      return;
    if (smi.HasTag(GameTags.Egg))
      smi.cavity.eggs.Add(component);
    else
      smi.cavity.creatures.Add(component);
    Game.Instance.roomProber.UpdateRoom(smi.cavity);
  }

  public class Def : StateMachine.BaseDef
  {
    public int spaceRequiredPerCreature;
  }

  public new class Instance : 
    GameStateMachine<OvercrowdingMonitor, OvercrowdingMonitor.Instance, IStateMachineTarget, OvercrowdingMonitor.Def>.GameInstance
  {
    public CavityInfo cavity;
    public bool isBaby;
    public bool isFish;

    public Instance(IStateMachineTarget master, OvercrowdingMonitor.Def def)
      : base(master, def)
    {
      this.isBaby = master.gameObject.GetDef<BabyMonitor.Def>() != null;
      this.isFish = master.gameObject.GetDef<FishOvercrowdingMonitor.Def>() != null;
      OvercrowdingMonitor.UpdateState(this, 0.0f);
    }

    protected override void OnCleanUp()
    {
      if (this.cavity == null)
        return;
      KPrefabID component = this.master.GetComponent<KPrefabID>();
      if (this.HasTag(GameTags.Egg))
        this.cavity.RemoveFromCavity(component, this.cavity.eggs);
      else
        this.cavity.RemoveFromCavity(component, this.cavity.creatures);
    }

    public void RoomRefreshUpdateCavity() => OvercrowdingMonitor.UpdateState(this, 0.0f);
  }
}
