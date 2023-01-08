// Decompiled with JetBrains decompiler
// Type: EventSystem2Syntax.KMonoBehaviour2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

namespace EventSystem2Syntax
{
  internal class KMonoBehaviour2
  {
    protected virtual void OnPrefabInit()
    {
    }

    public void Subscribe(int evt, Action<object> cb)
    {
    }

    public void Trigger(int evt, object data)
    {
    }

    public void Subscribe<ListenerType, EventType>(Action<ListenerType, EventType> cb) where EventType : IEventData
    {
    }

    public void Trigger<EventType>(EventType evt) where EventType : IEventData
    {
    }
  }
}
