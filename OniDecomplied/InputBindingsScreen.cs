// Decompiled with JetBrains decompiler
// Type: InputBindingsScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputBindingsScreen : KModalScreen
{
  private const string ROOT_KEY = "STRINGS.INPUT_BINDINGS.";
  [SerializeField]
  private OptionsMenuScreen optionsScreen;
  [SerializeField]
  private ConfirmDialogScreen confirmPrefab;
  public KButton backButton;
  public KButton resetButton;
  public KButton closeButton;
  public KButton prevScreenButton;
  public KButton nextScreenButton;
  private bool waitingForKeyPress;
  private Action actionToRebind = (Action) 275;
  private bool ignoreRootConflicts;
  private KButton activeButton;
  [SerializeField]
  private LocText screenTitle;
  [SerializeField]
  private GameObject parent;
  [SerializeField]
  private GameObject entryPrefab;
  private ConfirmDialogScreen confirmDialog;
  private int activeScreen = -1;
  private List<string> screens = new List<string>();
  private UIPool<HorizontalLayoutGroup> entryPool;
  private static readonly KeyCode[] validKeys;

  public override bool IsModal() => true;

  private bool IsKeyDown(KeyCode key_code) => Input.GetKey(key_code) || Input.GetKeyDown(key_code);

  private string GetModifierString(Modifier modifiers)
  {
    string modifierString = "";
    foreach (Modifier modifier in Enum.GetValues(typeof (Modifier)))
    {
      if ((modifiers & modifier) != null)
        modifierString = modifierString + " + " + modifier.ToString();
    }
    return modifierString;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.entryPrefab.SetActive(false);
    this.prevScreenButton.onClick += new System.Action(this.OnPrevScreen);
    this.nextScreenButton.onClick += new System.Action(this.OnNextScreen);
  }

  protected override void OnActivate()
  {
    this.CollectScreens();
    ((TMP_Text) this.screenTitle).text = StringEntry.op_Implicit(Strings.Get("STRINGS.INPUT_BINDINGS." + this.screens[this.activeScreen].ToUpper() + ".NAME"));
    this.closeButton.onClick += new System.Action(this.OnBack);
    this.backButton.onClick += new System.Action(this.OnBack);
    this.resetButton.onClick += new System.Action(this.OnReset);
    this.BuildDisplay();
  }

  private void CollectScreens()
  {
    this.screens.Clear();
    for (int index = 0; index < GameInputMapping.KeyBindings.Length; ++index)
    {
      BindingEntry keyBinding = GameInputMapping.KeyBindings[index];
      if (keyBinding.mGroup != null && keyBinding.mRebindable && !this.screens.Contains(keyBinding.mGroup) && DlcManager.IsDlcListValidForCurrentContent(keyBinding.dlcIds))
      {
        if (keyBinding.mGroup == "Root")
          this.activeScreen = this.screens.Count;
        this.screens.Add(keyBinding.mGroup);
      }
    }
  }

  protected override void OnDeactivate()
  {
    GameInputMapping.SaveBindings();
    this.DestroyDisplay();
  }

  private LocString GetActionString(Action action) => (LocString) null;

  private string GetBindingText(BindingEntry binding)
  {
    string keycodeLocalized = GameUtil.GetKeycodeLocalized(binding.mKeyCode);
    if (binding.mKeyCode != 308 && binding.mKeyCode != 307 && binding.mKeyCode != 306 && binding.mKeyCode != 305 && binding.mKeyCode != 304 && binding.mKeyCode != 303)
      keycodeLocalized += this.GetModifierString(binding.mModifier);
    return keycodeLocalized;
  }

  private void BuildDisplay()
  {
    ((TMP_Text) this.screenTitle).text = StringEntry.op_Implicit(Strings.Get("STRINGS.INPUT_BINDINGS." + this.screens[this.activeScreen].ToUpper() + ".NAME"));
    if (this.entryPool == null)
      this.entryPool = new UIPool<HorizontalLayoutGroup>(this.entryPrefab.GetComponent<HorizontalLayoutGroup>());
    this.DestroyDisplay();
    int num = 0;
    for (int index = 0; index < GameInputMapping.KeyBindings.Length; ++index)
    {
      BindingEntry binding = GameInputMapping.KeyBindings[index];
      if (binding.mGroup == this.screens[this.activeScreen] && binding.mRebindable && DlcManager.IsDlcListValidForCurrentContent(binding.dlcIds))
      {
        GameObject gameObject = ((Component) this.entryPool.GetFreeElement(this.parent, true)).gameObject;
        ((TMP_Text) ((Component) gameObject.transform.GetChild(0)).GetComponentInChildren<LocText>()).text = StringEntry.op_Implicit(Strings.Get("STRINGS.INPUT_BINDINGS." + binding.mGroup.ToUpper() + "." + binding.mAction.ToString().ToUpper()));
        LocText key_label = ((Component) gameObject.transform.GetChild(1)).GetComponentInChildren<LocText>();
        ((TMP_Text) key_label).text = this.GetBindingText(binding);
        KButton button = gameObject.GetComponentInChildren<KButton>();
        button.onClick += (System.Action) (() =>
        {
          this.waitingForKeyPress = true;
          this.actionToRebind = binding.mAction;
          this.ignoreRootConflicts = binding.mIgnoreRootConflics;
          this.activeButton = button;
          ((TMP_Text) key_label).text = (string) STRINGS.UI.FRONTEND.INPUT_BINDINGS_SCREEN.WAITING_FOR_INPUT;
        });
        gameObject.transform.SetSiblingIndex(num);
        ++num;
      }
    }
  }

  private void DestroyDisplay() => this.entryPool.ClearAll();

  private void Update()
  {
    if (!this.waitingForKeyPress)
      return;
    Modifier modifier = (Modifier) ((Modifier) ((Modifier) ((Modifier) ((Modifier) ((Modifier) 0 | (this.IsKeyDown((KeyCode) 308) || this.IsKeyDown((KeyCode) 307) ? 1 : 0)) | (this.IsKeyDown((KeyCode) 306) || this.IsKeyDown((KeyCode) 305) ? 2 : 0)) | (this.IsKeyDown((KeyCode) 304) || this.IsKeyDown((KeyCode) 303) ? 4 : 0)) | (this.IsKeyDown((KeyCode) 301) ? 8 : 0)) | (this.IsKeyDown((KeyCode) 96) ? 16 : 0));
    bool flag = false;
    for (int index = 0; index < InputBindingsScreen.validKeys.Length; ++index)
    {
      KeyCode validKey = (KeyCode) (int) InputBindingsScreen.validKeys[index];
      if (Input.GetKeyDown(validKey))
      {
        this.Bind((KKeyCode) validKey, modifier);
        flag = true;
      }
    }
    if (flag)
      return;
    float axis = Input.GetAxis("Mouse ScrollWheel");
    KKeyCode kkey_code = (KKeyCode) 0;
    if ((double) axis < 0.0)
      kkey_code = (KKeyCode) 1001;
    else if ((double) axis > 0.0)
      kkey_code = (KKeyCode) 1002;
    if (kkey_code == null)
      return;
    this.Bind(kkey_code, modifier);
  }

  private BindingEntry GetDuplicatedBinding(string activeScreen, BindingEntry new_binding)
  {
    BindingEntry duplicatedBinding = new BindingEntry();
    for (int index = 0; index < GameInputMapping.KeyBindings.Length; ++index)
    {
      BindingEntry keyBinding = GameInputMapping.KeyBindings[index];
      if (((BindingEntry) ref new_binding).IsBindingEqual(keyBinding) && (keyBinding.mGroup == null || keyBinding.mGroup == activeScreen || keyBinding.mGroup == "Root" || activeScreen == "Root") && (!(activeScreen == "Root") || !keyBinding.mIgnoreRootConflics) && (!(keyBinding.mGroup == "Root") || !new_binding.mIgnoreRootConflics))
      {
        duplicatedBinding = keyBinding;
        break;
      }
    }
    return duplicatedBinding;
  }

  public override void OnKeyDown(KButtonEvent e)
  {
    if (this.waitingForKeyPress)
      ((KInputEvent) e).Consumed = true;
    else if (e.TryConsume((Action) 1) || e.TryConsume((Action) 5))
      this.Deactivate();
    else
      base.OnKeyDown(e);
  }

  public override void OnKeyUp(KButtonEvent e) => ((KInputEvent) e).Consumed = true;

  private void OnBack()
  {
    string text;
    switch (this.NumUnboundActions())
    {
      case 0:
        this.Deactivate();
        return;
      case 1:
        BindingEntry firstUnbound = this.GetFirstUnbound();
        text = string.Format((string) STRINGS.UI.FRONTEND.INPUT_BINDINGS_SCREEN.UNBOUND_ACTION, (object) firstUnbound.mAction.ToString());
        break;
      default:
        text = (string) STRINGS.UI.FRONTEND.INPUT_BINDINGS_SCREEN.MULTIPLE_UNBOUND_ACTIONS;
        break;
    }
    this.confirmDialog = Util.KInstantiateUI(((Component) this.confirmPrefab).gameObject, ((Component) ((KMonoBehaviour) this).transform).gameObject, false).GetComponent<ConfirmDialogScreen>();
    this.confirmDialog.PopupConfirmDialog(text, (System.Action) (() => this.Deactivate()), (System.Action) (() => this.confirmDialog.Deactivate()));
    ((Component) this.confirmDialog).gameObject.SetActive(true);
  }

  private int NumUnboundActions()
  {
    int num = 0;
    for (int index = 0; index < GameInputMapping.KeyBindings.Length; ++index)
    {
      BindingEntry keyBinding = GameInputMapping.KeyBindings[index];
      if (keyBinding.mKeyCode == null && (BuildMenu.UseHotkeyBuildMenu() || !keyBinding.mIgnoreRootConflics))
        ++num;
    }
    return num;
  }

  private BindingEntry GetFirstUnbound()
  {
    BindingEntry firstUnbound = new BindingEntry();
    for (int index = 0; index < GameInputMapping.KeyBindings.Length; ++index)
    {
      BindingEntry keyBinding = GameInputMapping.KeyBindings[index];
      if (keyBinding.mKeyCode == null)
      {
        firstUnbound = keyBinding;
        break;
      }
    }
    return firstUnbound;
  }

  private void OnReset()
  {
    GameInputMapping.KeyBindings = (BindingEntry[]) GameInputMapping.DefaultBindings.Clone();
    Global.GetInputManager().RebindControls();
    this.BuildDisplay();
  }

  public void OnPrevScreen()
  {
    if (this.activeScreen > 0)
      --this.activeScreen;
    else
      this.activeScreen = this.screens.Count - 1;
    this.BuildDisplay();
  }

  public void OnNextScreen()
  {
    if (this.activeScreen < this.screens.Count - 1)
      ++this.activeScreen;
    else
      this.activeScreen = 0;
    this.BuildDisplay();
  }

  private void Bind(KKeyCode kkey_code, Modifier modifier)
  {
    BindingEntry bindingEntry;
    // ISSUE: explicit constructor call
    ((BindingEntry) ref bindingEntry).\u002Ector(this.screens[this.activeScreen], (GamepadButton) 16, kkey_code, modifier, this.actionToRebind, true, this.ignoreRootConflicts);
    for (int index = 0; index < GameInputMapping.KeyBindings.Length; ++index)
    {
      BindingEntry keyBinding = GameInputMapping.KeyBindings[index];
      if (keyBinding.mRebindable && keyBinding.mAction == this.actionToRebind)
      {
        BindingEntry duplicatedBinding = this.GetDuplicatedBinding(this.screens[this.activeScreen], bindingEntry);
        bindingEntry.mButton = GameInputMapping.KeyBindings[index].mButton;
        GameInputMapping.KeyBindings[index] = bindingEntry;
        ((TMP_Text) ((Component) this.activeButton).GetComponentInChildren<LocText>()).text = this.GetBindingText(bindingEntry);
        if (duplicatedBinding.mAction != null && duplicatedBinding.mAction != this.actionToRebind)
        {
          this.confirmDialog = Util.KInstantiateUI(((Component) this.confirmPrefab).gameObject, ((Component) ((KMonoBehaviour) this).transform).gameObject, false).GetComponent<ConfirmDialogScreen>();
          string str = StringEntry.op_Implicit(Strings.Get("STRINGS.INPUT_BINDINGS." + duplicatedBinding.mGroup.ToUpper() + "." + duplicatedBinding.mAction.ToString().ToUpper()));
          string bindingText = this.GetBindingText(duplicatedBinding);
          string text = string.Format((string) STRINGS.UI.FRONTEND.INPUT_BINDINGS_SCREEN.DUPLICATE, (object) str, (object) bindingText);
          this.Unbind(duplicatedBinding.mAction);
          this.confirmDialog.PopupConfirmDialog(text, (System.Action) null, (System.Action) null);
          ((Component) this.confirmDialog).gameObject.SetActive(true);
        }
        Global.GetInputManager().RebindControls();
        this.waitingForKeyPress = false;
        this.actionToRebind = (Action) 275;
        this.activeButton = (KButton) null;
        this.BuildDisplay();
        break;
      }
    }
  }

  private void Unbind(Action action)
  {
    for (int index = 0; index < GameInputMapping.KeyBindings.Length; ++index)
    {
      BindingEntry keyBinding = GameInputMapping.KeyBindings[index];
      if (keyBinding.mAction == action)
      {
        keyBinding.mKeyCode = (KKeyCode) 0;
        keyBinding.mModifier = (Modifier) 0;
        GameInputMapping.KeyBindings[index] = keyBinding;
      }
    }
  }

  static InputBindingsScreen()
  {
    // ISSUE: unable to decompile the method.
  }
}
