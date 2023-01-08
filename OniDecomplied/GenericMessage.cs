// Decompiled with JetBrains decompiler
// Type: GenericMessage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using UnityEngine;

public class GenericMessage : Message
{
  [Serialize]
  private string title;
  [Serialize]
  private string tooltip;
  [Serialize]
  private string body;
  [Serialize]
  private Ref<KMonoBehaviour> clickFocus = new Ref<KMonoBehaviour>();

  public GenericMessage(string _title, string _body, string _tooltip, KMonoBehaviour click_focus = null)
  {
    this.title = _title;
    this.body = _body;
    this.tooltip = _tooltip;
    this.clickFocus.Set(click_focus);
  }

  public GenericMessage()
  {
  }

  public override string GetSound() => (string) null;

  public override string GetMessageBody() => this.body;

  public override string GetTooltip() => this.tooltip;

  public override string GetTitle() => this.title;

  public override void OnClick()
  {
    KMonoBehaviour kmonoBehaviour = this.clickFocus.Get();
    if (Object.op_Equality((Object) kmonoBehaviour, (Object) null))
      return;
    Transform transform = kmonoBehaviour.transform;
    if (Object.op_Equality((Object) transform, (Object) null))
      return;
    Vector3 position = TransformExtensions.GetPosition(transform);
    position.z = -40f;
    CameraController.Instance.SetTargetPos(position, 8f, true);
    if (!Object.op_Inequality((Object) ((Component) transform).GetComponent<KSelectable>(), (Object) null))
      return;
    SelectTool.Instance.Select(((Component) transform).GetComponent<KSelectable>());
  }
}
