// Decompiled with JetBrains decompiler
// Type: Cancellable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/Cancellable")]
public class Cancellable : KMonoBehaviour
{
  private static readonly EventSystem.IntraObjectHandler<Cancellable> OnCancelDelegate = new EventSystem.IntraObjectHandler<Cancellable>((Action<Cancellable, object>) ((component, data) => component.OnCancel(data)));

  protected virtual void OnPrefabInit() => this.Subscribe<Cancellable>(2127324410, Cancellable.OnCancelDelegate);

  protected virtual void OnCancel(object data) => TracesExtesions.DeleteObject((Component) this);
}
