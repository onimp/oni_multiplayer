// Decompiled with JetBrains decompiler
// Type: GameAudioSheets
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class GameAudioSheets : AudioSheets
{
  private static GameAudioSheets _Instance;
  private HashSet<HashedString> validFileNames = new HashSet<HashedString>();
  private Dictionary<HashedString, HashSet<HashedString>> animsNotAllowedToPlaySpeech = new Dictionary<HashedString, HashSet<HashedString>>();

  public static GameAudioSheets Get()
  {
    if (Object.op_Equality((Object) GameAudioSheets._Instance, (Object) null))
      GameAudioSheets._Instance = Resources.Load<GameAudioSheets>(nameof (GameAudioSheets));
    return GameAudioSheets._Instance;
  }

  public override void Initialize()
  {
    this.validFileNames.Add(HashedString.op_Implicit("game_triggered"));
    foreach (KAnimFile animAsset in Assets.instance.AnimAssets)
    {
      if (!Object.op_Equality((Object) animAsset, (Object) null))
        this.validFileNames.Add(HashedString.op_Implicit(((Object) animAsset).name));
    }
    base.Initialize();
    foreach (AudioSheet sheet in this.sheets)
    {
      foreach (AudioSheet.SoundInfo soundInfo in sheet.soundInfos)
      {
        if (soundInfo.Type == "MouthFlapSoundEvent" || soundInfo.Type == "VoiceSoundEvent")
        {
          HashSet<HashedString> hashedStringSet = (HashSet<HashedString>) null;
          if (!this.animsNotAllowedToPlaySpeech.TryGetValue(HashedString.op_Implicit(soundInfo.File), out hashedStringSet))
          {
            hashedStringSet = new HashSet<HashedString>();
            this.animsNotAllowedToPlaySpeech[HashedString.op_Implicit(soundInfo.File)] = hashedStringSet;
          }
          hashedStringSet.Add(HashedString.op_Implicit(soundInfo.Anim));
        }
      }
    }
  }

  protected override AnimEvent CreateSoundOfType(
    string type,
    string file_name,
    string sound_name,
    int frame,
    float min_interval,
    string dlcId)
  {
    SoundEvent soundOfType = (SoundEvent) null;
    bool flag = true;
    if (sound_name.Contains(":disable_camera_position_scaling"))
    {
      sound_name = sound_name.Replace(":disable_camera_position_scaling", "");
      flag = false;
    }
    switch (type)
    {
      case "FloorSoundEvent":
        soundOfType = (SoundEvent) new FloorSoundEvent(file_name, sound_name, frame);
        break;
      case "SoundEvent":
      case "LoopingSoundEvent":
        bool is_looping = type == "LoopingSoundEvent";
        string[] strArray = sound_name.Split(':');
        sound_name = strArray[0];
        soundOfType = new SoundEvent(file_name, sound_name, frame, true, is_looping, min_interval, false);
        for (int index = 1; index < strArray.Length; ++index)
        {
          if (strArray[index] == "IGNORE_PAUSE")
            soundOfType.ignorePause = true;
          else
            Debug.LogWarning((object) (sound_name + " has unknown parameter " + strArray[index]));
        }
        break;
      case "LadderSoundEvent":
        soundOfType = (SoundEvent) new LadderSoundEvent(file_name, sound_name, frame);
        break;
      case "LaserSoundEvent":
        soundOfType = (SoundEvent) new LaserSoundEvent(file_name, sound_name, frame, min_interval);
        break;
      case "HatchDrillSoundEvent":
        soundOfType = (SoundEvent) new HatchDrillSoundEvent(file_name, sound_name, frame, min_interval);
        break;
      case "CreatureChewSoundEvent":
        soundOfType = (SoundEvent) new CreatureChewSoundEvent(file_name, sound_name, frame, min_interval);
        break;
      case "BuildingDamageSoundEvent":
        soundOfType = (SoundEvent) new BuildingDamageSoundEvent(file_name, sound_name, frame);
        break;
      case "WallDamageSoundEvent":
        soundOfType = (SoundEvent) new WallDamageSoundEvent(file_name, sound_name, frame, min_interval);
        break;
      case "RemoteSoundEvent":
        soundOfType = (SoundEvent) new RemoteSoundEvent(file_name, sound_name, frame, min_interval);
        break;
      case "VoiceSoundEvent":
      case "LoopingVoiceSoundEvent":
        soundOfType = (SoundEvent) new VoiceSoundEvent(file_name, sound_name, frame, type == "LoopingVoiceSoundEvent");
        break;
      case "MouthFlapSoundEvent":
        soundOfType = (SoundEvent) new MouthFlapSoundEvent(file_name, sound_name, frame, false);
        break;
      case "MainMenuSoundEvent":
        soundOfType = (SoundEvent) new MainMenuSoundEvent(file_name, sound_name, frame);
        break;
      case "ClusterMapSoundEvent":
        soundOfType = (SoundEvent) new ClusterMapSoundEvent(file_name, sound_name, frame, false);
        break;
      case "ClusterMapLoopingSoundEvent":
        soundOfType = (SoundEvent) new ClusterMapSoundEvent(file_name, sound_name, frame, true);
        break;
      case "UIAnimationSoundEvent":
        soundOfType = (SoundEvent) new UIAnimationSoundEvent(file_name, sound_name, frame, false);
        break;
      case "UIAnimationLoopingSoundEvent":
        soundOfType = (SoundEvent) new UIAnimationSoundEvent(file_name, sound_name, frame, true);
        break;
      case "CreatureVariationSoundEvent":
        soundOfType = (SoundEvent) new CreatureVariationSoundEvent(file_name, sound_name, frame, true, type == "LoopingSoundEvent", min_interval, false);
        break;
      case "CountedSoundEvent":
        soundOfType = (SoundEvent) new CountedSoundEvent(file_name, sound_name, frame, true, false, min_interval, false);
        break;
      case "SculptingSoundEvent":
        soundOfType = (SoundEvent) new SculptingSoundEvent(file_name, sound_name, frame, true, false, min_interval, false);
        break;
      case "PhonoboxSoundEvent":
        soundOfType = (SoundEvent) new PhonoboxSoundEvent(file_name, sound_name, frame, min_interval);
        break;
      case "PlantMutationSoundEvent":
        soundOfType = (SoundEvent) new PlantMutationSoundEvent(file_name, sound_name, frame, min_interval);
        break;
    }
    if (soundOfType != null)
      soundOfType.shouldCameraScalePosition = flag;
    return (AnimEvent) soundOfType;
  }

  public bool IsAnimAllowedToPlaySpeech(KAnim.Anim anim)
  {
    HashSet<HashedString> hashedStringSet = (HashSet<HashedString>) null;
    return !this.animsNotAllowedToPlaySpeech.TryGetValue(HashedString.op_Implicit(anim.animFile.name), out hashedStringSet) || !hashedStringSet.Contains(anim.hash);
  }

  private class SingleAudioSheetLoader : AsyncLoader
  {
    public AudioSheet sheet;
    public string text;
    public string name;

    public virtual void Run() => this.sheet.soundInfos = new ResourceLoader<AudioSheet.SoundInfo>(this.text, this.name).resources.ToArray();
  }

  private class GameAudioSheetLoader : GlobalAsyncLoader<GameAudioSheets.GameAudioSheetLoader>
  {
    public virtual void CollectLoaders(List<AsyncLoader> loaders)
    {
      foreach (AudioSheet sheet in GameAudioSheets.Get().sheets)
        loaders.Add((AsyncLoader) new GameAudioSheets.SingleAudioSheetLoader()
        {
          sheet = sheet,
          text = sheet.asset.text,
          name = ((Object) sheet.asset).name
        });
    }

    public virtual void Run()
    {
    }
  }
}
