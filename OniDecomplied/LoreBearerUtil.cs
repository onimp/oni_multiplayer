// Decompiled with JetBrains decompiler
// Type: LoreBearerUtil
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

public static class LoreBearerUtil
{
  public static void AddLoreTo(GameObject prefabOrGameObject) => prefabOrGameObject.AddOrGet<LoreBearer>();

  public static void AddLoreTo(GameObject prefabOrGameObject, LoreBearerAction unlockLoreFn)
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    LoreBearerUtil.\u003C\u003Ec__DisplayClass1_0 cDisplayClass10 = new LoreBearerUtil.\u003C\u003Ec__DisplayClass1_0();
    // ISSUE: reference to a compiler-generated field
    cDisplayClass10.unlockLoreFn = unlockLoreFn;
    KPrefabID component = prefabOrGameObject.GetComponent<KPrefabID>();
    if (((KMonoBehaviour) component).IsInitialized())
    {
      // ISSUE: reference to a compiler-generated field
      prefabOrGameObject.AddOrGet<LoreBearer>().Internal_SetContent(cDisplayClass10.unlockLoreFn);
    }
    else
    {
      prefabOrGameObject.AddComponent<LoreBearer>();
      // ISSUE: method pointer
      component.prefabInitFn += new KPrefabID.PrefabFn((object) cDisplayClass10, __methodptr(\u003CAddLoreTo\u003Eb__0));
    }
  }

  public static LoreBearerAction UnlockSpecificEntry(string unlockId, string searchDisplayText) => (LoreBearerAction) (screen =>
  {
    Game.Instance.unlocks.Unlock(unlockId);
    screen.AddPlainText(searchDisplayText);
    screen.AddOption((string) UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearerUtil.OpenCodexByLockKeyID(unlockId));
  });

  public static void UnlockNextEmail(InfoDialogScreen screen)
  {
    string key = Game.Instance.unlocks.UnlockNext("emails");
    if (key != null)
    {
      string str = "SEARCH" + Random.Range(1, 6).ToString();
      screen.AddPlainText(StringEntry.op_Implicit(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_COMPUTER_SUCCESS." + str)));
      screen.AddOption((string) UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearerUtil.OpenCodexByLockKeyID(key));
    }
    else
    {
      string str = "SEARCH" + Random.Range(1, 8).ToString();
      screen.AddPlainText(StringEntry.op_Implicit(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_COMPUTER_FAIL." + str)));
    }
  }

  public static void UnlockNextResearchNote(InfoDialogScreen screen)
  {
    string key = Game.Instance.unlocks.UnlockNext("researchnotes");
    if (key != null)
    {
      string str = "SEARCH" + Random.Range(1, 3).ToString();
      screen.AddPlainText(StringEntry.op_Implicit(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_TECHNOLOGY_SUCCESS." + str)));
      screen.AddOption((string) UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearerUtil.OpenCodexByLockKeyID(key));
    }
    else
    {
      string str = "SEARCH1";
      screen.AddPlainText(StringEntry.op_Implicit(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_FAIL." + str)));
    }
  }

  public static void UnlockNextJournalEntry(InfoDialogScreen screen)
  {
    string key = Game.Instance.unlocks.UnlockNext("journals");
    if (key != null)
    {
      string str = "SEARCH" + Random.Range(1, 6).ToString();
      screen.AddPlainText(StringEntry.op_Implicit(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_SUCCESS." + str)));
      screen.AddOption((string) UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearerUtil.OpenCodexByLockKeyID(key));
    }
    else
    {
      string str = "SEARCH1";
      screen.AddPlainText(StringEntry.op_Implicit(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_FAIL." + str)));
    }
  }

  public static void UnlockNextDimensionalLore(InfoDialogScreen screen)
  {
    string key = Game.Instance.unlocks.UnlockNext("dimensionallore");
    if (key != null)
    {
      string str = "SEARCH" + Random.Range(1, 6).ToString();
      screen.AddPlainText(StringEntry.op_Implicit(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_SUCCESS." + str)));
      screen.AddOption((string) UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearerUtil.OpenCodexByLockKeyID(key));
    }
    else
    {
      string str = "SEARCH1";
      screen.AddPlainText(StringEntry.op_Implicit(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_FAIL." + str)));
    }
  }

  public static void UnlockNextSpaceEntry(InfoDialogScreen screen)
  {
    string key = Game.Instance.unlocks.UnlockNext("space");
    if (key != null)
    {
      string str = "SEARCH" + Random.Range(1, 7).ToString();
      screen.AddPlainText(StringEntry.op_Implicit(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_SPACEPOI_SUCCESS." + str)));
      screen.AddOption((string) UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearerUtil.OpenCodexByLockKeyID(key));
    }
    else
    {
      string str = "SEARCH" + Random.Range(1, 4).ToString();
      screen.AddPlainText(StringEntry.op_Implicit(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_SPACEPOI_FAIL." + str)));
    }
  }

  public static void UnlockNextDeskPodiumEntry(InfoDialogScreen screen)
  {
    if (!Game.Instance.unlocks.IsUnlocked("story_trait_critter_manipulator_parking"))
    {
      Game.Instance.unlocks.Unlock("story_trait_critter_manipulator_parking");
      string str = "SEARCH" + Random.Range(1, 1).ToString();
      screen.AddPlainText(StringEntry.op_Implicit(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_COMPUTER_PODIUM." + str)));
      screen.AddOption((string) UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearerUtil.OpenCodexByLockKeyID("story_trait_critter_manipulator_parking"));
    }
    else
    {
      string str = "SEARCH" + Random.Range(1, 8).ToString();
      screen.AddPlainText(StringEntry.op_Implicit(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_COMPUTER_FAIL." + str)));
    }
  }

  public static void NerualVacillator(InfoDialogScreen screen)
  {
    Game.Instance.unlocks.Unlock("neuralvacillator");
    LoreBearerUtil.UnlockNextResearchNote(screen);
  }

  public static Action<InfoDialogScreen> OpenCodexByLockKeyID(string key, bool focusContent = false) => (Action<InfoDialogScreen>) (dialog =>
  {
    dialog.Deactivate();
    string entryForLock = CodexCache.GetEntryForLock(key);
    if (entryForLock == null)
    {
      DebugUtil.DevLogError("Missing codex entry for lock: " + key);
    }
    else
    {
      ContentContainer targetContainer = (ContentContainer) null;
      if (focusContent)
      {
        CodexEntry entry = CodexCache.FindEntry(entryForLock);
        for (int index = 0; targetContainer == null && index < entry.contentContainers.Count; ++index)
        {
          if (!(entry.contentContainers[index].lockID != key))
            targetContainer = entry.contentContainers[index];
        }
      }
      ManagementMenu.Instance.OpenCodexToEntry(entryForLock, targetContainer);
    }
  });

  public static Action<InfoDialogScreen> OpenCodexByEntryID(string id) => (Action<InfoDialogScreen>) (dialog =>
  {
    dialog.Deactivate();
    ManagementMenu.Instance.OpenCodexToEntry(id);
  });
}
