// Decompiled with JetBrains decompiler
// Type: ICheckboxControl
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public interface ICheckboxControl
{
  string CheckboxTitleKey { get; }

  string CheckboxLabel { get; }

  string CheckboxTooltip { get; }

  bool GetCheckboxValue();

  void SetCheckboxValue(bool value);
}
