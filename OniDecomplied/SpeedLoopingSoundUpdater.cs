// Decompiled with JetBrains decompiler
// Type: SpeedLoopingSoundUpdater
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using System.Collections.Generic;
using UnityEngine;

public class SpeedLoopingSoundUpdater : LoopingSoundParameterUpdater
{
  private List<SpeedLoopingSoundUpdater.Entry> entries = new List<SpeedLoopingSoundUpdater.Entry>();

  public SpeedLoopingSoundUpdater()
    : base(HashedString.op_Implicit("Speed"))
  {
  }

  public override void Add(LoopingSoundParameterUpdater.Sound sound) => this.entries.Add(new SpeedLoopingSoundUpdater.Entry()
  {
    ev = sound.ev,
    parameterId = ((SoundDescription) ref sound.description).GetParameterId(this.parameter)
  });

  public override void Update(float dt)
  {
    float speedParameterValue = SpeedLoopingSoundUpdater.GetSpeedParameterValue();
    foreach (SpeedLoopingSoundUpdater.Entry entry in this.entries)
    {
      EventInstance ev = entry.ev;
      ((EventInstance) ref ev).setParameterByID(entry.parameterId, speedParameterValue, false);
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

  public static float GetSpeedParameterValue() => Time.timeScale * 1f;

  private struct Entry
  {
    public EventInstance ev;
    public PARAMETER_ID parameterId;
  }
}
