// Decompiled with JetBrains decompiler
// Type: UserNameable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/UserNameable")]
public class UserNameable : KMonoBehaviour
{
  [Serialize]
  public string savedName = "";

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (string.IsNullOrEmpty(this.savedName))
      this.SetName(((Component) this).gameObject.GetProperName());
    else
      this.SetName(this.savedName);
  }

  public void SetName(string name)
  {
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    ((Object) this).name = name;
    if (Object.op_Inequality((Object) component, (Object) null))
      component.SetName(name);
    ((Object) ((Component) this).gameObject).name = name;
    NameDisplayScreen.Instance.UpdateName(((Component) this).gameObject);
    if (Object.op_Inequality((Object) ((Component) this).GetComponent<CommandModule>(), (Object) null))
      SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(((Component) this).GetComponent<LaunchConditionManager>()).SetRocketName(name);
    else if (Object.op_Inequality((Object) ((Component) this).GetComponent<Clustercraft>(), (Object) null))
      ClusterNameDisplayScreen.Instance.UpdateName((ClusterGridEntity) ((Component) this).GetComponent<Clustercraft>());
    this.savedName = name;
    this.Trigger(1102426921, (object) name);
  }
}
