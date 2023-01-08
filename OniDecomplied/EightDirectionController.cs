// Decompiled with JetBrains decompiler
// Type: EightDirectionController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class EightDirectionController
{
  public GameObject gameObject;
  private string defaultAnim;
  private KAnimLink link;

  public KBatchedAnimController controller { get; private set; }

  public EightDirectionController(
    KAnimControllerBase buildingController,
    string targetSymbol,
    string defaultAnim,
    EightDirectionController.Offset frontBank)
  {
    this.Initialize(buildingController, targetSymbol, defaultAnim, frontBank, Grid.SceneLayer.NoLayer);
  }

  private void Initialize(
    KAnimControllerBase buildingController,
    string targetSymbol,
    string defaultAnim,
    EightDirectionController.Offset frontBack,
    Grid.SceneLayer userSpecifiedRenderLayer)
  {
    string str = ((Object) buildingController).name + ".eight_direction";
    this.gameObject = new GameObject(str);
    this.gameObject.SetActive(false);
    this.gameObject.transform.parent = ((Component) buildingController).transform;
    this.gameObject.AddComponent<KPrefabID>().PrefabTag = new Tag(str);
    this.defaultAnim = defaultAnim;
    this.controller = this.gameObject.AddOrGet<KBatchedAnimController>();
    this.controller.AnimFiles = new KAnimFile[1]
    {
      buildingController.AnimFiles[0]
    };
    this.controller.initialAnim = defaultAnim;
    this.controller.isMovable = true;
    this.controller.sceneLayer = Grid.SceneLayer.NoLayer;
    if (EightDirectionController.Offset.UserSpecified == frontBack)
      this.controller.sceneLayer = userSpecifiedRenderLayer;
    buildingController.SetSymbolVisiblity(KAnimHashedString.op_Implicit(targetSymbol), false);
    Matrix4x4 symbolTransform = buildingController.GetSymbolTransform(new HashedString(targetSymbol), out bool _);
    Vector3 vector3 = Vector4.op_Implicit(((Matrix4x4) ref symbolTransform).GetColumn(3));
    switch (frontBack)
    {
      case EightDirectionController.Offset.Infront:
        vector3.z = TransformExtensions.GetPosition(((Component) buildingController).transform).z - 0.1f;
        break;
      case EightDirectionController.Offset.Behind:
        vector3.z = TransformExtensions.GetPosition(((Component) buildingController).transform).z + 0.1f;
        break;
      case EightDirectionController.Offset.UserSpecified:
        vector3.z = Grid.GetLayerZ(userSpecifiedRenderLayer);
        break;
    }
    TransformExtensions.SetPosition(this.gameObject.transform, vector3);
    this.gameObject.SetActive(true);
    this.link = new KAnimLink(buildingController, (KAnimControllerBase) this.controller);
  }

  public void SetPositionPercent(float percent_full)
  {
    if (Object.op_Equality((Object) this.controller, (Object) null))
      return;
    this.controller.SetPositionPercent(percent_full);
  }

  public void SetSymbolTint(KAnimHashedString symbol, Color32 colour)
  {
    if (!Object.op_Inequality((Object) this.controller, (Object) null))
      return;
    this.controller.SetSymbolTint(symbol, Color32.op_Implicit(colour));
  }

  public void SetRotation(float rot)
  {
    if (Object.op_Equality((Object) this.controller, (Object) null))
      return;
    this.controller.Rotation = rot;
  }

  public void PlayAnim(string anim, KAnim.PlayMode mode = 1) => this.controller.Play(HashedString.op_Implicit(anim), mode);

  public enum Offset
  {
    Infront,
    Behind,
    UserSpecified,
  }
}
