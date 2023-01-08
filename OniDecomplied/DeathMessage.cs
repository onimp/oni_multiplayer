// Decompiled with JetBrains decompiler
// Type: DeathMessage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using UnityEngine;

public class DeathMessage : TargetMessage
{
  [Serialize]
  private ResourceRef<Death> death = new ResourceRef<Death>();

  public DeathMessage()
  {
  }

  public DeathMessage(GameObject go, Death death)
    : base(go.GetComponent<KPrefabID>())
  {
    this.death.Set(death);
  }

  public override string GetSound() => "";

  public override bool PlayNotificationSound() => false;

  public override string GetTitle() => (string) MISC.NOTIFICATIONS.DUPLICANTDIED.NAME;

  public override string GetTooltip() => this.GetMessageBody();

  public override string GetMessageBody() => this.death.Get().description.Replace("{Target}", this.GetTarget().GetName());
}
