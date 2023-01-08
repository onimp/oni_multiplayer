// Decompiled with JetBrains decompiler
// Type: HoverTextHelper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class HoverTextHelper
{
  private static readonly string[] massStrings = new string[4];
  private static readonly string[] invalidCellMassStrings = new string[4]
  {
    "",
    "",
    "",
    ""
  };
  private static float cachedMass = -1f;
  private static Element cachedElement;

  public static void DestroyStatics()
  {
    HoverTextHelper.cachedElement = (Element) null;
    HoverTextHelper.cachedMass = -1f;
  }

  public static string[] MassStringsReadOnly(int cell)
  {
    if (!Grid.IsValidCell(cell))
      return HoverTextHelper.invalidCellMassStrings;
    Element element = Grid.Element[cell];
    float Mass = Grid.Mass[cell];
    if (element == HoverTextHelper.cachedElement && (double) Mass == (double) HoverTextHelper.cachedMass)
      return HoverTextHelper.massStrings;
    HoverTextHelper.cachedElement = element;
    HoverTextHelper.cachedMass = Mass;
    HoverTextHelper.massStrings[3] = " " + GameUtil.GetBreathableString(element, Mass);
    if (element.id == SimHashes.Vacuum)
    {
      HoverTextHelper.massStrings[0] = (string) UI.NA;
      HoverTextHelper.massStrings[1] = "";
      HoverTextHelper.massStrings[2] = "";
    }
    else if (element.id == SimHashes.Unobtanium)
    {
      HoverTextHelper.massStrings[0] = (string) UI.NEUTRONIUMMASS;
      HoverTextHelper.massStrings[1] = "";
      HoverTextHelper.massStrings[2] = "";
    }
    else
    {
      HoverTextHelper.massStrings[2] = (string) UI.UNITSUFFIXES.MASS.KILOGRAM;
      if ((double) Mass < 5.0)
      {
        Mass *= 1000f;
        HoverTextHelper.massStrings[2] = (string) UI.UNITSUFFIXES.MASS.GRAM;
      }
      if ((double) Mass < 5.0)
      {
        Mass *= 1000f;
        HoverTextHelper.massStrings[2] = (string) UI.UNITSUFFIXES.MASS.MILLIGRAM;
      }
      if ((double) Mass < 5.0)
      {
        float num = Mass * 1000f;
        HoverTextHelper.massStrings[2] = (string) UI.UNITSUFFIXES.MASS.MICROGRAM;
        Mass = Mathf.Floor(num);
      }
      int num1 = Mathf.FloorToInt(Mass);
      int num2 = Mathf.RoundToInt((float) (10.0 * ((double) Mass - (double) num1)));
      if (num2 == 10)
      {
        ++num1;
        num2 = 0;
      }
      HoverTextHelper.massStrings[0] = num1.ToString();
      HoverTextHelper.massStrings[1] = "." + num2.ToString();
    }
    return HoverTextHelper.massStrings;
  }
}
