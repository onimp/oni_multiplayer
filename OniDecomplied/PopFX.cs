// Decompiled with JetBrains decompiler
// Type: PopFX
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/PopFX")]
public class PopFX : KMonoBehaviour
{
  private float Speed = 2f;
  private Sprite icon;
  private string text;
  private Transform targetTransform;
  private Vector3 offset;
  public Image IconDisplay;
  public LocText TextDisplay;
  public CanvasGroup canvasGroup;
  private Camera uiCamera;
  private float lifetime;
  private float lifeElapsed;
  private bool trackTarget;
  private Vector3 startPos;
  private bool isLive;
  private bool isActiveWorld;

  public void Recycle()
  {
    this.icon = (Sprite) null;
    this.text = "";
    this.targetTransform = (Transform) null;
    this.lifeElapsed = 0.0f;
    this.trackTarget = false;
    this.startPos = Vector3.zero;
    ((Graphic) this.IconDisplay).color = Color.white;
    ((Graphic) this.TextDisplay).color = Color.white;
    PopFXManager.Instance.RecycleFX(this);
    this.canvasGroup.alpha = 0.0f;
    ((Component) this).gameObject.SetActive(false);
    this.isLive = false;
    this.isActiveWorld = false;
    Game.Instance.Unsubscribe(1983128072, new Action<object>(this.OnActiveWorldChanged));
  }

  public void Spawn(
    Sprite Icon,
    string Text,
    Transform TargetTransform,
    Vector3 Offset,
    float LifeTime = 1.5f,
    bool TrackTarget = false)
  {
    this.icon = Icon;
    this.text = Text;
    this.targetTransform = TargetTransform;
    this.trackTarget = TrackTarget;
    this.lifetime = LifeTime;
    this.offset = Offset;
    if (Object.op_Inequality((Object) this.targetTransform, (Object) null))
    {
      this.startPos = TransformExtensions.GetPosition(this.targetTransform);
      int y;
      Grid.PosToXY(this.startPos, out int _, out y);
      if (y % 2 != 0)
        this.startPos.x += 0.5f;
    }
    ((TMP_Text) this.TextDisplay).text = this.text;
    this.IconDisplay.sprite = this.icon;
    this.canvasGroup.alpha = 1f;
    this.isLive = true;
    Game.Instance.Subscribe(1983128072, new Action<object>(this.OnActiveWorldChanged));
    this.SetWorldActive(ClusterManager.Instance.activeWorldId);
    this.Update();
  }

  private void OnActiveWorldChanged(object data)
  {
    Tuple<int, int> tuple = (Tuple<int, int>) data;
    if (!this.isLive)
      return;
    this.SetWorldActive(tuple.first);
  }

  private void SetWorldActive(int worldId)
  {
    int cell = Grid.PosToCell(!this.trackTarget || !Object.op_Inequality((Object) this.targetTransform, (Object) null) ? Vector3.op_Addition(this.startPos, this.offset) : this.targetTransform.position);
    this.isActiveWorld = !Grid.IsValidCell(cell) || (int) Grid.WorldIdx[cell] == worldId;
  }

  private void Update()
  {
    if (!this.isLive || !PopFXManager.Instance.Ready())
      return;
    this.lifeElapsed += Time.unscaledDeltaTime;
    if ((double) this.lifeElapsed >= (double) this.lifetime)
      this.Recycle();
    if (this.trackTarget && Object.op_Inequality((Object) this.targetTransform, (Object) null))
    {
      Vector3 screen = PopFXManager.Instance.WorldToScreen(Vector3.op_Addition(Vector3.op_Addition(TransformExtensions.GetPosition(this.targetTransform), this.offset), Vector3.op_Multiply(Vector3.op_Multiply(Vector3.up, this.lifeElapsed), this.Speed * this.lifeElapsed)));
      screen.z = 0.0f;
      Util.rectTransform(((Component) this).gameObject).anchoredPosition = Vector2.op_Implicit(screen);
    }
    else
    {
      Vector3 screen = PopFXManager.Instance.WorldToScreen(Vector3.op_Addition(Vector3.op_Addition(this.startPos, this.offset), Vector3.op_Multiply(Vector3.op_Multiply(Vector3.up, this.lifeElapsed), this.Speed * (this.lifeElapsed / 2f))));
      screen.z = 0.0f;
      Util.rectTransform(((Component) this).gameObject).anchoredPosition = Vector2.op_Implicit(screen);
    }
    this.canvasGroup.alpha = this.isActiveWorld ? (float) (1.5 * (((double) this.lifetime - (double) this.lifeElapsed) / (double) this.lifetime)) : 0.0f;
  }
}
