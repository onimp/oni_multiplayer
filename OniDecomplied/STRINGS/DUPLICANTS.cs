// Decompiled with JetBrains decompiler
// Type: STRINGS.DUPLICANTS
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace STRINGS
{
  public class DUPLICANTS
  {
    public static LocString RACE_PREFIX = (LocString) "Species: {0}";
    public static LocString RACE = (LocString) "Duplicant";
    public static LocString NAMETITLE = (LocString) "Name: ";
    public static LocString GENDERTITLE = (LocString) "Gender: ";
    public static LocString ARRIVALTIME = (LocString) "Age: ";
    public static LocString ARRIVALTIME_TOOLTIP = (LocString) "This {1} was printed on <b>Cycle {0}</b>";
    public static LocString DESC_TOOLTIP = (LocString) "About {0}s";

    public class GENDER
    {
      public class MALE
      {
        public static LocString NAME = (LocString) "M";

        public class PLURALS
        {
          public static LocString ONE = (LocString) "he";
          public static LocString TWO = (LocString) "his";
        }
      }

      public class FEMALE
      {
        public static LocString NAME = (LocString) "F";

        public class PLURALS
        {
          public static LocString ONE = (LocString) "she";
          public static LocString TWO = (LocString) "her";
        }
      }

      public class NB
      {
        public static LocString NAME = (LocString) "X";

        public class PLURALS
        {
          public static LocString ONE = (LocString) "they";
          public static LocString TWO = (LocString) "their";
        }
      }
    }

    public class STATS
    {
      public class SUBJECTS
      {
        public static LocString DUPLICANT = (LocString) "Duplicant";
        public static LocString DUPLICANT_POSSESSIVE = (LocString) "Duplicant's";
        public static LocString DUPLICANT_PLURAL = (LocString) "Duplicants";
        public static LocString CREATURE = (LocString) "critter";
        public static LocString CREATURE_POSSESSIVE = (LocString) "critter's";
        public static LocString CREATURE_PLURAL = (LocString) "critters";
        public static LocString PLANT = (LocString) "plant";
        public static LocString PLANT_POSESSIVE = (LocString) "plant's";
        public static LocString PLANT_PLURAL = (LocString) "plants";
      }

      public class BREATH
      {
        public static LocString NAME = (LocString) "Breath";
        public static LocString TOOLTIP = (LocString) ("A Duplicant with zero remaining " + UI.PRE_KEYWORD + "Breath" + UI.PST_KEYWORD + " will begin suffocating");
      }

      public class STAMINA
      {
        public static LocString NAME = (LocString) "Stamina";
        public static LocString TOOLTIP = (LocString) ("Duplicants will pass out from fatigue when " + UI.PRE_KEYWORD + "Stamina" + UI.PST_KEYWORD + " reaches zero");
      }

      public class CALORIES
      {
        public static LocString NAME = (LocString) "Calories";
        public static LocString TOOLTIP = (LocString) "This {1} can burn <b>{0}</b> before starving";
      }

      public class TEMPERATURE
      {
        public static LocString NAME = (LocString) "Body Temperature";
        public static LocString TOOLTIP = (LocString) ("A healthy Duplicant's " + UI.PRE_KEYWORD + "Body Temperature" + UI.PST_KEYWORD + " is <b>{1}</b>");
        public static LocString TOOLTIP_DOMESTICATEDCRITTER = (LocString) ("This critter's " + UI.PRE_KEYWORD + "Body Temperature" + UI.PST_KEYWORD + " is <b>{1}</b>");
      }

      public class EXTERNALTEMPERATURE
      {
        public static LocString NAME = (LocString) "External Temperature";
        public static LocString TOOLTIP = (LocString) "This Duplicant's environment is <b>{0}</b>";
      }

      public class DECOR
      {
        public static LocString NAME = (LocString) "Decor";
        public static LocString TOOLTIP = (LocString) ("Duplicants become stressed in areas with " + UI.PRE_KEYWORD + "Decor" + UI.PST_KEYWORD + " lower than their expectations\n\nOpen the " + UI.FormatAsOverlay("Decor Overlay", (Action) 126) + " to view current " + UI.PRE_KEYWORD + "Decor" + UI.PST_KEYWORD + " values");
        public static LocString TOOLTIP_CURRENT = (LocString) "\n\nCurrent Environmental Decor: <b>{0}</b>";
        public static LocString TOOLTIP_AVERAGE_TODAY = (LocString) "\nAverage Decor This Cycle: <b>{0}</b>";
        public static LocString TOOLTIP_AVERAGE_YESTERDAY = (LocString) "\nAverage Decor Last Cycle: <b>{0}</b>";
      }

      public class STRESS
      {
        public static LocString NAME = (LocString) "Stress";
        public static LocString TOOLTIP = (LocString) ("Duplicants exhibit their Stress Reactions at one hundred percent " + UI.PRE_KEYWORD + "Stress" + UI.PST_KEYWORD);
      }

      public class RADIATIONBALANCE
      {
        public static LocString NAME = (LocString) "Absorbed Rad Dose";
        public static LocString TOOLTIP = (LocString) ("Duplicants accumulate Rads in areas with " + UI.PRE_KEYWORD + "Radiation" + UI.PST_KEYWORD + " and recover when using the toilet\n\nOpen the " + UI.FormatAsOverlay("Radiation Overlay", (Action) 133) + " to view current " + UI.PRE_KEYWORD + "Rad" + UI.PST_KEYWORD + " readings");
        public static LocString TOOLTIP_CURRENT_BALANCE = (LocString) ("Duplicants accumulate Rads in areas with " + UI.PRE_KEYWORD + "Radiation" + UI.PST_KEYWORD + " and recover when using the toilet\n\nOpen the " + UI.FormatAsOverlay("Radiation Overlay", (Action) 133) + " to view current " + UI.PRE_KEYWORD + "Rad" + UI.PST_KEYWORD + " readings");
        public static LocString CURRENT_EXPOSURE = (LocString) "Current Exposure: {0}/cycle";
        public static LocString CURRENT_REJUVENATION = (LocString) "Current Rejuvenation: {0}/cycle";
      }

      public class BLADDER
      {
        public static LocString NAME = (LocString) "Bladder";
        public static LocString TOOLTIP = (LocString) ("Duplicants make \"messes\" if no toilets are available at one hundred percent " + UI.PRE_KEYWORD + "Bladder" + UI.PST_KEYWORD);
      }

      public class HITPOINTS
      {
        public static LocString NAME = (LocString) "Health";
        public static LocString TOOLTIP = (LocString) ("When Duplicants reach zero " + UI.PRE_KEYWORD + "Health" + UI.PST_KEYWORD + " they become incapacitated and require rescuing\n\nWhen critters reach zero " + UI.PRE_KEYWORD + "Health" + UI.PST_KEYWORD + ", they will die immediately");
      }

      public class SKIN_THICKNESS
      {
        public static LocString NAME = (LocString) "Skin Thickness";
      }

      public class SKIN_DURABILITY
      {
        public static LocString NAME = (LocString) "Skin Durability";
      }

      public class DISEASERECOVERYTIME
      {
        public static LocString NAME = (LocString) "Disease Recovery";
      }

      public class TRUNKHEALTH
      {
        public static LocString NAME = (LocString) "Trunk Health";
        public static LocString TOOLTIP = (LocString) "Tree branches will die if they do not have a healthy trunk to grow from";
      }
    }

    public class DEATHS
    {
      public class GENERIC
      {
        public static LocString NAME = (LocString) "Death";
        public static LocString DESCRIPTION = (LocString) "{Target} has died.";
      }

      public class FROZEN
      {
        public static LocString NAME = (LocString) "Frozen";
        public static LocString DESCRIPTION = (LocString) "{Target} has frozen to death.";
      }

      public class SUFFOCATION
      {
        public static LocString NAME = (LocString) "Suffocation";
        public static LocString DESCRIPTION = (LocString) "{Target} has suffocated to death.";
      }

      public class STARVATION
      {
        public static LocString NAME = (LocString) "Starvation";
        public static LocString DESCRIPTION = (LocString) "{Target} has starved to death.";
      }

      public class OVERHEATING
      {
        public static LocString NAME = (LocString) "Overheated";
        public static LocString DESCRIPTION = (LocString) "{Target} overheated to death.";
      }

      public class DROWNED
      {
        public static LocString NAME = (LocString) "Drowned";
        public static LocString DESCRIPTION = (LocString) "{Target} has drowned.";
      }

      public class EXPLOSION
      {
        public static LocString NAME = (LocString) "Explosion";
        public static LocString DESCRIPTION = (LocString) "{Target} has died in an explosion.";
      }

      public class COMBAT
      {
        public static LocString NAME = (LocString) "Slain";
        public static LocString DESCRIPTION = (LocString) "{Target} succumbed to their wounds after being incapacitated.";
      }

      public class FATALDISEASE
      {
        public static LocString NAME = (LocString) "Succumbed to Disease";
        public static LocString DESCRIPTION = (LocString) "{Target} has died of a fatal illness.";
      }

      public class RADIATION
      {
        public static LocString NAME = (LocString) "Irradiated";
        public static LocString DESCRIPTION = (LocString) "{Target} perished from excessive radiation exposure.";
      }

      public class HITBYHIGHENERGYPARTICLE
      {
        public static LocString NAME = (LocString) "Struck by Radbolt";
        public static LocString DESCRIPTION = (LocString) ("{Target} was struck by a radioactive " + UI.PRE_KEYWORD + "Radbolt" + UI.PST_KEYWORD + " and perished.");
      }
    }

    public class CHORES
    {
      public static LocString NOT_EXISTING_TASK = (LocString) "Not Existing";
      public static LocString IS_DEAD_TASK = (LocString) "Dead";

      public class THINKING
      {
        public static LocString NAME = (LocString) "Ponder";
        public static LocString STATUS = (LocString) "Pondering";
        public static LocString TOOLTIP = (LocString) "This Duplicant is mulling over what they should do next";
      }

      public class ASTRONAUT
      {
        public static LocString NAME = (LocString) "Space Mission";
        public static LocString STATUS = (LocString) "On space mission";
        public static LocString TOOLTIP = (LocString) "This Duplicant is exploring the vast universe";
      }

      public class DIE
      {
        public static LocString NAME = (LocString) "Die";
        public static LocString STATUS = (LocString) "Dying";
        public static LocString TOOLTIP = (LocString) "Fare thee well, brave soul";
      }

      public class ENTOMBED
      {
        public static LocString NAME = (LocString) "Entombed";
        public static LocString STATUS = (LocString) "Entombed";
        public static LocString TOOLTIP = (LocString) "Entombed Duplicants are at risk of suffocating and must be dug out by others in the colony";
      }

      public class BEINCAPACITATED
      {
        public static LocString NAME = (LocString) "Incapacitated";
        public static LocString STATUS = (LocString) "Dying";
        public static LocString TOOLTIP = (LocString) "This Duplicant will die soon if they do not receive assistance";
      }

      public class GENESHUFFLE
      {
        public static LocString NAME = (LocString) "Use Neural Vacillator";
        public static LocString STATUS = (LocString) "Using Neural Vacillator";
        public static LocString TOOLTIP = (LocString) "This Duplicant is being experimented on!";
      }

      public class MIGRATE
      {
        public static LocString NAME = (LocString) "Use Teleporter";
        public static LocString STATUS = (LocString) "Using Teleporter";
        public static LocString TOOLTIP = (LocString) "This Duplicant's molecules are hurtling through the air!";
      }

      public class DEBUGGOTO
      {
        public static LocString NAME = (LocString) "DebugGoTo";
        public static LocString STATUS = (LocString) "DebugGoTo";
      }

      public class DISINFECT
      {
        public static LocString NAME = (LocString) "Disinfect";
        public static LocString STATUS = (LocString) "Going to disinfect";
        public static LocString TOOLTIP = (LocString) ("Buildings can be disinfected to remove contagious " + UI.PRE_KEYWORD + "Germs" + UI.PST_KEYWORD + " from their surface");
      }

      public class EQUIPPINGSUIT
      {
        public static LocString NAME = (LocString) "Equip Exosuit";
        public static LocString STATUS = (LocString) "Equipping exosuit";
        public static LocString TOOLTIP = (LocString) "This Duplicant is putting on protective gear";
      }

      public class STRESSIDLE
      {
        public static LocString NAME = (LocString) "Antsy";
        public static LocString STATUS = (LocString) "Antsy";
        public static LocString TOOLTIP = (LocString) "This Duplicant is a workaholic and gets stressed when they have nothing to do";
      }

      public class MOVETO
      {
        public static LocString NAME = (LocString) "Move to";
        public static LocString STATUS = (LocString) "Moving to location";
        public static LocString TOOLTIP = (LocString) "This Duplicant was manually directed to move to a specific location";
      }

      public class ROCKETENTEREXIT
      {
        public static LocString NAME = (LocString) "Rocket Recrewing";
        public static LocString STATUS = (LocString) "Recrewing Rocket";
        public static LocString TOOLTIP = (LocString) "This Duplicant is getting into (or out of) their assigned rocket";
      }

      public class DROPUNUSEDINVENTORY
      {
        public static LocString NAME = (LocString) "Drop Inventory";
        public static LocString STATUS = (LocString) "Dropping unused inventory";
        public static LocString TOOLTIP = (LocString) "This Duplicant is dropping carried items they no longer need";
      }

      public class PEE
      {
        public static LocString NAME = (LocString) "Relieve Self";
        public static LocString STATUS = (LocString) "Relieving self";
        public static LocString TOOLTIP = (LocString) "This Duplicant didn't find a toilet in time. Oops";
      }

      public class BREAK_PEE
      {
        public static LocString NAME = (LocString) "Downtime: Use Toilet";
        public static LocString STATUS = (LocString) "Downtime: Going to use toilet";
        public static LocString TOOLTIP = (LocString) ("This Duplicant has scheduled " + UI.PRE_KEYWORD + "Downtime" + UI.PST_KEYWORD + " and is using their break to go to the toilet\n\nDuplicants have to use the toilet at least once per day");
      }

      public class STRESSVOMIT
      {
        public static LocString NAME = (LocString) "Stress Vomit";
        public static LocString STATUS = (LocString) "Stress vomiting";
        public static LocString TOOLTIP = (LocString) ("Some people deal with " + UI.PRE_KEYWORD + "Stress" + UI.PST_KEYWORD + " better than others");
      }

      public class UGLY_CRY
      {
        public static LocString NAME = (LocString) "Ugly Cry";
        public static LocString STATUS = (LocString) "Ugly crying";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is having a healthy cry to alleviate their " + UI.PRE_KEYWORD + "Stress" + UI.PST_KEYWORD);
      }

      public class BINGE_EAT
      {
        public static LocString NAME = (LocString) "Binge Eat";
        public static LocString STATUS = (LocString) "Binge eating";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is attempting to eat their emotions due to " + UI.PRE_KEYWORD + "Stress" + UI.PST_KEYWORD);
      }

      public class BANSHEE_WAIL
      {
        public static LocString NAME = (LocString) "Banshee Wail";
        public static LocString STATUS = (LocString) "Wailing";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is emitting ear-piercing shrieks to relieve pent-up " + UI.PRE_KEYWORD + "Stress" + UI.PST_KEYWORD);
      }

      public class EMOTEHIGHPRIORITY
      {
        public static LocString NAME = (LocString) "Express Themselves";
        public static LocString STATUS = (LocString) "Expressing themselves";
        public static LocString TOOLTIP = (LocString) "This Duplicant needs a moment to express their feelings, then they'll be on their way";
      }

      public class HUG
      {
        public static LocString NAME = (LocString) "Hug";
        public static LocString STATUS = (LocString) "Hugging";
        public static LocString TOOLTIP = (LocString) "This Duplicant is enjoying a big warm hug";
      }

      public class FLEE
      {
        public static LocString NAME = (LocString) "Flee";
        public static LocString STATUS = (LocString) "Fleeing";
        public static LocString TOOLTIP = (LocString) "Run away!";
      }

      public class RECOVERBREATH
      {
        public static LocString NAME = (LocString) "Recover Breath";
        public static LocString STATUS = (LocString) "Recovering breath";
        public static LocString TOOLTIP = (LocString) "";
      }

      public class MOVETOQUARANTINE
      {
        public static LocString NAME = (LocString) "Move to Quarantine";
        public static LocString STATUS = (LocString) "Moving to quarantine";
        public static LocString TOOLTIP = (LocString) "This Duplicant will isolate themselves to keep their illness away from the colony";
      }

      public class ATTACK
      {
        public static LocString NAME = (LocString) "Attack";
        public static LocString STATUS = (LocString) "Attacking";
        public static LocString TOOLTIP = (LocString) "Chaaaarge!";
      }

      public class CAPTURE
      {
        public static LocString NAME = (LocString) "Wrangle";
        public static LocString STATUS = (LocString) "Wrangling";
        public static LocString TOOLTIP = (LocString) "Duplicants that possess the Critter Ranching Skill can wrangle most critters without traps";
      }

      public class SINGTOEGG
      {
        public static LocString NAME = (LocString) "Sing To Egg";
        public static LocString STATUS = (LocString) "Singing to egg";
        public static LocString TOOLTIP = (LocString) ("A gentle lullaby from a supportive Duplicant encourages developing " + UI.PRE_KEYWORD + "Eggs" + UI.PST_KEYWORD + "\n\nIncreases " + UI.PRE_KEYWORD + "Incubation Rate" + UI.PST_KEYWORD + "\n\nDuplicants must possess the " + (string) DUPLICANTS.ROLES.RANCHER.NAME + " Skill to sing to an egg");
      }

      public class USETOILET
      {
        public static LocString NAME = (LocString) "Use Toilet";
        public static LocString STATUS = (LocString) "Going to use toilet";
        public static LocString TOOLTIP = (LocString) "Duplicants have to use the toilet at least once per day";
      }

      public class WASHHANDS
      {
        public static LocString NAME = (LocString) "Wash Hands";
        public static LocString STATUS = (LocString) "Washing hands";
        public static LocString TOOLTIP = (LocString) ("Good hygiene removes " + UI.PRE_KEYWORD + "Germs" + UI.PST_KEYWORD + " and prevents the spread of " + UI.PRE_KEYWORD + "Disease" + UI.PST_KEYWORD);
      }

      public class CHECKPOINT
      {
        public static LocString NAME = (LocString) "Wait at Checkpoint";
        public static LocString STATUS = (LocString) "Waiting at Checkpoint";
        public static LocString TOOLTIP = (LocString) "This Duplicant is waiting for permission to pass";
      }

      public class TRAVELTUBEENTRANCE
      {
        public static LocString NAME = (LocString) "Enter Transit Tube";
        public static LocString STATUS = (LocString) "Entering Transit Tube";
        public static LocString TOOLTIP = (LocString) "Nyoooom!";
      }

      public class SCRUBORE
      {
        public static LocString NAME = (LocString) "Scrub Ore";
        public static LocString STATUS = (LocString) "Scrubbing ore";
        public static LocString TOOLTIP = (LocString) ("Material ore can be scrubbed to remove " + UI.PRE_KEYWORD + "Germs" + UI.PST_KEYWORD + " present on its surface");
      }

      public class EAT
      {
        public static LocString NAME = (LocString) "Eat";
        public static LocString STATUS = (LocString) "Going to eat";
        public static LocString TOOLTIP = (LocString) ("Duplicants eat to replenish their " + UI.PRE_KEYWORD + "Calorie" + UI.PST_KEYWORD + " stores");
      }

      public class VOMIT
      {
        public static LocString NAME = (LocString) "Vomit";
        public static LocString STATUS = (LocString) "Vomiting";
        public static LocString TOOLTIP = (LocString) ("Vomiting produces " + (string) ELEMENTS.DIRTYWATER.NAME + " and can spread " + UI.PRE_KEYWORD + "Disease" + UI.PST_KEYWORD);
      }

      public class RADIATIONPAIN
      {
        public static LocString NAME = (LocString) "Radiation Aches";
        public static LocString STATUS = (LocString) "Feeling radiation aches";
        public static LocString TOOLTIP = (LocString) ("Radiation Aches are a symptom of " + (string) DUPLICANTS.DISEASES.RADIATIONSICKNESS.NAME);
      }

      public class COUGH
      {
        public static LocString NAME = (LocString) "Cough";
        public static LocString STATUS = (LocString) "Coughing";
        public static LocString TOOLTIP = (LocString) ("Coughing is a symptom of " + (string) DUPLICANTS.DISEASES.SLIMESICKNESS.NAME + " and spreads airborne " + UI.PRE_KEYWORD + "Germs" + UI.PST_KEYWORD);
      }

      public class SLEEP
      {
        public static LocString NAME = (LocString) "Sleep";
        public static LocString STATUS = (LocString) "Sleeping";
        public static LocString TOOLTIP = (LocString) "Zzzzzz...";
      }

      public class NARCOLEPSY
      {
        public static LocString NAME = (LocString) "Narcoleptic Nap";
        public static LocString STATUS = (LocString) "Narcoleptic napping";
        public static LocString TOOLTIP = (LocString) "Zzzzzz...";
      }

      public class FLOORSLEEP
      {
        public static LocString NAME = (LocString) "Sleep on Floor";
        public static LocString STATUS = (LocString) "Sleeping on floor";
        public static LocString TOOLTIP = (LocString) ("Zzzzzz...\n\nSleeping on the floor will give Duplicants a " + (string) DUPLICANTS.MODIFIERS.SOREBACK.NAME);
      }

      public class TAKEMEDICINE
      {
        public static LocString NAME = (LocString) "Take Medicine";
        public static LocString STATUS = (LocString) "Taking medicine";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is taking a dose of medicine to ward off " + UI.PRE_KEYWORD + "Disease" + UI.PST_KEYWORD);
      }

      public class GETDOCTORED
      {
        public static LocString NAME = (LocString) "Visit Doctor";
        public static LocString STATUS = (LocString) "Visiting doctor";
        public static LocString TOOLTIP = (LocString) "This Duplicant is visiting a doctor to receive treatment";
      }

      public class DOCTOR
      {
        public static LocString NAME = (LocString) "Treat Patient";
        public static LocString STATUS = (LocString) "Treating patient";
        public static LocString TOOLTIP = (LocString) "This Duplicant is trying to make one of their peers feel better";
      }

      public class DELIVERFOOD
      {
        public static LocString NAME = (LocString) "Deliver Food";
        public static LocString STATUS = (LocString) "Delivering food";
        public static LocString TOOLTIP = (LocString) "Under thirty minutes or it's free";
      }

      public class SHOWER
      {
        public static LocString NAME = (LocString) "Shower";
        public static LocString STATUS = (LocString) "Showering";
        public static LocString TOOLTIP = (LocString) "This Duplicant is having a refreshing shower";
      }

      public class SIGH
      {
        public static LocString NAME = (LocString) "Sigh";
        public static LocString STATUS = (LocString) "Sighing";
        public static LocString TOOLTIP = (LocString) "Ho-hum.";
      }

      public class RESTDUETODISEASE
      {
        public static LocString NAME = (LocString) "Rest";
        public static LocString STATUS = (LocString) "Resting";
        public static LocString TOOLTIP = (LocString) "This Duplicant isn't feeling well and is taking a rest";
      }

      public class HEAL
      {
        public static LocString NAME = (LocString) "Heal";
        public static LocString STATUS = (LocString) "Healing";
        public static LocString TOOLTIP = (LocString) "This Duplicant is taking some time to recover from their wounds";
      }

      public class STRESSACTINGOUT
      {
        public static LocString NAME = (LocString) "Lash Out";
        public static LocString STATUS = (LocString) "Lashing out";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is having a " + UI.PRE_KEYWORD + "Stress" + UI.PST_KEYWORD + "-induced tantrum");
      }

      public class RELAX
      {
        public static LocString NAME = (LocString) "Relax";
        public static LocString STATUS = (LocString) "Relaxing";
        public static LocString TOOLTIP = (LocString) "This Duplicant is taking it easy";
      }

      public class STRESSHEAL
      {
        public static LocString NAME = (LocString) "De-Stress";
        public static LocString STATUS = (LocString) "De-stressing";
        public static LocString TOOLTIP = (LocString) ("This Duplicant taking some time to recover from their " + UI.PRE_KEYWORD + "Stress" + UI.PST_KEYWORD);
      }

      public class EQUIP
      {
        public static LocString NAME = (LocString) "Equip";
        public static LocString STATUS = (LocString) "Moving to equip";
        public static LocString TOOLTIP = (LocString) "This Duplicant is putting on a piece of equipment";
      }

      public class LEARNSKILL
      {
        public static LocString NAME = (LocString) "Learn Skill";
        public static LocString STATUS = (LocString) "Learning skill";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is learning a new " + UI.PRE_KEYWORD + "Skill" + UI.PST_KEYWORD);
      }

      public class UNLEARNSKILL
      {
        public static LocString NAME = (LocString) "Unlearn Skills";
        public static LocString STATUS = (LocString) "Unlearning skills";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is unlearning " + UI.PRE_KEYWORD + "Skills" + UI.PST_KEYWORD);
      }

      public class RECHARGE
      {
        public static LocString NAME = (LocString) "Recharge Equipment";
        public static LocString STATUS = (LocString) "Recharging equipment";
        public static LocString TOOLTIP = (LocString) "This Duplicant is recharging their equipment";
      }

      public class UNEQUIP
      {
        public static LocString NAME = (LocString) "Unequip";
        public static LocString STATUS = (LocString) "Moving to unequip";
        public static LocString TOOLTIP = (LocString) "This Duplicant is removing a piece of their equipment";
      }

      public class MOURN
      {
        public static LocString NAME = (LocString) "Mourn";
        public static LocString STATUS = (LocString) "Mourning";
        public static LocString TOOLTIP = (LocString) "This Duplicant is mourning the loss of a friend";
      }

      public class WARMUP
      {
        public static LocString NAME = (LocString) "Warm Up";
        public static LocString STATUS = (LocString) "Going to warm up";
        public static LocString TOOLTIP = (LocString) "This Duplicant got too cold and is going somewhere to warm up";
      }

      public class COOLDOWN
      {
        public static LocString NAME = (LocString) "Cool Off";
        public static LocString STATUS = (LocString) "Going to cool off";
        public static LocString TOOLTIP = (LocString) "This Duplicant got too hot and is going somewhere to cool off";
      }

      public class EMPTYSTORAGE
      {
        public static LocString NAME = (LocString) "Empty Storage";
        public static LocString STATUS = (LocString) "Going to empty storage";
        public static LocString TOOLTIP = (LocString) "This Duplicant is taking items out of storage";
      }

      public class ART
      {
        public static LocString NAME = (LocString) "Decorate";
        public static LocString STATUS = (LocString) "Going to decorate";
        public static LocString TOOLTIP = (LocString) "This Duplicant is going to work on their art";
      }

      public class MOP
      {
        public static LocString NAME = (LocString) "Mop";
        public static LocString STATUS = (LocString) "Going to mop";
        public static LocString TOOLTIP = (LocString) ("Mopping removes " + UI.PRE_KEYWORD + "Liquid" + UI.PST_KEYWORD + " from the floor and bottles them for transport");
      }

      public class RELOCATE
      {
        public static LocString NAME = (LocString) "Relocate";
        public static LocString STATUS = (LocString) "Going to relocate";
        public static LocString TOOLTIP = (LocString) "This Duplicant is moving a building to a new location";
      }

      public class TOGGLE
      {
        public static LocString NAME = (LocString) "Change Setting";
        public static LocString STATUS = (LocString) "Going to change setting";
        public static LocString TOOLTIP = (LocString) "This Duplicant is going to change the settings on a building";
      }

      public class RESCUEINCAPACITATED
      {
        public static LocString NAME = (LocString) "Rescue Friend";
        public static LocString STATUS = (LocString) "Rescuing friend";
        public static LocString TOOLTIP = (LocString) "This Duplicant is rescuing another Duplicant that has been incapacitated";
      }

      public class REPAIR
      {
        public static LocString NAME = (LocString) "Repair";
        public static LocString STATUS = (LocString) "Going to repair";
        public static LocString TOOLTIP = (LocString) "This Duplicant is fixing a broken building";
      }

      public class DECONSTRUCT
      {
        public static LocString NAME = (LocString) "Deconstruct";
        public static LocString STATUS = (LocString) "Going to deconstruct";
        public static LocString TOOLTIP = (LocString) "This Duplicant is deconstructing a building";
      }

      public class RESEARCH
      {
        public static LocString NAME = (LocString) "Research";
        public static LocString STATUS = (LocString) "Going to research";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is working on the current " + UI.PRE_KEYWORD + "Research" + UI.PST_KEYWORD + " focus");
      }

      public class ANALYZEARTIFACT
      {
        public static LocString NAME = (LocString) "Artifact Analysis";
        public static LocString STATUS = (LocString) "Going to analyze artifacts";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is analyzing " + UI.PRE_KEYWORD + "Artifacts" + UI.PST_KEYWORD);
      }

      public class ANALYZESEED
      {
        public static LocString NAME = (LocString) "Seed Analysis";
        public static LocString STATUS = (LocString) "Going to analyze seeds";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is analyzing " + UI.PRE_KEYWORD + "Seeds" + UI.PST_KEYWORD + " to find mutations");
      }

      public class RETURNSUIT
      {
        public static LocString NAME = (LocString) "Dock Exosuit";
        public static LocString STATUS = (LocString) "Docking exosuit";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is plugging an " + UI.PRE_KEYWORD + "Exosuit" + UI.PST_KEYWORD + " in for refilling");
      }

      public class GENERATEPOWER
      {
        public static LocString NAME = (LocString) "Generate Power";
        public static LocString STATUS = (LocString) "Going to generate power";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is producing electrical " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD);
      }

      public class HARVEST
      {
        public static LocString NAME = (LocString) "Harvest";
        public static LocString STATUS = (LocString) "Going to harvest";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is harvesting usable materials from a mature " + UI.PRE_KEYWORD + "Plant" + UI.PST_KEYWORD);
      }

      public class UPROOT
      {
        public static LocString NAME = (LocString) "Uproot";
        public static LocString STATUS = (LocString) "Going to uproot";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is uprooting a plant to retrieve a " + UI.PRE_KEYWORD + "Seed" + UI.PST_KEYWORD);
      }

      public class CLEANTOILET
      {
        public static LocString NAME = (LocString) "Clean Outhouse";
        public static LocString STATUS = (LocString) "Going to clean";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is cleaning out the " + (string) BUILDINGS.PREFABS.OUTHOUSE.NAME);
      }

      public class EMPTYDESALINATOR
      {
        public static LocString NAME = (LocString) "Empty Desalinator";
        public static LocString STATUS = (LocString) "Going to clean";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is emptying out the " + (string) BUILDINGS.PREFABS.DESALINATOR.NAME);
      }

      public class LIQUIDCOOLEDFAN
      {
        public static LocString NAME = (LocString) "Use Fan";
        public static LocString STATUS = (LocString) "Going to use fan";
        public static LocString TOOLTIP = (LocString) "This Duplicant is attempting to cool down the area";
      }

      public class ICECOOLEDFAN
      {
        public static LocString NAME = (LocString) "Use Fan";
        public static LocString STATUS = (LocString) "Going to use fan";
        public static LocString TOOLTIP = (LocString) "This Duplicant is attempting to cool down the area";
      }

      public class COOK
      {
        public static LocString NAME = (LocString) "Cook";
        public static LocString STATUS = (LocString) "Going to cook";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is cooking " + UI.PRE_KEYWORD + "Food" + UI.PST_KEYWORD);
      }

      public class COMPOUND
      {
        public static LocString NAME = (LocString) "Compound Medicine";
        public static LocString STATUS = (LocString) "Going to compound medicine";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is fabricating " + UI.PRE_KEYWORD + "Medicine" + UI.PST_KEYWORD);
      }

      public class TRAIN
      {
        public static LocString NAME = (LocString) "Train";
        public static LocString STATUS = (LocString) "Training";
        public static LocString TOOLTIP = (LocString) "This Duplicant is busy training";
      }

      public class MUSH
      {
        public static LocString NAME = (LocString) "Mush";
        public static LocString STATUS = (LocString) "Going to mush";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is producing " + UI.PRE_KEYWORD + "Food" + UI.PST_KEYWORD);
      }

      public class COMPOSTWORKABLE
      {
        public static LocString NAME = (LocString) "Compost";
        public static LocString STATUS = (LocString) "Going to compost";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is dropping off organic material at the " + (string) BUILDINGS.PREFABS.COMPOST.NAME);
      }

      public class FLIPCOMPOST
      {
        public static LocString NAME = (LocString) "Flip";
        public static LocString STATUS = (LocString) "Going to flip compost";
        public static LocString TOOLTIP = (LocString) ((string) BUILDINGS.PREFABS.COMPOST.NAME + "s need to be flipped in order for their contents to compost");
      }

      public class DEPRESSURIZE
      {
        public static LocString NAME = (LocString) "Depressurize Well";
        public static LocString STATUS = (LocString) "Going to depressurize well";
        public static LocString TOOLTIP = (LocString) ((string) BUILDINGS.PREFABS.OILWELLCAP.NAME + "s need to be periodically depressurized to function");
      }

      public class FABRICATE
      {
        public static LocString NAME = (LocString) "Fabricate";
        public static LocString STATUS = (LocString) "Going to fabricate";
        public static LocString TOOLTIP = (LocString) "This Duplicant is crafting something";
      }

      public class BUILD
      {
        public static LocString NAME = (LocString) "Build";
        public static LocString STATUS = (LocString) "Going to build";
        public static LocString TOOLTIP = (LocString) "This Duplicant is constructing a new building";
      }

      public class BUILDDIG
      {
        public static LocString NAME = (LocString) "Construction Dig";
        public static LocString STATUS = (LocString) "Going to construction dig";
        public static LocString TOOLTIP = (LocString) "This Duplicant is making room for a planned construction task by performing this dig";
      }

      public class DIG
      {
        public static LocString NAME = (LocString) "Dig";
        public static LocString STATUS = (LocString) "Going to dig";
        public static LocString TOOLTIP = (LocString) "This Duplicant is digging out a tile";
      }

      public class FETCH
      {
        public static LocString NAME = (LocString) "Deliver";
        public static LocString STATUS = (LocString) "Delivering";
        public static LocString TOOLTIP = (LocString) "This Duplicant is delivering materials where they need to go";
        public static LocString REPORT_NAME = (LocString) "Deliver to {0}";
      }

      public class JOYREACTION
      {
        public static LocString NAME = (LocString) "Joy Reaction";
        public static LocString STATUS = (LocString) "Overjoyed";
        public static LocString TOOLTIP = (LocString) "This Duplicant is taking a moment to relish in their own happiness";
        public static LocString REPORT_NAME = (LocString) "Overjoyed Reaction";
      }

      public class ROCKETCONTROL
      {
        public static LocString NAME = (LocString) "Rocket Control";
        public static LocString STATUS = (LocString) "Controlling rocket";
        public static LocString TOOLTIP = (LocString) "This Duplicant is keeping their spacecraft on course";
        public static LocString REPORT_NAME = (LocString) "Rocket Control";
      }

      public class STORAGEFETCH
      {
        public static LocString NAME = (LocString) "Store Materials";
        public static LocString STATUS = (LocString) "Storing materials";
        public static LocString TOOLTIP = (LocString) "This Duplicant is moving materials into storage for later use";
        public static LocString REPORT_NAME = (LocString) "Store {0}";
      }

      public class EQUIPMENTFETCH
      {
        public static LocString NAME = (LocString) "Store Equipment";
        public static LocString STATUS = (LocString) "Storing equipment";
        public static LocString TOOLTIP = (LocString) "This Duplicant is transporting equipment for storage";
        public static LocString REPORT_NAME = (LocString) "Store {0}";
      }

      public class REPAIRFETCH
      {
        public static LocString NAME = (LocString) "Repair Supply";
        public static LocString STATUS = (LocString) "Supplying repair materials";
        public static LocString TOOLTIP = (LocString) "This Duplicant is delivering materials to where they'll be needed to repair buildings";
      }

      public class RESEARCHFETCH
      {
        public static LocString NAME = (LocString) "Research Supply";
        public static LocString STATUS = (LocString) "Supplying research materials";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is delivering materials where they'll be needed to conduct " + UI.PRE_KEYWORD + "Research" + UI.PST_KEYWORD);
      }

      public class FARMFETCH
      {
        public static LocString NAME = (LocString) "Farming Supply";
        public static LocString STATUS = (LocString) "Supplying farming materials";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is delivering farming materials where they're needed to tend " + UI.PRE_KEYWORD + "Crops" + UI.PST_KEYWORD);
      }

      public class FETCHCRITICAL
      {
        public static LocString NAME = (LocString) "Life Support Supply";
        public static LocString STATUS = (LocString) "Supplying critical materials";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is delivering materials required to perform " + UI.PRE_KEYWORD + "Life Support" + UI.PST_KEYWORD + " Errands");
        public static LocString REPORT_NAME = (LocString) "Life Support Supply to {0}";
      }

      public class MACHINEFETCH
      {
        public static LocString NAME = (LocString) "Operational Supply";
        public static LocString STATUS = (LocString) "Supplying operational materials";
        public static LocString TOOLTIP = (LocString) "This Duplicant is delivering materials to where they'll be needed for machine operation";
        public static LocString REPORT_NAME = (LocString) "Operational Supply to {0}";
      }

      public class COOKFETCH
      {
        public static LocString NAME = (LocString) "Cook Supply";
        public static LocString STATUS = (LocString) "Supplying cook ingredients";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is delivering materials required to cook " + UI.PRE_KEYWORD + "Food" + UI.PST_KEYWORD);
      }

      public class DOCTORFETCH
      {
        public static LocString NAME = (LocString) "Medical Supply";
        public static LocString STATUS = (LocString) "Supplying medical resources";
        public static LocString TOOLTIP = (LocString) "This Duplicant is delivering the materials that will be needed to treat sick patients";
        public static LocString REPORT_NAME = (LocString) "Medical Supply to {0}";
      }

      public class FOODFETCH
      {
        public static LocString NAME = (LocString) "Store Food";
        public static LocString STATUS = (LocString) "Storing food";
        public static LocString TOOLTIP = (LocString) "This Duplicant is moving edible resources into proper storage";
        public static LocString REPORT_NAME = (LocString) "Store {0}";
      }

      public class POWERFETCH
      {
        public static LocString NAME = (LocString) "Power Supply";
        public static LocString STATUS = (LocString) "Supplying power materials";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is delivering materials to where they'll be needed for " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD);
        public static LocString REPORT_NAME = (LocString) "Power Supply to {0}";
      }

      public class FABRICATEFETCH
      {
        public static LocString NAME = (LocString) "Fabrication Supply";
        public static LocString STATUS = (LocString) "Supplying fabrication materials";
        public static LocString TOOLTIP = (LocString) "This Duplicant is delivering materials required to fabricate new objects";
        public static LocString REPORT_NAME = (LocString) "Fabrication Supply to {0}";
      }

      public class BUILDFETCH
      {
        public static LocString NAME = (LocString) "Construction Supply";
        public static LocString STATUS = (LocString) "Supplying construction materials";
        public static LocString TOOLTIP = (LocString) "This delivery will provide materials to a planned construction site";
      }

      public class FETCHCREATURE
      {
        public static LocString NAME = (LocString) "Relocate Critter";
        public static LocString STATUS = (LocString) "Relocating critter";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is moving a " + UI.PRE_KEYWORD + "Critter" + UI.PST_KEYWORD + " to a new location");
      }

      public class FETCHRANCHING
      {
        public static LocString NAME = (LocString) "Ranching Supply";
        public static LocString STATUS = (LocString) "Supplying ranching materials";
        public static LocString TOOLTIP = (LocString) "This Duplicant is delivering materials for ranching activities";
      }

      public class TRANSPORT
      {
        public static LocString NAME = (LocString) "Sweep";
        public static LocString STATUS = (LocString) "Going to sweep";
        public static LocString TOOLTIP = (LocString) ("Moving debris off the ground and into storage improves colony " + UI.PRE_KEYWORD + "Decor" + UI.PST_KEYWORD);
      }

      public class MOVETOSAFETY
      {
        public static LocString NAME = (LocString) "Find Safe Area";
        public static LocString STATUS = (LocString) "Finding safer area";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is " + UI.PRE_KEYWORD + "Idle" + UI.PST_KEYWORD + " and looking for somewhere safe and comfy to chill");
      }

      public class PARTY
      {
        public static LocString NAME = (LocString) "Party";
        public static LocString STATUS = (LocString) "Partying";
        public static LocString TOOLTIP = (LocString) "This Duplicant is partying hard";
      }

      public class POWER_TINKER
      {
        public static LocString NAME = (LocString) "Tinker";
        public static LocString STATUS = (LocString) "Tinkering";
        public static LocString TOOLTIP = (LocString) "Tinkering with buildings improves their functionality";
      }

      public class RANCH
      {
        public static LocString NAME = (LocString) "Ranch";
        public static LocString STATUS = (LocString) "Ranching";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is tending to a " + UI.PRE_KEYWORD + "Critter" + UI.PST_KEYWORD + "'s well-being");
        public static LocString REPORT_NAME = (LocString) "Deliver to {0}";
      }

      public class CROP_TEND
      {
        public static LocString NAME = (LocString) "Tend";
        public static LocString STATUS = (LocString) "Tending plant";
        public static LocString TOOLTIP = (LocString) ("Tending to plants increases their " + UI.PRE_KEYWORD + "Growth Rate" + UI.PST_KEYWORD);
      }

      public class DEMOLISH
      {
        public static LocString NAME = (LocString) "Demolish";
        public static LocString STATUS = (LocString) "Demolishing object";
        public static LocString TOOLTIP = (LocString) "Demolishing an object removes it permanently";
      }

      public class IDLE
      {
        public static LocString NAME = (LocString) "Idle";
        public static LocString STATUS = (LocString) "Idle";
        public static LocString TOOLTIP = (LocString) ("This Duplicant cannot reach any pending " + UI.PRE_KEYWORD + "Errands" + UI.PST_KEYWORD);
      }

      public class PRECONDITIONS
      {
        public static LocString HEADER = (LocString) "The selected {Selected} could:";
        public static LocString SUCCESS_ROW = (LocString) "{Duplicant} -- {Rank}";
        public static LocString CURRENT_ERRAND = (LocString) "Current Errand";
        public static LocString RANK_FORMAT = (LocString) "#{0}";
        public static LocString FAILURE_ROW = (LocString) "{Duplicant} -- {Reason}";
        public static LocString CONTAINS_OXYGEN = (LocString) "Not enough Oxygen";
        public static LocString IS_PREEMPTABLE = (LocString) "Already assigned to {Assignee}";
        public static LocString HAS_URGE = (LocString) "No current need";
        public static LocString IS_VALID = (LocString) "Invalid";
        public static LocString IS_PERMITTED = (LocString) "Not permitted";
        public static LocString IS_ASSIGNED_TO_ME = (LocString) "Not assigned to {Selected}";
        public static LocString IS_IN_MY_WORLD = (LocString) "Outside world";
        public static LocString IS_CELL_NOT_IN_MY_WORLD = (LocString) "Already there";
        public static LocString IS_IN_MY_ROOM = (LocString) "Outside {Selected}'s room";
        public static LocString IS_PREFERRED_ASSIGNABLE = (LocString) "Not preferred assignment";
        public static LocString IS_PREFERRED_ASSIGNABLE_OR_URGENT_BLADDER = (LocString) "Not preferred assignment";
        public static LocString HAS_SKILL_PERK = (LocString) "Requires learned skill";
        public static LocString IS_MORE_SATISFYING = (LocString) "Low priority";
        public static LocString CAN_CHAT = (LocString) "Unreachable";
        public static LocString IS_NOT_RED_ALERT = (LocString) "Unavailable in Red Alert";
        public static LocString NO_DEAD_BODIES = (LocString) "Unburied Duplicant";
        public static LocString NOT_A_ROBOT = (LocString) "Unavailable to Robots";
        public static LocString VALID_MOURNING_SITE = (LocString) "Nowhere to mourn";
        public static LocString HAS_PLACE_TO_STAND = (LocString) "Nowhere to stand";
        public static LocString IS_SCHEDULED_TIME = (LocString) "Not allowed by schedule";
        public static LocString CAN_MOVE_TO = (LocString) "Unreachable";
        public static LocString CAN_PICKUP = (LocString) "Cannot pickup";
        public static LocString IS_AWAKE = (LocString) "{Selected} is sleeping";
        public static LocString IS_STANDING = (LocString) "{Selected} must stand";
        public static LocString IS_MOVING = (LocString) "{Selected} is not moving";
        public static LocString IS_OFF_LADDER = (LocString) "{Selected} is busy climbing";
        public static LocString NOT_IN_TUBE = (LocString) "{Selected} is busy in transit";
        public static LocString HAS_TRAIT = (LocString) "Missing required trait";
        public static LocString IS_OPERATIONAL = (LocString) "Not operational";
        public static LocString IS_MARKED_FOR_DECONSTRUCTION = (LocString) "Being deconstructed";
        public static LocString IS_NOT_BURROWED = (LocString) "Is not burrowed";
        public static LocString IS_CREATURE_AVAILABLE_FOR_RANCHING = (LocString) "No Critters Available";
        public static LocString IS_CREATURE_AVAILABLE_FOR_FIXED_CAPTURE = (LocString) "Pen Status OK";
        public static LocString IS_MARKED_FOR_DISABLE = (LocString) "Building Disabled";
        public static LocString IS_FUNCTIONAL = (LocString) "Not functioning";
        public static LocString IS_OVERRIDE_TARGET_NULL_OR_ME = (LocString) "DebugIsOverrideTargetNullOrMe";
        public static LocString NOT_CHORE_CREATOR = (LocString) "DebugNotChoreCreator";
        public static LocString IS_GETTING_MORE_STRESSED = (LocString) "{Selected}'s stress is decreasing";
        public static LocString IS_ALLOWED_BY_AUTOMATION = (LocString) "Automated";
        public static LocString CAN_DO_RECREATION = (LocString) "Not Interested";
        public static LocString DOES_SUIT_NEED_RECHARGING_IDLE = (LocString) "Suit is currently charged";
        public static LocString DOES_SUIT_NEED_RECHARGING_URGENT = (LocString) "Suit is currently charged";
        public static LocString HAS_SUIT_MARKER = (LocString) "No Suit Checkpoint";
        public static LocString ALLOWED_TO_DEPRESSURIZE = (LocString) "Not currently overpressure";
        public static LocString IS_STRESS_ABOVE_ACTIVATION_RANGE = (LocString) "{Selected} is not stressed right now";
        public static LocString IS_NOT_ANGRY = (LocString) "{Selected} is too angry";
        public static LocString IS_NOT_BEING_ATTACKED = (LocString) "{Selected} is in combat";
        public static LocString IS_CONSUMPTION_PERMITTED = (LocString) "Disallowed by consumable permissions";
        public static LocString CAN_CURE = (LocString) "No applicable illness";
        public static LocString TREATMENT_AVAILABLE = (LocString) "No treatable illness";
        public static LocString DOCTOR_AVAILABLE = (LocString) "No doctors available\n(Duplicants cannot treat themselves)";
        public static LocString IS_OKAY_TIME_TO_SLEEP = (LocString) "No current need";
        public static LocString IS_NARCOLEPSING = (LocString) "{Selected} is currently napping";
        public static LocString IS_FETCH_TARGET_AVAILABLE = (LocString) "No pending deliveries";
        public static LocString EDIBLE_IS_NOT_NULL = (LocString) "Consumable Permission not allowed";
        public static LocString HAS_MINGLE_CELL = (LocString) "Nowhere to Mingle";
        public static LocString EXCLUSIVELY_AVAILABLE = (LocString) "Building Already Busy";
        public static LocString BLADDER_FULL = (LocString) "Bladder isn't full";
        public static LocString BLADDER_NOT_FULL = (LocString) "Bladder too full";
        public static LocString CURRENTLY_PEEING = (LocString) "Currently Peeing";
        public static LocString HAS_BALLOON_STALL_CELL = (LocString) "Has a location for a Balloon Stall";
        public static LocString IS_MINION = (LocString) "Must be a Duplicant";
        public static LocString IS_ROCKET_TRAVELLING = (LocString) "Rocket must be travelling";
      }
    }

    public class SKILLGROUPS
    {
      public class MINING
      {
        public static LocString NAME = (LocString) "Digger";
      }

      public class BUILDING
      {
        public static LocString NAME = (LocString) "Builder";
      }

      public class FARMING
      {
        public static LocString NAME = (LocString) "Farmer";
      }

      public class RANCHING
      {
        public static LocString NAME = (LocString) "Rancher";
      }

      public class COOKING
      {
        public static LocString NAME = (LocString) "Cooker";
      }

      public class ART
      {
        public static LocString NAME = (LocString) "Decorator";
      }

      public class RESEARCH
      {
        public static LocString NAME = (LocString) "Researcher";
      }

      public class SUITS
      {
        public static LocString NAME = (LocString) "Suit Wearer";
      }

      public class HAULING
      {
        public static LocString NAME = (LocString) "Supplier";
      }

      public class TECHNICALS
      {
        public static LocString NAME = (LocString) "Operator";
      }

      public class MEDICALAID
      {
        public static LocString NAME = (LocString) "Doctor";
      }

      public class BASEKEEPING
      {
        public static LocString NAME = (LocString) "Tidier";
      }

      public class ROCKETRY
      {
        public static LocString NAME = (LocString) "Pilot";
      }
    }

    public class CHOREGROUPS
    {
      public class ART
      {
        public static LocString NAME = (LocString) "Decorating";
        public static LocString DESC = (LocString) ("Sculpt or paint to improve colony " + UI.PRE_KEYWORD + "Decor" + UI.PST_KEYWORD + ".");
        public static LocString ARCHETYPE_NAME = (LocString) "Decorator";
      }

      public class COMBAT
      {
        public static LocString NAME = (LocString) "Attacking";
        public static LocString DESC = (LocString) ("Fight wild " + UI.FormatAsLink("Critters", "CREATURES") + ".");
        public static LocString ARCHETYPE_NAME = (LocString) "Attacker";
      }

      public class LIFESUPPORT
      {
        public static LocString NAME = (LocString) "Life Support";
        public static LocString DESC = (LocString) ("Maintain " + (string) BUILDINGS.PREFABS.ALGAEHABITAT.NAME + "s, " + (string) BUILDINGS.PREFABS.AIRFILTER.NAME + "s, and " + (string) BUILDINGS.PREFABS.WATERPURIFIER.NAME + "s to support colony life.");
        public static LocString ARCHETYPE_NAME = (LocString) "Life Supporter";
      }

      public class TOGGLE
      {
        public static LocString NAME = (LocString) "Toggling";
        public static LocString DESC = (LocString) "Enable or disable buildings, adjust building settings, and set or flip switches and sensors.";
        public static LocString ARCHETYPE_NAME = (LocString) "Toggler";
      }

      public class COOK
      {
        public static LocString NAME = (LocString) "Cooking";
        public static LocString DESC = (LocString) ("Operate " + UI.PRE_KEYWORD + "Food" + UI.PST_KEYWORD + " preparation buildings.");
        public static LocString ARCHETYPE_NAME = (LocString) "Cooker";
      }

      public class RESEARCH
      {
        public static LocString NAME = (LocString) "Researching";
        public static LocString DESC = (LocString) ("Use " + UI.PRE_KEYWORD + "Research Stations" + UI.PST_KEYWORD + " to unlock new technologies.");
        public static LocString ARCHETYPE_NAME = (LocString) "Researcher";
      }

      public class REPAIR
      {
        public static LocString NAME = (LocString) "Repairing";
        public static LocString DESC = (LocString) "Repair damaged buildings.";
        public static LocString ARCHETYPE_NAME = (LocString) "Repairer";
      }

      public class FARMING
      {
        public static LocString NAME = (LocString) "Farming";
        public static LocString DESC = (LocString) ("Gather crops from mature " + UI.PRE_KEYWORD + "Plants" + UI.PST_KEYWORD + ".");
        public static LocString ARCHETYPE_NAME = (LocString) "Farmer";
      }

      public class RANCHING
      {
        public static LocString NAME = (LocString) "Ranching";
        public static LocString DESC = (LocString) ("Tend to domesticated " + UI.FormatAsLink("Critters", "CREATURES") + ".");
        public static LocString ARCHETYPE_NAME = (LocString) "Rancher";
      }

      public class BUILD
      {
        public static LocString NAME = (LocString) "Building";
        public static LocString DESC = (LocString) "Construct new buildings.";
        public static LocString ARCHETYPE_NAME = (LocString) "Builder";
      }

      public class HAULING
      {
        public static LocString NAME = (LocString) "Supplying";
        public static LocString DESC = (LocString) "Run resources to critical buildings and urgent storage.";
        public static LocString ARCHETYPE_NAME = (LocString) "Supplier";
      }

      public class STORAGE
      {
        public static LocString NAME = (LocString) "Storing";
        public static LocString DESC = (LocString) "Fill storage buildings with resources when no other errands are available.";
        public static LocString ARCHETYPE_NAME = (LocString) "Storer";
      }

      public class RECREATION
      {
        public static LocString NAME = (LocString) "Relaxing";
        public static LocString DESC = (LocString) "Use leisure facilities, chat with other Duplicants, and relieve Stress.";
        public static LocString ARCHETYPE_NAME = (LocString) "Relaxer";
      }

      public class BASEKEEPING
      {
        public static LocString NAME = (LocString) "Tidying";
        public static LocString DESC = (LocString) "Sweep, mop, and disinfect objects within the colony.";
        public static LocString ARCHETYPE_NAME = (LocString) "Tidier";
      }

      public class DIG
      {
        public static LocString NAME = (LocString) "Digging";
        public static LocString DESC = (LocString) "Mine raw resources.";
        public static LocString ARCHETYPE_NAME = (LocString) "Digger";
      }

      public class MEDICALAID
      {
        public static LocString NAME = (LocString) "Doctoring";
        public static LocString DESC = (LocString) "Treat sick and injured Duplicants.";
        public static LocString ARCHETYPE_NAME = (LocString) "Doctor";
      }

      public class MASSAGE
      {
        public static LocString NAME = (LocString) "Relaxing";
        public static LocString DESC = (LocString) "Take breaks for massages.";
        public static LocString ARCHETYPE_NAME = (LocString) "Relaxer";
      }

      public class MACHINEOPERATING
      {
        public static LocString NAME = (LocString) "Operating";
        public static LocString DESC = (LocString) "Operating machinery for production, fabrication, and utility purposes.";
        public static LocString ARCHETYPE_NAME = (LocString) "Operator";
      }

      public class SUITS
      {
        public static LocString ARCHETYPE_NAME = (LocString) "Suit Wearer";
      }

      public class ROCKETRY
      {
        public static LocString NAME = (LocString) "Rocketry";
        public static LocString DESC = (LocString) "Pilot rockets";
        public static LocString ARCHETYPE_NAME = (LocString) "Pilot";
      }
    }

    public class STATUSITEMS
    {
      public class GENERIC_DELIVER
      {
        public static LocString NAME = (LocString) "Delivering resources to {Target}";
        public static LocString TOOLTIP = (LocString) "This Duplicant is transporting materials to <b>{Target}</b>";
      }

      public class COUGHING
      {
        public static LocString NAME = (LocString) "Yucky Lungs Coughing";
        public static LocString TOOLTIP = (LocString) ("Hey! Do that into your elbow\n• Coughing fit was caused by " + (string) DUPLICANTS.MODIFIERS.CONTAMINATEDLUNGS.NAME);
      }

      public class WEARING_PAJAMAS
      {
        public static LocString NAME = (LocString) ("Wearing " + UI.FormatAsLink("Pajamas", "SLEEP_CLINIC_PAJAMAS"));
        public static LocString TOOLTIP = (LocString) ("This Duplicant can now produce " + UI.FormatAsLink("Dream Journals", "DREAMJOURNAL") + " when sleeping");
      }

      public class DREAMING
      {
        public static LocString NAME = (LocString) "Dreaming";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is adventuring through their own subconscious\n\nDreams are caused by wearing " + UI.FormatAsLink("Pajamas", "SLEEP_CLINIC_PAJAMAS") + "\n\n" + UI.FormatAsLink("Dream Journal", "DREAMJOURNAL") + " will be ready in {time}");
      }

      public class SLEEPING
      {
        public static LocString NAME = (LocString) "Sleeping";
        public static LocString TOOLTIP = (LocString) "This Duplicant is recovering stamina";
        public static LocString TOOLTIP_DISTURBER = (LocString) "\n\nThey were sleeping peacefully until they were disturbed by <b>{Disturber}</b>";
      }

      public class SLEEPINGPEACEFULLY
      {
        public static LocString NAME = (LocString) "Sleeping peacefully";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is getting well-deserved, quality sleep\n\nAt this rate they're sure to feel " + UI.FormatAsLink("Well Rested", "SLEEP") + " tomorrow morning");
      }

      public class SLEEPINGBADLY
      {
        public static LocString NAME = (LocString) "Sleeping badly";
        public static LocString TOOLTIP = (LocString) ("This Duplicant's having trouble falling asleep due to noise from <b>{Disturber}</b>\n\nThey're going to feel a bit " + UI.FormatAsLink("Unrested", "SLEEP") + " tomorrow morning");
      }

      public class SLEEPINGTERRIBLY
      {
        public static LocString NAME = (LocString) "Can't sleep";
        public static LocString TOOLTIP = (LocString) ("This Duplicant was woken up by noise from <b>{Disturber}</b> and can't get back to sleep\n\nThey're going to feel " + UI.FormatAsLink("Dead Tired", "SLEEP") + " tomorrow morning");
      }

      public class SLEEPINGINTERRUPTEDBYLIGHT
      {
        public static LocString NAME = (LocString) "Interrupted Sleep: Bright Light";
        public static LocString TOOLTIP = (LocString) ("This Duplicant can't sleep because the " + UI.PRE_KEYWORD + "Lights" + UI.PST_KEYWORD + " are still on");
      }

      public class SLEEPINGINTERRUPTEDBYNOISE
      {
        public static LocString NAME = (LocString) "Interrupted Sleep: Snoring Friend";
        public static LocString TOOLTIP = (LocString) "This Duplicant is having trouble sleeping thanks to a certain noisy someone";
      }

      public class SLEEPINGINTERRUPTEDBYFEAROFDARK
      {
        public static LocString NAME = (LocString) "Interrupted Sleep: Afraid of Dark";
        public static LocString TOOLTIP = (LocString) "This Duplicant is having trouble sleeping because of their fear of the dark";
      }

      public class SLEEPINGINTERRUPTEDBYMOVEMENT
      {
        public static LocString NAME = (LocString) "Interrupted Sleep: Bed Jostling";
        public static LocString TOOLTIP = (LocString) "This Duplicant was woken up because their bed was moved";
      }

      public class REDALERT
      {
        public static LocString NAME = (LocString) "Red Alert!";
        public static LocString TOOLTIP = (LocString) ("The colony is in a state of " + UI.PRE_KEYWORD + "Red Alert" + UI.PST_KEYWORD + ". Duplicants will not eat, sleep, use the bathroom, or engage in leisure activities while the " + UI.PRE_KEYWORD + "Red Alert" + UI.PST_KEYWORD + " is active");
      }

      public class ROLE
      {
        public static LocString NAME = (LocString) "{Role}: {Progress} Mastery";
        public static LocString TOOLTIP = (LocString) "This Duplicant is working as a <b>{Role}</b>\n\nThey have <b>{Progress}</b> mastery of this job";
      }

      public class LOWOXYGEN
      {
        public static LocString NAME = (LocString) "Oxygen low";
        public static LocString TOOLTIP = (LocString) "This Duplicant is working in a low breathability area";
        public static LocString NOTIFICATION_NAME = (LocString) ("Low " + (string) ELEMENTS.OXYGEN.NAME + " area entered");
        public static LocString NOTIFICATION_TOOLTIP = (LocString) ("These Duplicants are working in areas with low " + (string) ELEMENTS.OXYGEN.NAME + ":");
      }

      public class SEVEREWOUNDS
      {
        public static LocString NAME = (LocString) "Severely injured";
        public static LocString TOOLTIP = (LocString) "This Duplicant is badly hurt";
        public static LocString NOTIFICATION_NAME = (LocString) "Severely injured";
        public static LocString NOTIFICATION_TOOLTIP = (LocString) "These Duplicants are badly hurt and require medical attention";
      }

      public class INCAPACITATED
      {
        public static LocString NAME = (LocString) "Incapacitated: {CauseOfIncapacitation}\nTime until death: {TimeUntilDeath}\n";
        public static LocString TOOLTIP = (LocString) "This Duplicant is near death!\n\nAssign them to a Triage Cot for rescue";
        public static LocString NOTIFICATION_NAME = (LocString) "Incapacitated";
        public static LocString NOTIFICATION_TOOLTIP = (LocString) ("These Duplicants are near death.\nA " + (string) BUILDINGS.PREFABS.MEDICALCOT.NAME + " is required for rescue:");
      }

      public class BEDUNREACHABLE
      {
        public static LocString NAME = (LocString) "Cannot reach bed";
        public static LocString TOOLTIP = (LocString) "This Duplicant cannot reach their bed";
        public static LocString NOTIFICATION_NAME = (LocString) "Unreachable bed";
        public static LocString NOTIFICATION_TOOLTIP = (LocString) ("These Duplicants cannot sleep because their " + UI.PRE_KEYWORD + "Beds" + UI.PST_KEYWORD + " are beyond their reach:");
      }

      public class COLD
      {
        public static LocString NAME = (LocString) "Chilly surroundings";
        public static LocString TOOLTIP = (LocString) "This Duplicant cannot retain enough heat to stay warm and may be under insulated for this area\n\nStress: <b>{StressModification}</b>\n\nCurrent Environmental Exchange: <b>{currentTransferWattage}</b>\n\nInsulation Thickness: {conductivityBarrier}";
      }

      public class DAILYRATIONLIMITREACHED
      {
        public static LocString NAME = (LocString) "Daily calorie limit reached";
        public static LocString TOOLTIP = (LocString) ("This Duplicant has consumed their allotted " + UI.FormatAsLink("Rations", "FOOD") + " for the day");
        public static LocString NOTIFICATION_NAME = (LocString) "Daily calorie limit reached";
        public static LocString NOTIFICATION_TOOLTIP = (LocString) ("These Duplicants have consumed their allotted " + UI.FormatAsLink("Rations", "FOOD") + " for the day:");
      }

      public class DOCTOR
      {
        public static LocString NAME = (LocString) "Treating Patient";
        public static LocString STATUS = (LocString) "This Duplicant is going to administer medical care to an ailing friend";
      }

      public class HOLDINGBREATH
      {
        public static LocString NAME = (LocString) "Holding breath";
        public static LocString TOOLTIP = (LocString) "This Duplicant cannot breathe in their current location";
      }

      public class RECOVERINGBREATH
      {
        public static LocString NAME = (LocString) "Recovering breath";
        public static LocString TOOLTIP = (LocString) "This Duplicant held their breath too long and needs a moment";
      }

      public class HOT
      {
        public static LocString NAME = (LocString) "Toasty surroundings";
        public static LocString TOOLTIP = (LocString) ("This Duplicant cannot let off enough " + UI.PRE_KEYWORD + "Heat" + UI.PST_KEYWORD + " to stay cool and may be over insulated for this area\n\nStress Modification: <b>{StressModification}</b>\n\nCurrent Environmental Exchange: <b>{currentTransferWattage}</b>\n\nInsulation Thickness: {conductivityBarrier}");
      }

      public class HUNGRY
      {
        public static LocString NAME = (LocString) "Hungry";
        public static LocString TOOLTIP = (LocString) "This Duplicant would really like something to eat";
      }

      public class POORDECOR
      {
        public static LocString NAME = (LocString) "Drab decor";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is depressed by the lack of " + UI.PRE_KEYWORD + "Decor" + UI.PST_KEYWORD + " in this area");
      }

      public class POORQUALITYOFLIFE
      {
        public static LocString NAME = (LocString) "Low Morale";
        public static LocString TOOLTIP = (LocString) ("The bad in this Duplicant's life is starting to outweigh the good\n\nImproved amenities and additional " + UI.PRE_KEYWORD + "Downtime" + UI.PST_KEYWORD + " would help improve their " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD);
      }

      public class POOR_FOOD_QUALITY
      {
        public static LocString NAME = (LocString) "Lousy Meal";
        public static LocString TOOLTIP = (LocString) "The last meal this Duplicant ate didn't quite meet their expectations";
      }

      public class GOOD_FOOD_QUALITY
      {
        public static LocString NAME = (LocString) "Decadent Meal";
        public static LocString TOOLTIP = (LocString) "The last meal this Duplicant ate exceeded their expectations!";
      }

      public class NERVOUSBREAKDOWN
      {
        public static LocString NAME = (LocString) "Nervous breakdown";
        public static LocString TOOLTIP = (LocString) (UI.PRE_KEYWORD + "Stress" + UI.PST_KEYWORD + " has completely eroded this Duplicant's ability to function");
        public static LocString NOTIFICATION_NAME = (LocString) "Nervous breakdown";
        public static LocString NOTIFICATION_TOOLTIP = (LocString) ("These Duplicants have cracked under the " + UI.PRE_KEYWORD + "Stress" + UI.PST_KEYWORD + " and need assistance:");
      }

      public class STRESSED
      {
        public static LocString NAME = (LocString) "Stressed";
        public static LocString TOOLTIP = (LocString) "This Duplicant is feeling the pressure";
        public static LocString NOTIFICATION_NAME = (LocString) "High stress";
        public static LocString NOTIFICATION_TOOLTIP = (LocString) ("These Duplicants are " + UI.PRE_KEYWORD + "Stressed" + UI.PST_KEYWORD + " and need to unwind:");
      }

      public class NORATIONSAVAILABLE
      {
        public static LocString NAME = (LocString) "No food available";
        public static LocString TOOLTIP = (LocString) "There's nothing in the colony for this Duplicant to eat";
        public static LocString NOTIFICATION_NAME = (LocString) "No food available";
        public static LocString NOTIFICATION_TOOLTIP = (LocString) "These Duplicants have nothing to eat:";
      }

      public class QUARANTINEAREAUNREACHABLE
      {
        public static LocString NAME = (LocString) "Cannot reach quarantine";
        public static LocString TOOLTIP = (LocString) "This Duplicant cannot reach their quarantine zone";
        public static LocString NOTIFICATION_NAME = (LocString) "Unreachable quarantine";
        public static LocString NOTIFICATION_TOOLTIP = (LocString) "These Duplicants cannot reach their assigned quarantine zones:";
      }

      public class QUARANTINED
      {
        public static LocString NAME = (LocString) "Quarantined";
        public static LocString TOOLTIP = (LocString) "This Duplicant has been isolated from the colony";
      }

      public class RATIONSUNREACHABLE
      {
        public static LocString NAME = (LocString) "Cannot reach food";
        public static LocString TOOLTIP = (LocString) ("There is " + UI.PRE_KEYWORD + "Food" + UI.PST_KEYWORD + " in the colony that this Duplicant cannot reach");
        public static LocString NOTIFICATION_NAME = (LocString) "Unreachable food";
        public static LocString NOTIFICATION_TOOLTIP = (LocString) ("These Duplicants cannot access the colony's " + UI.PRE_KEYWORD + "Food" + UI.PST_KEYWORD + ":");
      }

      public class RATIONSNOTPERMITTED
      {
        public static LocString NAME = (LocString) "Food Type Not Permitted";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is not allowed to eat any of the " + UI.FormatAsLink("Food", "FOOD") + " in their reach\n\nEnter the <color=#833A5FFF>CONSUMABLES</color> <color=#F44A47><b>[F]</b></color> to adjust their food permissions");
        public static LocString NOTIFICATION_NAME = (LocString) "Unpermitted food";
        public static LocString NOTIFICATION_TOOLTIP = (LocString) ("These Duplicants' <color=#833A5FFF>CONSUMABLES</color> <color=#F44A47><b>[F]</b></color> permissions prevent them from eating any of the " + UI.FormatAsLink("Food", "FOOD") + " within their reach:");
      }

      public class ROTTEN
      {
        public static LocString NAME = (LocString) "Rotten";
        public static LocString TOOLTIP = (LocString) "Gross!";
      }

      public class STARVING
      {
        public static LocString NAME = (LocString) "Starving";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is about to die and needs " + UI.PRE_KEYWORD + "Food" + UI.PST_KEYWORD + "!");
        public static LocString NOTIFICATION_NAME = (LocString) "Starvation";
        public static LocString NOTIFICATION_TOOLTIP = (LocString) ("These Duplicants are starving and will die if they can't find " + UI.PRE_KEYWORD + "Food" + UI.PST_KEYWORD + ":");
      }

      public class STRESS_SIGNAL_AGGRESIVE
      {
        public static LocString NAME = (LocString) "Frustrated";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is trying to keep their cool\n\nImprove this Duplicant's " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD + " before they destroy something to let off steam");
      }

      public class STRESS_SIGNAL_BINGE_EAT
      {
        public static LocString NAME = (LocString) "Stress Cravings";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is consumed by hunger\n\nImprove this Duplicant's " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD + " before they eat all the colony's " + UI.PRE_KEYWORD + "Food" + UI.PST_KEYWORD + " stores");
      }

      public class STRESS_SIGNAL_UGLY_CRIER
      {
        public static LocString NAME = (LocString) "Misty Eyed";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is trying and failing to swallow their emotions\n\nImprove this Duplicant's " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD + " before they have a good ugly cry");
      }

      public class STRESS_SIGNAL_VOMITER
      {
        public static LocString NAME = (LocString) "Stress Burp";
        public static LocString TOOLTIP = (LocString) ("Sort of like having butterflies in your stomach, except they're burps\n\nImprove this Duplicant's " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD + " before they start to stress vomit");
      }

      public class STRESS_SIGNAL_BANSHEE
      {
        public static LocString NAME = (LocString) "Suppressed Screams";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is fighting the urge to scream\n\nImprove this Duplicant's " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD + " before they start wailing uncontrollably");
      }

      public class ENTOMBEDCHORE
      {
        public static LocString NAME = (LocString) "Entombed";
        public static LocString TOOLTIP = (LocString) "This Duplicant needs someone to help dig them out!";
        public static LocString NOTIFICATION_NAME = (LocString) "Entombed";
        public static LocString NOTIFICATION_TOOLTIP = (LocString) "These Duplicants are trapped:";
      }

      public class EARLYMORNING
      {
        public static LocString NAME = (LocString) "Early Bird";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is jazzed to start the day\n• All " + UI.PRE_KEYWORD + "Attributes" + UI.PST_KEYWORD + " <b>+2</b> in the morning");
      }

      public class NIGHTTIME
      {
        public static LocString NAME = (LocString) "Night Owl";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is more efficient on a nighttime " + UI.PRE_KEYWORD + "Schedule" + UI.PST_KEYWORD + "\n• All " + UI.PRE_KEYWORD + "Attributes" + UI.PST_KEYWORD + " <b>+3</b> at night");
      }

      public class SUFFOCATING
      {
        public static LocString NAME = (LocString) "Suffocating";
        public static LocString TOOLTIP = (LocString) "This Duplicant cannot breathe!";
        public static LocString NOTIFICATION_NAME = (LocString) "Suffocating";
        public static LocString NOTIFICATION_TOOLTIP = (LocString) "These Duplicants cannot breathe:";
      }

      public class TIRED
      {
        public static LocString NAME = (LocString) "Tired";
        public static LocString TOOLTIP = (LocString) "This Duplicant could use a nice nap";
      }

      public class IDLE
      {
        public static LocString NAME = (LocString) "Idle";
        public static LocString TOOLTIP = (LocString) "This Duplicant cannot reach any pending errands";
        public static LocString NOTIFICATION_NAME = (LocString) "Idle";
        public static LocString NOTIFICATION_TOOLTIP = (LocString) ("These Duplicants cannot reach any pending " + UI.PRE_KEYWORD + "Errands" + UI.PST_KEYWORD + ":");
      }

      public class FIGHTING
      {
        public static LocString NAME = (LocString) "In combat";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is attacking a " + UI.PRE_KEYWORD + "Critter" + UI.PST_KEYWORD + "!");
        public static LocString NOTIFICATION_NAME = (LocString) "Combat!";
        public static LocString NOTIFICATION_TOOLTIP = (LocString) ("These Duplicants have engaged a " + UI.PRE_KEYWORD + "Critter" + UI.PST_KEYWORD + " in combat:");
      }

      public class FLEEING
      {
        public static LocString NAME = (LocString) "Fleeing";
        public static LocString TOOLTIP = (LocString) "This Duplicant is trying to escape something scary!";
        public static LocString NOTIFICATION_NAME = (LocString) "Fleeing!";
        public static LocString NOTIFICATION_TOOLTIP = (LocString) "These Duplicants are trying to escape:";
      }

      public class DEAD
      {
        public static LocString NAME = (LocString) "Dead: {Death}";
        public static LocString TOOLTIP = (LocString) "This Duplicant definitely isn't sleeping";
      }

      public class LASHINGOUT
      {
        public static LocString NAME = (LocString) "Lashing out";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is breaking buildings to relieve their " + UI.PRE_KEYWORD + "Stress" + UI.PST_KEYWORD);
        public static LocString NOTIFICATION_NAME = (LocString) "Lashing out";
        public static LocString NOTIFICATION_TOOLTIP = (LocString) ("These Duplicants broke buildings to relieve their " + UI.PRE_KEYWORD + "Stress" + UI.PST_KEYWORD + ":");
      }

      public class MOVETOSUITNOTREQUIRED
      {
        public static LocString NAME = (LocString) "Exiting Exosuit area";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is leaving an area where a " + UI.PRE_KEYWORD + "Suit" + UI.PST_KEYWORD + " was required");
      }

      public class NOROLE
      {
        public static LocString NAME = (LocString) "No Job";
        public static LocString TOOLTIP = (LocString) ("This Duplicant does not have a Job Assignment\n\nEnter the " + UI.FormatAsManagementMenu("Jobs Panel", "[J]") + " to view all available Jobs");
      }

      public class DROPPINGUNUSEDINVENTORY
      {
        public static LocString NAME = (LocString) "Dropping objects";
        public static LocString TOOLTIP = (LocString) "This Duplicant is dropping what they're holding";
      }

      public class MOVINGTOSAFEAREA
      {
        public static LocString NAME = (LocString) "Moving to safe area";
        public static LocString TOOLTIP = (LocString) "This Duplicant is finding a less dangerous place";
      }

      public class TOILETUNREACHABLE
      {
        public static LocString NAME = (LocString) "Unreachable toilet";
        public static LocString TOOLTIP = (LocString) ("This Duplicant cannot reach a functioning " + UI.FormatAsLink("Outhouse", "OUTHOUSE") + " or " + UI.FormatAsLink("Lavatory", "FLUSHTOILET"));
        public static LocString NOTIFICATION_NAME = (LocString) "Unreachable toilet";
        public static LocString NOTIFICATION_TOOLTIP = (LocString) ("These Duplicants cannot reach a functioning " + UI.FormatAsLink("Outhouse", "OUTHOUSE") + " or " + UI.FormatAsLink("Lavatory", "FLUSHTOILET") + ":");
      }

      public class NOUSABLETOILETS
      {
        public static LocString NAME = (LocString) "Toilet out of order";
        public static LocString TOOLTIP = (LocString) ("The only " + UI.FormatAsLink("Outhouses", "OUTHOUSE") + " or " + UI.FormatAsLink("Lavatories", "FLUSHTOILET") + " in this Duplicant's reach are out of order");
        public static LocString NOTIFICATION_NAME = (LocString) "Toilet out of order";
        public static LocString NOTIFICATION_TOOLTIP = (LocString) ("These Duplicants want to use an " + UI.FormatAsLink("Outhouse", "OUTHOUSE") + " or " + UI.FormatAsLink("Lavatory", "FLUSHTOILET") + " that is out of order:");
      }

      public class NOTOILETS
      {
        public static LocString NAME = (LocString) "No Outhouses";
        public static LocString TOOLTIP = (LocString) ("There are no " + UI.FormatAsLink("Outhouses", "OUTHOUSE") + " available for this Duplicant\n\n" + UI.FormatAsLink("Outhouses", "OUTHOUSE") + " can be built from the " + UI.FormatAsBuildMenuTab("Plumbing Tab", (Action) 40));
        public static LocString NOTIFICATION_NAME = (LocString) "No Outhouses built";
        public static LocString NOTIFICATION_TOOLTIP = (LocString) (UI.FormatAsLink("Outhouses", "OUTHOUSE") + " can be built from the " + UI.FormatAsBuildMenuTab("Plumbing Tab", (Action) 40) + ".\n\nThese Duplicants are in need of an " + UI.FormatAsLink("Outhouse", "OUTHOUSE") + ":");
      }

      public class FULLBLADDER
      {
        public static LocString NAME = (LocString) "Full bladder";
        public static LocString TOOLTIP = (LocString) ("This Duplicant would really appreciate an " + UI.FormatAsLink("Outhouse", "OUTHOUSE") + " or " + UI.FormatAsLink("Lavatory", "FLUSHTOILET"));
      }

      public class STRESSFULLYEMPTYINGBLADDER
      {
        public static LocString NAME = (LocString) "Making a mess";
        public static LocString TOOLTIP = (LocString) ("This poor Duplicant couldn't find an " + UI.FormatAsLink("Outhouse", "OUTHOUSE") + " in time and is super embarrassed\n\n" + UI.FormatAsLink("Outhouses", "OUTHOUSE") + " can be built from the " + UI.FormatAsBuildMenuTab("Plumbing Tab", (Action) 40));
        public static LocString NOTIFICATION_NAME = (LocString) "Made a mess";
        public static LocString NOTIFICATION_TOOLTIP = (LocString) ("The " + UI.FormatAsTool("Mop Tool", (Action) 150) + " can be used to clean up Duplicant-related \"spills\"\n\nThese Duplicants made messes that require cleaning up:\n");
      }

      public class WASHINGHANDS
      {
        public static LocString NAME = (LocString) "Washing hands";
        public static LocString TOOLTIP = (LocString) "This Duplicant is washing their hands";
      }

      public class SHOWERING
      {
        public static LocString NAME = (LocString) "Showering";
        public static LocString TOOLTIP = (LocString) "This Duplicant is gonna be squeaky clean";
      }

      public class RELAXING
      {
        public static LocString NAME = (LocString) "Relaxing";
        public static LocString TOOLTIP = (LocString) "This Duplicant's just taking it easy";
      }

      public class VOMITING
      {
        public static LocString NAME = (LocString) "Throwing up";
        public static LocString TOOLTIP = (LocString) ("This Duplicant has unceremoniously hurled as the result of a " + UI.FormatAsLink("Disease", "DISEASE") + "\n\nDuplicant-related \"spills\" can be cleaned up using the " + UI.PRE_KEYWORD + "Mop Tool" + UI.PST_KEYWORD + " " + UI.FormatAsHotKey((Action) 150));
        public static LocString NOTIFICATION_NAME = (LocString) "Throwing up";
        public static LocString NOTIFICATION_TOOLTIP = (LocString) ("The " + UI.FormatAsTool("Mop Tool", (Action) 150) + " can be used to clean up Duplicant-related \"spills\"\n\nA " + UI.PRE_KEYWORD + "Disease" + UI.PST_KEYWORD + " has caused these Duplicants to throw up:");
      }

      public class STRESSVOMITING
      {
        public static LocString NAME = (LocString) "Stress vomiting";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is relieving their " + UI.PRE_KEYWORD + "Stress" + UI.PST_KEYWORD + " all over the floor\n\nDuplicant-related \"spills\" can be cleaned up using the " + UI.PRE_KEYWORD + "Mop Tool" + UI.PST_KEYWORD + " " + UI.FormatAsHotKey((Action) 150));
        public static LocString NOTIFICATION_NAME = (LocString) "Stress vomiting";
        public static LocString NOTIFICATION_TOOLTIP = (LocString) ("The " + UI.FormatAsTool("Mop Tool", (Action) 150) + " can used to clean up Duplicant-related \"spills\"\n\nThese Duplicants became so " + UI.PRE_KEYWORD + "Stressed" + UI.PST_KEYWORD + " they threw up:");
      }

      public class RADIATIONVOMITING
      {
        public static LocString NAME = (LocString) "Radiation vomiting";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is sick due to " + UI.PRE_KEYWORD + "Radiation" + UI.PST_KEYWORD + " poisoning.\n\nDuplicant-related \"spills\" can be cleaned up using the " + UI.PRE_KEYWORD + "Mop Tool" + UI.PST_KEYWORD + " " + UI.FormatAsHotKey((Action) 150));
        public static LocString NOTIFICATION_NAME = (LocString) "Radiation vomiting";
        public static LocString NOTIFICATION_TOOLTIP = (LocString) ("The " + UI.FormatAsTool("Mop Tool", (Action) 150) + " can clean up Duplicant-related \"spills\"\n\nRadiation Sickness caused these Duplicants to throw up:");
      }

      public class HASDISEASE
      {
        public static LocString NAME = (LocString) "Feeling ill";
        public static LocString TOOLTIP = (LocString) ("This Duplicant has contracted a " + UI.FormatAsLink("Disease", "DISEASE") + " and requires recovery time at a " + UI.FormatAsLink("Sick Bay", "DOCTORSTATION"));
        public static LocString NOTIFICATION_NAME = (LocString) "Illness";
        public static LocString NOTIFICATION_TOOLTIP = (LocString) ("These Duplicants have contracted a " + UI.FormatAsLink("Disease", "DISEASE") + " and require recovery time at a " + UI.FormatAsLink("Sick Bay", "DOCTORSTATION") + ":");
      }

      public class BODYREGULATINGHEATING
      {
        public static LocString NAME = (LocString) "Regulating temperature at: {TempDelta}";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is regulating their internal " + UI.PRE_KEYWORD + "Temperature" + UI.PST_KEYWORD);
      }

      public class BODYREGULATINGCOOLING
      {
        public static LocString NAME = (LocString) "Regulating temperature at: {TempDelta}";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is regulating their internal " + UI.PRE_KEYWORD + "Temperature" + UI.PST_KEYWORD);
      }

      public class BREATHINGO2
      {
        public static LocString NAME = (LocString) "Inhaling {ConsumptionRate} O<sub>2</sub>";
        public static LocString TOOLTIP = (LocString) ("Duplicants require " + UI.FormatAsLink("Oxygen", "OXYGEN") + " to live");
      }

      public class EMITTINGCO2
      {
        public static LocString NAME = (LocString) "Exhaling {EmittingRate} CO<sub>2</sub>";
        public static LocString TOOLTIP = (LocString) ("Duplicants breathe out " + UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE"));
      }

      public class PICKUPDELIVERSTATUS
      {
        public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;
        public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
      }

      public class STOREDELIVERSTATUS
      {
        public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;
        public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
      }

      public class CLEARDELIVERSTATUS
      {
        public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;
        public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
      }

      public class STOREFORBUILDDELIVERSTATUS
      {
        public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;
        public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
      }

      public class STOREFORBUILDPRIORITIZEDDELIVERSTATUS
      {
        public static LocString NAME = (LocString) "Allocating {Item} to {Target}";
        public static LocString TOOLTIP = (LocString) "This Duplicant is delivering materials to a <b>{Target}</b> construction errand";
      }

      public class BUILDDELIVERSTATUS
      {
        public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;
        public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
      }

      public class BUILDPRIORITIZEDSTATUS
      {
        public static LocString NAME = (LocString) "Building {Target}";
        public static LocString TOOLTIP = (LocString) "This Duplicant is constructing a <b>{Target}</b>";
      }

      public class FABRICATEDELIVERSTATUS
      {
        public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;
        public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
      }

      public class USEITEMDELIVERSTATUS
      {
        public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;
        public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
      }

      public class STOREPRIORITYDELIVERSTATUS
      {
        public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;
        public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
      }

      public class STORECRITICALDELIVERSTATUS
      {
        public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;
        public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
      }

      public class COMPOSTFLIPSTATUS
      {
        public static LocString NAME = (LocString) "Going to flip compost";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is going to flip the " + (string) BUILDINGS.PREFABS.COMPOST.NAME);
      }

      public class DECONSTRUCTDELIVERSTATUS
      {
        public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;
        public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
      }

      public class TOGGLEDELIVERSTATUS
      {
        public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;
        public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
      }

      public class EMPTYSTORAGEDELIVERSTATUS
      {
        public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;
        public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
      }

      public class HARVESTDELIVERSTATUS
      {
        public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;
        public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
      }

      public class SLEEPDELIVERSTATUS
      {
        public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;
        public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
      }

      public class EATDELIVERSTATUS
      {
        public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;
        public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
      }

      public class WARMUPDELIVERSTATUS
      {
        public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;
        public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
      }

      public class REPAIRDELIVERSTATUS
      {
        public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;
        public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
      }

      public class REPAIRWORKSTATUS
      {
        public static LocString NAME = (LocString) "Repairing {Target}";
        public static LocString TOOLTIP = (LocString) "This Duplicant is fixing the <b>{Target}</b>";
      }

      public class BREAKDELIVERSTATUS
      {
        public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;
        public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
      }

      public class BREAKWORKSTATUS
      {
        public static LocString NAME = (LocString) "Breaking {Target}";
        public static LocString TOOLTIP = (LocString) "This Duplicant is going totally bananas on the <b>{Target}</b>!";
      }

      public class EQUIPDELIVERSTATUS
      {
        public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;
        public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
      }

      public class COOKDELIVERSTATUS
      {
        public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;
        public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
      }

      public class MUSHDELIVERSTATUS
      {
        public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;
        public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
      }

      public class PACIFYDELIVERSTATUS
      {
        public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;
        public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
      }

      public class RESCUEDELIVERSTATUS
      {
        public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;
        public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
      }

      public class RESCUEWORKSTATUS
      {
        public static LocString NAME = (LocString) "Rescuing {Target}";
        public static LocString TOOLTIP = (LocString) "This Duplicant is saving <b>{Target}</b> from certain peril!";
      }

      public class MOPDELIVERSTATUS
      {
        public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;
        public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
      }

      public class DIGGING
      {
        public static LocString NAME = (LocString) "Digging";
        public static LocString TOOLTIP = (LocString) "This Duplicant is excavating raw resources";
      }

      public class EATING
      {
        public static LocString NAME = (LocString) "Eating {Target}";
        public static LocString TOOLTIP = (LocString) "This Duplicant is having a meal";
      }

      public class CLEANING
      {
        public static LocString NAME = (LocString) "Cleaning {Target}";
        public static LocString TOOLTIP = (LocString) "This Duplicant is cleaning the <b>{Target}</b>";
      }

      public class LIGHTWORKEFFICIENCYBONUS
      {
        public static LocString NAME = (LocString) "Lit Workspace";
        public static LocString TOOLTIP = (LocString) ("Better visibility from the " + UI.PRE_KEYWORD + "Light" + UI.PST_KEYWORD + " is allowing this Duplicant to work faster:\n    {0}");
        public static LocString NO_BUILDING_WORK_ATTRIBUTE = (LocString) "{0} Speed";
      }

      public class LABORATORYWORKEFFICIENCYBONUS
      {
        public static LocString NAME = (LocString) "Lab Workspace";
        public static LocString TOOLTIP = (LocString) "Working in a Laboratory is allowing this Duplicant to work faster:\n    {0}";
        public static LocString NO_BUILDING_WORK_ATTRIBUTE = (LocString) "{0} Speed";
      }

      public class PICKINGUP
      {
        public static LocString NAME = (LocString) "Picking up {Target}";
        public static LocString TOOLTIP = (LocString) "This Duplicant is retrieving <b>{Target}</b>";
      }

      public class MOPPING
      {
        public static LocString NAME = (LocString) "Mopping";
        public static LocString TOOLTIP = (LocString) "This Duplicant is cleaning up a nasty spill";
      }

      public class ARTING
      {
        public static LocString NAME = (LocString) "Decorating";
        public static LocString TOOLTIP = (LocString) "This Duplicant is hard at work on their art";
      }

      public class MUSHING
      {
        public static LocString NAME = (LocString) "Mushing {Item}";
        public static LocString TOOLTIP = (LocString) "This Duplicant is cooking a <b>{Item}</b>";
      }

      public class COOKING
      {
        public static LocString NAME = (LocString) "Cooking {Item}";
        public static LocString TOOLTIP = (LocString) "This Duplicant is cooking up a tasty <b>{Item}</b>";
      }

      public class RESEARCHING
      {
        public static LocString NAME = (LocString) "Researching {Tech}";
        public static LocString TOOLTIP = (LocString) "This Duplicant is intently researching <b>{Tech}</b> technology";
      }

      public class MISSIONCONTROLLING
      {
        public static LocString NAME = (LocString) "Mission Controlling";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is guiding a " + UI.PRE_KEYWORD + "Rocket" + UI.PST_KEYWORD);
      }

      public class STORING
      {
        public static LocString NAME = (LocString) "Storing {Item}";
        public static LocString TOOLTIP = (LocString) "This Duplicant is putting <b>{Item}</b> away in <b>{Target}</b>";
      }

      public class BUILDING
      {
        public static LocString NAME = (LocString) "Building {Target}";
        public static LocString TOOLTIP = (LocString) "This Duplicant is constructing a <b>{Target}</b>";
      }

      public class EQUIPPING
      {
        public static LocString NAME = (LocString) "Equipping {Target}";
        public static LocString TOOLTIP = (LocString) "This Duplicant is equipping a <b>{Target}</b>";
      }

      public class WARMINGUP
      {
        public static LocString NAME = (LocString) "Warming up";
        public static LocString TOOLTIP = (LocString) "This Duplicant got too cold and is trying to warm up";
      }

      public class GENERATINGPOWER
      {
        public static LocString NAME = (LocString) "Generating power";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is using the <b>{Target}</b> to produce electrical " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD);
      }

      public class HARVESTING
      {
        public static LocString NAME = (LocString) "Harvesting {Target}";
        public static LocString TOOLTIP = (LocString) "This Duplicant is gathering resources from a <b>{Target}</b>";
      }

      public class UPROOTING
      {
        public static LocString NAME = (LocString) "Uprooting {Target}";
        public static LocString TOOLTIP = (LocString) "This Duplicant is digging up a <b>{Target}</b>";
      }

      public class EMPTYING
      {
        public static LocString NAME = (LocString) "Emptying {Target}";
        public static LocString TOOLTIP = (LocString) "This Duplicant is removing materials from the <b>{Target}</b>";
      }

      public class TOGGLING
      {
        public static LocString NAME = (LocString) "Change {Target} setting";
        public static LocString TOOLTIP = (LocString) "This Duplicant is changing the <b>{Target}</b>'s setting";
      }

      public class DECONSTRUCTING
      {
        public static LocString NAME = (LocString) "Deconstructing {Target}";
        public static LocString TOOLTIP = (LocString) "This Duplicant is deconstructing the <b>{Target}</b>";
      }

      public class DEMOLISHING
      {
        public static LocString NAME = (LocString) "Demolishing {Target}";
        public static LocString TOOLTIP = (LocString) "This Duplicant is demolishing the <b>{Target}</b>";
      }

      public class DISINFECTING
      {
        public static LocString NAME = (LocString) "Disinfecting {Target}";
        public static LocString TOOLTIP = (LocString) "This Duplicant is disinfecting <b>{Target}</b>";
      }

      public class FABRICATING
      {
        public static LocString NAME = (LocString) "Fabricating {Item}";
        public static LocString TOOLTIP = (LocString) "This Duplicant is crafting a <b>{Item}</b>";
      }

      public class PROCESSING
      {
        public static LocString NAME = (LocString) "Refining {Item}";
        public static LocString TOOLTIP = (LocString) "This Duplicant is refining <b>{Item}</b>";
      }

      public class SPICING
      {
        public static LocString NAME = (LocString) "Spicing Food";
        public static LocString TOOLTIP = (LocString) "This Duplicant is adding spice to a meal";
      }

      public class CLEARING
      {
        public static LocString NAME = (LocString) "Sweeping {Target}";
        public static LocString TOOLTIP = (LocString) "This Duplicant is sweeping away <b>{Target}</b>";
      }

      public class STUDYING
      {
        public static LocString NAME = (LocString) "Analyzing";
        public static LocString TOOLTIP = (LocString) "This Duplicant is conducting a field study of a Natural Feature";
      }

      public class SOCIALIZING
      {
        public static LocString NAME = (LocString) "Socializing";
        public static LocString TOOLTIP = (LocString) "This Duplicant is using their break to hang out";
      }

      public class MINGLING
      {
        public static LocString NAME = (LocString) "Mingling";
        public static LocString TOOLTIP = (LocString) "This Duplicant is using their break to chat with friends";
      }

      public class NOISEPEACEFUL
      {
        public static LocString NAME = (LocString) "Peace and Quiet";
        public static LocString TOOLTIP = (LocString) "This Duplicant has found a quiet place to concentrate";
      }

      public class NOISEMINOR
      {
        public static LocString NAME = (LocString) "Loud Noises";
        public static LocString TOOLTIP = (LocString) "This area is a bit too loud for comfort";
      }

      public class NOISEMAJOR
      {
        public static LocString NAME = (LocString) "Cacophony!";
        public static LocString TOOLTIP = (LocString) "It's very, very loud in here!";
      }

      public class LOWIMMUNITY
      {
        public static LocString NAME = (LocString) "Under the Weather";
        public static LocString TOOLTIP = (LocString) "This Duplicant has a weakened immune system and will become ill if it reaches zero";
        public static LocString NOTIFICATION_NAME = (LocString) "Low Immunity";
        public static LocString NOTIFICATION_TOOLTIP = (LocString) "These Duplicants are at risk of becoming sick:";
      }

      public abstract class TINKERING
      {
        public static LocString NAME = (LocString) "Tinkering";
        public static LocString TOOLTIP = (LocString) "This Duplicant is making functional improvements to a building";
      }

      public class CONTACTWITHGERMS
      {
        public static LocString NAME = (LocString) "Contact with {Sickness} Germs";
        public static LocString TOOLTIP = (LocString) ("This Duplicant has encountered {Sickness} Germs and is at risk of dangerous exposure if contact continues\n\n<i>" + UI.CLICK(UI.ClickType.Click) + " to jump to last contact location</i>");
      }

      public class EXPOSEDTOGERMS
      {
        public static LocString TIER1 = (LocString) "Mild Exposure";
        public static LocString TIER2 = (LocString) "Medium Exposure";
        public static LocString TIER3 = (LocString) "Exposure";
        public static readonly LocString[] EXPOSURE_TIERS = new LocString[3]
        {
          DUPLICANTS.STATUSITEMS.EXPOSEDTOGERMS.TIER1,
          DUPLICANTS.STATUSITEMS.EXPOSEDTOGERMS.TIER2,
          DUPLICANTS.STATUSITEMS.EXPOSEDTOGERMS.TIER3
        };
        public static LocString NAME = (LocString) "{Severity} to {Sickness} Germs";
        public static LocString TOOLTIP = (LocString) ("This Duplicant has been exposed to a concentration of {Sickness} Germs and is at risk of waking up sick on their next shift\n\nExposed {Source}\n\nRate of Contracting {Sickness}: {Chance}\n\nResistance Rating: {Total}\n    • Base {Sickness} Resistance: {Base}\n    • " + (string) DUPLICANTS.ATTRIBUTES.GERMRESISTANCE.NAME + ": {Dupe}\n    • {Severity} Exposure: {ExposureLevelBonus}\n\n<i>" + UI.CLICK(UI.ClickType.Click) + " to jump to last exposure location</i>");
      }

      public class GASLIQUIDEXPOSURE
      {
        public static LocString NAME_MINOR = (LocString) "Eye Irritation";
        public static LocString NAME_MAJOR = (LocString) "Major Eye Irritation";
        public static LocString TOOLTIP = (LocString) "Ah, it stings!\n\nThis poor Duplicant got a faceful of an irritating gas or liquid";
        public static LocString TOOLTIP_EXPOSED = (LocString) "Current exposure to {element} is {rate} eye irritation";
        public static LocString TOOLTIP_RATE_INCREASE = (LocString) "increasing";
        public static LocString TOOLTIP_RATE_DECREASE = (LocString) "decreasing";
        public static LocString TOOLTIP_RATE_STAYS = (LocString) "maintaining";
        public static LocString TOOLTIP_EXPOSURE_LEVEL = (LocString) "Time Remaining: {time}";
      }

      public class BEINGPRODUCTIVE
      {
        public static LocString NAME = (LocString) "Super Focused";
        public static LocString TOOLTIP = (LocString) "This Duplicant is focused on being super productive right now";
      }

      public class BALLOONARTISTPLANNING
      {
        public static LocString NAME = (LocString) "Balloon Artist";
        public static LocString TOOLTIP = (LocString) "This Duplicant is planning to hand out balloons in their downtime";
      }

      public class BALLOONARTISTHANDINGOUT
      {
        public static LocString NAME = (LocString) "Balloon Artist";
        public static LocString TOOLTIP = (LocString) "This Duplicant is handing out balloons to other Duplicants";
      }

      public class EXPELLINGRADS
      {
        public static LocString NAME = (LocString) "Cleansing Rads";
        public static LocString TOOLTIP = (LocString) "This Duplicant is, uh... \"expelling\" absorbed radiation from their system";
      }

      public class ANALYZINGGENES
      {
        public static LocString NAME = (LocString) "Analyzing Plant Genes";
        public static LocString TOOLTIP = (LocString) "This Duplicant is peering deep into the genetic code of an odd seed";
      }

      public class ANALYZINGARTIFACT
      {
        public static LocString NAME = (LocString) "Analyzing Artifact";
        public static LocString TOOLTIP = (LocString) "This Duplicant is studying an artifact";
      }

      public class RANCHING
      {
        public static LocString NAME = (LocString) "Ranching";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is tending to a " + UI.PRE_KEYWORD + "Critter" + UI.PST_KEYWORD + "'s well-being");
      }
    }

    public class DISEASES
    {
      public static LocString CURED_POPUP = (LocString) "Cured of {0}";
      public static LocString INFECTED_POPUP = (LocString) "Became infected by {0}";
      public static LocString ADDED_POPFX = (LocString) "{0}: {1} Germs";
      public static LocString NOTIFICATION_TOOLTIP = (LocString) "{0} contracted {1} from: {2}";
      public static LocString GERMS = (LocString) "Germs";
      public static LocString GERMS_CONSUMED_DESCRIPTION = (LocString) "A count of the number of germs this Duplicant is host to";
      public static LocString RECUPERATING = (LocString) "Recuperating";
      public static LocString INFECTION_MODIFIER = (LocString) "Recently consumed {0} ({1})";
      public static LocString INFECTION_MODIFIER_SOURCE = (LocString) "Fighting off {0} from {1}";
      public static LocString INFECTED_MODIFIER = (LocString) "Suppressed immune system";
      public static LocString LEGEND_POSTAMBLE = (LocString) "\n•  Select an infected object for more details";
      public static LocString ATTRIBUTE_MODIFIER_SYMPTOMS = (LocString) "{0}: {1}";
      public static LocString ATTRIBUTE_MODIFIER_SYMPTOMS_TOOLTIP = (LocString) "Modifies {0} by {1}";
      public static LocString DEATH_SYMPTOM = (LocString) "Death in {0} if untreated";
      public static LocString DEATH_SYMPTOM_TOOLTIP = (LocString) "Without medical treatment, this Duplicant will die of their illness in {0}";
      public static LocString RESISTANCES_PANEL_TOOLTIP = (LocString) "{0}";
      public static LocString IMMUNE_FROM_MISSING_REQUIRED_TRAIT = (LocString) "Immune: Does not have {0}";
      public static LocString IMMUNE_FROM_HAVING_EXLCLUDED_TRAIT = (LocString) "Immune: Has {0}";
      public static LocString IMMUNE_FROM_HAVING_EXCLUDED_EFFECT = (LocString) "Immunity: Has {0}";
      public static LocString CONTRACTION_PROBABILITY = (LocString) "{0} of {1}'s exposures to these germs will result in {2}";

      public class STATUS_ITEM_TOOLTIP
      {
        public static LocString TEMPLATE = (LocString) "{InfectionSource}{Duration}{Doctor}{Fatality}{Cures}{Bedrest}\n\n\n{Symptoms}";
        public static LocString DESCRIPTOR = (LocString) "<b>{0} {1}</b>\n";
        public static LocString SYMPTOMS = (LocString) "{0}\n";
        public static LocString INFECTION_SOURCE = (LocString) "Contracted by: {0}\n";
        public static LocString DURATION = (LocString) "Time to recovery: {0}\n";
        public static LocString CURES = (LocString) "Remedies taken: {0}\n";
        public static LocString NOMEDICINETAKEN = (LocString) "Remedies taken: None\n";
        public static LocString FATALITY = (LocString) "Fatal if untreated in: {0}\n";
        public static LocString BEDREST = (LocString) "Sick Bay assignment will allow faster recovery\n";
        public static LocString DOCTOR_REQUIRED = (LocString) "Sick Bay assignment required for recovery\n";
        public static LocString DOCTORED = (LocString) "Received medical treatment, recovery speed is increased\n";
      }

      public class MEDICINE
      {
        public static LocString SELF_ADMINISTERED_BOOSTER = (LocString) "Self-Administered: Anytime";
        public static LocString SELF_ADMINISTERED_BOOSTER_TOOLTIP = (LocString) "Duplicants can give themselves this medicine, whether they are currently sick or not";
        public static LocString SELF_ADMINISTERED_CURE = (LocString) "Self-Administered: Sick Only";
        public static LocString SELF_ADMINISTERED_CURE_TOOLTIP = (LocString) "Duplicants can give themselves this medicine, but only while they are sick";
        public static LocString DOCTOR_ADMINISTERED_BOOSTER = (LocString) "Doctor Administered: Anytime";
        public static LocString DOCTOR_ADMINISTERED_BOOSTER_TOOLTIP = (LocString) ("Duplicants can receive this medicine at a {Station}, whether they are currently sick or not\n\nThey cannot give it to themselves and must receive it from a friend with " + UI.PRE_KEYWORD + "Doctoring Skills" + UI.PST_KEYWORD);
        public static LocString DOCTOR_ADMINISTERED_CURE = (LocString) "Doctor Administered: Sick Only";
        public static LocString DOCTOR_ADMINISTERED_CURE_TOOLTIP = (LocString) ("Duplicants can receive this medicine at a {Station}, but only while they are sick\n\nThey cannot give it to themselves and must receive it from a friend with " + UI.PRE_KEYWORD + "Doctoring Skills" + UI.PST_KEYWORD);
        public static LocString BOOSTER = (LocString) UI.FormatAsLink("Immune Booster", "IMMUNE SYSTEM");
        public static LocString BOOSTER_TOOLTIP = (LocString) "Boosters can be taken by both healthy and sick Duplicants to prevent potential disease";
        public static LocString CURES_ANY = (LocString) ("Alleviates " + UI.FormatAsLink("All Diseases", "DISEASE"));
        public static LocString CURES_ANY_TOOLTIP = (LocString) ("This is a nonspecific " + UI.PRE_KEYWORD + "Disease" + UI.PST_KEYWORD + " treatment that can be taken by any sick Duplicant");
        public static LocString CURES = (LocString) "Alleviates {0}";
        public static LocString CURES_TOOLTIP = (LocString) "This medicine is used to treat {0} and can only be taken by sick Duplicants";
      }

      public class SEVERITY
      {
        public static LocString BENIGN = (LocString) "Benign";
        public static LocString MINOR = (LocString) "Minor";
        public static LocString MAJOR = (LocString) "Major";
        public static LocString CRITICAL = (LocString) "Critical";
      }

      public class TYPE
      {
        public static LocString PATHOGEN = (LocString) "Illness";
        public static LocString AILMENT = (LocString) "Ailment";
        public static LocString INJURY = (LocString) "Injury";
      }

      public class TRIGGERS
      {
        public static LocString EATCOMPLETEEDIBLE = (LocString) "May cause {Diseases}";

        public class TOOLTIPS
        {
          public static LocString EATCOMPLETEEDIBLE = (LocString) "May cause {Diseases}";
        }
      }

      public class INFECTIONSOURCES
      {
        public static LocString INTERNAL_TEMPERATURE = (LocString) "Extreme internal temperatures";
        public static LocString TOXIC_AREA = (LocString) "Exposure to toxic areas";
        public static LocString FOOD = (LocString) "Eating a germ-covered {0}";
        public static LocString AIR = (LocString) "Breathing germ-filled {0}";
        public static LocString SKIN = (LocString) "Skin contamination";
        public static LocString UNKNOWN = (LocString) "Unknown source";
      }

      public class DESCRIPTORS
      {
        public class INFO
        {
          public static LocString FOODBORNE = (LocString) ("Contracted via ingestion\n" + UI.HORIZONTAL_RULE);
          public static LocString FOODBORNE_TOOLTIP = (LocString) ("Duplicants may contract this " + UI.PRE_KEYWORD + "Disease" + UI.PST_KEYWORD + " by ingesting " + UI.PRE_KEYWORD + "Food" + UI.PST_KEYWORD + " contaminated with these " + UI.PRE_KEYWORD + "Germs" + UI.PST_KEYWORD);
          public static LocString AIRBORNE = (LocString) ("Contracted via inhalation\n" + UI.HORIZONTAL_RULE);
          public static LocString AIRBORNE_TOOLTIP = (LocString) ("Duplicants may contract this " + UI.PRE_KEYWORD + "Disease" + UI.PST_KEYWORD + " by breathing " + (string) ELEMENTS.OXYGEN.NAME + " containing these " + UI.PRE_KEYWORD + "Germs" + UI.PST_KEYWORD);
          public static LocString SKINBORNE = (LocString) ("Contracted via physical contact\n" + UI.HORIZONTAL_RULE);
          public static LocString SKINBORNE_TOOLTIP = (LocString) ("Duplicants may contract this " + UI.PRE_KEYWORD + "Disease" + UI.PST_KEYWORD + " by touching objects contaminated with these " + UI.PRE_KEYWORD + "Germs" + UI.PST_KEYWORD);
          public static LocString SUNBORNE = (LocString) ("Contracted via environmental exposure\n" + UI.HORIZONTAL_RULE);
          public static LocString SUNBORNE_TOOLTIP = (LocString) ("Duplicants may contract this " + UI.PRE_KEYWORD + "Disease" + UI.PST_KEYWORD + " through exposure to hazardous environments");
          public static LocString GROWS_ON = (LocString) "Multiplies in:";
          public static LocString GROWS_ON_TOOLTIP = (LocString) ("These substances allow " + UI.PRE_KEYWORD + "Germs" + UI.PST_KEYWORD + " to spread and reproduce");
          public static LocString NEUTRAL_ON = (LocString) "Survives in:";
          public static LocString NEUTRAL_ON_TOOLTIP = (LocString) (UI.PRE_KEYWORD + "Germs" + UI.PST_KEYWORD + " will survive contact with these substances, but will not reproduce");
          public static LocString DIES_SLOWLY_ON = (LocString) "Inhibited by:";
          public static LocString DIES_SLOWLY_ON_TOOLTIP = (LocString) ("Contact with these substances will slowly reduce " + UI.PRE_KEYWORD + "Germ" + UI.PST_KEYWORD + " numbers");
          public static LocString DIES_ON = (LocString) "Killed by:";
          public static LocString DIES_ON_TOOLTIP = (LocString) ("Contact with these substances kills " + UI.PRE_KEYWORD + "Germs" + UI.PST_KEYWORD + " over time");
          public static LocString DIES_QUICKLY_ON = (LocString) "Disinfected by:";
          public static LocString DIES_QUICKLY_ON_TOOLTIP = (LocString) ("Contact with these substances will quickly kill these " + UI.PRE_KEYWORD + "Germs" + UI.PST_KEYWORD);
          public static LocString GROWS = (LocString) "Multiplies";
          public static LocString GROWS_TOOLTIP = (LocString) "Doubles germ count every {0}";
          public static LocString NEUTRAL = (LocString) "Survives";
          public static LocString NEUTRAL_TOOLTIP = (LocString) "Germ count remains static";
          public static LocString DIES_SLOWLY = (LocString) "Inhibited";
          public static LocString DIES_SLOWLY_TOOLTIP = (LocString) "Halves germ count every {0}";
          public static LocString DIES = (LocString) "Dies";
          public static LocString DIES_TOOLTIP = (LocString) "Halves germ count every {0}";
          public static LocString DIES_QUICKLY = (LocString) "Disinfected";
          public static LocString DIES_QUICKLY_TOOLTIP = (LocString) "Halves germ count every {0}";
          public static LocString GROWTH_FORMAT = (LocString) "    • {0}";
          public static LocString TEMPERATURE_RANGE = (LocString) "Temperature range: {0} to {1}";
          public static LocString TEMPERATURE_RANGE_TOOLTIP = (LocString) ("These " + UI.PRE_KEYWORD + "Germs" + UI.PST_KEYWORD + " can survive " + UI.PRE_KEYWORD + "Temperatures" + UI.PST_KEYWORD + " between <b>{0}</b> and <b>{1}</b>\n\nThey thrive in " + UI.PRE_KEYWORD + "Temperatures" + UI.PST_KEYWORD + " between <b>{2}</b> and <b>{3}</b>");
          public static LocString PRESSURE_RANGE = (LocString) "Pressure range: {0} to {1}\n";
          public static LocString PRESSURE_RANGE_TOOLTIP = (LocString) ("These " + UI.PRE_KEYWORD + "Germs" + UI.PST_KEYWORD + " can survive between <b>{0}</b> and <b>{1}</b> of pressure\n\nThey thrive in pressures between <b>{2}</b> and <b>{3}</b>");
        }
      }

      public class ALLDISEASES
      {
        public static LocString NAME = (LocString) "All Diseases";
      }

      public class NODISEASES
      {
        public static LocString NAME = (LocString) "NO";
      }

      public class FOODPOISONING
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Food Poisoning", nameof (FOODPOISONING));
        public static LocString LEGEND_HOVERTEXT = (LocString) "Food Poisoning Germs present\n";
        public static LocString DESC = (LocString) "Food and drinks tainted with Food Poisoning germs are unsafe to consume, as they cause vomiting and other...bodily unpleasantness.";
      }

      public class SLIMELUNG
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Slimelung", nameof (SLIMELUNG));
        public static LocString LEGEND_HOVERTEXT = (LocString) "Slimelung Germs present\n";
        public static LocString DESC = (LocString) ("Slimelung germs are found in " + UI.FormatAsLink("Slime", "SLIMEMOLD") + " and " + UI.FormatAsLink("Polluted Oxygen", "CONTAMINATEDOXYGEN") + ". Inhaling these germs can cause Duplicants to cough and struggle to breathe.");
      }

      public class POLLENGERMS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Floral Scent", nameof (POLLENGERMS));
        public static LocString LEGEND_HOVERTEXT = (LocString) "Floral Scent allergens present\n";
        public static LocString DESC = (LocString) "Floral Scent allergens trigger excessive sneezing fits in Duplicants who possess the Allergies trait.";
      }

      public class ZOMBIESPORES
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Zombie Spores", nameof (ZOMBIESPORES));
        public static LocString LEGEND_HOVERTEXT = (LocString) "Zombie Spores present\n";
        public static LocString DESC = (LocString) ("Zombie Spores are a parasitic brain fungus released by " + UI.FormatAsLink("Sporechids", "EVIL_FLOWER") + ". Duplicants who touch or inhale the spores risk becoming infected and temporarily losing motor control.");
      }

      public class RADIATIONPOISONING
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Radioactive Contamination", nameof (RADIATIONPOISONING));
        public static LocString LEGEND_HOVERTEXT = (LocString) "Radioactive contamination present\n";
        public static LocString DESC = (LocString) ("Items tainted with Radioactive Contaminants emit low levels of " + UI.FormatAsLink("Radiation", "RADIATION") + " that can cause " + UI.FormatAsLink("Radiation Sickness", "RADIATIONSICKNESS") + ". They are unaffected by pressure or temperature, but do degrade over time.");
      }

      public class FOODSICKNESS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Food Poisoning", nameof (FOODSICKNESS));
        public static LocString DESCRIPTION = (LocString) "This Duplicant's last meal wasn't exactly food safe";
        public static LocString VOMIT_SYMPTOM = (LocString) "Vomiting";
        public static LocString VOMIT_SYMPTOM_TOOLTIP = (LocString) ("Duplicants periodically vomit throughout the day, producing additional " + UI.PRE_KEYWORD + "Germs" + UI.PST_KEYWORD + " and losing " + UI.PRE_KEYWORD + "Calories" + UI.PST_KEYWORD);
        public static LocString DESCRIPTIVE_SYMPTOMS = (LocString) "Nonlethal. A Duplicant's body \"purges\" from both ends, causing extreme fatigue.";
        public static LocString DISEASE_SOURCE_DESCRIPTOR = (LocString) "Currently infected with {2}.\n\nThis Duplicant will produce {1} when vomiting.";
        public static LocString DISEASE_SOURCE_DESCRIPTOR_TOOLTIP = (LocString) ("This Duplicant will vomit approximately every <b>{0}</b>\n\nEach time they vomit, they will release <b>{1}</b> and lose " + UI.PRE_KEYWORD + "Calories" + UI.PST_KEYWORD);
      }

      public class SLIMESICKNESS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Slimelung", nameof (SLIMESICKNESS));
        public static LocString DESCRIPTION = (LocString) "This Duplicant's chest congestion is making it difficult to breathe";
        public static LocString COUGH_SYMPTOM = (LocString) "Coughing";
        public static LocString COUGH_SYMPTOM_TOOLTIP = (LocString) ("Duplicants periodically cough up " + (string) ELEMENTS.CONTAMINATEDOXYGEN.NAME + ", producing additional " + UI.PRE_KEYWORD + "Germs" + UI.PST_KEYWORD);
        public static LocString DESCRIPTIVE_SYMPTOMS = (LocString) "Lethal without medical treatment. Duplicants experience coughing and shortness of breath.";
        public static LocString DISEASE_SOURCE_DESCRIPTOR = (LocString) "Currently infected with {2}.\n\nThis Duplicant will produce <b>{1}</b> when coughing.";
        public static LocString DISEASE_SOURCE_DESCRIPTOR_TOOLTIP = (LocString) "This Duplicant will cough approximately every <b>{0}</b>\n\nEach time they cough, they will release <b>{1}</b>";
      }

      public class ZOMBIESICKNESS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Zombie Spores", nameof (ZOMBIESICKNESS));
        public static LocString DESCRIPTIVE_SYMPTOMS = (LocString) "Duplicants lose much of their motor control and experience extreme discomfort.";
        public static LocString DESCRIPTION = (LocString) "Fungal spores have infiltrated the Duplicant's head and are sending unnatural electrical impulses to their brain";
        public static LocString LEGEND_HOVERTEXT = (LocString) "Area Causes Zombie Spores\n";
      }

      public class ALLERGIES
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Allergic Reaction", nameof (ALLERGIES));
        public static LocString DESCRIPTIVE_SYMPTOMS = (LocString) "Allergens cause excessive sneezing fits";
        public static LocString DESCRIPTION = (LocString) "Pollen and other irritants are causing this poor Duplicant's immune system to overreact, resulting in needless sneezing and congestion";
      }

      public class COLDSICKNESS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Hypothermia", nameof (COLDSICKNESS));
        public static LocString DESCRIPTIVE_SYMPTOMS = (LocString) "Nonlethal. Duplicants experience extreme body heat loss causing chills and discomfort.";
        public static LocString DESCRIPTION = (LocString) "This Duplicant's thought processes have been slowed to a crawl from extreme cold exposure";
        public static LocString LEGEND_HOVERTEXT = (LocString) "Area Causes Hypothermia\n";
      }

      public class SUNBURNSICKNESS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Sunburn", nameof (SUNBURNSICKNESS));
        public static LocString DESCRIPTION = (LocString) "Extreme sun exposure has given this Duplicant a nasty burn.";
        public static LocString LEGEND_HOVERTEXT = (LocString) "Area Causes Sunburn\n";
        public static LocString SUNEXPOSURE = (LocString) "Sun Exposure";
        public static LocString DESCRIPTIVE_SYMPTOMS = (LocString) "Nonlethal. Duplicants experience temporary discomfort due to dermatological damage.";
      }

      public class HEATSICKNESS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Heat Stroke", nameof (HEATSICKNESS));
        public static LocString DESCRIPTIVE_SYMPTOMS = (LocString) "Nonlethal. Duplicants experience high fever and discomfort.";
        public static LocString DESCRIPTION = (LocString) ("This Duplicant's thought processes have short circuited from extreme " + UI.PRE_KEYWORD + "Heat" + UI.PST_KEYWORD + " exposure");
        public static LocString LEGEND_HOVERTEXT = (LocString) "Area Causes Heat Stroke\n";
      }

      public class RADIATIONSICKNESS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Radioactive Contaminants", nameof (RADIATIONSICKNESS));
        public static LocString DESCRIPTIVE_SYMPTOMS = (LocString) "Extremely lethal. This Duplicant is not expected to survive.";
        public static LocString DESCRIPTION = (LocString) ("This Duplicant is leaving a trail of " + UI.PRE_KEYWORD + "Radiation" + UI.PST_KEYWORD + " behind them.");
        public static LocString LEGEND_HOVERTEXT = (LocString) "Area Causes Radiation Sickness\n";
        public static LocString DESC = DUPLICANTS.DISEASES.RADIATIONPOISONING.DESC;
      }

      public class PUTRIDODOUR
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Trench Stench", nameof (PUTRIDODOUR));
        public static LocString DESCRIPTION = (LocString) "\nThe pungent odor wafting off this Duplicant is nauseating to their peers";
        public static LocString CRINGE_EFFECT = (LocString) "Smelled a putrid odor";
        public static LocString LEGEND_HOVERTEXT = (LocString) "Trench Stench Germs Present\n";
      }
    }

    public class MODIFIERS
    {
      public static LocString MODIFIER_FORMAT = (LocString) (UI.PRE_KEYWORD + "{0}" + UI.PST_KEYWORD + ": {1}");
      public static LocString TIME_REMAINING = (LocString) "Time Remaining: {0}";
      public static LocString TIME_TOTAL = (LocString) "\nDuration: {0}";
      public static LocString EFFECT_HEADER = (LocString) (UI.PRE_POS_MODIFIER + "Effects:" + UI.PST_POS_MODIFIER);

      public class SKILLLEVEL
      {
        public static LocString NAME = (LocString) "Skill Level";
      }

      public class ROOMPARK
      {
        public static LocString NAME = (LocString) "Park";
        public static LocString TOOLTIP = (LocString) "This Duplicant recently passed through a Park\n\nWow, nature sure is neat!";
      }

      public class ROOMNATURERESERVE
      {
        public static LocString NAME = (LocString) "Nature Reserve";
        public static LocString TOOLTIP = (LocString) "This Duplicant recently passed through a splendid Nature Reserve\n\nWow, nature sure is neat!";
      }

      public class ROOMLATRINE
      {
        public static LocString NAME = (LocString) "Latrine";
        public static LocString TOOLTIP = (LocString) ("This Duplicant used an " + (string) BUILDINGS.PREFABS.OUTHOUSE.NAME + " in a " + UI.PRE_KEYWORD + "Latrine" + UI.PST_KEYWORD);
      }

      public class ROOMBATHROOM
      {
        public static LocString NAME = (LocString) "Washroom";
        public static LocString TOOLTIP = (LocString) ("This Duplicant used a " + (string) BUILDINGS.PREFABS.FLUSHTOILET.NAME + " in a " + UI.PRE_KEYWORD + "Washroom" + UI.PST_KEYWORD);
      }

      public class ROOMBARRACKS
      {
        public static LocString NAME = (LocString) "Barracks";
        public static LocString TOOLTIP = (LocString) ("This Duplicant slept in the " + UI.PRE_KEYWORD + "Barracks" + UI.PST_KEYWORD + " last night and feels refreshed");
      }

      public class ROOMBEDROOM
      {
        public static LocString NAME = (LocString) "Bedroom";
        public static LocString TOOLTIP = (LocString) ("This Duplicant slept in a private " + UI.PRE_KEYWORD + "Bedroom" + UI.PST_KEYWORD + " last night and feels extra refreshed");
      }

      public class BEDHEALTH
      {
        public static LocString NAME = (LocString) "Bed Rest";
        public static LocString TOOLTIP = (LocString) ("This Duplicant will incrementally heal over while on " + UI.PRE_KEYWORD + "Bed Rest" + UI.PST_KEYWORD);
      }

      public class BEDSTAMINA
      {
        public static LocString NAME = (LocString) "Sleeping in a cot";
        public static LocString TOOLTIP = (LocString) "This Duplicant's sleeping arrangements are adequate";
      }

      public class LUXURYBEDSTAMINA
      {
        public static LocString NAME = (LocString) "Sleeping in a comfy bed";
        public static LocString TOOLTIP = (LocString) "This Duplicant loves their snuggly bed";
      }

      public class BARRACKSSTAMINA
      {
        public static LocString NAME = (LocString) "Barracks";
        public static LocString TOOLTIP = (LocString) "This Duplicant shares sleeping quarters with others";
      }

      public class LADDERBEDSTAMINA
      {
        public static LocString NAME = (LocString) "Sleeping in a ladder bed";
        public static LocString TOOLTIP = (LocString) "This Duplicant's sleeping arrangements are adequate";
      }

      public class BEDROOMSTAMINA
      {
        public static LocString NAME = (LocString) "Private Bedroom";
        public static LocString TOOLTIP = (LocString) "This lucky Duplicant has their own private bedroom";
      }

      public class ROOMMESSHALL
      {
        public static LocString NAME = (LocString) "Mess Hall";
        public static LocString TOOLTIP = (LocString) ("This Duplicant's most recent meal was eaten in a " + UI.PRE_KEYWORD + "Mess Hall" + UI.PST_KEYWORD);
      }

      public class ROOMGREATHALL
      {
        public static LocString NAME = (LocString) "Great Hall";
        public static LocString TOOLTIP = (LocString) ("This Duplicant's most recent meal was eaten in a fancy " + UI.PRE_KEYWORD + "Great Hall" + UI.PST_KEYWORD);
      }

      public class ENTITLEMENT
      {
        public static LocString NAME = (LocString) "Entitlement";
        public static LocString TOOLTIP = (LocString) ("Duplicants will demand better " + UI.PRE_KEYWORD + "Decor" + UI.PST_KEYWORD + " and accommodations with each Expertise level they gain");
      }

      public class BASEDUPLICANT
      {
        public static LocString NAME = (LocString) "Duplicant";
      }

      public class HOMEOSTASIS
      {
        public static LocString NAME = (LocString) "Homeostasis";
      }

      public class WARMAIR
      {
        public static LocString NAME = (LocString) "Warm Air";
      }

      public class COLDAIR
      {
        public static LocString NAME = (LocString) "Cold Air";
      }

      public class CLAUSTROPHOBIC
      {
        public static LocString NAME = (LocString) "Claustrophobic";
        public static LocString TOOLTIP = (LocString) "This Duplicant recently found themselves in an upsettingly cramped space";
        public static LocString CAUSE = (LocString) "This Duplicant got so good at their job that they became claustrophobic";
      }

      public class VERTIGO
      {
        public static LocString NAME = (LocString) "Vertigo";
        public static LocString TOOLTIP = (LocString) "This Duplicant had to climb a tall ladder that left them dizzy and unsettled";
        public static LocString CAUSE = (LocString) "This Duplicant got so good at their job they became bad at ladders";
      }

      public class UNCOMFORTABLEFEET
      {
        public static LocString NAME = (LocString) "Aching Feet";
        public static LocString TOOLTIP = (LocString) "This Duplicant recently walked across floor without tile, much to their chagrin";
        public static LocString CAUSE = (LocString) "This Duplicant got so good at their job that their feet became sensitive";
      }

      public class PEOPLETOOCLOSEWHILESLEEPING
      {
        public static LocString NAME = (LocString) "Personal Bubble Burst";
        public static LocString TOOLTIP = (LocString) "This Duplicant had to sleep too close to others and it was awkward for them";
        public static LocString CAUSE = (LocString) "This Duplicant got so good at their job that they stopped being comfortable sleeping near other people";
      }

      public class RESTLESS
      {
        public static LocString NAME = (LocString) "Restless";
        public static LocString TOOLTIP = (LocString) "This Duplicant went a few minutes without working and is now completely awash with guilt";
        public static LocString CAUSE = (LocString) "This Duplicant got so good at their job that they forgot how to be comfortable doing anything else";
      }

      public class UNFASHIONABLECLOTHING
      {
        public static LocString NAME = (LocString) "Fashion Crime";
        public static LocString TOOLTIP = (LocString) "This Duplicant had to wear something that was an affront to fashion";
        public static LocString CAUSE = (LocString) "This Duplicant got so good at their job that they became incapable of tolerating unfashionable clothing";
      }

      public class BURNINGCALORIES
      {
        public static LocString NAME = (LocString) "Homeostasis";
      }

      public class EATINGCALORIES
      {
        public static LocString NAME = (LocString) "Eating";
      }

      public class TEMPEXCHANGE
      {
        public static LocString NAME = (LocString) "Environmental Exchange";
      }

      public class CLOTHING
      {
        public static LocString NAME = (LocString) "Clothing";
      }

      public class CRYFACE
      {
        public static LocString NAME = (LocString) "Cry Face";
        public static LocString TOOLTIP = (LocString) "This Duplicant recently had a crying fit and it shows";
        public static LocString CAUSE = (LocString) ("Obtained from the " + UI.PRE_KEYWORD + "Ugly Crier" + UI.PST_KEYWORD + " stress reaction");
      }

      public class SOILEDSUIT
      {
        public static LocString NAME = (LocString) "Soiled Suit";
        public static LocString TOOLTIP = (LocString) "This Duplicant's suit needs to be emptied of waste\n\n(Preferably soon)";
        public static LocString CAUSE = (LocString) "Obtained when a Duplicant wears a suit filled with... \"fluids\"";
      }

      public class SHOWERED
      {
        public static LocString NAME = (LocString) "Showered";
        public static LocString TOOLTIP = (LocString) "This Duplicant recently had a shower and feels squeaky clean!";
      }

      public class SOREBACK
      {
        public static LocString NAME = (LocString) "Sore Back";
        public static LocString TOOLTIP = (LocString) "This Duplicant feels achy from sleeping on the floor last night and would like a bed";
        public static LocString CAUSE = (LocString) "Obtained by sleeping on the ground";
      }

      public class GOODEATS
      {
        public static LocString NAME = (LocString) "Soul Food";
        public static LocString TOOLTIP = (LocString) "This Duplicant had a yummy home cooked meal and is totally stuffed";
        public static LocString CAUSE = (LocString) "Obtained by eating a hearty home cooked meal";
        public static LocString DESCRIPTION = (LocString) "Duplicants find this home cooked meal is emotionally comforting";
      }

      public class HOTSTUFF
      {
        public static LocString NAME = (LocString) "Hot Stuff";
        public static LocString TOOLTIP = (LocString) ("This Duplicant had an extremely spicy meal and is both exhilarated and a little " + UI.PRE_KEYWORD + "Stressed" + UI.PST_KEYWORD);
        public static LocString CAUSE = (LocString) "Obtained by eating a very spicy meal";
        public static LocString DESCRIPTION = (LocString) "Duplicants find this spicy meal quite invigorating";
      }

      public class SEAFOODRADIATIONRESISTANCE
      {
        public static LocString NAME = (LocString) "Radiation Resistant: Aquatic Diet";
        public static LocString TOOLTIP = (LocString) ("This Duplicant ate sea-grown foods, which boost " + UI.PRE_KEYWORD + "Radiation" + UI.PST_KEYWORD + " resistance");
        public static LocString CAUSE = (LocString) "Obtained by eating sea-grown foods like fish or lettuce";
        public static LocString DESCRIPTION = (LocString) ("Eating this improves " + UI.PRE_KEYWORD + "Radiation" + UI.PST_KEYWORD + " resistance");
      }

      public class RECENTLYPARTIED
      {
        public static LocString NAME = (LocString) "Partied Hard";
        public static LocString TOOLTIP = (LocString) "This Duplicant recently attended a great party!";
      }

      public class NOFUNALLOWED
      {
        public static LocString NAME = (LocString) "Fun Interrupted";
        public static LocString TOOLTIP = (LocString) "This Duplicant is upset a party was rejected";
      }

      public class CONTAMINATEDLUNGS
      {
        public static LocString NAME = (LocString) "Yucky Lungs";
        public static LocString TOOLTIP = (LocString) ("This Duplicant got a big nasty lungful of " + (string) ELEMENTS.CONTAMINATEDOXYGEN.NAME);
      }

      public class MINORIRRITATION
      {
        public static LocString NAME = (LocString) "Minor Eye Irritation";
        public static LocString TOOLTIP = (LocString) "A gas or liquid made this Duplicant's eyes sting a little";
        public static LocString CAUSE = (LocString) "Obtained by exposure to a harsh liquid or gas";
      }

      public class MAJORIRRITATION
      {
        public static LocString NAME = (LocString) "Major Eye Irritation";
        public static LocString TOOLTIP = (LocString) "Woah, something really messed up this Duplicant's eyes!\n\nCaused by exposure to a harsh liquid or gas";
        public static LocString CAUSE = (LocString) "Obtained by exposure to a harsh liquid or gas";
      }

      public class FRESH_AND_CLEAN
      {
        public static LocString NAME = (LocString) "Refreshingly Clean";
        public static LocString TOOLTIP = (LocString) "This Duplicant took a warm shower and it was great!";
        public static LocString CAUSE = (LocString) "Obtained by taking a comfortably heated shower";
      }

      public class BURNED_BY_SCALDING_WATER
      {
        public static LocString NAME = (LocString) "Scalded";
        public static LocString TOOLTIP = (LocString) "Ouch! This Duplicant showered or was doused in water that was way too hot";
        public static LocString CAUSE = (LocString) "Obtained by exposure to hot water";
      }

      public class STRESSED_BY_COLD_WATER
      {
        public static LocString NAME = (LocString) "Numb";
        public static LocString TOOLTIP = (LocString) "Brr! This Duplicant was showered or doused in water that was way too cold";
        public static LocString CAUSE = (LocString) "Obtained by exposure to icy water";
      }

      public class SMELLEDSTINKY
      {
        public static LocString NAME = (LocString) "Smelled Stinky";
        public static LocString TOOLTIP = (LocString) "This Duplicant got a whiff of a certain somebody";
      }

      public class STRESSREDUCTION
      {
        public static LocString NAME = (LocString) "Receiving Massage";
        public static LocString TOOLTIP = (LocString) ("This Duplicant's " + UI.PRE_KEYWORD + "Stress" + UI.PST_KEYWORD + " is just melting away");
      }

      public class STRESSREDUCTION_CLINIC
      {
        public static LocString NAME = (LocString) "Receiving Clinic Massage";
        public static LocString TOOLTIP = (LocString) ("Clinical facilities are improving the effectiveness of this massage\n\nThis Duplicant's " + UI.PRE_KEYWORD + "Stress" + UI.PST_KEYWORD + " is just melting away");
      }

      public class UGLY_CRYING
      {
        public static LocString NAME = (LocString) "Ugly Crying";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is having a cathartic ugly cry as a result of " + UI.PRE_KEYWORD + "Stress" + UI.PST_KEYWORD);
        public static LocString NOTIFICATION_NAME = (LocString) "Ugly Crying";
        public static LocString NOTIFICATION_TOOLTIP = (LocString) ("These Duplicants became so " + UI.FormatAsLink("Stressed", "STRESS") + " they broke down crying:");
      }

      public class BINGE_EATING
      {
        public static LocString NAME = (LocString) "Insatiable Hunger";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is stuffing their face as a result of " + UI.PRE_KEYWORD + "Stress" + UI.PST_KEYWORD);
        public static LocString NOTIFICATION_NAME = (LocString) "Binge Eating";
        public static LocString NOTIFICATION_TOOLTIP = (LocString) ("These Duplicants became so " + UI.FormatAsLink("Stressed", "STRESS") + " they began overeating:");
      }

      public class BANSHEE_WAILING
      {
        public static LocString NAME = (LocString) "Deafening Shriek";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is wailing at the top of their lungs as a result of " + UI.PRE_KEYWORD + "Stress" + UI.PST_KEYWORD);
        public static LocString NOTIFICATION_NAME = (LocString) "Banshee Wailing";
        public static LocString NOTIFICATION_TOOLTIP = (LocString) ("These Duplicants became so " + UI.FormatAsLink("Stressed", "STRESS") + " they began wailing:");
      }

      public class BANSHEE_WAILING_RECOVERY
      {
        public static LocString NAME = (LocString) "Guzzling Air";
        public static LocString TOOLTIP = (LocString) "This Duplicant needs a little extra oxygen to catch their breath";
      }

      public class METABOLISM_CALORIE_MODIFIER
      {
        public static LocString NAME = (LocString) "Metabolism";
        public static LocString TOOLTIP = (LocString) (UI.PRE_KEYWORD + "Metabolism" + UI.PST_KEYWORD + " determines how quickly a critter burns " + UI.PRE_KEYWORD + "Calories" + UI.PST_KEYWORD);
      }

      public class WORKING
      {
        public static LocString NAME = (LocString) "Working";
        public static LocString TOOLTIP = (LocString) "This Duplicant is working up a sweat";
      }

      public class UNCOMFORTABLESLEEP
      {
        public static LocString NAME = (LocString) "Sleeping Uncomfortably";
        public static LocString TOOLTIP = (LocString) "This Duplicant collapsed on the floor from sheer exhaustion";
      }

      public class MANAGERIALDUTIES
      {
        public static LocString NAME = (LocString) "Managerial Duties";
        public static LocString TOOLTIP = (LocString) "Being a manager is stressful";
      }

      public class MANAGEDCOLONY
      {
        public static LocString NAME = (LocString) "Managed Colony";
        public static LocString TOOLTIP = (LocString) "A Duplicant is in the colony manager job";
      }

      public class FLOORSLEEP
      {
        public static LocString NAME = (LocString) "Sleeping On Floor";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is uncomfortably recovering " + UI.PRE_KEYWORD + "Stamina" + UI.PST_KEYWORD);
      }

      public class PASSEDOUTSLEEP
      {
        public static LocString NAME = (LocString) "Exhausted";
        public static LocString TOOLTIP = (LocString) ("Lack of rest depleted this Duplicant's " + UI.PRE_KEYWORD + "Stamina" + UI.PST_KEYWORD + "\n\nThey passed out from the fatigue");
      }

      public class SLEEP
      {
        public static LocString NAME = (LocString) "Sleeping";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is recovering " + UI.PRE_KEYWORD + "Stamina" + UI.PST_KEYWORD);
      }

      public class SLEEPCLINIC
      {
        public static LocString NAME = (LocString) "Nodding Off";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is losing " + UI.PRE_KEYWORD + "Stamina" + UI.PST_KEYWORD + " because of their " + UI.PRE_KEYWORD + "Pajamas" + UI.PST_KEYWORD);
      }

      public class RESTFULSLEEP
      {
        public static LocString NAME = (LocString) "Sleeping Peacefully";
        public static LocString TOOLTIP = (LocString) "This Duplicant is getting a good night's rest";
      }

      public class SLEEPY
      {
        public static LocString NAME = (LocString) "Sleepy";
        public static LocString TOOLTIP = (LocString) "This Duplicant is getting tired";
      }

      public class HUNGRY
      {
        public static LocString NAME = (LocString) "Hungry";
        public static LocString TOOLTIP = (LocString) "This Duplicant is ready for lunch";
      }

      public class STARVING
      {
        public static LocString NAME = (LocString) "Starving";
        public static LocString TOOLTIP = (LocString) "This Duplicant needs to eat something, soon";
      }

      public class HOT
      {
        public static LocString NAME = (LocString) "Hot";
        public static LocString TOOLTIP = (LocString) "This Duplicant is uncomfortably warm";
      }

      public class COLD
      {
        public static LocString NAME = (LocString) "Cold";
        public static LocString TOOLTIP = (LocString) "This Duplicant is uncomfortably cold";
      }

      public class CARPETFEET
      {
        public static LocString NAME = (LocString) "Tickled Tootsies";
        public static LocString TOOLTIP = (LocString) "Walking on carpet has made this Duplicant's day a little more luxurious";
      }

      public class WETFEET
      {
        public static LocString NAME = (LocString) "Soggy Feet";
        public static LocString TOOLTIP = (LocString) ("This Duplicant recently stepped in " + UI.PRE_KEYWORD + "Liquid" + UI.PST_KEYWORD);
        public static LocString CAUSE = (LocString) "Obtained by walking through liquid";
      }

      public class SOAKINGWET
      {
        public static LocString NAME = (LocString) "Sopping Wet";
        public static LocString TOOLTIP = (LocString) ("This Duplicant was recently submerged in " + UI.PRE_KEYWORD + "Liquid" + UI.PST_KEYWORD);
        public static LocString CAUSE = (LocString) "Obtained from submergence in liquid";
      }

      public class POPPEDEARDRUMS
      {
        public static LocString NAME = (LocString) "Popped Eardrums";
        public static LocString TOOLTIP = (LocString) "This Duplicant was exposed to an over-pressurized area that popped their eardrums";
      }

      public class ANEWHOPE
      {
        public static LocString NAME = (LocString) "New Hope";
        public static LocString TOOLTIP = (LocString) "This Duplicant feels pretty optimistic about their new home";
      }

      public class MEGABRAINTANKBONUS
      {
        public static LocString NAME = (LocString) "Maximum Aptitude";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is smarter and stronger than usual thanks to the " + UI.PRE_KEYWORD + "Somnium Synthesizer" + UI.PST_KEYWORD + " ");
      }

      public class PRICKLEFRUITDAMAGE
      {
        public static LocString NAME = (LocString) "Ouch!";
        public static LocString TOOLTIP = (LocString) ("This Duplicant ate a raw " + UI.FormatAsLink("Bristle Berry", "PRICKLEFRUIT") + " and it gave their mouth ouchies");
      }

      public class NOOXYGEN
      {
        public static LocString NAME = (LocString) "No Oxygen";
        public static LocString TOOLTIP = (LocString) "There is no breathable air in this area";
      }

      public class LOWOXYGEN
      {
        public static LocString NAME = (LocString) "Low Oxygen";
        public static LocString TOOLTIP = (LocString) "The air is thin in this area";
      }

      public class MOURNING
      {
        public static LocString NAME = (LocString) "Mourning";
        public static LocString TOOLTIP = (LocString) "This Duplicant is grieving the loss of a friend";
      }

      public class NARCOLEPTICSLEEP
      {
        public static LocString NAME = (LocString) "Narcoleptic Nap";
        public static LocString TOOLTIP = (LocString) "This Duplicant just needs to rest their eyes for a second";
      }

      public class BADSLEEP
      {
        public static LocString NAME = (LocString) "Unrested: Too Bright";
        public static LocString TOOLTIP = (LocString) ("This Duplicant tossed and turned all night because a " + UI.PRE_KEYWORD + "Light" + UI.PST_KEYWORD + " was left on where they were trying to sleep");
      }

      public class BADSLEEPAFRAIDOFDARK
      {
        public static LocString NAME = (LocString) "Unrested: Afraid of Dark";
        public static LocString TOOLTIP = (LocString) ("This Duplicant didn't get much sleep because they were too anxious about the lack of " + UI.PRE_KEYWORD + "Light" + UI.PST_KEYWORD + " to relax");
      }

      public class BADSLEEPMOVEMENT
      {
        public static LocString NAME = (LocString) "Unrested: Bed Jostling";
        public static LocString TOOLTIP = (LocString) "This Duplicant was woken up when a friend climbed on their ladder bed";
      }

      public class TERRIBLESLEEP
      {
        public static LocString NAME = (LocString) "Dead Tired: Snoring Friend";
        public static LocString TOOLTIP = (LocString) "This Duplicant didn't get any shuteye last night because of all the racket from a friend's snoring";
      }

      public class PEACEFULSLEEP
      {
        public static LocString NAME = (LocString) "Well Rested";
        public static LocString TOOLTIP = (LocString) "This Duplicant had a blissfully quiet sleep last night";
      }

      public class CENTEROFATTENTION
      {
        public static LocString NAME = (LocString) "Center of Attention";
        public static LocString TOOLTIP = (LocString) "This Duplicant feels like someone's watching over them...";
      }

      public class INSPIRED
      {
        public static LocString NAME = (LocString) "Inspired";
        public static LocString TOOLTIP = (LocString) "This Duplicant has had a creative vision!";
      }

      public class NEWCREWARRIVAL
      {
        public static LocString NAME = (LocString) "New Friend";
        public static LocString TOOLTIP = (LocString) "This Duplicant is happy to see a new face in the colony";
      }

      public class UNDERWATER
      {
        public static LocString NAME = (LocString) "Underwater";
        public static LocString TOOLTIP = (LocString) "This Duplicant's movement is slowed";
      }

      public class NIGHTMARES
      {
        public static LocString NAME = (LocString) "Nightmares";
        public static LocString TOOLTIP = (LocString) "This Duplicant was visited by something in the night";
      }

      public class WASATTACKED
      {
        public static LocString NAME = (LocString) "Recently assailed";
        public static LocString TOOLTIP = (LocString) "This Duplicant is stressed out after having been attacked";
      }

      public class LIGHTWOUNDS
      {
        public static LocString NAME = (LocString) "Light Wounds";
        public static LocString TOOLTIP = (LocString) "This Duplicant sustained injuries that are a bit uncomfortable";
      }

      public class MODERATEWOUNDS
      {
        public static LocString NAME = (LocString) "Moderate Wounds";
        public static LocString TOOLTIP = (LocString) "This Duplicant sustained injuries that are affecting their ability to work";
      }

      public class SEVEREWOUNDS
      {
        public static LocString NAME = (LocString) "Severe Wounds";
        public static LocString TOOLTIP = (LocString) "This Duplicant sustained serious injuries that are impacting their work and well-being";
      }

      public class SANDBOXMORALEADJUSTMENT
      {
        public static LocString NAME = (LocString) "Sandbox Morale Adjustment";
        public static LocString TOOLTIP = (LocString) ("This Duplicant has had their " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD + " temporarily adjusted using the Sandbox tools");
      }

      public class ROTTEMPERATURE
      {
        public static LocString UNREFRIGERATED = (LocString) "Unrefrigerated";
        public static LocString REFRIGERATED = (LocString) "Refrigerated";
        public static LocString FROZEN = (LocString) "Frozen";
      }

      public class ROTATMOSPHERE
      {
        public static LocString CONTAMINATED = (LocString) "Contaminated Air";
        public static LocString NORMAL = (LocString) "Normal Atmosphere";
        public static LocString STERILE = (LocString) "Sterile Atmosphere";
      }

      public class BASEROT
      {
        public static LocString NAME = (LocString) "Base Decay Rate";
      }

      public class FULLBLADDER
      {
        public static LocString NAME = (LocString) "Full Bladder";
        public static LocString TOOLTIP = (LocString) ("This Duplicant's " + UI.PRE_KEYWORD + "Bladder" + UI.PST_KEYWORD + " is full");
      }

      public class DIARRHEA
      {
        public static LocString NAME = (LocString) "Diarrhea";
        public static LocString TOOLTIP = (LocString) "This Duplicant's gut is giving them some trouble";
        public static LocString CAUSE = (LocString) "Obtained by eating a disgusting meal";
        public static LocString DESCRIPTION = (LocString) "Most Duplicants experience stomach upset from this meal";
      }

      public class STRESSFULYEMPTYINGBLADDER
      {
        public static LocString NAME = (LocString) "Making a mess";
        public static LocString TOOLTIP = (LocString) ("This Duplicant had no choice but to empty their " + UI.PRE_KEYWORD + "Bladder" + UI.PST_KEYWORD);
      }

      public class REDALERT
      {
        public static LocString NAME = (LocString) "Red Alert!";
        public static LocString TOOLTIP = (LocString) ("The " + UI.PRE_KEYWORD + "Red Alert" + UI.PST_KEYWORD + " is stressing this Duplicant out");
      }

      public class FUSSY
      {
        public static LocString NAME = (LocString) "Fussy";
        public static LocString TOOLTIP = (LocString) "This Duplicant is hard to please";
      }

      public class WARMINGUP
      {
        public static LocString NAME = (LocString) "Warming Up";
        public static LocString TOOLTIP = (LocString) "This Duplicant is trying to warm back up";
      }

      public class COOLINGDOWN
      {
        public static LocString NAME = (LocString) "Cooling Down";
        public static LocString TOOLTIP = (LocString) "This Duplicant is trying to cool back down";
      }

      public class DARKNESS
      {
        public static LocString NAME = (LocString) "Darkness";
        public static LocString TOOLTIP = (LocString) "Eep! This Duplicant doesn't like being in the dark!";
      }

      public class STEPPEDINCONTAMINATEDWATER
      {
        public static LocString NAME = (LocString) "Stepped in polluted water";
        public static LocString TOOLTIP = (LocString) "Gross! This Duplicant stepped in something yucky";
      }

      public class WELLFED
      {
        public static LocString NAME = (LocString) "Well fed";
        public static LocString TOOLTIP = (LocString) "This Duplicant feels satisfied after having a big meal";
      }

      public class STALEFOOD
      {
        public static LocString NAME = (LocString) "Bad leftovers";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is in a bad mood from having to eat stale " + UI.PRE_KEYWORD + "Food" + UI.PST_KEYWORD);
      }

      public class SMELLEDPUTRIDODOUR
      {
        public static LocString NAME = (LocString) "Smelled a putrid odor";
        public static LocString TOOLTIP = (LocString) "This Duplicant got a whiff of something unspeakably foul";
      }

      public class VOMITING
      {
        public static LocString NAME = (LocString) "Vomiting";
        public static LocString TOOLTIP = (LocString) "Better out than in, as they say";
      }

      public class BREATHING
      {
        public static LocString NAME = (LocString) "Breathing";
      }

      public class HOLDINGBREATH
      {
        public static LocString NAME = (LocString) "Holding breath";
      }

      public class RECOVERINGBREATH
      {
        public static LocString NAME = (LocString) "Recovering breath";
      }

      public class ROTTING
      {
        public static LocString NAME = (LocString) "Rotting";
      }

      public class DEAD
      {
        public static LocString NAME = (LocString) "Dead";
      }

      public class TOXICENVIRONMENT
      {
        public static LocString NAME = (LocString) "Toxic environment";
      }

      public class RESTING
      {
        public static LocString NAME = (LocString) "Resting";
      }

      public class INTRAVENOUS_NUTRITION
      {
        public static LocString NAME = (LocString) "Intravenous Feeding";
      }

      public class CATHETERIZED
      {
        public static LocString NAME = (LocString) "Catheterized";
        public static LocString TOOLTIP = (LocString) "Let's leave it at that";
      }

      public class NOISEPEACEFUL
      {
        public static LocString NAME = (LocString) "Peace and Quiet";
        public static LocString TOOLTIP = (LocString) "This Duplicant has found a quiet place to concentrate";
      }

      public class NOISEMINOR
      {
        public static LocString NAME = (LocString) "Loud Noises";
        public static LocString TOOLTIP = (LocString) "This area is a bit too loud for comfort";
      }

      public class NOISEMAJOR
      {
        public static LocString NAME = (LocString) "Cacophony!";
        public static LocString TOOLTIP = (LocString) "It's very, very loud in here!";
      }

      public class MEDICALCOT
      {
        public static LocString NAME = (LocString) "Triage Cot Rest";
        public static LocString TOOLTIP = (LocString) "Bedrest is improving this Duplicant's physical recovery time";
      }

      public class MEDICALCOTDOCTORED
      {
        public static LocString NAME = (LocString) "Receiving treatment";
        public static LocString TOOLTIP = (LocString) "This Duplicant is receiving treatment for their physical injuries";
      }

      public class DOCTOREDOFFCOTEFFECT
      {
        public static LocString NAME = (LocString) "Runaway Patient";
        public static LocString TOOLTIP = (LocString) "Tsk tsk!\nThis Duplicant cannot receive treatment while out of their medical bed!";
      }

      public class POSTDISEASERECOVERY
      {
        public static LocString NAME = (LocString) "Feeling better";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is up and about, but they still have some lingering effects from their " + UI.PRE_KEYWORD + "Disease" + UI.PST_KEYWORD);
        public static LocString ADDITIONAL_EFFECTS = (LocString) "This Duplicant has temporary immunity to diseases from having beaten an infection";
      }

      public class IMMUNESYSTEMOVERWHELMED
      {
        public static LocString NAME = (LocString) "Immune System Overwhelmed";
        public static LocString TOOLTIP = (LocString) "This Duplicant's immune system is slowly being overwhelmed by a high concentration of germs";
      }

      public class MEDICINE_GENERICPILL
      {
        public static LocString NAME = (LocString) "Placebo";
        public static LocString TOOLTIP = ITEMS.PILLS.PLACEBO.DESC;
        public static LocString EFFECT_DESC = (LocString) ("Applies the " + UI.PRE_KEYWORD + "{0}" + UI.PST_KEYWORD + " effect");
      }

      public class MEDICINE_BASICBOOSTER
      {
        public static LocString NAME = ITEMS.PILLS.BASICBOOSTER.NAME;
        public static LocString TOOLTIP = ITEMS.PILLS.BASICBOOSTER.DESC;
      }

      public class MEDICINE_INTERMEDIATEBOOSTER
      {
        public static LocString NAME = ITEMS.PILLS.INTERMEDIATEBOOSTER.NAME;
        public static LocString TOOLTIP = ITEMS.PILLS.INTERMEDIATEBOOSTER.DESC;
      }

      public class MEDICINE_BASICRADPILL
      {
        public static LocString NAME = ITEMS.PILLS.BASICRADPILL.NAME;
        public static LocString TOOLTIP = ITEMS.PILLS.BASICRADPILL.DESC;
      }

      public class MEDICINE_INTERMEDIATERADPILL
      {
        public static LocString NAME = ITEMS.PILLS.INTERMEDIATERADPILL.NAME;
        public static LocString TOOLTIP = ITEMS.PILLS.INTERMEDIATERADPILL.DESC;
      }

      public class SUNLIGHT_PLEASANT
      {
        public static LocString NAME = (LocString) "Bright and Cheerful";
        public static LocString TOOLTIP = (LocString) ("The strong natural " + UI.PRE_KEYWORD + "Light" + UI.PST_KEYWORD + " is making this Duplicant feel light on their feet");
      }

      public class SUNLIGHT_BURNING
      {
        public static LocString NAME = (LocString) "Intensely Bright";
        public static LocString TOOLTIP = (LocString) ("The bright " + UI.PRE_KEYWORD + "Light" + UI.PST_KEYWORD + " is significantly improving this Duplicant's mood, but prolonged exposure may result in burning");
      }

      public class TOOKABREAK
      {
        public static LocString NAME = (LocString) "Downtime";
        public static LocString TOOLTIP = (LocString) "This Duplicant has a bit of time off from work to attend to personal matters";
      }

      public class SOCIALIZED
      {
        public static LocString NAME = (LocString) "Socialized";
        public static LocString TOOLTIP = (LocString) "This Duplicant had some free time to hang out with buddies";
      }

      public class GOODCONVERSATION
      {
        public static LocString NAME = (LocString) "Pleasant Chitchat";
        public static LocString TOOLTIP = (LocString) "This Duplicant recently had a chance to chat with a friend";
      }

      public class WORKENCOURAGED
      {
        public static LocString NAME = (LocString) "Appreciated";
        public static LocString TOOLTIP = (LocString) "Someone saw how hard this Duplicant was working and gave them a compliment\n\nThis Duplicant feels great about themselves now!";
      }

      public class ISSPARKLESTREAKER
      {
        public static LocString NAME = (LocString) "Sparkle Streaking";
        public static LocString TOOLTIP = (LocString) "This Duplicant is currently Sparkle Streaking!\n\nBaa-ling!";
      }

      public class SAWSPARKLESTREAKER
      {
        public static LocString NAME = (LocString) "Sparkle Flattered";
        public static LocString TOOLTIP = (LocString) "A Sparkle Streaker's sparkles dazzled this Duplicant\n\nThis Duplicant has a spring in their step now!";
      }

      public class ISJOYSINGER
      {
        public static LocString NAME = (LocString) "Yodeling";
        public static LocString TOOLTIP = (LocString) "This Duplicant is currently Yodeling!";
      }

      public class HEARDJOYSINGER
      {
        public static LocString NAME = (LocString) "Serenaded";
        public static LocString TOOLTIP = (LocString) "A Yodeler's singing thrilled this Duplicant\n\nThis Duplicant works at a higher tempo now!";
      }

      public class HASBALLOON
      {
        public static LocString NAME = (LocString) "Balloon Buddy";
        public static LocString TOOLTIP = (LocString) "A Balloon Artist gave this Duplicant a balloon!\n\nThis Duplicant feels super crafty now!";
      }

      public class GREETING
      {
        public static LocString NAME = (LocString) "Saw Friend";
        public static LocString TOOLTIP = (LocString) "This Duplicant recently saw a friend in the halls and got to say \"hi\"\n\nIt wasn't even awkward!";
      }

      public class HUGGED
      {
        public static LocString NAME = (LocString) "Hugged";
        public static LocString TOOLTIP = (LocString) "This Duplicant recently received a hug from a friendly critter\n\nIt was so fluffy!";
      }

      public class ARCADEPLAYING
      {
        public static LocString NAME = (LocString) "Gaming";
        public static LocString TOOLTIP = (LocString) "This Duplicant is playing a video game\n\nIt looks like fun!";
      }

      public class PLAYEDARCADE
      {
        public static LocString NAME = (LocString) "Played Video Games";
        public static LocString TOOLTIP = (LocString) "This Duplicant recently played video games and is feeling like a champ";
      }

      public class DANCING
      {
        public static LocString NAME = (LocString) "Dancing";
        public static LocString TOOLTIP = (LocString) "This Duplicant is showing off their best moves.";
      }

      public class DANCED
      {
        public static LocString NAME = (LocString) "Recently Danced";
        public static LocString TOOLTIP = (LocString) ("This Duplicant had a chance to cut loose!\n\nLeisure activities increase Duplicants' " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD);
      }

      public class JUICER
      {
        public static LocString NAME = (LocString) "Drank Juice";
        public static LocString TOOLTIP = (LocString) ("This Duplicant had delicious fruity drink!\n\nLeisure activities increase Duplicants' " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD);
      }

      public class ESPRESSO
      {
        public static LocString NAME = (LocString) "Drank Espresso";
        public static LocString TOOLTIP = (LocString) ("This Duplicant had delicious drink!\n\nLeisure activities increase Duplicants' " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD);
      }

      public class MECHANICALSURFBOARD
      {
        public static LocString NAME = (LocString) "Stoked";
        public static LocString TOOLTIP = (LocString) ("This Duplicant had a rad experience on a surfboard.\n\nLeisure activities increase Duplicants' " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD);
      }

      public class MECHANICALSURFING
      {
        public static LocString NAME = (LocString) "Surfin'";
        public static LocString TOOLTIP = (LocString) "This Duplicant is surfin' some artificial waves!";
      }

      public class SAUNA
      {
        public static LocString NAME = (LocString) "Steam Powered";
        public static LocString TOOLTIP = (LocString) ("This Duplicant just had a relaxing time in a sauna\n\nLeisure activities increase Duplicants' " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD);
      }

      public class SAUNARELAXING
      {
        public static LocString NAME = (LocString) "Relaxing";
        public static LocString TOOLTIP = (LocString) "This Duplicant is relaxing in a sauna";
      }

      public class HOTTUB
      {
        public static LocString NAME = (LocString) "Hot Tubbed";
        public static LocString TOOLTIP = (LocString) ("This Duplicant recently unwound in a Hot Tub\n\nLeisure activities increase Duplicants' " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD);
      }

      public class HOTTUBRELAXING
      {
        public static LocString NAME = (LocString) "Relaxing";
        public static LocString TOOLTIP = (LocString) "This Duplicant is unwinding in a hot tub\n\nThey sure look relaxed";
      }

      public class SODAFOUNTAIN
      {
        public static LocString NAME = (LocString) "Soda Filled";
        public static LocString TOOLTIP = (LocString) ("This Duplicant just enjoyed a bubbly beverage\n\nLeisure activities increase Duplicants' " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD);
      }

      public class VERTICALWINDTUNNELFLYING
      {
        public static LocString NAME = (LocString) "Airborne";
        public static LocString TOOLTIP = (LocString) "This Duplicant is having an exhilarating time in the wind tunnel\n\nWhoosh!";
      }

      public class VERTICALWINDTUNNEL
      {
        public static LocString NAME = (LocString) "Wind Swept";
        public static LocString TOOLTIP = (LocString) ("This Duplicant recently had an exhilarating wind tunnel experience\n\nLeisure activities increase Duplicants' " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD);
      }

      public class BEACHCHAIRRELAXING
      {
        public static LocString NAME = (LocString) "Totally Chill";
        public static LocString TOOLTIP = (LocString) "This Duplicant is totally chillin' in a beach chair";
      }

      public class BEACHCHAIRLIT
      {
        public static LocString NAME = (LocString) "Sun Kissed";
        public static LocString TOOLTIP = (LocString) ("This Duplicant had an amazing experience at the Beach\n\nLeisure activities increase Duplicants' " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD);
      }

      public class BEACHCHAIRUNLIT
      {
        public static LocString NAME = (LocString) "Passably Relaxed";
        public static LocString TOOLTIP = (LocString) ("This Duplicant just had a mediocre beach experience\n\nLeisure activities increase Duplicants' " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD);
      }

      public class TELEPHONECHAT
      {
        public static LocString NAME = (LocString) "Full of Gossip";
        public static LocString TOOLTIP = (LocString) ("This Duplicant chatted on the phone with at least one other Duplicant\n\nLeisure activities increase Duplicants' " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD);
      }

      public class TELEPHONEBABBLE
      {
        public static LocString NAME = (LocString) "Less Anxious";
        public static LocString TOOLTIP = (LocString) ("This Duplicant got some things off their chest by talking to themselves on the phone\n\nLeisure activities increase Duplicants' " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD);
      }

      public class TELEPHONELONGDISTANCE
      {
        public static LocString NAME = (LocString) "Sociable";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is feeling sociable after chatting on the phone with someone across space\n\nLeisure activities increase Duplicants' " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD);
      }

      public class EDIBLEMINUS3
      {
        public static LocString NAME = (LocString) "Grisly Meal";
        public static LocString TOOLTIP = (LocString) ("The food this Duplicant last ate was " + UI.PRE_KEYWORD + "Grisly" + UI.PST_KEYWORD + "\n\nThey hope their next meal will be better");
      }

      public class EDIBLEMINUS2
      {
        public static LocString NAME = (LocString) "Terrible Meal";
        public static LocString TOOLTIP = (LocString) ("The food this Duplicant last ate was " + UI.PRE_KEYWORD + "Terrible" + UI.PST_KEYWORD + "\n\nThey hope their next meal will be better");
      }

      public class EDIBLEMINUS1
      {
        public static LocString NAME = (LocString) "Poor Meal";
        public static LocString TOOLTIP = (LocString) ("The food this Duplicant last ate was " + UI.PRE_KEYWORD + "Poor" + UI.PST_KEYWORD + "\n\nThey hope their next meal will be a little better");
      }

      public class EDIBLE0
      {
        public static LocString NAME = (LocString) "Standard Meal";
        public static LocString TOOLTIP = (LocString) ("The food this Duplicant last ate was " + UI.PRE_KEYWORD + "Average" + UI.PST_KEYWORD + "\n\nThey thought it was sort of okay");
      }

      public class EDIBLE1
      {
        public static LocString NAME = (LocString) "Good Meal";
        public static LocString TOOLTIP = (LocString) ("The food this Duplicant last ate was " + UI.PRE_KEYWORD + "Good" + UI.PST_KEYWORD + "\n\nThey thought it was pretty good!");
      }

      public class EDIBLE2
      {
        public static LocString NAME = (LocString) "Great Meal";
        public static LocString TOOLTIP = (LocString) ("The food this Duplicant last ate was " + UI.PRE_KEYWORD + "Great" + UI.PST_KEYWORD + "\n\nThey thought it was pretty good!");
      }

      public class EDIBLE3
      {
        public static LocString NAME = (LocString) "Superb Meal";
        public static LocString TOOLTIP = (LocString) ("The food this Duplicant last ate was " + UI.PRE_KEYWORD + "Superb" + UI.PST_KEYWORD + "\n\nThey thought it was really good!");
      }

      public class EDIBLE4
      {
        public static LocString NAME = (LocString) "Ambrosial Meal";
        public static LocString TOOLTIP = (LocString) ("The food this Duplicant last ate was " + UI.PRE_KEYWORD + "Ambrosial" + UI.PST_KEYWORD + "\n\nThey thought it was super tasty!");
      }

      public class DECORMINUS1
      {
        public static LocString NAME = (LocString) "Last Cycle's Decor: Ugly";
        public static LocString TOOLTIP = (LocString) ("This Duplicant thought the overall " + UI.PRE_KEYWORD + "Decor" + UI.PST_KEYWORD + " yesterday was downright depressing");
      }

      public class DECOR0
      {
        public static LocString NAME = (LocString) "Last Cycle's Decor: Poor";
        public static LocString TOOLTIP = (LocString) ("This Duplicant thought the overall " + UI.PRE_KEYWORD + "Decor" + UI.PST_KEYWORD + " yesterday was quite poor");
      }

      public class DECOR1
      {
        public static LocString NAME = (LocString) "Last Cycle's Decor: Mediocre";
        public static LocString TOOLTIP = (LocString) ("This Duplicant had no strong opinions about the colony's " + UI.PRE_KEYWORD + "Decor" + UI.PST_KEYWORD + " yesterday");
      }

      public class DECOR2
      {
        public static LocString NAME = (LocString) "Last Cycle's Decor: Average";
        public static LocString TOOLTIP = (LocString) ("This Duplicant thought the overall " + UI.PRE_KEYWORD + "Decor" + UI.PST_KEYWORD + " yesterday was pretty alright");
      }

      public class DECOR3
      {
        public static LocString NAME = (LocString) "Last Cycle's Decor: Nice";
        public static LocString TOOLTIP = (LocString) ("This Duplicant thought the overall " + UI.PRE_KEYWORD + "Decor" + UI.PST_KEYWORD + " yesterday was quite nice!");
      }

      public class DECOR4
      {
        public static LocString NAME = (LocString) "Last Cycle's Decor: Charming";
        public static LocString TOOLTIP = (LocString) ("This Duplicant thought the overall " + UI.PRE_KEYWORD + "Decor" + UI.PST_KEYWORD + " yesterday was downright charming!");
      }

      public class DECOR5
      {
        public static LocString NAME = (LocString) "Last Cycle's Decor: Gorgeous";
        public static LocString TOOLTIP = (LocString) ("This Duplicant thought the overall " + UI.PRE_KEYWORD + "Decor" + UI.PST_KEYWORD + " yesterday was fantastic\n\nThey love what I've done with the place!");
      }

      public class BREAK1
      {
        public static LocString NAME = (LocString) "One Shift Break";
        public static LocString TOOLTIP = (LocString) ("This Duplicant has had one " + UI.PRE_KEYWORD + "Downtime" + UI.PST_KEYWORD + " shift in the last cycle");
      }

      public class BREAK2
      {
        public static LocString NAME = (LocString) "Two Shift Break";
        public static LocString TOOLTIP = (LocString) ("This Duplicant has had two " + UI.PRE_KEYWORD + "Downtime" + UI.PST_KEYWORD + " shifts in the last cycle");
      }

      public class BREAK3
      {
        public static LocString NAME = (LocString) "Three Shift Break";
        public static LocString TOOLTIP = (LocString) ("This Duplicant has had three " + UI.PRE_KEYWORD + "Downtime" + UI.PST_KEYWORD + " shifts in the last cycle");
      }

      public class BREAK4
      {
        public static LocString NAME = (LocString) "Four Shift Break";
        public static LocString TOOLTIP = (LocString) ("This Duplicant has had four " + UI.PRE_KEYWORD + "Downtime" + UI.PST_KEYWORD + " shifts in the last cycle");
      }

      public class BREAK5
      {
        public static LocString NAME = (LocString) "Five Shift Break";
        public static LocString TOOLTIP = (LocString) ("This Duplicant has had five " + UI.PRE_KEYWORD + "Downtime" + UI.PST_KEYWORD + " shifts in the last cycle");
      }

      public class POWERTINKER
      {
        public static LocString NAME = (LocString) "Engie's Tune-Up";
        public static LocString TOOLTIP = (LocString) ("A skilled Duplicant has improved this generator's " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD + " output efficiency\n\nApplying this effect consumed one " + UI.PRE_KEYWORD + (string) ITEMS.INDUSTRIAL_PRODUCTS.POWER_STATION_TOOLS.NAME + UI.PST_KEYWORD);
      }

      public class FARMTINKER
      {
        public static LocString NAME = (LocString) "Farmer's Touch";
        public static LocString TOOLTIP = (LocString) ("A skilled Duplicant has encouraged this " + UI.PRE_KEYWORD + "Plant" + UI.PST_KEYWORD + " to grow a little bit faster\n\nApplying this effect consumed one dose of " + UI.PRE_KEYWORD + (string) ITEMS.INDUSTRIAL_PRODUCTS.FARM_STATION_TOOLS.NAME + UI.PST_KEYWORD);
      }

      public class MACHINETINKER
      {
        public static LocString NAME = (LocString) "Engie's Jerry Rig";
        public static LocString TOOLTIP = (LocString) ("A skilled Duplicant has jerry rigged this " + UI.PRE_KEYWORD + "Generator" + UI.PST_KEYWORD + " to temporarily run faster");
      }

      public class SPACETOURIST
      {
        public static LocString NAME = (LocString) "Visited Space";
        public static LocString TOOLTIP = (LocString) "This Duplicant went on a trip to space and saw the wonders of the universe";
      }

      public class SUDDENMORALEHELPER
      {
        public static LocString NAME = (LocString) "Morale Upgrade Helper";
        public static LocString TOOLTIP = (LocString) ("This Duplicant will receive a temporary " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD + " bonus to buffer the new " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD + " system introduction");
      }

      public class EXPOSEDTOFOODGERMS
      {
        public static LocString NAME = (LocString) "Food Poisoning Exposure";
        public static LocString TOOLTIP = (LocString) ("This Duplicant was exposed to " + (string) DUPLICANTS.DISEASES.FOODPOISONING.NAME + " Germs and is at risk of developing the " + UI.PRE_KEYWORD + "Disease" + UI.PST_KEYWORD);
      }

      public class EXPOSEDTOSLIMEGERMS
      {
        public static LocString NAME = (LocString) "Slimelung Exposure";
        public static LocString TOOLTIP = (LocString) ("This Duplicant was exposed to " + (string) DUPLICANTS.DISEASES.SLIMELUNG.NAME + " and is at risk of developing the " + UI.PRE_KEYWORD + "Disease" + UI.PST_KEYWORD);
      }

      public class EXPOSEDTOZOMBIESPORES
      {
        public static LocString NAME = (LocString) "Zombie Spores Exposure";
        public static LocString TOOLTIP = (LocString) ("This Duplicant was exposed to " + (string) DUPLICANTS.DISEASES.ZOMBIESPORES.NAME + " and is at risk of developing the " + UI.PRE_KEYWORD + "Disease" + UI.PST_KEYWORD);
      }

      public class FEELINGSICKFOODGERMS
      {
        public static LocString NAME = (LocString) "Contracted: Food Poisoning";
        public static LocString TOOLTIP = (LocString) ("This Duplicant contracted " + (string) DUPLICANTS.DISEASES.FOODSICKNESS.NAME + " after a recent " + UI.PRE_KEYWORD + "Germ" + UI.PST_KEYWORD + " exposure and will begin exhibiting symptoms shortly");
      }

      public class FEELINGSICKSLIMEGERMS
      {
        public static LocString NAME = (LocString) "Contracted: Slimelung";
        public static LocString TOOLTIP = (LocString) ("This Duplicant contracted " + (string) DUPLICANTS.DISEASES.SLIMESICKNESS.NAME + " after a recent " + UI.PRE_KEYWORD + "Germ" + UI.PST_KEYWORD + " exposure and will begin exhibiting symptoms shortly");
      }

      public class FEELINGSICKZOMBIESPORES
      {
        public static LocString NAME = (LocString) "Contracted: Zombie Spores";
        public static LocString TOOLTIP = (LocString) ("This Duplicant contracted " + (string) DUPLICANTS.DISEASES.ZOMBIESICKNESS.NAME + " after a recent " + UI.PRE_KEYWORD + "Germ" + UI.PST_KEYWORD + " exposure and will begin exhibiting symptoms shortly");
      }

      public class SMELLEDFLOWERS
      {
        public static LocString NAME = (LocString) "Smelled Flowers";
        public static LocString TOOLTIP = (LocString) ("A pleasant " + (string) DUPLICANTS.DISEASES.POLLENGERMS.NAME + " wafted over this Duplicant and brightened their day");
      }

      public class HISTAMINESUPPRESSION
      {
        public static LocString NAME = (LocString) "Antihistamines";
        public static LocString TOOLTIP = (LocString) "This Duplicant's allergic reactions have been suppressed by medication";
      }

      public class FOODSICKNESSRECOVERY
      {
        public static LocString NAME = (LocString) "Food Poisoning Antibodies";
        public static LocString TOOLTIP = (LocString) ("This Duplicant recently recovered from " + (string) DUPLICANTS.DISEASES.FOODSICKNESS.NAME + " and is temporarily immune to the " + UI.PRE_KEYWORD + "Disease" + UI.PST_KEYWORD);
      }

      public class SLIMESICKNESSRECOVERY
      {
        public static LocString NAME = (LocString) "Slimelung Antibodies";
        public static LocString TOOLTIP = (LocString) ("This Duplicant recently recovered from " + (string) DUPLICANTS.DISEASES.SLIMESICKNESS.NAME + " and is temporarily immune to the " + UI.PRE_KEYWORD + "Disease" + UI.PST_KEYWORD);
      }

      public class ZOMBIESICKNESSRECOVERY
      {
        public static LocString NAME = (LocString) "Zombie Spores Antibodies";
        public static LocString TOOLTIP = (LocString) ("This Duplicant recently recovered from " + (string) DUPLICANTS.DISEASES.ZOMBIESICKNESS.NAME + " and is temporarily immune to the " + UI.PRE_KEYWORD + "Disease" + UI.PST_KEYWORD);
      }

      public class MESSTABLESALT
      {
        public static LocString NAME = (LocString) "Salted Food";
        public static LocString TOOLTIP = (LocString) ("This Duplicant had the luxury of using " + UI.PRE_KEYWORD + (string) ITEMS.INDUSTRIAL_PRODUCTS.TABLE_SALT.NAME + UI.PST_KEYWORD + " with their last meal at a " + (string) BUILDINGS.PREFABS.DININGTABLE.NAME);
      }

      public class RADIATIONEXPOSUREMINOR
      {
        public static LocString NAME = (LocString) "Minor Radiation Sickness";
        public static LocString TOOLTIP = (LocString) ("A bit of " + UI.PRE_KEYWORD + "Radiation" + UI.PST_KEYWORD + " exposure has made this Duplicant feel sluggish");
      }

      public class RADIATIONEXPOSUREMAJOR
      {
        public static LocString NAME = (LocString) "Major Radiation Sickness";
        public static LocString TOOLTIP = (LocString) ("Significant " + UI.PRE_KEYWORD + "Radiation" + UI.PST_KEYWORD + " exposure has left this Duplicant totally exhausted");
      }

      public class RADIATIONEXPOSUREEXTREME
      {
        public static LocString NAME = (LocString) "Extreme Radiation Sickness";
        public static LocString TOOLTIP = (LocString) ("Dangerously high " + UI.PRE_KEYWORD + "Radiation" + UI.PST_KEYWORD + " exposure is making this Duplicant wish they'd never been printed");
      }

      public class RADIATIONEXPOSUREDEADLY
      {
        public static LocString NAME = (LocString) "Deadly Radiation Sickness";
        public static LocString TOOLTIP = (LocString) ("Extreme " + UI.PRE_KEYWORD + "Radiation" + UI.PST_KEYWORD + " exposure has incapacitated this Duplicant");
      }

      public class CHARGING
      {
        public static LocString NAME = (LocString) "Charging";
        public static LocString TOOLTIP = (LocString) "This lil bot is charging its internal battery";
      }

      public class BOTSWEEPING
      {
        public static LocString NAME = (LocString) "Sweeping";
        public static LocString TOOLTIP = (LocString) "This lil bot is picking up debris from the floor";
      }

      public class BOTMOPPING
      {
        public static LocString NAME = (LocString) "Mopping";
        public static LocString TOOLTIP = (LocString) "This lil bot is clearing liquids from the ground";
      }

      public class SCOUTBOTCHARGING
      {
        public static LocString NAME = (LocString) "Charging";
        public static LocString TOOLTIP = (LocString) ((string) ROBOTS.MODELS.SCOUT.NAME + " is happily charging inside " + (string) BUILDINGS.PREFABS.SCOUTMODULE.NAME);
      }

      public class CRYOFRIEND
      {
        public static LocString NAME = (LocString) "Motivated By Friend";
        public static LocString TOOLTIP = (LocString) "This Duplicant feels motivated after meeting a long lost friend";
      }

      public class BONUSDREAM1
      {
        public static LocString NAME = (LocString) "Good Dream";
        public static LocString TOOLTIP = (LocString) "This Duplicant had a good dream and is feeling psyched!";
      }

      public class BONUSDREAM2
      {
        public static LocString NAME = (LocString) "Really Good Dream";
        public static LocString TOOLTIP = (LocString) "This Duplicant had a really good dream and is full of possibilities!";
      }

      public class BONUSDREAM3
      {
        public static LocString NAME = (LocString) "Great Dream";
        public static LocString TOOLTIP = (LocString) "This Duplicant had a great dream last night and periodically remembers another great moment they previously forgot";
      }

      public class BONUSDREAM4
      {
        public static LocString NAME = (LocString) "Dream Inspired";
        public static LocString TOOLTIP = (LocString) "This Duplicant is inspired from all the unforgettable dreams they had";
      }

      public class BONUSRESEARCH
      {
        public static LocString NAME = (LocString) "Inspired Learner";
        public static LocString TOOLTIP = (LocString) "This Duplicant is looking forward to some learning";
      }

      public class BONUSTOILET1
      {
        public static LocString NAME = (LocString) "Small Comforts";
        public static LocString TOOLTIP = (LocString) "This Duplicant visited the {building} and appreciated the small comforts";
      }

      public class BONUSTOILET2
      {
        public static LocString NAME = (LocString) "Greater Comforts";
        public static LocString TOOLTIP = (LocString) ("This Duplicant used a " + (string) BUILDINGS.PREFABS.OUTHOUSE.NAME + "and liked how comfortable it felt");
      }

      public class BONUSTOILET3
      {
        public static LocString NAME = (LocString) "Small Luxury";
        public static LocString TOOLTIP = (LocString) ("This Duplicant visited a " + (string) ROOMS.TYPES.LATRINE.NAME + " and feels they could get used to this luxury");
      }

      public class BONUSTOILET4
      {
        public static LocString NAME = (LocString) "Luxurious";
        public static LocString TOOLTIP = (LocString) ("This Duplicant feels endless luxury from the " + (string) ROOMS.TYPES.PRIVATE_BATHROOM.NAME);
      }

      public class BONUSDIGGING1
      {
        public static LocString NAME = (LocString) "Hot Diggity!";
        public static LocString TOOLTIP = (LocString) "This Duplicant did a lot of excavating and is really digging digging";
      }

      public class BONUSSTORAGE
      {
        public static LocString NAME = (LocString) "Something in Store";
        public static LocString TOOLTIP = (LocString) ("This Duplicant stored something in a " + (string) BUILDINGS.PREFABS.STORAGELOCKER.NAME + " and is feeling organized");
      }

      public class BONUSBUILDER
      {
        public static LocString NAME = (LocString) "Accomplished Builder";
        public static LocString TOOLTIP = (LocString) "This Duplicant has built many buildings and has a sense of accomplishment!";
      }

      public class BONUSOXYGEN
      {
        public static LocString NAME = (LocString) "Fresh Air";
        public static LocString TOOLTIP = (LocString) "This Duplicant breathed in some fresh air and is feeling refreshed";
      }

      public class BONUSGENERATOR
      {
        public static LocString NAME = (LocString) "Exercised";
        public static LocString TOOLTIP = (LocString) "This Duplicant ran in a Generator and has benefited from the exercise";
      }

      public class BONUSDOOR
      {
        public static LocString NAME = (LocString) "Open and Shut";
        public static LocString TOOLTIP = (LocString) "This Duplicant closed a door and appreciates the privacy";
      }

      public class BONUSHITTHEBOOKS
      {
        public static LocString NAME = (LocString) "Hit the Books";
        public static LocString TOOLTIP = (LocString) "This Duplicant did some research and is feeling smarter";
      }

      public class BONUSLITWORKSPACE
      {
        public static LocString NAME = (LocString) "Lit";
        public static LocString TOOLTIP = (LocString) "This Duplicant was in a well lit environment and is feeling lit";
      }

      public class BONUSTALKER
      {
        public static LocString NAME = (LocString) "Talker";
        public static LocString TOOLTIP = (LocString) "This Duplicant engaged in small talk with a coworker and is feeling connected";
      }

      public class THRIVER
      {
        public static LocString NAME = (LocString) "Clutchy";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is " + UI.PRE_KEYWORD + "Stressed" + UI.PST_KEYWORD + " and has kicked into hyperdrive");
      }

      public class LONER
      {
        public static LocString NAME = (LocString) "Alone";
        public static LocString TOOLTIP = (LocString) "This Duplicant is more feeling focused now that they're alone";
      }

      public class STARRYEYED
      {
        public static LocString NAME = (LocString) "Starry Eyed";
        public static LocString TOOLTIP = (LocString) "This Duplicant loves being in space!";
      }

      public class WAILEDAT
      {
        public static LocString NAME = (LocString) "Disturbed by Wailing";
        public static LocString TOOLTIP = (LocString) ("This Duplicant is feeling " + UI.PRE_KEYWORD + "Stressed" + UI.PST_KEYWORD + " by someone's Banshee Wail");
      }
    }

    public class CONGENITALTRAITS
    {
      public class NONE
      {
        public static LocString NAME = (LocString) "None";
        public static LocString DESC = (LocString) "This Duplicant seems pretty average overall";
      }

      public class JOSHUA
      {
        public static LocString NAME = (LocString) "Cheery Disposition";
        public static LocString DESC = (LocString) "This Duplicant brightens others' days wherever he goes";
      }

      public class ELLIE
      {
        public static LocString NAME = (LocString) "Fastidious";
        public static LocString DESC = (LocString) "This Duplicant needs things done in a very particular way";
      }

      public class LIAM
      {
        public static LocString NAME = (LocString) "Germaphobe";
        public static LocString DESC = (LocString) "This Duplicant has an all-consuming fear of bacteria";
      }

      public class BANHI
      {
        public static LocString NAME = (LocString) "";
        public static LocString DESC = (LocString) "";
      }

      public class STINKY
      {
        public static LocString NAME = (LocString) "Stinkiness";
        public static LocString DESC = (LocString) "This Duplicant is genetically cursed by a pungent bodily odor";
      }
    }

    public class TRAITS
    {
      public static LocString TRAIT_DESCRIPTION_LIST_ENTRY = (LocString) "\n• ";
      public static LocString ATTRIBUTE_MODIFIERS = (LocString) "{0}: {1}";
      public static LocString CANNOT_DO_TASK = (LocString) "Cannot do <b>{0} Errands</b>";
      public static LocString CANNOT_DO_TASK_TOOLTIP = (LocString) "{0}: {1}";
      public static LocString REFUSES_TO_DO_TASK = (LocString) "Cannot do<b>{0} Errands</b>";
      public static LocString IGNORED_EFFECTS = (LocString) "Immune to <b>{0}</b>";
      public static LocString IGNORED_EFFECTS_TOOLTIP = (LocString) "{0}: {1}";
      public static LocString GRANTED_SKILL_SHARED_NAME = (LocString) "Skilled: ";
      public static LocString GRANTED_SKILL_SHARED_DESC = (LocString) ("This Duplicant begins with a pre-learned " + UI.FormatAsKeyWord("Skill") + ", but does not have increased " + UI.FormatAsKeyWord((string) DUPLICANTS.NEEDS.QUALITYOFLIFE.NAME) + ".\n\n{0}\n{1}");
      public static LocString GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP = (LocString) ("This Duplicant receives a free " + UI.FormatAsKeyWord("Skill") + " without the drawback of increased " + UI.FormatAsKeyWord((string) DUPLICANTS.NEEDS.QUALITYOFLIFE.NAME));

      public class CHATTY
      {
        public static LocString NAME = (LocString) "Charismatic";
        public static LocString DESC = (LocString) ("This Duplicant's so charming, chatting with them is sometimes enough to trigger an " + UI.PRE_KEYWORD + "Overjoyed" + UI.PST_KEYWORD + " response");
      }

      public class NEEDS
      {
        public class CLAUSTROPHOBIC
        {
          public static LocString NAME = (LocString) "Claustrophobic";
          public static LocString DESC = (LocString) "This Duplicant feels suffocated in spaces fewer than four tiles high or three tiles wide";
        }

        public class FASHIONABLE
        {
          public static LocString NAME = (LocString) "Fashionista";
          public static LocString DESC = (LocString) "This Duplicant dies a bit inside when forced to wear unstylish clothing";
        }

        public class CLIMACOPHOBIC
        {
          public static LocString NAME = (LocString) "Vertigo Prone";
          public static LocString DESC = (LocString) "Climbing ladders more than four tiles tall makes this Duplicant's stomach do flips";
        }

        public class SOLITARYSLEEPER
        {
          public static LocString NAME = (LocString) "Solitary Sleeper";
          public static LocString DESC = (LocString) "This Duplicant prefers to sleep alone";
        }

        public class PREFERSWARMER
        {
          public static LocString NAME = (LocString) "Skinny";
          public static LocString DESC = (LocString) ("This Duplicant doesn't have much " + UI.PRE_KEYWORD + "Insulation" + UI.PST_KEYWORD + ", so they are more " + UI.PRE_KEYWORD + "Temperature" + UI.PST_KEYWORD + " sensitive than others");
        }

        public class PREFERSCOOLER
        {
          public static LocString NAME = (LocString) "Pudgy";
          public static LocString DESC = (LocString) ("This Duplicant has some extra " + UI.PRE_KEYWORD + "Insulation" + UI.PST_KEYWORD + ", so the room " + UI.PRE_KEYWORD + "Temperature" + UI.PST_KEYWORD + " affects them a little less");
        }

        public class SENSITIVEFEET
        {
          public static LocString NAME = (LocString) "Delicate Feetsies";
          public static LocString DESC = (LocString) "This Duplicant is a sensitive sole and would rather walk on tile than raw bedrock";
        }

        public class WORKAHOLIC
        {
          public static LocString NAME = (LocString) "Workaholic";
          public static LocString DESC = (LocString) "This Duplicant gets antsy when left idle";
        }
      }

      public class ANCIENTKNOWLEDGE
      {
        public static LocString NAME = (LocString) "Ancient Knowledge";
        public static LocString DESC = (LocString) "This Duplicant has knowledge from the before times\n• Starts with 3 skill points";
      }

      public class CANTRESEARCH
      {
        public static LocString NAME = (LocString) "Yokel";
        public static LocString DESC = (LocString) "This Duplicant isn't the brightest star in the solar system";
      }

      public class CANTBUILD
      {
        public static LocString NAME = (LocString) "Unconstructive";
        public static LocString DESC = (LocString) "This Duplicant is incapable of building even the most basic of structures";
      }

      public class CANTCOOK
      {
        public static LocString NAME = (LocString) "Gastrophobia";
        public static LocString DESC = (LocString) "This Duplicant has a deep-seated distrust of the culinary arts";
      }

      public class CANTDIG
      {
        public static LocString NAME = (LocString) "Trypophobia";
        public static LocString DESC = (LocString) "This Duplicant's fear of holes makes it impossible for them to dig";
      }

      public class HEMOPHOBIA
      {
        public static LocString NAME = (LocString) "Squeamish";
        public static LocString DESC = (LocString) "This Duplicant is of delicate disposition and cannot tend to the sick";
      }

      public class BEDSIDEMANNER
      {
        public static LocString NAME = (LocString) "Caregiver";
        public static LocString DESC = (LocString) "This Duplicant has good bedside manner and a healing touch";
      }

      public class MOUTHBREATHER
      {
        public static LocString NAME = (LocString) "Mouth Breather";
        public static LocString DESC = (LocString) ("This Duplicant sucks up way more than their fair share of " + (string) ELEMENTS.OXYGEN.NAME);
      }

      public class FUSSY
      {
        public static LocString NAME = (LocString) "Fussy";
        public static LocString DESC = (LocString) "Nothing's ever quite good enough for this Duplicant";
      }

      public class TWINKLETOES
      {
        public static LocString NAME = (LocString) "Twinkletoes";
        public static LocString DESC = (LocString) "This Duplicant is light as a feather on their feet";
      }

      public class STRONGARM
      {
        public static LocString NAME = (LocString) "Buff";
        public static LocString DESC = (LocString) "This Duplicant has muscles on their muscles";
      }

      public class NOODLEARMS
      {
        public static LocString NAME = (LocString) "Noodle Arms";
        public static LocString DESC = (LocString) "This Duplicant's arms have all the tensile strength of overcooked linguine";
      }

      public class AGGRESSIVE
      {
        public static LocString NAME = (LocString) "Destructive";
        public static LocString DESC = (LocString) "This Duplicant handles stress by taking their frustrations out on defenseless machines";
        public static LocString NOREPAIR = (LocString) ("• Will not repair buildings while above 60% " + UI.PRE_KEYWORD + "Stress" + UI.PST_KEYWORD);
      }

      public class UGLYCRIER
      {
        public static LocString NAME = (LocString) "Ugly Crier";
        public static LocString DESC = (LocString) ("If this Duplicant gets too " + UI.PRE_KEYWORD + "Stressed" + UI.PST_KEYWORD + " it won't be pretty");
      }

      public class BINGEEATER
      {
        public static LocString NAME = (LocString) "Binge Eater";
        public static LocString DESC = (LocString) ("This Duplicant will dangerously overeat when " + UI.PRE_KEYWORD + "Stressed" + UI.PST_KEYWORD);
      }

      public class ANXIOUS
      {
        public static LocString NAME = (LocString) "Anxious";
        public static LocString DESC = (LocString) "This Duplicant collapses when put under too much pressure";
      }

      public class STRESSVOMITER
      {
        public static LocString NAME = (LocString) "Vomiter";
        public static LocString DESC = (LocString) ("This Duplicant is liable to puke everywhere when " + UI.PRE_KEYWORD + "Stressed" + UI.PST_KEYWORD);
      }

      public class BANSHEE
      {
        public static LocString NAME = (LocString) "Banshee";
        public static LocString DESC = (LocString) ("This Duplicant wails uncontrollably when " + UI.PRE_KEYWORD + "Stressed" + UI.PST_KEYWORD);
      }

      public class BALLOONARTIST
      {
        public static LocString NAME = (LocString) "Balloon Artist";
        public static LocString DESC = (LocString) ("This Duplicant hands out balloons when they are " + UI.PRE_KEYWORD + "Overjoyed" + UI.PST_KEYWORD);
      }

      public class SPARKLESTREAKER
      {
        public static LocString NAME = (LocString) "Sparkle Streaker";
        public static LocString DESC = (LocString) ("This Duplicant leaves a trail of happy sparkles when they are " + UI.PRE_KEYWORD + "Overjoyed" + UI.PST_KEYWORD);
      }

      public class STICKERBOMBER
      {
        public static LocString NAME = (LocString) "Sticker Bomber";
        public static LocString DESC = (LocString) ("This Duplicant will spontaneously redecorate a room when they are " + UI.PRE_KEYWORD + "Overjoyed" + UI.PST_KEYWORD);
      }

      public class SUPERPRODUCTIVE
      {
        public static LocString NAME = (LocString) "Super Productive";
        public static LocString DESC = (LocString) ("This Duplicant is super productive when they are " + UI.PRE_KEYWORD + "Overjoyed" + UI.PST_KEYWORD);
      }

      public class HAPPYSINGER
      {
        public static LocString NAME = (LocString) "Yodeler";
        public static LocString DESC = (LocString) ("This Duplicant belts out catchy tunes when they are " + UI.PRE_KEYWORD + "Overjoyed" + UI.PST_KEYWORD);
      }

      public class IRONGUT
      {
        public static LocString NAME = (LocString) "Iron Gut";
        public static LocString DESC = (LocString) "This Duplicant can eat just about anything without getting sick";
        public static LocString SHORT_DESC = (LocString) ("Immune to <b>" + (string) DUPLICANTS.DISEASES.FOODSICKNESS.NAME + "</b>");
        public static LocString SHORT_DESC_TOOLTIP = (LocString) ("Eating food contaminated with " + (string) DUPLICANTS.DISEASES.FOODSICKNESS.NAME + " Germs will not affect this Duplicant");
      }

      public class STRONGIMMUNESYSTEM
      {
        public static LocString NAME = (LocString) "Germ Resistant";
        public static LocString DESC = (LocString) "This Duplicant's immune system bounces back faster than most";
      }

      public class SCAREDYCAT
      {
        public static LocString NAME = (LocString) "Pacifist";
        public static LocString DESC = (LocString) "This Duplicant abhors violence";
      }

      public class ALLERGIES
      {
        public static LocString NAME = (LocString) "Allergies";
        public static LocString DESC = (LocString) ("This Duplicant will sneeze uncontrollably when exposed to the pollen present in " + (string) DUPLICANTS.DISEASES.POLLENGERMS.NAME);
        public static LocString SHORT_DESC = (LocString) ("Allergic reaction to <b>" + (string) DUPLICANTS.DISEASES.POLLENGERMS.NAME + "</b>");
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.DISEASES.ALLERGIES.DESCRIPTIVE_SYMPTOMS;
      }

      public class WEAKIMMUNESYSTEM
      {
        public static LocString NAME = (LocString) "Biohazardous";
        public static LocString DESC = (LocString) "All the vitamin C in space couldn't stop this Duplicant from getting sick";
      }

      public class IRRITABLEBOWEL
      {
        public static LocString NAME = (LocString) "Irritable Bowel";
        public static LocString DESC = (LocString) "This Duplicant needs a little extra time to \"do their business\"";
      }

      public class CALORIEBURNER
      {
        public static LocString NAME = (LocString) "Bottomless Stomach";
        public static LocString DESC = (LocString) "This Duplicant might actually be several black holes in a trench coat";
      }

      public class SMALLBLADDER
      {
        public static LocString NAME = (LocString) "Small Bladder";
        public static LocString DESC = (LocString) ("This Duplicant has a tiny, pea-sized " + UI.PRE_KEYWORD + "Bladder" + UI.PST_KEYWORD + ". Adorable!");
      }

      public class ANEMIC
      {
        public static LocString NAME = (LocString) "Anemic";
        public static LocString DESC = (LocString) "This Duplicant has trouble keeping up with the others";
      }

      public class GREASEMONKEY
      {
        public static LocString NAME = (LocString) "Grease Monkey";
        public static LocString DESC = (LocString) "This Duplicant likes to throw a wrench into the colony's plans... in a good way";
      }

      public class MOLEHANDS
      {
        public static LocString NAME = (LocString) "Mole Hands";
        public static LocString DESC = (LocString) "They're great for tunneling, but finding good gloves is a nightmare";
      }

      public class FASTLEARNER
      {
        public static LocString NAME = (LocString) "Quick Learner";
        public static LocString DESC = (LocString) "This Duplicant's sharp as a tack and learns new skills with amazing speed";
      }

      public class SLOWLEARNER
      {
        public static LocString NAME = (LocString) "Slow Learner";
        public static LocString DESC = (LocString) "This Duplicant's a little slow on the uptake, but gosh do they try";
      }

      public class DIVERSLUNG
      {
        public static LocString NAME = (LocString) "Diver's Lungs";
        public static LocString DESC = (LocString) "This Duplicant could have been a talented opera singer in another life";
      }

      public class FLATULENCE
      {
        public static LocString NAME = (LocString) "Flatulent";
        public static LocString DESC = (LocString) "Some Duplicants are just full of it";
        public static LocString SHORT_DESC = (LocString) "Farts frequently";
        public static LocString SHORT_DESC_TOOLTIP = (LocString) ("This Duplicant will periodically \"output\" " + (string) ELEMENTS.METHANE.NAME);
      }

      public class SNORER
      {
        public static LocString NAME = (LocString) "Loud Sleeper";
        public static LocString DESC = (LocString) "In space, everyone can hear you snore";
        public static LocString SHORT_DESC = (LocString) "Snores loudly";
        public static LocString SHORT_DESC_TOOLTIP = (LocString) "This Duplicant's snoring will rudely awake nearby friends";
      }

      public class NARCOLEPSY
      {
        public static LocString NAME = (LocString) "Narcoleptic";
        public static LocString DESC = (LocString) "This Duplicant can and will fall asleep anytime, anyplace";
        public static LocString SHORT_DESC = (LocString) "Falls asleep periodically";
        public static LocString SHORT_DESC_TOOLTIP = (LocString) "This Duplicant's work will be periodically interrupted by naps";
      }

      public class INTERIORDECORATOR
      {
        public static LocString NAME = (LocString) "Interior Decorator";
        public static LocString DESC = (LocString) "\"Move it a little to the left...\"";
      }

      public class UNCULTURED
      {
        public static LocString NAME = (LocString) "Uncultured";
        public static LocString DESC = (LocString) "This Duplicant has simply no appreciation for the arts";
      }

      public class EARLYBIRD
      {
        public static LocString NAME = (LocString) "Early Bird";
        public static LocString DESC = (LocString) "This Duplicant always wakes up feeling fresh and efficient!";
        public static LocString EXTENDED_DESC = (LocString) ("• Morning: <b>{0}</b> bonus to all " + UI.PRE_KEYWORD + "Attributes" + UI.PST_KEYWORD + "\n• Duration: 5 Schedule Blocks");
        public static LocString SHORT_DESC = (LocString) "Gains morning Attribute bonuses";
        public static LocString SHORT_DESC_TOOLTIP = (LocString) ("Morning: <b>+2</b> bonus to all " + UI.PRE_KEYWORD + "Attributes" + UI.PST_KEYWORD + "\n• Duration: 5 Schedule Blocks");
      }

      public class NIGHTOWL
      {
        public static LocString NAME = (LocString) "Night Owl";
        public static LocString DESC = (LocString) "This Duplicant does their best work when they'd ought to be sleeping";
        public static LocString EXTENDED_DESC = (LocString) ("• Nighttime: <b>{0}</b> bonus to all " + UI.PRE_KEYWORD + "Attributes" + UI.PST_KEYWORD + "\n• Duration: All Night");
        public static LocString SHORT_DESC = (LocString) "Gains nighttime Attribute bonuses";
        public static LocString SHORT_DESC_TOOLTIP = (LocString) ("Nighttime: <b>+3</b> bonus to all " + UI.PRE_KEYWORD + "Attributes" + UI.PST_KEYWORD + "\n• Duration: All Night");
      }

      public class REGENERATION
      {
        public static LocString NAME = (LocString) "Regenerative";
        public static LocString DESC = (LocString) "This robust Duplicant is constantly regenerating health";
      }

      public class DEEPERDIVERSLUNGS
      {
        public static LocString NAME = (LocString) "Deep Diver's Lungs";
        public static LocString DESC = (LocString) "This Duplicant has a frankly impressive ability to hold their breath";
      }

      public class SUNNYDISPOSITION
      {
        public static LocString NAME = (LocString) "Sunny Disposition";
        public static LocString DESC = (LocString) "This Duplicant has an unwaveringly positive outlook on life";
      }

      public class ROCKCRUSHER
      {
        public static LocString NAME = (LocString) "Beefsteak";
        public static LocString DESC = (LocString) "This Duplicant's got muscles on their muscles!";
      }

      public class SIMPLETASTES
      {
        public static LocString NAME = (LocString) "Shrivelled Tastebuds";
        public static LocString DESC = (LocString) "This Duplicant could lick a Puft's backside and taste nothing";
      }

      public class FOODIE
      {
        public static LocString NAME = (LocString) "Gourmet";
        public static LocString DESC = (LocString) "This Duplicant's refined palate demands only the most luxurious dishes the colony can offer";
      }

      public class ARCHAEOLOGIST
      {
        public static LocString NAME = (LocString) "Relic Hunter";
        public static LocString DESC = (LocString) "This Duplicant was never taught the phrase \"take only pictures, leave only footprints\"";
      }

      public class DECORUP
      {
        public static LocString NAME = (LocString) "Innately Stylish";
        public static LocString DESC = (LocString) "This Duplicant's radiant self-confidence makes even the rattiest outfits look trendy";
      }

      public class DECORDOWN
      {
        public static LocString NAME = (LocString) "Shabby Dresser";
        public static LocString DESC = (LocString) "This Duplicant's clearly never heard of ironing";
      }

      public class THRIVER
      {
        public static LocString NAME = (LocString) "Duress to Impress";
        public static LocString DESC = (LocString) "This Duplicant kicks into hyperdrive when the stress is on";
        public static LocString SHORT_DESC = (LocString) "Attribute bonuses while stressed";
        public static LocString SHORT_DESC_TOOLTIP = (LocString) ("More than 60% Stress: <b>+7</b> bonus to all " + UI.FormatAsKeyWord("Attributes"));
      }

      public class LONER
      {
        public static LocString NAME = (LocString) "Loner";
        public static LocString DESC = (LocString) "This Duplicant prefers solitary pursuits";
        public static LocString SHORT_DESC = (LocString) "Attribute bonuses while alone";
        public static LocString SHORT_DESC_TOOLTIP = (LocString) ("Only Duplicant on a world: <b>+4</b> bonus to all " + UI.FormatAsKeyWord("Attributes"));
      }

      public class STARRYEYED
      {
        public static LocString NAME = (LocString) "Starry Eyed";
        public static LocString DESC = (LocString) "This Duplicant loves being in space";
        public static LocString SHORT_DESC = (LocString) "Morale bonus while in space";
        public static LocString SHORT_DESC_TOOLTIP = (LocString) ("In outer space: <b>+10</b> " + UI.FormatAsKeyWord("Morale"));
      }

      public class GLOWSTICK
      {
        public static LocString NAME = (LocString) "Glow Stick";
        public static LocString DESC = (LocString) "This Duplicant is positively glowing";
        public static LocString SHORT_DESC = (LocString) "Emits low amounts of rads and light";
        public static LocString SHORT_DESC_TOOLTIP = (LocString) "Emits low amounts of rads and light";
      }

      public class RADIATIONEATER
      {
        public static LocString NAME = (LocString) "Radiation Eater";
        public static LocString DESC = (LocString) "This Duplicant eats radiation for breakfast (and dinner)";
        public static LocString SHORT_DESC = (LocString) "Converts radiation exposure into calories";
        public static LocString SHORT_DESC_TOOLTIP = (LocString) "Converts radiation exposure into calories";
      }

      public class NIGHTLIGHT
      {
        public static LocString NAME = (LocString) "Nyctophobic";
        public static LocString DESC = (LocString) "This Duplicant will imagine scary shapes in the dark all night if no one leaves a light on";
        public static LocString SHORT_DESC = (LocString) "Requires light to sleep";
        public static LocString SHORT_DESC_TOOLTIP = (LocString) "This Duplicant can't sleep in complete darkness";
      }

      public class GREENTHUMB
      {
        public static LocString NAME = (LocString) "Green Thumb";
        public static LocString DESC = (LocString) "This Duplicant regards every plant as a potential friend";
      }

      public class CONSTRUCTIONUP
      {
        public static LocString NAME = (LocString) "Handy";
        public static LocString DESC = (LocString) "This Duplicant is a swift and skilled builder";
      }

      public class RANCHINGUP
      {
        public static LocString NAME = (LocString) "Animal Lover";
        public static LocString DESC = (LocString) "The fuzzy snoots! The little claws! The chitinous exoskeletons! This Duplicant's never met a critter they didn't like";
      }

      public class CONSTRUCTIONDOWN
      {
        public static LocString NAME = (LocString) "Building Impaired";
        public static LocString DESC = (LocString) "This Duplicant has trouble constructing anything besides meaningful friendships";
      }

      public class RANCHINGDOWN
      {
        public static LocString NAME = (LocString) "Critter Aversion";
        public static LocString DESC = (LocString) "This Duplicant just doesn't trust those beady little eyes";
      }

      public class DIGGINGDOWN
      {
        public static LocString NAME = (LocString) "Undigging";
        public static LocString DESC = (LocString) "This Duplicant couldn't dig themselves out of a paper bag";
      }

      public class MACHINERYDOWN
      {
        public static LocString NAME = (LocString) "Luddite";
        public static LocString DESC = (LocString) "This Duplicant always invites friends over just to make them hook up their electronics";
      }

      public class COOKINGDOWN
      {
        public static LocString NAME = (LocString) "Kitchen Menace";
        public static LocString DESC = (LocString) "This Duplicant could probably figure out a way to burn ice cream";
      }

      public class ARTDOWN
      {
        public static LocString NAME = (LocString) "Unpracticed Artist";
        public static LocString DESC = (LocString) "This Duplicant proudly proclaims they \"can't even draw a stick figure\"";
      }

      public class CARINGDOWN
      {
        public static LocString NAME = (LocString) "Unempathetic";
        public static LocString DESC = (LocString) "This Duplicant's lack of bedside manner makes it difficult for them to nurse peers back to health";
      }

      public class BOTANISTDOWN
      {
        public static LocString NAME = (LocString) "Plant Murderer";
        public static LocString DESC = (LocString) "Never ask this Duplicant to watch your ferns when you go on vacation";
      }

      public class GRANTSKILL_MINING1
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.JUNIOR_MINER.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.JUNIOR_MINER.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 1 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_MINING2
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.MINER.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.MINER.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 2 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_MINING3
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.SENIOR_MINER.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.SENIOR_MINER.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 3 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_MINING4
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.MASTER_MINER.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.MASTER_MINER.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 4 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_BUILDING1
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.JUNIOR_BUILDER.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.JUNIOR_BUILDER.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 1 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_BUILDING2
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.BUILDER.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.BUILDER.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 2 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_BUILDING3
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.SENIOR_BUILDER.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.SENIOR_BUILDER.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 3 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_FARMING1
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.JUNIOR_FARMER.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.JUNIOR_FARMER.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 1 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_FARMING2
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.FARMER.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.FARMER.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 2 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_FARMING3
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.SENIOR_FARMER.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.SENIOR_FARMER.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 3 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_RANCHING1
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.RANCHER.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.RANCHER.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 2 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_RANCHING2
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.SENIOR_RANCHER.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.SENIOR_RANCHER.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 3 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_RESEARCHING1
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.JUNIOR_RESEARCHER.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.JUNIOR_RESEARCHER.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 1 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_RESEARCHING2
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.RESEARCHER.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.RESEARCHER.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 2 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_RESEARCHING3
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.SENIOR_RESEARCHER.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.SENIOR_RESEARCHER.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 3 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_RESEARCHING4
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.NUCLEAR_RESEARCHER.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.NUCLEAR_RESEARCHER.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 3 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_COOKING1
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.JUNIOR_COOK.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.JUNIOR_COOK.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 1 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_COOKING2
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.COOK.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.COOK.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 2 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_ARTING1
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.JUNIOR_ARTIST.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.JUNIOR_ARTIST.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 1 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_ARTING2
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.ARTIST.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.ARTIST.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 2 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_ARTING3
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.MASTER_ARTIST.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.MASTER_ARTIST.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 3 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_HAULING1
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.HAULER.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.HAULER.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 1 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_HAULING2
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.MATERIALS_MANAGER.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.MATERIALS_MANAGER.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 2 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_SUITS1
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.SUIT_EXPERT.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.SUIT_EXPERT.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 3 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_TECHNICALS1
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.MACHINE_TECHNICIAN.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.MACHINE_TECHNICIAN.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 1 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_TECHNICALS2
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.POWER_TECHNICIAN.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.POWER_TECHNICIAN.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 2 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_ENGINEERING1
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.MECHATRONIC_ENGINEER.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.MECHATRONIC_ENGINEER.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 3 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_BASEKEEPING1
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.HANDYMAN.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.HANDYMAN.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 1 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_BASEKEEPING2
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.PLUMBER.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.PLUMBER.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 2 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_ASTRONAUTING1
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.ASTRONAUTTRAINEE.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.ASTRONAUTTRAINEE.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 4 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_ASTRONAUTING2
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.ASTRONAUT.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.ASTRONAUT.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 5 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_MEDICINE1
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.JUNIOR_MEDIC.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.JUNIOR_MEDIC.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 1 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_MEDICINE2
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.MEDIC.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.MEDIC.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 2 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }

      public class GRANTSKILL_MEDICINE3
      {
        public static LocString NAME = (LocString) ((string) DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_NAME + (string) DUPLICANTS.ROLES.SENIOR_MEDIC.NAME);
        public static LocString DESC = DUPLICANTS.ROLES.SENIOR_MEDIC.DESCRIPTION;
        public static LocString SHORT_DESC = (LocString) "Starts with a Tier 3 <b>Skill</b>";
        public static LocString SHORT_DESC_TOOLTIP = DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_SHORT_DESC_TOOLTIP;
      }
    }

    public class PERSONALITIES
    {
      public class CATALINA
      {
        public static LocString NAME = (LocString) "Catalina";
        public static LocString DESC = (LocString) "A {0} is admired by all for her seemingly tireless work ethic. Little do people know, she's dying on the inside.";
      }

      public class NISBET
      {
        public static LocString NAME = (LocString) "Nisbet";
        public static LocString DESC = (LocString) "This {0} likes to punch people to show her affection. Everyone's too afraid of her to tell her it hurts.";
      }

      public class ELLIE
      {
        public static LocString NAME = (LocString) "Ellie";
        public static LocString DESC = (LocString) "Nothing makes an {0} happier than a big tin of glitter and a pack of unicorn stickers.";
      }

      public class RUBY
      {
        public static LocString NAME = (LocString) "Ruby";
        public static LocString DESC = (LocString) "This {0} asks the pressing questions, like \"Where can I get a leather jacket in space?\"";
      }

      public class LEIRA
      {
        public static LocString NAME = (LocString) "Leira";
        public static LocString DESC = (LocString) "{0}s just want everyone to be happy.";
      }

      public class BUBBLES
      {
        public static LocString NAME = (LocString) "Bubbles";
        public static LocString DESC = (LocString) "This {0} is constantly challenging others to fight her, regardless of whether or not she can actually take them.";
      }

      public class MIMA
      {
        public static LocString NAME = (LocString) "Mi-Ma";
        public static LocString DESC = (LocString) "Ol' {0} here can't stand lookin' at people's knees.";
      }

      public class NAILS
      {
        public static LocString NAME = (LocString) "Nails";
        public static LocString DESC = (LocString) "People often expect a Duplicant named \"{0}\" to be tough, but they're all pretty huge wimps.";
      }

      public class MAE
      {
        public static LocString NAME = (LocString) "Mae";
        public static LocString DESC = (LocString) "There's nothing a {0} can't do if she sets her mind to it.";
      }

      public class GOSSMANN
      {
        public static LocString NAME = (LocString) "Gossmann";
        public static LocString DESC = (LocString) "{0}s are major goofballs who can make anyone laugh.";
      }

      public class MARIE
      {
        public static LocString NAME = (LocString) "Marie";
        public static LocString DESC = (LocString) "This {0} is positively glowing! What's her secret? Radioactive isotopes, of course.";
      }

      public class LINDSAY
      {
        public static LocString NAME = (LocString) "Lindsay";
        public static LocString DESC = (LocString) "A {0} is a charming woman, unless you make the mistake of messing with one of her friends.";
      }

      public class DEVON
      {
        public static LocString NAME = (LocString) "Devon";
        public static LocString DESC = (LocString) "This {0} dreams of owning their own personal computer so they can start a blog full of pictures of toast.";
      }

      public class REN
      {
        public static LocString NAME = (LocString) "Ren";
        public static LocString DESC = (LocString) "Every {0} has this unshakable feeling that his life's already happened and he's just watching it unfold from a memory.";
      }

      public class FRANKIE
      {
        public static LocString NAME = (LocString) "Frankie";
        public static LocString DESC = (LocString) "There's nothing {0}s are more proud of than their thick, dignified eyebrows.";
      }

      public class BANHI
      {
        public static LocString NAME = (LocString) "Banhi";
        public static LocString DESC = (LocString) "The \"cool loner\" vibes that radiate off a {0} never fail to make the colony swoon.";
      }

      public class ADA
      {
        public static LocString NAME = (LocString) "Ada";
        public static LocString DESC = (LocString) "{0}s enjoy writing poetry in their downtime. Dark poetry.";
      }

      public class HASSAN
      {
        public static LocString NAME = (LocString) "Hassan";
        public static LocString DESC = (LocString) "If someone says something nice to a {0} he'll think about it nonstop for no less than three weeks.";
      }

      public class STINKY
      {
        public static LocString NAME = (LocString) "Stinky";
        public static LocString DESC = (LocString) "This {0} has never been invited to a party, which is a shame. His dance moves are incredible.";
      }

      public class JOSHUA
      {
        public static LocString NAME = (LocString) "Joshua";
        public static LocString DESC = (LocString) "{0}s are precious goobers. Other Duplicants are strangely incapable of cursing in a {0}'s presence.";
      }

      public class LIAM
      {
        public static LocString NAME = (LocString) "Liam";
        public static LocString DESC = (LocString) "No matter how much this {0} scrubs, he can never truly feel clean.";
      }

      public class ABE
      {
        public static LocString NAME = (LocString) "Abe";
        public static LocString DESC = (LocString) "{0}s are sweet, delicate flowers. They need to be treated gingerly, with great consideration for their feelings.";
      }

      public class BURT
      {
        public static LocString NAME = (LocString) "Burt";
        public static LocString DESC = (LocString) "This {0} always feels great after a bubble bath and a good long cry.";
      }

      public class TRAVALDO
      {
        public static LocString NAME = (LocString) "Travaldo";
        public static LocString DESC = (LocString) "A {0}'s monotonous voice and lack of facial expression makes it impossible for others to tell when he's messing with them.";
      }

      public class HAROLD
      {
        public static LocString NAME = (LocString) "Harold";
        public static LocString DESC = (LocString) "Get a bunch of {0}s together in a room, and you'll have... a bunch of {0}s together in a room.";
      }

      public class MAX
      {
        public static LocString NAME = (LocString) "Max";
        public static LocString DESC = (LocString) "At any given moment a {0} is viscerally reliving ten different humiliating memories.";
      }

      public class ROWAN
      {
        public static LocString NAME = (LocString) "Rowan";
        public static LocString DESC = (LocString) "{0}s have exceptionally large hearts and express their emotions most efficiently by yelling.";
      }

      public class OTTO
      {
        public static LocString NAME = (LocString) "Otto";
        public static LocString DESC = (LocString) "{0}s always insult people by accident and generally exist in a perpetual state of deep regret.";
      }

      public class TURNER
      {
        public static LocString NAME = (LocString) "Turner";
        public static LocString DESC = (LocString) "This {0} is paralyzed by the knowledge that others have memories and perceptions of them they can't control.";
      }

      public class NIKOLA
      {
        public static LocString NAME = (LocString) "Nikola";
        public static LocString DESC = (LocString) "This {0} once claimed he could build a laser so powerful it would rip the colony in half. No one asked him to prove it.";
      }

      public class MEEP
      {
        public static LocString NAME = (LocString) "Meep";
        public static LocString DESC = (LocString) "{0}s have a face only a two tonne Printing Pod could love.";
      }

      public class ARI
      {
        public static LocString NAME = (LocString) "Ari";
        public static LocString DESC = (LocString) "{0}s tend to space out from time to time, but they always pay attention when it counts.";
      }

      public class JEAN
      {
        public static LocString NAME = (LocString) "Jean";
        public static LocString DESC = (LocString) "Just because {0}s are a little slow doesn't mean they can't suffer from soul-crushing existential crises.";
      }

      public class CAMILLE
      {
        public static LocString NAME = (LocString) "Camille";
        public static LocString DESC = (LocString) "This {0} loves anything that makes her feel nostalgic, including things that haven't aged well.";
      }

      public class ASHKAN
      {
        public static LocString NAME = (LocString) "Ashkan";
        public static LocString DESC = (LocString) "{0}s have what can only be described as a \"seriously infectious giggle\".";
      }

      public class STEVE
      {
        public static LocString NAME = (LocString) "Steve";
        public static LocString DESC = (LocString) "This {0} is convinced that he has psychic powers. And he knows exactly what his friends think about that.";
      }

      public class AMARI
      {
        public static LocString NAME = (LocString) "Amari";
        public static LocString DESC = (LocString) "{0}s likes to keep the peace. Ironically, they're a riot at parties.";
      }

      public class PEI
      {
        public static LocString NAME = (LocString) "Pei";
        public static LocString DESC = (LocString) "Every {0} spends at least half the day pretending that they remember what they came into this room for.";
      }

      public class QUINN
      {
        public static LocString NAME = (LocString) "Quinn";
        public static LocString DESC = (LocString) "This {0}'s favorite genre of music is \"festive power ballad\".";
      }

      public class JORGE
      {
        public static LocString NAME = (LocString) "Jorge";
        public static LocString DESC = (LocString) "{0} is very excited to join the colony and settle into their new home!";
      }
    }

    public class NEEDS
    {
      public class DECOR
      {
        public static LocString NAME = (LocString) "Decor Expectation";
        public static LocString PROFESSION_NAME = (LocString) "Critic";
        public static LocString OBSERVED_DECOR = (LocString) "Current Surroundings";
        public static LocString EXPECTATION_TOOLTIP = (LocString) ("Most objects have " + UI.PRE_KEYWORD + "Decor" + UI.PST_KEYWORD + " values that alter Duplicants' opinions of their surroundings.\nThis Duplicant desires " + UI.PRE_KEYWORD + "Decor" + UI.PST_KEYWORD + " values of <b>{0}</b> or higher, and becomes " + UI.PRE_KEYWORD + "Stressed" + UI.PST_KEYWORD + " in areas with lower " + UI.PRE_KEYWORD + "Decor" + UI.PST_KEYWORD + ".");
        public static LocString EXPECTATION_MOD_NAME = (LocString) "Job Tier Request";
      }

      public class FOOD_QUALITY
      {
        public static LocString NAME = (LocString) "Food Quality";
        public static LocString PROFESSION_NAME = (LocString) "Gourmet";
        public static LocString EXPECTATION_TOOLTIP = (LocString) ("Each Duplicant has a minimum quality of " + UI.PRE_KEYWORD + "Food" + UI.PST_KEYWORD + " they'll tolerate eating.\nThis Duplicant desires <b>Tier {0}<b> or better " + UI.PRE_KEYWORD + "Food" + UI.PST_KEYWORD + ", and becomes " + UI.PRE_KEYWORD + "Stressed" + UI.PST_KEYWORD + " when they eat meals of lower quality.");
        public static LocString BAD_FOOD_MOD = (LocString) "Food Quality";
        public static LocString NORMAL_FOOD_MOD = (LocString) "Food Quality";
        public static LocString GOOD_FOOD_MOD = (LocString) "Food Quality";
        public static LocString EXPECTATION_MOD_NAME = (LocString) "Job Tier Request";
        public static LocString ADJECTIVE_FORMAT_POSITIVE = (LocString) "{0} [{1}]";
        public static LocString ADJECTIVE_FORMAT_NEGATIVE = (LocString) "{0} [{1}]";
        public static LocString FOODQUALITY = (LocString) "\nFood Quality Score of {0}";
        public static LocString FOODQUALITY_EXPECTATION = (LocString) ("\nThis Duplicant is content to eat " + UI.PRE_KEYWORD + "Food" + UI.PST_KEYWORD + " with a " + UI.PRE_KEYWORD + "Food Quality" + UI.PST_KEYWORD + " of <b>{0}</b> or higher");
        public static int ADJECTIVE_INDEX_OFFSET = -1;

        public class ADJECTIVES
        {
          public static LocString MINUS_1 = (LocString) "Grisly";
          public static LocString ZERO = (LocString) "Terrible";
          public static LocString PLUS_1 = (LocString) "Poor";
          public static LocString PLUS_2 = (LocString) "Standard";
          public static LocString PLUS_3 = (LocString) "Good";
          public static LocString PLUS_4 = (LocString) "Great";
          public static LocString PLUS_5 = (LocString) "Superb";
          public static LocString PLUS_6 = (LocString) "Ambrosial";
        }
      }

      public class QUALITYOFLIFE
      {
        public static LocString NAME = (LocString) "Morale Requirements";
        public static LocString EXPECTATION_TOOLTIP = (LocString) ("The more responsibilities and stressors a Duplicant has, the more they will desire additional leisure time and improved amenities.\n\nFailing to keep a Duplicant's " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD + " at or above their " + UI.PRE_KEYWORD + "Morale Need" + UI.PST_KEYWORD + " means they will not be able to unwind, causing them " + UI.PRE_KEYWORD + "Stress" + UI.PST_KEYWORD + " over time.");
        public static LocString EXPECTATION_MOD_NAME = (LocString) "Skills Learned";
        public static LocString APTITUDE_SKILLS_MOD_NAME = (LocString) "Interested Skills Learned";
        public static LocString TOTAL_SKILL_POINTS = (LocString) "Total Skill Points: {0}";
        public static LocString GOOD_MODIFIER = (LocString) "High Morale";
        public static LocString NEUTRAL_MODIFIER = (LocString) "Sufficient Morale";
        public static LocString BAD_MODIFIER = (LocString) "Low Morale";
      }

      public class NOISE
      {
        public static LocString NAME = (LocString) "Noise Expectation";
      }
    }

    public class ATTRIBUTES
    {
      public static LocString VALUE = (LocString) "{0}: {1}";
      public static LocString TOTAL_VALUE = (LocString) "\n\nTotal <b>{1}</b>: {0}";
      public static LocString BASE_VALUE = (LocString) "\nBase: {0}";
      public static LocString MODIFIER_ENTRY = (LocString) "\n    • {0}: {1}";
      public static LocString UNPROFESSIONAL_NAME = (LocString) "Lump";
      public static LocString UNPROFESSIONAL_DESC = (LocString) "This Duplicant has no discernible skills";
      public static LocString PROFESSION_DESC = (LocString) ("Expertise is determined by a Duplicant's highest " + UI.PRE_KEYWORD + "Attribute" + UI.PST_KEYWORD + "\n\nDuplicants develop higher expectations as their Expertise level increases");
      public static LocString STORED_VALUE = (LocString) "Stored value";

      public class CONSTRUCTION
      {
        public static LocString NAME = (LocString) "Construction";
        public static LocString DESC = (LocString) "Determines a Duplicant's building Speed.";
        public static LocString SPEEDMODIFIER = (LocString) "{0} Construction Speed";
      }

      public class SCALDINGTHRESHOLD
      {
        public static LocString NAME = (LocString) "Scalding Threshold";
        public static LocString DESC = (LocString) ("Determines the " + UI.PRE_KEYWORD + "Temperature" + UI.PST_KEYWORD + " at which a Duplicant will get burned.");
      }

      public class DIGGING
      {
        public static LocString NAME = (LocString) "Excavation";
        public static LocString DESC = (LocString) "Determines a Duplicant's mining speed.";
        public static LocString SPEEDMODIFIER = (LocString) "{0} Digging Speed";
        public static LocString ATTACK_MODIFIER = (LocString) "{0} Attack Damage";
      }

      public class MACHINERY
      {
        public static LocString NAME = (LocString) "Machinery";
        public static LocString DESC = (LocString) "Determines how quickly a Duplicant uses machines.";
        public static LocString SPEEDMODIFIER = (LocString) "{0} Machine Operation Speed";
        public static LocString TINKER_EFFECT_MODIFIER = (LocString) "{0} Engie's Tune-Up Effect Duration";
      }

      public class LIFESUPPORT
      {
        public static LocString NAME = (LocString) "Life Support";
        public static LocString DESC = (LocString) ("Determines how efficiently a Duplicant maintains " + (string) BUILDINGS.PREFABS.ALGAEHABITAT.NAME + "s, " + (string) BUILDINGS.PREFABS.AIRFILTER.NAME + "s, and " + (string) BUILDINGS.PREFABS.WATERPURIFIER.NAME + "s");
      }

      public class TOGGLE
      {
        public static LocString NAME = (LocString) "Toggle";
        public static LocString DESC = (LocString) "Determines how efficiently a Duplicant tunes machinery, flips switches, and sets sensors.";
      }

      public class ATHLETICS
      {
        public static LocString NAME = (LocString) "Athletics";
        public static LocString DESC = (LocString) "Determines a Duplicant's default runspeed.";
        public static LocString SPEEDMODIFIER = (LocString) "{0} Runspeed";
      }

      public class DOCTOREDLEVEL
      {
        public static LocString NAME = (LocString) (UI.FormatAsLink("Treatment Received", "MEDICINE") + " Effect");
        public static LocString DESC = (LocString) ("Duplicants who receive medical care while in a " + (string) BUILDINGS.PREFABS.DOCTORSTATION.NAME + " or " + (string) BUILDINGS.PREFABS.ADVANCEDDOCTORSTATION.NAME + " will gain the " + UI.PRE_KEYWORD + "Treatment Received" + UI.PST_KEYWORD + " effect\n\nThis effect reduces the severity of " + UI.PRE_KEYWORD + "Disease" + UI.PST_KEYWORD + " symptoms");
      }

      public class SNEEZYNESS
      {
        public static LocString NAME = (LocString) "Sneeziness";
        public static LocString DESC = (LocString) "Determines how frequently a Duplicant sneezes.";
      }

      public class GERMRESISTANCE
      {
        public static LocString NAME = (LocString) "Germ Resistance";
        public static LocString DESC = (LocString) ("Duplicants with a higher " + UI.PRE_KEYWORD + "Germ Resistance" + UI.PST_KEYWORD + " rating are less likely to contract germ-based " + UI.PRE_KEYWORD + "Diseases" + UI.PST_KEYWORD + ".");

        public class MODIFIER_DESCRIPTORS
        {
          public static LocString NEGATIVE_LARGE = (LocString) "{0} (Large Loss)";
          public static LocString NEGATIVE_MEDIUM = (LocString) "{0} (Medium Loss)";
          public static LocString NEGATIVE_SMALL = (LocString) "{0} (Small Loss)";
          public static LocString NONE = (LocString) "No Effect";
          public static LocString POSITIVE_SMALL = (LocString) "{0} (Small Boost)";
          public static LocString POSITIVE_MEDIUM = (LocString) "{0} (Medium Boost)";
          public static LocString POSITIVE_LARGE = (LocString) "{0} (Large Boost)";
        }
      }

      public class LEARNING
      {
        public static LocString NAME = (LocString) "Science";
        public static LocString DESC = (LocString) ("Determines how quickly a Duplicant conducts " + UI.PRE_KEYWORD + "Research" + UI.PST_KEYWORD + " and gains " + UI.PRE_KEYWORD + "Skill Points" + UI.PST_KEYWORD + ".");
        public static LocString SPEEDMODIFIER = (LocString) "{0} Skill Leveling";
        public static LocString RESEARCHSPEED = (LocString) "{0} Research Speed";
        public static LocString GEOTUNER_SPEED_MODIFIER = (LocString) "{0} Geotuning Speed";
      }

      public class COOKING
      {
        public static LocString NAME = (LocString) "Cuisine";
        public static LocString DESC = (LocString) ("Determines how quickly a Duplicant prepares " + UI.PRE_KEYWORD + "Food" + UI.PST_KEYWORD + ".");
        public static LocString SPEEDMODIFIER = (LocString) "{0} Cooking Speed";
      }

      public class HAPPINESSDELTA
      {
        public static LocString NAME = (LocString) "Happiness";
        public static LocString DESC = (LocString) ("Contented " + UI.FormatAsLink("Critters", "CREATURES") + " produce usable materials with increased frequency.");
      }

      public class RADIATIONBALANCEDELTA
      {
        public static LocString NAME = (LocString) "Absorbed Radiation Dose";
        public static LocString TOOLTIP = (LocString) ("Duplicants accumulate Rads in areas with " + UI.PRE_KEYWORD + "Radiation" + UI.PST_KEYWORD + " and recover at very slow rates\n\nOpen the " + UI.FormatAsOverlay("Radiation Overlay", (Action) 133) + " to view current " + UI.PRE_KEYWORD + "Rad" + UI.PST_KEYWORD + " readings");
      }

      public class INSULATION
      {
        public static LocString NAME = (LocString) "Insulation";
        public static LocString DESC = (LocString) ("Highly " + UI.PRE_KEYWORD + "Insulated" + UI.PST_KEYWORD + " Duplicants retain body heat easily, while low " + UI.PRE_KEYWORD + "Insulation" + UI.PST_KEYWORD + " Duplicants are easier to keep cool.");
        public static LocString SPEEDMODIFIER = (LocString) "{0} Temperature Retention";
      }

      public class STRENGTH
      {
        public static LocString NAME = (LocString) "Strength";
        public static LocString DESC = (LocString) ("Determines a Duplicant's " + UI.PRE_KEYWORD + "Carrying Capacity" + UI.PST_KEYWORD + " and cleaning speed.");
        public static LocString CARRYMODIFIER = (LocString) ("{0} " + (string) DUPLICANTS.ATTRIBUTES.CARRYAMOUNT.NAME);
        public static LocString SPEEDMODIFIER = (LocString) "{0} Tidying Speed";
      }

      public class CARING
      {
        public static LocString NAME = (LocString) "Medicine";
        public static LocString DESC = (LocString) "Determines a Duplicant's ability to care for sick peers.";
        public static LocString SPEEDMODIFIER = (LocString) "{0} Treatment Speed";
        public static LocString FABRICATE_SPEEDMODIFIER = (LocString) "{0} Medicine Fabrication Speed";
      }

      public class IMMUNITY
      {
        public static LocString NAME = (LocString) "Immunity";
        public static LocString DESC = (LocString) ("Determines a Duplicant's " + UI.PRE_KEYWORD + "Disease" + UI.PST_KEYWORD + " susceptibility and recovery time.");
        public static LocString BOOST_MODIFIER = (LocString) "{0} Immunity Regen";
        public static LocString BOOST_STAT = (LocString) "Immunity Attribute";
      }

      public class BOTANIST
      {
        public static LocString NAME = (LocString) "Agriculture";
        public static LocString DESC = (LocString) ("Determines how quickly and efficiently a Duplicant cultivates " + UI.PRE_KEYWORD + "Plants" + UI.PST_KEYWORD + ".");
        public static LocString HARVEST_SPEED_MODIFIER = (LocString) "{0} Harvesting Speed";
        public static LocString TINKER_MODIFIER = (LocString) "{0} Tending Speed";
        public static LocString BONUS_SEEDS = (LocString) "{0} Seed Chance";
        public static LocString TINKER_EFFECT_MODIFIER = (LocString) "{0} Farmer's Touch Effect Duration";
      }

      public class RANCHING
      {
        public static LocString NAME = (LocString) "Husbandry";
        public static LocString DESC = (LocString) ("Determines how efficiently a Duplicant tends " + UI.FormatAsLink("Critters", "CREATURES") + ".");
        public static LocString EFFECTMODIFIER = (LocString) "{0} Groom Effect Duration";
        public static LocString CAPTURABLESPEED = (LocString) "{0} Wrangling Speed";
      }

      public class ART
      {
        public static LocString NAME = (LocString) "Creativity";
        public static LocString DESC = (LocString) ("Determines how quickly a Duplicant produces " + UI.PRE_KEYWORD + "Artwork" + UI.PST_KEYWORD + ".");
        public static LocString SPEEDMODIFIER = (LocString) "{0} Decorating Speed";
      }

      public class DECOR
      {
        public static LocString NAME = (LocString) "Decor";
        public static LocString DESC = (LocString) ("Affects a Duplicant's " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD + " and their opinion of their surroundings.");
      }

      public class THERMALCONDUCTIVITYBARRIER
      {
        public static LocString NAME = (LocString) "Insulation Thickness";
        public static LocString DESC = (LocString) ("Determines how quickly a Duplicant retains or loses body " + UI.PRE_KEYWORD + "Heat" + UI.PST_KEYWORD + " in any given area.\n\nIt is the sum of a Duplicant's " + UI.PRE_KEYWORD + "Equipment" + UI.PST_KEYWORD + " and their natural " + UI.PRE_KEYWORD + "Insulation" + UI.PST_KEYWORD + " values.");
      }

      public class DECORRADIUS
      {
        public static LocString NAME = (LocString) "Decor Radius";
        public static LocString DESC = (LocString) ("The influence range of an object's " + UI.PRE_KEYWORD + "Decor" + UI.PST_KEYWORD + " value.");
      }

      public class DECOREXPECTATION
      {
        public static LocString NAME = (LocString) "Decor Morale Bonus";
        public static LocString DESC = (LocString) ("A Decor Morale Bonus allows Duplicants to receive " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD + " boosts from lower " + UI.PRE_KEYWORD + "Decor" + UI.PST_KEYWORD + " values.\n\nMaintaining high " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD + " will allow Duplicants to learn more " + UI.PRE_KEYWORD + "Skills" + UI.PST_KEYWORD + ".");
      }

      public class FOODEXPECTATION
      {
        public static LocString NAME = (LocString) "Food Morale Bonus";
        public static LocString DESC = (LocString) ("A Food Morale Bonus allows Duplicants to receive " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD + " boosts from lower quality " + UI.PRE_KEYWORD + "Food" + UI.PST_KEYWORD + ".\n\nMaintaining high " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD + " will allow Duplicants to learn more " + UI.PRE_KEYWORD + "Skills" + UI.PST_KEYWORD + ".");
      }

      public class QUALITYOFLIFEEXPECTATION
      {
        public static LocString NAME = (LocString) "Morale Need";
        public static LocString DESC = (LocString) ("Dictates how high a Duplicant's " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD + " must be kept to prevent them from gaining " + UI.PRE_KEYWORD + "Stress" + UI.PST_KEYWORD);
      }

      public class HYGIENE
      {
        public static LocString NAME = (LocString) "Hygiene";
        public static LocString DESC = (LocString) "Affects a Duplicant's sense of cleanliness.";
      }

      public class CARRYAMOUNT
      {
        public static LocString NAME = (LocString) "Carrying Capacity";
        public static LocString DESC = (LocString) "Determines the maximum weight that a Duplicant can carry.";
      }

      public class SPACENAVIGATION
      {
        public static LocString NAME = (LocString) "Piloting";
        public static LocString DESC = (LocString) "Determines how long it takes a Duplicant to complete a space mission.";
        public static LocString DLC1_DESC = (LocString) "Determines how much of a speed bonus a Duplicant provides to a rocket they are piloting.";
        public static LocString SPEED_MODIFIER = (LocString) "{0} Rocket Speed";
      }

      public class QUALITYOFLIFE
      {
        public static LocString NAME = (LocString) "Morale";
        public static LocString DESC = (LocString) ("A Duplicant's " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD + " must exceed their " + UI.PRE_KEYWORD + "Morale Need" + UI.PST_KEYWORD + ", or they'll begin to accumulate " + UI.PRE_KEYWORD + "Stress" + UI.PST_KEYWORD + ".\n\n" + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD + " can be increased by providing Duplicants higher quality " + UI.PRE_KEYWORD + "Food" + UI.PST_KEYWORD + ", allotting more " + UI.PRE_KEYWORD + "Downtime" + UI.PST_KEYWORD + " in\nthe colony schedule, or building better " + UI.PRE_KEYWORD + "Bathrooms" + UI.PST_KEYWORD + " and " + UI.PRE_KEYWORD + "Bedrooms" + UI.PST_KEYWORD + " for them to live in.");
        public static LocString DESC_FORMAT = (LocString) "{0} / {1}";
        public static LocString TOOLTIP_EXPECTATION = (LocString) "Total <b>Morale Need</b>: {0}\n    • Skills Learned: +{0}";
        public static LocString TOOLTIP_EXPECTATION_OVER = (LocString) ("This Duplicant has sufficiently high " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD);
        public static LocString TOOLTIP_EXPECTATION_UNDER = (LocString) ("This Duplicant's low " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD + " will cause " + UI.PRE_KEYWORD + "Stress" + UI.PST_KEYWORD + " over time");
      }

      public class AIRCONSUMPTIONRATE
      {
        public static LocString NAME = (LocString) "Air Consumption Rate";
        public static LocString DESC = (LocString) ("Air Consumption determines how much " + (string) ELEMENTS.OXYGEN.NAME + " a Duplicant requires per minute to live.");
      }

      public class RADIATIONRESISTANCE
      {
        public static LocString NAME = (LocString) "Radiation Resistance";
        public static LocString DESC = (LocString) ("Determines how easily a Duplicant repels " + UI.PRE_KEYWORD + "Radiation Sickness" + UI.PST_KEYWORD + ".");
      }

      public class RADIATIONRECOVERY
      {
        public static LocString NAME = (LocString) "Radiation Absorption";
        public static LocString DESC = (LocString) ("The rate at which " + UI.PRE_KEYWORD + "Radiation" + UI.PST_KEYWORD + " is neutralized within a Duplicant body.");
      }

      public class STRESSDELTA
      {
        public static LocString NAME = (LocString) "Stress";
        public static LocString DESC = (LocString) ("Determines how quickly a Duplicant gains or reduces " + UI.PRE_KEYWORD + "Stress" + UI.PST_KEYWORD);
      }

      public class BREATHDELTA
      {
        public static LocString NAME = (LocString) "Breath";
        public static LocString DESC = (LocString) ("Determines how quickly a Duplicant gains or reduces " + UI.PRE_KEYWORD + "Breath" + UI.PST_KEYWORD + ".");
      }

      public class BLADDERDELTA
      {
        public static LocString NAME = (LocString) "Bladder";
        public static LocString DESC = (LocString) ("Determines how quickly a Duplicant's " + UI.PRE_KEYWORD + "Bladder" + UI.PST_KEYWORD + " fills or depletes.");
      }

      public class CALORIESDELTA
      {
        public static LocString NAME = (LocString) "Calories";
        public static LocString DESC = (LocString) ("Determines how quickly a Duplicant burns or stores " + UI.PRE_KEYWORD + "Calories" + UI.PST_KEYWORD + ".");
      }

      public class STAMINADELTA
      {
        public static LocString NAME = (LocString) "Stamina";
        public static LocString DESC = (LocString) "";
      }

      public class TOXICITYDELTA
      {
        public static LocString NAME = (LocString) "Toxicity";
        public static LocString DESC = (LocString) "";
      }

      public class IMMUNELEVELDELTA
      {
        public static LocString NAME = (LocString) "Immunity";
        public static LocString DESC = (LocString) "";
      }

      public class TOILETEFFICIENCY
      {
        public static LocString NAME = (LocString) "Bathroom Use Speed";
        public static LocString DESC = (LocString) "Determines how long a Duplicant needs to do their \"business\".";
        public static LocString SPEEDMODIFIER = (LocString) "{0} Bathroom Use Speed";
      }

      public class METABOLISM
      {
        public static LocString NAME = (LocString) "Critter Metabolism";
        public static LocString DESC = (LocString) ("Affects the rate at which a critter burns " + UI.PRE_KEYWORD + "Calories" + UI.PST_KEYWORD + ".");
      }

      public class ROOMTEMPERATUREPREFERENCE
      {
        public static LocString NAME = (LocString) "Temperature Preference";
        public static LocString DESC = (LocString) ("Determines the minimum body " + UI.PRE_KEYWORD + "Temperature" + UI.PST_KEYWORD + " a Duplicant prefers to maintain.");
      }

      public class MAXUNDERWATERTRAVELCOST
      {
        public static LocString NAME = (LocString) "Underwater Movement";
        public static LocString DESC = (LocString) ("Determines a Duplicant's runspeed when submerged in " + UI.PRE_KEYWORD + "Liquid" + UI.PST_KEYWORD);
      }

      public class OVERHEATTEMPERATURE
      {
        public static LocString NAME = (LocString) "Overheat Temperature";
        public static LocString DESC = (LocString) ("A building at Overheat " + UI.PRE_KEYWORD + "Temperature" + UI.PST_KEYWORD + " will take damage and break down if not cooled");
      }

      public class FATALTEMPERATURE
      {
        public static LocString NAME = (LocString) "Break Down Temperature";
        public static LocString DESC = (LocString) ("A building at break down " + UI.PRE_KEYWORD + "Temperature" + UI.PST_KEYWORD + " will lose functionality and take damage");
      }

      public class HITPOINTSDELTA
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Health", "HEALTH");
        public static LocString DESC = (LocString) "Health regeneration is increased when another Duplicant provides medical care to the patient";
      }

      public class DISEASECURESPEED
      {
        public static LocString NAME = (LocString) (UI.FormatAsLink("Disease", "DISEASE") + " Recovery Speed Bonus");
        public static LocString DESC = (LocString) "Recovery speed bonus is increased when another Duplicant provides medical care to the patient";
      }

      public abstract class MACHINERYSPEED
      {
        public static LocString NAME = (LocString) "Machinery Speed";
        public static LocString DESC = (LocString) "Speed Bonus";
      }

      public abstract class GENERATOROUTPUT
      {
        public static LocString NAME = (LocString) "Power Output";
      }

      public abstract class ROCKETBURDEN
      {
        public static LocString NAME = (LocString) "Burden";
      }

      public abstract class ROCKETENGINEPOWER
      {
        public static LocString NAME = (LocString) "Engine Power";
      }

      public abstract class FUELRANGEPERKILOGRAM
      {
        public static LocString NAME = (LocString) "Range";
      }

      public abstract class HEIGHT
      {
        public static LocString NAME = (LocString) "Height";
      }

      public class WILTTEMPRANGEMOD
      {
        public static LocString NAME = (LocString) "Viable Temperature Range";
        public static LocString DESC = (LocString) "Variance growth temperature relative to the base crop";
      }

      public class YIELDAMOUNT
      {
        public static LocString NAME = (LocString) "Yield Amount";
        public static LocString DESC = (LocString) "Plant production relative to the base crop";
      }

      public class HARVESTTIME
      {
        public static LocString NAME = (LocString) "Harvest Duration";
        public static LocString DESC = (LocString) "Time it takes an unskilled Duplicant to harvest this plant";
      }

      public class DECORBONUS
      {
        public static LocString NAME = (LocString) "Decor Bonus";
        public static LocString DESC = (LocString) "Change in Decor value relative to the base crop";
      }

      public class MINLIGHTLUX
      {
        public static LocString NAME = (LocString) "Light";
        public static LocString DESC = (LocString) "Minimum lux this plant requires for growth";
      }

      public class FERTILIZERUSAGEMOD
      {
        public static LocString NAME = (LocString) "Fertilizer Usage";
        public static LocString DESC = (LocString) "Fertilizer and irrigation amounts this plant requires relative to the base crop";
      }

      public class MINRADIATIONTHRESHOLD
      {
        public static LocString NAME = (LocString) "Minimum Radiation";
        public static LocString DESC = (LocString) "Smallest amount of ambient Radiation required for this plant to grow";
      }

      public class MAXRADIATIONTHRESHOLD
      {
        public static LocString NAME = (LocString) "Maximum Radiation";
        public static LocString DESC = (LocString) "Largest amount of ambient Radiation this plant can tolerate";
      }
    }

    public class ROLES
    {
      public class GROUPS
      {
        public static LocString APTITUDE_DESCRIPTION = (LocString) ("This Duplicant will gain <b>{1}</b> " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD + " when learning " + UI.PRE_KEYWORD + "{0}" + UI.PST_KEYWORD + " Skills");
        public static LocString APTITUDE_DESCRIPTION_CHOREGROUP = (LocString) ("{2}\n\nThis Duplicant will gain <b>+{1}</b> " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD + " when learning " + UI.PRE_KEYWORD + "{0}" + UI.PST_KEYWORD + " Skills");
        public static LocString SUITS = (LocString) "Suit Wearing";
      }

      public class NO_ROLE
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Unemployed", nameof (NO_ROLE));
        public static LocString DESCRIPTION = (LocString) "No job assignment";
      }

      public class JUNIOR_ARTIST
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Art Fundamentals", "ARTING1");
        public static LocString DESCRIPTION = (LocString) "Teaches the most basic level of art skill";
      }

      public class ARTIST
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Aesthetic Design", "ARTING2");
        public static LocString DESCRIPTION = (LocString) "Allows moderately attractive art to be created";
      }

      public class MASTER_ARTIST
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Masterworks", "ARTING3");
        public static LocString DESCRIPTION = (LocString) "Enables the painting and sculpting of masterpieces";
      }

      public class JUNIOR_BUILDER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Improved Construction I", "BUILDING1");
        public static LocString DESCRIPTION = (LocString) "Marginally improves a Duplicant's construction speeds";
      }

      public class BUILDER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Improved Construction II", "BUILDING2");
        public static LocString DESCRIPTION = (LocString) "Further increases a Duplicant's construction speeds";
      }

      public class SENIOR_BUILDER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Demolition", "BUILDING3");
        public static LocString DESCRIPTION = (LocString) "Enables a Duplicant to deconstruct Gravitas buildings";
      }

      public class JUNIOR_RESEARCHER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Advanced Research", "RESEARCHING1");
        public static LocString DESCRIPTION = (LocString) ("Allows Duplicants to perform research using a " + (string) BUILDINGS.PREFABS.ADVANCEDRESEARCHCENTER.NAME);
      }

      public class RESEARCHER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Field Research", "RESEARCHING2");
        public static LocString DESCRIPTION = (LocString) ("Duplicants can perform studies on " + UI.PRE_KEYWORD + "Geysers" + UI.PST_KEYWORD + ", " + (string) UI.CLUSTERMAP.PLANETOID_KEYWORD + ", and other geographical phenomena");
      }

      public class SENIOR_RESEARCHER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Astronomy", "ASTRONOMY");
        public static LocString DESCRIPTION = (LocString) ("Enables Duplicants to study outer space using the " + (string) BUILDINGS.PREFABS.CLUSTERTELESCOPE.NAME);
      }

      public class NUCLEAR_RESEARCHER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Applied Sciences Research", "ATOMICRESEARCH");
        public static LocString DESCRIPTION = (LocString) ("Enables Duplicants to study matter using the " + (string) BUILDINGS.PREFABS.NUCLEARRESEARCHCENTER.NAME);
      }

      public class SPACE_RESEARCHER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Data Analysis Researcher", "SPACERESEARCH");
        public static LocString DESCRIPTION = (LocString) ("Enables Duplicants to conduct research using the " + (string) BUILDINGS.PREFABS.DLC1COSMICRESEARCHCENTER.NAME);
      }

      public class JUNIOR_COOK
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Grilling", "COOKING1");
        public static LocString DESCRIPTION = (LocString) ("Allows Duplicants to cook using the " + (string) BUILDINGS.PREFABS.COOKINGSTATION.NAME);
      }

      public class COOK
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Grilling II", "COOKING2");
        public static LocString DESCRIPTION = (LocString) "Improves a Duplicant's cooking speed";
      }

      public class JUNIOR_MEDIC
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Medicine Compounding", "MEDICINE1");
        public static LocString DESCRIPTION = (LocString) ("Allows Duplicants to produce medicines at the " + (string) BUILDINGS.PREFABS.APOTHECARY.NAME);
      }

      public class MEDIC
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Bedside Manner", "MEDICINE2");
        public static LocString DESCRIPTION = (LocString) ("Trains Duplicants to administer medicine at the " + (string) BUILDINGS.PREFABS.DOCTORSTATION.NAME);
      }

      public class SENIOR_MEDIC
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Advanced Medical Care", "MEDICINE3");
        public static LocString DESCRIPTION = (LocString) ("Trains Duplicants to operate the " + (string) BUILDINGS.PREFABS.ADVANCEDDOCTORSTATION.NAME);
      }

      public class MACHINE_TECHNICIAN
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Improved Tinkering", "TECHNICALS1");
        public static LocString DESCRIPTION = (LocString) "Marginally improves a Duplicant's tinkering speeds";
      }

      public class OIL_TECHNICIAN
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Oil Engineering", nameof (OIL_TECHNICIAN));
        public static LocString DESCRIPTION = (LocString) ("Allows the extraction and refinement of " + (string) ELEMENTS.CRUDEOIL.NAME);
      }

      public class HAULER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Improved Carrying I", "HAULING1");
        public static LocString DESCRIPTION = (LocString) "Minorly increase a Duplicant's strength and carrying capacity";
      }

      public class MATERIALS_MANAGER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Improved Carrying II", "HAULING2");
        public static LocString DESCRIPTION = (LocString) "Further increases a Duplicant's strength and carrying capacity for even swifter deliveries";
      }

      public class JUNIOR_FARMER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Improved Farming I", "FARMING1");
        public static LocString DESCRIPTION = (LocString) ("Minorly increase a Duplicant's farming skills, increasing their chances of harvesting new plant " + UI.PRE_KEYWORD + "Seeds" + UI.PST_KEYWORD);
      }

      public class FARMER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Crop Tending", "FARMING2");
        public static LocString DESCRIPTION = (LocString) ("Enables tending " + UI.PRE_KEYWORD + "Plants" + UI.PST_KEYWORD + ", which will increase their growth speed");
      }

      public class SENIOR_FARMER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Improved Farming II", "FARMING3");
        public static LocString DESCRIPTION = (LocString) "Further increases a Duplicant's farming skills";
      }

      public class JUNIOR_MINER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Hard Digging", "MINING1");
        public static LocString DESCRIPTION = (LocString) ("Allows the excavation of " + UI.PRE_KEYWORD + (string) ELEMENTS.HARDNESS.HARDNESS_DESCRIPTOR.VERYFIRM + UI.PST_KEYWORD + " materials such as " + (string) ELEMENTS.GRANITE.NAME);
      }

      public class MINER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Superhard Digging", "MINING2");
        public static LocString DESCRIPTION = (LocString) ("Allows the excavation of the element " + (string) ELEMENTS.KATAIRITE.NAME);
      }

      public class SENIOR_MINER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Super-Duperhard Digging", "MINING3");
        public static LocString DESCRIPTION = (LocString) ("Allows the excavation of " + UI.PRE_KEYWORD + (string) ELEMENTS.HARDNESS.HARDNESS_DESCRIPTOR.NEARLYIMPENETRABLE + UI.PST_KEYWORD + " elements, including " + (string) ELEMENTS.DIAMOND.NAME + " and " + (string) ELEMENTS.OBSIDIAN.NAME);
      }

      public class MASTER_MINER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Hazmat Digging", "MINING4");
        public static LocString DESCRIPTION = (LocString) ("Allows the excavation of dangerous materials like " + (string) ELEMENTS.CORIUM.NAME);
      }

      public class SUIT_DURABILITY
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Suit Sustainability Training", "SUITDURABILITY");
        public static LocString DESCRIPTION = (LocString) ("Suits equipped by this Duplicant lose durability " + GameUtil.GetFormattedPercent(TUNING.EQUIPMENT.SUITS.SUIT_DURABILITY_SKILL_BONUS * 100f) + " slower.");
      }

      public class SUIT_EXPERT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Exosuit Training", "SUITS1");
        public static LocString DESCRIPTION = (LocString) "Eliminates the runspeed loss experienced while wearing exosuits";
      }

      public class POWER_TECHNICIAN
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Electrical Engineering", "TECHNICALS2");
        public static LocString DESCRIPTION = (LocString) ("Enables generator " + UI.PRE_KEYWORD + "Tune-Up" + UI.PST_KEYWORD + ", which will temporarily provide improved " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD + " output");
      }

      public class MECHATRONIC_ENGINEER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Mechatronics Engineering", "ENGINEERING1");
        public static LocString DESCRIPTION = (LocString) ("Allows the construction and maintenance of " + (string) BUILDINGS.PREFABS.SOLIDCONDUIT.NAME + " systems");
      }

      public class HANDYMAN
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Improved Strength", "BASEKEEPING1");
        public static LocString DESCRIPTION = (LocString) "Minorly improves a Duplicant's physical strength";
      }

      public class PLUMBER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Plumbing", "BASEKEEPING2");
        public static LocString DESCRIPTION = (LocString) ("Allows a Duplicant to empty " + UI.PRE_KEYWORD + "Pipes" + UI.PST_KEYWORD + " without making a mess");
      }

      public class RANCHER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Critter Ranching I", "RANCHING1");
        public static LocString DESCRIPTION = (LocString) ("Allows a Duplicant to handle and care for " + UI.FormatAsLink("Critters", "CREATURES"));
      }

      public class SENIOR_RANCHER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Critter Ranching II", "RANCHING2");
        public static LocString DESCRIPTION = (LocString) ("Improves a Duplicant's " + UI.PRE_KEYWORD + "Ranching" + UI.PST_KEYWORD + " skills");
      }

      public class ASTRONAUTTRAINEE
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Rocket Piloting", "ASTRONAUTING1");
        public static LocString DESCRIPTION = (LocString) ("Allows a Duplicant to operate a " + (string) BUILDINGS.PREFABS.COMMANDMODULE.NAME + " to pilot a rocket ship");
      }

      public class ASTRONAUT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Rocket Navigation", "ASTRONAUTING2");
        public static LocString DESCRIPTION = (LocString) "Improves the speed that space missions are completed";
      }

      public class ROCKETPILOT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Rocket Piloting", "ROCKETPILOTING1");
        public static LocString DESCRIPTION = (LocString) ("Allows a Duplicant to operate a " + (string) BUILDINGS.PREFABS.ROCKETCONTROLSTATION.NAME + " and pilot rockets");
      }

      public class SENIOR_ROCKETPILOT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Rocket Piloting II", "ROCKETPILOTING2");
        public static LocString DESCRIPTION = (LocString) "Allows Duplicants to pilot rockets at faster speeds";
      }

      public class USELESSSKILL
      {
        public static LocString NAME = (LocString) "W.I.P. Skill";
        public static LocString DESCRIPTION = (LocString) "This skill doesn't really do anything right now.";
      }
    }

    public class THOUGHTS
    {
      public class STARVING
      {
        public static LocString TOOLTIP = (LocString) "Starving";
      }

      public class HOT
      {
        public static LocString TOOLTIP = (LocString) "Hot";
      }

      public class COLD
      {
        public static LocString TOOLTIP = (LocString) "Cold";
      }

      public class BREAKBLADDER
      {
        public static LocString TOOLTIP = (LocString) "Washroom Break";
      }

      public class FULLBLADDER
      {
        public static LocString TOOLTIP = (LocString) "Full Bladder";
      }

      public class HAPPY
      {
        public static LocString TOOLTIP = (LocString) "Happy";
      }

      public class UNHAPPY
      {
        public static LocString TOOLTIP = (LocString) "Unhappy";
      }

      public class POORDECOR
      {
        public static LocString TOOLTIP = (LocString) "Poor Decor";
      }

      public class POOR_FOOD_QUALITY
      {
        public static LocString TOOLTIP = (LocString) "Lousy Meal";
      }

      public class GOOD_FOOD_QUALITY
      {
        public static LocString TOOLTIP = (LocString) "Delicious Meal";
      }

      public class SLEEPY
      {
        public static LocString TOOLTIP = (LocString) "Sleepy";
      }

      public class DREAMY
      {
        public static LocString TOOLTIP = (LocString) "Dreaming";
      }

      public class SUFFOCATING
      {
        public static LocString TOOLTIP = (LocString) "Suffocating";
      }

      public class ANGRY
      {
        public static LocString TOOLTIP = (LocString) "Angry";
      }

      public class RAGING
      {
        public static LocString TOOLTIP = (LocString) "Raging";
      }

      public class GOTINFECTED
      {
        public static LocString TOOLTIP = (LocString) "Got Infected";
      }

      public class PUTRIDODOUR
      {
        public static LocString TOOLTIP = (LocString) "Smelled Something Putrid";
      }

      public class NOISY
      {
        public static LocString TOOLTIP = (LocString) "Loud Area";
      }

      public class NEWROLE
      {
        public static LocString TOOLTIP = (LocString) "New Skill";
      }

      public class CHATTY
      {
        public static LocString TOOLTIP = (LocString) "Greeting";
      }

      public class ENCOURAGE
      {
        public static LocString TOOLTIP = (LocString) "Encouraging";
      }

      public class CONVERSATION
      {
        public static LocString TOOLTIP = (LocString) "Chatting";
      }

      public class CATCHYTUNE
      {
        public static LocString TOOLTIP = (LocString) "WHISTLING";
      }
    }
  }
}
