// Decompiled with JetBrains decompiler
// Type: KCanvasScaler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/KCanvasScaler")]
public class KCanvasScaler : KMonoBehaviour
{
  [MyCmpReq]
  private CanvasScaler canvasScaler;
  public static string UIScalePrefKey = "UIScalePref";
  private float userScale = 1f;
  [Range(0.75f, 2f)]
  private KCanvasScaler.ScaleStep[] scaleSteps = new KCanvasScaler.ScaleStep[3]
  {
    new KCanvasScaler.ScaleStep(720f, 0.86f),
    new KCanvasScaler.ScaleStep(1080f, 1f),
    new KCanvasScaler.ScaleStep(2160f, 1.33f)
  };

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    if (KPlayerPrefs.HasKey(KCanvasScaler.UIScalePrefKey))
      this.SetUserScale(KPlayerPrefs.GetFloat(KCanvasScaler.UIScalePrefKey) / 100f);
    else
      this.SetUserScale(1f);
    ScreenResize.Instance.OnResize += new System.Action(this.OnResize);
  }

  private void OnResize() => this.SetUserScale(this.userScale);

  public void SetUserScale(float scale)
  {
    if (Object.op_Equality((Object) this.canvasScaler, (Object) null))
      this.canvasScaler = ((Component) this).GetComponent<CanvasScaler>();
    this.userScale = scale;
    this.canvasScaler.scaleFactor = this.GetCanvasScale();
  }

  public float GetUserScale() => this.userScale;

  public float GetCanvasScale() => this.userScale * this.ScreenRelativeScale();

  private float ScreenRelativeScale()
  {
    double dpi = (double) Screen.dpi;
    Camera camera = Camera.main;
    if (Object.op_Equality((Object) camera, (Object) null))
      camera = Object.FindObjectOfType<Camera>();
    Object.op_Inequality((Object) camera, (Object) null);
    if ((double) Screen.height <= (double) this.scaleSteps[0].maxRes_y || (double) Screen.width / (double) Screen.height < 1.6777777671813965)
      return this.scaleSteps[0].scale;
    if ((double) Screen.height > (double) this.scaleSteps[this.scaleSteps.Length - 1].maxRes_y)
      return this.scaleSteps[this.scaleSteps.Length - 1].scale;
    for (int index = 0; index < this.scaleSteps.Length; ++index)
    {
      if ((double) Screen.height > (double) this.scaleSteps[index].maxRes_y && (double) Screen.height <= (double) this.scaleSteps[index + 1].maxRes_y)
      {
        float num = (float) (((double) Screen.height - (double) this.scaleSteps[index].maxRes_y) / ((double) this.scaleSteps[index + 1].maxRes_y - (double) this.scaleSteps[index].maxRes_y));
        return Mathf.Lerp(this.scaleSteps[index].scale, this.scaleSteps[index + 1].scale, num);
      }
    }
    return 1f;
  }

  [Serializable]
  public struct ScaleStep
  {
    public float scale;
    public float maxRes_y;

    public ScaleStep(float maxRes_y, float scale)
    {
      this.maxRes_y = maxRes_y;
      this.scale = scale;
    }
  }
}
