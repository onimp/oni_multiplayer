// Decompiled with JetBrains decompiler
// Type: PlantMutationSoundEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class PlantMutationSoundEvent : SoundEvent
{
  public PlantMutationSoundEvent(
    string file_name,
    string sound_name,
    int frame,
    float min_interval)
    : base(file_name, sound_name, frame, false, false, min_interval, true)
  {
  }

  public override void OnPlay(AnimEventManager.EventPlayerData behaviour)
  {
    MutantPlant component = ((Component) behaviour.controller).gameObject.GetComponent<MutantPlant>();
    Vector3 position = TransformExtensions.GetPosition(behaviour.GetComponent<Transform>());
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    for (int index = 0; index < component.GetSoundEvents().Count; ++index)
      SoundEvent.PlayOneShot(component.GetSoundEvents()[index], position);
  }
}
