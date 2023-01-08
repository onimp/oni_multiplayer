// Decompiled with JetBrains decompiler
// Type: TimeOfDay
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using KSerialization;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/TimeOfDay")]
public class TimeOfDay : KMonoBehaviour, ISaveLoadable
{
  [Serialize]
  private float scale;
  private TimeOfDay.TimeRegion timeRegion;
  private EventInstance nightLPEvent;
  public static TimeOfDay Instance;
  private bool isEclipse;

  public static void DestroyInstance() => TimeOfDay.Instance = (TimeOfDay) null;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    TimeOfDay.Instance = this;
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    TimeOfDay.Instance = (TimeOfDay) null;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.timeRegion = this.GetCurrentTimeRegion();
    double num = (double) this.UpdateSunlightIntensity();
  }

  [System.Runtime.Serialization.OnDeserialized]
  private void OnDeserialized() => this.UpdateVisuals();

  public TimeOfDay.TimeRegion GetCurrentTimeRegion() => GameClock.Instance.IsNighttime() ? TimeOfDay.TimeRegion.Night : TimeOfDay.TimeRegion.Day;

  private void Update()
  {
    this.UpdateVisuals();
    this.UpdateAudio();
  }

  private void UpdateVisuals()
  {
    float num1 = 0.875f;
    float num2 = 0.2f;
    float num3 = 1f;
    float num4 = 0.0f;
    if ((double) GameClock.Instance.GetCurrentCycleAsPercentage() >= (double) num1)
      num4 = num3;
    this.scale = Mathf.Lerp(this.scale, num4, Time.deltaTime * num2);
    Shader.SetGlobalVector("_TimeOfDay", new Vector4(this.scale, this.UpdateSunlightIntensity(), 0.0f, 0.0f));
  }

  private void UpdateAudio()
  {
    TimeOfDay.TimeRegion currentTimeRegion = this.GetCurrentTimeRegion();
    if (currentTimeRegion == this.timeRegion)
      return;
    this.TriggerSoundChange(currentTimeRegion);
    this.timeRegion = currentTimeRegion;
    this.Trigger(1791086652, (object) null);
  }

  public void Sim4000ms(float dt)
  {
    double num = (double) this.UpdateSunlightIntensity();
  }

  public void SetEclipse(bool eclipse) => this.isEclipse = eclipse;

  private float UpdateSunlightIntensity()
  {
    float durationInPercentage = GameClock.Instance.GetDaytimeDurationInPercentage();
    float num1 = GameClock.Instance.GetCurrentCycleAsPercentage() / durationInPercentage;
    if ((double) num1 >= 1.0 || this.isEclipse)
      num1 = 0.0f;
    float num2 = Mathf.Sin(num1 * 3.14159274f);
    Game.Instance.currentFallbackSunlightIntensity = num2 * 80000f;
    foreach (WorldContainer worldContainer in (IEnumerable<WorldContainer>) ClusterManager.Instance.WorldContainers)
    {
      worldContainer.currentSunlightIntensity = num2 * (float) worldContainer.sunlight;
      worldContainer.currentCosmicIntensity = (float) worldContainer.cosmicRadiation;
    }
    return num2;
  }

  private void TriggerSoundChange(TimeOfDay.TimeRegion new_region)
  {
    switch (new_region)
    {
      case TimeOfDay.TimeRegion.Day:
        AudioMixer.instance.Stop(AudioMixerSnapshots.Get().NightStartedMigrated);
        if (MusicManager.instance.SongIsPlaying("Stinger_Loop_Night"))
          MusicManager.instance.StopSong("Stinger_Loop_Night");
        MusicManager.instance.PlaySong("Stinger_Day");
        MusicManager.instance.PlayDynamicMusic();
        break;
      case TimeOfDay.TimeRegion.Night:
        AudioMixer.instance.Start(AudioMixerSnapshots.Get().NightStartedMigrated);
        MusicManager.instance.PlaySong("Stinger_Loop_Night");
        break;
    }
  }

  public void SetScale(float new_scale) => this.scale = new_scale;

  public enum TimeRegion
  {
    Invalid,
    Day,
    Night,
  }
}
