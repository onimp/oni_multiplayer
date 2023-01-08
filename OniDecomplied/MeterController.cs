// Decompiled with JetBrains decompiler
// Type: MeterController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class MeterController
{
  public GameObject gameObject;
  private KAnimLink link;

  public KBatchedAnimController meterController { get; private set; }

  public MeterController(
    KMonoBehaviour target,
    Meter.Offset front_back,
    Grid.SceneLayer user_specified_render_layer,
    params string[] symbols_to_hide)
  {
    string[] destinationArray = new string[symbols_to_hide.Length + 1];
    Array.Copy((Array) symbols_to_hide, (Array) destinationArray, symbols_to_hide.Length);
    destinationArray[destinationArray.Length - 1] = "meter_target";
    this.Initialize((KAnimControllerBase) ((Component) target).GetComponent<KBatchedAnimController>(), "meter_target", "meter", front_back, user_specified_render_layer, Vector3.zero, destinationArray);
  }

  public MeterController(
    KAnimControllerBase building_controller,
    string meter_target,
    string meter_animation,
    Meter.Offset front_back,
    Grid.SceneLayer user_specified_render_layer,
    params string[] symbols_to_hide)
  {
    this.Initialize(building_controller, meter_target, meter_animation, front_back, user_specified_render_layer, Vector3.zero, symbols_to_hide);
  }

  public MeterController(
    KAnimControllerBase building_controller,
    string meter_target,
    string meter_animation,
    Meter.Offset front_back,
    Grid.SceneLayer user_specified_render_layer,
    Vector3 tracker_offset,
    params string[] symbols_to_hide)
  {
    this.Initialize(building_controller, meter_target, meter_animation, front_back, user_specified_render_layer, tracker_offset, symbols_to_hide);
  }

  private void Initialize(
    KAnimControllerBase building_controller,
    string meter_target,
    string meter_animation,
    Meter.Offset front_back,
    Grid.SceneLayer user_specified_render_layer,
    Vector3 tracker_offset,
    params string[] symbols_to_hide)
  {
    if (building_controller.HasAnimation(HashedString.op_Implicit(meter_animation + "_cb")) && !GlobalAssets.Instance.colorSet.IsDefaultColorSet())
      meter_animation += "_cb";
    string str = ((Object) building_controller).name + "." + meter_animation;
    GameObject gameObject = Object.Instantiate<GameObject>(Assets.GetPrefab(Tag.op_Implicit(MeterConfig.ID)));
    ((Object) gameObject).name = str;
    gameObject.SetActive(false);
    gameObject.transform.parent = ((Component) building_controller).transform;
    this.gameObject = gameObject;
    gameObject.GetComponent<KPrefabID>().PrefabTag = new Tag(str);
    Vector3 position = TransformExtensions.GetPosition(((Component) building_controller).transform);
    switch (front_back)
    {
      case Meter.Offset.Infront:
        position.z -= 0.1f;
        break;
      case Meter.Offset.Behind:
        position.z += 0.1f;
        break;
      case Meter.Offset.UserSpecified:
        position.z = Grid.GetLayerZ(user_specified_render_layer);
        break;
    }
    TransformExtensions.SetPosition(gameObject.transform, position);
    KBatchedAnimController component1 = gameObject.GetComponent<KBatchedAnimController>();
    component1.AnimFiles = new KAnimFile[1]
    {
      building_controller.AnimFiles[0]
    };
    component1.initialAnim = meter_animation;
    component1.fgLayer = Grid.SceneLayer.NoLayer;
    component1.initialMode = (KAnim.PlayMode) 2;
    component1.isMovable = true;
    component1.FlipX = building_controller.FlipX;
    component1.FlipY = building_controller.FlipY;
    if (Meter.Offset.UserSpecified == front_back)
      component1.sceneLayer = user_specified_render_layer;
    this.meterController = component1;
    KBatchedAnimTracker component2 = gameObject.GetComponent<KBatchedAnimTracker>();
    component2.offset = tracker_offset;
    component2.symbol = new HashedString(meter_target);
    gameObject.SetActive(true);
    building_controller.SetSymbolVisiblity(KAnimHashedString.op_Implicit(meter_target), false);
    if (symbols_to_hide != null)
    {
      for (int index = 0; index < symbols_to_hide.Length; ++index)
        building_controller.SetSymbolVisiblity(KAnimHashedString.op_Implicit(symbols_to_hide[index]), false);
    }
    this.link = new KAnimLink(building_controller, (KAnimControllerBase) component1);
  }

  public MeterController(
    KAnimControllerBase building_controller,
    KBatchedAnimController meter_controller,
    params string[] symbol_names)
  {
    if (Object.op_Equality((Object) meter_controller, (Object) null))
      return;
    this.meterController = meter_controller;
    this.link = new KAnimLink(building_controller, (KAnimControllerBase) meter_controller);
    for (int index = 0; index < symbol_names.Length; ++index)
      building_controller.SetSymbolVisiblity(KAnimHashedString.op_Implicit(symbol_names[index]), false);
    ((Component) this.meterController).GetComponent<KBatchedAnimTracker>().symbol = new HashedString(symbol_names[0]);
  }

  public void SetPositionPercent(float percent_full)
  {
    if (Object.op_Equality((Object) this.meterController, (Object) null))
      return;
    this.meterController.SetPositionPercent(percent_full);
  }

  public void SetSymbolTint(KAnimHashedString symbol, Color32 colour)
  {
    if (!Object.op_Inequality((Object) this.meterController, (Object) null))
      return;
    this.meterController.SetSymbolTint(symbol, Color32.op_Implicit(colour));
  }

  public void SetRotation(float rot)
  {
    if (Object.op_Equality((Object) this.meterController, (Object) null))
      return;
    this.meterController.Rotation = rot;
  }
}
