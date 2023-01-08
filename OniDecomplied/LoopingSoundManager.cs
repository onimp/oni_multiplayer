// Decompiled with JetBrains decompiler
// Type: LoopingSoundManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD;
using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/LoopingSoundManager")]
public class LoopingSoundManager : KMonoBehaviour, IRenderEveryTick
{
  private static LoopingSoundManager instance;
  private bool GameIsPaused;
  private Dictionary<HashedString, LoopingSoundParameterUpdater> parameterUpdaters = new Dictionary<HashedString, LoopingSoundParameterUpdater>();
  private KCompactedVector<LoopingSoundManager.Sound> sounds = new KCompactedVector<LoopingSoundManager.Sound>(0);

  public static void DestroyInstance() => LoopingSoundManager.instance = (LoopingSoundManager) null;

  protected virtual void OnPrefabInit()
  {
    LoopingSoundManager.instance = this;
    this.CollectParameterUpdaters();
  }

  protected virtual void OnSpawn()
  {
    if (Object.op_Inequality((Object) SpeedControlScreen.Instance, (Object) null) && Object.op_Inequality((Object) Game.Instance, (Object) null))
      Game.Instance.Subscribe(-1788536802, new Action<object>(LoopingSoundManager.instance.OnPauseChanged));
    Game.Instance.Subscribe(1983128072, (Action<object>) (worlds => this.OnActiveWorldChanged()));
  }

  private void OnActiveWorldChanged() => this.StopAllSounds();

  private void CollectParameterUpdaters()
  {
    foreach (System.Type currentDomainType in App.GetCurrentDomainTypes())
    {
      if (!currentDomainType.IsAbstract)
      {
        bool flag = false;
        for (System.Type baseType = currentDomainType.BaseType; baseType != (System.Type) null; baseType = baseType.BaseType)
        {
          if (baseType == typeof (LoopingSoundParameterUpdater))
          {
            flag = true;
            break;
          }
        }
        if (flag)
        {
          LoopingSoundParameterUpdater instance = (LoopingSoundParameterUpdater) Activator.CreateInstance(currentDomainType);
          DebugUtil.Assert(!this.parameterUpdaters.ContainsKey(instance.parameter));
          this.parameterUpdaters[instance.parameter] = instance;
        }
      }
    }
  }

  public void UpdateFirstParameter(
    HandleVector<int>.Handle handle,
    HashedString parameter,
    float value)
  {
    LoopingSoundManager.Sound data = this.sounds.GetData(handle) with
    {
      firstParameterValue = value,
      firstParameter = parameter
    };
    if (data.IsPlaying)
    {
      ref EventInstance local = ref data.ev;
      SoundDescription soundDescription = this.GetSoundDescription(data.path);
      PARAMETER_ID parameterId = ((SoundDescription) ref soundDescription).GetParameterId(parameter);
      double num = (double) value;
      ((EventInstance) ref local).setParameterByID(parameterId, (float) num, false);
    }
    this.sounds.SetData(handle, data);
  }

  public void UpdateSecondParameter(
    HandleVector<int>.Handle handle,
    HashedString parameter,
    float value)
  {
    LoopingSoundManager.Sound data = this.sounds.GetData(handle) with
    {
      secondParameterValue = value,
      secondParameter = parameter
    };
    if (data.IsPlaying)
    {
      ref EventInstance local = ref data.ev;
      SoundDescription soundDescription = this.GetSoundDescription(data.path);
      PARAMETER_ID parameterId = ((SoundDescription) ref soundDescription).GetParameterId(parameter);
      double num = (double) value;
      ((EventInstance) ref local).setParameterByID(parameterId, (float) num, false);
    }
    this.sounds.SetData(handle, data);
  }

  public void UpdateObjectSelection(
    HandleVector<int>.Handle handle,
    Vector3 sound_pos,
    float vol,
    bool objectIsSelectedAndVisible)
  {
    LoopingSoundManager.Sound data = this.sounds.GetData(handle) with
    {
      pos = sound_pos,
      vol = vol,
      objectIsSelectedAndVisible = objectIsSelectedAndVisible
    };
    ATTRIBUTES_3D attributes3D = RuntimeUtils.To3DAttributes(sound_pos);
    if (data.IsPlaying)
    {
      ((EventInstance) ref data.ev).set3DAttributes(attributes3D);
      ((EventInstance) ref data.ev).setVolume(vol);
    }
    this.sounds.SetData(handle, data);
  }

  public void UpdateVelocity(HandleVector<int>.Handle handle, Vector2 velocity)
  {
    LoopingSoundManager.Sound data = this.sounds.GetData(handle) with
    {
      velocity = velocity
    };
    this.sounds.SetData(handle, data);
  }

  public void RenderEveryTick(float dt)
  {
    ListPool<LoopingSoundManager.Sound, LoopingSoundManager>.PooledList pooledList1 = ListPool<LoopingSoundManager.Sound, LoopingSoundManager>.Allocate();
    ListPool<int, LoopingSoundManager>.PooledList pooledList2 = ListPool<int, LoopingSoundManager>.Allocate();
    ListPool<int, LoopingSoundManager>.PooledList pooledList3 = ListPool<int, LoopingSoundManager>.Allocate();
    List<LoopingSoundManager.Sound> dataList = this.sounds.GetDataList();
    bool flag = (double) Time.timeScale == 0.0;
    SoundCuller soundCuller = CameraController.Instance.soundCuller;
    for (int index = 0; index < dataList.Count; ++index)
    {
      LoopingSoundManager.Sound sound = dataList[index];
      if (sound.objectIsSelectedAndVisible)
      {
        sound.pos = SoundEvent.AudioHighlightListenerPosition(TransformExtensions.GetPosition(sound.transform));
        sound.vol = 1f;
      }
      else if (Object.op_Inequality((Object) sound.transform, (Object) null))
      {
        sound.pos = TransformExtensions.GetPosition(sound.transform);
        sound.pos.z = 0.0f;
      }
      if (Object.op_Inequality((Object) sound.animController, (Object) null))
      {
        Vector3 offset = sound.animController.Offset;
        sound.pos.x += offset.x;
        sound.pos.y += offset.y;
      }
      int num = !sound.IsCullingEnabled || sound.ShouldCameraScalePosition && soundCuller.IsAudible(Vector2.op_Implicit(sound.pos), sound.falloffDistanceSq) ? 1 : (soundCuller.IsAudibleNoCameraScaling(Vector2.op_Implicit(sound.pos), sound.falloffDistanceSq) ? 1 : 0);
      bool isPlaying = sound.IsPlaying;
      if (num != 0)
      {
        ((List<LoopingSoundManager.Sound>) pooledList1).Add(sound);
        if (!isPlaying)
        {
          SoundDescription soundDescription = this.GetSoundDescription(sound.path);
          sound.ev = KFMOD.CreateInstance(soundDescription.path);
          dataList[index] = sound;
          ((List<int>) pooledList2).Add(index);
        }
      }
      else if (isPlaying)
        ((List<int>) pooledList3).Add(index);
    }
    foreach (int index in (List<int>) pooledList2)
    {
      LoopingSoundManager.Sound sound1 = dataList[index];
      SoundDescription soundDescription = this.GetSoundDescription(sound1.path);
      ((EventInstance) ref sound1.ev).setPaused(flag && sound1.ShouldPauseOnGamePaused);
      sound1.pos.z = 0.0f;
      Vector3 pos = sound1.pos;
      if (sound1.objectIsSelectedAndVisible)
      {
        sound1.pos = SoundEvent.AudioHighlightListenerPosition(TransformExtensions.GetPosition(sound1.transform));
        sound1.vol = 1f;
      }
      else if (Object.op_Inequality((Object) sound1.transform, (Object) null))
        sound1.pos = TransformExtensions.GetPosition(sound1.transform);
      ((EventInstance) ref sound1.ev).set3DAttributes(RuntimeUtils.To3DAttributes(pos));
      ((EventInstance) ref sound1.ev).setVolume(sound1.vol);
      ((EventInstance) ref sound1.ev).start();
      sound1.flags |= LoopingSoundManager.Sound.Flags.PLAYING;
      if (HashedString.op_Inequality(sound1.firstParameter, HashedString.Invalid))
        ((EventInstance) ref sound1.ev).setParameterByID(((SoundDescription) ref soundDescription).GetParameterId(sound1.firstParameter), sound1.firstParameterValue, false);
      if (HashedString.op_Inequality(sound1.secondParameter, HashedString.Invalid))
        ((EventInstance) ref sound1.ev).setParameterByID(((SoundDescription) ref soundDescription).GetParameterId(sound1.secondParameter), sound1.secondParameterValue, false);
      LoopingSoundParameterUpdater.Sound sound2 = new LoopingSoundParameterUpdater.Sound()
      {
        ev = sound1.ev,
        path = sound1.path,
        description = soundDescription,
        transform = sound1.transform,
        objectIsSelectedAndVisible = false
      };
      foreach (SoundDescription.Parameter parameter in soundDescription.parameters)
      {
        LoopingSoundParameterUpdater parameterUpdater = (LoopingSoundParameterUpdater) null;
        if (this.parameterUpdaters.TryGetValue(parameter.name, out parameterUpdater))
          parameterUpdater.Add(sound2);
      }
      dataList[index] = sound1;
    }
    pooledList2.Recycle();
    foreach (int index in (List<int>) pooledList3)
    {
      LoopingSoundManager.Sound sound3 = dataList[index];
      SoundDescription soundDescription = this.GetSoundDescription(sound3.path);
      LoopingSoundParameterUpdater.Sound sound4 = new LoopingSoundParameterUpdater.Sound()
      {
        ev = sound3.ev,
        path = sound3.path,
        description = soundDescription,
        transform = sound3.transform,
        objectIsSelectedAndVisible = false
      };
      foreach (SoundDescription.Parameter parameter in soundDescription.parameters)
      {
        LoopingSoundParameterUpdater parameterUpdater = (LoopingSoundParameterUpdater) null;
        if (this.parameterUpdaters.TryGetValue(parameter.name, out parameterUpdater))
          parameterUpdater.Remove(sound4);
      }
      if (sound3.ShouldCameraScalePosition)
        ((EventInstance) ref sound3.ev).stop((STOP_MODE) 1);
      else
        ((EventInstance) ref sound3.ev).stop((STOP_MODE) 0);
      sound3.flags &= ~LoopingSoundManager.Sound.Flags.PLAYING;
      ((EventInstance) ref sound3.ev).release();
      dataList[index] = sound3;
    }
    pooledList3.Recycle();
    float velocityScale = TuningData<LoopingSoundManager.Tuning>.Get().velocityScale;
    foreach (LoopingSoundManager.Sound sound in (List<LoopingSoundManager.Sound>) pooledList1)
    {
      ATTRIBUTES_3D attributes3D = RuntimeUtils.To3DAttributes(SoundEvent.GetCameraScaledPosition(sound.pos, sound.objectIsSelectedAndVisible));
      attributes3D.velocity = RuntimeUtils.ToFMODVector(Vector2.op_Implicit(Vector2.op_Multiply(sound.velocity, velocityScale)));
      EventInstance ev = sound.ev;
      ((EventInstance) ref ev).set3DAttributes(attributes3D);
    }
    foreach (KeyValuePair<HashedString, LoopingSoundParameterUpdater> parameterUpdater in this.parameterUpdaters)
      parameterUpdater.Value.Update(dt);
    pooledList1.Recycle();
  }

  public static LoopingSoundManager Get() => LoopingSoundManager.instance;

  public void StopAllSounds()
  {
    foreach (LoopingSoundManager.Sound data in this.sounds.GetDataList())
    {
      if (data.IsPlaying)
      {
        EventInstance ev1 = data.ev;
        ((EventInstance) ref ev1).stop((STOP_MODE) 1);
        EventInstance ev2 = data.ev;
        ((EventInstance) ref ev2).release();
      }
    }
  }

  private SoundDescription GetSoundDescription(HashedString path) => KFMOD.GetSoundEventDescription(path);

  public HandleVector<int>.Handle Add(
    string path,
    Vector3 pos,
    Transform transform = null,
    bool pause_on_game_pause = true,
    bool enable_culling = true,
    bool enable_camera_scaled_position = true,
    float vol = 1f,
    bool objectIsSelectedAndVisible = false)
  {
    SoundDescription eventDescription = KFMOD.GetSoundEventDescription(HashedString.op_Implicit(path));
    LoopingSoundManager.Sound.Flags flags = (LoopingSoundManager.Sound.Flags) 0;
    if (pause_on_game_pause)
      flags |= LoopingSoundManager.Sound.Flags.PAUSE_ON_GAME_PAUSED;
    if (enable_culling)
      flags |= LoopingSoundManager.Sound.Flags.ENABLE_CULLING;
    if (enable_camera_scaled_position)
      flags |= LoopingSoundManager.Sound.Flags.ENABLE_CAMERA_SCALED_POSITION;
    KBatchedAnimController kbatchedAnimController = (KBatchedAnimController) null;
    if (Object.op_Inequality((Object) transform, (Object) null))
      kbatchedAnimController = ((Component) transform).GetComponent<KBatchedAnimController>();
    return this.sounds.Allocate(new LoopingSoundManager.Sound()
    {
      transform = transform,
      animController = kbatchedAnimController,
      falloffDistanceSq = eventDescription.falloffDistanceSq,
      path = HashedString.op_Implicit(path),
      pos = pos,
      flags = flags,
      firstParameter = HashedString.Invalid,
      secondParameter = HashedString.Invalid,
      vol = vol,
      objectIsSelectedAndVisible = objectIsSelectedAndVisible
    });
  }

  public static HandleVector<int>.Handle StartSound(
    EventReference event_ref,
    Vector3 pos,
    bool pause_on_game_pause = true,
    bool enable_culling = true)
  {
    return LoopingSoundManager.StartSound(KFMOD.GetEventReferencePath(event_ref), pos, pause_on_game_pause, enable_culling);
  }

  public static HandleVector<int>.Handle StartSound(
    string path,
    Vector3 pos,
    bool pause_on_game_pause = true,
    bool enable_culling = true)
  {
    if (!string.IsNullOrEmpty(path))
      return LoopingSoundManager.Get().Add(path, pos, pause_on_game_pause: pause_on_game_pause, enable_culling: enable_culling);
    Debug.LogWarning((object) "Missing sound");
    return HandleVector<int>.InvalidHandle;
  }

  public static void StopSound(HandleVector<int>.Handle handle)
  {
    if (Object.op_Equality((Object) LoopingSoundManager.Get(), (Object) null))
      return;
    LoopingSoundManager.Sound data = LoopingSoundManager.Get().sounds.GetData(handle);
    if (data.IsPlaying)
    {
      ((EventInstance) ref data.ev).stop(LoopingSoundManager.Get().GameIsPaused ? (STOP_MODE) 1 : (STOP_MODE) 0);
      ((EventInstance) ref data.ev).release();
      SoundDescription eventDescription = KFMOD.GetSoundEventDescription(data.path);
      foreach (SoundDescription.Parameter parameter in eventDescription.parameters)
      {
        LoopingSoundParameterUpdater parameterUpdater = (LoopingSoundParameterUpdater) null;
        if (LoopingSoundManager.Get().parameterUpdaters.TryGetValue(parameter.name, out parameterUpdater))
        {
          LoopingSoundParameterUpdater.Sound sound = new LoopingSoundParameterUpdater.Sound()
          {
            ev = data.ev,
            path = data.path,
            description = eventDescription,
            transform = data.transform,
            objectIsSelectedAndVisible = false
          };
          parameterUpdater.Remove(sound);
        }
      }
    }
    LoopingSoundManager.Get().sounds.Free(handle);
  }

  public static void PauseSound(HandleVector<int>.Handle handle, bool paused)
  {
    LoopingSoundManager.Sound data = LoopingSoundManager.Get().sounds.GetData(handle);
    if (!data.IsPlaying)
      return;
    ((EventInstance) ref data.ev).setPaused(paused);
  }

  private void OnPauseChanged(object data)
  {
    bool flag = (bool) data;
    this.GameIsPaused = flag;
    foreach (LoopingSoundManager.Sound data1 in this.sounds.GetDataList())
    {
      if (data1.IsPlaying)
      {
        EventInstance ev = data1.ev;
        ((EventInstance) ref ev).setPaused(flag && data1.ShouldPauseOnGamePaused);
      }
    }
  }

  public class Tuning : TuningData<LoopingSoundManager.Tuning>
  {
    public float velocityScale;
  }

  public struct Sound
  {
    public EventInstance ev;
    public Transform transform;
    public KBatchedAnimController animController;
    public float falloffDistanceSq;
    public HashedString path;
    public Vector3 pos;
    public Vector2 velocity;
    public HashedString firstParameter;
    public HashedString secondParameter;
    public float firstParameterValue;
    public float secondParameterValue;
    public float vol;
    public bool objectIsSelectedAndVisible;
    public LoopingSoundManager.Sound.Flags flags;

    public bool IsPlaying => (this.flags & LoopingSoundManager.Sound.Flags.PLAYING) != 0;

    public bool ShouldPauseOnGamePaused => (this.flags & LoopingSoundManager.Sound.Flags.PAUSE_ON_GAME_PAUSED) != 0;

    public bool IsCullingEnabled => (this.flags & LoopingSoundManager.Sound.Flags.ENABLE_CULLING) != 0;

    public bool ShouldCameraScalePosition => (this.flags & LoopingSoundManager.Sound.Flags.ENABLE_CAMERA_SCALED_POSITION) != 0;

    [Flags]
    public enum Flags
    {
      PLAYING = 1,
      PAUSE_ON_GAME_PAUSED = 2,
      ENABLE_CULLING = 4,
      ENABLE_CAMERA_SCALED_POSITION = 8,
    }
  }
}
