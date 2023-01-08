// Decompiled with JetBrains decompiler
// Type: KleiItemDropScreen_PermitVis_DupeEquipment
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using UnityEngine;

public class KleiItemDropScreen_PermitVis_DupeEquipment : 
  KMonoBehaviour,
  IKleiItemDropScreen_PermitVis_Target
{
  [SerializeField]
  private KBatchedAnimController droppedItemKAnim;
  [SerializeField]
  private KBatchedAnimController dupeKAnim;

  public void ConfigureWith(PermitResource permit, PermitPresentationInfo permitPresInfo)
  {
    ((Component) this.dupeKAnim).GetComponent<UIDupeRandomizer>().Randomize();
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit(permitPresInfo.buildOverride));
    this.dupeKAnim.AddAnimOverrides(anim);
    KAnimHashedString symbol1;
    // ISSUE: explicit constructor call
    ((KAnimHashedString) ref symbol1).\u002Ector("snapto_neck");
    KAnim.Build.Symbol symbol2 = anim.GetData().build.GetSymbol(symbol1);
    if (symbol2 != null)
    {
      ((Component) this.dupeKAnim).GetComponent<SymbolOverrideController>().AddSymbolOverride(HashedString.op_Implicit(symbol1), symbol2, 6);
      this.dupeKAnim.SetSymbolVisiblity(symbol1, true);
    }
    else
    {
      ((Component) this.dupeKAnim).GetComponent<SymbolOverrideController>().RemoveSymbolOverride(HashedString.op_Implicit(symbol1), 6);
      this.dupeKAnim.SetSymbolVisiblity(symbol1, false);
    }
    this.dupeKAnim.Play(HashedString.op_Implicit("idle_default"), (KAnim.PlayMode) 0);
    this.dupeKAnim.Queue(HashedString.op_Implicit("cheer_pre"));
    this.dupeKAnim.Queue(HashedString.op_Implicit("cheer_loop"));
    this.dupeKAnim.Queue(HashedString.op_Implicit("cheer_pst"));
    this.dupeKAnim.Queue(HashedString.op_Implicit("idle_default"), (KAnim.PlayMode) 0);
  }
}
