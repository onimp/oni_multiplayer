// Decompiled with JetBrains decompiler
// Type: EventInfoDataHelper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class EventInfoDataHelper
{
  public static EventInfoData GenerateStoryTraitData(
    string titleText,
    string descriptionText,
    string buttonText,
    string animFileName,
    EventInfoDataHelper.PopupType popupType,
    string buttonTooltip = null,
    GameObject[] minions = null,
    System.Action callback = null)
  {
    EventInfoData storyTraitData = new EventInfoData(titleText, descriptionText, HashedString.op_Implicit(animFileName));
    storyTraitData.minions = minions;
    if (popupType == EventInfoDataHelper.PopupType.BEGIN)
      storyTraitData.showCallback = (System.Action) (() => KFMOD.PlayUISound(GlobalAssets.GetSound("StoryTrait_Activation_Popup")));
    if (popupType == EventInfoDataHelper.PopupType.COMPLETE)
      storyTraitData.showCallback = (System.Action) (() => MusicManager.instance.PlaySong("Stinger_StoryTraitUnlock"));
    EventInfoData.Option option = storyTraitData.AddOption(buttonText);
    option.callback = callback;
    option.tooltip = buttonTooltip;
    return storyTraitData;
  }

  public enum PopupType
  {
    NONE = -1, // 0xFFFFFFFF
    BEGIN = 0,
    NORMAL = 1,
    COMPLETE = 2,
  }
}
