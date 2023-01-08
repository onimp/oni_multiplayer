// Decompiled with JetBrains decompiler
// Type: FlowOffsetRenderer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/FlowOffsetRenderer")]
public class FlowOffsetRenderer : KMonoBehaviour
{
  private float GasPhase0;
  private float GasPhase1;
  public float PhaseMultiplier;
  public float NoiseInfluence;
  public float NoiseScale;
  public float OffsetSpeed;
  public string OffsetTextureName;
  public string ParametersName;
  public Vector2 MinFlow0;
  public Vector2 MinFlow1;
  public Vector2 LiquidGasMask;
  [SerializeField]
  private Material FlowMaterial;
  [SerializeField]
  private bool forceUpdate;
  private TextureLerper FlowLerper;
  public RenderTexture[] OffsetTextures = new RenderTexture[2];
  private int OffsetIdx;
  private float CurrentTime;

  protected virtual void OnSpawn()
  {
    this.FlowMaterial = new Material(Shader.Find("Klei/Flow"));
    ScreenResize.Instance.OnResize += new System.Action(this.OnResize);
    this.OnResize();
    this.DoUpdate(0.1f);
  }

  private void OnResize()
  {
    for (int index = 0; index < this.OffsetTextures.Length; ++index)
    {
      if (Object.op_Inequality((Object) this.OffsetTextures[index], (Object) null))
        RenderTextureDestroyerExtensions.DestroyRenderTexture(this.OffsetTextures[index]);
      this.OffsetTextures[index] = new RenderTexture(Screen.width / 2, Screen.height / 2, 0, (RenderTextureFormat) 2);
      ((Texture) this.OffsetTextures[index]).filterMode = (FilterMode) 1;
      ((Object) this.OffsetTextures[index]).name = "FlowOffsetTexture";
    }
  }

  private void LateUpdate()
  {
    if (((double) Time.deltaTime <= 0.0 || (double) Time.timeScale <= 0.0) && !this.forceUpdate)
      return;
    float num = Time.deltaTime / Time.timeScale;
    this.DoUpdate((float) ((double) num * (double) Time.timeScale / 4.0 + (double) num * 0.5));
  }

  private void DoUpdate(float dt)
  {
    this.CurrentTime += dt;
    float num1 = this.CurrentTime * this.PhaseMultiplier;
    float num2 = num1 - (float) (int) num1;
    float num3 = num2 - (float) (int) num2;
    float num4 = 1f;
    if ((double) num3 <= (double) this.GasPhase0)
      num4 = 0.0f;
    this.GasPhase0 = num3;
    float num5 = 1f;
    float num6 = num2 + 0.5f - (float) (int) ((double) num2 + 0.5);
    if ((double) num6 <= (double) this.GasPhase1)
      num5 = 0.0f;
    this.GasPhase1 = num6;
    Shader.SetGlobalVector(this.ParametersName, new Vector4(this.GasPhase0, 0.0f, 0.0f, 0.0f));
    Shader.SetGlobalVector("_NoiseParameters", new Vector4(this.NoiseInfluence, this.NoiseScale, 0.0f, 0.0f));
    RenderTexture offsetTexture1 = this.OffsetTextures[this.OffsetIdx];
    this.OffsetIdx = (this.OffsetIdx + 1) % 2;
    RenderTexture offsetTexture2 = this.OffsetTextures[this.OffsetIdx];
    Material flowMaterial = this.FlowMaterial;
    flowMaterial.SetTexture("_PreviousOffsetTex", (Texture) offsetTexture1);
    flowMaterial.SetVector("_FlowParameters", new Vector4(Time.deltaTime * this.OffsetSpeed, num4, num5, 0.0f));
    flowMaterial.SetVector("_MinFlow", new Vector4(this.MinFlow0.x, this.MinFlow0.y, this.MinFlow1.x, this.MinFlow1.y));
    flowMaterial.SetVector("_VisibleArea", new Vector4(0.0f, 0.0f, (float) Grid.WidthInCells, (float) Grid.HeightInCells));
    flowMaterial.SetVector("_LiquidGasMask", new Vector4(this.LiquidGasMask.x, this.LiquidGasMask.y, 0.0f, 0.0f));
    Graphics.Blit((Texture) offsetTexture1, offsetTexture2, flowMaterial);
    Shader.SetGlobalTexture(this.OffsetTextureName, (Texture) offsetTexture2);
  }
}
