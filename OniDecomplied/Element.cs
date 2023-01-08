// Decompiled with JetBrains decompiler
// Type: Element
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Diagnostics;

[DebuggerDisplay("{name}")]
[Serializable]
public class Element : IComparable<Element>
{
  public const int INVALID_ID = 0;
  public SimHashes id;
  public Tag tag;
  public ushort idx;
  public float specificHeatCapacity;
  public float thermalConductivity = 1f;
  public float molarMass = 1f;
  public float strength;
  public float flow;
  public float maxCompression;
  public float viscosity;
  public float minHorizontalFlow = float.PositiveInfinity;
  public float minVerticalFlow = float.PositiveInfinity;
  public float maxMass = 10000f;
  public float solidSurfaceAreaMultiplier;
  public float liquidSurfaceAreaMultiplier;
  public float gasSurfaceAreaMultiplier;
  public Element.State state;
  public byte hardness;
  public float lowTemp;
  public SimHashes lowTempTransitionTarget;
  public Element lowTempTransition;
  public float highTemp;
  public SimHashes highTempTransitionTarget;
  public Element highTempTransition;
  public SimHashes highTempTransitionOreID = SimHashes.Vacuum;
  public float highTempTransitionOreMassConversion;
  public SimHashes lowTempTransitionOreID = SimHashes.Vacuum;
  public float lowTempTransitionOreMassConversion;
  public SimHashes sublimateId;
  public SimHashes convertId;
  public SpawnFXHashes sublimateFX;
  public float sublimateRate;
  public float sublimateEfficiency;
  public float sublimateProbability;
  public float offGasPercentage;
  public float lightAbsorptionFactor;
  public float radiationAbsorptionFactor;
  public float radiationPer1000Mass;
  public Sim.PhysicsData defaultValues;
  public float toxicity;
  public Substance substance;
  public Tag materialCategory;
  public int buildMenuSort;
  public ElementLoader.ElementComposition[] elementComposition;
  public Tag[] oreTags = new Tag[0];
  public List<AttributeModifier> attributeModifiers = new List<AttributeModifier>();
  public bool disabled;
  public string dlcId;
  public const byte StateMask = 3;

  public float PressureToMass(float pressure) => pressure / this.defaultValues.pressure;

  public bool IsUnstable => this.HasTag(GameTags.Unstable);

  public bool IsLiquid => (this.state & Element.State.Solid) == Element.State.Liquid;

  public bool IsGas => (this.state & Element.State.Solid) == Element.State.Gas;

  public bool IsSolid => (this.state & Element.State.Solid) == Element.State.Solid;

  public bool IsVacuum => (this.state & Element.State.Solid) == Element.State.Vacuum;

  public bool IsTemperatureInsulated => (this.state & Element.State.TemperatureInsulated) != 0;

  public bool IsState(Element.State expected_state) => (this.state & Element.State.Solid) == expected_state;

  public bool HasTransitionUp => this.highTempTransitionTarget != (SimHashes) 0 && this.highTempTransitionTarget != SimHashes.Unobtanium && this.highTempTransition != null && this.highTempTransition != this;

  public string name { get; set; }

  public string nameUpperCase { get; set; }

  public string description { get; set; }

  public string GetStateString() => Element.GetStateString(this.state);

  public static string GetStateString(Element.State state)
  {
    if ((state & Element.State.Solid) == Element.State.Solid)
      return (string) ELEMENTS.STATE.SOLID;
    if ((state & Element.State.Solid) == Element.State.Liquid)
      return (string) ELEMENTS.STATE.LIQUID;
    return (state & Element.State.Solid) == Element.State.Gas ? (string) ELEMENTS.STATE.GAS : (string) ELEMENTS.STATE.VACUUM;
  }

  public string FullDescription(bool addHardnessColor = true)
  {
    string str1 = this.Description();
    if (this.IsSolid)
      str1 = str1 + "\n\n" + string.Format((string) ELEMENTS.ELEMENTDESCSOLID, (object) this.GetMaterialCategoryTag().ProperName(), (object) GameUtil.GetFormattedTemperature(this.highTemp), (object) GameUtil.GetHardnessString(this, addHardnessColor));
    else if (this.IsLiquid)
      str1 = str1 + "\n\n" + string.Format((string) ELEMENTS.ELEMENTDESCLIQUID, (object) this.GetMaterialCategoryTag().ProperName(), (object) GameUtil.GetFormattedTemperature(this.lowTemp), (object) GameUtil.GetFormattedTemperature(this.highTemp));
    else if (!this.IsVacuum)
      str1 = str1 + "\n\n" + string.Format((string) ELEMENTS.ELEMENTDESCGAS, (object) this.GetMaterialCategoryTag().ProperName(), (object) GameUtil.GetFormattedTemperature(this.lowTemp));
    string str2 = ((string) ELEMENTS.THERMALPROPERTIES).Replace("{SPECIFIC_HEAT_CAPACITY}", GameUtil.GetFormattedSHC(this.specificHeatCapacity)).Replace("{THERMAL_CONDUCTIVITY}", GameUtil.GetFormattedThermalConductivity(this.thermalConductivity));
    string str3 = str1 + "\n" + str2;
    if (DlcManager.FeatureRadiationEnabled())
      str3 = str3 + "\n" + string.Format((string) ELEMENTS.RADIATIONPROPERTIES, (object) this.radiationAbsorptionFactor, (object) GameUtil.GetFormattedRads((float) ((double) this.radiationPer1000Mass * 1.1000000238418579 / 600.0), GameUtil.TimeSlice.PerCycle));
    if (this.oreTags.Length != 0 && !this.IsVacuum)
    {
      string str4 = str3 + "\n\n";
      string str5 = "";
      for (int index = 0; index < this.oreTags.Length; ++index)
      {
        Tag tag;
        // ISSUE: explicit constructor call
        ((Tag) ref tag).\u002Ector(this.oreTags[index]);
        str5 += tag.ProperName();
        if (index < this.oreTags.Length - 1)
          str5 += ", ";
      }
      str3 = str4 + string.Format((string) ELEMENTS.ELEMENTPROPERTIES, (object) str5);
    }
    if (this.attributeModifiers.Count > 0)
    {
      foreach (AttributeModifier attributeModifier in this.attributeModifiers)
      {
        string name = Db.Get().BuildingAttributes.Get(attributeModifier.AttributeId).Name;
        string formattedString = attributeModifier.GetFormattedString();
        str3 = str3 + "\n" + string.Format((string) DUPLICANTS.MODIFIERS.MODIFIER_FORMAT, (object) name, (object) formattedString);
      }
    }
    return str3;
  }

  public string Description() => this.description;

  public bool HasTag(Tag search_tag) => Tag.op_Equality(this.tag, search_tag) || Array.IndexOf<Tag>(this.oreTags, search_tag) != -1;

  public Tag GetMaterialCategoryTag() => this.materialCategory;

  public int CompareTo(Element other) => this.id - other.id;

  [Serializable]
  public enum State : byte
  {
    Vacuum = 0,
    Gas = 1,
    Liquid = 2,
    Solid = 3,
    Unbreakable = 4,
    Unstable = 8,
    TemperatureInsulated = 16, // 0x10
  }
}
