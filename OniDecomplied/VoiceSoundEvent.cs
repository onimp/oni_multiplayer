// Decompiled with JetBrains decompiler
// Type: VoiceSoundEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using Klei.AI;
using UnityEngine;

public class VoiceSoundEvent : SoundEvent
{
  public static float locomotionSoundProb = 50f;
  public float timeLastSpoke;
  public float intervalBetweenSpeaking = 10f;

  public VoiceSoundEvent(string file_name, string sound_name, int frame, bool is_looping)
    : base(file_name, sound_name, frame, false, is_looping, (float) SoundEvent.IGNORE_INTERVAL, true)
  {
    this.noiseValues = SoundEventVolumeCache.instance.GetVolume(nameof (VoiceSoundEvent), sound_name);
  }

  public override void OnPlay(AnimEventManager.EventPlayerData behaviour) => VoiceSoundEvent.PlayVoice(this.name, behaviour.controller, this.intervalBetweenSpeaking, this.looping);

  public static EventInstance PlayVoice(
    string name,
    KBatchedAnimController controller,
    float interval_between_speaking,
    bool looping,
    bool objectIsSelectedAndVisible = false)
  {
    EventInstance instance = new EventInstance();
    MinionIdentity component1 = ((Component) controller).GetComponent<MinionIdentity>();
    if (Object.op_Equality((Object) component1, (Object) null) || name.Contains("state") && (double) Time.time - (double) component1.timeLastSpoke < (double) interval_between_speaking)
      return instance;
    if (name.Contains(":"))
    {
      float num = float.Parse(name.Split(':')[1]);
      if ((double) Random.Range(0, 100) > (double) num)
        return instance;
    }
    Worker component2 = ((Component) controller).GetComponent<Worker>();
    string assetName = VoiceSoundEvent.GetAssetName(name, (Component) component2);
    StaminaMonitor.Instance smi = ((Component) component2).GetSMI<StaminaMonitor.Instance>();
    if (!name.Contains("sleep_") && smi != null && smi.IsSleeping())
      return instance;
    Vector3 vector3 = TransformExtensions.GetPosition(component2.transform);
    vector3.z = 0.0f;
    if (SoundEvent.ObjectIsSelectedAndVisible(((Component) controller).gameObject))
      vector3 = SoundEvent.AudioHighlightListenerPosition(vector3);
    string sound = GlobalAssets.GetSound(assetName, true);
    if (!SoundEvent.ShouldPlaySound(controller, sound, looping, false))
      return instance;
    if (sound != null)
    {
      if (looping)
      {
        LoopingSounds component3 = ((Component) controller).GetComponent<LoopingSounds>();
        if (Object.op_Equality((Object) component3, (Object) null))
          Debug.Log((object) (((Object) controller).name + " is missing LoopingSounds component. "));
        else if (!component3.StartSound(sound))
          DebugUtil.LogWarningArgs(new object[1]
          {
            (object) string.Format("SoundEvent has invalid sound [{0}] on behaviour [{1}]", (object) sound, (object) ((Object) controller).name)
          });
      }
      else
      {
        instance = SoundEvent.BeginOneShot(sound, vector3);
        if (sound.Contains("sleep_") && ((Component) controller).GetComponent<Traits>().HasTrait("Snorer"))
          ((EventInstance) ref instance).setParameterByName("snoring", 1f, false);
        SoundEvent.EndOneShot(instance);
        component1.timeLastSpoke = Time.time;
      }
    }
    else if (AudioDebug.Get().debugVoiceSounds)
      Debug.LogWarning((object) ("Missing voice sound: " + assetName));
    return instance;
  }

  private static string GetAssetName(string name, Component cmp)
  {
    string str1 = "F01";
    if (Object.op_Inequality((Object) cmp, (Object) null))
    {
      MinionIdentity component = cmp.GetComponent<MinionIdentity>();
      if (Object.op_Inequality((Object) component, (Object) null))
        str1 = component.GetVoiceId();
    }
    string str2 = name;
    if (name.Contains(":"))
      str2 = name.Split(':')[0];
    return StringFormatter.Combine("DupVoc_", str1, "_", str2);
  }

  public override void Stop(AnimEventManager.EventPlayerData behaviour)
  {
    if (!this.looping)
      return;
    LoopingSounds component = behaviour.GetComponent<LoopingSounds>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    string sound = GlobalAssets.GetSound(VoiceSoundEvent.GetAssetName(this.name, (Component) component), true);
    component.StopSound(sound);
  }
}
