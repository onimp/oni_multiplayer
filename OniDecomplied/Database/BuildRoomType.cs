// Decompiled with JetBrains decompiler
// Type: Database.BuildRoomType
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

namespace Database
{
  public class BuildRoomType : 
    ColonyAchievementRequirement,
    AchievementRequirementSerialization_Deprecated
  {
    private RoomType roomType;

    public BuildRoomType(RoomType roomType) => this.roomType = roomType;

    public override bool Success()
    {
      foreach (Room room in Game.Instance.roomProber.rooms)
      {
        if (room.roomType == this.roomType)
          return true;
      }
      return false;
    }

    public void Deserialize(IReader reader)
    {
      string str = reader.ReadKleiString();
      this.roomType = Db.Get().RoomTypes.Get(str);
    }

    public override string GetProgress(bool complete) => string.Format((string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.BUILT_A_ROOM, (object) this.roomType.Name);
  }
}
