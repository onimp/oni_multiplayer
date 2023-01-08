// Decompiled with JetBrains decompiler
// Type: TerrainBG
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/TerrainBG")]
public class TerrainBG : KMonoBehaviour
{
  public Material starsMaterial_surface;
  public Material starsMaterial_orbit;
  public Material starsMaterial_space;
  public Material backgroundMaterial;
  public Material gasMaterial;
  public bool doDraw = true;
  [SerializeField]
  private Texture3D noiseVolume;
  private Mesh starsPlane;
  private Mesh worldPlane;
  private Mesh gasPlane;
  private int layer;
  private MaterialPropertyBlock[] propertyBlocks;

  protected virtual void OnSpawn()
  {
    this.layer = LayerMask.NameToLayer("Default");
    this.noiseVolume = this.CreateTexture3D(32);
    this.starsPlane = this.CreateStarsPlane("StarsPlane");
    this.worldPlane = this.CreateWorldPlane("WorldPlane");
    this.gasPlane = this.CreateGasPlane("GasPlane");
    this.propertyBlocks = new MaterialPropertyBlock[Lighting.Instance.Settings.BackgroundLayers];
    for (int index = 0; index < this.propertyBlocks.Length; ++index)
      this.propertyBlocks[index] = new MaterialPropertyBlock();
  }

  private Texture3D CreateTexture3D(int size)
  {
    Color32[] color32Array = new Color32[size * size * size];
    Texture3D texture3D = new Texture3D(size, size, size, (TextureFormat) 4, true);
    for (int index1 = 0; index1 < size; ++index1)
    {
      for (int index2 = 0; index2 < size; ++index2)
      {
        for (int index3 = 0; index3 < size; ++index3)
        {
          Color32 color32;
          // ISSUE: explicit constructor call
          ((Color32) ref color32).\u002Ector((byte) Random.Range(0, (int) byte.MaxValue), (byte) Random.Range(0, (int) byte.MaxValue), (byte) Random.Range(0, (int) byte.MaxValue), (byte) Random.Range(0, (int) byte.MaxValue));
          color32Array[index1 + index2 * size + index3 * size * size] = color32;
        }
      }
    }
    texture3D.SetPixels32(color32Array);
    texture3D.Apply();
    return texture3D;
  }

  public Mesh CreateGasPlane(string name)
  {
    Mesh gasPlane = new Mesh();
    ((Object) gasPlane).name = name;
    int length = 4;
    Vector3[] vector3Array1 = new Vector3[length];
    Vector2[] vector2Array1 = new Vector2[length];
    int[] numArray1 = new int[6];
    Vector3[] vector3Array2 = new Vector3[4]
    {
      new Vector3(0.0f, 0.0f, 0.0f),
      new Vector3((float) Grid.WidthInCells, 0.0f, 0.0f),
      new Vector3(0.0f, Grid.HeightInMeters, 0.0f),
      new Vector3(Grid.WidthInMeters, Grid.HeightInMeters, 0.0f)
    };
    Vector2[] vector2Array2 = new Vector2[4]
    {
      new Vector2(0.0f, 0.0f),
      new Vector2(1f, 0.0f),
      new Vector2(0.0f, 1f),
      new Vector2(1f, 1f)
    };
    int[] numArray2 = new int[6]{ 0, 2, 1, 1, 2, 3 };
    gasPlane.vertices = vector3Array2;
    gasPlane.uv = vector2Array2;
    gasPlane.triangles = numArray2;
    gasPlane.bounds = new Bounds(new Vector3((float) Grid.WidthInCells * 0.5f, (float) Grid.HeightInCells * 0.5f, 0.0f), new Vector3((float) Grid.WidthInCells, (float) Grid.HeightInCells, 0.0f));
    return gasPlane;
  }

  public Mesh CreateWorldPlane(string name)
  {
    Mesh worldPlane = new Mesh();
    ((Object) worldPlane).name = name;
    int length = 4;
    Vector3[] vector3Array1 = new Vector3[length];
    Vector2[] vector2Array1 = new Vector2[length];
    int[] numArray1 = new int[6];
    Vector3[] vector3Array2 = new Vector3[4]
    {
      new Vector3((float) -Grid.WidthInCells, (float) -Grid.HeightInCells, 0.0f),
      new Vector3((float) Grid.WidthInCells * 2f, (float) -Grid.HeightInCells, 0.0f),
      new Vector3((float) -Grid.WidthInCells, Grid.HeightInMeters * 2f, 0.0f),
      new Vector3(Grid.WidthInMeters * 2f, Grid.HeightInMeters * 2f, 0.0f)
    };
    Vector2[] vector2Array2 = new Vector2[4]
    {
      new Vector2(0.0f, 0.0f),
      new Vector2(1f, 0.0f),
      new Vector2(0.0f, 1f),
      new Vector2(1f, 1f)
    };
    int[] numArray2 = new int[6]{ 0, 2, 1, 1, 2, 3 };
    worldPlane.vertices = vector3Array2;
    worldPlane.uv = vector2Array2;
    worldPlane.triangles = numArray2;
    worldPlane.bounds = new Bounds(new Vector3((float) Grid.WidthInCells * 0.5f, (float) Grid.HeightInCells * 0.5f, 0.0f), new Vector3((float) Grid.WidthInCells, (float) Grid.HeightInCells, 0.0f));
    return worldPlane;
  }

  public Mesh CreateStarsPlane(string name)
  {
    Mesh starsPlane = new Mesh();
    ((Object) starsPlane).name = name;
    int length = 4;
    Vector3[] vector3Array1 = new Vector3[length];
    Vector2[] vector2Array1 = new Vector2[length];
    int[] numArray1 = new int[6];
    Vector3[] vector3Array2 = new Vector3[4]
    {
      new Vector3((float) -Grid.WidthInCells, (float) -Grid.HeightInCells, 0.0f),
      new Vector3((float) Grid.WidthInCells * 2f, (float) -Grid.HeightInCells, 0.0f),
      new Vector3((float) -Grid.WidthInCells, Grid.HeightInMeters * 2f, 0.0f),
      new Vector3(Grid.WidthInMeters * 2f, Grid.HeightInMeters * 2f, 0.0f)
    };
    Vector2[] vector2Array2 = new Vector2[4]
    {
      new Vector2(0.0f, 0.0f),
      new Vector2(1f, 0.0f),
      new Vector2(0.0f, 1f),
      new Vector2(1f, 1f)
    };
    int[] numArray2 = new int[6]{ 0, 2, 1, 1, 2, 3 };
    starsPlane.vertices = vector3Array2;
    starsPlane.uv = vector2Array2;
    starsPlane.triangles = numArray2;
    Vector2 vector2;
    // ISSUE: explicit constructor call
    ((Vector2) ref vector2).\u002Ector((float) Grid.WidthInCells, 2f * (float) Grid.HeightInCells);
    starsPlane.bounds = new Bounds(new Vector3(0.5f * vector2.x, 0.5f * vector2.y, 0.0f), new Vector3(vector2.x, vector2.y, 0.0f));
    return starsPlane;
  }

  private void LateUpdate()
  {
    if (!this.doDraw)
      return;
    Material material = this.starsMaterial_surface;
    if (ClusterManager.Instance.activeWorld.IsModuleInterior)
    {
      Clustercraft component = ((Component) ClusterManager.Instance.activeWorld).GetComponent<Clustercraft>();
      material = component.Status == Clustercraft.CraftStatus.InFlight ? (!Object.op_Inequality((Object) ClusterGrid.Instance.GetVisibleEntityOfLayerAtAdjacentCell(component.Location, EntityLayer.Asteroid), (Object) null) ? this.starsMaterial_space : this.starsMaterial_orbit) : this.starsMaterial_surface;
    }
    material.renderQueue = RenderQueues.Stars;
    material.SetTexture("_NoiseVolume", (Texture) this.noiseVolume);
    Vector3 vector3_1;
    // ISSUE: explicit constructor call
    ((Vector3) ref vector3_1).\u002Ector(0.0f, 0.0f, Grid.GetLayerZ(Grid.SceneLayer.Background) + 1f);
    Graphics.DrawMesh(this.starsPlane, vector3_1, Quaternion.identity, material, this.layer);
    this.backgroundMaterial.renderQueue = RenderQueues.Backwall;
    for (int index = 0; index < Lighting.Instance.Settings.BackgroundLayers; ++index)
    {
      if (index >= Lighting.Instance.Settings.BackgroundLayers - 1)
      {
        float num1 = (float) index / (float) (Lighting.Instance.Settings.BackgroundLayers - 1);
        float num2 = Mathf.Lerp(1f, Lighting.Instance.Settings.BackgroundDarkening, num1);
        float num3 = Mathf.Lerp(1f, Lighting.Instance.Settings.BackgroundUVScale, num1);
        float num4 = 1f;
        if (index == Lighting.Instance.Settings.BackgroundLayers - 1)
          num4 = 0.0f;
        MaterialPropertyBlock propertyBlock = this.propertyBlocks[index];
        propertyBlock.SetVector("_BackWallParameters", new Vector4(num2, Lighting.Instance.Settings.BackgroundClip, num3, num4));
        Vector3 vector3_2;
        // ISSUE: explicit constructor call
        ((Vector3) ref vector3_2).\u002Ector(0.0f, 0.0f, Grid.GetLayerZ(Grid.SceneLayer.Background));
        Graphics.DrawMesh(this.worldPlane, vector3_2, Quaternion.identity, this.backgroundMaterial, this.layer, (Camera) null, 0, propertyBlock);
      }
    }
    this.gasMaterial.renderQueue = RenderQueues.Gas;
    Vector3 vector3_3;
    // ISSUE: explicit constructor call
    ((Vector3) ref vector3_3).\u002Ector(0.0f, 0.0f, Grid.GetLayerZ(Grid.SceneLayer.Gas));
    Graphics.DrawMesh(this.gasPlane, vector3_3, Quaternion.identity, this.gasMaterial, this.layer);
    Vector3 vector3_4;
    // ISSUE: explicit constructor call
    ((Vector3) ref vector3_4).\u002Ector(0.0f, 0.0f, Grid.GetLayerZ(Grid.SceneLayer.GasFront));
    Graphics.DrawMesh(this.gasPlane, vector3_4, Quaternion.identity, this.gasMaterial, this.layer);
  }
}
