// Decompiled with JetBrains decompiler
// Type: ChoreGroupManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/ChoreGroupManager")]
public class ChoreGroupManager : KMonoBehaviour, ISaveLoadable
{
  public static ChoreGroupManager instance;
  [Serialize]
  private List<Tag> defaultForbiddenTagsList = new List<Tag>();
  [Serialize]
  private Dictionary<Tag, int> defaultChorePermissions = new Dictionary<Tag, int>();

  public static void DestroyInstance() => ChoreGroupManager.instance = (ChoreGroupManager) null;

  public List<Tag> DefaultForbiddenTagsList => this.defaultForbiddenTagsList;

  public Dictionary<Tag, int> DefaultChorePermission => this.defaultChorePermissions;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    ChoreGroupManager.instance = this;
    this.ConvertOldVersion();
    foreach (ChoreGroup resource in Db.Get().ChoreGroups.resources)
    {
      if (!this.defaultChorePermissions.ContainsKey(TagExtensions.ToTag(resource.Id)))
        this.defaultChorePermissions.Add(TagExtensions.ToTag(resource.Id), 2);
    }
  }

  private void ConvertOldVersion()
  {
    foreach (Tag defaultForbiddenTags in this.defaultForbiddenTagsList)
    {
      if (!this.defaultChorePermissions.ContainsKey(defaultForbiddenTags))
        this.defaultChorePermissions.Add(defaultForbiddenTags, -1);
      this.defaultChorePermissions[defaultForbiddenTags] = 0;
    }
    this.defaultForbiddenTagsList.Clear();
  }
}
