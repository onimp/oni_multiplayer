// Decompiled with JetBrains decompiler
// Type: CustomOutfit
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class CustomOutfit : KMonoBehaviour
{
  private Accessorizer accessorizer;

  protected virtual void OnPrefabInit() => this.accessorizer = ((Component) this).GetComponent<Accessorizer>();

  public void Update()
  {
    if (!((HashedString) ref this.accessorizer.bodyData.legs).IsValid)
      return;
    KCompBuilder.BodyData bodyData = this.accessorizer.bodyData;
    int num = Hash.SDBMLower("Jorge");
    bodyData.neck = HashedString.op_Implicit(string.Format("neck_{0}", (object) num));
    bodyData.legs = HashedString.op_Implicit(string.Format("leg_{0}", (object) num));
    bodyData.belt = HashedString.op_Implicit(string.Format("belt_{0}", (object) num));
    bodyData.pelvis = HashedString.op_Implicit(string.Format("pelvis_{0}", (object) num));
    bodyData.foot = HashedString.op_Implicit(string.Format("foot_{0}", (object) num));
    bodyData.hand = HashedString.op_Implicit(string.Format("hand_paint_{0}", (object) num));
    bodyData.cuff = HashedString.op_Implicit(string.Format("cuff_{0}", (object) num));
    this.accessorizer.bodyData = bodyData;
    ((Behaviour) this).enabled = false;
  }
}
