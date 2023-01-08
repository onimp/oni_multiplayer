// Decompiled with JetBrains decompiler
// Type: CellSolidFilterEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Diagnostics;

public class CellSolidFilterEvent : CellEvent
{
  public CellSolidFilterEvent(string id, bool enable_logging = true)
    : base(id, "filtered", false, enable_logging)
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

  public override string GetDescription(EventInstanceBase ev)
  {
    CellEventInstance cellEventInstance = ev as CellEventInstance;
    return this.GetMessagePrefix() + "Filtered Solid Event solid=" + cellEventInstance.data.ToString();
  }
}
