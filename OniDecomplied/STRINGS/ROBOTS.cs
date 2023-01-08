// Decompiled with JetBrains decompiler
// Type: STRINGS.ROBOTS
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace STRINGS
{
  public class ROBOTS
  {
    public static LocString CATEGORY_NAME = (LocString) "Robots";

    public class STATS
    {
      public class INTERNALBATTERY
      {
        public static LocString NAME = (LocString) "Rechargeable Battery";
        public static LocString TOOLTIP = (LocString) "When this bot's battery runs out it must temporarily stop working to go recharge";
      }

      public class INTERNALCHEMICALBATTERY
      {
        public static LocString NAME = (LocString) "Chemical Battery";
        public static LocString TOOLTIP = (LocString) "This bot will shut down permanently when its battery runs out";
      }
    }

    public class ATTRIBUTES
    {
      public class INTERNALBATTERYDELTA
      {
        public static LocString NAME = (LocString) "Rechargeable Battery Drain";
        public static LocString TOOLTIP = (LocString) "The rate at which battery life is depleted";
      }
    }

    public class STATUSITEMS
    {
      public class CANTREACHSTATION
      {
        public static LocString NAME = (LocString) "Unreachable Dock";
        public static LocString DESC = (LocString) "Obstacles are preventing {0} from heading home";
        public static LocString TOOLTIP = (LocString) "Obstacles are preventing {0} from heading home";
      }

      public class MOVINGTOCHARGESTATION
      {
        public static LocString NAME = (LocString) "Traveling to Dock";
        public static LocString DESC = (LocString) "{0} is on its way home to recharge";
        public static LocString TOOLTIP = (LocString) "{0} is on its way home to recharge";
      }

      public class LOWBATTERY
      {
        public static LocString NAME = (LocString) "Low Battery";
        public static LocString DESC = (LocString) "{0}'s battery is low and needs to recharge";
        public static LocString TOOLTIP = (LocString) "{0}'s battery is low and needs to recharge";
      }

      public class LOWBATTERYNOCHARGE
      {
        public static LocString NAME = (LocString) "Low Battery";
        public static LocString DESC = (LocString) "{0}'s battery is low\n\nThe internal battery cannot be recharged and this robot will cease functioning after it is depleted.";
        public static LocString TOOLTIP = (LocString) "{0}'s battery is low\n\nThe internal battery cannot be recharged and this robot will cease functioning after it is depleted.";
      }

      public class DEADBATTERY
      {
        public static LocString NAME = (LocString) "Shut Down";
        public static LocString DESC = (LocString) "RIP {0}\n\n{0}'s battery has been depleted and cannot be recharged";
        public static LocString TOOLTIP = (LocString) "RIP {0}\n\n{0}'s battery has been depleted and cannot be recharged";
      }

      public class DUSTBINFULL
      {
        public static LocString NAME = (LocString) "Dust Bin Full";
        public static LocString DESC = (LocString) "{0} must return to its dock to unload";
        public static LocString TOOLTIP = (LocString) "{0} must return to its dock to unload";
      }

      public class WORKING
      {
        public static LocString NAME = (LocString) "Working";
        public static LocString DESC = (LocString) "{0} is working diligently. Great job, {0}!";
        public static LocString TOOLTIP = (LocString) "{0} is working diligently. Great job, {0}!";
      }

      public class UNLOADINGSTORAGE
      {
        public static LocString NAME = (LocString) "Unloading";
        public static LocString DESC = (LocString) "{0} is emptying out its dust bin";
        public static LocString TOOLTIP = (LocString) "{0} is emptying out its dust bin";
      }

      public class CHARGING
      {
        public static LocString NAME = (LocString) "Charging";
        public static LocString DESC = (LocString) "{0} is recharging its battery";
        public static LocString TOOLTIP = (LocString) "{0} is recharging its battery";
      }

      public class REACTPOSITIVE
      {
        public static LocString NAME = (LocString) "Happy Reaction";
        public static LocString DESC = (LocString) "This bot saw something nice!";
        public static LocString TOOLTIP = (LocString) "This bot saw something nice!";
      }

      public class REACTNEGATIVE
      {
        public static LocString NAME = (LocString) "Bothered Reaction";
        public static LocString DESC = (LocString) "This bot saw something upsetting";
        public static LocString TOOLTIP = (LocString) "This bot saw something upsetting";
      }
    }

    public class MODELS
    {
      public class SCOUT
      {
        public static LocString NAME = (LocString) "Rover";
        public static LocString DESC = (LocString) ("A curious bot that can remotely explore new " + (string) UI.CLUSTERMAP.PLANETOID_KEYWORD + " locations.");
      }

      public class SWEEPBOT
      {
        public static LocString NAME = (LocString) "Sweepy";
        public static LocString DESC = (LocString) ("An automated sweeping robot.\n\nSweeps up " + UI.FormatAsLink("Solid", "ELEMENTS_SOLID") + " debris and " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " spills and stores the material back in its " + UI.FormatAsLink("Sweepy Dock", "SWEEPBOTSTATION") + ".");
      }
    }
  }
}
