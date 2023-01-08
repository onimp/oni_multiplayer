// Decompiled with JetBrains decompiler
// Type: SkillPerkMissingComplainer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SkillPerkMissingComplainer")]
public class SkillPerkMissingComplainer : KMonoBehaviour
{
  public string requiredSkillPerk;
  private int skillUpdateHandle = -1;
  private Guid workStatusItemHandle;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (!string.IsNullOrEmpty(this.requiredSkillPerk))
      this.skillUpdateHandle = Game.Instance.Subscribe(-1523247426, new Action<object>(this.UpdateStatusItem));
    this.UpdateStatusItem();
  }

  protected virtual void OnCleanUp()
  {
    if (this.skillUpdateHandle != -1)
      Game.Instance.Unsubscribe(this.skillUpdateHandle);
    base.OnCleanUp();
  }

  protected virtual void UpdateStatusItem(object data = null)
  {
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    if (Object.op_Equality((Object) component, (Object) null) || string.IsNullOrEmpty(this.requiredSkillPerk))
      return;
    bool flag = MinionResume.AnyMinionHasPerk(this.requiredSkillPerk, this.GetMyWorldId());
    if (!flag && this.workStatusItemHandle == Guid.Empty)
    {
      this.workStatusItemHandle = component.AddStatusItem(Db.Get().BuildingStatusItems.ColonyLacksRequiredSkillPerk, (object) this.requiredSkillPerk);
    }
    else
    {
      if (!flag || !(this.workStatusItemHandle != Guid.Empty))
        return;
      component.RemoveStatusItem(this.workStatusItemHandle);
      this.workStatusItemHandle = Guid.Empty;
    }
  }
}
