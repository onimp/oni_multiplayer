// Decompiled with JetBrains decompiler
// Type: Database.UpgradeAllBasicBuildings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

namespace Database
{
  public class UpgradeAllBasicBuildings : 
    ColonyAchievementRequirement,
    AchievementRequirementSerialization_Deprecated
  {
    private Tag basicBuilding;
    private Tag upgradeBuilding;

    public UpgradeAllBasicBuildings(Tag basicBuilding, Tag upgradeBuilding)
    {
      this.basicBuilding = basicBuilding;
      this.upgradeBuilding = upgradeBuilding;
    }

    public override bool Success()
    {
      bool flag = false;
      foreach (IBasicBuilding basicBuilding in Components.BasicBuildings.Items)
      {
        KPrefabID component = ((Component) basicBuilding.transform).GetComponent<KPrefabID>();
        if (component.HasTag(this.basicBuilding))
          return false;
        if (component.HasTag(this.upgradeBuilding))
          flag = true;
      }
      return flag;
    }

    public void Deserialize(IReader reader)
    {
      this.basicBuilding = new Tag(reader.ReadKleiString());
      this.upgradeBuilding = new Tag(reader.ReadKleiString());
    }

    public override string GetProgress(bool complete)
    {
      BuildingDef buildingDef1 = Assets.GetBuildingDef(((Tag) ref this.basicBuilding).Name);
      BuildingDef buildingDef2 = Assets.GetBuildingDef(((Tag) ref this.upgradeBuilding).Name);
      return string.Format((string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.UPGRADE_ALL_BUILDINGS, (object) buildingDef1.Name, (object) buildingDef2.Name);
    }
  }
}
