// Decompiled with JetBrains decompiler
// Type: Klei.AI.ZombieSickness
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;

namespace Klei.AI
{
  public class ZombieSickness : Sickness
  {
    public const string ID = "ZombieSickness";
    public const string RECOVERY_ID = "ZombieSicknessRecovery";
    public const int ATTRIBUTE_PENALTY = -10;

    public ZombieSickness()
      : base(nameof (ZombieSickness), Sickness.SicknessType.Pathogen, Sickness.Severity.Major, 0.00025f, new List<Sickness.InfectionVector>()
      {
        Sickness.InfectionVector.Inhalation,
        Sickness.InfectionVector.Contact
      }, 10800f, "ZombieSicknessRecovery")
    {
      this.AddSicknessComponent((Sickness.SicknessComponent) new CustomSickEffectSickness("spore_fx_kanim", "working_loop"));
      this.AddSicknessComponent((Sickness.SicknessComponent) new AnimatedSickness(new HashedString[2]
      {
        HashedString.op_Implicit("anim_idle_spores_kanim"),
        HashedString.op_Implicit("anim_loco_spore_kanim")
      }, Db.Get().Expressions.SickSpores));
      this.AddSicknessComponent((Sickness.SicknessComponent) new AttributeModifierSickness(new AttributeModifier[11]
      {
        new AttributeModifier(Db.Get().Attributes.Athletics.Id, -10f, (string) DUPLICANTS.DISEASES.ZOMBIESICKNESS.NAME),
        new AttributeModifier(Db.Get().Attributes.Strength.Id, -10f, (string) DUPLICANTS.DISEASES.ZOMBIESICKNESS.NAME),
        new AttributeModifier(Db.Get().Attributes.Digging.Id, -10f, (string) DUPLICANTS.DISEASES.ZOMBIESICKNESS.NAME),
        new AttributeModifier(Db.Get().Attributes.Construction.Id, -10f, (string) DUPLICANTS.DISEASES.ZOMBIESICKNESS.NAME),
        new AttributeModifier(Db.Get().Attributes.Art.Id, -10f, (string) DUPLICANTS.DISEASES.ZOMBIESICKNESS.NAME),
        new AttributeModifier(Db.Get().Attributes.Caring.Id, -10f, (string) DUPLICANTS.DISEASES.ZOMBIESICKNESS.NAME),
        new AttributeModifier(Db.Get().Attributes.Learning.Id, -10f, (string) DUPLICANTS.DISEASES.ZOMBIESICKNESS.NAME),
        new AttributeModifier(Db.Get().Attributes.Machinery.Id, -10f, (string) DUPLICANTS.DISEASES.ZOMBIESICKNESS.NAME),
        new AttributeModifier(Db.Get().Attributes.Cooking.Id, -10f, (string) DUPLICANTS.DISEASES.ZOMBIESICKNESS.NAME),
        new AttributeModifier(Db.Get().Attributes.Botanist.Id, -10f, (string) DUPLICANTS.DISEASES.ZOMBIESICKNESS.NAME),
        new AttributeModifier(Db.Get().Attributes.Ranching.Id, -10f, (string) DUPLICANTS.DISEASES.ZOMBIESICKNESS.NAME)
      }));
    }
  }
}
