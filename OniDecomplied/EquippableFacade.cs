// Decompiled with JetBrains decompiler
// Type: EquippableFacade
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using UnityEngine;

public class EquippableFacade : KMonoBehaviour
{
  [Serialize]
  private string _facadeID;
  [Serialize]
  public string BuildOverride;

  public static void AddFacadeToEquippable(Equippable equippable, string facadeID)
  {
    EquippableFacade equippableFacade = ((Component) equippable).gameObject.AddOrGet<EquippableFacade>();
    equippableFacade.FacadeID = facadeID;
    equippableFacade.BuildOverride = Db.GetEquippableFacades().Get(facadeID).BuildOverride;
    equippableFacade.ApplyAnimOverride();
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.OverrideName();
    this.ApplyAnimOverride();
  }

  public string FacadeID
  {
    get => this._facadeID;
    private set
    {
      this._facadeID = value;
      this.OverrideName();
    }
  }

  public void ApplyAnimOverride()
  {
    if (Util.IsNullOrWhiteSpace(this.FacadeID))
      return;
    ((Component) this).GetComponent<KBatchedAnimController>().SwapAnims(new KAnimFile[1]
    {
      Db.GetEquippableFacades().Get(this.FacadeID).AnimFile
    });
  }

  private void OverrideName() => ((Component) this).GetComponent<KSelectable>().SetName(EquippableFacade.GetNameOverride(((Component) this).GetComponent<Equippable>().def.Id, this.FacadeID));

  public static string GetNameOverride(string defID, string facadeID) => Util.IsNullOrWhiteSpace(facadeID) ? StringEntry.op_Implicit(Strings.Get("STRINGS.EQUIPMENT.PREFABS." + defID.ToUpper() + ".NAME")) : Db.GetEquippableFacades().Get(facadeID).Name;
}
