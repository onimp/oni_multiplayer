// Decompiled with JetBrains decompiler
// Type: CodexImage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodexImage : CodexWidget<CodexImage>
{
  public Sprite sprite { get; set; }

  public Color color { get; set; }

  public string spriteName
  {
    set => this.sprite = Assets.GetSprite(HashedString.op_Implicit(value));
    get => "--> " + (Object.op_Equality((Object) this.sprite, (Object) null) ? "NULL" : ((object) this.sprite).ToString());
  }

  public string batchedAnimPrefabSourceID
  {
    set
    {
      GameObject prefab = Assets.GetPrefab(Tag.op_Implicit(value));
      KBatchedAnimController kbatchedAnimController = Object.op_Inequality((Object) prefab, (Object) null) ? prefab.GetComponent<KBatchedAnimController>() : (KBatchedAnimController) null;
      KAnimFile animFile = Object.op_Inequality((Object) kbatchedAnimController, (Object) null) ? kbatchedAnimController.AnimFiles[0] : (KAnimFile) null;
      this.sprite = Object.op_Inequality((Object) animFile, (Object) null) ? Def.GetUISpriteFromMultiObjectAnim(animFile) : (Sprite) null;
    }
    get => "--> " + (Object.op_Equality((Object) this.sprite, (Object) null) ? "NULL" : ((object) this.sprite).ToString());
  }

  public CodexImage() => this.color = Color.white;

  public CodexImage(int preferredWidth, int preferredHeight, Sprite sprite, Color color)
    : base(preferredWidth, preferredHeight)
  {
    this.sprite = sprite;
    this.color = color;
  }

  public CodexImage(int preferredWidth, int preferredHeight, Sprite sprite)
    : this(preferredWidth, preferredHeight, sprite, Color.white)
  {
  }

  public CodexImage(int preferredWidth, int preferredHeight, Tuple<Sprite, Color> coloredSprite)
    : this(preferredWidth, preferredHeight, coloredSprite.first, coloredSprite.second)
  {
  }

  public CodexImage(Tuple<Sprite, Color> coloredSprite)
    : this(-1, -1, coloredSprite)
  {
  }

  public void ConfigureImage(Image image)
  {
    image.sprite = this.sprite;
    ((Graphic) image).color = this.color;
  }

  public override void Configure(
    GameObject contentGameObject,
    Transform displayPane,
    Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
  {
    this.ConfigureImage(contentGameObject.GetComponent<Image>());
    this.ConfigurePreferredLayout(contentGameObject);
  }
}
