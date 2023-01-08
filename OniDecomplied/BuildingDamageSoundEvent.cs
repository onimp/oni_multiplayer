// Decompiled with JetBrains decompiler
// Type: BuildingDamageSoundEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[Serializable]
public class BuildingDamageSoundEvent : SoundEvent
{
  public BuildingDamageSoundEvent(string file_name, string sound_name, int frame)
    : base(file_name, sound_name, frame, false, false, (float) SoundEvent.IGNORE_INTERVAL, false)
  {
  }

  public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
  {
    Vector3 sound_pos = TransformExtensions.GetPosition(behaviour.GetComponent<Transform>());
    sound_pos.z = 0.0f;
    this.objectIsSelectedAndVisible = SoundEvent.ObjectIsSelectedAndVisible(((Component) behaviour.controller).gameObject);
    if (this.objectIsSelectedAndVisible)
      sound_pos = SoundEvent.AudioHighlightListenerPosition(sound_pos);
    Worker component1 = behaviour.GetComponent<Worker>();
    if (Object.op_Equality((Object) component1, (Object) null))
    {
      string sound = GlobalAssets.GetSound("Building_Dmg_Metal");
      if (this.objectIsSelectedAndVisible || SoundEvent.ShouldPlaySound(behaviour.controller, sound, this.looping, this.isDynamic))
      {
        SoundEvent.PlayOneShot(this.sound, sound_pos, SoundEvent.GetVolume(this.objectIsSelectedAndVisible));
        return;
      }
    }
    Workable workable = component1.workable;
    if (!Object.op_Inequality((Object) workable, (Object) null))
      return;
    Building component2 = ((Component) workable).GetComponent<Building>();
    if (!Object.op_Inequality((Object) component2, (Object) null))
      return;
    string sound1 = GlobalAssets.GetSound(StringFormatter.Combine(this.name, "_", component2.Def.AudioCategory)) ?? GlobalAssets.GetSound("Building_Dmg_Metal");
    if (sound1 == null || !this.objectIsSelectedAndVisible && !SoundEvent.ShouldPlaySound(behaviour.controller, sound1, this.looping, this.isDynamic))
      return;
    SoundEvent.PlayOneShot(sound1, sound_pos, SoundEvent.GetVolume(this.objectIsSelectedAndVisible));
  }
}
