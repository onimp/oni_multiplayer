// Decompiled with JetBrains decompiler
// Type: SelfEmoteReactable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class SelfEmoteReactable : EmoteReactable
{
  private EmoteChore chore;

  public SelfEmoteReactable(
    GameObject gameObject,
    HashedString id,
    ChoreType chore_type,
    float globalCooldown = 0.0f,
    float localCooldown = 20f,
    float lifeSpan = float.PositiveInfinity,
    float max_initial_delay = 0.0f)
    : base(gameObject, id, chore_type, 3, 3, globalCooldown, localCooldown, lifeSpan, max_initial_delay)
  {
  }

  public override bool InternalCanBegin(GameObject reactor, Navigator.ActiveTransition transition)
  {
    if (Object.op_Inequality((Object) reactor, (Object) this.gameObject))
      return false;
    Navigator component = reactor.GetComponent<Navigator>();
    return !Object.op_Equality((Object) component, (Object) null) && component.IsMoving();
  }

  public void PairEmote(EmoteChore emoteChore) => this.chore = emoteChore;

  protected override void InternalEnd()
  {
    if (this.chore != null && Object.op_Inequality((Object) this.chore.driver, (Object) null))
    {
      this.chore.PairReactable((SelfEmoteReactable) null);
      this.chore.Cancel("Reactable ended");
      this.chore = (EmoteChore) null;
    }
    base.InternalEnd();
  }
}
