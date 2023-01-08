// Decompiled with JetBrains decompiler
// Type: FloorSoundEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using System.Diagnostics;
using UnityEngine;

[DebuggerDisplay("{Name}")]
public class FloorSoundEvent : SoundEvent
{
  public static float IDLE_WALKING_VOLUME_REDUCTION = 0.55f;

  public FloorSoundEvent(string file_name, string sound_name, int frame)
    : base(file_name, sound_name, frame, false, false, (float) SoundEvent.IGNORE_INTERVAL, true)
  {
    this.noiseValues = SoundEventVolumeCache.instance.GetVolume(nameof (FloorSoundEvent), sound_name);
  }

  public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
  {
    Vector3 pos = TransformExtensions.GetPosition(behaviour.GetComponent<Transform>());
    KBatchedAnimController component = behaviour.GetComponent<KBatchedAnimController>();
    if (Object.op_Inequality((Object) component, (Object) null))
      pos = component.GetPivotSymbolPosition();
    int cell = Grid.PosToCell(pos);
    string str = GlobalAssets.GetSound(StringFormatter.Combine(FloorSoundEvent.GetAudioCategory(Grid.CellBelow(cell)), "_", this.name), true) ?? GlobalAssets.GetSound(StringFormatter.Combine("Rock_", this.name), true) ?? GlobalAssets.GetSound(this.name, true);
    this.objectIsSelectedAndVisible = SoundEvent.ObjectIsSelectedAndVisible(((Component) behaviour.controller).gameObject);
    if (SoundEvent.IsLowPrioritySound(str) && !this.objectIsSelectedAndVisible)
      return;
    Vector3 vector3 = SoundEvent.GetCameraScaledPosition(pos);
    vector3.z = 0.0f;
    if (this.objectIsSelectedAndVisible)
      vector3 = SoundEvent.AudioHighlightListenerPosition(vector3);
    if (Grid.Element == null)
      return;
    int num1 = Grid.Element[cell].IsLiquid ? 1 : 0;
    float num2 = 0.0f;
    if (num1 != 0)
    {
      num2 = SoundUtil.GetLiquidDepth(cell);
      string sound = GlobalAssets.GetSound("Liquid_footstep", true);
      if (sound != null && (this.objectIsSelectedAndVisible || SoundEvent.ShouldPlaySound(behaviour.controller, sound, this.looping, this.isDynamic)))
      {
        EventInstance instance = SoundEvent.BeginOneShot(sound, vector3, SoundEvent.GetVolume(this.objectIsSelectedAndVisible));
        if ((double) num2 > 0.0)
          ((EventInstance) ref instance).setParameterByName("liquidDepth", num2, false);
        SoundEvent.EndOneShot(instance);
      }
    }
    if (str == null || !this.objectIsSelectedAndVisible && !SoundEvent.ShouldPlaySound(behaviour.controller, str, this.looping, this.isDynamic))
      return;
    EventInstance instance1 = SoundEvent.BeginOneShot(str, vector3);
    if (!((EventInstance) ref instance1).isValid())
      return;
    if ((double) num2 > 0.0)
      ((EventInstance) ref instance1).setParameterByName("liquidDepth", num2, false);
    if (behaviour.controller.HasAnimationFile(KAnimHashedString.op_Implicit("anim_loco_walk_kanim")))
      ((EventInstance) ref instance1).setVolume(FloorSoundEvent.IDLE_WALKING_VOLUME_REDUCTION);
    SoundEvent.EndOneShot(instance1);
  }

  private static string GetAudioCategory(int cell)
  {
    if (!Grid.IsValidCell(cell))
      return "Rock";
    Element element = Grid.Element[cell];
    if (Grid.Foundation[cell])
    {
      BuildingDef buildingDef = (BuildingDef) null;
      GameObject gameObject = Grid.Objects[cell, 1];
      if (Object.op_Inequality((Object) gameObject, (Object) null))
      {
        Building component = (Building) gameObject.GetComponent<BuildingComplete>();
        if (Object.op_Inequality((Object) component, (Object) null))
          buildingDef = component.Def;
      }
      string audioCategory = "";
      if (Object.op_Inequality((Object) buildingDef, (Object) null))
      {
        switch (buildingDef.PrefabID)
        {
          case "PlasticTile":
            audioCategory = "TilePlastic";
            break;
          case "GlassTile":
            audioCategory = "TileGlass";
            break;
          case "BunkerTile":
            audioCategory = "TileBunker";
            break;
          case "MetalTile":
            audioCategory = "TileMetal";
            break;
          case "CarpetTile":
            audioCategory = "Carpet";
            break;
          default:
            audioCategory = "Tile";
            break;
        }
      }
      return audioCategory;
    }
    string eventAudioCategory = element.substance.GetFloorEventAudioCategory();
    if (eventAudioCategory != null)
      return eventAudioCategory;
    if (element.HasTag(GameTags.RefinedMetal))
      return "RefinedMetal";
    return element.HasTag(GameTags.Metal) ? "RawMetal" : "Rock";
  }
}
