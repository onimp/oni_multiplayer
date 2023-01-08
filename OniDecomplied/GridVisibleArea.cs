// Decompiled with JetBrains decompiler
// Type: GridVisibleArea
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class GridVisibleArea
{
  private GridArea[] Areas = new GridArea[3];
  private List<GridVisibleArea.Callback> Callbacks = new List<GridVisibleArea.Callback>();

  public GridArea CurrentArea => this.Areas[0];

  public GridArea PreviousArea => this.Areas[1];

  public GridArea PreviousPreviousArea => this.Areas[2];

  public void Update()
  {
    this.Areas[2] = this.Areas[1];
    this.Areas[1] = this.Areas[0];
    this.Areas[0] = GridVisibleArea.GetVisibleArea();
    foreach (GridVisibleArea.Callback callback in this.Callbacks)
      callback.OnUpdate();
  }

  public void AddCallback(string name, System.Action on_update) => this.Callbacks.Add(new GridVisibleArea.Callback()
  {
    Name = name,
    OnUpdate = on_update
  });

  public void Run(Action<int> in_view)
  {
    if (in_view == null)
      return;
    this.CurrentArea.Run(in_view);
  }

  public void Run(
    Action<int> outside_view,
    Action<int> inside_view,
    Action<int> inside_view_second_time)
  {
    if (outside_view != null)
      this.PreviousArea.RunOnDifference(this.CurrentArea, outside_view);
    if (inside_view != null)
      this.CurrentArea.RunOnDifference(this.PreviousArea, inside_view);
    if (inside_view_second_time == null)
      return;
    this.PreviousArea.RunOnDifference(this.PreviousPreviousArea, inside_view_second_time);
  }

  public void RunIfVisible(int cell, Action<int> action) => this.CurrentArea.RunIfInside(cell, action);

  public static GridArea GetVisibleArea()
  {
    GridArea visibleArea = new GridArea();
    Camera mainCamera = Game.MainCamera;
    if (Object.op_Inequality((Object) mainCamera, (Object) null))
    {
      Vector3 worldPoint1 = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, TransformExtensions.GetPosition(((Component) mainCamera).transform).z));
      Vector3 worldPoint2 = mainCamera.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, TransformExtensions.GetPosition(((Component) mainCamera).transform).z));
      if (Object.op_Inequality((Object) CameraController.Instance, (Object) null))
      {
        Vector2I worldOffset;
        Vector2I worldSize;
        CameraController.Instance.GetWorldCamera(out worldOffset, out worldSize);
        visibleArea.SetExtents(Math.Max((int) ((double) worldPoint2.x - 0.5), worldOffset.x), Math.Max((int) ((double) worldPoint2.y - 0.5), worldOffset.y), Math.Min((int) ((double) worldPoint1.x + 1.5), worldSize.x + worldOffset.x), Math.Min((int) ((double) worldPoint1.y + 1.5), worldSize.y + worldOffset.y));
      }
      else
        visibleArea.SetExtents(Math.Max((int) ((double) worldPoint2.x - 0.5), 0), Math.Max((int) ((double) worldPoint2.y - 0.5), 0), Math.Min((int) ((double) worldPoint1.x + 1.5), Grid.WidthInCells), Math.Min((int) ((double) worldPoint1.y + 1.5), Grid.HeightInCells));
    }
    return visibleArea;
  }

  public struct Callback
  {
    public System.Action OnUpdate;
    public string Name;
  }
}
