// Decompiled with JetBrains decompiler
// Type: CarePackageInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class CarePackageInfo : ITelepadDeliverable
{
  public readonly string id;
  public readonly float quantity;
  public readonly Func<bool> requirement;
  public readonly string facadeID;

  public CarePackageInfo(string ID, float amount, Func<bool> requirement)
  {
    this.id = ID;
    this.quantity = amount;
    this.requirement = requirement;
  }

  public CarePackageInfo(string ID, float amount, Func<bool> requirement, string facadeID)
  {
    this.id = ID;
    this.quantity = amount;
    this.requirement = requirement;
    this.facadeID = facadeID;
  }

  public GameObject Deliver(Vector3 location)
  {
    location = Vector3.op_Addition(location, Vector3.op_Division(Vector3.right, 2f));
    GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(CarePackageConfig.ID)), location);
    gameObject.SetActive(true);
    gameObject.GetComponent<CarePackage>().SetInfo(this);
    return gameObject;
  }
}
