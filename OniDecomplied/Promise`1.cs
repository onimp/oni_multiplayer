// Decompiled with JetBrains decompiler
// Type: Promise`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections;

public class Promise<T> : IEnumerator
{
  private Promise promise = new Promise();
  private T result;

  public Promise(Action<Action<T>> fn) => fn((Action<T>) (value => this.Resolve(value)));

  public Promise()
  {
  }

  public void EnsureResolved(T value)
  {
    this.result = value;
    this.promise.EnsureResolved();
  }

  public void Resolve(T value)
  {
    this.result = value;
    this.promise.Resolve();
  }

  public Promise<T> Then(Action<T> fn)
  {
    this.promise.Then((System.Action) (() => fn(this.result)));
    return this;
  }

  public Promise ThenWait(Func<Promise> fn) => this.promise.ThenWait(fn);

  public Promise<T> ThenWait(Func<Promise<T>> fn) => this.promise.ThenWait<T>(fn);

  object IEnumerator.Current => (object) null;

  bool IEnumerator.MoveNext() => !this.promise.IsResolved;

  void IEnumerator.Reset()
  {
  }
}
