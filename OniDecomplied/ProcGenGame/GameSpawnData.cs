// Decompiled with JetBrains decompiler
// Type: ProcGenGame.GameSpawnData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.CustomSettings;
using KSerialization;
using System.Collections.Generic;
using TemplateClasses;
using UnityEngine;

namespace ProcGenGame
{
  [SerializationConfig]
  public class GameSpawnData
  {
    public Vector2I baseStartPos;
    public List<Prefab> buildings = new List<Prefab>();
    public List<Prefab> pickupables = new List<Prefab>();
    public List<Prefab> elementalOres = new List<Prefab>();
    public List<Prefab> otherEntities = new List<Prefab>();
    public List<KeyValuePair<Vector2I, bool>> preventFoWReveal = new List<KeyValuePair<Vector2I, bool>>();

    public void AddRange(IEnumerable<KeyValuePair<int, string>> newItems)
    {
      foreach (KeyValuePair<int, string> newItem in newItems)
      {
        Vector2I xy = Grid.CellToXY(newItem.Key);
        this.otherEntities.Add(new Prefab(newItem.Value, Prefab.Type.Other, xy.x, xy.y, (SimHashes) 0));
      }
    }

    public void AddTemplate(
      TemplateContainer template,
      Vector2I position,
      ref Dictionary<int, int> claimedCells)
    {
      int cell1 = Grid.XYToCell(position.x, position.y);
      bool flag = true;
      if (DlcManager.IsExpansion1Active() && Object.op_Inequality((Object) CustomGameSettings.Instance, (Object) null))
        flag = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.Teleporters).id == "Enabled";
      if (template.buildings != null)
      {
        foreach (Prefab building in template.buildings)
        {
          if (!claimedCells.ContainsKey(Grid.OffsetCell(cell1, building.location_x, building.location_y)) && (flag || !this.IsWarpTeleporter(building)))
            this.buildings.Add(building.Clone(position));
        }
      }
      if (template.pickupables != null)
      {
        foreach (Prefab pickupable in template.pickupables)
        {
          if (!claimedCells.ContainsKey(Grid.OffsetCell(cell1, pickupable.location_x, pickupable.location_y)))
            this.pickupables.Add(pickupable.Clone(position));
        }
      }
      if (template.elementalOres != null)
      {
        foreach (Prefab elementalOre in template.elementalOres)
        {
          if (!claimedCells.ContainsKey(Grid.OffsetCell(cell1, elementalOre.location_x, elementalOre.location_y)))
            this.elementalOres.Add(elementalOre.Clone(position));
        }
      }
      if (template.otherEntities != null)
      {
        foreach (Prefab otherEntity in template.otherEntities)
        {
          if (!claimedCells.ContainsKey(Grid.OffsetCell(cell1, otherEntity.location_x, otherEntity.location_y)) && (flag || !this.IsWarpTeleporter(otherEntity)))
            this.otherEntities.Add(otherEntity.Clone(position));
        }
      }
      if (template.cells == null)
        return;
      for (int index = 0; index < template.cells.Count; ++index)
      {
        int cell2 = Grid.XYToCell(position.x + template.cells[index].location_x, position.y + template.cells[index].location_y);
        if (!claimedCells.ContainsKey(cell2))
        {
          claimedCells[cell2] = 1;
          this.preventFoWReveal.Add(new KeyValuePair<Vector2I, bool>(new Vector2I(position.x + template.cells[index].location_x, position.y + template.cells[index].location_y), template.cells[index].preventFoWReveal));
        }
        else
          ++claimedCells[cell2];
      }
    }

    private bool IsWarpTeleporter(Prefab prefab) => prefab.id == "WarpPortal" || prefab.id == WarpReceiverConfig.ID || prefab.id == "WarpConduitSender" || prefab.id == "WarpConduitReceiver";
  }
}
