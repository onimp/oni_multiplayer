// Decompiled with JetBrains decompiler
// Type: InputInit
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

internal class InputInit : MonoBehaviour
{
  private void Awake()
  {
    GameInputManager inputManager = Global.GetInputManager();
    for (int index = 0; index < ((KInputManager) inputManager).GetControllerCount(); ++index)
    {
      KInputController controller = ((KInputManager) inputManager).GetController(index);
      if (controller.IsGamepad)
      {
        foreach (Component component in ((Component) this).gameObject.GetComponents<Component>())
        {
          if (component is IInputHandler iinputHandler)
          {
            KInputHandler.Add((IInputHandler) controller, iinputHandler, 0);
            inputManager.usedMenus.Add(iinputHandler);
          }
        }
      }
    }
    if (KInputManager.currentController != null)
      KInputHandler.Add((IInputHandler) KInputManager.currentController, (IInputHandler) KScreenManager.Instance, 10);
    else
      KInputHandler.Add((IInputHandler) ((KInputManager) inputManager).GetDefaultController(), (IInputHandler) KScreenManager.Instance, 10);
    inputManager.usedMenus.Add((IInputHandler) KScreenManager.Instance);
    DebugHandler debugHandler = new DebugHandler();
    if (KInputManager.currentController != null)
      KInputHandler.Add((IInputHandler) KInputManager.currentController, (IInputHandler) debugHandler, -1);
    else
      KInputHandler.Add((IInputHandler) ((KInputManager) inputManager).GetDefaultController(), (IInputHandler) debugHandler, -1);
    inputManager.usedMenus.Add((IInputHandler) debugHandler);
  }
}
