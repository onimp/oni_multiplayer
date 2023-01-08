// Decompiled with JetBrains decompiler
// Type: EventSystem2Syntax.NewExample
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

namespace EventSystem2Syntax
{
  internal class NewExample : KMonoBehaviour2
  {
    protected override void OnPrefabInit()
    {
      this.Subscribe<NewExample, NewExample.ObjectDestroyedEvent>(new Action<NewExample, NewExample.ObjectDestroyedEvent>(NewExample.OnObjectDestroyed));
      this.Trigger<NewExample.ObjectDestroyedEvent>(new NewExample.ObjectDestroyedEvent()
      {
        parameter = false
      });
    }

    private static void OnObjectDestroyed(NewExample example, NewExample.ObjectDestroyedEvent evt)
    {
    }

    private struct ObjectDestroyedEvent : IEventData
    {
      public bool parameter;
    }
  }
}
