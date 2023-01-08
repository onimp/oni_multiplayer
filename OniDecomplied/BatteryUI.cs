// Decompiled with JetBrains decompiler
// Type: BatteryUI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/BatteryUI")]
public class BatteryUI : KMonoBehaviour
{
  [SerializeField]
  private LocText currentKJLabel;
  [SerializeField]
  private Image batteryBG;
  [SerializeField]
  private Image batteryMeter;
  [SerializeField]
  private Sprite regularBatteryBG;
  [SerializeField]
  private Sprite bigBatteryBG;
  [SerializeField]
  private Color energyIncreaseColor = Color.green;
  [SerializeField]
  private Color energyDecreaseColor = Color.red;
  private LocText unitLabel;
  private const float UIUnit = 10f;
  private Dictionary<float, float> sizeMap;

  private void Initialize()
  {
    if (Object.op_Equality((Object) this.unitLabel, (Object) null))
      this.unitLabel = KMonoBehaviourExtensions.GetComponentInChildrenOnly<LocText>(((Component) this.currentKJLabel).gameObject);
    if (this.sizeMap != null && this.sizeMap.Count != 0)
      return;
    this.sizeMap = new Dictionary<float, float>();
    this.sizeMap.Add(20000f, 10f);
    this.sizeMap.Add(40000f, 25f);
    this.sizeMap.Add(60000f, 40f);
  }

  public void SetContent(Battery bat)
  {
    if (Object.op_Equality((Object) bat, (Object) null) || bat.GetMyWorldId() != ClusterManager.Instance.activeWorldId)
    {
      if (!((Component) this).gameObject.activeSelf)
        return;
      ((Component) this).gameObject.SetActive(false);
    }
    else
    {
      ((Component) this).gameObject.SetActive(true);
      this.Initialize();
      RectTransform component1 = ((Component) this.batteryBG).GetComponent<RectTransform>();
      float num1 = 0.0f;
      foreach (KeyValuePair<float, float> size in this.sizeMap)
      {
        if ((double) bat.Capacity <= (double) size.Key)
        {
          num1 = size.Value;
          break;
        }
      }
      this.batteryBG.sprite = (double) bat.Capacity >= 40000.0 ? this.bigBatteryBG : this.regularBatteryBG;
      float num2 = 25f;
      component1.sizeDelta = new Vector2(num1, num2);
      BuildingEnabledButton component2 = ((Component) bat).GetComponent<BuildingEnabledButton>();
      Color color = !Object.op_Inequality((Object) component2, (Object) null) || component2.IsEnabled ? ((double) bat.PercentFull >= (double) bat.PreviousPercentFull ? this.energyIncreaseColor : this.energyDecreaseColor) : Color.gray;
      ((Graphic) this.batteryMeter).color = color;
      ((Graphic) this.batteryBG).color = color;
      Rect rect = ((Component) this.batteryBG).GetComponent<RectTransform>().rect;
      float num3 = ((Rect) ref rect).height * bat.PercentFull;
      ((Component) this.batteryMeter).GetComponent<RectTransform>().sizeDelta = new Vector2(num1 - 5.5f, num3 - 5.5f);
      color.a = 1f;
      if (Color.op_Inequality(((Graphic) this.currentKJLabel).color, color))
      {
        ((Graphic) this.currentKJLabel).color = color;
        ((Graphic) this.unitLabel).color = color;
      }
      ((TMP_Text) this.currentKJLabel).text = bat.JoulesAvailable.ToString("F0");
    }
  }
}
