// Decompiled with JetBrains decompiler
// Type: WorldDetectedMessage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using UnityEngine;

public class WorldDetectedMessage : Message
{
  [Serialize]
  private int worldID;

  public WorldDetectedMessage()
  {
  }

  public WorldDetectedMessage(WorldContainer world) => this.worldID = world.id;

  public override string GetSound() => "AI_Notification_ResearchComplete";

  public override string GetMessageBody()
  {
    WorldContainer world = ClusterManager.Instance.GetWorld(this.worldID);
    return string.Format((string) MISC.NOTIFICATIONS.WORLDDETECTED.MESSAGEBODY, (object) ((Component) world).GetProperName());
  }

  public override string GetTitle() => (string) MISC.NOTIFICATIONS.WORLDDETECTED.NAME;

  public override string GetTooltip()
  {
    WorldContainer world = ClusterManager.Instance.GetWorld(this.worldID);
    return string.Format((string) MISC.NOTIFICATIONS.WORLDDETECTED.TOOLTIP, (object) ((Component) world).GetProperName());
  }

  public override bool IsValid() => this.worldID != (int) ClusterManager.INVALID_WORLD_IDX;
}
