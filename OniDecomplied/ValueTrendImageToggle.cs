// Decompiled with JetBrains decompiler
// Type: ValueTrendImageToggle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using UnityEngine;
using UnityEngine.UI;

public class ValueTrendImageToggle : MonoBehaviour
{
  public Image targetImage;
  public Sprite Up_One;
  public Sprite Up_Two;
  public Sprite Up_Three;
  public Sprite Down_One;
  public Sprite Down_Two;
  public Sprite Down_Three;
  public Sprite Zero;

  public void SetValue(AmountInstance ainstance)
  {
    float delta = ainstance.GetDelta();
    Sprite sprite = (Sprite) null;
    if (ainstance.paused || (double) delta == 0.0)
    {
      ((Component) this.targetImage).gameObject.SetActive(false);
    }
    else
    {
      ((Component) this.targetImage).gameObject.SetActive(true);
      if ((double) delta <= -(double) ainstance.amount.visualDeltaThreshold * 2.0)
        sprite = this.Down_Three;
      else if ((double) delta <= -(double) ainstance.amount.visualDeltaThreshold)
        sprite = this.Down_Two;
      else if ((double) delta <= 0.0)
        sprite = this.Down_One;
      else if ((double) delta > (double) ainstance.amount.visualDeltaThreshold * 2.0)
        sprite = this.Up_Three;
      else if ((double) delta > (double) ainstance.amount.visualDeltaThreshold)
        sprite = this.Up_Two;
      else if ((double) delta > 0.0)
        sprite = this.Up_One;
    }
    this.targetImage.sprite = sprite;
  }
}
