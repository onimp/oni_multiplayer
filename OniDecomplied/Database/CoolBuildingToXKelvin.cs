// Decompiled with JetBrains decompiler
// Type: Database.CoolBuildingToXKelvin
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

namespace Database
{
  public class CoolBuildingToXKelvin : 
    ColonyAchievementRequirement,
    AchievementRequirementSerialization_Deprecated
  {
    private int kelvinToCoolTo;

    public CoolBuildingToXKelvin(int kelvinToCoolTo) => this.kelvinToCoolTo = kelvinToCoolTo;

    public override bool Success() => (double) BuildingComplete.MinKelvinSeen <= (double) this.kelvinToCoolTo;

    public void Deserialize(IReader reader) => this.kelvinToCoolTo = reader.ReadInt32();

    public override string GetProgress(bool complete)
    {
      float minKelvinSeen = BuildingComplete.MinKelvinSeen;
      return string.Format((string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.KELVIN_COOLING, (object) minKelvinSeen);
    }
  }
}
