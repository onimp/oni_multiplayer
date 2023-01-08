// Decompiled with JetBrains decompiler
// Type: CellEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class CellEvent : EventBase
{
  public string reason;
  public bool isSend;
  public bool enableLogging;

  public CellEvent(string id, string reason, bool is_send, bool enable_logging = true)
    : base(id)
  {
    this.reason = reason;
    this.isSend = is_send;
    this.enableLogging = enable_logging;
  }

  public string GetMessagePrefix() => this.isSend ? ">>>: " : "<<<: ";
}
