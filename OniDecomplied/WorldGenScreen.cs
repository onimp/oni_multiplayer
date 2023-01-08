// Decompiled with JetBrains decompiler
// Type: WorldGenScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ProcGenGame;
using System;
using System.IO;
using UnityEngine;

public class WorldGenScreen : NewGameFlowScreen
{
  [MyCmpReq]
  private OfflineWorldGen offlineWorldGen;
  public static WorldGenScreen Instance;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    WorldGenScreen.Instance = this;
  }

  protected virtual void OnForcedCleanUp()
  {
    WorldGenScreen.Instance = (WorldGenScreen) null;
    ((KMonoBehaviour) this).OnForcedCleanUp();
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (Object.op_Inequality((Object) MainMenu.Instance, (Object) null))
      MainMenu.Instance.StopAmbience();
    this.TriggerLoadingMusic();
    ((Component) Object.FindObjectOfType<FrontEndBackground>()).gameObject.SetActive(false);
    SaveLoader.SetActiveSaveFilePath((string) null);
    try
    {
      for (int baseID = 0; File.Exists(WorldGen.GetSIMSaveFilename(baseID)); ++baseID)
        File.Delete(WorldGen.GetSIMSaveFilename(baseID));
    }
    catch (Exception ex)
    {
      DebugUtil.LogWarningArgs(new object[1]
      {
        (object) ex.ToString()
      });
    }
    this.offlineWorldGen.Generate();
  }

  private void TriggerLoadingMusic()
  {
    if (!AudioDebug.Get().musicEnabled || MusicManager.instance.SongIsPlaying("Music_FrontEnd"))
      return;
    MainMenu.Instance.StopMainMenuMusic();
    AudioMixer.instance.Start(AudioMixerSnapshots.Get().FrontEndWorldGenerationSnapshot);
    MusicManager.instance.PlaySong("Music_FrontEnd");
    MusicManager.instance.SetSongParameter("Music_FrontEnd", "songSection", 1f);
  }

  public override void OnKeyDown(KButtonEvent e)
  {
    if (!((KInputEvent) e).Consumed)
      e.TryConsume((Action) 1);
    if (!((KInputEvent) e).Consumed)
      e.TryConsume((Action) 5);
    base.OnKeyDown(e);
  }
}
