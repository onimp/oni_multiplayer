// Decompiled with JetBrains decompiler
// Type: TargetMessageDialog
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TMPro;
using UnityEngine;

public class TargetMessageDialog : MessageDialog
{
  [SerializeField]
  private LocText description;
  private TargetMessage message;

  public override bool CanDisplay(Message message) => typeof (TargetMessage).IsAssignableFrom(message.GetType());

  public override void SetMessage(Message base_message)
  {
    this.message = (TargetMessage) base_message;
    ((TMP_Text) this.description).text = this.message.GetMessageBody();
  }

  public override void OnClickAction()
  {
    MessageTarget target = this.message.GetTarget();
    SelectTool.Instance.SelectAndFocus(target.GetPosition(), target.GetSelectable());
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    this.message.OnCleanUp();
  }
}
