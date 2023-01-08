// Decompiled with JetBrains decompiler
// Type: ReportScreenEntry
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ReportScreenEntry")]
public class ReportScreenEntry : KMonoBehaviour
{
  [SerializeField]
  private ReportScreenEntryRow rowTemplate;
  private ReportScreenEntryRow mainRow;
  private List<ReportScreenEntryRow> contextRows = new List<ReportScreenEntryRow>();
  private int currentContextCount;

  public void SetMainEntry(ReportManager.ReportEntry entry, ReportManager.ReportGroup reportGroup)
  {
    if (Object.op_Equality((Object) this.mainRow, (Object) null))
    {
      this.mainRow = Util.KInstantiateUI(((Component) this.rowTemplate).gameObject, ((Component) this).gameObject, true).GetComponent<ReportScreenEntryRow>();
      this.mainRow.toggle.onClick += new System.Action(this.ToggleContext);
      ((Component) this.mainRow.name).GetComponentInChildren<MultiToggle>().onClick += new System.Action(this.ToggleContext);
      ((Component) this.mainRow.added).GetComponentInChildren<MultiToggle>().onClick += new System.Action(this.ToggleContext);
      ((Component) this.mainRow.removed).GetComponentInChildren<MultiToggle>().onClick += new System.Action(this.ToggleContext);
      ((Component) this.mainRow.net).GetComponentInChildren<MultiToggle>().onClick += new System.Action(this.ToggleContext);
    }
    this.mainRow.SetLine(entry, reportGroup);
    this.currentContextCount = entry.contextEntries.Count;
    for (int index = 0; index < entry.contextEntries.Count; ++index)
    {
      if (index >= this.contextRows.Count)
        this.contextRows.Add(Util.KInstantiateUI(((Component) this.rowTemplate).gameObject, ((Component) this).gameObject, false).GetComponent<ReportScreenEntryRow>());
      this.contextRows[index].SetLine(entry.contextEntries[index], reportGroup);
    }
    this.UpdateVisibility();
  }

  private void ToggleContext()
  {
    this.mainRow.toggle.NextState();
    this.UpdateVisibility();
  }

  private void UpdateVisibility()
  {
    int index;
    for (index = 0; index < this.currentContextCount; ++index)
      ((Component) this.contextRows[index]).gameObject.SetActive(this.mainRow.toggle.CurrentState == 1);
    for (; index < this.contextRows.Count; ++index)
      ((Component) this.contextRows[index]).gameObject.SetActive(false);
  }
}
