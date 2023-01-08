// Decompiled with JetBrains decompiler
// Type: BreakdownList
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/BreakdownList")]
public class BreakdownList : KMonoBehaviour
{
  public Image headerIcon;
  public Sprite headerIconSprite;
  public Image headerBar;
  public LocText headerTitle;
  public LocText headerValue;
  public LocText infoTextLabel;
  public BreakdownListRow listRowTemplate;
  private List<BreakdownListRow> listRows = new List<BreakdownListRow>();
  private List<BreakdownListRow> unusedListRows = new List<BreakdownListRow>();
  private List<GameObject> customRows = new List<GameObject>();

  public BreakdownListRow AddRow()
  {
    BreakdownListRow breakdownListRow;
    if (this.unusedListRows.Count > 0)
    {
      breakdownListRow = this.unusedListRows[0];
      this.unusedListRows.RemoveAt(0);
    }
    else
      breakdownListRow = Object.Instantiate<BreakdownListRow>(this.listRowTemplate);
    ((Component) breakdownListRow).gameObject.transform.SetParent(this.transform);
    ((Component) breakdownListRow).gameObject.transform.SetAsLastSibling();
    this.listRows.Add(breakdownListRow);
    ((Component) breakdownListRow).gameObject.SetActive(true);
    return breakdownListRow;
  }

  public GameObject AddCustomRow(GameObject newRow)
  {
    newRow.transform.SetParent(this.transform);
    newRow.gameObject.transform.SetAsLastSibling();
    this.customRows.Add(newRow);
    newRow.SetActive(true);
    return newRow;
  }

  public void ClearRows()
  {
    foreach (BreakdownListRow listRow in this.listRows)
    {
      this.unusedListRows.Add(listRow);
      ((Component) listRow).gameObject.SetActive(false);
      listRow.ClearTooltip();
    }
    this.listRows.Clear();
    foreach (GameObject customRow in this.customRows)
      customRow.SetActive(false);
  }

  public void SetTitle(string title) => ((TMP_Text) this.headerTitle).text = title;

  public void SetDescription(string description)
  {
    if (description != null && description.Length >= 0)
    {
      ((Component) this.infoTextLabel).gameObject.SetActive(true);
      ((TMP_Text) this.infoTextLabel).text = description;
    }
    else
      ((Component) this.infoTextLabel).gameObject.SetActive(false);
  }

  public void SetIcon(Sprite icon) => this.headerIcon.sprite = icon;
}
