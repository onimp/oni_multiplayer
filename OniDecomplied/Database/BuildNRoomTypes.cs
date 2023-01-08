// Decompiled with JetBrains decompiler
// Type: Database.BuildNRoomTypes
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

namespace Database
{
  public class BuildNRoomTypes : 
    ColonyAchievementRequirement,
    AchievementRequirementSerialization_Deprecated
  {
    private RoomType roomType;
    private int numToCreate;

    public BuildNRoomTypes(RoomType roomType, int numToCreate = 1)
    {
      this.roomType = roomType;
      this.numToCreate = numToCreate;
    }

    public override bool Success()
    {
      int num = 0;
      foreach (Room room in Game.Instance.roomProber.rooms)
      {
        if (room.roomType == this.roomType)
          ++num;
      }
      return num >= this.numToCreate;
    }

    public void Deserialize(IReader reader)
    {
      string str = reader.ReadKleiString();
      this.roomType = Db.Get().RoomTypes.Get(str);
      this.numToCreate = reader.ReadInt32();
    }

    public override string GetProgress(bool complete)
    {
      int num = 0;
      foreach (Room room in Game.Instance.roomProber.rooms)
      {
        if (room.roomType == this.roomType)
          ++num;
      }
      return string.Format((string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.BUILT_N_ROOMS, (object) this.roomType.Name, (object) (complete ? this.numToCreate : num), (object) this.numToCreate);
    }
  }
}
