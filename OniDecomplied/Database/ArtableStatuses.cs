// Decompiled with JetBrains decompiler
// Type: Database.ArtableStatuses
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace Database
{
  public class ArtableStatuses : ResourceSet<ArtableStatusItem>
  {
    public ArtableStatusItem AwaitingArting;
    public ArtableStatusItem LookingUgly;
    public ArtableStatusItem LookingOkay;
    public ArtableStatusItem LookingGreat;

    public ArtableStatuses(ResourceSet parent)
      : base(nameof (ArtableStatuses), parent)
    {
      this.AwaitingArting = this.Add(nameof (AwaitingArting), ArtableStatuses.ArtableStatusType.AwaitingArting);
      this.LookingUgly = this.Add(nameof (LookingUgly), ArtableStatuses.ArtableStatusType.LookingUgly);
      this.LookingOkay = this.Add(nameof (LookingOkay), ArtableStatuses.ArtableStatusType.LookingOkay);
      this.LookingGreat = this.Add(nameof (LookingGreat), ArtableStatuses.ArtableStatusType.LookingGreat);
    }

    public ArtableStatusItem Add(string id, ArtableStatuses.ArtableStatusType statusType)
    {
      ArtableStatusItem artableStatusItem = new ArtableStatusItem(id, statusType);
      this.resources.Add(artableStatusItem);
      return artableStatusItem;
    }

    public enum ArtableStatusType
    {
      AwaitingArting,
      LookingUgly,
      LookingOkay,
      LookingGreat,
    }
  }
}
