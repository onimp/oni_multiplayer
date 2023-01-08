// Decompiled with JetBrains decompiler
// Type: PrioritizableRenderer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class PrioritizableRenderer
{
  private Mesh mesh;
  private int layer;
  private Material material;
  private int prioritizableCount;
  private Vector3[] vertices;
  private Vector2[] uvs;
  private int[] triangles;
  private List<Prioritizable> prioritizables;
  private PrioritizeTool tool;

  public PrioritizeTool currentTool
  {
    get => this.tool;
    set => this.tool = value;
  }

  public PrioritizableRenderer()
  {
    this.layer = LayerMask.NameToLayer("UI");
    Shader shader = Shader.Find("Klei/Prioritizable");
    Texture2D texture = Assets.GetTexture("priority_overlay_atlas");
    this.material = new Material(shader);
    this.material.SetTexture(Shader.PropertyToID("_MainTex"), (Texture) texture);
    this.prioritizables = new List<Prioritizable>();
    this.mesh = new Mesh();
    ((Object) this.mesh).name = "Prioritizables";
    this.mesh.MarkDynamic();
  }

  public void Cleanup()
  {
    this.material = (Material) null;
    this.vertices = (Vector3[]) null;
    this.uvs = (Vector2[]) null;
    this.prioritizables = (List<Prioritizable>) null;
    this.triangles = (int[]) null;
    Object.DestroyImmediate((Object) this.mesh);
    this.mesh = (Mesh) null;
  }

  public void RenderEveryTick()
  {
    KProfiler.Region region;
    // ISSUE: explicit constructor call
    ((KProfiler.Region) ref region).\u002Ector(nameof (PrioritizableRenderer), (Object) null);
    try
    {
      if (Object.op_Equality((Object) GameScreenManager.Instance, (Object) null) || Object.op_Equality((Object) SimDebugView.Instance, (Object) null) || HashedString.op_Inequality(SimDebugView.Instance.GetMode(), OverlayModes.Priorities.ID))
        return;
      this.prioritizables.Clear();
      Vector2I min;
      Vector2I max;
      Grid.GetVisibleExtents(out min, out max);
      int height = max.y - min.y;
      int width = max.x - min.x;
      Extents extents = new Extents(min.x, min.y, width, height);
      List<ScenePartitionerEntry> gathered_entries = new List<ScenePartitionerEntry>();
      GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.prioritizableObjects, gathered_entries);
      foreach (ScenePartitionerEntry partitionerEntry in gathered_entries)
      {
        Prioritizable component = (Prioritizable) partitionerEntry.obj;
        if (Object.op_Inequality((Object) component, (Object) null) && component.showIcon && component.IsPrioritizable() && this.tool.IsActiveLayer(this.tool.GetFilterLayerFromGameObject(((Component) component).gameObject)) && component.GetMyWorldId() == ClusterManager.Instance.activeWorldId)
          this.prioritizables.Add(component);
      }
      if (this.prioritizableCount != this.prioritizables.Count)
      {
        this.prioritizableCount = this.prioritizables.Count;
        this.vertices = new Vector3[4 * this.prioritizableCount];
        this.uvs = new Vector2[4 * this.prioritizableCount];
        this.triangles = new int[6 * this.prioritizableCount];
      }
      if (this.prioritizableCount == 0)
        return;
      for (int index1 = 0; index1 < this.prioritizables.Count; ++index1)
      {
        Prioritizable prioritizable = this.prioritizables[index1];
        Vector3 vector3 = Vector3.zero;
        KAnimControllerBase component = ((Component) prioritizable).GetComponent<KAnimControllerBase>();
        vector3 = !Object.op_Inequality((Object) component, (Object) null) ? TransformExtensions.GetPosition(prioritizable.transform) : component.GetWorldPivot();
        vector3.x += prioritizable.iconOffset.x;
        vector3.y += prioritizable.iconOffset.y;
        Vector2 vector2 = Vector2.op_Multiply(new Vector2(0.2f, 0.3f), prioritizable.iconScale);
        float num1 = -5f;
        int index2 = 4 * index1;
        this.vertices[index2] = new Vector3(vector3.x - vector2.x, vector3.y - vector2.y, num1);
        this.vertices[1 + index2] = new Vector3(vector3.x - vector2.x, vector3.y + vector2.y, num1);
        this.vertices[2 + index2] = new Vector3(vector3.x + vector2.x, vector3.y - vector2.y, num1);
        this.vertices[3 + index2] = new Vector3(vector3.x + vector2.x, vector3.y + vector2.y, num1);
        double num2 = 0.10000000149011612;
        PrioritySetting masterPriority = prioritizable.GetMasterPriority();
        float num3 = -1f;
        if (masterPriority.priority_class >= PriorityScreen.PriorityClass.high)
          num3 += 9f;
        if (masterPriority.priority_class >= PriorityScreen.PriorityClass.topPriority)
          num3 += 0.0f;
        float num4 = (float) num2 * (num3 + (float) masterPriority.priority_value);
        float num5 = 0.0f;
        float num6 = (float) num2;
        float num7 = 1f;
        this.uvs[index2] = new Vector2(num4, num5);
        this.uvs[1 + index2] = new Vector2(num4, num5 + num7);
        this.uvs[2 + index2] = new Vector2(num4 + num6, num5);
        this.uvs[3 + index2] = new Vector2(num4 + num6, num5 + num7);
        int index3 = 6 * index1;
        this.triangles[index3] = index2;
        this.triangles[1 + index3] = index2 + 1;
        this.triangles[2 + index3] = index2 + 2;
        this.triangles[3 + index3] = index2 + 2;
        this.triangles[4 + index3] = index2 + 1;
        this.triangles[5 + index3] = index2 + 3;
      }
      this.mesh.Clear();
      this.mesh.vertices = this.vertices;
      this.mesh.uv = this.uvs;
      this.mesh.SetTriangles(this.triangles, 0);
      this.mesh.RecalculateBounds();
      Graphics.DrawMesh(this.mesh, Vector3.zero, Quaternion.identity, this.material, this.layer, GameScreenManager.Instance.worldSpaceCanvas.GetComponent<Canvas>().worldCamera, 0, (MaterialPropertyBlock) null, false, false);
    }
    finally
    {
      region.Dispose();
    }
  }
}
