// Decompiled with JetBrains decompiler
// Type: MinionGroupProber
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/MinionGroupProber")]
public class MinionGroupProber : KMonoBehaviour, IGroupProber
{
  private static MinionGroupProber Instance;
  private Dictionary<object, short>[] cells;
  private Dictionary<object, KeyValuePair<short, short>> valid_serial_nos = new Dictionary<object, KeyValuePair<short, short>>();
  private List<object> pending_removals = new List<object>();
  private readonly object access = new object();

  public static void DestroyInstance() => MinionGroupProber.Instance = (MinionGroupProber) null;

  public static MinionGroupProber Get() => MinionGroupProber.Instance;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    MinionGroupProber.Instance = this;
    this.cells = new Dictionary<object, short>[Grid.CellCount];
  }

  private bool IsReachable_AssumeLock(int cell)
  {
    if (!Grid.IsValidCell(cell))
      return false;
    Dictionary<object, short> cell1 = this.cells[cell];
    if (cell1 == null)
      return false;
    bool flag = false;
    foreach (KeyValuePair<object, short> keyValuePair1 in cell1)
    {
      object key = keyValuePair1.Key;
      short num = keyValuePair1.Value;
      KeyValuePair<short, short> keyValuePair2;
      if (this.valid_serial_nos.TryGetValue(key, out keyValuePair2) && ((int) num == (int) keyValuePair2.Key || (int) num == (int) keyValuePair2.Value))
      {
        flag = true;
        break;
      }
      this.pending_removals.Add(key);
    }
    foreach (object pendingRemoval in this.pending_removals)
    {
      cell1.Remove(pendingRemoval);
      if (cell1.Count == 0)
        this.cells[cell] = (Dictionary<object, short>) null;
    }
    this.pending_removals.Clear();
    return flag;
  }

  public bool IsReachable(int cell)
  {
    if (!Grid.IsValidCell(cell))
      return false;
    lock (this.access)
      return this.IsReachable_AssumeLock(cell);
  }

  public bool IsReachable(int cell, CellOffset[] offsets)
  {
    if (!Grid.IsValidCell(cell))
      return false;
    bool flag = false;
    lock (this.access)
    {
      foreach (CellOffset offset in offsets)
      {
        if (this.IsReachable_AssumeLock(Grid.OffsetCell(cell, offset)))
        {
          flag = true;
          break;
        }
      }
    }
    return flag;
  }

  public bool IsAllReachable(int cell, CellOffset[] offsets)
  {
    if (!Grid.IsValidCell(cell))
      return false;
    bool flag = false;
    lock (this.access)
    {
      if (this.IsReachable_AssumeLock(cell))
      {
        flag = true;
      }
      else
      {
        foreach (CellOffset offset in offsets)
        {
          if (this.IsReachable_AssumeLock(Grid.OffsetCell(cell, offset)))
          {
            flag = true;
            break;
          }
        }
      }
    }
    return flag;
  }

  public bool IsReachable(Workable workable) => this.IsReachable(Grid.PosToCell((KMonoBehaviour) workable), workable.GetOffsets());

  public void Occupy(object prober, short serial_no, IEnumerable<int> cells)
  {
    lock (this.access)
    {
      foreach (int cell in cells)
      {
        if (this.cells[cell] == null)
          this.cells[cell] = new Dictionary<object, short>();
        this.cells[cell][prober] = serial_no;
      }
    }
  }

  public void SetValidSerialNos(object prober, short previous_serial_no, short serial_no)
  {
    lock (this.access)
      this.valid_serial_nos[prober] = new KeyValuePair<short, short>(previous_serial_no, serial_no);
  }

  public bool ReleaseProber(object prober)
  {
    lock (this.access)
      return this.valid_serial_nos.Remove(prober);
  }
}
