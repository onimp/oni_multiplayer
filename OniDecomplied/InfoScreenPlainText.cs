// Decompiled with JetBrains decompiler
// Type: InfoScreenPlainText
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TMPro;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/InfoScreenPlainText")]
public class InfoScreenPlainText : KMonoBehaviour
{
  [SerializeField]
  private LocText locText;

  public void SetText(string text) => ((TMP_Text) this.locText).text = text;
}
