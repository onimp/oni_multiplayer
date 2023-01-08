// Decompiled with JetBrains decompiler
// Type: Reservable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Reservable")]
public class Reservable : KMonoBehaviour
{
  private GameObject reservedBy;

  public GameObject ReservedBy => this.reservedBy;

  public bool isReserved => !Object.op_Equality((Object) this.reservedBy, (Object) null);

  public bool Reserve(GameObject reserver)
  {
    if (!Object.op_Equality((Object) this.reservedBy, (Object) null))
      return false;
    this.reservedBy = reserver;
    return true;
  }

  public void ClearReservation(GameObject reserver)
  {
    if (!Object.op_Equality((Object) this.reservedBy, (Object) reserver))
      return;
    this.reservedBy = (GameObject) null;
  }
}
