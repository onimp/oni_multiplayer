// Decompiled with JetBrains decompiler
// Type: UpdateConsumedMassParameter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using System.Collections.Generic;
using UnityEngine;

internal class UpdateConsumedMassParameter : LoopingSoundParameterUpdater
{
  private List<UpdateConsumedMassParameter.Entry> entries = new List<UpdateConsumedMassParameter.Entry>();

  public UpdateConsumedMassParameter()
    : base(HashedString.op_Implicit("consumedMass"))
  {
  }

  public override void Add(LoopingSoundParameterUpdater.Sound sound) => this.entries.Add(new UpdateConsumedMassParameter.Entry()
  {
    creatureCalorieMonitor = ((Component) sound.transform).GetSMI<CreatureCalorieMonitor.Instance>(),
    ev = sound.ev,
    parameterId = ((SoundDescription) ref sound.description).GetParameterId(this.parameter)
  });

  public override void Update(float dt)
  {
    foreach (UpdateConsumedMassParameter.Entry entry in this.entries)
    {
      if (!entry.creatureCalorieMonitor.IsNullOrStopped())
      {
        float fullness = entry.creatureCalorieMonitor.stomach.GetFullness();
        EventInstance ev = entry.ev;
        ((EventInstance) ref ev).setParameterByID(entry.parameterId, fullness, false);
      }
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
    public CreatureCalorieMonitor.Instance creatureCalorieMonitor;
    public EventInstance ev;
    public PARAMETER_ID parameterId;
  }
}
