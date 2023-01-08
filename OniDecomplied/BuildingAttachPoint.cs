// Decompiled with JetBrains decompiler
// Type: BuildingAttachPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/BuildingAttachPoint")]
public class BuildingAttachPoint : KMonoBehaviour
{
  public BuildingAttachPoint.HardPoint[] points = new BuildingAttachPoint.HardPoint[0];

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    Components.BuildingAttachPoints.Add(this);
    this.TryAttachEmptyHardpoints();
  }

  protected virtual void OnSpawn() => base.OnSpawn();

  private void TryAttachEmptyHardpoints()
  {
    for (int index = 0; index < this.points.Length; ++index)
    {
      if (!Object.op_Inequality((Object) this.points[index].attachedBuilding, (Object) null))
      {
        bool flag = false;
        for (int idx = 0; idx < Components.AttachableBuildings.Count && !flag; ++idx)
        {
          if (Tag.op_Equality(Components.AttachableBuildings[idx].attachableToTag, this.points[index].attachableType) && Grid.OffsetCell(Grid.PosToCell(((Component) this).gameObject), this.points[index].position) == Grid.PosToCell((KMonoBehaviour) Components.AttachableBuildings[idx]))
          {
            this.points[index].attachedBuilding = Components.AttachableBuildings[idx];
            flag = true;
          }
        }
      }
    }
  }

  public bool AcceptsAttachment(Tag type, int cell)
  {
    int cell1 = Grid.PosToCell(((Component) this).gameObject);
    for (int index = 0; index < this.points.Length; ++index)
    {
      if (Grid.OffsetCell(cell1, this.points[index].position) == cell && Tag.op_Equality(this.points[index].attachableType, type))
        return true;
    }
    return false;
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    Components.BuildingAttachPoints.Remove(this);
  }

  [Serializable]
  public struct HardPoint
  {
    public CellOffset position;
    public Tag attachableType;
    public AttachableBuilding attachedBuilding;

    public HardPoint(CellOffset position, Tag attachableType, AttachableBuilding attachedBuilding)
    {
      this.position = position;
      this.attachableType = attachableType;
      this.attachedBuilding = attachedBuilding;
    }
  }
}
