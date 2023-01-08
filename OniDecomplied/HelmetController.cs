// Decompiled with JetBrains decompiler
// Type: HelmetController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/HelmetController")]
public class HelmetController : KMonoBehaviour
{
  public string anim_file;
  public bool has_jets;
  private bool is_shown;
  private bool in_tube;
  private bool is_flying;
  private Navigator owner_navigator;
  private GameObject jet_go;
  private GameObject glow_go;
  private static readonly EventSystem.IntraObjectHandler<HelmetController> OnEquippedDelegate = new EventSystem.IntraObjectHandler<HelmetController>((Action<HelmetController, object>) ((component, data) => component.OnEquipped(data)));
  private static readonly EventSystem.IntraObjectHandler<HelmetController> OnUnequippedDelegate = new EventSystem.IntraObjectHandler<HelmetController>((Action<HelmetController, object>) ((component, data) => component.OnUnequipped(data)));

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<HelmetController>(-1617557748, HelmetController.OnEquippedDelegate);
    this.Subscribe<HelmetController>(-170173755, HelmetController.OnUnequippedDelegate);
  }

  private KBatchedAnimController GetAssigneeController()
  {
    Equippable component = ((Component) this).GetComponent<Equippable>();
    if (component.assignee != null)
    {
      GameObject assigneeGameObject = this.GetAssigneeGameObject(component.assignee);
      if (Object.op_Implicit((Object) assigneeGameObject))
        return assigneeGameObject.GetComponent<KBatchedAnimController>();
    }
    return (KBatchedAnimController) null;
  }

  private GameObject GetAssigneeGameObject(IAssignableIdentity ass_id)
  {
    GameObject assigneeGameObject = (GameObject) null;
    MinionAssignablesProxy assignablesProxy = ass_id as MinionAssignablesProxy;
    if (Object.op_Implicit((Object) assignablesProxy))
    {
      assigneeGameObject = assignablesProxy.GetTargetGameObject();
    }
    else
    {
      MinionIdentity minionIdentity = ass_id as MinionIdentity;
      if (Object.op_Implicit((Object) minionIdentity))
        assigneeGameObject = ((Component) minionIdentity).gameObject;
    }
    return assigneeGameObject;
  }

  private void OnEquipped(object data)
  {
    Equippable component = ((Component) this).GetComponent<Equippable>();
    this.ShowHelmet();
    GameObject assigneeGameObject = this.GetAssigneeGameObject(component.assignee);
    KMonoBehaviourExtensions.Subscribe(assigneeGameObject, 961737054, new Action<object>(this.OnBeginRecoverBreath));
    KMonoBehaviourExtensions.Subscribe(assigneeGameObject, -2037519664, new Action<object>(this.OnEndRecoverBreath));
    KMonoBehaviourExtensions.Subscribe(assigneeGameObject, 1347184327, new Action<object>(this.OnPathAdvanced));
    this.in_tube = false;
    this.is_flying = false;
    this.owner_navigator = assigneeGameObject.GetComponent<Navigator>();
  }

  private void OnUnequipped(object data)
  {
    this.owner_navigator = (Navigator) null;
    Equippable component = ((Component) this).GetComponent<Equippable>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    this.HideHelmet();
    if (component.assignee == null)
      return;
    GameObject assigneeGameObject = this.GetAssigneeGameObject(component.assignee);
    if (!Object.op_Implicit((Object) assigneeGameObject))
      return;
    KMonoBehaviourExtensions.Unsubscribe(assigneeGameObject, 961737054, new Action<object>(this.OnBeginRecoverBreath));
    KMonoBehaviourExtensions.Unsubscribe(assigneeGameObject, -2037519664, new Action<object>(this.OnEndRecoverBreath));
    KMonoBehaviourExtensions.Unsubscribe(assigneeGameObject, 1347184327, new Action<object>(this.OnPathAdvanced));
  }

  private void ShowHelmet()
  {
    KBatchedAnimController assigneeController = this.GetAssigneeController();
    if (Object.op_Equality((Object) assigneeController, (Object) null))
      return;
    KAnimHashedString symbol;
    // ISSUE: explicit constructor call
    ((KAnimHashedString) ref symbol).\u002Ector("snapTo_neck");
    if (!string.IsNullOrEmpty(this.anim_file))
    {
      KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit(this.anim_file));
      ((Component) assigneeController).GetComponent<SymbolOverrideController>().AddSymbolOverride(HashedString.op_Implicit(symbol), anim.GetData().build.GetSymbol(symbol), 6);
    }
    assigneeController.SetSymbolVisiblity(symbol, true);
    this.is_shown = true;
    this.UpdateJets();
  }

  private void HideHelmet()
  {
    this.is_shown = false;
    KBatchedAnimController assigneeController = this.GetAssigneeController();
    if (Object.op_Equality((Object) assigneeController, (Object) null))
      return;
    KAnimHashedString symbol = KAnimHashedString.op_Implicit("snapTo_neck");
    if (!string.IsNullOrEmpty(this.anim_file))
    {
      SymbolOverrideController component = ((Component) assigneeController).GetComponent<SymbolOverrideController>();
      if (Object.op_Equality((Object) component, (Object) null))
        return;
      component.RemoveSymbolOverride(HashedString.op_Implicit(symbol), 6);
    }
    assigneeController.SetSymbolVisiblity(symbol, false);
    this.UpdateJets();
  }

  private void UpdateJets()
  {
    if (this.is_shown && this.is_flying)
      this.EnableJets();
    else
      this.DisableJets();
  }

  private void EnableJets()
  {
    if (!this.has_jets || Object.op_Implicit((Object) this.jet_go))
      return;
    this.jet_go = this.AddTrackedAnim("jet", Assets.GetAnim(HashedString.op_Implicit("jetsuit_thruster_fx_kanim")), "loop", Grid.SceneLayer.Creatures, "snapTo_neck");
    this.glow_go = this.AddTrackedAnim("glow", Assets.GetAnim(HashedString.op_Implicit("jetsuit_thruster_glow_fx_kanim")), "loop", Grid.SceneLayer.Front, "snapTo_neck");
  }

  private void DisableJets()
  {
    if (!this.has_jets)
      return;
    Object.Destroy((Object) this.jet_go);
    this.jet_go = (GameObject) null;
    Object.Destroy((Object) this.glow_go);
    this.glow_go = (GameObject) null;
  }

  private GameObject AddTrackedAnim(
    string name,
    KAnimFile tracked_anim_file,
    string anim_clip,
    Grid.SceneLayer layer,
    string symbol_name)
  {
    KBatchedAnimController assigneeController = this.GetAssigneeController();
    if (Object.op_Equality((Object) assigneeController, (Object) null))
      return (GameObject) null;
    string str = ((Object) assigneeController).name + "." + name;
    GameObject gameObject = new GameObject(str);
    gameObject.SetActive(false);
    gameObject.transform.parent = ((Component) assigneeController).transform;
    gameObject.AddComponent<KPrefabID>().PrefabTag = new Tag(str);
    KBatchedAnimController kbatchedAnimController = gameObject.AddComponent<KBatchedAnimController>();
    kbatchedAnimController.AnimFiles = new KAnimFile[1]
    {
      tracked_anim_file
    };
    kbatchedAnimController.initialAnim = anim_clip;
    kbatchedAnimController.isMovable = true;
    kbatchedAnimController.sceneLayer = layer;
    gameObject.AddComponent<KBatchedAnimTracker>().symbol = HashedString.op_Implicit(symbol_name);
    Matrix4x4 symbolTransform = assigneeController.GetSymbolTransform(HashedString.op_Implicit(symbol_name), out bool _);
    Vector3 vector3 = Vector4.op_Implicit(((Matrix4x4) ref symbolTransform).GetColumn(3));
    vector3.z = Grid.GetLayerZ(layer);
    TransformExtensions.SetPosition(gameObject.transform, vector3);
    gameObject.SetActive(true);
    kbatchedAnimController.Play(HashedString.op_Implicit(anim_clip), (KAnim.PlayMode) 0);
    return gameObject;
  }

  private void OnBeginRecoverBreath(object data) => this.HideHelmet();

  private void OnEndRecoverBreath(object data) => this.ShowHelmet();

  private void OnPathAdvanced(object data)
  {
    if (Object.op_Equality((Object) this.owner_navigator, (Object) null))
      return;
    bool flag1 = this.owner_navigator.CurrentNavType == NavType.Hover;
    bool flag2 = this.owner_navigator.CurrentNavType == NavType.Tube;
    if (flag2 != this.in_tube)
    {
      this.in_tube = flag2;
      if (this.in_tube)
        this.HideHelmet();
      else
        this.ShowHelmet();
    }
    if (flag1 == this.is_flying)
      return;
    this.is_flying = flag1;
    this.UpdateJets();
  }
}
