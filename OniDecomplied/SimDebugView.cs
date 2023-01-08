// Decompiled with JetBrains decompiler
// Type: SimDebugView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

[AddComponentMenu("KMonoBehaviour/scripts/SimDebugView")]
public class SimDebugView : KMonoBehaviour
{
  [SerializeField]
  public Material material;
  public Material diseaseMaterial;
  public bool hideFOW;
  public const int colourSize = 4;
  private byte[] texBytes;
  private int currentFrame;
  [SerializeField]
  private Texture2D tex;
  [SerializeField]
  private GameObject plane;
  private HashedString mode;
  private SimDebugView.GameGridMode gameGridMode;
  private PathProber selectedPathProber;
  public float minTempExpected;
  public float maxTempExpected;
  public float minMassExpected;
  public float maxMassExpected;
  public float minPressureExpected;
  public float maxPressureExpected;
  public float minThermalConductivity;
  public float maxThermalConductivity;
  public float thresholdRange;
  public float thresholdOpacity;
  public static float minimumBreathable = 0.05f;
  public static float optimallyBreathable = 1f;
  public SimDebugView.ColorThreshold[] temperatureThresholds;
  public SimDebugView.ColorThreshold[] heatFlowThresholds;
  public Color32[] networkColours;
  public Gradient breathableGradient;
  public Color32 unbreathableColour;
  public Color32[] toxicColour;
  public static SimDebugView Instance;
  private WorkItemCollection<SimDebugView.UpdateSimViewWorkItem, SimDebugView.UpdateSimViewSharedData> updateSimViewWorkItems;
  private int selectedCell;
  private Dictionary<HashedString, Action<SimDebugView, Texture>> dataUpdateFuncs;
  private Dictionary<HashedString, Func<SimDebugView, int, Color>> getColourFuncs;
  public static readonly Color[] dbColours = new Color[13]
  {
    new Color(0.0f, 0.0f, 0.0f, 0.0f),
    new Color(1f, 1f, 1f, 0.3f),
    new Color(0.7058824f, 0.8235294f, 1f, 0.2f),
    new Color(0.0f, 0.3137255f, 1f, 0.3f),
    new Color(0.7058824f, 1f, 0.7058824f, 0.5f),
    new Color(0.0784313753f, 1f, 0.0f, 0.7f),
    new Color(1f, 0.9019608f, 0.7058824f, 0.9f),
    new Color(1f, 0.8235294f, 0.0f, 0.9f),
    new Color(1f, 0.7176471f, 0.3019608f, 0.9f),
    new Color(1f, 0.41568628f, 0.0f, 0.9f),
    new Color(1f, 0.7058824f, 0.7058824f, 1f),
    new Color(1f, 0.0f, 0.0f, 1f),
    new Color(1f, 0.0f, 0.0f, 1f)
  };
  private static float minMinionTemperature = 260f;
  private static float maxMinionTemperature = 310f;
  private static float minMinionPressure = 80f;

  public static void DestroyInstance() => SimDebugView.Instance = (SimDebugView) null;

  protected virtual void OnPrefabInit()
  {
    SimDebugView.Instance = this;
    this.material = Object.Instantiate<Material>(this.material);
    this.diseaseMaterial = Object.Instantiate<Material>(this.diseaseMaterial);
  }

  protected virtual void OnSpawn()
  {
    SimDebugViewCompositor.Instance.material.SetColor("_Color0", Color32.op_Implicit(GlobalAssets.Instance.colorSet.GetColorByName(this.temperatureThresholds[0].colorName)));
    SimDebugViewCompositor.Instance.material.SetColor("_Color1", Color32.op_Implicit(GlobalAssets.Instance.colorSet.GetColorByName(this.temperatureThresholds[1].colorName)));
    SimDebugViewCompositor.Instance.material.SetColor("_Color2", Color32.op_Implicit(GlobalAssets.Instance.colorSet.GetColorByName(this.temperatureThresholds[2].colorName)));
    SimDebugViewCompositor.Instance.material.SetColor("_Color3", Color32.op_Implicit(GlobalAssets.Instance.colorSet.GetColorByName(this.temperatureThresholds[3].colorName)));
    SimDebugViewCompositor.Instance.material.SetColor("_Color4", Color32.op_Implicit(GlobalAssets.Instance.colorSet.GetColorByName(this.temperatureThresholds[4].colorName)));
    SimDebugViewCompositor.Instance.material.SetColor("_Color5", Color32.op_Implicit(GlobalAssets.Instance.colorSet.GetColorByName(this.temperatureThresholds[5].colorName)));
    SimDebugViewCompositor.Instance.material.SetColor("_Color6", Color32.op_Implicit(GlobalAssets.Instance.colorSet.GetColorByName(this.temperatureThresholds[6].colorName)));
    SimDebugViewCompositor.Instance.material.SetColor("_Color7", Color32.op_Implicit(GlobalAssets.Instance.colorSet.GetColorByName(this.temperatureThresholds[7].colorName)));
    SimDebugViewCompositor.Instance.material.SetColor("_Color0", Color32.op_Implicit(GlobalAssets.Instance.colorSet.GetColorByName(this.heatFlowThresholds[0].colorName)));
    SimDebugViewCompositor.Instance.material.SetColor("_Color1", Color32.op_Implicit(GlobalAssets.Instance.colorSet.GetColorByName(this.heatFlowThresholds[1].colorName)));
    SimDebugViewCompositor.Instance.material.SetColor("_Color2", Color32.op_Implicit(GlobalAssets.Instance.colorSet.GetColorByName(this.heatFlowThresholds[2].colorName)));
    this.SetMode(global::OverlayModes.None.ID);
  }

  public void OnReset()
  {
    this.plane = SimDebugView.CreatePlane(nameof (SimDebugView), this.transform);
    this.tex = SimDebugView.CreateTexture(out this.texBytes, Grid.WidthInCells, Grid.HeightInCells);
    this.plane.GetComponent<Renderer>().sharedMaterial = this.material;
    this.plane.GetComponent<Renderer>().sharedMaterial.mainTexture = (Texture) this.tex;
    TransformExtensions.SetLocalPosition(this.plane.transform, new Vector3(0.0f, 0.0f, -6f));
    this.SetMode(global::OverlayModes.None.ID);
  }

  public static Texture2D CreateTexture(int width, int height)
  {
    Texture2D texture = new Texture2D(width, height);
    ((Object) texture).name = nameof (SimDebugView);
    ((Texture) texture).wrapMode = (TextureWrapMode) 1;
    ((Texture) texture).filterMode = (FilterMode) 0;
    return texture;
  }

  public static Texture2D CreateTexture(out byte[] textureBytes, int width, int height)
  {
    textureBytes = new byte[width * height * 4];
    Texture2D texture = new Texture2D(width, height, TextureUtil.TextureFormatToGraphicsFormat((TextureFormat) 4), (TextureCreationFlags) 0);
    ((Object) texture).name = nameof (SimDebugView);
    ((Texture) texture).wrapMode = (TextureWrapMode) 1;
    ((Texture) texture).filterMode = (FilterMode) 0;
    return texture;
  }

  public static Texture2D CreateTexture(int width, int height, Color col)
  {
    Color[] colorArray = new Color[width * height];
    for (int index = 0; index < colorArray.Length; ++index)
      colorArray[index] = col;
    Texture2D texture = new Texture2D(width, height);
    texture.SetPixels(colorArray);
    texture.Apply();
    return texture;
  }

  public static GameObject CreatePlane(string layer, Transform parent)
  {
    GameObject plane = new GameObject();
    ((Object) plane).name = "overlayViewDisplayPlane";
    Util.SetLayerRecursively(plane, LayerMask.NameToLayer(layer));
    plane.transform.SetParent(parent);
    TransformExtensions.SetPosition(plane.transform, Vector3.zero);
    ((Renderer) plane.AddComponent<MeshRenderer>()).reflectionProbeUsage = (ReflectionProbeUsage) 0;
    MeshFilter meshFilter = plane.AddComponent<MeshFilter>();
    Mesh mesh1 = new Mesh();
    Mesh mesh2 = mesh1;
    meshFilter.mesh = mesh2;
    int length = 4;
    Vector3[] vector3Array1 = new Vector3[length];
    Vector2[] vector2Array1 = new Vector2[length];
    int[] numArray1 = new int[6];
    float num = 2f * (float) Grid.HeightInCells;
    Vector3[] vector3Array2 = new Vector3[4]
    {
      new Vector3(0.0f, 0.0f, 0.0f),
      new Vector3((float) Grid.WidthInCells, 0.0f, 0.0f),
      new Vector3(0.0f, num, 0.0f),
      new Vector3(Grid.WidthInMeters, num, 0.0f)
    };
    Vector2[] vector2Array2 = new Vector2[4]
    {
      new Vector2(0.0f, 0.0f),
      new Vector2(1f, 0.0f),
      new Vector2(0.0f, 2f),
      new Vector2(1f, 2f)
    };
    int[] numArray2 = new int[6]{ 0, 2, 1, 1, 2, 3 };
    mesh1.vertices = vector3Array2;
    mesh1.uv = vector2Array2;
    mesh1.triangles = numArray2;
    Vector2 vector2;
    // ISSUE: explicit constructor call
    ((Vector2) ref vector2).\u002Ector((float) Grid.WidthInCells, num);
    mesh1.bounds = new Bounds(new Vector3(0.5f * vector2.x, 0.5f * vector2.y, 0.0f), new Vector3(vector2.x, vector2.y, 0.0f));
    return plane;
  }

  private void Update()
  {
    if (Object.op_Equality((Object) this.plane, (Object) null))
      return;
    bool flag = HashedString.op_Inequality(this.mode, global::OverlayModes.None.ID);
    this.plane.SetActive(flag);
    SimDebugViewCompositor.Instance.Toggle(flag && !GameUtil.IsCapturingTimeLapse());
    SimDebugViewCompositor.Instance.material.SetVector("_Thresholds0", new Vector4(0.1f, 0.2f, 0.3f, 0.4f));
    SimDebugViewCompositor.Instance.material.SetVector("_Thresholds1", new Vector4(0.5f, 0.6f, 0.7f, 0.8f));
    float num = 0.0f;
    if (HashedString.op_Equality(this.mode, global::OverlayModes.ThermalConductivity.ID) || HashedString.op_Equality(this.mode, global::OverlayModes.Temperature.ID))
      num = 1f;
    SimDebugViewCompositor.Instance.material.SetVector("_ThresholdParameters", new Vector4(num, this.thresholdRange, this.thresholdOpacity, 0.0f));
    if (!flag)
      return;
    this.UpdateData(this.tex, this.texBytes, this.mode, (byte) 192);
  }

  private static void SetDefaultBilinear(SimDebugView instance, Texture texture)
  {
    Renderer component = instance.plane.GetComponent<Renderer>();
    component.sharedMaterial = instance.material;
    component.sharedMaterial.mainTexture = (Texture) instance.tex;
    texture.filterMode = (FilterMode) 1;
  }

  private static void SetDefaultPoint(SimDebugView instance, Texture texture)
  {
    Renderer component = instance.plane.GetComponent<Renderer>();
    component.sharedMaterial = instance.material;
    component.sharedMaterial.mainTexture = (Texture) instance.tex;
    texture.filterMode = (FilterMode) 0;
  }

  private static void SetDisease(SimDebugView instance, Texture texture)
  {
    Renderer component = instance.plane.GetComponent<Renderer>();
    component.sharedMaterial = instance.diseaseMaterial;
    component.sharedMaterial.mainTexture = (Texture) instance.tex;
    texture.filterMode = (FilterMode) 1;
  }

  public void UpdateData(
    Texture2D texture,
    byte[] textureBytes,
    HashedString viewMode,
    byte alpha)
  {
    Action<SimDebugView, Texture> action;
    if (!this.dataUpdateFuncs.TryGetValue(viewMode, out action))
      action = new Action<SimDebugView, Texture>(SimDebugView.SetDefaultPoint);
    action(this, (Texture) texture);
    int min_x;
    int min_y;
    int max_x;
    int max_y;
    Grid.GetVisibleExtents(out min_x, out min_y, out max_x, out max_y);
    this.selectedPathProber = (PathProber) null;
    KSelectable selected = SelectTool.Instance.selected;
    if (Object.op_Inequality((Object) selected, (Object) null))
      this.selectedPathProber = ((Component) selected).GetComponent<PathProber>();
    this.updateSimViewWorkItems.Reset(new SimDebugView.UpdateSimViewSharedData(this, this.texBytes, viewMode, this));
    int num = 16;
    for (int y0 = min_y; y0 <= max_y; y0 += num)
    {
      int y1 = Math.Min(y0 + num - 1, max_y);
      this.updateSimViewWorkItems.Add(new SimDebugView.UpdateSimViewWorkItem(min_x, y0, max_x, y1));
    }
    this.currentFrame = Time.frameCount;
    this.selectedCell = Grid.PosToCell(Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos()));
    GlobalJobManager.Run((IWorkItemCollection) this.updateSimViewWorkItems);
    texture.LoadRawTextureData(textureBytes);
    texture.Apply();
  }

  public void SetGameGridMode(SimDebugView.GameGridMode mode) => this.gameGridMode = mode;

  public SimDebugView.GameGridMode GetGameGridMode() => this.gameGridMode;

  public void SetMode(HashedString mode)
  {
    this.mode = mode;
    EventExtensions.Trigger(((Component) Game.Instance).gameObject, 1798162660, (object) mode);
  }

  public HashedString GetMode() => this.mode;

  public static Color TemperatureToColor(
    float temperature,
    float minTempExpected,
    float maxTempExpected)
  {
    return Color.HSVToRGB((float) ((10.0 + (1.0 - (double) Mathf.Clamp((float) (((double) temperature - (double) minTempExpected) / ((double) maxTempExpected - (double) minTempExpected)), 0.0f, 1f)) * 171.0) / 360.0), 1f, 1f);
  }

  public static Color LiquidTemperatureToColor(
    float temperature,
    float minTempExpected,
    float maxTempExpected)
  {
    double num = ((double) temperature - (double) minTempExpected) / ((double) maxTempExpected - (double) minTempExpected);
    return Color.HSVToRGB((float) ((10.0 + (1.0 - (double) Mathf.Clamp((float) num, 0.5f, 1f)) * 171.0) / 360.0), Mathf.Clamp((float) num, 0.0f, 1f), 1f);
  }

  public static Color SolidTemperatureToColor(
    float temperature,
    float minTempExpected,
    float maxTempExpected)
  {
    return Color.HSVToRGB((float) ((10.0 + (1.0 - (double) Mathf.Clamp((float) (((double) temperature - (double) minTempExpected) / ((double) maxTempExpected - (double) minTempExpected)), 0.5f, 1f)) * 171.0) / 360.0), 1f, 1f);
  }

  public static Color GasTemperatureToColor(
    float temperature,
    float minTempExpected,
    float maxTempExpected)
  {
    return Color.HSVToRGB((float) ((10.0 + (1.0 - (double) Mathf.Clamp((float) (((double) temperature - (double) minTempExpected) / ((double) maxTempExpected - (double) minTempExpected)), 0.0f, 0.5f)) * 171.0) / 360.0), 1f, 1f);
  }

  public Color NormalizedTemperature(float temperature)
  {
    int index1 = 0;
    int index2 = 0;
    for (int index3 = 0; index3 < this.temperatureThresholds.Length; ++index3)
    {
      if ((double) temperature <= (double) this.temperatureThresholds[index3].value)
      {
        index2 = index3;
        break;
      }
      index1 = index3;
      index2 = index3;
    }
    float num1 = 0.0f;
    if (index1 != index2)
      num1 = (float) (((double) temperature - (double) this.temperatureThresholds[index1].value) / ((double) this.temperatureThresholds[index2].value - (double) this.temperatureThresholds[index1].value));
    float num2 = Mathf.Min(Mathf.Max(num1, 0.0f), 1f);
    return Color.Lerp(Color32.op_Implicit(GlobalAssets.Instance.colorSet.GetColorByName(this.temperatureThresholds[index1].colorName)), Color32.op_Implicit(GlobalAssets.Instance.colorSet.GetColorByName(this.temperatureThresholds[index2].colorName)), num2);
  }

  public Color NormalizedHeatFlow(int cell)
  {
    int index1 = 0;
    int index2 = 0;
    float thermalComfort = GameUtil.GetThermalComfort(cell);
    for (int index3 = 0; index3 < this.heatFlowThresholds.Length; ++index3)
    {
      if ((double) thermalComfort <= (double) this.heatFlowThresholds[index3].value)
      {
        index2 = index3;
        break;
      }
      index1 = index3;
      index2 = index3;
    }
    float num1 = 0.0f;
    if (index1 != index2)
      num1 = (float) (((double) thermalComfort - (double) this.heatFlowThresholds[index1].value) / ((double) this.heatFlowThresholds[index2].value - (double) this.heatFlowThresholds[index1].value));
    float num2 = Mathf.Min(Mathf.Max(num1, 0.0f), 1f);
    Color color = Color.Lerp(Color32.op_Implicit(GlobalAssets.Instance.colorSet.GetColorByName(this.heatFlowThresholds[index1].colorName)), Color32.op_Implicit(GlobalAssets.Instance.colorSet.GetColorByName(this.heatFlowThresholds[index2].colorName)), num2);
    if (Grid.Solid[cell])
      color = Color.black;
    return color;
  }

  private static bool IsInsulated(int cell) => (Grid.Element[cell].state & Element.State.TemperatureInsulated) != 0;

  private static Color GetDiseaseColour(SimDebugView instance, int cell)
  {
    Color diseaseColour = Color.black;
    if (Grid.DiseaseIdx[cell] != byte.MaxValue)
    {
      diseaseColour = Color32.op_Implicit(GlobalAssets.Instance.colorSet.GetColorByName(Db.Get().Diseases[(int) Grid.DiseaseIdx[cell]].overlayColourName));
      diseaseColour.a = SimUtil.DiseaseCountToAlpha(Grid.DiseaseCount[cell]);
    }
    else
      diseaseColour.a = 0.0f;
    return diseaseColour;
  }

  private static Color GetHeatFlowColour(SimDebugView instance, int cell) => instance.NormalizedHeatFlow(cell);

  private static Color GetBlack(SimDebugView instance, int cell) => Color.black;

  public static Color GetLightColour(SimDebugView instance, int cell)
  {
    Color lightColour = Color32.op_Implicit(GlobalAssets.Instance.colorSet.lightOverlay);
    lightColour.a = Mathf.Clamp(Mathf.Sqrt((float) (Grid.LightIntensity[cell] + LightGridManager.previewLux[cell])) / Mathf.Sqrt(80000f), 0.0f, 1f);
    if (Grid.LightIntensity[cell] > 72000)
    {
      float num = (float) (((double) Grid.LightIntensity[cell] + (double) LightGridManager.previewLux[cell] - 72000.0) / 8000.0) / 10f;
      lightColour.r += Mathf.Min(0.1f, PerlinSimplexNoise.noise(Grid.CellToPos2D(cell).x / 8f, (float) ((double) Grid.CellToPos2D(cell).y / 8.0 + (double) instance.currentFrame / 32.0)) * num);
    }
    return lightColour;
  }

  public static Color GetRadiationColour(SimDebugView instance, int cell) => new Color(0.2f, 0.9f, 0.3f, Mathf.Clamp(Mathf.Sqrt(Grid.Radiation[cell]) / 30f, 0.0f, 1f));

  public static Color GetRoomsColour(SimDebugView instance, int cell)
  {
    Color roomsColour = Color.black;
    if (Grid.IsValidCell(instance.selectedCell))
    {
      CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(cell);
      if (cavityForCell != null && cavityForCell.room != null)
      {
        roomsColour = Color32.op_Implicit(GlobalAssets.Instance.colorSet.GetColorByName(cavityForCell.room.roomType.category.colorName));
        roomsColour.a = 0.45f;
        if (Game.Instance.roomProber.GetCavityForCell(instance.selectedCell) == cavityForCell)
          roomsColour.a += 0.3f;
      }
    }
    return roomsColour;
  }

  public static Color GetJoulesColour(SimDebugView instance, int cell) => Color.Lerp(Color.black, Color.red, (float) (0.5 * ((double) Grid.Element[cell].specificHeatCapacity * (double) Grid.Temperature[cell] * ((double) Grid.Mass[cell] * 1000.0)) / ((double) ElementLoader.FindElementByHash(SimHashes.SandStone).specificHeatCapacity * 294.0 * 1000000.0)));

  public static Color GetNormalizedTemperatureColourMode(SimDebugView instance, int cell)
  {
    switch (Game.Instance.temperatureOverlayMode)
    {
      case Game.TemperatureOverlayModes.AbsoluteTemperature:
        return SimDebugView.GetNormalizedTemperatureColour(instance, cell);
      case Game.TemperatureOverlayModes.AdaptiveTemperature:
        return SimDebugView.GetNormalizedTemperatureColour(instance, cell);
      case Game.TemperatureOverlayModes.HeatFlow:
        return SimDebugView.GetHeatFlowColour(instance, cell);
      case Game.TemperatureOverlayModes.StateChange:
        return SimDebugView.GetStateChangeProximityColour(instance, cell);
      default:
        return SimDebugView.GetNormalizedTemperatureColour(instance, cell);
    }
  }

  public static Color GetStateChangeProximityColour(SimDebugView instance, int cell)
  {
    float temperature = Grid.Temperature[cell];
    Element element = Grid.Element[cell];
    float lowTemp = element.lowTemp;
    float highTemp = element.highTemp;
    if (element.IsGas)
    {
      float maxTempExpected = Mathf.Min(lowTemp + 150f, highTemp);
      return SimDebugView.GasTemperatureToColor(temperature, lowTemp, maxTempExpected);
    }
    if (!element.IsSolid)
      return SimDebugView.TemperatureToColor(temperature, lowTemp, highTemp);
    float minTempExpected = Mathf.Max(highTemp - 150f, lowTemp);
    return SimDebugView.SolidTemperatureToColor(temperature, minTempExpected, highTemp);
  }

  public static Color GetNormalizedTemperatureColour(SimDebugView instance, int cell)
  {
    float temperature = Grid.Temperature[cell];
    return instance.NormalizedTemperature(temperature);
  }

  private static Color GetGameGridColour(SimDebugView instance, int cell)
  {
    Color gameGridColour = Color32.op_Implicit(new Color32((byte) 0, (byte) 0, (byte) 0, byte.MaxValue));
    switch (instance.gameGridMode)
    {
      case SimDebugView.GameGridMode.GameSolidMap:
        gameGridColour = Grid.Solid[cell] ? Color.white : Color.black;
        break;
      case SimDebugView.GameGridMode.Lighting:
        gameGridColour = Grid.LightCount[cell] > 0 || LightGridManager.previewLux[cell] > 0 ? Color.white : Color.black;
        break;
      case SimDebugView.GameGridMode.DigAmount:
        if (Grid.Element[cell].IsSolid)
        {
          gameGridColour = Color.HSVToRGB(1f - Grid.Damage[cell] / (float) byte.MaxValue, 1f, 1f);
          break;
        }
        break;
      case SimDebugView.GameGridMode.DupePassable:
        gameGridColour = Grid.DupePassable[cell] ? Color.white : Color.black;
        break;
    }
    return gameGridColour;
  }

  public Color32 GetColourForID(int id) => this.networkColours[id % this.networkColours.Length];

  private static Color GetThermalConductivityColour(SimDebugView instance, int cell)
  {
    int num1 = SimDebugView.IsInsulated(cell) ? 1 : 0;
    Color black = Color.black;
    float num2 = instance.maxThermalConductivity - instance.minThermalConductivity;
    if (num1 == 0 && (double) num2 != 0.0)
    {
      float num3 = Mathf.Min(Mathf.Max((Grid.Element[cell].thermalConductivity - instance.minThermalConductivity) / num2, 0.0f), 1f);
      // ISSUE: explicit constructor call
      ((Color) ref black).\u002Ector(num3, num3, num3);
    }
    return black;
  }

  private static Color GetPressureMapColour(SimDebugView instance, int cell)
  {
    Color32 color32 = Color32.op_Implicit(Color.black);
    if ((double) Grid.Pressure[cell] > 0.0)
    {
      double num = (double) Mathf.Clamp((float) (((double) Grid.Pressure[cell] - (double) instance.minPressureExpected) / ((double) instance.maxPressureExpected - (double) instance.minPressureExpected)), 0.0f, 1f) * 0.89999997615814209;
      color32 = Color32.op_Implicit(new Color((float) num, (float) num, (float) num, 1f));
    }
    return Color32.op_Implicit(color32);
  }

  private static Color GetOxygenMapColour(SimDebugView instance, int cell)
  {
    Color oxygenMapColour = Color.black;
    if (!Grid.IsLiquid(cell) && !Grid.Solid[cell])
    {
      if ((double) Grid.Mass[cell] > (double) SimDebugView.minimumBreathable && (Grid.Element[cell].id == SimHashes.Oxygen || Grid.Element[cell].id == SimHashes.ContaminatedOxygen))
      {
        float num = Mathf.Clamp((Grid.Mass[cell] - SimDebugView.minimumBreathable) / SimDebugView.optimallyBreathable, 0.0f, 1f);
        oxygenMapColour = instance.breathableGradient.Evaluate(num);
      }
      else
        oxygenMapColour = Color32.op_Implicit(instance.unbreathableColour);
    }
    return oxygenMapColour;
  }

  private static Color GetTileColour(SimDebugView instance, int cell)
  {
    float num = 0.33f;
    Color tileColour;
    // ISSUE: explicit constructor call
    ((Color) ref tileColour).\u002Ector(num, num, num);
    Element element = Grid.Element[cell];
    bool flag = false;
    foreach (Tag tileOverlayFilter in Game.Instance.tileOverlayFilters)
    {
      if (element.HasTag(tileOverlayFilter))
        flag = true;
    }
    if (flag)
      tileColour = Color32.op_Implicit(element.substance.uiColour);
    return tileColour;
  }

  private static Color GetTileTypeColour(SimDebugView instance, int cell) => Color32.op_Implicit(Grid.Element[cell].substance.uiColour);

  private static Color GetStateMapColour(SimDebugView instance, int cell)
  {
    Color stateMapColour = Color.black;
    switch (Grid.Element[cell].state & Element.State.Solid)
    {
      case Element.State.Gas:
        stateMapColour = Color.yellow;
        break;
      case Element.State.Liquid:
        stateMapColour = Color.green;
        break;
      case Element.State.Solid:
        stateMapColour = Color.blue;
        break;
    }
    return stateMapColour;
  }

  private static Color GetSolidLiquidMapColour(SimDebugView instance, int cell)
  {
    Color solidLiquidMapColour = Color.black;
    switch (Grid.Element[cell].state & Element.State.Solid)
    {
      case Element.State.Liquid:
        solidLiquidMapColour = Color.green;
        break;
      case Element.State.Solid:
        solidLiquidMapColour = Color.blue;
        break;
    }
    return solidLiquidMapColour;
  }

  private static Color GetStateChangeColour(SimDebugView instance, int cell)
  {
    Color stateChangeColour = Color.black;
    Element element = Grid.Element[cell];
    if (!element.IsVacuum)
    {
      double num1 = (double) Grid.Temperature[cell];
      float num2 = element.lowTemp * 0.05f;
      float num3 = Mathf.Abs((float) num1 - element.lowTemp) / num2;
      float num4 = element.highTemp * 0.05f;
      float num5 = Mathf.Abs((float) num1 - element.highTemp) / num4;
      stateChangeColour = Color.Lerp(Color.black, Color.red, Mathf.Max(0.0f, 1f - Mathf.Min(num3, num5)));
    }
    return stateChangeColour;
  }

  private static Color GetDecorColour(SimDebugView instance, int cell)
  {
    Color decorColour = Color.black;
    if (!Grid.Solid[cell])
    {
      float num = GameUtil.GetDecorAtCell(cell) / 100f;
      decorColour = (double) num <= 0.0 ? Color.Lerp(Color32.op_Implicit(GlobalAssets.Instance.colorSet.decorBaseline), Color32.op_Implicit(GlobalAssets.Instance.colorSet.decorNegative), Mathf.Abs(num)) : Color.Lerp(Color32.op_Implicit(GlobalAssets.Instance.colorSet.decorBaseline), Color32.op_Implicit(GlobalAssets.Instance.colorSet.decorPositive), Mathf.Abs(num));
    }
    return decorColour;
  }

  private static Color GetDangerColour(SimDebugView instance, int cell)
  {
    Color dangerColour = Color.black;
    SimDebugView.DangerAmount dangerAmount = SimDebugView.DangerAmount.None;
    if (!Grid.Element[cell].IsSolid)
    {
      float num = 0.0f;
      if ((double) Grid.Temperature[cell] < (double) SimDebugView.minMinionTemperature)
        num = Mathf.Abs(Grid.Temperature[cell] - SimDebugView.minMinionTemperature);
      if ((double) Grid.Temperature[cell] > (double) SimDebugView.maxMinionTemperature)
        num = Mathf.Abs(Grid.Temperature[cell] - SimDebugView.maxMinionTemperature);
      if ((double) num > 0.0)
      {
        if ((double) num < 10.0)
          dangerAmount = SimDebugView.DangerAmount.VeryLow;
        else if ((double) num < 30.0)
          dangerAmount = SimDebugView.DangerAmount.Low;
        else if ((double) num < 100.0)
          dangerAmount = SimDebugView.DangerAmount.Moderate;
        else if ((double) num < 200.0)
          dangerAmount = SimDebugView.DangerAmount.High;
        else if ((double) num < 400.0)
          dangerAmount = SimDebugView.DangerAmount.VeryHigh;
        else if ((double) num > 800.0)
          dangerAmount = SimDebugView.DangerAmount.Extreme;
      }
    }
    if (dangerAmount < SimDebugView.DangerAmount.VeryHigh && (Grid.Element[cell].IsVacuum || Grid.Element[cell].IsGas && (Grid.Element[cell].id != SimHashes.Oxygen || (double) Grid.Pressure[cell] < (double) SimDebugView.minMinionPressure)))
      ++dangerAmount;
    if (dangerAmount != SimDebugView.DangerAmount.None)
      dangerColour = Color.HSVToRGB((float) ((80.0 - (double) ((float) dangerAmount / 6f) * 80.0) / 360.0), 1f, 1f);
    return dangerColour;
  }

  private static Color GetSimCheckErrorMapColour(SimDebugView instance, int cell)
  {
    Color checkErrorMapColour = Color.black;
    Element element = Grid.Element[cell];
    float f1 = Grid.Mass[cell];
    float f2 = Grid.Temperature[cell];
    if (float.IsNaN(f1) || float.IsNaN(f2) || (double) f1 > 10000.0 || (double) f2 > 10000.0)
      return Color.red;
    if (element.IsVacuum)
      checkErrorMapColour = (double) f2 == 0.0 ? ((double) f1 == 0.0 ? Color.gray : Color.blue) : Color.yellow;
    else if ((double) f2 < 10.0)
      checkErrorMapColour = Color.red;
    else if ((double) Grid.Mass[cell] < 1.0 && (double) Grid.Pressure[cell] < 1.0)
      checkErrorMapColour = Color.green;
    else if ((double) f2 > (double) element.highTemp + 3.0 && element.highTempTransition != null)
      checkErrorMapColour = Color.magenta;
    else if ((double) f2 < (double) element.lowTemp + 3.0 && element.lowTempTransition != null)
      checkErrorMapColour = Color.cyan;
    return checkErrorMapColour;
  }

  private static Color GetFakeFloorColour(SimDebugView instance, int cell) => !Grid.FakeFloor[cell] ? Color.black : Color.cyan;

  private static Color GetFoundationColour(SimDebugView instance, int cell) => !Grid.Foundation[cell] ? Color.black : Color.white;

  private static Color GetDupePassableColour(SimDebugView instance, int cell) => !Grid.DupePassable[cell] ? Color.black : Color.green;

  private static Color GetCritterImpassableColour(SimDebugView instance, int cell) => !Grid.CritterImpassable[cell] ? Color.black : Color.yellow;

  private static Color GetDupeImpassableColour(SimDebugView instance, int cell) => !Grid.DupeImpassable[cell] ? Color.black : Color.red;

  private static Color GetMinionOccupiedColour(SimDebugView instance, int cell) => !Object.op_Inequality((Object) Grid.Objects[cell, 0], (Object) null) ? Color.black : Color.white;

  private static Color GetMinionGroupProberColour(SimDebugView instance, int cell) => !MinionGroupProber.Get().IsReachable(cell) ? Color.black : Color.white;

  private static Color GetPathProberColour(SimDebugView instance, int cell) => !Object.op_Inequality((Object) instance.selectedPathProber, (Object) null) || instance.selectedPathProber.GetCost(cell) == -1 ? Color.black : Color.white;

  private static Color GetReservedColour(SimDebugView instance, int cell) => !Grid.Reserved[cell] ? Color.black : Color.white;

  private static Color GetAllowPathFindingColour(SimDebugView instance, int cell) => !Grid.AllowPathfinding[cell] ? Color.black : Color.white;

  private static Color GetMassColour(SimDebugView instance, int cell)
  {
    Color massColour = Color.black;
    if (!SimDebugView.IsInsulated(cell))
    {
      float num = Grid.Mass[cell];
      if ((double) num > 0.0)
        massColour = Color.HSVToRGB(1f - (float) (((double) num - (double) SimDebugView.Instance.minMassExpected) / ((double) SimDebugView.Instance.maxMassExpected - (double) SimDebugView.Instance.minMassExpected)), 1f, 1f);
    }
    return massColour;
  }

  public static Color GetScenePartitionerColour(SimDebugView instance, int cell) => !GameScenePartitioner.Instance.DoDebugLayersContainItemsOnCell(cell) ? Color.black : Color.white;

  public SimDebugView()
  {
    Dictionary<HashedString, Action<SimDebugView, Texture>> dictionary1 = new Dictionary<HashedString, Action<SimDebugView, Texture>>();
    dictionary1.Add(global::OverlayModes.Temperature.ID, new Action<SimDebugView, Texture>(SimDebugView.SetDefaultBilinear));
    dictionary1.Add(global::OverlayModes.Oxygen.ID, new Action<SimDebugView, Texture>(SimDebugView.SetDefaultBilinear));
    dictionary1.Add(global::OverlayModes.Decor.ID, new Action<SimDebugView, Texture>(SimDebugView.SetDefaultBilinear));
    dictionary1.Add(global::OverlayModes.TileMode.ID, new Action<SimDebugView, Texture>(SimDebugView.SetDefaultPoint));
    dictionary1.Add(global::OverlayModes.Disease.ID, new Action<SimDebugView, Texture>(SimDebugView.SetDisease));
    this.dataUpdateFuncs = dictionary1;
    Dictionary<HashedString, Func<SimDebugView, int, Color>> dictionary2 = new Dictionary<HashedString, Func<SimDebugView, int, Color>>();
    dictionary2.Add(global::OverlayModes.ThermalConductivity.ID, new Func<SimDebugView, int, Color>(SimDebugView.GetThermalConductivityColour));
    dictionary2.Add(global::OverlayModes.Temperature.ID, new Func<SimDebugView, int, Color>(SimDebugView.GetNormalizedTemperatureColourMode));
    dictionary2.Add(global::OverlayModes.Disease.ID, new Func<SimDebugView, int, Color>(SimDebugView.GetDiseaseColour));
    dictionary2.Add(global::OverlayModes.Decor.ID, new Func<SimDebugView, int, Color>(SimDebugView.GetDecorColour));
    dictionary2.Add(global::OverlayModes.Oxygen.ID, new Func<SimDebugView, int, Color>(SimDebugView.GetOxygenMapColour));
    dictionary2.Add(global::OverlayModes.Light.ID, new Func<SimDebugView, int, Color>(SimDebugView.GetLightColour));
    dictionary2.Add(global::OverlayModes.Radiation.ID, new Func<SimDebugView, int, Color>(SimDebugView.GetRadiationColour));
    dictionary2.Add(global::OverlayModes.Rooms.ID, new Func<SimDebugView, int, Color>(SimDebugView.GetRoomsColour));
    dictionary2.Add(global::OverlayModes.TileMode.ID, new Func<SimDebugView, int, Color>(SimDebugView.GetTileColour));
    dictionary2.Add(global::OverlayModes.Suit.ID, new Func<SimDebugView, int, Color>(SimDebugView.GetBlack));
    dictionary2.Add(global::OverlayModes.Priorities.ID, new Func<SimDebugView, int, Color>(SimDebugView.GetBlack));
    dictionary2.Add(global::OverlayModes.Crop.ID, new Func<SimDebugView, int, Color>(SimDebugView.GetBlack));
    dictionary2.Add(global::OverlayModes.Harvest.ID, new Func<SimDebugView, int, Color>(SimDebugView.GetBlack));
    dictionary2.Add(SimDebugView.OverlayModes.GameGrid, new Func<SimDebugView, int, Color>(SimDebugView.GetGameGridColour));
    dictionary2.Add(SimDebugView.OverlayModes.StateChange, new Func<SimDebugView, int, Color>(SimDebugView.GetStateChangeColour));
    dictionary2.Add(SimDebugView.OverlayModes.SimCheckErrorMap, new Func<SimDebugView, int, Color>(SimDebugView.GetSimCheckErrorMapColour));
    dictionary2.Add(SimDebugView.OverlayModes.Foundation, new Func<SimDebugView, int, Color>(SimDebugView.GetFoundationColour));
    dictionary2.Add(SimDebugView.OverlayModes.FakeFloor, new Func<SimDebugView, int, Color>(SimDebugView.GetFakeFloorColour));
    dictionary2.Add(SimDebugView.OverlayModes.DupePassable, new Func<SimDebugView, int, Color>(SimDebugView.GetDupePassableColour));
    dictionary2.Add(SimDebugView.OverlayModes.DupeImpassable, new Func<SimDebugView, int, Color>(SimDebugView.GetDupeImpassableColour));
    dictionary2.Add(SimDebugView.OverlayModes.CritterImpassable, new Func<SimDebugView, int, Color>(SimDebugView.GetCritterImpassableColour));
    dictionary2.Add(SimDebugView.OverlayModes.MinionGroupProber, new Func<SimDebugView, int, Color>(SimDebugView.GetMinionGroupProberColour));
    dictionary2.Add(SimDebugView.OverlayModes.PathProber, new Func<SimDebugView, int, Color>(SimDebugView.GetPathProberColour));
    dictionary2.Add(SimDebugView.OverlayModes.Reserved, new Func<SimDebugView, int, Color>(SimDebugView.GetReservedColour));
    dictionary2.Add(SimDebugView.OverlayModes.AllowPathFinding, new Func<SimDebugView, int, Color>(SimDebugView.GetAllowPathFindingColour));
    dictionary2.Add(SimDebugView.OverlayModes.Danger, new Func<SimDebugView, int, Color>(SimDebugView.GetDangerColour));
    dictionary2.Add(SimDebugView.OverlayModes.MinionOccupied, new Func<SimDebugView, int, Color>(SimDebugView.GetMinionOccupiedColour));
    dictionary2.Add(SimDebugView.OverlayModes.Pressure, new Func<SimDebugView, int, Color>(SimDebugView.GetPressureMapColour));
    dictionary2.Add(SimDebugView.OverlayModes.TileType, new Func<SimDebugView, int, Color>(SimDebugView.GetTileTypeColour));
    dictionary2.Add(SimDebugView.OverlayModes.State, new Func<SimDebugView, int, Color>(SimDebugView.GetStateMapColour));
    dictionary2.Add(SimDebugView.OverlayModes.SolidLiquid, new Func<SimDebugView, int, Color>(SimDebugView.GetSolidLiquidMapColour));
    dictionary2.Add(SimDebugView.OverlayModes.Mass, new Func<SimDebugView, int, Color>(SimDebugView.GetMassColour));
    dictionary2.Add(SimDebugView.OverlayModes.Joules, new Func<SimDebugView, int, Color>(SimDebugView.GetJoulesColour));
    dictionary2.Add(SimDebugView.OverlayModes.ScenePartitioner, new Func<SimDebugView, int, Color>(SimDebugView.GetScenePartitionerColour));
    this.getColourFuncs = dictionary2;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  public static class OverlayModes
  {
    public static readonly HashedString Mass = HashedString.op_Implicit(nameof (Mass));
    public static readonly HashedString Pressure = HashedString.op_Implicit(nameof (Pressure));
    public static readonly HashedString GameGrid = HashedString.op_Implicit(nameof (GameGrid));
    public static readonly HashedString ScenePartitioner = HashedString.op_Implicit(nameof (ScenePartitioner));
    public static readonly HashedString ConduitUpdates = HashedString.op_Implicit(nameof (ConduitUpdates));
    public static readonly HashedString Flow = HashedString.op_Implicit(nameof (Flow));
    public static readonly HashedString StateChange = HashedString.op_Implicit(nameof (StateChange));
    public static readonly HashedString SimCheckErrorMap = HashedString.op_Implicit(nameof (SimCheckErrorMap));
    public static readonly HashedString DupePassable = HashedString.op_Implicit(nameof (DupePassable));
    public static readonly HashedString Foundation = HashedString.op_Implicit(nameof (Foundation));
    public static readonly HashedString FakeFloor = HashedString.op_Implicit(nameof (FakeFloor));
    public static readonly HashedString CritterImpassable = HashedString.op_Implicit(nameof (CritterImpassable));
    public static readonly HashedString DupeImpassable = HashedString.op_Implicit(nameof (DupeImpassable));
    public static readonly HashedString MinionGroupProber = HashedString.op_Implicit(nameof (MinionGroupProber));
    public static readonly HashedString PathProber = HashedString.op_Implicit(nameof (PathProber));
    public static readonly HashedString Reserved = HashedString.op_Implicit(nameof (Reserved));
    public static readonly HashedString AllowPathFinding = HashedString.op_Implicit(nameof (AllowPathFinding));
    public static readonly HashedString Danger = HashedString.op_Implicit(nameof (Danger));
    public static readonly HashedString MinionOccupied = HashedString.op_Implicit(nameof (MinionOccupied));
    public static readonly HashedString TileType = HashedString.op_Implicit(nameof (TileType));
    public static readonly HashedString State = HashedString.op_Implicit(nameof (State));
    public static readonly HashedString SolidLiquid = HashedString.op_Implicit(nameof (SolidLiquid));
    public static readonly HashedString Joules = HashedString.op_Implicit(nameof (Joules));
  }

  public enum GameGridMode
  {
    GameSolidMap,
    Lighting,
    RoomMap,
    Style,
    PlantDensity,
    DigAmount,
    DupePassable,
  }

  [Serializable]
  public struct ColorThreshold
  {
    public string colorName;
    public float value;
  }

  private struct UpdateSimViewSharedData
  {
    public SimDebugView instance;
    public HashedString simViewMode;
    public SimDebugView simDebugView;
    public byte[] textureBytes;

    public UpdateSimViewSharedData(
      SimDebugView instance,
      byte[] texture_bytes,
      HashedString sim_view_mode,
      SimDebugView sim_debug_view)
    {
      this.instance = instance;
      this.textureBytes = texture_bytes;
      this.simViewMode = sim_view_mode;
      this.simDebugView = sim_debug_view;
    }
  }

  private struct UpdateSimViewWorkItem : IWorkItem<SimDebugView.UpdateSimViewSharedData>
  {
    private int x0;
    private int y0;
    private int x1;
    private int y1;

    public UpdateSimViewWorkItem(int x0, int y0, int x1, int y1)
    {
      this.x0 = Mathf.Clamp(x0, 0, Grid.WidthInCells - 1);
      this.x1 = Mathf.Clamp(x1, 0, Grid.WidthInCells - 1);
      this.y0 = Mathf.Clamp(y0, 0, Grid.HeightInCells - 1);
      this.y1 = Mathf.Clamp(y1, 0, Grid.HeightInCells - 1);
    }

    public void Run(SimDebugView.UpdateSimViewSharedData shared_data)
    {
      Func<SimDebugView, int, Color> func;
      if (!shared_data.instance.getColourFuncs.TryGetValue(shared_data.simViewMode, out func))
        func = new Func<SimDebugView, int, Color>(SimDebugView.GetBlack);
      for (int y0 = this.y0; y0 <= this.y1; ++y0)
      {
        int cell1 = Grid.XYToCell(this.x0, y0);
        int cell2 = Grid.XYToCell(this.x1, y0);
        for (int cell3 = cell1; cell3 <= cell2; ++cell3)
        {
          int index = cell3 * 4;
          if (Grid.IsActiveWorld(cell3))
          {
            Color color = func(shared_data.instance, cell3);
            shared_data.textureBytes[index] = (byte) ((double) Mathf.Min(color.r, 1f) * (double) byte.MaxValue);
            shared_data.textureBytes[index + 1] = (byte) ((double) Mathf.Min(color.g, 1f) * (double) byte.MaxValue);
            shared_data.textureBytes[index + 2] = (byte) ((double) Mathf.Min(color.b, 1f) * (double) byte.MaxValue);
            shared_data.textureBytes[index + 3] = (byte) ((double) Mathf.Min(color.a, 1f) * (double) byte.MaxValue);
          }
          else
          {
            shared_data.textureBytes[index] = (byte) 0;
            shared_data.textureBytes[index + 1] = (byte) 0;
            shared_data.textureBytes[index + 2] = (byte) 0;
            shared_data.textureBytes[index + 3] = (byte) 0;
          }
        }
      }
    }
  }

  public enum DangerAmount
  {
    None = 0,
    VeryLow = 1,
    Low = 2,
    Moderate = 3,
    High = 4,
    VeryHigh = 5,
    Extreme = 6,
    MAX_DANGERAMOUNT = 6,
  }
}
