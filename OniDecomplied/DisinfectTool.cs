// Decompiled with JetBrains decompiler
// Type: DisinfectTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class DisinfectTool : DragTool
{
  public static DisinfectTool Instance;

  public static void DestroyInstance() => DisinfectTool.Instance = (DisinfectTool) null;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    DisinfectTool.Instance = this;
    this.interceptNumberKeysForPriority = true;
    this.viewMode = OverlayModes.Disease.ID;
  }

  public void Activate() => PlayerController.Instance.ActivateTool((InterfaceTool) this);

  protected override void OnDragTool(int cell, int distFromOrigin)
  {
    for (int layer = 0; layer < 44; ++layer)
    {
      GameObject gameObject = Grid.Objects[cell, layer];
      if (Object.op_Inequality((Object) gameObject, (Object) null))
      {
        Disinfectable component = gameObject.GetComponent<Disinfectable>();
        if (Object.op_Inequality((Object) component, (Object) null) && ((Component) component).GetComponent<PrimaryElement>().DiseaseCount > 0)
          component.MarkForDisinfect();
      }
    }
  }
}
