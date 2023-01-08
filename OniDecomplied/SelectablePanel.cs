// Decompiled with JetBrains decompiler
// Type: SelectablePanel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.EventSystems;

public class SelectablePanel : MonoBehaviour, IDeselectHandler, IEventSystemHandler
{
  public void OnDeselect(BaseEventData evt) => ((Component) this).gameObject.SetActive(false);
}
