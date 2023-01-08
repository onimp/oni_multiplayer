// Decompiled with JetBrains decompiler
// Type: Klei.AI.HeatRash
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;

namespace Klei.AI
{
  public class HeatRash : Sickness
  {
    public const string ID = "HeatSickness";

    public HeatRash()
      : base("HeatSickness", Sickness.SicknessType.Ailment, Sickness.Severity.Minor, 0.005f, new List<Sickness.InfectionVector>()
      {
        Sickness.InfectionVector.Inhalation
      }, 180f)
    {
      this.AddSicknessComponent((Sickness.SicknessComponent) new CommonSickEffectSickness());
      this.AddSicknessComponent((Sickness.SicknessComponent) new AttributeModifierSickness(new AttributeModifier[4]
      {
        new AttributeModifier("Learning", -5f, (string) DUPLICANTS.DISEASES.HEATSICKNESS.NAME),
        new AttributeModifier("Machinery", -5f, (string) DUPLICANTS.DISEASES.HEATSICKNESS.NAME),
        new AttributeModifier("Construction", -5f, (string) DUPLICANTS.DISEASES.HEATSICKNESS.NAME),
        new AttributeModifier("Cooking", -5f, (string) DUPLICANTS.DISEASES.HEATSICKNESS.NAME)
      }));
      this.AddSicknessComponent((Sickness.SicknessComponent) new AnimatedSickness(new HashedString[3]
      {
        HashedString.op_Implicit("anim_idle_hot_kanim"),
        HashedString.op_Implicit("anim_loco_run_hot_kanim"),
        HashedString.op_Implicit("anim_loco_walk_hot_kanim")
      }, Db.Get().Expressions.SickFierySkin));
      this.AddSicknessComponent((Sickness.SicknessComponent) new PeriodicEmoteSickness(Db.Get().Emotes.Minion.Hot, 15f));
    }
  }
}
