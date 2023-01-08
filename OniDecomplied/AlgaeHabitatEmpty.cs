// Decompiled with JetBrains decompiler
// Type: AlgaeHabitatEmpty
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/AlgaeHabitatEmpty")]
public class AlgaeHabitatEmpty : Workable
{
  private static readonly HashedString[] CLEAN_ANIMS = new HashedString[2]
  {
    HashedString.op_Implicit("sponge_pre"),
    HashedString.op_Implicit("sponge_loop")
  };
  private static readonly HashedString PST_ANIM = new HashedString("sponge_pst");

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.workerStatusItem = Db.Get().DuplicantStatusItems.Cleaning;
    this.workingStatusItem = Db.Get().MiscStatusItems.Cleaning;
    this.attributeConverter = Db.Get().AttributeConverters.TidyingSpeed;
    this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
    this.workAnims = AlgaeHabitatEmpty.CLEAN_ANIMS;
    this.workingPstComplete = new HashedString[1]
    {
      AlgaeHabitatEmpty.PST_ANIM
    };
    this.workingPstFailed = new HashedString[1]
    {
      AlgaeHabitatEmpty.PST_ANIM
    };
    this.synchronizeAnims = false;
  }
}
