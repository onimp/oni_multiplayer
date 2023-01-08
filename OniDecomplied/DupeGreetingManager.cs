// Decompiled with JetBrains decompiler
// Type: DupeGreetingManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/DupeGreetingManager")]
public class DupeGreetingManager : KMonoBehaviour, ISim200ms
{
  private const float COOLDOWN_TIME = 720f;
  private Dictionary<int, MinionIdentity> candidateCells;
  private List<DupeGreetingManager.GreetingSetup> activeSetups;
  private Dictionary<MinionIdentity, float> cooldowns;
  private static List<Emote> emotes;

  protected virtual void OnPrefabInit()
  {
    this.candidateCells = new Dictionary<int, MinionIdentity>();
    this.activeSetups = new List<DupeGreetingManager.GreetingSetup>();
    this.cooldowns = new Dictionary<MinionIdentity, float>();
  }

  public void Sim200ms(float dt)
  {
    if ((double) GameClock.Instance.GetTime() / 600.0 < (double) TuningData<DupeGreetingManager.Tuning>.Get().cyclesBeforeFirstGreeting)
      return;
    for (int index = this.activeSetups.Count - 1; index >= 0; --index)
    {
      DupeGreetingManager.GreetingSetup activeSetup = this.activeSetups[index];
      if (!this.ValidNavigatingMinion(activeSetup.A.minion) || !this.ValidOppositionalMinion(activeSetup.A.minion, activeSetup.B.minion))
      {
        activeSetup.A.reactable.Cleanup();
        activeSetup.B.reactable.Cleanup();
        this.activeSetups.RemoveAt(index);
      }
    }
    this.candidateCells.Clear();
    foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.Items)
    {
      if ((!this.cooldowns.ContainsKey(minionIdentity) || (double) GameClock.Instance.GetTime() - (double) this.cooldowns[minionIdentity] >= 720.0 * (double) TuningData<DupeGreetingManager.Tuning>.Get().greetingDelayMultiplier) && this.ValidNavigatingMinion(minionIdentity))
      {
        for (int offset = 0; offset <= 2; ++offset)
        {
          int offsetCell = this.GetOffsetCell(minionIdentity, offset);
          if (this.candidateCells.ContainsKey(offsetCell) && this.ValidOppositionalMinion(minionIdentity, this.candidateCells[offsetCell]))
          {
            this.BeginNewGreeting(minionIdentity, this.candidateCells[offsetCell], offsetCell);
            break;
          }
          this.candidateCells[offsetCell] = minionIdentity;
        }
      }
    }
  }

  private int GetOffsetCell(MinionIdentity minion, int offset) => !((Component) minion).GetComponent<Facing>().GetFacing() ? Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) minion), offset, 0) : Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) minion), -offset, 0);

  private bool ValidNavigatingMinion(MinionIdentity minion)
  {
    if (Object.op_Equality((Object) minion, (Object) null))
      return false;
    Navigator component = ((Component) minion).GetComponent<Navigator>();
    return Object.op_Inequality((Object) component, (Object) null) && component.IsMoving() && component.CurrentNavType == NavType.Floor;
  }

  private bool ValidOppositionalMinion(MinionIdentity reference_minion, MinionIdentity minion)
  {
    if (Object.op_Equality((Object) reference_minion, (Object) null) || Object.op_Equality((Object) minion, (Object) null))
      return false;
    Facing component1 = ((Component) minion).GetComponent<Facing>();
    Facing component2 = ((Component) reference_minion).GetComponent<Facing>();
    return this.ValidNavigatingMinion(minion) && Object.op_Inequality((Object) component1, (Object) null) && Object.op_Inequality((Object) component2, (Object) null) && component1.GetFacing() != component2.GetFacing();
  }

  private void BeginNewGreeting(MinionIdentity minion_a, MinionIdentity minion_b, int cell) => this.activeSetups.Add(new DupeGreetingManager.GreetingSetup()
  {
    cell = cell,
    A = new DupeGreetingManager.GreetingUnit(minion_a, this.GetReactable(minion_a)),
    B = new DupeGreetingManager.GreetingUnit(minion_b, this.GetReactable(minion_b))
  });

  private Reactable GetReactable(MinionIdentity minion)
  {
    if (DupeGreetingManager.emotes == null)
      DupeGreetingManager.emotes = new List<Emote>()
      {
        Db.Get().Emotes.Minion.Wave,
        Db.Get().Emotes.Minion.Wave_Shy,
        Db.Get().Emotes.Minion.FingerGuns
      };
    Emote emote = DupeGreetingManager.emotes[Random.Range(0, DupeGreetingManager.emotes.Count)];
    SelfEmoteReactable reactable = new SelfEmoteReactable(((Component) minion).gameObject, HashedString.op_Implicit("NavigatorPassingGreeting"), Db.Get().ChoreTypes.Emote, 1000f);
    reactable.SetEmote(emote).SetThought(Db.Get().Thoughts.Chatty);
    reactable.RegisterEmoteStepCallbacks(HashedString.op_Implicit("react"), new Action<GameObject>(this.BeginReacting), (Action<GameObject>) null);
    return (Reactable) reactable;
  }

  private void BeginReacting(GameObject minionGO)
  {
    if (Object.op_Equality((Object) minionGO, (Object) null))
      return;
    MinionIdentity component = minionGO.GetComponent<MinionIdentity>();
    Vector3 vector3 = Vector3.zero;
    foreach (DupeGreetingManager.GreetingSetup activeSetup in this.activeSetups)
    {
      if (Object.op_Equality((Object) activeSetup.A.minion, (Object) component))
      {
        if (Object.op_Inequality((Object) activeSetup.B.minion, (Object) null))
        {
          vector3 = TransformExtensions.GetPosition(activeSetup.B.minion.transform);
          activeSetup.A.minion.Trigger(-594200555, (object) activeSetup.B.minion);
          activeSetup.B.minion.Trigger(-594200555, (object) activeSetup.A.minion);
          break;
        }
        break;
      }
      if (Object.op_Equality((Object) activeSetup.B.minion, (Object) component))
      {
        if (Object.op_Inequality((Object) activeSetup.A.minion, (Object) null))
        {
          vector3 = TransformExtensions.GetPosition(activeSetup.A.minion.transform);
          break;
        }
        break;
      }
    }
    minionGO.GetComponent<Facing>().SetFacing((double) vector3.x < (double) TransformExtensions.GetPosition(minionGO.transform).x);
    minionGO.GetComponent<Effects>().Add("Greeting", true);
    this.cooldowns[component] = GameClock.Instance.GetTime();
  }

  public class Tuning : TuningData<DupeGreetingManager.Tuning>
  {
    public float cyclesBeforeFirstGreeting;
    public float greetingDelayMultiplier;
  }

  private class GreetingUnit
  {
    public MinionIdentity minion;
    public Reactable reactable;

    public GreetingUnit(MinionIdentity minion, Reactable reactable)
    {
      this.minion = minion;
      this.reactable = reactable;
    }
  }

  private class GreetingSetup
  {
    public int cell;
    public DupeGreetingManager.GreetingUnit A;
    public DupeGreetingManager.GreetingUnit B;
  }
}
