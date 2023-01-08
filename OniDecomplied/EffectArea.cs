// Decompiled with JetBrains decompiler
// Type: EffectArea
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/EffectArea")]
public class EffectArea : KMonoBehaviour
{
  public string EffectName;
  public int Area;
  private Effect Effect;

  protected virtual void OnPrefabInit() => this.Effect = Db.Get().effects.Get(this.EffectName);

  private void Update()
  {
    int x1 = 0;
    int y1 = 0;
    Grid.PosToXY(TransformExtensions.GetPosition(this.transform), out x1, out y1);
    foreach (MinionIdentity minionIdentity in Components.MinionIdentities.Items)
    {
      int x2 = 0;
      int y2 = 0;
      Grid.PosToXY(TransformExtensions.GetPosition(minionIdentity.transform), out x2, out y2);
      if (Math.Abs(x2 - x1) <= this.Area && Math.Abs(y2 - y1) <= this.Area)
        ((Component) minionIdentity).GetComponent<Effects>().Add(this.Effect, true);
    }
  }
}
