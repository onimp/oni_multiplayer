// Decompiled with JetBrains decompiler
// Type: ButtonMenuTextOverride
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

[Serializable]
public struct ButtonMenuTextOverride
{
  public LocString Text;
  public LocString CancelText;
  public LocString ToolTip;
  public LocString CancelToolTip;

  public bool IsValid => !string.IsNullOrEmpty((string) this.Text) && !string.IsNullOrEmpty((string) this.ToolTip);

  public bool HasCancelText => !string.IsNullOrEmpty((string) this.CancelText) && !string.IsNullOrEmpty((string) this.CancelToolTip);
}
