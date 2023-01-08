// Decompiled with JetBrains decompiler
// Type: OneshotReactableLocator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class OneshotReactableLocator : IEntityConfig
{
  public static readonly string ID = nameof (OneshotReactableLocator);

  public static EmoteReactable CreateOneshotReactable(
    GameObject source,
    float lifetime,
    string id,
    ChoreType chore_type,
    int range_width = 15,
    int range_height = 15,
    float min_reactor_time = 20f)
  {
    GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(OneshotReactableLocator.ID)), TransformExtensions.GetPosition(source.transform));
    EmoteReactable oneshotReactable = new EmoteReactable(gameObject, HashedString.op_Implicit(id), chore_type, range_width, range_height, 100000f, min_reactor_time);
    oneshotReactable.AddPrecondition(OneshotReactableLocator.ReactorIsNotSource(source));
    OneshotReactableHost component = gameObject.GetComponent<OneshotReactableHost>();
    component.lifetime = lifetime;
    component.SetReactable((Reactable) oneshotReactable);
    gameObject.SetActive(true);
    return oneshotReactable;
  }

  private static Reactable.ReactablePrecondition ReactorIsNotSource(GameObject source) => (Reactable.ReactablePrecondition) ((reactor, transition) => Object.op_Inequality((Object) reactor, (Object) source));

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject entity = EntityTemplates.CreateEntity(OneshotReactableLocator.ID, OneshotReactableLocator.ID, false);
    entity.AddTag(GameTags.NotConversationTopic);
    entity.AddOrGet<OneshotReactableHost>();
    return entity;
  }

  public void OnPrefabInit(GameObject go)
  {
  }

  public void OnSpawn(GameObject go)
  {
  }
}
