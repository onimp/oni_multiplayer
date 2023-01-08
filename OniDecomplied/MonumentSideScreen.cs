// Decompiled with JetBrains decompiler
// Type: MonumentSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MonumentSideScreen : SideScreenContent
{
  private MonumentPart target;
  public KButton debugVictoryButton;
  public KButton flipButton;
  public GameObject stateButtonPrefab;
  private List<GameObject> buttons = new List<GameObject>();
  [SerializeField]
  private RectTransform buttonContainer;

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<MonumentPart>(), (Object) null);

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.debugVictoryButton.onClick += (System.Action) (() =>
    {
      ((Component) SaveGame.Instance).GetComponent<ColonyAchievementTracker>().DebugTriggerAchievement(Db.Get().ColonyAchievements.Thriving.Id);
      ((Component) SaveGame.Instance).GetComponent<ColonyAchievementTracker>().DebugTriggerAchievement(Db.Get().ColonyAchievements.Clothe8Dupes.Id);
      ((Component) SaveGame.Instance).GetComponent<ColonyAchievementTracker>().DebugTriggerAchievement(Db.Get().ColonyAchievements.Build4NatureReserves.Id);
      ((Component) SaveGame.Instance).GetComponent<ColonyAchievementTracker>().DebugTriggerAchievement(Db.Get().ColonyAchievements.ReachedSpace.Id);
      GameScheduler.Instance.Schedule("ForceCheckAchievements", 0.1f, (Action<object>) (data => Game.Instance.Trigger(395452326, (object) null)), (object) null, (SchedulerGroup) null);
    });
    ((Component) this.debugVictoryButton).gameObject.SetActive(DebugHandler.InstantBuildMode && this.target.part == MonumentPartResource.Part.Top);
    int num;
    this.flipButton.onClick += (System.Action) (() => num = (int) ((Component) this.target).GetComponent<Rotatable>().Rotate());
  }

  public override void SetTarget(GameObject target)
  {
    base.SetTarget(target);
    this.target = target.GetComponent<MonumentPart>();
    ((Component) this.debugVictoryButton).gameObject.SetActive(DebugHandler.InstantBuildMode && this.target.part == MonumentPartResource.Part.Top);
    this.GenerateStateButtons();
  }

  public void GenerateStateButtons()
  {
    for (int index = this.buttons.Count - 1; index >= 0; --index)
      Util.KDestroyGameObject(this.buttons[index]);
    this.buttons.Clear();
    foreach (MonumentPartResource part in Db.GetMonumentParts().GetParts(this.target.part))
    {
      MonumentPartResource state = part;
      GameObject gameObject = Util.KInstantiateUI(this.stateButtonPrefab, ((Component) this.buttonContainer).gameObject, true);
      string state1 = state.State;
      string symbolName = state.SymbolName;
      gameObject.GetComponent<KButton>().onClick += (System.Action) (() => this.target.SetState(state.Id));
      this.buttons.Add(gameObject);
      gameObject.GetComponent<KButton>().fgImage.sprite = Def.GetUISpriteFromMultiObjectAnim(state.AnimFile, state1, symbolName: symbolName);
    }
  }
}
