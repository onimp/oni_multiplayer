// Decompiled with JetBrains decompiler
// Type: Database.HarvestAmountFromSpacePOI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

namespace Database
{
  public class HarvestAmountFromSpacePOI : ColonyAchievementRequirement
  {
    private float amountToHarvest;

    public HarvestAmountFromSpacePOI(float amountToHarvest) => this.amountToHarvest = amountToHarvest;

    public override string GetProgress(bool complete) => string.Format((string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.MINE_SPACE_POI, (object) ((Component) SaveGame.Instance).GetComponent<ColonyAchievementTracker>().totalMaterialsHarvestFromPOI, (object) this.amountToHarvest);

    public override bool Success() => (double) ((Component) SaveGame.Instance).GetComponent<ColonyAchievementTracker>().totalMaterialsHarvestFromPOI > (double) this.amountToHarvest;
  }
}
