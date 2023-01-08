// Decompiled with JetBrains decompiler
// Type: LightingSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class LightingSettings : ScriptableObject
{
  [Header("Global")]
  public bool UpdateLightSettings;
  public float BloomScale;
  public Color32 LightColour = Color32.op_Implicit(Color.white);
  [Header("Digging")]
  public float DigMapScale;
  public Color DigMapColour;
  public Texture2D DigDamageMap;
  [Header("State Transition")]
  public Texture2D StateTransitionMap;
  public Color StateTransitionColor;
  public float StateTransitionUVScale;
  public Vector2 StateTransitionUVOffsetRate;
  [Header("Falling Solids")]
  public Texture2D FallingSolidMap;
  public Color FallingSolidColor;
  public float FallingSolidUVScale;
  public Vector2 FallingSolidUVOffsetRate;
  [Header("Metal Shine")]
  public Vector2 ShineCenter;
  public Vector2 ShineRange;
  public float ShineZoomSpeed;
  [Header("Water")]
  public Color WaterTrimColor;
  public float WaterTrimSize;
  public float WaterAlphaTrimSize;
  public float WaterAlphaThreshold;
  public float WaterCubesAlphaThreshold;
  public float WaterWaveAmplitude;
  public float WaterWaveFrequency;
  public float WaterWaveSpeed;
  public float WaterDetailSpeed;
  public float WaterDetailTiling;
  public float WaterDetailTiling2;
  public Vector2 WaterDetailDirection;
  public float WaterWaveAmplitude2;
  public float WaterWaveFrequency2;
  public float WaterWaveSpeed2;
  public float WaterCubeMapScale;
  public float WaterColorScale;
  public float WaterDistortionScaleStart;
  public float WaterDistortionScaleEnd;
  public float WaterDepthColorOpacityStart;
  public float WaterDepthColorOpacityEnd;
  [Header("Liquid")]
  public float LiquidMin;
  public float LiquidMax;
  public float LiquidCutoff;
  public float LiquidTransparency;
  public float LiquidAmountOffset;
  public float LiquidMaxMass;
  [Header("Grid")]
  public float GridLineWidth;
  public float GridSize;
  public float GridMaxIntensity;
  public float GridMinIntensity;
  public Color GridColor;
  [Header("Terrain")]
  public float EdgeGlowCutoffStart;
  public float EdgeGlowCutoffEnd;
  public float EdgeGlowIntensity;
  public int BackgroundLayers;
  public float BackgroundBaseParallax;
  public float BackgroundLayerParallax;
  public float BackgroundDarkening;
  public float BackgroundClip;
  public float BackgroundUVScale;
  public LightingSettings.EdgeLighting substanceEdgeParameters;
  public LightingSettings.EdgeLighting tileEdgeParameters;
  public float AnimIntensity;
  public float GasMinOpacity;
  public float GasMaxOpacity;
  public Color[] DarkenTints;
  public LightingSettings.LightingColours characterLighting;
  public Color BrightenOverlayColour;
  public Color[] ColdColours;
  public Color[] HotColours;
  [Header("Temperature Overlay Effects")]
  public Vector4 TemperatureParallax;
  public Texture2D EmberTex;
  public Texture2D FrostTex;
  public Texture2D Thermal1Tex;
  public Texture2D Thermal2Tex;
  public Vector2 ColdFGUVOffset;
  public Vector2 ColdMGUVOffset;
  public Vector2 ColdBGUVOffset;
  public Vector2 HotFGUVOffset;
  public Vector2 HotMGUVOffset;
  public Vector2 HotBGUVOffset;
  public Texture2D DustTex;
  public Color DustColour;
  public float DustScale;
  public Vector3 DustMovement;
  public float ShowGas;
  public float ShowTemperature;
  public float ShowDust;
  public float ShowShadow;
  public Vector4 HeatHazeParameters;
  public Texture2D HeatHazeTexture;
  [Header("Biome")]
  public float WorldZoneGasBlend;
  public float WorldZoneLiquidBlend;
  public float WorldZoneForegroundBlend;
  public float WorldZoneSimpleAnimBlend;
  public float WorldZoneAnimBlend;
  [Header("FX")]
  public Color32 SmokeDamageTint;
  [Header("Building Damage")]
  public Texture2D BuildingDamagedTex;
  public Vector4 BuildingDamagedUVParameters;
  [Header("Disease")]
  public Texture2D DiseaseOverlayTex;
  public Vector4 DiseaseOverlayTexInfo;
  [Header("Conduits")]
  public ConduitFlowVisualizer.Tuning GasConduit;
  public ConduitFlowVisualizer.Tuning LiquidConduit;
  public SolidConduitFlowVisualizer.Tuning SolidConduit;
  [Header("Radiation Overlay")]
  public bool ShowRadiation;
  public Texture2D Radiation1Tex;
  public Texture2D Radiation2Tex;
  public Texture2D Radiation3Tex;
  public Texture2D Radiation4Tex;
  public Texture2D NoiseTex;
  public Color RadColor;
  public Vector2 Rad1UVOffset;
  public Vector2 Rad2UVOffset;
  public Vector2 Rad3UVOffset;
  public Vector2 Rad4UVOffset;
  public Vector4 RadUVScales;
  public Vector2 Rad1Range;
  public Vector2 Rad2Range;
  public Vector2 Rad3Range;
  public Vector2 Rad4Range;

  [Serializable]
  public struct EdgeLighting
  {
    public float intensity;
    public float edgeIntensity;
    public float directSunlightScale;
    public float power;
  }

  public enum TintLayers
  {
    Background,
    Midground,
    Foreground,
    NumLayers,
  }

  [Serializable]
  public struct LightingColours
  {
    public Color32 litColour;
    public Color32 unlitColour;
  }
}
