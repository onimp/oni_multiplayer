// Decompiled with JetBrains decompiler
// Type: LightShapePreview
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/LightShapePreview")]
public class LightShapePreview : KMonoBehaviour
{
  public float radius;
  public int lux;
  public LightShape shape;
  public CellOffset offset;
  private int previousCell = -1;

  private void Update()
  {
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
    if (cell == this.previousCell)
      return;
    this.previousCell = cell;
    LightGridManager.DestroyPreview();
    LightGridManager.CreatePreview(Grid.OffsetCell(cell, this.offset), this.radius, this.shape, this.lux);
  }

  protected virtual void OnCleanUp() => LightGridManager.DestroyPreview();
}
