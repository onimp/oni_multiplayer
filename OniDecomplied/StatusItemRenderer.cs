// Decompiled with JetBrains decompiler
// Type: StatusItemRenderer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class StatusItemRenderer
{
  private StatusItemRenderer.Entry[] entries;
  private int entryCount;
  private Dictionary<int, int> handleTable = new Dictionary<int, int>();
  private Shader shader;
  public List<StatusItemRenderer.Entry> visibleEntries = new List<StatusItemRenderer.Entry>();

  public int layer { get; private set; }

  public int selectedHandle { get; private set; }

  public int highlightHandle { get; private set; }

  public Color32 backgroundColor { get; private set; }

  public Color32 selectedColor { get; private set; }

  public Color32 neutralColor { get; private set; }

  public Sprite arrowSprite { get; private set; }

  public Sprite backgroundSprite { get; private set; }

  public float scale { get; private set; }

  public StatusItemRenderer()
  {
    this.layer = LayerMask.NameToLayer("UI");
    this.entries = new StatusItemRenderer.Entry[100];
    this.shader = Shader.Find("Klei/StatusItem");
    for (int index = 0; index < this.entries.Length; ++index)
    {
      StatusItemRenderer.Entry entry = new StatusItemRenderer.Entry();
      entry.Init(this.shader);
      this.entries[index] = entry;
    }
    this.backgroundColor = new Color32((byte) 244, (byte) 74, (byte) 71, byte.MaxValue);
    this.selectedColor = new Color32((byte) 225, (byte) 181, (byte) 180, byte.MaxValue);
    this.neutralColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
    this.arrowSprite = Assets.GetSprite(HashedString.op_Implicit("StatusBubbleTop"));
    this.backgroundSprite = Assets.GetSprite(HashedString.op_Implicit("StatusBubble"));
    this.scale = 1f;
    Game.Instance.Subscribe(2095258329, new Action<object>(this.OnHighlightObject));
  }

  public int GetIdx(Transform transform)
  {
    int instanceId = ((Object) transform).GetInstanceID();
    int idx = 0;
    if (!this.handleTable.TryGetValue(instanceId, out idx))
    {
      idx = this.entryCount++;
      this.handleTable[instanceId] = idx;
      StatusItemRenderer.Entry entry = this.entries[idx] with
      {
        handle = instanceId,
        transform = transform,
        buildingPos = TransformExtensions.GetPosition(transform),
        building = ((Component) transform).GetComponent<Building>()
      };
      entry.isBuilding = Object.op_Inequality((Object) entry.building, (Object) null);
      entry.selectable = ((Component) transform).GetComponent<KSelectable>();
      this.entries[idx] = entry;
    }
    return idx;
  }

  public void Add(Transform transform, StatusItem status_item)
  {
    if (this.entryCount == this.entries.Length)
    {
      StatusItemRenderer.Entry[] entryArray = new StatusItemRenderer.Entry[this.entries.Length * 2];
      for (int index = 0; index < this.entries.Length; ++index)
        entryArray[index] = this.entries[index];
      for (int length = this.entries.Length; length < entryArray.Length; ++length)
        entryArray[length].Init(this.shader);
      this.entries = entryArray;
    }
    int idx = this.GetIdx(transform);
    StatusItemRenderer.Entry entry = this.entries[idx];
    entry.Add(status_item);
    this.entries[idx] = entry;
  }

  public void Remove(Transform transform, StatusItem status_item)
  {
    int instanceId = ((Object) transform).GetInstanceID();
    int idx = 0;
    if (!this.handleTable.TryGetValue(instanceId, out idx))
      return;
    StatusItemRenderer.Entry entry = this.entries[idx];
    if (entry.statusItems.Count == 0)
      return;
    entry.Remove(status_item);
    this.entries[idx] = entry;
    if (entry.statusItems.Count != 0)
      return;
    this.ClearIdx(idx);
  }

  private void ClearIdx(int idx)
  {
    StatusItemRenderer.Entry entry1 = this.entries[idx];
    this.handleTable.Remove(entry1.handle);
    if (idx != this.entryCount - 1)
    {
      entry1.Replace(this.entries[this.entryCount - 1]);
      this.entries[idx] = entry1;
      this.handleTable[entry1.handle] = idx;
    }
    StatusItemRenderer.Entry entry2 = this.entries[this.entryCount - 1];
    entry2.Clear();
    this.entries[this.entryCount - 1] = entry2;
    --this.entryCount;
  }

  private HashedString GetMode() => Object.op_Inequality((Object) OverlayScreen.Instance, (Object) null) ? OverlayScreen.Instance.mode : OverlayModes.None.ID;

  public void MarkAllDirty()
  {
    for (int index = 0; index < this.entryCount; ++index)
      this.entries[index].MarkDirty();
  }

  public void RenderEveryTick()
  {
    if (DebugHandler.HideUI)
      return;
    this.scale = (float) (1.0 + (double) Mathf.Sin(Time.unscaledTime * 8f) * 0.10000000149011612);
    Shader.SetGlobalVector("_StatusItemParameters", new Vector4(this.scale, 0.0f, 0.0f, 0.0f));
    Vector3 worldPoint1 = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, TransformExtensions.GetPosition(((Component) Camera.main).transform).z));
    Vector3 worldPoint2 = Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, TransformExtensions.GetPosition(((Component) Camera.main).transform).z));
    this.visibleEntries.Clear();
    Camera worldCamera = GameScreenManager.Instance.worldSpaceCanvas.GetComponent<Canvas>().worldCamera;
    for (int index = 0; index < this.entryCount; ++index)
      this.entries[index].Render(this, worldPoint2, worldPoint1, this.GetMode(), worldCamera);
  }

  public void GetIntersections(Vector2 pos, List<InterfaceTool.Intersection> intersections)
  {
    foreach (StatusItemRenderer.Entry visibleEntry in this.visibleEntries)
      visibleEntry.GetIntersection(pos, intersections, this.scale);
  }

  public void GetIntersections(Vector2 pos, List<KSelectable> selectables)
  {
    foreach (StatusItemRenderer.Entry visibleEntry in this.visibleEntries)
      visibleEntry.GetIntersection(pos, selectables, this.scale);
  }

  public void SetOffset(Transform transform, Vector3 offset)
  {
    int index = 0;
    if (!this.handleTable.TryGetValue(((Object) transform).GetInstanceID(), out index))
      return;
    this.entries[index].offset = offset;
  }

  private void OnSelectObject(object data)
  {
    int index = 0;
    if (this.handleTable.TryGetValue(this.selectedHandle, out index))
      this.entries[index].MarkDirty();
    GameObject gameObject = (GameObject) data;
    if (Object.op_Inequality((Object) gameObject, (Object) null))
    {
      this.selectedHandle = ((Object) gameObject.transform).GetInstanceID();
      if (!this.handleTable.TryGetValue(this.selectedHandle, out index))
        return;
      this.entries[index].MarkDirty();
    }
    else
      this.highlightHandle = -1;
  }

  private void OnHighlightObject(object data)
  {
    int index = 0;
    if (this.handleTable.TryGetValue(this.highlightHandle, out index))
    {
      StatusItemRenderer.Entry entry = this.entries[index];
      entry.MarkDirty();
      this.entries[index] = entry;
    }
    GameObject gameObject = (GameObject) data;
    if (Object.op_Inequality((Object) gameObject, (Object) null))
    {
      this.highlightHandle = ((Object) gameObject.transform).GetInstanceID();
      if (!this.handleTable.TryGetValue(this.highlightHandle, out index))
        return;
      StatusItemRenderer.Entry entry = this.entries[index];
      entry.MarkDirty();
      this.entries[index] = entry;
    }
    else
      this.highlightHandle = -1;
  }

  public void Destroy()
  {
    Game.Instance.Unsubscribe(-1503271301, new Action<object>(this.OnSelectObject));
    Game.Instance.Unsubscribe(-1201923725, new Action<object>(this.OnHighlightObject));
    foreach (StatusItemRenderer.Entry entry in this.entries)
    {
      entry.Clear();
      entry.FreeResources();
    }
  }

  public struct Entry
  {
    public int handle;
    public Transform transform;
    public Building building;
    public Vector3 buildingPos;
    public KSelectable selectable;
    public List<StatusItem> statusItems;
    public Mesh mesh;
    public bool dirty;
    public int layer;
    public Material material;
    public Vector3 offset;
    public bool hasVisibleStatusItems;
    public bool isBuilding;

    public void Init(Shader shader)
    {
      this.statusItems = new List<StatusItem>();
      this.mesh = new Mesh();
      ((Object) this.mesh).name = nameof (StatusItemRenderer);
      this.dirty = true;
      this.material = new Material(shader);
    }

    public void Render(
      StatusItemRenderer renderer,
      Vector3 camera_bl,
      Vector3 camera_tr,
      HashedString overlay,
      Camera camera)
    {
      if (Object.op_Equality((Object) this.transform, (Object) null))
      {
        string str = "Error cleaning up status items:";
        foreach (StatusItem statusItem in this.statusItems)
          str += statusItem.Id;
        Debug.LogWarning((object) str);
      }
      else
      {
        Vector3 pos = this.isBuilding ? this.buildingPos : TransformExtensions.GetPosition(this.transform);
        if (this.isBuilding)
          pos.x += (float) ((this.building.Def.WidthInCells - 1) % 2) / 2f;
        if ((double) pos.x < (double) camera_bl.x || (double) pos.x > (double) camera_tr.x || (double) pos.y < (double) camera_bl.y || (double) pos.y > (double) camera_tr.y)
          return;
        int cell = Grid.PosToCell(pos);
        if (Grid.IsValidCell(cell) && (!Grid.IsVisible(cell) || (int) Grid.WorldIdx[cell] != ClusterManager.Instance.activeWorldId) || !this.selectable.IsSelectable)
          return;
        renderer.visibleEntries.Add(this);
        if (this.dirty)
        {
          int num1 = 0;
          foreach (StatusItem statusItem in this.statusItems)
          {
            if (statusItem.UseConditionalCallback(overlay, this.transform) || !HashedString.op_Inequality(overlay, OverlayModes.None.ID) || !HashedString.op_Inequality(statusItem.render_overlay, overlay))
              ++num1;
          }
          this.hasVisibleStatusItems = num1 != 0;
          StatusItemRenderer.Entry.MeshBuilder meshBuilder = new StatusItemRenderer.Entry.MeshBuilder(num1 + 6, this.material);
          float num2 = 0.25f;
          float z = -5f;
          Vector2 vector2;
          // ISSUE: explicit constructor call
          ((Vector2) ref vector2).\u002Ector(0.05f, -0.05f);
          float num3 = 0.02f;
          Color32 color32_1;
          // ISSUE: explicit constructor call
          ((Color32) ref color32_1).\u002Ector((byte) 0, (byte) 0, (byte) 0, byte.MaxValue);
          Color32 color32_2;
          // ISSUE: explicit constructor call
          ((Color32) ref color32_2).\u002Ector((byte) 0, (byte) 0, (byte) 0, (byte) 75);
          Color32 color32_3 = renderer.neutralColor;
          if (renderer.selectedHandle == this.handle || renderer.highlightHandle == this.handle)
          {
            color32_3 = renderer.selectedColor;
          }
          else
          {
            for (int index = 0; index < this.statusItems.Count; ++index)
            {
              if (this.statusItems[index].notificationType != NotificationType.Neutral)
              {
                color32_3 = renderer.backgroundColor;
                break;
              }
            }
          }
          meshBuilder.AddQuad(Vector2.op_Addition(new Vector2(0.0f, 0.29f), vector2), new Vector2(0.05f, 0.05f), z, renderer.arrowSprite, Color32.op_Implicit(color32_2));
          meshBuilder.AddQuad(Vector2.op_Addition(new Vector2(0.0f, 0.0f), vector2), new Vector2(num2 * (float) num1, num2), z, renderer.backgroundSprite, Color32.op_Implicit(color32_2));
          meshBuilder.AddQuad(new Vector2(0.0f, 0.0f), new Vector2(num2 * (float) num1 + num3, num2 + num3), z, renderer.backgroundSprite, Color32.op_Implicit(color32_1));
          meshBuilder.AddQuad(new Vector2(0.0f, 0.0f), new Vector2(num2 * (float) num1, num2), z, renderer.backgroundSprite, Color32.op_Implicit(color32_3));
          int num4 = 0;
          for (int index = 0; index < this.statusItems.Count; ++index)
          {
            StatusItem statusItem = this.statusItems[index];
            if (statusItem.UseConditionalCallback(overlay, this.transform) || !HashedString.op_Inequality(overlay, OverlayModes.None.ID) || !HashedString.op_Inequality(statusItem.render_overlay, overlay))
            {
              float num5 = (float) ((double) num4 * (double) num2 * 2.0 - (double) num2 * (double) (num1 - 1));
              if (this.statusItems[index].sprite == null)
              {
                DebugUtil.DevLogError("Status Item " + this.statusItems[index].Id + " has null sprite for icon '" + this.statusItems[index].iconName + "', you need to add the sprite to the TintedSprites list in the GameAssets prefab manually.");
                this.statusItems[index].iconName = "status_item_exclamation";
                this.statusItems[index].sprite = Assets.GetTintedSprite("status_item_exclamation");
              }
              Sprite sprite = this.statusItems[index].sprite.sprite;
              meshBuilder.AddQuad(new Vector2(num5, 0.0f), new Vector2(num2, num2), z, sprite, Color32.op_Implicit(color32_1));
              ++num4;
            }
          }
          meshBuilder.AddQuad(new Vector2(0.0f, 0.29f + num3), new Vector2(0.05f + num3, 0.05f + num3), z, renderer.arrowSprite, Color32.op_Implicit(color32_1));
          meshBuilder.AddQuad(new Vector2(0.0f, 0.29f), new Vector2(0.05f, 0.05f), z, renderer.arrowSprite, Color32.op_Implicit(color32_3));
          meshBuilder.End(this.mesh);
          this.dirty = false;
        }
        if (!this.hasVisibleStatusItems || !Object.op_Inequality((Object) GameScreenManager.Instance, (Object) null))
          return;
        Graphics.DrawMesh(this.mesh, Vector3.op_Addition(pos, this.offset), Quaternion.identity, this.material, renderer.layer, camera, 0, (MaterialPropertyBlock) null, false, false);
      }
    }

    public void Add(StatusItem status_item)
    {
      this.statusItems.Add(status_item);
      this.dirty = true;
    }

    public void Remove(StatusItem status_item)
    {
      this.statusItems.Remove(status_item);
      this.dirty = true;
    }

    public void Replace(StatusItemRenderer.Entry entry)
    {
      this.handle = entry.handle;
      this.transform = entry.transform;
      this.building = ((Component) this.transform).GetComponent<Building>();
      this.buildingPos = TransformExtensions.GetPosition(this.transform);
      this.isBuilding = Object.op_Inequality((Object) this.building, (Object) null);
      this.selectable = ((Component) this.transform).GetComponent<KSelectable>();
      this.offset = entry.offset;
      this.dirty = true;
      this.statusItems.Clear();
      this.statusItems.AddRange((IEnumerable<StatusItem>) entry.statusItems);
    }

    private bool Intersects(Vector2 pos, float scale)
    {
      if (Object.op_Equality((Object) this.transform, (Object) null))
        return false;
      Bounds bounds = this.mesh.bounds;
      Vector3 vector3 = Vector3.op_Addition(Vector3.op_Addition(this.buildingPos, this.offset), ((Bounds) ref bounds).center);
      Vector2 vector2_1 = new Vector2(vector3.x, vector3.y);
      Vector3 size = ((Bounds) ref bounds).size;
      Vector2 vector2_2;
      // ISSUE: explicit constructor call
      ((Vector2) ref vector2_2).\u002Ector((float) ((double) size.x * (double) scale * 0.5), (float) ((double) size.y * (double) scale * 0.5));
      Vector2 vector2_3 = Vector2.op_Subtraction(vector2_1, vector2_2);
      Vector2 vector2_4 = Vector2.op_Addition(vector2_1, vector2_2);
      return (double) pos.x >= (double) vector2_3.x && (double) pos.x <= (double) vector2_4.x && (double) pos.y >= (double) vector2_3.y && (double) pos.y <= (double) vector2_4.y;
    }

    public void GetIntersection(
      Vector2 pos,
      List<InterfaceTool.Intersection> intersections,
      float scale)
    {
      if (!this.Intersects(pos, scale) || !this.selectable.IsSelectable)
        return;
      intersections.Add(new InterfaceTool.Intersection()
      {
        component = (MonoBehaviour) this.selectable,
        distance = -100f
      });
    }

    public void GetIntersection(Vector2 pos, List<KSelectable> selectables, float scale)
    {
      if (!this.Intersects(pos, scale) || !this.selectable.IsSelectable || selectables.Contains(this.selectable))
        return;
      selectables.Add(this.selectable);
    }

    public void Clear()
    {
      this.statusItems.Clear();
      this.offset = Vector3.zero;
      this.dirty = false;
    }

    public void FreeResources()
    {
      if (Object.op_Inequality((Object) this.mesh, (Object) null))
      {
        Object.DestroyImmediate((Object) this.mesh);
        this.mesh = (Mesh) null;
      }
      if (!Object.op_Inequality((Object) this.material, (Object) null))
        return;
      Object.DestroyImmediate((Object) this.material);
    }

    public void MarkDirty() => this.dirty = true;

    private struct MeshBuilder
    {
      private Vector3[] vertices;
      private Vector2[] uvs;
      private Vector2[] uv2s;
      private int[] triangles;
      private Color32[] colors;
      private int quadIdx;
      private Material material;
      private static int[] textureIds = new int[11]
      {
        Shader.PropertyToID("_Tex0"),
        Shader.PropertyToID("_Tex1"),
        Shader.PropertyToID("_Tex2"),
        Shader.PropertyToID("_Tex3"),
        Shader.PropertyToID("_Tex4"),
        Shader.PropertyToID("_Tex5"),
        Shader.PropertyToID("_Tex6"),
        Shader.PropertyToID("_Tex7"),
        Shader.PropertyToID("_Tex8"),
        Shader.PropertyToID("_Tex9"),
        Shader.PropertyToID("_Tex10")
      };

      public MeshBuilder(int quad_count, Material material)
      {
        this.vertices = new Vector3[4 * quad_count];
        this.uvs = new Vector2[4 * quad_count];
        this.uv2s = new Vector2[4 * quad_count];
        this.colors = new Color32[4 * quad_count];
        this.triangles = new int[6 * quad_count];
        this.material = material;
        this.quadIdx = 0;
      }

      public void AddQuad(Vector2 center, Vector2 half_size, float z, Sprite sprite, Color color)
      {
        if (this.quadIdx == StatusItemRenderer.Entry.MeshBuilder.textureIds.Length)
          return;
        Rect rect = sprite.rect;
        Rect textureRect = sprite.textureRect;
        float num1 = ((Rect) ref textureRect).width / ((Rect) ref rect).width;
        float num2 = ((Rect) ref textureRect).height / ((Rect) ref rect).height;
        int index1 = 4 * this.quadIdx;
        this.vertices[index1] = new Vector3((center.x - half_size.x) * num1, (center.y - half_size.y) * num2, z);
        this.vertices[1 + index1] = new Vector3((center.x - half_size.x) * num1, (center.y + half_size.y) * num2, z);
        this.vertices[2 + index1] = new Vector3((center.x + half_size.x) * num1, (center.y - half_size.y) * num2, z);
        this.vertices[3 + index1] = new Vector3((center.x + half_size.x) * num1, (center.y + half_size.y) * num2, z);
        float num3 = ((Rect) ref textureRect).x / (float) ((Texture) sprite.texture).width;
        float num4 = ((Rect) ref textureRect).y / (float) ((Texture) sprite.texture).height;
        float num5 = ((Rect) ref textureRect).width / (float) ((Texture) sprite.texture).width;
        float num6 = ((Rect) ref textureRect).height / (float) ((Texture) sprite.texture).height;
        this.uvs[index1] = new Vector2(num3, num4);
        this.uvs[1 + index1] = new Vector2(num3, num4 + num6);
        this.uvs[2 + index1] = new Vector2(num3 + num5, num4);
        this.uvs[3 + index1] = new Vector2(num3 + num5, num4 + num6);
        this.colors[index1] = Color32.op_Implicit(color);
        this.colors[1 + index1] = Color32.op_Implicit(color);
        this.colors[2 + index1] = Color32.op_Implicit(color);
        this.colors[3 + index1] = Color32.op_Implicit(color);
        float num7 = (float) this.quadIdx + 0.5f;
        this.uv2s[index1] = new Vector2(num7, 0.0f);
        this.uv2s[1 + index1] = new Vector2(num7, 0.0f);
        this.uv2s[2 + index1] = new Vector2(num7, 0.0f);
        this.uv2s[3 + index1] = new Vector2(num7, 0.0f);
        int index2 = 6 * this.quadIdx;
        this.triangles[index2] = index1;
        this.triangles[1 + index2] = index1 + 1;
        this.triangles[2 + index2] = index1 + 2;
        this.triangles[3 + index2] = index1 + 2;
        this.triangles[4 + index2] = index1 + 1;
        this.triangles[5 + index2] = index1 + 3;
        this.material.SetTexture(StatusItemRenderer.Entry.MeshBuilder.textureIds[this.quadIdx], (Texture) sprite.texture);
        ++this.quadIdx;
      }

      public void End(Mesh mesh)
      {
        mesh.Clear();
        mesh.vertices = this.vertices;
        mesh.uv = this.uvs;
        mesh.uv2 = this.uv2s;
        mesh.colors32 = this.colors;
        mesh.SetTriangles(this.triangles, 0);
        mesh.RecalculateBounds();
      }
    }
  }
}
