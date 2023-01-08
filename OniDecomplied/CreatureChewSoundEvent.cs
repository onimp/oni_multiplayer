// Decompiled with JetBrains decompiler
// Type: CreatureChewSoundEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using UnityEngine;

public class CreatureChewSoundEvent : SoundEvent
{
  private static string DEFAULT_CHEW_SOUND = "Rock";
  private const string FMOD_PARAM_IS_BABY_ID = "isBaby";

  public CreatureChewSoundEvent(
    string file_name,
    string sound_name,
    int frame,
    float min_interval)
    : base(file_name, sound_name, frame, false, false, min_interval, true)
  {
  }

  public override void OnPlay(AnimEventManager.EventPlayerData behaviour)
  {
    string sound = GlobalAssets.GetSound(StringFormatter.Combine(this.name, "_", CreatureChewSoundEvent.GetChewSound(behaviour)));
    this.objectIsSelectedAndVisible = SoundEvent.ObjectIsSelectedAndVisible(((Component) behaviour.controller).gameObject);
    if (!this.objectIsSelectedAndVisible && !SoundEvent.ShouldPlaySound(behaviour.controller, sound, this.looping, this.isDynamic))
      return;
    Vector3 vector3 = TransformExtensions.GetPosition(behaviour.GetComponent<Transform>());
    vector3.z = 0.0f;
    if (this.objectIsSelectedAndVisible)
      vector3 = SoundEvent.AudioHighlightListenerPosition(vector3);
    EventInstance instance = SoundEvent.BeginOneShot(sound, vector3, SoundEvent.GetVolume(this.objectIsSelectedAndVisible));
    if (((Component) behaviour.controller).gameObject.GetDef<BabyMonitor.Def>() != null)
      ((EventInstance) ref instance).setParameterByName("isBaby", 1f, false);
    SoundEvent.EndOneShot(instance);
  }

  private static string GetChewSound(AnimEventManager.EventPlayerData behaviour)
  {
    string chewSound = CreatureChewSoundEvent.DEFAULT_CHEW_SOUND;
    EatStates.Instance smi = ((Component) behaviour.controller).GetSMI<EatStates.Instance>();
    if (smi != null)
    {
      Element latestMealElement = smi.GetLatestMealElement();
      if (latestMealElement != null)
      {
        string creatureChewSound = latestMealElement.substance.GetCreatureChewSound();
        if (!string.IsNullOrEmpty(creatureChewSound))
          chewSound = creatureChewSound;
      }
    }
    return chewSound;
  }
}
