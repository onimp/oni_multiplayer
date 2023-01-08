// Decompiled with JetBrains decompiler
// Type: CodexUnlockedMessage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

public class CodexUnlockedMessage : Message
{
  private string unlockMessage;
  private string lockId;

  public CodexUnlockedMessage()
  {
  }

  public CodexUnlockedMessage(string lock_id, string unlock_message)
  {
    this.lockId = lock_id;
    this.unlockMessage = unlock_message;
  }

  public string GetLockId() => this.lockId;

  public override string GetSound() => "AI_Notification_ResearchComplete";

  public override string GetMessageBody() => UI.CODEX.CODEX_DISCOVERED_MESSAGE.BODY.Replace("{codex}", this.unlockMessage);

  public override string GetTitle() => (string) UI.CODEX.CODEX_DISCOVERED_MESSAGE.TITLE;

  public override string GetTooltip() => this.GetMessageBody();

  public override bool IsValid() => true;
}
