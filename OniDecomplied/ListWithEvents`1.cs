// Decompiled with JetBrains decompiler
// Type: ListWithEvents`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;

public class ListWithEvents<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
  private List<T> internalList = new List<T>();

  public int Count => this.internalList.Count;

  public bool IsReadOnly => ((ICollection<T>) this.internalList).IsReadOnly;

  public event Action<T> onAdd;

  public event Action<T> onRemove;

  public T this[int index]
  {
    get => this.internalList[index];
    set => this.internalList[index] = value;
  }

  public void Add(T item)
  {
    this.internalList.Add(item);
    if (this.onAdd == null)
      return;
    this.onAdd(item);
  }

  public void Insert(int index, T item)
  {
    this.internalList.Insert(index, item);
    if (this.onAdd == null)
      return;
    this.onAdd(item);
  }

  public void RemoveAt(int index)
  {
    T obj = this.internalList[index];
    this.internalList.RemoveAt(index);
    if (this.onRemove == null)
      return;
    this.onRemove(obj);
  }

  public bool Remove(T item)
  {
    int num = this.internalList.Remove(item) ? 1 : 0;
    if (num == 0)
      return num != 0;
    if (this.onRemove == null)
      return num != 0;
    this.onRemove(item);
    return num != 0;
  }

  public void Clear()
  {
    while (this.Count > 0)
      this.RemoveAt(0);
  }

  public int IndexOf(T item) => this.internalList.IndexOf(item);

  public void CopyTo(T[] array, int arrayIndex) => this.internalList.CopyTo(array, arrayIndex);

  public bool Contains(T item) => this.internalList.Contains(item);

  public IEnumerator<T> GetEnumerator() => (IEnumerator<T>) this.internalList.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.internalList.GetEnumerator();
}
