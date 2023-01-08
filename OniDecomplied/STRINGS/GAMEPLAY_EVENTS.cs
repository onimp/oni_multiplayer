// Decompiled with JetBrains decompiler
// Type: STRINGS.GAMEPLAY_EVENTS
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace STRINGS
{
  public class GAMEPLAY_EVENTS
  {
    public static LocString CANCELED = (LocString) "{0} Canceled";
    public static LocString CANCELED_TOOLTIP = (LocString) "The {0} event was canceled";
    public static LocString DEFAULT_OPTION_NAME = (LocString) "Okay";
    public static LocString DEFAULT_OPTION_CONSIDER_NAME = (LocString) "Let me think about it";
    public static LocString CHAIN_EVENT_TOOLTIP = (LocString) "This event is a chain event.";
    public static LocString BONUS_EVENT_DESCRIPTION = (LocString) "{effects} for {duration}";

    public class LOCATIONS
    {
      public static LocString NONE_AVAILABLE = (LocString) "No location currently available";
      public static LocString SUN = (LocString) "The Sun";
      public static LocString SURFACE = (LocString) "Planetary Surface";
      public static LocString PRINTING_POD = BUILDINGS.PREFABS.HEADQUARTERS.NAME;
      public static LocString COLONY_WIDE = (LocString) "Colonywide";
    }

    public class TIMES
    {
      public static LocString NOW = (LocString) "Right now";
      public static LocString IN_CYCLES = (LocString) "In {0} cycles";
      public static LocString UNKNOWN = (LocString) "Sometime";
    }

    public class EVENT_TYPES
    {
      public class PARTY
      {
        public static LocString NAME = (LocString) "Party";
        public static LocString DESCRIPTION = (LocString) ("THIS EVENT IS NOT WORKING\n{host} is throwing a birthday party for {dupe}. Make sure there is an available " + (string) ROOMS.TYPES.REC_ROOM.NAME + " for the party.\n\nSocial events are good for Duplicant morale. Rejecting this party will hurt {host} and {dupe}'s fragile ego.");
        public static LocString CANCELED_NO_ROOM_TITLE = (LocString) "Party Canceled";
        public static LocString CANCELED_NO_ROOM_DESCRIPTION = (LocString) ("The party was canceled because no " + (string) ROOMS.TYPES.REC_ROOM.NAME + " was available.");
        public static LocString UNDERWAY = (LocString) "Party Happening";
        public static LocString UNDERWAY_TOOLTIP = (LocString) "There's a party going on";
        public static LocString ACCEPT_OPTION_NAME = (LocString) "Allow the party to happen";
        public static LocString ACCEPT_OPTION_DESC = (LocString) "Party goers will get {goodEffect}";
        public static LocString ACCEPT_OPTION_INVALID_TOOLTIP = (LocString) "A cake must be built for this event to take place.";
        public static LocString REJECT_OPTION_NAME = (LocString) "Cancel the party";
        public static LocString REJECT_OPTION_DESC = (LocString) "{host} and {dupe} gain {badEffect}";
      }

      public class ECLIPSE
      {
        public static LocString NAME = (LocString) "Eclipse";
        public static LocString DESCRIPTION = (LocString) "A celestial object has obscured the sunlight";
      }

      public class SOLAR_FLARE
      {
        public static LocString NAME = (LocString) "Solar Storm";
        public static LocString DESCRIPTION = (LocString) "A solar flare is headed this way";
      }

      public class CREATURE_SPAWN
      {
        public static LocString NAME = (LocString) "Critter Infestation";
        public static LocString DESCRIPTION = (LocString) "There was a massive influx of destructive critters";
      }

      public class SATELLITE_CRASH
      {
        public static LocString NAME = (LocString) "Satellite Crash";
        public static LocString DESCRIPTION = (LocString) "Mysterious space junk has crashed into the surface.\n\nIt may contain useful resources or information, but it may also be dangerous. Approach with caution.";
      }

      public class FOOD_FIGHT
      {
        public static LocString NAME = (LocString) "Food Fight";
        public static LocString DESCRIPTION = (LocString) "Duplicants will throw food at each other for recreation\n\nIt may be wasteful, but everyone who participates will benefit from a major stress reduction.";
        public static LocString UNDERWAY = (LocString) "Food Fight";
        public static LocString UNDERWAY_TOOLTIP = (LocString) "There is a food fight happening now";
        public static LocString ACCEPT_OPTION_NAME = (LocString) "Dupes start preparing to fight.";
        public static LocString ACCEPT_OPTION_DETAILS = (LocString) "(Plus morale)";
        public static LocString REJECT_OPTION_NAME = (LocString) "No food fight today";
        public static LocString REJECT_OPTION_DETAILS = (LocString) "Sadface";
      }

      public class PLANT_BLIGHT
      {
        public static LocString NAME = (LocString) "Plant Blight: {plant}";
        public static LocString DESCRIPTION = (LocString) "Our {plant} crops have been afflicted by a fungal sickness!\n\nI must get the Duplicants to uproot and compost the sick plants to save our farms.";
        public static LocString SUCCESS = (LocString) "Blight Managed: {plant}";
        public static LocString SUCCESS_TOOLTIP = (LocString) "All the blighted {plant} plants have been dealt with, halting the infection.";
      }

      public class CRYOFRIEND
      {
        public static LocString NAME = (LocString) "New Event: A Frozen Friend";
        public static LocString DESCRIPTION = (LocString) ("{dupe} has made an amazing discovery! A barely working " + (string) BUILDINGS.PREFABS.CRYOTANK.NAME + " has been uncovered containing a {friend} inside in a frozen state.\n\n{dupe} was successful in thawing {friend} and this encounter has filled both Duplicants with a sense of hope, something they will desperately need to keep their " + UI.FormatAsLink("Morale", "MORALE") + " up when facing the dangers ahead.");
        public static LocString BUTTON = (LocString) "{friend} is thawed!";
      }

      public class WARPWORLDREVEAL
      {
        public static LocString NAME = (LocString) "New Event: Personnel Teleporter";
        public static LocString DESCRIPTION = (LocString) ("I've discovered a functioning teleportation device with a pre-programmed destination.\n\nIt appears to go to another " + (string) UI.CLUSTERMAP.PLANETOID + ", and I'm fairly certain there's a return device on the other end.\n\nI could send a Duplicant through safely if I desired.");
        public static LocString BUTTON = (LocString) "See Destination";
      }

      public class ARTIFACT_REVEAL
      {
        public static LocString NAME = (LocString) "New Event: Artifact Analyzed";
        public static LocString DESCRIPTION = (LocString) "An artifact from a past civilization was analyzed.\n\n{desc}";
        public static LocString BUTTON = (LocString) "Close";
      }
    }

    public class BONUS
    {
      public class BONUSDREAM1
      {
        public static LocString NAME = (LocString) "Good Dream";
        public static LocString DESCRIPTION = (LocString) ("I've observed many improvements to {dupe}'s demeanor today. Analysis indicates unusually high amounts of dopamine in their system. There's a good chance this is due to an exceptionally good dream and analysis indicates that current sleeping conditions may have contributed to this occurrence.\n\nFurther improvements to sleeping conditions may have additional positive effects to the " + UI.FormatAsLink("Morale", "MORALE") + " of {dupe} and other Duplicants.");
        public static LocString CHAIN_TOOLTIP = (LocString) "Improving the living conditions of {dupe} will lead to more good dreams.";
      }

      public class BONUSDREAM2
      {
        public static LocString NAME = (LocString) "Really Good Dream";
        public static LocString DESCRIPTION = (LocString) ("{dupe} had another really good dream and the resulting release of dopamine has made this Duplicant energetic and full of possibilities! This is an encouraging byproduct of improving the living conditions of the colony.\n\nBased on these observations, building a better sleeping area for my Duplicants will have a similar effect on their " + UI.FormatAsLink("Morale", "MORALE") + ".");
      }

      public class BONUSDREAM3
      {
        public static LocString NAME = (LocString) "Great Dream";
        public static LocString DESCRIPTION = (LocString) "I have detected a distinct spring in {dupe}'s step today. There is a good chance that this Duplicant had another great dream last night. Such incidents are further indications that working on the care and comfort of the colony is not a waste of time.\n\nI do wonder though: What do Duplicants dream of?";
      }

      public class BONUSDREAM4
      {
        public static LocString NAME = (LocString) "Amazing Dream";
        public static LocString DESCRIPTION = (LocString) ("{dupe}'s dream last night must have been simply amazing! Their dopamine levels are at an all time high. Based on these results, it can be safely assumed that improving the living conditions of my Duplicants will reduce " + UI.FormatAsLink("Stress", "STRESS") + " and have similar positive effects on their well-being.\n\nObservations such as this are an integral and enjoyable part of science. When I see my Duplicants happy, I can't help but share in some of their joy.");
      }

      public class BONUSTOILET1
      {
        public static LocString NAME = (LocString) "Small Comforts";
        public static LocString DESCRIPTION = (LocString) ("{dupe} recently visited an Outhouse and appears to have appreciated the small comforts based on the marked increase to their " + UI.FormatAsLink("Morale", "MORALE") + ".\n\nHigh " + UI.FormatAsLink("Morale", "MORALE") + " has been linked to a better work ethic and greater enthusiasm for complex jobs, which are essential in building a successful new colony.");
      }

      public class BONUSTOILET2
      {
        public static LocString NAME = (LocString) "Greater Comforts";
        public static LocString DESCRIPTION = (LocString) ("{dupe} used a Lavatory and analysis shows a decided improvement to this Duplicant's " + UI.FormatAsLink("Morale", "MORALE") + ".\n\nAs my colony grows and expands, it's important not to ignore the benefits of giving my Duplicants a pleasant place to relieve themselves.");
      }

      public class BONUSTOILET3
      {
        public static LocString NAME = (LocString) "Small Luxury";
        public static LocString DESCRIPTION = (LocString) ("{dupe} visited a " + (string) ROOMS.TYPES.LATRINE.NAME + " and experienced luxury unlike they anything this Duplicant had previously experienced as analysis has revealed yet another boost to their " + UI.FormatAsLink("Morale", "MORALE") + ".\n\nIt is unclear whether this development is a result of increased hygiene or whether there is something else inherently about working plumbing which would improve " + UI.FormatAsLink("Morale", "MORALE") + " in this way. Further analysis is needed.");
      }

      public class BONUSTOILET4
      {
        public static LocString NAME = (LocString) "Greater Luxury";
        public static LocString DESCRIPTION = (LocString) ("{dupe} visited a Washroom and the experience has left this Duplicant with significantly improved " + UI.FormatAsLink("Morale", "MORALE") + ". Analysis indicates this improvement should continue for many cycles.\n\nThe relationship of my Duplicants and their surroundings is an interesting aspect of colony life. I should continue to watch future developments in this department closely.");
      }

      public class BONUSRESEARCH
      {
        public static LocString NAME = (LocString) "Inspired Learner";
        public static LocString DESCRIPTION = (LocString) ("Analysis indicates that the appearance of a " + UI.PRE_KEYWORD + "Research Station" + UI.PST_KEYWORD + " has inspired {dupe} and heightened their brain activity on a cellular level.\n\nBrain stimulation is important if my Duplicants are going to adapt and innovate in their increasingly harsh environment.");
      }

      public class BONUSDIGGING1
      {
        public static LocString NAME = (LocString) "Hot Diggity!";
        public static LocString DESCRIPTION = (LocString) "Some interesting data has revealed that {dupe} has had a marked increase in physical abilities, an increase that cannot entirely be attributed to the usual improvements that occur after regular physical activity.\n\nBased on previous observations this Duplicant's positive associations with digging appear to account for this additional physical boost.\n\nThis would mean the personal preferences of my Duplicants are directly correlated to how hard they work.  How interesting...";
      }

      public class BONUSSTORAGE
      {
        public static LocString NAME = (LocString) "Something in Store";
        public static LocString DESCRIPTION = (LocString) ("Data indicates that {dupe}'s activity in storing something in a Storage Bin has led to an increase in this Duplicant's physical strength as well as an overall improvement to their general demeanor.\n\nThere have been many studies connecting organization with an increase in well-being. It is possible this explains {dupe}'s " + UI.FormatAsLink("Morale", "MORALE") + " improvements.");
      }

      public class BONUSBUILDER
      {
        public static LocString NAME = (LocString) "Accomplished Builder";
        public static LocString DESCRIPTION = (LocString) "{dupe} has been hard at work building many structures crucial to the future of the colony. It seems this activity has improved this Duplicant's budding construction and mechanical skills beyond what my models predicted.\n\nWhether this increase in ability is due to them learning new skills or simply gaining self-confidence I cannot say, but this unexpected development is a welcome surprise development.";
      }

      public class BONUSOXYGEN
      {
        public static LocString NAME = (LocString) "Fresh Air";
        public static LocString DESCRIPTION = (LocString) "{dupe} is experiencing a sudden unexpected improvement to their physical prowess which appears to be a result of exposure to elevated levels of oxygen from passing by an Oxygen Diffuser.\n\nObservations such as this are important in documenting just how beneficial having access to oxygen is to my colony.";
      }

      public class BONUSALGAE
      {
        public static LocString NAME = (LocString) "Fresh Algae Smell";
        public static LocString DESCRIPTION = (LocString) "{dupe}'s recent proximity to an Algae Terrarium has left them feeling refreshed and exuberant and is correlated to an increase in their physical attributes. It is unclear whether these physical improvements came from the excess of oxygen or the invigorating smell of algae.\n\nIt's curious that I find myself nostalgic for the smell of algae growing in a lab. But how could this be...?";
      }

      public class BONUSGENERATOR
      {
        public static LocString NAME = (LocString) "Exercised";
        public static LocString DESCRIPTION = (LocString) ("{dupe} ran in a Manual Generator and the physical activity appears to have given this Duplicant increased strength and sense of well-being.\n\nWhile not the primary reason for building Manual Generators, I am very pleased to see my Duplicants reaping the " + UI.FormatAsLink("Stress", "STRESS") + " relieving benefits to physical activity.");
      }

      public class BONUSDOOR
      {
        public static LocString NAME = (LocString) "Open and Shut";
        public static LocString DESCRIPTION = (LocString) ("The act of closing a door has apparently lead to a decrease in the " + UI.FormatAsLink("Stress", "STRESS") + " levels of {dupe}, as well as decreased the exposure of this Duplicant to harmful " + UI.FormatAsLink("Germs", "GERMS") + ".\n\nWhile it may be more efficient to group all my Duplicants together in common sleeping quarters, it's important to remember the mental benefits to privacy and space to express their individuality.");
      }

      public class BONUSHITTHEBOOKS
      {
        public static LocString NAME = (LocString) "Hit the Books";
        public static LocString DESCRIPTION = (LocString) "{dupe}'s recent Research errand has resulted in a significant increase to this Duplicant's brain activity. The discovery of newly found knowledge has given {dupe} an invigorating jolt of excitement.\n\nI am all too familiar with this feeling.";
      }

      public class BONUSLITWORKSPACE
      {
        public static LocString NAME = (LocString) "Lit-erally Great";
        public static LocString DESCRIPTION = (LocString) "{dupe}'s recent time in a well lit area has greatly improved this Duplicant's ability to work with, and on, machinery.\n\nThis supports the prevailing theory that a well lit workspace has many benefits beyond just improving my Duplicant's ability to see.";
      }

      public class BONUSTALKER
      {
        public static LocString NAME = (LocString) "Big Small Talker";
        public static LocString DESCRIPTION = (LocString) ("{dupe}'s recent conversation with another Duplicant shows a correlation to improved serotonin and " + UI.FormatAsLink("Morale", "MORALE") + " levels in this Duplicant. It is very possible that small talk with a co-worker, however short and seemingly insignificant, will make my Duplicant's feel connected to the colony as a whole.\n\nAs the colony gets bigger and more sophisticated, I must ensure that the opportunity for such connections continue, for the good of my Duplicants' mental well being.");
      }
    }
  }
}
