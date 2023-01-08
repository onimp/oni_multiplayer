// Decompiled with JetBrains decompiler
// Type: FogOfWarMask
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/FogOfWarMask")]
public class FogOfWarMask : KMonoBehaviour
{
  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    Grid.OnReveal += new Action<int>(this.OnReveal);
  }

  private void OnReveal(int cell)
  {
    if (Grid.PosToCell((KMonoBehaviour) this) != cell)
      return;
    Grid.OnReveal -= new Action<int>(this.OnReveal);
    TracesExtesions.DeleteObject(((Component) this).gameObject);
  }

  protected virtual void OnCmpEnable()
  {
    base.OnCmpEnable();
    GameUtil.FloodCollectCells(Grid.PosToCell((KMonoBehaviour) this), (Func<int, bool>) (cell =>
    {
      Grid.Visible[cell] = (byte) 0;
      Grid.PreventFogOfWarReveal[cell] = true;
      return !Grid.Solid[cell];
    }));
    GameUtil.FloodCollectCells(Grid.PosToCell((KMonoBehaviour) this), (Func<int, bool>) (cell =>
    {
      int num = Grid.PreventFogOfWarReveal[cell] ? 1 : 0;
      if (Grid.Solid[cell] && Grid.Foundation[cell])
      {
        Grid.PreventFogOfWarReveal[cell] = true;
        Grid.Visible[cell] = (byte) 0;
        GameObject gameObject = Grid.Objects[cell, 1];
        if (Object.op_Inequality((Object) gameObject, (Object) null) && gameObject.GetComponent<KPrefabID>().PrefabTag.ToString() == "POIBunkerExteriorDoor")
        {
          Grid.PreventFogOfWarReveal[cell] = false;
          Grid.Visible[cell] = byte.MaxValue;
        }
      }
      return num != 0 || Grid.Foundation[cell];
    }));
  }

  public static void ClearMask(int cell)
  {
    if (!Grid.PreventFogOfWarReveal[cell])
      return;
    GameUtil.FloodCollectCells(cell, new Func<int, bool>(FogOfWarMask.RevealFogOfWarMask));
  }

  public static bool RevealFogOfWarMask(int cell)
  {
    int num = Grid.PreventFogOfWarReveal[cell] ? 1 : 0;
    if (num == 0)
      return num != 0;
    Grid.PreventFogOfWarReveal[cell] = false;
    Grid.Reveal(cell);
    return num != 0;
  }
}
