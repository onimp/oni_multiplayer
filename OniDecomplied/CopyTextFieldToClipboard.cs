// Decompiled with JetBrains decompiler
// Type: CopyTextFieldToClipboard
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/CopyTextFieldToClipboard")]
public class CopyTextFieldToClipboard : KMonoBehaviour
{
  public KButton button;
  public Func<string> GetText;

  protected virtual void OnPrefabInit() => this.button.onClick += new System.Action(this.OnClick);

  private void OnClick()
  {
    TextEditor textEditor = new TextEditor();
    textEditor.text = this.GetText();
    textEditor.SelectAll();
    textEditor.Copy();
  }
}
