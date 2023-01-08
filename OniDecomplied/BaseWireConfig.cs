// Decompiled with JetBrains decompiler
// Type: BaseWireConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public abstract class BaseWireConfig : IBuildingConfig
{
  public abstract override BuildingDef CreateBuildingDef();

  public BuildingDef CreateBuildingDef(
    string id,
    string anim,
    float construction_time,
    float[] construction_mass,
    float insulation,
    EffectorValues decor,
    EffectorValues noise)
  {
    string id1 = id;
    string anim1 = anim;
    double construction_time1 = (double) construction_time;
    float[] construction_mass1 = construction_mass;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues effectorValues = noise;
    EffectorValues decor1 = decor;
    EffectorValues noise1 = effectorValues;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id1, 1, 1, anim1, 10, (float) construction_time1, construction_mass1, allMetals, 1600f, BuildLocationRule.Anywhere, decor1, noise1);
    buildingDef.ThermalConductivity = insulation;
    buildingDef.Floodable = false;
    buildingDef.Overheatable = false;
    buildingDef.Entombable = false;
    buildingDef.ViewMode = OverlayModes.Power.ID;
    buildingDef.ObjectLayer = ObjectLayer.Wire;
    buildingDef.TileLayer = ObjectLayer.WireTile;
    buildingDef.ReplacementLayer = ObjectLayer.ReplacementWire;
    buildingDef.AudioCategory = "Metal";
    buildingDef.AudioSize = "small";
    buildingDef.BaseTimeUntilRepair = -1f;
    buildingDef.SceneLayer = Grid.SceneLayer.Wires;
    buildingDef.isKAnimTile = true;
    buildingDef.isUtility = true;
    buildingDef.DragBuild = true;
    GeneratedBuildings.RegisterWithOverlay(OverlayScreen.WireIDs, id);
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    GeneratedBuildings.MakeBuildingAlwaysOperational(go);
    BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof (RequiresFoundation), prefab_tag);
    go.AddOrGet<Wire>();
    KAnimGraphTileVisualizer graphTileVisualizer = go.AddOrGet<KAnimGraphTileVisualizer>();
    graphTileVisualizer.isPhysicalBuilding = true;
    graphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Electrical;
  }

  public override void DoPostConfigureUnderConstruction(GameObject go)
  {
    base.DoPostConfigureUnderConstruction(go);
    go.GetComponent<Constructable>().isDiggingRequired = false;
    KAnimGraphTileVisualizer graphTileVisualizer = go.AddOrGet<KAnimGraphTileVisualizer>();
    graphTileVisualizer.isPhysicalBuilding = false;
    graphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Electrical;
  }

  protected void DoPostConfigureComplete(Wire.WattageRating rating, GameObject go)
  {
    go.GetComponent<Wire>().MaxWattageRating = rating;
    float maxWattageAsFloat = Wire.GetMaxWattageAsFloat(rating);
    Descriptor descriptor = new Descriptor();
    ((Descriptor) ref descriptor).SetupDescriptor(string.Format((string) UI.BUILDINGEFFECTS.MAX_WATTAGE, (object) GameUtil.GetFormattedWattage(maxWattageAsFloat)), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.MAX_WATTAGE), (Descriptor.DescriptorType) 1);
    BuildingDef def = go.GetComponent<Building>().Def;
    if (def.EffectDescription == null)
      def.EffectDescription = new List<Descriptor>();
    def.EffectDescription.Add(descriptor);
  }
}
