// Decompiled with JetBrains decompiler
// Type: Automatable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/Automatable")]
public class Automatable : KMonoBehaviour
{
  [Serialize]
  private bool automationOnly = true;
  [MyCmpAdd]
  private CopyBuildingSettings copyBuildingSettings;
  private static readonly EventSystem.IntraObjectHandler<Automatable> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<Automatable>((Action<Automatable, object>) ((component, data) => component.OnCopySettings(data)));

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<Automatable>(-905833192, Automatable.OnCopySettingsDelegate);
  }

  private void OnCopySettings(object data)
  {
    Automatable component = ((GameObject) data).GetComponent<Automatable>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    this.automationOnly = component.automationOnly;
  }

  public bool GetAutomationOnly() => this.automationOnly;

  public void SetAutomationOnly(bool only) => this.automationOnly = only;

  public bool AllowedByAutomation(bool is_transfer_arm) => !this.GetAutomationOnly() | is_transfer_arm;
}
