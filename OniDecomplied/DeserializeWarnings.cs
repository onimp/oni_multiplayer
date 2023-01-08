// Decompiled with JetBrains decompiler
// Type: DeserializeWarnings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/DeserializeWarnings")]
public class DeserializeWarnings : KMonoBehaviour
{
  public DeserializeWarnings.Warning BuildingTemeperatureIsZeroKelvin;
  public DeserializeWarnings.Warning PipeContentsTemperatureIsNan;
  public DeserializeWarnings.Warning PrimaryElementTemperatureIsNan;
  public DeserializeWarnings.Warning PrimaryElementHasNoElement;
  public static DeserializeWarnings Instance;

  public static void DestroyInstance() => DeserializeWarnings.Instance = (DeserializeWarnings) null;

  protected virtual void OnPrefabInit() => DeserializeWarnings.Instance = this;

  public struct Warning
  {
    private bool isSet;

    public void Warn(string message, GameObject obj = null)
    {
      if (this.isSet)
        return;
      Debug.LogWarning((object) message, (Object) obj);
      this.isSet = true;
    }
  }
}
