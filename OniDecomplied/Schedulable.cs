// Decompiled with JetBrains decompiler
// Type: Schedulable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Schedulable")]
public class Schedulable : KMonoBehaviour
{
  public Schedule GetSchedule() => ScheduleManager.Instance.GetSchedule(this);

  public bool IsAllowed(ScheduleBlockType schedule_block_type)
  {
    WorldContainer myWorld = ((Component) this).gameObject.GetMyWorld();
    if (Object.op_Equality((Object) myWorld, (Object) null))
    {
      DebugUtil.LogWarningArgs(new object[1]
      {
        (object) string.Format("Trying to schedule {0} but {1} is not on a valid world. Grid cell: {2}", (object) schedule_block_type.Id, (object) ((Object) ((Component) this).gameObject).name, (object) Grid.PosToCell((KMonoBehaviour) ((Component) this).gameObject.GetComponent<KPrefabID>()))
      });
      return false;
    }
    return myWorld.AlertManager.IsRedAlert() || ScheduleManager.Instance.IsAllowed(this, schedule_block_type);
  }

  public void OnScheduleChanged(Schedule schedule) => this.Trigger(467134493, (object) schedule);

  public void OnScheduleBlocksTick(Schedule schedule) => this.Trigger(1714332666, (object) schedule);

  public void OnScheduleBlocksChanged(Schedule schedule) => this.Trigger(-894023145, (object) schedule);
}
