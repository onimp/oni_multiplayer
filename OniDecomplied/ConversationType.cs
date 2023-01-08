// Decompiled with JetBrains decompiler
// Type: ConversationType
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class ConversationType
{
  public string id;
  public string target;

  public virtual void NewTarget(MinionIdentity speaker)
  {
  }

  public virtual Conversation.Topic GetNextTopic(
    MinionIdentity speaker,
    Conversation.Topic lastTopic)
  {
    return (Conversation.Topic) null;
  }

  public virtual Sprite GetSprite(string topic) => (Sprite) null;
}
