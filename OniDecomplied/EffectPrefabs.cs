// Decompiled with JetBrains decompiler
// Type: EffectPrefabs
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class EffectPrefabs : MonoBehaviour
{
  public GameObject DreamBubble;
  public GameObject ThoughtBubble;
  public GameObject ThoughtBubbleConvo;
  public GameObject MeteorBackground;
  public GameObject SparkleStreakFX;
  public GameObject HappySingerFX;
  public GameObject HugFrenzyFX;
  public GameObject GameplayEventDisplay;
  public GameObject OpenTemporalTearBeam;

  public static EffectPrefabs Instance { get; private set; }

  private void Awake() => EffectPrefabs.Instance = this;
}
