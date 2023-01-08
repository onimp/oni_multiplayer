// Decompiled with JetBrains decompiler
// Type: UserVolumeOneShotUpdater
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;

internal abstract class UserVolumeOneShotUpdater : OneShotSoundParameterUpdater
{
  private string playerPref;

  public UserVolumeOneShotUpdater(string parameter, string player_pref)
    : base(HashedString.op_Implicit(parameter))
  {
    this.playerPref = player_pref;
  }

  public virtual void Play(OneShotSoundParameterUpdater.Sound sound)
  {
    if (string.IsNullOrEmpty(this.playerPref))
      return;
    float num = KPlayerPrefs.GetFloat(this.playerPref);
    ((EventInstance) ref sound.ev).setParameterByID(((SoundDescription) ref sound.description).GetParameterId(this.parameter), num, false);
  }
}
