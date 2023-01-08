// Decompiled with JetBrains decompiler
// Type: TUNING.LIGHT2D
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

namespace TUNING
{
  public class LIGHT2D
  {
    public const int SUNLIGHT_MAX_DEFAULT = 80000;
    public static readonly Color LIGHT_BLUE = new Color(0.38f, 0.61f, 1f, 1f);
    public static readonly Color LIGHT_PURPLE = new Color(0.9f, 0.4f, 0.74f, 1f);
    public static readonly Color LIGHT_YELLOW = new Color(0.57f, 0.55f, 0.44f, 1f);
    public static readonly Color LIGHT_OVERLAY = new Color(0.56f, 0.56f, 0.56f, 1f);
    public static readonly Vector2 DEFAULT_DIRECTION = new Vector2(0.0f, -1f);
    public const int FLOORLAMP_LUX = 1000;
    public const float FLOORLAMP_RANGE = 4f;
    public const float FLOORLAMP_ANGLE = 0.0f;
    public const LightShape FLOORLAMP_SHAPE = LightShape.Circle;
    public static readonly Color FLOORLAMP_COLOR = LIGHT2D.LIGHT_YELLOW;
    public static readonly Color FLOORLAMP_OVERLAYCOLOR = LIGHT2D.LIGHT_OVERLAY;
    public static readonly Vector2 FLOORLAMP_OFFSET = new Vector2(0.05f, 1.5f);
    public static readonly Vector2 FLOORLAMP_DIRECTION = LIGHT2D.DEFAULT_DIRECTION;
    public const float CEILINGLIGHT_RANGE = 8f;
    public const float CEILINGLIGHT_ANGLE = 2.6f;
    public const LightShape CEILINGLIGHT_SHAPE = LightShape.Cone;
    public static readonly Color CEILINGLIGHT_COLOR = LIGHT2D.LIGHT_YELLOW;
    public static readonly Color CEILINGLIGHT_OVERLAYCOLOR = LIGHT2D.LIGHT_OVERLAY;
    public static readonly Vector2 CEILINGLIGHT_OFFSET = new Vector2(0.05f, 0.65f);
    public static readonly Vector2 CEILINGLIGHT_DIRECTION = LIGHT2D.DEFAULT_DIRECTION;
    public const int CEILINGLIGHT_LUX = 1800;
    public const int SUNLAMP_LUX = 40000;
    public const float SUNLAMP_RANGE = 16f;
    public const float SUNLAMP_ANGLE = 5.2f;
    public const LightShape SUNLAMP_SHAPE = LightShape.Cone;
    public static readonly Color SUNLAMP_COLOR = LIGHT2D.LIGHT_YELLOW;
    public static readonly Color SUNLAMP_OVERLAYCOLOR = LIGHT2D.LIGHT_OVERLAY;
    public static readonly Vector2 SUNLAMP_OFFSET = new Vector2(0.0f, 3.5f);
    public static readonly Vector2 SUNLAMP_DIRECTION = LIGHT2D.DEFAULT_DIRECTION;
    public static readonly Color LIGHT_PREVIEW_COLOR = LIGHT2D.LIGHT_YELLOW;
    public const float HEADQUARTERS_RANGE = 5f;
    public const LightShape HEADQUARTERS_SHAPE = LightShape.Circle;
    public static readonly Color HEADQUARTERS_COLOR = LIGHT2D.LIGHT_YELLOW;
    public static readonly Color HEADQUARTERS_OVERLAYCOLOR = LIGHT2D.LIGHT_OVERLAY;
    public static readonly Vector2 HEADQUARTERS_OFFSET = new Vector2(0.5f, 3f);
    public static readonly Vector2 EXOBASE_HEADQUARTERS_OFFSET = new Vector2(0.0f, 2.5f);
    public const float ENGINE_RANGE = 10f;
    public const LightShape ENGINE_SHAPE = LightShape.Circle;
    public const int ENGINE_LUX = 80000;
    public const float WALLLIGHT_RANGE = 4f;
    public const float WALLLIGHT_ANGLE = 0.0f;
    public const LightShape WALLLIGHT_SHAPE = LightShape.Circle;
    public static readonly Color WALLLIGHT_COLOR = LIGHT2D.LIGHT_YELLOW;
    public static readonly Color WALLLIGHT_OVERLAYCOLOR = LIGHT2D.LIGHT_OVERLAY;
    public static readonly Vector2 WALLLIGHT_OFFSET = new Vector2(0.0f, 0.5f);
    public static readonly Vector2 WALLLIGHT_DIRECTION = LIGHT2D.DEFAULT_DIRECTION;
    public const float LIGHTBUG_RANGE = 5f;
    public const float LIGHTBUG_ANGLE = 0.0f;
    public const LightShape LIGHTBUG_SHAPE = LightShape.Circle;
    public const int LIGHTBUG_LUX = 1800;
    public static readonly Color LIGHTBUG_COLOR = LIGHT2D.LIGHT_YELLOW;
    public static readonly Color LIGHTBUG_OVERLAYCOLOR = LIGHT2D.LIGHT_OVERLAY;
    public static readonly Color LIGHTBUG_COLOR_ORANGE = new Color(0.5686275f, 0.482352942f, 0.4392157f, 1f);
    public static readonly Color LIGHTBUG_COLOR_PURPLE = new Color(0.490196079f, 0.4392157f, 0.5686275f, 1f);
    public static readonly Color LIGHTBUG_COLOR_PINK = new Color(0.5686275f, 0.4392157f, 0.5686275f, 1f);
    public static readonly Color LIGHTBUG_COLOR_BLUE = new Color(0.4392157f, 0.4862745f, 0.5686275f, 1f);
    public static readonly Color LIGHTBUG_COLOR_CRYSTAL = new Color(0.5137255f, 0.6666667f, 0.6666667f, 1f);
    public static readonly Color LIGHTBUG_COLOR_GREEN = new Color(0.431372553f, 1f, 0.533333361f, 1f);
    public static readonly Vector2 LIGHTBUG_OFFSET = new Vector2(0.05f, 0.25f);
    public static readonly Vector2 LIGHTBUG_DIRECTION = LIGHT2D.DEFAULT_DIRECTION;
    public const int PLASMALAMP_LUX = 666;
    public const float PLASMALAMP_RANGE = 2f;
    public const float PLASMALAMP_ANGLE = 0.0f;
    public const LightShape PLASMALAMP_SHAPE = LightShape.Circle;
    public static readonly Color PLASMALAMP_COLOR = LIGHT2D.LIGHT_PURPLE;
    public static readonly Color PLASMALAMP_OVERLAYCOLOR = LIGHT2D.LIGHT_OVERLAY;
    public static readonly Vector2 PLASMALAMP_OFFSET = new Vector2(0.05f, 0.5f);
    public static readonly Vector2 PLASMALAMP_DIRECTION = LIGHT2D.DEFAULT_DIRECTION;
    public const int MAGMALAMP_LUX = 666;
    public const float MAGMALAMP_RANGE = 2f;
    public const float MAGMALAMP_ANGLE = 0.0f;
    public const LightShape MAGMALAMP_SHAPE = LightShape.Cone;
    public static readonly Color MAGMALAMP_COLOR = LIGHT2D.LIGHT_YELLOW;
    public static readonly Color MAGMALAMP_OVERLAYCOLOR = LIGHT2D.LIGHT_OVERLAY;
    public static readonly Vector2 MAGMALAMP_OFFSET = new Vector2(0.05f, 0.33f);
    public static readonly Vector2 MAGMALAMP_DIRECTION = LIGHT2D.DEFAULT_DIRECTION;
    public const int BIOLUMROCK_LUX = 666;
    public const float BIOLUMROCK_RANGE = 2f;
    public const float BIOLUMROCK_ANGLE = 0.0f;
    public const LightShape BIOLUMROCK_SHAPE = LightShape.Cone;
    public static readonly Color BIOLUMROCK_COLOR = LIGHT2D.LIGHT_BLUE;
    public static readonly Color BIOLUMROCK_OVERLAYCOLOR = LIGHT2D.LIGHT_OVERLAY;
    public static readonly Vector2 BIOLUMROCK_OFFSET = new Vector2(0.05f, 0.33f);
    public static readonly Vector2 BIOLUMROCK_DIRECTION = LIGHT2D.DEFAULT_DIRECTION;
  }
}
