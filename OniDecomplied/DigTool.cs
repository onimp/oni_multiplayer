// Decompiled with JetBrains decompiler
// Type: DigTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class DigTool : DragTool
{
  public static DigTool Instance;

  public static void DestroyInstance() => DigTool.Instance = (DigTool) null;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    DigTool.Instance = this;
  }

  protected override void OnDragTool(int cell, int distFromOrigin)
  {
    InterfaceTool.ActiveConfig.DigAction.Uproot(cell);
    InterfaceTool.ActiveConfig.DigAction.Dig(cell, distFromOrigin);
  }

  public static GameObject PlaceDig(int cell, int animationDelay = 0)
  {
    if (Grid.Solid[cell] && !Grid.Foundation[cell] && Object.op_Equality((Object) Grid.Objects[cell, 7], (Object) null))
    {
      for (int layer = 0; layer < 44; ++layer)
      {
        if (Object.op_Inequality((Object) Grid.Objects[cell, layer], (Object) null) && Object.op_Inequality((Object) Grid.Objects[cell, layer].GetComponent<Constructable>(), (Object) null))
          return (GameObject) null;
      }
      GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(new Tag("DigPlacer")), (GameObject) null, (string) null);
      gameObject.SetActive(true);
      Grid.Objects[cell, 7] = gameObject;
      Vector3 posCbc = Grid.CellToPosCBC(cell, DigTool.Instance.visualizerLayer);
      float num = -0.15f;
      posCbc.z += num;
      TransformExtensions.SetPosition(gameObject.transform, posCbc);
      gameObject.GetComponentInChildren<EasingAnimations>().PlayAnimation("ScaleUp", Mathf.Max(0.0f, (float) animationDelay * 0.02f));
      return gameObject;
    }
    return Object.op_Inequality((Object) Grid.Objects[cell, 7], (Object) null) ? Grid.Objects[cell, 7] : (GameObject) null;
  }

  protected override void OnActivateTool()
  {
    base.OnActivateTool();
    ToolMenu.Instance.PriorityScreen.Show(true);
  }

  protected override void OnDeactivateTool(InterfaceTool new_tool)
  {
    base.OnDeactivateTool(new_tool);
    ToolMenu.Instance.PriorityScreen.Show(false);
  }
}
