// Decompiled with JetBrains decompiler
// Type: VictoryScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TMPro;
using UnityEngine;

public class VictoryScreen : KModalScreen
{
  [SerializeField]
  private KButton DismissButton;
  [SerializeField]
  private LocText descriptionText;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.Init();
  }

  private void Init()
  {
    if (!Object.op_Implicit((Object) this.DismissButton))
      return;
    this.DismissButton.onClick += (System.Action) (() => this.Dismiss());
  }

  private void Retire()
  {
    if (!RetireColonyUtility.SaveColonySummaryData())
      return;
    this.Show(false);
  }

  private void Dismiss() => this.Show(false);

  public void SetAchievements(string[] achievementIDs)
  {
    string str = "";
    for (int index = 0; index < achievementIDs.Length; ++index)
    {
      if (index > 0)
        str += "\n";
      str = str + GameUtil.ApplyBoldString(Db.Get().ColonyAchievements.Get(achievementIDs[index]).Name) + "\n" + Db.Get().ColonyAchievements.Get(achievementIDs[index]).description;
    }
    ((TMP_Text) this.descriptionText).text = str;
  }
}
