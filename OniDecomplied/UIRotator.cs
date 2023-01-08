// Decompiled with JetBrains decompiler
// Type: UIRotator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/prefabs/UIRotator")]
public class UIRotator : KMonoBehaviour
{
  public float minRotationSpeed = 1f;
  public float maxRotationSpeed = 1f;
  public float rotationSpeed = 1f;

  protected virtual void OnPrefabInit() => this.rotationSpeed = Random.Range(this.minRotationSpeed, this.maxRotationSpeed);

  private void Update() => ((Transform) ((Component) this).GetComponent<RectTransform>()).Rotate(0.0f, 0.0f, this.rotationSpeed * Time.unscaledDeltaTime);
}
