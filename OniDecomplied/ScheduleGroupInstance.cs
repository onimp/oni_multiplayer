// Decompiled with JetBrains decompiler
// Type: ScheduleGroupInstance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;

[SerializationConfig]
public class ScheduleGroupInstance
{
  [Serialize]
  private string scheduleGroupID;
  [Serialize]
  public int segments;

  public ScheduleGroup scheduleGroup
  {
    get => Db.Get().ScheduleGroups.Get(this.scheduleGroupID);
    set => this.scheduleGroupID = value.Id;
  }

  public ScheduleGroupInstance(ScheduleGroup scheduleGroup)
  {
    this.scheduleGroup = scheduleGroup;
    this.segments = scheduleGroup.defaultSegments;
  }
}
