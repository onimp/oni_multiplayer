// Decompiled with JetBrains decompiler
// Type: ControlsScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine.UI;

public class ControlsScreen : KScreen
{
  public Text controlLabel;

  protected virtual void OnPrefabInit()
  {
    BindingEntry[] bindingEntries = GameInputMapping.GetBindingEntries();
    string str = "";
    foreach (BindingEntry bindingEntry in bindingEntries)
      str = str + bindingEntry.mAction.ToString() + ": " + bindingEntry.mKeyCode.ToString() + "\n";
    this.controlLabel.text = str;
  }

  public virtual void OnKeyDown(KButtonEvent e)
  {
    if (!e.TryConsume((Action) 2) && !e.TryConsume((Action) 1))
      return;
    this.Deactivate();
  }
}
