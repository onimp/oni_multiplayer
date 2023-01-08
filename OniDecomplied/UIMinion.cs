// Decompiled with JetBrains decompiler
// Type: UIMinion
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using System.Collections.Generic;
using UnityEngine;

public class UIMinion : KMonoBehaviour, UIMinionOrMannequin.ITarget
{
  public const float ANIM_SCALE = 0.38f;
  private KBatchedAnimController animController;
  private GameObject spawn;
  private UIMinionOrMannequinReactSource lastReactSource;

  public GameObject SpawnedAvatar
  {
    get
    {
      if (Object.op_Equality((Object) this.spawn, (Object) null))
        this.TrySpawn();
      return this.spawn;
    }
  }

  public Option<global::Personality> Personality { get; private set; }

  protected virtual void OnSpawn() => this.TrySpawn();

  public void TrySpawn()
  {
    if (!Object.op_Equality((Object) this.animController, (Object) null))
      return;
    this.animController = Util.KInstantiateUI(Assets.GetPrefab(Tag.op_Implicit(MinionUIPortrait.ID)), ((Component) this).gameObject, false).GetComponent<KBatchedAnimController>();
    ((Component) this.animController).gameObject.SetActive(true);
    this.animController.animScale = 0.38f;
    this.animController.Play(HashedString.op_Implicit("idle_default"), (KAnim.PlayMode) 0);
    this.spawn = ((Component) this.animController).gameObject;
  }

  public void SetMinion(global::Personality personality)
  {
    this.SpawnedAvatar.GetComponent<Accessorizer>().ApplyMinionPersonality(personality);
    this.Personality = (Option<global::Personality>) personality;
    CustomOutfit customOutfit = this.SpawnedAvatar.AddOrGet<CustomOutfit>();
    ((Behaviour) customOutfit).enabled = false;
    if (!(personality.Name == "Jorge"))
      return;
    customOutfit.Update();
  }

  public void SetOutfit(IEnumerable<ClothingItemResource> outfit) => this.SpawnedAvatar.GetComponent<Accessorizer>().ApplyClothingItems(outfit);

  public void React(UIMinionOrMannequinReactSource source)
  {
    if (source != UIMinionOrMannequinReactSource.OnPersonalityChanged && this.lastReactSource == source)
    {
      KAnim.Anim currentAnim = this.animController.GetCurrentAnim();
      if (currentAnim != null && currentAnim.name != "idle_default")
        return;
    }
    switch (source)
    {
      case UIMinionOrMannequinReactSource.OnPersonalityChanged:
        this.animController.Play(HashedString.op_Implicit("react"));
        break;
      case UIMinionOrMannequinReactSource.OnWholeOutfitChanged:
      case UIMinionOrMannequinReactSource.OnBottomChanged:
        this.animController.Play(HashedString.op_Implicit("react_bottoms"));
        break;
      case UIMinionOrMannequinReactSource.OnTopChanged:
        this.animController.Play(HashedString.op_Implicit("react_tops"));
        break;
      case UIMinionOrMannequinReactSource.OnGlovesChanged:
        this.animController.Play(HashedString.op_Implicit("react_gloves"));
        break;
      case UIMinionOrMannequinReactSource.OnShoesChanged:
        this.animController.Play(HashedString.op_Implicit("react_shoes"));
        break;
      default:
        this.animController.Play(HashedString.op_Implicit("cheer_pre"));
        this.animController.Queue(HashedString.op_Implicit("cheer_loop"));
        this.animController.Queue(HashedString.op_Implicit("cheer_pst"));
        break;
    }
    this.animController.Queue(HashedString.op_Implicit("idle_default"), (KAnim.PlayMode) 0);
    this.lastReactSource = source;
  }
}
