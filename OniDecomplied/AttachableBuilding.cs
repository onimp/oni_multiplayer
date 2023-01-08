// Decompiled with JetBrains decompiler
// Type: AttachableBuilding
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/AttachableBuilding")]
public class AttachableBuilding : KMonoBehaviour
{
  public Tag attachableToTag;
  public Action<object> onAttachmentNetworkChanged;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.RegisterWithAttachPoint(true);
    Components.AttachableBuildings.Add(this);
    foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(this))
    {
      AttachableBuilding component = gameObject.GetComponent<AttachableBuilding>();
      if (Object.op_Inequality((Object) component, (Object) null) && component.onAttachmentNetworkChanged != null)
        component.onAttachmentNetworkChanged((object) this);
    }
  }

  protected virtual void OnSpawn() => base.OnSpawn();

  public void RegisterWithAttachPoint(bool register)
  {
    BuildingDef buildingDef = (BuildingDef) null;
    BuildingComplete component1 = ((Component) this).GetComponent<BuildingComplete>();
    BuildingUnderConstruction component2 = ((Component) this).GetComponent<BuildingUnderConstruction>();
    if (Object.op_Inequality((Object) component1, (Object) null))
      buildingDef = component1.Def;
    else if (Object.op_Inequality((Object) component2, (Object) null))
      buildingDef = component2.Def;
    int num = Grid.OffsetCell(Grid.PosToCell(((Component) this).gameObject), buildingDef.attachablePosition);
    bool flag = false;
    for (int idx = 0; !flag && idx < Components.BuildingAttachPoints.Count; ++idx)
    {
      for (int index = 0; index < Components.BuildingAttachPoints[idx].points.Length; ++index)
      {
        if (num == Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) Components.BuildingAttachPoints[idx]), Components.BuildingAttachPoints[idx].points[index].position))
        {
          if (register)
            Components.BuildingAttachPoints[idx].points[index].attachedBuilding = this;
          else if (Object.op_Equality((Object) Components.BuildingAttachPoints[idx].points[index].attachedBuilding, (Object) this))
            Components.BuildingAttachPoints[idx].points[index].attachedBuilding = (AttachableBuilding) null;
          flag = true;
          break;
        }
      }
    }
  }

  public static void GetAttachedBelow(
    AttachableBuilding searchStart,
    ref List<GameObject> buildings)
  {
    AttachableBuilding attachableBuilding = searchStart;
    while (Object.op_Inequality((Object) attachableBuilding, (Object) null))
    {
      BuildingAttachPoint attachedTo = attachableBuilding.GetAttachedTo();
      attachableBuilding = (AttachableBuilding) null;
      if (Object.op_Inequality((Object) attachedTo, (Object) null))
      {
        buildings.Add(((Component) attachedTo).gameObject);
        attachableBuilding = ((Component) attachedTo).GetComponent<AttachableBuilding>();
      }
    }
  }

  public static int CountAttachedBelow(AttachableBuilding searchStart)
  {
    int num = 0;
    AttachableBuilding attachableBuilding = searchStart;
    while (Object.op_Inequality((Object) attachableBuilding, (Object) null))
    {
      BuildingAttachPoint attachedTo = attachableBuilding.GetAttachedTo();
      attachableBuilding = (AttachableBuilding) null;
      if (Object.op_Inequality((Object) attachedTo, (Object) null))
      {
        ++num;
        attachableBuilding = ((Component) attachedTo).GetComponent<AttachableBuilding>();
      }
    }
    return num;
  }

  public static void GetAttachedAbove(
    AttachableBuilding searchStart,
    ref List<GameObject> buildings)
  {
    BuildingAttachPoint buildingAttachPoint = ((Component) searchStart).GetComponent<BuildingAttachPoint>();
    while (Object.op_Inequality((Object) buildingAttachPoint, (Object) null))
    {
      bool flag = false;
      foreach (BuildingAttachPoint.HardPoint point in buildingAttachPoint.points)
      {
        if (!flag)
        {
          if (Object.op_Inequality((Object) point.attachedBuilding, (Object) null))
          {
            foreach (AttachableBuilding attachableBuilding in Components.AttachableBuildings)
            {
              if (Object.op_Equality((Object) attachableBuilding, (Object) point.attachedBuilding))
              {
                buildings.Add(((Component) attachableBuilding).gameObject);
                buildingAttachPoint = ((Component) attachableBuilding).GetComponent<BuildingAttachPoint>();
                flag = true;
              }
            }
          }
        }
        else
          break;
      }
      if (!flag)
        buildingAttachPoint = (BuildingAttachPoint) null;
    }
  }

  public static void NotifyBuildingsNetworkChanged(
    List<GameObject> buildings,
    AttachableBuilding attachable = null)
  {
    foreach (GameObject building in buildings)
    {
      AttachableBuilding component = building.GetComponent<AttachableBuilding>();
      if (Object.op_Inequality((Object) component, (Object) null) && component.onAttachmentNetworkChanged != null)
        component.onAttachmentNetworkChanged((object) attachable);
    }
  }

  public static List<GameObject> GetAttachedNetwork(AttachableBuilding searchStart)
  {
    List<GameObject> buildings = new List<GameObject>();
    buildings.Add(((Component) searchStart).gameObject);
    AttachableBuilding.GetAttachedAbove(searchStart, ref buildings);
    AttachableBuilding.GetAttachedBelow(searchStart, ref buildings);
    return buildings;
  }

  public BuildingAttachPoint GetAttachedTo()
  {
    for (int idx = 0; idx < Components.BuildingAttachPoints.Count; ++idx)
    {
      for (int index = 0; index < Components.BuildingAttachPoints[idx].points.Length; ++index)
      {
        if (Object.op_Equality((Object) Components.BuildingAttachPoints[idx].points[index].attachedBuilding, (Object) this) && (Object.op_Equality((Object) ((Component) Components.BuildingAttachPoints[idx].points[index].attachedBuilding).GetComponent<Deconstructable>(), (Object) null) || !((Component) Components.BuildingAttachPoints[idx].points[index].attachedBuilding).GetComponent<Deconstructable>().HasBeenDestroyed))
          return Components.BuildingAttachPoints[idx];
      }
    }
    return (BuildingAttachPoint) null;
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    AttachableBuilding.NotifyBuildingsNetworkChanged(AttachableBuilding.GetAttachedNetwork(this), this);
    this.RegisterWithAttachPoint(false);
    Components.AttachableBuildings.Remove(this);
  }
}
