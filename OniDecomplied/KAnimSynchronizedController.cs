// Decompiled with JetBrains decompiler
// Type: KAnimSynchronizedController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class KAnimSynchronizedController
{
  private KAnimControllerBase controller;
  public KAnimControllerBase synchronizedController;
  private KAnimLink link;
  private string postfix;

  public string Postfix
  {
    get => this.postfix;
    set => this.postfix = value;
  }

  public KAnimSynchronizedController(
    KAnimControllerBase controller,
    Grid.SceneLayer layer,
    string postfix)
  {
    this.controller = controller;
    this.Postfix = postfix;
    GameObject gameObject = Util.KInstantiate(EntityPrefabs.Instance.ForegroundLayer, ((Component) controller).gameObject, (string) null);
    ((Object) gameObject).name = ((Object) controller).name + postfix;
    this.synchronizedController = gameObject.GetComponent<KAnimControllerBase>();
    this.synchronizedController.AnimFiles = controller.AnimFiles;
    gameObject.SetActive(true);
    this.synchronizedController.initialAnim = controller.initialAnim + postfix;
    this.synchronizedController.defaultAnim = this.synchronizedController.initialAnim;
    Vector3 vector3;
    // ISSUE: explicit constructor call
    ((Vector3) ref vector3).\u002Ector(0.0f, 0.0f, Grid.GetLayerZ(layer) - 0.1f);
    TransformExtensions.SetLocalPosition(gameObject.transform, vector3);
    this.link = new KAnimLink(controller, this.synchronizedController);
    this.Dirty();
    KAnimSynchronizer synchronizer = controller.GetSynchronizer();
    synchronizer.Add(this);
    synchronizer.SyncController(this);
  }

  public void Enable(bool enable) => this.synchronizedController.enabled = enable;

  public void Play(HashedString anim_name, KAnim.PlayMode mode = 1, float speed = 1f, float time_offset = 0.0f)
  {
    if (!this.synchronizedController.enabled || !this.synchronizedController.HasAnimation(anim_name))
      return;
    this.synchronizedController.Play(anim_name, mode, speed, time_offset);
  }

  public void Dirty()
  {
    if (Object.op_Equality((Object) this.synchronizedController, (Object) null))
      return;
    this.synchronizedController.Offset = this.controller.Offset;
    this.synchronizedController.Pivot = this.controller.Pivot;
    this.synchronizedController.Rotation = this.controller.Rotation;
    this.synchronizedController.FlipX = this.controller.FlipX;
    this.synchronizedController.FlipY = this.controller.FlipY;
  }
}
