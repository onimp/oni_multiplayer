// Decompiled with JetBrains decompiler
// Type: MainCamera
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class MainCamera : MonoBehaviour
{
  private void Awake()
  {
    if (Object.op_Inequality((Object) Camera.main, (Object) null))
      Object.Destroy((Object) ((Component) Camera.main).gameObject);
    ((Component) this).gameObject.tag = nameof (MainCamera);
  }
}
