// Decompiled with JetBrains decompiler
// Type: PumpingStationGuide
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/PumpingStationGuide")]
public class PumpingStationGuide : KMonoBehaviour, IRenderEveryTick
{
  private int previousDepthAvailable = -1;
  public GameObject parent;
  public bool occupyTiles;
  private KBatchedAnimController parentController;
  private KBatchedAnimController guideController;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.parentController = this.parent.GetComponent<KBatchedAnimController>();
    this.guideController = ((Component) this).GetComponent<KBatchedAnimController>();
    this.RefreshTint();
    this.RefreshDepthAvailable();
  }

  private void RefreshTint() => this.guideController.TintColour = this.parentController.TintColour;

  private void RefreshDepthAvailable()
  {
    int depthAvailable = PumpingStationGuide.GetDepthAvailable(Grid.PosToCell((KMonoBehaviour) this), this.parent);
    if (depthAvailable == this.previousDepthAvailable)
      return;
    KBatchedAnimController component = ((Component) this).GetComponent<KBatchedAnimController>();
    if (depthAvailable == 0)
    {
      component.enabled = false;
    }
    else
    {
      component.enabled = true;
      component.Play(new HashedString("place_pipe" + depthAvailable.ToString()));
    }
    if (this.occupyTiles)
      PumpingStationGuide.OccupyArea(this.parent, depthAvailable);
    this.previousDepthAvailable = depthAvailable;
  }

  public void RenderEveryTick(float dt)
  {
    this.RefreshTint();
    this.RefreshDepthAvailable();
  }

  public static void OccupyArea(GameObject go, int depth_available)
  {
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(go.transform));
    for (int index = 1; index <= depth_available; ++index)
    {
      int key1 = Grid.OffsetCell(cell, 0, -index);
      int key2 = Grid.OffsetCell(cell, 1, -index);
      Grid.ObjectLayers[1][key1] = go;
      Grid.ObjectLayers[1][key2] = go;
    }
  }

  public static int GetDepthAvailable(int root_cell, GameObject pump)
  {
    int num1 = 4;
    int depthAvailable = 0;
    for (int index = 1; index <= num1; ++index)
    {
      int num2 = Grid.OffsetCell(root_cell, 0, -index);
      int num3 = Grid.OffsetCell(root_cell, 1, -index);
      if (Grid.IsValidCell(num2) && !Grid.Solid[num2] && Grid.IsValidCell(num3) && !Grid.Solid[num3] && (!Grid.ObjectLayers[1].ContainsKey(num2) || Object.op_Equality((Object) Grid.ObjectLayers[1][num2], (Object) null) || Object.op_Equality((Object) Grid.ObjectLayers[1][num2], (Object) pump)) && (!Grid.ObjectLayers[1].ContainsKey(num3) || Object.op_Equality((Object) Grid.ObjectLayers[1][num3], (Object) null) || Object.op_Equality((Object) Grid.ObjectLayers[1][num3], (Object) pump)))
        depthAvailable = index;
      else
        break;
    }
    return depthAvailable;
  }
}
