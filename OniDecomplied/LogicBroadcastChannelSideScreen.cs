// Decompiled with JetBrains decompiler
// Type: LogicBroadcastChannelSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogicBroadcastChannelSideScreen : SideScreenContent
{
  private LogicBroadcastReceiver sensor;
  [SerializeField]
  private GameObject rowPrefab;
  [SerializeField]
  private GameObject listContainer;
  [SerializeField]
  private LocText headerLabel;
  [SerializeField]
  private GameObject noChannelRow;
  private Dictionary<LogicBroadcaster, GameObject> broadcasterRows = new Dictionary<LogicBroadcaster, GameObject>();
  private GameObject emptySpaceRow;

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<LogicBroadcastReceiver>(), (Object) null);

  public override void SetTarget(GameObject target)
  {
    base.SetTarget(target);
    this.sensor = target.GetComponent<LogicBroadcastReceiver>();
    this.Build();
  }

  private void ClearRows()
  {
    if (Object.op_Inequality((Object) this.emptySpaceRow, (Object) null))
      Util.KDestroyGameObject(this.emptySpaceRow);
    foreach (KeyValuePair<LogicBroadcaster, GameObject> broadcasterRow in this.broadcasterRows)
      Util.KDestroyGameObject(broadcasterRow.Value);
    this.broadcasterRows.Clear();
  }

  private void Build()
  {
    ((TMP_Text) this.headerLabel).SetText((string) STRINGS.UI.UISIDESCREENS.LOGICBROADCASTCHANNELSIDESCREEN.HEADER);
    this.ClearRows();
    foreach (LogicBroadcaster logicBroadcaster in Components.LogicBroadcasters)
    {
      if (!Util.IsNullOrDestroyed((object) logicBroadcaster))
      {
        GameObject gameObject = Util.KInstantiateUI(this.rowPrefab, this.listContainer, false);
        ((Object) gameObject.gameObject).name = ((Component) logicBroadcaster).gameObject.GetProperName();
        Debug.Assert(!this.broadcasterRows.ContainsKey(logicBroadcaster), (object) ("Adding two of the same broadcaster to LogicBroadcastChannelSideScreen UI: " + ((Component) logicBroadcaster).gameObject.GetProperName()));
        this.broadcasterRows.Add(logicBroadcaster, gameObject);
        gameObject.SetActive(true);
      }
    }
    this.noChannelRow.SetActive(Components.LogicBroadcasters.Count == 0);
    this.Refresh();
  }

  private void Refresh()
  {
    foreach (KeyValuePair<LogicBroadcaster, GameObject> broadcasterRow in this.broadcasterRows)
    {
      KeyValuePair<LogicBroadcaster, GameObject> kvp = broadcasterRow;
      ((TMP_Text) kvp.Value.GetComponent<HierarchyReferences>().GetReference<LocText>("Label")).SetText(((Component) kvp.Key).gameObject.GetProperName());
      ((TMP_Text) kvp.Value.GetComponent<HierarchyReferences>().GetReference<LocText>("DistanceLabel")).SetText((string) (LogicBroadcastReceiver.CheckRange(((Component) this.sensor).gameObject, ((Component) kvp.Key).gameObject) ? STRINGS.UI.UISIDESCREENS.LOGICBROADCASTCHANNELSIDESCREEN.IN_RANGE : STRINGS.UI.UISIDESCREENS.LOGICBROADCASTCHANNELSIDESCREEN.OUT_OF_RANGE));
      kvp.Value.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").sprite = Def.GetUISprite((object) ((Component) kvp.Key).gameObject).first;
      ((Graphic) kvp.Value.GetComponent<HierarchyReferences>().GetReference<Image>("Icon")).color = Def.GetUISprite((object) ((Component) kvp.Key).gameObject).second;
      WorldContainer myWorld = kvp.Key.GetMyWorld();
      kvp.Value.GetComponent<HierarchyReferences>().GetReference<Image>("WorldIcon").sprite = myWorld.IsModuleInterior ? Assets.GetSprite(HashedString.op_Implicit("icon_category_rocketry")) : Def.GetUISprite((object) ((Component) myWorld).GetComponent<ClusterGridEntity>()).first;
      ((Graphic) kvp.Value.GetComponent<HierarchyReferences>().GetReference<Image>("WorldIcon")).color = myWorld.IsModuleInterior ? Color.white : Def.GetUISprite((object) ((Component) myWorld).GetComponent<ClusterGridEntity>()).second;
      kvp.Value.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("Toggle").onClick = (System.Action) (() =>
      {
        this.sensor.SetChannel(kvp.Key);
        this.Refresh();
      });
      kvp.Value.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("Toggle").ChangeState(Object.op_Equality((Object) this.sensor.GetChannel(), (Object) kvp.Key) ? 1 : 0);
    }
  }
}
