// Decompiled with JetBrains decompiler
// Type: DreamBubble
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class DreamBubble : KMonoBehaviour
{
  public KBatchedAnimController dreamBackgroundComponent;
  public KBatchedAnimController maskKanim;
  public KBatchedAnimController dreamBubbleBorderKanim;
  public KImage dreamContentComponent;
  private const string dreamBackgroundAnimationName = "dream_loop";
  private const string dreamMaskAnimationName = "dream_bubble_mask";
  private const string dreamBubbleBorderAnimationName = "dream_bubble_loop";
  private HashedString snapToPivotSymbol = new HashedString("snapto_pivot");
  private Dream _currentDream;
  private float _timePassedSinceDreamStarted;
  private Color _color = Color.white;
  private const float PI_2 = 6.28318548f;
  private const float HALF_PI = 1.57079637f;

  public bool IsVisible { private set; get; }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.dreamBackgroundComponent.SetSymbolVisiblity(KAnimHashedString.op_Implicit(this.snapToPivotSymbol), false);
    this.SetVisibility(false);
  }

  public void Tick(float dt)
  {
    if (this._currentDream == null || this._currentDream.Icons.Length == 0)
      return;
    double num1 = (double) this._timePassedSinceDreamStarted / (double) this._currentDream.secondPerImage;
    float num2 = (float) num1 - (float) Mathf.FloorToInt((float) num1);
    int index = (int) Mathf.Repeat((float) Mathf.FloorToInt((float) num1), (float) this._currentDream.Icons.Length);
    if (Object.op_Inequality((Object) ((Image) this.dreamContentComponent).sprite, (Object) this._currentDream.Icons[index]))
      ((Image) this.dreamContentComponent).sprite = this._currentDream.Icons[index];
    ((Transform) ((Graphic) this.dreamContentComponent).rectTransform).localScale = Vector3.op_Multiply(Vector3.one, num2);
    this._color.a = (float) (((double) Mathf.Sin((float) ((double) num2 * 6.2831854820251465 - 1.5707963705062866)) + 1.0) * 0.5);
    ((Graphic) this.dreamContentComponent).color = this._color;
    this._timePassedSinceDreamStarted += dt;
  }

  public void SetDream(Dream dream)
  {
    this._currentDream = dream;
    this.dreamBackgroundComponent.Stop();
    this.dreamBackgroundComponent.AnimFiles = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit(dream.BackgroundAnim))
    };
    ((Graphic) this.dreamContentComponent).color = this._color;
    ((Behaviour) this.dreamContentComponent).enabled = dream != null && dream.Icons != null && dream.Icons.Length != 0;
    this._timePassedSinceDreamStarted = 0.0f;
    this._color.a = 0.0f;
  }

  public void SetVisibility(bool visible)
  {
    this.IsVisible = visible;
    this.dreamBackgroundComponent.SetVisiblity(visible);
    ((Component) this.dreamContentComponent).gameObject.SetActive(visible);
    if (visible)
    {
      if (this._currentDream != null)
        this.dreamBackgroundComponent.Play(HashedString.op_Implicit("dream_loop"), (KAnim.PlayMode) 0);
      this.dreamBubbleBorderKanim.Play(HashedString.op_Implicit("dream_bubble_loop"), (KAnim.PlayMode) 0);
      this.maskKanim.Play(HashedString.op_Implicit("dream_bubble_mask"), (KAnim.PlayMode) 0);
    }
    else
    {
      this.dreamBackgroundComponent.Stop();
      this.maskKanim.Stop();
      this.dreamBubbleBorderKanim.Stop();
    }
  }

  public void StopDreaming()
  {
    this._currentDream = (Dream) null;
    this.SetVisibility(false);
  }
}
