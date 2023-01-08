// Decompiled with JetBrains decompiler
// Type: World
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Rendering;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/World")]
public class World : KMonoBehaviour
{
  public Action<int> OnSolidChanged;
  public Action<int> OnLiquidChanged;
  public BlockTileRenderer blockTileRenderer;
  [MyCmpGet]
  [NonSerialized]
  public GroundRenderer groundRenderer;
  private List<int> revealedCells = new List<int>();
  public static int DebugCellID = -1;
  private List<int> changedCells = new List<int>();

  public static World Instance { get; private set; }

  public SubworldZoneRenderData zoneRenderData { get; private set; }

  protected virtual void OnPrefabInit()
  {
    Debug.Assert(Object.op_Equality((Object) World.Instance, (Object) null));
    World.Instance = this;
    this.blockTileRenderer = ((Component) this).GetComponent<BlockTileRenderer>();
  }

  protected virtual void OnSpawn()
  {
    ((Component) this).GetComponent<SimDebugView>().OnReset();
    ((Component) this).GetComponent<PropertyTextures>().OnReset();
    this.zoneRenderData = ((Component) this).GetComponent<SubworldZoneRenderData>();
    Grid.OnReveal += new Action<int>(this.OnReveal);
  }

  protected virtual void OnLoadLevel()
  {
    World.Instance = (World) null;
    if (Object.op_Inequality((Object) this.blockTileRenderer, (Object) null))
      this.blockTileRenderer.FreeResources();
    this.blockTileRenderer = (BlockTileRenderer) null;
    if (Object.op_Inequality((Object) this.groundRenderer, (Object) null))
      this.groundRenderer.FreeResources();
    this.groundRenderer = (GroundRenderer) null;
    this.revealedCells.Clear();
    this.revealedCells = (List<int>) null;
    base.OnLoadLevel();
  }

  public unsafe void UpdateCellInfo(
    List<Klei.SolidInfo> solidInfo,
    List<Klei.CallbackInfo> callbackInfo,
    int num_solid_substance_change_info,
    Sim.SolidSubstanceChangeInfo* solid_substance_change_info,
    int num_liquid_change_info,
    Sim.LiquidChangeInfo* liquid_change_info)
  {
    int count1 = solidInfo.Count;
    this.changedCells.Clear();
    for (int index = 0; index < count1; ++index)
    {
      int cellIdx = solidInfo[index].cellIdx;
      if (!this.changedCells.Contains(cellIdx))
        this.changedCells.Add(cellIdx);
      Pathfinding.Instance.AddDirtyNavGridCell(cellIdx);
      WorldDamage.Instance.OnSolidStateChanged(cellIdx);
      if (this.OnSolidChanged != null)
        this.OnSolidChanged(cellIdx);
    }
    if (this.changedCells.Count != 0)
    {
      SaveGame.Instance.entombedItemManager.OnSolidChanged(this.changedCells);
      GameScenePartitioner.Instance.TriggerEvent(this.changedCells, GameScenePartitioner.Instance.solidChangedLayer, (object) null);
    }
    int count2 = callbackInfo.Count;
    for (int index = 0; index < count2; ++index)
      callbackInfo[index].Release();
    for (int index = 0; index < num_solid_substance_change_info; ++index)
    {
      int cellIdx = solid_substance_change_info[index].cellIdx;
      if (!Grid.IsValidCell(cellIdx))
      {
        Debug.LogError((object) cellIdx);
      }
      else
      {
        Grid.RenderedByWorld[cellIdx] = Grid.Element[cellIdx].substance.renderedByWorld && Object.op_Equality((Object) Grid.Objects[cellIdx, 9], (Object) null);
        this.groundRenderer.MarkDirty(cellIdx);
      }
    }
    GameScenePartitioner instance = GameScenePartitioner.Instance;
    this.changedCells.Clear();
    for (int index = 0; index < num_liquid_change_info; ++index)
    {
      int cellIdx = liquid_change_info[index].cellIdx;
      this.changedCells.Add(cellIdx);
      if (this.OnLiquidChanged != null)
        this.OnLiquidChanged(cellIdx);
    }
    instance.TriggerEvent(this.changedCells, GameScenePartitioner.Instance.liquidChangedLayer, (object) null);
  }

  private void OnReveal(int cell) => this.revealedCells.Add(cell);

  private void LateUpdate()
  {
    if (Game.IsQuitting())
      return;
    if (GameUtil.IsCapturingTimeLapse())
    {
      this.groundRenderer.RenderAll();
      KAnimBatchManager.Instance().UpdateActiveArea(new Vector2I(0, 0), new Vector2I(9999, 9999));
      KAnimBatchManager.Instance().UpdateDirty(Time.frameCount);
      KAnimBatchManager.Instance().Render();
    }
    else
    {
      GridArea visibleArea = GridVisibleArea.GetVisibleArea();
      this.groundRenderer.Render(visibleArea.Min, visibleArea.Max);
      Vector2I vis_chunk_min;
      Vector2I vis_chunk_max;
      Singleton<KBatchedAnimUpdater>.Instance.GetVisibleArea(out vis_chunk_min, out vis_chunk_max);
      KAnimBatchManager.Instance().UpdateActiveArea(vis_chunk_min, vis_chunk_max);
      KAnimBatchManager.Instance().UpdateDirty(Time.frameCount);
      KAnimBatchManager.Instance().Render();
    }
    if (Object.op_Inequality((Object) Camera.main, (Object) null))
    {
      Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(KInputManager.GetMousePos().x, KInputManager.GetMousePos().y, -TransformExtensions.GetPosition(((Component) Camera.main).transform).z));
      Shader.SetGlobalVector("_CursorPos", new Vector4(worldPoint.x, worldPoint.y, worldPoint.z, 0.0f));
    }
    FallingWater.instance.UpdateParticles(Time.deltaTime);
    FallingWater.instance.Render();
    if (this.revealedCells.Count <= 0)
      return;
    GameScenePartitioner.Instance.TriggerEvent(this.revealedCells, GameScenePartitioner.Instance.fogOfWarChangedLayer, (object) null);
    this.revealedCells.Clear();
  }
}
