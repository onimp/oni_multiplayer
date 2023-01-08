// Decompiled with JetBrains decompiler
// Type: Infrared
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class Infrared : MonoBehaviour
{
  private RenderTexture minionTexture;
  private RenderTexture cameraTexture;
  private Infrared.Mode mode;
  public static int temperatureParametersId;
  public static Infrared Instance;

  public static void DestroyInstance() => Infrared.Instance = (Infrared) null;

  private void Awake()
  {
    Infrared.temperatureParametersId = Shader.PropertyToID("_TemperatureParameters");
    Infrared.Instance = this;
    this.OnResize();
    this.UpdateState();
  }

  private void OnRenderImage(RenderTexture source, RenderTexture dest)
  {
    Graphics.Blit((Texture) source, this.minionTexture);
    Graphics.Blit((Texture) source, dest);
  }

  private void OnResize()
  {
    if (Object.op_Inequality((Object) this.minionTexture, (Object) null))
      RenderTextureDestroyerExtensions.DestroyRenderTexture(this.minionTexture);
    if (Object.op_Inequality((Object) this.cameraTexture, (Object) null))
      RenderTextureDestroyerExtensions.DestroyRenderTexture(this.cameraTexture);
    int num = 2;
    this.minionTexture = new RenderTexture(Screen.width / num, Screen.height / num, 0, (RenderTextureFormat) 0);
    this.cameraTexture = new RenderTexture(Screen.width / num, Screen.height / num, 0, (RenderTextureFormat) 0);
    ((Component) this).GetComponent<Camera>().targetTexture = this.cameraTexture;
  }

  public void SetMode(Infrared.Mode mode)
  {
    Vector4 zero;
    switch (mode)
    {
      case Infrared.Mode.Disabled:
        zero = Vector4.zero;
        break;
      case Infrared.Mode.Disease:
        // ISSUE: explicit constructor call
        ((Vector4) ref zero).\u002Ector(1f, 0.0f, 0.0f, 0.0f);
        GameComps.InfraredVisualizers.ClearOverlayColour();
        break;
      default:
        // ISSUE: explicit constructor call
        ((Vector4) ref zero).\u002Ector(1f, 0.0f, 0.0f, 0.0f);
        break;
    }
    Shader.SetGlobalVector("_ColouredOverlayParameters", zero);
    this.mode = mode;
    this.UpdateState();
  }

  private void UpdateState()
  {
    ((Component) this).gameObject.SetActive(this.mode != 0);
    if (!((Component) this).gameObject.activeSelf)
      return;
    this.Update();
  }

  private void Update()
  {
    switch (this.mode)
    {
      case Infrared.Mode.Infrared:
        GameComps.InfraredVisualizers.UpdateTemperature();
        break;
      case Infrared.Mode.Disease:
        GameComps.DiseaseContainers.UpdateOverlayColours();
        break;
    }
  }

  public enum Mode
  {
    Disabled,
    Infrared,
    Disease,
  }
}
