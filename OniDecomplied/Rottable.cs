// Decompiled with JetBrains decompiler
// Type: Rottable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class Rottable : 
  GameStateMachine<Rottable, Rottable.Instance, IStateMachineTarget, Rottable.Def>
{
  public StateMachine<Rottable, Rottable.Instance, IStateMachineTarget, Rottable.Def>.FloatParameter rotParameter;
  public GameStateMachine<Rottable, Rottable.Instance, IStateMachineTarget, Rottable.Def>.State Preserved;
  public GameStateMachine<Rottable, Rottable.Instance, IStateMachineTarget, Rottable.Def>.State Fresh;
  public GameStateMachine<Rottable, Rottable.Instance, IStateMachineTarget, Rottable.Def>.State Stale_Pre;
  public GameStateMachine<Rottable, Rottable.Instance, IStateMachineTarget, Rottable.Def>.State Stale;
  public GameStateMachine<Rottable, Rottable.Instance, IStateMachineTarget, Rottable.Def>.State Spoiled;
  private static readonly Tag[] PRESERVED_TAGS = new Tag[2]
  {
    GameTags.Preserved,
    GameTags.Entombed
  };
  private static readonly Rottable.RotCB rotCB = new Rottable.RotCB();
  public static Dictionary<int, Rottable.RotAtmosphereQuality> AtmosphereModifier = new Dictionary<int, Rottable.RotAtmosphereQuality>()
  {
    {
      721531317,
      Rottable.RotAtmosphereQuality.Contaminating
    },
    {
      1887387588,
      Rottable.RotAtmosphereQuality.Contaminating
    },
    {
      -1528777920,
      Rottable.RotAtmosphereQuality.Normal
    },
    {
      1836671383,
      Rottable.RotAtmosphereQuality.Normal
    },
    {
      1960575215,
      Rottable.RotAtmosphereQuality.Sterilizing
    },
    {
      -899515856,
      Rottable.RotAtmosphereQuality.Sterilizing
    },
    {
      -1554872654,
      Rottable.RotAtmosphereQuality.Sterilizing
    },
    {
      -1858722091,
      Rottable.RotAtmosphereQuality.Sterilizing
    },
    {
      758759285,
      Rottable.RotAtmosphereQuality.Sterilizing
    },
    {
      -1046145888,
      Rottable.RotAtmosphereQuality.Sterilizing
    },
    {
      -1324664829,
      Rottable.RotAtmosphereQuality.Sterilizing
    },
    {
      -1406916018,
      Rottable.RotAtmosphereQuality.Sterilizing
    },
    {
      -432557516,
      Rottable.RotAtmosphereQuality.Sterilizing
    },
    {
      -805366663,
      Rottable.RotAtmosphereQuality.Sterilizing
    },
    {
      1966552544,
      Rottable.RotAtmosphereQuality.Sterilizing
    }
  };

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.Fresh;
    this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
    this.root.TagTransition(GameTags.Preserved, this.Preserved).TagTransition(GameTags.Entombed, this.Preserved);
    double num1;
    this.Fresh.ToggleStatusItem(Db.Get().CreatureStatusItems.Fresh, (Func<Rottable.Instance, object>) (smi => (object) smi)).ParamTransition<float>((StateMachine<Rottable, Rottable.Instance, IStateMachineTarget, Rottable.Def>.Parameter<float>) this.rotParameter, this.Stale_Pre, (StateMachine<Rottable, Rottable.Instance, IStateMachineTarget, Rottable.Def>.Parameter<float>.Callback) ((smi, p) => (double) p <= (double) smi.def.spoilTime - ((double) smi.def.spoilTime - (double) smi.def.staleTime))).Update((System.Action<Rottable.Instance, float>) ((smi, dt) => num1 = (double) smi.sm.rotParameter.Set(smi.RotValue, smi)), (UpdateRate) 6, true).FastUpdate("Rot", (UpdateBucketWithUpdater<Rottable.Instance>.IUpdater) Rottable.rotCB, (UpdateRate) 6, true);
    this.Preserved.TagTransition(Rottable.PRESERVED_TAGS, this.Fresh, true).Enter("RefreshModifiers", (StateMachine<Rottable, Rottable.Instance, IStateMachineTarget, Rottable.Def>.State.Callback) (smi => smi.RefreshModifiers(0.0f)));
    this.Stale_Pre.Enter((StateMachine<Rottable, Rottable.Instance, IStateMachineTarget, Rottable.Def>.State.Callback) (smi => smi.GoTo((StateMachine.BaseState) this.Stale)));
    double num2;
    this.Stale.ToggleStatusItem(Db.Get().CreatureStatusItems.Stale, (Func<Rottable.Instance, object>) (smi => (object) smi)).ParamTransition<float>((StateMachine<Rottable, Rottable.Instance, IStateMachineTarget, Rottable.Def>.Parameter<float>) this.rotParameter, this.Fresh, (StateMachine<Rottable, Rottable.Instance, IStateMachineTarget, Rottable.Def>.Parameter<float>.Callback) ((smi, p) => (double) p > (double) smi.def.spoilTime - ((double) smi.def.spoilTime - (double) smi.def.staleTime))).ParamTransition<float>((StateMachine<Rottable, Rottable.Instance, IStateMachineTarget, Rottable.Def>.Parameter<float>) this.rotParameter, this.Spoiled, GameStateMachine<Rottable, Rottable.Instance, IStateMachineTarget, Rottable.Def>.IsLTEZero).Update((System.Action<Rottable.Instance, float>) ((smi, dt) => num2 = (double) smi.sm.rotParameter.Set(smi.RotValue, smi)), (UpdateRate) 6).FastUpdate("Rot", (UpdateBucketWithUpdater<Rottable.Instance>.IUpdater) Rottable.rotCB, (UpdateRate) 6);
    this.Spoiled.Enter((StateMachine<Rottable, Rottable.Instance, IStateMachineTarget, Rottable.Def>.State.Callback) (smi =>
    {
      GameObject gameObject = Scenario.SpawnPrefab(Grid.PosToCell(smi.master.gameObject), 0, 0, "RotPile");
      gameObject.gameObject.GetComponent<KSelectable>().SetName((string) UI.GAMEOBJECTEFFECTS.ROTTEN + " " + smi.master.gameObject.GetProperName());
      TransformExtensions.SetPosition(gameObject.transform, TransformExtensions.GetPosition(smi.master.transform));
      gameObject.GetComponent<PrimaryElement>().Mass = smi.master.GetComponent<PrimaryElement>().Mass;
      gameObject.GetComponent<PrimaryElement>().Temperature = smi.master.GetComponent<PrimaryElement>().Temperature;
      gameObject.SetActive(true);
      PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, (string) ITEMS.FOOD.ROTPILE.NAME, gameObject.transform);
      Edible component1 = smi.GetComponent<Edible>();
      if (Object.op_Inequality((Object) component1, (Object) null))
      {
        if (Object.op_Inequality((Object) component1.worker, (Object) null))
        {
          ChoreDriver component2 = ((Component) component1.worker).GetComponent<ChoreDriver>();
          if (Object.op_Inequality((Object) component2, (Object) null) && component2.GetCurrentChore() != null)
            component2.GetCurrentChore().Fail("food rotted");
        }
        ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, -component1.Calories, StringFormatter.Replace((string) UI.ENDOFDAYREPORT.NOTES.ROTTED, "{0}", smi.gameObject.GetProperName()), (string) UI.ENDOFDAYREPORT.NOTES.ROTTED_CONTEXT);
      }
      Util.KDestroyGameObject(smi.gameObject);
    }));
  }

  private static string OnStaleTooltip(List<Notification> notifications, object data)
  {
    string str = "\n";
    foreach (Notification notification in notifications)
    {
      if (notification.tooltipData != null)
      {
        GameObject tooltipData = (GameObject) notification.tooltipData;
        if (Object.op_Inequality((Object) tooltipData, (Object) null))
          str = str + "\n" + tooltipData.GetProperName();
      }
    }
    return string.Format((string) MISC.NOTIFICATIONS.FOODSTALE.TOOLTIP, (object) str);
  }

  public static void SetStatusItems(IRottable rottable)
  {
    Grid.PosToCell(rottable.gameObject);
    KSelectable component = rottable.gameObject.GetComponent<KSelectable>();
    switch (Rottable.RefrigerationLevel(rottable))
    {
      case Rottable.RotRefrigerationLevel.Refrigerated:
        component.SetStatusItem(Db.Get().StatusItemCategories.PreservationTemperature, Db.Get().CreatureStatusItems.Refrigerated, (object) rottable);
        break;
      case Rottable.RotRefrigerationLevel.Frozen:
        component.SetStatusItem(Db.Get().StatusItemCategories.PreservationTemperature, Db.Get().CreatureStatusItems.RefrigeratedFrozen, (object) rottable);
        break;
      default:
        component.SetStatusItem(Db.Get().StatusItemCategories.PreservationTemperature, Db.Get().CreatureStatusItems.Unrefrigerated, (object) rottable);
        break;
    }
    switch (Rottable.AtmosphereQuality(rottable))
    {
      case Rottable.RotAtmosphereQuality.Sterilizing:
        component.SetStatusItem(Db.Get().StatusItemCategories.PreservationAtmosphere, Db.Get().CreatureStatusItems.SterilizingAtmosphere);
        break;
      case Rottable.RotAtmosphereQuality.Contaminating:
        component.SetStatusItem(Db.Get().StatusItemCategories.PreservationAtmosphere, Db.Get().CreatureStatusItems.ContaminatedAtmosphere);
        break;
      default:
        component.SetStatusItem(Db.Get().StatusItemCategories.PreservationAtmosphere, (StatusItem) null);
        break;
    }
  }

  public static bool IsInActiveFridge(IRottable rottable)
  {
    Pickupable component1 = rottable.gameObject.GetComponent<Pickupable>();
    if (!Object.op_Inequality((Object) component1, (Object) null) || !Object.op_Inequality((Object) component1.storage, (Object) null))
      return false;
    Refrigerator component2 = ((Component) component1.storage).GetComponent<Refrigerator>();
    return Object.op_Inequality((Object) component2, (Object) null) && component2.IsActive();
  }

  public static Rottable.RotRefrigerationLevel RefrigerationLevel(IRottable rottable)
  {
    int cell = Grid.PosToCell(rottable.gameObject);
    Rottable.Instance smi = rottable.gameObject.GetSMI<Rottable.Instance>();
    PrimaryElement component = rottable.gameObject.GetComponent<PrimaryElement>();
    float num = component.Temperature;
    bool flag = false;
    if (!Grid.IsValidCell(cell))
    {
      if (!smi.IsRottableInSpace())
        return Rottable.RotRefrigerationLevel.Normal;
      flag = true;
    }
    if (!flag && Grid.Element[cell].id != SimHashes.Vacuum)
      num = Mathf.Min(Grid.Temperature[cell], component.Temperature);
    if ((double) num < (double) rottable.PreserveTemperature)
      return Rottable.RotRefrigerationLevel.Frozen;
    return (double) num < (double) rottable.RotTemperature || Rottable.IsInActiveFridge(rottable) ? Rottable.RotRefrigerationLevel.Refrigerated : Rottable.RotRefrigerationLevel.Normal;
  }

  public static Rottable.RotAtmosphereQuality AtmosphereQuality(IRottable rottable)
  {
    int cell1 = Grid.PosToCell(rottable.gameObject);
    int cell2 = Grid.CellAbove(cell1);
    if (!Grid.IsValidCell(cell1))
      return rottable.gameObject.GetSMI<Rottable.Instance>().IsRottableInSpace() ? Rottable.RotAtmosphereQuality.Sterilizing : Rottable.RotAtmosphereQuality.Normal;
    SimHashes id1 = Grid.Element[cell1].id;
    Rottable.RotAtmosphereQuality atmosphereQuality1 = Rottable.RotAtmosphereQuality.Normal;
    Rottable.AtmosphereModifier.TryGetValue((int) id1, out atmosphereQuality1);
    Rottable.RotAtmosphereQuality atmosphereQuality2 = Rottable.RotAtmosphereQuality.Normal;
    if (Grid.IsValidCell(cell2))
    {
      SimHashes id2 = Grid.Element[cell2].id;
      if (!Rottable.AtmosphereModifier.TryGetValue((int) id2, out atmosphereQuality2))
        atmosphereQuality2 = atmosphereQuality1;
    }
    else
      atmosphereQuality2 = atmosphereQuality1;
    if (atmosphereQuality1 == atmosphereQuality2)
      return atmosphereQuality1;
    if (atmosphereQuality1 == Rottable.RotAtmosphereQuality.Contaminating || atmosphereQuality2 == Rottable.RotAtmosphereQuality.Contaminating)
      return Rottable.RotAtmosphereQuality.Contaminating;
    return atmosphereQuality1 == Rottable.RotAtmosphereQuality.Normal || atmosphereQuality2 == Rottable.RotAtmosphereQuality.Normal ? Rottable.RotAtmosphereQuality.Normal : Rottable.RotAtmosphereQuality.Sterilizing;
  }

  public class Def : StateMachine.BaseDef
  {
    public float spoilTime;
    public float staleTime;
    public float preserveTemperature = 255.15f;
    public float rotTemperature = 277.15f;
  }

  private class RotCB : UpdateBucketWithUpdater<Rottable.Instance>.IUpdater
  {
    public void Update(Rottable.Instance smi, float dt) => smi.Rot(smi, dt);
  }

  public new class Instance : 
    GameStateMachine<Rottable, Rottable.Instance, IStateMachineTarget, Rottable.Def>.GameInstance,
    IRottable
  {
    private AmountInstance rotAmountInstance;
    private static AttributeModifier unrefrigeratedModifier;
    private static AttributeModifier refrigeratedModifier;
    private static AttributeModifier frozenModifier;
    private static AttributeModifier contaminatedAtmosphereModifier;
    private static AttributeModifier normalAtmosphereModifier;
    private static AttributeModifier sterileAtmosphereModifier;
    public PrimaryElement primaryElement;
    public Pickupable pickupable;

    public float RotValue
    {
      get => this.rotAmountInstance.value;
      set
      {
        double num1 = (double) this.sm.rotParameter.Set(value, this);
        double num2 = (double) this.rotAmountInstance.SetValue(value);
      }
    }

    public float RotConstitutionPercentage => this.RotValue / this.def.spoilTime;

    public float RotTemperature => this.def.rotTemperature;

    public float PreserveTemperature => this.def.preserveTemperature;

    public Instance(IStateMachineTarget master, Rottable.Def def)
      : base(master, def)
    {
      this.pickupable = Util.RequireComponent<Pickupable>(this.gameObject);
      this.master.Subscribe(-2064133523, new System.Action<object>(this.OnAbsorb));
      this.master.Subscribe(1335436905, new System.Action<object>(this.OnSplitFromChunk));
      this.primaryElement = this.gameObject.GetComponent<PrimaryElement>();
      this.rotAmountInstance = master.gameObject.GetAmounts().Add(new AmountInstance(Db.Get().Amounts.Rot, master.gameObject));
      this.rotAmountInstance.maxAttribute.Add(new AttributeModifier("Rot", def.spoilTime));
      double num1 = (double) this.rotAmountInstance.SetValue(def.spoilTime);
      double num2 = (double) this.sm.rotParameter.Set(this.rotAmountInstance.value, this.smi);
      if (Rottable.Instance.unrefrigeratedModifier == null)
      {
        Rottable.Instance.unrefrigeratedModifier = new AttributeModifier(this.rotAmountInstance.amount.Id, -0.7f, (string) DUPLICANTS.MODIFIERS.ROTTEMPERATURE.UNREFRIGERATED);
        Rottable.Instance.refrigeratedModifier = new AttributeModifier(this.rotAmountInstance.amount.Id, -0.2f, (string) DUPLICANTS.MODIFIERS.ROTTEMPERATURE.REFRIGERATED);
        Rottable.Instance.frozenModifier = new AttributeModifier(this.rotAmountInstance.amount.Id, -0.0f, (string) DUPLICANTS.MODIFIERS.ROTTEMPERATURE.FROZEN);
        Rottable.Instance.contaminatedAtmosphereModifier = new AttributeModifier(this.rotAmountInstance.amount.Id, -1f, (string) DUPLICANTS.MODIFIERS.ROTATMOSPHERE.CONTAMINATED);
        Rottable.Instance.normalAtmosphereModifier = new AttributeModifier(this.rotAmountInstance.amount.Id, -0.3f, (string) DUPLICANTS.MODIFIERS.ROTATMOSPHERE.NORMAL);
        Rottable.Instance.sterileAtmosphereModifier = new AttributeModifier(this.rotAmountInstance.amount.Id, -0.0f, (string) DUPLICANTS.MODIFIERS.ROTATMOSPHERE.STERILE);
      }
      this.RefreshModifiers(0.0f);
    }

    [OnDeserialized]
    private void OnDeserialized()
    {
      if (!SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 23))
        return;
      double num = (double) this.rotAmountInstance.SetValue(this.rotAmountInstance.value * 2f);
    }

    public string StateString()
    {
      string str = "";
      if (this.smi.GetCurrentState() == this.sm.Fresh)
        str = Db.Get().CreatureStatusItems.Fresh.resolveStringCallback((string) CREATURES.STATUSITEMS.FRESH.NAME, (object) this);
      if (this.smi.GetCurrentState() == this.sm.Stale)
        str = Db.Get().CreatureStatusItems.Fresh.resolveStringCallback((string) CREATURES.STATUSITEMS.STALE.NAME, (object) this);
      return str;
    }

    public void Rot(Rottable.Instance smi, float deltaTime)
    {
      this.RefreshModifiers(deltaTime);
      if (!Object.op_Inequality((Object) smi.pickupable.storage, (Object) null))
        return;
      smi.pickupable.storage.Trigger(-1197125120, (object) null);
    }

    public bool IsRottableInSpace()
    {
      if (Object.op_Equality((Object) this.gameObject.GetMyWorld(), (Object) null))
      {
        Pickupable component = this.GetComponent<Pickupable>();
        if (Object.op_Inequality((Object) component, (Object) null) && Object.op_Implicit((Object) component.storage) && (Object.op_Implicit((Object) ((Component) component.storage).GetComponent<RocketModuleCluster>()) || Object.op_Implicit((Object) ((Component) component.storage).GetComponent<ClusterTraveler>())))
          return true;
      }
      return false;
    }

    public void RefreshModifiers(float dt)
    {
      if (this.GetMaster().isNull || !Grid.IsValidCell(Grid.PosToCell(this.gameObject)) && !this.IsRottableInSpace())
        return;
      this.rotAmountInstance.deltaAttribute.ClearModifiers();
      KPrefabID component = this.GetComponent<KPrefabID>();
      if (!component.HasAnyTags(Rottable.PRESERVED_TAGS))
      {
        switch (Rottable.RefrigerationLevel((IRottable) this))
        {
          case Rottable.RotRefrigerationLevel.Refrigerated:
            this.rotAmountInstance.deltaAttribute.Add(Rottable.Instance.refrigeratedModifier);
            break;
          case Rottable.RotRefrigerationLevel.Frozen:
            this.rotAmountInstance.deltaAttribute.Add(Rottable.Instance.frozenModifier);
            break;
          default:
            this.rotAmountInstance.deltaAttribute.Add(Rottable.Instance.unrefrigeratedModifier);
            break;
        }
        switch (Rottable.AtmosphereQuality((IRottable) this))
        {
          case Rottable.RotAtmosphereQuality.Sterilizing:
            this.rotAmountInstance.deltaAttribute.Add(Rottable.Instance.sterileAtmosphereModifier);
            break;
          case Rottable.RotAtmosphereQuality.Contaminating:
            this.rotAmountInstance.deltaAttribute.Add(Rottable.Instance.contaminatedAtmosphereModifier);
            break;
          default:
            this.rotAmountInstance.deltaAttribute.Add(Rottable.Instance.normalAtmosphereModifier);
            break;
        }
      }
      if (component.HasTag(Tag.op_Implicit(Db.Get().Spices.PreservingSpice.Id)))
        this.rotAmountInstance.deltaAttribute.Add(Db.Get().Spices.PreservingSpice.FoodModifier);
      Rottable.SetStatusItems((IRottable) this);
    }

    private void OnAbsorb(object data)
    {
      Pickupable pickupable = (Pickupable) data;
      if (!Object.op_Inequality((Object) pickupable, (Object) null))
        return;
      PrimaryElement component = this.gameObject.GetComponent<PrimaryElement>();
      PrimaryElement primaryElement = pickupable.PrimaryElement;
      Rottable.Instance smi = ((Component) pickupable).gameObject.GetSMI<Rottable.Instance>();
      if (!Object.op_Inequality((Object) component, (Object) null) || !Object.op_Inequality((Object) primaryElement, (Object) null) || smi == null)
        return;
      double num = (double) this.sm.rotParameter.Set((float) (((double) component.Units * (double) this.sm.rotParameter.Get(this.smi) + (double) (primaryElement.Units * this.sm.rotParameter.Get(smi))) / ((double) component.Units + (double) primaryElement.Units)), this.smi);
    }

    public bool IsRotLevelStackable(Rottable.Instance other) => (double) Mathf.Abs(this.RotConstitutionPercentage - other.RotConstitutionPercentage) < 0.10000000149011612;

    public string GetToolTip() => this.rotAmountInstance.GetTooltip();

    private void OnSplitFromChunk(object data)
    {
      Pickupable cmp = (Pickupable) data;
      if (!Object.op_Inequality((Object) cmp, (Object) null))
        return;
      Rottable.Instance smi = ((Component) cmp).GetSMI<Rottable.Instance>();
      if (smi == null)
        return;
      this.RotValue = smi.RotValue;
    }

    public void OnPreserved(object data)
    {
      if ((bool) data)
        this.smi.GoTo((StateMachine.BaseState) this.sm.Preserved);
      else
        this.smi.GoTo((StateMachine.BaseState) this.sm.Fresh);
    }
  }

  public enum RotAtmosphereQuality
  {
    Normal,
    Sterilizing,
    Contaminating,
  }

  public enum RotRefrigerationLevel
  {
    Normal,
    Refrigerated,
    Frozen,
  }
}
