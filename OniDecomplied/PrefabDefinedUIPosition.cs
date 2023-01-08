// Decompiled with JetBrains decompiler
// Type: PrefabDefinedUIPosition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class PrefabDefinedUIPosition
{
  private Option<Vector2> position;

  public void SetOn(GameObject gameObject)
  {
    if (this.position.HasValue)
      Util.rectTransform(gameObject).anchoredPosition = this.position.Value;
    else
      this.position = (Option<Vector2>) Util.rectTransform(gameObject).anchoredPosition;
  }

  public void SetOn(Component component)
  {
    if (this.position.HasValue)
      Util.rectTransform(component).anchoredPosition = this.position.Value;
    else
      this.position = (Option<Vector2>) Util.rectTransform(component).anchoredPosition;
  }
}
