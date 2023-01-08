// Decompiled with JetBrains decompiler
// Type: PlayerControlledToggleSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class PlayerControlledToggleSideScreen : SideScreenContent, IRenderEveryTick
{
  public IPlayerControlledToggle target;
  public KButton toggleButton;
  protected static readonly HashedString[] ON_ANIMS = new HashedString[2]
  {
    HashedString.op_Implicit("on_pre"),
    HashedString.op_Implicit("on")
  };
  protected static readonly HashedString[] OFF_ANIMS = new HashedString[2]
  {
    HashedString.op_Implicit("off_pre"),
    HashedString.op_Implicit("off")
  };
  public float animScaleBase = 0.25f;
  private StatusItem togglePendingStatusItem;
  [SerializeField]
  private KBatchedAnimController kbac;
  private float lastKeyboardShortcutTime;
  private const float KEYBOARD_COOLDOWN = 0.1f;
  private bool keyDown;
  private bool currentState;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.toggleButton.onClick += new System.Action(this.ClickToggle);
    this.togglePendingStatusItem = new StatusItem(nameof (PlayerControlledToggleSideScreen), "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
  }

  public override bool IsValidForTarget(GameObject target) => target.GetComponent<IPlayerControlledToggle>() != null;

  public void RenderEveryTick(float dt)
  {
    if (!((Behaviour) this).isActiveAndEnabled)
      return;
    if (!this.keyDown && Input.GetKeyDown((KeyCode) 13) & (double) Time.unscaledTime - (double) this.lastKeyboardShortcutTime > 0.10000000149011612)
    {
      if (SpeedControlScreen.Instance.IsPaused)
        this.RequestToggle();
      else
        this.Toggle();
      this.lastKeyboardShortcutTime = Time.unscaledTime;
      this.keyDown = true;
    }
    if (!this.keyDown || !Input.GetKeyUp((KeyCode) 13))
      return;
    this.keyDown = false;
  }

  private void ClickToggle()
  {
    if (SpeedControlScreen.Instance.IsPaused)
      this.RequestToggle();
    else
      this.Toggle();
  }

  private void RequestToggle()
  {
    this.target.ToggleRequested = !this.target.ToggleRequested;
    if (this.target.ToggleRequested && SpeedControlScreen.Instance.IsPaused)
      this.target.GetSelectable().SetStatusItem(Db.Get().StatusItemCategories.Main, this.togglePendingStatusItem, (object) this);
    else
      this.target.GetSelectable().SetStatusItem(Db.Get().StatusItemCategories.Main, (StatusItem) null);
    this.UpdateVisuals(this.target.ToggleRequested ? !this.target.ToggledOn() : this.target.ToggledOn(), true);
  }

  public override void SetTarget(GameObject new_target)
  {
    if (Object.op_Equality((Object) new_target, (Object) null))
    {
      Debug.LogError((object) "Invalid gameObject received");
    }
    else
    {
      this.target = new_target.GetComponent<IPlayerControlledToggle>();
      if (this.target == null)
      {
        Debug.LogError((object) "The gameObject received is not an IPlayerControlledToggle");
      }
      else
      {
        this.UpdateVisuals(this.target.ToggleRequested ? !this.target.ToggledOn() : this.target.ToggledOn(), false);
        this.titleKey = this.target.SideScreenTitleKey;
      }
    }
  }

  private void Toggle()
  {
    this.target.ToggledByPlayer();
    this.UpdateVisuals(this.target.ToggledOn(), true);
    this.target.ToggleRequested = false;
    this.target.GetSelectable().RemoveStatusItem(this.togglePendingStatusItem);
  }

  private void UpdateVisuals(bool state, bool smooth)
  {
    if (state != this.currentState)
    {
      if (smooth)
        this.kbac.Play(state ? PlayerControlledToggleSideScreen.ON_ANIMS : PlayerControlledToggleSideScreen.OFF_ANIMS);
      else
        this.kbac.Play(state ? PlayerControlledToggleSideScreen.ON_ANIMS[1] : PlayerControlledToggleSideScreen.OFF_ANIMS[1]);
    }
    this.currentState = state;
  }
}
