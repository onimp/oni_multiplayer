// Decompiled with JetBrains decompiler
// Type: WallDamageSoundEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using UnityEngine;

public class WallDamageSoundEvent : SoundEvent
{
  public int tile;

  public WallDamageSoundEvent(string file_name, string sound_name, int frame, float min_interval)
    : base(file_name, sound_name, frame, true, false, min_interval, false)
  {
  }

  public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
  {
    Vector3 vector3_1 = new Vector3();
    AggressiveChore.StatesInstance smi = ((Component) behaviour.controller).gameObject.GetSMI<AggressiveChore.StatesInstance>();
    if (smi == null)
      return;
    this.tile = smi.sm.wallCellToBreak;
    int audioCategory = WallDamageSoundEvent.GetAudioCategory(this.tile);
    Vector3 vector3_2 = Grid.CellToPos(this.tile);
    vector3_2.z = 0.0f;
    GameObject gameObject = ((Component) behaviour.controller).gameObject;
    if (this.objectIsSelectedAndVisible)
      vector3_2 = SoundEvent.AudioHighlightListenerPosition(vector3_2);
    if (!this.objectIsSelectedAndVisible && !SoundEvent.ShouldPlaySound(behaviour.controller, this.sound, this.soundHash, this.looping, this.isDynamic))
      return;
    EventInstance instance = SoundEvent.BeginOneShot(this.sound, vector3_2, SoundEvent.GetVolume(this.objectIsSelectedAndVisible));
    ((EventInstance) ref instance).setParameterByName("material_ID", (float) audioCategory, false);
    SoundEvent.EndOneShot(instance);
  }

  private static int GetAudioCategory(int tile)
  {
    Element element = Grid.Element[tile];
    if (Grid.Foundation[tile])
      return 12;
    if (element.id == SimHashes.Dirt)
      return 0;
    if (element.id == SimHashes.CrushedIce || element.id == SimHashes.Ice || element.id == SimHashes.DirtyIce)
      return 1;
    if (element.id == SimHashes.OxyRock)
      return 3;
    if (element.HasTag(GameTags.Metal))
      return 5;
    if (element.HasTag(GameTags.RefinedMetal))
      return 6;
    if (element.id == SimHashes.Sand)
      return 8;
    return element.id == SimHashes.Algae ? 10 : 7;
  }
}
