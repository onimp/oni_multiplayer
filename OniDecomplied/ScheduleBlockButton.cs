// Decompiled with JetBrains decompiler
// Type: ScheduleBlockButton
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ScheduleBlockButton")]
public class ScheduleBlockButton : KMonoBehaviour
{
  [SerializeField]
  private KImage image;
  [SerializeField]
  private ToolTip toolTip;
  private Dictionary<string, ColorStyleSetting> paintStyles;

  public int idx { get; private set; }

  public void Setup(int idx, Dictionary<string, ColorStyleSetting> paintStyles, int totalBlocks)
  {
    this.idx = idx;
    this.paintStyles = paintStyles;
    if (idx < TRAITS.EARLYBIRD_SCHEDULEBLOCK)
      ((Component) ((Component) this).GetComponent<HierarchyReferences>().GetReference<RectTransform>("MorningIcon")).gameObject.SetActive(true);
    else if (idx >= totalBlocks - 3)
      ((Component) ((Component) this).GetComponent<HierarchyReferences>().GetReference<RectTransform>("NightIcon")).gameObject.SetActive(true);
    ((Object) ((Component) this).gameObject).name = "ScheduleBlock_" + idx.ToString();
  }

  public void SetBlockTypes(List<ScheduleBlockType> blockTypes)
  {
    ScheduleGroup forScheduleTypes = Db.Get().ScheduleGroups.FindGroupForScheduleTypes(blockTypes);
    if (forScheduleTypes != null && this.paintStyles.ContainsKey(forScheduleTypes.Id))
    {
      this.image.colorStyleSetting = this.paintStyles[forScheduleTypes.Id];
      this.image.ApplyColorStyleSetting();
      this.toolTip.SetSimpleTooltip(forScheduleTypes.GetTooltip());
    }
    else
      this.toolTip.SetSimpleTooltip("UNKNOWN");
  }
}
