// Decompiled with JetBrains decompiler
// Type: Ladder
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/Ladder")]
public class Ladder : KMonoBehaviour, IGameObjectEffectDescriptor
{
  public float upwardsMovementSpeedMultiplier = 1f;
  public float downwardsMovementSpeedMultiplier = 1f;
  public bool isPole;
  public CellOffset[] offsets = new CellOffset[1]
  {
    CellOffset.none
  };

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    Rotatable component = ((Component) this).GetComponent<Rotatable>();
    foreach (CellOffset offset1 in this.offsets)
    {
      CellOffset offset2 = offset1;
      if (Object.op_Inequality((Object) component, (Object) null))
        offset2 = component.GetRotatedCellOffset(offset1);
      int i = Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) this), offset2);
      Grid.HasPole[i] = this.isPole;
      Grid.HasLadder[i] = !this.isPole;
    }
    ((Component) this).GetComponent<KPrefabID>().AddTag(GameTags.Ladders, false);
    Components.Ladders.Add(this);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    ((Component) this).GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Normal);
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    Rotatable component = ((Component) this).GetComponent<Rotatable>();
    foreach (CellOffset offset1 in this.offsets)
    {
      CellOffset offset2 = offset1;
      if (Object.op_Inequality((Object) component, (Object) null))
        offset2 = component.GetRotatedCellOffset(offset1);
      int num = Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) this), offset2);
      if (Object.op_Equality((Object) Grid.Objects[num, 24], (Object) null))
      {
        Grid.HasPole[num] = false;
        Grid.HasLadder[num] = false;
      }
    }
    Components.Ladders.Remove(this);
  }

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = (List<Descriptor>) null;
    if ((double) this.upwardsMovementSpeedMultiplier != 1.0)
    {
      descriptors = new List<Descriptor>();
      Descriptor descriptor = new Descriptor();
      ((Descriptor) ref descriptor).SetupDescriptor(string.Format((string) UI.BUILDINGEFFECTS.DUPLICANTMOVEMENTBOOST, (object) GameUtil.GetFormattedPercent((float) ((double) this.upwardsMovementSpeedMultiplier * 100.0 - 100.0))), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.DUPLICANTMOVEMENTBOOST, (object) GameUtil.GetFormattedPercent((float) ((double) this.upwardsMovementSpeedMultiplier * 100.0 - 100.0))), (Descriptor.DescriptorType) 1);
      descriptors.Add(descriptor);
    }
    return descriptors;
  }
}
