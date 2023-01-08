// Decompiled with JetBrains decompiler
// Type: Database.Faces
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace Database
{
  public class Faces : ResourceSet<Face>
  {
    public Face Neutral;
    public Face Happy;
    public Face Uncomfortable;
    public Face Cold;
    public Face Hot;
    public Face Tired;
    public Face Sleep;
    public Face Hungry;
    public Face Angry;
    public Face Suffocate;
    public Face Dead;
    public Face Sick;
    public Face SickSpores;
    public Face Zombie;
    public Face SickFierySkin;
    public Face SickCold;
    public Face Pollen;
    public Face Productive;
    public Face Determined;
    public Face Sticker;
    public Face Balloon;
    public Face Sparkle;
    public Face Tickled;
    public Face Music;
    public Face Radiation1;
    public Face Radiation2;
    public Face Radiation3;
    public Face Radiation4;

    public Faces()
    {
      this.Neutral = this.Add(new Face(nameof (Neutral)));
      this.Happy = this.Add(new Face(nameof (Happy)));
      this.Uncomfortable = this.Add(new Face(nameof (Uncomfortable)));
      this.Cold = this.Add(new Face(nameof (Cold)));
      this.Hot = this.Add(new Face(nameof (Hot), "headfx_sweat"));
      this.Tired = this.Add(new Face(nameof (Tired)));
      this.Sleep = this.Add(new Face(nameof (Sleep)));
      this.Hungry = this.Add(new Face(nameof (Hungry)));
      this.Angry = this.Add(new Face(nameof (Angry)));
      this.Suffocate = this.Add(new Face(nameof (Suffocate)));
      this.Sick = this.Add(new Face(nameof (Sick), "headfx_sick"));
      this.SickSpores = this.Add(new Face("Spores", "headfx_spores"));
      this.Zombie = this.Add(new Face(nameof (Zombie)));
      this.SickFierySkin = this.Add(new Face("Fiery", "headfx_fiery"));
      this.SickCold = this.Add(new Face(nameof (SickCold), "headfx_sickcold"));
      this.Pollen = this.Add(new Face(nameof (Pollen), "headfx_pollen"));
      this.Dead = this.Add(new Face("Death"));
      this.Productive = this.Add(new Face(nameof (Productive)));
      this.Determined = this.Add(new Face(nameof (Determined)));
      this.Sticker = this.Add(new Face(nameof (Sticker)));
      this.Sparkle = this.Add(new Face(nameof (Sparkle)));
      this.Balloon = this.Add(new Face(nameof (Balloon)));
      this.Tickled = this.Add(new Face(nameof (Tickled)));
      this.Music = this.Add(new Face(nameof (Music)));
      this.Radiation1 = this.Add(new Face(nameof (Radiation1), "headfx_radiation1"));
      this.Radiation2 = this.Add(new Face(nameof (Radiation2), "headfx_radiation2"));
      this.Radiation3 = this.Add(new Face(nameof (Radiation3), "headfx_radiation3"));
      this.Radiation4 = this.Add(new Face(nameof (Radiation4), "headfx_radiation4"));
    }
  }
}
