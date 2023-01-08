// Decompiled with JetBrains decompiler
// Type: ReportScreenEntryRow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/ReportScreenEntryRow")]
public class ReportScreenEntryRow : KMonoBehaviour
{
  [SerializeField]
  public LocText name;
  [SerializeField]
  public LocText added;
  [SerializeField]
  public LocText removed;
  [SerializeField]
  public LocText net;
  private float addedValue = float.NegativeInfinity;
  private float removedValue = float.NegativeInfinity;
  private float netValue = float.NegativeInfinity;
  [SerializeField]
  public MultiToggle toggle;
  [SerializeField]
  private LayoutElement spacer;
  [SerializeField]
  private Image bgImage;
  public float groupSpacerWidth;
  public float contextSpacerWidth;
  private float nameWidth = 164f;
  private float indentWidth = 6f;
  [SerializeField]
  private Color oddRowColor;
  private static List<ReportManager.ReportEntry.Note> notes = new List<ReportManager.ReportEntry.Note>();
  private ReportManager.ReportEntry entry;
  private ReportManager.ReportGroup reportGroup;

  private List<ReportManager.ReportEntry.Note> Sort(
    List<ReportManager.ReportEntry.Note> notes,
    ReportManager.ReportEntry.Order order)
  {
    switch (order)
    {
      case ReportManager.ReportEntry.Order.Ascending:
        notes.Sort((Comparison<ReportManager.ReportEntry.Note>) ((x, y) => x.value.CompareTo(y.value)));
        break;
      case ReportManager.ReportEntry.Order.Descending:
        notes.Sort((Comparison<ReportManager.ReportEntry.Note>) ((x, y) => y.value.CompareTo(x.value)));
        break;
    }
    return notes;
  }

  public static void DestroyStatics() => ReportScreenEntryRow.notes = (List<ReportManager.ReportEntry.Note>) null;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    ((Component) this.added).GetComponent<ToolTip>().OnToolTip = new Func<string>(this.OnPositiveNoteTooltip);
    ((Component) this.removed).GetComponent<ToolTip>().OnToolTip = new Func<string>(this.OnNegativeNoteTooltip);
    ((Component) this.net).GetComponent<ToolTip>().OnToolTip = new Func<string>(this.OnNetNoteTooltip);
    ((Component) this.name).GetComponent<ToolTip>().OnToolTip = new Func<string>(this.OnNetNoteTooltip);
  }

  private string OnNoteTooltip(
    float total_accumulation,
    string tooltip_text,
    ReportManager.ReportEntry.Order order,
    ReportManager.FormattingFn format_fn,
    Func<ReportManager.ReportEntry.Note, bool> is_note_applicable_cb,
    ReportManager.GroupFormattingFn group_format_fn = null)
  {
    ReportScreenEntryRow.notes.Clear();
    this.entry.IterateNotes((Action<ReportManager.ReportEntry.Note>) (note =>
    {
      if (!is_note_applicable_cb(note))
        return;
      ReportScreenEntryRow.notes.Add(note);
    }));
    string str1 = "";
    float numEntries = Mathf.Max(this.entry.contextEntries.Count <= 0 ? (float) ReportScreenEntryRow.notes.Count : (float) this.entry.contextEntries.Count, 1f);
    foreach (ReportManager.ReportEntry.Note note in this.Sort(ReportScreenEntryRow.notes, this.reportGroup.posNoteOrder))
    {
      string str2 = format_fn(note.value);
      if (((Component) this.toggle).gameObject.activeInHierarchy && group_format_fn != null)
        str2 = group_format_fn(note.value, numEntries);
      str1 = string.Format((string) STRINGS.UI.ENDOFDAYREPORT.NOTES.NOTE_ENTRY_LINE_ITEM, (object) str1, (object) note.note, (object) str2);
    }
    string str3 = format_fn(total_accumulation);
    if (this.entry.context != null)
      return string.Format(tooltip_text + "\n" + str1, (object) str3, (object) this.entry.context);
    if (group_format_fn == null)
      return string.Format(tooltip_text + "\n" + str1, (object) str3, (object) STRINGS.UI.ENDOFDAYREPORT.MY_COLONY);
    string str4 = group_format_fn(total_accumulation, numEntries);
    return string.Format(tooltip_text + "\n" + str1, (object) str4, (object) STRINGS.UI.ENDOFDAYREPORT.MY_COLONY);
  }

  private string OnNegativeNoteTooltip() => this.OnNoteTooltip(-this.entry.Negative, this.reportGroup.negativeTooltip, this.reportGroup.negNoteOrder, this.reportGroup.formatfn, (Func<ReportManager.ReportEntry.Note, bool>) (note => this.IsNegativeNote(note)), this.reportGroup.groupFormatfn);

  private string OnPositiveNoteTooltip() => this.OnNoteTooltip(this.entry.Positive, this.reportGroup.positiveTooltip, this.reportGroup.posNoteOrder, this.reportGroup.formatfn, (Func<ReportManager.ReportEntry.Note, bool>) (note => this.IsPositiveNote(note)), this.reportGroup.groupFormatfn);

  private string OnNetNoteTooltip() => (double) this.entry.Net > 0.0 ? this.OnPositiveNoteTooltip() : this.OnNegativeNoteTooltip();

  private bool IsPositiveNote(ReportManager.ReportEntry.Note note) => (double) note.value > 0.0;

  private bool IsNegativeNote(ReportManager.ReportEntry.Note note) => (double) note.value < 0.0;

  public void SetLine(ReportManager.ReportEntry entry, ReportManager.ReportGroup reportGroup)
  {
    this.entry = entry;
    this.reportGroup = reportGroup;
    ListPool<ReportManager.ReportEntry.Note, ReportScreenEntryRow>.PooledList pos_notes = ListPool<ReportManager.ReportEntry.Note, ReportScreenEntryRow>.Allocate();
    entry.IterateNotes((Action<ReportManager.ReportEntry.Note>) (note =>
    {
      if (!this.IsPositiveNote(note))
        return;
      ((List<ReportManager.ReportEntry.Note>) pos_notes).Add(note);
    }));
    ListPool<ReportManager.ReportEntry.Note, ReportScreenEntryRow>.PooledList neg_notes = ListPool<ReportManager.ReportEntry.Note, ReportScreenEntryRow>.Allocate();
    entry.IterateNotes((Action<ReportManager.ReportEntry.Note>) (note =>
    {
      if (!this.IsNegativeNote(note))
        return;
      ((List<ReportManager.ReportEntry.Note>) neg_notes).Add(note);
    }));
    LayoutElement component = ((Component) this.name).GetComponent<LayoutElement>();
    if (entry.context == null)
    {
      component.minWidth = component.preferredWidth = this.nameWidth;
      if (entry.HasContextEntries())
      {
        ((Component) this.toggle).gameObject.SetActive(true);
        this.spacer.minWidth = this.groupSpacerWidth;
      }
      else
      {
        ((Component) this.toggle).gameObject.SetActive(false);
        this.spacer.minWidth = this.groupSpacerWidth + ((Component) this.toggle).GetComponent<LayoutElement>().minWidth;
      }
      ((TMP_Text) this.name).text = reportGroup.stringKey;
    }
    else
    {
      ((Component) this.toggle).gameObject.SetActive(false);
      this.spacer.minWidth = this.contextSpacerWidth;
      ((TMP_Text) this.name).text = entry.context;
      component.minWidth = component.preferredWidth = this.nameWidth - this.indentWidth;
      if (this.transform.GetSiblingIndex() % 2 != 0)
        ((Graphic) this.bgImage).color = this.oddRowColor;
    }
    if ((double) this.addedValue != (double) entry.Positive)
    {
      string str = reportGroup.formatfn(entry.Positive);
      if (reportGroup.groupFormatfn != null && entry.context == null)
      {
        float numEntries = Mathf.Max(entry.contextEntries.Count <= 0 ? (float) ((List<ReportManager.ReportEntry.Note>) pos_notes).Count : (float) entry.contextEntries.Count, 1f);
        str = reportGroup.groupFormatfn(entry.Positive, numEntries);
      }
      ((TMP_Text) this.added).text = str;
      this.addedValue = entry.Positive;
    }
    if ((double) this.removedValue != (double) entry.Negative)
    {
      string str = reportGroup.formatfn(-entry.Negative);
      if (reportGroup.groupFormatfn != null && entry.context == null)
      {
        float numEntries = Mathf.Max(entry.contextEntries.Count <= 0 ? (float) ((List<ReportManager.ReportEntry.Note>) neg_notes).Count : (float) entry.contextEntries.Count, 1f);
        str = reportGroup.groupFormatfn(-entry.Negative, numEntries);
      }
      ((TMP_Text) this.removed).text = str;
      this.removedValue = entry.Negative;
    }
    if ((double) this.netValue != (double) entry.Net)
    {
      string str = reportGroup.formatfn == null ? entry.Net.ToString() : reportGroup.formatfn(entry.Net);
      if (reportGroup.groupFormatfn != null && entry.context == null)
      {
        float numEntries = Mathf.Max(entry.contextEntries.Count <= 0 ? (float) (((List<ReportManager.ReportEntry.Note>) pos_notes).Count + ((List<ReportManager.ReportEntry.Note>) neg_notes).Count) : (float) entry.contextEntries.Count, 1f);
        str = reportGroup.groupFormatfn(entry.Net, numEntries);
      }
      ((TMP_Text) this.net).text = str;
      this.netValue = entry.Net;
    }
    pos_notes.Recycle();
    neg_notes.Recycle();
  }
}
