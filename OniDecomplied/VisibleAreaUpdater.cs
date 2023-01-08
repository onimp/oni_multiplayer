// Decompiled with JetBrains decompiler
// Type: VisibleAreaUpdater
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class VisibleAreaUpdater
{
  private GridVisibleArea VisibleArea;
  private Action<int> OutsideViewFirstTimeCallback;
  private Action<int> InsideViewFirstTimeCallback;
  private Action<int> UpdateCallback;
  private string Name;

  public VisibleAreaUpdater(
    Action<int> outside_view_first_time_cb,
    Action<int> inside_view_first_time_cb,
    string name)
  {
    this.OutsideViewFirstTimeCallback = outside_view_first_time_cb;
    this.InsideViewFirstTimeCallback = inside_view_first_time_cb;
    this.UpdateCallback = new Action<int>(this.InternalUpdateCell);
    this.Name = name;
  }

  public void Update()
  {
    if (!Object.op_Inequality((Object) CameraController.Instance, (Object) null) || this.VisibleArea != null)
      return;
    this.VisibleArea = CameraController.Instance.VisibleArea;
    this.VisibleArea.Run(this.InsideViewFirstTimeCallback);
  }

  private void InternalUpdateCell(int cell)
  {
    this.OutsideViewFirstTimeCallback(cell);
    this.InsideViewFirstTimeCallback(cell);
  }

  public void UpdateCell(int cell)
  {
    if (this.VisibleArea == null)
      return;
    this.VisibleArea.RunIfVisible(cell, this.UpdateCallback);
  }
}
