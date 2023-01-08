// Decompiled with JetBrains decompiler
// Type: GeneticAnalysisCompleteMessage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;

public class GeneticAnalysisCompleteMessage : Message
{
  [Serialize]
  public Tag subSpeciesID;

  public GeneticAnalysisCompleteMessage()
  {
  }

  public GeneticAnalysisCompleteMessage(Tag subSpeciesID) => this.subSpeciesID = subSpeciesID;

  public override string GetSound() => "";

  public override string GetMessageBody()
  {
    PlantSubSpeciesCatalog.SubSpeciesInfo subSpecies = PlantSubSpeciesCatalog.Instance.FindSubSpecies(this.subSpeciesID);
    return MISC.NOTIFICATIONS.GENETICANALYSISCOMPLETE.MESSAGEBODY.Replace("{Plant}", subSpecies.speciesID.ProperName()).Replace("{Subspecies}", subSpecies.GetNameWithMutations(subSpecies.speciesID.ProperName(), true, false)).Replace("{Info}", subSpecies.GetMutationsTooltip());
  }

  public override string GetTitle() => (string) MISC.NOTIFICATIONS.GENETICANALYSISCOMPLETE.NAME;

  public override string GetTooltip()
  {
    PlantSubSpeciesCatalog.SubSpeciesInfo subSpecies = PlantSubSpeciesCatalog.Instance.FindSubSpecies(this.subSpeciesID);
    return MISC.NOTIFICATIONS.GENETICANALYSISCOMPLETE.TOOLTIP.Replace("{Plant}", subSpecies.speciesID.ProperName());
  }

  public override bool IsValid() => ((Tag) ref this.subSpeciesID).IsValid;
}
