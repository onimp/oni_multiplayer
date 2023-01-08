// Decompiled with JetBrains decompiler
// Type: Database.Urges
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace Database
{
  public class Urges : ResourceSet<Urge>
  {
    public Urge BeIncapacitated;
    public Urge Sleep;
    public Urge Narcolepsy;
    public Urge Eat;
    public Urge WashHands;
    public Urge Shower;
    public Urge Pee;
    public Urge MoveToQuarantine;
    public Urge HealCritical;
    public Urge RecoverBreath;
    public Urge Emote;
    public Urge Feed;
    public Urge Doctor;
    public Urge Flee;
    public Urge Heal;
    public Urge PacifyIdle;
    public Urge PacifyEat;
    public Urge PacifySleep;
    public Urge PacifyRelocate;
    public Urge RestDueToDisease;
    public Urge EmoteHighPriority;
    public Urge Aggression;
    public Urge MoveToSafety;
    public Urge WarmUp;
    public Urge CoolDown;
    public Urge LearnSkill;
    public Urge EmoteIdle;

    public Urges()
    {
      this.HealCritical = this.Add(new Urge(nameof (HealCritical)));
      this.BeIncapacitated = this.Add(new Urge(nameof (BeIncapacitated)));
      this.PacifyEat = this.Add(new Urge(nameof (PacifyEat)));
      this.PacifySleep = this.Add(new Urge(nameof (PacifySleep)));
      this.PacifyIdle = this.Add(new Urge(nameof (PacifyIdle)));
      this.EmoteHighPriority = this.Add(new Urge(nameof (EmoteHighPriority)));
      this.RecoverBreath = this.Add(new Urge(nameof (RecoverBreath)));
      this.Aggression = this.Add(new Urge(nameof (Aggression)));
      this.MoveToQuarantine = this.Add(new Urge(nameof (MoveToQuarantine)));
      this.WashHands = this.Add(new Urge(nameof (WashHands)));
      this.Shower = this.Add(new Urge(nameof (Shower)));
      this.Eat = this.Add(new Urge(nameof (Eat)));
      this.Pee = this.Add(new Urge(nameof (Pee)));
      this.RestDueToDisease = this.Add(new Urge(nameof (RestDueToDisease)));
      this.Sleep = this.Add(new Urge(nameof (Sleep)));
      this.Narcolepsy = this.Add(new Urge(nameof (Narcolepsy)));
      this.Doctor = this.Add(new Urge(nameof (Doctor)));
      this.Heal = this.Add(new Urge(nameof (Heal)));
      this.Feed = this.Add(new Urge(nameof (Feed)));
      this.PacifyRelocate = this.Add(new Urge(nameof (PacifyRelocate)));
      this.Emote = this.Add(new Urge(nameof (Emote)));
      this.MoveToSafety = this.Add(new Urge(nameof (MoveToSafety)));
      this.WarmUp = this.Add(new Urge(nameof (WarmUp)));
      this.CoolDown = this.Add(new Urge(nameof (CoolDown)));
      this.LearnSkill = this.Add(new Urge(nameof (LearnSkill)));
      this.EmoteIdle = this.Add(new Urge(nameof (EmoteIdle)));
    }
  }
}
