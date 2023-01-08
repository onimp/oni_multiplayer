// Decompiled with JetBrains decompiler
// Type: Klei.AI.EffectInstance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Diagnostics;
using UnityEngine;

namespace Klei.AI
{
  [DebuggerDisplay("{effect.Id}")]
  public class EffectInstance : ModifierInstance<Effect>
  {
    public Effect effect;
    public bool shouldSave;
    public StatusItem statusItem;
    public float timeRemaining;
    public EmoteReactable reactable;

    public EffectInstance(GameObject game_object, Effect effect, bool should_save)
      : base(game_object, effect)
    {
      this.effect = effect;
      this.shouldSave = should_save;
      this.ConfigureStatusItem();
      if (effect.showInUI)
      {
        KSelectable component = this.gameObject.GetComponent<KSelectable>();
        if (!component.GetStatusItemGroup().HasStatusItem(this.statusItem))
          component.AddStatusItem(this.statusItem, (object) this);
      }
      if (effect.triggerFloatingText && Object.op_Inequality((Object) PopFXManager.Instance, (Object) null))
        PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, effect.Name, game_object.transform);
      if (effect.emote != null)
        this.RegisterEmote(effect.emote, effect.emoteCooldown);
      if (string.IsNullOrEmpty(effect.emoteAnim))
        return;
      this.RegisterEmote(effect.emoteAnim, effect.emoteCooldown);
    }

    public void RegisterEmote(string emoteAnim, float cooldown = -1f)
    {
      ReactionMonitor.Instance smi = this.gameObject.GetSMI<ReactionMonitor.Instance>();
      if (smi == null)
        return;
      bool isOneShot = (double) cooldown < 0.0;
      float globalCooldown = isOneShot ? 100000f : cooldown;
      EmoteReactable emoteReactable = (EmoteReactable) smi.AddSelfEmoteReactable(this.gameObject, this.effect.Name + "_Emote", emoteAnim, isOneShot, Db.Get().ChoreTypes.Emote, globalCooldown, maxInitialDelay: this.effect.maxInitialDelay, emotePreconditions: this.effect.emotePreconditions);
      if (emoteReactable == null)
        return;
      emoteReactable.InsertPrecondition(0, new Reactable.ReactablePrecondition(this.NotInATube));
      if (isOneShot)
        return;
      this.reactable = emoteReactable;
    }

    public void RegisterEmote(Emote emote, float cooldown = -1f)
    {
      ReactionMonitor.Instance smi = this.gameObject.GetSMI<ReactionMonitor.Instance>();
      if (smi == null)
        return;
      bool isOneShot = (double) cooldown < 0.0;
      float globalCooldown = isOneShot ? 100000f : cooldown;
      EmoteReactable emoteReactable = (EmoteReactable) smi.AddSelfEmoteReactable(this.gameObject, HashedString.op_Implicit(this.effect.Name + "_Emote"), emote, isOneShot, Db.Get().ChoreTypes.Emote, globalCooldown, maxInitialDelay: this.effect.maxInitialDelay, emotePreconditions: this.effect.emotePreconditions);
      if (emoteReactable == null)
        return;
      emoteReactable.InsertPrecondition(0, new Reactable.ReactablePrecondition(this.NotInATube));
      if (isOneShot)
        return;
      this.reactable = emoteReactable;
    }

    private bool NotInATube(GameObject go, Navigator.ActiveTransition transition) => transition.navGridTransition.start != NavType.Tube && transition.navGridTransition.end != NavType.Tube;

    public override void OnCleanUp()
    {
      if (this.statusItem != null)
      {
        this.gameObject.GetComponent<KSelectable>().RemoveStatusItem(this.statusItem);
        this.statusItem = (StatusItem) null;
      }
      if (this.reactable == null)
        return;
      this.reactable.Cleanup();
      this.reactable = (EmoteReactable) null;
    }

    public float GetTimeRemaining() => this.timeRemaining;

    public bool IsExpired() => (double) this.effect.duration > 0.0 && (double) this.timeRemaining <= 0.0;

    private void ConfigureStatusItem()
    {
      StatusItem.IconType icon_type = this.effect.isBad ? StatusItem.IconType.Exclamation : StatusItem.IconType.Info;
      if (!Util.IsNullOrWhiteSpace(this.effect.customIcon))
        icon_type = StatusItem.IconType.Custom;
      this.statusItem = new StatusItem(this.effect.Id, this.effect.Name, this.effect.description, this.effect.customIcon, icon_type, this.effect.isBad ? NotificationType.Bad : NotificationType.Neutral, false, OverlayModes.None.ID, 2, false);
      this.statusItem.resolveStringCallback = new Func<string, object, string>(this.ResolveString);
      this.statusItem.resolveTooltipCallback = new Func<string, object, string>(this.ResolveTooltip);
    }

    private string ResolveString(string str, object data) => str;

    private string ResolveTooltip(string str, object data)
    {
      string str1 = str;
      EffectInstance effectInstance = (EffectInstance) data;
      string tooltip = Effect.CreateTooltip(effectInstance.effect, false);
      if (!string.IsNullOrEmpty(tooltip))
        str1 = str1 + "\n\n" + tooltip;
      if ((double) effectInstance.effect.duration > 0.0)
        str1 = str1 + "\n\n" + string.Format((string) DUPLICANTS.MODIFIERS.TIME_REMAINING, (object) GameUtil.GetFormattedCycles(this.GetTimeRemaining()));
      return str1;
    }
  }
}
