// Decompiled with JetBrains decompiler
// Type: UserVolumeLoopingUpdater
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using System.Collections.Generic;

internal abstract class UserVolumeLoopingUpdater : LoopingSoundParameterUpdater
{
  private List<UserVolumeLoopingUpdater.Entry> entries = new List<UserVolumeLoopingUpdater.Entry>();
  private string playerPref;

  public UserVolumeLoopingUpdater(string parameter, string player_pref)
    : base(HashedString.op_Implicit(parameter))
  {
    this.playerPref = player_pref;
  }

  public override void Add(LoopingSoundParameterUpdater.Sound sound) => this.entries.Add(new UserVolumeLoopingUpdater.Entry()
  {
    ev = sound.ev,
    parameterId = ((SoundDescription) ref sound.description).GetParameterId(this.parameter)
  });

  public override void Update(float dt)
  {
    if (string.IsNullOrEmpty(this.playerPref))
      return;
    float num = KPlayerPrefs.GetFloat(this.playerPref);
    foreach (UserVolumeLoopingUpdater.Entry entry in this.entries)
    {
      EventInstance ev = entry.ev;
      ((EventInstance) ref ev).setParameterByID(entry.parameterId, num, false);
    }
  }

  public override void Remove(LoopingSoundParameterUpdater.Sound sound)
  {
    for (int index = 0; index < this.entries.Count; ++index)
    {
      if (this.entries[index].ev.handle == sound.ev.handle)
      {
        this.entries.RemoveAt(index);
        break;
      }
    }
  }

  private struct Entry
  {
    public EventInstance ev;
    public PARAMETER_ID parameterId;
  }
}
