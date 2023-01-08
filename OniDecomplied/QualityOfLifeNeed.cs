// Decompiled with JetBrains decompiler
// Type: QualityOfLifeNeed
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using Klei.CustomSettings;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
public class QualityOfLifeNeed : Need, ISim4000ms
{
  private AttributeInstance qolAttribute;
  public bool skipUpdate;
  [Serialize]
  private List<bool> breakBlocks;
  private static readonly EventSystem.IntraObjectHandler<QualityOfLifeNeed> OnScheduleBlocksTickDelegate = new EventSystem.IntraObjectHandler<QualityOfLifeNeed>((Action<QualityOfLifeNeed, object>) ((component, data) => component.OnScheduleBlocksTick(data)));
  private static List<string> breakLengthEffects = new List<string>()
  {
    "Break1",
    "Break2",
    "Break3",
    "Break4",
    "Break5"
  };

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.breakBlocks = new List<bool>(24);
    this.expectationAttribute = ((Component) this).gameObject.GetAttributes().Add(Db.Get().Attributes.QualityOfLifeExpectation);
    this.Name = (string) DUPLICANTS.NEEDS.QUALITYOFLIFE.NAME;
    this.ExpectationTooltip = string.Format((string) DUPLICANTS.NEEDS.QUALITYOFLIFE.EXPECTATION_TOOLTIP, (object) Db.Get().Attributes.QualityOfLifeExpectation.Lookup((Component) this).GetTotalValue());
    this.stressBonus = new Need.ModifierType()
    {
      modifier = new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0.0f, (string) DUPLICANTS.NEEDS.QUALITYOFLIFE.GOOD_MODIFIER, is_readonly: false)
    };
    this.stressNeutral = new Need.ModifierType()
    {
      modifier = new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, -0.008333334f, (string) DUPLICANTS.NEEDS.QUALITYOFLIFE.NEUTRAL_MODIFIER)
    };
    this.stressPenalty = new Need.ModifierType()
    {
      modifier = new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0.0f, (string) DUPLICANTS.NEEDS.QUALITYOFLIFE.BAD_MODIFIER, is_readonly: false),
      statusItem = Db.Get().DuplicantStatusItems.PoorQualityOfLife
    };
    this.qolAttribute = Db.Get().Attributes.QualityOfLife.Lookup(((Component) this).gameObject);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    while (this.breakBlocks.Count < 24)
      this.breakBlocks.Add(false);
    while (this.breakBlocks.Count > 24)
      this.breakBlocks.RemoveAt(this.breakBlocks.Count - 1);
    this.Subscribe<QualityOfLifeNeed>(1714332666, QualityOfLifeNeed.OnScheduleBlocksTickDelegate);
  }

  public void Sim4000ms(float dt)
  {
    if (this.skipUpdate)
      return;
    float num1 = 0.004166667f;
    float num2 = 0.0416666679f;
    SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.Morale);
    if (currentQualitySetting.id == "Disabled")
    {
      this.SetModifier((Need.ModifierType) null);
    }
    else
    {
      if (currentQualitySetting.id == "Easy")
      {
        num1 = 1f / 300f;
        num2 = 0.0166666675f;
      }
      else if (currentQualitySetting.id == "Hard")
      {
        num1 = 0.008333334f;
        num2 = 0.05f;
      }
      else if (currentQualitySetting.id == "VeryHard")
      {
        num1 = 0.0166666675f;
        num2 = 0.0833333358f;
      }
      float totalValue1 = this.qolAttribute.GetTotalValue();
      float totalValue2 = this.expectationAttribute.GetTotalValue();
      float num3 = totalValue2 - totalValue1;
      if ((double) totalValue1 < (double) totalValue2)
      {
        this.stressPenalty.modifier.SetValue(Mathf.Min(num3 * num1, num2));
        this.SetModifier(this.stressPenalty);
      }
      else if ((double) totalValue1 > (double) totalValue2)
      {
        this.stressBonus.modifier.SetValue(Mathf.Max((float) (-(double) num3 * -0.01666666753590107), -0.0333333351f));
        this.SetModifier(this.stressBonus);
      }
      else
        this.SetModifier(this.stressNeutral);
    }
  }

  private void OnScheduleBlocksTick(object data)
  {
    Schedule schedule = (Schedule) data;
    ScheduleBlock block1 = schedule.GetBlock(Schedule.GetLastBlockIdx());
    ScheduleBlock block2 = schedule.GetBlock(Schedule.GetBlockIdx());
    bool flag1 = block1.IsAllowed(Db.Get().ScheduleBlockTypes.Recreation);
    ScheduleBlockType recreation = Db.Get().ScheduleBlockTypes.Recreation;
    bool flag2 = block2.IsAllowed(recreation);
    this.breakBlocks[Schedule.GetLastBlockIdx()] = flag1;
    if (!flag1 || flag2)
      return;
    int numBlocks = 0;
    foreach (bool breakBlock in this.breakBlocks)
    {
      if (breakBlock)
        ++numBlocks;
    }
    this.ApplyBreakBonus(numBlocks);
  }

  private void ApplyBreakBonus(int numBlocks)
  {
    string breakBonus = QualityOfLifeNeed.GetBreakBonus(numBlocks);
    if (breakBonus == null)
      return;
    ((Component) this).GetComponent<Effects>().Add(breakBonus, true);
  }

  public static string GetBreakBonus(int numBlocks)
  {
    int index = numBlocks - 1;
    if (index >= QualityOfLifeNeed.breakLengthEffects.Count)
      return QualityOfLifeNeed.breakLengthEffects[QualityOfLifeNeed.breakLengthEffects.Count - 1];
    return index >= 0 ? QualityOfLifeNeed.breakLengthEffects[index] : (string) null;
  }
}
