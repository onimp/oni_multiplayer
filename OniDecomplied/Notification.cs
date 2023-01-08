// Decompiled with JetBrains decompiler
// Type: Notification
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Notification
{
  public object tooltipData;
  public bool expires = true;
  public bool playSound = true;
  public bool volume_attenuation = true;
  public Notification.ClickCallback customClickCallback;
  public bool clearOnClick;
  public bool showDismissButton;
  public object customClickData;
  private int notificationIncrement;
  private string notifierName;

  public NotificationType Type { get; set; }

  public Notifier Notifier { get; set; }

  public Transform clickFocus { get; set; }

  public float Time { get; set; }

  public float GameTime { get; set; }

  public float Delay { get; set; }

  public int Idx { get; set; }

  public Func<List<Notification>, object, string> ToolTip { get; set; }

  public bool IsReady() => (double) UnityEngine.Time.time >= (double) this.GameTime + (double) this.Delay;

  public string titleText { get; private set; }

  public string NotifierName
  {
    get => this.notifierName;
    set
    {
      this.notifierName = value;
      this.titleText = this.ReplaceTags(this.titleText);
    }
  }

  public Notification(
    string title,
    NotificationType type,
    Func<List<Notification>, object, string> tooltip = null,
    object tooltip_data = null,
    bool expires = true,
    float delay = 0.0f,
    Notification.ClickCallback custom_click_callback = null,
    object custom_click_data = null,
    Transform click_focus = null,
    bool volume_attenuation = true,
    bool clear_on_click = false,
    bool show_dismiss_button = false)
  {
    this.titleText = title;
    this.Type = type;
    this.ToolTip = tooltip;
    this.tooltipData = tooltip_data;
    this.expires = expires;
    this.Delay = delay;
    this.customClickCallback = custom_click_callback;
    this.customClickData = custom_click_data;
    this.clickFocus = click_focus;
    this.volume_attenuation = volume_attenuation;
    this.clearOnClick = clear_on_click;
    this.showDismissButton = show_dismiss_button;
    this.Idx = this.notificationIncrement++;
  }

  public void Clear()
  {
    if (!Object.op_Inequality((Object) this.Notifier, (Object) null))
      return;
    this.Notifier.Remove(this);
  }

  private string ReplaceTags(string text)
  {
    DebugUtil.Assert(text != null);
    int startIndex1 = text.IndexOf('{');
    int num = text.IndexOf('}');
    if (0 > startIndex1 || startIndex1 >= num)
      return text;
    StringBuilder stringBuilder = new StringBuilder();
    int startIndex2 = 0;
    int startIndex3;
    for (; 0 <= startIndex1; startIndex1 = text.IndexOf('{', startIndex3))
    {
      string str = text.Substring(startIndex2, startIndex1 - startIndex2);
      stringBuilder.Append(str);
      startIndex3 = text.IndexOf('}', startIndex1);
      if (startIndex1 < startIndex3)
      {
        string tagDescription = this.GetTagDescription(text.Substring(startIndex1 + 1, startIndex3 - startIndex1 - 1));
        stringBuilder.Append(tagDescription);
        startIndex2 = startIndex3 + 1;
      }
      else
        break;
    }
    stringBuilder.Append(text.Substring(startIndex2, text.Length - startIndex2));
    return stringBuilder.ToString();
  }

  private string GetTagDescription(string tag) => !(tag == "NotifierName") ? "UNKNOWN TAG: " + tag : this.notifierName;

  public delegate void ClickCallback(object data);
}
