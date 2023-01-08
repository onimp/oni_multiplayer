// Decompiled with JetBrains decompiler
// Type: MessageDialog
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public abstract class MessageDialog : KMonoBehaviour
{
  public virtual bool CanDontShowAgain => false;

  public abstract bool CanDisplay(Message message);

  public abstract void SetMessage(Message message);

  public abstract void OnClickAction();

  public virtual void OnDontShowAgain()
  {
  }
}
