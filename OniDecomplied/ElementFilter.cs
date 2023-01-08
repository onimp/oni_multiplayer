// Decompiled with JetBrains decompiler
// Type: ElementFilter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/ElementFilter")]
public class ElementFilter : KMonoBehaviour, ISaveLoadable, ISecondaryOutput
{
  [SerializeField]
  public ConduitPortInfo portInfo;
  [MyCmpReq]
  private Operational operational;
  [MyCmpReq]
  private Building building;
  [MyCmpReq]
  private KSelectable selectable;
  [MyCmpReq]
  private Filterable filterable;
  private Guid needsConduitStatusItemGuid;
  private Guid conduitBlockedStatusItemGuid;
  private int inputCell = -1;
  private int outputCell = -1;
  private int filteredCell = -1;
  private FlowUtilityNetwork.NetworkItem itemFilter;
  private HandleVector<int>.Handle partitionerEntry;
  private static StatusItem filterStatusItem;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.InitializeStatusItems();
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.inputCell = this.building.GetUtilityInputCell();
    this.outputCell = this.building.GetUtilityOutputCell();
    this.filteredCell = Grid.OffsetCell(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)), this.building.GetRotatedOffset(this.portInfo.offset));
    IUtilityNetworkMgr utilityNetworkMgr = this.portInfo.conduitType == ConduitType.Solid ? SolidConduit.GetFlowManager().networkMgr : Conduit.GetNetworkManager(this.portInfo.conduitType);
    this.itemFilter = new FlowUtilityNetwork.NetworkItem(this.portInfo.conduitType, Endpoint.Source, this.filteredCell, ((Component) this).gameObject);
    int filteredCell = this.filteredCell;
    FlowUtilityNetwork.NetworkItem itemFilter = this.itemFilter;
    utilityNetworkMgr.AddToNetworks(filteredCell, (object) itemFilter, true);
    if (this.portInfo.conduitType == ConduitType.Gas || this.portInfo.conduitType == ConduitType.Liquid)
      ((Component) this).GetComponent<ConduitConsumer>().isConsuming = false;
    this.OnFilterChanged(this.filterable.SelectedTag);
    this.filterable.onFilterChanged += new Action<Tag>(this.OnFilterChanged);
    if (this.portInfo.conduitType == ConduitType.Solid)
      SolidConduit.GetFlowManager().AddConduitUpdater(new Action<float>(this.OnConduitTick), ConduitFlowPriority.Default);
    else
      Conduit.GetFlowManager(this.portInfo.conduitType).AddConduitUpdater(new Action<float>(this.OnConduitTick), ConduitFlowPriority.Default);
    ((Component) this).GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, ElementFilter.filterStatusItem, (object) this);
    this.UpdateConduitExistsStatus();
    this.UpdateConduitBlockedStatus();
    ScenePartitionerLayer layer = (ScenePartitionerLayer) null;
    switch (this.portInfo.conduitType)
    {
      case ConduitType.Gas:
        layer = GameScenePartitioner.Instance.gasConduitsLayer;
        break;
      case ConduitType.Liquid:
        layer = GameScenePartitioner.Instance.liquidConduitsLayer;
        break;
      case ConduitType.Solid:
        layer = GameScenePartitioner.Instance.solidConduitsLayer;
        break;
    }
    if (layer == null)
      return;
    this.partitionerEntry = GameScenePartitioner.Instance.Add("ElementFilterConduitExists", (object) ((Component) this).gameObject, this.filteredCell, layer, (Action<object>) (data => this.UpdateConduitExistsStatus()));
  }

  protected virtual void OnCleanUp()
  {
    Conduit.GetNetworkManager(this.portInfo.conduitType).RemoveFromNetworks(this.filteredCell, (object) this.itemFilter, true);
    if (this.portInfo.conduitType == ConduitType.Solid)
      SolidConduit.GetFlowManager().RemoveConduitUpdater(new Action<float>(this.OnConduitTick));
    else
      Conduit.GetFlowManager(this.portInfo.conduitType).RemoveConduitUpdater(new Action<float>(this.OnConduitTick));
    if (this.partitionerEntry.IsValid() && Object.op_Inequality((Object) GameScenePartitioner.Instance, (Object) null))
      GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
    base.OnCleanUp();
  }

  private void OnConduitTick(float dt)
  {
    bool flag = false;
    this.UpdateConduitBlockedStatus();
    if (this.operational.IsOperational)
    {
      if (this.portInfo.conduitType == ConduitType.Gas || this.portInfo.conduitType == ConduitType.Liquid)
      {
        ConduitFlow flowManager = Conduit.GetFlowManager(this.portInfo.conduitType);
        ConduitFlow.ConduitContents contents1 = flowManager.GetContents(this.inputCell);
        int num = Tag.op_Equality(contents1.element.CreateTag(), this.filterable.SelectedTag) ? this.filteredCell : this.outputCell;
        ConduitFlow.ConduitContents contents2 = flowManager.GetContents(num);
        if ((double) contents1.mass > 0.0 && (double) contents2.mass <= 0.0)
        {
          flag = true;
          float delta = flowManager.AddElement(num, contents1.element, contents1.mass, contents1.temperature, contents1.diseaseIdx, contents1.diseaseCount);
          if ((double) delta > 0.0)
            flowManager.RemoveElement(this.inputCell, delta);
        }
      }
      else
      {
        SolidConduitFlow flowManager = SolidConduit.GetFlowManager();
        SolidConduitFlow.ConduitContents contents3 = flowManager.GetContents(this.inputCell);
        Pickupable pickupable1 = flowManager.GetPickupable(contents3.pickupableHandle);
        if (Object.op_Inequality((Object) pickupable1, (Object) null))
        {
          int num = Tag.op_Equality(((Component) pickupable1).GetComponent<KPrefabID>().PrefabTag, this.filterable.SelectedTag) ? this.filteredCell : this.outputCell;
          SolidConduitFlow.ConduitContents contents4 = flowManager.GetContents(num);
          Pickupable pickupable2 = flowManager.GetPickupable(contents4.pickupableHandle);
          PrimaryElement primaryElement = (PrimaryElement) null;
          if (Object.op_Inequality((Object) pickupable2, (Object) null))
            primaryElement = pickupable2.PrimaryElement;
          if ((double) pickupable1.PrimaryElement.Mass > 0.0 && (Object.op_Equality((Object) pickupable2, (Object) null) || (double) primaryElement.Mass <= 0.0))
          {
            flag = true;
            Pickupable pickupable3 = flowManager.RemovePickupable(this.inputCell);
            if (Object.op_Inequality((Object) pickupable3, (Object) null))
              flowManager.AddPickupable(num, pickupable3);
          }
        }
        else
          flowManager.RemovePickupable(this.inputCell);
      }
    }
    this.operational.SetActive(flag);
  }

  private void UpdateConduitExistsStatus()
  {
    bool flag1 = RequireOutputs.IsConnected(this.filteredCell, this.portInfo.conduitType);
    StatusItem status_item;
    switch (this.portInfo.conduitType)
    {
      case ConduitType.Gas:
        status_item = Db.Get().BuildingStatusItems.NeedGasOut;
        break;
      case ConduitType.Liquid:
        status_item = Db.Get().BuildingStatusItems.NeedLiquidOut;
        break;
      case ConduitType.Solid:
        status_item = Db.Get().BuildingStatusItems.NeedSolidOut;
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
    bool flag2 = this.needsConduitStatusItemGuid != Guid.Empty;
    if (flag1 != flag2)
      return;
    this.needsConduitStatusItemGuid = this.selectable.ToggleStatusItem(status_item, this.needsConduitStatusItemGuid, !flag1);
  }

  private void UpdateConduitBlockedStatus()
  {
    bool flag1 = Conduit.GetFlowManager(this.portInfo.conduitType).IsConduitEmpty(this.filteredCell);
    StatusItem blockedMultiples = Db.Get().BuildingStatusItems.ConduitBlockedMultiples;
    bool flag2 = this.conduitBlockedStatusItemGuid != Guid.Empty;
    if (flag1 != flag2)
      return;
    this.conduitBlockedStatusItemGuid = this.selectable.ToggleStatusItem(blockedMultiples, this.conduitBlockedStatusItemGuid, !flag1);
  }

  private void OnFilterChanged(Tag tag)
  {
    bool on = !((Tag) ref tag).IsValid || Tag.op_Equality(tag, GameTags.Void);
    ((Component) this).GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.NoFilterElementSelected, on);
  }

  private void InitializeStatusItems()
  {
    if (ElementFilter.filterStatusItem != null)
      return;
    ElementFilter.filterStatusItem = new StatusItem("Filter", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.LiquidConduits.ID);
    ElementFilter.filterStatusItem.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
    {
      ElementFilter elementFilter = (ElementFilter) data;
      Tag selectedTag = elementFilter.filterable.SelectedTag;
      str = !((Tag) ref selectedTag).IsValid || Tag.op_Equality(elementFilter.filterable.SelectedTag, GameTags.Void) ? string.Format((string) BUILDINGS.PREFABS.GASFILTER.STATUS_ITEM, (object) BUILDINGS.PREFABS.GASFILTER.ELEMENT_NOT_SPECIFIED) : string.Format((string) BUILDINGS.PREFABS.GASFILTER.STATUS_ITEM, (object) elementFilter.filterable.SelectedTag.ProperName());
      return str;
    });
    ElementFilter.filterStatusItem.conditionalOverlayCallback = new Func<HashedString, object, bool>(this.ShowInUtilityOverlay);
  }

  private bool ShowInUtilityOverlay(HashedString mode, object data)
  {
    bool flag = false;
    switch (((ElementFilter) data).portInfo.conduitType)
    {
      case ConduitType.Gas:
        flag = HashedString.op_Equality(mode, OverlayModes.GasConduits.ID);
        break;
      case ConduitType.Liquid:
        flag = HashedString.op_Equality(mode, OverlayModes.LiquidConduits.ID);
        break;
      case ConduitType.Solid:
        flag = HashedString.op_Equality(mode, OverlayModes.SolidConveyor.ID);
        break;
    }
    return flag;
  }

  public bool HasSecondaryConduitType(ConduitType type) => this.portInfo.conduitType == type;

  public CellOffset GetSecondaryConduitOffset(ConduitType type) => this.portInfo.offset;

  public int GetFilteredCell() => this.filteredCell;
}
