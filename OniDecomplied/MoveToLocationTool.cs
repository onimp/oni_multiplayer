// Decompiled with JetBrains decompiler
// Type: MoveToLocationTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class MoveToLocationTool : InterfaceTool
{
  public static MoveToLocationTool Instance;
  private Navigator targetNavigator;

  public static void DestroyInstance() => MoveToLocationTool.Instance = (MoveToLocationTool) null;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    MoveToLocationTool.Instance = this;
    this.visualizer = Util.KInstantiate(this.visualizer, (GameObject) null, (string) null);
  }

  public void Activate(Navigator navigator)
  {
    this.targetNavigator = navigator;
    PlayerController.Instance.ActivateTool((InterfaceTool) this);
  }

  public bool CanMoveTo(int target_cell) => this.targetNavigator.CanReach(target_cell);

  protected override void OnActivateTool()
  {
    base.OnActivateTool();
    this.visualizer.gameObject.SetActive(true);
  }

  protected override void OnDeactivateTool(InterfaceTool new_tool)
  {
    base.OnDeactivateTool(new_tool);
    if (Object.op_Inequality((Object) this.targetNavigator, (Object) null) && Object.op_Equality((Object) new_tool, (Object) SelectTool.Instance))
      SelectTool.Instance.SelectNextFrame(((Component) this.targetNavigator).GetComponent<KSelectable>(), true);
    this.visualizer.gameObject.SetActive(false);
  }

  public override void OnLeftClickDown(Vector3 cursor_pos)
  {
    base.OnLeftClickDown(cursor_pos);
    if (!Object.op_Inequality((Object) this.targetNavigator, (Object) null))
      return;
    int mouseCell = DebugHandler.GetMouseCell();
    MoveToLocationMonitor.Instance smi = ((Component) this.targetNavigator).GetSMI<MoveToLocationMonitor.Instance>();
    if (this.CanMoveTo(mouseCell) && smi != null)
    {
      KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click"));
      smi.MoveToLocation(mouseCell);
      SelectTool.Instance.Activate();
    }
    else
      KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative"));
  }

  private void RefreshColor()
  {
    Color white;
    // ISSUE: explicit constructor call
    ((Color) ref white).\u002Ector(0.91f, 0.21f, 0.2f);
    if (this.CanMoveTo(DebugHandler.GetMouseCell()))
      white = Color.white;
    this.SetColor(this.visualizer, white);
  }

  public override void OnMouseMove(Vector3 cursor_pos)
  {
    base.OnMouseMove(cursor_pos);
    this.RefreshColor();
  }

  private void SetColor(GameObject root, Color c) => ((Renderer) root.GetComponentInChildren<MeshRenderer>()).material.color = c;
}
