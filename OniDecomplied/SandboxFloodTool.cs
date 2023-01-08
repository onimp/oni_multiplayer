// Decompiled with JetBrains decompiler
// Type: SandboxFloodTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class SandboxFloodTool : FloodTool
{
  public static SandboxFloodTool instance;
  protected HashSet<int> recentlyAffectedCells = new HashSet<int>();
  protected HashSet<int> cellsToAffect = new HashSet<int>();
  protected Color recentlyAffectedCellColor = new Color(1f, 1f, 1f, 0.1f);

  public static void DestroyInstance() => SandboxFloodTool.instance = (SandboxFloodTool) null;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    SandboxFloodTool.instance = this;
    this.floodCriteria = (Func<int, bool>) (cell => Grid.IsValidCell(cell) && Grid.Element[cell] == Grid.Element[this.mouseCell] && (int) Grid.WorldIdx[cell] == (int) Grid.WorldIdx[this.mouseCell]);
    this.paintArea = (Action<HashSet<int>>) (cells =>
    {
      foreach (int cell in cells)
        this.PaintCell(cell);
    });
  }

  private void PaintCell(int cell)
  {
    this.recentlyAffectedCells.Add(cell);
    Game.CallbackInfo callbackInfo = new Game.CallbackInfo((System.Action) (() => this.recentlyAffectedCells.Remove(cell)));
    Element element = ElementLoader.elements[this.settings.GetIntSetting("SandboxTools.SelectedElement")];
    byte index1 = Db.Get().Diseases.GetIndex(Db.Get().Diseases.Get("FoodPoisoning").id);
    Klei.AI.Disease disease = Db.Get().Diseases.TryGet(this.settings.GetStringSetting("SandboxTools.SelectedDisease"));
    if (disease != null)
      index1 = Db.Get().Diseases.GetIndex(disease.id);
    int index2 = Game.Instance.callbackManager.Add(callbackInfo).index;
    int gameCell = cell;
    int id = (int) element.id;
    CellElementEvent sandBoxTool = CellEventLogger.Instance.SandBoxTool;
    double floatSetting1 = (double) this.settings.GetFloatSetting("SandboxTools.Mass");
    double floatSetting2 = (double) this.settings.GetFloatSetting("SandbosTools.Temperature");
    int num = index2;
    int diseaseIdx = (int) index1;
    int intSetting = this.settings.GetIntSetting("SandboxTools.DiseaseCount");
    int callbackIdx = num;
    SimMessages.ReplaceElement(gameCell, (SimHashes) id, sandBoxTool, (float) floatSetting1, (float) floatSetting2, (byte) diseaseIdx, intSetting, callbackIdx);
  }

  private SandboxSettings settings => SandboxToolParameterMenu.instance.settings;

  public void Activate() => PlayerController.Instance.ActivateTool((InterfaceTool) this);

  protected override void OnActivateTool()
  {
    base.OnActivateTool();
    ((Component) SandboxToolParameterMenu.instance).gameObject.SetActive(true);
    SandboxToolParameterMenu.instance.DisableParameters();
    SandboxToolParameterMenu.instance.massSlider.row.SetActive(true);
    SandboxToolParameterMenu.instance.temperatureSlider.row.SetActive(true);
    SandboxToolParameterMenu.instance.elementSelector.row.SetActive(true);
    SandboxToolParameterMenu.instance.diseaseSelector.row.SetActive(true);
    SandboxToolParameterMenu.instance.diseaseCountSlider.row.SetActive(true);
  }

  protected override void OnDeactivateTool(InterfaceTool new_tool)
  {
    base.OnDeactivateTool(new_tool);
    ((Component) SandboxToolParameterMenu.instance).gameObject.SetActive(false);
  }

  public override void GetOverlayColorData(out HashSet<ToolMenu.CellColorData> colors)
  {
    colors = new HashSet<ToolMenu.CellColorData>();
    foreach (int recentlyAffectedCell in this.recentlyAffectedCells)
      colors.Add(new ToolMenu.CellColorData(recentlyAffectedCell, this.recentlyAffectedCellColor));
    foreach (int cell in this.cellsToAffect)
      colors.Add(new ToolMenu.CellColorData(cell, Color32.op_Implicit(this.areaColour)));
  }

  public override void OnMouseMove(Vector3 cursorPos)
  {
    base.OnMouseMove(cursorPos);
    this.cellsToAffect = this.Flood(Grid.PosToCell(cursorPos));
  }

  public override void OnKeyDown(KButtonEvent e)
  {
    if (e.TryConsume((Action) 242))
    {
      int cell = Grid.PosToCell(PlayerController.GetCursorPos(KInputManager.GetMousePos()));
      if (Grid.IsValidCell(cell))
        SandboxSampleTool.Sample(cell);
    }
    if (((KInputEvent) e).Consumed)
      return;
    base.OnKeyDown(e);
  }
}
