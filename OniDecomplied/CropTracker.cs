// Decompiled with JetBrains decompiler
// Type: CropTracker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class CropTracker : WorldTracker
{
  public CropTracker(int worldID)
    : base(worldID)
  {
  }

  public override void UpdateData()
  {
    float num = 0.0f;
    foreach (PlantablePlot plantablePlot in Components.PlantablePlots.GetItems(this.WorldID))
    {
      if (!Object.op_Equality((Object) plantablePlot.plant, (Object) null) && plantablePlot.HasDepositTag(GameTags.CropSeed) && !plantablePlot.plant.HasTag(GameTags.Wilting))
        ++num;
    }
    this.AddPoint(num);
  }

  public override string FormatValueString(float value) => value.ToString() + "%";
}
