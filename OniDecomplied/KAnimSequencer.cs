// Decompiled with JetBrains decompiler
// Type: KAnimSequencer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/KAnimSequencer")]
public class KAnimSequencer : KMonoBehaviour, ISaveLoadable
{
  [Serialize]
  public bool autoRun;
  [Serialize]
  public KAnimSequencer.KAnimSequence[] sequence = new KAnimSequencer.KAnimSequence[0];
  private int currentIndex;
  private KBatchedAnimController kbac;
  private MinionBrain mb;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.kbac = ((Component) this).GetComponent<KBatchedAnimController>();
    this.mb = ((Component) this).GetComponent<MinionBrain>();
    if (!this.autoRun)
      return;
    this.PlaySequence();
  }

  public void Reset() => this.currentIndex = 0;

  public void PlaySequence()
  {
    if (this.sequence == null || this.sequence.Length == 0)
      return;
    if (Object.op_Inequality((Object) this.mb, (Object) null))
      this.mb.Suspend("AnimSequencer");
    this.kbac.onAnimComplete += new KAnimControllerBase.KAnimEvent(this.PlayNext);
    this.PlayNext(HashedString.op_Implicit((string) null));
  }

  private void PlayNext(HashedString name)
  {
    if (this.sequence.Length > this.currentIndex)
    {
      this.kbac.Play(new HashedString(this.sequence[this.currentIndex].anim), this.sequence[this.currentIndex].mode, this.sequence[this.currentIndex].speed);
      ++this.currentIndex;
    }
    else
    {
      this.kbac.onAnimComplete -= new KAnimControllerBase.KAnimEvent(this.PlayNext);
      if (!Object.op_Inequality((Object) this.mb, (Object) null))
        return;
      this.mb.Resume("AnimSequencer");
    }
  }

  [SerializationConfig]
  [Serializable]
  public class KAnimSequence
  {
    public string anim;
    public float speed = 1f;
    public KAnim.PlayMode mode = (KAnim.PlayMode) 1;
  }
}
