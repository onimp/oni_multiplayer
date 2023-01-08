// Decompiled with JetBrains decompiler
// Type: SandboxSprinkleTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class SandboxSprinkleTool : BrushTool
{
  public static SandboxSprinkleTool instance;
  protected HashSet<int> recentlyAffectedCells = new HashSet<int>();
  private Dictionary<int, Color> recentAffectedCellColor = new Dictionary<int, Color>();

  public static void DestroyInstance() => SandboxSprinkleTool.instance = (SandboxSprinkleTool) null;

  private SandboxSettings settings => SandboxToolParameterMenu.instance.settings;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    SandboxSprinkleTool.instance = this;
  }

  public void Activate() => PlayerController.Instance.ActivateTool((InterfaceTool) this);

  protected override void OnActivateTool()
  {
    base.OnActivateTool();
    ((Component) SandboxToolParameterMenu.instance).gameObject.SetActive(true);
    SandboxToolParameterMenu.instance.DisableParameters();
    SandboxToolParameterMenu.instance.brushRadiusSlider.row.SetActive(true);
    SandboxToolParameterMenu.instance.noiseScaleSlider.row.SetActive(true);
    SandboxToolParameterMenu.instance.noiseDensitySlider.row.SetActive(true);
    SandboxToolParameterMenu.instance.massSlider.row.SetActive(true);
    SandboxToolParameterMenu.instance.temperatureSlider.row.SetActive(true);
    SandboxToolParameterMenu.instance.elementSelector.row.SetActive(true);
    SandboxToolParameterMenu.instance.diseaseSelector.row.SetActive(true);
    SandboxToolParameterMenu.instance.diseaseCountSlider.row.SetActive(true);
    SandboxToolParameterMenu.instance.brushRadiusSlider.SetValue((float) this.settings.GetIntSetting("SandboxTools.BrushSize"));
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
      ((Color) ref color).\u002Ector(this.recentAffectedCellColor[recentlyAffectedCell].r, this.recentAffectedCellColor[recentlyAffectedCell].g, this.recentAffectedCellColor[recentlyAffectedCell].b, MathUtil.ReRange(Mathf.Sin(Time.realtimeSinceStartup * 5f), -1f, 1f, 0.1f, 0.2f));
      colors.Add(new ToolMenu.CellColorData(recentlyAffectedCell, color));
    }
    foreach (int cellsInRadiu in this.cellsInRadius)
    {
      if (this.recentlyAffectedCells.Contains(cellsInRadiu))
      {
        Color radiusIndicatorColor = this.radiusIndicatorColor;
        Color color1 = this.recentAffectedCellColor[cellsInRadiu];
        color1.a = 0.2f;
        Color color2;
        // ISSUE: explicit constructor call
        ((Color) ref color2).\u002Ector((float) (((double) radiusIndicatorColor.r + (double) color1.r) / 2.0), (float) (((double) radiusIndicatorColor.g + (double) color1.g) / 2.0), (float) (((double) radiusIndicatorColor.b + (double) color1.b) / 2.0), radiusIndicatorColor.a + (1f - radiusIndicatorColor.a) * color1.a);
        colors.Add(new ToolMenu.CellColorData(cellsInRadiu, color2));
      }
      else
        colors.Add(new ToolMenu.CellColorData(cellsInRadiu, this.radiusIndicatorColor));
    }
  }

  public override void SetBrushSize(int radius)
  {
    this.brushRadius = radius;
    this.brushOffsets.Clear();
    for (int x = 0; x < this.brushRadius * 2; ++x)
    {
      for (int y = 0; y < this.brushRadius * 2; ++y)
      {
        if ((double) Vector2.Distance(new Vector2((float) x, (float) y), new Vector2((float) this.brushRadius, (float) this.brushRadius)) < (double) this.brushRadius - 0.800000011920929)
        {
          Vector2 vector2 = Vector2I.op_Implicit(Grid.CellToXY(Grid.OffsetCell(this.currentCell, x, y)));
          float num = PerlinSimplexNoise.noise(vector2.x / this.settings.GetFloatSetting("SandboxTools.NoiseDensity"), vector2.y / this.settings.GetFloatSetting("SandboxTools.NoiseDensity"), Time.realtimeSinceStartup);
          if ((double) this.settings.GetFloatSetting("SandboxTools.NoiseScale") <= (double) num)
            this.brushOffsets.Add(new Vector2((float) (x - this.brushRadius), (float) (y - this.brushRadius)));
        }
      }
    }
  }

  private void Update() => this.OnMouseMove(Grid.CellToPos(this.currentCell));

  public override void OnMouseMove(Vector3 cursorPos)
  {
    base.OnMouseMove(cursorPos);
    this.SetBrushSize(this.settings.GetIntSetting("SandboxTools.BrushSize"));
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
    this.SetBrushSize(this.brushRadius);
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
