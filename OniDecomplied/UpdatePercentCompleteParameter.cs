// Decompiled with JetBrains decompiler
// Type: UpdatePercentCompleteParameter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using System.Collections.Generic;
using UnityEngine;

internal class UpdatePercentCompleteParameter : LoopingSoundParameterUpdater
{
  private List<UpdatePercentCompleteParameter.Entry> entries = new List<UpdatePercentCompleteParameter.Entry>();

  public UpdatePercentCompleteParameter()
    : base(HashedString.op_Implicit("percentComplete"))
  {
  }

  public override void Add(LoopingSoundParameterUpdater.Sound sound) => this.entries.Add(new UpdatePercentCompleteParameter.Entry()
  {
    worker = ((Component) sound.transform).GetComponent<Worker>(),
    ev = sound.ev,
    parameterId = ((SoundDescription) ref sound.description).GetParameterId(this.parameter)
  });

  public override void Update(float dt)
  {
    foreach (UpdatePercentCompleteParameter.Entry entry in this.entries)
    {
      if (!Object.op_Equality((Object) entry.worker, (Object) null))
      {
        Workable workable = entry.worker.workable;
        if (!Object.op_Equality((Object) workable, (Object) null))
        {
          float percentComplete = workable.GetPercentComplete();
          EventInstance ev = entry.ev;
          ((EventInstance) ref ev).setParameterByID(entry.parameterId, percentComplete, false);
        }
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
    public Worker worker;
    public EventInstance ev;
    public PARAMETER_ID parameterId;
  }
}
