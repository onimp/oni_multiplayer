// Decompiled with JetBrains decompiler
// Type: FertilityMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class FertilityMonitor : 
  GameStateMachine<FertilityMonitor, FertilityMonitor.Instance, IStateMachineTarget, FertilityMonitor.Def>
{
  private GameStateMachine<FertilityMonitor, FertilityMonitor.Instance, IStateMachineTarget, FertilityMonitor.Def>.State fertile;
  private GameStateMachine<FertilityMonitor, FertilityMonitor.Instance, IStateMachineTarget, FertilityMonitor.Def>.State infertile;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.fertile;
    this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
    this.root.DefaultState(this.fertile);
    this.fertile.ToggleBehaviour(GameTags.Creatures.Fertile, (StateMachine<FertilityMonitor, FertilityMonitor.Instance, IStateMachineTarget, FertilityMonitor.Def>.Transition.ConditionCallback) (smi => smi.IsReadyToLayEgg())).ToggleEffect((Func<FertilityMonitor.Instance, Effect>) (smi => smi.fertileEffect)).Transition(this.infertile, GameStateMachine<FertilityMonitor, FertilityMonitor.Instance, IStateMachineTarget, FertilityMonitor.Def>.Not(new StateMachine<FertilityMonitor, FertilityMonitor.Instance, IStateMachineTarget, FertilityMonitor.Def>.Transition.ConditionCallback(FertilityMonitor.IsFertile)), (UpdateRate) 6);
    this.infertile.Transition(this.fertile, new StateMachine<FertilityMonitor, FertilityMonitor.Instance, IStateMachineTarget, FertilityMonitor.Def>.Transition.ConditionCallback(FertilityMonitor.IsFertile), (UpdateRate) 6);
  }

  public static bool IsFertile(FertilityMonitor.Instance smi) => !smi.HasTag(GameTags.Creatures.Confined) && !smi.HasTag(GameTags.Creatures.Expecting);

  public static Tag EggBreedingRoll(
    List<FertilityMonitor.BreedingChance> breedingChances,
    bool excludeOriginalCreature = false)
  {
    float num = Random.value;
    if (excludeOriginalCreature)
      num *= 1f - breedingChances[0].weight;
    foreach (FertilityMonitor.BreedingChance breedingChance in breedingChances)
    {
      if (excludeOriginalCreature)
      {
        excludeOriginalCreature = false;
      }
      else
      {
        num -= breedingChance.weight;
        if ((double) num <= 0.0)
          return breedingChance.egg;
      }
    }
    return Tag.Invalid;
  }

  [Serializable]
  public class BreedingChance
  {
    public Tag egg;
    public float weight;
  }

  public class Def : StateMachine.BaseDef
  {
    public Tag eggPrefab;
    public List<FertilityMonitor.BreedingChance> initialBreedingWeights;
    public float baseFertileCycles;

    public override void Configure(GameObject prefab) => prefab.AddOrGet<Modifiers>().initialAmounts.Add(Db.Get().Amounts.Fertility.Id);
  }

  public new class Instance : 
    GameStateMachine<FertilityMonitor, FertilityMonitor.Instance, IStateMachineTarget, FertilityMonitor.Def>.GameInstance
  {
    public AmountInstance fertility;
    private GameObject egg;
    [Serialize]
    public List<FertilityMonitor.BreedingChance> breedingChances;
    public Effect fertileEffect;
    private static HashedString targetEggSymbol = HashedString.op_Implicit("snapto_egg");

    public Instance(IStateMachineTarget master, FertilityMonitor.Def def)
      : base(master, def)
    {
      this.fertility = Db.Get().Amounts.Fertility.Lookup(this.gameObject);
      if (GenericGameSettings.instance.acceleratedLifecycle)
        this.fertility.deltaAttribute.Add(new AttributeModifier(this.fertility.deltaAttribute.Id, 33.3333321f, "Accelerated Lifecycle"));
      float num = (float) (100.0 / ((double) def.baseFertileCycles * 600.0));
      this.fertileEffect = new Effect("Fertile", (string) CREATURES.MODIFIERS.BASE_FERTILITY.NAME, (string) CREATURES.MODIFIERS.BASE_FERTILITY.TOOLTIP, 0.0f, false, false, false);
      this.fertileEffect.Add(new AttributeModifier(Db.Get().Amounts.Fertility.deltaAttribute.Id, num, (string) CREATURES.MODIFIERS.BASE_FERTILITY.NAME));
      this.InitializeBreedingChances();
    }

    [OnDeserialized]
    private void OnDeserialized()
    {
      if (this.breedingChances.Count == (this.def.initialBreedingWeights != null ? this.def.initialBreedingWeights.Count : 0))
        return;
      this.InitializeBreedingChances();
    }

    private void InitializeBreedingChances()
    {
      this.breedingChances = new List<FertilityMonitor.BreedingChance>();
      if (this.def.initialBreedingWeights == null)
        return;
      foreach (FertilityMonitor.BreedingChance initialBreedingWeight in this.def.initialBreedingWeights)
      {
        this.breedingChances.Add(new FertilityMonitor.BreedingChance()
        {
          egg = initialBreedingWeight.egg,
          weight = initialBreedingWeight.weight
        });
        foreach (FertilityModifier fertilityModifier in Db.Get().FertilityModifiers.GetForTag(initialBreedingWeight.egg))
          fertilityModifier.ApplyFunction(this, initialBreedingWeight.egg);
      }
      this.NormalizeBreedingChances();
    }

    public void ShowEgg()
    {
      if (!Object.op_Inequality((Object) this.egg, (Object) null))
        return;
      bool symbolVisible;
      Matrix4x4 symbolTransform = this.GetComponent<KBatchedAnimController>().GetSymbolTransform(FertilityMonitor.Instance.targetEggSymbol, out symbolVisible);
      Vector3 pos = ((Matrix4x4) ref symbolTransform).MultiplyPoint3x4(Vector3.zero);
      if (symbolVisible)
      {
        pos.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
        int cell = Grid.PosToCell(pos);
        if (Grid.IsValidCell(cell) && !Grid.Solid[cell])
          TransformExtensions.SetPosition(this.egg.transform, pos);
      }
      this.egg.SetActive(true);
      Db.Get().Amounts.Wildness.Copy(this.egg, this.gameObject);
      this.egg = (GameObject) null;
    }

    public void LayEgg()
    {
      this.fertility.value = 0.0f;
      Vector3 position = TransformExtensions.GetPosition(this.smi.transform);
      position.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
      Tag tag = FertilityMonitor.EggBreedingRoll(this.breedingChances);
      if (GenericGameSettings.instance.acceleratedLifecycle)
      {
        float num = 0.0f;
        foreach (FertilityMonitor.BreedingChance breedingChance in this.breedingChances)
        {
          if ((double) breedingChance.weight > (double) num)
          {
            num = breedingChance.weight;
            tag = breedingChance.egg;
          }
        }
      }
      Debug.Assert(Tag.op_Inequality(tag, Tag.Invalid), (object) "Didn't pick an egg to lay. Weights weren't normalized?");
      GameObject prefab = Assets.GetPrefab(tag);
      this.egg = Util.KInstantiate(prefab, position);
      SymbolOverrideController component1 = this.GetComponent<SymbolOverrideController>();
      string str = "egg01";
      CreatureBrain component2 = Assets.GetPrefab(prefab.GetDef<IncubationMonitor.Def>().spawnedCreature).GetComponent<CreatureBrain>();
      if (!string.IsNullOrEmpty(component2.symbolPrefix))
        str = component2.symbolPrefix + "egg01";
      KAnim.Build.Symbol symbol = this.egg.GetComponent<KBatchedAnimController>().AnimFiles[0].GetData().build.GetSymbol(KAnimHashedString.op_Implicit(str));
      if (symbol != null)
        component1.AddSymbolOverride(FertilityMonitor.Instance.targetEggSymbol, symbol);
      this.Trigger(1193600993, (object) this.egg);
    }

    public bool IsReadyToLayEgg() => (double) this.smi.fertility.value >= (double) this.smi.fertility.GetMax();

    public void AddBreedingChance(Tag type, float addedPercentChance)
    {
      foreach (FertilityMonitor.BreedingChance breedingChance in this.breedingChances)
      {
        if (Tag.op_Equality(breedingChance.egg, type))
        {
          float num = Mathf.Min(1f - breedingChance.weight, Mathf.Max(0.0f - breedingChance.weight, addedPercentChance));
          breedingChance.weight += num;
        }
      }
      this.NormalizeBreedingChances();
      this.master.Trigger(1059811075, (object) this.breedingChances);
    }

    public float GetBreedingChance(Tag type)
    {
      foreach (FertilityMonitor.BreedingChance breedingChance in this.breedingChances)
      {
        if (Tag.op_Equality(breedingChance.egg, type))
          return breedingChance.weight;
      }
      return -1f;
    }

    public void NormalizeBreedingChances()
    {
      float num = 0.0f;
      foreach (FertilityMonitor.BreedingChance breedingChance in this.breedingChances)
        num += breedingChance.weight;
      foreach (FertilityMonitor.BreedingChance breedingChance in this.breedingChances)
        breedingChance.weight /= num;
    }

    protected override void OnCleanUp()
    {
      base.OnCleanUp();
      if (!Object.op_Inequality((Object) this.egg, (Object) null))
        return;
      Object.Destroy((Object) this.egg);
      this.egg = (GameObject) null;
    }
  }
}
