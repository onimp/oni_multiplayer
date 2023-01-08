// Decompiled with JetBrains decompiler
// Type: WaterCubes
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Rendering;

[AddComponentMenu("KMonoBehaviour/scripts/WaterCubes")]
public class WaterCubes : KMonoBehaviour
{
  public Material material;
  public Texture2D waveTexture;
  private GameObject cubes;

  public static WaterCubes Instance { get; private set; }

  public static void DestroyInstance() => WaterCubes.Instance = (WaterCubes) null;

  protected virtual void OnPrefabInit() => WaterCubes.Instance = this;

  public void Init()
  {
    this.cubes = Util.NewGameObject(((Component) this).gameObject, nameof (WaterCubes));
    GameObject gameObject = new GameObject();
    ((Object) gameObject).name = "WaterCubesMesh";
    gameObject.transform.parent = this.cubes.transform;
    this.material.renderQueue = RenderQueues.Liquid;
    MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
    MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
    ((Renderer) meshRenderer).sharedMaterial = this.material;
    ((Renderer) meshRenderer).shadowCastingMode = (ShadowCastingMode) 0;
    ((Renderer) meshRenderer).receiveShadows = false;
    ((Renderer) meshRenderer).lightProbeUsage = (LightProbeUsage) 0;
    ((Renderer) meshRenderer).reflectionProbeUsage = (ReflectionProbeUsage) 0;
    ((Renderer) meshRenderer).sharedMaterial.SetTexture("_MainTex2", (Texture) this.waveTexture);
    meshFilter.sharedMesh = this.CreateNewMesh();
    ((Component) meshRenderer).gameObject.layer = 0;
    ((Component) meshRenderer).gameObject.transform.parent = this.transform;
    TransformExtensions.SetPosition(((Component) meshRenderer).gameObject.transform, new Vector3(0.0f, 0.0f, Grid.GetLayerZ(Grid.SceneLayer.Liquid)));
  }

  private Mesh CreateNewMesh()
  {
    Mesh newMesh = new Mesh();
    ((Object) newMesh).name = nameof (WaterCubes);
    int length = 4;
    Vector3[] vector3Array1 = new Vector3[length];
    Vector2[] vector2Array1 = new Vector2[length];
    Vector3[] vector3Array2 = new Vector3[length];
    Vector4[] vector4Array1 = new Vector4[length];
    int[] numArray1 = new int[6];
    float layerZ = Grid.GetLayerZ(Grid.SceneLayer.Liquid);
    Vector3[] vector3Array3 = new Vector3[4]
    {
      new Vector3(0.0f, 0.0f, layerZ),
      new Vector3((float) Grid.WidthInCells, 0.0f, layerZ),
      new Vector3(0.0f, Grid.HeightInMeters, layerZ),
      new Vector3(Grid.WidthInMeters, Grid.HeightInMeters, layerZ)
    };
    Vector2[] vector2Array2 = new Vector2[4]
    {
      new Vector2(0.0f, 0.0f),
      new Vector2(1f, 0.0f),
      new Vector2(0.0f, 1f),
      new Vector2(1f, 1f)
    };
    Vector3[] vector3Array4 = new Vector3[4]
    {
      new Vector3(0.0f, 0.0f, -1f),
      new Vector3(0.0f, 0.0f, -1f),
      new Vector3(0.0f, 0.0f, -1f),
      new Vector3(0.0f, 0.0f, -1f)
    };
    Vector4[] vector4Array2 = new Vector4[4]
    {
      new Vector4(0.0f, 1f, 0.0f, -1f),
      new Vector4(0.0f, 1f, 0.0f, -1f),
      new Vector4(0.0f, 1f, 0.0f, -1f),
      new Vector4(0.0f, 1f, 0.0f, -1f)
    };
    int[] numArray2 = new int[6]{ 0, 2, 1, 1, 2, 3 };
    newMesh.vertices = vector3Array3;
    newMesh.uv = vector2Array2;
    newMesh.uv2 = vector2Array2;
    newMesh.normals = vector3Array4;
    newMesh.tangents = vector4Array2;
    newMesh.triangles = numArray2;
    newMesh.bounds = new Bounds(Vector3.zero, new Vector3(float.MaxValue, float.MaxValue, 0.0f));
    return newMesh;
  }
}
