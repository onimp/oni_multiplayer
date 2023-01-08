// Decompiled with JetBrains decompiler
// Type: Klei.AI.Effect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Klei.AI
{
  [DebuggerDisplay("{Id}")]
  public class Effect : Modifier
  {
    public float duration;
    public bool showInUI;
    public bool triggerFloatingText;
    public bool isBad;
    public string customIcon;
    public string emoteAnim;
    public Emote emote;
    public float emoteCooldown;
    public float maxInitialDelay;
    public List<Reactable.ReactablePrecondition> emotePreconditions;
    public string stompGroup;

    public Effect(
      string id,
      string name,
      string description,
      float duration,
      bool show_in_ui,
      bool trigger_floating_text,
      bool is_bad,
      Emote emote = null,
      float emote_cooldown = -1f,
      float max_initial_delay = 0.0f,
      string stompGroup = null,
      string custom_icon = "")
      : base(id, name, description)
    {
      this.duration = duration;
      this.showInUI = show_in_ui;
      this.triggerFloatingText = trigger_floating_text;
      this.isBad = is_bad;
      this.emote = emote;
      this.emoteCooldown = emote_cooldown;
      this.maxInitialDelay = max_initial_delay;
      this.stompGroup = stompGroup;
      this.customIcon = custom_icon;
    }

    public Effect(
      string id,
      string name,
      string description,
      float duration,
      bool show_in_ui,
      bool trigger_floating_text,
      bool is_bad,
      string emoteAnim,
      float emote_cooldown = -1f,
      string stompGroup = null,
      string custom_icon = "")
      : base(id, name, description)
    {
      this.duration = duration;
      this.showInUI = show_in_ui;
      this.triggerFloatingText = trigger_floating_text;
      this.isBad = is_bad;
      this.emoteAnim = emoteAnim;
      this.emoteCooldown = emote_cooldown;
      this.stompGroup = stompGroup;
      this.customIcon = custom_icon;
    }

    public override void AddTo(Attributes attributes) => base.AddTo(attributes);

    public override void RemoveFrom(Attributes attributes) => base.RemoveFrom(attributes);

    public void SetEmote(Emote emote, float emoteCooldown = -1f)
    {
      this.emote = emote;
      this.emoteCooldown = emoteCooldown;
    }

    public void AddEmotePrecondition(Reactable.ReactablePrecondition precon)
    {
      if (this.emotePreconditions == null)
        this.emotePreconditions = new List<Reactable.ReactablePrecondition>();
      this.emotePreconditions.Add(precon);
    }

    public static string CreateTooltip(
      Effect effect,
      bool showDuration,
      string linePrefix = "\n    • ",
      bool showHeader = true)
    {
      string tooltip = showHeader ? DUPLICANTS.MODIFIERS.EFFECT_HEADER.text : "";
      foreach (AttributeModifier selfModifier in effect.SelfModifiers)
      {
        Attribute attribute = Db.Get().Attributes.TryGet(selfModifier.AttributeId) ?? Db.Get().CritterAttributes.TryGet(selfModifier.AttributeId);
        if (attribute != null && attribute.ShowInUI != Attribute.Display.Never)
          tooltip = tooltip + linePrefix + string.Format((string) DUPLICANTS.MODIFIERS.MODIFIER_FORMAT, (object) attribute.Name, (object) selfModifier.GetFormattedString());
      }
      StringEntry stringEntry;
      if (Strings.TryGet("STRINGS.DUPLICANTS.MODIFIERS." + effect.Id.ToUpper() + ".ADDITIONAL_EFFECTS", ref stringEntry))
        tooltip = tooltip + linePrefix + StringEntry.op_Implicit(stringEntry);
      if (showDuration && (double) effect.duration > 0.0)
        tooltip = tooltip + "\n" + string.Format((string) DUPLICANTS.MODIFIERS.TIME_TOTAL, (object) GameUtil.GetFormattedCycles(effect.duration));
      return tooltip;
    }

    public static string CreateFullTooltip(Effect effect, bool showDuration) => effect.Name + "\n\n" + effect.description + "\n\n" + Effect.CreateTooltip(effect, showDuration);

    public static void AddModifierDescriptions(
      GameObject parent,
      List<Descriptor> descs,
      string effect_id,
      bool increase_indent = false)
    {
      foreach (AttributeModifier selfModifier in Db.Get().effects.Get(effect_id).SelfModifiers)
      {
        Descriptor descriptor;
        // ISSUE: explicit constructor call
        ((Descriptor) ref descriptor).\u002Ector(StringEntry.op_Implicit(Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + selfModifier.AttributeId.ToUpper() + ".NAME")) + ": " + selfModifier.GetFormattedString(), "", (Descriptor.DescriptorType) 1, false);
        if (increase_indent)
          ((Descriptor) ref descriptor).IncreaseIndent();
        descs.Add(descriptor);
      }
    }
  }
}
