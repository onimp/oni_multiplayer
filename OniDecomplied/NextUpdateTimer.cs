// Decompiled with JetBrains decompiler
// Type: NextUpdateTimer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using TMPro;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/NextUpdateTimer")]
public class NextUpdateTimer : KMonoBehaviour
{
  public LocText TimerText;
  public KBatchedAnimController UpdateAnimController;
  public KBatchedAnimController UpdateAnimMeterController;
  public float initialAnimScale;
  public System.DateTime nextReleaseDate;
  public System.DateTime currentReleaseDate;
  private string m_releaseTextOverride;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.initialAnimScale = this.UpdateAnimController.animScale;
  }

  protected virtual void OnCleanUp() => base.OnCleanUp();

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.RefreshReleaseTimes();
  }

  public void UpdateReleaseTimes(string lastUpdateTime, string nextUpdateTime, string textOverride)
  {
    if (!System.DateTime.TryParse(lastUpdateTime, out this.currentReleaseDate))
      Debug.LogWarning((object) ("Failed to parse last_update_time: " + lastUpdateTime));
    if (!System.DateTime.TryParse(nextUpdateTime, out this.nextReleaseDate))
      Debug.LogWarning((object) ("Failed to parse next_update_time: " + nextUpdateTime));
    this.m_releaseTextOverride = textOverride;
    this.RefreshReleaseTimes();
  }

  private void RefreshReleaseTimes()
  {
    TimeSpan timeSpan1 = this.nextReleaseDate - this.currentReleaseDate;
    TimeSpan timeSpan2 = this.nextReleaseDate - System.DateTime.UtcNow;
    TimeSpan timeSpan3 = System.DateTime.UtcNow - this.currentReleaseDate;
    string str1 = "4";
    string str2;
    if (!string.IsNullOrEmpty(this.m_releaseTextOverride))
      str2 = this.m_releaseTextOverride;
    else if (timeSpan2.TotalHours < 8.0)
    {
      str2 = (string) UI.DEVELOPMENTBUILDS.UPDATES.TWENTY_FOUR_HOURS;
      str1 = "4";
    }
    else if (timeSpan2.TotalDays < 1.0)
    {
      str2 = string.Format((string) UI.DEVELOPMENTBUILDS.UPDATES.FINAL_WEEK, (object) 1);
      str1 = "3";
    }
    else
    {
      int num1 = timeSpan2.Days % 7;
      int num2 = (timeSpan2.Days - num1) / 7;
      if (num2 <= 0)
      {
        str2 = string.Format((string) UI.DEVELOPMENTBUILDS.UPDATES.FINAL_WEEK, (object) num1);
        str1 = "2";
      }
      else
      {
        str2 = string.Format((string) UI.DEVELOPMENTBUILDS.UPDATES.BIGGER_TIMES, (object) num1, (object) num2);
        str1 = "1";
      }
    }
    ((TMP_Text) this.TimerText).text = str2;
    this.UpdateAnimController.Play(HashedString.op_Implicit(str1), (KAnim.PlayMode) 0);
    this.UpdateAnimMeterController.SetPositionPercent(Mathf.Clamp01((float) (timeSpan3.TotalSeconds / timeSpan1.TotalSeconds)));
  }
}
