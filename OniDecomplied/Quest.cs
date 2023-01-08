// Decompiled with JetBrains decompiler
// Type: Quest
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class Quest : Resource
{
  public const string STRINGS_PREFIX = "STRINGS.CODEX.QUESTS.";
  public readonly QuestCriteria[] Criteria;
  public readonly string Title;
  public readonly string CompletionText;

  public Quest(string id, QuestCriteria[] criteria)
    : base(id, id)
  {
    Debug.Assert(criteria.Length != 0);
    this.Criteria = criteria;
    string str = "STRINGS.CODEX.QUESTS." + id.ToUpperInvariant();
    StringEntry stringEntry;
    if (Strings.TryGet(str + ".NAME", ref stringEntry))
      this.Title = stringEntry.String;
    if (Strings.TryGet(str + ".COMPLETE", ref stringEntry))
      this.CompletionText = stringEntry.String;
    for (int index = 0; index < this.Criteria.Length; ++index)
      this.Criteria[index].PopulateStrings("STRINGS.CODEX.QUESTS.");
  }

  public struct ItemData
  {
    public int LocalCellId;
    public float CurrentValue;
    public Tag SatisfyingItem;
    public Tag QualifyingTag;
    public HashedString CriteriaId;
    private int valueHandle;

    public int ValueHandle
    {
      get => this.valueHandle - 1;
      set => this.valueHandle = value + 1;
    }
  }

  public enum State
  {
    NotStarted,
    InProgress,
    Completed,
  }
}
