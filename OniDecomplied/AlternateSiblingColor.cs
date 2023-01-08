// Decompiled with JetBrains decompiler
// Type: AlternateSiblingColor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class AlternateSiblingColor : KMonoBehaviour
{
  public Color evenColor;
  public Color oddColor;
  public Image image;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.RefreshColor(this.transform.GetSiblingIndex() % 2 == 0);
  }

  private void RefreshColor(bool evenIndex)
  {
    if (Object.op_Equality((Object) this.image, (Object) null))
      return;
    ((Graphic) this.image).color = evenIndex ? this.evenColor : this.oddColor;
  }
}
