// Decompiled with JetBrains decompiler
// Type: RequireOutputs
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/RequireOutputs")]
public class RequireOutputs : KMonoBehaviour
{
  [MyCmpReq]
  private KSelectable selectable;
  [MyCmpReq]
  private Operational operational;
  public bool ignoreFullPipe;
  private int utilityCell;
  private ConduitType conduitType;
  private static readonly Operational.Flag outputConnectedFlag = new Operational.Flag("output_connected", Operational.Flag.Type.Requirement);
  private static readonly Operational.Flag pipesHaveRoomFlag = new Operational.Flag("pipesHaveRoom", Operational.Flag.Type.Requirement);
  private bool previouslyConnected = true;
  private bool previouslyHadRoom = true;
  private bool connected;
  private Guid hasPipeGuid;
  private Guid pipeBlockedGuid;
  private HandleVector<int>.Handle partitionerEntry;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    ScenePartitionerLayer layer = (ScenePartitionerLayer) null;
    Building component = ((Component) this).GetComponent<Building>();
    this.utilityCell = component.GetUtilityOutputCell();
    this.conduitType = component.Def.OutputConduitType;
    switch (component.Def.OutputConduitType)
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
    this.UpdateConnectionState(true);
    this.UpdatePipeRoomState(true);
    if (layer != null)
      this.partitionerEntry = GameScenePartitioner.Instance.Add(nameof (RequireOutputs), (object) ((Component) this).gameObject, this.utilityCell, layer, (Action<object>) (data => this.UpdateConnectionState()));
    this.GetConduitFlow().AddConduitUpdater(new Action<float>(this.UpdatePipeState), ConduitFlowPriority.First);
  }

  protected virtual void OnCleanUp()
  {
    GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
    this.GetConduitFlow()?.RemoveConduitUpdater(new Action<float>(this.UpdatePipeState));
    base.OnCleanUp();
  }

  private void UpdateConnectionState(bool force_update = false)
  {
    this.connected = this.IsConnected(this.utilityCell);
    if (!(this.connected != this.previouslyConnected | force_update))
      return;
    this.operational.SetFlag(RequireOutputs.outputConnectedFlag, this.connected);
    this.previouslyConnected = this.connected;
    StatusItem status_item = (StatusItem) null;
    switch (this.conduitType)
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
    }
    this.hasPipeGuid = this.selectable.ToggleStatusItem(status_item, this.hasPipeGuid, !this.connected, (object) this);
  }

  private bool OutputPipeIsEmpty()
  {
    if (this.ignoreFullPipe)
      return true;
    bool flag = true;
    if (this.connected)
      flag = this.GetConduitFlow().IsConduitEmpty(this.utilityCell);
    return flag;
  }

  private void UpdatePipeState(float dt) => this.UpdatePipeRoomState();

  private void UpdatePipeRoomState(bool force_update = false)
  {
    bool flag = this.OutputPipeIsEmpty();
    if (!(flag != this.previouslyHadRoom | force_update))
      return;
    this.operational.SetFlag(RequireOutputs.pipesHaveRoomFlag, flag);
    this.previouslyHadRoom = flag;
    StatusItem blockedMultiples = Db.Get().BuildingStatusItems.ConduitBlockedMultiples;
    if (this.conduitType == ConduitType.Solid)
      blockedMultiples = Db.Get().BuildingStatusItems.SolidConduitBlockedMultiples;
    this.pipeBlockedGuid = this.selectable.ToggleStatusItem(blockedMultiples, this.pipeBlockedGuid, !flag);
  }

  private IConduitFlow GetConduitFlow()
  {
    switch (this.conduitType)
    {
      case ConduitType.Gas:
        return (IConduitFlow) Game.Instance.gasConduitFlow;
      case ConduitType.Liquid:
        return (IConduitFlow) Game.Instance.liquidConduitFlow;
      case ConduitType.Solid:
        return (IConduitFlow) Game.Instance.solidConduitFlow;
      default:
        Debug.LogWarning((object) ("GetConduitFlow() called with unexpected conduitType: " + this.conduitType.ToString()));
        return (IConduitFlow) null;
    }
  }

  private bool IsConnected(int cell) => RequireOutputs.IsConnected(cell, this.conduitType);

  public static bool IsConnected(int cell, ConduitType conduitType)
  {
    ObjectLayer layer = ObjectLayer.NumLayers;
    switch (conduitType)
    {
      case ConduitType.Gas:
        layer = ObjectLayer.GasConduit;
        break;
      case ConduitType.Liquid:
        layer = ObjectLayer.LiquidConduit;
        break;
      case ConduitType.Solid:
        layer = ObjectLayer.SolidConduit;
        break;
    }
    GameObject gameObject = Grid.Objects[cell, (int) layer];
    return Object.op_Inequality((Object) gameObject, (Object) null) && Object.op_Inequality((Object) gameObject.GetComponent<BuildingComplete>(), (Object) null);
  }
}
