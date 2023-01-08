// Decompiled with JetBrains decompiler
// Type: SpeedOneShotUpdater
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;

public class SpeedOneShotUpdater : OneShotSoundParameterUpdater
{
  public SpeedOneShotUpdater()
    : base(HashedString.op_Implicit("Speed"))
  {
  }

  public virtual void Play(OneShotSoundParameterUpdater.Sound sound) => ((EventInstance) ref sound.ev).setParameterByID(((SoundDescription) ref sound.description).GetParameterId(this.parameter), SpeedLoopingSoundUpdater.GetSpeedParameterValue(), false);
}
