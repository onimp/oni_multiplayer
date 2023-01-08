// Decompiled with JetBrains decompiler
// Type: CharacterOverlay
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/CharacterOverlay")]
public class CharacterOverlay : KMonoBehaviour
{
  public bool shouldShowName;
  private bool registered;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.Register();
  }

  public void Register()
  {
    if (this.registered)
      return;
    this.registered = true;
    NameDisplayScreen.Instance.AddNewEntry(((Component) this).gameObject);
  }
}
