// Decompiled with JetBrains decompiler
// Type: Database.RoomTypes
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Database
{
  public class RoomTypes : ResourceSet<RoomType>
  {
    public RoomType Neutral;
    public RoomType Latrine;
    public RoomType PlumbedBathroom;
    public RoomType Barracks;
    public RoomType Bedroom;
    public RoomType PrivateBedroom;
    public RoomType MessHall;
    public RoomType Kitchen;
    public RoomType GreatHall;
    public RoomType Hospital;
    public RoomType MassageClinic;
    public RoomType PowerPlant;
    public RoomType Farm;
    public RoomType CreaturePen;
    public RoomType MachineShop;
    public RoomType RecRoom;
    public RoomType Park;
    public RoomType NatureReserve;
    public RoomType Laboratory;

    public RoomTypes(ResourceSet parent)
      : base(nameof (RoomTypes), parent)
    {
      this.Initialize();
      this.Neutral = this.Add(new RoomType(nameof (Neutral), (string) ROOMS.TYPES.NEUTRAL.NAME, (string) ROOMS.TYPES.NEUTRAL.DESCRIPTION, (string) ROOMS.TYPES.NEUTRAL.TOOLTIP, (string) ROOMS.TYPES.NEUTRAL.EFFECT, Db.Get().RoomTypeCategories.None, (RoomConstraints.Constraint) null, (RoomConstraints.Constraint[]) null, new RoomDetails.Detail[4]
      {
        RoomDetails.SIZE,
        RoomDetails.BUILDING_COUNT,
        RoomDetails.CREATURE_COUNT,
        RoomDetails.PLANT_COUNT
      }));
      this.PlumbedBathroom = this.Add(new RoomType(nameof (PlumbedBathroom), (string) ROOMS.TYPES.PLUMBEDBATHROOM.NAME, (string) ROOMS.TYPES.PLUMBEDBATHROOM.DESCRIPTION, (string) ROOMS.TYPES.PLUMBEDBATHROOM.TOOLTIP, (string) ROOMS.TYPES.PLUMBEDBATHROOM.EFFECT, Db.Get().RoomTypeCategories.Bathroom, RoomConstraints.FLUSH_TOILET, new RoomConstraints.Constraint[5]
      {
        RoomConstraints.ADVANCED_WASH_STATION,
        RoomConstraints.NO_OUTHOUSES,
        RoomConstraints.NO_INDUSTRIAL_MACHINERY,
        RoomConstraints.MINIMUM_SIZE_12,
        RoomConstraints.MAXIMUM_SIZE_64
      }, new RoomDetails.Detail[2]
      {
        RoomDetails.SIZE,
        RoomDetails.BUILDING_COUNT
      }, 1, effects: new string[1]{ "RoomBathroom" }, sortKey: 2));
      this.Latrine = this.Add(new RoomType(nameof (Latrine), (string) ROOMS.TYPES.LATRINE.NAME, (string) ROOMS.TYPES.LATRINE.DESCRIPTION, (string) ROOMS.TYPES.LATRINE.TOOLTIP, (string) ROOMS.TYPES.LATRINE.EFFECT, Db.Get().RoomTypeCategories.Bathroom, RoomConstraints.TOILET, new RoomConstraints.Constraint[4]
      {
        RoomConstraints.WASH_STATION,
        RoomConstraints.NO_INDUSTRIAL_MACHINERY,
        RoomConstraints.MINIMUM_SIZE_12,
        RoomConstraints.MAXIMUM_SIZE_64
      }, new RoomDetails.Detail[2]
      {
        RoomDetails.SIZE,
        RoomDetails.BUILDING_COUNT
      }, 1, new RoomType[1]{ this.PlumbedBathroom }, effects: new string[1]
      {
        "RoomLatrine"
      }, sortKey: 1));
      this.PrivateBedroom = this.Add(new RoomType("Private Bedroom", (string) ROOMS.TYPES.PRIVATE_BEDROOM.NAME, (string) ROOMS.TYPES.PRIVATE_BEDROOM.DESCRIPTION, (string) ROOMS.TYPES.PRIVATE_BEDROOM.TOOLTIP, (string) ROOMS.TYPES.PRIVATE_BEDROOM.EFFECT, Db.Get().RoomTypeCategories.Sleep, RoomConstraints.LUXURY_BED_SINGLE, new RoomConstraints.Constraint[7]
      {
        RoomConstraints.NO_COTS,
        RoomConstraints.MINIMUM_SIZE_24,
        RoomConstraints.MAXIMUM_SIZE_64,
        RoomConstraints.CEILING_HEIGHT_4,
        RoomConstraints.DECORATIVE_ITEM_2,
        RoomConstraints.NO_INDUSTRIAL_MACHINERY,
        RoomConstraints.IS_BACKWALLED
      }, new RoomDetails.Detail[2]
      {
        RoomDetails.SIZE,
        RoomDetails.BUILDING_COUNT
      }, 1, effects: new string[1]{ "RoomPrivateBedroom" }, sortKey: 5));
      this.Bedroom = this.Add(new RoomType(nameof (Bedroom), (string) ROOMS.TYPES.BEDROOM.NAME, (string) ROOMS.TYPES.BEDROOM.DESCRIPTION, (string) ROOMS.TYPES.BEDROOM.TOOLTIP, (string) ROOMS.TYPES.BEDROOM.EFFECT, Db.Get().RoomTypeCategories.Sleep, RoomConstraints.HAS_LUXURY_BED, new RoomConstraints.Constraint[6]
      {
        RoomConstraints.NO_COTS,
        RoomConstraints.NO_INDUSTRIAL_MACHINERY,
        RoomConstraints.MINIMUM_SIZE_12,
        RoomConstraints.MAXIMUM_SIZE_64,
        RoomConstraints.DECORATIVE_ITEM,
        RoomConstraints.CEILING_HEIGHT_4
      }, new RoomDetails.Detail[2]
      {
        RoomDetails.SIZE,
        RoomDetails.BUILDING_COUNT
      }, 1, new RoomType[1]{ this.PrivateBedroom }, effects: new string[1]
      {
        "RoomBedroom"
      }, sortKey: 4));
      this.Barracks = this.Add(new RoomType(nameof (Barracks), (string) ROOMS.TYPES.BARRACKS.NAME, (string) ROOMS.TYPES.BARRACKS.DESCRIPTION, (string) ROOMS.TYPES.BARRACKS.TOOLTIP, (string) ROOMS.TYPES.BARRACKS.EFFECT, Db.Get().RoomTypeCategories.Sleep, RoomConstraints.HAS_BED, new RoomConstraints.Constraint[3]
      {
        RoomConstraints.NO_INDUSTRIAL_MACHINERY,
        RoomConstraints.MINIMUM_SIZE_12,
        RoomConstraints.MAXIMUM_SIZE_64
      }, new RoomDetails.Detail[2]
      {
        RoomDetails.SIZE,
        RoomDetails.BUILDING_COUNT
      }, 1, new RoomType[2]
      {
        this.Bedroom,
        this.PrivateBedroom
      }, effects: new string[1]{ "RoomBarracks" }, sortKey: 3));
      this.GreatHall = this.Add(new RoomType(nameof (GreatHall), (string) ROOMS.TYPES.GREATHALL.NAME, (string) ROOMS.TYPES.GREATHALL.DESCRIPTION, (string) ROOMS.TYPES.GREATHALL.TOOLTIP, (string) ROOMS.TYPES.GREATHALL.EFFECT, Db.Get().RoomTypeCategories.Food, RoomConstraints.MESS_STATION_SINGLE, new RoomConstraints.Constraint[5]
      {
        RoomConstraints.NO_INDUSTRIAL_MACHINERY,
        RoomConstraints.MINIMUM_SIZE_32,
        RoomConstraints.MAXIMUM_SIZE_120,
        RoomConstraints.DECORATIVE_ITEM_SCORE_20,
        RoomConstraints.REC_BUILDING
      }, new RoomDetails.Detail[2]
      {
        RoomDetails.SIZE,
        RoomDetails.BUILDING_COUNT
      }, 1, effects: new string[1]{ "RoomGreatHall" }, sortKey: 6));
      this.MessHall = this.Add(new RoomType(nameof (MessHall), (string) ROOMS.TYPES.MESSHALL.NAME, (string) ROOMS.TYPES.MESSHALL.DESCRIPTION, (string) ROOMS.TYPES.MESSHALL.TOOLTIP, (string) ROOMS.TYPES.MESSHALL.EFFECT, Db.Get().RoomTypeCategories.Food, RoomConstraints.MESS_STATION_SINGLE, new RoomConstraints.Constraint[3]
      {
        RoomConstraints.NO_INDUSTRIAL_MACHINERY,
        RoomConstraints.MINIMUM_SIZE_12,
        RoomConstraints.MAXIMUM_SIZE_64
      }, new RoomDetails.Detail[2]
      {
        RoomDetails.SIZE,
        RoomDetails.BUILDING_COUNT
      }, 1, new RoomType[1]{ this.GreatHall }, effects: new string[1]
      {
        "RoomMessHall"
      }, sortKey: 5));
      this.Kitchen = this.Add(new RoomType(nameof (Kitchen), (string) ROOMS.TYPES.KITCHEN.NAME, (string) ROOMS.TYPES.KITCHEN.DESCRIPTION, (string) ROOMS.TYPES.KITCHEN.TOOLTIP, (string) ROOMS.TYPES.KITCHEN.EFFECT, Db.Get().RoomTypeCategories.Food, RoomConstraints.SPICE_STATION, new RoomConstraints.Constraint[5]
      {
        RoomConstraints.COOK_TOP,
        RoomConstraints.REFRIGERATOR,
        RoomConstraints.NO_MESS_STATION,
        RoomConstraints.MINIMUM_SIZE_12,
        RoomConstraints.MAXIMUM_SIZE_96
      }, new RoomDetails.Detail[2]
      {
        RoomDetails.SIZE,
        RoomDetails.BUILDING_COUNT
      }, 1, sortKey: 7));
      this.MassageClinic = this.Add(new RoomType(nameof (MassageClinic), (string) ROOMS.TYPES.MASSAGE_CLINIC.NAME, (string) ROOMS.TYPES.MASSAGE_CLINIC.DESCRIPTION, (string) ROOMS.TYPES.MASSAGE_CLINIC.TOOLTIP, (string) ROOMS.TYPES.MASSAGE_CLINIC.EFFECT, Db.Get().RoomTypeCategories.Hospital, RoomConstraints.MASSAGE_TABLE, new RoomConstraints.Constraint[4]
      {
        RoomConstraints.NO_INDUSTRIAL_MACHINERY,
        RoomConstraints.DECORATIVE_ITEM,
        RoomConstraints.MINIMUM_SIZE_12,
        RoomConstraints.MAXIMUM_SIZE_64
      }, new RoomDetails.Detail[2]
      {
        RoomDetails.SIZE,
        RoomDetails.BUILDING_COUNT
      }, 2, single_assignee: true, priority_building_use: true, sortKey: 9));
      this.Hospital = this.Add(new RoomType(nameof (Hospital), (string) ROOMS.TYPES.HOSPITAL.NAME, (string) ROOMS.TYPES.HOSPITAL.DESCRIPTION, (string) ROOMS.TYPES.HOSPITAL.TOOLTIP, (string) ROOMS.TYPES.HOSPITAL.EFFECT, Db.Get().RoomTypeCategories.Hospital, RoomConstraints.CLINIC, new RoomConstraints.Constraint[5]
      {
        RoomConstraints.TOILET,
        RoomConstraints.MESS_STATION_SINGLE,
        RoomConstraints.NO_INDUSTRIAL_MACHINERY,
        RoomConstraints.MINIMUM_SIZE_12,
        RoomConstraints.MAXIMUM_SIZE_96
      }, new RoomDetails.Detail[2]
      {
        RoomDetails.SIZE,
        RoomDetails.BUILDING_COUNT
      }, 2, single_assignee: true, priority_building_use: true, sortKey: 10));
      this.PowerPlant = this.Add(new RoomType(nameof (PowerPlant), (string) ROOMS.TYPES.POWER_PLANT.NAME, (string) ROOMS.TYPES.POWER_PLANT.DESCRIPTION, (string) ROOMS.TYPES.POWER_PLANT.TOOLTIP, (string) ROOMS.TYPES.POWER_PLANT.EFFECT, Db.Get().RoomTypeCategories.Industrial, RoomConstraints.POWER_STATION, new RoomConstraints.Constraint[2]
      {
        RoomConstraints.MINIMUM_SIZE_12,
        RoomConstraints.MAXIMUM_SIZE_96
      }, new RoomDetails.Detail[2]
      {
        RoomDetails.SIZE,
        RoomDetails.BUILDING_COUNT
      }, 2, single_assignee: true, priority_building_use: true, sortKey: 11));
      this.Farm = this.Add(new RoomType(nameof (Farm), (string) ROOMS.TYPES.FARM.NAME, (string) ROOMS.TYPES.FARM.DESCRIPTION, (string) ROOMS.TYPES.FARM.TOOLTIP, (string) ROOMS.TYPES.FARM.EFFECT, Db.Get().RoomTypeCategories.Agricultural, RoomConstraints.FARM_STATION, new RoomConstraints.Constraint[2]
      {
        RoomConstraints.MINIMUM_SIZE_12,
        RoomConstraints.MAXIMUM_SIZE_96
      }, new RoomDetails.Detail[2]
      {
        RoomDetails.SIZE,
        RoomDetails.BUILDING_COUNT
      }, 2, single_assignee: true, priority_building_use: true, sortKey: 12));
      this.CreaturePen = this.Add(new RoomType(nameof (CreaturePen), (string) ROOMS.TYPES.CREATUREPEN.NAME, (string) ROOMS.TYPES.CREATUREPEN.DESCRIPTION, (string) ROOMS.TYPES.CREATUREPEN.TOOLTIP, (string) ROOMS.TYPES.CREATUREPEN.EFFECT, Db.Get().RoomTypeCategories.Agricultural, RoomConstraints.RANCH_STATION, new RoomConstraints.Constraint[2]
      {
        RoomConstraints.MINIMUM_SIZE_12,
        RoomConstraints.MAXIMUM_SIZE_96
      }, new RoomDetails.Detail[3]
      {
        RoomDetails.SIZE,
        RoomDetails.BUILDING_COUNT,
        RoomDetails.CREATURE_COUNT
      }, 2, single_assignee: true, priority_building_use: true, sortKey: 13));
      this.Laboratory = this.Add(new RoomType(nameof (Laboratory), (string) ROOMS.TYPES.LABORATORY.NAME, (string) ROOMS.TYPES.LABORATORY.DESCRIPTION, (string) ROOMS.TYPES.LABORATORY.TOOLTIP, (string) ROOMS.TYPES.LABORATORY.EFFECT, Db.Get().RoomTypeCategories.Science, RoomConstraints.SCIENCE_BUILDINGS, new RoomConstraints.Constraint[4]
      {
        RoomConstraints.LIGHT,
        RoomConstraints.NO_INDUSTRIAL_MACHINERY,
        RoomConstraints.MINIMUM_SIZE_32,
        RoomConstraints.MAXIMUM_SIZE_120
      }, new RoomDetails.Detail[2]
      {
        RoomDetails.SIZE,
        RoomDetails.BUILDING_COUNT
      }, 2, single_assignee: true, priority_building_use: true, sortKey: 14));
      this.MachineShop = new RoomType(nameof (MachineShop), (string) ROOMS.TYPES.MACHINE_SHOP.NAME, (string) ROOMS.TYPES.MACHINE_SHOP.DESCRIPTION, (string) ROOMS.TYPES.MACHINE_SHOP.TOOLTIP, (string) ROOMS.TYPES.MACHINE_SHOP.EFFECT, Db.Get().RoomTypeCategories.Industrial, RoomConstraints.MACHINE_SHOP, new RoomConstraints.Constraint[2]
      {
        RoomConstraints.MINIMUM_SIZE_12,
        RoomConstraints.MAXIMUM_SIZE_96
      }, new RoomDetails.Detail[2]
      {
        RoomDetails.SIZE,
        RoomDetails.BUILDING_COUNT
      }, 2, single_assignee: true, priority_building_use: true, sortKey: 14);
      this.RecRoom = this.Add(new RoomType(nameof (RecRoom), (string) ROOMS.TYPES.REC_ROOM.NAME, (string) ROOMS.TYPES.REC_ROOM.DESCRIPTION, (string) ROOMS.TYPES.REC_ROOM.TOOLTIP, (string) ROOMS.TYPES.REC_ROOM.EFFECT, Db.Get().RoomTypeCategories.Recreation, RoomConstraints.REC_BUILDING, new RoomConstraints.Constraint[4]
      {
        RoomConstraints.NO_INDUSTRIAL_MACHINERY,
        RoomConstraints.DECORATIVE_ITEM,
        RoomConstraints.MINIMUM_SIZE_12,
        RoomConstraints.MAXIMUM_SIZE_96
      }, new RoomDetails.Detail[2]
      {
        RoomDetails.SIZE,
        RoomDetails.BUILDING_COUNT
      }, single_assignee: true, priority_building_use: true, sortKey: 15));
      this.NatureReserve = this.Add(new RoomType(nameof (NatureReserve), (string) ROOMS.TYPES.NATURERESERVE.NAME, (string) ROOMS.TYPES.NATURERESERVE.DESCRIPTION, (string) ROOMS.TYPES.NATURERESERVE.TOOLTIP, (string) ROOMS.TYPES.NATURERESERVE.EFFECT, Db.Get().RoomTypeCategories.Park, RoomConstraints.PARK_BUILDING, new RoomConstraints.Constraint[4]
      {
        RoomConstraints.WILDPLANTS,
        RoomConstraints.NO_INDUSTRIAL_MACHINERY,
        RoomConstraints.MINIMUM_SIZE_32,
        RoomConstraints.MAXIMUM_SIZE_120
      }, new RoomDetails.Detail[4]
      {
        RoomDetails.SIZE,
        RoomDetails.BUILDING_COUNT,
        RoomDetails.CREATURE_COUNT,
        RoomDetails.PLANT_COUNT
      }, 1, effects: new string[1]{ "RoomNatureReserve" }, sortKey: 17));
      this.Park = this.Add(new RoomType(nameof (Park), (string) ROOMS.TYPES.PARK.NAME, (string) ROOMS.TYPES.PARK.DESCRIPTION, (string) ROOMS.TYPES.PARK.TOOLTIP, (string) ROOMS.TYPES.PARK.EFFECT, Db.Get().RoomTypeCategories.Park, RoomConstraints.PARK_BUILDING, new RoomConstraints.Constraint[4]
      {
        RoomConstraints.WILDPLANT,
        RoomConstraints.NO_INDUSTRIAL_MACHINERY,
        RoomConstraints.MINIMUM_SIZE_12,
        RoomConstraints.MAXIMUM_SIZE_64
      }, new RoomDetails.Detail[4]
      {
        RoomDetails.SIZE,
        RoomDetails.BUILDING_COUNT,
        RoomDetails.CREATURE_COUNT,
        RoomDetails.PLANT_COUNT
      }, 1, new RoomType[1]{ this.NatureReserve }, effects: new string[1]
      {
        "RoomPark"
      }, sortKey: 16));
    }

    public Assignables[] GetAssignees(Room room)
    {
      if (room == null)
        return new Assignables[0];
      RoomType roomType = room.roomType;
      if (roomType.primary_constraint == null)
        return new Assignables[0];
      List<Assignables> assignablesList = new List<Assignables>();
      foreach (KPrefabID building in room.buildings)
      {
        if (!Object.op_Equality((Object) building, (Object) null) && roomType.primary_constraint.building_criteria(building))
        {
          Assignable component = ((Component) building).GetComponent<Assignable>();
          if (component.assignee != null)
          {
            foreach (Ownables owner in component.assignee.GetOwners())
            {
              if (!assignablesList.Contains((Assignables) owner))
                assignablesList.Add((Assignables) owner);
            }
          }
        }
      }
      return assignablesList.ToArray();
    }

    public RoomType GetRoomTypeForID(string id)
    {
      foreach (RoomType resource in this.resources)
      {
        if (resource.Id == id)
          return resource;
      }
      return (RoomType) null;
    }

    public RoomType GetRoomType(Room room)
    {
      foreach (RoomType resource1 in this.resources)
      {
        if (resource1 != this.Neutral && resource1.isSatisfactory(room) == RoomType.RoomIdentificationResult.all_satisfied)
        {
          bool flag = false;
          foreach (RoomType resource2 in this.resources)
          {
            if (resource1 != resource2 && resource2 != this.Neutral && this.HasAmbiguousRoomType(room, resource1, resource2))
            {
              flag = true;
              break;
            }
          }
          if (!flag)
            return resource1;
        }
      }
      return this.Neutral;
    }

    public bool HasAmbiguousRoomType(Room room, RoomType suspected_type, RoomType potential_type)
    {
      RoomType.RoomIdentificationResult identificationResult1 = potential_type.isSatisfactory(room);
      RoomType.RoomIdentificationResult identificationResult2 = suspected_type.isSatisfactory(room);
      if (identificationResult1 == RoomType.RoomIdentificationResult.all_satisfied && identificationResult2 == RoomType.RoomIdentificationResult.all_satisfied)
      {
        if (potential_type.priority > suspected_type.priority || suspected_type.upgrade_paths != null && Array.IndexOf<RoomType>(suspected_type.upgrade_paths, potential_type) != -1)
          return true;
        if (potential_type.upgrade_paths != null && Array.IndexOf<RoomType>(potential_type.upgrade_paths, suspected_type) != -1)
          return false;
      }
      if (identificationResult1 != RoomType.RoomIdentificationResult.primary_unsatisfied && (suspected_type.upgrade_paths == null || Array.IndexOf<RoomType>(suspected_type.upgrade_paths, potential_type) == -1))
      {
        if (suspected_type.primary_constraint == potential_type.primary_constraint)
        {
          suspected_type = this.Neutral;
        }
        else
        {
          bool flag = false;
          if (suspected_type.primary_constraint.stomp_in_conflict != null && suspected_type.primary_constraint.stomp_in_conflict.Contains(potential_type.primary_constraint))
            flag = true;
          else if (suspected_type.additional_constraints != null)
          {
            foreach (RoomConstraints.Constraint additionalConstraint in suspected_type.additional_constraints)
            {
              if (additionalConstraint == potential_type.primary_constraint || additionalConstraint.stomp_in_conflict != null && additionalConstraint.stomp_in_conflict.Contains(potential_type.primary_constraint))
              {
                flag = true;
                break;
              }
            }
          }
          return !flag;
        }
      }
      return false;
    }

    public RoomTypes.RoomTypeQueryResult[] GetPossibleRoomTypes(Room room)
    {
      RoomTypes.RoomTypeQueryResult[] array = new RoomTypes.RoomTypeQueryResult[((ResourceSet) this).Count];
      int newSize = 0;
      foreach (RoomType resource in this.resources)
      {
        if (resource != this.Neutral)
        {
          RoomType.RoomIdentificationResult identificationResult = resource.isSatisfactory(room);
          if (identificationResult != RoomType.RoomIdentificationResult.primary_unsatisfied)
          {
            array[newSize] = new RoomTypes.RoomTypeQueryResult()
            {
              Type = resource,
              SatisfactionRating = identificationResult
            };
            ++newSize;
          }
        }
      }
      if (newSize == 0)
      {
        array[newSize] = new RoomTypes.RoomTypeQueryResult()
        {
          Type = this.Neutral,
          SatisfactionRating = RoomType.RoomIdentificationResult.all_satisfied
        };
        ++newSize;
      }
      Array.Resize<RoomTypes.RoomTypeQueryResult>(ref array, newSize);
      return array;
    }

    public struct RoomTypeQueryResult
    {
      public RoomType Type;
      public RoomType.RoomIdentificationResult SatisfactionRating;
    }
  }
}
