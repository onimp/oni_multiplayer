// Decompiled with JetBrains decompiler
// Type: ZoneTile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ProcGen;
using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ZoneTile")]
public class ZoneTile : KMonoBehaviour
{
  [MyCmpReq]
  public Building building;
  private bool wasReplaced;
  private static readonly EventSystem.IntraObjectHandler<ZoneTile> OnObjectReplacedDelegate = new EventSystem.IntraObjectHandler<ZoneTile>((Action<ZoneTile, object>) ((component, data) => component.OnObjectReplaced(data)));

  protected virtual void OnSpawn()
  {
    foreach (int placementCell in this.building.PlacementCells)
      SimMessages.ModifyCellWorldZone(placementCell, (byte) 0);
    this.Subscribe<ZoneTile>(1606648047, ZoneTile.OnObjectReplacedDelegate);
  }

  protected virtual void OnCleanUp()
  {
    if (this.wasReplaced)
      return;
    this.ClearZone();
  }

  private void OnObjectReplaced(object data)
  {
    this.ClearZone();
    this.wasReplaced = true;
  }

  private void ClearZone()
  {
    foreach (int placementCell in this.building.PlacementCells)
    {
      SubWorld.ZoneType subWorldZoneType = World.Instance.zoneRenderData.GetSubWorldZoneType(placementCell);
      byte zone_id = subWorldZoneType == 7 ? byte.MaxValue : (byte) subWorldZoneType;
      SimMessages.ModifyCellWorldZone(placementCell, zone_id);
    }
  }
}
