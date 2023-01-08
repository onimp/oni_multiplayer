// Decompiled with JetBrains decompiler
// Type: VideoOverlay
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TMPro;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/VideoOverlay")]
public class VideoOverlay : KMonoBehaviour
{
  public List<LocText> textFields;

  public void SetText(List<string> strings)
  {
    if (strings.Count != this.textFields.Count)
      DebugUtil.LogErrorArgs(new object[5]
      {
        (object) ((Object) this).name,
        (object) "expects",
        (object) this.textFields.Count,
        (object) "strings passed to it, got",
        (object) strings.Count
      });
    for (int index = 0; index < this.textFields.Count; ++index)
      ((TMP_Text) this.textFields[index]).text = strings[index];
  }
}
