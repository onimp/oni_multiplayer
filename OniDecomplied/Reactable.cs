// Decompiled with JetBrains decompiler
// Type: Reactable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Reactable
{
  private HandleVector<int>.Handle partitionerEntry;
  protected GameObject gameObject;
  public HashedString id;
  public bool preventChoreInterruption = true;
  public int sourceCell;
  private int rangeWidth;
  private int rangeHeight;
  private int transformId = -1;
  public float globalCooldown;
  public float localCooldown;
  public float lifeSpan = float.PositiveInfinity;
  private float lastTriggerTime = (float) int.MinValue;
  private float initialDelay;
  protected GameObject reactor;
  private ChoreType choreType;
  protected LoggerFSS log;
  private List<Reactable.ReactablePrecondition> additionalPreconditions;
  private ObjectLayer reactionLayer;

  public bool IsValid => this.partitionerEntry.IsValid();

  public float creationTime { get; private set; }

  public bool IsReacting => Object.op_Inequality((Object) this.reactor, (Object) null);

  public Reactable(
    GameObject gameObject,
    HashedString id,
    ChoreType chore_type,
    int range_width = 15,
    int range_height = 8,
    bool follow_transform = false,
    float globalCooldown = 0.0f,
    float localCooldown = 0.0f,
    float lifeSpan = float.PositiveInfinity,
    float max_initial_delay = 0.0f,
    ObjectLayer overrideLayer = ObjectLayer.NumLayers)
  {
    this.rangeHeight = range_height;
    this.rangeWidth = range_width;
    this.id = id;
    this.gameObject = gameObject;
    this.choreType = chore_type;
    this.globalCooldown = globalCooldown;
    this.localCooldown = localCooldown;
    this.lifeSpan = lifeSpan;
    this.initialDelay = (double) max_initial_delay > 0.0 ? Random.Range(0.0f, max_initial_delay) : 0.0f;
    this.creationTime = GameClock.Instance.GetTime();
    ObjectLayer objectLayer = overrideLayer == ObjectLayer.NumLayers ? this.reactionLayer : overrideLayer;
    ReactionMonitor.Def def = gameObject.GetDef<ReactionMonitor.Def>();
    if (overrideLayer != objectLayer && def != null)
      objectLayer = def.ReactionLayer;
    this.reactionLayer = objectLayer;
    this.Initialize(follow_transform);
  }

  public void Initialize(bool followTransform)
  {
    this.UpdateLocation();
    if (!followTransform)
      return;
    this.transformId = Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(this.gameObject.transform, new System.Action(this.UpdateLocation), "Reactable follow transform");
  }

  public void Begin(GameObject reactor)
  {
    this.reactor = reactor;
    this.lastTriggerTime = GameClock.Instance.GetTime();
    this.InternalBegin();
  }

  public void End()
  {
    this.InternalEnd();
    if (!Object.op_Inequality((Object) this.reactor, (Object) null))
      return;
    GameObject reactor = this.reactor;
    this.InternalEnd();
    this.reactor = (GameObject) null;
    if (!Object.op_Inequality((Object) reactor, (Object) null))
      return;
    reactor.GetSMI<ReactionMonitor.Instance>()?.StopReaction();
  }

  public bool CanBegin(GameObject reactor, Navigator.ActiveTransition transition)
  {
    double time = (double) GameClock.Instance.GetTime();
    float num1 = (float) time - this.creationTime;
    float num2 = (float) time - this.lastTriggerTime;
    if ((double) num1 < (double) this.initialDelay || (double) num2 < (double) this.globalCooldown)
      return false;
    ChoreConsumer component = reactor.GetComponent<ChoreConsumer>();
    Chore chore = Object.op_Inequality((Object) component, (Object) null) ? component.choreDriver.GetCurrentChore() : (Chore) null;
    if (chore == null || this.choreType.priority <= chore.choreType.priority)
      return false;
    for (int index = 0; this.additionalPreconditions != null && index < this.additionalPreconditions.Count; ++index)
    {
      if (!this.additionalPreconditions[index](reactor, transition))
        return false;
    }
    return this.InternalCanBegin(reactor, transition);
  }

  public bool IsExpired() => (double) GameClock.Instance.GetTime() - (double) this.creationTime > (double) this.lifeSpan;

  public abstract bool InternalCanBegin(GameObject reactor, Navigator.ActiveTransition transition);

  public abstract void Update(float dt);

  protected abstract void InternalBegin();

  protected abstract void InternalEnd();

  protected abstract void InternalCleanup();

  public void Cleanup()
  {
    this.End();
    this.InternalCleanup();
    if (this.transformId != -1)
    {
      Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(this.transformId, new System.Action(this.UpdateLocation));
      this.transformId = -1;
    }
    GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
  }

  private void UpdateLocation()
  {
    GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
    if (!Object.op_Inequality((Object) this.gameObject, (Object) null))
      return;
    this.sourceCell = Grid.PosToCell(this.gameObject);
    this.partitionerEntry = GameScenePartitioner.Instance.Add(nameof (Reactable), (object) this, new Extents(Grid.PosToXY(TransformExtensions.GetPosition(this.gameObject.transform)).x - this.rangeWidth / 2, Grid.PosToXY(TransformExtensions.GetPosition(this.gameObject.transform)).y - this.rangeHeight / 2, this.rangeWidth, this.rangeHeight), GameScenePartitioner.Instance.objectLayers[(int) this.reactionLayer], (Action<object>) null);
  }

  public Reactable AddPrecondition(Reactable.ReactablePrecondition precondition)
  {
    if (this.additionalPreconditions == null)
      this.additionalPreconditions = new List<Reactable.ReactablePrecondition>();
    this.additionalPreconditions.Add(precondition);
    return this;
  }

  public void InsertPrecondition(int index, Reactable.ReactablePrecondition precondition)
  {
    if (this.additionalPreconditions == null)
      this.additionalPreconditions = new List<Reactable.ReactablePrecondition>();
    index = Math.Min(index, this.additionalPreconditions.Count);
    this.additionalPreconditions.Insert(index, precondition);
  }

  public delegate bool ReactablePrecondition(GameObject go, Navigator.ActiveTransition transition);
}
