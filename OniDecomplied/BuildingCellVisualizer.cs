// Decompiled with JetBrains decompiler
// Type: BuildingCellVisualizer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/BuildingCellVisualizer")]
public class BuildingCellVisualizer : KMonoBehaviour
{
  private BuildingCellVisualizerResources resources;
  [MyCmpReq]
  private Building building;
  [SerializeField]
  public static Color32 secondOutputColour = Color32.op_Implicit(new Color(0.9843137f, 0.6901961f, 0.23137255f));
  [SerializeField]
  public static Color32 secondInputColour = Color32.op_Implicit(new Color(0.9843137f, 0.6901961f, 0.23137255f));
  private const BuildingCellVisualizer.Ports POWER_PORTS = BuildingCellVisualizer.Ports.PowerIn | BuildingCellVisualizer.Ports.PowerOut;
  private const BuildingCellVisualizer.Ports GAS_PORTS = BuildingCellVisualizer.Ports.GasIn | BuildingCellVisualizer.Ports.GasOut;
  private const BuildingCellVisualizer.Ports LIQUID_PORTS = BuildingCellVisualizer.Ports.LiquidIn | BuildingCellVisualizer.Ports.LiquidOut;
  private const BuildingCellVisualizer.Ports SOLID_PORTS = BuildingCellVisualizer.Ports.SolidIn | BuildingCellVisualizer.Ports.SolidOut;
  private const BuildingCellVisualizer.Ports MATTER_PORTS = BuildingCellVisualizer.Ports.GasIn | BuildingCellVisualizer.Ports.GasOut | BuildingCellVisualizer.Ports.LiquidIn | BuildingCellVisualizer.Ports.LiquidOut | BuildingCellVisualizer.Ports.SolidIn | BuildingCellVisualizer.Ports.SolidOut;
  private BuildingCellVisualizer.Ports ports;
  private BuildingCellVisualizer.Ports secondary_ports;
  private Sprite diseaseSourceSprite;
  private Color32 diseaseSourceColour;
  private GameObject inputVisualizer;
  private GameObject outputVisualizer;
  private GameObject secondaryInputVisualizer;
  private GameObject secondaryOutputVisualizer;
  private bool enableRaycast;
  private Dictionary<GameObject, Image> icons;

  public bool RequiresPowerInput => (this.ports & BuildingCellVisualizer.Ports.PowerIn) != 0;

  public bool RequiresPowerOutput => (this.ports & BuildingCellVisualizer.Ports.PowerOut) != 0;

  public bool RequiresPower => (this.ports & (BuildingCellVisualizer.Ports.PowerIn | BuildingCellVisualizer.Ports.PowerOut)) != 0;

  public bool RequiresGas => (this.ports & (BuildingCellVisualizer.Ports.GasIn | BuildingCellVisualizer.Ports.GasOut)) != 0;

  public bool RequiresLiquid => (this.ports & (BuildingCellVisualizer.Ports.LiquidIn | BuildingCellVisualizer.Ports.LiquidOut)) != 0;

  public bool RequiresSolid => (this.ports & (BuildingCellVisualizer.Ports.SolidIn | BuildingCellVisualizer.Ports.SolidOut)) != 0;

  public bool RequiresUtilityConnection => (this.ports & (BuildingCellVisualizer.Ports.GasIn | BuildingCellVisualizer.Ports.GasOut | BuildingCellVisualizer.Ports.LiquidIn | BuildingCellVisualizer.Ports.LiquidOut | BuildingCellVisualizer.Ports.SolidIn | BuildingCellVisualizer.Ports.SolidOut)) != 0;

  public bool RequiresHighEnergyParticles => (this.ports & BuildingCellVisualizer.Ports.HighEnergyParticle) != 0;

  public void ConnectedEventWithDelay(
    float delay,
    int connectionCount,
    int cell,
    string soundName)
  {
    ((MonoBehaviour) this).StartCoroutine(this.ConnectedDelay(delay, connectionCount, cell, soundName));
  }

  private IEnumerator ConnectedDelay(float delay, int connectionCount, int cell, string soundName)
  {
    BuildingCellVisualizer buildingCellVisualizer = this;
    float startTime = Time.realtimeSinceStartup;
    float currentTime = startTime;
    while ((double) currentTime < (double) startTime + (double) delay)
    {
      currentTime += Time.unscaledDeltaTime;
      yield return (object) SequenceUtil.WaitForEndOfFrame;
    }
    buildingCellVisualizer.ConnectedEvent(cell);
    string sound = GlobalAssets.GetSound(soundName);
    if (sound != null)
    {
      Vector3 position = TransformExtensions.GetPosition(buildingCellVisualizer.transform);
      position.z = 0.0f;
      EventInstance instance = SoundEvent.BeginOneShot(sound, position);
      ((EventInstance) ref instance).setParameterByName("connectedCount", (float) connectionCount, false);
      SoundEvent.EndOneShot(instance);
    }
  }

  public void ConnectedEvent(int cell)
  {
    GameObject gameObject = (GameObject) null;
    if (Object.op_Inequality((Object) this.inputVisualizer, (Object) null) && Grid.PosToCell(this.inputVisualizer) == cell)
      gameObject = this.inputVisualizer;
    else if (Object.op_Inequality((Object) this.outputVisualizer, (Object) null) && Grid.PosToCell(this.outputVisualizer) == cell)
      gameObject = this.outputVisualizer;
    else if (Object.op_Inequality((Object) this.secondaryInputVisualizer, (Object) null) && Grid.PosToCell(this.secondaryInputVisualizer) == cell)
      gameObject = this.secondaryInputVisualizer;
    else if (Object.op_Inequality((Object) this.secondaryOutputVisualizer, (Object) null) && Grid.PosToCell(this.secondaryOutputVisualizer) == cell)
      gameObject = this.secondaryOutputVisualizer;
    if (Object.op_Equality((Object) gameObject, (Object) null))
      return;
    SizePulse pulse = gameObject.gameObject.AddComponent<SizePulse>();
    pulse.speed = 20f;
    pulse.multiplier = 0.75f;
    pulse.updateWhenPaused = true;
    pulse.onComplete += (System.Action) (() => Object.Destroy((Object) pulse));
  }

  private void MapBuilding()
  {
    BuildingDef def = this.building.Def;
    if (def.CheckRequiresPowerInput())
      this.ports |= BuildingCellVisualizer.Ports.PowerIn;
    if (def.CheckRequiresPowerOutput())
      this.ports |= BuildingCellVisualizer.Ports.PowerOut;
    if (def.CheckRequiresGasInput())
      this.ports |= BuildingCellVisualizer.Ports.GasIn;
    if (def.CheckRequiresGasOutput())
      this.ports |= BuildingCellVisualizer.Ports.GasOut;
    if (def.CheckRequiresLiquidInput())
      this.ports |= BuildingCellVisualizer.Ports.LiquidIn;
    if (def.CheckRequiresLiquidOutput())
      this.ports |= BuildingCellVisualizer.Ports.LiquidOut;
    if (def.CheckRequiresSolidInput())
      this.ports |= BuildingCellVisualizer.Ports.SolidIn;
    if (def.CheckRequiresSolidOutput())
      this.ports |= BuildingCellVisualizer.Ports.SolidOut;
    if (def.CheckRequiresHighEnergyParticleInput())
      this.ports |= BuildingCellVisualizer.Ports.HighEnergyParticle;
    if (def.CheckRequiresHighEnergyParticleOutput())
      this.ports |= BuildingCellVisualizer.Ports.HighEnergyParticle;
    DiseaseVisualization.Info info = Assets.instance.DiseaseVisualization.GetInfo(HashedString.op_Implicit(def.DiseaseCellVisName));
    if (info.name != null)
    {
      this.diseaseSourceSprite = Assets.instance.DiseaseVisualization.overlaySprite;
      this.diseaseSourceColour = GlobalAssets.Instance.colorSet.GetColorByName(info.overlayColourName);
    }
    foreach (ISecondaryInput component in def.BuildingComplete.GetComponents<ISecondaryInput>())
    {
      if (component != null)
      {
        if (component.HasSecondaryConduitType(ConduitType.Gas))
          this.secondary_ports |= BuildingCellVisualizer.Ports.GasIn;
        if (component.HasSecondaryConduitType(ConduitType.Liquid))
          this.secondary_ports |= BuildingCellVisualizer.Ports.LiquidIn;
        if (component.HasSecondaryConduitType(ConduitType.Solid))
          this.secondary_ports |= BuildingCellVisualizer.Ports.SolidIn;
      }
    }
    foreach (ISecondaryOutput component in def.BuildingComplete.GetComponents<ISecondaryOutput>())
    {
      if (component != null)
      {
        if (component.HasSecondaryConduitType(ConduitType.Gas))
          this.secondary_ports |= BuildingCellVisualizer.Ports.GasOut;
        if (component.HasSecondaryConduitType(ConduitType.Liquid))
          this.secondary_ports |= BuildingCellVisualizer.Ports.LiquidOut;
        if (component.HasSecondaryConduitType(ConduitType.Solid))
          this.secondary_ports |= BuildingCellVisualizer.Ports.SolidOut;
      }
    }
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    if (Object.op_Inequality((Object) this.inputVisualizer, (Object) null))
      Object.Destroy((Object) this.inputVisualizer);
    if (Object.op_Inequality((Object) this.outputVisualizer, (Object) null))
      Object.Destroy((Object) this.outputVisualizer);
    if (Object.op_Inequality((Object) this.secondaryInputVisualizer, (Object) null))
      Object.Destroy((Object) this.secondaryInputVisualizer);
    if (!Object.op_Inequality((Object) this.secondaryOutputVisualizer, (Object) null))
      return;
    Object.Destroy((Object) this.secondaryOutputVisualizer);
  }

  private Color GetWireColor(int cell)
  {
    GameObject gameObject = Grid.Objects[cell, 26];
    if (Object.op_Equality((Object) gameObject, (Object) null))
      return Color.white;
    KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
    return !Object.op_Inequality((Object) component, (Object) null) ? Color.white : Color32.op_Implicit(component.TintColour);
  }

  protected virtual void OnCmpEnable()
  {
    base.OnCmpEnable();
    if (Object.op_Equality((Object) this.resources, (Object) null))
      this.resources = BuildingCellVisualizerResources.Instance();
    if (this.icons == null)
      this.icons = new Dictionary<GameObject, Image>();
    this.enableRaycast = Object.op_Inequality((Object) (this.building as BuildingComplete), (Object) null);
    this.MapBuilding();
    Components.BuildingCellVisualizers.Add(this);
  }

  protected virtual void OnCmpDisable()
  {
    base.OnCmpDisable();
    Components.BuildingCellVisualizers.Remove(this);
  }

  public void DrawIcons(HashedString mode)
  {
    if (((Component) this).gameObject.GetMyWorldId() != ClusterManager.Instance.activeWorldId)
      this.DisableIcons();
    else if (HashedString.op_Equality(mode, OverlayModes.Power.ID))
    {
      if (this.RequiresPower)
      {
        bool flag = Object.op_Inequality((Object) (this.building as BuildingPreview), (Object) null);
        BuildingEnabledButton component = ((Component) this.building).GetComponent<BuildingEnabledButton>();
        int powerInputCell = this.building.GetPowerInputCell();
        if (this.RequiresPowerInput)
        {
          int circuitId = (int) Game.Instance.circuitManager.GetCircuitID(powerInputCell);
          Color tint = !Object.op_Inequality((Object) component, (Object) null) || component.IsEnabled ? Color.white : Color.gray;
          Sprite icon_img = flag || circuitId == (int) ushort.MaxValue ? this.resources.electricityInputIcon : this.resources.electricityConnectedIcon;
          this.DrawUtilityIcon(powerInputCell, icon_img, ref this.inputVisualizer, tint, this.GetWireColor(powerInputCell), 1f);
        }
        if (!this.RequiresPowerOutput)
          return;
        int powerOutputCell = this.building.GetPowerOutputCell();
        int circuitId1 = (int) Game.Instance.circuitManager.GetCircuitID(powerOutputCell);
        Color color = this.building.Def.UseWhitePowerOutputConnectorColour ? Color.white : this.resources.electricityOutputColor;
        Color32 color32 = Color32.op_Implicit(!Object.op_Inequality((Object) component, (Object) null) || component.IsEnabled ? color : Color.gray);
        Sprite icon_img1 = flag || circuitId1 == (int) ushort.MaxValue ? this.resources.electricityInputIcon : this.resources.electricityConnectedIcon;
        this.DrawUtilityIcon(powerOutputCell, icon_img1, ref this.outputVisualizer, Color32.op_Implicit(color32), this.GetWireColor(powerOutputCell), 1f);
      }
      else
      {
        bool flag = true;
        Switch component1 = ((Component) this).GetComponent<Switch>();
        if (Object.op_Inequality((Object) component1, (Object) null))
        {
          this.DrawUtilityIcon(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)), this.resources.switchIcon, ref this.outputVisualizer, Color32.op_Implicit(component1.IsHandlerOn() ? this.resources.switchColor : this.resources.switchOffColor), Color.white, 1f);
          flag = false;
        }
        else
        {
          WireUtilityNetworkLink component2 = ((Component) this).GetComponent<WireUtilityNetworkLink>();
          if (Object.op_Inequality((Object) component2, (Object) null))
          {
            int linked_cell1;
            int linked_cell2;
            component2.GetCells(out linked_cell1, out linked_cell2);
            this.DrawUtilityIcon(linked_cell1, Game.Instance.circuitManager.GetCircuitID(linked_cell1) == ushort.MaxValue ? this.resources.electricityBridgeIcon : this.resources.electricityConnectedIcon, ref this.inputVisualizer, this.resources.electricityInputColor, Color.white, 1f);
            this.DrawUtilityIcon(linked_cell2, Game.Instance.circuitManager.GetCircuitID(linked_cell2) == ushort.MaxValue ? this.resources.electricityBridgeIcon : this.resources.electricityConnectedIcon, ref this.outputVisualizer, this.resources.electricityInputColor, Color.white, 1f);
            flag = false;
          }
        }
        if (!flag)
          return;
        this.DisableIcons();
      }
    }
    else if (HashedString.op_Equality(mode, OverlayModes.GasConduits.ID))
    {
      if (this.RequiresGas || (this.secondary_ports & (BuildingCellVisualizer.Ports.GasIn | BuildingCellVisualizer.Ports.GasOut)) != (BuildingCellVisualizer.Ports) 0)
      {
        if ((this.ports & BuildingCellVisualizer.Ports.GasIn) != (BuildingCellVisualizer.Ports) 0)
        {
          int num = Object.op_Inequality((Object) null, (Object) Grid.Objects[this.building.GetUtilityInputCell(), 12]) ? 1 : 0;
          BuildingCellVisualizerResources.ConnectedDisconnectedColours input = this.resources.gasIOColours.input;
          Color tint = Color32.op_Implicit(num != 0 ? input.connected : input.disconnected);
          this.DrawUtilityIcon(this.building.GetUtilityInputCell(), this.resources.gasInputIcon, ref this.inputVisualizer, tint);
        }
        if ((this.ports & BuildingCellVisualizer.Ports.GasOut) != (BuildingCellVisualizer.Ports) 0)
        {
          int num = Object.op_Inequality((Object) null, (Object) Grid.Objects[this.building.GetUtilityOutputCell(), 12]) ? 1 : 0;
          BuildingCellVisualizerResources.ConnectedDisconnectedColours output = this.resources.gasIOColours.output;
          Color tint = Color32.op_Implicit(num != 0 ? output.connected : output.disconnected);
          this.DrawUtilityIcon(this.building.GetUtilityOutputCell(), this.resources.gasOutputIcon, ref this.outputVisualizer, tint);
        }
        if ((this.secondary_ports & BuildingCellVisualizer.Ports.GasIn) != (BuildingCellVisualizer.Ports) 0)
        {
          ISecondaryInput[] components = ((Component) this.building).GetComponents<ISecondaryInput>();
          CellOffset offset = CellOffset.none;
          foreach (ISecondaryInput secondaryInput in components)
          {
            offset = secondaryInput.GetSecondaryConduitOffset(ConduitType.Gas);
            if (CellOffset.op_Inequality(offset, CellOffset.none))
              break;
          }
          Color tint = Color32.op_Implicit(BuildingCellVisualizer.secondInputColour);
          if ((this.ports & BuildingCellVisualizer.Ports.GasIn) == (BuildingCellVisualizer.Ports) 0)
          {
            int num = Object.op_Inequality((Object) null, (Object) Grid.Objects[Grid.OffsetCell(Grid.PosToCell(TransformExtensions.GetPosition(this.building.transform)), offset), 12]) ? 1 : 0;
            BuildingCellVisualizerResources.ConnectedDisconnectedColours input = this.resources.gasIOColours.input;
            tint = Color32.op_Implicit(num != 0 ? input.connected : input.disconnected);
          }
          this.DrawUtilityIcon(this.GetVisualizerCell(this.building, offset), this.resources.gasInputIcon, ref this.secondaryInputVisualizer, tint, Color.white);
        }
        if ((this.secondary_ports & BuildingCellVisualizer.Ports.GasOut) == (BuildingCellVisualizer.Ports) 0)
          return;
        ISecondaryOutput[] components1 = ((Component) this.building).GetComponents<ISecondaryOutput>();
        CellOffset offset1 = CellOffset.none;
        foreach (ISecondaryOutput secondaryOutput in components1)
        {
          offset1 = secondaryOutput.GetSecondaryConduitOffset(ConduitType.Gas);
          if (CellOffset.op_Inequality(offset1, CellOffset.none))
            break;
        }
        Color tint1 = Color32.op_Implicit(BuildingCellVisualizer.secondOutputColour);
        if ((this.ports & BuildingCellVisualizer.Ports.GasOut) == (BuildingCellVisualizer.Ports) 0)
        {
          int num = Object.op_Inequality((Object) null, (Object) Grid.Objects[Grid.OffsetCell(Grid.PosToCell(TransformExtensions.GetPosition(this.building.transform)), offset1), 12]) ? 1 : 0;
          BuildingCellVisualizerResources.ConnectedDisconnectedColours output = this.resources.gasIOColours.output;
          tint1 = Color32.op_Implicit(num != 0 ? output.connected : output.disconnected);
        }
        this.DrawUtilityIcon(this.GetVisualizerCell(this.building, offset1), this.resources.gasOutputIcon, ref this.secondaryOutputVisualizer, tint1, Color.white);
      }
      else
        this.DisableIcons();
    }
    else if (HashedString.op_Equality(mode, OverlayModes.LiquidConduits.ID))
    {
      if (this.RequiresLiquid || (this.secondary_ports & (BuildingCellVisualizer.Ports.LiquidIn | BuildingCellVisualizer.Ports.LiquidOut)) != (BuildingCellVisualizer.Ports) 0)
      {
        if ((this.ports & BuildingCellVisualizer.Ports.LiquidIn) != (BuildingCellVisualizer.Ports) 0)
        {
          int num = Object.op_Inequality((Object) null, (Object) Grid.Objects[this.building.GetUtilityInputCell(), 16]) ? 1 : 0;
          BuildingCellVisualizerResources.ConnectedDisconnectedColours input = this.resources.liquidIOColours.input;
          Color tint = Color32.op_Implicit(num != 0 ? input.connected : input.disconnected);
          this.DrawUtilityIcon(this.building.GetUtilityInputCell(), this.resources.liquidInputIcon, ref this.inputVisualizer, tint);
        }
        if ((this.ports & BuildingCellVisualizer.Ports.LiquidOut) != (BuildingCellVisualizer.Ports) 0)
        {
          int num = Object.op_Inequality((Object) null, (Object) Grid.Objects[this.building.GetUtilityOutputCell(), 16]) ? 1 : 0;
          BuildingCellVisualizerResources.ConnectedDisconnectedColours output = this.resources.liquidIOColours.output;
          Color tint = Color32.op_Implicit(num != 0 ? output.connected : output.disconnected);
          this.DrawUtilityIcon(this.building.GetUtilityOutputCell(), this.resources.liquidOutputIcon, ref this.outputVisualizer, tint);
        }
        if ((this.secondary_ports & BuildingCellVisualizer.Ports.LiquidIn) != (BuildingCellVisualizer.Ports) 0)
        {
          ISecondaryInput[] components = ((Component) this.building).GetComponents<ISecondaryInput>();
          CellOffset offset = CellOffset.none;
          foreach (ISecondaryInput secondaryInput in components)
          {
            offset = secondaryInput.GetSecondaryConduitOffset(ConduitType.Liquid);
            if (CellOffset.op_Inequality(offset, CellOffset.none))
              break;
          }
          Color tint = Color32.op_Implicit(BuildingCellVisualizer.secondInputColour);
          if ((this.ports & BuildingCellVisualizer.Ports.LiquidIn) == (BuildingCellVisualizer.Ports) 0)
          {
            int num = Object.op_Inequality((Object) null, (Object) Grid.Objects[Grid.OffsetCell(Grid.PosToCell(TransformExtensions.GetPosition(this.building.transform)), offset), 16]) ? 1 : 0;
            BuildingCellVisualizerResources.ConnectedDisconnectedColours input = this.resources.liquidIOColours.input;
            tint = Color32.op_Implicit(num != 0 ? input.connected : input.disconnected);
          }
          this.DrawUtilityIcon(this.GetVisualizerCell(this.building, offset), this.resources.liquidInputIcon, ref this.secondaryInputVisualizer, tint, Color.white);
        }
        if ((this.secondary_ports & BuildingCellVisualizer.Ports.LiquidOut) == (BuildingCellVisualizer.Ports) 0)
          return;
        ISecondaryOutput[] components2 = ((Component) this.building).GetComponents<ISecondaryOutput>();
        CellOffset offset2 = CellOffset.none;
        foreach (ISecondaryOutput secondaryOutput in components2)
        {
          offset2 = secondaryOutput.GetSecondaryConduitOffset(ConduitType.Liquid);
          if (CellOffset.op_Inequality(offset2, CellOffset.none))
            break;
        }
        Color tint2 = Color32.op_Implicit(BuildingCellVisualizer.secondOutputColour);
        if ((this.ports & BuildingCellVisualizer.Ports.LiquidOut) == (BuildingCellVisualizer.Ports) 0)
        {
          int num = Object.op_Inequality((Object) null, (Object) Grid.Objects[Grid.OffsetCell(Grid.PosToCell(TransformExtensions.GetPosition(this.building.transform)), offset2), 16]) ? 1 : 0;
          BuildingCellVisualizerResources.ConnectedDisconnectedColours output = this.resources.liquidIOColours.output;
          tint2 = Color32.op_Implicit(num != 0 ? output.connected : output.disconnected);
        }
        this.DrawUtilityIcon(this.GetVisualizerCell(this.building, offset2), this.resources.liquidOutputIcon, ref this.secondaryOutputVisualizer, tint2, Color.white);
      }
      else
        this.DisableIcons();
    }
    else if (HashedString.op_Equality(mode, OverlayModes.SolidConveyor.ID))
    {
      if (this.RequiresSolid || (this.secondary_ports & (BuildingCellVisualizer.Ports.SolidIn | BuildingCellVisualizer.Ports.SolidOut)) != (BuildingCellVisualizer.Ports) 0)
      {
        if ((this.ports & BuildingCellVisualizer.Ports.SolidIn) != (BuildingCellVisualizer.Ports) 0)
        {
          int num = Object.op_Inequality((Object) null, (Object) Grid.Objects[this.building.GetUtilityInputCell(), 20]) ? 1 : 0;
          BuildingCellVisualizerResources.ConnectedDisconnectedColours input = this.resources.liquidIOColours.input;
          Color tint = Color32.op_Implicit(num != 0 ? input.connected : input.disconnected);
          this.DrawUtilityIcon(this.building.GetUtilityInputCell(), this.resources.liquidInputIcon, ref this.inputVisualizer, tint);
        }
        if ((this.ports & BuildingCellVisualizer.Ports.SolidOut) != (BuildingCellVisualizer.Ports) 0)
        {
          int num = Object.op_Inequality((Object) null, (Object) Grid.Objects[this.building.GetUtilityOutputCell(), 20]) ? 1 : 0;
          BuildingCellVisualizerResources.ConnectedDisconnectedColours output = this.resources.liquidIOColours.output;
          Color tint = Color32.op_Implicit(num != 0 ? output.connected : output.disconnected);
          this.DrawUtilityIcon(this.building.GetUtilityOutputCell(), this.resources.liquidOutputIcon, ref this.outputVisualizer, tint);
        }
        if ((this.secondary_ports & BuildingCellVisualizer.Ports.SolidIn) != (BuildingCellVisualizer.Ports) 0)
        {
          ISecondaryInput[] components = ((Component) this.building).GetComponents<ISecondaryInput>();
          CellOffset offset = CellOffset.none;
          foreach (ISecondaryInput secondaryInput in components)
          {
            offset = secondaryInput.GetSecondaryConduitOffset(ConduitType.Solid);
            if (CellOffset.op_Inequality(offset, CellOffset.none))
              break;
          }
          Color tint = Color32.op_Implicit(BuildingCellVisualizer.secondInputColour);
          if ((this.ports & BuildingCellVisualizer.Ports.SolidIn) == (BuildingCellVisualizer.Ports) 0)
          {
            int num = Object.op_Inequality((Object) null, (Object) Grid.Objects[Grid.OffsetCell(Grid.PosToCell(TransformExtensions.GetPosition(this.building.transform)), offset), 20]) ? 1 : 0;
            BuildingCellVisualizerResources.ConnectedDisconnectedColours input = this.resources.liquidIOColours.input;
            tint = Color32.op_Implicit(num != 0 ? input.connected : input.disconnected);
          }
          this.DrawUtilityIcon(this.GetVisualizerCell(this.building, offset), this.resources.liquidInputIcon, ref this.secondaryInputVisualizer, tint, Color.white);
        }
        if ((this.secondary_ports & BuildingCellVisualizer.Ports.SolidOut) == (BuildingCellVisualizer.Ports) 0)
          return;
        ISecondaryOutput[] components3 = ((Component) this.building).GetComponents<ISecondaryOutput>();
        CellOffset offset3 = CellOffset.none;
        foreach (ISecondaryOutput secondaryOutput in components3)
        {
          offset3 = secondaryOutput.GetSecondaryConduitOffset(ConduitType.Solid);
          if (CellOffset.op_Inequality(offset3, CellOffset.none))
            break;
        }
        Color tint3 = Color32.op_Implicit(BuildingCellVisualizer.secondOutputColour);
        if ((this.ports & BuildingCellVisualizer.Ports.SolidOut) == (BuildingCellVisualizer.Ports) 0)
        {
          int num = Object.op_Inequality((Object) null, (Object) Grid.Objects[Grid.OffsetCell(Grid.PosToCell(TransformExtensions.GetPosition(this.building.transform)), offset3), 20]) ? 1 : 0;
          BuildingCellVisualizerResources.ConnectedDisconnectedColours output = this.resources.liquidIOColours.output;
          tint3 = Color32.op_Implicit(num != 0 ? output.connected : output.disconnected);
        }
        this.DrawUtilityIcon(this.GetVisualizerCell(this.building, offset3), this.resources.liquidOutputIcon, ref this.secondaryOutputVisualizer, tint3, Color.white);
      }
      else
        this.DisableIcons();
    }
    else if (HashedString.op_Equality(mode, OverlayModes.Disease.ID))
    {
      if (!Object.op_Inequality((Object) this.diseaseSourceSprite, (Object) null))
        return;
      this.DrawUtilityIcon(this.building.GetUtilityOutputCell(), this.diseaseSourceSprite, ref this.inputVisualizer, Color32.op_Implicit(this.diseaseSourceColour));
    }
    else
    {
      if (!HashedString.op_Equality(mode, OverlayModes.Radiation.ID) || !this.RequiresHighEnergyParticles)
        return;
      int scaleMultiplier = 3;
      if (this.building.Def.UseHighEnergyParticleInputPort)
        this.DrawUtilityIcon(this.building.GetHighEnergyParticleInputCell(), this.resources.highEnergyParticleInputIcon, ref this.inputVisualizer, this.resources.highEnergyParticleInputColour, Color.white, (float) scaleMultiplier, true);
      if (!this.building.Def.UseHighEnergyParticleOutputPort)
        return;
      int particleOutputCell = this.building.GetHighEnergyParticleOutputCell();
      IHighEnergyParticleDirection component = ((Component) this.building).GetComponent<IHighEnergyParticleDirection>();
      Sprite particleOutputIcon = this.resources.highEnergyParticleOutputIcons[0];
      if (component != null)
        particleOutputIcon = this.resources.highEnergyParticleOutputIcons[EightDirectionUtil.GetDirectionIndex(component.Direction)];
      this.DrawUtilityIcon(particleOutputCell, particleOutputIcon, ref this.outputVisualizer, this.resources.highEnergyParticleOutputColour, Color.white, (float) scaleMultiplier, true);
    }
  }

  private int GetVisualizerCell(Building building, CellOffset offset)
  {
    CellOffset rotatedOffset = building.GetRotatedOffset(offset);
    return Grid.OffsetCell(building.GetCell(), rotatedOffset);
  }

  public void DisableIcons()
  {
    if (Object.op_Inequality((Object) this.inputVisualizer, (Object) null) && this.inputVisualizer.activeInHierarchy)
      this.inputVisualizer.SetActive(false);
    if (Object.op_Inequality((Object) this.outputVisualizer, (Object) null) && this.outputVisualizer.activeInHierarchy)
      this.outputVisualizer.SetActive(false);
    if (Object.op_Inequality((Object) this.secondaryInputVisualizer, (Object) null) && this.secondaryInputVisualizer.activeInHierarchy)
      this.secondaryInputVisualizer.SetActive(false);
    if (!Object.op_Inequality((Object) this.secondaryOutputVisualizer, (Object) null) || !this.secondaryOutputVisualizer.activeInHierarchy)
      return;
    this.secondaryOutputVisualizer.SetActive(false);
  }

  private void DrawUtilityIcon(int cell, Sprite icon_img, ref GameObject visualizerObj) => this.DrawUtilityIcon(cell, icon_img, ref visualizerObj, Color.white, Color.white);

  private void DrawUtilityIcon(
    int cell,
    Sprite icon_img,
    ref GameObject visualizerObj,
    Color tint)
  {
    this.DrawUtilityIcon(cell, icon_img, ref visualizerObj, tint, Color.white);
  }

  private void DrawUtilityIcon(
    int cell,
    Sprite icon_img,
    ref GameObject visualizerObj,
    Color tint,
    Color connectorColor,
    float scaleMultiplier = 1.5f,
    bool hideBG = false)
  {
    Vector3 posCcc = Grid.CellToPosCCC(cell, Grid.SceneLayer.Building);
    if (Object.op_Equality((Object) visualizerObj, (Object) null))
    {
      visualizerObj = Util.KInstantiate(Assets.UIPrefabs.ResourceVisualizer, GameScreenManager.Instance.worldSpaceCanvas, (string) null);
      visualizerObj.transform.SetAsFirstSibling();
      this.icons.Add(visualizerObj, ((Component) visualizerObj.transform.GetChild(0)).GetComponent<Image>());
    }
    if (!visualizerObj.gameObject.activeInHierarchy)
      visualizerObj.gameObject.SetActive(true);
    ((Behaviour) visualizerObj.GetComponent<Image>()).enabled = !hideBG;
    ((Graphic) this.icons[visualizerObj]).raycastTarget = this.enableRaycast;
    this.icons[visualizerObj].sprite = icon_img;
    ((Graphic) ((Component) visualizerObj.transform.GetChild(0)).gameObject.GetComponent<Image>()).color = tint;
    TransformExtensions.SetPosition(visualizerObj.transform, posCcc);
    if (!Object.op_Equality((Object) visualizerObj.GetComponent<SizePulse>(), (Object) null))
      return;
    visualizerObj.transform.localScale = Vector3.op_Multiply(Vector3.one, scaleMultiplier);
  }

  public Image GetOutputIcon() => !Object.op_Equality((Object) this.outputVisualizer, (Object) null) ? ((Component) this.outputVisualizer.transform.GetChild(0)).GetComponent<Image>() : (Image) null;

  public Image GetInputIcon() => !Object.op_Equality((Object) this.inputVisualizer, (Object) null) ? ((Component) this.inputVisualizer.transform.GetChild(0)).GetComponent<Image>() : (Image) null;

  [Flags]
  private enum Ports
  {
    PowerIn = 1,
    PowerOut = 2,
    GasIn = 4,
    GasOut = 8,
    LiquidIn = 16, // 0x00000010
    LiquidOut = 32, // 0x00000020
    SolidIn = 64, // 0x00000040
    SolidOut = 128, // 0x00000080
    HighEnergyParticle = 256, // 0x00000100
  }
}
