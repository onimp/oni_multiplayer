// Decompiled with JetBrains decompiler
// Type: Message
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;

[SerializationConfig]
public abstract class Message : ISaveLoadable
{
  public abstract string GetTitle();

  public abstract string GetSound();

  public abstract string GetMessageBody();

  public abstract string GetTooltip();

  public virtual bool ShowDialog() => true;

  public virtual void OnCleanUp()
  {
  }

  public virtual bool IsValid() => true;

  public virtual bool PlayNotificationSound() => true;

  public virtual void OnClick()
  {
  }
}
