// Decompiled with JetBrains decompiler
// Type: IActivationRangeTarget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public interface IActivationRangeTarget
{
  float ActivateValue { get; set; }

  float DeactivateValue { get; set; }

  float MinValue { get; }

  float MaxValue { get; }

  bool UseWholeNumbers { get; }

  string ActivationRangeTitleText { get; }

  string ActivateSliderLabelText { get; }

  string DeactivateSliderLabelText { get; }

  string ActivateTooltip { get; }

  string DeactivateTooltip { get; }
}
