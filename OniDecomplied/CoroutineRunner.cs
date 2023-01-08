// Decompiled with JetBrains decompiler
// Type: CoroutineRunner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections;
using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
  public Promise Run(IEnumerator routine) => new Promise((Action<System.Action>) (resolve => this.StartCoroutine(this.RunRoutine(routine, resolve))));

  public (Promise, System.Action) RunCancellable(IEnumerator routine)
  {
    Promise promise = new Promise();
    Coroutine coroutine = this.StartCoroutine(this.RunRoutine(routine, new System.Action(promise.Resolve)));
    System.Action action = (System.Action) (() => this.StopCoroutine(coroutine));
    return (promise, action);
  }

  private IEnumerator RunRoutine(IEnumerator routine, System.Action completedCallback)
  {
    yield return (object) routine;
    completedCallback();
  }

  public static CoroutineRunner Create() => new GameObject(nameof (CoroutineRunner)).AddComponent<CoroutineRunner>();

  public static Promise RunOne(IEnumerator routine)
  {
    CoroutineRunner runner = CoroutineRunner.Create();
    return runner.Run(routine).Then((System.Action) (() => Object.Destroy((Object) ((Component) runner).gameObject)));
  }
}
