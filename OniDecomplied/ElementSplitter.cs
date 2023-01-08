// Decompiled with JetBrains decompiler
// Type: ElementSplitter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public struct ElementSplitter
{
  public PrimaryElement primaryElement;
  public Func<float, Pickupable> onTakeCB;
  public Func<Pickupable, bool> canAbsorbCB;

  public ElementSplitter(GameObject go)
  {
    this.primaryElement = go.GetComponent<PrimaryElement>();
    this.onTakeCB = (Func<float, Pickupable>) null;
    this.canAbsorbCB = (Func<Pickupable, bool>) null;
  }
}
