// Decompiled with JetBrains decompiler
// Type: URLOpenFunction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class URLOpenFunction : MonoBehaviour
{
  [SerializeField]
  private KButton triggerButton;
  [SerializeField]
  private string fixedURL;

  private void Start()
  {
    if (!Object.op_Inequality((Object) this.triggerButton, (Object) null))
      return;
    this.triggerButton.ClearOnClick();
    this.triggerButton.onClick += (System.Action) (() => this.OpenUrl(this.fixedURL));
  }

  public void OpenUrl(string url) => App.OpenWebURL(url);
}
