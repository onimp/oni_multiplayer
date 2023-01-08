// Decompiled with JetBrains decompiler
// Type: LimitValveTuning
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class LimitValveTuning
{
  public const float MAX_LIMIT = 500f;
  public const float DEFAULT_LIMIT = 100f;

  public static NonLinearSlider.Range[] GetDefaultSlider() => new NonLinearSlider.Range[2]
  {
    new NonLinearSlider.Range(70f, 100f),
    new NonLinearSlider.Range(30f, 500f)
  };
}
