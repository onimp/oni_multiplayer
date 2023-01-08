// Decompiled with JetBrains decompiler
// Type: Database.Stories
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ProcGen;
using System;
using UnityEngine;

namespace Database
{
  public class Stories : ResourceSet<Story>
  {
    public Story MegaBrainTank;
    public Story CreatureManipulator;
    public Story LonelyMinion;

    public Stories(ResourceSet parent)
      : base(nameof (Stories), parent)
    {
      this.MegaBrainTank = this.Add(new Story(nameof (MegaBrainTank), "storytraits/MegaBrainTank", 0, 1, 43).SetKeepsake("keepsake_megabrain"));
      this.CreatureManipulator = this.Add(new Story(nameof (CreatureManipulator), "storytraits/CritterManipulator", 1, 2, 43).SetKeepsake("keepsake_crittermanipulator"));
      this.LonelyMinion = this.Add(new Story(nameof (LonelyMinion), "storytraits/LonelyMinion", 2, 3, 44).SetKeepsake("keepsake_lonelyminion"));
      this.resources.Sort();
    }

    public void AddStoryMod(Story mod)
    {
      mod.kleiUseOnlyCoordinateOffset = -1;
      this.Add(mod);
      this.resources.Sort();
    }

    public int GetHighestCoordinateOffset()
    {
      int coordinateOffset = 0;
      foreach (Story resource in this.resources)
        coordinateOffset = Mathf.Max(coordinateOffset, resource.kleiUseOnlyCoordinateOffset);
      return coordinateOffset;
    }

    public WorldTrait GetStoryTrait(string id, bool assertMissingTrait = false)
    {
      Story story = this.resources.Find((Predicate<Story>) (x => x.Id == id));
      return story != null ? SettingsCache.GetCachedStoryTrait(story.worldgenStoryTraitKey, assertMissingTrait) : (WorldTrait) null;
    }

    public Story GetStoryFromStoryTrait(string storyTraitTemplate) => this.resources.Find((Predicate<Story>) (x => x.worldgenStoryTraitKey == storyTraitTemplate));
  }
}
