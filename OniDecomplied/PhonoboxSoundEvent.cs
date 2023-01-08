// Decompiled with JetBrains decompiler
// Type: PhonoboxSoundEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using FMODUnity;
using System;
using UnityEngine;

public class PhonoboxSoundEvent : SoundEvent
{
  private const string SOUND_PARAM_SONG = "jukeboxSong";
  private const string SOUND_PARAM_PITCH = "jukeboxPitch";
  private int song;
  private int pitch;

  public PhonoboxSoundEvent(string file_name, string sound_name, int frame, float min_interval)
    : base(file_name, sound_name, frame, true, true, min_interval, false)
  {
  }

  public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
  {
    Vector3 position = TransformExtensions.GetPosition(behaviour.GetComponent<Transform>());
    position.z = 0.0f;
    AudioDebug audioDebug = AudioDebug.Get();
    if (Object.op_Inequality((Object) audioDebug, (Object) null) && audioDebug.debugSoundEvents)
      Debug.Log((object) (behaviour.name + ", " + this.sound + ", " + this.frame.ToString() + ", " + position.ToString()));
    try
    {
      LoopingSounds component = behaviour.GetComponent<LoopingSounds>();
      if (Object.op_Equality((Object) component, (Object) null))
      {
        Debug.Log((object) (behaviour.name + " is missing LoopingSounds component. "));
      }
      else
      {
        if (component.IsSoundPlaying(this.sound))
          return;
        if (component.StartSound(this.sound, behaviour, this.noiseValues, this.ignorePause))
        {
          EventDescription eventDescription = RuntimeManager.GetEventDescription(this.sound);
          PARAMETER_DESCRIPTION parameterDescription1;
          ((EventDescription) ref eventDescription).getParameterDescriptionByName("jukeboxSong", ref parameterDescription1);
          int maximum1 = (int) parameterDescription1.maximum;
          PARAMETER_DESCRIPTION parameterDescription2;
          ((EventDescription) ref eventDescription).getParameterDescriptionByName("jukeboxPitch", ref parameterDescription2);
          int maximum2 = (int) parameterDescription2.maximum;
          this.song = Random.Range(0, maximum1 + 1);
          this.pitch = Random.Range(0, maximum2 + 1);
          component.UpdateFirstParameter(this.sound, HashedString.op_Implicit("jukeboxSong"), (float) this.song);
          component.UpdateSecondParameter(this.sound, HashedString.op_Implicit("jukeboxPitch"), (float) this.pitch);
        }
        else
          DebugUtil.LogWarningArgs(new object[1]
          {
            (object) string.Format("SoundEvent has invalid sound [{0}] on behaviour [{1}]", (object) this.sound, (object) behaviour.name)
          });
      }
    }
    catch (Exception ex)
    {
      string message = string.Format("Error trying to trigger sound [{0}] in behaviour [{1}] [{2}]\n{3}" + this.sound != null ? this.sound.ToString() : "null", (object) behaviour.GetType().ToString(), (object) ex.Message, (object) ex.StackTrace);
      Debug.LogError((object) message);
      throw new ArgumentException(message, ex);
    }
  }
}
