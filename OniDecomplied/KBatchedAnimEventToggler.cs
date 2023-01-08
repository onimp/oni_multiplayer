// Decompiled with JetBrains decompiler
// Type: KBatchedAnimEventToggler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/KBatchedAnimEventToggler")]
public class KBatchedAnimEventToggler : KMonoBehaviour
{
  [SerializeField]
  public GameObject eventSource;
  [SerializeField]
  public string enableEvent;
  [SerializeField]
  public string disableEvent;
  [SerializeField]
  public List<KBatchedAnimEventToggler.Entry> entries;
  private AnimEventHandler animEventHandler;

  protected virtual void OnPrefabInit()
  {
    Vector3 position = TransformExtensions.GetPosition(this.eventSource.transform);
    position.z = Grid.GetLayerZ(Grid.SceneLayer.Front);
    int layer = LayerMask.NameToLayer("Default");
    foreach (KBatchedAnimEventToggler.Entry entry in this.entries)
    {
      TransformExtensions.SetPosition(((Component) entry.controller).transform, position);
      entry.controller.SetLayer(layer);
      ((Component) entry.controller).gameObject.SetActive(false);
    }
    int num1 = Hash.SDBMLower(this.enableEvent);
    int num2 = Hash.SDBMLower(this.disableEvent);
    this.Subscribe(this.eventSource, num1, new Action<object>(this.Enable));
    this.Subscribe(this.eventSource, num2, new Action<object>(this.Disable));
  }

  protected virtual void OnSpawn() => this.animEventHandler = ((Component) this).GetComponentInParent<AnimEventHandler>();

  private void Enable(object data)
  {
    this.StopAll();
    HashedString context = this.animEventHandler.GetContext();
    if (!((HashedString) ref context).IsValid)
      return;
    foreach (KBatchedAnimEventToggler.Entry entry in this.entries)
    {
      if (HashedString.op_Equality(entry.context, context))
      {
        ((Component) entry.controller).gameObject.SetActive(true);
        entry.controller.Play(HashedString.op_Implicit(entry.anim), (KAnim.PlayMode) 0);
      }
    }
  }

  private void Disable(object data) => this.StopAll();

  private void StopAll()
  {
    foreach (KBatchedAnimEventToggler.Entry entry in this.entries)
    {
      entry.controller.StopAndClear();
      ((Component) entry.controller).gameObject.SetActive(false);
    }
  }

  [Serializable]
  public struct Entry
  {
    public string anim;
    public HashedString context;
    public KBatchedAnimController controller;
  }
}
