// Decompiled with JetBrains decompiler
// Type: BeeConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using UnityEngine;

public class BeeConfig : IEntityConfig
{
  public const string ID = "Bee";
  public const string BASE_TRAIT_ID = "BeeBaseTrait";

  public static GameObject CreateBee(
    string id,
    string name,
    string desc,
    string anim_file,
    bool is_baby)
  {
    GameObject go = BaseBeeConfig.BaseBee(id, name, desc, anim_file, "BeeBaseTrait", TUNING.DECOR.BONUS.TIER4, is_baby);
    Trait trait = Db.Get().CreateTrait("BeeBaseTrait", name, name, (string) null, false, (ChoreGroup[]) null, true, true);
    trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 5f, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 5f, name));
    go.AddTag(GameTags.OriginalCreature);
    return go;
  }

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public GameObject CreatePrefab() => BeeConfig.CreateBee("Bee", (string) STRINGS.CREATURES.SPECIES.BEE.NAME, (string) STRINGS.CREATURES.SPECIES.BEE.DESC, "bee_kanim", false);

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst) => BaseBeeConfig.SetupLoopingSounds(inst);
}
