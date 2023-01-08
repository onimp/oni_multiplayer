// Decompiled with JetBrains decompiler
// Type: GridVisibility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/GridVisibility")]
public class GridVisibility : KMonoBehaviour
{
  public int radius = 18;
  public float innerRadius = 16.5f;

  protected virtual void OnSpawn()
  {
    Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(this.transform, new System.Action(this.OnCellChange), "GridVisibility.OnSpawn");
    this.OnCellChange();
    ((Component) this).gameObject.GetMyWorld().SetDiscovered();
  }

  private void OnCellChange()
  {
    if (((Component) this).gameObject.HasTag(GameTags.Dead))
      return;
    int cell = Grid.PosToCell((KMonoBehaviour) this);
    if (!Grid.IsValidCell(cell))
      return;
    if (!Grid.Revealed[cell])
    {
      int x;
      int y;
      Grid.PosToXY(TransformExtensions.GetPosition(this.transform), out x, out y);
      GridVisibility.Reveal(x, y, this.radius, this.innerRadius);
      Grid.Revealed[cell] = true;
    }
    FogOfWarMask.ClearMask(cell);
  }

  public static void Reveal(int baseX, int baseY, int radius, float innerRadius)
  {
    int num1 = (int) Grid.WorldIdx[baseY * Grid.WidthInCells + baseX];
    for (int index1 = -radius; index1 <= radius; ++index1)
    {
      for (int index2 = -radius; index2 <= radius; ++index2)
      {
        int num2 = baseY + index1;
        int num3 = baseX + index2;
        if (num2 >= 0 && Grid.HeightInCells - 1 >= num2 && num3 >= 0 && Grid.WidthInCells - 1 >= num3)
        {
          int cell = num2 * Grid.WidthInCells + num3;
          if (Grid.Visible[cell] < byte.MaxValue && num1 == (int) Grid.WorldIdx[cell])
          {
            Vector2 vector2;
            // ISSUE: explicit constructor call
            ((Vector2) ref vector2).\u002Ector((float) index2, (float) index1);
            float num4 = Mathf.Lerp(1f, 0.0f, (float) (((double) ((Vector2) ref vector2).magnitude - (double) innerRadius) / ((double) radius - (double) innerRadius)));
            Grid.Reveal(cell, (byte) ((double) byte.MaxValue * (double) num4));
          }
        }
      }
    }
  }

  protected virtual void OnCleanUp() => Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(this.transform, new System.Action(this.OnCellChange));
}
