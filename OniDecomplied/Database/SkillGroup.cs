// Decompiled with JetBrains decompiler
// Type: Database.SkillGroup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System.Collections.Generic;

namespace Database
{
  public class SkillGroup : Resource, IListableOption
  {
    public string choreGroupID;
    public List<Attribute> relevantAttributes;
    public List<string> requiredChoreGroups;
    public string choreGroupIcon;
    public string archetypeIcon;

    string IListableOption.GetProperName() => StringEntry.op_Implicit(Strings.Get("STRINGS.DUPLICANTS.SKILLGROUPS." + this.Id.ToUpper() + ".NAME"));

    public SkillGroup(
      string id,
      string choreGroupID,
      string name,
      string icon,
      string archetype_icon)
      : base(id, name)
    {
      this.choreGroupID = choreGroupID;
      this.choreGroupIcon = icon;
      this.archetypeIcon = archetype_icon;
    }
  }
}
