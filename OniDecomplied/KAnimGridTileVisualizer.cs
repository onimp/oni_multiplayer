// Decompiled with JetBrains decompiler
// Type: KAnimGridTileVisualizer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Rendering;
using System;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/KAnimGridTileVisualizer")]
public class KAnimGridTileVisualizer : KMonoBehaviour, IBlockTileInfo
{
  [SerializeField]
  public int blockTileConnectorID;
  private static readonly EventSystem.IntraObjectHandler<KAnimGridTileVisualizer> OnSelectionChangedDelegate = new EventSystem.IntraObjectHandler<KAnimGridTileVisualizer>((Action<KAnimGridTileVisualizer, object>) ((component, data) => component.OnSelectionChanged(data)));
  private static readonly EventSystem.IntraObjectHandler<KAnimGridTileVisualizer> OnHighlightChangedDelegate = new EventSystem.IntraObjectHandler<KAnimGridTileVisualizer>((Action<KAnimGridTileVisualizer, object>) ((component, data) => component.OnHighlightChanged(data)));

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.Subscribe<KAnimGridTileVisualizer>(-1503271301, KAnimGridTileVisualizer.OnSelectionChangedDelegate);
    this.Subscribe<KAnimGridTileVisualizer>(-1201923725, KAnimGridTileVisualizer.OnHighlightChangedDelegate);
  }

  protected virtual void OnCleanUp()
  {
    Building component = ((Component) this).GetComponent<Building>();
    if (Object.op_Inequality((Object) component, (Object) null))
    {
      int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
      ObjectLayer tileLayer = component.Def.TileLayer;
      if (Object.op_Equality((Object) Grid.Objects[cell, (int) tileLayer], (Object) ((Component) this).gameObject))
        Grid.Objects[cell, (int) tileLayer] = (GameObject) null;
      TileVisualizer.RefreshCell(cell, tileLayer, component.Def.ReplacementLayer);
    }
    base.OnCleanUp();
  }

  private void OnSelectionChanged(object data)
  {
    bool enabled = (bool) data;
    World.Instance.blockTileRenderer.SelectCell(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)), enabled);
  }

  private void OnHighlightChanged(object data)
  {
    bool enabled = (bool) data;
    World.Instance.blockTileRenderer.HighlightCell(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)), enabled);
  }

  public int GetBlockTileConnectorID() => this.blockTileConnectorID;
}
