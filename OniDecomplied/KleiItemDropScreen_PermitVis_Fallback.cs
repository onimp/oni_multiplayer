// Decompiled with JetBrains decompiler
// Type: KleiItemDropScreen_PermitVis_Fallback
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using UnityEngine;
using UnityEngine.UI;

public class KleiItemDropScreen_PermitVis_Fallback : 
  KMonoBehaviour,
  IKleiItemDropScreen_PermitVis_Target
{
  [SerializeField]
  private Image sprite;

  public void ConfigureWith(PermitResource permit, PermitPresentationInfo permitPresInfo) => this.sprite.sprite = permitPresInfo.sprite;
}
