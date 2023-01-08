// Decompiled with JetBrains decompiler
// Type: Unlocks
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Newtonsoft.Json;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Unlocks")]
public class Unlocks : KMonoBehaviour
{
  private const int FILE_IO_RETRY_ATTEMPTS = 5;
  private List<string> unlocked = new List<string>();
  private List<Unlocks.MetaUnlockCategory> MetaUnlockCategories = new List<Unlocks.MetaUnlockCategory>()
  {
    new Unlocks.MetaUnlockCategory("dimensionalloreMeta", "dimensionallore", 4)
  };
  public Dictionary<string, string[]> lockCollections = new Dictionary<string, string[]>()
  {
    {
      "emails",
      new string[25]
      {
        "email_thermodynamiclaws",
        "email_security2",
        "email_pens2",
        "email_atomiconrecruitment",
        "email_devonsblog",
        "email_researchgiant",
        "email_thejanitor",
        "email_newemployee",
        "email_timeoffapproved",
        "email_security3",
        "email_preliminarycalculations",
        "email_hollandsdog",
        "email_temporalbowupdate",
        "email_retemporalbowupdate",
        "email_memorychip",
        "email_arthistoryrequest",
        "email_AIcontrol",
        "email_AIcontrol2",
        "email_friendlyemail",
        "email_AIcontrol3",
        "email_AIcontrol4",
        "email_engineeringcandidate",
        "email_missingnotes",
        "email_journalistrequest",
        "email_journalistrequest2"
      }
    },
    {
      "journals",
      new string[35]
      {
        "journal_timesarrowthoughts",
        "journal_A046_1",
        "journal_B835_1",
        "journal_sunflowerseeds",
        "journal_B327_1",
        "journal_B556_1",
        "journal_employeeprocessing",
        "journal_B327_2",
        "journal_A046_2",
        "journal_elliesbirthday1",
        "journal_B835_2",
        "journal_ants",
        "journal_pipedream",
        "journal_B556_2",
        "journal_movedrats",
        "journal_B835_3",
        "journal_A046_3",
        "journal_B556_3",
        "journal_B327_3",
        "journal_B835_4",
        "journal_cleanup",
        "journal_A046_4",
        "journal_B327_4",
        "journal_revisitednumbers",
        "journal_B556_4",
        "journal_B835_5",
        "journal_elliesbirthday2",
        "journal_B111_1",
        "journal_revisitednumbers2",
        "journal_timemusings",
        "journal_evil",
        "journal_timesorder",
        "journal_inspace",
        "journal_mysteryaward",
        "journal_courier"
      }
    },
    {
      "researchnotes",
      new string[18]
      {
        "notes_clonedrats",
        "notes_agriculture1",
        "notes_husbandry1",
        "notes_hibiscus3",
        "notes_husbandry2",
        "notes_agriculture2",
        "notes_geneticooze",
        "notes_agriculture3",
        "notes_husbandry3",
        "notes_memoryimplantation",
        "notes_husbandry4",
        "notes_agriculture4",
        "notes_neutronium",
        "notes_firstsuccess",
        "notes_neutroniumapplications",
        "notes_teleportation",
        "notes_AI",
        "cryotank_warning"
      }
    },
    {
      "misc",
      new string[6]
      {
        "misc_newsecurity",
        "misc_mailroometiquette",
        "misc_unattendedcultures",
        "misc_politerequest",
        "misc_casualfriday",
        "misc_dishbot"
      }
    },
    {
      "dimensionallore",
      new string[6]
      {
        "notes_clonedrabbits",
        "notes_clonedraccoons",
        "journal_movedrabbits",
        "journal_movedraccoons",
        "journal_strawberries",
        "journal_shrimp"
      }
    },
    {
      "dimensionalloreMeta",
      new string[1]{ "log9" }
    },
    {
      "space",
      new string[4]
      {
        "display_spaceprop1",
        "notice_pilot",
        "journal_inspace",
        "notes_firstcolony"
      }
    },
    {
      "storytraits",
      new string[5]
      {
        "story_trait_critter_manipulator_initial",
        "story_trait_critter_manipulator_complete",
        "storytrait_crittermanipulator_workiversary",
        "story_trait_mega_brain_tank_initial",
        "story_trait_mega_brain_tank_competed"
      }
    }
  };
  public Dictionary<int, string> cycleLocked = new Dictionary<int, string>()
  {
    {
      0,
      "log1"
    },
    {
      3,
      "log2"
    },
    {
      15,
      "log3"
    },
    {
      1000,
      "log4"
    },
    {
      1500,
      "log4b"
    },
    {
      2000,
      "log5"
    },
    {
      2500,
      "log5b"
    },
    {
      3000,
      "log6"
    },
    {
      3500,
      "log6b"
    },
    {
      4000,
      "log7"
    },
    {
      4001,
      "log8"
    }
  };
  private static readonly EventSystem.IntraObjectHandler<Unlocks> OnLaunchRocketDelegate = new EventSystem.IntraObjectHandler<Unlocks>((Action<Unlocks, object>) ((component, data) => component.OnLaunchRocket(data)));
  private static readonly EventSystem.IntraObjectHandler<Unlocks> OnDuplicantDiedDelegate = new EventSystem.IntraObjectHandler<Unlocks>((Action<Unlocks, object>) ((component, data) => component.OnDuplicantDied(data)));
  private static readonly EventSystem.IntraObjectHandler<Unlocks> OnDiscoveredSpaceDelegate = new EventSystem.IntraObjectHandler<Unlocks>((Action<Unlocks, object>) ((component, data) => component.OnDiscoveredSpace(data)));

  private static string UnlocksFilename => System.IO.Path.Combine(Util.RootFolder(), "unlocks.json");

  protected virtual void OnPrefabInit() => this.LoadUnlocks();

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.UnlockCycleCodexes();
    GameClock.Instance.Subscribe(631075836, new Action<object>(this.OnNewDay));
    this.Subscribe<Unlocks>(-1277991738, Unlocks.OnLaunchRocketDelegate);
    this.Subscribe<Unlocks>(282337316, Unlocks.OnDuplicantDiedDelegate);
    this.Subscribe<Unlocks>(-818188514, Unlocks.OnDiscoveredSpaceDelegate);
    Components.LiveMinionIdentities.OnAdd += new Action<MinionIdentity>(this.OnNewDupe);
  }

  public bool IsUnlocked(string unlockID)
  {
    if (string.IsNullOrEmpty(unlockID))
      return false;
    return DebugHandler.InstantBuildMode || this.unlocked.Contains(unlockID);
  }

  public IReadOnlyList<string> GetAllUnlockedIds() => (IReadOnlyList<string>) this.unlocked;

  public void Lock(string unlockID)
  {
    if (!this.unlocked.Contains(unlockID))
      return;
    this.unlocked.Remove(unlockID);
    this.SaveUnlocks();
    Game.Instance.Trigger(1594320620, (object) unlockID);
  }

  public void Unlock(string unlockID, bool shouldTryShowCodexNotification = true)
  {
    if (string.IsNullOrEmpty(unlockID))
    {
      DebugUtil.DevAssert(false, "Unlock called with null or empty string", (Object) null);
    }
    else
    {
      if (!this.unlocked.Contains(unlockID))
      {
        this.unlocked.Add(unlockID);
        this.SaveUnlocks();
        Game.Instance.Trigger(1594320620, (object) unlockID);
        if (shouldTryShowCodexNotification)
        {
          MessageNotification unlockNotification = this.GenerateCodexUnlockNotification(unlockID);
          if (unlockNotification != null)
            ((Component) this).GetComponent<Notifier>().Add((Notification) unlockNotification);
        }
      }
      this.EvalMetaCategories();
    }
  }

  private void EvalMetaCategories()
  {
    foreach (Unlocks.MetaUnlockCategory metaUnlockCategory in this.MetaUnlockCategories)
    {
      string metaCollectionId = metaUnlockCategory.metaCollectionID;
      string mesaCollectionId = metaUnlockCategory.mesaCollectionID;
      int mesaUnlockCount = metaUnlockCategory.mesaUnlockCount;
      int num = 0;
      foreach (string unlockID in this.lockCollections[mesaCollectionId])
      {
        if (this.IsUnlocked(unlockID))
          ++num;
      }
      if (num >= mesaUnlockCount)
        this.UnlockNext(metaCollectionId);
    }
  }

  private void SaveUnlocks()
  {
    if (!Directory.Exists(Util.RootFolder()))
      Directory.CreateDirectory(Util.RootFolder());
    string s = JsonConvert.SerializeObject((object) this.unlocked);
    bool flag = false;
    int num = 0;
    while (!flag)
    {
      if (num >= 5)
        break;
      try
      {
        Thread.Sleep(num * 100);
        using (FileStream fileStream = File.Open(Unlocks.UnlocksFilename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
        {
          flag = true;
          byte[] bytes = new ASCIIEncoding().GetBytes(s);
          fileStream.Write(bytes, 0, bytes.Length);
        }
      }
      catch (Exception ex)
      {
        Debug.LogWarningFormat("Failed to save Unlocks attempt {0}: {1}", new object[2]
        {
          (object) (num + 1),
          (object) ex.ToString()
        });
      }
      ++num;
    }
  }

  public void LoadUnlocks()
  {
    this.unlocked.Clear();
    if (!File.Exists(Unlocks.UnlocksFilename))
      return;
    string str1 = "";
    bool flag = false;
    int num = 0;
    while (!flag)
    {
      if (num < 5)
      {
        try
        {
          Thread.Sleep(num * 100);
          using (FileStream fileStream = File.Open(Unlocks.UnlocksFilename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
          {
            flag = true;
            ASCIIEncoding asciiEncoding = new ASCIIEncoding();
            byte[] numArray = new byte[fileStream.Length];
            if ((long) fileStream.Read(numArray, 0, numArray.Length) == fileStream.Length)
              str1 += asciiEncoding.GetString(numArray);
          }
        }
        catch (Exception ex)
        {
          Debug.LogWarningFormat("Failed to load Unlocks attempt {0}: {1}", new object[2]
          {
            (object) (num + 1),
            (object) ex.ToString()
          });
        }
        ++num;
      }
      else
        break;
    }
    if (string.IsNullOrEmpty(str1))
      return;
    try
    {
      foreach (string str2 in JsonConvert.DeserializeObject<string[]>(str1))
      {
        if (!string.IsNullOrEmpty(str2) && !this.unlocked.Contains(str2))
          this.unlocked.Add(str2);
      }
    }
    catch (Exception ex)
    {
      Debug.LogErrorFormat("Error parsing unlocks file [{0}]: {1}", new object[2]
      {
        (object) Unlocks.UnlocksFilename,
        (object) ex.ToString()
      });
    }
  }

  public string UnlockNext(string collectionID)
  {
    foreach (string unlockID in this.lockCollections[collectionID])
    {
      if (string.IsNullOrEmpty(unlockID))
        DebugUtil.DevAssertArgs(false, new object[2]
        {
          (object) "Found null/empty string in Unlocks collection: ",
          (object) collectionID
        });
      else if (!this.IsUnlocked(unlockID))
      {
        this.Unlock(unlockID);
        return unlockID;
      }
    }
    return (string) null;
  }

  private MessageNotification GenerateCodexUnlockNotification(string lockID)
  {
    string entryForLock = CodexCache.GetEntryForLock(lockID);
    if (string.IsNullOrEmpty(entryForLock))
      return (MessageNotification) null;
    string str = (string) null;
    if (CodexCache.FindSubEntry(lockID) != null)
      str = CodexCache.FindSubEntry(lockID).title;
    else if (CodexCache.FindSubEntry(entryForLock) != null)
      str = CodexCache.FindSubEntry(entryForLock).title;
    else if (CodexCache.FindEntry(entryForLock) != null)
      str = CodexCache.FindEntry(entryForLock).title;
    string unlock_message = UI.FormatAsLink(StringEntry.op_Implicit(Strings.Get(str)), entryForLock);
    if (string.IsNullOrEmpty(str))
      return (MessageNotification) null;
    ContentContainer contentContainer = CodexCache.FindEntry(entryForLock).contentContainers.Find((Predicate<ContentContainer>) (match => match.lockID == lockID));
    if (contentContainer != null)
    {
      foreach (ICodexWidget codexWidget in contentContainer.content)
      {
        if (codexWidget is CodexText codexText)
          unlock_message = unlock_message + "\n\n" + codexText.text;
      }
    }
    return new MessageNotification((Message) new CodexUnlockedMessage(lockID, unlock_message));
  }

  private void UnlockCycleCodexes()
  {
    foreach (KeyValuePair<int, string> keyValuePair in this.cycleLocked)
    {
      if (GameClock.Instance.GetCycle() + 1 >= keyValuePair.Key)
        this.Unlock(keyValuePair.Value);
    }
  }

  private void OnNewDay(object data) => this.UnlockCycleCodexes();

  private void OnLaunchRocket(object data)
  {
    this.Unlock("surfacebreach");
    this.Unlock("firstrocketlaunch");
  }

  private void OnDuplicantDied(object data)
  {
    this.Unlock("duplicantdeath");
    if (Components.LiveMinionIdentities.Count != 1)
      return;
    this.Unlock("onedupeleft");
  }

  private void OnNewDupe(MinionIdentity minion_identity)
  {
    if (Components.LiveMinionIdentities.Count < Db.Get().Personalities.GetAll(true, false).Count)
      return;
    this.Unlock("fulldupecolony");
  }

  private void OnDiscoveredSpace(object data) => this.Unlock("surfacebreach");

  public void Sim4000ms(float dt)
  {
    int x1 = int.MinValue;
    int num1 = int.MinValue;
    int x2 = int.MaxValue;
    int num2 = int.MaxValue;
    foreach (MinionIdentity cmp in Components.MinionIdentities.Items)
    {
      if (!Object.op_Equality((Object) cmp, (Object) null))
      {
        int cell = Grid.PosToCell((KMonoBehaviour) cmp);
        if (Grid.IsValidCell(cell))
        {
          int x3;
          int y;
          Grid.CellToXY(cell, out x3, out y);
          if (y > num1)
          {
            num1 = y;
            x1 = x3;
          }
          if (y < num2)
          {
            x2 = x3;
            num2 = y;
          }
        }
      }
    }
    if (num1 != int.MinValue)
    {
      int y = num1;
      for (int index = 0; index < 30; ++index)
      {
        ++y;
        int cell = Grid.XYToCell(x1, y);
        if (Grid.IsValidCell(cell))
        {
          if (World.Instance.zoneRenderData.GetSubWorldZoneType(cell) == 7)
          {
            this.Unlock("nearingsurface");
            break;
          }
        }
        else
          break;
      }
    }
    if (num2 == int.MaxValue)
      return;
    int y1 = num2;
    for (int index = 0; index < 30; ++index)
    {
      --y1;
      int cell = Grid.XYToCell(x2, y1);
      if (!Grid.IsValidCell(cell))
        break;
      if (World.Instance.zoneRenderData.GetSubWorldZoneType(cell) == 4 && Grid.Element[cell].id == SimHashes.Magma)
      {
        this.Unlock("nearingmagma");
        break;
      }
    }
  }

  private class MetaUnlockCategory
  {
    public string metaCollectionID;
    public string mesaCollectionID;
    public int mesaUnlockCount;

    public MetaUnlockCategory(
      string metaCollectionID,
      string mesaCollectionID,
      int mesaUnlockCount)
    {
      this.metaCollectionID = metaCollectionID;
      this.mesaCollectionID = mesaCollectionID;
      this.mesaUnlockCount = mesaUnlockCount;
    }
  }
}
