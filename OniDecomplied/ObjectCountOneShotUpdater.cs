// Decompiled with JetBrains decompiler
// Type: ObjectCountOneShotUpdater
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

internal class ObjectCountOneShotUpdater : OneShotSoundParameterUpdater
{
  private Dictionary<HashedString, int> soundCounts = new Dictionary<HashedString, int>();

  public ObjectCountOneShotUpdater()
    : base(HashedString.op_Implicit("objectCount"))
  {
  }

  public virtual void Update(float dt) => this.soundCounts.Clear();

  public virtual void Play(OneShotSoundParameterUpdater.Sound sound)
  {
    UpdateObjectCountParameter.Settings settings = UpdateObjectCountParameter.GetSettings(sound.path, sound.description);
    int num = 0;
    this.soundCounts.TryGetValue(sound.path, out num);
    int count;
    this.soundCounts[sound.path] = count = num + 1;
    UpdateObjectCountParameter.ApplySettings(sound.ev, count, settings);
  }
}
