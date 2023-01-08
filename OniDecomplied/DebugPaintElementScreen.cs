// Decompiled with JetBrains decompiler
// Type: DebugPaintElementScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DebugPaintElementScreen : KScreen
{
  [Header("Current State")]
  public SimHashes element;
  [NonSerialized]
  public float mass = 1000f;
  [NonSerialized]
  public float temperature = -1f;
  [NonSerialized]
  public bool set_prevent_fow_reveal;
  [NonSerialized]
  public bool set_allow_fow_reveal;
  [NonSerialized]
  public int diseaseCount;
  public byte diseaseIdx;
  [Header("Popup Buttons")]
  [SerializeField]
  private KButton elementButton;
  [SerializeField]
  private KButton diseaseButton;
  [Header("Popup Menus")]
  [SerializeField]
  private KPopupMenu elementPopup;
  [SerializeField]
  private KPopupMenu diseasePopup;
  [Header("Value Inputs")]
  [SerializeField]
  private KInputTextField massPressureInput;
  [SerializeField]
  private KInputTextField temperatureInput;
  [SerializeField]
  private KInputTextField diseaseCountInput;
  [SerializeField]
  private KInputTextField filterInput;
  [Header("Tool Buttons")]
  [SerializeField]
  private KButton paintButton;
  [SerializeField]
  private KButton fillButton;
  [SerializeField]
  private KButton sampleButton;
  [SerializeField]
  private KButton spawnButton;
  [SerializeField]
  private KButton storeButton;
  [Header("Parameter Toggles")]
  public Toggle paintElement;
  public Toggle paintMass;
  public Toggle paintTemperature;
  public Toggle paintDisease;
  public Toggle paintDiseaseCount;
  public Toggle affectBuildings;
  public Toggle affectCells;
  public Toggle paintPreventFOWReveal;
  public Toggle paintAllowFOWReveal;
  private List<KInputTextField> inputFields = new List<KInputTextField>();
  private List<string> options_list = new List<string>();
  private string filter;

  public static DebugPaintElementScreen Instance { get; private set; }

  public static void DestroyInstance() => DebugPaintElementScreen.Instance = (DebugPaintElementScreen) null;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    DebugPaintElementScreen.Instance = this;
    this.SetupLocText();
    this.inputFields.Add(this.massPressureInput);
    this.inputFields.Add(this.temperatureInput);
    this.inputFields.Add(this.diseaseCountInput);
    this.inputFields.Add(this.filterInput);
    foreach (KInputTextField inputField in this.inputFields)
    {
      ((TMP_InputField) inputField).onFocus = ((TMP_InputField) inputField).onFocus + (System.Action) (() => this.isEditing = true);
      // ISSUE: method pointer
      ((UnityEvent<string>) ((TMP_InputField) inputField).onEndEdit).AddListener(new UnityAction<string>((object) this, __methodptr(\u003COnPrefabInit\u003Eb__35_0)));
    }
    ((Component) this).gameObject.SetActive(false);
    this.activateOnSpawn = true;
    this.ConsumeMouseScroll = true;
  }

  private void SetupLocText()
  {
    HierarchyReferences component = ((Component) this).GetComponent<HierarchyReferences>();
    ((TMP_Text) component.GetReference<LocText>("Title")).text = (string) STRINGS.UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.TITLE;
    ((TMP_Text) component.GetReference<LocText>("ElementLabel")).text = (string) STRINGS.UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.ELEMENT;
    ((TMP_Text) component.GetReference<LocText>("MassLabel")).text = (string) STRINGS.UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.MASS_KG;
    ((TMP_Text) component.GetReference<LocText>("TemperatureLabel")).text = (string) STRINGS.UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.TEMPERATURE_KELVIN;
    ((TMP_Text) component.GetReference<LocText>("DiseaseLabel")).text = (string) STRINGS.UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.DISEASE;
    ((TMP_Text) component.GetReference<LocText>("DiseaseCountLabel")).text = (string) STRINGS.UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.DISEASE_COUNT;
    ((TMP_Text) component.GetReference<LocText>("AddFoWMaskLabel")).text = (string) STRINGS.UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.ADD_FOW_MASK;
    ((TMP_Text) component.GetReference<LocText>("RemoveFoWMaskLabel")).text = (string) STRINGS.UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.REMOVE_FOW_MASK;
    ((TMP_Text) ((Component) this.elementButton).GetComponentsInChildren<LocText>()[0]).text = (string) STRINGS.UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.ELEMENT;
    ((TMP_Text) ((Component) this.diseaseButton).GetComponentsInChildren<LocText>()[0]).text = (string) STRINGS.UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.DISEASE;
    ((TMP_Text) ((Component) this.paintButton).GetComponentsInChildren<LocText>()[0]).text = (string) STRINGS.UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.PAINT;
    ((TMP_Text) ((Component) this.fillButton).GetComponentsInChildren<LocText>()[0]).text = (string) STRINGS.UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.FILL;
    ((TMP_Text) ((Component) this.spawnButton).GetComponentsInChildren<LocText>()[0]).text = (string) STRINGS.UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.SPAWN_ALL;
    ((TMP_Text) ((Component) this.sampleButton).GetComponentsInChildren<LocText>()[0]).text = (string) STRINGS.UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.SAMPLE;
    ((TMP_Text) ((Component) this.storeButton).GetComponentsInChildren<LocText>()[0]).text = (string) STRINGS.UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.STORE;
    ((TMP_Text) ((Component) ((Component) this.affectBuildings).transform.parent).GetComponentsInChildren<LocText>()[0]).text = (string) STRINGS.UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.BUILDINGS;
    ((TMP_Text) ((Component) ((Component) this.affectCells).transform.parent).GetComponentsInChildren<LocText>()[0]).text = (string) STRINGS.UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.CELLS;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.element = SimHashes.Ice;
    this.diseaseIdx = byte.MaxValue;
    this.ConfigureElements();
    List<string> stringList = new List<string>();
    stringList.Insert(0, "None");
    foreach (Klei.AI.Disease resource in Db.Get().Diseases.resources)
      stringList.Add(resource.Name);
    this.diseasePopup.SetOptions((IList<string>) stringList.ToArray());
    this.diseasePopup.OnSelect += new Action<string, int>(this.OnSelectDisease);
    this.SelectDiseaseOption((int) this.diseaseIdx);
    this.paintButton.onClick += new System.Action(this.OnClickPaint);
    this.fillButton.onClick += new System.Action(this.OnClickFill);
    this.sampleButton.onClick += new System.Action(this.OnClickSample);
    this.storeButton.onClick += new System.Action(this.OnClickStore);
    if (SaveGame.Instance.worldGenSpawner.SpawnsRemain())
      this.spawnButton.onClick += new System.Action(this.OnClickSpawn);
    this.elementPopup.OnSelect += new Action<string, int>(this.OnSelectElement);
    this.elementButton.onClick += new System.Action(this.elementPopup.OnClick);
    this.diseaseButton.onClick += new System.Action(this.diseasePopup.OnClick);
  }

  private void FilterElements(string filterValue)
  {
    if (string.IsNullOrEmpty(filterValue))
    {
      foreach (KButtonMenu.ButtonInfo button in (IEnumerable<KButtonMenu.ButtonInfo>) this.elementPopup.GetButtons())
        ((Component) button.uibutton).gameObject.SetActive(true);
    }
    else
    {
      filterValue = this.filter.ToLower();
      foreach (KButtonMenu.ButtonInfo button in (IEnumerable<KButtonMenu.ButtonInfo>) this.elementPopup.GetButtons())
        ((Component) button.uibutton).gameObject.SetActive(button.text.ToLower().Contains(filterValue));
    }
  }

  private void ConfigureElements()
  {
    if (this.filter != null)
      this.filter = this.filter.ToLower();
    List<DebugPaintElementScreen.ElemDisplayInfo> elemDisplayInfoList = new List<DebugPaintElementScreen.ElemDisplayInfo>();
    foreach (Element element in ElementLoader.elements)
    {
      if (element.name != "Element Not Loaded" && element.substance != null && element.substance.showInEditor && (string.IsNullOrEmpty(this.filter) || element.name.ToLower().Contains(this.filter)))
        elemDisplayInfoList.Add(new DebugPaintElementScreen.ElemDisplayInfo()
        {
          id = element.id,
          displayStr = element.name + " (" + element.GetStateString() + ")"
        });
    }
    elemDisplayInfoList.Sort((Comparison<DebugPaintElementScreen.ElemDisplayInfo>) ((a, b) => a.displayStr.CompareTo(b.displayStr)));
    if (string.IsNullOrEmpty(this.filter))
    {
      SimHashes[] simHashesArray = new SimHashes[6]
      {
        SimHashes.SlimeMold,
        SimHashes.Vacuum,
        SimHashes.Dirt,
        SimHashes.CarbonDioxide,
        SimHashes.Water,
        SimHashes.Oxygen
      };
      foreach (SimHashes hash in simHashesArray)
      {
        Element elementByHash = ElementLoader.FindElementByHash(hash);
        elemDisplayInfoList.Insert(0, new DebugPaintElementScreen.ElemDisplayInfo()
        {
          id = elementByHash.id,
          displayStr = elementByHash.name + " (" + elementByHash.GetStateString() + ")"
        });
      }
    }
    this.options_list = new List<string>();
    List<string> options = new List<string>();
    foreach (DebugPaintElementScreen.ElemDisplayInfo elemDisplayInfo in elemDisplayInfoList)
    {
      options.Add(elemDisplayInfo.displayStr);
      this.options_list.Add(elemDisplayInfo.id.ToString());
    }
    this.elementPopup.SetOptions((IList<string>) options);
    for (int index = 0; index < elemDisplayInfoList.Count; ++index)
    {
      if (elemDisplayInfoList[index].id == this.element)
        this.elementPopup.SelectOption(options[index], index);
    }
    ((Component) this.elementPopup).GetComponent<ScrollRect>().normalizedPosition = new Vector2(0.0f, 1f);
  }

  private void OnClickSpawn()
  {
    foreach (WorldContainer worldContainer in (IEnumerable<WorldContainer>) ClusterManager.Instance.WorldContainers)
      worldContainer.SetDiscovered(true);
    SaveGame.Instance.worldGenSpawner.SpawnEverything();
    ((Component) this.spawnButton).GetComponent<KButton>().isInteractable = false;
  }

  private void OnClickPaint()
  {
    this.OnChangeMassPressure();
    this.OnChangeTemperature();
    this.OnDiseaseCountChange();
    this.OnChangeFOWReveal();
    DebugTool.Instance.Activate(DebugTool.Type.ReplaceSubstance);
  }

  private void OnClickStore()
  {
    this.OnChangeMassPressure();
    this.OnChangeTemperature();
    this.OnDiseaseCountChange();
    this.OnChangeFOWReveal();
    DebugTool.Instance.Activate(DebugTool.Type.StoreSubstance);
  }

  private void OnClickSample()
  {
    this.OnChangeMassPressure();
    this.OnChangeTemperature();
    this.OnDiseaseCountChange();
    this.OnChangeFOWReveal();
    DebugTool.Instance.Activate(DebugTool.Type.Sample);
  }

  private void OnClickFill()
  {
    this.OnChangeMassPressure();
    this.OnChangeTemperature();
    this.OnDiseaseCountChange();
    DebugTool.Instance.Activate(DebugTool.Type.FillReplaceSubstance);
  }

  private void OnSelectElement(string str, int index)
  {
    this.element = (SimHashes) Enum.Parse(typeof (SimHashes), this.options_list[index]);
    ((TMP_Text) ((Component) this.elementButton).GetComponentInChildren<LocText>()).text = str;
  }

  private void OnSelectElement(SimHashes element)
  {
    this.element = element;
    ((TMP_Text) ((Component) this.elementButton).GetComponentInChildren<LocText>()).text = ElementLoader.FindElementByHash(element).name;
  }

  private void OnSelectDisease(string str, int index)
  {
    this.diseaseIdx = byte.MaxValue;
    for (int index1 = 0; index1 < ((ResourceSet) Db.Get().Diseases).Count; ++index1)
    {
      if (Db.Get().Diseases[index1].Name == str)
        this.diseaseIdx = (byte) index1;
    }
    this.SelectDiseaseOption((int) this.diseaseIdx);
  }

  private void SelectDiseaseOption(int diseaseIdx)
  {
    if (diseaseIdx == (int) byte.MaxValue)
    {
      ((TMP_Text) ((Component) this.diseaseButton).GetComponentInChildren<LocText>()).text = "None";
    }
    else
    {
      string name = Db.Get().Diseases[diseaseIdx].Name;
      ((TMP_Text) ((Component) this.diseaseButton).GetComponentInChildren<LocText>()).text = name;
    }
  }

  private void OnChangeFOWReveal()
  {
    if (this.paintPreventFOWReveal.isOn)
      this.paintAllowFOWReveal.isOn = false;
    if (this.paintAllowFOWReveal.isOn)
      this.paintPreventFOWReveal.isOn = false;
    this.set_prevent_fow_reveal = this.paintPreventFOWReveal.isOn;
    this.set_allow_fow_reveal = this.paintAllowFOWReveal.isOn;
  }

  public void OnChangeMassPressure()
  {
    float num;
    try
    {
      num = Convert.ToSingle(((TMP_InputField) this.massPressureInput).text);
    }
    catch
    {
      num = -1f;
    }
    this.mass = num;
  }

  public void OnChangeTemperature()
  {
    float num;
    try
    {
      num = Convert.ToSingle(((TMP_InputField) this.temperatureInput).text);
    }
    catch
    {
      num = -1f;
    }
    this.temperature = num;
  }

  public void OnDiseaseCountChange()
  {
    int num;
    try
    {
      num = Convert.ToInt32(((TMP_InputField) this.diseaseCountInput).text);
    }
    catch
    {
      num = 0;
    }
    this.diseaseCount = num;
  }

  public void OnElementsFilterEdited(string new_filter)
  {
    this.filter = string.IsNullOrEmpty(((TMP_InputField) this.filterInput).text) ? (string) null : ((TMP_InputField) this.filterInput).text;
    this.FilterElements(this.filter);
  }

  public void SampleCell(int cell)
  {
    ((TMP_InputField) this.massPressureInput).text = (Grid.Pressure[cell] * 0.0100000007f).ToString();
    ((TMP_InputField) this.temperatureInput).text = Grid.Temperature[cell].ToString();
    this.OnSelectElement(ElementLoader.GetElementID(Grid.Element[cell].tag));
    this.OnChangeMassPressure();
    this.OnChangeTemperature();
  }

  private struct ElemDisplayInfo
  {
    public SimHashes id;
    public string displayStr;
  }
}
