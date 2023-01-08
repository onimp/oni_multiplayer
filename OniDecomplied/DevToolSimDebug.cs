// Decompiled with JetBrains decompiler
// Type: DevToolSimDebug
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class DevToolSimDebug : DevTool
{
  private Vector3 worldPos = Vector3.zero;
  private string[] elementNames;
  private Dictionary<SimHashes, double> elementCounts = new Dictionary<SimHashes, double>();
  public static DevToolSimDebug Instance;
  private const string INVALID_OVERLAY_MODE_STR = "None";
  private bool showElementData;
  private bool showMouseData = true;
  private bool showAccessRestrictions;
  private bool showGridContents;
  private bool showScenePartitionerContents;
  private bool showLayerToggles;
  private bool showCavityInfo;
  private bool showPropertyInfo;
  private bool showBuildings;
  private bool showPhysicsData;
  private bool showGasConduitData;
  private bool showLiquidConduitData;
  private string[] overlayModes;
  private int selectedOverlayMode;
  private string[] gameGridModes;
  private Dictionary<string, HashedString> modeLookup;
  private Dictionary<HashedString, string> revModeLookup;
  private HashSet<ScenePartitionerLayer> toggledLayers = new HashSet<ScenePartitionerLayer>();

  public DevToolSimDebug()
  {
    this.elementNames = Enum.GetNames(typeof (SimHashes));
    Array.Sort<string>(this.elementNames);
    DevToolSimDebug.Instance = this;
    List<string> stringList = new List<string>();
    this.modeLookup = new Dictionary<string, HashedString>();
    this.revModeLookup = new Dictionary<HashedString, string>();
    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
    {
      foreach (System.Type type in assembly.GetTypes())
      {
        if (typeof (OverlayModes.Mode).IsAssignableFrom(type))
        {
          FieldInfo field = type.GetField("ID");
          if (field != (FieldInfo) null)
          {
            object obj = field.GetValue((object) null);
            if (obj != null)
            {
              HashedString key = (HashedString) obj;
              stringList.Add(type.Name);
              this.modeLookup[type.Name] = key;
              this.revModeLookup[key] = type.Name;
            }
          }
        }
      }
    }
    foreach (FieldInfo field in typeof (SimDebugView.OverlayModes).GetFields())
    {
      if (field.FieldType == typeof (HashedString))
      {
        object obj = field.GetValue((object) null);
        if (obj != null)
        {
          HashedString key = (HashedString) obj;
          stringList.Add(field.Name);
          this.modeLookup[field.Name] = key;
          this.revModeLookup[key] = field.Name;
        }
      }
    }
    stringList.Sort();
    stringList.Insert(0, "None");
    this.modeLookup["None"] = HashedString.op_Implicit("None");
    this.revModeLookup[HashedString.op_Implicit("None")] = "None";
    stringList.RemoveAll((Predicate<string>) (s => s == null));
    this.overlayModes = stringList.ToArray();
    this.gameGridModes = Enum.GetNames(typeof (SimDebugView.GameGridMode));
  }

  protected override void RenderTo(DevPanel panel)
  {
    if (Object.op_Equality((Object) Game.Instance, (Object) null))
      return;
    HashedString hashedString1 = SimDebugView.Instance.GetMode();
    HashedString hashedString2 = hashedString1;
    if (this.overlayModes != null)
    {
      this.selectedOverlayMode = Array.IndexOf<string>(this.overlayModes, this.revModeLookup[hashedString1]);
      this.selectedOverlayMode = this.selectedOverlayMode == -1 ? 0 : this.selectedOverlayMode;
      ImGui.Combo("Debug Mode", ref this.selectedOverlayMode, this.overlayModes, this.overlayModes.Length);
      hashedString1 = this.modeLookup[this.overlayModes[this.selectedOverlayMode]];
      if (HashedString.op_Equality(hashedString1, HashedString.op_Implicit("None")))
        hashedString1 = OverlayModes.None.ID;
    }
    if (HashedString.op_Inequality(hashedString1, hashedString2))
      SimDebugView.Instance.SetMode(hashedString1);
    if (HashedString.op_Equality(hashedString1, OverlayModes.Temperature.ID))
    {
      ImGui.InputFloat("Min Expected Temp:", ref SimDebugView.Instance.minTempExpected);
      ImGui.InputFloat("Max Expected Temp:", ref SimDebugView.Instance.maxTempExpected);
    }
    else if (HashedString.op_Equality(hashedString1, SimDebugView.OverlayModes.Mass))
    {
      ImGui.InputFloat("Min Expected Mass:", ref SimDebugView.Instance.minMassExpected);
      ImGui.InputFloat("Max Expected Mass:", ref SimDebugView.Instance.maxMassExpected);
    }
    else if (HashedString.op_Equality(hashedString1, SimDebugView.OverlayModes.Pressure))
    {
      ImGui.InputFloat("Min Expected Pressure:", ref SimDebugView.Instance.minPressureExpected);
      ImGui.InputFloat("Max Expected Pressure:", ref SimDebugView.Instance.maxPressureExpected);
    }
    else if (HashedString.op_Equality(hashedString1, SimDebugView.OverlayModes.GameGrid))
    {
      int gameGridMode = (int) SimDebugView.Instance.GetGameGridMode();
      ImGui.Combo("Grid Mode", ref gameGridMode, this.gameGridModes, this.gameGridModes.Length);
      SimDebugView.Instance.SetGameGridMode((SimDebugView.GameGridMode) gameGridMode);
    }
    int x;
    int y;
    Grid.PosToXY(this.worldPos, out x, out y);
    int index = y * Grid.WidthInCells + x;
    this.showMouseData = ImGui.CollapsingHeader("Mouse Data");
    if (this.showMouseData)
    {
      ImGui.Indent();
      ImGui.Text("WorldPos: " + this.worldPos.ToString());
      ImGui.Unindent();
    }
    if (index < 0 || Grid.CellCount <= index)
      return;
    bool flag1;
    if (this.showMouseData)
    {
      ImGui.Indent();
      ImGui.Text("CellPos: " + x.ToString() + ", " + y.ToString());
      int num1 = (y + 1) * (Grid.WidthInCells + 2) + (x + 1);
      if (ImGui.InputInt("Sim Cell:", ref num1))
      {
        x = Mathf.Max(0, num1 % (Grid.WidthInCells + 2) - 1);
        y = Mathf.Max(0, num1 / (Grid.WidthInCells + 2) - 1);
        this.worldPos = Grid.CellToPosCCC(Grid.XYToCell(x, y), Grid.SceneLayer.Front);
      }
      if (ImGui.InputInt("Game Cell:", ref index))
      {
        x = index % Grid.WidthInCells;
        y = index / Grid.WidthInCells;
        this.worldPos = Grid.CellToPosCCC(Grid.XYToCell(x, y), Grid.SceneLayer.Front);
      }
      int num2 = Grid.WidthInCells / 32;
      int num3 = x / 32;
      int num4 = y / 32;
      int num5 = num4 * num2 + num3;
      ImGui.Text(string.Format("Chunk Idx ({0}, {1}): {2}", (object) num3, (object) num4, (object) num5));
      bool flag2 = Grid.RenderedByWorld[index];
      ImGui.Text("RenderedByWorld: " + flag2.ToString());
      flag2 = Grid.Solid[index];
      ImGui.Text("Solid: " + flag2.ToString());
      ImGui.Text("Damage: " + Grid.Damage[index].ToString());
      ImGui.Text("Foundation: " + Grid.Foundation[index].ToString());
      ImGui.Text("Revealed: " + Grid.Revealed[index].ToString());
      ImGui.Text("Visible: " + Grid.Visible[index].ToString());
      ImGui.Text("DupePassable: " + Grid.DupePassable[index].ToString());
      ImGui.Text("DupeImpassable: " + Grid.DupeImpassable[index].ToString());
      ImGui.Text("CritterImpassable: " + Grid.CritterImpassable[index].ToString());
      flag1 = Grid.FakeFloor[index];
      ImGui.Text("FakeFloor: " + flag1.ToString());
      flag1 = Grid.HasDoor[index];
      ImGui.Text("HasDoor: " + flag1.ToString());
      flag1 = Grid.HasLadder[index];
      ImGui.Text("HasLadder: " + flag1.ToString());
      flag1 = Grid.HasPole[index];
      ImGui.Text("HasPole: " + flag1.ToString());
      flag1 = Grid.HasNavTeleporter[index];
      ImGui.Text("HasNavTeleporter: " + flag1.ToString());
      flag1 = Grid.IsTileUnderConstruction[index];
      ImGui.Text("IsTileUnderConstruction: " + flag1.ToString());
      UtilityConnections connections = Game.Instance.liquidConduitSystem.GetConnections(index, false);
      ImGui.Text("LiquidVisPlacers: " + connections.ToString());
      connections = Game.Instance.liquidConduitSystem.GetConnections(index, true);
      ImGui.Text("LiquidPhysPlacers: " + connections.ToString());
      connections = Game.Instance.gasConduitSystem.GetConnections(index, false);
      ImGui.Text("GasVisPlacers: " + connections.ToString());
      connections = Game.Instance.gasConduitSystem.GetConnections(index, true);
      ImGui.Text("GasPhysPlacers: " + connections.ToString());
      connections = Game.Instance.electricalConduitSystem.GetConnections(index, false);
      ImGui.Text("ElecVisPlacers: " + connections.ToString());
      connections = Game.Instance.electricalConduitSystem.GetConnections(index, true);
      ImGui.Text("ElecPhysPlacers: " + connections.ToString());
      ImGui.Text("World Idx: " + Grid.WorldIdx[index].ToString());
      ImGui.Text("ZoneType: " + World.Instance.zoneRenderData.GetSubWorldZoneType(index).ToString());
      ImGui.Text("Light Intensity: " + Grid.LightIntensity[index].ToString());
      ImGui.Text("Radiation: " + Grid.Radiation[index].ToString());
      this.showAccessRestrictions = ImGui.CollapsingHeader("Access Restrictions");
      if (this.showAccessRestrictions)
      {
        ImGui.Indent();
        Grid.Restriction restriction;
        if (!Grid.DEBUG_GetRestrictions(index, out restriction))
        {
          ImGui.Text("No access control.");
        }
        else
        {
          ImGui.Text("Orientation: " + restriction.orientation.ToString());
          ImGui.Text("Default Restriction: " + restriction.DirectionMasksForMinionInstanceID[-1].ToString());
          ImGui.Indent();
          foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.Items)
          {
            int instanceId = ((Component) ((Component) minionIdentity).GetComponent<MinionIdentity>().assignableProxy.Get()).GetComponent<KPrefabID>().InstanceID;
            Grid.Restriction.Directions directions;
            if (restriction.DirectionMasksForMinionInstanceID.TryGetValue(instanceId, out directions))
              ImGui.Text(((Object) minionIdentity).name + " Restriction: " + directions.ToString());
            else
              ImGui.Text(((Object) minionIdentity).name + ": Has No restriction");
          }
          ImGui.Unindent();
        }
        ImGui.Unindent();
      }
      this.showGridContents = ImGui.CollapsingHeader("Grid Objects");
      if (this.showGridContents)
      {
        ImGui.Indent();
        for (int layer = 0; layer < 44; ++layer)
        {
          GameObject gameObject = Grid.Objects[index, layer];
          ImGui.Text(Enum.GetName(typeof (ObjectLayer), (object) layer) + ": " + (Object.op_Inequality((Object) gameObject, (Object) null) ? ((Object) gameObject).name : "None"));
        }
        ImGui.Unindent();
      }
      this.showScenePartitionerContents = ImGui.CollapsingHeader("Scene Partitioner");
      if (this.showScenePartitionerContents)
      {
        ImGui.Indent();
        if (Object.op_Inequality((Object) GameScenePartitioner.Instance, (Object) null))
        {
          this.showLayerToggles = ImGui.CollapsingHeader("Layers");
          if (this.showLayerToggles)
          {
            bool flag3 = false;
            foreach (ScenePartitionerLayer layer in GameScenePartitioner.Instance.GetLayers())
            {
              bool flag4 = this.toggledLayers.Contains(layer);
              bool flag5 = flag4;
              ImGui.Checkbox(HashCache.Get().Get(layer.name), ref flag5);
              if (flag5 != flag4)
              {
                flag3 = true;
                if (flag5)
                  this.toggledLayers.Add(layer);
                else
                  this.toggledLayers.Remove(layer);
              }
            }
            if (flag3)
            {
              GameScenePartitioner.Instance.SetToggledLayers(this.toggledLayers);
              if (this.toggledLayers.Count > 0)
                SimDebugView.Instance.SetMode(SimDebugView.OverlayModes.ScenePartitioner);
            }
          }
          ListPool<ScenePartitionerEntry, ScenePartitioner>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, ScenePartitioner>.Allocate();
          foreach (ScenePartitionerLayer layer in GameScenePartitioner.Instance.GetLayers())
          {
            ((List<ScenePartitionerEntry>) gathered_entries).Clear();
            GameScenePartitioner.Instance.GatherEntries(x, y, 1, 1, layer, (List<ScenePartitionerEntry>) gathered_entries);
            foreach (ScenePartitionerEntry partitionerEntry in (List<ScenePartitionerEntry>) gathered_entries)
            {
              GameObject gameObject = partitionerEntry.obj as GameObject;
              MonoBehaviour monoBehaviour = partitionerEntry.obj as MonoBehaviour;
              if (Object.op_Inequality((Object) gameObject, (Object) null))
                ImGui.Text(((Object) gameObject).name);
              else if (Object.op_Inequality((Object) monoBehaviour, (Object) null))
                ImGui.Text(((Object) monoBehaviour).name);
            }
          }
          gathered_entries.Recycle();
        }
        ImGui.Unindent();
      }
      this.showCavityInfo = ImGui.CollapsingHeader("Cavity Info");
      if (this.showCavityInfo)
      {
        ImGui.Indent();
        CavityInfo cavityInfo = (CavityInfo) null;
        if (Object.op_Inequality((Object) Game.Instance, (Object) null) && Game.Instance.roomProber != null)
          cavityInfo = Game.Instance.roomProber.GetCavityForCell(index);
        if (cavityInfo != null)
        {
          ImGui.Text("Cell Count: " + cavityInfo.numCells.ToString());
          Room room = cavityInfo.room;
          if (room != null)
          {
            ImGui.Text("Is Room: True");
            this.showBuildings = ImGui.CollapsingHeader("Buildings (" + room.buildings.Count.ToString() + ")");
            if (this.showBuildings)
            {
              foreach (object building in room.buildings)
                ImGui.Text(building.ToString());
            }
          }
          else
            ImGui.Text("Is Room: False");
        }
        else
          ImGui.Text("No Cavity Detected");
        ImGui.Unindent();
      }
      this.showPropertyInfo = ImGui.CollapsingHeader("Property Info");
      if (this.showPropertyInfo)
      {
        ImGui.Indent();
        bool flag6 = true;
        byte property = Grid.Properties[index];
        foreach (object obj in Enum.GetValues(typeof (Sim.Cell.Properties)))
        {
          if (((int) property & (int) obj) != 0)
          {
            ImGui.Text(obj.ToString());
            flag6 = false;
          }
        }
        if (flag6)
          ImGui.Text("No properties");
        ImGui.Unindent();
      }
      ImGui.Unindent();
    }
    float num;
    if (Grid.ObjectLayers != null)
    {
      Element element = Grid.Element[index];
      this.showElementData = ImGui.CollapsingHeader("Element");
      ImGui.SameLine();
      ImGui.Text("[" + element.name + "]");
      ImGui.Text("Mass:" + Grid.Mass[index].ToString());
      if (this.showElementData)
        this.DrawElem(element);
      num = Grid.AccumulatedFlow[index] / 3f;
      ImGui.Text("Average Flow Rate (kg/s):" + num.ToString());
    }
    this.showPhysicsData = ImGui.CollapsingHeader("Physics Data");
    if (this.showPhysicsData)
    {
      ImGui.Indent();
      flag1 = Grid.Solid[index];
      ImGui.Text("Solid: " + flag1.ToString());
      num = Grid.Pressure[index];
      ImGui.Text("Pressure: " + num.ToString());
      num = Grid.Temperature[index];
      ImGui.Text("Temperature (kelvin -272.15): " + num.ToString());
      num = Grid.Radiation[index];
      ImGui.Text("Radiation: " + num.ToString());
      num = Grid.Mass[index];
      ImGui.Text("Mass: " + num.ToString());
      num = (float) Grid.Insulation[index] / (float) byte.MaxValue;
      ImGui.Text("Insulation: " + num.ToString());
      ImGui.Text("Strength Multiplier: " + Grid.StrengthInfo[index].ToString());
      ImGui.Text("Properties: 0x: " + Grid.Properties[index].ToString("X"));
      ImGui.Text("Disease: " + (Grid.DiseaseIdx[index] == byte.MaxValue ? "None" : Db.Get().Diseases[(int) Grid.DiseaseIdx[index]].Name));
      ImGui.Text("Disease Count: " + Grid.DiseaseCount[index].ToString());
      ImGui.Unindent();
    }
    this.showGasConduitData = ImGui.CollapsingHeader("Gas Conduit Data");
    if (this.showGasConduitData)
      this.DrawConduitFlow(Game.Instance.gasConduitFlow, index);
    this.showLiquidConduitData = ImGui.CollapsingHeader("Liquid Conduit Data");
    if (!this.showLiquidConduitData)
      return;
    this.DrawConduitFlow(Game.Instance.liquidConduitFlow, index);
  }

  private void DrawElem(Element element)
  {
    ImGui.Indent();
    ImGui.Text("State: " + element.state.ToString());
    ImGui.Text("Thermal Conductivity: " + element.thermalConductivity.ToString());
    ImGui.Text("Specific Heat Capacity: " + element.specificHeatCapacity.ToString());
    if (element.lowTempTransition != null)
    {
      ImGui.Text("Low Temperature: " + element.lowTemp.ToString());
      ImGui.Text("Low Temperature Transition: " + element.lowTempTransitionTarget.ToString());
    }
    if (element.highTempTransition != null)
    {
      ImGui.Text("High Temperature: " + element.highTemp.ToString());
      ImGui.Text("HighTemp Temperature Transition: " + element.highTempTransitionTarget.ToString());
    }
    ImGui.Text("Light Absorption Factor: " + element.lightAbsorptionFactor.ToString());
    ImGui.Text("Radiation Absorption Factor: " + element.radiationAbsorptionFactor.ToString());
    ImGui.Text("Radiation Per 1000 Mass: " + element.radiationPer1000Mass.ToString());
    ImGui.Text("Sublimate ID: " + element.sublimateId.ToString());
    ImGui.Text("Sublimate FX: " + element.sublimateFX.ToString());
    ImGui.Text("Sublimate Rate: " + element.sublimateRate.ToString());
    ImGui.Text("Sublimate Efficiency: " + element.sublimateEfficiency.ToString());
    ImGui.Text("Sublimate Probability: " + element.sublimateProbability.ToString());
    ImGui.Text("Off Gas Percentage: " + element.offGasPercentage.ToString());
    if (element.IsGas)
      ImGui.Text("Default Pressure: " + element.defaultValues.pressure.ToString());
    else
      ImGui.Text("Default Mass: " + element.defaultValues.mass.ToString());
    ImGui.Text("Default Temperature: " + element.defaultValues.temperature.ToString());
    if (element.IsGas)
      ImGui.Text("Flow: " + element.flow.ToString());
    if (element.IsLiquid)
    {
      ImGui.Text("Max Comp: " + element.maxCompression.ToString());
      ImGui.Text("Max Mass: " + element.maxMass.ToString());
    }
    if (element.IsSolid)
    {
      ImGui.Text("Hardness: " + element.hardness.ToString());
      ImGui.Text("Unstable: " + element.IsUnstable.ToString());
    }
    ImGui.Unindent();
  }

  private void DrawConduitFlow(ConduitFlow flow_mgr, int cell)
  {
    ImGui.Indent();
    ConduitFlow.ConduitContents contents = flow_mgr.GetContents(cell);
    ImGui.Text("Element: " + contents.element.ToString());
    ImGui.Text(string.Format("Mass: {0}", (object) contents.mass));
    ImGui.Text(string.Format("Movable Mass: {0}", (object) contents.movable_mass));
    ImGui.Text("Temperature: " + contents.temperature.ToString());
    ImGui.Text("Disease: " + (contents.diseaseIdx == byte.MaxValue ? "None" : Db.Get().Diseases[(int) contents.diseaseIdx].Name));
    ImGui.Text("Disease Count: " + contents.diseaseCount.ToString());
    ImGui.Text(string.Format("Update Order: {0}", (object) flow_mgr.ComputeUpdateOrder(cell)));
    flow_mgr.SetContents(cell, contents);
    ConduitFlow.FlowDirections permittedFlow = flow_mgr.GetPermittedFlow(cell);
    if (permittedFlow == ConduitFlow.FlowDirections.None)
    {
      ImGui.Text("PermittedFlow: None");
    }
    else
    {
      string str = "";
      if ((permittedFlow & ConduitFlow.FlowDirections.Up) != ConduitFlow.FlowDirections.None)
        str += " Up ";
      if ((permittedFlow & ConduitFlow.FlowDirections.Down) != ConduitFlow.FlowDirections.None)
        str += " Down ";
      if ((permittedFlow & ConduitFlow.FlowDirections.Left) != ConduitFlow.FlowDirections.None)
        str += " Left ";
      if ((permittedFlow & ConduitFlow.FlowDirections.Right) != ConduitFlow.FlowDirections.None)
        str += " Right ";
      ImGui.Text("PermittedFlow: " + str);
    }
    ImGui.Unindent();
  }

  public void SetCell(int cell) => this.worldPos = Grid.CellToPosCCC(cell, Grid.SceneLayer.Move);
}
