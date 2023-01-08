// Decompiled with JetBrains decompiler
// Type: GridLayouter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class GridLayouter
{
  public float minCellSize = -1f;
  public float maxCellSize = -1f;
  public GridLayoutGroup targetGridLayout;
  private Vector2 oldScreenSize;
  private float oldScreenScale;
  private int framesLeftToResizeGrid;

  [Conditional("UNITY_EDITOR")]
  private void ValidateImportantFieldsAreSet()
  {
    Debug.Assert((double) this.minCellSize >= 0.0, (object) string.Format("[{0} Error] Minimum cell size is invalid. Given: {1}", (object) nameof (GridLayouter), (object) this.minCellSize));
    Debug.Assert((double) this.maxCellSize >= 0.0, (object) string.Format("[{0} Error] Maximum cell size is invalid. Given: {1}", (object) nameof (GridLayouter), (object) this.maxCellSize));
    Debug.Assert(Object.op_Inequality((Object) this.targetGridLayout, (Object) null) && Object.op_Implicit((Object) this.targetGridLayout), (object) string.Format("[{0} Error] Target grid layout is invalid. Given: {1}", (object) nameof (GridLayouter), (object) this.targetGridLayout));
  }

  public void CheckIfShouldResizeGrid()
  {
    Vector2 vector2;
    // ISSUE: explicit constructor call
    ((Vector2) ref vector2).\u002Ector((float) Screen.width, (float) Screen.height);
    if (Vector2.op_Inequality(vector2, this.oldScreenSize))
      this.RequestGridResize();
    this.oldScreenSize = vector2;
    float num = KPlayerPrefs.GetFloat(KCanvasScaler.UIScalePrefKey);
    if ((double) num != (double) this.oldScreenScale)
      this.RequestGridResize();
    this.oldScreenScale = num;
    this.ResizeGridIfRequested();
  }

  public void RequestGridResize() => this.framesLeftToResizeGrid = 3;

  private void ResizeGridIfRequested()
  {
    if (this.framesLeftToResizeGrid <= 0)
      return;
    this.ImmediateSizeGridToScreenResolution();
    --this.framesLeftToResizeGrid;
  }

  public void ImmediateSizeGridToScreenResolution()
  {
    Rect rect = Util.rectTransform((Component) ((Component) this.targetGridLayout).transform.parent).rect;
    float workingWidth = ((Rect) ref rect).size.x - (float) ((LayoutGroup) this.targetGridLayout).padding.left - (float) ((LayoutGroup) this.targetGridLayout).padding.right;
    float x = this.targetGridLayout.spacing.x;
    int count = GetCellCountToFit(this.maxCellSize, x, workingWidth) + 1;
    float num;
    for (num = GetCellSize(workingWidth, x, count); (double) num < (double) this.minCellSize; num = Mathf.Min(this.maxCellSize, GetCellSize(workingWidth, x, count)))
    {
      --count;
      if (count <= 0)
      {
        count = 1;
        num = this.minCellSize;
        break;
      }
    }
    ((LayoutGroup) this.targetGridLayout).childAlignment = count == 1 ? (TextAnchor) 1 : (TextAnchor) 0;
    this.targetGridLayout.constraint = (GridLayoutGroup.Constraint) 1;
    this.targetGridLayout.constraintCount = count;
    this.targetGridLayout.cellSize = Vector2.op_Multiply(Vector2.one, num);

    static float GetCellSize(float workingWidth, float spacingSize, int count) => (workingWidth - (float) ((double) spacingSize * (double) count - 1.0)) / (float) count;

    static int GetCellCountToFit(float cellSize, float spacingSize, float workingWidth)
    {
      int cellCountToFit = 0;
      for (float num = cellSize; (double) num < (double) workingWidth; num += cellSize + spacingSize)
        ++cellCountToFit;
      return cellCountToFit;
    }
  }
}
