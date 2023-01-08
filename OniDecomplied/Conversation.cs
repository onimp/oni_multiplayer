// Decompiled with JetBrains decompiler
// Type: Conversation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

public class Conversation
{
  public List<MinionIdentity> minions = new List<MinionIdentity>();
  public MinionIdentity lastTalked;
  public ConversationType conversationType;
  public float lastTalkedTime;
  public Conversation.Topic lastTopic;
  public int numUtterances;

  public enum ModeType
  {
    Query,
    Statement,
    Agreement,
    Disagreement,
    Musing,
    Satisfaction,
    Nominal,
    Dissatisfaction,
    Stressing,
    Segue,
    End,
  }

  public class Mode
  {
    public Conversation.ModeType type;
    public string voice;
    public string mouth;
    public string anim;
    public string icon;
    public bool newTopic;

    public Mode(
      Conversation.ModeType type,
      string voice,
      string icon,
      string mouth,
      string anim,
      bool newTopic = false)
    {
      this.type = type;
      this.voice = voice;
      this.mouth = mouth;
      this.anim = anim;
      this.icon = icon;
      this.newTopic = newTopic;
    }
  }

  public class Topic
  {
    public static List<Conversation.Mode> modeList = new List<Conversation.Mode>()
    {
      new Conversation.Mode(Conversation.ModeType.Query, "conversation_question", "mode_query", SpeechMonitor.PREFIX_HAPPY, "happy"),
      new Conversation.Mode(Conversation.ModeType.Statement, "conversation_answer", "mode_statement", SpeechMonitor.PREFIX_HAPPY, "happy"),
      new Conversation.Mode(Conversation.ModeType.Agreement, "conversation_answer", "mode_agreement", SpeechMonitor.PREFIX_HAPPY, "happy"),
      new Conversation.Mode(Conversation.ModeType.Disagreement, "conversation_answer", "mode_disagreement", SpeechMonitor.PREFIX_SAD, "unhappy"),
      new Conversation.Mode(Conversation.ModeType.Musing, "conversation_short", "mode_musing", SpeechMonitor.PREFIX_HAPPY, "happy"),
      new Conversation.Mode(Conversation.ModeType.Satisfaction, "conversation_short", "mode_satisfaction", SpeechMonitor.PREFIX_HAPPY, "happy"),
      new Conversation.Mode(Conversation.ModeType.Nominal, "conversation_short", "mode_nominal", SpeechMonitor.PREFIX_HAPPY, "happy"),
      new Conversation.Mode(Conversation.ModeType.Dissatisfaction, "conversation_short", "mode_dissatisfaction", SpeechMonitor.PREFIX_SAD, "unhappy"),
      new Conversation.Mode(Conversation.ModeType.Stressing, "conversation_short", "mode_stressing", SpeechMonitor.PREFIX_SAD, "unhappy"),
      new Conversation.Mode(Conversation.ModeType.Segue, "conversation_question", "mode_segue", SpeechMonitor.PREFIX_HAPPY, "happy", true)
    };
    private static Dictionary<int, Conversation.Mode> _modes;
    public string topic;
    public Conversation.ModeType mode;

    public static Dictionary<int, Conversation.Mode> Modes
    {
      get
      {
        if (Conversation.Topic._modes == null)
        {
          Conversation.Topic._modes = new Dictionary<int, Conversation.Mode>();
          foreach (Conversation.Mode mode in Conversation.Topic.modeList)
            Conversation.Topic._modes[(int) mode.type] = mode;
        }
        return Conversation.Topic._modes;
      }
    }

    public Topic(string topic, Conversation.ModeType mode)
    {
      this.topic = topic;
      this.mode = mode;
    }
  }
}
