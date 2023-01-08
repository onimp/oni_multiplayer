// Decompiled with JetBrains decompiler
// Type: ResearchCompleteMessage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;

public class ResearchCompleteMessage : Message
{
  [Serialize]
  private ResourceRef<Tech> tech = new ResourceRef<Tech>();

  public ResearchCompleteMessage()
  {
  }

  public ResearchCompleteMessage(Tech tech) => this.tech.Set(tech);

  public override string GetSound() => "AI_Notification_ResearchComplete";

  public override string GetMessageBody()
  {
    Tech tech = this.tech.Get();
    string str = "";
    for (int index = 0; index < tech.unlockedItems.Count; ++index)
    {
      if (index != 0)
        str += ", ";
      str += tech.unlockedItems[index].Name;
    }
    return string.Format((string) MISC.NOTIFICATIONS.RESEARCHCOMPLETE.MESSAGEBODY, (object) tech.Name, (object) str);
  }

  public override string GetTitle() => (string) MISC.NOTIFICATIONS.RESEARCHCOMPLETE.NAME;

  public override string GetTooltip()
  {
    Tech tech = this.tech.Get();
    return string.Format((string) MISC.NOTIFICATIONS.RESEARCHCOMPLETE.TOOLTIP, (object) tech.Name);
  }

  public override bool IsValid() => this.tech.Get() != null;
}
