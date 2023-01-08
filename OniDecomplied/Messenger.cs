// Decompiled with JetBrains decompiler
// Type: Messenger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Messenger")]
public class Messenger : KMonoBehaviour
{
  [Serialize]
  private SerializedList<Message> messages = new SerializedList<Message>();
  public static Messenger Instance;

  public int Count => this.messages.Count;

  public IEnumerator<Message> GetEnumerator() => this.messages.GetEnumerator();

  public static void DestroyInstance() => Messenger.Instance = (Messenger) null;

  public SerializedList<Message> Messages => this.messages;

  protected virtual void OnPrefabInit() => Messenger.Instance = this;

  protected virtual void OnSpawn()
  {
    int idx = 0;
    while (idx < this.messages.Count)
    {
      if (this.messages[idx].IsValid())
        ++idx;
      else
        this.messages.RemoveAt(idx);
    }
    this.Trigger(-599791736, (object) null);
  }

  public void QueueMessage(Message message)
  {
    this.messages.Add(message);
    this.Trigger(1558809273, (object) message);
  }

  public Message DequeueMessage()
  {
    Message message = (Message) null;
    if (this.messages.Count > 0)
    {
      message = this.messages[0];
      this.messages.RemoveAt(0);
    }
    return message;
  }

  public void ClearAllMessages()
  {
    for (int idx = this.messages.Count - 1; idx >= 0; --idx)
      this.messages.RemoveAt(idx);
  }

  public void RemoveMessage(Message m)
  {
    this.messages.Remove(m);
    this.Trigger(-599791736, (object) null);
  }
}
