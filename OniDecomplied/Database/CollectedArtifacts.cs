// Decompiled with JetBrains decompiler
// Type: Database.CollectedArtifacts
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

namespace Database
{
  public class CollectedArtifacts : VictoryColonyAchievementRequirement
  {
    private const int REQUIRED_ARTIFACT_COUNT = 10;

    public override string GetProgress(bool complete) => ((string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.COLLECT_ARTIFACTS).Replace("{collectedCount}", this.GetStudiedArtifactCount().ToString()).Replace("{neededCount}", 10.ToString());

    public override string Description() => this.GetProgress(this.Success());

    public override bool Success() => ArtifactSelector.Instance.AnalyzedArtifactCount >= 10;

    private int GetStudiedArtifactCount() => ArtifactSelector.Instance.AnalyzedArtifactCount;

    public override string Name() => ((string) COLONY_ACHIEVEMENTS.STUDY_ARTIFACTS.REQUIREMENTS.STUDY_ARTIFACTS).Replace("{artifactCount}", 10.ToString());
  }
}
