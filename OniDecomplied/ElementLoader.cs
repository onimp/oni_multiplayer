// Decompiled with JetBrains decompiler
// Type: ElementLoader
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using ProcGenGame;
using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElementLoader
{
  public static List<Element> elements;
  public static Dictionary<int, Element> elementTable;
  public static Dictionary<Tag, Element> elementTagTable;
  private static string path = Application.streamingAssetsPath + "/elements/";
  private static readonly Color noColour = new Color(0.0f, 0.0f, 0.0f, 0.0f);

  public static List<ElementLoader.ElementEntry> CollectElementsFromYAML()
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    ElementLoader.\u003C\u003Ec__DisplayClass7_0 cDisplayClass70 = new ElementLoader.\u003C\u003Ec__DisplayClass7_0();
    List<ElementLoader.ElementEntry> elementEntryList = new List<ElementLoader.ElementEntry>();
    ListPool<FileHandle, ElementLoader>.PooledList pooledList = ListPool<FileHandle, ElementLoader>.Allocate();
    FileSystem.GetFiles(FileSystem.Normalize(ElementLoader.path), "*.yaml", (ICollection<FileHandle>) pooledList);
    // ISSUE: reference to a compiler-generated field
    cDisplayClass70.errors = ListPool<YamlIO.Error, ElementLoader>.Allocate();
    foreach (FileHandle fileHandle in (List<FileHandle>) pooledList)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      ElementLoader.ElementEntryCollection elementEntryCollection = YamlIO.LoadFile<ElementLoader.ElementEntryCollection>(fileHandle.full_path, cDisplayClass70.\u003C\u003E9__0 ?? (cDisplayClass70.\u003C\u003E9__0 = new YamlIO.ErrorHandler((object) cDisplayClass70, __methodptr(\u003CCollectElementsFromYAML\u003Eb__0))), (List<Tuple<string, System.Type>>) null);
      if (elementEntryCollection != null)
        elementEntryList.AddRange((IEnumerable<ElementLoader.ElementEntry>) elementEntryCollection.elements);
    }
    pooledList.Recycle();
    if (Object.op_Inequality((Object) Global.Instance, (Object) null) && Global.Instance.modManager != null)
    {
      // ISSUE: reference to a compiler-generated field
      Global.Instance.modManager.HandleErrors((List<YamlIO.Error>) cDisplayClass70.errors);
    }
    // ISSUE: reference to a compiler-generated field
    cDisplayClass70.errors.Recycle();
    return elementEntryList;
  }

  public static void Load(
    ref Hashtable substanceList,
    Dictionary<string, SubstanceTable> substanceTablesByDlc)
  {
    ElementLoader.elements = new List<Element>();
    ElementLoader.elementTable = new Dictionary<int, Element>();
    ElementLoader.elementTagTable = new Dictionary<Tag, Element>();
    foreach (ElementLoader.ElementEntry entry in ElementLoader.CollectElementsFromYAML())
    {
      int key = Hash.SDBMLower(entry.elementId);
      if (!ElementLoader.elementTable.ContainsKey(key) && substanceTablesByDlc.ContainsKey(entry.dlcId))
      {
        Element elem = new Element()
        {
          id = (SimHashes) key,
          name = StringEntry.op_Implicit(Strings.Get(entry.localizationID))
        };
        elem.nameUpperCase = elem.name.ToUpper();
        elem.description = StringEntry.op_Implicit(Strings.Get(entry.description));
        elem.tag = TagManager.Create(entry.elementId, elem.name);
        ElementLoader.CopyEntryToElement(entry, elem);
        ElementLoader.elements.Add(elem);
        ElementLoader.elementTable[key] = elem;
        ElementLoader.elementTagTable[elem.tag] = elem;
        if (!ElementLoader.ManifestSubstanceForElement(elem, ref substanceList, substanceTablesByDlc[entry.dlcId]))
          Debug.LogWarning((object) ("Missing substance for element: " + elem.id.ToString()));
      }
    }
    ElementLoader.FinaliseElementsTable(ref substanceList);
    WorldGen.SetupDefaultElements();
  }

  private static void CopyEntryToElement(ElementLoader.ElementEntry entry, Element elem)
  {
    Hash.SDBMLower(entry.elementId);
    elem.tag = TagManager.Create(entry.elementId.ToString());
    elem.specificHeatCapacity = entry.specificHeatCapacity;
    elem.thermalConductivity = entry.thermalConductivity;
    elem.molarMass = entry.molarMass;
    elem.strength = entry.strength;
    elem.disabled = entry.isDisabled;
    elem.dlcId = entry.dlcId;
    elem.flow = entry.flow;
    elem.maxMass = entry.maxMass;
    elem.maxCompression = entry.liquidCompression;
    elem.viscosity = entry.speed;
    elem.minHorizontalFlow = entry.minHorizontalFlow;
    elem.minVerticalFlow = entry.minVerticalFlow;
    elem.solidSurfaceAreaMultiplier = entry.solidSurfaceAreaMultiplier;
    elem.liquidSurfaceAreaMultiplier = entry.liquidSurfaceAreaMultiplier;
    elem.gasSurfaceAreaMultiplier = entry.gasSurfaceAreaMultiplier;
    elem.state = entry.state;
    elem.hardness = entry.hardness;
    elem.lowTemp = entry.lowTemp;
    elem.lowTempTransitionTarget = (SimHashes) Hash.SDBMLower(entry.lowTempTransitionTarget);
    elem.highTemp = entry.highTemp;
    elem.highTempTransitionTarget = (SimHashes) Hash.SDBMLower(entry.highTempTransitionTarget);
    elem.highTempTransitionOreID = (SimHashes) Hash.SDBMLower(entry.highTempTransitionOreId);
    elem.highTempTransitionOreMassConversion = entry.highTempTransitionOreMassConversion;
    elem.lowTempTransitionOreID = (SimHashes) Hash.SDBMLower(entry.lowTempTransitionOreId);
    elem.lowTempTransitionOreMassConversion = entry.lowTempTransitionOreMassConversion;
    elem.sublimateId = (SimHashes) Hash.SDBMLower(entry.sublimateId);
    elem.convertId = (SimHashes) Hash.SDBMLower(entry.convertId);
    elem.sublimateFX = (SpawnFXHashes) Hash.SDBMLower(entry.sublimateFx);
    elem.sublimateRate = entry.sublimateRate;
    elem.sublimateEfficiency = entry.sublimateEfficiency;
    elem.sublimateProbability = entry.sublimateProbability;
    elem.offGasPercentage = entry.offGasPercentage;
    elem.lightAbsorptionFactor = entry.lightAbsorptionFactor;
    elem.radiationAbsorptionFactor = entry.radiationAbsorptionFactor;
    elem.radiationPer1000Mass = entry.radiationPer1000Mass;
    elem.toxicity = entry.toxicity;
    elem.elementComposition = entry.composition;
    Tag phaseTag = TagManager.Create(entry.state.ToString());
    elem.materialCategory = ElementLoader.CreateMaterialCategoryTag(elem.id, phaseTag, entry.materialCategory);
    elem.oreTags = ElementLoader.CreateOreTags(elem.materialCategory, phaseTag, entry.tags);
    elem.buildMenuSort = entry.buildMenuSort;
    Sim.PhysicsData physicsData = new Sim.PhysicsData();
    physicsData.temperature = entry.defaultTemperature;
    physicsData.mass = entry.defaultMass;
    physicsData.pressure = entry.defaultPressure;
    switch (entry.state)
    {
      case Element.State.Gas:
        GameTags.GasElements.Add(elem.tag);
        physicsData.mass = 1f;
        elem.maxMass = 1.8f;
        break;
      case Element.State.Liquid:
        GameTags.LiquidElements.Add(elem.tag);
        break;
      case Element.State.Solid:
        GameTags.SolidElements.Add(elem.tag);
        break;
    }
    elem.defaultValues = physicsData;
  }

  private static bool ManifestSubstanceForElement(
    Element elem,
    ref Hashtable substanceList,
    SubstanceTable substanceTable)
  {
    elem.substance = (Substance) null;
    if (substanceList.ContainsKey((object) elem.id))
    {
      elem.substance = substanceList[(object) elem.id] as Substance;
      return false;
    }
    if (Object.op_Inequality((Object) substanceTable, (Object) null))
      elem.substance = substanceTable.GetSubstance(elem.id);
    if (elem.substance == null)
    {
      elem.substance = new Substance();
      substanceTable.GetList().Add(elem.substance);
    }
    elem.substance.elementID = elem.id;
    elem.substance.renderedByWorld = elem.IsSolid;
    elem.substance.idx = substanceList.Count;
    if (Color.op_Equality(Color32.op_Implicit(elem.substance.uiColour), ElementLoader.noColour))
    {
      int count = ElementLoader.elements.Count;
      int idx = elem.substance.idx;
      elem.substance.uiColour = Color32.op_Implicit(Color.HSVToRGB((float) idx / (float) count, 1f, 1f));
    }
    string str = UI.StripLinkFormatting(elem.name);
    elem.substance.name = str;
    elem.substance.nameTag = Array.IndexOf<SimHashes>((SimHashes[]) Enum.GetValues(typeof (SimHashes)), elem.id) < 0 ? (str != null ? TagManager.Create(str) : Tag.Invalid) : GameTagExtensions.Create(elem.id);
    elem.substance.audioConfig = ElementsAudio.Instance.GetConfigForElement(elem.id);
    substanceList.Add((object) elem.id, (object) elem.substance);
    return true;
  }

  public static Element FindElementByName(string name)
  {
    try
    {
      return ElementLoader.FindElementByHash((SimHashes) Enum.Parse(typeof (SimHashes), name));
    }
    catch
    {
      return ElementLoader.FindElementByHash((SimHashes) Hash.SDBMLower(name));
    }
  }

  public static Element FindElementByHash(SimHashes hash)
  {
    Element elementByHash = (Element) null;
    ElementLoader.elementTable.TryGetValue((int) hash, out elementByHash);
    return elementByHash;
  }

  public static ushort GetElementIndex(SimHashes hash)
  {
    Element element = (Element) null;
    ElementLoader.elementTable.TryGetValue((int) hash, out element);
    return element != null ? element.idx : ushort.MaxValue;
  }

  public static Element GetElement(Tag tag)
  {
    Element element;
    ElementLoader.elementTagTable.TryGetValue(tag, out element);
    return element;
  }

  public static SimHashes GetElementID(Tag tag)
  {
    Element element;
    ElementLoader.elementTagTable.TryGetValue(tag, out element);
    return element != null ? element.id : SimHashes.Vacuum;
  }

  private static SimHashes GetID(int column, int row, string[,] grid, SimHashes defaultValue = SimHashes.Vacuum)
  {
    if (column >= grid.GetLength(0) || row > grid.GetLength(1))
    {
      Debug.LogError((object) string.Format("Could not find element at loc [{0},{1}] grid is only [{2},{3}]", (object) column, (object) row, (object) grid.GetLength(0), (object) grid.GetLength(1)));
      return defaultValue;
    }
    string str = grid[column, row];
    if (str == null || str == "")
      return defaultValue;
    object id;
    try
    {
      id = Enum.Parse(typeof (SimHashes), str);
    }
    catch (Exception ex)
    {
      Debug.LogError((object) string.Format("Could not find element {0}: {1}", (object) str, (object) ex.ToString()));
      return defaultValue;
    }
    return (SimHashes) id;
  }

  private static SpawnFXHashes GetSpawnFX(int column, int row, string[,] grid)
  {
    if (column >= grid.GetLength(0) || row > grid.GetLength(1))
    {
      Debug.LogError((object) string.Format("Could not find SpawnFXHashes at loc [{0},{1}] grid is only [{2},{3}]", (object) column, (object) row, (object) grid.GetLength(0), (object) grid.GetLength(1)));
      return SpawnFXHashes.None;
    }
    string str = grid[column, row];
    if (str == null || str == "")
      return SpawnFXHashes.None;
    object spawnFx;
    try
    {
      spawnFx = Enum.Parse(typeof (SpawnFXHashes), str);
    }
    catch (Exception ex)
    {
      Debug.LogError((object) string.Format("Could not find FX {0}: {1}", (object) str, (object) ex.ToString()));
      return SpawnFXHashes.None;
    }
    return (SpawnFXHashes) spawnFx;
  }

  private static Tag CreateMaterialCategoryTag(
    SimHashes element_id,
    Tag phaseTag,
    string materialCategoryField)
  {
    if (string.IsNullOrEmpty(materialCategoryField))
      return phaseTag;
    Tag materialCategoryTag = TagManager.Create(materialCategoryField);
    if (!GameTags.MaterialCategories.Contains(materialCategoryTag) && !GameTags.IgnoredMaterialCategories.Contains(materialCategoryTag))
      Debug.LogWarningFormat("Element {0} has category {1}, but that isn't in GameTags.MaterialCategores!", new object[2]
      {
        (object) element_id,
        (object) materialCategoryField
      });
    return materialCategoryTag;
  }

  private static Tag[] CreateOreTags(Tag materialCategory, Tag phaseTag, string[] ore_tags_split)
  {
    List<Tag> tagList = new List<Tag>();
    if (ore_tags_split != null)
    {
      foreach (string str in ore_tags_split)
      {
        if (!string.IsNullOrEmpty(str))
          tagList.Add(TagManager.Create(str));
      }
    }
    tagList.Add(phaseTag);
    if (((Tag) ref materialCategory).IsValid && !tagList.Contains(materialCategory))
      tagList.Add(materialCategory);
    return tagList.ToArray();
  }

  private static void FinaliseElementsTable(ref Hashtable substanceList)
  {
    foreach (Element element in ElementLoader.elements)
    {
      if (element != null)
      {
        if (element.substance == null)
        {
          Debug.LogWarning((object) ("Skipping finalise for missing element: " + element.id.ToString()));
        }
        else
        {
          Debug.Assert(((Tag) ref element.substance.nameTag).IsValid);
          if ((double) element.thermalConductivity == 0.0)
            element.state |= Element.State.TemperatureInsulated;
          if ((double) element.strength == 0.0)
            element.state |= Element.State.Unbreakable;
          if (element.IsSolid)
          {
            Element elementByHash = ElementLoader.FindElementByHash(element.highTempTransitionTarget);
            if (elementByHash != null)
              element.highTempTransition = elementByHash;
          }
          else if (element.IsLiquid)
          {
            Element elementByHash1 = ElementLoader.FindElementByHash(element.highTempTransitionTarget);
            if (elementByHash1 != null)
              element.highTempTransition = elementByHash1;
            Element elementByHash2 = ElementLoader.FindElementByHash(element.lowTempTransitionTarget);
            if (elementByHash2 != null)
              element.lowTempTransition = elementByHash2;
          }
          else if (element.IsGas)
          {
            Element elementByHash = ElementLoader.FindElementByHash(element.lowTempTransitionTarget);
            if (elementByHash != null)
              element.lowTempTransition = elementByHash;
          }
        }
      }
    }
    ElementLoader.elements = ElementLoader.elements.OrderByDescending<Element, int>((Func<Element, int>) (e => (int) (e.state & Element.State.Solid))).ThenBy<Element, SimHashes>((Func<Element, SimHashes>) (e => e.id)).ToList<Element>();
    for (int index = 0; index < ElementLoader.elements.Count; ++index)
    {
      if (ElementLoader.elements[index].substance != null)
        ElementLoader.elements[index].substance.idx = index;
      ElementLoader.elements[index].idx = (ushort) index;
    }
  }

  private static void ValidateElements()
  {
    Debug.Log((object) "------ Start Validating Elements ------");
    foreach (Element element in ElementLoader.elements)
    {
      string str = string.Format("{0} ({1})", (object) element.tag.ProperNameStripLink(), (object) element.state);
      if (element.IsLiquid && element.sublimateId != (SimHashes) 0)
      {
        Debug.Assert((double) element.sublimateRate == 0.0, (object) (str + ": Liquids don't use sublimateRate, use offGasPercentage instead."));
        Debug.Assert((double) element.offGasPercentage > 0.0, (object) (str + ": Missing offGasPercentage"));
      }
      if (element.IsSolid && element.sublimateId != (SimHashes) 0)
      {
        Debug.Assert((double) element.offGasPercentage == 0.0, (object) (str + ": Solids don't use offGasPercentage, use sublimateRate instead."));
        Debug.Assert((double) element.sublimateRate > 0.0, (object) (str + ": Missing sublimationRate"));
        Debug.Assert((double) element.sublimateRate * (double) element.sublimateEfficiency > 1.0 / 1000.0, (object) (str + ": Sublimation rate and efficiency will result in gas that will be obliterated because its less than 1g. Increase these values and use sublimateProbability if you want a low amount of sublimation"));
      }
      if (element.highTempTransition != null && element.highTempTransition.lowTempTransition == element)
        Debug.Assert((double) element.highTemp >= (double) element.highTempTransition.lowTemp, (object) (str + ": highTemp is higher than transition element's (" + element.highTempTransition.tag.ProperNameStripLink() + ") lowTemp"));
      Debug.Assert((double) element.defaultValues.mass <= (double) element.maxMass, (object) (str + ": Default mass should be less than max mass"));
      if (false)
      {
        if (element.IsSolid && element.highTempTransition != null && element.highTempTransition.IsLiquid && (double) element.defaultValues.mass > (double) element.highTempTransition.maxMass)
          Debug.LogWarning((object) string.Format("{0} defaultMass {1} > {2}: maxMass {3}", (object) str, (object) element.defaultValues.mass, (object) element.highTempTransition.tag.ProperNameStripLink(), (object) element.highTempTransition.maxMass));
        if ((double) element.defaultValues.mass < (double) element.maxMass && element.IsLiquid)
          Debug.LogWarning((object) string.Format("{0} has defaultMass: {1} and maxMass {2}", (object) element.tag.ProperNameStripLink(), (object) element.defaultValues.mass, (object) element.maxMass));
      }
    }
    Debug.Log((object) "------ End Validating Elements ------");
  }

  public class ElementEntryCollection
  {
    public ElementLoader.ElementEntry[] elements { get; set; }
  }

  public class ElementComposition
  {
    public string elementID { get; set; }

    public float percentage { get; set; }
  }

  public class ElementEntry
  {
    private string description_backing;

    public ElementEntry()
    {
      this.lowTemp = 0.0f;
      this.highTemp = 10000f;
    }

    public string elementId { get; set; }

    public float specificHeatCapacity { get; set; }

    public float thermalConductivity { get; set; }

    public float solidSurfaceAreaMultiplier { get; set; }

    public float liquidSurfaceAreaMultiplier { get; set; }

    public float gasSurfaceAreaMultiplier { get; set; }

    public float defaultMass { get; set; }

    public float defaultTemperature { get; set; }

    public float defaultPressure { get; set; }

    public float molarMass { get; set; }

    public float lightAbsorptionFactor { get; set; }

    public float radiationAbsorptionFactor { get; set; }

    public float radiationPer1000Mass { get; set; }

    public string lowTempTransitionTarget { get; set; }

    public float lowTemp { get; set; }

    public string highTempTransitionTarget { get; set; }

    public float highTemp { get; set; }

    public string lowTempTransitionOreId { get; set; }

    public float lowTempTransitionOreMassConversion { get; set; }

    public string highTempTransitionOreId { get; set; }

    public float highTempTransitionOreMassConversion { get; set; }

    public string sublimateId { get; set; }

    public string sublimateFx { get; set; }

    public float sublimateRate { get; set; }

    public float sublimateEfficiency { get; set; }

    public float sublimateProbability { get; set; }

    public float offGasPercentage { get; set; }

    public string materialCategory { get; set; }

    public string[] tags { get; set; }

    public bool isDisabled { get; set; }

    public float strength { get; set; }

    public float maxMass { get; set; }

    public byte hardness { get; set; }

    public float toxicity { get; set; }

    public float liquidCompression { get; set; }

    public float speed { get; set; }

    public float minHorizontalFlow { get; set; }

    public float minVerticalFlow { get; set; }

    public string convertId { get; set; }

    public float flow { get; set; }

    public int buildMenuSort { get; set; }

    public Element.State state { get; set; }

    public string localizationID { get; set; }

    public string dlcId { get; set; }

    public ElementLoader.ElementComposition[] composition { get; set; }

    public string description
    {
      get => this.description_backing ?? "STRINGS.ELEMENTS." + this.elementId.ToString().ToUpper() + ".DESC";
      set => this.description_backing = value;
    }
  }
}
