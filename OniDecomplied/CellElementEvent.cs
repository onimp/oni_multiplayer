// Decompiled with JetBrains decompiler
// Type: CellElementEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Diagnostics;

public class CellElementEvent : CellEvent
{
  public CellElementEvent(string id, string reason, bool is_send, bool enable_logging = true)
    : base(id, reason, is_send, enable_logging)
  {
  }

  [Conditional("ENABLE_CELL_EVENT_LOGGER")]
  public void Log(int cell, SimHashes element, int callback_id)
  {
    if (!this.enableLogging)
      return;
    CellEventInstance ev = new CellEventInstance(cell, (int) element, 0, (CellEvent) this);
    CellEventLogger.Instance.Add(ev);
  }

  public override string GetDescription(EventInstanceBase ev)
  {
    SimHashes data = (SimHashes) (ev as CellEventInstance).data;
    return this.GetMessagePrefix() + "Element=" + data.ToString() + " (" + this.reason + ")";
  }
}
