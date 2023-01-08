// Decompiled with JetBrains decompiler
// Type: PreventFOWRevealTracker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/PreventFOWRevealTracker")]
public class PreventFOWRevealTracker : KMonoBehaviour
{
  [Serialize]
  public List<int> preventFOWRevealCells;

  [OnSerializing]
  private void OnSerialize()
  {
    this.preventFOWRevealCells.Clear();
    for (int i = 0; i < Grid.VisMasks.Length; ++i)
    {
      if (Grid.PreventFogOfWarReveal[i])
        this.preventFOWRevealCells.Add(i);
    }
  }

  [System.Runtime.Serialization.OnDeserialized]
  private void OnDeserialized()
  {
    foreach (int preventFowRevealCell in this.preventFOWRevealCells)
      Grid.PreventFogOfWarReveal[preventFowRevealCell] = true;
  }
}
