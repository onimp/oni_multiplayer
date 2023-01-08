// Decompiled with JetBrains decompiler
// Type: Building
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Building")]
public class Building : 
  KMonoBehaviour,
  IGameObjectEffectDescriptor,
  IUniformGridObject,
  IApproachable
{
  public BuildingDef Def;
  [MyCmpGet]
  private Rotatable rotatable;
  [MyCmpAdd]
  private StateMachineController stateMachineController;
  private int[] placementCells;
  private Extents extents;
  private static StatusItem deprecatedBuildingStatusItem;
  private string description;
  private HandleVector<int>.Handle scenePartitionerEntry;

  public Orientation Orientation => !Object.op_Inequality((Object) this.rotatable, (Object) null) ? Orientation.Neutral : this.rotatable.GetOrientation();

  public int[] PlacementCells
  {
    get
    {
      if (this.placementCells == null)
        this.RefreshCells();
      return this.placementCells;
    }
  }

  public Extents GetExtents()
  {
    if (this.extents.width == 0 || this.extents.height == 0)
      this.RefreshCells();
    return this.extents;
  }

  public Extents GetValidPlacementExtents()
  {
    Extents extents = this.GetExtents();
    --extents.x;
    --extents.y;
    extents.width += 2;
    extents.height += 2;
    return extents;
  }

  public void RefreshCells()
  {
    this.placementCells = new int[this.Def.PlacementOffsets.Length];
    int cell = Grid.PosToCell((KMonoBehaviour) this);
    if (cell < 0)
    {
      this.extents.x = -1;
      this.extents.y = -1;
      this.extents.width = this.Def.WidthInCells;
      this.extents.height = this.Def.HeightInCells;
    }
    else
    {
      Orientation orientation = this.Orientation;
      for (int index = 0; index < this.Def.PlacementOffsets.Length; ++index)
      {
        CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(this.Def.PlacementOffsets[index], orientation);
        int num = Grid.OffsetCell(cell, rotatedCellOffset);
        this.placementCells[index] = num;
      }
      int x = 0;
      int y = 0;
      Grid.CellToXY(this.placementCells[0], out x, out y);
      int val1_1 = x;
      int val1_2 = y;
      foreach (int placementCell in this.placementCells)
      {
        int val2_1 = 0;
        int val2_2 = 0;
        ref int local1 = ref val2_1;
        ref int local2 = ref val2_2;
        Grid.CellToXY(placementCell, out local1, out local2);
        x = Math.Min(x, val2_1);
        y = Math.Min(y, val2_2);
        val1_1 = Math.Max(val1_1, val2_1);
        val1_2 = Math.Max(val1_2, val2_2);
      }
      this.extents.x = x;
      this.extents.y = y;
      this.extents.width = val1_1 - x + 1;
      this.extents.height = val1_2 - y + 1;
    }
  }

  [System.Runtime.Serialization.OnDeserialized]
  internal void OnDeserialized()
  {
    PrimaryElement component = ((Component) this).GetComponent<PrimaryElement>();
    if (!Object.op_Inequality((Object) component, (Object) null) || (double) component.Temperature != 0.0)
      return;
    if (component.Element == null)
    {
      DeserializeWarnings.Instance.PrimaryElementHasNoElement.Warn(((Object) this).name + " primary element has no element.", ((Component) this).gameObject);
    }
    else
    {
      if (this is BuildingUnderConstruction)
        return;
      DeserializeWarnings.Instance.BuildingTemeperatureIsZeroKelvin.Warn(((Object) this).name + " is at zero degrees kelvin. Resetting temperature.");
      component.Temperature = component.Element.defaultValues.temperature;
    }
  }

  public void SetDescription(string desc) => this.description = desc;

  public string Desc => this.Def.AvailableFacades.Count > 0 && !Util.IsNullOrWhiteSpace(this.description) ? this.description : this.Def.Desc;

  protected virtual void OnSpawn()
  {
    if (Object.op_Equality((Object) this.Def, (Object) null))
      Debug.LogError((object) ("Missing building definition on object " + ((Object) this).name));
    KSelectable component1 = ((Component) this).GetComponent<KSelectable>();
    if (Object.op_Inequality((Object) component1, (Object) null))
    {
      component1.SetName(this.Def.Name);
      component1.SetStatusIndicatorOffset(new Vector3(0.0f, -0.35f, 0.0f));
    }
    Prioritizable component2 = ((Component) this).GetComponent<Prioritizable>();
    if (Object.op_Inequality((Object) component2, (Object) null))
      component2.iconOffset.y = 0.3f;
    if (((Component) this).GetComponent<KPrefabID>().HasTag(RoomConstraints.ConstraintTags.IndustrialMachinery))
      this.scenePartitionerEntry = GameScenePartitioner.Instance.Add(((Object) this).name, (object) ((Component) this).gameObject, this.GetExtents(), GameScenePartitioner.Instance.industrialBuildings, (Action<object>) null);
    if (!this.Def.Deprecated || !Object.op_Inequality((Object) ((Component) this).GetComponent<KSelectable>(), (Object) null))
      return;
    KSelectable component3 = ((Component) this).GetComponent<KSelectable>();
    Building.deprecatedBuildingStatusItem = new StatusItem("BUILDING_DEPRECATED", "BUILDING", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID);
    StatusItem buildingStatusItem = Building.deprecatedBuildingStatusItem;
    component3.AddStatusItem(buildingStatusItem);
  }

  protected virtual void OnCleanUp()
  {
    GameScenePartitioner.Instance.Free(ref this.scenePartitionerEntry);
    base.OnCleanUp();
  }

  public virtual void UpdatePosition()
  {
    this.RefreshCells();
    GameScenePartitioner.Instance.UpdatePosition(this.scenePartitionerEntry, this.GetExtents());
  }

  protected void RegisterBlockTileRenderer()
  {
    if (!Object.op_Inequality((Object) this.Def.BlockTileAtlas, (Object) null))
      return;
    PrimaryElement component1 = ((Component) this).GetComponent<PrimaryElement>();
    if (!Object.op_Inequality((Object) component1, (Object) null))
      return;
    SimHashes visualizationElementId = this.GetVisualizationElementID(component1);
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
    Constructable component2 = ((Component) this).GetComponent<Constructable>();
    World.Instance.blockTileRenderer.AddBlock(((Component) this).gameObject.layer, this.Def, Object.op_Inequality((Object) component2, (Object) null) && component2.IsReplacementTile, visualizationElementId, cell);
  }

  public CellOffset GetRotatedOffset(CellOffset offset) => !Object.op_Inequality((Object) this.rotatable, (Object) null) ? offset : this.rotatable.GetRotatedCellOffset(offset);

  private int GetBottomLeftCell() => Grid.PosToCell(TransformExtensions.GetPosition(this.transform));

  public int GetPowerInputCell()
  {
    CellOffset rotatedOffset = this.GetRotatedOffset(this.Def.PowerInputOffset);
    return Grid.OffsetCell(this.GetBottomLeftCell(), rotatedOffset);
  }

  public int GetPowerOutputCell()
  {
    CellOffset rotatedOffset = this.GetRotatedOffset(this.Def.PowerOutputOffset);
    return Grid.OffsetCell(this.GetBottomLeftCell(), rotatedOffset);
  }

  public int GetUtilityInputCell()
  {
    CellOffset rotatedOffset = this.GetRotatedOffset(this.Def.UtilityInputOffset);
    return Grid.OffsetCell(this.GetBottomLeftCell(), rotatedOffset);
  }

  public int GetHighEnergyParticleInputCell()
  {
    CellOffset rotatedOffset = this.GetRotatedOffset(this.Def.HighEnergyParticleInputOffset);
    return Grid.OffsetCell(this.GetBottomLeftCell(), rotatedOffset);
  }

  public int GetHighEnergyParticleOutputCell()
  {
    CellOffset rotatedOffset = this.GetRotatedOffset(this.Def.HighEnergyParticleOutputOffset);
    return Grid.OffsetCell(this.GetBottomLeftCell(), rotatedOffset);
  }

  public int GetUtilityOutputCell()
  {
    CellOffset rotatedOffset = this.GetRotatedOffset(this.Def.UtilityOutputOffset);
    return Grid.OffsetCell(this.GetBottomLeftCell(), rotatedOffset);
  }

  public CellOffset GetUtilityInputOffset() => this.GetRotatedOffset(this.Def.UtilityInputOffset);

  public CellOffset GetUtilityOutputOffset() => this.GetRotatedOffset(this.Def.UtilityOutputOffset);

  public CellOffset GetHighEnergyParticleInputOffset() => this.GetRotatedOffset(this.Def.HighEnergyParticleInputOffset);

  public CellOffset GetHighEnergyParticleOutputOffset() => this.GetRotatedOffset(this.Def.HighEnergyParticleOutputOffset);

  protected void UnregisterBlockTileRenderer()
  {
    if (!Object.op_Inequality((Object) this.Def.BlockTileAtlas, (Object) null))
      return;
    PrimaryElement component1 = ((Component) this).GetComponent<PrimaryElement>();
    if (!Object.op_Inequality((Object) component1, (Object) null))
      return;
    SimHashes visualizationElementId = this.GetVisualizationElementID(component1);
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
    Constructable component2 = ((Component) this).GetComponent<Constructable>();
    World.Instance.blockTileRenderer.RemoveBlock(this.Def, Object.op_Inequality((Object) component2, (Object) null) && component2.IsReplacementTile, visualizationElementId, cell);
  }

  private SimHashes GetVisualizationElementID(PrimaryElement pe) => !(this is BuildingComplete) ? SimHashes.Void : pe.ElementID;

  public void RunOnArea(Action<int> callback) => this.Def.RunOnArea(Grid.PosToCell((KMonoBehaviour) this), this.Orientation, callback);

  public List<Descriptor> RequirementDescriptors(BuildingDef def)
  {
    List<Descriptor> descriptorList = new List<Descriptor>();
    BuildingComplete component1 = def.BuildingComplete.GetComponent<BuildingComplete>();
    if (def.RequiresPowerInput)
    {
      float neededWhenActive = ((Component) component1).GetComponent<IEnergyConsumer>().WattsNeededWhenActive;
      if ((double) neededWhenActive > 0.0)
      {
        string formattedWattage = GameUtil.GetFormattedWattage(neededWhenActive);
        Descriptor descriptor;
        // ISSUE: explicit constructor call
        ((Descriptor) ref descriptor).\u002Ector(string.Format((string) UI.BUILDINGEFFECTS.REQUIRESPOWER, (object) formattedWattage), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESPOWER, (object) formattedWattage), (Descriptor.DescriptorType) 0, false);
        descriptorList.Add(descriptor);
      }
    }
    if (def.InputConduitType == ConduitType.Liquid)
    {
      Descriptor descriptor = new Descriptor();
      ((Descriptor) ref descriptor).SetupDescriptor((string) UI.BUILDINGEFFECTS.REQUIRESLIQUIDINPUT, (string) UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESLIQUIDINPUT, (Descriptor.DescriptorType) 0);
      descriptorList.Add(descriptor);
    }
    else if (def.InputConduitType == ConduitType.Gas)
    {
      Descriptor descriptor = new Descriptor();
      ((Descriptor) ref descriptor).SetupDescriptor((string) UI.BUILDINGEFFECTS.REQUIRESGASINPUT, (string) UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESGASINPUT, (Descriptor.DescriptorType) 0);
      descriptorList.Add(descriptor);
    }
    if (def.OutputConduitType == ConduitType.Liquid)
    {
      Descriptor descriptor = new Descriptor();
      ((Descriptor) ref descriptor).SetupDescriptor((string) UI.BUILDINGEFFECTS.REQUIRESLIQUIDOUTPUT, (string) UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESLIQUIDOUTPUT, (Descriptor.DescriptorType) 0);
      descriptorList.Add(descriptor);
    }
    else if (def.OutputConduitType == ConduitType.Gas)
    {
      Descriptor descriptor = new Descriptor();
      ((Descriptor) ref descriptor).SetupDescriptor((string) UI.BUILDINGEFFECTS.REQUIRESGASOUTPUT, (string) UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESGASOUTPUT, (Descriptor.DescriptorType) 0);
      descriptorList.Add(descriptor);
    }
    if (component1.isManuallyOperated)
    {
      Descriptor descriptor = new Descriptor();
      ((Descriptor) ref descriptor).SetupDescriptor((string) UI.BUILDINGEFFECTS.REQUIRESMANUALOPERATION, (string) UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESMANUALOPERATION, (Descriptor.DescriptorType) 0);
      descriptorList.Add(descriptor);
    }
    if (component1.isArtable)
    {
      Descriptor descriptor = new Descriptor();
      ((Descriptor) ref descriptor).SetupDescriptor((string) UI.BUILDINGEFFECTS.REQUIRESCREATIVITY, (string) UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESCREATIVITY, (Descriptor.DescriptorType) 0);
      descriptorList.Add(descriptor);
    }
    if (Object.op_Inequality((Object) def.BuildingUnderConstruction, (Object) null))
    {
      Constructable component2 = def.BuildingUnderConstruction.GetComponent<Constructable>();
      if (Object.op_Inequality((Object) component2, (Object) null) && HashedString.op_Inequality(HashedString.op_Implicit(component2.requiredSkillPerk), HashedString.Invalid))
      {
        StringBuilder stringBuilder = new StringBuilder();
        List<Skill> skillsWithPerk = Db.Get().Skills.GetSkillsWithPerk(component2.requiredSkillPerk);
        for (int index = 0; index < skillsWithPerk.Count; ++index)
        {
          Skill skill = skillsWithPerk[index];
          stringBuilder.Append(skill.Name);
          if (index != skillsWithPerk.Count - 1)
            stringBuilder.Append(", ");
        }
        string replacement = stringBuilder.ToString();
        descriptorList.Add(new Descriptor(UI.BUILD_REQUIRES_SKILL.Replace("{Skill}", replacement), UI.BUILD_REQUIRES_SKILL_TOOLTIP.Replace("{Skill}", replacement), (Descriptor.DescriptorType) 0, false));
      }
    }
    return descriptorList;
  }

  public List<Descriptor> EffectDescriptors(BuildingDef def)
  {
    List<Descriptor> descriptorList = new List<Descriptor>();
    if (def.EffectDescription != null)
      descriptorList.AddRange((IEnumerable<Descriptor>) def.EffectDescription);
    if ((double) def.GeneratorWattageRating > 0.0 && Object.op_Equality((Object) ((Component) this).GetComponent<Battery>(), (Object) null))
    {
      Descriptor descriptor = new Descriptor();
      ((Descriptor) ref descriptor).SetupDescriptor(string.Format((string) UI.BUILDINGEFFECTS.ENERGYGENERATED, (object) GameUtil.GetFormattedWattage(def.GeneratorWattageRating)), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.ENERGYGENERATED, (object) GameUtil.GetFormattedWattage(def.GeneratorWattageRating)), (Descriptor.DescriptorType) 1);
      descriptorList.Add(descriptor);
    }
    if ((double) def.ExhaustKilowattsWhenActive > 0.0 || (double) def.SelfHeatKilowattsWhenActive > 0.0)
    {
      Descriptor descriptor = new Descriptor();
      string formattedHeatEnergy = GameUtil.GetFormattedHeatEnergy((float) (((double) def.ExhaustKilowattsWhenActive + (double) def.SelfHeatKilowattsWhenActive) * 1000.0));
      ((Descriptor) ref descriptor).SetupDescriptor(string.Format((string) UI.BUILDINGEFFECTS.HEATGENERATED, (object) formattedHeatEnergy), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.HEATGENERATED, (object) formattedHeatEnergy), (Descriptor.DescriptorType) 1);
      descriptorList.Add(descriptor);
    }
    return descriptorList;
  }

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    foreach (Descriptor requirementDescriptor in this.RequirementDescriptors(this.Def))
      descriptors.Add(requirementDescriptor);
    foreach (Descriptor effectDescriptor in this.EffectDescriptors(this.Def))
      descriptors.Add(effectDescriptor);
    return descriptors;
  }

  public virtual Vector2 PosMin()
  {
    Extents extents = this.GetExtents();
    return new Vector2((float) extents.x, (float) extents.y);
  }

  public virtual Vector2 PosMax()
  {
    Extents extents = this.GetExtents();
    return new Vector2((float) (extents.x + extents.width), (float) (extents.y + extents.height));
  }

  public CellOffset[] GetOffsets() => OffsetGroups.Use;

  public int GetCell() => Grid.PosToCell((KMonoBehaviour) this);
}
