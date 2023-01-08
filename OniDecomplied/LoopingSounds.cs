// Decompiled with JetBrains decompiler
// Type: LoopingSounds
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/LoopingSounds")]
public class LoopingSounds : KMonoBehaviour
{
  private List<LoopingSounds.LoopingSoundEvent> loopingSounds = new List<LoopingSounds.LoopingSoundEvent>();
  private Dictionary<HashedString, float> lastTimePlayed = new Dictionary<HashedString, float>();
  [SerializeField]
  public bool updatePosition;
  public float vol = 1f;
  public bool objectIsSelectedAndVisible;
  public Vector3 sound_pos;

  public bool IsSoundPlaying(string path)
  {
    foreach (LoopingSounds.LoopingSoundEvent loopingSound in this.loopingSounds)
    {
      if (loopingSound.asset == path)
        return true;
    }
    return false;
  }

  public bool StartSound(
    string asset,
    AnimEventManager.EventPlayerData behaviour,
    EffectorValues noiseValues,
    bool ignore_pause = false,
    bool enable_camera_scaled_position = true)
  {
    if (asset == null || asset == "")
    {
      Debug.LogWarning((object) "Missing sound");
      return false;
    }
    if (!this.IsSoundPlaying(asset))
    {
      LoopingSounds.LoopingSoundEvent loopingSoundEvent = new LoopingSounds.LoopingSoundEvent()
      {
        asset = asset
      };
      this.objectIsSelectedAndVisible = SoundEvent.ObjectIsSelectedAndVisible(((Component) this).gameObject);
      if (this.objectIsSelectedAndVisible)
      {
        this.sound_pos = SoundEvent.AudioHighlightListenerPosition(TransformExtensions.GetPosition(this.transform));
        this.vol = SoundEvent.GetVolume(this.objectIsSelectedAndVisible);
      }
      else
      {
        this.sound_pos = TransformExtensions.GetPosition(behaviour.GetComponent<Transform>());
        this.sound_pos.z = 0.0f;
      }
      loopingSoundEvent.handle = LoopingSoundManager.Get().Add(asset, this.sound_pos, this.transform, !ignore_pause, enable_camera_scaled_position: enable_camera_scaled_position, vol: this.vol, objectIsSelectedAndVisible: this.objectIsSelectedAndVisible);
      this.loopingSounds.Add(loopingSoundEvent);
    }
    return true;
  }

  public bool StartSound(EventReference event_ref) => this.StartSound(KFMOD.GetEventReferencePath(event_ref));

  public bool StartSound(string asset)
  {
    if (Util.IsNullOrWhiteSpace(asset))
    {
      Debug.LogWarning((object) "Missing sound");
      return false;
    }
    if (!this.IsSoundPlaying(asset))
    {
      LoopingSounds.LoopingSoundEvent loopingSoundEvent = new LoopingSounds.LoopingSoundEvent()
      {
        asset = asset
      };
      this.objectIsSelectedAndVisible = SoundEvent.ObjectIsSelectedAndVisible(((Component) this).gameObject);
      if (this.objectIsSelectedAndVisible)
      {
        this.sound_pos = SoundEvent.AudioHighlightListenerPosition(TransformExtensions.GetPosition(this.transform));
        this.vol = SoundEvent.GetVolume(this.objectIsSelectedAndVisible);
      }
      else
      {
        this.sound_pos = TransformExtensions.GetPosition(this.transform);
        this.sound_pos.z = 0.0f;
      }
      loopingSoundEvent.handle = LoopingSoundManager.Get().Add(asset, this.sound_pos, this.transform, vol: this.vol, objectIsSelectedAndVisible: this.objectIsSelectedAndVisible);
      this.loopingSounds.Add(loopingSoundEvent);
    }
    return true;
  }

  public bool StartSound(
    string asset,
    bool pause_on_game_pause = true,
    bool enable_culling = true,
    bool enable_camera_scaled_position = true)
  {
    if (Util.IsNullOrWhiteSpace(asset))
    {
      Debug.LogWarning((object) "Missing sound");
      return false;
    }
    if (!this.IsSoundPlaying(asset))
    {
      LoopingSounds.LoopingSoundEvent loopingSoundEvent = new LoopingSounds.LoopingSoundEvent()
      {
        asset = asset
      };
      this.objectIsSelectedAndVisible = SoundEvent.ObjectIsSelectedAndVisible(((Component) this).gameObject);
      if (this.objectIsSelectedAndVisible)
      {
        this.sound_pos = SoundEvent.AudioHighlightListenerPosition(TransformExtensions.GetPosition(this.transform));
        this.vol = SoundEvent.GetVolume(this.objectIsSelectedAndVisible);
      }
      else
      {
        this.sound_pos = TransformExtensions.GetPosition(this.transform);
        this.sound_pos.z = 0.0f;
      }
      loopingSoundEvent.handle = LoopingSoundManager.Get().Add(asset, this.sound_pos, this.transform, pause_on_game_pause, enable_culling, enable_camera_scaled_position, this.vol, this.objectIsSelectedAndVisible);
      this.loopingSounds.Add(loopingSoundEvent);
    }
    return true;
  }

  public void UpdateVelocity(string asset, Vector2 value)
  {
    foreach (LoopingSounds.LoopingSoundEvent loopingSound in this.loopingSounds)
    {
      if (loopingSound.asset == asset)
      {
        LoopingSoundManager.Get().UpdateVelocity(loopingSound.handle, value);
        break;
      }
    }
  }

  public void UpdateFirstParameter(string asset, HashedString parameter, float value)
  {
    foreach (LoopingSounds.LoopingSoundEvent loopingSound in this.loopingSounds)
    {
      if (loopingSound.asset == asset)
      {
        LoopingSoundManager.Get().UpdateFirstParameter(loopingSound.handle, parameter, value);
        break;
      }
    }
  }

  public void UpdateSecondParameter(string asset, HashedString parameter, float value)
  {
    foreach (LoopingSounds.LoopingSoundEvent loopingSound in this.loopingSounds)
    {
      if (loopingSound.asset == asset)
      {
        LoopingSoundManager.Get().UpdateSecondParameter(loopingSound.handle, parameter, value);
        break;
      }
    }
  }

  private void StopSoundAtIndex(int i) => LoopingSoundManager.StopSound(this.loopingSounds[i].handle);

  public void StopSound(EventReference event_ref) => this.StopSound(KFMOD.GetEventReferencePath(event_ref));

  public void StopSound(string asset)
  {
    for (int index = 0; index < this.loopingSounds.Count; ++index)
    {
      if (this.loopingSounds[index].asset == asset)
      {
        this.StopSoundAtIndex(index);
        this.loopingSounds.RemoveAt(index);
        break;
      }
    }
  }

  public void PauseSound(string asset, bool paused)
  {
    for (int index = 0; index < this.loopingSounds.Count; ++index)
    {
      if (this.loopingSounds[index].asset == asset)
      {
        LoopingSoundManager.PauseSound(this.loopingSounds[index].handle, paused);
        break;
      }
    }
  }

  public void StopAllSounds()
  {
    for (int i = 0; i < this.loopingSounds.Count; ++i)
      this.StopSoundAtIndex(i);
    this.loopingSounds.Clear();
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    this.StopAllSounds();
  }

  public void SetParameter(EventReference event_ref, HashedString parameter, float value) => this.SetParameter(KFMOD.GetEventReferencePath(event_ref), parameter, value);

  public void SetParameter(string path, HashedString parameter, float value)
  {
    foreach (LoopingSounds.LoopingSoundEvent loopingSound in this.loopingSounds)
    {
      if (loopingSound.asset == path)
      {
        LoopingSoundManager.Get().UpdateFirstParameter(loopingSound.handle, parameter, value);
        break;
      }
    }
  }

  public void PlayEvent(GameSoundEvents.Event ev)
  {
    if (AudioDebug.Get().debugGameEventSounds)
      Debug.Log((object) ("GameSoundEvent: " + ev.Name.ToString()));
    List<AnimEvent> events = GameAudioSheets.Get().GetEvents(ev.Name);
    if (events == null)
      return;
    Vector2 vector2 = Vector2.op_Implicit(TransformExtensions.GetPosition(this.transform));
    for (int index = 0; index < events.Count && events[index] is SoundEvent soundEvent && soundEvent.sound != null; ++index)
    {
      if (CameraController.Instance.IsAudibleSound(Vector2.op_Implicit(vector2), HashedString.op_Implicit(soundEvent.sound)))
      {
        if (AudioDebug.Get().debugGameEventSounds)
          Debug.Log((object) ("GameSound: " + soundEvent.sound));
        float num = 0.0f;
        if (this.lastTimePlayed.TryGetValue(soundEvent.soundHash, out num))
        {
          if ((double) Time.time - (double) num > (double) soundEvent.minInterval)
            SoundEvent.PlayOneShot(soundEvent.sound, Vector2.op_Implicit(vector2));
        }
        else
          SoundEvent.PlayOneShot(soundEvent.sound, Vector2.op_Implicit(vector2));
        this.lastTimePlayed[soundEvent.soundHash] = Time.time;
      }
    }
  }

  public void UpdateObjectSelection(bool selected)
  {
    GameObject gameObject = ((Component) this).gameObject;
    if (selected && Object.op_Inequality((Object) gameObject, (Object) null) && CameraController.Instance.IsVisiblePos(gameObject.transform.position))
    {
      this.objectIsSelectedAndVisible = true;
      this.sound_pos = SoundEvent.AudioHighlightListenerPosition(this.sound_pos);
      this.vol = 1f;
    }
    else
    {
      this.objectIsSelectedAndVisible = false;
      this.sound_pos = TransformExtensions.GetPosition(this.transform);
      this.sound_pos.z = 0.0f;
      this.vol = 1f;
    }
    for (int index = 0; index < this.loopingSounds.Count; ++index)
      LoopingSoundManager.Get().UpdateObjectSelection(this.loopingSounds[index].handle, this.sound_pos, this.vol, this.objectIsSelectedAndVisible);
  }

  private struct LoopingSoundEvent
  {
    public string asset;
    public HandleVector<int>.Handle handle;
  }
}
