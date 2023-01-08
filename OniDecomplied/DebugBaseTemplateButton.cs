// Decompiled with JetBrains decompiler
// Type: DebugBaseTemplateButton
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using System.Collections.Generic;
using TemplateClasses;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DebugBaseTemplateButton : KScreen
{
  private bool SaveAllBuildings;
  private bool SaveAllPickups;
  public KButton saveBaseButton;
  public KButton clearButton;
  private TemplateContainer pasteAndSelectAsset;
  public KButton AddSelectionButton;
  public KButton RemoveSelectionButton;
  public KButton clearSelectionButton;
  public KButton DestroyButton;
  public KButton DeconstructButton;
  public KButton MoveButton;
  public TemplateContainer moveAsset;
  public KInputTextField nameField;
  private string SaveName = "enter_template_name";
  public GameObject Placer;
  public Grid.SceneLayer visualizerLayer = Grid.SceneLayer.Move;
  public List<int> SelectedCells = new List<int>();

  public static DebugBaseTemplateButton Instance { get; private set; }

  public static void DestroyInstance() => DebugBaseTemplateButton.Instance = (DebugBaseTemplateButton) null;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    DebugBaseTemplateButton.Instance = this;
    ((Component) this).gameObject.SetActive(false);
    this.SetupLocText();
    this.ConsumeMouseScroll = true;
    KInputTextField nameField = this.nameField;
    ((TMP_InputField) nameField).onFocus = ((TMP_InputField) nameField).onFocus + (System.Action) (() => this.isEditing = true);
    // ISSUE: method pointer
    ((UnityEvent<string>) ((TMP_InputField) this.nameField).onEndEdit).AddListener(new UnityAction<string>((object) this, __methodptr(\u003COnPrefabInit\u003Eb__22_0)));
    // ISSUE: method pointer
    ((UnityEvent<string>) ((TMP_InputField) this.nameField).onValueChanged).AddListener(new UnityAction<string>((object) this, __methodptr(\u003COnPrefabInit\u003Eb__22_1)));
  }

  protected virtual void OnActivate()
  {
    base.OnActivate();
    this.ConsumeMouseScroll = true;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (Object.op_Inequality((Object) this.saveBaseButton, (Object) null))
    {
      this.saveBaseButton.onClick -= new System.Action(this.OnClickSaveBase);
      this.saveBaseButton.onClick += new System.Action(this.OnClickSaveBase);
    }
    if (Object.op_Inequality((Object) this.clearButton, (Object) null))
    {
      this.clearButton.onClick -= new System.Action(this.OnClickClear);
      this.clearButton.onClick += new System.Action(this.OnClickClear);
    }
    if (Object.op_Inequality((Object) this.AddSelectionButton, (Object) null))
    {
      this.AddSelectionButton.onClick -= new System.Action(this.OnClickAddSelection);
      this.AddSelectionButton.onClick += new System.Action(this.OnClickAddSelection);
    }
    if (Object.op_Inequality((Object) this.RemoveSelectionButton, (Object) null))
    {
      this.RemoveSelectionButton.onClick -= new System.Action(this.OnClickRemoveSelection);
      this.RemoveSelectionButton.onClick += new System.Action(this.OnClickRemoveSelection);
    }
    if (Object.op_Inequality((Object) this.clearSelectionButton, (Object) null))
    {
      this.clearSelectionButton.onClick -= new System.Action(this.OnClickClearSelection);
      this.clearSelectionButton.onClick += new System.Action(this.OnClickClearSelection);
    }
    if (Object.op_Inequality((Object) this.MoveButton, (Object) null))
    {
      this.MoveButton.onClick -= new System.Action(this.OnClickMove);
      this.MoveButton.onClick += new System.Action(this.OnClickMove);
    }
    if (Object.op_Inequality((Object) this.DestroyButton, (Object) null))
    {
      this.DestroyButton.onClick -= new System.Action(this.OnClickDestroySelection);
      this.DestroyButton.onClick += new System.Action(this.OnClickDestroySelection);
    }
    if (!Object.op_Inequality((Object) this.DeconstructButton, (Object) null))
      return;
    this.DeconstructButton.onClick -= new System.Action(this.OnClickDeconstructSelection);
    this.DeconstructButton.onClick += new System.Action(this.OnClickDeconstructSelection);
  }

  private void SetupLocText()
  {
  }

  private void OnClickDestroySelection() => DebugTool.Instance.Activate(DebugTool.Type.Destroy);

  private void OnClickDeconstructSelection() => DebugTool.Instance.Activate(DebugTool.Type.Deconstruct);

  private void OnClickMove()
  {
    DebugTool.Instance.DeactivateTool();
    this.moveAsset = this.GetSelectionAsAsset();
    StampTool.Instance.Activate(this.moveAsset);
  }

  private void OnClickAddSelection() => DebugTool.Instance.Activate(DebugTool.Type.AddSelection);

  private void OnClickRemoveSelection() => DebugTool.Instance.Activate(DebugTool.Type.RemoveSelection);

  private void OnClickClearSelection()
  {
    this.ClearSelection();
    ((TMP_InputField) this.nameField).text = "";
  }

  private void OnClickClear() => DebugTool.Instance.Activate(DebugTool.Type.Clear);

  protected virtual void OnDeactivate()
  {
    if (Object.op_Inequality((Object) DebugTool.Instance, (Object) null))
      DebugTool.Instance.DeactivateTool();
    base.OnDeactivate();
  }

  private void OnDisable()
  {
    if (!Object.op_Inequality((Object) DebugTool.Instance, (Object) null))
      return;
    DebugTool.Instance.DeactivateTool();
  }

  private TemplateContainer GetSelectionAsAsset()
  {
    List<TemplateClasses.Cell> _cells = new List<TemplateClasses.Cell>();
    List<Prefab> _buildings = new List<Prefab>();
    List<Prefab> _pickupables = new List<Prefab>();
    List<Prefab> _primaryElementOres = new List<Prefab>();
    List<Prefab> _otherEntities = new List<Prefab>();
    HashSet<GameObject> _excludeEntities = new HashSet<GameObject>();
    float num1 = 0.0f;
    float num2 = 0.0f;
    foreach (int selectedCell in this.SelectedCells)
    {
      num1 += (float) Grid.CellToXY(selectedCell).x;
      num2 += (float) Grid.CellToXY(selectedCell).y;
    }
    float num3;
    int rootX;
    int rootY;
    Grid.CellToXY(Grid.PosToCell(new Vector3(num1 / (float) this.SelectedCells.Count, num3 = num2 / (float) this.SelectedCells.Count, 0.0f)), out rootX, out rootY);
    for (int index = 0; index < this.SelectedCells.Count; ++index)
    {
      int selectedCell = this.SelectedCells[index];
      int x;
      int y;
      Grid.CellToXY(this.SelectedCells[index], out x, out y);
      Element element = ElementLoader.elements[(int) Grid.ElementIdx[selectedCell]];
      string _diseaseName = Grid.DiseaseIdx[selectedCell] != byte.MaxValue ? Db.Get().Diseases[(int) Grid.DiseaseIdx[selectedCell]].Id : (string) null;
      int _diseaseCount = Grid.DiseaseCount[selectedCell];
      if (_diseaseCount <= 0)
      {
        _diseaseCount = 0;
        _diseaseName = (string) null;
      }
      _cells.Add(new TemplateClasses.Cell(x - rootX, y - rootY, element.id, Grid.Temperature[selectedCell], Grid.Mass[selectedCell], _diseaseName, _diseaseCount, Grid.PreventFogOfWarReveal[this.SelectedCells[index]]));
    }
    for (int idx = 0; idx < Components.BuildingCompletes.Count; ++idx)
    {
      BuildingComplete buildingComplete = Components.BuildingCompletes[idx];
      if (!_excludeEntities.Contains(((Component) buildingComplete).gameObject))
      {
        int cell = Grid.PosToCell((KMonoBehaviour) buildingComplete);
        int x;
        int y;
        Grid.CellToXY(cell, out x, out y);
        if (this.SaveAllBuildings || this.SelectedCells.Contains(cell))
        {
          foreach (int placementCell in buildingComplete.PlacementCells)
          {
            int xplace;
            int yplace;
            Grid.CellToXY(placementCell, out xplace, out yplace);
            string _diseaseName = Grid.DiseaseIdx[placementCell] != byte.MaxValue ? Db.Get().Diseases[(int) Grid.DiseaseIdx[placementCell]].Id : (string) null;
            if (_cells.Find((Predicate<TemplateClasses.Cell>) (c => c.location_x == xplace - rootX && c.location_y == yplace - rootY)) == null)
              _cells.Add(new TemplateClasses.Cell(xplace - rootX, yplace - rootY, Grid.Element[placementCell].id, Grid.Temperature[placementCell], Grid.Mass[placementCell], _diseaseName, Grid.DiseaseCount[placementCell]));
          }
          Orientation _rotation = Orientation.Neutral;
          Rotatable component1 = ((Component) buildingComplete).gameObject.GetComponent<Rotatable>();
          if (Object.op_Inequality((Object) component1, (Object) null))
            _rotation = component1.GetOrientation();
          SimHashes _element1 = SimHashes.Void;
          float num4 = 280f;
          string _disease1 = (string) null;
          int _disease_count1 = 0;
          PrimaryElement component2 = ((Component) buildingComplete).GetComponent<PrimaryElement>();
          if (Object.op_Inequality((Object) component2, (Object) null))
          {
            _element1 = component2.ElementID;
            num4 = component2.Temperature;
            _disease1 = component2.DiseaseIdx != byte.MaxValue ? Db.Get().Diseases[(int) component2.DiseaseIdx].Id : (string) null;
            _disease_count1 = component2.DiseaseCount;
          }
          List<Prefab.template_amount_value> templateAmountValueList1 = new List<Prefab.template_amount_value>();
          List<Prefab.template_amount_value> templateAmountValueList2 = new List<Prefab.template_amount_value>();
          foreach (AmountInstance amount in (Modifications<Amount, AmountInstance>) ((Component) buildingComplete).gameObject.GetAmounts())
            templateAmountValueList1.Add(new Prefab.template_amount_value(amount.amount.Id, amount.value));
          Battery component3 = ((Component) buildingComplete).GetComponent<Battery>();
          if (Object.op_Inequality((Object) component3, (Object) null))
          {
            float joulesAvailable = component3.JoulesAvailable;
            templateAmountValueList2.Add(new Prefab.template_amount_value("joulesAvailable", joulesAvailable));
          }
          Unsealable component4 = ((Component) buildingComplete).GetComponent<Unsealable>();
          if (Object.op_Inequality((Object) component4, (Object) null))
          {
            float num5 = component4.facingRight ? 1f : 0.0f;
            templateAmountValueList2.Add(new Prefab.template_amount_value("sealedDoorDirection", num5));
          }
          LogicSwitch component5 = ((Component) buildingComplete).GetComponent<LogicSwitch>();
          if (Object.op_Inequality((Object) component5, (Object) null))
          {
            float num6 = component5.IsSwitchedOn ? 1f : 0.0f;
            templateAmountValueList2.Add(new Prefab.template_amount_value("switchSetting", num6));
          }
          int _connections = 0;
          IHaveUtilityNetworkMgr component6 = ((Component) buildingComplete).GetComponent<IHaveUtilityNetworkMgr>();
          if (component6 != null)
            _connections = (int) component6.GetNetworkManager().GetConnections(cell, true);
          x -= rootX;
          y -= rootY;
          float _temperature = Mathf.Clamp(num4, 1f, 99999f);
          Tag tag1 = ((Component) buildingComplete).PrefabID();
          Prefab prefab = new Prefab(((Tag) ref tag1).Name, Prefab.Type.Building, x, y, _element1, _temperature, 0.0f, _disease1, _disease_count1, _rotation, templateAmountValueList1.ToArray(), templateAmountValueList2.ToArray(), _connections);
          Storage component7 = ((Component) buildingComplete).gameObject.GetComponent<Storage>();
          if (Object.op_Inequality((Object) component7, (Object) null))
          {
            foreach (GameObject go in component7.items)
            {
              float _units = 0.0f;
              SimHashes _element2 = SimHashes.Vacuum;
              float _temp = 280f;
              string _disease2 = (string) null;
              int _disease_count2 = 0;
              bool _isOre = false;
              PrimaryElement component8 = go.GetComponent<PrimaryElement>();
              if (Object.op_Inequality((Object) component8, (Object) null))
              {
                _units = component8.Units;
                _element2 = component8.ElementID;
                _temp = component8.Temperature;
                _disease2 = component8.DiseaseIdx != byte.MaxValue ? Db.Get().Diseases[(int) component8.DiseaseIdx].Id : (string) null;
                _disease_count2 = component8.DiseaseCount;
              }
              Rottable.Instance smi = go.gameObject.GetSMI<Rottable.Instance>();
              if (Object.op_Inequality((Object) go.GetComponent<ElementChunk>(), (Object) null))
                _isOre = true;
              Tag tag2 = go.PrefabID();
              StorageItem _storage = new StorageItem(((Tag) ref tag2).Name, _units, _temp, _element2, _disease2, _disease_count2, _isOre);
              if (smi != null)
                _storage.rottable.rotAmount = smi.RotValue;
              prefab.AssignStorage(_storage);
              _excludeEntities.Add(go);
            }
          }
          _buildings.Add(prefab);
          _excludeEntities.Add(((Component) buildingComplete).gameObject);
        }
      }
    }
    for (int idx = 0; idx < Components.Pickupables.Count; ++idx)
    {
      if (((Component) Components.Pickupables[idx]).gameObject.activeSelf)
      {
        Pickupable pickupable = Components.Pickupables[idx];
        if (!_excludeEntities.Contains(((Component) pickupable).gameObject))
        {
          int cell = Grid.PosToCell((KMonoBehaviour) pickupable);
          if ((this.SaveAllPickups || this.SelectedCells.Contains(cell)) && !Object.op_Implicit((Object) ((Component) Components.Pickupables[idx]).gameObject.GetComponent<MinionBrain>()))
          {
            int x;
            int y;
            Grid.CellToXY(cell, out x, out y);
            x -= rootX;
            y -= rootY;
            SimHashes _element = SimHashes.Void;
            float _temperature = 280f;
            float _units = 1f;
            string _disease = (string) null;
            int _disease_count = 0;
            float num7 = 0.0f;
            Rottable.Instance smi = ((Component) pickupable).gameObject.GetSMI<Rottable.Instance>();
            if (smi != null)
              num7 = smi.RotValue;
            PrimaryElement component = ((Component) pickupable).gameObject.GetComponent<PrimaryElement>();
            if (Object.op_Inequality((Object) component, (Object) null))
            {
              _element = component.ElementID;
              _units = component.Units;
              _temperature = component.Temperature;
              _disease = component.DiseaseIdx != byte.MaxValue ? Db.Get().Diseases[(int) component.DiseaseIdx].Id : (string) null;
              _disease_count = component.DiseaseCount;
            }
            Tag tag;
            if (Object.op_Inequality((Object) ((Component) pickupable).gameObject.GetComponent<ElementChunk>(), (Object) null))
            {
              tag = ((Component) pickupable).PrefabID();
              Prefab prefab = new Prefab(((Tag) ref tag).Name, Prefab.Type.Ore, x, y, _element, _temperature, _units, _disease, _disease_count);
              _primaryElementOres.Add(prefab);
            }
            else
            {
              tag = ((Component) pickupable).PrefabID();
              Prefab prefab = new Prefab(((Tag) ref tag).Name, Prefab.Type.Pickupable, x, y, _element, _temperature, _units, _disease, _disease_count)
              {
                rottable = new TemplateClasses.Rottable()
              };
              prefab.rottable.rotAmount = num7;
              _pickupables.Add(prefab);
            }
            _excludeEntities.Add(((Component) pickupable).gameObject);
          }
        }
      }
    }
    this.GetEntities<Crop>((IEnumerable<Crop>) Components.Crops.Items, rootX, rootY, ref _primaryElementOres, ref _otherEntities, ref _excludeEntities);
    this.GetEntities<Health>((IEnumerable<Health>) Components.Health.Items, rootX, rootY, ref _primaryElementOres, ref _otherEntities, ref _excludeEntities);
    this.GetEntities<Harvestable>((IEnumerable<Harvestable>) Components.Harvestables.Items, rootX, rootY, ref _primaryElementOres, ref _otherEntities, ref _excludeEntities);
    this.GetEntities<Edible>((IEnumerable<Edible>) Components.Edibles.Items, rootX, rootY, ref _primaryElementOres, ref _otherEntities, ref _excludeEntities);
    this.GetEntities<Geyser>(rootX, rootY, ref _primaryElementOres, ref _otherEntities, ref _excludeEntities);
    this.GetEntities<OccupyArea>(rootX, rootY, ref _primaryElementOres, ref _otherEntities, ref _excludeEntities);
    this.GetEntities<FogOfWarMask>(rootX, rootY, ref _primaryElementOres, ref _otherEntities, ref _excludeEntities);
    TemplateContainer selectionAsAsset = new TemplateContainer();
    selectionAsAsset.Init(_cells, _buildings, _pickupables, _primaryElementOres, _otherEntities);
    return selectionAsAsset;
  }

  private void GetEntities<T>(
    int rootX,
    int rootY,
    ref List<Prefab> _primaryElementOres,
    ref List<Prefab> _otherEntities,
    ref HashSet<GameObject> _excludeEntities)
  {
    this.GetEntities<object>((IEnumerable<object>) Object.FindObjectsOfType(typeof (T)), rootX, rootY, ref _primaryElementOres, ref _otherEntities, ref _excludeEntities);
  }

  private void GetEntities<T>(
    IEnumerable<T> component_collection,
    int rootX,
    int rootY,
    ref List<Prefab> _primaryElementOres,
    ref List<Prefab> _otherEntities,
    ref HashSet<GameObject> _excludeEntities)
  {
    foreach (T component1 in component_collection)
    {
      if (!_excludeEntities.Contains(((Component) ((object) component1 as KMonoBehaviour)).gameObject) && ((Component) ((object) component1 as KMonoBehaviour)).gameObject.activeSelf)
      {
        int cell = Grid.PosToCell((object) component1 as KMonoBehaviour);
        if (this.SelectedCells.Contains(cell) && !Object.op_Implicit((Object) ((Component) ((object) component1 as KMonoBehaviour)).gameObject.GetComponent<MinionBrain>()))
        {
          int x;
          int y;
          Grid.CellToXY(cell, out x, out y);
          x -= rootX;
          y -= rootY;
          SimHashes _element = SimHashes.Void;
          float _temperature = 280f;
          float _units = 1f;
          string _disease = (string) null;
          int _disease_count = 0;
          PrimaryElement component2 = ((Component) ((object) component1 as KMonoBehaviour)).gameObject.GetComponent<PrimaryElement>();
          if (Object.op_Inequality((Object) component2, (Object) null))
          {
            _element = component2.ElementID;
            _units = component2.Units;
            _temperature = component2.Temperature;
            _disease = component2.DiseaseIdx != byte.MaxValue ? Db.Get().Diseases[(int) component2.DiseaseIdx].Id : (string) null;
            _disease_count = component2.DiseaseCount;
          }
          List<Prefab.template_amount_value> templateAmountValueList = new List<Prefab.template_amount_value>();
          if (((Component) ((object) component1 as KMonoBehaviour)).gameObject.GetAmounts() != null)
          {
            foreach (AmountInstance amount in (Modifications<Amount, AmountInstance>) ((Component) ((object) component1 as KMonoBehaviour)).gameObject.GetAmounts())
              templateAmountValueList.Add(new Prefab.template_amount_value(amount.amount.Id, amount.value));
          }
          if (Object.op_Inequality((Object) ((Component) ((object) component1 as KMonoBehaviour)).gameObject.GetComponent<ElementChunk>(), (Object) null))
          {
            Tag tag = ((Component) ((object) component1 as KMonoBehaviour)).PrefabID();
            Prefab prefab = new Prefab(((Tag) ref tag).Name, Prefab.Type.Ore, x, y, _element, _temperature, _units, _disease, _disease_count, _amount_values: templateAmountValueList.ToArray());
            _primaryElementOres.Add(prefab);
            _excludeEntities.Add(((Component) ((object) component1 as KMonoBehaviour)).gameObject);
          }
          else
          {
            Tag tag = ((Component) ((object) component1 as KMonoBehaviour)).PrefabID();
            Prefab prefab = new Prefab(((Tag) ref tag).Name, Prefab.Type.Other, x, y, _element, _temperature, _units, _disease, _disease_count, _amount_values: templateAmountValueList.ToArray());
            _otherEntities.Add(prefab);
            _excludeEntities.Add(((Component) ((object) component1 as KMonoBehaviour)).gameObject);
          }
        }
      }
    }
  }

  private void OnClickSaveBase()
  {
    TemplateContainer selectionAsAsset = this.GetSelectionAsAsset();
    if (this.SelectedCells.Count <= 0)
    {
      Debug.LogWarning((object) "No cells selected. Use buttons above to select the area you want to save.");
    }
    else
    {
      this.SaveName = ((TMP_InputField) this.nameField).text;
      if (this.SaveName == null || this.SaveName == "")
      {
        Debug.LogWarning((object) "Invalid save name. Please enter a name in the input field.");
      }
      else
      {
        selectionAsAsset.SaveToYaml(this.SaveName);
        TemplateCache.Clear();
        TemplateCache.Init();
        PasteBaseTemplateScreen.Instance.RefreshStampButtons();
      }
    }
  }

  public void ClearSelection()
  {
    for (int index = this.SelectedCells.Count - 1; index >= 0; --index)
      this.RemoveFromSelection(this.SelectedCells[index]);
  }

  public void DestroySelection()
  {
  }

  public void DeconstructSelection()
  {
  }

  public void AddToSelection(int cell)
  {
    if (this.SelectedCells.Contains(cell))
      return;
    GameObject gameObject = Util.KInstantiate(this.Placer, (GameObject) null, (string) null);
    Grid.Objects[cell, 7] = gameObject;
    Vector3 posCbc = Grid.CellToPosCBC(cell, this.visualizerLayer);
    float num = -0.15f;
    posCbc.z += num;
    TransformExtensions.SetPosition(gameObject.transform, posCbc);
    this.SelectedCells.Add(cell);
  }

  public void RemoveFromSelection(int cell)
  {
    if (!this.SelectedCells.Contains(cell))
      return;
    GameObject gameObject = Grid.Objects[cell, 7];
    if (Object.op_Inequality((Object) gameObject, (Object) null))
      TracesExtesions.DeleteObject(gameObject);
    this.SelectedCells.Remove(cell);
  }
}
