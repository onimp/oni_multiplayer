// Decompiled with JetBrains decompiler
// Type: InputModuleSwitch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.EventSystems;

public class InputModuleSwitch : MonoBehaviour
{
  public VirtualInputModule virtualInput;
  public StandaloneInputModule standaloneInput;
  private Vector3 lastMousePosition;

  private void Update()
  {
    if (Vector3.op_Inequality(this.lastMousePosition, Input.mousePosition) && KInputManager.currentControllerIsGamepad)
    {
      KInputManager.currentControllerIsGamepad = false;
      KInputManager.InputChange.Invoke();
    }
    if (KInputManager.currentControllerIsGamepad)
    {
      ((Behaviour) this.virtualInput).enabled = KInputManager.currentControllerIsGamepad;
      if (!((Behaviour) this.standaloneInput).enabled)
        return;
      ((Behaviour) this.standaloneInput).enabled = false;
      this.virtualInput.forceModuleActive = true;
      this.ChangeInputHandler();
    }
    else
    {
      this.lastMousePosition = Input.mousePosition;
      ((Behaviour) this.standaloneInput).enabled = true;
      if (!((Behaviour) this.virtualInput).enabled)
        return;
      ((Behaviour) this.virtualInput).enabled = false;
      this.standaloneInput.forceModuleActive = true;
      this.ChangeInputHandler();
    }
  }

  private void ChangeInputHandler()
  {
    GameInputManager inputManager = Global.GetInputManager();
    for (int index = 0; index < inputManager.usedMenus.Count; ++index)
    {
      if (((object) inputManager.usedMenus[index]).Equals((object) null))
        inputManager.usedMenus.RemoveAt(index);
    }
    if (((KInputManager) inputManager).GetControllerCount() <= 1)
      return;
    if (KInputManager.currentControllerIsGamepad)
    {
      Cursor.visible = false;
      ((KInputManager) inputManager).GetController(1).inputHandler.TransferHandles(((KInputManager) inputManager).GetController(0).inputHandler);
    }
    else
    {
      Cursor.visible = true;
      ((KInputManager) inputManager).GetController(0).inputHandler.TransferHandles(((KInputManager) inputManager).GetController(1).inputHandler);
    }
  }
}
