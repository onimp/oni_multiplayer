// Decompiled with JetBrains decompiler
// Type: DevToolBigBaseMutations
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ImGuiNET;
using UnityEngine;

public class DevToolBigBaseMutations : DevTool
{
  protected override void RenderTo(DevPanel panel)
  {
    if (Object.op_Inequality((Object) Game.Instance, (Object) null))
      this.ShowButtons();
    else
      ImGui.Text("Game not available");
  }

  private void ShowButtons()
  {
    if (ImGui.Button("Destroy Ladders"))
      this.DestroyGameObjects<Ladder>(Components.Ladders, Tag.Invalid);
    if (ImGui.Button("Destroy Tiles"))
      this.DestroyGameObjects<BuildingComplete>(Components.BuildingCompletes, GameTags.FloorTiles);
    if (ImGui.Button("Destroy Wires"))
      this.DestroyGameObjects<BuildingComplete>(Components.BuildingCompletes, GameTags.Wires);
    if (!ImGui.Button("Destroy Pipes"))
      return;
    this.DestroyGameObjects<BuildingComplete>(Components.BuildingCompletes, GameTags.Pipes);
  }

  private void DestroyGameObjects<T>(Components.Cmps<T> componentsList, Tag filterForTag)
  {
    for (int idx = componentsList.Count - 1; idx >= 0; --idx)
    {
      if (!Util.IsNullOrDestroyed((object) componentsList[idx]) && (!Tag.op_Inequality(filterForTag, Tag.Invalid) || ((Component) ((object) componentsList[idx] as KMonoBehaviour)).gameObject.HasTag(filterForTag)))
        Util.KDestroyGameObject((Component) ((object) componentsList[idx] as KMonoBehaviour));
    }
  }
}
