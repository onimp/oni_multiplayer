// Decompiled with JetBrains decompiler
// Type: Klei.AI.Emote
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace Klei.AI
{
  public class Emote : Resource
  {
    private HashedString animSetName = HashedString.op_Implicit((string) null);
    private KAnimFile animSet;
    private List<EmoteStep> emoteSteps = new List<EmoteStep>();

    public int StepCount => this.emoteSteps != null ? this.emoteSteps.Count : 0;

    public KAnimFile AnimSet
    {
      get
      {
        if (HashedString.op_Inequality(this.animSetName, HashedString.Invalid) && Object.op_Equality((Object) this.animSet, (Object) null))
          this.animSet = Assets.GetAnim(this.animSetName);
        return this.animSet;
      }
    }

    public Emote(ResourceSet parent, string emoteId, EmoteStep[] defaultSteps, string animSetName = null)
      : base(emoteId, parent, (string) null)
    {
      this.emoteSteps.AddRange((IEnumerable<EmoteStep>) defaultSteps);
      this.animSetName = HashedString.op_Implicit(animSetName);
    }

    public bool IsValidForController(KBatchedAnimController animController)
    {
      bool flag1 = true;
      for (int index = 0; flag1 && index < this.StepCount; ++index)
        flag1 = animController.HasAnimation(this.emoteSteps[index].anim);
      KAnimFileData kanimFileData = Object.op_Equality((Object) this.animSet, (Object) null) ? (KAnimFileData) null : this.animSet.GetData();
      for (int index1 = 0; kanimFileData != null & flag1 && index1 < this.StepCount; ++index1)
      {
        bool flag2 = false;
        for (int index2 = 0; !flag2 && index2 < kanimFileData.animCount; ++index2)
          flag2 = HashedString.op_Equality(kanimFileData.GetAnim(index1).id, this.emoteSteps[index1].anim);
        flag1 = flag2;
      }
      return flag1;
    }

    public void ApplyAnimOverrides(KBatchedAnimController animController, KAnimFile overrideSet)
    {
      KAnimFile kanim_file = Object.op_Inequality((Object) overrideSet, (Object) null) ? overrideSet : this.AnimSet;
      if (Object.op_Equality((Object) kanim_file, (Object) null) || Object.op_Equality((Object) animController, (Object) null))
        return;
      animController.AddAnimOverrides(kanim_file);
    }

    public void RemoveAnimOverrides(KBatchedAnimController animController, KAnimFile overrideSet)
    {
      KAnimFile kanim_file = Object.op_Inequality((Object) overrideSet, (Object) null) ? overrideSet : this.AnimSet;
      if (Object.op_Equality((Object) kanim_file, (Object) null) || Object.op_Equality((Object) animController, (Object) null))
        return;
      animController.RemoveAnimOverrides(kanim_file);
    }

    public void CollectStepAnims(out HashedString[] emoteAnims, int iterations)
    {
      emoteAnims = new HashedString[this.emoteSteps.Count * iterations];
      for (int index = 0; index < emoteAnims.Length; ++index)
        emoteAnims[index] = this.emoteSteps[index % this.emoteSteps.Count].anim;
    }

    public bool IsValidStep(int stepIdx) => stepIdx >= 0 && stepIdx < this.emoteSteps.Count;

    public EmoteStep this[int stepIdx] => !this.IsValidStep(stepIdx) ? (EmoteStep) null : this.emoteSteps[stepIdx];

    public int GetStepIndex(HashedString animName)
    {
      int index = 0;
      bool flag = false;
      for (; index < this.emoteSteps.Count; ++index)
      {
        if (HashedString.op_Equality(this.emoteSteps[index].anim, animName))
        {
          flag = true;
          break;
        }
      }
      Debug.Assert(flag, (object) string.Format("Could not find emote step {0} for emote {1}!", (object) animName, (object) this.Id));
      return index;
    }
  }
}
