// Decompiled with JetBrains decompiler
// Type: Klei.AI.Sunburn
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;

namespace Klei.AI
{
  public class Sunburn : Sickness
  {
    public const string ID = "SunburnSickness";

    public Sunburn()
      : base("SunburnSickness", Sickness.SicknessType.Ailment, Sickness.Severity.Minor, 0.005f, new List<Sickness.InfectionVector>()
      {
        Sickness.InfectionVector.Exposure
      }, 1020f)
    {
      this.AddSicknessComponent((Sickness.SicknessComponent) new CommonSickEffectSickness());
      this.AddSicknessComponent((Sickness.SicknessComponent) new AttributeModifierSickness(new AttributeModifier[1]
      {
        new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0.0333333351f, (string) DUPLICANTS.DISEASES.SUNBURNSICKNESS.NAME)
      }));
      this.AddSicknessComponent((Sickness.SicknessComponent) new AnimatedSickness(new HashedString[3]
      {
        HashedString.op_Implicit("anim_idle_hot_kanim"),
        HashedString.op_Implicit("anim_loco_run_hot_kanim"),
        HashedString.op_Implicit("anim_loco_walk_hot_kanim")
      }, Db.Get().Expressions.SickFierySkin));
      this.AddSicknessComponent((Sickness.SicknessComponent) new PeriodicEmoteSickness(Db.Get().Emotes.Minion.Hot, 5f));
    }
  }
}
