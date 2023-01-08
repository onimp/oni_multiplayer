// Decompiled with JetBrains decompiler
// Type: SimpleUIShowHide
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SimpleUIShowHide")]
public class SimpleUIShowHide : KMonoBehaviour
{
  [MyCmpReq]
  private MultiToggle toggle;
  [SerializeField]
  public GameObject content;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.toggle.onClick += new System.Action(this.OnClick);
  }

  private void OnClick()
  {
    this.toggle.NextState();
    this.content.SetActive(this.toggle.CurrentState == 0);
  }
}
