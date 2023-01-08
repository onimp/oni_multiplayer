// Decompiled with JetBrains decompiler
// Type: ConversationManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ConversationManager")]
public class ConversationManager : KMonoBehaviour, ISim200ms
{
  private List<Conversation> activeSetups;
  private Dictionary<MinionIdentity, float> lastConvoTimeByMinion;
  private Dictionary<MinionIdentity, Conversation> setupsByMinion = new Dictionary<MinionIdentity, Conversation>();
  private List<System.Type> convoTypes = new List<System.Type>()
  {
    typeof (RecentThingConversation),
    typeof (AmountStateConversation),
    typeof (CurrentJobConversation)
  };
  private static readonly Tag[] invalidConvoTags = new Tag[3]
  {
    GameTags.Asleep,
    GameTags.HoldingBreath,
    GameTags.Dead
  };

  protected virtual void OnPrefabInit()
  {
    this.activeSetups = new List<Conversation>();
    this.lastConvoTimeByMinion = new Dictionary<MinionIdentity, float>();
    this.simRenderLoadBalance = true;
  }

  public void Sim200ms(float dt)
  {
    for (int index1 = this.activeSetups.Count - 1; index1 >= 0; --index1)
    {
      Conversation activeSetup = this.activeSetups[index1];
      for (int index2 = activeSetup.minions.Count - 1; index2 >= 0; --index2)
      {
        if (!this.ValidMinionTags(activeSetup.minions[index2]) || !this.MinionCloseEnoughToConvo(activeSetup.minions[index2], activeSetup))
          activeSetup.minions.RemoveAt(index2);
        else
          this.setupsByMinion[activeSetup.minions[index2]] = activeSetup;
      }
      if (activeSetup.minions.Count <= 1)
      {
        this.activeSetups.RemoveAt(index1);
      }
      else
      {
        bool flag1 = true;
        bool flag2 = Object.op_Equality((Object) activeSetup.minions.Find((Predicate<MinionIdentity>) (match => !((Component) match).HasTag(GameTags.Partying))), (Object) null);
        if (activeSetup.numUtterances == 0 && flag2 && (double) GameClock.Instance.GetTime() > (double) activeSetup.lastTalkedTime || (double) GameClock.Instance.GetTime() > (double) activeSetup.lastTalkedTime + (double) TuningData<ConversationManager.Tuning>.Get().delayBeforeStart)
        {
          MinionIdentity minion = activeSetup.minions[Random.Range(0, activeSetup.minions.Count)];
          activeSetup.conversationType.NewTarget(minion);
          flag1 = this.DoTalking(activeSetup, minion);
        }
        else if (activeSetup.numUtterances > 0 && activeSetup.numUtterances < TuningData<ConversationManager.Tuning>.Get().maxUtterances && (flag2 && (double) GameClock.Instance.GetTime() > (double) activeSetup.lastTalkedTime + (double) TuningData<ConversationManager.Tuning>.Get().speakTime / 4.0 || (double) GameClock.Instance.GetTime() > (double) activeSetup.lastTalkedTime + (double) TuningData<ConversationManager.Tuning>.Get().speakTime + (double) TuningData<ConversationManager.Tuning>.Get().delayBetweenUtterances))
        {
          int index3 = (activeSetup.minions.IndexOf(activeSetup.lastTalked) + Random.Range(1, activeSetup.minions.Count)) % activeSetup.minions.Count;
          MinionIdentity minion = activeSetup.minions[index3];
          flag1 = this.DoTalking(activeSetup, minion);
        }
        else if (activeSetup.numUtterances >= TuningData<ConversationManager.Tuning>.Get().maxUtterances)
          flag1 = false;
        if (!flag1)
          this.activeSetups.RemoveAt(index1);
      }
    }
    foreach (MinionIdentity minionIdentity1 in Components.LiveMinionIdentities.Items)
    {
      if (this.ValidMinionTags(minionIdentity1) && !this.setupsByMinion.ContainsKey(minionIdentity1) && !this.MinionOnCooldown(minionIdentity1))
      {
        foreach (MinionIdentity minionIdentity2 in Components.LiveMinionIdentities.Items)
        {
          if (!Object.op_Equality((Object) minionIdentity2, (Object) minionIdentity1) && this.ValidMinionTags(minionIdentity2))
          {
            if (this.setupsByMinion.ContainsKey(minionIdentity2))
            {
              Conversation setup = this.setupsByMinion[minionIdentity2];
              if (setup.minions.Count < TuningData<ConversationManager.Tuning>.Get().maxDupesPerConvo)
              {
                Vector3 vector3 = Vector3.op_Subtraction(this.GetCentroid(setup), TransformExtensions.GetPosition(minionIdentity1.transform));
                if ((double) ((Vector3) ref vector3).magnitude < (double) TuningData<ConversationManager.Tuning>.Get().maxDistance * 0.5)
                {
                  setup.minions.Add(minionIdentity1);
                  this.setupsByMinion[minionIdentity1] = setup;
                  break;
                }
              }
            }
            else if (!this.MinionOnCooldown(minionIdentity2))
            {
              Vector3 vector3 = Vector3.op_Subtraction(TransformExtensions.GetPosition(minionIdentity2.transform), TransformExtensions.GetPosition(minionIdentity1.transform));
              if ((double) ((Vector3) ref vector3).magnitude < (double) TuningData<ConversationManager.Tuning>.Get().maxDistance)
              {
                Conversation conversation = new Conversation();
                conversation.minions.Add(minionIdentity1);
                conversation.minions.Add(minionIdentity2);
                System.Type convoType = this.convoTypes[Random.Range(0, this.convoTypes.Count)];
                conversation.conversationType = (ConversationType) Activator.CreateInstance(convoType);
                conversation.lastTalkedTime = GameClock.Instance.GetTime();
                this.activeSetups.Add(conversation);
                this.setupsByMinion[minionIdentity1] = conversation;
                this.setupsByMinion[minionIdentity2] = conversation;
                break;
              }
            }
          }
        }
      }
    }
    this.setupsByMinion.Clear();
  }

  private bool DoTalking(Conversation setup, MinionIdentity new_speaker)
  {
    DebugUtil.Assert(setup != null, "setup was null");
    DebugUtil.Assert(Object.op_Inequality((Object) new_speaker, (Object) null), "new_speaker was null");
    if (Object.op_Inequality((Object) setup.lastTalked, (Object) null))
      setup.lastTalked.Trigger(25860745, (object) ((Component) setup.lastTalked).gameObject);
    DebugUtil.Assert(setup.conversationType != null, "setup.conversationType was null");
    Conversation.Topic nextTopic = setup.conversationType.GetNextTopic(new_speaker, setup.lastTopic);
    if (nextTopic == null || nextTopic.mode == Conversation.ModeType.End || nextTopic.mode == Conversation.ModeType.Segue)
      return false;
    Thought thoughtForTopic = this.GetThoughtForTopic(setup, nextTopic);
    if (thoughtForTopic == null)
      return false;
    ThoughtGraph.Instance smi = ((Component) new_speaker).GetSMI<ThoughtGraph.Instance>();
    if (smi == null)
      return false;
    smi.AddThought(thoughtForTopic);
    setup.lastTopic = nextTopic;
    setup.lastTalked = new_speaker;
    setup.lastTalkedTime = GameClock.Instance.GetTime();
    DebugUtil.Assert(this.lastConvoTimeByMinion != null, "lastConvoTimeByMinion was null");
    this.lastConvoTimeByMinion[setup.lastTalked] = GameClock.Instance.GetTime();
    Effects component = ((Component) setup.lastTalked).GetComponent<Effects>();
    DebugUtil.Assert(Object.op_Inequality((Object) component, (Object) null), "effects was null");
    component.Add("GoodConversation", true);
    Conversation.Mode mode = Conversation.Topic.Modes[(int) nextTopic.mode];
    DebugUtil.Assert(mode != null, "mode was null");
    ConversationManager.StartedTalkingEvent startedTalkingEvent = new ConversationManager.StartedTalkingEvent()
    {
      talker = ((Component) new_speaker).gameObject,
      anim = mode.anim
    };
    foreach (MinionIdentity minion in setup.minions)
    {
      if (!Object.op_Implicit((Object) minion))
        DebugUtil.DevAssert(false, "minion in setup.minions was null", (Object) null);
      else
        minion.Trigger(-594200555, (object) startedTalkingEvent);
    }
    ++setup.numUtterances;
    return true;
  }

  public bool TryGetConversation(MinionIdentity minion, out Conversation conversation) => this.setupsByMinion.TryGetValue(minion, out conversation);

  private Vector3 GetCentroid(Conversation setup)
  {
    Vector3 vector3 = Vector3.zero;
    foreach (MinionIdentity minion in setup.minions)
    {
      if (!Object.op_Equality((Object) minion, (Object) null))
        vector3 = Vector3.op_Addition(vector3, TransformExtensions.GetPosition(minion.transform));
    }
    return Vector3.op_Division(vector3, (float) setup.minions.Count);
  }

  private Thought GetThoughtForTopic(Conversation setup, Conversation.Topic topic)
  {
    if (string.IsNullOrEmpty(topic.topic))
    {
      DebugUtil.DevAssert(false, "topic.topic was null", (Object) null);
      return (Thought) null;
    }
    Sprite sprite = setup.conversationType.GetSprite(topic.topic);
    if (!Object.op_Inequality((Object) sprite, (Object) null))
      return (Thought) null;
    Conversation.Mode mode = Conversation.Topic.Modes[(int) topic.mode];
    return new Thought("Topic_" + topic.topic, (ResourceSet) null, sprite, mode.icon, mode.voice, "bubble_chatter", mode.mouth, DUPLICANTS.THOUGHTS.CONVERSATION.TOOLTIP, true, TuningData<ConversationManager.Tuning>.Get().speakTime);
  }

  private bool ValidMinionTags(MinionIdentity minion) => !Object.op_Equality((Object) minion, (Object) null) && !((Component) minion).GetComponent<KPrefabID>().HasAnyTags(ConversationManager.invalidConvoTags);

  private bool MinionCloseEnoughToConvo(MinionIdentity minion, Conversation setup)
  {
    Vector3 vector3 = Vector3.op_Subtraction(this.GetCentroid(setup), TransformExtensions.GetPosition(minion.transform));
    return (double) ((Vector3) ref vector3).magnitude < (double) TuningData<ConversationManager.Tuning>.Get().maxDistance * 0.5;
  }

  private bool MinionOnCooldown(MinionIdentity minion)
  {
    if (((Component) minion).GetComponent<KPrefabID>().HasTag(GameTags.AlwaysConverse))
      return false;
    return this.lastConvoTimeByMinion.ContainsKey(minion) && (double) GameClock.Instance.GetTime() < (double) this.lastConvoTimeByMinion[minion] + (double) TuningData<ConversationManager.Tuning>.Get().minionCooldownTime || (double) GameClock.Instance.GetTime() / 600.0 < (double) TuningData<ConversationManager.Tuning>.Get().cyclesBeforeFirstConversation;
  }

  public class Tuning : TuningData<ConversationManager.Tuning>
  {
    public float cyclesBeforeFirstConversation;
    public float maxDistance;
    public int maxDupesPerConvo;
    public float minionCooldownTime;
    public float speakTime;
    public float delayBetweenUtterances;
    public float delayBeforeStart;
    public int maxUtterances;
  }

  public class StartedTalkingEvent
  {
    public GameObject talker;
    public string anim;
  }
}
