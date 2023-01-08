// Decompiled with JetBrains decompiler
// Type: ICheckboxListGroupControl
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

public interface ICheckboxListGroupControl
{
  string Title { get; }

  string Description { get; }

  ICheckboxListGroupControl.ListGroup[] GetData();

  bool SidescreenEnabled();

  int CheckboxSideScreenSortOrder();

  struct ListGroup
  {
    public Func<string, string> resolveTitleCallback;
    public string title;
    public ICheckboxListGroupControl.CheckboxItem[] checkboxItems;

    public ListGroup(
      string title,
      ICheckboxListGroupControl.CheckboxItem[] checkboxItems,
      Func<string, string> resolveTitleCallback = null)
    {
      this.title = title;
      this.checkboxItems = checkboxItems;
      this.resolveTitleCallback = resolveTitleCallback;
    }
  }

  struct CheckboxItem
  {
    public string text;
    public string tooltip;
    public bool isOn;
    public Func<string, object, string> resolveTooltipCallback;
  }
}
