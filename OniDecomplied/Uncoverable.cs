// Decompiled with JetBrains decompiler
// Type: Uncoverable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Uncoverable")]
public class Uncoverable : KMonoBehaviour
{
  [MyCmpReq]
  private OccupyArea occupyArea;
  [Serialize]
  private bool hasBeenUncovered;
  private HandleVector<int>.Handle partitionerEntry;
  private static readonly Func<int, object, bool> IsCellBlockedDelegate = (Func<int, object, bool>) ((cell, data) => Uncoverable.IsCellBlocked(cell, data));

  public bool IsUncovered => this.hasBeenUncovered;

  private bool IsAnyCellShowing() => !this.occupyArea.TestArea(Grid.PosToCell((KMonoBehaviour) this), (object) null, Uncoverable.IsCellBlockedDelegate);

  private static bool IsCellBlocked(int cell, object data) => Grid.Element[cell].IsSolid && !Grid.Foundation[cell];

  protected virtual void OnPrefabInit() => base.OnPrefabInit();

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (this.IsAnyCellShowing())
      this.hasBeenUncovered = true;
    if (this.hasBeenUncovered)
      return;
    ((Component) this).GetComponent<KSelectable>().IsSelectable = false;
    this.partitionerEntry = GameScenePartitioner.Instance.Add("Uncoverable.OnSpawn", (object) ((Component) this).gameObject, this.occupyArea.GetExtents(), GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnSolidChanged));
  }

  private void OnSolidChanged(object data)
  {
    if (!this.IsAnyCellShowing() || this.hasBeenUncovered || !this.partitionerEntry.IsValid())
      return;
    GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
    this.hasBeenUncovered = true;
    ((Component) this).GetComponent<KSelectable>().IsSelectable = true;
    Notification notification = new Notification((string) MISC.STATUSITEMS.BURIEDITEM.NOTIFICATION, NotificationType.Good, new Func<List<Notification>, object, string>(Uncoverable.OnNotificationToolTip), (object) this);
    ((Component) this).gameObject.AddOrGet<Notifier>().Add(notification);
  }

  private static string OnNotificationToolTip(List<Notification> notifications, object data)
  {
    Uncoverable cmp = (Uncoverable) data;
    return MISC.STATUSITEMS.BURIEDITEM.NOTIFICATION_TOOLTIP.Replace("{Uncoverable}", ((Component) cmp).GetProperName());
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
  }
}
