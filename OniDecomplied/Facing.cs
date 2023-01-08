// Decompiled with JetBrains decompiler
// Type: Facing
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/Facing")]
public class Facing : KMonoBehaviour
{
  [MyCmpGet]
  private KAnimControllerBase kanimController;
  private LoggerFS log;
  private bool facingLeft;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.log = new LoggerFS(nameof (Facing), 35);
  }

  public void Face(float target_x)
  {
    float x = TransformExtensions.GetLocalPosition(this.transform).x;
    if ((double) target_x < (double) x)
    {
      this.facingLeft = true;
      this.UpdateMirror();
    }
    else
    {
      if ((double) target_x <= (double) x)
        return;
      this.facingLeft = false;
      this.UpdateMirror();
    }
  }

  public void Face(Vector3 target_pos)
  {
    int num1 = Grid.CellColumn(Grid.PosToCell(TransformExtensions.GetLocalPosition(this.transform)));
    int num2 = Grid.CellColumn(Grid.PosToCell(target_pos));
    if (num1 > num2)
    {
      this.facingLeft = true;
      this.UpdateMirror();
    }
    else
    {
      if (num2 <= num1)
        return;
      this.facingLeft = false;
      this.UpdateMirror();
    }
  }

  [ContextMenu("Flip")]
  public void SwapFacing()
  {
    this.facingLeft = !this.facingLeft;
    this.UpdateMirror();
  }

  private void UpdateMirror()
  {
    if (!Object.op_Inequality((Object) this.kanimController, (Object) null) || this.kanimController.FlipX == this.facingLeft)
      return;
    this.kanimController.FlipX = this.facingLeft;
    int num = this.facingLeft ? 1 : 0;
  }

  public bool GetFacing() => this.facingLeft;

  public void SetFacing(bool mirror_x)
  {
    this.facingLeft = mirror_x;
    this.UpdateMirror();
  }

  public int GetFrontCell()
  {
    int cell = Grid.PosToCell((KMonoBehaviour) this);
    return this.GetFacing() ? Grid.CellLeft(cell) : Grid.CellRight(cell);
  }

  public int GetBackCell()
  {
    int cell = Grid.PosToCell((KMonoBehaviour) this);
    return !this.GetFacing() ? Grid.CellLeft(cell) : Grid.CellRight(cell);
  }
}
