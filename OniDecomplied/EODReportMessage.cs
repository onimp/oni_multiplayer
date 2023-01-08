// Decompiled with JetBrains decompiler
// Type: EODReportMessage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;

public class EODReportMessage : Message
{
  [Serialize]
  private int day;
  [Serialize]
  private string title;
  [Serialize]
  private string tooltip;

  public EODReportMessage(string title, string tooltip)
  {
    this.day = GameUtil.GetCurrentCycle();
    this.title = title;
    this.tooltip = tooltip;
  }

  public EODReportMessage()
  {
  }

  public override string GetSound() => (string) null;

  public override string GetMessageBody() => "";

  public override string GetTooltip() => this.tooltip;

  public override string GetTitle() => this.title;

  public void OpenReport() => ManagementMenu.Instance.OpenReports(this.day);

  public override bool ShowDialog() => false;

  public override void OnClick() => this.OpenReport();
}
