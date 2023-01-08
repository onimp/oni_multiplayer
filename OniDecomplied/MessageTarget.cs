// Decompiled with JetBrains decompiler
// Type: MessageTarget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using UnityEngine;

[SerializationConfig]
public class MessageTarget : ISaveLoadable
{
  [Serialize]
  private Ref<KPrefabID> prefabId = new Ref<KPrefabID>();
  [Serialize]
  private Vector3 position;
  [Serialize]
  private string name;

  public MessageTarget(KPrefabID prefab_id)
  {
    this.prefabId.Set(prefab_id);
    this.position = TransformExtensions.GetPosition(((KMonoBehaviour) prefab_id).transform);
    this.name = "Unknown";
    KSelectable component = ((Component) prefab_id).GetComponent<KSelectable>();
    if (Object.op_Inequality((Object) component, (Object) null))
      this.name = component.GetName();
    ((KMonoBehaviour) prefab_id).Subscribe(-1940207677, new Action<object>(this.OnAbsorbedBy));
  }

  public Vector3 GetPosition() => Object.op_Inequality((Object) this.prefabId.Get(), (Object) null) ? TransformExtensions.GetPosition(((KMonoBehaviour) this.prefabId.Get()).transform) : this.position;

  public KSelectable GetSelectable() => Object.op_Inequality((Object) this.prefabId.Get(), (Object) null) ? ((Component) ((KMonoBehaviour) this.prefabId.Get()).transform).GetComponent<KSelectable>() : (KSelectable) null;

  public string GetName() => this.name;

  private void OnAbsorbedBy(object data)
  {
    if (Object.op_Inequality((Object) this.prefabId.Get(), (Object) null))
      ((KMonoBehaviour) this.prefabId.Get()).Unsubscribe(-1940207677, new Action<object>(this.OnAbsorbedBy));
    KPrefabID component = ((GameObject) data).GetComponent<KPrefabID>();
    ((KMonoBehaviour) component).Subscribe(-1940207677, new Action<object>(this.OnAbsorbedBy));
    this.prefabId.Set(component);
  }

  public void OnCleanUp()
  {
    if (!Object.op_Inequality((Object) this.prefabId.Get(), (Object) null))
      return;
    ((KMonoBehaviour) this.prefabId.Get()).Unsubscribe(-1940207677, new Action<object>(this.OnAbsorbedBy));
    this.prefabId.Set((KPrefabID) null);
  }
}
