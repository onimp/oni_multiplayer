// Decompiled with JetBrains decompiler
// Type: CommonPlacerConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Rendering;

public class CommonPlacerConfig
{
  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab(string id, string name, Material default_material)
  {
    GameObject entity = EntityTemplates.CreateEntity(id, name);
    entity.layer = LayerMask.NameToLayer("PlaceWithDepth");
    entity.AddOrGet<SaveLoadRoot>();
    entity.AddOrGet<StateMachineController>();
    entity.AddOrGet<Prioritizable>().iconOffset = new Vector2(0.3f, 0.32f);
    KBoxCollider2D kboxCollider2D = entity.AddOrGet<KBoxCollider2D>();
    kboxCollider2D.offset = new Vector2(0.0f, 0.5f);
    kboxCollider2D.size = new Vector2(1f, 1f);
    GameObject gameObject = new GameObject("Mask");
    gameObject.layer = LayerMask.NameToLayer("PlaceWithDepth");
    gameObject.transform.parent = entity.transform;
    TransformExtensions.SetLocalPosition(gameObject.transform, new Vector3(0.0f, 0.5f, -3.537f));
    gameObject.transform.eulerAngles = new Vector3(0.0f, 180f, 0.0f);
    gameObject.AddComponent<MeshFilter>().sharedMesh = Assets.instance.commonPlacerAssets.mesh;
    MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
    ((Renderer) meshRenderer).lightProbeUsage = (LightProbeUsage) 0;
    ((Renderer) meshRenderer).reflectionProbeUsage = (ReflectionProbeUsage) 0;
    ((Renderer) meshRenderer).shadowCastingMode = (ShadowCastingMode) 0;
    ((Renderer) meshRenderer).receiveShadows = false;
    ((Renderer) meshRenderer).sharedMaterial = default_material;
    gameObject.AddComponent<EasingAnimations>().scales = new EasingAnimations.AnimationScales[2]
    {
      new EasingAnimations.AnimationScales()
      {
        name = "ScaleUp",
        startScale = 0.0f,
        endScale = 1f,
        type = EasingAnimations.AnimationScales.AnimationType.EaseInOutBack,
        easingMultiplier = 5f
      },
      new EasingAnimations.AnimationScales()
      {
        name = "ScaleDown",
        startScale = 1f,
        endScale = 0.0f,
        type = EasingAnimations.AnimationScales.AnimationType.EaseOutBack,
        easingMultiplier = 1f
      }
    };
    return entity;
  }

  [Serializable]
  public class CommonPlacerAssets
  {
    public Mesh mesh;
  }
}
