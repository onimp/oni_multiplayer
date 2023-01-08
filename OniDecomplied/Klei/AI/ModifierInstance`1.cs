// Decompiled with JetBrains decompiler
// Type: Klei.AI.ModifierInstance`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace Klei.AI
{
  public class ModifierInstance<ModifierType> : IStateMachineTarget
  {
    public ModifierType modifier;

    public GameObject gameObject { get; private set; }

    public ModifierInstance(GameObject game_object, ModifierType modifier)
    {
      this.gameObject = game_object;
      this.modifier = modifier;
    }

    public ComponentType GetComponent<ComponentType>() => this.gameObject.GetComponent<ComponentType>();

    public int Subscribe(int hash, Action<object> handler) => this.gameObject.GetComponent<KMonoBehaviour>().Subscribe(hash, handler);

    public void Unsubscribe(int hash, Action<object> handler) => this.gameObject.GetComponent<KMonoBehaviour>().Unsubscribe(hash, handler);

    public void Unsubscribe(int id) => this.gameObject.GetComponent<KMonoBehaviour>().Unsubscribe(id);

    public void Trigger(int hash, object data = null) => ((KMonoBehaviour) this.gameObject.GetComponent<KPrefabID>()).Trigger(hash, data);

    public Transform transform => this.gameObject.transform;

    public bool isNull => Object.op_Equality((Object) this.gameObject, (Object) null);

    public string name => ((Object) this.gameObject).name;

    public virtual void OnCleanUp()
    {
    }
  }
}
