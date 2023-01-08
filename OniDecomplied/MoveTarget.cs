// Decompiled with JetBrains decompiler
// Type: MoveTarget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/MoveTarget")]
public class MoveTarget : KMonoBehaviour
{
  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    ((Object) ((Component) this).gameObject).hideFlags = (HideFlags) 63;
  }
}
