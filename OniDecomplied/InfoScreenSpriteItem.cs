// Decompiled with JetBrains decompiler
// Type: InfoScreenSpriteItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/InfoScreenSpriteItem")]
public class InfoScreenSpriteItem : KMonoBehaviour
{
  [SerializeField]
  private Image image;
  [SerializeField]
  private LayoutElement layout;

  public void SetSprite(Sprite sprite)
  {
    this.image.sprite = sprite;
    Rect rect = sprite.rect;
    double width = (double) ((Rect) ref rect).width;
    rect = sprite.rect;
    double height = (double) ((Rect) ref rect).height;
    this.layout.preferredWidth = this.layout.preferredHeight * (float) (width / height);
  }
}
