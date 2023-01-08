// Decompiled with JetBrains decompiler
// Type: DebugHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using STRINGS;
using System;
using System.IO;
using UnityEngine;

public class DebugHandler : IInputHandler
{
  public static bool InstantBuildMode;
  public static bool InvincibleMode;
  public static bool SelectInEditor;
  public static bool DebugPathFinding;
  public static bool ScreenshotMode;
  public static bool TimelapseMode;
  public static bool HideUI;
  public static bool DebugCellInfo;
  public static bool DebugNextCall;
  public static bool RevealFogOfWar;
  private bool superTestMode;
  private bool ultraTestMode;
  private bool slowTestMode;
  private static int activeWorldBeforeOverride = -1;

  public static bool NotificationsDisabled { get; private set; }

  public static bool enabled { get; private set; }

  public DebugHandler()
  {
    DebugHandler.enabled = File.Exists(System.IO.Path.Combine(Application.dataPath, "debug_enable.txt"));
    DebugHandler.enabled = DebugHandler.enabled || File.Exists(System.IO.Path.Combine(Application.dataPath, "../debug_enable.txt"));
    DebugHandler.enabled = DebugHandler.enabled || GenericGameSettings.instance.debugEnable;
  }

  public string handlerName => nameof (DebugHandler);

  public KInputHandler inputHandler { get; set; }

  public static int GetMouseCell()
  {
    Vector3 mousePos = KInputManager.GetMousePos();
    mousePos.z = -TransformExtensions.GetPosition(((Component) Camera.main).transform).z - Grid.CellSizeInMeters;
    return Grid.PosToCell(Camera.main.ScreenToWorldPoint(mousePos));
  }

  public static Vector3 GetMousePos()
  {
    Vector3 mousePos = KInputManager.GetMousePos();
    mousePos.z = -TransformExtensions.GetPosition(((Component) Camera.main).transform).z - Grid.CellSizeInMeters;
    return Camera.main.ScreenToWorldPoint(mousePos);
  }

  private void SpawnMinion(bool addAtmoSuit = false)
  {
    if (Object.op_Equality((Object) Immigration.Instance, (Object) null))
      return;
    if (!Grid.IsValidBuildingCell(DebugHandler.GetMouseCell()))
    {
      PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, (string) UI.DEBUG_TOOLS.INVALID_LOCATION, (Transform) null, DebugHandler.GetMousePos(), force_spawn: true);
    }
    else
    {
      GameObject gameObject1 = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(MinionConfig.ID)), (GameObject) null, (string) null);
      ((Object) gameObject1).name = ((Object) Assets.GetPrefab(Tag.op_Implicit(MinionConfig.ID))).name;
      Immigration.Instance.ApplyDefaultPersonalPriorities(gameObject1);
      Vector3 posCbc = Grid.CellToPosCBC(DebugHandler.GetMouseCell(), Grid.SceneLayer.Move);
      TransformExtensions.SetLocalPosition(gameObject1.transform, posCbc);
      gameObject1.SetActive(true);
      new MinionStartingStats(false, isDebugMinion: true).Apply(gameObject1);
      if (addAtmoSuit)
      {
        GameObject gameObject2 = GameUtil.KInstantiate(Assets.GetPrefab(Tag.op_Implicit("Atmo_Suit")), posCbc, Grid.SceneLayer.Creatures);
        gameObject2.SetActive(true);
        SuitTank component1 = gameObject2.GetComponent<SuitTank>();
        GameObject go = GameUtil.KInstantiate(Assets.GetPrefab(GameTags.Oxygen), posCbc, Grid.SceneLayer.Ore);
        go.GetComponent<PrimaryElement>().Units = component1.capacity;
        go.SetActive(true);
        component1.storage.Store(go, true);
        Equippable component2 = gameObject2.GetComponent<Equippable>();
        gameObject1.GetComponent<MinionIdentity>().ValidateProxy();
        Equipment component3 = ((Component) gameObject1.GetComponent<MinionIdentity>().assignableProxy.Get()).GetComponent<Equipment>();
        component2.Assign(((Component) component3).GetComponent<IAssignableIdentity>());
        gameObject2.GetComponent<EquippableWorkable>().CancelChore("Debug Handler");
        component3.Equip(component2);
      }
      gameObject1.GetMyWorld().SetDupeVisited();
    }
  }

  public static void SetDebugEnabled(bool debugEnabled) => DebugHandler.enabled = debugEnabled;

  public static void ToggleDisableNotifications() => DebugHandler.NotificationsDisabled = !DebugHandler.NotificationsDisabled;

  private string GetScreenshotFileName()
  {
    string activeSaveFilePath = SaveLoader.GetActiveSaveFilePath();
    string str = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(activeSaveFilePath), "screenshot");
    string fileName = System.IO.Path.GetFileName(activeSaveFilePath);
    Directory.CreateDirectory(str);
    return System.IO.Path.ChangeExtension(System.IO.Path.Combine(str, fileName), ".png");
  }

  public unsafe void OnKeyDown(KButtonEvent e)
  {
    if (!DebugHandler.enabled)
      return;
    if (e.TryConsume((Action) 174))
      this.SpawnMinion();
    else if (e.TryConsume((Action) 268))
      this.SpawnMinion(true);
    else if (e.TryConsume((Action) 186))
    {
      for (int idx = 0; idx < Components.MinionIdentities.Count; ++idx)
      {
        EmoteChore emoteChore1 = new EmoteChore((IStateMachineTarget) ((Component) Components.MinionIdentities[idx]).GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, HashedString.op_Implicit("anim_cheer_kanim"), new HashedString[3]
        {
          HashedString.op_Implicit("cheer_pre"),
          HashedString.op_Implicit("cheer_loop"),
          HashedString.op_Implicit("cheer_pst")
        });
        EmoteChore emoteChore2 = new EmoteChore((IStateMachineTarget) ((Component) Components.MinionIdentities[idx]).GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, HashedString.op_Implicit("anim_cheer_kanim"), new HashedString[3]
        {
          HashedString.op_Implicit("cheer_pre"),
          HashedString.op_Implicit("cheer_loop"),
          HashedString.op_Implicit("cheer_pst")
        });
      }
    }
    else if (e.TryConsume((Action) 175))
    {
      for (int index = 0; index < 60; ++index)
        this.SpawnMinion();
    }
    else if (e.TryConsume((Action) 176))
    {
      if (!this.superTestMode)
      {
        Time.timeScale = 15f;
        this.superTestMode = true;
      }
      else
      {
        Time.timeScale = 1f;
        this.superTestMode = false;
      }
    }
    else if (e.TryConsume((Action) 177))
    {
      if (!this.ultraTestMode)
      {
        Time.timeScale = 30f;
        this.ultraTestMode = true;
      }
      else
      {
        Time.timeScale = 1f;
        this.ultraTestMode = false;
      }
    }
    else if (e.TryConsume((Action) 178))
    {
      if (!this.slowTestMode)
      {
        Time.timeScale = 0.06f;
        this.slowTestMode = true;
      }
      else
      {
        Time.timeScale = 1f;
        this.slowTestMode = false;
      }
    }
    else if (e.TryConsume((Action) 187))
      SimMessages.Dig(DebugHandler.GetMouseCell());
    else if (e.TryConsume((Action) 180))
      Game.Instance.FastWorkersModeActive = !Game.Instance.FastWorkersModeActive;
    else if (e.TryConsume((Action) 179))
    {
      DebugHandler.InstantBuildMode = !DebugHandler.InstantBuildMode;
      InterfaceTool.ToggleConfig((Action) 179);
      if (Object.op_Equality((Object) Game.Instance, (Object) null))
        return;
      Game.Instance.Trigger(1557339983, (object) null);
      if (Object.op_Inequality((Object) PlanScreen.Instance, (Object) null))
        PlanScreen.Instance.Refresh();
      if (Object.op_Inequality((Object) BuildMenu.Instance, (Object) null))
        BuildMenu.Instance.Refresh();
      if (Object.op_Inequality((Object) OverlayMenu.Instance, (Object) null))
        OverlayMenu.Instance.Refresh();
      if (Object.op_Inequality((Object) ConsumerManager.instance, (Object) null))
        ConsumerManager.instance.RefreshDiscovered();
      if (Object.op_Inequality((Object) ManagementMenu.Instance, (Object) null))
      {
        ManagementMenu.Instance.CheckResearch((object) null);
        ManagementMenu.Instance.CheckSkills();
        ManagementMenu.Instance.CheckStarmap();
      }
      Game.Instance.Trigger(1594320620, (object) "all_the_things");
    }
    else if (e.TryConsume((Action) 181))
    {
      Vector3 mousePos = KInputManager.GetMousePos();
      mousePos.z = -TransformExtensions.GetPosition(((Component) Camera.main).transform).z - Grid.CellSizeInMeters;
      GameUtil.CreateExplosion(Camera.main.ScreenToWorldPoint(mousePos));
    }
    else if (e.TryConsume((Action) 227))
    {
      if (GenericGameSettings.instance.developerDebugEnable)
      {
        KInputManager.isMousePosLocked = !KInputManager.isMousePosLocked;
        KInputManager.lockedMousePos = KInputManager.GetMousePos();
      }
    }
    else if (e.TryConsume((Action) 182))
    {
      if (Object.op_Inequality((Object) DiscoveredResources.Instance, (Object) null))
      {
        foreach (Element element in ElementLoader.elements)
          DiscoveredResources.Instance.Discover(element.tag, element.GetMaterialCategoryTag());
      }
    }
    else if (e.TryConsume((Action) 188))
      DebugHandler.ToggleScreenshotMode();
    else if (e.TryConsume((Action) 169))
      ScreenCapture.CaptureScreenshot(this.GetScreenshotFileName(), 1);
    else if (e.TryConsume((Action) 170))
      ScreenCapture.CaptureScreenshot(this.GetScreenshotFileName(), 2);
    else if (e.TryConsume((Action) 171))
      ScreenCapture.CaptureScreenshot(this.GetScreenshotFileName(), 8);
    else if (e.TryConsume((Action) 172))
      ScreenCapture.CaptureScreenshot(this.GetScreenshotFileName(), 32);
    else if (e.TryConsume((Action) 220))
      DebugHandler.DebugCellInfo = !DebugHandler.DebugCellInfo;
    else if (e.TryConsume((Action) 173))
    {
      if (Object.op_Inequality((Object) Game.Instance, (Object) null))
        SaveGame.Instance.worldGenSpawner.SpawnEverything();
      InterfaceTool.ToggleConfig((Action) 173);
      if (Object.op_Inequality((Object) DebugPaintElementScreen.Instance, (Object) null))
      {
        bool activeSelf = ((Component) DebugPaintElementScreen.Instance).gameObject.activeSelf;
        ((Component) DebugPaintElementScreen.Instance).gameObject.SetActive(!activeSelf);
        if (Object.op_Implicit((Object) DebugElementMenu.Instance) && DebugElementMenu.Instance.root.activeSelf)
          DebugElementMenu.Instance.root.SetActive(false);
        ((Component) DebugBaseTemplateButton.Instance).gameObject.SetActive(!activeSelf);
        PropertyTextures.FogOfWarScale = !activeSelf ? 1f : 0.0f;
        if (Object.op_Inequality((Object) CameraController.Instance, (Object) null))
          CameraController.Instance.EnableFreeCamera(!activeSelf);
        DebugHandler.RevealFogOfWar = !DebugHandler.RevealFogOfWar;
        Game.Instance.Trigger(-1991583975, (object) null);
      }
    }
    else if (e.TryConsume((Action) 189))
      GC.Collect();
    else if (e.TryConsume((Action) 190))
      DebugHandler.InvincibleMode = !DebugHandler.InvincibleMode;
    else if (e.TryConsume((Action) 197))
      Scenario.Instance.SetupVisualTest();
    else if (e.TryConsume((Action) 198))
      Scenario.Instance.SetupGameplayTest();
    else if (e.TryConsume((Action) 199))
      Scenario.Instance.SetupElementTest();
    else if (e.TryConsume((Action) 215))
      Sim.SIM_HandleMessage(-409964931, 0, (byte*) null);
    else if (e.TryConsume((Action) 191))
      Pathfinding.Instance.RefreshNavCell(DebugHandler.GetMouseCell());
    else if (e.TryConsume((Action) 206))
      DebugHandler.SetSelectInEditor(!DebugHandler.SelectInEditor);
    else if (e.TryConsume((Action) 202))
    {
      Debug.Log((object) "Debug GoTo");
      Game.Instance.Trigger(775300118, (object) null);
      foreach (Brain cmp in Components.Brains.Items)
      {
        ((Component) cmp).GetSMI<DebugGoToMonitor.Instance>()?.GoToCursor();
        ((Component) cmp).GetSMI<CreatureDebugGoToMonitor.Instance>()?.GoToCursor();
      }
    }
    else if (e.TryConsume((Action) 203))
    {
      if (Object.op_Equality((Object) SelectTool.Instance, (Object) null))
        return;
      KSelectable selected = SelectTool.Instance.selected;
      if (Object.op_Inequality((Object) selected, (Object) null))
      {
        int mouseCell = DebugHandler.GetMouseCell();
        if (!Grid.IsValidBuildingCell(mouseCell))
        {
          PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, (string) UI.DEBUG_TOOLS.INVALID_LOCATION, (Transform) null, DebugHandler.GetMousePos(), force_spawn: true);
          return;
        }
        TransformExtensions.SetPosition(selected.transform, Grid.CellToPosCBC(mouseCell, Grid.SceneLayer.Move));
      }
    }
    else if (!e.TryConsume((Action) 196) && !e.TryConsume((Action) 204))
    {
      if (e.TryConsume((Action) 211))
      {
        if (GenericGameSettings.instance.developerDebugEnable)
          Tutorial.Instance.DebugNotification();
      }
      else if (e.TryConsume((Action) 212))
      {
        if (GenericGameSettings.instance.developerDebugEnable)
          Tutorial.Instance.DebugNotificationMessage();
      }
      else if (e.TryConsume((Action) 208))
      {
        if (Object.op_Inequality((Object) SpeedControlScreen.Instance, (Object) null))
          SpeedControlScreen.Instance.ToggleRidiculousSpeed();
      }
      else if (e.TryConsume((Action) 209))
      {
        if (Object.op_Inequality((Object) SpeedControlScreen.Instance, (Object) null))
          SpeedControlScreen.Instance.DebugStepFrame();
      }
      else if (e.TryConsume((Action) 210))
        Game.Instance.ForceSimStep();
      else if (e.TryConsume((Action) 205))
        AudioDebug.Get().ToggleMusic();
      else if (e.TryConsume((Action) 201))
        Scenario.Instance.SetupTileTest();
      else if (e.TryConsume((Action) 195))
        PropertyTextures.instance.ForceLightEverywhere = !PropertyTextures.instance.ForceLightEverywhere;
      else if (e.TryConsume((Action) 207))
      {
        DebugHandler.DebugPathFinding = !DebugHandler.DebugPathFinding;
        Debug.Log((object) ("DebugPathFinding=" + DebugHandler.DebugPathFinding.ToString()));
      }
      else if (!e.TryConsume((Action) 219))
      {
        if (e.TryConsume((Action) 218))
        {
          if (GenericGameSettings.instance.developerDebugEnable)
          {
            int num = 0;
            string validSaveFilename;
            while (true)
            {
              validSaveFilename = SaveScreen.GetValidSaveFilename("bug_report_savefile_" + num.ToString());
              if (File.Exists(validSaveFilename))
                ++num;
              else
                break;
            }
            string save_file = "No save file (front end)";
            if (Object.op_Inequality((Object) SaveLoader.Instance, (Object) null))
              save_file = SaveLoader.Instance.Save(validSaveFilename, updateSavePointer: false);
            KCrashReporter.ReportBug("Bug Report", save_file, GameObject.Find("ScreenSpaceOverlayCanvas"));
          }
          else
            Debug.Log((object) "Debug crash keys are not enabled.");
        }
        else if (e.TryConsume((Action) 183))
        {
          if (GenericGameSettings.instance.developerDebugEnable)
            throw new ArgumentException("My test exception");
        }
        else if (e.TryConsume((Action) 184))
        {
          if (GenericGameSettings.instance.developerDebugEnable)
            Debug.LogError((object) "Oooops! Testing error!");
        }
        else if (e.TryConsume((Action) 221))
          GarbageProfiler.DebugDumpRootItems();
        else if (e.TryConsume((Action) 222))
          GarbageProfiler.DebugDumpGarbageStats();
        else if (e.TryConsume((Action) 223))
        {
          if (GenericGameSettings.instance.developerDebugEnable)
            KObjectManager.Instance.DumpEventData();
        }
        else if (e.TryConsume((Action) 224))
        {
          if (!GenericGameSettings.instance.developerDebugEnable)
            ;
        }
        else if (e.TryConsume((Action) 225))
        {
          if (GenericGameSettings.instance.developerDebugEnable)
            Sim.SIM_DebugCrash();
        }
        else if (e.TryConsume((Action) 226))
          DebugHandler.DebugNextCall = true;
        else if (e.TryConsume((Action) 185))
          Chore.ENABLE_PERSONAL_PRIORITIES = !Chore.ENABLE_PERSONAL_PRIORITIES;
        else if (e.TryConsume((Action) 192))
          CameraController.Instance.ToggleClusterFX();
      }
    }
    if (!((KInputEvent) e).Consumed || !Object.op_Inequality((Object) Game.Instance, (Object) null))
      return;
    Game.Instance.debugWasUsed = true;
    KCrashReporter.debugWasUsed = true;
  }

  public static void SetSelectInEditor(bool select_in_editor)
  {
  }

  public static void ToggleScreenshotMode()
  {
    DebugHandler.ScreenshotMode = !DebugHandler.ScreenshotMode;
    DebugHandler.UpdateUI();
    if (Object.op_Inequality((Object) CameraController.Instance, (Object) null))
      CameraController.Instance.EnableFreeCamera(DebugHandler.ScreenshotMode);
    if (!Object.op_Inequality((Object) KScreenManager.Instance, (Object) null))
      return;
    KScreenManager.Instance.DisableInput(DebugHandler.ScreenshotMode);
  }

  public static void SetTimelapseMode(bool enabled, int world_id = 0)
  {
    DebugHandler.TimelapseMode = enabled;
    if (enabled)
    {
      DebugHandler.activeWorldBeforeOverride = ClusterManager.Instance.activeWorldId;
      ClusterManager.Instance.TimelapseModeOverrideActiveWorld(world_id);
    }
    else
      ClusterManager.Instance.TimelapseModeOverrideActiveWorld(DebugHandler.activeWorldBeforeOverride);
    World.Instance.zoneRenderData.OnActiveWorldChanged();
    DebugHandler.UpdateUI();
  }

  private static void UpdateUI()
  {
    DebugHandler.HideUI = DebugHandler.TimelapseMode || DebugHandler.ScreenshotMode;
    float num = DebugHandler.HideUI ? 0.0f : 1f;
    GameScreenManager.Instance.ssHoverTextCanvas.GetComponent<CanvasGroup>().alpha = num;
    GameScreenManager.Instance.ssCameraCanvas.GetComponent<CanvasGroup>().alpha = num;
    GameScreenManager.Instance.ssOverlayCanvas.GetComponent<CanvasGroup>().alpha = num;
    GameScreenManager.Instance.worldSpaceCanvas.GetComponent<CanvasGroup>().alpha = num;
    GameScreenManager.Instance.screenshotModeCanvas.GetComponent<CanvasGroup>().alpha = 1f - num;
  }

  public enum PaintMode
  {
    None,
    Element,
    Hot,
    Cold,
  }
}
