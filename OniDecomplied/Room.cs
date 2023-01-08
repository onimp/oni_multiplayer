// Decompiled with JetBrains decompiler
// Type: Room
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class Room : IAssignableIdentity
{
  public CavityInfo cavity;
  public RoomType roomType;
  private List<KPrefabID> primary_buildings = new List<KPrefabID>();
  private List<Ownables> current_owners = new List<Ownables>();

  public List<KPrefabID> buildings => this.cavity.buildings;

  public List<KPrefabID> plants => this.cavity.plants;

  public string GetProperName() => this.roomType.Name;

  public List<Ownables> GetOwners()
  {
    this.current_owners.Clear();
    foreach (KPrefabID primaryEntity in this.GetPrimaryEntities())
    {
      if (Object.op_Inequality((Object) primaryEntity, (Object) null))
      {
        Ownable component = ((Component) primaryEntity).GetComponent<Ownable>();
        if (Object.op_Inequality((Object) component, (Object) null) && component.assignee != null && component.assignee != this)
        {
          foreach (Ownables owner in component.assignee.GetOwners())
          {
            if (!this.current_owners.Contains(owner))
              this.current_owners.Add(owner);
          }
        }
      }
    }
    return this.current_owners;
  }

  public List<GameObject> GetBuildingsOnFloor()
  {
    List<GameObject> buildingsOnFloor = new List<GameObject>();
    for (int index = 0; index < this.buildings.Count; ++index)
    {
      if (!Grid.Solid[Grid.PosToCell((KMonoBehaviour) this.buildings[index])] && Grid.Solid[Grid.CellBelow(Grid.PosToCell((KMonoBehaviour) this.buildings[index]))])
        buildingsOnFloor.Add(((Component) this.buildings[index]).gameObject);
    }
    return buildingsOnFloor;
  }

  public Ownables GetSoleOwner()
  {
    List<Ownables> owners = this.GetOwners();
    return owners.Count <= 0 ? (Ownables) null : owners[0];
  }

  public bool HasOwner(Assignables owner) => Object.op_Inequality((Object) this.GetOwners().Find((Predicate<Ownables>) (x => Object.op_Equality((Object) x, (Object) owner))), (Object) null);

  public int NumOwners() => this.GetOwners().Count;

  public List<KPrefabID> GetPrimaryEntities()
  {
    this.primary_buildings.Clear();
    RoomType roomType = this.roomType;
    if (roomType.primary_constraint != null)
    {
      foreach (KPrefabID building in this.buildings)
      {
        if (Object.op_Inequality((Object) building, (Object) null) && roomType.primary_constraint.building_criteria(building))
          this.primary_buildings.Add(building);
      }
      foreach (KPrefabID plant in this.plants)
      {
        if (Object.op_Inequality((Object) plant, (Object) null) && roomType.primary_constraint.building_criteria(plant))
          this.primary_buildings.Add(plant);
      }
    }
    return this.primary_buildings;
  }

  public void RetriggerBuildings()
  {
    foreach (KPrefabID building in this.buildings)
    {
      if (!Object.op_Equality((Object) building, (Object) null))
        ((KMonoBehaviour) building).Trigger(144050788, (object) this);
    }
    foreach (KPrefabID plant in this.plants)
    {
      if (!Object.op_Equality((Object) plant, (Object) null))
        ((KMonoBehaviour) plant).Trigger(144050788, (object) this);
    }
  }

  public bool IsNull() => false;

  public void CleanUp() => Game.Instance.assignmentManager.RemoveFromAllGroups((IAssignableIdentity) this);
}
