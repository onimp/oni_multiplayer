// Decompiled with JetBrains decompiler
// Type: PlanCategoryNotifications
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class PlanCategoryNotifications : MonoBehaviour
{
  public Image AttentionImage;

  public void ToggleAttention(bool active)
  {
    if (!Object.op_Implicit((Object) this.AttentionImage))
      return;
    ((Component) this.AttentionImage).gameObject.SetActive(active);
  }
}
