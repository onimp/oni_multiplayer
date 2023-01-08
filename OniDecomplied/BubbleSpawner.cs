// Decompiled with JetBrains decompiler
// Type: BubbleSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/BubbleSpawner")]
public class BubbleSpawner : KMonoBehaviour
{
  public SimHashes element;
  public float emitMass;
  public float emitVariance;
  public Vector3 emitOffset = Vector3.zero;
  public Vector2 initialVelocity;
  [MyCmpGet]
  private Storage storage;
  private static readonly EventSystem.IntraObjectHandler<BubbleSpawner> OnStorageChangedDelegate = new EventSystem.IntraObjectHandler<BubbleSpawner>((Action<BubbleSpawner, object>) ((component, data) => component.OnStorageChanged(data)));

  protected virtual void OnSpawn()
  {
    this.emitMass += (Random.value - 0.5f) * this.emitVariance * this.emitMass;
    base.OnSpawn();
    this.Subscribe<BubbleSpawner>(-1697596308, BubbleSpawner.OnStorageChangedDelegate);
  }

  private void OnStorageChanged(object data)
  {
    GameObject first = this.storage.FindFirst(ElementLoader.FindElementByHash(this.element).tag);
    if (Object.op_Equality((Object) first, (Object) null))
      return;
    PrimaryElement component = first.GetComponent<PrimaryElement>();
    if ((double) component.Mass < (double) this.emitMass)
      return;
    first.GetComponent<PrimaryElement>().Mass -= this.emitMass;
    BubbleManager.instance.SpawnBubble(Vector2.op_Implicit(TransformExtensions.GetPosition(this.transform)), this.initialVelocity, component.ElementID, this.emitMass, component.Temperature);
  }
}
