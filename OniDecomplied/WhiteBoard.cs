// Decompiled with JetBrains decompiler
// Type: WhiteBoard
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class WhiteBoard : KGameObjectComponentManager<WhiteBoard.Data>, IKComponentManager
{
  public HandleVector<int>.Handle Add(GameObject go) => this.Add(go, new WhiteBoard.Data()
  {
    keyValueStore = new Dictionary<HashedString, object>()
  });

  protected virtual void OnCleanUp(HandleVector<int>.Handle h)
  {
    WhiteBoard.Data data = ((KCompactedVector<WhiteBoard.Data>) this).GetData(h);
    data.keyValueStore.Clear();
    data.keyValueStore = (Dictionary<HashedString, object>) null;
    ((KCompactedVector<WhiteBoard.Data>) this).SetData(h, data);
  }

  public bool HasValue(HandleVector<int>.Handle h, HashedString key) => h.IsValid() && ((KCompactedVector<WhiteBoard.Data>) this).GetData(h).keyValueStore.ContainsKey(key);

  public object GetValue(HandleVector<int>.Handle h, HashedString key) => ((KCompactedVector<WhiteBoard.Data>) this).GetData(h).keyValueStore[key];

  public void SetValue(HandleVector<int>.Handle h, HashedString key, object value)
  {
    if (!h.IsValid())
      return;
    WhiteBoard.Data data = ((KCompactedVector<WhiteBoard.Data>) this).GetData(h);
    data.keyValueStore[key] = value;
    ((KCompactedVector<WhiteBoard.Data>) this).SetData(h, data);
  }

  public void RemoveValue(HandleVector<int>.Handle h, HashedString key)
  {
    if (!h.IsValid())
      return;
    WhiteBoard.Data data = ((KCompactedVector<WhiteBoard.Data>) this).GetData(h);
    data.keyValueStore.Remove(key);
    ((KCompactedVector<WhiteBoard.Data>) this).SetData(h, data);
  }

  public struct Data
  {
    public Dictionary<HashedString, object> keyValueStore;
  }
}
