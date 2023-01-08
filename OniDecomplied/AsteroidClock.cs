// Decompiled with JetBrains decompiler
// Type: AsteroidClock
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class AsteroidClock : MonoBehaviour
{
  public Transform rotationTransform;
  public Image NightOverlay;

  private void Awake() => this.UpdateOverlay();

  private void Start()
  {
  }

  private void Update()
  {
    if (!Object.op_Inequality((Object) GameClock.Instance, (Object) null))
      return;
    this.rotationTransform.rotation = Quaternion.Euler(0.0f, 0.0f, (float) (360.0 * -(double) GameClock.Instance.GetCurrentCycleAsPercentage()));
  }

  private void UpdateOverlay() => this.NightOverlay.fillAmount = 0.125f;
}
