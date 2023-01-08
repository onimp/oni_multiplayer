// Decompiled with JetBrains decompiler
// Type: DuplicantsLeftMessage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

public class DuplicantsLeftMessage : Message
{
  public override string GetSound() => "";

  public override string GetTitle() => (string) MISC.NOTIFICATIONS.DUPLICANTABSORBED.NAME;

  public override string GetMessageBody() => (string) MISC.NOTIFICATIONS.DUPLICANTABSORBED.MESSAGEBODY;

  public override string GetTooltip() => (string) MISC.NOTIFICATIONS.DUPLICANTABSORBED.TOOLTIP;
}
