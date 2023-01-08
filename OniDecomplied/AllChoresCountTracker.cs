// Decompiled with JetBrains decompiler
// Type: AllChoresCountTracker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class AllChoresCountTracker : WorldTracker
{
  public AllChoresCountTracker(int worldID)
    : base(worldID)
  {
  }

  public override void UpdateData()
  {
    float num = 0.0f;
    for (int index = 0; index < ((ResourceSet) Db.Get().ChoreGroups).Count; ++index)
    {
      Tracker choreGroupTracker = (Tracker) TrackerTool.Instance.GetChoreGroupTracker(this.WorldID, Db.Get().ChoreGroups[index]);
      num += choreGroupTracker == null ? 0.0f : choreGroupTracker.GetCurrentValue();
    }
    this.AddPoint(num);
  }

  public override string FormatValueString(float value) => value.ToString();
}
