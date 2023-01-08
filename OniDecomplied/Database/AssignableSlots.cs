// Decompiled with JetBrains decompiler
// Type: Database.AssignableSlots
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

namespace Database
{
  public class AssignableSlots : ResourceSet<AssignableSlot>
  {
    public AssignableSlot Bed;
    public AssignableSlot MessStation;
    public AssignableSlot Clinic;
    public AssignableSlot GeneShuffler;
    public AssignableSlot MedicalBed;
    public AssignableSlot Toilet;
    public AssignableSlot MassageTable;
    public AssignableSlot RocketCommandModule;
    public AssignableSlot HabitatModule;
    public AssignableSlot ResetSkillsStation;
    public AssignableSlot WarpPortal;
    public AssignableSlot Toy;
    public AssignableSlot Suit;
    public AssignableSlot Tool;
    public AssignableSlot Outfit;

    public AssignableSlots()
    {
      this.Bed = this.Add((AssignableSlot) new OwnableSlot(nameof (Bed), (string) MISC.TAGS.BED));
      this.MessStation = this.Add((AssignableSlot) new OwnableSlot(nameof (MessStation), (string) MISC.TAGS.MESSSTATION));
      this.Clinic = this.Add((AssignableSlot) new OwnableSlot(nameof (Clinic), (string) MISC.TAGS.CLINIC));
      this.MedicalBed = this.Add((AssignableSlot) new OwnableSlot(nameof (MedicalBed), (string) MISC.TAGS.CLINIC));
      this.MedicalBed.showInUI = false;
      this.GeneShuffler = this.Add((AssignableSlot) new OwnableSlot(nameof (GeneShuffler), (string) MISC.TAGS.GENE_SHUFFLER));
      this.GeneShuffler.showInUI = false;
      this.Toilet = this.Add((AssignableSlot) new OwnableSlot(nameof (Toilet), (string) MISC.TAGS.TOILET));
      this.MassageTable = this.Add((AssignableSlot) new OwnableSlot(nameof (MassageTable), (string) MISC.TAGS.MASSAGE_TABLE));
      this.RocketCommandModule = this.Add((AssignableSlot) new OwnableSlot(nameof (RocketCommandModule), (string) MISC.TAGS.COMMAND_MODULE));
      this.HabitatModule = this.Add((AssignableSlot) new OwnableSlot(nameof (HabitatModule), (string) MISC.TAGS.HABITAT_MODULE));
      this.ResetSkillsStation = this.Add((AssignableSlot) new OwnableSlot(nameof (ResetSkillsStation), nameof (ResetSkillsStation)));
      this.WarpPortal = this.Add((AssignableSlot) new OwnableSlot(nameof (WarpPortal), (string) MISC.TAGS.WARP_PORTAL));
      this.WarpPortal.showInUI = false;
      this.Toy = this.Add((AssignableSlot) new EquipmentSlot(TUNING.EQUIPMENT.TOYS.SLOT, (string) MISC.TAGS.TOY, false));
      this.Suit = this.Add((AssignableSlot) new EquipmentSlot(TUNING.EQUIPMENT.SUITS.SLOT, (string) MISC.TAGS.SUIT));
      this.Tool = this.Add((AssignableSlot) new EquipmentSlot(TUNING.EQUIPMENT.TOOLS.TOOLSLOT, (string) MISC.TAGS.MULTITOOL, false));
      this.Outfit = this.Add((AssignableSlot) new EquipmentSlot(TUNING.EQUIPMENT.CLOTHING.SLOT, (string) MISC.TAGS.CLOTHES));
    }
  }
}
