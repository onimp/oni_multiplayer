// Decompiled with JetBrains decompiler
// Type: EventLogger`2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System.Collections.Generic;

[SerializationConfig]
public class EventLogger<EventInstanceType, EventType> : KMonoBehaviour, ISaveLoadable
  where EventInstanceType : EventInstanceBase
  where EventType : EventBase
{
  private const int MAX_NUM_EVENTS = 10000;
  private List<EventType> Events = new List<EventType>();
  [Serialize]
  private List<EventInstanceType> EventInstances = new List<EventInstanceType>();

  public IEnumerator<EventInstanceType> GetEnumerator() => (IEnumerator<EventInstanceType>) this.EventInstances.GetEnumerator();

  public EventType AddEvent(EventType ev)
  {
    for (int index = 0; index < this.Events.Count; ++index)
    {
      if (this.Events[index].hash == ev.hash)
      {
        this.Events[index] = ev;
        return this.Events[index];
      }
    }
    this.Events.Add(ev);
    return ev;
  }

  public EventInstanceType Add(EventInstanceType ev)
  {
    if (this.EventInstances.Count > 10000)
      this.EventInstances.RemoveAt(0);
    this.EventInstances.Add(ev);
    return ev;
  }

  [System.Runtime.Serialization.OnDeserialized]
  protected internal void OnDeserialized()
  {
    if (this.EventInstances.Count > 10000)
      this.EventInstances.RemoveRange(0, this.EventInstances.Count - 10000);
    for (int index1 = 0; index1 < this.EventInstances.Count; ++index1)
    {
      for (int index2 = 0; index2 < this.Events.Count; ++index2)
      {
        if (this.Events[index2].hash == this.EventInstances[index1].eventHash)
        {
          this.EventInstances[index1].ev = (EventBase) this.Events[index2];
          break;
        }
      }
    }
  }

  public void Clear() => this.EventInstances.Clear();
}
