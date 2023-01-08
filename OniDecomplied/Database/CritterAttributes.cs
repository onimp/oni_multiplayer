// Decompiled with JetBrains decompiler
// Type: Database.CritterAttributes
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;

namespace Database
{
  public class CritterAttributes : ResourceSet<Attribute>
  {
    public Attribute Happiness;
    public Attribute Metabolism;

    public CritterAttributes(ResourceSet parent)
      : base(nameof (CritterAttributes), parent)
    {
      this.Happiness = this.Add(new Attribute(nameof (Happiness), StringEntry.op_Implicit(Strings.Get("STRINGS.CREATURES.STATS.HAPPINESS.NAME")), "", StringEntry.op_Implicit(Strings.Get("STRINGS.CREATURES.STATS.HAPPINESS.TOOLTIP")), 0.0f, Attribute.Display.General, false, "ui_icon_happiness"));
      this.Happiness.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
      this.Metabolism = this.Add(new Attribute(nameof (Metabolism), false, Attribute.Display.Details, false));
      this.Metabolism.SetFormatter((IAttributeFormatter) new ToPercentAttributeFormatter(100f));
    }
  }
}
