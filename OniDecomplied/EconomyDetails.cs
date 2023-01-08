// Decompiled with JetBrains decompiler
// Type: EconomyDetails
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EconomyDetails
{
  private List<EconomyDetails.Transformation> transformations = new List<EconomyDetails.Transformation>();
  private List<EconomyDetails.Resource> resources = new List<EconomyDetails.Resource>();
  public Dictionary<Element, float> startingBiomeAmounts = new Dictionary<Element, float>();
  public int startingBiomeCellCount;
  public EconomyDetails.Resource energyResource;
  public EconomyDetails.Resource heatResource;
  public EconomyDetails.Resource duplicantTimeResource;
  public EconomyDetails.Resource caloriesResource;
  public EconomyDetails.Resource fixedCaloriesResource;
  public EconomyDetails.Resource.Type massResourceType;
  public EconomyDetails.Resource.Type heatResourceType;
  public EconomyDetails.Resource.Type energyResourceType;
  public EconomyDetails.Resource.Type timeResourceType;
  public EconomyDetails.Resource.Type attributeResourceType;
  public EconomyDetails.Resource.Type caloriesResourceType;
  public EconomyDetails.Resource.Type amountResourceType;
  public EconomyDetails.Transformation.Type buildingTransformationType;
  public EconomyDetails.Transformation.Type foodTransformationType;
  public EconomyDetails.Transformation.Type plantTransformationType;
  public EconomyDetails.Transformation.Type creatureTransformationType;
  public EconomyDetails.Transformation.Type dupeTransformationType;
  public EconomyDetails.Transformation.Type referenceTransformationType;
  public EconomyDetails.Transformation.Type effectTransformationType;
  private const string GEYSER_ACTIVE_SUFFIX = "_ActiveOnly";
  public EconomyDetails.Transformation.Type geyserActivePeriodTransformationType;
  public EconomyDetails.Transformation.Type geyserLifetimeTransformationType;
  private static string debugTag = "CO2Scrubber";

  public EconomyDetails()
  {
    this.massResourceType = new EconomyDetails.Resource.Type("Mass", "kg");
    this.heatResourceType = new EconomyDetails.Resource.Type("Heat Energy", "kdtu");
    this.energyResourceType = new EconomyDetails.Resource.Type("Energy", "joules");
    this.timeResourceType = new EconomyDetails.Resource.Type("Time", "seconds");
    this.attributeResourceType = new EconomyDetails.Resource.Type("Attribute", "units");
    this.caloriesResourceType = new EconomyDetails.Resource.Type("Calories", "kcal");
    this.amountResourceType = new EconomyDetails.Resource.Type("Amount", "units");
    this.buildingTransformationType = new EconomyDetails.Transformation.Type("Building");
    this.foodTransformationType = new EconomyDetails.Transformation.Type("Food");
    this.plantTransformationType = new EconomyDetails.Transformation.Type("Plant");
    this.creatureTransformationType = new EconomyDetails.Transformation.Type("Creature");
    this.dupeTransformationType = new EconomyDetails.Transformation.Type("Duplicant");
    this.referenceTransformationType = new EconomyDetails.Transformation.Type("Reference");
    this.effectTransformationType = new EconomyDetails.Transformation.Type("Effect");
    this.geyserActivePeriodTransformationType = new EconomyDetails.Transformation.Type("GeyserActivePeriod");
    this.geyserLifetimeTransformationType = new EconomyDetails.Transformation.Type("GeyserLifetime");
    this.energyResource = this.CreateResource(TagManager.Create("Energy"), this.energyResourceType);
    this.heatResource = this.CreateResource(TagManager.Create("Heat"), this.heatResourceType);
    this.duplicantTimeResource = this.CreateResource(TagManager.Create("DupeTime"), this.timeResourceType);
    this.caloriesResource = this.CreateResource(new Tag(Db.Get().Amounts.Calories.deltaAttribute.Id), this.caloriesResourceType);
    this.fixedCaloriesResource = this.CreateResource(new Tag(Db.Get().Amounts.Calories.Id), this.caloriesResourceType);
    foreach (Element element in ElementLoader.elements)
      this.CreateResource(element);
    List<Tag> tagList = new List<Tag>();
    tagList.Add(GameTags.CombustibleLiquid);
    tagList.Add(GameTags.CombustibleGas);
    tagList.Add(GameTags.CombustibleSolid);
    foreach (Tag tag in tagList)
      this.CreateResource(tag, this.massResourceType);
    foreach (EdiblesManager.FoodInfo allFoodType in EdiblesManager.GetAllFoodTypes())
      this.CreateResource(TagExtensions.ToTag(allFoodType.Id), this.amountResourceType);
    this.GatherStartingBiomeAmounts();
    foreach (KPrefabID prefab in Assets.Prefabs)
    {
      this.CreateTransformation(prefab, prefab.PrefabTag);
      if (Object.op_Inequality((Object) ((Component) prefab).GetComponent<GeyserConfigurator>(), (Object) null))
        this.CreateTransformation(prefab, Tag.op_Implicit(prefab.PrefabTag.ToString() + "_ActiveOnly"));
    }
    foreach (Effect resource in Db.Get().effects.resources)
      this.CreateTransformation(resource);
    EconomyDetails.Transformation transformation1 = new EconomyDetails.Transformation(TagManager.Create("Duplicant"), this.dupeTransformationType, 1f);
    transformation1.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(GameTags.Oxygen), -0.1f));
    transformation1.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(GameTags.CarbonDioxide), 0.1f * Assets.GetPrefab(Tag.op_Implicit(MinionConfig.ID)).GetComponent<OxygenBreather>().O2toCO2conversion));
    transformation1.AddDelta(new EconomyDetails.Transformation.Delta(this.duplicantTimeResource, 0.875f));
    transformation1.AddDelta(new EconomyDetails.Transformation.Delta(this.caloriesResource, -1.66666675f));
    transformation1.AddDelta(new EconomyDetails.Transformation.Delta(this.CreateResource(new Tag(Db.Get().Amounts.Bladder.deltaAttribute.Id), this.amountResourceType), 0.166666672f));
    this.transformations.Add(transformation1);
    EconomyDetails.Transformation transformation2 = new EconomyDetails.Transformation(TagManager.Create("Electrolysis"), this.referenceTransformationType, 1f);
    transformation2.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(GameTags.Oxygen), 1.77777779f));
    transformation2.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(GameTags.Hydrogen), 0.222222224f));
    transformation2.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(GameTags.Water), -2f));
    this.transformations.Add(transformation2);
    EconomyDetails.Transformation transformation3 = new EconomyDetails.Transformation(TagManager.Create("MethaneCombustion"), this.referenceTransformationType, 1f);
    transformation3.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(GameTags.Methane), -1f));
    transformation3.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(GameTags.Oxygen), -4f));
    transformation3.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(GameTags.CarbonDioxide), 2.75f));
    transformation3.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(GameTags.Water), 2.25f));
    this.transformations.Add(transformation3);
    EconomyDetails.Transformation transformation4 = new EconomyDetails.Transformation(TagManager.Create("CoalCombustion"), this.referenceTransformationType, 1f);
    transformation4.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(GameTags.Carbon), -1f));
    transformation4.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(GameTags.Oxygen), -2.66666675f));
    transformation4.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(GameTags.CarbonDioxide), 3.66666675f));
    this.transformations.Add(transformation4);
  }

  private static void WriteProduct(StreamWriter o, string a, string b) => o.Write("\"=PRODUCT(" + a + ", " + b + ")\"");

  private static void WriteProduct(StreamWriter o, string a, string b, string c) => o.Write("\"=PRODUCT(" + a + ", " + b + ", " + c + ")\"");

  public void DumpTransformations(EconomyDetails.Scenario scenario, StreamWriter o)
  {
    List<EconomyDetails.Resource> used_resources = new List<EconomyDetails.Resource>();
    foreach (EconomyDetails.Transformation transformation in this.transformations)
    {
      if (scenario.IncludesTransformation(transformation))
      {
        foreach (EconomyDetails.Transformation.Delta delta in transformation.deltas)
        {
          if (!used_resources.Contains(delta.resource))
            used_resources.Add(delta.resource);
        }
      }
    }
    used_resources.Sort((Comparison<EconomyDetails.Resource>) ((x, y) =>
    {
      Tag tag = x.tag;
      string name1 = ((Tag) ref tag).Name;
      tag = y.tag;
      string name2 = ((Tag) ref tag).Name;
      return name1.CompareTo(name2);
    }));
    List<EconomyDetails.Ratio> ratioList = new List<EconomyDetails.Ratio>();
    ratioList.Add(new EconomyDetails.Ratio(this.GetResource(GameTags.Algae), this.GetResource(GameTags.Oxygen), false));
    ratioList.Add(new EconomyDetails.Ratio(this.energyResource, this.GetResource(GameTags.Oxygen), false));
    ratioList.Add(new EconomyDetails.Ratio(this.GetResource(GameTags.Oxygen), this.energyResource, false));
    ratioList.Add(new EconomyDetails.Ratio(this.GetResource(GameTags.Water), this.GetResource(GameTags.Oxygen), false));
    ratioList.Add(new EconomyDetails.Ratio(this.GetResource(GameTags.DirtyWater), this.caloriesResource, false));
    ratioList.Add(new EconomyDetails.Ratio(this.GetResource(GameTags.Water), this.caloriesResource, false));
    ratioList.Add(new EconomyDetails.Ratio(this.GetResource(GameTags.Fertilizer), this.caloriesResource, false));
    ratioList.Add(new EconomyDetails.Ratio(this.energyResource, this.CreateResource(new Tag(Db.Get().Amounts.Stress.deltaAttribute.Id), this.amountResourceType), true));
    ratioList.RemoveAll((Predicate<EconomyDetails.Ratio>) (x => !used_resources.Contains(x.input) || !used_resources.Contains(x.output)));
    o.Write("Id");
    o.Write(",Count");
    o.Write(",Type");
    o.Write(",Time(s)");
    int num1 = 4;
    foreach (EconomyDetails.Resource resource in used_resources)
    {
      StreamWriter streamWriter = o;
      string[] strArray = new string[5]
      {
        ", ",
        null,
        null,
        null,
        null
      };
      Tag tag = resource.tag;
      strArray[1] = ((Tag) ref tag).Name;
      strArray[2] = "(";
      strArray[3] = resource.type.unit;
      strArray[4] = ")";
      string str = string.Concat(strArray);
      streamWriter.Write(str);
      ++num1;
    }
    o.Write(",MassDelta");
    foreach (EconomyDetails.Ratio ratio in ratioList)
    {
      StreamWriter streamWriter = o;
      string[] strArray = new string[9];
      strArray[0] = ", ";
      Tag tag = ratio.output.tag;
      strArray[1] = ((Tag) ref tag).Name;
      strArray[2] = "(";
      strArray[3] = ratio.output.type.unit;
      strArray[4] = ")/";
      tag = ratio.input.tag;
      strArray[5] = ((Tag) ref tag).Name;
      strArray[6] = "(";
      strArray[7] = ratio.input.type.unit;
      strArray[8] = ")";
      string str = string.Concat(strArray);
      streamWriter.Write(str);
      ++num1;
    }
    string str1 = "B";
    o.Write("\n");
    int num2 = 1;
    this.transformations.Sort((Comparison<EconomyDetails.Transformation>) ((x, y) =>
    {
      Tag tag = x.tag;
      string name3 = ((Tag) ref tag).Name;
      tag = y.tag;
      string name4 = ((Tag) ref tag).Name;
      return name3.CompareTo(name4);
    }));
    for (int index = 0; index < this.transformations.Count; ++index)
    {
      EconomyDetails.Transformation transformation = this.transformations[index];
      if (scenario.IncludesTransformation(transformation))
        ++num2;
    }
    string c = "B" + (num2 + 4).ToString();
    int num3 = 1;
    for (int index = 0; index < this.transformations.Count; ++index)
    {
      EconomyDetails.Transformation transformation = this.transformations[index];
      if (scenario.IncludesTransformation(transformation))
      {
        if (Tag.op_Equality(transformation.tag, new Tag(EconomyDetails.debugTag)))
        {
          int num4 = 0 + 1;
        }
        ++num3;
        StreamWriter streamWriter1 = o;
        Tag tag = transformation.tag;
        string str2 = "\"" + ((Tag) ref tag).Name + "\"";
        streamWriter1.Write(str2);
        StreamWriter streamWriter2 = o;
        float num5 = scenario.GetCount(transformation.tag);
        string str3 = "," + num5.ToString();
        streamWriter2.Write(str3);
        o.Write(",\"" + transformation.type.id + "\"");
        if (!transformation.timeInvariant)
        {
          StreamWriter streamWriter3 = o;
          num5 = transformation.timeInSeconds;
          string str4 = ",\"" + num5.ToString("0.00") + "\"";
          streamWriter3.Write(str4);
        }
        else
          o.Write(",\"invariant\"");
        string a = str1 + num3.ToString();
        float num6 = 0.0f;
        bool flag = false;
        foreach (EconomyDetails.Resource resource in used_resources)
        {
          EconomyDetails.Transformation.Delta delta1 = (EconomyDetails.Transformation.Delta) null;
          foreach (EconomyDetails.Transformation.Delta delta2 in transformation.deltas)
          {
            if (Tag.op_Equality(delta2.resource.tag, resource.tag))
            {
              delta1 = delta2;
              break;
            }
          }
          o.Write(",");
          if (delta1 != null && (double) delta1.amount != 0.0)
          {
            if (delta1.resource.type == this.massResourceType)
            {
              flag = true;
              num6 += delta1.amount;
            }
            if (!transformation.timeInvariant)
            {
              float num7 = delta1.amount / transformation.timeInSeconds;
              EconomyDetails.WriteProduct(o, a, num7.ToString("0.00000"), c);
            }
            else
              EconomyDetails.WriteProduct(o, a, delta1.amount.ToString("0.00000"));
          }
        }
        o.Write(",");
        if (flag)
        {
          float num8 = num6 / transformation.timeInSeconds;
          EconomyDetails.WriteProduct(o, a, num8.ToString("0.00000"), c);
        }
        foreach (EconomyDetails.Ratio ratio in ratioList)
        {
          o.Write(", ");
          EconomyDetails.Transformation.Delta delta3 = transformation.GetDelta(ratio.input);
          EconomyDetails.Transformation.Delta delta4 = transformation.GetDelta(ratio.output);
          if (delta4 != null && delta3 != null && (double) delta3.amount < 0.0 && ((double) delta4.amount > 0.0 || ratio.allowNegativeOutput))
            o.Write(delta4.amount / Mathf.Abs(delta3.amount));
        }
        o.Write("\n");
      }
    }
    int num9 = 4;
    for (int index = 0; index < num1; ++index)
    {
      if (index >= num9 && index < num9 + used_resources.Count)
      {
        string str5 = ((char) (65 + index % 26)).ToString();
        int num10 = Mathf.FloorToInt((float) index / 26f);
        if (num10 > 0)
          str5 = ((char) (65 + num10 - 1)).ToString() + str5;
        o.Write("\"=SUM(" + str5 + "2: " + str5 + num2.ToString() + ")\"");
      }
      o.Write(",");
    }
    string str6 = "B" + (num2 + 5).ToString();
    o.Write("\n");
    o.Write("\nTiming:");
    StreamWriter streamWriter4 = o;
    float num11 = scenario.timeInSeconds;
    string str7 = "\nTimeInSeconds:," + num11.ToString();
    streamWriter4.Write(str7);
    StreamWriter streamWriter5 = o;
    num11 = 600f;
    string str8 = "\nSecondsPerCycle:," + num11.ToString();
    streamWriter5.Write(str8);
    o.Write("\nCycles:,=" + c + "/" + str6);
  }

  public EconomyDetails.Resource CreateResource(Tag tag, EconomyDetails.Resource.Type resource_type)
  {
    foreach (EconomyDetails.Resource resource in this.resources)
    {
      if (Tag.op_Equality(resource.tag, tag))
        return resource;
    }
    EconomyDetails.Resource resource1 = new EconomyDetails.Resource(tag, resource_type);
    this.resources.Add(resource1);
    return resource1;
  }

  public EconomyDetails.Resource CreateResource(Element element) => this.CreateResource(element.tag, this.massResourceType);

  public EconomyDetails.Transformation CreateTransformation(Effect effect)
  {
    EconomyDetails.Transformation transformation = new EconomyDetails.Transformation(new Tag(effect.Id), this.effectTransformationType, 1f);
    foreach (AttributeModifier selfModifier in effect.SelfModifiers)
    {
      EconomyDetails.Resource resource = this.CreateResource(new Tag(selfModifier.AttributeId), this.attributeResourceType);
      transformation.AddDelta(new EconomyDetails.Transformation.Delta(resource, selfModifier.Value));
    }
    this.transformations.Add(transformation);
    return transformation;
  }

  public EconomyDetails.Transformation GetTransformation(Tag tag)
  {
    foreach (EconomyDetails.Transformation transformation in this.transformations)
    {
      if (Tag.op_Equality(transformation.tag, tag))
        return transformation;
    }
    return (EconomyDetails.Transformation) null;
  }

  public EconomyDetails.Transformation CreateTransformation(KPrefabID prefab_id, Tag tag)
  {
    if (Tag.op_Equality(tag, new Tag(EconomyDetails.debugTag)))
    {
      int num = 0 + 1;
    }
    Building component1 = ((Component) prefab_id).GetComponent<Building>();
    ElementConverter component2 = ((Component) prefab_id).GetComponent<ElementConverter>();
    EnergyConsumer component3 = ((Component) prefab_id).GetComponent<EnergyConsumer>();
    ElementConsumer component4 = ((Component) prefab_id).GetComponent<ElementConsumer>();
    BuildingElementEmitter component5 = ((Component) prefab_id).GetComponent<BuildingElementEmitter>();
    Generator component6 = ((Component) prefab_id).GetComponent<Generator>();
    EnergyGenerator component7 = ((Component) prefab_id).GetComponent<EnergyGenerator>();
    ManualGenerator component8 = ((Component) prefab_id).GetComponent<ManualGenerator>();
    ManualDeliveryKG[] components = ((Component) prefab_id).GetComponents<ManualDeliveryKG>();
    StateMachineController component9 = ((Component) prefab_id).GetComponent<StateMachineController>();
    Edible component10 = ((Component) prefab_id).GetComponent<Edible>();
    Crop component11 = ((Component) prefab_id).GetComponent<Crop>();
    Uprootable component12 = ((Component) prefab_id).GetComponent<Uprootable>();
    ComplexRecipe complexRecipe = ComplexRecipeManager.Get().recipes.Find((Predicate<ComplexRecipe>) (r => Tag.op_Equality(r.FirstResult, prefab_id.PrefabTag)));
    List<FertilizationMonitor.Def> defList1 = (List<FertilizationMonitor.Def>) null;
    List<IrrigationMonitor.Def> defList2 = (List<IrrigationMonitor.Def>) null;
    GeyserConfigurator component13 = ((Component) prefab_id).GetComponent<GeyserConfigurator>();
    Toilet component14 = ((Component) prefab_id).GetComponent<Toilet>();
    FlushToilet component15 = ((Component) prefab_id).GetComponent<FlushToilet>();
    RelaxationPoint component16 = ((Component) prefab_id).GetComponent<RelaxationPoint>();
    CreatureCalorieMonitor.Def def1 = ((Component) prefab_id).gameObject.GetDef<CreatureCalorieMonitor.Def>();
    if (Object.op_Inequality((Object) component9, (Object) null))
    {
      defList1 = component9.GetDefs<FertilizationMonitor.Def>();
      defList2 = component9.GetDefs<IrrigationMonitor.Def>();
    }
    EconomyDetails.Transformation transformation = (EconomyDetails.Transformation) null;
    float time_in_seconds = 1f;
    if (Object.op_Inequality((Object) component10, (Object) null))
      transformation = new EconomyDetails.Transformation(tag, this.foodTransformationType, time_in_seconds, complexRecipe != null);
    else if (Object.op_Inequality((Object) component2, (Object) null) || Object.op_Inequality((Object) component3, (Object) null) || Object.op_Inequality((Object) component4, (Object) null) || Object.op_Inequality((Object) component5, (Object) null) || Object.op_Inequality((Object) component6, (Object) null) || Object.op_Inequality((Object) component7, (Object) null) || Object.op_Inequality((Object) component12, (Object) null) || Object.op_Inequality((Object) component13, (Object) null) || Object.op_Inequality((Object) component14, (Object) null) || Object.op_Inequality((Object) component15, (Object) null) || Object.op_Inequality((Object) component16, (Object) null) || def1 != null)
    {
      if (Object.op_Inequality((Object) component12, (Object) null) || Object.op_Inequality((Object) component11, (Object) null))
      {
        if (Object.op_Inequality((Object) component11, (Object) null))
          time_in_seconds = component11.cropVal.cropDuration;
        transformation = new EconomyDetails.Transformation(tag, this.plantTransformationType, time_in_seconds);
      }
      else if (def1 != null)
        transformation = new EconomyDetails.Transformation(tag, this.creatureTransformationType, time_in_seconds);
      else if (Object.op_Inequality((Object) component13, (Object) null))
      {
        GeyserConfigurator.GeyserInstanceConfiguration instanceConfiguration = new GeyserConfigurator.GeyserInstanceConfiguration()
        {
          typeId = component13.presetType,
          rateRoll = 0.5f,
          iterationLengthRoll = 0.5f,
          iterationPercentRoll = 0.5f,
          yearLengthRoll = 0.5f,
          yearPercentRoll = 0.5f
        };
        if (((Tag) ref tag).Name.Contains("_ActiveOnly"))
        {
          float iterationLength = instanceConfiguration.GetIterationLength();
          transformation = new EconomyDetails.Transformation(tag, this.geyserActivePeriodTransformationType, iterationLength);
        }
        else
        {
          float yearLength = instanceConfiguration.GetYearLength();
          transformation = new EconomyDetails.Transformation(tag, this.geyserLifetimeTransformationType, yearLength);
        }
      }
      else
      {
        if (Object.op_Inequality((Object) component14, (Object) null) || Object.op_Inequality((Object) component15, (Object) null))
          time_in_seconds = 600f;
        transformation = new EconomyDetails.Transformation(tag, this.buildingTransformationType, time_in_seconds);
      }
    }
    if (transformation != null)
    {
      if (Object.op_Inequality((Object) component2, (Object) null) && component2.consumedElements != null)
      {
        foreach (ElementConverter.ConsumedElement consumedElement in component2.consumedElements)
        {
          EconomyDetails.Resource resource = this.CreateResource(consumedElement.Tag, this.massResourceType);
          transformation.AddDelta(new EconomyDetails.Transformation.Delta(resource, -consumedElement.MassConsumptionRate));
        }
        if (component2.outputElements != null)
        {
          foreach (ElementConverter.OutputElement outputElement in component2.outputElements)
          {
            EconomyDetails.Resource resource = this.CreateResource(ElementLoader.FindElementByHash(outputElement.elementHash).tag, this.massResourceType);
            transformation.AddDelta(new EconomyDetails.Transformation.Delta(resource, outputElement.massGenerationRate));
          }
        }
      }
      if (Object.op_Inequality((Object) component4, (Object) null) && Object.op_Equality((Object) component7, (Object) null) && (Object.op_Equality((Object) component2, (Object) null) || Object.op_Inequality((Object) ((Component) prefab_id).GetComponent<AlgaeHabitat>(), (Object) null)))
      {
        EconomyDetails.Resource resource = this.GetResource(ElementLoader.FindElementByHash(component4.elementToConsume).tag);
        transformation.AddDelta(new EconomyDetails.Transformation.Delta(resource, -component4.consumptionRate));
      }
      if (Object.op_Inequality((Object) component3, (Object) null))
        transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.energyResource, -component3.WattsNeededWhenActive));
      if (Object.op_Inequality((Object) component5, (Object) null))
        transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(component5.element), component5.emitRate));
      if (Object.op_Inequality((Object) component6, (Object) null))
        transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.energyResource, ((Component) component6).GetComponent<Building>().Def.GeneratorWattageRating));
      if (Object.op_Inequality((Object) component7, (Object) null))
      {
        if (component7.formula.inputs != null)
        {
          foreach (EnergyGenerator.InputItem input in component7.formula.inputs)
            transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(input.tag), -input.consumptionRate));
        }
        if (component7.formula.outputs != null)
        {
          foreach (EnergyGenerator.OutputItem output in component7.formula.outputs)
            transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(output.element), output.creationRate));
        }
      }
      if (Object.op_Implicit((Object) component1))
      {
        BuildingDef def2 = component1.Def;
        transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.heatResource, def2.SelfHeatKilowattsWhenActive + def2.ExhaustKilowattsWhenActive));
      }
      if (Object.op_Implicit((Object) component8))
        transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.duplicantTimeResource, -1f));
      if (Object.op_Implicit((Object) component10))
      {
        EdiblesManager.FoodInfo foodInfo = component10.FoodInfo;
        transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.fixedCaloriesResource, foodInfo.CaloriesPerUnit * (1f / 1000f)));
        ComplexRecipeManager.Get().recipes.Find((Predicate<ComplexRecipe>) (a => Tag.op_Equality(a.FirstResult, tag)));
      }
      if (Object.op_Inequality((Object) component11, (Object) null))
      {
        EconomyDetails.Resource resource = this.CreateResource(TagManager.Create(component11.cropVal.cropId), this.amountResourceType);
        float numProduced = (float) component11.cropVal.numProduced;
        transformation.AddDelta(new EconomyDetails.Transformation.Delta(resource, numProduced));
        GameObject prefab = Assets.GetPrefab(new Tag(component11.cropVal.cropId));
        if (Object.op_Inequality((Object) prefab, (Object) null))
        {
          Edible component17 = prefab.GetComponent<Edible>();
          if (Object.op_Inequality((Object) component17, (Object) null))
            transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.caloriesResource, (float) ((double) component17.FoodInfo.CaloriesPerUnit * (double) numProduced * (1.0 / 1000.0))));
        }
      }
      if (complexRecipe != null)
      {
        foreach (ComplexRecipe.RecipeElement ingredient in complexRecipe.ingredients)
        {
          this.CreateResource(ingredient.material, this.amountResourceType);
          transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(ingredient.material), -ingredient.amount));
        }
        foreach (ComplexRecipe.RecipeElement result in complexRecipe.results)
        {
          this.CreateResource(result.material, this.amountResourceType);
          transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(result.material), result.amount));
        }
      }
      if (components != null)
      {
        for (int index = 0; index < components.Length; ++index)
          transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.duplicantTimeResource, -0.1f * transformation.timeInSeconds));
      }
      if (defList1 != null && defList1.Count > 0)
      {
        foreach (FertilizationMonitor.Def def3 in defList1)
        {
          foreach (PlantElementAbsorber.ConsumeInfo consumedElement in def3.consumedElements)
          {
            EconomyDetails.Resource resource = this.CreateResource(consumedElement.tag, this.massResourceType);
            transformation.AddDelta(new EconomyDetails.Transformation.Delta(resource, -consumedElement.massConsumptionRate * transformation.timeInSeconds));
          }
        }
      }
      if (defList2 != null && defList2.Count > 0)
      {
        foreach (IrrigationMonitor.Def def4 in defList2)
        {
          foreach (PlantElementAbsorber.ConsumeInfo consumedElement in def4.consumedElements)
          {
            EconomyDetails.Resource resource = this.CreateResource(consumedElement.tag, this.massResourceType);
            transformation.AddDelta(new EconomyDetails.Transformation.Delta(resource, -consumedElement.massConsumptionRate * transformation.timeInSeconds));
          }
        }
      }
      if (Object.op_Inequality((Object) component13, (Object) null))
      {
        GeyserConfigurator.GeyserInstanceConfiguration instanceConfiguration = new GeyserConfigurator.GeyserInstanceConfiguration()
        {
          typeId = component13.presetType,
          rateRoll = 0.5f,
          iterationLengthRoll = 0.5f,
          iterationPercentRoll = 0.5f,
          yearLengthRoll = 0.5f,
          yearPercentRoll = 0.5f
        };
        if (((Tag) ref tag).Name.Contains("_ActiveOnly"))
        {
          float amount = instanceConfiguration.GetMassPerCycle() / 600f * instanceConfiguration.GetIterationLength();
          transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.CreateResource(instanceConfiguration.GetElement().CreateTag(), this.massResourceType), amount));
        }
        else
        {
          float amount = instanceConfiguration.GetMassPerCycle() / 600f * instanceConfiguration.GetYearLength() * instanceConfiguration.GetYearPercent();
          transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.CreateResource(instanceConfiguration.GetElement().CreateTag(), this.massResourceType), amount));
        }
      }
      if (Object.op_Inequality((Object) component14, (Object) null))
      {
        transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.CreateResource(new Tag(Db.Get().Amounts.Bladder.deltaAttribute.Id), this.amountResourceType), -0.166666672f));
        transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(SimHashes.Dirt), -component14.solidWastePerUse.mass));
        transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(component14.solidWastePerUse.elementID), component14.solidWastePerUse.mass));
      }
      if (Object.op_Inequality((Object) component15, (Object) null))
      {
        transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.CreateResource(new Tag(Db.Get().Amounts.Bladder.deltaAttribute.Id), this.amountResourceType), -0.166666672f));
        transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(SimHashes.Water), -component15.massConsumedPerUse));
        transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(SimHashes.DirtyWater), component15.massEmittedPerUse));
      }
      if (Object.op_Inequality((Object) component16, (Object) null))
      {
        foreach (AttributeModifier selfModifier in component16.CreateEffect().SelfModifiers)
        {
          EconomyDetails.Resource resource = this.CreateResource(new Tag(selfModifier.AttributeId), this.attributeResourceType);
          transformation.AddDelta(new EconomyDetails.Transformation.Delta(resource, selfModifier.Value));
        }
      }
      if (def1 != null)
        this.CollectDietTransformations(prefab_id);
      this.transformations.Add(transformation);
    }
    return transformation;
  }

  private void CollectDietTransformations(KPrefabID prefab_id)
  {
    Trait trait = Db.Get().traits.Get(((Component) prefab_id).GetComponent<Modifiers>().initialTraits[0]);
    CreatureCalorieMonitor.Def def1 = ((Component) prefab_id).gameObject.GetDef<CreatureCalorieMonitor.Def>();
    WildnessMonitor.Def def2 = ((Component) prefab_id).gameObject.GetDef<WildnessMonitor.Def>();
    List<AttributeModifier> attributeModifierList = new List<AttributeModifier>();
    attributeModifierList.AddRange((IEnumerable<AttributeModifier>) trait.SelfModifiers);
    attributeModifierList.AddRange((IEnumerable<AttributeModifier>) def2.tameEffect.SelfModifiers);
    float amount1 = 0.0f;
    float num1 = 0.0f;
    foreach (AttributeModifier attributeModifier in attributeModifierList)
    {
      if (attributeModifier.AttributeId == Db.Get().Amounts.Calories.maxAttribute.Id)
        amount1 = attributeModifier.Value;
      if (attributeModifier.AttributeId == Db.Get().Amounts.Calories.deltaAttribute.Id)
        num1 = attributeModifier.Value;
    }
    foreach (Diet.Info info in def1.diet.infos)
    {
      foreach (Tag consumedTag in info.consumedTags)
      {
        float time_in_seconds = Mathf.Abs(amount1 / num1);
        float num2 = amount1 / info.caloriesPerKg;
        float amount2 = num2 * info.producedConversionRate;
        EconomyDetails.Transformation transformation = new EconomyDetails.Transformation(new Tag(((Tag) ref prefab_id.PrefabTag).Name + "Diet" + ((Tag) ref consumedTag).Name), this.creatureTransformationType, time_in_seconds);
        transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.CreateResource(consumedTag, this.massResourceType), -num2));
        transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.CreateResource(new Tag(info.producedElement.ToString()), this.massResourceType), amount2));
        transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.caloriesResource, amount1));
        this.transformations.Add(transformation);
      }
    }
  }

  private static void CollectDietScenarios(List<EconomyDetails.Scenario> scenarios)
  {
    EconomyDetails.Scenario scenario1 = new EconomyDetails.Scenario("diets/all", 0.0f, (Func<EconomyDetails.Transformation, bool>) null);
    foreach (KPrefabID prefab in Assets.Prefabs)
    {
      CreatureCalorieMonitor.Def def = ((Component) prefab).gameObject.GetDef<CreatureCalorieMonitor.Def>();
      if (def != null)
      {
        EconomyDetails.Scenario scenario2 = new EconomyDetails.Scenario("diets/" + ((Object) prefab).name, 0.0f, (Func<EconomyDetails.Transformation, bool>) null);
        foreach (Diet.Info info in def.diet.infos)
        {
          foreach (Tag consumedTag in info.consumedTags)
          {
            Tag tag = Tag.op_Implicit(((Tag) ref prefab.PrefabTag).Name + "Diet" + ((Tag) ref consumedTag).Name);
            scenario2.AddEntry(new EconomyDetails.Scenario.Entry(tag, 1f));
            scenario1.AddEntry(new EconomyDetails.Scenario.Entry(tag, 1f));
          }
        }
        scenarios.Add(scenario2);
      }
    }
    scenarios.Add(scenario1);
  }

  public void GatherStartingBiomeAmounts()
  {
    for (int i = 0; i < Grid.CellCount; ++i)
    {
      if ((int) World.Instance.zoneRenderData.worldZoneTypes[i] == 3)
      {
        Element key = Grid.Element[i];
        float num = 0.0f;
        this.startingBiomeAmounts.TryGetValue(key, out num);
        this.startingBiomeAmounts[key] = num + Grid.Mass[i];
        ++this.startingBiomeCellCount;
      }
    }
  }

  public EconomyDetails.Resource GetResource(SimHashes element) => this.GetResource(ElementLoader.FindElementByHash(element).tag);

  public EconomyDetails.Resource GetResource(Tag tag)
  {
    foreach (EconomyDetails.Resource resource in this.resources)
    {
      if (Tag.op_Equality(resource.tag, tag))
        return resource;
    }
    DebugUtil.LogErrorArgs(new object[2]
    {
      (object) "Found a tag without a matching resource!",
      (object) tag
    });
    return (EconomyDetails.Resource) null;
  }

  private float GetDupeBreathingPerSecond(EconomyDetails details) => details.GetTransformation(TagManager.Create("Duplicant")).GetDelta(details.GetResource(GameTags.Oxygen)).amount;

  private EconomyDetails.BiomeTransformation CreateBiomeTransformationFromTransformation(
    EconomyDetails details,
    Tag transformation_tag,
    Tag input_resource_tag,
    Tag output_resource_tag)
  {
    EconomyDetails.Resource resource1 = details.GetResource(input_resource_tag);
    EconomyDetails.Resource resource2 = details.GetResource(output_resource_tag);
    EconomyDetails.Transformation transformation = details.GetTransformation(transformation_tag);
    float num1 = transformation.GetDelta(resource2).amount / -transformation.GetDelta(resource1).amount;
    float num2 = this.GetDupeBreathingPerSecond(details) * 600f;
    return new EconomyDetails.BiomeTransformation(TagExtensions.ToTag(((Tag) ref transformation_tag).Name + ((Tag) ref input_resource_tag).Name + "Cycles"), resource1, num1 / -num2);
  }

  private static void DumpEconomyDetails()
  {
    Debug.Log((object) "Starting Economy Details Dump...");
    EconomyDetails details = new EconomyDetails();
    List<EconomyDetails.Scenario> scenarios = new List<EconomyDetails.Scenario>();
    scenarios.Add(new EconomyDetails.Scenario("default", 1f, (Func<EconomyDetails.Transformation, bool>) (t => true)));
    scenarios.Add(new EconomyDetails.Scenario("all_buildings", 1f, (Func<EconomyDetails.Transformation, bool>) (t => t.type == details.buildingTransformationType)));
    scenarios.Add(new EconomyDetails.Scenario("all_plants", 1f, (Func<EconomyDetails.Transformation, bool>) (t => t.type == details.plantTransformationType)));
    scenarios.Add(new EconomyDetails.Scenario("all_creatures", 1f, (Func<EconomyDetails.Transformation, bool>) (t => t.type == details.creatureTransformationType)));
    scenarios.Add(new EconomyDetails.Scenario("all_stress", 1f, (Func<EconomyDetails.Transformation, bool>) (t => t.GetDelta(details.GetResource(new Tag(Db.Get().Amounts.Stress.deltaAttribute.Id))) != null)));
    scenarios.Add(new EconomyDetails.Scenario("all_foods", 1f, (Func<EconomyDetails.Transformation, bool>) (t => t.type == details.foodTransformationType)));
    scenarios.Add(new EconomyDetails.Scenario("geysers/geysers_active_period_only", 1f, (Func<EconomyDetails.Transformation, bool>) (t => t.type == details.geyserActivePeriodTransformationType)));
    scenarios.Add(new EconomyDetails.Scenario("geysers/geysers_whole_lifetime", 1f, (Func<EconomyDetails.Transformation, bool>) (t => t.type == details.geyserLifetimeTransformationType)));
    EconomyDetails.Scenario scenario1 = new EconomyDetails.Scenario("oxygen/algae_distillery", 0.0f, (Func<EconomyDetails.Transformation, bool>) null);
    scenario1.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("AlgaeDistillery"), 3f));
    scenario1.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("AlgaeHabitat"), 22f));
    scenario1.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("Duplicant"), 9f));
    scenario1.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("WaterPurifier"), 1f));
    scenarios.Add(scenario1);
    EconomyDetails.Scenario scenario2 = new EconomyDetails.Scenario("oxygen/algae_habitat_electrolyzer", 0.0f, (Func<EconomyDetails.Transformation, bool>) null);
    scenario2.AddEntry(new EconomyDetails.Scenario.Entry(Tag.op_Implicit("AlgaeHabitat"), 1f));
    scenario2.AddEntry(new EconomyDetails.Scenario.Entry(Tag.op_Implicit("Duplicant"), 1f));
    scenario2.AddEntry(new EconomyDetails.Scenario.Entry(Tag.op_Implicit("Electrolyzer"), 1f));
    scenarios.Add(scenario2);
    EconomyDetails.Scenario scenario3 = new EconomyDetails.Scenario("oxygen/electrolyzer", 0.0f, (Func<EconomyDetails.Transformation, bool>) null);
    scenario3.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("Electrolyzer"), 1f));
    scenario3.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("LiquidPump"), 1f));
    scenario3.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("Duplicant"), 9f));
    scenario3.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("HydrogenGenerator"), 1f));
    scenario3.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("GasPump"), 1f));
    scenarios.Add(scenario3);
    EconomyDetails.Scenario scenario4 = new EconomyDetails.Scenario("purifiers/methane_generator", 0.0f, (Func<EconomyDetails.Transformation, bool>) null);
    scenario4.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("MethaneGenerator"), 1f));
    scenario4.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("FertilizerMaker"), 3f));
    scenario4.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("Electrolyzer"), 1f));
    scenario4.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("GasPump"), 1f));
    scenario4.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("LiquidPump"), 2f));
    scenario4.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("HydrogenGenerator"), 1f));
    scenario4.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("PrickleFlower"), 0.0f));
    scenarios.Add(scenario4);
    EconomyDetails.Scenario scenario5 = new EconomyDetails.Scenario("purifiers/water_purifier", 0.0f, (Func<EconomyDetails.Transformation, bool>) null);
    scenario5.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("WaterPurifier"), 1f));
    scenario5.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("Compost"), 2f));
    scenario5.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("Electrolyzer"), 1f));
    scenario5.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("LiquidPump"), 2f));
    scenario5.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("GasPump"), 1f));
    scenario5.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("HydrogenGenerator"), 1f));
    scenario5.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("PrickleFlower"), 29f));
    scenarios.Add(scenario5);
    EconomyDetails.Scenario scenario6 = new EconomyDetails.Scenario("energy/petroleum_generator", 0.0f, (Func<EconomyDetails.Transformation, bool>) null);
    scenario6.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("PetroleumGenerator"), 1f));
    scenario6.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("OilRefinery"), 1f));
    scenario6.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("WaterPurifier"), 1f));
    scenario6.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("LiquidPump"), 1f));
    scenario6.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("GasPump"), 1f));
    scenario6.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("CO2Scrubber"), 1f));
    scenario6.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("MethaneGenerator"), 1f));
    scenarios.Add(scenario6);
    EconomyDetails.Scenario scenario7 = new EconomyDetails.Scenario("energy/coal_generator", 0.0f, (Func<EconomyDetails.Transformation, bool>) (t =>
    {
      Tag tag = t.tag;
      return ((Tag) ref tag).Name.Contains("Hatch");
    }));
    scenario7.AddEntry(new EconomyDetails.Scenario.Entry(Tag.op_Implicit("Generator"), 1f));
    scenarios.Add(scenario7);
    EconomyDetails.Scenario scenario8 = new EconomyDetails.Scenario("waste/outhouse", 0.0f, (Func<EconomyDetails.Transformation, bool>) null);
    scenario8.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("Outhouse"), 1f));
    scenario8.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("Compost"), 1f));
    scenarios.Add(scenario8);
    EconomyDetails.Scenario scenario9 = new EconomyDetails.Scenario("stress/massage_table", 0.0f, (Func<EconomyDetails.Transformation, bool>) null);
    scenario9.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("MassageTable"), 1f));
    scenario9.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("ManualGenerator"), 1f));
    scenarios.Add(scenario9);
    EconomyDetails.Scenario scenario10 = new EconomyDetails.Scenario("waste/flush_toilet", 0.0f, (Func<EconomyDetails.Transformation, bool>) null);
    scenario10.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("FlushToilet"), 1f));
    scenario10.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("WaterPurifier"), 1f));
    scenario10.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("LiquidPump"), 1f));
    scenario10.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("FertilizerMaker"), 1f));
    scenarios.Add(scenario10);
    EconomyDetails.CollectDietScenarios(scenarios);
    foreach (EconomyDetails.Transformation transformation in details.transformations)
    {
      EconomyDetails.Transformation transformation_iter = transformation;
      Tag tag = transformation.tag;
      EconomyDetails.Scenario scenario11 = new EconomyDetails.Scenario("transformations/" + ((Tag) ref tag).Name, 1f, (Func<EconomyDetails.Transformation, bool>) (t => transformation_iter == t));
      scenarios.Add(scenario11);
    }
    foreach (EconomyDetails.Transformation transformation1 in details.transformations)
    {
      Tag tag = transformation1.tag;
      EconomyDetails.Scenario scenario12 = new EconomyDetails.Scenario("transformation_groups/" + ((Tag) ref tag).Name, 0.0f, (Func<EconomyDetails.Transformation, bool>) null);
      scenario12.AddEntry(new EconomyDetails.Scenario.Entry(transformation1.tag, 1f));
      foreach (EconomyDetails.Transformation transformation2 in details.transformations)
      {
        bool flag = false;
        foreach (EconomyDetails.Transformation.Delta delta1 in transformation1.deltas)
        {
          if (delta1.resource.type != details.energyResourceType)
          {
            foreach (EconomyDetails.Transformation.Delta delta2 in transformation2.deltas)
            {
              if (delta1.resource == delta2.resource)
              {
                scenario12.AddEntry(new EconomyDetails.Scenario.Entry(transformation2.tag, 0.0f));
                flag = true;
                break;
              }
            }
            if (flag)
              break;
          }
        }
      }
      scenarios.Add(scenario12);
    }
    foreach (EdiblesManager.FoodInfo allFoodType in EdiblesManager.GetAllFoodTypes())
    {
      EconomyDetails.Scenario scenario13 = new EconomyDetails.Scenario("food/" + allFoodType.Id, 0.0f, (Func<EconomyDetails.Transformation, bool>) null);
      Tag tag1 = TagManager.Create(allFoodType.Id);
      scenario13.AddEntry(new EconomyDetails.Scenario.Entry(tag1, 1f));
      scenario13.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("Duplicant"), 1f));
      List<Tag> tagList = new List<Tag>();
      tagList.Add(tag1);
      while (tagList.Count > 0)
      {
        Tag tag = tagList[0];
        tagList.RemoveAt(0);
        ComplexRecipe complexRecipe = ComplexRecipeManager.Get().recipes.Find((Predicate<ComplexRecipe>) (a => Tag.op_Equality(a.FirstResult, tag)));
        if (complexRecipe != null)
        {
          foreach (ComplexRecipe.RecipeElement ingredient in complexRecipe.ingredients)
          {
            scenario13.AddEntry(new EconomyDetails.Scenario.Entry(ingredient.material, 1f));
            tagList.Add(ingredient.material);
          }
        }
        foreach (KPrefabID prefab in Assets.Prefabs)
        {
          Crop component = ((Component) prefab).GetComponent<Crop>();
          if (Object.op_Inequality((Object) component, (Object) null) && component.cropVal.cropId == ((Tag) ref tag).Name)
          {
            scenario13.AddEntry(new EconomyDetails.Scenario.Entry(prefab.PrefabTag, 1f));
            tagList.Add(prefab.PrefabTag);
          }
        }
      }
      scenarios.Add(scenario13);
    }
    if (!Directory.Exists("assets/Tuning/Economy"))
      Directory.CreateDirectory("assets/Tuning/Economy");
    foreach (EconomyDetails.Scenario scenario14 in scenarios)
    {
      string path = "assets/Tuning/Economy/" + scenario14.name + ".csv";
      if (!Directory.Exists(System.IO.Path.GetDirectoryName(path)))
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
      using (StreamWriter o = new StreamWriter(path))
        details.DumpTransformations(scenario14, o);
    }
    float breathingPerSecond = details.GetDupeBreathingPerSecond(details);
    List<EconomyDetails.BiomeTransformation> biomeTransformationList = new List<EconomyDetails.BiomeTransformation>();
    biomeTransformationList.Add(details.CreateBiomeTransformationFromTransformation(details, TagExtensions.ToTag("MineralDeoxidizer"), GameTags.Algae, GameTags.Oxygen));
    biomeTransformationList.Add(details.CreateBiomeTransformationFromTransformation(details, TagExtensions.ToTag("AlgaeHabitat"), GameTags.Algae, GameTags.Oxygen));
    biomeTransformationList.Add(details.CreateBiomeTransformationFromTransformation(details, TagExtensions.ToTag("AlgaeHabitat"), GameTags.Water, GameTags.Oxygen));
    biomeTransformationList.Add(details.CreateBiomeTransformationFromTransformation(details, TagExtensions.ToTag("Electrolyzer"), GameTags.Water, GameTags.Oxygen));
    biomeTransformationList.Add(new EconomyDetails.BiomeTransformation(TagExtensions.ToTag("StartingOxygenCycles"), details.GetResource(GameTags.Oxygen), (float) (1.0 / -((double) breathingPerSecond * 600.0))));
    biomeTransformationList.Add(new EconomyDetails.BiomeTransformation(TagExtensions.ToTag("StartingOxyliteCycles"), details.CreateResource(GameTags.OxyRock, details.massResourceType), (float) (1.0 / -((double) breathingPerSecond * 600.0))));
    string path1 = "assets/Tuning/Economy/biomes/starting_amounts.csv";
    if (!Directory.Exists(System.IO.Path.GetDirectoryName(path1)))
      Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path1));
    using (StreamWriter streamWriter = new StreamWriter(path1))
    {
      streamWriter.Write("Resource,Amount");
      foreach (EconomyDetails.BiomeTransformation biomeTransformation in biomeTransformationList)
        streamWriter.Write("," + biomeTransformation.tag.ToString());
      streamWriter.Write("\n");
      streamWriter.Write("Cells, " + details.startingBiomeCellCount.ToString() + "\n");
      foreach (KeyValuePair<Element, float> startingBiomeAmount in details.startingBiomeAmounts)
      {
        streamWriter.Write(startingBiomeAmount.Key.id.ToString() + ", " + startingBiomeAmount.Value.ToString());
        foreach (EconomyDetails.BiomeTransformation biomeTransformation in biomeTransformationList)
        {
          streamWriter.Write(",");
          Element key = startingBiomeAmount.Key;
          double amount = (double) startingBiomeAmount.Value;
          float num = biomeTransformation.Transform(key, (float) amount);
          if ((double) num > 0.0)
            streamWriter.Write(num);
        }
        streamWriter.Write("\n");
      }
    }
    Debug.Log((object) "Completed economy details dump!!");
  }

  private static void DumpNameMapping()
  {
    if (!Directory.Exists("assets/Tuning/Economy"))
      Directory.CreateDirectory("assets/Tuning/Economy");
    using (StreamWriter streamWriter = new StreamWriter("assets/Tuning/Economy/name_mapping.csv"))
    {
      streamWriter.Write("Game Name, Prefab Name, Anim Files\n");
      foreach (KPrefabID prefab in Assets.Prefabs)
      {
        string str = TagManager.StripLinkFormatting(((Component) prefab).GetProperName());
        Tag tag = prefab.PrefabID();
        if (!Util.IsNullOrWhiteSpace(str) && !((Tag) ref tag).Name.Contains("UnderConstruction") && !((Tag) ref tag).Name.Contains("Preview"))
        {
          streamWriter.Write(str);
          streamWriter.Write("," + tag.ToString());
          KAnimControllerBase component = ((Component) prefab).GetComponent<KAnimControllerBase>();
          if (Object.op_Inequality((Object) component, (Object) null))
          {
            foreach (KAnimFile animFile in component.AnimFiles)
              streamWriter.Write("," + ((Object) animFile).name);
          }
          else
            streamWriter.Write(",");
          streamWriter.Write("\n");
        }
      }
    }
  }

  public class Resource
  {
    public Tag tag { get; private set; }

    public EconomyDetails.Resource.Type type { get; private set; }

    public Resource(Tag tag, EconomyDetails.Resource.Type type)
    {
      this.tag = tag;
      this.type = type;
    }

    public class Type
    {
      public string id { get; private set; }

      public string unit { get; private set; }

      public Type(string id, string unit)
      {
        this.id = id;
        this.unit = unit;
      }
    }
  }

  public class BiomeTransformation
  {
    public Tag tag { get; private set; }

    public EconomyDetails.Resource resource { get; private set; }

    public float ratio { get; private set; }

    public BiomeTransformation(Tag tag, EconomyDetails.Resource resource, float ratio)
    {
      this.tag = tag;
      this.resource = resource;
      this.ratio = ratio;
    }

    public float Transform(Element element, float amount) => Tag.op_Equality(this.resource.tag, element.tag) ? this.ratio * amount : 0.0f;
  }

  public class Ratio
  {
    public EconomyDetails.Resource input { get; private set; }

    public EconomyDetails.Resource output { get; private set; }

    public bool allowNegativeOutput { get; private set; }

    public Ratio(
      EconomyDetails.Resource input,
      EconomyDetails.Resource output,
      bool allow_negative_output)
    {
      this.input = input;
      this.output = output;
      this.allowNegativeOutput = allow_negative_output;
    }
  }

  public class Scenario
  {
    private Func<EconomyDetails.Transformation, bool> filter;
    private List<EconomyDetails.Scenario.Entry> entries = new List<EconomyDetails.Scenario.Entry>();

    public string name { get; private set; }

    public float defaultCount { get; private set; }

    public float timeInSeconds { get; set; }

    public Scenario(
      string name,
      float default_count,
      Func<EconomyDetails.Transformation, bool> filter)
    {
      this.name = name;
      this.defaultCount = default_count;
      this.filter = filter;
      this.timeInSeconds = 600f;
    }

    public void AddEntry(EconomyDetails.Scenario.Entry entry) => this.entries.Add(entry);

    public float GetCount(Tag tag)
    {
      foreach (EconomyDetails.Scenario.Entry entry in this.entries)
      {
        if (Tag.op_Equality(entry.tag, tag))
          return entry.count;
      }
      return this.defaultCount;
    }

    public bool IncludesTransformation(EconomyDetails.Transformation transformation)
    {
      if (this.filter != null && this.filter(transformation))
        return true;
      foreach (EconomyDetails.Scenario.Entry entry in this.entries)
      {
        if (Tag.op_Equality(entry.tag, transformation.tag))
          return true;
      }
      return false;
    }

    public class Entry
    {
      public Tag tag { get; private set; }

      public float count { get; private set; }

      public Entry(Tag tag, float count)
      {
        this.tag = tag;
        this.count = count;
      }
    }
  }

  public class Transformation
  {
    public List<EconomyDetails.Transformation.Delta> deltas = new List<EconomyDetails.Transformation.Delta>();

    public Tag tag { get; private set; }

    public EconomyDetails.Transformation.Type type { get; private set; }

    public float timeInSeconds { get; private set; }

    public bool timeInvariant { get; private set; }

    public Transformation(
      Tag tag,
      EconomyDetails.Transformation.Type type,
      float time_in_seconds,
      bool timeInvariant = false)
    {
      this.tag = tag;
      this.type = type;
      this.timeInSeconds = time_in_seconds;
      this.timeInvariant = timeInvariant;
    }

    public void AddDelta(EconomyDetails.Transformation.Delta delta)
    {
      Debug.Assert(delta.resource != null);
      this.deltas.Add(delta);
    }

    public EconomyDetails.Transformation.Delta GetDelta(EconomyDetails.Resource resource)
    {
      foreach (EconomyDetails.Transformation.Delta delta in this.deltas)
      {
        if (delta.resource == resource)
          return delta;
      }
      return (EconomyDetails.Transformation.Delta) null;
    }

    public class Delta
    {
      public EconomyDetails.Resource resource { get; private set; }

      public float amount { get; set; }

      public Delta(EconomyDetails.Resource resource, float amount)
      {
        this.resource = resource;
        this.amount = amount;
      }
    }

    public class Type
    {
      public string id { get; private set; }

      public Type(string id) => this.id = id;
    }
  }
}
