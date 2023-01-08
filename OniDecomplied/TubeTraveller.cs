// Decompiled with JetBrains decompiler
// Type: TubeTraveller
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System.Collections.Generic;
using UnityEngine;

public class TubeTraveller : GameStateMachine<TubeTraveller, TubeTraveller.Instance>
{
  private List<Effect> immunities = new List<Effect>();
  private List<AttributeModifier> modifiers = new List<AttributeModifier>();

  public void InitModifiers()
  {
    this.modifiers.Add(new AttributeModifier(Db.Get().Attributes.Insulation.Id, (float) TUNING.EQUIPMENT.SUITS.ATMOSUIT_INSULATION, (string) STRINGS.BUILDINGS.PREFABS.TRAVELTUBE.NAME));
    this.modifiers.Add(new AttributeModifier(Db.Get().Attributes.ThermalConductivityBarrier.Id, TUNING.EQUIPMENT.SUITS.ATMOSUIT_THERMAL_CONDUCTIVITY_BARRIER, (string) STRINGS.BUILDINGS.PREFABS.TRAVELTUBE.NAME));
    this.modifiers.Add(new AttributeModifier(Db.Get().Amounts.Bladder.deltaAttribute.Id, TUNING.EQUIPMENT.SUITS.ATMOSUIT_BLADDER, (string) STRINGS.BUILDINGS.PREFABS.TRAVELTUBE.NAME));
    this.modifiers.Add(new AttributeModifier(Db.Get().Attributes.ScaldingThreshold.Id, (float) TUNING.EQUIPMENT.SUITS.ATMOSUIT_SCALDING, (string) STRINGS.BUILDINGS.PREFABS.TRAVELTUBE.NAME));
    this.immunities.Add(Db.Get().effects.Get("SoakingWet"));
    this.immunities.Add(Db.Get().effects.Get("WetFeet"));
    this.immunities.Add(Db.Get().effects.Get("PoppedEarDrums"));
    this.immunities.Add(Db.Get().effects.Get("MinorIrritation"));
    this.immunities.Add(Db.Get().effects.Get("MajorIrritation"));
  }

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    this.InitModifiers();
    default_state = (StateMachine.BaseState) this.root;
    this.root.DoNothing();
  }

  public void OnSetOxygenBreather(OxygenBreather oxygen_breather)
  {
  }

  public void OnClearOxygenBreather(OxygenBreather oxygen_breather)
  {
  }

  public bool ConsumeGas(OxygenBreather oxygen_breather, float amount) => false;

  public bool ShouldEmitCO2() => false;

  public bool ShouldStoreCO2() => false;

  public new class Instance : 
    GameStateMachine<TubeTraveller, TubeTraveller.Instance, IStateMachineTarget, object>.GameInstance
  {
    private List<TravelTubeEntrance> reservations = new List<TravelTubeEntrance>();
    public bool inTube;

    public int prefabInstanceID => ((Component) this.GetComponent<Navigator>()).gameObject.GetComponent<KPrefabID>().InstanceID;

    public Instance(IStateMachineTarget master)
      : base(master)
    {
    }

    public void OnPathAdvanced(object data)
    {
      this.UnreserveEntrances();
      this.ReserveEntrances();
    }

    public void ReserveEntrances()
    {
      PathFinder.Path path = this.GetComponent<Navigator>().path;
      if (path.nodes == null)
        return;
      for (int index = 0; index < path.nodes.Count - 1; ++index)
      {
        if (path.nodes[index].navType == NavType.Floor && path.nodes[index + 1].navType == NavType.Tube)
        {
          int cell = path.nodes[index].cell;
          if (Grid.HasUsableTubeEntrance(cell, this.prefabInstanceID))
          {
            GameObject gameObject = Grid.Objects[cell, 1];
            if (Object.op_Implicit((Object) gameObject))
            {
              TravelTubeEntrance component = gameObject.GetComponent<TravelTubeEntrance>();
              if (Object.op_Implicit((Object) component))
              {
                component.Reserve(this, this.prefabInstanceID);
                this.reservations.Add(component);
              }
            }
          }
        }
      }
    }

    public void UnreserveEntrances()
    {
      foreach (TravelTubeEntrance reservation in this.reservations)
      {
        if (!Object.op_Equality((Object) reservation, (Object) null))
          reservation.Unreserve(this, this.prefabInstanceID);
      }
      this.reservations.Clear();
    }

    public void ApplyEnteringTubeEffects()
    {
      Effects component1 = this.GetComponent<Effects>();
      Attributes attributes = this.gameObject.GetAttributes();
      this.gameObject.AddTag(GameTags.InTransitTube);
      Tag inTransitTube = GameTags.InTransitTube;
      string name = ((Tag) ref inTransitTube).Name;
      foreach (Effect immunity in this.sm.immunities)
        component1.AddImmunity(immunity, name);
      foreach (AttributeModifier modifier in this.sm.modifiers)
        attributes.Add(modifier);
      CreatureSimTemperatureTransfer component2 = this.gameObject.GetComponent<CreatureSimTemperatureTransfer>();
      if (!Object.op_Inequality((Object) component2, (Object) null))
        return;
      component2.RefreshRegistration();
    }

    public void ClearAllEffects()
    {
      Effects component1 = this.GetComponent<Effects>();
      Attributes attributes = this.gameObject.GetAttributes();
      this.gameObject.RemoveTag(GameTags.InTransitTube);
      Tag inTransitTube = GameTags.InTransitTube;
      string name = ((Tag) ref inTransitTube).Name;
      foreach (Effect immunity in this.sm.immunities)
        component1.RemoveImmunity(immunity, name);
      foreach (AttributeModifier modifier in this.sm.modifiers)
        attributes.Remove(modifier);
      CreatureSimTemperatureTransfer component2 = this.gameObject.GetComponent<CreatureSimTemperatureTransfer>();
      if (!Object.op_Inequality((Object) component2, (Object) null))
        return;
      component2.RefreshRegistration();
    }

    public void OnTubeTransition(bool nowInTube)
    {
      if (nowInTube == this.inTube)
        return;
      this.inTube = nowInTube;
      this.GetComponent<Effects>();
      this.gameObject.GetAttributes();
      if (nowInTube)
        this.ApplyEnteringTubeEffects();
      else
        this.ClearAllEffects();
    }
  }
}
