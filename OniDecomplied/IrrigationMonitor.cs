// Decompiled with JetBrains decompiler
// Type: IrrigationMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class IrrigationMonitor : 
  GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>
{
  public StateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.TargetParameter resourceStorage;
  public StateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.BoolParameter hasCorrectLiquid;
  public StateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.BoolParameter hasIncorrectLiquid;
  public StateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.BoolParameter enoughCorrectLiquidToRecover;
  public GameHashes ResourceRecievedEvent = GameHashes.LiquidResourceRecieved;
  public GameHashes ResourceDepletedEvent = GameHashes.LiquidResourceEmpty;
  public GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.State wild;
  public GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.State unfertilizable;
  public IrrigationMonitor.ReplantedStates replanted;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.wild;
    this.serializable = StateMachine.SerializeType.Never;
    this.wild.ParamTransition<GameObject>((StateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.Parameter<GameObject>) this.resourceStorage, this.unfertilizable, (StateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.Parameter<GameObject>.Callback) ((smi, p) => Object.op_Inequality((Object) p, (Object) null)));
    this.unfertilizable.Enter((StateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.State.Callback) (smi =>
    {
      if (!smi.AcceptsLiquid())
        return;
      smi.GoTo((StateMachine.BaseState) this.replanted.irrigated);
    }));
    this.replanted.Enter((StateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.State.Callback) (smi =>
    {
      foreach (ManualDeliveryKG component in smi.gameObject.GetComponents<ManualDeliveryKG>())
        component.Pause(false, "replanted");
      smi.UpdateIrrigation(0.0333333351f);
    })).Target(this.resourceStorage).EventHandler(GameHashes.OnStorageChange, (StateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.State.Callback) (smi => smi.UpdateIrrigation(0.2f))).Target(this.masterTarget);
    this.replanted.irrigated.DefaultState((GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.State) this.replanted.irrigated.absorbing).TriggerOnEnter(this.ResourceRecievedEvent);
    this.replanted.irrigated.absorbing.DefaultState(this.replanted.irrigated.absorbing.normal).ParamTransition<bool>((StateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.Parameter<bool>) this.hasCorrectLiquid, (GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.State) this.replanted.starved, GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.IsFalse).ToggleAttributeModifier("Absorbing", (Func<IrrigationMonitor.Instance, AttributeModifier>) (smi => smi.absorptionRate)).Enter((StateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.State.Callback) (smi => smi.UpdateAbsorbing(true))).EventHandler(GameHashes.TagsChanged, (StateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.State.Callback) (smi => smi.UpdateAbsorbing(true))).Exit((StateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.State.Callback) (smi => smi.UpdateAbsorbing(false)));
    this.replanted.irrigated.absorbing.normal.ParamTransition<bool>((StateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.Parameter<bool>) this.hasIncorrectLiquid, this.replanted.irrigated.absorbing.wrongLiquid, GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.IsTrue);
    this.replanted.irrigated.absorbing.wrongLiquid.ParamTransition<bool>((StateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.Parameter<bool>) this.hasIncorrectLiquid, this.replanted.irrigated.absorbing.normal, GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.IsFalse);
    this.replanted.starved.DefaultState(this.replanted.starved.normal).TriggerOnEnter(this.ResourceDepletedEvent).ParamTransition<bool>((StateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.Parameter<bool>) this.enoughCorrectLiquidToRecover, (GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.State) this.replanted.irrigated.absorbing, (StateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.Parameter<bool>.Callback) ((smi, p) => p && this.hasCorrectLiquid.Get(smi))).ParamTransition<bool>((StateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.Parameter<bool>) this.hasCorrectLiquid, (GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.State) this.replanted.irrigated.absorbing, (StateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.Parameter<bool>.Callback) ((smi, p) => p && this.enoughCorrectLiquidToRecover.Get(smi)));
    this.replanted.starved.normal.ParamTransition<bool>((StateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.Parameter<bool>) this.hasIncorrectLiquid, this.replanted.starved.wrongLiquid, GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.IsTrue);
    this.replanted.starved.wrongLiquid.ParamTransition<bool>((StateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.Parameter<bool>) this.hasIncorrectLiquid, this.replanted.starved.normal, GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.IsFalse);
  }

  public class Def : StateMachine.BaseDef, IGameObjectEffectDescriptor
  {
    public Tag wrongIrrigationTestTag;
    public PlantElementAbsorber.ConsumeInfo[] consumedElements;

    public List<Descriptor> GetDescriptors(GameObject obj)
    {
      if (this.consumedElements.Length == 0)
        return (List<Descriptor>) null;
      List<Descriptor> descriptors = new List<Descriptor>();
      float modifiedAttributeValue = obj.GetComponent<Modifiers>().GetPreModifiedAttributeValue(Db.Get().PlantAttributes.FertilizerUsageMod);
      foreach (PlantElementAbsorber.ConsumeInfo consumedElement in this.consumedElements)
      {
        float mass = consumedElement.massConsumptionRate * modifiedAttributeValue;
        descriptors.Add(new Descriptor(string.Format((string) UI.GAMEOBJECTEFFECTS.IDEAL_FERTILIZER, (object) consumedElement.tag.ProperName(), (object) GameUtil.GetFormattedMass(-mass, GameUtil.TimeSlice.PerCycle)), string.Format((string) UI.GAMEOBJECTEFFECTS.TOOLTIPS.IDEAL_FERTILIZER, (object) consumedElement.tag.ProperName(), (object) GameUtil.GetFormattedMass(mass, GameUtil.TimeSlice.PerCycle)), (Descriptor.DescriptorType) 0, false));
      }
      return descriptors;
    }
  }

  public class VariableIrrigationStates : 
    GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.State
  {
    public GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.State normal;
    public GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.State wrongLiquid;
  }

  public class Irrigated : 
    GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.State
  {
    public IrrigationMonitor.VariableIrrigationStates absorbing;
  }

  public class ReplantedStates : 
    GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.State
  {
    public IrrigationMonitor.Irrigated irrigated;
    public IrrigationMonitor.VariableIrrigationStates starved;
  }

  public new class Instance : 
    GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Def>.GameInstance,
    IWiltCause
  {
    public AttributeModifier consumptionRate;
    public AttributeModifier absorptionRate;
    protected AmountInstance irrigation;
    private float total_available_mass;
    private Storage storage;
    private HandleVector<int>.Handle absorberHandle = HandleVector<int>.InvalidHandle;

    public float total_fertilizer_available => this.total_available_mass;

    public Instance(IStateMachineTarget master, IrrigationMonitor.Def def)
      : base(master, def)
    {
      this.AddAmounts(this.gameObject);
      this.MakeModifiers();
      master.Subscribe(1309017699, new System.Action<object>(this.SetStorage));
    }

    public virtual StatusItem GetStarvedStatusItem() => Db.Get().CreatureStatusItems.NeedsIrrigation;

    public virtual StatusItem GetIncorrectLiquidStatusItem() => Db.Get().CreatureStatusItems.WrongIrrigation;

    public virtual StatusItem GetIncorrectLiquidStatusItemMajor() => Db.Get().CreatureStatusItems.WrongIrrigationMajor;

    protected virtual void AddAmounts(GameObject gameObject) => this.irrigation = gameObject.GetAmounts().Add(new AmountInstance(Db.Get().Amounts.Irrigation, gameObject));

    protected virtual void MakeModifiers()
    {
      this.consumptionRate = new AttributeModifier(Db.Get().Amounts.Irrigation.deltaAttribute.Id, -0.166666672f, (string) CREATURES.STATS.IRRIGATION.CONSUME_MODIFIER);
      this.absorptionRate = new AttributeModifier(Db.Get().Amounts.Irrigation.deltaAttribute.Id, 1.66666663f, (string) CREATURES.STATS.IRRIGATION.ABSORBING_MODIFIER);
    }

    public static void DumpIncorrectFertilizers(Storage storage, GameObject go)
    {
      if (Object.op_Equality((Object) storage, (Object) null) || Object.op_Equality((Object) go, (Object) null))
        return;
      IrrigationMonitor.Instance smi1 = go.GetSMI<IrrigationMonitor.Instance>();
      PlantElementAbsorber.ConsumeInfo[] consumed_infos1 = (PlantElementAbsorber.ConsumeInfo[]) null;
      if (smi1 != null)
        consumed_infos1 = smi1.def.consumedElements;
      IrrigationMonitor.Instance.DumpIncorrectFertilizers(storage, consumed_infos1, false);
      FertilizationMonitor.Instance smi2 = go.GetSMI<FertilizationMonitor.Instance>();
      PlantElementAbsorber.ConsumeInfo[] consumed_infos2 = (PlantElementAbsorber.ConsumeInfo[]) null;
      if (smi2 != null)
        consumed_infos2 = smi2.def.consumedElements;
      IrrigationMonitor.Instance.DumpIncorrectFertilizers(storage, consumed_infos2, true);
    }

    private static void DumpIncorrectFertilizers(
      Storage storage,
      PlantElementAbsorber.ConsumeInfo[] consumed_infos,
      bool validate_solids)
    {
      if (Object.op_Equality((Object) storage, (Object) null))
        return;
      for (int index = storage.items.Count - 1; index >= 0; --index)
      {
        GameObject go = storage.items[index];
        if (!Object.op_Equality((Object) go, (Object) null))
        {
          PrimaryElement component1 = go.GetComponent<PrimaryElement>();
          if (!Object.op_Equality((Object) component1, (Object) null) && !Object.op_Equality((Object) go.GetComponent<ElementChunk>(), (Object) null))
          {
            if (validate_solids)
            {
              if (!component1.Element.IsSolid)
                continue;
            }
            else if (!component1.Element.IsLiquid)
              continue;
            bool flag = false;
            KPrefabID component2 = ((Component) component1).GetComponent<KPrefabID>();
            if (consumed_infos != null)
            {
              foreach (PlantElementAbsorber.ConsumeInfo consumedInfo in consumed_infos)
              {
                if (component2.HasTag(consumedInfo.tag))
                {
                  flag = true;
                  break;
                }
              }
            }
            if (!flag)
              storage.Drop(go, true);
          }
        }
      }
    }

    public void SetStorage(object obj)
    {
      this.storage = (Storage) obj;
      this.sm.resourceStorage.Set((KMonoBehaviour) this.storage, this.smi);
      IrrigationMonitor.Instance.DumpIncorrectFertilizers(this.storage, this.smi.gameObject);
      foreach (ManualDeliveryKG component in this.smi.gameObject.GetComponents<ManualDeliveryKG>())
      {
        bool flag = false;
        foreach (PlantElementAbsorber.ConsumeInfo consumedElement in this.def.consumedElements)
        {
          if (Tag.op_Equality(component.RequestedItemTag, consumedElement.tag))
          {
            flag = true;
            break;
          }
        }
        if (flag)
        {
          component.SetStorage(this.storage);
          ((Behaviour) component).enabled = !((Component) this.storage).gameObject.GetComponent<PlantablePlot>().has_liquid_pipe_input;
        }
      }
    }

    public WiltCondition.Condition[] Conditions => new WiltCondition.Condition[1]
    {
      WiltCondition.Condition.Irrigation
    };

    public string WiltStateString
    {
      get
      {
        string wiltStateString = "";
        if (this.smi.IsInsideState((StateMachine.BaseState) this.smi.sm.replanted.irrigated.absorbing.wrongLiquid))
          wiltStateString = this.GetIncorrectLiquidStatusItem().resolveStringCallback((string) CREATURES.STATUSITEMS.WRONGIRRIGATION.NAME, (object) this);
        else if (this.smi.IsInsideState((StateMachine.BaseState) this.smi.sm.replanted.starved))
          wiltStateString = this.GetStarvedStatusItem().resolveStringCallback((string) CREATURES.STATUSITEMS.NEEDSIRRIGATION.NAME, (object) this);
        else if (this.smi.IsInsideState((StateMachine.BaseState) this.smi.sm.replanted.starved.wrongLiquid))
          wiltStateString = this.GetIncorrectLiquidStatusItemMajor().resolveStringCallback((string) CREATURES.STATUSITEMS.WRONGIRRIGATIONMAJOR.NAME, (object) this);
        return wiltStateString;
      }
    }

    public virtual bool AcceptsLiquid()
    {
      PlantablePlot component = this.sm.resourceStorage.Get(this).GetComponent<PlantablePlot>();
      return Object.op_Inequality((Object) component, (Object) null) && component.AcceptsIrrigation;
    }

    public bool Starved() => (double) this.irrigation.value == 0.0;

    public void UpdateIrrigation(float dt)
    {
      if (this.def.consumedElements == null)
        return;
      Storage storage = this.sm.resourceStorage.Get<Storage>(this.smi);
      bool flag1 = true;
      bool flag2 = false;
      bool flag3 = true;
      if (Object.op_Inequality((Object) storage, (Object) null))
      {
        List<GameObject> items = storage.items;
        for (int index1 = 0; index1 < this.def.consumedElements.Length; ++index1)
        {
          float num = 0.0f;
          PlantElementAbsorber.ConsumeInfo consumedElement = this.def.consumedElements[index1];
          for (int index2 = 0; index2 < items.Count; ++index2)
          {
            GameObject go = items[index2];
            if (go.HasTag(consumedElement.tag))
              num += go.GetComponent<PrimaryElement>().Mass;
            else if (go.HasTag(this.def.wrongIrrigationTestTag))
              flag2 = true;
          }
          this.total_available_mass = num;
          float totalValue = this.gameObject.GetAttributes().Get(Db.Get().PlantAttributes.FertilizerUsageMod).GetTotalValue();
          if ((double) num < (double) consumedElement.massConsumptionRate * (double) totalValue * (double) dt)
          {
            flag1 = false;
            break;
          }
          if ((double) num < (double) consumedElement.massConsumptionRate * (double) totalValue * ((double) dt * 30.0))
          {
            flag3 = false;
            break;
          }
        }
      }
      else
      {
        flag1 = false;
        flag3 = false;
        flag2 = false;
      }
      this.sm.hasCorrectLiquid.Set(flag1, this.smi);
      this.sm.hasIncorrectLiquid.Set(flag2, this.smi);
      this.sm.enoughCorrectLiquidToRecover.Set(flag3 & flag1, this.smi);
    }

    public void UpdateAbsorbing(bool allow)
    {
      bool flag = allow && !this.smi.gameObject.HasTag(GameTags.Wilting);
      if (flag == this.absorberHandle.IsValid())
        return;
      if (flag)
      {
        if (this.def.consumedElements == null || this.def.consumedElements.Length == 0)
          return;
        float totalValue = this.gameObject.GetAttributes().Get(Db.Get().PlantAttributes.FertilizerUsageMod).GetTotalValue();
        PlantElementAbsorber.ConsumeInfo[] consumed_elements = new PlantElementAbsorber.ConsumeInfo[this.def.consumedElements.Length];
        for (int index = 0; index < this.def.consumedElements.Length; ++index)
        {
          PlantElementAbsorber.ConsumeInfo consumedElement = this.def.consumedElements[index];
          consumedElement.massConsumptionRate *= totalValue;
          consumed_elements[index] = consumedElement;
        }
        this.absorberHandle = Game.Instance.plantElementAbsorbers.Add(this.storage, consumed_elements);
      }
      else
        this.absorberHandle = Game.Instance.plantElementAbsorbers.Remove(this.absorberHandle);
    }
  }
}
