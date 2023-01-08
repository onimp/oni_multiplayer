// Decompiled with JetBrains decompiler
// Type: Chatty
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class Chatty : KMonoBehaviour, ISimEveryTick
{
  private MinionIdentity identity;
  private List<MinionIdentity> conversationPartners = new List<MinionIdentity>();

  protected virtual void OnPrefabInit()
  {
    ((Component) this).GetComponent<KPrefabID>().AddTag(GameTags.AlwaysConverse, false);
    this.Subscribe(-594200555, new Action<object>(this.OnStartedTalking));
    this.identity = ((Component) this).GetComponent<MinionIdentity>();
  }

  private void OnStartedTalking(object data)
  {
    MinionIdentity minionIdentity = data as MinionIdentity;
    if (Object.op_Equality((Object) minionIdentity, (Object) null))
      return;
    this.conversationPartners.Add(minionIdentity);
  }

  public void SimEveryTick(float dt)
  {
    if (this.conversationPartners.Count == 0)
      return;
    for (int index = this.conversationPartners.Count - 1; index >= 0; --index)
    {
      MinionIdentity conversationPartner = this.conversationPartners[index];
      this.conversationPartners.RemoveAt(index);
      if (!Object.op_Equality((Object) conversationPartner, (Object) this.identity))
        ((Component) conversationPartner).AddTag(GameTags.PleasantConversation);
    }
    ((Component) this).gameObject.AddTag(GameTags.PleasantConversation);
  }
}
