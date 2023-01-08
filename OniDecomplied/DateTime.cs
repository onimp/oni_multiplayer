// Decompiled with JetBrains decompiler
// Type: DateTime
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TMPro;
using UnityEngine;

public class DateTime : KScreen
{
  public static DateTime Instance;
  public LocText day;
  private int displayedDayCount = -1;
  [SerializeField]
  private LocText text;
  [SerializeField]
  private ToolTip tooltip;
  [SerializeField]
  private TextStyleSetting tooltipstyle_Days;
  [SerializeField]
  private TextStyleSetting tooltipstyle_Playtime;
  [SerializeField]
  public KToggle scheduleToggle;

  public static void DestroyInstance() => DateTime.Instance = (DateTime) null;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    DateTime.Instance = this;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    // ISSUE: method pointer
    this.tooltip.OnComplexToolTip = new ToolTip.ComplexTooltipDelegate((object) SaveGame.Instance, __methodptr(GetColonyToolTip));
  }

  private void Update()
  {
    if (!Object.op_Inequality((Object) GameClock.Instance, (Object) null) || this.displayedDayCount == GameUtil.GetCurrentCycle())
      return;
    ((TMP_Text) this.text).text = this.Days();
    this.displayedDayCount = GameUtil.GetCurrentCycle();
  }

  private string Days() => GameUtil.GetCurrentCycle().ToString();
}
