// Decompiled with JetBrains decompiler
// Type: EntityModifierSet
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;

public class EntityModifierSet : ModifierSet
{
  public DuplicantStatusItems DuplicantStatusItems;
  public ChoreGroups ChoreGroups;

  public override void Initialize()
  {
    base.Initialize();
    this.DuplicantStatusItems = new DuplicantStatusItems(this.Root);
    this.ChoreGroups = new ChoreGroups(this.Root);
    this.LoadTraits();
  }
}
