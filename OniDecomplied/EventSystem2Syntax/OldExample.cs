// Decompiled with JetBrains decompiler
// Type: EventSystem2Syntax.OldExample
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

namespace EventSystem2Syntax
{
  internal class OldExample : KMonoBehaviour2
  {
    protected override void OnPrefabInit()
    {
      base.OnPrefabInit();
      this.Subscribe(0, new Action<object>(this.OnObjectDestroyed));
      this.Trigger(0, (object) false);
    }

    private void OnObjectDestroyed(object data) => Debug.Log((object) (bool) data);
  }
}
