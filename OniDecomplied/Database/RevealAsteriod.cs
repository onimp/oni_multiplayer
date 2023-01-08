// Decompiled with JetBrains decompiler
// Type: Database.RevealAsteriod
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

namespace Database
{
  public class RevealAsteriod : 
    ColonyAchievementRequirement,
    AchievementRequirementSerialization_Deprecated
  {
    private float percentToReveal;
    private float amountRevealed;

    public RevealAsteriod(float percentToReveal) => this.percentToReveal = percentToReveal;

    public override bool Success()
    {
      this.amountRevealed = 0.0f;
      float num = 0.0f;
      WorldContainer startWorld = ClusterManager.Instance.GetStartWorld();
      Vector2 minimumBounds = startWorld.minimumBounds;
      Vector2 maximumBounds = startWorld.maximumBounds;
      for (int x = (int) minimumBounds.x; (double) x <= (double) maximumBounds.x; ++x)
      {
        for (int y = (int) minimumBounds.y; (double) y <= (double) maximumBounds.y; ++y)
        {
          if (Grid.Visible[Grid.PosToCell(new Vector2((float) x, (float) y))] > (byte) 0)
            ++num;
        }
      }
      this.amountRevealed = num / (float) (startWorld.Width * startWorld.Height);
      return (double) this.amountRevealed > (double) this.percentToReveal;
    }

    public void Deserialize(IReader reader) => this.percentToReveal = reader.ReadSingle();

    public override string GetProgress(bool complete) => string.Format((string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.REVEALED, (object) (float) ((double) this.amountRevealed * 100.0), (object) (float) ((double) this.percentToReveal * 100.0));
  }
}
