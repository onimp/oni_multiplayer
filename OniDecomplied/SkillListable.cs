// Decompiled with JetBrains decompiler
// Type: SkillListable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;

public class SkillListable : IListableOption
{
  public LocString name;

  public SkillListable(string name)
  {
    this.skillName = name;
    Skill skill = Db.Get().Skills.TryGet(this.skillName);
    if (skill == null)
      return;
    this.name = (LocString) skill.Name;
    this.skillHat = skill.hat;
  }

  public string skillName { get; private set; }

  public string skillHat { get; private set; }

  public string GetProperName() => (string) this.name;
}
