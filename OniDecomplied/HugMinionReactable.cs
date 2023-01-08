// Decompiled with JetBrains decompiler
// Type: HugMinionReactable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using UnityEngine;

public class HugMinionReactable : Reactable
{
  public HugMinionReactable(GameObject gameObject)
    : base(gameObject, HashedString.op_Implicit(nameof (HugMinionReactable)), Db.Get().ChoreTypes.Hug, 1, 1, true, 1f, overrideLayer: ObjectLayer.Minion)
  {
  }

  public override bool InternalCanBegin(
    GameObject newReactor,
    Navigator.ActiveTransition transition)
  {
    if (Object.op_Inequality((Object) this.reactor, (Object) null))
      return false;
    Navigator component = newReactor.GetComponent<Navigator>();
    return !Object.op_Equality((Object) component, (Object) null) && component.IsMoving();
  }

  public override void Update(float dt) => this.gameObject.GetComponent<Facing>().SetFacing(this.reactor.GetComponent<Facing>().GetFacing());

  protected override void InternalBegin()
  {
    KAnimControllerBase component = this.reactor.GetComponent<KAnimControllerBase>();
    component.AddAnimOverrides(Assets.GetAnim(HashedString.op_Implicit("anim_react_pip_kanim")));
    component.Play(HashedString.op_Implicit("hug_dupe_pre"));
    component.Queue(HashedString.op_Implicit("hug_dupe_loop"));
    component.Queue(HashedString.op_Implicit("hug_dupe_pst"));
    component.onAnimComplete += new KAnimControllerBase.KAnimEvent(this.Finish);
    this.gameObject.GetSMI<AnimInterruptMonitor.Instance>().PlayAnimSequence(new HashedString[3]
    {
      HashedString.op_Implicit("hug_dupe_pre"),
      HashedString.op_Implicit("hug_dupe_loop"),
      HashedString.op_Implicit("hug_dupe_pst")
    });
  }

  private void Finish(HashedString anim)
  {
    if (!HashedString.op_Equality(anim, HashedString.op_Implicit("hug_dupe_pst")))
      return;
    if (Object.op_Inequality((Object) this.reactor, (Object) null))
    {
      this.reactor.GetComponent<KAnimControllerBase>().onAnimComplete -= new KAnimControllerBase.KAnimEvent(this.Finish);
      this.ApplyEffects();
    }
    else
      DebugUtil.DevLogError("HugMinionReactable finishing without adding a Hugged effect.");
    this.End();
  }

  private void ApplyEffects()
  {
    this.reactor.GetComponent<Effects>().Add("Hugged", true);
    this.gameObject.GetSMI<HugMonitor.Instance>()?.EnterHuggingFrenzy();
  }

  protected override void InternalEnd()
  {
  }

  protected override void InternalCleanup()
  {
  }
}
