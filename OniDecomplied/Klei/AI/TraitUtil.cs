// Decompiled with JetBrains decompiler
// Type: Klei.AI.TraitUtil
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klei.AI
{
  public class TraitUtil
  {
    public static System.Action CreateDisabledTaskTrait(
      string id,
      string name,
      string desc,
      string disabled_chore_group,
      bool is_valid_starter_trait)
    {
      return (System.Action) (() =>
      {
        ChoreGroup[] disabled_chore_groups = new ChoreGroup[1]
        {
          Db.Get().ChoreGroups.Get(disabled_chore_group)
        };
        Db.Get().CreateTrait(id, name, desc, (string) null, true, disabled_chore_groups, false, is_valid_starter_trait);
      });
    }

    public static System.Action CreateTrait(
      string id,
      string name,
      string desc,
      string attributeId,
      float delta,
      string[] chore_groups,
      bool positiveTrait = false)
    {
      return (System.Action) (() =>
      {
        List<ChoreGroup> choreGroupList = new List<ChoreGroup>();
        foreach (string choreGroup in chore_groups)
          choreGroupList.Add(Db.Get().ChoreGroups.Get(choreGroup));
        Db.Get().CreateTrait(id, name, desc, (string) null, true, choreGroupList.ToArray(), positiveTrait, true).Add(new AttributeModifier(attributeId, delta, name));
      });
    }

    public static System.Action CreateAttributeEffectTrait(
      string id,
      string name,
      string desc,
      string attributeId,
      float delta,
      string attributeId2,
      float delta2,
      bool positiveTrait = false)
    {
      return (System.Action) (() =>
      {
        Trait trait = Db.Get().CreateTrait(id, name, desc, (string) null, true, (ChoreGroup[]) null, positiveTrait, true);
        trait.Add(new AttributeModifier(attributeId, delta, name));
        trait.Add(new AttributeModifier(attributeId2, delta2, name));
      });
    }

    public static System.Action CreateAttributeEffectTrait(
      string id,
      string name,
      string desc,
      string attributeId,
      float delta,
      bool positiveTrait = false,
      Action<GameObject> on_add = null,
      bool is_valid_starter_trait = true)
    {
      return (System.Action) (() =>
      {
        Trait trait = Db.Get().CreateTrait(id, name, desc, (string) null, true, (ChoreGroup[]) null, positiveTrait, is_valid_starter_trait);
        trait.Add(new AttributeModifier(attributeId, delta, name));
        trait.OnAddTrait = on_add;
      });
    }

    public static System.Action CreateEffectModifierTrait(
      string id,
      string name,
      string desc,
      string[] ignoredEffects,
      bool positiveTrait = false)
    {
      return (System.Action) (() => Db.Get().CreateTrait(id, name, desc, (string) null, true, (ChoreGroup[]) null, positiveTrait, true).AddIgnoredEffects(ignoredEffects));
    }

    public static System.Action CreateNamedTrait(
      string id,
      string name,
      string desc,
      bool positiveTrait = false)
    {
      return (System.Action) (() => Db.Get().CreateTrait(id, name, desc, (string) null, true, (ChoreGroup[]) null, positiveTrait, true));
    }

    public static System.Action CreateTrait(
      string id,
      string name,
      string desc,
      Action<GameObject> on_add,
      ChoreGroup[] disabled_chore_groups = null,
      bool positiveTrait = false,
      Func<string> extendedDescFn = null)
    {
      return (System.Action) (() =>
      {
        Trait trait = Db.Get().CreateTrait(id, name, desc, (string) null, true, disabled_chore_groups, positiveTrait, true);
        trait.OnAddTrait = on_add;
        if (extendedDescFn == null)
          return;
        trait.ExtendedTooltip += extendedDescFn;
      });
    }

    public static System.Action CreateComponentTrait<T>(
      string id,
      string name,
      string desc,
      bool positiveTrait = false,
      Func<string> extendedDescFn = null)
      where T : KMonoBehaviour
    {
      return (System.Action) (() =>
      {
        Trait trait = Db.Get().CreateTrait(id, name, desc, (string) null, true, (ChoreGroup[]) null, positiveTrait, true);
        trait.OnAddTrait = (Action<GameObject>) (go => Util.FindOrAddUnityComponent<T>(go));
        if (extendedDescFn == null)
          return;
        trait.ExtendedTooltip += extendedDescFn;
      });
    }

    public static System.Action CreateSkillGrantingTrait(
      string id,
      string name,
      string desc,
      string skillId)
    {
      return (System.Action) (() =>
      {
        Trait trait = Db.Get().CreateTrait(id, name, desc, (string) null, true, (ChoreGroup[]) null, true, true);
        trait.TooltipCB = (Func<string>) (() => string.Format((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_DESC, (object) desc, (object) SkillWidget.SkillPerksString(Db.Get().Skills.Get(skillId))));
        trait.OnAddTrait = (Action<GameObject>) (go =>
        {
          MinionResume component = go.GetComponent<MinionResume>();
          if (!Object.op_Inequality((Object) component, (Object) null))
            return;
          component.GrantSkill(skillId);
        });
      });
    }

    public static string GetSkillGrantingTraitNameById(string id)
    {
      string grantingTraitNameById = "";
      StringEntry stringEntry;
      if (Strings.TryGet("STRINGS.DUPLICANTS.TRAITS.GRANTSKILL_" + id.ToUpper() + ".NAME", ref stringEntry))
        grantingTraitNameById = stringEntry.String;
      return grantingTraitNameById;
    }
  }
}
