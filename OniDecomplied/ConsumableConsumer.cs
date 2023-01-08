// Decompiled with JetBrains decompiler
// Type: ConsumableConsumer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ConsumableConsumer")]
public class ConsumableConsumer : KMonoBehaviour
{
  [Obsolete("Deprecated, use forbiddenTagSet")]
  [Serialize]
  public Tag[] forbiddenTags;
  [Serialize]
  public HashSet<Tag> forbiddenTagSet;
  public System.Action consumableRulesChanged;

  [System.Runtime.Serialization.OnDeserialized]
  [Obsolete]
  private void OnDeserialized()
  {
    if (!SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 29))
      return;
    this.forbiddenTagSet = new HashSet<Tag>((IEnumerable<Tag>) this.forbiddenTags);
    this.forbiddenTags = (Tag[]) null;
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    if (Object.op_Inequality((Object) ConsumerManager.instance, (Object) null))
      this.forbiddenTagSet = new HashSet<Tag>((IEnumerable<Tag>) ConsumerManager.instance.DefaultForbiddenTagsList);
    else
      this.forbiddenTagSet = new HashSet<Tag>();
  }

  public bool IsPermitted(string consumable_id)
  {
    Tag tag;
    // ISSUE: explicit constructor call
    ((Tag) ref tag).\u002Ector(consumable_id);
    return !this.forbiddenTagSet.Contains(tag);
  }

  public void SetPermitted(string consumable_id, bool is_allowed)
  {
    Tag tag;
    // ISSUE: explicit constructor call
    ((Tag) ref tag).\u002Ector(consumable_id);
    if (is_allowed)
      this.forbiddenTagSet.Remove(tag);
    else
      this.forbiddenTagSet.Add(tag);
    Util.Signal(this.consumableRulesChanged);
  }
}
