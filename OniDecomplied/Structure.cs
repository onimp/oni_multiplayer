// Decompiled with JetBrains decompiler
// Type: Structure
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/Structure")]
public class Structure : KMonoBehaviour
{
  [MyCmpReq]
  private Building building;
  [MyCmpReq]
  private PrimaryElement primaryElement;
  [MyCmpReq]
  private Operational operational;
  public static readonly Operational.Flag notEntombedFlag = new Operational.Flag("not_entombed", Operational.Flag.Type.Functional);
  private bool isEntombed;
  private HandleVector<int>.Handle partitionerEntry;
  private static EventSystem.IntraObjectHandler<Structure> RocketLandedDelegate = new EventSystem.IntraObjectHandler<Structure>((Action<Structure, object>) ((cmp, data) => cmp.RocketChanged(data)));

  public bool IsEntombed() => this.isEntombed;

  public static bool IsBuildingEntombed(Building building)
  {
    if (!Grid.IsValidCell(Grid.PosToCell((KMonoBehaviour) building)))
      return false;
    for (int index = 0; index < building.PlacementCells.Length; ++index)
    {
      int placementCell = building.PlacementCells[index];
      if (Grid.Element[placementCell].IsSolid && !Grid.Foundation[placementCell])
        return true;
    }
    return false;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.partitionerEntry = GameScenePartitioner.Instance.Add("Structure.OnSpawn", (object) ((Component) this).gameObject, this.building.GetExtents(), GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnSolidChanged));
    this.OnSolidChanged((object) null);
    this.Subscribe<Structure>(-887025858, Structure.RocketLandedDelegate);
  }

  public void UpdatePosition() => GameScenePartitioner.Instance.UpdatePosition(this.partitionerEntry, this.building.GetExtents());

  private void RocketChanged(object data) => this.OnSolidChanged(data);

  private void OnSolidChanged(object data)
  {
    bool flag = Structure.IsBuildingEntombed(this.building);
    if (flag == this.isEntombed)
      return;
    this.isEntombed = flag;
    if (this.isEntombed)
      ((Component) this).GetComponent<KPrefabID>().AddTag(GameTags.Entombed, false);
    else
      ((Component) this).GetComponent<KPrefabID>().RemoveTag(GameTags.Entombed);
    this.operational.SetFlag(Structure.notEntombedFlag, !this.isEntombed);
    ((Component) this).GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.Entombed, this.isEntombed, (object) this);
    this.Trigger(-1089732772, (object) null);
  }

  protected virtual void OnCleanUp() => GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
}
