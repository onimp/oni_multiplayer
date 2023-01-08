// Decompiled with JetBrains decompiler
// Type: SimpleMassStatusItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SimpleMassStatusItem")]
public class SimpleMassStatusItem : KMonoBehaviour
{
  public string symbolPrefix = "";

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    ((Component) this).GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.OreMass, (object) ((Component) this).gameObject);
  }
}
