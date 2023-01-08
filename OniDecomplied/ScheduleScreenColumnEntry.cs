// Decompiled with JetBrains decompiler
// Type: ScheduleScreenColumnEntry
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScheduleScreenColumnEntry : 
  MonoBehaviour,
  IPointerEnterHandler,
  IEventSystemHandler,
  IPointerDownHandler
{
  public Image image;
  public System.Action onLeftClick;

  public void OnPointerEnter(PointerEventData event_data) => this.RunCallbacks();

  private void RunCallbacks()
  {
    if (!Input.GetMouseButton(0) || this.onLeftClick == null)
      return;
    this.onLeftClick();
  }

  public void OnPointerDown(PointerEventData event_data) => this.RunCallbacks();
}
