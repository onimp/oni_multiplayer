// Decompiled with JetBrains decompiler
// Type: TextureLerper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Rendering;

public class TextureLerper
{
  private static int offsetCounter;
  public string name;
  private RenderTexture[] BlendTextures = new RenderTexture[2];
  private float BlendDt;
  private float BlendTime;
  private int BlendIdx;
  private Material Material;
  public float Speed = 1f;
  private Mesh mesh;
  private RenderTexture source;
  private RenderTexture dest;
  private GameObject meshGO;
  private GameObject cameraGO;
  private Camera textureCam;
  private float blend;

  public TextureLerper(
    Texture target_texture,
    string name,
    FilterMode filter_mode = 1,
    TextureFormat texture_format = 5)
  {
    this.name = name;
    this.Init(target_texture.width, target_texture.height, name, filter_mode, texture_format);
    this.Material.SetTexture("_TargetTex", target_texture);
  }

  private void Init(
    int width,
    int height,
    string name,
    FilterMode filter_mode,
    TextureFormat texture_format)
  {
    for (int index = 0; index < 2; ++index)
    {
      this.BlendTextures[index] = new RenderTexture(width, height, 0, TextureUtil.GetRenderTextureFormat(texture_format));
      ((Texture) this.BlendTextures[index]).filterMode = filter_mode;
      ((Object) this.BlendTextures[index]).name = name;
    }
    this.Material = new Material(Shader.Find("Klei/LerpEffect"));
    this.Material.globalIlluminationFlags = (MaterialGlobalIlluminationFlags) 0;
    this.mesh = new Mesh();
    ((Object) this.mesh).name = "LerpEffect";
    this.mesh.vertices = new Vector3[4]
    {
      new Vector3(0.0f, 0.0f, 0.0f),
      new Vector3(1f, 1f, 0.0f),
      new Vector3(0.0f, 1f, 0.0f),
      new Vector3(1f, 0.0f, 0.0f)
    };
    this.mesh.triangles = new int[6]{ 0, 1, 2, 0, 3, 1 };
    this.mesh.uv = new Vector2[4]
    {
      new Vector2(0.0f, 0.0f),
      new Vector2(1f, 1f),
      new Vector2(0.0f, 1f),
      new Vector2(1f, 0.0f)
    };
    int layer = LayerMask.NameToLayer("RTT");
    int mask = LayerMask.GetMask(new string[1]{ "RTT" });
    this.cameraGO = new GameObject();
    ((Object) this.cameraGO).name = "TextureLerper_" + name;
    this.textureCam = this.cameraGO.AddComponent<Camera>();
    TransformExtensions.SetPosition(((Component) this.textureCam).transform, new Vector3((float) TextureLerper.offsetCounter + 0.5f, 0.5f, 0.0f));
    this.textureCam.clearFlags = (CameraClearFlags) 4;
    this.textureCam.depth = -100f;
    this.textureCam.allowHDR = false;
    this.textureCam.orthographic = true;
    this.textureCam.orthographicSize = 0.5f;
    this.textureCam.cullingMask = mask;
    this.textureCam.targetTexture = this.dest;
    this.textureCam.nearClipPlane = -5f;
    this.textureCam.farClipPlane = 5f;
    this.textureCam.useOcclusionCulling = false;
    this.textureCam.aspect = 1f;
    this.textureCam.rect = new Rect(0.0f, 0.0f, 1f, 1f);
    this.meshGO = new GameObject();
    ((Object) this.meshGO).name = "mesh";
    this.meshGO.transform.parent = this.cameraGO.transform;
    TransformExtensions.SetLocalPosition(this.meshGO.transform, new Vector3(-0.5f, -0.5f, 0.0f));
    this.meshGO.isStatic = true;
    MeshRenderer meshRenderer = this.meshGO.AddComponent<MeshRenderer>();
    ((Renderer) meshRenderer).receiveShadows = false;
    ((Renderer) meshRenderer).shadowCastingMode = (ShadowCastingMode) 0;
    ((Renderer) meshRenderer).lightProbeUsage = (LightProbeUsage) 0;
    ((Renderer) meshRenderer).reflectionProbeUsage = (ReflectionProbeUsage) 0;
    this.meshGO.AddComponent<MeshFilter>().mesh = this.mesh;
    ((Renderer) meshRenderer).sharedMaterial = this.Material;
    Util.SetLayerRecursively(this.cameraGO, layer);
    ++TextureLerper.offsetCounter;
  }

  public void LongUpdate(float dt)
  {
    this.BlendDt = dt;
    this.BlendTime = 0.0f;
  }

  public Texture Update()
  {
    float num1 = Time.deltaTime * this.Speed;
    if ((double) Time.deltaTime == 0.0)
      num1 = Time.unscaledDeltaTime * this.Speed;
    float num2 = Mathf.Min(num1 / Mathf.Max(this.BlendDt - this.BlendTime, 0.0f), 1f);
    this.BlendTime += num1;
    if (GameUtil.IsCapturingTimeLapse())
      num2 = 1f;
    this.source = this.BlendTextures[this.BlendIdx];
    this.BlendIdx = (this.BlendIdx + 1) % 2;
    this.dest = this.BlendTextures[this.BlendIdx];
    Vector4 visibleCellRange = this.GetVisibleCellRange();
    // ISSUE: explicit constructor call
    ((Vector4) ref visibleCellRange).\u002Ector(0.0f, 0.0f, (float) Grid.WidthInCells, (float) Grid.HeightInCells);
    this.Material.SetFloat("_Lerp", num2);
    this.Material.SetTexture("_SourceTex", (Texture) this.source);
    this.Material.SetVector("_MeshParams", visibleCellRange);
    this.textureCam.targetTexture = this.dest;
    return (Texture) this.dest;
  }

  private Vector4 GetVisibleCellRange()
  {
    Camera main = Camera.main;
    float cellSizeInMeters = Grid.CellSizeInMeters;
    Ray ray1 = main.ViewportPointToRay(Vector3.zero);
    float num1 = Mathf.Abs(((Ray) ref ray1).origin.z / ((Ray) ref ray1).direction.z);
    int cell = Grid.PosToCell(((Ray) ref ray1).GetPoint(num1));
    float num2 = -Grid.HalfCellSizeInMeters;
    double x_offset = (double) num2;
    double y_offset = (double) num2;
    double z_offset = (double) num2;
    Vector3 pos = Grid.CellToPos(cell, (float) x_offset, (float) y_offset, (float) z_offset);
    int num3 = Math.Max(0, (int) ((double) pos.x / (double) cellSizeInMeters));
    int num4 = Math.Max(0, (int) ((double) pos.y / (double) cellSizeInMeters));
    Ray ray2 = main.ViewportPointToRay(Vector3.one);
    float num5 = Mathf.Abs(((Ray) ref ray2).origin.z / ((Ray) ref ray2).direction.z);
    Vector3 point = ((Ray) ref ray2).GetPoint(num5);
    int num6 = Mathf.CeilToInt(point.x / cellSizeInMeters);
    int num7 = Mathf.CeilToInt(point.y / cellSizeInMeters);
    int num8 = Mathf.Min(num6, Grid.WidthInCells - 1);
    int num9 = Mathf.Min(num7, Grid.HeightInCells - 1);
    return new Vector4((float) num3, (float) num4, (float) num8, (float) num9);
  }
}
