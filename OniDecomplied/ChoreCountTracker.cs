// Decompiled with JetBrains decompiler
// Type: ChoreCountTracker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class ChoreCountTracker : WorldTracker
{
  public ChoreGroup choreGroup;

  public ChoreCountTracker(int worldID, ChoreGroup group)
    : base(worldID)
  {
    this.choreGroup = group;
  }

  public override void UpdateData()
  {
    float num = 0.0f;
    List<Chore> choreList;
    GlobalChoreProvider.Instance.choreWorldMap.TryGetValue(this.WorldID, out choreList);
    for (int index = 0; choreList != null && index < choreList.Count; ++index)
    {
      Chore chore = choreList[index];
      if (chore != null && !((object) chore.target).Equals((object) null) && !Object.op_Equality((Object) chore.gameObject, (Object) null))
      {
        foreach (ChoreGroup group in chore.choreType.groups)
        {
          if (group == this.choreGroup)
          {
            ++num;
            break;
          }
        }
      }
    }
    List<FetchChore> fetchChoreList;
    GlobalChoreProvider.Instance.fetchMap.TryGetValue(this.WorldID, out fetchChoreList);
    for (int index = 0; fetchChoreList != null && index < fetchChoreList.Count; ++index)
    {
      Chore chore = (Chore) fetchChoreList[index];
      if (chore != null && !((object) chore.target).Equals((object) null) && !Object.op_Equality((Object) chore.gameObject, (Object) null))
      {
        foreach (ChoreGroup group in chore.choreType.groups)
        {
          if (group == this.choreGroup)
          {
            ++num;
            break;
          }
        }
      }
    }
    this.AddPoint(num);
  }

  public override string FormatValueString(float value) => value.ToString();
}
