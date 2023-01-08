// Decompiled with JetBrains decompiler
// Type: MinimumOperatingTemperature
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/MinimumOperatingTemperature")]
public class MinimumOperatingTemperature : KMonoBehaviour, ISim200ms, IGameObjectEffectDescriptor
{
  [MyCmpReq]
  private Building building;
  [MyCmpReq]
  private Operational operational;
  [MyCmpReq]
  private PrimaryElement primaryElement;
  public float minimumTemperature = 275.15f;
  private const float TURN_ON_DELAY = 5f;
  private float lastOffTime;
  public static Operational.Flag warmEnoughFlag = new Operational.Flag("warm_enough", Operational.Flag.Type.Functional);
  private bool isWarm;
  private HandleVector<int>.Handle partitionerEntry;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.TestTemperature(true);
  }

  public void Sim200ms(float dt) => this.TestTemperature(false);

  private void TestTemperature(bool force)
  {
    bool flag;
    if ((double) this.primaryElement.Temperature < (double) this.minimumTemperature)
    {
      flag = false;
    }
    else
    {
      flag = true;
      for (int index = 0; index < this.building.PlacementCells.Length; ++index)
      {
        int placementCell = this.building.PlacementCells[index];
        float num1 = Grid.Temperature[placementCell];
        float num2 = Grid.Mass[placementCell];
        if (((double) num1 != 0.0 || (double) num2 != 0.0) && (double) num1 < (double) this.minimumTemperature)
        {
          flag = false;
          break;
        }
      }
    }
    if (!flag)
      this.lastOffTime = Time.time;
    if (((flag == this.isWarm || flag ? (flag == this.isWarm || !flag ? 0 : ((double) Time.time > (double) this.lastOffTime + 5.0 ? 1 : 0)) : 1) | (force ? 1 : 0)) == 0)
      return;
    this.isWarm = flag;
    this.operational.SetFlag(MinimumOperatingTemperature.warmEnoughFlag, this.isWarm);
    ((Component) this).GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.TooCold, !this.isWarm, (object) this);
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
  }

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    Descriptor descriptor;
    // ISSUE: explicit constructor call
    ((Descriptor) ref descriptor).\u002Ector(string.Format((string) UI.BUILDINGEFFECTS.MINIMUM_TEMP, (object) GameUtil.GetFormattedTemperature(this.minimumTemperature)), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.MINIMUM_TEMP, (object) GameUtil.GetFormattedTemperature(this.minimumTemperature)), (Descriptor.DescriptorType) 1, false);
    descriptors.Add(descriptor);
    return descriptors;
  }
}
