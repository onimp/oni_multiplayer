// Decompiled with JetBrains decompiler
// Type: VisibilityTester
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/VisibilityTester")]
public class VisibilityTester : KMonoBehaviour
{
  public static VisibilityTester Instance;
  public bool enableTesting;

  public static void DestroyInstance() => VisibilityTester.Instance = (VisibilityTester) null;

  protected virtual void OnPrefabInit() => VisibilityTester.Instance = this;

  private void Update()
  {
    if (Object.op_Equality((Object) SelectTool.Instance, (Object) null) || Object.op_Equality((Object) SelectTool.Instance.selected, (Object) null) || !this.enableTesting)
      return;
    int cell = Grid.PosToCell((KMonoBehaviour) SelectTool.Instance.selected);
    int mouseCell = DebugHandler.GetMouseCell();
    string text = "" + "Source Cell: " + cell.ToString() + "\n" + "Target Cell: " + mouseCell.ToString() + "\n" + "Visible: " + Grid.VisibilityTest(cell, mouseCell).ToString();
    for (int index = 0; index < 10000; ++index)
      Grid.VisibilityTest(cell, mouseCell);
    DebugText.Instance.Draw(text, Grid.CellToPosCCC(mouseCell, Grid.SceneLayer.Move), Color.white);
  }
}
