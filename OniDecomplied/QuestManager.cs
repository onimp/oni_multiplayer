// Decompiled with JetBrains decompiler
// Type: QuestManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class QuestManager : KMonoBehaviour
{
  private static QuestManager instance;
  [Serialize]
  private Dictionary<int, Dictionary<HashedString, QuestInstance>> ownerToQuests = new Dictionary<int, Dictionary<HashedString, QuestInstance>>();

  protected virtual void OnPrefabInit()
  {
    if (Object.op_Inequality((Object) QuestManager.instance, (Object) null))
    {
      Object.Destroy((Object) QuestManager.instance);
    }
    else
    {
      QuestManager.instance = this;
      base.OnPrefabInit();
    }
  }

  public static QuestInstance InitializeQuest(Tag ownerId, Quest quest)
  {
    QuestInstance qInst;
    if (!QuestManager.TryGetQuest(((Tag) ref ownerId).GetHash(), quest, out qInst))
      qInst = QuestManager.instance.ownerToQuests[((Tag) ref ownerId).GetHash()][quest.IdHash] = new QuestInstance(quest);
    qInst.Initialize(quest);
    return qInst;
  }

  public static QuestInstance InitializeQuest(HashedString ownerId, Quest quest)
  {
    QuestInstance qInst;
    if (!QuestManager.TryGetQuest(((HashedString) ref ownerId).HashValue, quest, out qInst))
      qInst = QuestManager.instance.ownerToQuests[((HashedString) ref ownerId).HashValue][quest.IdHash] = new QuestInstance(quest);
    qInst.Initialize(quest);
    return qInst;
  }

  public static QuestInstance GetInstance(Tag ownerId, Quest quest)
  {
    QuestInstance qInst;
    QuestManager.TryGetQuest(((Tag) ref ownerId).GetHash(), quest, out qInst);
    return qInst;
  }

  public static QuestInstance GetInstance(HashedString ownerId, Quest quest)
  {
    QuestInstance qInst;
    QuestManager.TryGetQuest(((HashedString) ref ownerId).HashValue, quest, out qInst);
    return qInst;
  }

  public static bool CheckState(HashedString ownerId, Quest quest, Quest.State state)
  {
    QuestInstance qInst;
    QuestManager.TryGetQuest(((HashedString) ref ownerId).HashValue, quest, out qInst);
    return qInst != null && qInst.CurrentState == state;
  }

  public static bool CheckState(Tag ownerId, Quest quest, Quest.State state)
  {
    QuestInstance qInst;
    QuestManager.TryGetQuest(((Tag) ref ownerId).GetHash(), quest, out qInst);
    return qInst != null && qInst.CurrentState == state;
  }

  private static bool TryGetQuest(int ownerId, Quest quest, out QuestInstance qInst)
  {
    qInst = (QuestInstance) null;
    Dictionary<HashedString, QuestInstance> dictionary;
    if (!QuestManager.instance.ownerToQuests.TryGetValue(ownerId, out dictionary))
      dictionary = QuestManager.instance.ownerToQuests[ownerId] = new Dictionary<HashedString, QuestInstance>();
    return dictionary.TryGetValue(quest.IdHash, out qInst);
  }
}
