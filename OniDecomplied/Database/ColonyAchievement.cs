// Decompiled with JetBrains decompiler
// Type: Database.ColonyAchievement
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMODUnity;
using System;
using System.Collections.Generic;

namespace Database
{
  public class ColonyAchievement : Resource
  {
    public string description;
    public bool isVictoryCondition;
    public string messageTitle;
    public string messageBody;
    public string shortVideoName;
    public string loopVideoName;
    public string platformAchievementId;
    public string icon;
    public List<ColonyAchievementRequirement> requirementChecklist = new List<ColonyAchievementRequirement>();
    public Action<KMonoBehaviour> victorySequence;
    public string[] dlcIds;

    public EventReference victoryNISSnapshot { get; private set; }

    public ColonyAchievement(
      string Id,
      string platformAchievementId,
      string Name,
      string description,
      bool isVictoryCondition,
      List<ColonyAchievementRequirement> requirementChecklist,
      string messageTitle = "",
      string messageBody = "",
      string videoDataName = "",
      string victoryLoopVideo = "",
      Action<KMonoBehaviour> VictorySequence = null,
      EventReference victorySnapshot = default (EventReference),
      string icon = "",
      string[] dlcIds = null)
      : base(Id, Name)
    {
      this.Id = Id;
      this.platformAchievementId = platformAchievementId;
      this.Name = Name;
      this.description = description;
      this.isVictoryCondition = isVictoryCondition;
      this.requirementChecklist = requirementChecklist;
      this.messageTitle = messageTitle;
      this.messageBody = messageBody;
      this.shortVideoName = videoDataName;
      this.loopVideoName = victoryLoopVideo;
      this.victorySequence = VictorySequence;
      this.victoryNISSnapshot = ((EventReference) ref victorySnapshot).IsNull ? AudioMixerSnapshots.Get().VictoryNISGenericSnapshot : victorySnapshot;
      this.icon = icon;
      this.dlcIds = dlcIds;
      if (this.dlcIds != null)
        return;
      this.dlcIds = DlcManager.AVAILABLE_ALL_VERSIONS;
    }
  }
}
