// Decompiled with JetBrains decompiler
// Type: Database.RoomTypeCategories
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

namespace Database
{
  public class RoomTypeCategories : ResourceSet<RoomTypeCategory>
  {
    public RoomTypeCategory None;
    public RoomTypeCategory Food;
    public RoomTypeCategory Sleep;
    public RoomTypeCategory Recreation;
    public RoomTypeCategory Bathroom;
    public RoomTypeCategory Hospital;
    public RoomTypeCategory Industrial;
    public RoomTypeCategory Agricultural;
    public RoomTypeCategory Park;
    public RoomTypeCategory Science;

    private RoomTypeCategory Add(string id, string name, string colorName, string icon)
    {
      RoomTypeCategory roomTypeCategory = new RoomTypeCategory(id, name, colorName, icon);
      this.Add(roomTypeCategory);
      return roomTypeCategory;
    }

    public RoomTypeCategories(ResourceSet parent)
      : base(nameof (RoomTypeCategories), parent)
    {
      this.Initialize();
      this.None = this.Add(nameof (None), (string) ROOMS.CATEGORY.NONE.NAME, "roomNone", "unknown");
      this.Food = this.Add(nameof (Food), (string) ROOMS.CATEGORY.FOOD.NAME, "roomFood", "ui_room_food");
      this.Sleep = this.Add(nameof (Sleep), (string) ROOMS.CATEGORY.SLEEP.NAME, "roomSleep", "ui_room_sleep");
      this.Recreation = this.Add(nameof (Recreation), (string) ROOMS.CATEGORY.RECREATION.NAME, "roomRecreation", "ui_room_recreational");
      this.Bathroom = this.Add(nameof (Bathroom), (string) ROOMS.CATEGORY.BATHROOM.NAME, "roomBathroom", "ui_room_bathroom");
      this.Hospital = this.Add(nameof (Hospital), (string) ROOMS.CATEGORY.HOSPITAL.NAME, "roomHospital", "ui_room_hospital");
      this.Industrial = this.Add(nameof (Industrial), (string) ROOMS.CATEGORY.INDUSTRIAL.NAME, "roomIndustrial", "ui_room_industrial");
      this.Agricultural = this.Add(nameof (Agricultural), (string) ROOMS.CATEGORY.AGRICULTURAL.NAME, "roomAgricultural", "ui_room_agricultural");
      this.Park = this.Add(nameof (Park), (string) ROOMS.CATEGORY.PARK.NAME, "roomPark", "ui_room_park");
      this.Science = this.Add(nameof (Science), (string) ROOMS.CATEGORY.SCIENCE.NAME, "roomScience", "ui_room_science");
    }
  }
}
