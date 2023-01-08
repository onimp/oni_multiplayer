// Decompiled with JetBrains decompiler
// Type: LadderSoundEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class LadderSoundEvent : SoundEvent
{
  public LadderSoundEvent(string file_name, string sound_name, int frame)
    : base(file_name, sound_name, frame, false, false, (float) SoundEvent.IGNORE_INTERVAL, true)
  {
  }

  public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
  {
    this.objectIsSelectedAndVisible = SoundEvent.ObjectIsSelectedAndVisible(((Component) behaviour.controller).gameObject);
    if (!this.objectIsSelectedAndVisible && !SoundEvent.ShouldPlaySound(behaviour.controller, this.sound, this.looping, this.isDynamic))
      return;
    Vector3 vector3 = TransformExtensions.GetPosition(behaviour.GetComponent<Transform>());
    vector3.z = 0.0f;
    float volume = 1f;
    if (this.objectIsSelectedAndVisible)
    {
      vector3 = SoundEvent.AudioHighlightListenerPosition(vector3);
      volume = SoundEvent.GetVolume(this.objectIsSelectedAndVisible);
    }
    int cell = Grid.PosToCell(vector3);
    BuildingDef buildingDef = (BuildingDef) null;
    if (Grid.IsValidCell(cell))
    {
      GameObject gameObject = Grid.Objects[cell, 1];
      if (Object.op_Inequality((Object) gameObject, (Object) null) && Object.op_Inequality((Object) gameObject.GetComponent<Ladder>(), (Object) null))
      {
        Building component = (Building) gameObject.GetComponent<BuildingComplete>();
        if (Object.op_Inequality((Object) component, (Object) null))
          buildingDef = component.Def;
      }
    }
    if (!Object.op_Inequality((Object) buildingDef, (Object) null))
      return;
    string sound = GlobalAssets.GetSound(buildingDef.PrefabID == "LadderFast" ? StringFormatter.Combine(this.name, "_Plastic") : this.name);
    if (sound == null)
      return;
    SoundEvent.PlayOneShot(sound, vector3, volume);
  }
}
