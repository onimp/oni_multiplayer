// Decompiled with JetBrains decompiler
// Type: Database.StatusItemCategories
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace Database
{
  public class StatusItemCategories : ResourceSet<StatusItemCategory>
  {
    public StatusItemCategory Main;
    public StatusItemCategory Role;
    public StatusItemCategory Power;
    public StatusItemCategory Toilet;
    public StatusItemCategory Research;
    public StatusItemCategory Hitpoints;
    public StatusItemCategory Suffocation;
    public StatusItemCategory WoundEffects;
    public StatusItemCategory EntityReceptacle;
    public StatusItemCategory PreservationState;
    public StatusItemCategory PreservationAtmosphere;
    public StatusItemCategory PreservationTemperature;
    public StatusItemCategory ExhaustTemperature;
    public StatusItemCategory OperatingEnergy;
    public StatusItemCategory AccessControl;
    public StatusItemCategory RequiredRoom;
    public StatusItemCategory Yield;
    public StatusItemCategory Heat;
    public StatusItemCategory Stored;
    public StatusItemCategory Ownable;

    public StatusItemCategories(ResourceSet parent)
      : base(nameof (StatusItemCategories), parent)
    {
      this.Main = new StatusItemCategory(nameof (Main), (ResourceSet) this, nameof (Main));
      this.Role = new StatusItemCategory(nameof (Role), (ResourceSet) this, nameof (Role));
      this.Power = new StatusItemCategory(nameof (Power), (ResourceSet) this, nameof (Power));
      this.Toilet = new StatusItemCategory(nameof (Toilet), (ResourceSet) this, nameof (Toilet));
      this.Research = new StatusItemCategory(nameof (Research), (ResourceSet) this, nameof (Research));
      this.Hitpoints = new StatusItemCategory(nameof (Hitpoints), (ResourceSet) this, nameof (Hitpoints));
      this.Suffocation = new StatusItemCategory(nameof (Suffocation), (ResourceSet) this, nameof (Suffocation));
      this.WoundEffects = new StatusItemCategory(nameof (WoundEffects), (ResourceSet) this, nameof (WoundEffects));
      this.EntityReceptacle = new StatusItemCategory(nameof (EntityReceptacle), (ResourceSet) this, nameof (EntityReceptacle));
      this.PreservationState = new StatusItemCategory(nameof (PreservationState), (ResourceSet) this, nameof (PreservationState));
      this.PreservationTemperature = new StatusItemCategory(nameof (PreservationTemperature), (ResourceSet) this, nameof (PreservationTemperature));
      this.PreservationAtmosphere = new StatusItemCategory(nameof (PreservationAtmosphere), (ResourceSet) this, nameof (PreservationAtmosphere));
      this.ExhaustTemperature = new StatusItemCategory(nameof (ExhaustTemperature), (ResourceSet) this, nameof (ExhaustTemperature));
      this.OperatingEnergy = new StatusItemCategory(nameof (OperatingEnergy), (ResourceSet) this, nameof (OperatingEnergy));
      this.AccessControl = new StatusItemCategory(nameof (AccessControl), (ResourceSet) this, nameof (AccessControl));
      this.RequiredRoom = new StatusItemCategory(nameof (RequiredRoom), (ResourceSet) this, nameof (RequiredRoom));
      this.Yield = new StatusItemCategory(nameof (Yield), (ResourceSet) this, nameof (Yield));
      this.Heat = new StatusItemCategory(nameof (Heat), (ResourceSet) this, nameof (Heat));
      this.Stored = new StatusItemCategory(nameof (Stored), (ResourceSet) this, nameof (Stored));
      this.Ownable = new StatusItemCategory(nameof (Ownable), (ResourceSet) this, nameof (Ownable));
    }
  }
}
