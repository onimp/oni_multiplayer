// Decompiled with JetBrains decompiler
// Type: ColonyAchievementStatus
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

public class ColonyAchievementStatus
{
  public bool success;
  public bool failed;
  private ColonyAchievement m_achievement;

  public List<ColonyAchievementRequirement> Requirements => this.m_achievement.requirementChecklist;

  public ColonyAchievementStatus(string achievementId) => this.m_achievement = Db.Get().ColonyAchievements.TryGet(achievementId);

  public void UpdateAchievement()
  {
    if (this.Requirements.Count <= 0 || this.m_achievement.Disabled)
      return;
    this.success = true;
    foreach (ColonyAchievementRequirement requirement in this.Requirements)
    {
      this.success &= requirement.Success();
      this.failed |= requirement.Fail();
    }
  }

  public static ColonyAchievementStatus Deserialize(IReader reader, string achievementId)
  {
    bool flag1 = reader.ReadByte() > (byte) 0;
    bool flag2 = reader.ReadByte() > (byte) 0;
    if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 22))
    {
      int num = reader.ReadInt32();
      for (int index = 0; index < num; ++index)
      {
        System.Type type = System.Type.GetType(reader.ReadKleiString());
        if (type != (System.Type) null)
        {
          AchievementRequirementSerialization_Deprecated uninitializedObject = FormatterServices.GetUninitializedObject(type) as AchievementRequirementSerialization_Deprecated;
          Debug.Assert(uninitializedObject != null, (object) string.Format("Cannot deserialize old data for type {0}", (object) type));
          uninitializedObject.Deserialize(reader);
        }
      }
    }
    return new ColonyAchievementStatus(achievementId)
    {
      success = flag1,
      failed = flag2
    };
  }

  public void Serialize(BinaryWriter writer)
  {
    writer.Write(this.success ? (byte) 1 : (byte) 0);
    writer.Write(this.failed ? (byte) 1 : (byte) 0);
  }
}
