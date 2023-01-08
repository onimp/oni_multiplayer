// Decompiled with JetBrains decompiler
// Type: BuildingConduitEndpoints
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/BuildingConduitEndpoints")]
public class BuildingConduitEndpoints : KMonoBehaviour
{
  private FlowUtilityNetwork.NetworkItem itemInput;
  private FlowUtilityNetwork.NetworkItem itemOutput;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.AddEndpoint();
  }

  protected virtual void OnCleanUp()
  {
    this.RemoveEndPoint();
    base.OnCleanUp();
  }

  public void RemoveEndPoint()
  {
    if (this.itemInput != null)
    {
      if (this.itemInput.ConduitType == ConduitType.Solid)
        Game.Instance.solidConduitSystem.RemoveFromNetworks(this.itemInput.Cell, (object) this.itemInput, true);
      else
        Conduit.GetNetworkManager(this.itemInput.ConduitType).RemoveFromNetworks(this.itemInput.Cell, (object) this.itemInput, true);
      this.itemInput = (FlowUtilityNetwork.NetworkItem) null;
    }
    if (this.itemOutput == null)
      return;
    if (this.itemOutput.ConduitType == ConduitType.Solid)
      Game.Instance.solidConduitSystem.RemoveFromNetworks(this.itemOutput.Cell, (object) this.itemOutput, true);
    else
      Conduit.GetNetworkManager(this.itemOutput.ConduitType).RemoveFromNetworks(this.itemOutput.Cell, (object) this.itemOutput, true);
    this.itemOutput = (FlowUtilityNetwork.NetworkItem) null;
  }

  public void AddEndpoint()
  {
    Building component = ((Component) this).GetComponent<Building>();
    BuildingDef def = component.Def;
    this.RemoveEndPoint();
    if (def.InputConduitType != ConduitType.None)
    {
      int utilityInputCell = component.GetUtilityInputCell();
      this.itemInput = new FlowUtilityNetwork.NetworkItem(def.InputConduitType, Endpoint.Sink, utilityInputCell, ((Component) this).gameObject);
      if (def.InputConduitType == ConduitType.Solid)
        Game.Instance.solidConduitSystem.AddToNetworks(utilityInputCell, (object) this.itemInput, true);
      else
        Conduit.GetNetworkManager(def.InputConduitType).AddToNetworks(utilityInputCell, (object) this.itemInput, true);
    }
    if (def.OutputConduitType == ConduitType.None)
      return;
    int utilityOutputCell = component.GetUtilityOutputCell();
    this.itemOutput = new FlowUtilityNetwork.NetworkItem(def.OutputConduitType, Endpoint.Source, utilityOutputCell, ((Component) this).gameObject);
    if (def.OutputConduitType == ConduitType.Solid)
      Game.Instance.solidConduitSystem.AddToNetworks(utilityOutputCell, (object) this.itemOutput, true);
    else
      Conduit.GetNetworkManager(def.OutputConduitType).AddToNetworks(utilityOutputCell, (object) this.itemOutput, true);
  }
}
