// Decompiled with JetBrains decompiler
// Type: AnimEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[Serializable]
public class AnimEvent
{
  [SerializeField]
  private KAnimHashedString fileHash;
  public bool OnExit;

  [SerializeField]
  public string name { get; private set; }

  [SerializeField]
  public string file { get; private set; }

  [SerializeField]
  public int frame { get; private set; }

  public AnimEvent()
  {
  }

  public AnimEvent(string file, string name, int frame)
  {
    this.file = file == "" ? (string) null : file;
    if (this.file != null)
      this.fileHash = new KAnimHashedString(this.file);
    this.name = name;
    this.frame = frame;
  }

  public void Play(AnimEventManager.EventPlayerData behaviour)
  {
    if (this.IsFilteredOut(behaviour))
      return;
    if (behaviour.previousFrame < behaviour.currentFrame)
    {
      if (behaviour.previousFrame >= this.frame || behaviour.currentFrame < this.frame)
        return;
      this.OnPlay(behaviour);
    }
    else
    {
      if (behaviour.previousFrame <= behaviour.currentFrame || behaviour.previousFrame >= this.frame && this.frame > behaviour.currentFrame)
        return;
      this.OnPlay(behaviour);
    }
  }

  private void DebugAnimEvent(string ev_name, AnimEventManager.EventPlayerData behaviour)
  {
  }

  public virtual void OnPlay(AnimEventManager.EventPlayerData behaviour)
  {
  }

  public virtual void OnUpdate(AnimEventManager.EventPlayerData behaviour)
  {
  }

  public virtual void Stop(AnimEventManager.EventPlayerData behaviour)
  {
  }

  protected bool IsFilteredOut(AnimEventManager.EventPlayerData behaviour) => this.file != null && !behaviour.controller.HasAnimationFile(this.fileHash);
}
