// Decompiled with JetBrains decompiler
// Type: DevTool_StoryTrait_CritterManipulator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ImGuiNET;
using STRINGS;
using System.Collections.Generic;

public class DevTool_StoryTrait_CritterManipulator : DevTool
{
  protected override void RenderTo(DevPanel panel)
  {
    if (!ImGui.CollapsingHeader("Debug species lore unlock popup", (ImGuiTreeNodeFlags) 32))
      return;
    this.Button_OpenSpecies(Tag.Invalid, "Unknown Species");
    ImGui.Separator();
    foreach (Tag critterSpeciesTag in this.GetCritterSpeciesTags())
      this.Button_OpenSpecies(critterSpeciesTag, (string) GravitasCreatureManipulatorConfig.GetNameForSpeciesTag(critterSpeciesTag));
  }

  public void Button_OpenSpecies(Tag species, string speciesName = null)
  {
    speciesName = speciesName != null ? string.Format("\"{0}\" (ID: {1})", (object) UI.StripLinkFormatting(speciesName), (object) species) : ((Tag) ref species).Name;
    if (!ImGui.Button("Show popup for: " + speciesName))
      return;
    GravitasCreatureManipulator.Instance.ShowLoreUnlockedPopup(species);
  }

  public IEnumerable<Tag> GetCritterSpeciesTags()
  {
    yield return GameTags.Creatures.Species.HatchSpecies;
    yield return GameTags.Creatures.Species.LightBugSpecies;
    yield return GameTags.Creatures.Species.OilFloaterSpecies;
    yield return GameTags.Creatures.Species.DreckoSpecies;
    yield return GameTags.Creatures.Species.GlomSpecies;
    yield return GameTags.Creatures.Species.PuftSpecies;
    yield return GameTags.Creatures.Species.PacuSpecies;
    yield return GameTags.Creatures.Species.MooSpecies;
    yield return GameTags.Creatures.Species.MoleSpecies;
    yield return GameTags.Creatures.Species.SquirrelSpecies;
    yield return GameTags.Creatures.Species.CrabSpecies;
    yield return GameTags.Creatures.Species.DivergentSpecies;
    yield return GameTags.Creatures.Species.StaterpillarSpecies;
    yield return GameTags.Creatures.Species.BeetaSpecies;
  }
}
