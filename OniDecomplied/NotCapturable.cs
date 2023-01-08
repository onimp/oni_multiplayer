// Decompiled with JetBrains decompiler
// Type: NotCapturable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/NotCapturable")]
public class NotCapturable : KMonoBehaviour
{
  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    if (Object.op_Inequality((Object) ((Component) this).GetComponent<Capturable>(), (Object) null))
      DebugUtil.LogErrorArgs((Object) this, new object[1]
      {
        (object) "Entity has both Capturable and NotCapturable!"
      });
    Components.NotCapturables.Add(this);
  }

  protected virtual void OnCleanUp()
  {
    Components.NotCapturables.Remove(this);
    base.OnCleanUp();
  }
}
