// Decompiled with JetBrains decompiler
// Type: FXHelpers
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public static class FXHelpers
{
  public static KBatchedAnimController CreateEffect(
    string anim_file_name,
    Vector3 position,
    Transform parent = null,
    bool update_looping_sounds_position = false,
    Grid.SceneLayer layer = Grid.SceneLayer.Front,
    bool set_inactive = false)
  {
    KBatchedAnimController component = GameUtil.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(EffectConfigs.EffectTemplateId)), position, layer).GetComponent<KBatchedAnimController>();
    ((Component) component).GetComponent<KPrefabID>().PrefabTag = TagManager.Create(anim_file_name);
    ((Object) component).name = anim_file_name;
    if (Object.op_Inequality((Object) parent, (Object) null))
      ((Component) component).transform.SetParent(parent, false);
    TransformExtensions.SetPosition(((Component) component).transform, position);
    if (update_looping_sounds_position)
      Util.FindOrAddComponent<LoopingSounds>((Component) component).updatePosition = true;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit(anim_file_name));
    if (Object.op_Equality((Object) anim, (Object) null))
      Debug.LogWarning((object) ("Missing effect anim: " + anim_file_name));
    else
      component.AnimFiles = new KAnimFile[1]{ anim };
    if (!set_inactive)
      ((Component) component).gameObject.SetActive(true);
    return component;
  }
}
