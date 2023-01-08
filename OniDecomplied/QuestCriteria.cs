// Decompiled with JetBrains decompiler
// Type: QuestCriteria
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class QuestCriteria
{
  public const int MAX_VALUES = 32;
  public const int INVALID_VALUE = -1;
  public readonly Tag CriteriaId;
  public readonly QuestCriteria.BehaviorFlags EvaluationBehaviors;
  public readonly float[] TargetValues;
  public readonly int RequiredCount = 1;
  public readonly HashSet<Tag> AcceptedTags;

  public string Text { get; private set; }

  public string Tooltip { get; private set; }

  public QuestCriteria(
    Tag id,
    float[] targetValues = null,
    int requiredCount = 1,
    HashSet<Tag> acceptedTags = null,
    QuestCriteria.BehaviorFlags flags = QuestCriteria.BehaviorFlags.None)
  {
    Debug.Assert(targetValues == null || targetValues.Length != 0 && targetValues.Length <= 32);
    this.CriteriaId = id;
    this.EvaluationBehaviors = flags;
    this.TargetValues = targetValues;
    this.AcceptedTags = acceptedTags;
    this.RequiredCount = requiredCount;
  }

  public bool ValueSatisfies(float value, int valueHandle)
  {
    if (float.IsNaN(value))
      return false;
    float target = this.TargetValues == null ? 0.0f : this.TargetValues[valueHandle];
    return this.ValueSatisfies_Internal(value, target);
  }

  protected virtual bool ValueSatisfies_Internal(float current, float target) => true;

  public bool IsSatisfied(uint satisfactionState, uint satisfactionMask) => ((int) satisfactionState & (int) satisfactionMask) == (int) satisfactionMask;

  public void PopulateStrings(string prefix)
  {
    Tag criteriaId = this.CriteriaId;
    string upperInvariant = ((Tag) ref criteriaId).Name.ToUpperInvariant();
    StringEntry stringEntry;
    if (Strings.TryGet(prefix + "CRITERIA." + upperInvariant + ".NAME", ref stringEntry))
      this.Text = stringEntry.String;
    if (!Strings.TryGet(prefix + "CRITERIA." + upperInvariant + ".TOOLTIP", ref stringEntry))
      return;
    this.Tooltip = stringEntry.String;
  }

  public uint GetSatisfactionMask() => this.TargetValues == null ? 1U : (uint) Mathf.Pow(2f, (float) (this.TargetValues.Length - 1));

  public uint GetValueMask(int valueHandle)
  {
    if (this.TargetValues == null)
      return 1;
    if (!QuestCriteria.HasBehavior(this.EvaluationBehaviors, QuestCriteria.BehaviorFlags.TrackArea))
      valueHandle %= this.TargetValues.Length;
    return (uint) (1 << valueHandle);
  }

  public static bool HasBehavior(
    QuestCriteria.BehaviorFlags flags,
    QuestCriteria.BehaviorFlags behavior)
  {
    return (flags & behavior) == behavior;
  }

  public enum BehaviorFlags
  {
    None = 0,
    TrackArea = 1,
    AllowsRegression = 2,
    TrackValues = 4,
    TrackItems = 8,
    UniqueItems = 24, // 0x00000018
  }
}
