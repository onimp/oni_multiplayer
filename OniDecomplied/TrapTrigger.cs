// Decompiled with JetBrains decompiler
// Type: TrapTrigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class TrapTrigger : KMonoBehaviour
{
  private HandleVector<int>.Handle partitionerEntry;
  public Tag[] trappableCreatures;
  public Vector2 trappedOffset = Vector2.zero;
  [MyCmpReq]
  private Storage storage;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    GameObject gameObject = ((Component) this).gameObject;
    this.partitionerEntry = GameScenePartitioner.Instance.Add("Trap", (object) gameObject, Grid.PosToCell(gameObject), GameScenePartitioner.Instance.trapsLayer, new Action<object>(this.OnCreatureOnTrap));
    foreach (GameObject go in this.storage.items)
    {
      this.SetStoredPosition(go);
      KBoxCollider2D component = go.GetComponent<KBoxCollider2D>();
      if (Object.op_Inequality((Object) component, (Object) null))
        ((Behaviour) component).enabled = true;
    }
  }

  private void SetStoredPosition(GameObject go)
  {
    Vector3 posCbc = Grid.CellToPosCBC(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)), Grid.SceneLayer.BuildingBack);
    posCbc.x += this.trappedOffset.x;
    posCbc.y += this.trappedOffset.y;
    TransformExtensions.SetPosition(go.transform, posCbc);
    go.GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.BuildingBack);
  }

  public void OnCreatureOnTrap(object data)
  {
    if (!this.storage.IsEmpty())
      return;
    Trappable cmp = (Trappable) data;
    if (((Component) cmp).HasTag(GameTags.Stored) || ((Component) cmp).HasTag(GameTags.Trapped) || ((Component) cmp).HasTag(GameTags.Creatures.Bagged))
      return;
    bool flag = false;
    foreach (Tag trappableCreature in this.trappableCreatures)
    {
      if (((Component) cmp).HasTag(trappableCreature))
      {
        flag = true;
        break;
      }
    }
    if (!flag)
      return;
    this.storage.Store(((Component) cmp).gameObject, true);
    this.SetStoredPosition(((Component) cmp).gameObject);
    this.Trigger(-358342870, (object) ((Component) cmp).gameObject);
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
  }
}
