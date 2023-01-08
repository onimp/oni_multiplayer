// Decompiled with JetBrains decompiler
// Type: BuildingUnderConstruction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class BuildingUnderConstruction : Building
{
  [MyCmpAdd]
  private KSelectable selectable;
  [MyCmpAdd]
  private SaveLoadRoot saveLoadRoot;
  [MyCmpAdd]
  private KPrefabID kPrefabID;

  protected virtual void OnPrefabInit()
  {
    Vector3 position = TransformExtensions.GetPosition(this.transform);
    position.z = Grid.GetLayerZ(this.Def.SceneLayer);
    TransformExtensions.SetPosition(this.transform, position);
    Util.SetLayerRecursively(((Component) this).gameObject, LayerMask.NameToLayer("Construction"));
    KBatchedAnimController component1 = ((Component) this).GetComponent<KBatchedAnimController>();
    Rotatable component2 = ((Component) this).GetComponent<Rotatable>();
    if (Object.op_Inequality((Object) component1, (Object) null) && Object.op_Equality((Object) component2, (Object) null))
      component1.Offset = this.Def.GetVisualizerOffset();
    KBoxCollider2D component3 = ((Component) this).GetComponent<KBoxCollider2D>();
    if (Object.op_Inequality((Object) component3, (Object) null))
    {
      Vector3 visualizerOffset = this.Def.GetVisualizerOffset();
      component3.offset = Vector2.op_Addition(component3.offset, new Vector2(visualizerOffset.x, visualizerOffset.y));
    }
    base.OnPrefabInit();
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    if (this.Def.IsTilePiece)
      this.Def.RunOnArea(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)), this.Orientation, (Action<int>) (c => TileVisualizer.RefreshCell(c, this.Def.TileLayer, this.Def.ReplacementLayer)));
    this.RegisterBlockTileRenderer();
  }

  protected override void OnCleanUp()
  {
    this.UnregisterBlockTileRenderer();
    base.OnCleanUp();
  }
}
