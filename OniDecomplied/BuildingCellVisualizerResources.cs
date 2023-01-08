// Decompiled with JetBrains decompiler
// Type: BuildingCellVisualizerResources
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class BuildingCellVisualizerResources : ScriptableObject
{
  [Header("Electricity")]
  public Color electricityInputColor;
  public Color electricityOutputColor;
  public Sprite electricityInputIcon;
  public Sprite electricityOutputIcon;
  public Sprite electricityConnectedIcon;
  public Sprite electricityBridgeIcon;
  public Sprite electricityBridgeConnectedIcon;
  public Sprite electricityArrowIcon;
  public Sprite switchIcon;
  public Color32 switchColor;
  public Color32 switchOffColor = Color32.op_Implicit(Color.red);
  [Header("Gas")]
  public Sprite gasInputIcon;
  public Sprite gasOutputIcon;
  public BuildingCellVisualizerResources.IOColours gasIOColours;
  [Header("Liquid")]
  public Sprite liquidInputIcon;
  public Sprite liquidOutputIcon;
  public BuildingCellVisualizerResources.IOColours liquidIOColours;
  [Header("High Energy Particle")]
  public Sprite highEnergyParticleInputIcon;
  public Sprite[] highEnergyParticleOutputIcons;
  public Color highEnergyParticleInputColour;
  public Color highEnergyParticleOutputColour;
  private static BuildingCellVisualizerResources _Instance;

  public Material backgroundMaterial { get; set; }

  public Material iconBackgroundMaterial { get; set; }

  public Material powerInputMaterial { get; set; }

  public Material powerOutputMaterial { get; set; }

  public Material liquidInputMaterial { get; set; }

  public Material liquidOutputMaterial { get; set; }

  public Material gasInputMaterial { get; set; }

  public Material gasOutputMaterial { get; set; }

  public Material highEnergyParticleInputMaterial { get; set; }

  public Material highEnergyParticleOutputMaterial { get; set; }

  public Mesh backgroundMesh { get; set; }

  public Mesh iconMesh { get; set; }

  public int backgroundLayer { get; set; }

  public int iconLayer { get; set; }

  public static void DestroyInstance() => BuildingCellVisualizerResources._Instance = (BuildingCellVisualizerResources) null;

  public static BuildingCellVisualizerResources Instance()
  {
    if (Object.op_Equality((Object) BuildingCellVisualizerResources._Instance, (Object) null))
    {
      BuildingCellVisualizerResources._Instance = Resources.Load<BuildingCellVisualizerResources>(nameof (BuildingCellVisualizerResources));
      BuildingCellVisualizerResources._Instance.Initialize();
    }
    return BuildingCellVisualizerResources._Instance;
  }

  private void Initialize()
  {
    Shader shader = Shader.Find("Klei/BuildingCell");
    this.backgroundMaterial = new Material(shader);
    this.backgroundMaterial.mainTexture = (Texture) GlobalResources.Instance().WhiteTexture;
    this.iconBackgroundMaterial = new Material(shader);
    this.iconBackgroundMaterial.mainTexture = (Texture) GlobalResources.Instance().WhiteTexture;
    this.powerInputMaterial = new Material(shader);
    this.powerOutputMaterial = new Material(shader);
    this.liquidInputMaterial = new Material(shader);
    this.liquidOutputMaterial = new Material(shader);
    this.gasInputMaterial = new Material(shader);
    this.gasOutputMaterial = new Material(shader);
    this.highEnergyParticleInputMaterial = new Material(shader);
    this.highEnergyParticleOutputMaterial = new Material(shader);
    this.backgroundMesh = this.CreateMesh("BuildingCellVisualizer", Vector2.zero, 0.5f);
    this.iconMesh = this.CreateMesh("BuildingCellVisualizerIcon", Vector2.zero, 0.5f * 0.5f);
    this.backgroundLayer = LayerMask.NameToLayer("Default");
    this.iconLayer = LayerMask.NameToLayer("Place");
  }

  private Mesh CreateMesh(string name, Vector2 base_offset, float half_size)
  {
    Mesh mesh = new Mesh();
    ((Object) mesh).name = name;
    mesh.vertices = new Vector3[4]
    {
      new Vector3(-half_size + base_offset.x, -half_size + base_offset.y, 0.0f),
      new Vector3(half_size + base_offset.x, -half_size + base_offset.y, 0.0f),
      new Vector3(-half_size + base_offset.x, half_size + base_offset.y, 0.0f),
      new Vector3(half_size + base_offset.x, half_size + base_offset.y, 0.0f)
    };
    mesh.uv = new Vector2[4]
    {
      new Vector2(0.0f, 0.0f),
      new Vector2(1f, 0.0f),
      new Vector2(0.0f, 1f),
      new Vector2(1f, 1f)
    };
    mesh.triangles = new int[6]{ 0, 1, 2, 2, 1, 3 };
    mesh.RecalculateBounds();
    return mesh;
  }

  [Serializable]
  public struct ConnectedDisconnectedColours
  {
    public Color32 connected;
    public Color32 disconnected;
  }

  [Serializable]
  public struct IOColours
  {
    public BuildingCellVisualizerResources.ConnectedDisconnectedColours input;
    public BuildingCellVisualizerResources.ConnectedDisconnectedColours output;
  }
}
