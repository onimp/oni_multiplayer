// Decompiled with JetBrains decompiler
// Type: ObjectLayerListItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class ObjectLayerListItem
{
  private int cell = Grid.InvalidCell;
  private ObjectLayer layer;

  public ObjectLayerListItem previousItem { get; private set; }

  public ObjectLayerListItem nextItem { get; private set; }

  public GameObject gameObject { get; private set; }

  public ObjectLayerListItem(GameObject gameObject, ObjectLayer layer, int new_cell)
  {
    this.gameObject = gameObject;
    this.layer = layer;
    this.Refresh(new_cell);
  }

  public void Clear() => this.Refresh(Grid.InvalidCell);

  public bool Refresh(int new_cell)
  {
    if (this.cell == new_cell)
      return false;
    if (this.cell != Grid.InvalidCell && Object.op_Equality((Object) Grid.Objects[this.cell, (int) this.layer], (Object) this.gameObject))
    {
      GameObject gameObject = (GameObject) null;
      if (this.nextItem != null && Object.op_Inequality((Object) this.nextItem.gameObject, (Object) null))
        gameObject = this.nextItem.gameObject;
      Grid.Objects[this.cell, (int) this.layer] = gameObject;
    }
    if (this.previousItem != null)
      this.previousItem.nextItem = this.nextItem;
    if (this.nextItem != null)
      this.nextItem.previousItem = this.previousItem;
    this.previousItem = (ObjectLayerListItem) null;
    this.nextItem = (ObjectLayerListItem) null;
    this.cell = new_cell;
    if (this.cell != Grid.InvalidCell)
    {
      GameObject gameObject = Grid.Objects[this.cell, (int) this.layer];
      if (Object.op_Inequality((Object) gameObject, (Object) null) && Object.op_Inequality((Object) gameObject, (Object) this.gameObject))
      {
        ObjectLayerListItem objectLayerListItem = gameObject.GetComponent<Pickupable>().objectLayerListItem;
        this.nextItem = objectLayerListItem;
        objectLayerListItem.previousItem = this;
      }
      Grid.Objects[this.cell, (int) this.layer] = this.gameObject;
    }
    return true;
  }

  public bool Update(int cell) => this.Refresh(cell);
}
