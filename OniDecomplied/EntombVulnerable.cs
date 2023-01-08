// Decompiled with JetBrains decompiler
// Type: EntombVulnerable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using UnityEngine;

public class EntombVulnerable : KMonoBehaviour, IWiltCause
{
  [MyCmpReq]
  private KSelectable selectable;
  private OccupyArea _occupyArea;
  [Serialize]
  private bool isEntombed;
  private HandleVector<int>.Handle partitionerEntry;
  private static readonly Func<int, object, bool> IsCellSafeCBDelegate = (Func<int, object, bool>) ((cell, data) => EntombVulnerable.IsCellSafeCB(cell, data));

  private OccupyArea occupyArea
  {
    get
    {
      if (Object.op_Equality((Object) this._occupyArea, (Object) null))
        this._occupyArea = ((Component) this).GetComponent<OccupyArea>();
      return this._occupyArea;
    }
  }

  public bool GetEntombed => this.isEntombed;

  public string WiltStateString => Db.Get().CreatureStatusItems.Entombed.resolveStringCallback((string) CREATURES.STATUSITEMS.ENTOMBED.LINE_ITEM, (object) ((Component) this).gameObject);

  public WiltCondition.Condition[] Conditions => new WiltCondition.Condition[1]
  {
    WiltCondition.Condition.Entombed
  };

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.partitionerEntry = GameScenePartitioner.Instance.Add(nameof (EntombVulnerable), (object) ((Component) this).gameObject, this.occupyArea.GetExtents(), GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnSolidChanged));
    this.CheckEntombed();
    if (!this.isEntombed)
      return;
    ((Component) this).GetComponent<KPrefabID>().AddTag(GameTags.Entombed, false);
    this.Trigger(-1089732772, (object) true);
  }

  protected virtual void OnCleanUp()
  {
    GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
    base.OnCleanUp();
  }

  private void OnSolidChanged(object data) => this.CheckEntombed();

  private void CheckEntombed()
  {
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(((Component) this).gameObject.transform));
    if (!Grid.IsValidCell(cell))
      return;
    if (!this.IsCellSafe(cell))
    {
      if (this.isEntombed)
        return;
      this.isEntombed = true;
      this.selectable.AddStatusItem(Db.Get().CreatureStatusItems.Entombed, (object) ((Component) this).gameObject);
      ((Component) this).GetComponent<KPrefabID>().AddTag(GameTags.Entombed, false);
      this.Trigger(-1089732772, (object) true);
    }
    else
    {
      if (!this.isEntombed)
        return;
      this.isEntombed = false;
      this.selectable.RemoveStatusItem(Db.Get().CreatureStatusItems.Entombed);
      ((Component) this).GetComponent<KPrefabID>().RemoveTag(GameTags.Entombed);
      this.Trigger(-1089732772, (object) false);
    }
  }

  public bool IsCellSafe(int cell) => this.occupyArea.TestArea(cell, (object) null, EntombVulnerable.IsCellSafeCBDelegate);

  private static bool IsCellSafeCB(int cell, object data) => Grid.IsValidCell(cell) && !Grid.Solid[cell];
}
