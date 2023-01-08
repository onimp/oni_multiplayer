// Decompiled with JetBrains decompiler
// Type: ScenePartitionerEntry
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

public class ScenePartitionerEntry
{
  public int x;
  public int y;
  public int width;
  public int height;
  public int layer;
  public int queryId;
  public ScenePartitioner partitioner;
  public Action<object> eventCallback;
  public object obj;

  public ScenePartitionerEntry(
    string name,
    object obj,
    int x,
    int y,
    int width,
    int height,
    ScenePartitionerLayer layer,
    ScenePartitioner partitioner,
    Action<object> event_callback)
  {
    if (x >= 0 && y >= 0 && width >= 0)
      ;
    this.x = x;
    this.y = y;
    this.width = width;
    this.height = height;
    this.layer = layer.layer;
    this.partitioner = partitioner;
    this.eventCallback = event_callback;
    this.obj = obj;
  }

  public void UpdatePosition(int x, int y) => this.partitioner.UpdatePosition(x, y, this);

  public void UpdatePosition(Extents e) => this.partitioner.UpdatePosition(e, this);

  public void Release()
  {
    if (this.partitioner == null)
      return;
    this.partitioner.Remove(this);
  }
}
