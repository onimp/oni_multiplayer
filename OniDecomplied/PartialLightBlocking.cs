// Decompiled with JetBrains decompiler
// Type: PartialLightBlocking
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using UnityEngine;

[SerializationConfig]
public class PartialLightBlocking : KMonoBehaviour
{
  private const byte PartialLightBlockingProperties = 48;

  protected virtual void OnSpawn()
  {
    this.SetLightBlocking();
    base.OnSpawn();
  }

  protected virtual void OnCleanUp()
  {
    this.ClearLightBlocking();
    base.OnCleanUp();
  }

  public void SetLightBlocking()
  {
    foreach (int placementCell in ((Component) this).GetComponent<Building>().PlacementCells)
      SimMessages.SetCellProperties(placementCell, (byte) 48);
  }

  public void ClearLightBlocking()
  {
    foreach (int placementCell in ((Component) this).GetComponent<Building>().PlacementCells)
      SimMessages.ClearCellProperties(placementCell, (byte) 48);
  }
}
