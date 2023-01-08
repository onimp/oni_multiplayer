// Decompiled with JetBrains decompiler
// Type: TimeRangeSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TimeRangeSideScreen : SideScreenContent, IRender200ms
{
  public Image imageInactiveZone;
  public Image imageActiveZone;
  private LogicTimeOfDaySensor targetTimedSwitch;
  public KSlider startTime;
  public KSlider duration;
  public RectTransform endIndicator;
  public LocText labelHeaderStart;
  public LocText labelHeaderDuration;
  public LocText labelValueStart;
  public LocText labelValueDuration;
  public RectTransform currentTimeMarker;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    ((TMP_Text) this.labelHeaderStart).text = (string) STRINGS.UI.UISIDESCREENS.TIME_RANGE_SIDE_SCREEN.ON;
    ((TMP_Text) this.labelHeaderDuration).text = (string) STRINGS.UI.UISIDESCREENS.TIME_RANGE_SIDE_SCREEN.DURATION;
  }

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<LogicTimeOfDaySensor>(), (Object) null);

  public override void SetTarget(GameObject target)
  {
    ((Graphic) this.imageActiveZone).color = Color32.op_Implicit(GlobalAssets.Instance.colorSet.logicOnSidescreen);
    ((Graphic) this.imageInactiveZone).color = Color32.op_Implicit(GlobalAssets.Instance.colorSet.logicOffSidescreen);
    base.SetTarget(target);
    this.targetTimedSwitch = target.GetComponent<LogicTimeOfDaySensor>();
    ((UnityEventBase) ((Slider) this.duration).onValueChanged).RemoveAllListeners();
    ((UnityEventBase) ((Slider) this.startTime).onValueChanged).RemoveAllListeners();
    ((Slider) this.startTime).value = this.targetTimedSwitch.startTime;
    ((Slider) this.duration).value = this.targetTimedSwitch.duration;
    this.ChangeSetting();
    // ISSUE: method pointer
    ((UnityEvent<float>) ((Slider) this.startTime).onValueChanged).AddListener(new UnityAction<float>((object) this, __methodptr(\u003CSetTarget\u003Eb__13_0)));
    // ISSUE: method pointer
    ((UnityEvent<float>) ((Slider) this.duration).onValueChanged).AddListener(new UnityAction<float>((object) this, __methodptr(\u003CSetTarget\u003Eb__13_1)));
  }

  private void ChangeSetting()
  {
    this.targetTimedSwitch.startTime = ((Slider) this.startTime).value;
    this.targetTimedSwitch.duration = ((Slider) this.duration).value;
    ((Transform) ((Graphic) this.imageActiveZone).rectTransform).rotation = Quaternion.identity;
    ((Transform) ((Graphic) this.imageActiveZone).rectTransform).Rotate(0.0f, 0.0f, this.NormalizedValueToDegrees(((Slider) this.startTime).value));
    this.imageActiveZone.fillAmount = ((Slider) this.duration).value;
    ((TMP_Text) this.labelValueStart).text = GameUtil.GetFormattedPercent(this.targetTimedSwitch.startTime * 100f);
    ((TMP_Text) this.labelValueDuration).text = GameUtil.GetFormattedPercent(this.targetTimedSwitch.duration * 100f);
    ((Transform) this.endIndicator).rotation = Quaternion.identity;
    ((Transform) this.endIndicator).Rotate(0.0f, 0.0f, this.NormalizedValueToDegrees(((Slider) this.startTime).value + ((Slider) this.duration).value));
    this.startTime.SetTooltipText(string.Format((string) STRINGS.UI.UISIDESCREENS.TIME_RANGE_SIDE_SCREEN.ON_TOOLTIP, (object) GameUtil.GetFormattedPercent(this.targetTimedSwitch.startTime * 100f)));
    this.duration.SetTooltipText(string.Format((string) STRINGS.UI.UISIDESCREENS.TIME_RANGE_SIDE_SCREEN.DURATION_TOOLTIP, (object) GameUtil.GetFormattedPercent(this.targetTimedSwitch.duration * 100f)));
  }

  public void Render200ms(float dt)
  {
    ((Transform) this.currentTimeMarker).rotation = Quaternion.identity;
    ((Transform) this.currentTimeMarker).Rotate(0.0f, 0.0f, this.NormalizedValueToDegrees(GameClock.Instance.GetCurrentCycleAsPercentage()));
  }

  private float NormalizedValueToDegrees(float value) => 360f * value;

  private float SecondsToDegrees(float seconds) => (float) (360.0 * ((double) seconds / 600.0));

  private float DegreesToNormalizedValue(float degrees) => degrees / 360f;
}
