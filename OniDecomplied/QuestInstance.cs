// Decompiled with JetBrains decompiler
// Type: QuestInstance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class QuestInstance : ISaveLoadable
{
  public Action<QuestInstance, Quest.State, float> QuestProgressChanged;
  private Quest quest;
  [Serialize]
  private Dictionary<int, QuestInstance.CriteriaState> criteriaStates;
  [Serialize]
  private Quest.State currentState;

  public HashedString Id => this.quest.IdHash;

  public string Name => this.quest.Name;

  public string CompletionText => this.quest.CompletionText;

  public bool IsStarted => this.currentState != 0;

  public bool IsComplete => this.currentState == Quest.State.Completed;

  public float CurrentProgress { get; private set; }

  public Quest.State CurrentState => this.currentState;

  public QuestInstance(Quest quest)
  {
    this.quest = quest;
    this.criteriaStates = new Dictionary<int, QuestInstance.CriteriaState>(quest.Criteria.Length);
    for (int index = 0; index < quest.Criteria.Length; ++index)
    {
      QuestCriteria criterion = quest.Criteria[index];
      QuestInstance.CriteriaState criteriaState1 = new QuestInstance.CriteriaState()
      {
        Handle = index
      };
      if (criterion.TargetValues != null)
      {
        if ((criterion.EvaluationBehaviors & QuestCriteria.BehaviorFlags.TrackItems) == QuestCriteria.BehaviorFlags.TrackItems)
          criteriaState1.SatisfyingItems = new Tag[criterion.TargetValues.Length * criterion.RequiredCount];
        if ((criterion.EvaluationBehaviors & QuestCriteria.BehaviorFlags.TrackValues) == QuestCriteria.BehaviorFlags.TrackValues)
          criteriaState1.CurrentValues = new float[criterion.TargetValues.Length * criterion.RequiredCount];
      }
      Dictionary<int, QuestInstance.CriteriaState> criteriaStates = this.criteriaStates;
      Tag criteriaId = criterion.CriteriaId;
      int hash = ((Tag) ref criteriaId).GetHash();
      QuestInstance.CriteriaState criteriaState2 = criteriaState1;
      criteriaStates[hash] = criteriaState2;
    }
  }

  public void Initialize(Quest quest)
  {
    this.quest = quest;
    this.UpdateQuestProgress();
  }

  public bool HasCriteria(HashedString criteriaId) => this.criteriaStates.ContainsKey(((HashedString) ref criteriaId).HashValue);

  public bool HasBehavior(QuestCriteria.BehaviorFlags behavior)
  {
    bool flag = false;
    for (int index = 0; !flag && index < this.quest.Criteria.Length; ++index)
      flag = (this.quest.Criteria[index].EvaluationBehaviors & behavior) != 0;
    return flag;
  }

  public int GetTargetCount(HashedString criteriaId)
  {
    QuestInstance.CriteriaState criteriaState;
    return !this.criteriaStates.TryGetValue(((HashedString) ref criteriaId).HashValue, out criteriaState) ? 0 : this.quest.Criteria[criteriaState.Handle].RequiredCount;
  }

  public int GetCurrentCount(HashedString criteriaId)
  {
    QuestInstance.CriteriaState criteriaState;
    return !this.criteriaStates.TryGetValue(((HashedString) ref criteriaId).HashValue, out criteriaState) ? 0 : criteriaState.CurrentCount;
  }

  public float GetCurrentValue(HashedString criteriaId, int valueHandle = 0)
  {
    QuestInstance.CriteriaState criteriaState;
    return !this.criteriaStates.TryGetValue(((HashedString) ref criteriaId).HashValue, out criteriaState) || criteriaState.CurrentValues == null ? float.NaN : criteriaState.CurrentValues[valueHandle];
  }

  public float GetTargetValue(HashedString criteriaId, int valueHandle = 0)
  {
    QuestInstance.CriteriaState criteriaState;
    return !this.criteriaStates.TryGetValue(((HashedString) ref criteriaId).HashValue, out criteriaState) || this.quest.Criteria[criteriaState.Handle].TargetValues == null ? float.NaN : this.quest.Criteria[criteriaState.Handle].TargetValues[valueHandle];
  }

  public Tag GetSatisfyingItem(HashedString criteriaId, int valueHandle = 0)
  {
    QuestInstance.CriteriaState criteriaState;
    return !this.criteriaStates.TryGetValue(((HashedString) ref criteriaId).HashValue, out criteriaState) || criteriaState.SatisfyingItems == null ? new Tag() : criteriaState.SatisfyingItems[valueHandle];
  }

  public float GetAreaAverage(HashedString criteriaId)
  {
    QuestInstance.CriteriaState criteriaState;
    if (!this.criteriaStates.TryGetValue(((HashedString) ref criteriaId).HashValue, out criteriaState) || !QuestCriteria.HasBehavior(this.quest.Criteria[criteriaState.Handle].EvaluationBehaviors, QuestCriteria.BehaviorFlags.TrackArea | QuestCriteria.BehaviorFlags.TrackValues))
      return float.NaN;
    float num = 0.0f;
    for (int index = 0; index < criteriaState.CurrentValues.Length; ++index)
      num += criteriaState.CurrentValues[index];
    return num / (float) criteriaState.CurrentValues.Length;
  }

  public bool IsItemRedundant(HashedString criteriaId, Tag item)
  {
    QuestInstance.CriteriaState criteriaState;
    if (!this.criteriaStates.TryGetValue(((HashedString) ref criteriaId).HashValue, out criteriaState) || criteriaState.SatisfyingItems == null)
      return false;
    bool flag = false;
    for (int index = 0; !flag && index < criteriaState.SatisfyingItems.Length; ++index)
      flag = Tag.op_Equality(criteriaState.SatisfyingItems[index], item);
    return flag;
  }

  public bool IsCriteriaSatisfied(HashedString id)
  {
    QuestInstance.CriteriaState state;
    return this.criteriaStates.TryGetValue(((HashedString) ref id).HashValue, out state) && this.quest.Criteria[state.Handle].IsSatisfied(state.SatisfactionState, this.GetSatisfactionMask(state));
  }

  public bool IsCriteriaSatisfied(Tag id)
  {
    QuestInstance.CriteriaState state;
    return this.criteriaStates.TryGetValue(((Tag) ref id).GetHash(), out state) && this.quest.Criteria[state.Handle].IsSatisfied(state.SatisfactionState, this.GetSatisfactionMask(state));
  }

  public void TrackAreaForCriteria(HashedString criteriaId, Extents area)
  {
    QuestInstance.CriteriaState criteriaState;
    if (!this.criteriaStates.TryGetValue(((HashedString) ref criteriaId).HashValue, out criteriaState))
      return;
    int length = area.width * area.height;
    QuestCriteria criterion = this.quest.Criteria[criteriaState.Handle];
    Debug.Assert(length <= 32);
    if (QuestCriteria.HasBehavior(criterion.EvaluationBehaviors, QuestCriteria.BehaviorFlags.TrackValues))
      criteriaState.CurrentValues = new float[length];
    if (QuestCriteria.HasBehavior(criterion.EvaluationBehaviors, QuestCriteria.BehaviorFlags.TrackItems))
      criteriaState.SatisfyingItems = new Tag[length];
    this.criteriaStates[((HashedString) ref criteriaId).HashValue] = criteriaState;
  }

  private uint GetSatisfactionMask(QuestInstance.CriteriaState state)
  {
    QuestCriteria criterion = this.quest.Criteria[state.Handle];
    if (!QuestCriteria.HasBehavior(criterion.EvaluationBehaviors, QuestCriteria.BehaviorFlags.TrackArea))
      return criterion.GetSatisfactionMask();
    int num = 0;
    if (state.SatisfyingItems != null)
      num = state.SatisfyingItems.Length;
    else if (state.CurrentValues != null)
      num = state.CurrentValues.Length;
    return (uint) ((double) Mathf.Pow(2f, (float) num) - 1.0);
  }

  public int TrackProgress(Quest.ItemData data, out bool dataSatisfies, out bool itemIsRedundant)
  {
    dataSatisfies = false;
    itemIsRedundant = false;
    QuestInstance.CriteriaState state;
    if (!this.criteriaStates.TryGetValue(((HashedString) ref data.CriteriaId).HashValue, out state))
      return -1;
    int valueHandle = data.ValueHandle;
    QuestCriteria criterion = this.quest.Criteria[state.Handle];
    dataSatisfies = this.DataSatisfiesCriteria(data, ref valueHandle);
    if (valueHandle == -1)
      return valueHandle;
    bool flag1 = QuestCriteria.HasBehavior(criterion.EvaluationBehaviors, QuestCriteria.BehaviorFlags.AllowsRegression);
    bool flag2 = QuestCriteria.HasBehavior(criterion.EvaluationBehaviors, QuestCriteria.BehaviorFlags.TrackItems);
    Tag tag = flag2 ? state.SatisfyingItems[valueHandle] : new Tag();
    if (dataSatisfies)
    {
      itemIsRedundant = QuestCriteria.HasBehavior(criterion.EvaluationBehaviors, QuestCriteria.BehaviorFlags.UniqueItems) && this.IsItemRedundant(data.CriteriaId, data.SatisfyingItem);
      if (itemIsRedundant)
        return valueHandle;
      tag = data.SatisfyingItem;
      state.SatisfactionState |= criterion.GetValueMask(valueHandle);
    }
    else if (flag1)
      state.SatisfactionState &= ~criterion.GetValueMask(valueHandle);
    if (QuestCriteria.HasBehavior(criterion.EvaluationBehaviors, QuestCriteria.BehaviorFlags.TrackValues))
      state.CurrentValues[valueHandle] = data.CurrentValue;
    if (flag2)
      state.SatisfyingItems[valueHandle] = tag;
    bool flag3 = this.IsCriteriaSatisfied(data.CriteriaId);
    bool flag4 = criterion.IsSatisfied(state.SatisfactionState, this.GetSatisfactionMask(state));
    if (flag3 != flag4)
    {
      state.CurrentCount += flag3 ? -1 : 1;
      if (flag4 && state.CurrentCount < criterion.RequiredCount)
        state.SatisfactionState = 0U;
    }
    this.criteriaStates[((HashedString) ref data.CriteriaId).HashValue] = state;
    this.UpdateQuestProgress(true);
    return valueHandle;
  }

  public bool DataSatisfiesCriteria(Quest.ItemData data, ref int valueHandle)
  {
    QuestInstance.CriteriaState criteriaState;
    if (!this.criteriaStates.TryGetValue(((HashedString) ref data.CriteriaId).HashValue, out criteriaState))
      return false;
    QuestCriteria criterion = this.quest.Criteria[criteriaState.Handle];
    bool flag1 = criterion.AcceptedTags == null || ((Tag) ref data.QualifyingTag).IsValid && criterion.AcceptedTags.Contains(data.QualifyingTag);
    if (flag1 && criterion.TargetValues == null)
      valueHandle = 0;
    if (!flag1 || valueHandle != -1)
      return flag1 && criterion.ValueSatisfies(data.CurrentValue, valueHandle);
    if (QuestCriteria.HasBehavior(criterion.EvaluationBehaviors, QuestCriteria.BehaviorFlags.TrackArea))
      valueHandle = data.LocalCellId;
    int index = -1;
    bool flag2 = QuestCriteria.HasBehavior(criterion.EvaluationBehaviors, QuestCriteria.BehaviorFlags.TrackValues);
    bool flag3 = false;
    for (int valueHandle1 = 0; !flag3 && valueHandle1 < criterion.TargetValues.Length; ++valueHandle1)
    {
      if (criterion.ValueSatisfies(data.CurrentValue, valueHandle1))
      {
        flag3 = true;
        index = valueHandle1;
        break;
      }
      if (flag2 && (index == -1 || (double) criteriaState.CurrentValues[index] > (double) criteriaState.CurrentValues[valueHandle1]))
        index = valueHandle1;
    }
    if (valueHandle == -1 && index != -1)
      valueHandle = criterion.RequiredCount * index + Mathf.Min(criteriaState.CurrentCount, criterion.RequiredCount - 1);
    return flag3;
  }

  private void UpdateQuestProgress(bool startQuest = false)
  {
    if (!this.IsStarted && !startQuest)
      return;
    float currentProgress = this.CurrentProgress;
    Quest.State currentState = this.currentState;
    this.currentState = Quest.State.InProgress;
    this.CurrentProgress = 0.0f;
    float num1 = 0.0f;
    for (int index1 = 0; index1 < this.quest.Criteria.Length; ++index1)
    {
      QuestCriteria criterion = this.quest.Criteria[index1];
      Dictionary<int, QuestInstance.CriteriaState> criteriaStates = this.criteriaStates;
      Tag criteriaId = criterion.CriteriaId;
      int hash = ((Tag) ref criteriaId).GetHash();
      QuestInstance.CriteriaState criteriaState = criteriaStates[hash];
      float num2 = criterion.TargetValues != null ? (float) criterion.TargetValues.Length : 1f;
      num1 += (float) criterion.RequiredCount;
      this.CurrentProgress += (float) criteriaState.CurrentCount;
      if (!this.IsCriteriaSatisfied(criterion.CriteriaId))
      {
        float num3 = 0.0f;
        for (int valueHandle = 0; criterion.TargetValues != null && (double) valueHandle < (double) num2; ++valueHandle)
        {
          if (((int) criteriaState.SatisfactionState & (int) criterion.GetValueMask(valueHandle)) == 0)
          {
            if (QuestCriteria.HasBehavior(criterion.EvaluationBehaviors, QuestCriteria.BehaviorFlags.TrackValues))
            {
              int index2 = criterion.RequiredCount * valueHandle + Mathf.Min(criteriaState.CurrentCount, criterion.RequiredCount - 1);
              num3 += Mathf.Max(0.0f, criteriaState.CurrentValues[index2] / criterion.TargetValues[valueHandle]);
            }
          }
          else
            ++num3;
        }
        this.CurrentProgress += num3 / num2;
      }
    }
    this.CurrentProgress = Mathf.Clamp01(this.CurrentProgress / num1);
    if ((double) this.CurrentProgress == 1.0)
      this.currentState = Quest.State.Completed;
    float num4 = this.CurrentProgress - currentProgress;
    if (currentState == this.currentState && (double) Mathf.Abs(num4) <= (double) Mathf.Epsilon)
      return;
    Action<QuestInstance, Quest.State, float> questProgressChanged = this.QuestProgressChanged;
    if (questProgressChanged == null)
      return;
    questProgressChanged(this, currentState, num4);
  }

  public ICheckboxListGroupControl.CheckboxItem[] GetCheckBoxData(
    Func<int, string, QuestInstance, string> resolveToolTip = null)
  {
    ICheckboxListGroupControl.CheckboxItem[] checkBoxData = new ICheckboxListGroupControl.CheckboxItem[this.quest.Criteria.Length];
    for (int index = 0; index < this.quest.Criteria.Length; ++index)
    {
      QuestCriteria c = this.quest.Criteria[index];
      checkBoxData[index] = new ICheckboxListGroupControl.CheckboxItem()
      {
        text = c.Text,
        isOn = this.IsCriteriaSatisfied(c.CriteriaId),
        tooltip = c.Tooltip
      };
      if (resolveToolTip != null)
        checkBoxData[index].resolveTooltipCallback = (Func<string, object, string>) ((tooltip, owner) =>
        {
          Func<int, string, QuestInstance, string> func = resolveToolTip;
          Tag criteriaId = c.CriteriaId;
          int hash = ((Tag) ref criteriaId).GetHash();
          string tooltip1 = c.Tooltip;
          return func(hash, tooltip1, this);
        });
    }
    return checkBoxData;
  }

  private struct CriteriaState
  {
    public int Handle;
    public int CurrentCount;
    public uint SatisfactionState;
    public Tag[] SatisfyingItems;
    public float[] CurrentValues;

    public static bool ItemAlreadySatisfying(QuestInstance.CriteriaState state, Tag item)
    {
      bool flag = false;
      for (int index = 0; state.SatisfyingItems != null && index < state.SatisfyingItems.Length; ++index)
      {
        if (Tag.op_Equality(state.SatisfyingItems[index], item))
        {
          flag = true;
          break;
        }
      }
      return flag;
    }
  }
}
