// Decompiled with JetBrains decompiler
// Type: SubstanceSource
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using UnityEngine;

[SerializationConfig]
public abstract class SubstanceSource : KMonoBehaviour
{
  private bool enableRefresh;
  private static readonly float MaxPickupTime = 8f;
  [MyCmpReq]
  public Pickupable pickupable;
  [MyCmpReq]
  private PrimaryElement primaryElement;

  protected virtual void OnPrefabInit() => this.pickupable.SetWorkTime(SubstanceSource.MaxPickupTime);

  protected virtual void OnSpawn() => this.pickupable.SetWorkTime(10f);

  protected abstract CellOffset[] GetOffsetGroup();

  protected abstract IChunkManager GetChunkManager();

  public SimHashes GetElementID() => this.primaryElement.ElementID;

  public Tag GetElementTag()
  {
    Tag elementTag = Tag.Invalid;
    if (Object.op_Inequality((Object) ((Component) this).gameObject, (Object) null) && Object.op_Inequality((Object) this.primaryElement, (Object) null) && this.primaryElement.Element != null)
      elementTag = this.primaryElement.Element.tag;
    return elementTag;
  }

  public Tag GetMaterialCategoryTag()
  {
    Tag materialCategoryTag = Tag.Invalid;
    if (Object.op_Inequality((Object) ((Component) this).gameObject, (Object) null) && Object.op_Inequality((Object) this.primaryElement, (Object) null) && this.primaryElement.Element != null)
      materialCategoryTag = this.primaryElement.Element.GetMaterialCategoryTag();
    return materialCategoryTag;
  }
}
