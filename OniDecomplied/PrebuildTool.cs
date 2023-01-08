// Decompiled with JetBrains decompiler
// Type: PrebuildTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class PrebuildTool : InterfaceTool
{
  public static PrebuildTool Instance;
  private BuildingDef def;

  public static void DestroyInstance() => PrebuildTool.Instance = (PrebuildTool) null;

  protected virtual void OnPrefabInit() => PrebuildTool.Instance = this;

  protected override void OnActivateTool()
  {
    this.viewMode = this.def.ViewMode;
    base.OnActivateTool();
  }

  public void Activate(BuildingDef def, string errorMessage)
  {
    this.def = def;
    PlayerController.Instance.ActivateTool((InterfaceTool) this);
    PrebuildToolHoverTextCard component = ((Component) this).GetComponent<PrebuildToolHoverTextCard>();
    component.errorMessage = errorMessage;
    component.currentDef = def;
  }

  public override void OnLeftClickDown(Vector3 cursor_pos)
  {
    UISounds.PlaySound(UISounds.Sound.Negative);
    base.OnLeftClickDown(cursor_pos);
  }
}
