// Decompiled with JetBrains decompiler
// Type: RoomDetails
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

public class RoomDetails
{
  public static readonly RoomDetails.Detail AVERAGE_TEMPERATURE = new RoomDetails.Detail((Func<Room, string>) (room =>
  {
    float temp = 0.0f;
    return (double) temp == 0.0 ? string.Format((string) ROOMS.DETAILS.AVERAGE_TEMPERATURE.NAME, (object) UI.OVERLAYS.TEMPERATURE.EXTREMECOLD) : string.Format((string) ROOMS.DETAILS.AVERAGE_TEMPERATURE.NAME, (object) GameUtil.GetFormattedTemperature(temp));
  }));
  public static readonly RoomDetails.Detail AVERAGE_ATMO_MASS = new RoomDetails.Detail((Func<Room, string>) (room =>
  {
    float num1 = 0.0f;
    float num2 = 0.0f;
    float mass = (double) num2 <= 0.0 ? 0.0f : num1 / num2;
    return string.Format((string) ROOMS.DETAILS.AVERAGE_ATMO_MASS.NAME, (object) GameUtil.GetFormattedMass(mass));
  }));
  public static readonly RoomDetails.Detail ASSIGNED_TO = new RoomDetails.Detail((Func<Room, string>) (room =>
  {
    string str = "";
    foreach (KPrefabID primaryEntity in room.GetPrimaryEntities())
    {
      if (!Object.op_Equality((Object) primaryEntity, (Object) null))
      {
        Assignable component = ((Component) primaryEntity).GetComponent<Assignable>();
        if (!Object.op_Equality((Object) component, (Object) null))
        {
          IAssignableIdentity assignee = component.assignee;
          if (assignee == null)
          {
            str += str == "" ? "<color=#BCBCBC>    • " + ((Component) primaryEntity).GetProperName() + ": " + (string) ROOMS.DETAILS.ASSIGNED_TO.UNASSIGNED : "\n<color=#BCBCBC>    • " + ((Component) primaryEntity).GetProperName() + ": " + (string) ROOMS.DETAILS.ASSIGNED_TO.UNASSIGNED;
            str += "</color>";
          }
          else
            str += str == "" ? "    • " + ((Component) primaryEntity).GetProperName() + ": " + assignee.GetProperName() : "\n    • " + ((Component) primaryEntity).GetProperName() + ": " + assignee.GetProperName();
        }
      }
    }
    if (str == "")
      str = (string) ROOMS.DETAILS.ASSIGNED_TO.UNASSIGNED;
    return string.Format((string) ROOMS.DETAILS.ASSIGNED_TO.NAME, (object) str);
  }));
  public static readonly RoomDetails.Detail SIZE = new RoomDetails.Detail((Func<Room, string>) (room => string.Format((string) ROOMS.DETAILS.SIZE.NAME, (object) room.cavity.numCells)));
  public static readonly RoomDetails.Detail BUILDING_COUNT = new RoomDetails.Detail((Func<Room, string>) (room => string.Format((string) ROOMS.DETAILS.BUILDING_COUNT.NAME, (object) room.buildings.Count)));
  public static readonly RoomDetails.Detail CREATURE_COUNT = new RoomDetails.Detail((Func<Room, string>) (room => string.Format((string) ROOMS.DETAILS.CREATURE_COUNT.NAME, (object) (room.cavity.creatures.Count + room.cavity.eggs.Count))));
  public static readonly RoomDetails.Detail PLANT_COUNT = new RoomDetails.Detail((Func<Room, string>) (room => string.Format((string) ROOMS.DETAILS.PLANT_COUNT.NAME, (object) room.cavity.plants.Count)));
  public static readonly RoomDetails.Detail EFFECT = new RoomDetails.Detail((Func<Room, string>) (room => room.roomType.effect));
  public static readonly RoomDetails.Detail EFFECTS = new RoomDetails.Detail((Func<Room, string>) (room => room.roomType.GetRoomEffectsString()));

  public static string RoomDetailString(Room room)
  {
    string str = "" + "<b>" + (string) ROOMS.DETAILS.HEADER + "</b>";
    foreach (RoomDetails.Detail displayDetail in room.roomType.display_details)
      str = str + "\n    • " + displayDetail.resolve_string_function(room);
    return str;
  }

  public class Detail
  {
    public Func<Room, string> resolve_string_function;

    public Detail(Func<Room, string> resolve_string_function) => this.resolve_string_function = resolve_string_function;
  }
}
