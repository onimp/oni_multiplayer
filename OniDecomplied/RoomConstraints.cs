// Decompiled with JetBrains decompiler
// Type: RoomConstraints
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class RoomConstraints
{
  public static RoomConstraints.Constraint CEILING_HEIGHT_4 = new RoomConstraints.Constraint((Func<KPrefabID, bool>) null, (Func<Room, bool>) (room => 1 + room.cavity.maxY - room.cavity.minY >= 4), name: string.Format((string) ROOMS.CRITERIA.CEILING_HEIGHT.NAME, (object) "4"), description: string.Format((string) ROOMS.CRITERIA.CEILING_HEIGHT.DESCRIPTION, (object) "4"));
  public static RoomConstraints.Constraint CEILING_HEIGHT_6 = new RoomConstraints.Constraint((Func<KPrefabID, bool>) null, (Func<Room, bool>) (room => 1 + room.cavity.maxY - room.cavity.minY >= 6), name: string.Format((string) ROOMS.CRITERIA.CEILING_HEIGHT.NAME, (object) "6"), description: string.Format((string) ROOMS.CRITERIA.CEILING_HEIGHT.DESCRIPTION, (object) "6"));
  public static RoomConstraints.Constraint MINIMUM_SIZE_12 = new RoomConstraints.Constraint((Func<KPrefabID, bool>) null, (Func<Room, bool>) (room => room.cavity.numCells >= 12), name: string.Format((string) ROOMS.CRITERIA.MINIMUM_SIZE.NAME, (object) "12"), description: string.Format((string) ROOMS.CRITERIA.MINIMUM_SIZE.DESCRIPTION, (object) "12"));
  public static RoomConstraints.Constraint MINIMUM_SIZE_24 = new RoomConstraints.Constraint((Func<KPrefabID, bool>) null, (Func<Room, bool>) (room => room.cavity.numCells >= 24), name: string.Format((string) ROOMS.CRITERIA.MINIMUM_SIZE.NAME, (object) "24"), description: string.Format((string) ROOMS.CRITERIA.MINIMUM_SIZE.DESCRIPTION, (object) "24"));
  public static RoomConstraints.Constraint MINIMUM_SIZE_32 = new RoomConstraints.Constraint((Func<KPrefabID, bool>) null, (Func<Room, bool>) (room => room.cavity.numCells >= 32), name: string.Format((string) ROOMS.CRITERIA.MINIMUM_SIZE.NAME, (object) "32"), description: string.Format((string) ROOMS.CRITERIA.MINIMUM_SIZE.DESCRIPTION, (object) "32"));
  public static RoomConstraints.Constraint MAXIMUM_SIZE_64 = new RoomConstraints.Constraint((Func<KPrefabID, bool>) null, (Func<Room, bool>) (room => room.cavity.numCells <= 64), name: string.Format((string) ROOMS.CRITERIA.MAXIMUM_SIZE.NAME, (object) "64"), description: string.Format((string) ROOMS.CRITERIA.MAXIMUM_SIZE.DESCRIPTION, (object) "64"));
  public static RoomConstraints.Constraint MAXIMUM_SIZE_96 = new RoomConstraints.Constraint((Func<KPrefabID, bool>) null, (Func<Room, bool>) (room => room.cavity.numCells <= 96), name: string.Format((string) ROOMS.CRITERIA.MAXIMUM_SIZE.NAME, (object) "96"), description: string.Format((string) ROOMS.CRITERIA.MAXIMUM_SIZE.DESCRIPTION, (object) "96"));
  public static RoomConstraints.Constraint MAXIMUM_SIZE_120 = new RoomConstraints.Constraint((Func<KPrefabID, bool>) null, (Func<Room, bool>) (room => room.cavity.numCells <= 120), name: string.Format((string) ROOMS.CRITERIA.MAXIMUM_SIZE.NAME, (object) "120"), description: string.Format((string) ROOMS.CRITERIA.MAXIMUM_SIZE.DESCRIPTION, (object) "120"));
  public static RoomConstraints.Constraint NO_INDUSTRIAL_MACHINERY = new RoomConstraints.Constraint((Func<KPrefabID, bool>) null, (Func<Room, bool>) (room =>
  {
    foreach (KPrefabID building in room.buildings)
    {
      if (building.HasTag(RoomConstraints.ConstraintTags.IndustrialMachinery))
        return false;
    }
    return true;
  }), name: ((string) ROOMS.CRITERIA.NO_INDUSTRIAL_MACHINERY.NAME), description: ((string) ROOMS.CRITERIA.NO_INDUSTRIAL_MACHINERY.DESCRIPTION));
  public static RoomConstraints.Constraint NO_COTS = new RoomConstraints.Constraint((Func<KPrefabID, bool>) null, (Func<Room, bool>) (room =>
  {
    foreach (KPrefabID building in room.buildings)
    {
      if (building.HasTag(RoomConstraints.ConstraintTags.BedType) && !building.HasTag(RoomConstraints.ConstraintTags.LuxuryBedType))
        return false;
    }
    return true;
  }), name: ((string) ROOMS.CRITERIA.NO_COTS.NAME), description: ((string) ROOMS.CRITERIA.NO_COTS.DESCRIPTION));
  public static RoomConstraints.Constraint NO_LUXURY_BEDS = new RoomConstraints.Constraint((Func<KPrefabID, bool>) null, (Func<Room, bool>) (room =>
  {
    foreach (KPrefabID building in room.buildings)
    {
      if (building.HasTag(RoomConstraints.ConstraintTags.LuxuryBedType))
        return false;
    }
    return true;
  }), name: ((string) ROOMS.CRITERIA.NO_COTS.NAME), description: ((string) ROOMS.CRITERIA.NO_COTS.DESCRIPTION));
  public static RoomConstraints.Constraint NO_OUTHOUSES = new RoomConstraints.Constraint((Func<KPrefabID, bool>) null, (Func<Room, bool>) (room =>
  {
    foreach (KPrefabID building in room.buildings)
    {
      if (building.HasTag(RoomConstraints.ConstraintTags.ToiletType) && !building.HasTag(RoomConstraints.ConstraintTags.FlushToiletType))
        return false;
    }
    return true;
  }), name: ((string) ROOMS.CRITERIA.NO_OUTHOUSES.NAME), description: ((string) ROOMS.CRITERIA.NO_OUTHOUSES.DESCRIPTION));
  public static RoomConstraints.Constraint NO_MESS_STATION = new RoomConstraints.Constraint((Func<KPrefabID, bool>) null, (Func<Room, bool>) (room =>
  {
    bool flag = false;
    for (int index = 0; !flag && index < room.buildings.Count; ++index)
      flag = room.buildings[index].HasTag(RoomConstraints.ConstraintTags.MessTable);
    return !flag;
  }), name: ((string) ROOMS.CRITERIA.NO_MESS_STATION.NAME), description: ((string) ROOMS.CRITERIA.NO_MESS_STATION.DESCRIPTION));
  public static RoomConstraints.Constraint HAS_LUXURY_BED = new RoomConstraints.Constraint((Func<KPrefabID, bool>) (bc => bc.HasTag(RoomConstraints.ConstraintTags.LuxuryBedType)), (Func<Room, bool>) null, name: ((string) ROOMS.CRITERIA.HAS_LUXURY_BED.NAME), description: ((string) ROOMS.CRITERIA.HAS_LUXURY_BED.DESCRIPTION));
  public static RoomConstraints.Constraint HAS_BED = new RoomConstraints.Constraint((Func<KPrefabID, bool>) (bc => bc.HasTag(RoomConstraints.ConstraintTags.BedType) && !bc.HasTag(RoomConstraints.ConstraintTags.Clinic)), (Func<Room, bool>) null, name: ((string) ROOMS.CRITERIA.HAS_BED.NAME), description: ((string) ROOMS.CRITERIA.HAS_BED.DESCRIPTION));
  public static RoomConstraints.Constraint SCIENCE_BUILDINGS = new RoomConstraints.Constraint((Func<KPrefabID, bool>) (bc => bc.HasTag(RoomConstraints.ConstraintTags.ScienceBuilding)), (Func<Room, bool>) null, 2, (string) ROOMS.CRITERIA.SCIENCE_BUILDINGS.NAME, (string) ROOMS.CRITERIA.SCIENCE_BUILDINGS.DESCRIPTION);
  public static RoomConstraints.Constraint BED_SINGLE = new RoomConstraints.Constraint((Func<KPrefabID, bool>) (bc => bc.HasTag(RoomConstraints.ConstraintTags.BedType) && !bc.HasTag(RoomConstraints.ConstraintTags.Clinic)), (Func<Room, bool>) (room =>
  {
    short num = 0;
    for (int index = 0; num < (short) 2 && index < room.buildings.Count; ++index)
    {
      if (room.buildings[index].HasTag(RoomConstraints.ConstraintTags.BedType))
        ++num;
    }
    return num == (short) 1;
  }), name: ((string) ROOMS.CRITERIA.BED_SINGLE.NAME), description: ((string) ROOMS.CRITERIA.BED_SINGLE.DESCRIPTION));
  public static RoomConstraints.Constraint LUXURY_BED_SINGLE = new RoomConstraints.Constraint((Func<KPrefabID, bool>) (bc => bc.HasTag(RoomConstraints.ConstraintTags.LuxuryBedType)), (Func<Room, bool>) (room =>
  {
    short num = 0;
    for (int index = 0; num <= (short) 2 && index < room.buildings.Count; ++index)
    {
      if (room.buildings[index].HasTag(RoomConstraints.ConstraintTags.LuxuryBedType))
        ++num;
    }
    return num == (short) 1;
  }), name: ((string) ROOMS.CRITERIA.LUXURY_BED_SINGLE.NAME), description: ((string) ROOMS.CRITERIA.LUXURY_BED_SINGLE.DESCRIPTION));
  public static RoomConstraints.Constraint BUILDING_DECOR_POSITIVE = new RoomConstraints.Constraint((Func<KPrefabID, bool>) (bc =>
  {
    DecorProvider component = ((Component) bc).GetComponent<DecorProvider>();
    return Object.op_Inequality((Object) component, (Object) null) && (double) component.baseDecor > 0.0;
  }), (Func<Room, bool>) null, name: ((string) ROOMS.CRITERIA.BUILDING_DECOR_POSITIVE.NAME), description: ((string) ROOMS.CRITERIA.BUILDING_DECOR_POSITIVE.DESCRIPTION));
  public static RoomConstraints.Constraint DECORATIVE_ITEM = new RoomConstraints.Constraint((Func<KPrefabID, bool>) (bc => bc.HasTag(GameTags.Decoration)), (Func<Room, bool>) null, name: string.Format((string) ROOMS.CRITERIA.DECORATIVE_ITEM.NAME, (object) 1), description: string.Format((string) ROOMS.CRITERIA.DECORATIVE_ITEM.DESCRIPTION, (object) 1));
  public static RoomConstraints.Constraint DECORATIVE_ITEM_2 = new RoomConstraints.Constraint((Func<KPrefabID, bool>) (bc => bc.HasTag(GameTags.Decoration)), (Func<Room, bool>) null, 2, string.Format((string) ROOMS.CRITERIA.DECORATIVE_ITEM.NAME, (object) 2), string.Format((string) ROOMS.CRITERIA.DECORATIVE_ITEM.DESCRIPTION, (object) 2));
  public static RoomConstraints.Constraint DECORATIVE_ITEM_SCORE_20 = new RoomConstraints.Constraint((Func<KPrefabID, bool>) (bc => bc.HasTag(GameTags.Decoration) && bc.HasTag(RoomConstraints.ConstraintTags.Decor20)), (Func<Room, bool>) null, name: string.Format((string) ROOMS.CRITERIA.DECORATIVE_ITEM_N.NAME, (object) "20"), description: string.Format((string) ROOMS.CRITERIA.DECORATIVE_ITEM_N.DESCRIPTION, (object) "20"));
  public static RoomConstraints.Constraint POWER_STATION = new RoomConstraints.Constraint((Func<KPrefabID, bool>) (bc => bc.HasTag(RoomConstraints.ConstraintTags.PowerStation)), (Func<Room, bool>) null, name: ((string) ROOMS.CRITERIA.POWER_STATION.NAME), description: ((string) ROOMS.CRITERIA.POWER_STATION.DESCRIPTION));
  public static RoomConstraints.Constraint FARM_STATION = new RoomConstraints.Constraint((Func<KPrefabID, bool>) (bc => bc.HasTag(RoomConstraints.ConstraintTags.FarmStationType)), (Func<Room, bool>) null, name: ((string) ROOMS.CRITERIA.FARM_STATION.NAME), description: ((string) ROOMS.CRITERIA.FARM_STATION.DESCRIPTION));
  public static RoomConstraints.Constraint RANCH_STATION = new RoomConstraints.Constraint((Func<KPrefabID, bool>) (bc => bc.HasTag(RoomConstraints.ConstraintTags.RanchStationType)), (Func<Room, bool>) null, name: ((string) ROOMS.CRITERIA.RANCH_STATION.NAME), description: ((string) ROOMS.CRITERIA.RANCH_STATION.DESCRIPTION));
  public static RoomConstraints.Constraint SPICE_STATION = new RoomConstraints.Constraint((Func<KPrefabID, bool>) (bc => bc.HasTag(RoomConstraints.ConstraintTags.SpiceStation)), (Func<Room, bool>) null, name: ((string) ROOMS.CRITERIA.SPICE_STATION.NAME), description: ((string) ROOMS.CRITERIA.SPICE_STATION.DESCRIPTION));
  public static RoomConstraints.Constraint COOK_TOP = new RoomConstraints.Constraint((Func<KPrefabID, bool>) (bc => bc.HasTag(RoomConstraints.ConstraintTags.CookTop)), (Func<Room, bool>) null, name: ((string) ROOMS.CRITERIA.COOK_TOP.NAME), description: ((string) ROOMS.CRITERIA.COOK_TOP.DESCRIPTION));
  public static RoomConstraints.Constraint REFRIGERATOR = new RoomConstraints.Constraint((Func<KPrefabID, bool>) (bc => bc.HasTag(RoomConstraints.ConstraintTags.Refrigerator)), (Func<Room, bool>) null, name: ((string) ROOMS.CRITERIA.REFRIGERATOR.NAME), description: ((string) ROOMS.CRITERIA.REFRIGERATOR.DESCRIPTION));
  public static RoomConstraints.Constraint REC_BUILDING = new RoomConstraints.Constraint((Func<KPrefabID, bool>) (bc => bc.HasTag(RoomConstraints.ConstraintTags.RecBuilding)), (Func<Room, bool>) null, name: ((string) ROOMS.CRITERIA.REC_BUILDING.NAME), description: ((string) ROOMS.CRITERIA.REC_BUILDING.DESCRIPTION));
  public static RoomConstraints.Constraint MACHINE_SHOP = new RoomConstraints.Constraint((Func<KPrefabID, bool>) (bc => bc.HasTag(RoomConstraints.ConstraintTags.MachineShopType)), (Func<Room, bool>) null, name: ((string) ROOMS.CRITERIA.MACHINE_SHOP.NAME), description: ((string) ROOMS.CRITERIA.MACHINE_SHOP.DESCRIPTION));
  public static RoomConstraints.Constraint LIGHT = new RoomConstraints.Constraint((Func<KPrefabID, bool>) null, (Func<Room, bool>) (room =>
  {
    foreach (KPrefabID creature in room.cavity.creatures)
    {
      if (Object.op_Inequality((Object) creature, (Object) null) && Object.op_Inequality((Object) ((Component) creature).GetComponent<Light2D>(), (Object) null))
        return true;
    }
    foreach (KPrefabID building in room.buildings)
    {
      if (!Object.op_Equality((Object) building, (Object) null))
      {
        Light2D component1 = ((Component) building).GetComponent<Light2D>();
        if (Object.op_Inequality((Object) component1, (Object) null))
        {
          RequireInputs component2 = ((Component) building).GetComponent<RequireInputs>();
          return ((Behaviour) component1).enabled || Object.op_Inequality((Object) component2, (Object) null) && component2.RequirementsMet;
        }
      }
    }
    return false;
  }), name: ((string) ROOMS.CRITERIA.LIGHT.NAME), description: ((string) ROOMS.CRITERIA.LIGHT.DESCRIPTION));
  public static RoomConstraints.Constraint DESTRESSING_BUILDING = new RoomConstraints.Constraint((Func<KPrefabID, bool>) (bc => bc.HasTag(RoomConstraints.ConstraintTags.DeStressingBuilding)), (Func<Room, bool>) null, name: ((string) ROOMS.CRITERIA.DESTRESSING_BUILDING.NAME), description: ((string) ROOMS.CRITERIA.DESTRESSING_BUILDING.DESCRIPTION));
  public static RoomConstraints.Constraint MASSAGE_TABLE = new RoomConstraints.Constraint((Func<KPrefabID, bool>) (bc => bc.IsPrefabID(RoomConstraints.ConstraintTags.MassageTable)), (Func<Room, bool>) null, name: ((string) ROOMS.CRITERIA.MASSAGE_TABLE.NAME), description: ((string) ROOMS.CRITERIA.MASSAGE_TABLE.DESCRIPTION));
  public static RoomConstraints.Constraint MESS_STATION_SINGLE = new RoomConstraints.Constraint((Func<KPrefabID, bool>) (bc => bc.HasTag(RoomConstraints.ConstraintTags.MessTable)), (Func<Room, bool>) null, name: ((string) ROOMS.CRITERIA.MESS_STATION_SINGLE.NAME), description: ((string) ROOMS.CRITERIA.MESS_STATION_SINGLE.DESCRIPTION), stomp_in_conflict: new List<RoomConstraints.Constraint>()
  {
    RoomConstraints.REC_BUILDING
  });
  public static RoomConstraints.Constraint TOILET = new RoomConstraints.Constraint((Func<KPrefabID, bool>) (bc => bc.HasTag(RoomConstraints.ConstraintTags.ToiletType)), (Func<Room, bool>) null, name: ((string) ROOMS.CRITERIA.TOILET.NAME), description: ((string) ROOMS.CRITERIA.TOILET.DESCRIPTION));
  public static RoomConstraints.Constraint FLUSH_TOILET = new RoomConstraints.Constraint((Func<KPrefabID, bool>) (bc => bc.HasTag(RoomConstraints.ConstraintTags.FlushToiletType)), (Func<Room, bool>) null, name: ((string) ROOMS.CRITERIA.FLUSH_TOILET.NAME), description: ((string) ROOMS.CRITERIA.FLUSH_TOILET.DESCRIPTION));
  public static RoomConstraints.Constraint WASH_STATION = new RoomConstraints.Constraint((Func<KPrefabID, bool>) (bc => bc.HasTag(RoomConstraints.ConstraintTags.WashStation)), (Func<Room, bool>) null, name: ((string) ROOMS.CRITERIA.WASH_STATION.NAME), description: ((string) ROOMS.CRITERIA.WASH_STATION.DESCRIPTION));
  public static RoomConstraints.Constraint ADVANCED_WASH_STATION = new RoomConstraints.Constraint((Func<KPrefabID, bool>) (bc => bc.HasTag(RoomConstraints.ConstraintTags.AdvancedWashStation)), (Func<Room, bool>) null, name: ((string) ROOMS.CRITERIA.ADVANCED_WASH_STATION.NAME), description: ((string) ROOMS.CRITERIA.ADVANCED_WASH_STATION.DESCRIPTION));
  public static RoomConstraints.Constraint CLINIC = new RoomConstraints.Constraint((Func<KPrefabID, bool>) (bc => bc.HasTag(RoomConstraints.ConstraintTags.Clinic)), (Func<Room, bool>) null, name: ((string) ROOMS.CRITERIA.CLINIC.NAME), description: ((string) ROOMS.CRITERIA.CLINIC.DESCRIPTION), stomp_in_conflict: new List<RoomConstraints.Constraint>()
  {
    RoomConstraints.TOILET,
    RoomConstraints.FLUSH_TOILET,
    RoomConstraints.MESS_STATION_SINGLE
  });
  public static RoomConstraints.Constraint PARK_BUILDING = new RoomConstraints.Constraint((Func<KPrefabID, bool>) (bc => bc.HasTag(RoomConstraints.ConstraintTags.Park)), (Func<Room, bool>) null, name: ((string) ROOMS.CRITERIA.PARK_BUILDING.NAME), description: ((string) ROOMS.CRITERIA.PARK_BUILDING.DESCRIPTION));
  public static RoomConstraints.Constraint ORIGINALTILES = new RoomConstraints.Constraint((Func<KPrefabID, bool>) null, (Func<Room, bool>) (room => 1 + room.cavity.maxY - room.cavity.minY >= 4));
  public static RoomConstraints.Constraint IS_BACKWALLED = new RoomConstraints.Constraint((Func<KPrefabID, bool>) null, (Func<Room, bool>) (room =>
  {
    bool flag = true;
    int num = (room.cavity.maxX - room.cavity.minX + 1) / 2 + 1;
    for (int index = 0; flag && index < num; ++index)
    {
      int x1 = room.cavity.minX + index;
      int x2 = room.cavity.maxX - index;
      for (int minY = room.cavity.minY; flag && minY <= room.cavity.maxY; ++minY)
      {
        int cell1 = Grid.XYToCell(x1, minY);
        int cell2 = Grid.XYToCell(x2, minY);
        if (Game.Instance.roomProber.GetCavityForCell(cell1) == room.cavity)
          flag &= Object.op_Inequality((Object) Grid.Objects[cell1, 2], (Object) null);
        if (Game.Instance.roomProber.GetCavityForCell(cell2) == room.cavity)
          flag &= Object.op_Inequality((Object) Grid.Objects[cell2, 2], (Object) null);
      }
    }
    return flag;
  }), name: ((string) ROOMS.CRITERIA.IS_BACKWALLED.NAME), description: ((string) ROOMS.CRITERIA.IS_BACKWALLED.DESCRIPTION));
  public static RoomConstraints.Constraint WILDANIMAL = new RoomConstraints.Constraint((Func<KPrefabID, bool>) null, (Func<Room, bool>) (room => room.cavity.creatures.Count + room.cavity.eggs.Count > 0), name: ((string) ROOMS.CRITERIA.WILDANIMAL.NAME), description: ((string) ROOMS.CRITERIA.WILDANIMAL.DESCRIPTION));
  public static RoomConstraints.Constraint WILDANIMALS = new RoomConstraints.Constraint((Func<KPrefabID, bool>) null, (Func<Room, bool>) (room =>
  {
    int num = 0;
    foreach (KPrefabID creature in room.cavity.creatures)
    {
      if (creature.HasTag(GameTags.Creatures.Wild))
        ++num;
    }
    return num >= 2;
  }), name: ((string) ROOMS.CRITERIA.WILDANIMALS.NAME), description: ((string) ROOMS.CRITERIA.WILDANIMALS.DESCRIPTION));
  public static RoomConstraints.Constraint WILDPLANT = new RoomConstraints.Constraint((Func<KPrefabID, bool>) null, (Func<Room, bool>) (room =>
  {
    int num = 0;
    foreach (KPrefabID plant in room.cavity.plants)
    {
      if (Object.op_Inequality((Object) plant, (Object) null))
      {
        BasicForagePlantPlanted component3 = ((Component) plant).GetComponent<BasicForagePlantPlanted>();
        ReceptacleMonitor component4 = ((Component) plant).GetComponent<ReceptacleMonitor>();
        if (Object.op_Inequality((Object) component4, (Object) null) && !component4.Replanted)
          ++num;
        else if (Object.op_Inequality((Object) component3, (Object) null))
          ++num;
      }
    }
    return num >= 2;
  }), name: ((string) ROOMS.CRITERIA.WILDPLANT.NAME), description: ((string) ROOMS.CRITERIA.WILDPLANT.DESCRIPTION));
  public static RoomConstraints.Constraint WILDPLANTS = new RoomConstraints.Constraint((Func<KPrefabID, bool>) null, (Func<Room, bool>) (room =>
  {
    int num = 0;
    foreach (KPrefabID plant in room.cavity.plants)
    {
      if (Object.op_Inequality((Object) plant, (Object) null))
      {
        BasicForagePlantPlanted component5 = ((Component) plant).GetComponent<BasicForagePlantPlanted>();
        ReceptacleMonitor component6 = ((Component) plant).GetComponent<ReceptacleMonitor>();
        if (Object.op_Inequality((Object) component6, (Object) null) && !component6.Replanted)
          ++num;
        else if (Object.op_Inequality((Object) component5, (Object) null))
          ++num;
      }
    }
    return num >= 4;
  }), name: ((string) ROOMS.CRITERIA.WILDPLANTS.NAME), description: ((string) ROOMS.CRITERIA.WILDPLANTS.DESCRIPTION));

  public static string RoomCriteriaString(Room room)
  {
    string str1 = "";
    RoomType roomType = room.roomType;
    if (roomType != Db.Get().RoomTypes.Neutral)
    {
      string str2 = str1 + "<b>" + (string) ROOMS.CRITERIA.HEADER + "</b>" + "\n    • " + roomType.primary_constraint.name;
      if (roomType.additional_constraints != null)
      {
        foreach (RoomConstraints.Constraint additionalConstraint in roomType.additional_constraints)
          str2 = !additionalConstraint.isSatisfied(room) ? str2 + "\n<color=#F44A47FF>    • " + additionalConstraint.name + "</color>" : str2 + "\n    • " + additionalConstraint.name;
      }
      return str2;
    }
    RoomTypes.RoomTypeQueryResult[] possibleRoomTypes = Db.Get().RoomTypes.GetPossibleRoomTypes(room);
    string str3 = str1 + (possibleRoomTypes.Length > 1 ? "<b>" + (string) ROOMS.CRITERIA.POSSIBLE_TYPES_HEADER + "</b>" : "");
    foreach (RoomTypes.RoomTypeQueryResult roomTypeQueryResult1 in possibleRoomTypes)
    {
      RoomType type1 = roomTypeQueryResult1.Type;
      if (type1 != Db.Get().RoomTypes.Neutral)
      {
        if (str3 != "")
          str3 += "\n";
        str3 = str3 + "<b><color=#BCBCBC>    • " + type1.Name + "</b> (" + type1.primary_constraint.name + ")</color>";
        if (roomTypeQueryResult1.SatisfactionRating == RoomType.RoomIdentificationResult.all_satisfied)
        {
          bool flag = false;
          foreach (RoomTypes.RoomTypeQueryResult roomTypeQueryResult2 in possibleRoomTypes)
          {
            RoomType type2 = roomTypeQueryResult2.Type;
            if (type2 != type1 && type2 != Db.Get().RoomTypes.Neutral && Db.Get().RoomTypes.HasAmbiguousRoomType(room, type1, type2))
            {
              flag = true;
              break;
            }
          }
          if (flag)
            str3 += string.Format("\n<color=#F44A47FF>{0}{1}{2}</color>", (object) "    ", (object) "    • ", (object) ROOMS.CRITERIA.NO_TYPE_CONFLICTS);
        }
        else
        {
          foreach (RoomConstraints.Constraint additionalConstraint in type1.additional_constraints)
          {
            if (!additionalConstraint.isSatisfied(room))
            {
              string empty = string.Empty;
              string str4 = additionalConstraint.building_criteria == null ? string.Format((string) ROOMS.CRITERIA.CRITERIA_FAILED.FAILED, (object) additionalConstraint.name) : string.Format((string) ROOMS.CRITERIA.CRITERIA_FAILED.MISSING_BUILDING, (object) additionalConstraint.name);
              str3 = str3 + "\n<color=#F44A47FF>        • " + str4 + "</color>";
            }
          }
        }
      }
    }
    return str3;
  }

  public static class ConstraintTags
  {
    public static Tag BedType = TagExtensions.ToTag(nameof (BedType));
    public static Tag LuxuryBedType = TagExtensions.ToTag(nameof (LuxuryBedType));
    public static Tag ToiletType = TagExtensions.ToTag(nameof (ToiletType));
    public static Tag FlushToiletType = TagExtensions.ToTag(nameof (FlushToiletType));
    public static Tag MessTable = TagExtensions.ToTag(nameof (MessTable));
    public static Tag Clinic = TagExtensions.ToTag(nameof (Clinic));
    public static Tag WashStation = TagExtensions.ToTag(nameof (WashStation));
    public static Tag AdvancedWashStation = TagExtensions.ToTag(nameof (AdvancedWashStation));
    public static Tag ScienceBuilding = TagExtensions.ToTag(nameof (ScienceBuilding));
    public static Tag LightSource = TagExtensions.ToTag(nameof (LightSource));
    public static Tag MassageTable = TagExtensions.ToTag(nameof (MassageTable));
    public static Tag DeStressingBuilding = TagExtensions.ToTag(nameof (DeStressingBuilding));
    public static Tag IndustrialMachinery = TagExtensions.ToTag(nameof (IndustrialMachinery));
    public static Tag PowerStation = TagExtensions.ToTag(nameof (PowerStation));
    public static Tag FarmStationType = TagExtensions.ToTag(nameof (FarmStationType));
    public static Tag CreatureRelocator = TagExtensions.ToTag(nameof (CreatureRelocator));
    public static Tag RanchStationType = TagExtensions.ToTag(nameof (RanchStationType));
    public static Tag SpiceStation = TagExtensions.ToTag(nameof (SpiceStation));
    public static Tag CookTop = TagExtensions.ToTag(nameof (CookTop));
    public static Tag Refrigerator = TagExtensions.ToTag(nameof (Refrigerator));
    public static Tag RecBuilding = TagExtensions.ToTag(nameof (RecBuilding));
    public static Tag MachineShopType = TagExtensions.ToTag(nameof (MachineShopType));
    public static Tag Park = TagExtensions.ToTag(nameof (Park));
    public static Tag NatureReserve = TagExtensions.ToTag(nameof (NatureReserve));
    public static Tag Decor20 = TagExtensions.ToTag(nameof (Decor20));
    public static Tag RocketInterior = TagExtensions.ToTag(nameof (RocketInterior));
  }

  public class Constraint
  {
    public string name;
    public string description;
    public int times_required = 1;
    public Func<Room, bool> room_criteria;
    public Func<KPrefabID, bool> building_criteria;
    public List<RoomConstraints.Constraint> stomp_in_conflict;

    public Constraint(
      Func<KPrefabID, bool> building_criteria,
      Func<Room, bool> room_criteria,
      int times_required = 1,
      string name = "",
      string description = "",
      List<RoomConstraints.Constraint> stomp_in_conflict = null)
    {
      this.room_criteria = room_criteria;
      this.building_criteria = building_criteria;
      this.times_required = times_required;
      this.name = name;
      this.description = description;
      this.stomp_in_conflict = stomp_in_conflict;
    }

    public bool isSatisfied(Room room)
    {
      int num = 0;
      if (this.room_criteria != null && !this.room_criteria(room))
        return false;
      if (this.building_criteria == null)
        return true;
      for (int index = 0; num < this.times_required && index < room.buildings.Count; ++index)
      {
        KPrefabID building = room.buildings[index];
        if (!Object.op_Equality((Object) building, (Object) null) && this.building_criteria(building))
          ++num;
      }
      for (int index = 0; num < this.times_required && index < room.plants.Count; ++index)
      {
        KPrefabID plant = room.plants[index];
        if (!Object.op_Equality((Object) plant, (Object) null) && this.building_criteria(plant))
          ++num;
      }
      return num >= this.times_required;
    }
  }
}
