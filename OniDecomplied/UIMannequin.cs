// Decompiled with JetBrains decompiler
// Type: UIMannequin
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using System.Collections.Generic;
using UnityEngine;

public class UIMannequin : KMonoBehaviour, UIMinionOrMannequin.ITarget
{
  public const float ANIM_SCALE = 0.38f;
  private KBatchedAnimController animController;
  private GameObject spawn;

  public GameObject SpawnedAvatar
  {
    get
    {
      if (Object.op_Equality((Object) this.spawn, (Object) null))
        this.TrySpawn();
      return this.spawn;
    }
  }

  public Option<global::Personality> Personality => new Option<global::Personality>();

  protected virtual void OnSpawn() => this.TrySpawn();

  public void TrySpawn()
  {
    if (!Object.op_Equality((Object) this.animController, (Object) null))
      return;
    this.animController = Util.KInstantiateUI(Assets.GetPrefab(Tag.op_Implicit(MannequinUIPortrait.ID)), ((Component) this).gameObject, false).GetComponent<KBatchedAnimController>();
    ((Component) this.animController).gameObject.SetActive(true);
    this.animController.animScale = 0.38f;
    this.animController.Play(HashedString.op_Implicit("idle"), (KAnim.PlayMode) 2);
    this.spawn = ((Component) this.animController).gameObject;
    this.SpawnedAvatar.GetComponent<KBatchedAnimController>().SetSymbolVisiblity(KAnimHashedString.op_Implicit("hand_paint"), false);
    this.SpawnedAvatar.GetComponent<KBatchedAnimController>().SetSymbolVisiblity(KAnimHashedString.op_Implicit("foot"), false);
    this.SpawnedAvatar.GetComponent<KBatchedAnimController>().SetSymbolVisiblity(KAnimHashedString.op_Implicit("torso"), false);
  }

  public void SetOutfit(IEnumerable<ClothingItemResource> outfit) => this.SpawnedAvatar.GetComponent<Accessorizer>().ApplyClothingItems(outfit, false);

  public void React(UIMinionOrMannequinReactSource source) => this.animController.Play(HashedString.op_Implicit("idle"));
}
