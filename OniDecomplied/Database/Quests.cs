// Decompiled with JetBrains decompiler
// Type: Database.Quests
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

namespace Database
{
  public class Quests : ResourceSet<Quest>
  {
    public Quest LonelyMinionGreetingQuest;
    public Quest LonelyMinionFoodQuest;
    public Quest LonelyMinionPowerQuest;
    public Quest LonelyMinionDecorQuest;

    public Quests(ResourceSet parent)
      : base(nameof (Quests), parent)
    {
      this.LonelyMinionGreetingQuest = this.Add(new Quest("KnockQuest", new QuestCriteria[1]
      {
        new QuestCriteria(Tag.op_Implicit("Neighbor"))
      }));
      QuestCriteria[] criteria = new QuestCriteria[1];
      Tag id = Tag.op_Implicit("FoodQuality");
      float[] targetValues = new float[1]{ 4f };
      HashSet<Tag> acceptedTags = new HashSet<Tag>();
      acceptedTags.Add(GameTags.Edible);
      criteria[0] = (QuestCriteria) new QuestCriteria_GreaterOrEqual(id, targetValues, 3, acceptedTags, QuestCriteria.BehaviorFlags.UniqueItems);
      this.LonelyMinionFoodQuest = this.Add(new Quest("FoodQuest", criteria));
      this.LonelyMinionPowerQuest = this.Add(new Quest("PluggedIn", new QuestCriteria[1]
      {
        (QuestCriteria) new QuestCriteria_GreaterOrEqual(Tag.op_Implicit("SuppliedPower"), new float[1]
        {
          3000f
        })
      }));
      this.LonelyMinionDecorQuest = this.Add(new Quest("HighDecor", new QuestCriteria[1]
      {
        (QuestCriteria) new QuestCriteria_GreaterOrEqual(Tag.op_Implicit("Decor"), new float[1]
        {
          120f
        }, flags: (QuestCriteria.BehaviorFlags.AllowsRegression | QuestCriteria.BehaviorFlags.TrackValues))
      }));
    }
  }
}
