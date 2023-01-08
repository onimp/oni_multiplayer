// Decompiled with JetBrains decompiler
// Type: CellModifyMassEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Diagnostics;

public class CellModifyMassEvent : CellEvent
{
  public CellModifyMassEvent(string id, string reason, bool enable_logging = false)
    : base(id, reason, true, enable_logging)
  {
  }

  [Conditional("ENABLE_CELL_EVENT_LOGGER")]
  public void Log(int cell, SimHashes element, float amount)
  {
    if (!this.enableLogging)
      return;
    CellEventInstance ev = new CellEventInstance(cell, (int) element, (int) ((double) amount * 1000.0), (CellEvent) this);
    CellEventLogger.Instance.Add(ev);
  }

  public override string GetDescription(EventInstanceBase ev)
  {
    CellEventInstance cellEventInstance = ev as CellEventInstance;
    SimHashes data = (SimHashes) cellEventInstance.data;
    return this.GetMessagePrefix() + "Element=" + data.ToString() + ", Mass=" + ((float) cellEventInstance.data2 / 1000f).ToString() + " (" + this.reason + ")";
  }
}
