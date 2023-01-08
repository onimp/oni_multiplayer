// Decompiled with JetBrains decompiler
// Type: ISliderControl
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public interface ISliderControl
{
  string SliderTitleKey { get; }

  string SliderUnits { get; }

  int SliderDecimalPlaces(int index);

  float GetSliderMin(int index);

  float GetSliderMax(int index);

  float GetSliderValue(int index);

  void SetSliderValue(float percent, int index);

  string GetSliderTooltipKey(int index);

  string GetSliderTooltip();
}
