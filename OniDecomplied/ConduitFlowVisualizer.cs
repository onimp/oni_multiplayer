// Decompiled with JetBrains decompiler
// Type: ConduitFlowVisualizer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ConduitFlowVisualizer
{
  private ConduitFlow flowManager;
  private EventReference overlaySound;
  private bool showContents;
  private double animTime;
  private int layer;
  private static Vector2 GRID_OFFSET = new Vector2(0.5f, 0.5f);
  private List<ConduitFlowVisualizer.AudioInfo> audioInfo;
  private HashSet<int> insulatedCells = new HashSet<int>();
  private HashSet<int> radiantCells = new HashSet<int>();
  private Game.ConduitVisInfo visInfo;
  private ConduitFlowVisualizer.ConduitFlowMesh movingBallMesh;
  private ConduitFlowVisualizer.ConduitFlowMesh staticBallMesh;
  private int highlightedCell = -1;
  private Color32 highlightColour = Color32.op_Implicit(new Color(0.2f, 0.2f, 0.2f, 0.2f));
  private ConduitFlowVisualizer.Tuning tuning;
  private static WorkItemCollection<ConduitFlowVisualizer.RenderMeshTask, ConduitFlowVisualizer.RenderMeshContext> render_mesh_job = new WorkItemCollection<ConduitFlowVisualizer.RenderMeshTask, ConduitFlowVisualizer.RenderMeshContext>();

  public ConduitFlowVisualizer(
    ConduitFlow flow_manager,
    Game.ConduitVisInfo vis_info,
    EventReference overlay_sound,
    ConduitFlowVisualizer.Tuning tuning)
  {
    this.flowManager = flow_manager;
    this.visInfo = vis_info;
    this.overlaySound = overlay_sound;
    this.tuning = tuning;
    this.movingBallMesh = new ConduitFlowVisualizer.ConduitFlowMesh();
    this.staticBallMesh = new ConduitFlowVisualizer.ConduitFlowMesh();
    ConduitFlowVisualizer.RenderMeshTask.Ball.InitializeResources();
  }

  public void FreeResources()
  {
    this.movingBallMesh.Cleanup();
    this.staticBallMesh.Cleanup();
  }

  private float CalculateMassScale(float mass) => Mathf.Lerp(this.visInfo.overlayMassScaleValues.x, this.visInfo.overlayMassScaleValues.y, (float) (((double) mass - (double) this.visInfo.overlayMassScaleRange.x) / ((double) this.visInfo.overlayMassScaleRange.y - (double) this.visInfo.overlayMassScaleRange.x)));

  private Color32 GetContentsColor(Element element, Color32 default_color)
  {
    if (element == null)
      return default_color;
    Color color = Color32.op_Implicit(element.substance.conduitColour);
    color.a = 128f;
    return Color32.op_Implicit(color);
  }

  private Color32 GetTintColour() => !this.showContents ? this.visInfo.tint : GlobalAssets.Instance.colorSet.GetColorByName(this.visInfo.overlayTintName);

  private Color32 GetInsulatedTintColour() => !this.showContents ? this.visInfo.insulatedTint : GlobalAssets.Instance.colorSet.GetColorByName(this.visInfo.overlayInsulatedTintName);

  private Color32 GetRadiantTintColour() => !this.showContents ? this.visInfo.radiantTint : GlobalAssets.Instance.colorSet.GetColorByName(this.visInfo.overlayRadiantTintName);

  private Color32 GetCellTintColour(int cell) => !this.insulatedCells.Contains(cell) ? (!this.radiantCells.Contains(cell) ? this.GetTintColour() : this.GetRadiantTintColour()) : this.GetInsulatedTintColour();

  public void Render(float z, int render_layer, float lerp_percent, bool trigger_audio = false)
  {
    this.animTime += (double) Time.deltaTime;
    if (trigger_audio)
    {
      if (this.audioInfo == null)
        this.audioInfo = new List<ConduitFlowVisualizer.AudioInfo>();
      for (int index = 0; index < this.audioInfo.Count; ++index)
      {
        ConduitFlowVisualizer.AudioInfo audioInfo = this.audioInfo[index] with
        {
          distance = float.PositiveInfinity,
          position = Vector3.zero
        };
        audioInfo.blobCount = (audioInfo.blobCount + 1) % 10;
        this.audioInfo[index] = audioInfo;
      }
    }
    if (this.tuning.renderMesh)
      this.RenderMesh(z, render_layer, lerp_percent, trigger_audio);
    if (!trigger_audio)
      return;
    this.TriggerAudio();
  }

  private void RenderMesh(float z, int render_layer, float lerp_percent, bool trigger_audio)
  {
    GridArea visibleArea = GridVisibleArea.GetVisibleArea();
    Vector2I min;
    // ISSUE: explicit constructor call
    ((Vector2I) ref min).\u002Ector(Mathf.Max(0, visibleArea.Min.x - 1), Mathf.Max(0, visibleArea.Min.y - 1));
    Vector2I max;
    // ISSUE: explicit constructor call
    ((Vector2I) ref max).\u002Ector(Mathf.Min(Grid.WidthInCells - 1, visibleArea.Max.x + 1), Mathf.Min(Grid.HeightInCells - 1, visibleArea.Max.y + 1));
    ConduitFlowVisualizer.RenderMeshContext renderMeshContext = new ConduitFlowVisualizer.RenderMeshContext(this, lerp_percent, min, max);
    if (((List<int>) renderMeshContext.visible_conduits).Count == 0)
    {
      renderMeshContext.Finish();
    }
    else
    {
      ConduitFlowVisualizer.render_mesh_job.Reset(renderMeshContext);
      int num1 = Mathf.Max(1, (int) ((double) (((List<int>) renderMeshContext.visible_conduits).Count / CPUBudget.coreCount) / 1.5));
      int num2 = Mathf.Max(1, ((List<int>) renderMeshContext.visible_conduits).Count / num1);
      for (int index = 0; index != num2; ++index)
      {
        int start = index * num1;
        int end = index == num2 - 1 ? ((List<int>) renderMeshContext.visible_conduits).Count : start + num1;
        ConduitFlowVisualizer.render_mesh_job.Add(new ConduitFlowVisualizer.RenderMeshTask(start, end));
      }
      GlobalJobManager.Run((IWorkItemCollection) ConduitFlowVisualizer.render_mesh_job);
      float num3 = 0.0f;
      if (this.showContents)
        num3 = 1f;
      float num4 = (float) ((int) (this.animTime / (1.0 / (double) this.tuning.framesPerSecond)) % (int) this.tuning.spriteCount) * (1f / this.tuning.spriteCount);
      this.movingBallMesh.Begin();
      this.movingBallMesh.SetTexture("_BackgroundTex", this.tuning.backgroundTexture);
      this.movingBallMesh.SetTexture("_ForegroundTex", this.tuning.foregroundTexture);
      this.movingBallMesh.SetVector("_SpriteSettings", new Vector4(1f / this.tuning.spriteCount, 1f, num3, num4));
      this.movingBallMesh.SetVector("_Highlight", new Vector4((float) this.highlightColour.r / (float) byte.MaxValue, (float) this.highlightColour.g / (float) byte.MaxValue, (float) this.highlightColour.b / (float) byte.MaxValue, 0.0f));
      this.staticBallMesh.Begin();
      this.staticBallMesh.SetTexture("_BackgroundTex", this.tuning.backgroundTexture);
      this.staticBallMesh.SetTexture("_ForegroundTex", this.tuning.foregroundTexture);
      this.staticBallMesh.SetVector("_SpriteSettings", new Vector4(1f / this.tuning.spriteCount, 1f, num3, 0.0f));
      this.staticBallMesh.SetVector("_Highlight", new Vector4((float) this.highlightColour.r / (float) byte.MaxValue, (float) this.highlightColour.g / (float) byte.MaxValue, (float) this.highlightColour.b / (float) byte.MaxValue, 0.0f));
      Vector3 position = TransformExtensions.GetPosition(CameraController.Instance.transform);
      ConduitFlowVisualizer visualizer = trigger_audio ? this : (ConduitFlowVisualizer) null;
      for (int index = 0; index != ConduitFlowVisualizer.render_mesh_job.Count; ++index)
        ConduitFlowVisualizer.render_mesh_job.GetWorkItem(index).Finish(this.movingBallMesh, this.staticBallMesh, position, visualizer);
      this.movingBallMesh.End(z, this.layer);
      this.staticBallMesh.End(z, this.layer);
      renderMeshContext.Finish();
      ConduitFlowVisualizer.render_mesh_job.Reset((ConduitFlowVisualizer.RenderMeshContext) null);
    }
  }

  public void ColourizePipeContents(bool show_contents, bool move_to_overlay_layer)
  {
    this.showContents = show_contents;
    this.layer = show_contents & move_to_overlay_layer ? LayerMask.NameToLayer("MaskedOverlay") : 0;
  }

  private void AddAudioSource(ConduitFlow.Conduit conduit, Vector3 camera_pos)
  {
    KProfiler.Region region;
    // ISSUE: explicit constructor call
    ((KProfiler.Region) ref region).\u002Ector(nameof (AddAudioSource), (Object) null);
    try
    {
      UtilityNetwork network = this.flowManager.GetNetwork(conduit);
      if (network == null)
        return;
      Vector3 posCcc = Grid.CellToPosCCC(conduit.GetCell(this.flowManager), Grid.SceneLayer.Building);
      float num = Vector3.SqrMagnitude(Vector3.op_Subtraction(posCcc, camera_pos));
      bool flag = false;
      for (int index = 0; index < this.audioInfo.Count; ++index)
      {
        ConduitFlowVisualizer.AudioInfo audioInfo = this.audioInfo[index];
        if (audioInfo.networkID == network.id)
        {
          if ((double) num < (double) audioInfo.distance)
          {
            audioInfo.distance = num;
            audioInfo.position = posCcc;
            this.audioInfo[index] = audioInfo;
          }
          flag = true;
          break;
        }
      }
      if (flag)
        return;
      this.audioInfo.Add(new ConduitFlowVisualizer.AudioInfo()
      {
        networkID = network.id,
        position = posCcc,
        distance = num,
        blobCount = 0
      });
    }
    finally
    {
      region.Dispose();
    }
  }

  private void TriggerAudio()
  {
    if (SpeedControlScreen.Instance.IsPaused)
      return;
    CameraController instance1 = CameraController.Instance;
    int num = 0;
    List<ConduitFlowVisualizer.AudioInfo> audioInfoList = new List<ConduitFlowVisualizer.AudioInfo>();
    for (int index = 0; index < this.audioInfo.Count; ++index)
    {
      if (instance1.IsVisiblePos(this.audioInfo[index].position))
      {
        audioInfoList.Add(this.audioInfo[index]);
        ++num;
      }
    }
    for (int index = 0; index < audioInfoList.Count; ++index)
    {
      ConduitFlowVisualizer.AudioInfo audioInfo = audioInfoList[index];
      if ((double) audioInfo.distance != double.PositiveInfinity)
      {
        Vector3 position = audioInfo.position;
        position.z = 0.0f;
        EventInstance instance2 = SoundEvent.BeginOneShot(this.overlaySound, position);
        ((EventInstance) ref instance2).setParameterByName("blobCount", (float) audioInfo.blobCount, false);
        ((EventInstance) ref instance2).setParameterByName("networkCount", (float) num, false);
        SoundEvent.EndOneShot(instance2);
      }
    }
  }

  public void AddThermalConductivity(int cell, float conductivity)
  {
    if ((double) conductivity < 1.0)
    {
      this.insulatedCells.Add(cell);
    }
    else
    {
      if ((double) conductivity <= 1.0)
        return;
      this.radiantCells.Add(cell);
    }
  }

  public void RemoveThermalConductivity(int cell, float conductivity)
  {
    if ((double) conductivity < 1.0)
    {
      this.insulatedCells.Remove(cell);
    }
    else
    {
      if ((double) conductivity <= 1.0)
        return;
      this.radiantCells.Remove(cell);
    }
  }

  public void SetHighlightedCell(int cell) => this.highlightedCell = cell;

  [Serializable]
  public class Tuning
  {
    public bool renderMesh;
    public float size;
    public float spriteCount;
    public float framesPerSecond;
    public Texture2D backgroundTexture;
    public Texture2D foregroundTexture;
  }

  private class ConduitFlowMesh
  {
    private Mesh mesh;
    private Material material;
    private List<Vector3> positions = new List<Vector3>();
    private List<Vector4> uvs = new List<Vector4>();
    private List<int> triangles = new List<int>();
    private List<Color32> colors = new List<Color32>();
    private int quadIndex;

    public ConduitFlowMesh()
    {
      this.mesh = new Mesh();
      ((Object) this.mesh).name = "ConduitMesh";
      this.material = new Material(Shader.Find("Klei/ConduitBall"));
    }

    public void AddQuad(
      Vector2 pos,
      Color32 color,
      float size,
      float is_foreground,
      float highlight,
      Vector2I uvbl,
      Vector2I uvtl,
      Vector2I uvbr,
      Vector2I uvtr)
    {
      float num = size * 0.5f;
      this.positions.Add(new Vector3(pos.x - num, pos.y - num, 0.0f));
      this.positions.Add(new Vector3(pos.x - num, pos.y + num, 0.0f));
      this.positions.Add(new Vector3(pos.x + num, pos.y - num, 0.0f));
      this.positions.Add(new Vector3(pos.x + num, pos.y + num, 0.0f));
      this.uvs.Add(new Vector4((float) uvbl.x, (float) uvbl.y, is_foreground, highlight));
      this.uvs.Add(new Vector4((float) uvtl.x, (float) uvtl.y, is_foreground, highlight));
      this.uvs.Add(new Vector4((float) uvbr.x, (float) uvbr.y, is_foreground, highlight));
      this.uvs.Add(new Vector4((float) uvtr.x, (float) uvtr.y, is_foreground, highlight));
      this.colors.Add(color);
      this.colors.Add(color);
      this.colors.Add(color);
      this.colors.Add(color);
      this.triangles.Add(this.quadIndex * 4);
      this.triangles.Add(this.quadIndex * 4 + 1);
      this.triangles.Add(this.quadIndex * 4 + 2);
      this.triangles.Add(this.quadIndex * 4 + 2);
      this.triangles.Add(this.quadIndex * 4 + 1);
      this.triangles.Add(this.quadIndex * 4 + 3);
      ++this.quadIndex;
    }

    public void SetTexture(string id, Texture2D texture) => this.material.SetTexture(id, (Texture) texture);

    public void SetVector(string id, Vector4 data) => this.material.SetVector(id, data);

    public void Begin()
    {
      this.positions.Clear();
      this.uvs.Clear();
      this.triangles.Clear();
      this.colors.Clear();
      this.quadIndex = 0;
    }

    public void End(float z, int layer)
    {
      this.mesh.Clear();
      this.mesh.SetVertices(this.positions);
      this.mesh.SetUVs(0, this.uvs);
      this.mesh.SetColors(this.colors);
      this.mesh.SetTriangles(this.triangles, 0, false);
      Graphics.DrawMesh(this.mesh, new Vector3(ConduitFlowVisualizer.GRID_OFFSET.x, ConduitFlowVisualizer.GRID_OFFSET.y, z - 0.1f), Quaternion.identity, this.material, layer);
    }

    public void Cleanup()
    {
      Object.Destroy((Object) this.mesh);
      this.mesh = (Mesh) null;
      Object.Destroy((Object) this.material);
      this.material = (Material) null;
    }
  }

  private struct AudioInfo
  {
    public int networkID;
    public int blobCount;
    public float distance;
    public Vector3 position;
  }

  private class RenderMeshContext
  {
    public ListPool<int, ConduitFlowVisualizer>.PooledList visible_conduits;
    public ConduitFlowVisualizer outer;
    public float lerp_percent;

    public RenderMeshContext(
      ConduitFlowVisualizer outer,
      float lerp_percent,
      Vector2I min,
      Vector2I max)
    {
      this.outer = outer;
      this.lerp_percent = lerp_percent;
      this.visible_conduits = ListPool<int, ConduitFlowVisualizer>.Allocate();
      ((List<int>) this.visible_conduits).Capacity = Math.Max(outer.flowManager.soaInfo.NumEntries, ((List<int>) this.visible_conduits).Capacity);
      for (int idx = 0; idx != outer.flowManager.soaInfo.NumEntries; ++idx)
      {
        Vector2I xy = Grid.CellToXY(outer.flowManager.soaInfo.GetCell(idx));
        if (Vector2I.op_LessThanOrEqual(min, xy) && Vector2I.op_LessThanOrEqual(xy, max))
          ((List<int>) this.visible_conduits).Add(idx);
      }
    }

    public void Finish() => this.visible_conduits.Recycle();
  }

  private struct RenderMeshTask : IWorkItem<ConduitFlowVisualizer.RenderMeshContext>
  {
    private ListPool<ConduitFlowVisualizer.RenderMeshTask.Ball, ConduitFlowVisualizer.RenderMeshTask>.PooledList moving_balls;
    private ListPool<ConduitFlowVisualizer.RenderMeshTask.Ball, ConduitFlowVisualizer.RenderMeshTask>.PooledList static_balls;
    private ListPool<ConduitFlow.Conduit, ConduitFlowVisualizer.RenderMeshTask>.PooledList moving_conduits;
    private int start;
    private int end;

    public RenderMeshTask(int start, int end)
    {
      this.start = start;
      this.end = end;
      int num = end - start;
      this.moving_balls = ListPool<ConduitFlowVisualizer.RenderMeshTask.Ball, ConduitFlowVisualizer.RenderMeshTask>.Allocate();
      ((List<ConduitFlowVisualizer.RenderMeshTask.Ball>) this.moving_balls).Capacity = num;
      this.static_balls = ListPool<ConduitFlowVisualizer.RenderMeshTask.Ball, ConduitFlowVisualizer.RenderMeshTask>.Allocate();
      ((List<ConduitFlowVisualizer.RenderMeshTask.Ball>) this.static_balls).Capacity = num;
      this.moving_conduits = ListPool<ConduitFlow.Conduit, ConduitFlowVisualizer.RenderMeshTask>.Allocate();
      ((List<ConduitFlow.Conduit>) this.moving_conduits).Capacity = num;
    }

    public void Run(ConduitFlowVisualizer.RenderMeshContext context)
    {
      Element element = (Element) null;
      for (int start = this.start; start != this.end; ++start)
      {
        ConduitFlow.Conduit conduit = context.outer.flowManager.soaInfo.GetConduit(((List<int>) context.visible_conduits)[start]);
        ConduitFlow.ConduitFlowInfo lastFlowInfo = conduit.GetLastFlowInfo(context.outer.flowManager);
        ConduitFlow.ConduitContents initialContents = conduit.GetInitialContents(context.outer.flowManager);
        if ((double) lastFlowInfo.contents.mass > 0.0)
        {
          int cell = conduit.GetCell(context.outer.flowManager);
          int cellFromDirection = ConduitFlow.GetCellFromDirection(cell, lastFlowInfo.direction);
          Vector2I xy1 = Grid.CellToXY(cell);
          Vector2I xy2 = Grid.CellToXY(cellFromDirection);
          Vector2 pos = cell == -1 ? Vector2I.op_Implicit(xy1) : Vector2.Lerp(new Vector2((float) xy1.x, (float) xy1.y), new Vector2((float) xy2.x, (float) xy2.y), context.lerp_percent);
          Color32 color32 = Color32.Lerp(context.outer.GetCellTintColour(cell), context.outer.GetCellTintColour(cellFromDirection), context.lerp_percent);
          bool highlight = false;
          if (context.outer.showContents)
          {
            if ((double) lastFlowInfo.contents.mass >= (double) initialContents.mass)
              ((List<ConduitFlowVisualizer.RenderMeshTask.Ball>) this.moving_balls).Add(new ConduitFlowVisualizer.RenderMeshTask.Ball(lastFlowInfo.direction, pos, color32, context.outer.tuning.size, false, false));
            if (element == null || lastFlowInfo.contents.element != element.id)
              element = ElementLoader.FindElementByHash(lastFlowInfo.contents.element);
          }
          else
          {
            element = (Element) null;
            highlight = Grid.PosToCell(new Vector3(pos.x + ConduitFlowVisualizer.GRID_OFFSET.x, pos.y + ConduitFlowVisualizer.GRID_OFFSET.y, 0.0f)) == context.outer.highlightedCell;
          }
          Color32 contentsColor = context.outer.GetContentsColor(element, color32);
          float num = 1f;
          if (context.outer.showContents || (double) lastFlowInfo.contents.mass < (double) initialContents.mass)
            num = context.outer.CalculateMassScale(lastFlowInfo.contents.mass);
          ((List<ConduitFlowVisualizer.RenderMeshTask.Ball>) this.moving_balls).Add(new ConduitFlowVisualizer.RenderMeshTask.Ball(lastFlowInfo.direction, pos, contentsColor, context.outer.tuning.size * num, true, highlight));
          ((List<ConduitFlow.Conduit>) this.moving_conduits).Add(conduit);
        }
        if ((double) initialContents.mass > (double) lastFlowInfo.contents.mass && (double) initialContents.mass > 0.0)
        {
          int cell = conduit.GetCell(context.outer.flowManager);
          Vector2 pos = Vector2I.op_Implicit(Grid.CellToXY(cell));
          float mass = initialContents.mass - lastFlowInfo.contents.mass;
          bool highlight = false;
          Color32 cellTintColour = context.outer.GetCellTintColour(cell);
          float massScale = context.outer.CalculateMassScale(mass);
          if (context.outer.showContents)
          {
            ((List<ConduitFlowVisualizer.RenderMeshTask.Ball>) this.static_balls).Add(new ConduitFlowVisualizer.RenderMeshTask.Ball(ConduitFlow.FlowDirections.None, pos, cellTintColour, context.outer.tuning.size * massScale, false, false));
            if (element == null || initialContents.element != element.id)
              element = ElementLoader.FindElementByHash(initialContents.element);
          }
          else
          {
            element = (Element) null;
            highlight = cell == context.outer.highlightedCell;
          }
          Color32 contentsColor = context.outer.GetContentsColor(element, cellTintColour);
          ((List<ConduitFlowVisualizer.RenderMeshTask.Ball>) this.static_balls).Add(new ConduitFlowVisualizer.RenderMeshTask.Ball(ConduitFlow.FlowDirections.None, pos, contentsColor, context.outer.tuning.size * massScale, true, highlight));
        }
      }
    }

    public void Finish(
      ConduitFlowVisualizer.ConduitFlowMesh moving_ball_mesh,
      ConduitFlowVisualizer.ConduitFlowMesh static_ball_mesh,
      Vector3 camera_pos,
      ConduitFlowVisualizer visualizer)
    {
      for (int index = 0; index != ((List<ConduitFlowVisualizer.RenderMeshTask.Ball>) this.moving_balls).Count; ++index)
        ((List<ConduitFlowVisualizer.RenderMeshTask.Ball>) this.moving_balls)[index].Consume(moving_ball_mesh);
      this.moving_balls.Recycle();
      for (int index = 0; index != ((List<ConduitFlowVisualizer.RenderMeshTask.Ball>) this.static_balls).Count; ++index)
        ((List<ConduitFlowVisualizer.RenderMeshTask.Ball>) this.static_balls)[index].Consume(static_ball_mesh);
      this.static_balls.Recycle();
      if (visualizer != null)
      {
        foreach (ConduitFlow.Conduit movingConduit in (List<ConduitFlow.Conduit>) this.moving_conduits)
          visualizer.AddAudioSource(movingConduit, camera_pos);
      }
      this.moving_conduits.Recycle();
    }

    public struct Ball
    {
      private Vector2 pos;
      private float size;
      private Color32 color;
      private ConduitFlow.FlowDirections direction;
      private bool foreground;
      private bool highlight;
      private static Dictionary<ConduitFlow.FlowDirections, ConduitFlowVisualizer.RenderMeshTask.Ball.UVPack> uv_packs = new Dictionary<ConduitFlow.FlowDirections, ConduitFlowVisualizer.RenderMeshTask.Ball.UVPack>();

      public Ball(
        ConduitFlow.FlowDirections direction,
        Vector2 pos,
        Color32 color,
        float size,
        bool foreground,
        bool highlight)
      {
        this.pos = pos;
        this.size = size;
        this.color = color;
        this.direction = direction;
        this.foreground = foreground;
        this.highlight = highlight;
      }

      public static void InitializeResources()
      {
        ConduitFlowVisualizer.RenderMeshTask.Ball.uv_packs[ConduitFlow.FlowDirections.None] = new ConduitFlowVisualizer.RenderMeshTask.Ball.UVPack()
        {
          bl = new Vector2I(0, 0),
          tl = new Vector2I(0, 1),
          br = new Vector2I(1, 0),
          tr = new Vector2I(1, 1)
        };
        ConduitFlowVisualizer.RenderMeshTask.Ball.uv_packs[ConduitFlow.FlowDirections.Left] = new ConduitFlowVisualizer.RenderMeshTask.Ball.UVPack()
        {
          bl = new Vector2I(0, 0),
          tl = new Vector2I(0, 1),
          br = new Vector2I(1, 0),
          tr = new Vector2I(1, 1)
        };
        ConduitFlowVisualizer.RenderMeshTask.Ball.uv_packs[ConduitFlow.FlowDirections.Right] = ConduitFlowVisualizer.RenderMeshTask.Ball.uv_packs[ConduitFlow.FlowDirections.Left];
        ConduitFlowVisualizer.RenderMeshTask.Ball.uv_packs[ConduitFlow.FlowDirections.Up] = new ConduitFlowVisualizer.RenderMeshTask.Ball.UVPack()
        {
          bl = new Vector2I(1, 0),
          tl = new Vector2I(0, 0),
          br = new Vector2I(1, 1),
          tr = new Vector2I(0, 1)
        };
        ConduitFlowVisualizer.RenderMeshTask.Ball.uv_packs[ConduitFlow.FlowDirections.Down] = ConduitFlowVisualizer.RenderMeshTask.Ball.uv_packs[ConduitFlow.FlowDirections.Up];
      }

      private static ConduitFlowVisualizer.RenderMeshTask.Ball.UVPack GetUVPack(
        ConduitFlow.FlowDirections direction)
      {
        return ConduitFlowVisualizer.RenderMeshTask.Ball.uv_packs[direction];
      }

      public void Consume(ConduitFlowVisualizer.ConduitFlowMesh mesh)
      {
        ConduitFlowVisualizer.RenderMeshTask.Ball.UVPack uvPack = ConduitFlowVisualizer.RenderMeshTask.Ball.GetUVPack(this.direction);
        mesh.AddQuad(this.pos, this.color, this.size, this.foreground ? 1f : 0.0f, this.highlight ? 1f : 0.0f, uvPack.bl, uvPack.tl, uvPack.br, uvPack.tr);
      }

      private class UVPack
      {
        public Vector2I bl;
        public Vector2I tl;
        public Vector2I br;
        public Vector2I tr;
      }
    }
  }
}
