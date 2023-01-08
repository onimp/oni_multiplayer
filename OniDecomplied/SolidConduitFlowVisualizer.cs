// Decompiled with JetBrains decompiler
// Type: SolidConduitFlowVisualizer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SolidConduitFlowVisualizer
{
  private SolidConduitFlow flowManager;
  private EventReference overlaySound;
  private bool showContents;
  private double animTime;
  private int layer;
  private static Vector2 GRID_OFFSET = new Vector2(0.5f, 0.5f);
  private static int BLOB_SOUND_COUNT = 7;
  private List<SolidConduitFlowVisualizer.AudioInfo> audioInfo;
  private HashSet<int> insulatedCells = new HashSet<int>();
  private Game.ConduitVisInfo visInfo;
  private SolidConduitFlowVisualizer.ConduitFlowMesh movingBallMesh;
  private SolidConduitFlowVisualizer.ConduitFlowMesh staticBallMesh;
  private int highlightedCell = -1;
  private Color32 highlightColour = Color32.op_Implicit(new Color(0.2f, 0.2f, 0.2f, 0.2f));
  private SolidConduitFlowVisualizer.Tuning tuning;

  public SolidConduitFlowVisualizer(
    SolidConduitFlow flow_manager,
    Game.ConduitVisInfo vis_info,
    EventReference overlay_sound,
    SolidConduitFlowVisualizer.Tuning tuning)
  {
    this.flowManager = flow_manager;
    this.visInfo = vis_info;
    this.overlaySound = overlay_sound;
    this.tuning = tuning;
    this.movingBallMesh = new SolidConduitFlowVisualizer.ConduitFlowMesh();
    this.staticBallMesh = new SolidConduitFlowVisualizer.ConduitFlowMesh();
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

  private Color32 GetBackgroundColor(float insulation_lerp) => this.showContents ? Color32.Lerp(GlobalAssets.Instance.colorSet.GetColorByName(this.visInfo.overlayTintName), GlobalAssets.Instance.colorSet.GetColorByName(this.visInfo.overlayInsulatedTintName), insulation_lerp) : Color32.Lerp(this.visInfo.tint, this.visInfo.insulatedTint, insulation_lerp);

  public void Render(float z, int render_layer, float lerp_percent, bool trigger_audio = false)
  {
    GridArea visibleArea = GridVisibleArea.GetVisibleArea();
    Vector2I vector2I1;
    // ISSUE: explicit constructor call
    ((Vector2I) ref vector2I1).\u002Ector(Mathf.Max(0, visibleArea.Min.x - 1), Mathf.Max(0, visibleArea.Min.y - 1));
    Vector2I vector2I2;
    // ISSUE: explicit constructor call
    ((Vector2I) ref vector2I2).\u002Ector(Mathf.Min(Grid.WidthInCells - 1, visibleArea.Max.x + 1), Mathf.Min(Grid.HeightInCells - 1, visibleArea.Max.y + 1));
    this.animTime += (double) Time.deltaTime;
    if (trigger_audio)
    {
      if (this.audioInfo == null)
        this.audioInfo = new List<SolidConduitFlowVisualizer.AudioInfo>();
      for (int index = 0; index < this.audioInfo.Count; ++index)
      {
        SolidConduitFlowVisualizer.AudioInfo audioInfo = this.audioInfo[index] with
        {
          distance = float.PositiveInfinity,
          position = Vector3.zero
        };
        audioInfo.blobCount = (audioInfo.blobCount + 1) % SolidConduitFlowVisualizer.BLOB_SOUND_COUNT;
        this.audioInfo[index] = audioInfo;
      }
    }
    Vector3 position = TransformExtensions.GetPosition(CameraController.Instance.transform);
    Element element = (Element) null;
    if (this.tuning.renderMesh)
    {
      float num1 = 0.0f;
      if (this.showContents)
        num1 = 1f;
      float num2 = (float) ((int) (this.animTime / (1.0 / (double) this.tuning.framesPerSecond)) % (int) this.tuning.spriteCount) * (1f / this.tuning.spriteCount);
      this.movingBallMesh.Begin();
      this.movingBallMesh.SetTexture("_BackgroundTex", this.tuning.backgroundTexture);
      this.movingBallMesh.SetTexture("_ForegroundTex", this.tuning.foregroundTexture);
      this.movingBallMesh.SetVector("_SpriteSettings", new Vector4(1f / this.tuning.spriteCount, 1f, num1, num2));
      this.movingBallMesh.SetVector("_Highlight", new Vector4((float) this.highlightColour.r / (float) byte.MaxValue, (float) this.highlightColour.g / (float) byte.MaxValue, (float) this.highlightColour.b / (float) byte.MaxValue, 0.0f));
      this.staticBallMesh.Begin();
      this.staticBallMesh.SetTexture("_BackgroundTex", this.tuning.backgroundTexture);
      this.staticBallMesh.SetTexture("_ForegroundTex", this.tuning.foregroundTexture);
      this.staticBallMesh.SetVector("_SpriteSettings", new Vector4(1f / this.tuning.spriteCount, 1f, num1, 0.0f));
      this.staticBallMesh.SetVector("_Highlight", new Vector4((float) this.highlightColour.r / (float) byte.MaxValue, (float) this.highlightColour.g / (float) byte.MaxValue, (float) this.highlightColour.b / (float) byte.MaxValue, 0.0f));
      for (int idx = 0; idx < this.flowManager.GetSOAInfo().NumEntries; ++idx)
      {
        Vector2I xy1 = Grid.CellToXY(this.flowManager.GetSOAInfo().GetCell(idx));
        if (!Vector2I.op_LessThan(xy1, vector2I1) && !Vector2I.op_GreaterThan(xy1, vector2I2))
        {
          SolidConduitFlow.Conduit conduit = this.flowManager.GetSOAInfo().GetConduit(idx);
          SolidConduitFlow.ConduitFlowInfo lastFlowInfo = conduit.GetLastFlowInfo(this.flowManager);
          SolidConduitFlow.ConduitContents initialContents = conduit.GetInitialContents(this.flowManager);
          bool flag = lastFlowInfo.direction != 0;
          if (flag)
          {
            int cell = conduit.GetCell(this.flowManager);
            int cellFromDirection = SolidConduitFlow.GetCellFromDirection(cell, lastFlowInfo.direction);
            Vector2I xy2 = Grid.CellToXY(cell);
            Vector2I xy3 = Grid.CellToXY(cellFromDirection);
            Vector2 pos = Vector2I.op_Implicit(xy2);
            if (cell != -1)
              pos = Vector2.Lerp(new Vector2((float) xy2.x, (float) xy2.y), new Vector2((float) xy3.x, (float) xy3.y), lerp_percent);
            Color color = Color32.op_Implicit(this.GetBackgroundColor(Mathf.Lerp(this.insulatedCells.Contains(cell) ? 1f : 0.0f, this.insulatedCells.Contains(cellFromDirection) ? 1f : 0.0f, lerp_percent)));
            Vector2I uvbl;
            // ISSUE: explicit constructor call
            ((Vector2I) ref uvbl).\u002Ector(0, 0);
            Vector2I uvtl;
            // ISSUE: explicit constructor call
            ((Vector2I) ref uvtl).\u002Ector(0, 1);
            Vector2I uvbr;
            // ISSUE: explicit constructor call
            ((Vector2I) ref uvbr).\u002Ector(1, 0);
            Vector2I uvtr;
            // ISSUE: explicit constructor call
            ((Vector2I) ref uvtr).\u002Ector(1, 1);
            float highlight = 0.0f;
            if (this.showContents)
            {
              if (flag != initialContents.pickupableHandle.IsValid())
                this.movingBallMesh.AddQuad(pos, Color32.op_Implicit(color), this.tuning.size, 0.0f, 0.0f, uvbl, uvtl, uvbr, uvtr);
            }
            else
            {
              element = (Element) null;
              if (Grid.PosToCell(new Vector3(pos.x + SolidConduitFlowVisualizer.GRID_OFFSET.x, pos.y + SolidConduitFlowVisualizer.GRID_OFFSET.y, 0.0f)) == this.highlightedCell)
                highlight = 1f;
            }
            Color32 contentsColor = this.GetContentsColor(element, Color32.op_Implicit(color));
            float num3 = 1f;
            this.movingBallMesh.AddQuad(pos, contentsColor, this.tuning.size * num3, 1f, highlight, uvbl, uvtl, uvbr, uvtr);
            if (trigger_audio)
              this.AddAudioSource(conduit, position);
          }
          if (initialContents.pickupableHandle.IsValid() && !flag)
          {
            int cell = conduit.GetCell(this.flowManager);
            Vector2 pos = Vector2I.op_Implicit(Grid.CellToXY(cell));
            float insulation_lerp = this.insulatedCells.Contains(cell) ? 1f : 0.0f;
            Vector2I uvbl;
            // ISSUE: explicit constructor call
            ((Vector2I) ref uvbl).\u002Ector(0, 0);
            Vector2I uvtl;
            // ISSUE: explicit constructor call
            ((Vector2I) ref uvtl).\u002Ector(0, 1);
            Vector2I uvbr;
            // ISSUE: explicit constructor call
            ((Vector2I) ref uvbr).\u002Ector(1, 0);
            Vector2I uvtr;
            // ISSUE: explicit constructor call
            ((Vector2I) ref uvtr).\u002Ector(1, 1);
            float highlight = 0.0f;
            Color color = Color32.op_Implicit(this.GetBackgroundColor(insulation_lerp));
            float num4 = 1f;
            if (this.showContents)
            {
              this.staticBallMesh.AddQuad(pos, Color32.op_Implicit(color), this.tuning.size * num4, 0.0f, 0.0f, uvbl, uvtl, uvbr, uvtr);
            }
            else
            {
              element = (Element) null;
              if (cell == this.highlightedCell)
                highlight = 1f;
            }
            Color32 contentsColor = this.GetContentsColor(element, Color32.op_Implicit(color));
            this.staticBallMesh.AddQuad(pos, contentsColor, this.tuning.size * num4, 1f, highlight, uvbl, uvtl, uvbr, uvtr);
          }
        }
      }
      this.movingBallMesh.End(z, this.layer);
      this.staticBallMesh.End(z, this.layer);
    }
    if (!trigger_audio)
      return;
    this.TriggerAudio();
  }

  public void ColourizePipeContents(bool show_contents, bool move_to_overlay_layer)
  {
    this.showContents = show_contents;
    this.layer = show_contents & move_to_overlay_layer ? LayerMask.NameToLayer("MaskedOverlay") : 0;
  }

  private void AddAudioSource(SolidConduitFlow.Conduit conduit, Vector3 camera_pos)
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
        SolidConduitFlowVisualizer.AudioInfo audioInfo = this.audioInfo[index];
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
      this.audioInfo.Add(new SolidConduitFlowVisualizer.AudioInfo()
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
    List<SolidConduitFlowVisualizer.AudioInfo> audioInfoList = new List<SolidConduitFlowVisualizer.AudioInfo>();
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
      SolidConduitFlowVisualizer.AudioInfo audioInfo = audioInfoList[index];
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

  public void SetInsulated(int cell, bool insulated)
  {
    if (insulated)
      this.insulatedCells.Add(cell);
    else
      this.insulatedCells.Remove(cell);
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
      Graphics.DrawMesh(this.mesh, new Vector3(SolidConduitFlowVisualizer.GRID_OFFSET.x, SolidConduitFlowVisualizer.GRID_OFFSET.y, z - 0.1f), Quaternion.identity, this.material, layer);
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
}
