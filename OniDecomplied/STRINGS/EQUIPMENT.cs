// Decompiled with JetBrains decompiler
// Type: STRINGS.EQUIPMENT
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace STRINGS
{
  public class EQUIPMENT
  {
    public class PREFABS
    {
      public class OXYGEN_MASK
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Oxygen Mask", nameof (OXYGEN_MASK));
        public static LocString DESC = (LocString) "Ensures my Duplicants can breathe easy... for a little while, anyways.";
        public static LocString EFFECT = (LocString) ("Supplies Duplicants with <style=\"oxygen\">Oxygen</style> in toxic and low breathability environments.\n\nMust be refilled with oxygen at an " + UI.FormatAsLink("Atmo Suit Dock", "SUITLOCKER") + " when depleted.");
        public static LocString RECIPE_DESC = (LocString) "Supplies Duplicants with <style=\"oxygen\">Oxygen</style> in toxic and low breathability environments.";
        public static LocString GENERICNAME = (LocString) "Suit";
        public static LocString WORN_NAME = (LocString) UI.FormatAsLink("Worn Oxygen Mask", nameof (OXYGEN_MASK));
        public static LocString WORN_DESC = (LocString) ("A worn out " + UI.FormatAsLink("Oxygen Mask", nameof (OXYGEN_MASK)) + ".\n\nMasks can be repaired at a " + UI.FormatAsLink("Crafting Station", "CRAFTINGTABLE") + ".");
      }

      public class ATMO_SUIT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Atmo Suit", nameof (ATMO_SUIT));
        public static LocString DESC = (LocString) "Ensures my Duplicants can breathe easy, anytime, anywhere.";
        public static LocString EFFECT = (LocString) ("Supplies Duplicants with <style=\"oxygen\">Oxygen</style> in toxic and low breathability environments.\n\nMust be refilled with oxygen at an " + UI.FormatAsLink("Atmo Suit Dock", "SUITLOCKER") + " when depleted.");
        public static LocString RECIPE_DESC = (LocString) "Supplies Duplicants with <style=\"oxygen\">Oxygen</style> in toxic and low breathability environments.";
        public static LocString GENERICNAME = (LocString) "Suit";
        public static LocString WORN_NAME = (LocString) UI.FormatAsLink("Worn Atmo Suit", nameof (ATMO_SUIT));
        public static LocString WORN_DESC = (LocString) ("A worn out " + UI.FormatAsLink("Atmo Suit", nameof (ATMO_SUIT)) + ".\n\nSuits can be repaired at an " + UI.FormatAsLink("Exosuit Forge", "SUITFABRICATOR") + ".");
        public static LocString REPAIR_WORN_RECIPE_NAME = (LocString) ("Repair" + (string) EQUIPMENT.PREFABS.ATMO_SUIT.NAME);
        public static LocString REPAIR_WORN_DESC = (LocString) ("Restore a " + UI.FormatAsLink("Worn Atmo Suit", nameof (ATMO_SUIT)) + " to working order.");
      }

      public class AQUA_SUIT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Aqua Suit", nameof (AQUA_SUIT));
        public static LocString DESC = (LocString) "Because breathing underwater is better than... not.";
        public static LocString EFFECT = (LocString) ("Supplies Duplicants with <style=\"oxygen\">Oxygen</style> in underwater environments.\n\nMust be refilled with " + UI.FormatAsLink("Oxygen", "OXYGEN") + " at an " + UI.FormatAsLink("Atmo Suit Dock", "SUITLOCKER") + " when depleted.");
        public static LocString RECIPE_DESC = (LocString) "Supplies Duplicants with <style=\"oxygen\">Oxygen</style> in underwater environments.";
        public static LocString WORN_NAME = (LocString) UI.FormatAsLink("Worn Lead Suit", nameof (AQUA_SUIT));
        public static LocString WORN_DESC = (LocString) ("A worn out " + UI.FormatAsLink("Aqua Suit", nameof (AQUA_SUIT)) + ".\n\nSuits can be repaired at a " + UI.FormatAsLink("Crafting Station", "CRAFTINGTABLE") + ".");
      }

      public class TEMPERATURE_SUIT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Thermo Suit", nameof (TEMPERATURE_SUIT));
        public static LocString DESC = (LocString) "Keeps my Duplicants cool in case things heat up.";
        public static LocString EFFECT = (LocString) "Provides insulation in regions with extreme <style=\"heat\">Temperatures</style>.\n\nMust be powered at a Thermo Suit Dock when depleted.";
        public static LocString RECIPE_DESC = (LocString) "Provides insulation in regions with extreme <style=\"heat\">Temperatures</style>.";
        public static LocString WORN_NAME = (LocString) UI.FormatAsLink("Worn Lead Suit", nameof (TEMPERATURE_SUIT));
        public static LocString WORN_DESC = (LocString) ("A worn out " + UI.FormatAsLink("Thermo Suit", nameof (TEMPERATURE_SUIT)) + ".\n\nSuits can be repaired at a " + UI.FormatAsLink("Crafting Station", "CRAFTINGTABLE") + ".");
      }

      public class JET_SUIT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Jet Suit", nameof (JET_SUIT));
        public static LocString DESC = (LocString) "Allows my Duplicants to take to the skies, for a time.";
        public static LocString EFFECT = (LocString) ("Supplies Duplicants with " + UI.FormatAsLink("Oxygen", "OXYGEN") + " in toxic and low breathability environments.\n\nMust be refilled with " + UI.FormatAsLink("Oxygen", "OXYGEN") + " and " + UI.FormatAsLink("Petroleum", "PETROLEUM") + " at a " + UI.FormatAsLink("Jet Suit Dock", "JETSUITLOCKER") + " when depleted.");
        public static LocString RECIPE_DESC = (LocString) ("Supplies Duplicants with " + UI.FormatAsLink("Oxygen", "OXYGEN") + " in toxic and low breathability environments.\n\nAllows Duplicant flight.");
        public static LocString GENERICNAME = (LocString) "Jet Suit";
        public static LocString TANK_EFFECT_NAME = (LocString) "Fuel Tank";
        public static LocString WORN_NAME = (LocString) UI.FormatAsLink("Worn Jet Suit", nameof (JET_SUIT));
        public static LocString WORN_DESC = (LocString) ("A worn out " + UI.FormatAsLink("Jet Suit", nameof (JET_SUIT)) + ".\n\nSuits can be repaired at an " + UI.FormatAsLink("Exosuit Forge", "SUITFABRICATOR") + ".");
      }

      public class LEAD_SUIT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Lead Suit", nameof (LEAD_SUIT));
        public static LocString DESC = (LocString) "Because exposure to radiation doesn't grant Duplicants superpowers.";
        public static LocString EFFECT = (LocString) ("Supplies Duplicants with " + UI.FormatAsLink("Oxygen", "OXYGEN") + " and protection in areas with " + UI.FormatAsLink("Radiation", "RADIATION") + ".\n\nMust be refilled with " + UI.FormatAsLink("Oxygen", "OXYGEN") + " at a " + UI.FormatAsLink("Lead Suit Dock", "LEADSUITLOCKER") + " when depleted.");
        public static LocString RECIPE_DESC = (LocString) ("Supplies Duplicants with " + UI.FormatAsLink("Oxygen", "OXYGEN") + " in toxic and low breathability environments.\n\nProtects Duplicants from " + UI.FormatAsLink("Radiation", "RADIATION") + ".");
        public static LocString GENERICNAME = (LocString) "Lead Suit";
        public static LocString BATTERY_EFFECT_NAME = (LocString) "Suit Battery";
        public static LocString SUIT_OUT_OF_BATTERIES = (LocString) "Suit Batteries Empty";
        public static LocString WORN_NAME = (LocString) UI.FormatAsLink("Worn Lead Suit", nameof (LEAD_SUIT));
        public static LocString WORN_DESC = (LocString) ("A worn out " + UI.FormatAsLink("Lead Suit", nameof (LEAD_SUIT)) + ".\n\nSuits can be repaired at an " + UI.FormatAsLink("Exosuit Forge", "SUITFABRICATOR") + ".");
      }

      public class COOL_VEST
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Cool Vest", nameof (COOL_VEST));
        public static LocString GENERICNAME = (LocString) "Clothing";
        public static LocString DESC = (LocString) "Don't sweat it!";
        public static LocString EFFECT = (LocString) "Protects the wearer from <style=\"heat\">Heat</style> by decreasing insulation.";
        public static LocString RECIPE_DESC = (LocString) "Protects the wearer from <style=\"heat\">Heat</style> by decreasing insulation";
      }

      public class WARM_VEST
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Warm Sweater", nameof (WARM_VEST));
        public static LocString GENERICNAME = (LocString) "Clothing";
        public static LocString DESC = (LocString) "Happiness is a warm Duplicant.";
        public static LocString EFFECT = (LocString) "Protects the wearer from <style=\"heat\">Cold</style> by increasing insulation.";
        public static LocString RECIPE_DESC = (LocString) "Protects the wearer from <style=\"heat\">Cold</style> by increasing insulation";
      }

      public class FUNKY_VEST
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Snazzy Suit", nameof (FUNKY_VEST));
        public static LocString GENERICNAME = (LocString) "Clothing";
        public static LocString DESC = (LocString) "This transforms my Duplicant into a walking beacon of charm and style.";
        public static LocString EFFECT = (LocString) ("Increases Decor in a small area effect around the wearer. Can be upgraded to " + UI.FormatAsLink("Primo Garb", "CUSTOMCLOTHING") + " at the " + UI.FormatAsLink("Clothing Refashionator", "CLOTHINGALTERATIONSTATION") + ".");
        public static LocString RECIPE_DESC = (LocString) ("Increases Decor in a small area effect around the wearer. Can be upgraded to " + UI.FormatAsLink("Primo Garb", "CUSTOMCLOTHING") + " at the " + UI.FormatAsLink("Clothing Refashionator", "CLOTHINGALTERATIONSTATION"));
      }

      public class CUSTOMCLOTHING
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Primo Garb", nameof (CUSTOMCLOTHING));
        public static LocString GENERICNAME = (LocString) "Clothing";
        public static LocString DESC = (LocString) "This transforms my Duplicant into a colony-inspiring fashion icon.";
        public static LocString EFFECT = (LocString) "Increases Decor in a small area effect around the wearer.";
        public static LocString RECIPE_DESC = (LocString) "Increases Decor in a small area effect around the wearer";

        public class FACADES
        {
          public static LocString CLUBSHIRT = (LocString) UI.FormatAsLink("Purple Polyester", nameof (CUSTOMCLOTHING));
          public static LocString CUMMERBUND = (LocString) UI.FormatAsLink("Classic Cummerbund", nameof (CUSTOMCLOTHING));
          public static LocString DECOR_02 = (LocString) UI.FormatAsLink("Snazzier Red Suit", nameof (CUSTOMCLOTHING));
          public static LocString DECOR_03 = (LocString) UI.FormatAsLink("Snazzier Blue Suit", nameof (CUSTOMCLOTHING));
          public static LocString DECOR_04 = (LocString) UI.FormatAsLink("Snazzier Green Suit", nameof (CUSTOMCLOTHING));
          public static LocString DECOR_05 = (LocString) UI.FormatAsLink("Snazzier Violet Suit", nameof (CUSTOMCLOTHING));
          public static LocString GAUDYSWEATER = (LocString) UI.FormatAsLink("Pompom Knit", nameof (CUSTOMCLOTHING));
          public static LocString LIMONE = (LocString) UI.FormatAsLink("Citrus Spandex", nameof (CUSTOMCLOTHING));
          public static LocString MONDRIAN = (LocString) UI.FormatAsLink("Cubist Knit", nameof (CUSTOMCLOTHING));
          public static LocString OVERALLS = (LocString) UI.FormatAsLink("Spiffy Overalls", nameof (CUSTOMCLOTHING));
          public static LocString TRIANGLES = (LocString) UI.FormatAsLink("Confetti Suit", nameof (CUSTOMCLOTHING));
          public static LocString WORKOUT = (LocString) UI.FormatAsLink("Pink Unitard", nameof (CUSTOMCLOTHING));
        }
      }

      public class CLOTHING_GLOVES
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Gloves", nameof (CLOTHING_GLOVES));
        public static LocString GENERICNAME = (LocString) "Clothing";
        public static LocString DESC = (LocString) "Testing desc for gloves skins";
        public static LocString EFFECT = (LocString) "Testing effect for gloves skins";
        public static LocString RECIPE_DESC = (LocString) "Testing recipe desc for gloves skins";

        public class FACADES
        {
          public class BASIC_BLUE_MIDDLE
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic Aqua Gloves", nameof (BASIC_BLUE_MIDDLE));
            public static LocString DESC = (LocString) "A good, solid pair of aqua-blue gloves that go with everything.";
          }

          public class BASIC_YELLOW
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic Yellow Gloves", nameof (BASIC_YELLOW));
            public static LocString DESC = (LocString) "A good, solid pair of yellow gloves that go with everything.";
          }

          public class BASIC_BLACK
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic Black Gloves", nameof (BASIC_BLACK));
            public static LocString DESC = (LocString) "A good, solid pair of black gloves that go with everything.";
          }

          public class BASIC_PINK_ORCHID
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic Bubblegum Gloves", nameof (BASIC_PINK_ORCHID));
            public static LocString DESC = (LocString) "A good, solid pair of bubblegum-pink gloves that go with everything.";
          }

          public class BASIC_GREEN
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic Green Gloves", nameof (BASIC_GREEN));
            public static LocString DESC = (LocString) "A good, solid pair of green gloves that go with everything.";
          }

          public class BASIC_ORANGE
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic Orange Gloves", nameof (BASIC_ORANGE));
            public static LocString DESC = (LocString) "A good, solid pair of orange gloves that go with everything.";
          }

          public class BASIC_PURPLE
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic Purple Gloves", nameof (BASIC_PURPLE));
            public static LocString DESC = (LocString) "A good, solid pair of purple gloves that go with everything.";
          }

          public class BASIC_RED
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic Red Gloves", nameof (BASIC_RED));
            public static LocString DESC = (LocString) "A good, solid pair of red gloves that go with everything.";
          }

          public class BASIC_WHITE
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic White Gloves", nameof (BASIC_WHITE));
            public static LocString DESC = (LocString) "A good, solid pair of white gloves that go with everything.";
          }
        }
      }

      public class CLOTHING_TOPS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Tops", nameof (CLOTHING_TOPS));
        public static LocString GENERICNAME = (LocString) "Clothing";
        public static LocString DESC = (LocString) "Testing desc for tops skins";
        public static LocString EFFECT = (LocString) "Testing effect for tops skins";
        public static LocString RECIPE_DESC = (LocString) "Testing recipe desc for tops skins";

        public class FACADES
        {
          public class BASIC_BLUE_MIDDLE
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic Aqua Shirt", nameof (BASIC_BLUE_MIDDLE));
            public static LocString DESC = (LocString) "A nice aqua-blue shirt that goes with everything.";
          }

          public class BASIC_BLACK
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic Black Shirt", nameof (BASIC_BLACK));
            public static LocString DESC = (LocString) "A nice black shirt that goes with everything.";
          }

          public class BASIC_PINK_ORCHID
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic Bubblegum Shirt", nameof (BASIC_PINK_ORCHID));
            public static LocString DESC = (LocString) "A nice bubblegum-pink shirt that goes with everything.";
          }

          public class BASIC_GREEN
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic Green Shirt", nameof (BASIC_GREEN));
            public static LocString DESC = (LocString) "A nice green shirt that goes with everything.";
          }

          public class BASIC_ORANGE
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic Orange Shirt", nameof (BASIC_ORANGE));
            public static LocString DESC = (LocString) "A nice orange shirt that goes with everything.";
          }

          public class BASIC_PURPLE
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic Purple Shirt", nameof (BASIC_PURPLE));
            public static LocString DESC = (LocString) "A nice purple shirt that goes with everything.";
          }

          public class BASIC_RED_BURNT
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic Red Shirt", nameof (BASIC_RED_BURNT));
            public static LocString DESC = (LocString) "A nice red shirt that goes with everything.";
          }

          public class BASIC_WHITE
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic White Shirt", nameof (BASIC_WHITE));
            public static LocString DESC = (LocString) "A nice white shirt that goes with everything.";
          }

          public class BASIC_YELLOW
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic Yellow Shirt", nameof (BASIC_YELLOW));
            public static LocString DESC = (LocString) "A nice yellow shirt that goes with everything.";
          }
        }
      }

      public class CLOTHING_BOTTOMS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Bottoms", nameof (CLOTHING_BOTTOMS));
        public static LocString GENERICNAME = (LocString) "Clothing";
        public static LocString DESC = (LocString) "Testing desc for bottoms skins";
        public static LocString EFFECT = (LocString) "Testing effect for bottoms skins";
        public static LocString RECIPE_DESC = (LocString) "Testing recipe desc for bottoms skins";

        public class FACADES
        {
          public class BASIC_BLUE_MIDDLE
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic Aqua Pants", nameof (BASIC_BLUE_MIDDLE));
            public static LocString DESC = (LocString) "A clean pair of aqua-blue pants that go with everything.";
          }

          public class BASIC_PINK_ORCHID
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic Bubblegum Pants", nameof (BASIC_PINK_ORCHID));
            public static LocString DESC = (LocString) "A clean pair of bubblegum-pink pants that go with everything.";
          }

          public class BASIC_GREEN
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic Green Pants", nameof (BASIC_GREEN));
            public static LocString DESC = (LocString) "A clean pair of green pants that go with everything.";
          }

          public class BASIC_ORANGE
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic Orange Pants", nameof (BASIC_ORANGE));
            public static LocString DESC = (LocString) "A clean pair of orange pants that go with everything.";
          }

          public class BASIC_PURPLE
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic Purple Pants", nameof (BASIC_PURPLE));
            public static LocString DESC = (LocString) "A clean pair of purple pants that go with everything.";
          }

          public class BASIC_RED
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic Red Pants", nameof (BASIC_RED));
            public static LocString DESC = (LocString) "A clean pair of red pants that go with everything.";
          }

          public class BASIC_WHITE
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic White Pants", nameof (BASIC_WHITE));
            public static LocString DESC = (LocString) "A clean pair of white pants that go with everything.";
          }

          public class BASIC_YELLOW
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic Yellow Pants", nameof (BASIC_YELLOW));
            public static LocString DESC = (LocString) "A clean pair of yellow pants that go with everything.";
          }

          public class BASIC_BLACK
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic Black Pants", nameof (BASIC_BLACK));
            public static LocString DESC = (LocString) "A clean pair of black pants that go with everything.";
          }
        }
      }

      public class CLOTHING_SHOES
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Shoes", nameof (CLOTHING_SHOES));
        public static LocString GENERICNAME = (LocString) "Clothing";
        public static LocString DESC = (LocString) "Testing desc for shoes skins";
        public static LocString EFFECT = (LocString) "Testing effect for shoes skins";
        public static LocString RECIPE_DESC = (LocString) "Testing recipe desc for shoes skins";

        public class FACADES
        {
          public class BASIC_BLUE_MIDDLE
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic Aqua Shoes", nameof (BASIC_BLUE_MIDDLE));
            public static LocString DESC = (LocString) "A fresh pair of aqua-blue shoes that go with everything.";
          }

          public class BASIC_PINK_ORCHID
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic Bubblegum Shoes", nameof (BASIC_PINK_ORCHID));
            public static LocString DESC = (LocString) "A fresh pair of bubblegum-pink shoes that go with everything.";
          }

          public class BASIC_GREEN
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic Green Shoes", nameof (BASIC_GREEN));
            public static LocString DESC = (LocString) "A fresh pair of green shoes that go with everything.";
          }

          public class BASIC_ORANGE
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic Orange Shoes", nameof (BASIC_ORANGE));
            public static LocString DESC = (LocString) "A fresh pair of orange shoes that go with everything.";
          }

          public class BASIC_PURPLE
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic Purple Shoes", nameof (BASIC_PURPLE));
            public static LocString DESC = (LocString) "A fresh pair of purple shoes that go with everything.";
          }

          public class BASIC_RED
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic Red Shoes", nameof (BASIC_RED));
            public static LocString DESC = (LocString) "A fresh pair of red shoes that go with everything.";
          }

          public class BASIC_WHITE
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic White Shoes", nameof (BASIC_WHITE));
            public static LocString DESC = (LocString) "A fresh pair of white shoes that go with everything.";
          }

          public class BASIC_YELLOW
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic Yellow Shoes", nameof (BASIC_YELLOW));
            public static LocString DESC = (LocString) "A fresh pair of yellow shoes that go with everything.";
          }

          public class BASIC_BLACK
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Basic Black Shoes", nameof (BASIC_BLACK));
            public static LocString DESC = (LocString) "A fresh pair of black shoes that go with everything.";
          }
        }
      }

      public class OXYGEN_TANK
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Oxygen Tank", nameof (OXYGEN_TANK));
        public static LocString GENERICNAME = (LocString) "Equipment";
        public static LocString DESC = (LocString) "It's like a to-go bag for your lungs.";
        public static LocString EFFECT = (LocString) "Allows Duplicants to breathe in hazardous environments.\n\nDoes not work when submerged in <style=\"liquid\">Liquid</style>.";
        public static LocString RECIPE_DESC = (LocString) "Allows Duplicants to breathe in hazardous environments.\n\nDoes not work when submerged in <style=\"liquid\">Liquid</style>";
      }

      public class OXYGEN_TANK_UNDERWATER
      {
        public static LocString NAME = (LocString) "Oxygen Rebreather";
        public static LocString GENERICNAME = (LocString) "Equipment";
        public static LocString DESC = (LocString) "";
        public static LocString EFFECT = (LocString) "Allows Duplicants to breathe while submerged in <style=\"liquid\">Liquid</style>.\n\nDoes not work outside of liquid.";
        public static LocString RECIPE_DESC = (LocString) "Allows Duplicants to breathe while submerged in <style=\"liquid\">Liquid</style>.\n\nDoes not work outside of liquid";
      }

      public class EQUIPPABLEBALLOON
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Balloon Friend", nameof (EQUIPPABLEBALLOON));
        public static LocString DESC = (LocString) "A floating friend to reassure my Duplicants they are so very, very clever.";
        public static LocString EFFECT = (LocString) ("Gives Duplicants a boost in brain function.\n\nSupplied by Duplicants with the Balloon Artist " + UI.FormatAsLink("Overjoyed", "MORALE") + " response.");
        public static LocString RECIPE_DESC = (LocString) ("Gives Duplicants a boost in brain function.\n\nSupplied by Duplicants with the Balloon Artist " + UI.FormatAsLink("Overjoyed", "MORALE") + " response");
        public static LocString GENERICNAME = (LocString) "Balloon Friend";
      }

      public class SLEEPCLINICPAJAMAS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Pajamas", "SLEEP_CLINIC_PAJAMAS");
        public static LocString GENERICNAME = (LocString) "Clothing";
        public static LocString DESC = (LocString) "A soft, fleecy ticket to dreamland.";
        public static LocString EFFECT = (LocString) ("Helps Duplicants fall asleep by reducing " + UI.FormatAsLink("Stamina", "STAMINA") + ".\n\nEnables the wearer to dream and produce " + UI.FormatAsLink("Dream Journals", "DREAMJOURNAL") + ".");
        public static LocString DESTROY_TOAST = (LocString) "Ripped Pajamas";
      }
    }
  }
}
