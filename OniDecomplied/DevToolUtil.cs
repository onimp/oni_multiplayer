// Decompiled with JetBrains decompiler
// Type: DevToolUtil
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public static class DevToolUtil
{
  public static DevPanel Open(DevTool devTool) => DevToolManager.Instance.panels.AddPanelFor(devTool);

  public static DevPanel Open<T>() where T : DevTool, new() => DevToolManager.Instance.panels.AddPanelFor<T>();

  public static void Close(DevTool devTool) => devTool.ClosePanel();

  public static void Close(DevPanel devPanel) => devPanel.Close();

  public static string GenerateDevToolName(DevTool devTool) => DevToolUtil.GenerateDevToolName(devTool.GetType());

  public static string GenerateDevToolName(System.Type devToolType)
  {
    string devToolName1;
    if (DevToolManager.Instance != null && DevToolManager.Instance.devToolNameDict.TryGetValue(devToolType, out devToolName1))
      return devToolName1;
    string devToolName2 = devToolType.Name;
    if (devToolName2.StartsWith("DevTool_"))
      devToolName2 = devToolName2.Substring("DevTool_".Length);
    else if (devToolName2.StartsWith("DevTool"))
      devToolName2 = devToolName2.Substring("DevTool".Length);
    return devToolName2;
  }

  public static bool CanRevealAndFocus(GameObject gameObject) => DevToolUtil.GetCellIndexFor(gameObject).HasValue;

  public static void RevealAndFocus(GameObject gameObject)
  {
    Option<int> cellIndexFor = DevToolUtil.GetCellIndexFor(gameObject);
    if (!cellIndexFor.HasValue)
      return;
    DevToolUtil.RevealAndFocusAt(cellIndexFor.Value);
    if (!Util.IsNullOrDestroyed((object) gameObject.GetComponent<KSelectable>()))
      SelectTool.Instance.Select(gameObject.GetComponent<KSelectable>());
    else
      SelectTool.Instance.Select((KSelectable) null);
  }

  public static void FocusCameraOnCell(int cellIndex) => CameraController.Instance.SetPosition(Grid.CellToPos2D(cellIndex));

  public static Option<int> GetCellIndexFor(GameObject gameObject)
  {
    if (Util.IsNullOrDestroyed((object) gameObject))
      return (Option<int>) Option.None;
    return !Util.IsNullOrDestroyed((object) gameObject.GetComponent<RectTransform>()) ? (Option<int>) Option.None : (Option<int>) Grid.PosToCell(gameObject);
  }

  public static Option<int> GetCellIndexForUniqueBuilding(string prefabId)
  {
    BuildingComplete[] objectsOfType = Object.FindObjectsOfType<BuildingComplete>(true);
    if (objectsOfType == null)
      return (Option<int>) Option.None;
    foreach (BuildingComplete buildingComplete in objectsOfType)
    {
      if (prefabId == buildingComplete.Def.PrefabID)
        return (Option<int>) buildingComplete.GetCell();
    }
    return (Option<int>) Option.None;
  }

  public static void RevealAndFocusAt(int cellIndex)
  {
    int x1;
    int y1;
    Grid.CellToXY(cellIndex, out x1, out y1);
    GridVisibility.Reveal(x1 + 2, y1 + 2, 10, 10f);
    DevToolUtil.FocusCameraOnCell(cellIndex);
    Option<int> forUniqueBuilding = DevToolUtil.GetCellIndexForUniqueBuilding("Headquarters");
    if (!forUniqueBuilding.HasValue)
      return;
    Vector3 pos2D1 = Grid.CellToPos2D(cellIndex);
    Vector3 pos2D2 = Grid.CellToPos2D((int) forUniqueBuilding);
    float num1 = 2f / Vector3.Distance(pos2D1, pos2D2);
    for (float num2 = 0.0f; (double) num2 < 1.0; num2 += num1)
    {
      int x2;
      int y2;
      Grid.PosToXY(Vector3.Lerp(pos2D1, pos2D2, num2), out x2, out y2);
      GridVisibility.Reveal(x2 + 2, y2 + 2, 4, 4f);
    }
  }
}
