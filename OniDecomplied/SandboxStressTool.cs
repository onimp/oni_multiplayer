// Decompiled with JetBrains decompiler
// Type: SandboxStressTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SandboxStressTool : BrushTool
{
  public static SandboxStressTool instance;
  protected HashSet<int> recentlyAffectedCells = new HashSet<int>();
  protected Color recentlyAffectedCellColor = new Color(1f, 1f, 1f, 0.1f);
  private Dictionary<MinionIdentity, AttributeModifier> moraleAdjustments = new Dictionary<MinionIdentity, AttributeModifier>();

  public static void DestroyInstance() => SandboxStressTool.instance = (SandboxStressTool) null;

  public override string[] DlcIDs => DlcManager.AVAILABLE_ALL_VERSIONS;

  private SandboxSettings settings => SandboxToolParameterMenu.instance.settings;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    SandboxStressTool.instance = this;
  }

  public void Activate() => PlayerController.Instance.ActivateTool((InterfaceTool) this);

  protected override void OnActivateTool()
  {
    base.OnActivateTool();
    ((Component) SandboxToolParameterMenu.instance).gameObject.SetActive(true);
    SandboxToolParameterMenu.instance.DisableParameters();
    SandboxToolParameterMenu.instance.brushRadiusSlider.row.SetActive(true);
    SandboxToolParameterMenu.instance.stressAdditiveSlider.row.SetActive(true);
    SandboxToolParameterMenu.instance.stressAdditiveSlider.SetValue(5f);
    SandboxToolParameterMenu.instance.moraleSlider.SetValue(0.0f);
    if (!DebugHandler.InstantBuildMode)
      return;
    SandboxToolParameterMenu.instance.moraleSlider.row.SetActive(true);
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
    foreach (int cellsInRadiu in this.cellsInRadius)
      colors.Add(new ToolMenu.CellColorData(cellsInRadiu, this.radiusIndicatorColor));
  }

  public override void OnMouseMove(Vector3 cursorPos) => base.OnMouseMove(cursorPos);

  protected override void OnPaintCell(int cell, int distFromOrigin)
  {
    base.OnPaintCell(cell, distFromOrigin);
    for (int idx = 0; idx < Components.LiveMinionIdentities.Count; ++idx)
    {
      GameObject gameObject = ((Component) Components.LiveMinionIdentities[idx]).gameObject;
      if (Grid.PosToCell(gameObject) == cell)
      {
        float num1 = -1f * SandboxToolParameterMenu.instance.settings.GetFloatSetting("SandbosTools.StressAdditive");
        double num2 = (double) Db.Get().Amounts.Stress.Lookup(((Component) Components.LiveMinionIdentities[idx]).gameObject).ApplyDelta(num1);
        PopFXManager.Instance.SpawnFX((double) num1 >= 0.0 ? Assets.GetSprite(HashedString.op_Implicit("crew_state_angry")) : Assets.GetSprite(HashedString.op_Implicit("crew_state_happy")), GameUtil.GetFormattedPercent(num1), gameObject.transform);
        int intSetting = SandboxToolParameterMenu.instance.settings.GetIntSetting("SandbosTools.MoraleAdjustment");
        AttributeInstance attributeInstance = gameObject.GetAttributes().Get(Db.Get().Attributes.QualityOfLife);
        MinionIdentity component = gameObject.GetComponent<MinionIdentity>();
        if (this.moraleAdjustments.ContainsKey(component))
        {
          attributeInstance.Remove(this.moraleAdjustments[component]);
          this.moraleAdjustments.Remove(component);
        }
        if (intSetting != 0)
        {
          AttributeModifier modifier = new AttributeModifier(attributeInstance.Id, (float) intSetting, (Func<string>) (() => (string) DUPLICANTS.MODIFIERS.SANDBOXMORALEADJUSTMENT.NAME));
          modifier.SetValue((float) intSetting);
          attributeInstance.Add(modifier);
          this.moraleAdjustments.Add(component, modifier);
        }
      }
    }
  }
}
