// Decompiled with JetBrains decompiler
// Type: Database.EquipNDupes
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

namespace Database
{
  public class EquipNDupes : 
    ColonyAchievementRequirement,
    AchievementRequirementSerialization_Deprecated
  {
    private AssignableSlot equipmentSlot;
    private int numToEquip;

    public EquipNDupes(AssignableSlot equipmentSlot, int numToEquip)
    {
      this.equipmentSlot = equipmentSlot;
      this.numToEquip = numToEquip;
    }

    public override bool Success()
    {
      int num = 0;
      foreach (MinionIdentity minionIdentity in Components.MinionIdentities.Items)
      {
        Equipment equipment = minionIdentity.GetEquipment();
        if (Object.op_Inequality((Object) equipment, (Object) null) && equipment.IsSlotOccupied(this.equipmentSlot))
          ++num;
      }
      return num >= this.numToEquip;
    }

    public void Deserialize(IReader reader)
    {
      string str = reader.ReadKleiString();
      this.equipmentSlot = Db.Get().AssignableSlots.Get(str);
      this.numToEquip = reader.ReadInt32();
    }

    public override string GetProgress(bool complete)
    {
      int num = 0;
      foreach (MinionIdentity minionIdentity in Components.MinionIdentities.Items)
      {
        Equipment equipment = minionIdentity.GetEquipment();
        if (Object.op_Inequality((Object) equipment, (Object) null) && equipment.IsSlotOccupied(this.equipmentSlot))
          ++num;
      }
      return string.Format((string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.CLOTHE_DUPES, (object) (complete ? this.numToEquip : num), (object) this.numToEquip);
    }
  }
}
