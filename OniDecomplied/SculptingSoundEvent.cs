// Decompiled with JetBrains decompiler
// Type: SculptingSoundEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using System;
using UnityEngine;

public class SculptingSoundEvent : SoundEvent
{
  private const int COUNTER_MODULUS_INVALID = -2147483648;
  private const int COUNTER_MODULUS_CLEAR = -1;
  private int counterModulus = int.MinValue;

  private static string BaseSoundName(string sound_name)
  {
    int length = sound_name.IndexOf(":");
    return length > 0 ? sound_name.Substring(0, length) : sound_name;
  }

  public SculptingSoundEvent(
    string file_name,
    string sound_name,
    int frame,
    bool do_load,
    bool is_looping,
    float min_interval,
    bool is_dynamic)
    : base(file_name, SculptingSoundEvent.BaseSoundName(sound_name), frame, do_load, is_looping, min_interval, is_dynamic)
  {
    if (sound_name.Contains(":"))
    {
      string[] strArray = sound_name.Split(':');
      if (strArray.Length != 2)
        DebugUtil.LogErrorArgs(new object[3]
        {
          (object) "Invalid CountedSoundEvent parameter for",
          (object) (file_name + "." + sound_name + "." + frame.ToString() + ":"),
          (object) ("'" + sound_name + "'")
        });
      for (int index = 1; index < strArray.Length; ++index)
        this.ParseParameter(strArray[index]);
    }
    else
      DebugUtil.LogErrorArgs(new object[3]
      {
        (object) "CountedSoundEvent for",
        (object) (file_name + "." + sound_name + "." + frame.ToString()),
        (object) (" - Must specify max number of steps on event: '" + sound_name + "'")
      });
  }

  public override void OnPlay(AnimEventManager.EventPlayerData behaviour)
  {
    if (string.IsNullOrEmpty(this.sound))
      return;
    GameObject gameObject = ((Component) behaviour.controller).gameObject;
    this.objectIsSelectedAndVisible = SoundEvent.ObjectIsSelectedAndVisible(gameObject);
    if (!this.objectIsSelectedAndVisible && !SoundEvent.ShouldPlaySound(behaviour.controller, this.sound, this.soundHash, this.looping, this.isDynamic))
      return;
    int num1 = -1;
    if (this.counterModulus >= -1)
    {
      HandleVector<int>.Handle h = GameComps.WhiteBoards.GetHandle(gameObject);
      if (!h.IsValid())
        h = GameComps.WhiteBoards.Add(gameObject);
      num1 = GameComps.WhiteBoards.HasValue(h, this.soundHash) ? (int) GameComps.WhiteBoards.GetValue(h, this.soundHash) : 0;
      int num2 = this.counterModulus == -1 ? 0 : (num1 + 1) % this.counterModulus;
      GameComps.WhiteBoards.SetValue(h, this.soundHash, (object) num2);
    }
    Vector3 vector3 = TransformExtensions.GetPosition(behaviour.GetComponent<Transform>());
    float volume = 1f;
    if (this.objectIsSelectedAndVisible)
    {
      vector3 = SoundEvent.AudioHighlightListenerPosition(vector3);
      volume = SoundEvent.GetVolume(this.objectIsSelectedAndVisible);
    }
    else
      vector3.z = 0.0f;
    string sound = GlobalAssets.GetSound("Hammer_sculpture");
    Worker component1 = behaviour.GetComponent<Worker>();
    if (Object.op_Inequality((Object) component1, (Object) null))
    {
      Workable workable = component1.workable;
      if (Object.op_Inequality((Object) workable, (Object) null))
      {
        Building component2 = ((Component) workable).GetComponent<Building>();
        if (Object.op_Inequality((Object) component2, (Object) null))
        {
          switch (((Object) component2.Def).name)
          {
            case "MetalSculpture":
              sound = GlobalAssets.GetSound("Hammer_sculpture_metal");
              break;
            case "MarbleSculpture":
              sound = GlobalAssets.GetSound("Hammer_sculpture_marble");
              break;
          }
        }
      }
    }
    EventInstance instance = SoundEvent.BeginOneShot(sound, vector3, volume);
    if (!((EventInstance) ref instance).isValid())
      return;
    if (num1 >= 0)
      ((EventInstance) ref instance).setParameterByName("eventCount", (float) num1, false);
    SoundEvent.EndOneShot(instance);
  }

  private void ParseParameter(string param)
  {
    this.counterModulus = int.Parse(param);
    if (this.counterModulus != -1 && this.counterModulus < 2)
      throw new ArgumentException("CountedSoundEvent modulus must be 2 or larger");
  }
}
