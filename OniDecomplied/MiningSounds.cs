// Decompiled with JetBrains decompiler
// Type: MiningSounds
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMODUnity;
using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/MiningSounds")]
public class MiningSounds : KMonoBehaviour
{
  private static HashedString HASH_PERCENTCOMPLETE = HashedString.op_Implicit("percentComplete");
  [MyCmpGet]
  private LoopingSounds loopingSounds;
  private FMODAsset miningSound;
  private EventReference miningSoundEvent;
  private static readonly EventSystem.IntraObjectHandler<MiningSounds> OnStartMiningSoundDelegate = new EventSystem.IntraObjectHandler<MiningSounds>((Action<MiningSounds, object>) ((component, data) => component.OnStartMiningSound(data)));
  private static readonly EventSystem.IntraObjectHandler<MiningSounds> OnStopMiningSoundDelegate = new EventSystem.IntraObjectHandler<MiningSounds>((Action<MiningSounds, object>) ((component, data) => component.OnStopMiningSound(data)));

  protected virtual void OnPrefabInit()
  {
    this.Subscribe<MiningSounds>(-1762453998, MiningSounds.OnStartMiningSoundDelegate);
    this.Subscribe<MiningSounds>(939543986, MiningSounds.OnStopMiningSoundDelegate);
  }

  private void OnStartMiningSound(object data)
  {
    if (!Object.op_Equality((Object) this.miningSound, (Object) null) || !(data is Element element))
      return;
    string miningSound = element.substance.GetMiningSound();
    switch (miningSound)
    {
      case null:
        break;
      case "":
        break;
      default:
        this.miningSoundEvent = RuntimeManager.PathToEventReference(GlobalAssets.GetSound("Mine_" + miningSound));
        if (((EventReference) ref this.miningSoundEvent).IsNull)
          break;
        this.loopingSounds.StartSound(this.miningSoundEvent);
        break;
    }
  }

  private void OnStopMiningSound(object data)
  {
    if (((EventReference) ref this.miningSoundEvent).IsNull)
      return;
    this.loopingSounds.StopSound(this.miningSoundEvent);
    this.miningSound = (FMODAsset) null;
  }

  public void SetPercentComplete(float progress)
  {
    if (((EventReference) ref this.miningSoundEvent).IsNull)
      return;
    this.loopingSounds.SetParameter(this.miningSoundEvent, MiningSounds.HASH_PERCENTCOMPLETE, progress);
  }
}
