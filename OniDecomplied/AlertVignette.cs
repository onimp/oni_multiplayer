// Decompiled with JetBrains decompiler
// Type: AlertVignette
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class AlertVignette : KMonoBehaviour
{
  public Image image;
  public int worldID;

  protected virtual void OnSpawn() => base.OnSpawn();

  private void Update()
  {
    Color color = ((Graphic) this.image).color;
    if (Object.op_Equality((Object) ClusterManager.Instance.GetWorld(this.worldID), (Object) null))
    {
      ((Graphic) this.image).color = Color.clear;
    }
    else
    {
      if (ClusterManager.Instance.GetWorld(this.worldID).IsRedAlert())
      {
        if ((double) color.r != (double) Vignette.Instance.redAlertColor.r || (double) color.g != (double) Vignette.Instance.redAlertColor.g || (double) color.b != (double) Vignette.Instance.redAlertColor.b)
          color = Vignette.Instance.redAlertColor;
      }
      else if (ClusterManager.Instance.GetWorld(this.worldID).IsYellowAlert())
      {
        if ((double) color.r != (double) Vignette.Instance.yellowAlertColor.r || (double) color.g != (double) Vignette.Instance.yellowAlertColor.g || (double) color.b != (double) Vignette.Instance.yellowAlertColor.b)
          color = Vignette.Instance.yellowAlertColor;
      }
      else
        color = Color.clear;
      if (Color.op_Inequality(color, Color.clear))
        color.a = (float) (0.20000000298023224 + (0.5 + (double) Mathf.Sin((float) ((double) Time.unscaledTime * 4.0 - 1.0)) / 2.0) * 0.5);
      if (!Color.op_Inequality(((Graphic) this.image).color, color))
        return;
      ((Graphic) this.image).color = color;
    }
  }
}
