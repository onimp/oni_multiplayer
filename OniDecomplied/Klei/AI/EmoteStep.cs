// Decompiled with JetBrains decompiler
// Type: Klei.AI.EmoteStep
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace Klei.AI
{
  public class EmoteStep
  {
    public HashedString anim = HashedString.Invalid;
    public KAnim.PlayMode mode = (KAnim.PlayMode) 1;
    public float timeout = -1f;
    private HandleVector<EmoteStep.Callbacks> callbacks = new HandleVector<EmoteStep.Callbacks>(64);

    public int Id => ((HashedString) ref this.anim).HashValue;

    public HandleVector<EmoteStep.Callbacks>.Handle RegisterCallbacks(
      Action<GameObject> startedCb,
      Action<GameObject> finishedCb)
    {
      if (startedCb == null && finishedCb == null)
        return HandleVector<EmoteStep.Callbacks>.InvalidHandle;
      return this.callbacks.Add(new EmoteStep.Callbacks()
      {
        StartedCb = startedCb,
        FinishedCb = finishedCb
      });
    }

    public void UnregisterCallbacks(
      HandleVector<EmoteStep.Callbacks>.Handle callbackHandle)
    {
      this.callbacks.Release(callbackHandle);
    }

    public void UnregisterAllCallbacks() => this.callbacks = new HandleVector<EmoteStep.Callbacks>(64);

    public void OnStepStarted(
      HandleVector<EmoteStep.Callbacks>.Handle callbackHandle,
      GameObject parameter)
    {
      if (HandleVector<EmoteStep.Callbacks>.Handle.op_Equality(callbackHandle, HandleVector<EmoteStep.Callbacks>.Handle.InvalidHandle))
        return;
      EmoteStep.Callbacks callbacks = this.callbacks.GetItem(callbackHandle);
      if (callbacks.StartedCb == null)
        return;
      callbacks.StartedCb(parameter);
    }

    public void OnStepFinished(
      HandleVector<EmoteStep.Callbacks>.Handle callbackHandle,
      GameObject parameter)
    {
      if (HandleVector<EmoteStep.Callbacks>.Handle.op_Equality(callbackHandle, HandleVector<EmoteStep.Callbacks>.Handle.InvalidHandle))
        return;
      EmoteStep.Callbacks callbacks = this.callbacks.GetItem(callbackHandle);
      if (callbacks.FinishedCb == null)
        return;
      callbacks.FinishedCb(parameter);
    }

    public struct Callbacks
    {
      public Action<GameObject> StartedCb;
      public Action<GameObject> FinishedCb;
    }
  }
}
