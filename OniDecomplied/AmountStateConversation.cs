// Decompiled with JetBrains decompiler
// Type: AmountStateConversation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System.Collections.Generic;
using UnityEngine;

public class AmountStateConversation : ConversationType
{
  public static Dictionary<Conversation.ModeType, List<Conversation.ModeType>> transitions = new Dictionary<Conversation.ModeType, List<Conversation.ModeType>>()
  {
    {
      Conversation.ModeType.Query,
      new List<Conversation.ModeType>()
      {
        Conversation.ModeType.Statement
      }
    },
    {
      Conversation.ModeType.Satisfaction,
      new List<Conversation.ModeType>()
      {
        Conversation.ModeType.Agreement,
        Conversation.ModeType.Statement
      }
    },
    {
      Conversation.ModeType.Nominal,
      new List<Conversation.ModeType>()
      {
        Conversation.ModeType.Musing,
        Conversation.ModeType.Statement
      }
    },
    {
      Conversation.ModeType.Dissatisfaction,
      new List<Conversation.ModeType>()
      {
        Conversation.ModeType.Disagreement,
        Conversation.ModeType.Statement
      }
    },
    {
      Conversation.ModeType.Agreement,
      new List<Conversation.ModeType>()
      {
        Conversation.ModeType.Query,
        Conversation.ModeType.End
      }
    },
    {
      Conversation.ModeType.Disagreement,
      new List<Conversation.ModeType>()
      {
        Conversation.ModeType.Query,
        Conversation.ModeType.End
      }
    },
    {
      Conversation.ModeType.Musing,
      new List<Conversation.ModeType>()
      {
        Conversation.ModeType.Query,
        Conversation.ModeType.End
      }
    }
  };
  public static List<string> targets = new List<string>()
  {
    "Stress",
    "QualityOfLife",
    "HitPoints",
    "Calories",
    "Stamina",
    "ImmuneLevel"
  };

  public AmountStateConversation() => this.id = nameof (AmountStateConversation);

  public override void NewTarget(MinionIdentity speaker) => this.target = AmountStateConversation.targets[Random.Range(0, AmountStateConversation.targets.Count)];

  public override Conversation.Topic GetNextTopic(
    MinionIdentity speaker,
    Conversation.Topic lastTopic)
  {
    if (lastTopic == null)
      return new Conversation.Topic(this.target, Conversation.ModeType.Query);
    List<Conversation.ModeType> transition = AmountStateConversation.transitions[lastTopic.mode];
    Conversation.ModeType mode = transition[Random.Range(0, transition.Count)];
    return mode == Conversation.ModeType.Statement ? new Conversation.Topic(this.target, this.GetModeForAmount(speaker, this.target)) : new Conversation.Topic(this.target, mode);
  }

  public override Sprite GetSprite(string topic)
  {
    if (Db.Get().Amounts.Exists(topic))
      return Assets.GetSprite(HashedString.op_Implicit(Db.Get().Amounts.Get(topic).thoughtSprite));
    return Db.Get().Attributes.Exists(topic) ? Assets.GetSprite(HashedString.op_Implicit(Db.Get().Attributes.Get(topic).thoughtSprite)) : (Sprite) null;
  }

  private Conversation.ModeType GetModeForAmount(MinionIdentity speaker, string target)
  {
    if (target == Db.Get().Amounts.Stress.Id)
    {
      AmountInstance amountInstance = Db.Get().Amounts.Stress.Lookup((Component) speaker);
      float num = amountInstance.value / amountInstance.GetMax();
      if ((double) num < 0.10000000149011612)
        return Conversation.ModeType.Satisfaction;
      if ((double) num > 0.60000002384185791)
        return Conversation.ModeType.Dissatisfaction;
    }
    else if (target == Db.Get().Attributes.QualityOfLife.Id)
    {
      AttributeInstance attributeInstance1 = Db.Get().Attributes.QualityOfLife.Lookup((Component) speaker);
      AttributeInstance attributeInstance2 = Db.Get().Attributes.QualityOfLifeExpectation.Lookup((Component) speaker);
      float num = attributeInstance1.GetTotalValue() - attributeInstance2.GetTotalValue();
      if ((double) num > 0.0)
        return Conversation.ModeType.Satisfaction;
      if ((double) num < 0.0)
        return Conversation.ModeType.Dissatisfaction;
    }
    else if (target == Db.Get().Amounts.HitPoints.Id)
    {
      AmountInstance amountInstance = Db.Get().Amounts.HitPoints.Lookup((Component) speaker);
      float num = amountInstance.value / amountInstance.GetMax();
      if ((double) num >= 1.0)
        return Conversation.ModeType.Satisfaction;
      if ((double) num < 0.800000011920929)
        return Conversation.ModeType.Dissatisfaction;
    }
    else if (target == Db.Get().Amounts.Calories.Id)
    {
      AmountInstance amountInstance = Db.Get().Amounts.Calories.Lookup((Component) speaker);
      float num = amountInstance.value / amountInstance.GetMax();
      if ((double) num > 0.85000002384185791)
        return Conversation.ModeType.Satisfaction;
      if ((double) num < 0.5)
        return Conversation.ModeType.Dissatisfaction;
    }
    else if (target == Db.Get().Amounts.Stamina.Id)
    {
      AmountInstance amountInstance = Db.Get().Amounts.Stamina.Lookup((Component) speaker);
      float num = amountInstance.value / amountInstance.GetMax();
      if ((double) num > 0.5)
        return Conversation.ModeType.Satisfaction;
      if ((double) num < 0.20000000298023224)
        return Conversation.ModeType.Dissatisfaction;
    }
    else if (target == Db.Get().Amounts.ImmuneLevel.Id)
    {
      AmountInstance amountInstance = Db.Get().Amounts.ImmuneLevel.Lookup((Component) speaker);
      float num = amountInstance.value / amountInstance.GetMax();
      if ((double) num > 0.89999997615814209)
        return Conversation.ModeType.Satisfaction;
      if ((double) num < 0.5)
        return Conversation.ModeType.Dissatisfaction;
    }
    return Conversation.ModeType.Nominal;
  }
}
