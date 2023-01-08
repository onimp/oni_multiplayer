// Decompiled with JetBrains decompiler
// Type: SandboxBrushTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class SandboxBrushTool : BrushTool
{
  public static SandboxBrushTool instance;
  protected HashSet<int> recentlyAffectedCells = new HashSet<int>();
  private Dictionary<int, Color> recentAffectedCellColor = new Dictionary<int, Color>();

  public static void DestroyInstance() => SandboxBrushTool.instance = (SandboxBrushTool) null;

  private SandboxSettings settings => SandboxToolParameterMenu.instance.settings;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    SandboxBrushTool.instance = this;
  }

  public void Activate() => PlayerController.Instance.ActivateTool((InterfaceTool) this);

  protected override void OnActivateTool()
  {
    base.OnActivateTool();
    ((Component) SandboxToolParameterMenu.instance).gameObject.SetActive(true);
    SandboxToolParameterMenu.instance.DisableParameters();
    SandboxToolParameterMenu.instance.brushRadiusSlider.row.SetActive(true);
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
    {
      Color color;
      // ISSUE: explicit constructor call
      ((Color) ref color).\u002Ector(this.recentAffectedCellColor[recentlyAffectedCell].r, this.recentAffectedCellColor[recentlyAffectedCell].g, this.recentAffectedCellColor[recentlyAffectedCell].b, MathUtil.ReRange(Mathf.Sin(Time.realtimeSinceStartup * 10f), -1f, 1f, 0.1f, 0.2f));
      colors.Add(new ToolMenu.CellColorData(recentlyAffectedCell, color));
    }
    foreach (int cellsInRadiu in this.cellsInRadius)
      colors.Add(new ToolMenu.CellColorData(cellsInRadiu, this.radiusIndicatorColor));
  }

  public override void SetBrushSize(int radius)
  {
    this.brushRadius = radius;
    this.brushOffsets.Clear();
    for (int index1 = 0; index1 < this.brushRadius * 2; ++index1)
    {
      for (int index2 = 0; index2 < this.brushRadius * 2; ++index2)
      {
        if ((double) Vector2.Distance(new Vector2((float) index1, (float) index2), new Vector2((float) this.brushRadius, (float) this.brushRadius)) < (double) this.brushRadius - 0.800000011920929)
          this.brushOffsets.Add(new Vector2((float) (index1 - this.brushRadius), (float) (index2 - this.brushRadius)));
      }
    }
  }

  protected override void OnPaintCell(int cell, int distFromOrigin)
  {
    base.OnPaintCell(cell, distFromOrigin);
    this.recentlyAffectedCells.Add(cell);
    Element element = ElementLoader.elements[this.settings.GetIntSetting("SandboxTools.SelectedElement")];
    if (!this.recentAffectedCellColor.ContainsKey(cell))
      this.recentAffectedCellColor.Add(cell, Color32.op_Implicit(element.substance.uiColour));
    else
      this.recentAffectedCellColor[cell] = Color32.op_Implicit(element.substance.uiColour);
    int index1 = Game.Instance.callbackManager.Add(new Game.CallbackInfo((System.Action) (() =>
    {
      this.recentlyAffectedCells.Remove(cell);
      this.recentAffectedCellColor.Remove(cell);
    }))).index;
    byte index2 = Db.Get().Diseases.GetIndex(Db.Get().Diseases.Get("FoodPoisoning").id);
    Klei.AI.Disease disease = Db.Get().Diseases.TryGet(this.settings.GetStringSetting("SandboxTools.SelectedDisease"));
    if (disease != null)
      index2 = Db.Get().Diseases.GetIndex(disease.id);
    int gameCell = cell;
    int id = (int) element.id;
    CellElementEvent sandBoxTool = CellEventLogger.Instance.SandBoxTool;
    double floatSetting1 = (double) this.settings.GetFloatSetting("SandboxTools.Mass");
    double floatSetting2 = (double) this.settings.GetFloatSetting("SandbosTools.Temperature");
    int num = index1;
    int diseaseIdx = (int) index2;
    int intSetting = this.settings.GetIntSetting("SandboxTools.DiseaseCount");
    int callbackIdx = num;
    SimMessages.ReplaceElement(gameCell, (SimHashes) id, sandBoxTool, (float) floatSetting1, (float) floatSetting2, (byte) diseaseIdx, intSetting, callbackIdx);
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
