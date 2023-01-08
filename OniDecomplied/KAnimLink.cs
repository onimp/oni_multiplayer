// Decompiled with JetBrains decompiler
// Type: KAnimLink
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class KAnimLink
{
  public bool syncTint = true;
  private KAnimControllerBase master;
  private KAnimControllerBase slave;

  public KAnimLink(KAnimControllerBase master, KAnimControllerBase slave)
  {
    this.slave = slave;
    this.master = master;
    this.Register();
  }

  private void Register()
  {
    this.master.OnOverlayColourChanged += new Action<Color32>(this.OnOverlayColourChanged);
    this.master.OnTintChanged += new Action<Color>(this.OnTintColourChanged);
    this.master.OnHighlightChanged += new Action<Color>(this.OnHighlightColourChanged);
    this.master.onLayerChanged += new Action<int>(this.slave.SetLayer);
  }

  public void Unregister()
  {
    if (!Object.op_Inequality((Object) this.master, (Object) null))
      return;
    this.master.OnOverlayColourChanged -= new Action<Color32>(this.OnOverlayColourChanged);
    this.master.OnTintChanged -= new Action<Color>(this.OnTintColourChanged);
    this.master.OnHighlightChanged -= new Action<Color>(this.OnHighlightColourChanged);
    if (!Object.op_Inequality((Object) this.slave, (Object) null))
      return;
    this.master.onLayerChanged -= new Action<int>(this.slave.SetLayer);
  }

  private void OnOverlayColourChanged(Color32 c)
  {
    if (!Object.op_Inequality((Object) this.slave, (Object) null))
      return;
    this.slave.OverlayColour = Color32.op_Implicit(c);
  }

  private void OnTintColourChanged(Color c)
  {
    if (!this.syncTint || !Object.op_Inequality((Object) this.slave, (Object) null))
      return;
    this.slave.TintColour = Color32.op_Implicit(c);
  }

  private void OnHighlightColourChanged(Color c)
  {
    if (!Object.op_Inequality((Object) this.slave, (Object) null))
      return;
    this.slave.HighlightColour = Color32.op_Implicit(c);
  }
}
