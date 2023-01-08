// Decompiled with JetBrains decompiler
// Type: RepairableEquipment
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using UnityEngine;

public class RepairableEquipment : KMonoBehaviour
{
  public DefHandle defHandle;
  [Serialize]
  public string facadeID;

  public EquipmentDef def
  {
    get => ((DefHandle) ref this.defHandle).Get<EquipmentDef>();
    set => ((DefHandle) ref this.defHandle).Set<EquipmentDef>(value);
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    if (this.def.AdditionalTags == null)
      return;
    foreach (Tag additionalTag in this.def.AdditionalTags)
      ((Component) this).GetComponent<KPrefabID>().AddTag(additionalTag, false);
  }

  protected virtual void OnSpawn()
  {
    if (Util.IsNullOrWhiteSpace(this.facadeID))
      return;
    KAnim.Build.Symbol symbol = Db.GetEquippableFacades().Get(this.facadeID).AnimFile.GetData().build.GetSymbol(KAnimHashedString.op_Implicit("object"));
    SymbolOverrideController component = ((Component) this).GetComponent<SymbolOverrideController>();
    component.TryRemoveSymbolOverride(HashedString.op_Implicit("object"));
    component.AddSymbolOverride(HashedString.op_Implicit("object"), symbol);
  }
}
