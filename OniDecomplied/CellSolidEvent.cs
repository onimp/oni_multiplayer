// Decompiled with JetBrains decompiler
// Type: CellSolidEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Diagnostics;

public class CellSolidEvent : CellEvent
{
  public CellSolidEvent(string id, string reason, bool is_send, bool enable_logging = true)
    : base(id, reason, is_send, enable_logging)
  {
  }

  [Conditional("ENABLE_CELL_EVENT_LOGGER")]
  public void Log(int cell, bool solid)
  {
    if (!this.enableLogging)
      return;
    CellEventInstance ev = new CellEventInstance(cell, solid ? 1 : 0, 0, (CellEvent) this);
    CellEventLogger.Instance.Add(ev);
  }

  public override string GetDescription(EventInstanceBase ev) => (ev as CellEventInstance).data == 1 ? this.GetMessagePrefix() + "Solid=true (" + this.reason + ")" : this.GetMessagePrefix() + "Solid=false (" + this.reason + ")";
}
