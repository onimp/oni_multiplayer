// Decompiled with JetBrains decompiler
// Type: Database.PlantAttributes
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;

namespace Database
{
  public class PlantAttributes : ResourceSet<Attribute>
  {
    public Attribute WiltTempRangeMod;
    public Attribute YieldAmount;
    public Attribute HarvestTime;
    public Attribute DecorBonus;
    public Attribute MinLightLux;
    public Attribute FertilizerUsageMod;
    public Attribute MinRadiationThreshold;
    public Attribute MaxRadiationThreshold;

    public PlantAttributes(ResourceSet parent)
      : base(nameof (PlantAttributes), parent)
    {
      this.WiltTempRangeMod = this.Add(new Attribute(nameof (WiltTempRangeMod), false, Attribute.Display.Normal, false, 1f));
      this.WiltTempRangeMod.SetFormatter((IAttributeFormatter) new PercentAttributeFormatter());
      this.YieldAmount = this.Add(new Attribute(nameof (YieldAmount), false, Attribute.Display.Normal, false));
      this.YieldAmount.SetFormatter((IAttributeFormatter) new PercentAttributeFormatter());
      this.HarvestTime = this.Add(new Attribute(nameof (HarvestTime), false, Attribute.Display.Normal, false));
      this.HarvestTime.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.Time, GameUtil.TimeSlice.None));
      this.DecorBonus = this.Add(new Attribute(nameof (DecorBonus), false, Attribute.Display.Normal, false));
      this.DecorBonus.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
      this.MinLightLux = this.Add(new Attribute(nameof (MinLightLux), false, Attribute.Display.Normal, false));
      this.MinLightLux.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.Lux, GameUtil.TimeSlice.None));
      this.FertilizerUsageMod = this.Add(new Attribute(nameof (FertilizerUsageMod), false, Attribute.Display.Normal, false, 1f));
      this.FertilizerUsageMod.SetFormatter((IAttributeFormatter) new PercentAttributeFormatter());
      this.MinRadiationThreshold = this.Add(new Attribute(nameof (MinRadiationThreshold), false, Attribute.Display.Normal, false));
      this.MinRadiationThreshold.SetFormatter((IAttributeFormatter) new RadsPerCycleAttributeFormatter());
      this.MaxRadiationThreshold = this.Add(new Attribute(nameof (MaxRadiationThreshold), false, Attribute.Display.Normal, false));
      this.MaxRadiationThreshold.SetFormatter((IAttributeFormatter) new RadsPerCycleAttributeFormatter());
    }
  }
}
