// Decompiled with JetBrains decompiler
// Type: Klei.AI.SlimeSickness
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klei.AI
{
  public class SlimeSickness : Sickness
  {
    private const float COUGH_FREQUENCY = 20f;
    private const float COUGH_MASS = 0.1f;
    private const int DISEASE_AMOUNT = 1000;
    public const string ID = "SlimeSickness";
    public const string RECOVERY_ID = "SlimeSicknessRecovery";

    public SlimeSickness()
      : base(nameof (SlimeSickness), Sickness.SicknessType.Pathogen, Sickness.Severity.Minor, 0.00025f, new List<Sickness.InfectionVector>()
      {
        Sickness.InfectionVector.Inhalation
      }, 2220f, "SlimeSicknessRecovery")
    {
      this.AddSicknessComponent((Sickness.SicknessComponent) new CommonSickEffectSickness());
      this.AddSicknessComponent((Sickness.SicknessComponent) new AttributeModifierSickness(new AttributeModifier[2]
      {
        new AttributeModifier("BreathDelta", -1.13636363f, (string) DUPLICANTS.DISEASES.SLIMESICKNESS.NAME),
        new AttributeModifier("Athletics", -3f, (string) DUPLICANTS.DISEASES.SLIMESICKNESS.NAME)
      }));
      this.AddSicknessComponent((Sickness.SicknessComponent) new AnimatedSickness(new HashedString[1]
      {
        HashedString.op_Implicit("anim_idle_sick_kanim")
      }, Db.Get().Expressions.Sick));
      this.AddSicknessComponent((Sickness.SicknessComponent) new PeriodicEmoteSickness(Db.Get().Emotes.Minion.Sick, 50f));
      this.AddSicknessComponent((Sickness.SicknessComponent) new SlimeSickness.SlimeLungComponent());
    }

    public class SlimeLungComponent : Sickness.SicknessComponent
    {
      public override object OnInfect(GameObject go, SicknessInstance diseaseInstance)
      {
        SlimeSickness.SlimeLungComponent.StatesInstance statesInstance = new SlimeSickness.SlimeLungComponent.StatesInstance(diseaseInstance);
        statesInstance.StartSM();
        return (object) statesInstance;
      }

      public override void OnCure(GameObject go, object instance_data) => ((StateMachine.Instance) instance_data).StopSM("Cured");

      public override List<Descriptor> GetSymptoms()
      {
        List<Descriptor> symptoms = new List<Descriptor>();
        symptoms.Add(new Descriptor((string) DUPLICANTS.DISEASES.SLIMESICKNESS.COUGH_SYMPTOM, (string) DUPLICANTS.DISEASES.SLIMESICKNESS.COUGH_SYMPTOM_TOOLTIP, (Descriptor.DescriptorType) 7, false));
        return symptoms;
      }

      public class StatesInstance : 
        GameStateMachine<SlimeSickness.SlimeLungComponent.States, SlimeSickness.SlimeLungComponent.StatesInstance, SicknessInstance, object>.GameInstance
      {
        public float lastCoughTime;

        public StatesInstance(SicknessInstance master)
          : base(master)
        {
        }

        public Reactable GetReactable()
        {
          Emote cough = Db.Get().Emotes.Minion.Cough;
          SelfEmoteReactable reactable = new SelfEmoteReactable(this.master.gameObject, HashedString.op_Implicit("SlimeLungCough"), Db.Get().ChoreTypes.Cough, localCooldown: 0.0f);
          reactable.SetEmote(cough);
          reactable.RegisterEmoteStepCallbacks(HashedString.op_Implicit("react"), (Action<GameObject>) null, new Action<GameObject>(this.FinishedCoughing));
          return (Reactable) reactable;
        }

        private void ProduceSlime(GameObject cougher)
        {
          AmountInstance amountInstance = Db.Get().Amounts.Temperature.Lookup(cougher);
          int cell = Grid.PosToCell(cougher);
          string id = Db.Get().Diseases.SlimeGerms.Id;
          Equippable equippable = this.master.gameObject.GetComponent<SuitEquipper>().IsWearingAirtightSuit();
          if (Object.op_Inequality((Object) equippable, (Object) null))
            ((Component) equippable).GetComponent<Storage>().AddGasChunk(SimHashes.ContaminatedOxygen, 0.1f, amountInstance.value, Db.Get().Diseases.GetIndex(HashedString.op_Implicit(id)), 1000, false);
          else
            SimMessages.AddRemoveSubstance(cell, SimHashes.ContaminatedOxygen, CellEventLogger.Instance.Cough, 0.1f, amountInstance.value, Db.Get().Diseases.GetIndex(HashedString.op_Implicit(id)), 1000);
          PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, string.Format((string) DUPLICANTS.DISEASES.ADDED_POPFX, (object) this.master.modifier.Name, (object) 1000), cougher.transform);
        }

        private void FinishedCoughing(GameObject cougher)
        {
          this.ProduceSlime(cougher);
          this.sm.coughFinished.Trigger(this);
        }
      }

      public class States : 
        GameStateMachine<SlimeSickness.SlimeLungComponent.States, SlimeSickness.SlimeLungComponent.StatesInstance, SicknessInstance>
      {
        public StateMachine<SlimeSickness.SlimeLungComponent.States, SlimeSickness.SlimeLungComponent.StatesInstance, SicknessInstance, object>.Signal coughFinished;
        public SlimeSickness.SlimeLungComponent.States.BreathingStates breathing;
        public GameStateMachine<SlimeSickness.SlimeLungComponent.States, SlimeSickness.SlimeLungComponent.StatesInstance, SicknessInstance, object>.State notbreathing;

        public override void InitializeStates(out StateMachine.BaseState default_state)
        {
          default_state = (StateMachine.BaseState) this.breathing;
          this.breathing.DefaultState(this.breathing.normal).TagTransition(GameTags.NoOxygen, this.notbreathing);
          this.breathing.normal.Enter("SetCoughTime", (StateMachine<SlimeSickness.SlimeLungComponent.States, SlimeSickness.SlimeLungComponent.StatesInstance, SicknessInstance, object>.State.Callback) (smi =>
          {
            if ((double) smi.lastCoughTime >= (double) Time.time)
              return;
            smi.lastCoughTime = Time.time;
          })).Update("Cough", (Action<SlimeSickness.SlimeLungComponent.StatesInstance, float>) ((smi, dt) =>
          {
            if (smi.master.IsDoctored || (double) Time.time - (double) smi.lastCoughTime <= 20.0)
              return;
            smi.GoTo((StateMachine.BaseState) this.breathing.cough);
          }), (UpdateRate) 7);
          this.breathing.cough.ToggleReactable((Func<SlimeSickness.SlimeLungComponent.StatesInstance, Reactable>) (smi => smi.GetReactable())).OnSignal(this.coughFinished, this.breathing.normal);
          this.notbreathing.TagTransition(new Tag[1]
          {
            GameTags.NoOxygen
          }, (GameStateMachine<SlimeSickness.SlimeLungComponent.States, SlimeSickness.SlimeLungComponent.StatesInstance, SicknessInstance, object>.State) this.breathing, true);
        }

        public class BreathingStates : 
          GameStateMachine<SlimeSickness.SlimeLungComponent.States, SlimeSickness.SlimeLungComponent.StatesInstance, SicknessInstance, object>.State
        {
          public GameStateMachine<SlimeSickness.SlimeLungComponent.States, SlimeSickness.SlimeLungComponent.StatesInstance, SicknessInstance, object>.State normal;
          public GameStateMachine<SlimeSickness.SlimeLungComponent.States, SlimeSickness.SlimeLungComponent.StatesInstance, SicknessInstance, object>.State cough;
        }
      }
    }
  }
}
