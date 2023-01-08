// Decompiled with JetBrains decompiler
// Type: TUNING.OVERLAY
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

namespace TUNING
{
  public class OVERLAY
  {
    public class TEMPERATURE_LEGEND
    {
      public static readonly LegendEntry MAXHOT = new LegendEntry((string) UI.OVERLAYS.TEMPERATURE.MAXHOT, (string) UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.0f, 0.0f, 0.0f));
      public static readonly LegendEntry EXTREMEHOT = new LegendEntry((string) UI.OVERLAYS.TEMPERATURE.EXTREMEHOT, (string) UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.0f, 0.0f, 0.0f));
      public static readonly LegendEntry VERYHOT = new LegendEntry((string) UI.OVERLAYS.TEMPERATURE.VERYHOT, (string) UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(1f, 0.0f, 0.0f));
      public static readonly LegendEntry HOT = new LegendEntry((string) UI.OVERLAYS.TEMPERATURE.HOT, (string) UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.0f, 1f, 0.0f));
      public static readonly LegendEntry TEMPERATE = new LegendEntry((string) UI.OVERLAYS.TEMPERATURE.TEMPERATE, (string) UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.0f, 0.0f, 0.0f));
      public static readonly LegendEntry COLD = new LegendEntry((string) UI.OVERLAYS.TEMPERATURE.COLD, (string) UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.0f, 0.0f, 1f));
      public static readonly LegendEntry VERYCOLD = new LegendEntry((string) UI.OVERLAYS.TEMPERATURE.VERYCOLD, (string) UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.0f, 0.0f, 1f));
      public static readonly LegendEntry EXTREMECOLD = new LegendEntry((string) UI.OVERLAYS.TEMPERATURE.EXTREMECOLD, (string) UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.0f, 0.0f, 0.0f));
    }

    public class HEATFLOW_LEGEND
    {
      public static readonly LegendEntry HEATING = new LegendEntry((string) UI.OVERLAYS.HEATFLOW.HEATING, (string) UI.OVERLAYS.HEATFLOW.TOOLTIPS.HEATING, new Color(0.0f, 0.0f, 0.0f));
      public static readonly LegendEntry NEUTRAL = new LegendEntry((string) UI.OVERLAYS.HEATFLOW.NEUTRAL, (string) UI.OVERLAYS.HEATFLOW.TOOLTIPS.NEUTRAL, new Color(0.0f, 0.0f, 0.0f));
      public static readonly LegendEntry COOLING = new LegendEntry((string) UI.OVERLAYS.HEATFLOW.COOLING, (string) UI.OVERLAYS.HEATFLOW.TOOLTIPS.COOLING, new Color(0.0f, 0.0f, 0.0f));
    }

    public class POWER_LEGEND
    {
      public const float WATTAGE_WARNING_THRESHOLD = 0.75f;
    }
  }
}
