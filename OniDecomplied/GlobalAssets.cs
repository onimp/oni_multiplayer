// Decompiled with JetBrains decompiler
// Type: GlobalAssets
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD;
using FMOD.Studio;
using FMODUnity;
using STRINGS;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GlobalAssets : KMonoBehaviour
{
  private static Dictionary<string, string> SoundTable = new Dictionary<string, string>();
  private static HashSet<string> LowPrioritySounds = new HashSet<string>();
  private static HashSet<string> HighPrioritySounds = new HashSet<string>();
  public ColorSet colorSet;
  public ColorSet[] colorSetOptions;

  public static GlobalAssets Instance { get; private set; }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    GlobalAssets.Instance = this;
    if (GlobalAssets.SoundTable.Count == 0)
    {
      Bank[] bankArray = (Bank[]) null;
      try
      {
        FMOD.Studio.System studioSystem = RuntimeManager.StudioSystem;
        if (((FMOD.Studio.System) ref studioSystem).getBankList(ref bankArray) != null)
          bankArray = (Bank[]) null;
      }
      catch
      {
        bankArray = (Bank[]) null;
      }
      if (bankArray != null)
      {
        foreach (Bank bank in bankArray)
        {
          EventDescription[] eventDescriptionArray;
          RESULT eventList = ((Bank) ref bank).getEventList(ref eventDescriptionArray);
          string path;
          if (eventList != null)
          {
            ((Bank) ref bank).getPath(ref path);
            Debug.LogError((object) string.Format("ERROR [{0}] loading FMOD events for bank [{1}]", (object) eventList, (object) path));
          }
          else
          {
            for (int index = 0; index < eventDescriptionArray.Length; ++index)
            {
              EventDescription eventDescription = eventDescriptionArray[index];
              ((EventDescription) ref eventDescription).getPath(ref path);
              if (path == null)
              {
                ((Bank) ref bank).getPath(ref path);
                GUID guid;
                ((EventDescription) ref eventDescription).getID(ref guid);
                Debug.LogError((object) string.Format("Got a FMOD event with a null path! {0} {1} in bank {2}", (object) eventDescription.ToString(), (object) guid, (object) path));
              }
              else
              {
                string lowerInvariant = Assets.GetSimpleSoundEventName(path).ToLowerInvariant();
                if (lowerInvariant.Length > 0 && !GlobalAssets.SoundTable.ContainsKey(lowerInvariant))
                {
                  GlobalAssets.SoundTable[lowerInvariant] = path;
                  if (path.ToLower().Contains("lowpriority") || lowerInvariant.Contains("lowpriority"))
                    GlobalAssets.LowPrioritySounds.Add(path);
                  else if (path.ToLower().Contains("highpriority") || lowerInvariant.Contains("highpriority"))
                    GlobalAssets.HighPrioritySounds.Add(path);
                }
              }
            }
          }
        }
      }
    }
    SetDefaults.Initialize();
    GraphicsOptionsScreen.SetColorModeFromPrefs();
    this.AddColorModeStyles();
    LocString.CreateLocStringKeys(typeof (UI));
    LocString.CreateLocStringKeys(typeof (INPUT));
    LocString.CreateLocStringKeys(typeof (GAMEPLAY_EVENTS));
    LocString.CreateLocStringKeys(typeof (ROOMS));
    LocString.CreateLocStringKeys(typeof (BUILDING.STATUSITEMS), "STRINGS.BUILDING.");
    LocString.CreateLocStringKeys(typeof (BUILDING.DETAILS), "STRINGS.BUILDING.");
    LocString.CreateLocStringKeys(typeof (SETITEMS));
    LocString.CreateLocStringKeys(typeof (COLONY_ACHIEVEMENTS));
    LocString.CreateLocStringKeys(typeof (CREATURES));
    LocString.CreateLocStringKeys(typeof (RESEARCH));
    LocString.CreateLocStringKeys(typeof (DUPLICANTS));
    LocString.CreateLocStringKeys(typeof (ITEMS));
    LocString.CreateLocStringKeys(typeof (ROBOTS));
    LocString.CreateLocStringKeys(typeof (ELEMENTS));
    LocString.CreateLocStringKeys(typeof (MISC));
    LocString.CreateLocStringKeys(typeof (VIDEOS));
    LocString.CreateLocStringKeys(typeof (NAMEGEN));
    LocString.CreateLocStringKeys(typeof (WORLDS));
    LocString.CreateLocStringKeys(typeof (CLUSTER_NAMES));
    LocString.CreateLocStringKeys(typeof (SUBWORLDS));
    LocString.CreateLocStringKeys(typeof (WORLD_TRAITS));
    LocString.CreateLocStringKeys(typeof (INPUT_BINDINGS));
    LocString.CreateLocStringKeys(typeof (LORE));
    LocString.CreateLocStringKeys(typeof (CODEX));
    LocString.CreateLocStringKeys(typeof (SUBWORLDS));
  }

  private void AddColorModeStyles()
  {
    TMP_StyleSheet.instance.AddStyle(new TMP_Style("logic_on", string.Format("<color=#{0}>", (object) ColorUtility.ToHtmlStringRGB(Color32.op_Implicit(this.colorSet.logicOn))), "</color>"));
    TMP_StyleSheet.instance.AddStyle(new TMP_Style("logic_off", string.Format("<color=#{0}>", (object) ColorUtility.ToHtmlStringRGB(Color32.op_Implicit(this.colorSet.logicOff))), "</color>"));
    TMP_StyleSheet.RefreshStyles();
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    GlobalAssets.Instance = (GlobalAssets) null;
  }

  public static string GetSound(string name, bool force_no_warning = false)
  {
    if (name == null)
      return (string) null;
    name = name.ToLowerInvariant();
    string sound = (string) null;
    GlobalAssets.SoundTable.TryGetValue(name, out sound);
    return sound;
  }

  public static bool IsLowPriority(string path) => GlobalAssets.LowPrioritySounds.Contains(path);

  public static bool IsHighPriority(string path) => GlobalAssets.HighPrioritySounds.Contains(path);
}
