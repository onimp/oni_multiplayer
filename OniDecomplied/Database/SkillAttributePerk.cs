// Decompiled with JetBrains decompiler
// Type: Database.SkillAttributePerk
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;

namespace Database
{
  public class SkillAttributePerk : SkillPerk
  {
    public AttributeModifier modifier;

    public SkillAttributePerk(
      string id,
      string attributeId,
      float modifierBonus,
      string modifierDesc)
      : base(id, "", (Action<MinionResume>) null, (Action<MinionResume>) null, (Action<MinionResume>) (identity => { }))
    {
      Klei.AI.Attribute attribute = Db.Get().Attributes.Get(attributeId);
      this.modifier = new AttributeModifier(attributeId, modifierBonus, modifierDesc);
      this.Name = string.Format((string) UI.ROLES_SCREEN.PERKS.ATTRIBUTE_EFFECT_FMT, (object) this.modifier.GetFormattedString(), (object) attribute.Name);
      this.OnApply = (Action<MinionResume>) (identity =>
      {
        if (identity.GetAttributes().Get(this.modifier.AttributeId).Modifiers.FindIndex((Predicate<AttributeModifier>) (mod => mod == this.modifier)) != -1)
          return;
        identity.GetAttributes().Add(this.modifier);
      });
      this.OnRemove = (Action<MinionResume>) (identity => identity.GetAttributes().Remove(this.modifier));
    }
  }
}
