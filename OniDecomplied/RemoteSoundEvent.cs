// Decompiled with JetBrains decompiler
// Type: RemoteSoundEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using System;
using UnityEngine;

[Serializable]
public class RemoteSoundEvent : SoundEvent
{
  private const string STATE_PARAMETER = "State";

  public RemoteSoundEvent(string file_name, string sound_name, int frame, float min_interval)
    : base(file_name, sound_name, frame, true, false, min_interval, false)
  {
  }

  public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
  {
    Vector3 vector3 = TransformExtensions.GetPosition(behaviour.GetComponent<Transform>());
    vector3.z = 0.0f;
    if (SoundEvent.ObjectIsSelectedAndVisible(((Component) behaviour.controller).gameObject))
      vector3 = SoundEvent.AudioHighlightListenerPosition(vector3);
    Workable workable = behaviour.GetComponent<Worker>().workable;
    if (!Object.op_Inequality((Object) workable, (Object) null))
      return;
    Toggleable component = ((Component) workable).GetComponent<Toggleable>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    IToggleHandler handlerForWorker = component.GetToggleHandlerForWorker(behaviour.GetComponent<Worker>());
    float num = 1f;
    if (handlerForWorker != null && handlerForWorker.IsHandlerOn())
      num = 0.0f;
    if (!this.objectIsSelectedAndVisible && !SoundEvent.ShouldPlaySound(behaviour.controller, this.sound, this.soundHash, this.looping, this.isDynamic))
      return;
    EventInstance instance = SoundEvent.BeginOneShot(this.sound, vector3, SoundEvent.GetVolume(this.objectIsSelectedAndVisible));
    ((EventInstance) ref instance).setParameterByName("State", num, false);
    SoundEvent.EndOneShot(instance);
  }
}
