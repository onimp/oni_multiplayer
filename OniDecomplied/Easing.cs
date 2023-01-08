// Decompiled with JetBrains decompiler
// Type: Easing
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class Easing
{
  public const Easing.EasingFn PARAM_DEFAULT = null;
  public static readonly Easing.EasingFn Linear = (Easing.EasingFn) (x => x);
  public static readonly Easing.EasingFn SmoothStep = (Easing.EasingFn) (x => Mathf.SmoothStep(0.0f, 1f, x));
  public static readonly Easing.EasingFn QuadIn = (Easing.EasingFn) (x => x * x);
  public static readonly Easing.EasingFn QuadOut = (Easing.EasingFn) (x => (float) (1.0 - (1.0 - (double) x) * (1.0 - (double) x)));
  public static readonly Easing.EasingFn QuadInOut = (Easing.EasingFn) (x => (double) x >= 0.5 ? (float) (1.0 - (double) Mathf.Pow((float) (-2.0 * (double) x + 2.0), 2f) / 2.0) : 2f * x * x);
  public static readonly Easing.EasingFn CubicIn = (Easing.EasingFn) (x => x * x * x);
  public static readonly Easing.EasingFn CubicOut = (Easing.EasingFn) (x => 1f - Mathf.Pow(1f - x, 3f));
  public static readonly Easing.EasingFn CubicInOut = (Easing.EasingFn) (x => (double) x >= 0.5 ? (float) (1.0 - (double) Mathf.Pow((float) (-2.0 * (double) x + 2.0), 3f) / 2.0) : 4f * x * x * x);
  public static readonly Easing.EasingFn QuartIn = (Easing.EasingFn) (x => x * x * x * x);
  public static readonly Easing.EasingFn QuartOut = (Easing.EasingFn) (x => 1f - Mathf.Pow(1f - x, 4f));
  public static readonly Easing.EasingFn QuartInOut = (Easing.EasingFn) (x => (double) x >= 0.5 ? (float) (1.0 - (double) Mathf.Pow((float) (-2.0 * (double) x + 2.0), 4f) / 2.0) : 8f * x * x * x * x);
  public static readonly Easing.EasingFn QuintIn = (Easing.EasingFn) (x => x * x * x * x * x);
  public static readonly Easing.EasingFn QuintOut = (Easing.EasingFn) (x => 1f - Mathf.Pow(1f - x, 5f));
  public static readonly Easing.EasingFn QuintInOut = (Easing.EasingFn) (x => (double) x >= 0.5 ? (float) (1.0 - (double) Mathf.Pow((float) (-2.0 * (double) x + 2.0), 5f) / 2.0) : 16f * x * x * x * x * x);
  public static readonly Easing.EasingFn ExpoIn = (Easing.EasingFn) (x => (double) x != 0.0 ? Mathf.Pow(2f, (float) (10.0 * (double) x - 10.0)) : 0.0f);
  public static readonly Easing.EasingFn ExpoOut = (Easing.EasingFn) (x => (double) x != 1.0 ? 1f - Mathf.Pow(2f, -10f * x) : 1f);
  public static readonly Easing.EasingFn ExpoInOut = (Easing.EasingFn) (x =>
  {
    if ((double) x == 0.0)
      return 0.0f;
    if ((double) x == 1.0)
      return 1f;
    return (double) x >= 0.5 ? (float) ((2.0 - (double) Mathf.Pow(2f, (float) (-20.0 * (double) x + 10.0))) / 2.0) : Mathf.Pow(2f, (float) (20.0 * (double) x - 10.0)) / 2f;
  });
  public static readonly Easing.EasingFn SineIn = (Easing.EasingFn) (x => 1f - Mathf.Cos((float) ((double) x * 3.1415927410125732 / 2.0)));
  public static readonly Easing.EasingFn SineOut = (Easing.EasingFn) (x => Mathf.Sin((float) ((double) x * 3.1415927410125732 / 2.0)));
  public static readonly Easing.EasingFn SineInOut = (Easing.EasingFn) (x => (float) (-((double) Mathf.Cos(3.14159274f * x) - 1.0) / 2.0));
  public static readonly Easing.EasingFn CircIn = (Easing.EasingFn) (x => 1f - Mathf.Sqrt(1f - Mathf.Pow(x, 2f)));
  public static readonly Easing.EasingFn CircOut = (Easing.EasingFn) (x => Mathf.Sqrt(1f - Mathf.Pow(x - 1f, 2f)));
  public static readonly Easing.EasingFn CircInOut = (Easing.EasingFn) (x => (double) x >= 0.5 ? (float) (((double) Mathf.Sqrt(1f - Mathf.Pow((float) (-2.0 * (double) x + 2.0), 2f)) + 1.0) / 2.0) : (float) ((1.0 - (double) Mathf.Sqrt(1f - Mathf.Pow(2f * x, 2f))) / 2.0));
  public static readonly Easing.EasingFn ElasticIn = (Easing.EasingFn) (x =>
  {
    if ((double) x == 0.0)
      return 0.0f;
    return (double) x != 1.0 ? -Mathf.Pow(2f, (float) (10.0 * (double) x - 10.0)) * Mathf.Sin((float) (((double) x * 10.0 - 10.75) * 2.0943951606750488)) : 1f;
  });
  public static readonly Easing.EasingFn ElasticOut = (Easing.EasingFn) (x =>
  {
    if ((double) x == 0.0)
      return 0.0f;
    return (double) x != 1.0 ? (float) ((double) Mathf.Pow(2f, -10f * x) * (double) Mathf.Sin((float) (((double) x * 10.0 - 0.75) * 2.0943951606750488)) + 1.0) : 1f;
  });
  public static readonly Easing.EasingFn ElasticInOut = (Easing.EasingFn) (x =>
  {
    if ((double) x == 0.0)
      return 0.0f;
    if ((double) x == 1.0)
      return 1f;
    return (double) x >= 0.5 ? (float) ((double) Mathf.Pow(2f, (float) (-20.0 * (double) x + 10.0)) * (double) Mathf.Sin((float) ((20.0 * (double) x - 11.125) * 1.3962634801864624)) / 2.0 + 1.0) : (float) (-((double) Mathf.Pow(2f, (float) (20.0 * (double) x - 10.0)) * (double) Mathf.Sin((float) ((20.0 * (double) x - 11.125) * 1.3962634801864624))) / 2.0);
  });
  public static readonly Easing.EasingFn BackIn = (Easing.EasingFn) (x => (float) (2.7015800476074219 * (double) x * (double) x * (double) x - 1.7015800476074219 * (double) x * (double) x));
  public static readonly Easing.EasingFn BackOut = (Easing.EasingFn) (x => (float) (1.0 + 2.7015800476074219 * (double) Mathf.Pow(x - 1f, 3f) + 1.7015800476074219 * (double) Mathf.Pow(x - 1f, 2f)));
  public static readonly Easing.EasingFn BackInOut = (Easing.EasingFn) (x => (double) x >= 0.5 ? (float) (((double) Mathf.Pow((float) (2.0 * (double) x - 2.0), 2f) * (3.5949094295501709 * ((double) x * 2.0 - 2.0) + 2.5949094295501709) + 2.0) / 2.0) : (float) ((double) Mathf.Pow(2f * x, 2f) * (7.1898188591003418 * (double) x - 2.5949094295501709) / 2.0));
  public static readonly Easing.EasingFn BounceIn = (Easing.EasingFn) (x => 1f - Easing.BounceOut(1f - x));
  public static readonly Easing.EasingFn BounceOut = (Easing.EasingFn) (x =>
  {
    if ((double) x < 0.36363637447357178)
      return 121f / 16f * x * x;
    if ((double) x < 0.72727274894714355)
      return (float) (121.0 / 16.0 * (double) (x -= 0.545454562f) * (double) x + 0.75);
    return (double) x < 10.0 / 11.0 ? (float) (121.0 / 16.0 * (double) (x -= 0.8181818f) * (double) x + 15.0 / 16.0) : (float) (121.0 / 16.0 * (double) (x -= 0.954545438f) * (double) x + 63.0 / 64.0);
  });
  public static readonly Easing.EasingFn BounceInOut = (Easing.EasingFn) (x => (double) x >= 0.5 ? (float) ((1.0 + (double) Easing.BounceOut((float) (2.0 * (double) x - 1.0))) / 2.0) : (float) ((1.0 - (double) Easing.BounceOut((float) (1.0 - 2.0 * (double) x))) / 2.0));

  public delegate float EasingFn(float f);
}
