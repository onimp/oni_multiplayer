// Decompiled with JetBrains decompiler
// Type: CreatureVariationSoundEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class CreatureVariationSoundEvent : SoundEvent
{
  public CreatureVariationSoundEvent(
    string file_name,
    string sound_name,
    int frame,
    bool do_load,
    bool is_looping,
    float min_interval,
    bool is_dynamic)
    : base(file_name, sound_name, frame, do_load, is_looping, min_interval, is_dynamic)
  {
  }

  public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
  {
    string sound1 = this.sound;
    CreatureBrain component = behaviour.GetComponent<CreatureBrain>();
    if (Object.op_Inequality((Object) component, (Object) null) && !string.IsNullOrEmpty(component.symbolPrefix))
    {
      string sound2 = GlobalAssets.GetSound(StringFormatter.Combine(component.symbolPrefix, this.name));
      if (!string.IsNullOrEmpty(sound2))
        sound1 = sound2;
    }
    this.PlaySound(behaviour, sound1);
  }
}
