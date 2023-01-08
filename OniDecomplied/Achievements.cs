// Decompiled with JetBrains decompiler
// Type: Achievements
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Achievements")]
public class Achievements : KMonoBehaviour
{
  public void Unlock(string id)
  {
    if (!Object.op_Implicit((Object) SteamAchievementService.Instance))
      return;
    SteamAchievementService.Instance.Unlock(id);
  }
}
