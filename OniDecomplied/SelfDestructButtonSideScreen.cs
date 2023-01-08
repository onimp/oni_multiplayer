// Decompiled with JetBrains decompiler
// Type: SelfDestructButtonSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using TMPro;
using UnityEngine;

public class SelfDestructButtonSideScreen : SideScreenContent
{
  public KButton button;
  public LocText statusText;
  private CraftModuleInterface craftInterface;
  private bool acknowledgeWarnings;
  private static readonly EventSystem.IntraObjectHandler<SelfDestructButtonSideScreen> TagsChangedDelegate = new EventSystem.IntraObjectHandler<SelfDestructButtonSideScreen>((Action<SelfDestructButtonSideScreen, object>) ((cmp, data) => cmp.OnTagsChanged(data)));

  protected virtual void OnSpawn()
  {
    this.Refresh();
    this.button.onClick += new System.Action(this.TriggerDestruct);
  }

  public override int GetSideScreenSortOrder() => -150;

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<CraftModuleInterface>(), (Object) null) && target.HasTag(GameTags.RocketInSpace);

  public override void SetTarget(GameObject target)
  {
    this.craftInterface = target.GetComponent<CraftModuleInterface>();
    this.acknowledgeWarnings = false;
    this.craftInterface.Subscribe<SelfDestructButtonSideScreen>(-1582839653, SelfDestructButtonSideScreen.TagsChangedDelegate);
    this.Refresh();
  }

  public override void ClearTarget()
  {
    if (!Object.op_Inequality((Object) this.craftInterface, (Object) null))
      return;
    this.craftInterface.Unsubscribe<SelfDestructButtonSideScreen>(-1582839653, SelfDestructButtonSideScreen.TagsChangedDelegate, false);
    this.craftInterface = (CraftModuleInterface) null;
  }

  private void OnTagsChanged(object data)
  {
    if (!Tag.op_Equality(((TagChangedEventData) data).tag, GameTags.RocketStranded))
      return;
    this.Refresh();
  }

  private void TriggerDestruct()
  {
    if (this.acknowledgeWarnings)
    {
      EventExtensions.Trigger(((Component) this.craftInterface).gameObject, -1061799784, (object) null);
      this.acknowledgeWarnings = false;
    }
    else
      this.acknowledgeWarnings = true;
    this.Refresh();
  }

  private void Refresh()
  {
    if (Object.op_Equality((Object) this.craftInterface, (Object) null))
      return;
    ((TMP_Text) this.statusText).text = (string) UI.UISIDESCREENS.SELFDESTRUCTSIDESCREEN.MESSAGE_TEXT;
    if (this.acknowledgeWarnings)
    {
      ((TMP_Text) ((Component) this.button).GetComponentInChildren<LocText>()).text = (string) UI.UISIDESCREENS.SELFDESTRUCTSIDESCREEN.BUTTON_TEXT_CONFIRM;
      ((Component) this.button).GetComponentInChildren<ToolTip>().toolTip = (string) UI.UISIDESCREENS.SELFDESTRUCTSIDESCREEN.BUTTON_TOOLTIP_CONFIRM;
    }
    else
    {
      ((TMP_Text) ((Component) this.button).GetComponentInChildren<LocText>()).text = (string) UI.UISIDESCREENS.SELFDESTRUCTSIDESCREEN.BUTTON_TEXT;
      ((Component) this.button).GetComponentInChildren<ToolTip>().toolTip = (string) UI.UISIDESCREENS.SELFDESTRUCTSIDESCREEN.BUTTON_TOOLTIP;
    }
  }
}
