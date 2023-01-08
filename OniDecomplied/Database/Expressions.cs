// Decompiled with JetBrains decompiler
// Type: Database.Expressions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace Database
{
  public class Expressions : ResourceSet<Expression>
  {
    public Expression Neutral;
    public Expression Happy;
    public Expression Uncomfortable;
    public Expression Cold;
    public Expression Hot;
    public Expression FullBladder;
    public Expression Tired;
    public Expression Hungry;
    public Expression Angry;
    public Expression Unhappy;
    public Expression RedAlert;
    public Expression Suffocate;
    public Expression RecoverBreath;
    public Expression Sick;
    public Expression SickSpores;
    public Expression Zombie;
    public Expression SickFierySkin;
    public Expression SickCold;
    public Expression Pollen;
    public Expression Relief;
    public Expression Productive;
    public Expression Determined;
    public Expression Sticker;
    public Expression Balloon;
    public Expression Sparkle;
    public Expression Music;
    public Expression Tickled;
    public Expression Radiation1;
    public Expression Radiation2;
    public Expression Radiation3;
    public Expression Radiation4;

    public Expressions(ResourceSet parent)
      : base(nameof (Expressions), parent)
    {
      Faces faces = Db.Get().Faces;
      this.Angry = new Expression(nameof (Angry), (ResourceSet) this, faces.Angry);
      this.Suffocate = new Expression(nameof (Suffocate), (ResourceSet) this, faces.Suffocate);
      this.RecoverBreath = new Expression(nameof (RecoverBreath), (ResourceSet) this, faces.Uncomfortable);
      this.RedAlert = new Expression(nameof (RedAlert), (ResourceSet) this, faces.Hot);
      this.Hungry = new Expression(nameof (Hungry), (ResourceSet) this, faces.Hungry);
      this.Radiation1 = new Expression(nameof (Radiation1), (ResourceSet) this, faces.Radiation1);
      this.Radiation2 = new Expression(nameof (Radiation2), (ResourceSet) this, faces.Radiation2);
      this.Radiation3 = new Expression(nameof (Radiation3), (ResourceSet) this, faces.Radiation3);
      this.Radiation4 = new Expression(nameof (Radiation4), (ResourceSet) this, faces.Radiation4);
      this.SickSpores = new Expression(nameof (SickSpores), (ResourceSet) this, faces.SickSpores);
      this.Zombie = new Expression(nameof (Zombie), (ResourceSet) this, faces.Zombie);
      this.SickFierySkin = new Expression(nameof (SickFierySkin), (ResourceSet) this, faces.SickFierySkin);
      this.SickCold = new Expression(nameof (SickCold), (ResourceSet) this, faces.SickCold);
      this.Pollen = new Expression(nameof (Pollen), (ResourceSet) this, faces.Pollen);
      this.Sick = new Expression(nameof (Sick), (ResourceSet) this, faces.Sick);
      this.Cold = new Expression(nameof (Cold), (ResourceSet) this, faces.Cold);
      this.Hot = new Expression(nameof (Hot), (ResourceSet) this, faces.Hot);
      this.FullBladder = new Expression(nameof (FullBladder), (ResourceSet) this, faces.Uncomfortable);
      this.Tired = new Expression(nameof (Tired), (ResourceSet) this, faces.Tired);
      this.Unhappy = new Expression(nameof (Unhappy), (ResourceSet) this, faces.Uncomfortable);
      this.Uncomfortable = new Expression(nameof (Uncomfortable), (ResourceSet) this, faces.Uncomfortable);
      this.Productive = new Expression(nameof (Productive), (ResourceSet) this, faces.Productive);
      this.Determined = new Expression(nameof (Determined), (ResourceSet) this, faces.Determined);
      this.Sticker = new Expression(nameof (Sticker), (ResourceSet) this, faces.Sticker);
      this.Balloon = new Expression(nameof (Sticker), (ResourceSet) this, faces.Balloon);
      this.Sparkle = new Expression(nameof (Sticker), (ResourceSet) this, faces.Sparkle);
      this.Music = new Expression(nameof (Music), (ResourceSet) this, faces.Music);
      this.Tickled = new Expression(nameof (Tickled), (ResourceSet) this, faces.Tickled);
      this.Happy = new Expression(nameof (Happy), (ResourceSet) this, faces.Happy);
      this.Relief = new Expression(nameof (Relief), (ResourceSet) this, faces.Happy);
      this.Neutral = new Expression(nameof (Neutral), (ResourceSet) this, faces.Neutral);
      for (int index = ((ResourceSet) this).Count - 1; index >= 0; --index)
        this.resources[index].priority = 100 * (((ResourceSet) this).Count - index);
    }
  }
}
