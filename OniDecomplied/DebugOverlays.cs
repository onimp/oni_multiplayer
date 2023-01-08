// Decompiled with JetBrains decompiler
// Type: DebugOverlays
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugOverlays : KScreen
{
  public static DebugOverlays instance { get; private set; }

  protected virtual void OnPrefabInit()
  {
    DebugOverlays.instance = this;
    KPopupMenu componentInChildren = ((Component) this).GetComponentInChildren<KPopupMenu>();
    componentInChildren.SetOptions((IList<string>) new string[5]
    {
      "None",
      "Rooms",
      "Lighting",
      "Style",
      "Flow"
    });
    componentInChildren.OnSelect += new Action<string, int>(this.OnSelect);
    ((Component) this).gameObject.SetActive(false);
  }

  private void OnSelect(string str, int index)
  {
    switch (str)
    {
      case "None":
        SimDebugView.Instance.SetMode(OverlayModes.None.ID);
        break;
      case "Flow":
        SimDebugView.Instance.SetMode(SimDebugView.OverlayModes.Flow);
        break;
      case "Lighting":
        SimDebugView.Instance.SetMode(OverlayModes.Light.ID);
        break;
      case "Rooms":
        SimDebugView.Instance.SetMode(OverlayModes.Rooms.ID);
        break;
      default:
        Debug.LogError((object) ("Unknown debug view: " + str));
        break;
    }
  }
}
