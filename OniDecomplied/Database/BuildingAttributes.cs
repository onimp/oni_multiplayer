// Decompiled with JetBrains decompiler
// Type: Database.BuildingAttributes
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;

namespace Database
{
  public class BuildingAttributes : ResourceSet<Attribute>
  {
    public Attribute Decor;
    public Attribute DecorRadius;
    public Attribute NoisePollution;
    public Attribute NoisePollutionRadius;
    public Attribute Hygiene;
    public Attribute Comfort;
    public Attribute OverheatTemperature;
    public Attribute FatalTemperature;

    public BuildingAttributes(ResourceSet parent)
      : base(nameof (BuildingAttributes), parent)
    {
      this.Decor = this.Add(new Attribute(nameof (Decor), true, Attribute.Display.General, false));
      this.DecorRadius = this.Add(new Attribute(nameof (DecorRadius), true, Attribute.Display.General, false));
      this.NoisePollution = this.Add(new Attribute(nameof (NoisePollution), true, Attribute.Display.General, false));
      this.NoisePollutionRadius = this.Add(new Attribute(nameof (NoisePollutionRadius), true, Attribute.Display.General, false));
      this.Hygiene = this.Add(new Attribute(nameof (Hygiene), true, Attribute.Display.General, false));
      this.Comfort = this.Add(new Attribute(nameof (Comfort), true, Attribute.Display.General, false));
      this.OverheatTemperature = this.Add(new Attribute(nameof (OverheatTemperature), true, Attribute.Display.General, false));
      this.OverheatTemperature.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.Temperature, GameUtil.TimeSlice.ModifyOnly));
      this.FatalTemperature = this.Add(new Attribute(nameof (FatalTemperature), true, Attribute.Display.General, false));
      this.FatalTemperature.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.Temperature, GameUtil.TimeSlice.ModifyOnly));
    }
  }
}
