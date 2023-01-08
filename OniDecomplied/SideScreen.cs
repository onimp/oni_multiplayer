// Decompiled with JetBrains decompiler
// Type: SideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class SideScreen : KScreen
{
  [SerializeField]
  private GameObject contentBody;

  public void SetContent(SideScreenContent sideScreenContent, GameObject target)
  {
    if (Object.op_Inequality((Object) ((KMonoBehaviour) sideScreenContent).transform.parent, (Object) this.contentBody.transform))
      ((KMonoBehaviour) sideScreenContent).transform.SetParent(this.contentBody.transform);
    sideScreenContent.SetTarget(target);
  }
}
