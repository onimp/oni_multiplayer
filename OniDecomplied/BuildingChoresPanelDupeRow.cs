// Decompiled with JetBrains decompiler
// Type: BuildingChoresPanelDupeRow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/BuildingChoresPanelDupeRow")]
public class BuildingChoresPanelDupeRow : KMonoBehaviour
{
  public Image icon;
  public LocText label;
  public ToolTip toolTip;
  private ChoreConsumer choreConsumer;
  public KButton button;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.button.onClick += new System.Action(this.OnClick);
  }

  public void Init(BuildingChoresPanel.DupeEntryData data)
  {
    this.choreConsumer = data.consumer;
    if (data.context.IsPotentialSuccess())
    {
      string newValue = Object.op_Equality((Object) data.context.chore.driver, (Object) data.consumer.choreDriver) ? DUPLICANTS.CHORES.PRECONDITIONS.CURRENT_ERRAND.text : string.Format(DUPLICANTS.CHORES.PRECONDITIONS.RANK_FORMAT.text, (object) data.rank);
      ((TMP_Text) this.label).text = DUPLICANTS.CHORES.PRECONDITIONS.SUCCESS_ROW.Replace("{Duplicant}", ((Object) data.consumer).name).Replace("{Rank}", newValue);
    }
    else
    {
      string str = data.context.chore.GetPreconditions()[data.context.failedPreconditionId].description;
      DebugUtil.Assert(str != null, "Chore requires description!", data.context.chore.GetPreconditions()[data.context.failedPreconditionId].id);
      if (Object.op_Inequality((Object) data.context.chore.driver, (Object) null))
        str = str.Replace("{Assignee}", ((Component) data.context.chore.driver).GetProperName());
      string newValue = str.Replace("{Selected}", data.context.chore.gameObject.GetProperName());
      ((TMP_Text) this.label).text = DUPLICANTS.CHORES.PRECONDITIONS.FAILURE_ROW.Replace("{Duplicant}", ((Object) data.consumer).name).Replace("{Reason}", newValue);
    }
    this.icon.sprite = JobsTableScreen.priorityInfo[data.personalPriority].sprite;
    this.toolTip.toolTip = BuildingChoresPanelDupeRow.TooltipForDupe(data.context, data.consumer, data.rank);
  }

  private void OnClick() => CameraController.Instance.SetTargetPos(Vector3.op_Addition(TransformExtensions.GetPosition(((Component) this.choreConsumer).gameObject.transform), Vector3.up), 10f, true);

  private static string TooltipForDupe(
    Chore.Precondition.Context context,
    ChoreConsumer choreConsumer,
    int rank)
  {
    bool flag = context.IsPotentialSuccess();
    string str1 = (string) (flag ? STRINGS.UI.DETAILTABS.BUILDING_CHORES.DUPE_TOOLTIP_SUCCEEDED : STRINGS.UI.DETAILTABS.BUILDING_CHORES.DUPE_TOOLTIP_FAILED);
    float num1 = 0.0f;
    int personalPriority = choreConsumer.GetPersonalPriority(context.chore.choreType);
    float num2 = num1 + (float) (personalPriority * 10);
    int priorityValue = context.chore.masterPriority.priority_value;
    float num3 = num2 + (float) priorityValue;
    float num4 = (float) context.priority / 10000f;
    float num5 = num3 + num4;
    string str2 = str1.Replace("{Description}", (string) (Object.op_Equality((Object) context.chore.driver, (Object) choreConsumer.choreDriver) ? STRINGS.UI.DETAILTABS.BUILDING_CHORES.DUPE_TOOLTIP_DESC_ACTIVE : STRINGS.UI.DETAILTABS.BUILDING_CHORES.DUPE_TOOLTIP_DESC_INACTIVE));
    string newValue1 = GameUtil.ChoreGroupsForChoreType(context.chore.choreType);
    string newValue2 = STRINGS.UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_NA.text;
    if (flag && context.chore.choreType.groups.Length != 0)
    {
      ChoreGroup group = context.chore.choreType.groups[0];
      for (int index = 1; index < context.chore.choreType.groups.Length; ++index)
      {
        if (choreConsumer.GetPersonalPriority(group) < choreConsumer.GetPersonalPriority(context.chore.choreType.groups[index]))
          group = context.chore.choreType.groups[index];
      }
      newValue2 = group.Name;
    }
    string str3 = str2.Replace("{Name}", ((Object) choreConsumer).name).Replace("{Errand}", GameUtil.GetChoreName(context.chore, context.data));
    return flag ? str3.Replace("{Rank}", rank.ToString()).Replace("{Groups}", newValue1).Replace("{BestGroup}", newValue2).Replace("{PersonalPriority}", JobsTableScreen.priorityInfo[personalPriority].name.text).Replace("{PersonalPriorityValue}", (personalPriority * 10).ToString()).Replace("{Building}", context.chore.gameObject.GetProperName()).Replace("{BuildingPriority}", priorityValue.ToString()).Replace("{TypePriority}", num4.ToString()).Replace("{TotalPriority}", num5.ToString()) : str3.Replace("{FailedPrecondition}", context.chore.GetPreconditions()[context.failedPreconditionId].description);
  }
}
