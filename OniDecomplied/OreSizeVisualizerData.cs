// Decompiled with JetBrains decompiler
// Type: OreSizeVisualizerData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public struct OreSizeVisualizerData
{
  public PrimaryElement primaryElement;
  public Action<object> onMassChangedCB;

  public OreSizeVisualizerData(GameObject go)
  {
    this.primaryElement = go.GetComponent<PrimaryElement>();
    this.onMassChangedCB = (Action<object>) null;
  }
}
