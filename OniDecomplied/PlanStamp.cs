// Decompiled with JetBrains decompiler
// Type: PlanStamp
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/PlanStamp")]
public class PlanStamp : KMonoBehaviour
{
  public PlanStamp.StampArt stampSprites;
  [SerializeField]
  private Image StampImage;
  [SerializeField]
  private Text StampText;

  public void SetStamp(Sprite sprite, string Text)
  {
    this.StampImage.sprite = sprite;
    this.StampText.text = Text.ToUpper();
  }

  [Serializable]
  public struct StampArt
  {
    public Sprite UnderConstruction;
    public Sprite NeedsResearch;
    public Sprite SelectResource;
    public Sprite NeedsRepair;
    public Sprite NeedsPower;
    public Sprite NeedsResource;
    public Sprite NeedsGasPipe;
    public Sprite NeedsLiquidPipe;
  }
}
