// Decompiled with JetBrains decompiler
// Type: RestartWarning
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class RestartWarning : MonoBehaviour
{
  public static bool ShouldWarn;
  public LocText text;
  public Image image;

  private void Update()
  {
    if (!RestartWarning.ShouldWarn)
      return;
    ((Behaviour) this.text).enabled = true;
    ((Behaviour) this.image).enabled = true;
  }
}
