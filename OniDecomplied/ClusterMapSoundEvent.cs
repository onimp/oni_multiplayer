// Decompiled with JetBrains decompiler
// Type: ClusterMapSoundEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using UnityEngine;

public class ClusterMapSoundEvent : SoundEvent
{
  private static string X_POSITION_PARAMETER = "Starmap_Position_X";
  private static string Y_POSITION_PARAMETER = "Starmap_Position_Y";
  private static string ZOOM_PARAMETER = "Starmap_Zoom_Percentage";

  public ClusterMapSoundEvent(string file_name, string sound_name, int frame, bool looping)
    : base(file_name, sound_name, frame, true, looping, (float) SoundEvent.IGNORE_INTERVAL, false)
  {
  }

  public override void OnPlay(AnimEventManager.EventPlayerData behaviour)
  {
    if (!ClusterMapScreen.Instance.IsActive())
      return;
    this.PlaySound(behaviour);
  }

  public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
  {
    if (this.looping)
    {
      LoopingSounds component = behaviour.GetComponent<LoopingSounds>();
      if (Object.op_Equality((Object) component, (Object) null))
      {
        Debug.Log((object) (behaviour.name + " (Cluster Map Object) is missing LoopingSounds component."));
      }
      else
      {
        if (component.StartSound(this.sound, enable_culling: false, enable_camera_scaled_position: false))
          return;
        DebugUtil.LogWarningArgs(new object[1]
        {
          (object) string.Format("SoundEvent has invalid sound [{0}] on behaviour [{1}]", (object) this.sound, (object) behaviour.name)
        });
      }
    }
    else
    {
      EventInstance eventInstance = KFMOD.BeginOneShot(this.sound, Vector3.zero, 1f);
      ((EventInstance) ref eventInstance).setParameterByName(ClusterMapSoundEvent.X_POSITION_PARAMETER, TransformExtensions.GetPosition(((Component) behaviour.controller).transform).x / (float) Screen.width, false);
      ((EventInstance) ref eventInstance).setParameterByName(ClusterMapSoundEvent.Y_POSITION_PARAMETER, TransformExtensions.GetPosition(((Component) behaviour.controller).transform).y / (float) Screen.height, false);
      ((EventInstance) ref eventInstance).setParameterByName(ClusterMapSoundEvent.ZOOM_PARAMETER, ClusterMapScreen.Instance.CurrentZoomPercentage(), false);
      KFMOD.EndOneShot(eventInstance);
    }
  }

  public override void Stop(AnimEventManager.EventPlayerData behaviour)
  {
    if (!this.looping)
      return;
    LoopingSounds component = behaviour.GetComponent<LoopingSounds>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.StopSound(this.sound);
  }
}
