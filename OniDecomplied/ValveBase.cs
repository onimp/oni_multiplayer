// Decompiled with JetBrains decompiler
// Type: ValveBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/ValveBase")]
public class ValveBase : KMonoBehaviour, ISaveLoadable
{
  [SerializeField]
  public ConduitType conduitType;
  [SerializeField]
  public float maxFlow = 0.5f;
  [Serialize]
  private float currentFlow;
  [MyCmpGet]
  protected KBatchedAnimController controller;
  protected HandleVector<int>.Handle flowAccumulator = HandleVector<int>.InvalidHandle;
  private int curFlowIdx = -1;
  private int inputCell;
  private int outputCell;
  [SerializeField]
  public ValveBase.AnimRangeInfo[] animFlowRanges;

  public float CurrentFlow
  {
    set => this.currentFlow = value;
    get => this.currentFlow;
  }

  public HandleVector<int>.Handle AccumulatorHandle => this.flowAccumulator;

  public float MaxFlow => this.maxFlow;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.flowAccumulator = Game.Instance.accumulators.Add("Flow", (KMonoBehaviour) this);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    Building component = ((Component) this).GetComponent<Building>();
    this.inputCell = component.GetUtilityInputCell();
    this.outputCell = component.GetUtilityOutputCell();
    Conduit.GetFlowManager(this.conduitType).AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.Default);
    this.UpdateAnim();
    this.OnCmpEnable();
  }

  protected virtual void OnCleanUp()
  {
    Game.Instance.accumulators.Remove(this.flowAccumulator);
    Conduit.GetFlowManager(this.conduitType).RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
    base.OnCleanUp();
  }

  private void ConduitUpdate(float dt)
  {
    ConduitFlow flowManager = Conduit.GetFlowManager(this.conduitType);
    ConduitFlow.Conduit conduit = flowManager.GetConduit(this.inputCell);
    if (!flowManager.HasConduit(this.inputCell) || !flowManager.HasConduit(this.outputCell))
    {
      this.OnMassTransfer(0.0f);
      this.UpdateAnim();
    }
    else
    {
      ConduitFlow.ConduitContents contents = conduit.GetContents(flowManager);
      float mass = Mathf.Min(contents.mass, this.currentFlow * dt);
      float num = 0.0f;
      if ((double) mass > 0.0)
      {
        int disease_count = (int) ((double) mass / (double) contents.mass * (double) contents.diseaseCount);
        num = flowManager.AddElement(this.outputCell, contents.element, mass, contents.temperature, contents.diseaseIdx, disease_count);
        Game.Instance.accumulators.Accumulate(this.flowAccumulator, num);
        if ((double) num > 0.0)
          flowManager.RemoveElement(this.inputCell, num);
      }
      this.OnMassTransfer(num);
      this.UpdateAnim();
    }
  }

  protected virtual void OnMassTransfer(float amount)
  {
  }

  public virtual void UpdateAnim()
  {
    float averageRate = Game.Instance.accumulators.GetAverageRate(this.flowAccumulator);
    if ((double) averageRate > 0.0)
    {
      for (int index = 0; index < this.animFlowRanges.Length; ++index)
      {
        if ((double) averageRate <= (double) this.animFlowRanges[index].minFlow)
        {
          if (this.curFlowIdx == index)
            break;
          this.curFlowIdx = index;
          this.controller.Play(HashedString.op_Implicit(this.animFlowRanges[index].animName), (double) averageRate <= 0.0 ? (KAnim.PlayMode) 1 : (KAnim.PlayMode) 0);
          break;
        }
      }
    }
    else
      this.controller.Play(HashedString.op_Implicit("off"));
  }

  [Serializable]
  public struct AnimRangeInfo
  {
    public float minFlow;
    public string animName;

    public AnimRangeInfo(float min_flow, string anim_name)
    {
      this.minFlow = min_flow;
      this.animName = anim_name;
    }
  }
}
