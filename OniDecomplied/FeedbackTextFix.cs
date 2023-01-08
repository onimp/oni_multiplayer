// Decompiled with JetBrains decompiler
// Type: FeedbackTextFix
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Steamworks;
using UnityEngine;

public class FeedbackTextFix : MonoBehaviour
{
  public string newKey;
  public LocText locText;

  private void Awake()
  {
    if (!DistributionPlatform.Initialized || !SteamUtils.IsSteamRunningOnSteamDeck())
      Object.DestroyImmediate((Object) this);
    else
      this.locText.key = this.newKey;
  }
}
