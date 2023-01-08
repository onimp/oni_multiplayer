// Decompiled with JetBrains decompiler
// Type: SaveUpgradeWarning
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using Klei.AI;
using Klei.CustomSettings;
using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SaveUpgradeWarning")]
public class SaveUpgradeWarning : KMonoBehaviour
{
  [MyCmpReq]
  private Game game;
  private static string[] buildingIDsWithNewPorts = new string[6]
  {
    "LiquidVent",
    "GasVent",
    "GasVentHighPressure",
    "SolidVent",
    "LiquidReservoir",
    "GasReservoir"
  };

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.game.OnLoad += new Action<Game.GameSaveData>(this.OnLoad);
  }

  protected virtual void OnCleanUp()
  {
    this.game.OnLoad -= new Action<Game.GameSaveData>(this.OnLoad);
    base.OnCleanUp();
  }

  private void OnLoad(Game.GameSaveData data)
  {
    List<SaveUpgradeWarning.Upgrade> upgradeList = new List<SaveUpgradeWarning.Upgrade>()
    {
      new SaveUpgradeWarning.Upgrade(7, 5, new System.Action(this.SuddenMoraleHelper)),
      new SaveUpgradeWarning.Upgrade(7, 13, new System.Action(this.BedAndBathHelper)),
      new SaveUpgradeWarning.Upgrade(7, 16, new System.Action(this.NewAutomationWarning))
    };
    if (DlcManager.IsPureVanilla())
      upgradeList.Add(new SaveUpgradeWarning.Upgrade(7, 25, new System.Action(this.MergedownWarning)));
    foreach (SaveUpgradeWarning.Upgrade upgrade in upgradeList)
    {
      if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(upgrade.major, upgrade.minor))
        upgrade.action();
    }
  }

  private void SuddenMoraleHelper()
  {
    Effect morale_effect = Db.Get().effects.Get(nameof (SuddenMoraleHelper));
    CustomizableDialogScreen screen = Util.KInstantiateUI<CustomizableDialogScreen>(((Component) ScreenPrefabs.Instance.CustomizableDialogScreen).gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, true);
    screen.AddOption((string) UI.FRONTEND.SAVEUPGRADEWARNINGS.SUDDENMORALEHELPER_BUFF, (System.Action) (() =>
    {
      foreach (Component component in Components.LiveMinionIdentities.Items)
        component.GetComponent<Effects>().Add(morale_effect, true);
      screen.Deactivate();
    }));
    screen.AddOption((string) UI.FRONTEND.SAVEUPGRADEWARNINGS.SUDDENMORALEHELPER_DISABLE, (System.Action) (() =>
    {
      SettingConfig morale = CustomGameSettingConfigs.Morale;
      CustomGameSettings.Instance.customGameMode = CustomGameSettings.CustomGameMode.Custom;
      CustomGameSettings.Instance.SetQualitySetting(morale, morale.GetLevel("Disabled").id);
      screen.Deactivate();
    }));
    screen.PopupConfirmDialog(string.Format((string) UI.FRONTEND.SAVEUPGRADEWARNINGS.SUDDENMORALEHELPER, (object) Mathf.RoundToInt(morale_effect.duration / 600f)), (string) UI.FRONTEND.SAVEUPGRADEWARNINGS.SUDDENMORALEHELPER_TITLE);
  }

  private void BedAndBathHelper()
  {
    if (Object.op_Equality((Object) SaveGame.Instance, (Object) null))
      return;
    ColonyAchievementTracker component = ((Component) SaveGame.Instance).GetComponent<ColonyAchievementTracker>();
    if (Object.op_Equality((Object) component, (Object) null))
      return;
    ColonyAchievement basicComforts = Db.Get().ColonyAchievements.BasicComforts;
    ColonyAchievementStatus achievementStatus = (ColonyAchievementStatus) null;
    if (!component.achievements.TryGetValue(basicComforts.Id, out achievementStatus))
      return;
    achievementStatus.failed = false;
  }

  private void NewAutomationWarning()
  {
    SpriteListDialogScreen screen = Util.KInstantiateUI<SpriteListDialogScreen>(((Component) ScreenPrefabs.Instance.SpriteListDialogScreen).gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, true);
    screen.AddOption((string) UI.CONFIRMDIALOG.OK, (System.Action) (() => screen.Deactivate()));
    foreach (string buildingIdsWithNewPort in SaveUpgradeWarning.buildingIDsWithNewPorts)
    {
      BuildingDef buildingDef = Assets.GetBuildingDef(buildingIdsWithNewPort);
      screen.AddSprite(buildingDef.GetUISprite(), buildingDef.Name);
    }
    screen.PopupConfirmDialog((string) UI.FRONTEND.SAVEUPGRADEWARNINGS.NEWAUTOMATIONWARNING, (string) UI.FRONTEND.SAVEUPGRADEWARNINGS.NEWAUTOMATIONWARNING_TITLE);
    ((MonoBehaviour) this).StartCoroutine(this.SendAutomationWarningNotifications());
  }

  private IEnumerator SendAutomationWarningNotifications()
  {
    yield return (object) SequenceUtil.WaitForEndOfFrame;
    if (Components.BuildingCompletes.Count == 0)
      Debug.LogWarning((object) "Could not send automation warnings because buildings have not yet loaded");
    foreach (BuildingComplete buildingComplete in Components.BuildingCompletes)
    {
      foreach (string buildingIdsWithNewPort in SaveUpgradeWarning.buildingIDsWithNewPorts)
      {
        BuildingDef buildingDef = Assets.GetBuildingDef(buildingIdsWithNewPort);
        if (Object.op_Equality((Object) buildingComplete.Def, (Object) buildingDef))
        {
          List<ILogicUIElement> logicUiElementList = new List<ILogicUIElement>();
          LogicPorts component = ((Component) buildingComplete).GetComponent<LogicPorts>();
          if (component.outputPorts != null)
            logicUiElementList.AddRange((IEnumerable<ILogicUIElement>) component.outputPorts);
          if (component.inputPorts != null)
            logicUiElementList.AddRange((IEnumerable<ILogicUIElement>) component.inputPorts);
          foreach (ILogicUIElement logicUiElement in logicUiElementList)
          {
            if (Object.op_Inequality((Object) Grid.Objects[logicUiElement.GetLogicUICell(), 31], (Object) null))
            {
              Debug.Log((object) ("Triggering automation warning for building of type " + buildingIdsWithNewPort));
              GenericMessage genericMessage = new GenericMessage((string) MISC.NOTIFICATIONS.NEW_AUTOMATION_WARNING.NAME, (string) MISC.NOTIFICATIONS.NEW_AUTOMATION_WARNING.TOOLTIP, (string) MISC.NOTIFICATIONS.NEW_AUTOMATION_WARNING.TOOLTIP, (KMonoBehaviour) buildingComplete);
              Messenger.Instance.QueueMessage((Message) genericMessage);
            }
          }
        }
      }
    }
  }

  private void MergedownWarning()
  {
    SpriteListDialogScreen screen = Util.KInstantiateUI<SpriteListDialogScreen>(((Component) ScreenPrefabs.Instance.SpriteListDialogScreen).gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, true);
    screen.AddOption((string) UI.DEVELOPMENTBUILDS.FULL_PATCH_NOTES, (System.Action) (() => App.OpenWebURL("https://forums.kleientertainment.com/game-updates/oni-alpha/")));
    screen.AddOption((string) UI.CONFIRMDIALOG.OK, (System.Action) (() => screen.Deactivate()));
    screen.AddSprite(Assets.GetSprite(HashedString.op_Implicit("upgrade_mergedown_fridge")), (string) UI.FRONTEND.SAVEUPGRADEWARNINGS.MERGEDOWNCHANGES_FOOD, 150f, 120f);
    screen.AddSprite(Assets.GetSprite(HashedString.op_Implicit("upgrade_mergedown_deodorizer")), (string) UI.FRONTEND.SAVEUPGRADEWARNINGS.MERGEDOWNCHANGES_AIRFILTER, 150f, 120f);
    screen.AddSprite(Assets.GetSprite(HashedString.op_Implicit("upgrade_mergedown_steamturbine")), (string) UI.FRONTEND.SAVEUPGRADEWARNINGS.MERGEDOWNCHANGES_SIMULATION, 150f, 120f);
    screen.AddSprite(Assets.GetSprite(HashedString.op_Implicit("upgrade_mergedown_oxygen_meter")), (string) UI.FRONTEND.SAVEUPGRADEWARNINGS.MERGEDOWNCHANGES_BUILDINGS, 150f, 120f);
    screen.PopupConfirmDialog((string) UI.FRONTEND.SAVEUPGRADEWARNINGS.MERGEDOWNCHANGES, (string) UI.FRONTEND.SAVEUPGRADEWARNINGS.MERGEDOWNCHANGES_TITLE);
    ((MonoBehaviour) this).StartCoroutine(this.SendAutomationWarningNotifications());
  }

  private struct Upgrade
  {
    public int major;
    public int minor;
    public System.Action action;

    public Upgrade(int major, int minor, System.Action action)
    {
      this.major = major;
      this.minor = minor;
      this.action = action;
    }
  }
}
