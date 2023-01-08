// Decompiled with JetBrains decompiler
// Type: DuplicantNoiseLevels
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/DuplicantNoiseLevels")]
public class DuplicantNoiseLevels : KMonoBehaviour
{
  public static void SetupNoiseLevels()
  {
    SoundEventVolumeCache.instance.AddVolume("anim_eat_floor_kanim", "Chew_food_gross", NOISE_POLLUTION.CREATURES.TIER2);
    SoundEventVolumeCache.instance.AddVolume("anim_eat_floor_kanim", "Chew_food_first", NOISE_POLLUTION.CREATURES.TIER2);
    SoundEventVolumeCache.instance.AddVolume("anim_eat_overeat_kanim", "Chew_food_gross", NOISE_POLLUTION.CREATURES.TIER2);
    SoundEventVolumeCache.instance.AddVolume("anim_eat_overeat_kanim", "Chew_food_first", NOISE_POLLUTION.CREATURES.TIER2);
    SoundEventVolumeCache.instance.AddVolume("anim_eat_table_kanim", "Chew_food_gross", NOISE_POLLUTION.CREATURES.TIER2);
    SoundEventVolumeCache.instance.AddVolume("anim_eat_table_kanim", "Chew_food_first", NOISE_POLLUTION.CREATURES.TIER2);
    SoundEventVolumeCache.instance.AddVolume("anim_mop_dirtywater_kanim", "Mop_liquid", NOISE_POLLUTION.CREATURES.TIER2);
    SoundEventVolumeCache.instance.AddVolume("anim_loco_destructive_kanim", "Ladder_footstep", NOISE_POLLUTION.CREATURES.TIER2);
    SoundEventVolumeCache.instance.AddVolume("anim_loco_new_kanim", "Ladder_footstep", NOISE_POLLUTION.CREATURES.TIER2);
    SoundEventVolumeCache.instance.AddVolume("anim_loco_wounded_kanim", "Ladder_footstep", NOISE_POLLUTION.CREATURES.TIER2);
    SoundEventVolumeCache.instance.AddVolume("anim_expel_kanim", "Expel_waste", NOISE_POLLUTION.CREATURES.TIER3);
    SoundEventVolumeCache.instance.AddVolume("anim_interacts_apothecary_kanim", "Apothecary_grind", NOISE_POLLUTION.CREATURES.TIER3);
    SoundEventVolumeCache.instance.AddVolume("anim_interrupt_binge_eat_kanim", "Hungry_tooth_chomps", NOISE_POLLUTION.CREATURES.TIER3);
    SoundEventVolumeCache.instance.AddVolume("anim_out_of_reach_binge_eat_kanim", "Hungry_tooth_chomps", NOISE_POLLUTION.CREATURES.TIER3);
    SoundEventVolumeCache.instance.AddVolume("anim_interrupt_binge_eat_kanim", "Stomach_grumble", NOISE_POLLUTION.CREATURES.TIER3);
    SoundEventVolumeCache.instance.AddVolume("anim_emotes_default_kanim", "Bodyfall_rock", NOISE_POLLUTION.CREATURES.TIER4);
    SoundEventVolumeCache.instance.AddVolume("anim_loco_new_kanim", "Bodyfall_rock", NOISE_POLLUTION.CREATURES.TIER4);
    SoundEventVolumeCache.instance.AddVolume("anim_incapacitated_kanim", "Bodyfall_rock", NOISE_POLLUTION.CREATURES.TIER4);
    SoundEventVolumeCache.instance.AddVolume("anim_interacts_portal_kanim", "Bodyfall_rock", NOISE_POLLUTION.CREATURES.TIER4);
    SoundEventVolumeCache.instance.AddVolume("anim_interacts_generatormanual_kanim", "Bodyfall_rock", NOISE_POLLUTION.CREATURES.TIER4);
    SoundEventVolumeCache.instance.AddVolume("anim_equip_clothing_kanim", "Dressed_impact", NOISE_POLLUTION.CREATURES.TIER4);
    SoundEventVolumeCache.instance.AddVolume("anim_healing_bed_kanim", "Healed_impact", NOISE_POLLUTION.CREATURES.TIER4);
    SoundEventVolumeCache.instance.AddVolume("anim_interacts_outhouse_kanim", "Plunge_toilet", NOISE_POLLUTION.CREATURES.TIER4);
    SoundEventVolumeCache.instance.AddVolume("anim_interrupt_vomiter_kanim", "Burp", NOISE_POLLUTION.CREATURES.TIER4);
    SoundEventVolumeCache.instance.AddVolume("anim_break_kanim", "Throw_gears", NOISE_POLLUTION.CREATURES.TIER5);
    SoundEventVolumeCache.instance.AddVolume("anim_interacts_sculpture_kanim", "Hammer_sculpture", NOISE_POLLUTION.CREATURES.TIER5);
    SoundEventVolumeCache.instance.AddVolume("anim_break_kanim", "Kick_building", NOISE_POLLUTION.CREATURES.TIER6);
    SoundEventVolumeCache.instance.AddVolume("anim_emotes_default_kanim", "Kick_building", NOISE_POLLUTION.CREATURES.TIER6);
    SoundEventVolumeCache.instance.AddVolume("anim_loco_destructive_kanim", "Destructive_Punch", NOISE_POLLUTION.CREATURES.TIER6);
    SoundEventVolumeCache.instance.AddVolume("anim_out_of_reach_destructive_low_kanim", "Destructive_Punch", NOISE_POLLUTION.CREATURES.TIER6);
    SoundEventVolumeCache.instance.AddVolume("anim_out_of_reach_destructive_low_kanim", "Destructive_Headbash", NOISE_POLLUTION.CREATURES.TIER6);
    SoundEventVolumeCache.instance.AddVolume("anim_out_of_reach_destructive_high_kanim", "Destructive_Punch", NOISE_POLLUTION.CREATURES.TIER6);
    SoundEventVolumeCache.instance.AddVolume("anim_out_of_reach_destructive_high_kanim", "Destructive_Headbash", NOISE_POLLUTION.CREATURES.TIER6);
    SoundEventVolumeCache.instance.AddVolume("anim_sneeze_kanim", "Dupe_sneeze", NOISE_POLLUTION.CREATURES.TIER6);
    SoundEventVolumeCache.instance.AddVolume("FloorSoundEvent", "footstep", NOISE_POLLUTION.CREATURES.TIER1);
    SoundEventVolumeCache.instance.AddVolume("FloorSoundEvent", "land", NOISE_POLLUTION.CREATURES.TIER2);
    SoundEventVolumeCache.instance.AddVolume("FloorSoundEvent", "jump", NOISE_POLLUTION.CREATURES.TIER2);
    SoundEventVolumeCache.instance.AddVolume("FloorSoundEvent", "climb", NOISE_POLLUTION.CREATURES.TIER2);
    SoundEventVolumeCache.instance.AddVolume("FloorSoundEvent", "scoot", NOISE_POLLUTION.CREATURES.TIER2);
    SoundEventVolumeCache.instance.AddVolume("VoiceSoundEvent", "sleep_inhale", NOISE_POLLUTION.CREATURES.TIER0);
    SoundEventVolumeCache.instance.AddVolume("VoiceSoundEvent", "sleep_exhale", NOISE_POLLUTION.CREATURES.TIER0);
    SoundEventVolumeCache.instance.AddVolume("VoiceSoundEvent", "voice_crying", NOISE_POLLUTION.CREATURES.TIER0);
    SoundEventVolumeCache.instance.AddVolume("VoiceSoundEvent", "voice_land", NOISE_POLLUTION.CREATURES.TIER2);
    SoundEventVolumeCache.instance.AddVolume("VoiceSoundEvent", "voice_jump", NOISE_POLLUTION.CREATURES.TIER2);
    SoundEventVolumeCache.instance.AddVolume("VoiceSoundEvent", "voice_die", NOISE_POLLUTION.CREATURES.TIER2);
    SoundEventVolumeCache.instance.AddVolume("VoiceSoundEvent", "voice_sick", NOISE_POLLUTION.CREATURES.TIER2);
    SoundEventVolumeCache.instance.AddVolume("VoiceSoundEvent", "voice_meh", NOISE_POLLUTION.CREATURES.TIER3);
    SoundEventVolumeCache.instance.AddVolume("VoiceSoundEvent", "voice_flinch", NOISE_POLLUTION.CREATURES.TIER3);
    SoundEventVolumeCache.instance.AddVolume("VoiceSoundEvent", "voice_destructive_rage", NOISE_POLLUTION.CREATURES.TIER3);
    SoundEventVolumeCache.instance.AddVolume("VoiceSoundEvent", "voice_woohoo", NOISE_POLLUTION.CREATURES.TIER5);
    SoundEventVolumeCache.instance.AddVolume("VoiceSoundEvent", "voice_incapacitated", NOISE_POLLUTION.CREATURES.TIER5);
    SoundEventVolumeCache.instance.AddVolume("VoiceSoundEvent", "voice_vomit", NOISE_POLLUTION.CREATURES.TIER5);
    SoundEventVolumeCache.instance.AddVolume("VoiceSoundEvent", "voice_falling", NOISE_POLLUTION.CREATURES.TIER5);
    SoundEventVolumeCache.instance.AddVolume("LaserSoundEvent", "DigLaser_gun", NOISE_POLLUTION.CREATURES.TIER3);
    SoundEventVolumeCache.instance.AddVolume("LaserSoundEvent", "BuildLaser_gun", NOISE_POLLUTION.CREATURES.TIER3);
    SoundEventVolumeCache.instance.AddVolume("LaserSoundEvent", "WaterLaser_gun", NOISE_POLLUTION.CREATURES.TIER3);
    SoundEventVolumeCache.instance.AddVolume("LaserSoundEvent", "PaintLaser_gun", NOISE_POLLUTION.CREATURES.TIER3);
    SoundEventVolumeCache.instance.AddVolume("LaserSoundEvent", "HarvestLaser_object", NOISE_POLLUTION.CREATURES.TIER3);
    SoundEventVolumeCache.instance.AddVolume("LaserSoundEvent", "AttackLaser_gun", NOISE_POLLUTION.CREATURES.TIER3);
    SoundEventVolumeCache.instance.AddVolume("LaserSoundEvent", "CollectLaser_gun_shoot", NOISE_POLLUTION.CREATURES.TIER3);
    SoundEventVolumeCache.instance.AddVolume("LaserSoundEvent", "CollectLaser_gun", NOISE_POLLUTION.CREATURES.TIER3);
  }
}
