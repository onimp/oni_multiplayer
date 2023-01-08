// Decompiled with JetBrains decompiler
// Type: Database.CollectedSpaceArtifacts
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

namespace Database
{
  public class CollectedSpaceArtifacts : VictoryColonyAchievementRequirement
  {
    private const int REQUIRED_ARTIFACT_COUNT = 10;

    public override string GetProgress(bool complete) => ((string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.COLLECT_SPACE_ARTIFACTS).Replace("{collectedCount}", this.GetStudiedSpaceArtifactCount().ToString()).Replace("{neededCount}", 10.ToString());

    public override string Description() => this.GetProgress(this.Success());

    public override bool Success() => ArtifactSelector.Instance.AnalyzedSpaceArtifactCount >= 10;

    private int GetStudiedSpaceArtifactCount() => ArtifactSelector.Instance.AnalyzedSpaceArtifactCount;

    public override string Name() => ((string) COLONY_ACHIEVEMENTS.STUDY_ARTIFACTS.REQUIREMENTS.STUDY_SPACE_ARTIFACTS).Replace("{artifactCount}", 10.ToString());
  }
}
