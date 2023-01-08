// Decompiled with JetBrains decompiler
// Type: Database.Diseases
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace Database
{
  public class Diseases : ResourceSet<Klei.AI.Disease>
  {
    public Klei.AI.Disease FoodGerms;
    public Klei.AI.Disease SlimeGerms;
    public Klei.AI.Disease PollenGerms;
    public Klei.AI.Disease ZombieSpores;
    public Klei.AI.Disease RadiationPoisoning;

    public Diseases(ResourceSet parent, bool statsOnly = false)
      : base(nameof (Diseases), parent)
    {
      this.FoodGerms = this.Add((Klei.AI.Disease) new Klei.AI.FoodGerms(statsOnly));
      this.SlimeGerms = this.Add((Klei.AI.Disease) new Klei.AI.SlimeGerms(statsOnly));
      this.PollenGerms = this.Add((Klei.AI.Disease) new Klei.AI.PollenGerms(statsOnly));
      this.ZombieSpores = this.Add((Klei.AI.Disease) new Klei.AI.ZombieSpores(statsOnly));
      if (!DlcManager.FeatureRadiationEnabled())
        return;
      this.RadiationPoisoning = this.Add((Klei.AI.Disease) new Klei.AI.RadiationPoisoning(statsOnly));
    }

    public bool IsValidID(string id)
    {
      bool flag = false;
      foreach (Resource resource in this.resources)
      {
        if (resource.Id == id)
          flag = true;
      }
      return flag;
    }

    public byte GetIndex(int hash)
    {
      for (byte index = 0; (int) index < this.resources.Count; ++index)
      {
        Klei.AI.Disease resource = this.resources[(int) index];
        if (hash == resource.id.GetHashCode())
          return index;
      }
      return byte.MaxValue;
    }

    public byte GetIndex(HashedString id) => this.GetIndex(id.GetHashCode());
  }
}
