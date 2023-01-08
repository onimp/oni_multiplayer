// Decompiled with JetBrains decompiler
// Type: BreathabilityTracker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class BreathabilityTracker : WorldTracker
{
  public BreathabilityTracker(int worldID)
    : base(worldID)
  {
  }

  public override void UpdateData()
  {
    float num = 0.0f;
    int count = Components.LiveMinionIdentities.GetWorldItems(this.WorldID).Count;
    if (count == 0)
    {
      this.AddPoint(0.0f);
    }
    else
    {
      foreach (Component worldItem in Components.LiveMinionIdentities.GetWorldItems(this.WorldID))
      {
        OxygenBreather component = worldItem.GetComponent<OxygenBreather>();
        if (component.GetGasProvider() is GasBreatherFromWorldProvider)
        {
          if (component.IsBreathableElement)
          {
            num += 100f;
            if (component.IsLowOxygen())
              num -= 50f;
          }
        }
        else if (!component.IsSuffocating)
        {
          num += 100f;
          if (component.IsLowOxygen())
            num -= 50f;
        }
      }
      this.AddPoint((float) Mathf.RoundToInt(num / (float) count));
    }
  }

  public override string FormatValueString(float value) => value.ToString() + "%";
}
