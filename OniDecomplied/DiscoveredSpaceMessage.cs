// Decompiled with JetBrains decompiler
// Type: DiscoveredSpaceMessage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using UnityEngine;

public class DiscoveredSpaceMessage : Message
{
  [Serialize]
  private Vector3 cameraFocusPos;
  private const string MUSIC_STINGER = "Stinger_Surface";

  public DiscoveredSpaceMessage()
  {
  }

  public DiscoveredSpaceMessage(Vector3 pos)
  {
    this.cameraFocusPos = pos;
    this.cameraFocusPos.z = -40f;
  }

  public override string GetSound() => "Discover_Space";

  public override string GetMessageBody() => (string) MISC.NOTIFICATIONS.DISCOVERED_SPACE.TOOLTIP;

  public override string GetTitle() => (string) MISC.NOTIFICATIONS.DISCOVERED_SPACE.NAME;

  public override string GetTooltip() => (string) null;

  public override bool IsValid() => true;

  public override void OnClick() => this.OnDiscoveredSpaceClicked();

  private void OnDiscoveredSpaceClicked()
  {
    KFMOD.PlayUISound(GlobalAssets.GetSound(this.GetSound()));
    MusicManager.instance.PlaySong("Stinger_Surface");
    CameraController.Instance.SetTargetPos(this.cameraFocusPos, 8f, true);
  }
}
