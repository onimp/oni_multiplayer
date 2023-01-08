// Decompiled with JetBrains decompiler
// Type: RecentThingConversation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class RecentThingConversation : ConversationType
{
  public static Dictionary<Conversation.ModeType, List<Conversation.ModeType>> transitions = new Dictionary<Conversation.ModeType, List<Conversation.ModeType>>()
  {
    {
      Conversation.ModeType.Query,
      new List<Conversation.ModeType>()
      {
        Conversation.ModeType.Agreement,
        Conversation.ModeType.Disagreement,
        Conversation.ModeType.Musing
      }
    },
    {
      Conversation.ModeType.Statement,
      new List<Conversation.ModeType>()
      {
        Conversation.ModeType.Agreement,
        Conversation.ModeType.Disagreement,
        Conversation.ModeType.Query,
        Conversation.ModeType.Segue
      }
    },
    {
      Conversation.ModeType.Agreement,
      new List<Conversation.ModeType>()
      {
        Conversation.ModeType.Satisfaction
      }
    },
    {
      Conversation.ModeType.Disagreement,
      new List<Conversation.ModeType>()
      {
        Conversation.ModeType.Dissatisfaction
      }
    },
    {
      Conversation.ModeType.Musing,
      new List<Conversation.ModeType>()
      {
        Conversation.ModeType.Query,
        Conversation.ModeType.Statement,
        Conversation.ModeType.Segue
      }
    },
    {
      Conversation.ModeType.Satisfaction,
      new List<Conversation.ModeType>()
      {
        Conversation.ModeType.Segue,
        Conversation.ModeType.End
      }
    },
    {
      Conversation.ModeType.Nominal,
      new List<Conversation.ModeType>()
      {
        Conversation.ModeType.Segue,
        Conversation.ModeType.End
      }
    },
    {
      Conversation.ModeType.Dissatisfaction,
      new List<Conversation.ModeType>()
      {
        Conversation.ModeType.Segue,
        Conversation.ModeType.End
      }
    }
  };

  public RecentThingConversation() => this.id = nameof (RecentThingConversation);

  public override void NewTarget(MinionIdentity speaker) => this.target = ((Component) speaker).GetSMI<ConversationMonitor.Instance>().GetATopic();

  public override Conversation.Topic GetNextTopic(
    MinionIdentity speaker,
    Conversation.Topic lastTopic)
  {
    if (string.IsNullOrEmpty(this.target))
      return (Conversation.Topic) null;
    List<Conversation.ModeType> modeTypeList;
    if (lastTopic == null)
      modeTypeList = new List<Conversation.ModeType>()
      {
        Conversation.ModeType.Query,
        Conversation.ModeType.Statement,
        Conversation.ModeType.Musing
      };
    else
      modeTypeList = RecentThingConversation.transitions[lastTopic.mode];
    return new Conversation.Topic(this.target, modeTypeList[Random.Range(0, modeTypeList.Count)]);
  }

  public override Sprite GetSprite(string topic) => Def.GetUISprite((object) topic, centered: true)?.first;
}
