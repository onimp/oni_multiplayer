// Decompiled with JetBrains decompiler
// Type: BrainScheduler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/BrainScheduler")]
public class BrainScheduler : KMonoBehaviour, IRenderEveryTick, ICPULoad
{
  public const float millisecondsPerFrame = 33.33333f;
  public const float secondsPerFrame = 0.0333333276f;
  public const float framesPerSecond = 30.0000057f;
  private List<BrainScheduler.BrainGroup> brainGroups = new List<BrainScheduler.BrainGroup>();

  private bool isAsyncPathProbeEnabled => !TuningData<BrainScheduler.Tuning>.Get().disableAsyncPathProbes;

  protected virtual void OnPrefabInit()
  {
    this.brainGroups.Add((BrainScheduler.BrainGroup) new BrainScheduler.DupeBrainGroup());
    this.brainGroups.Add((BrainScheduler.BrainGroup) new BrainScheduler.CreatureBrainGroup());
    Components.Brains.Register(new Action<Brain>(this.OnAddBrain), new Action<Brain>(this.OnRemoveBrain));
    CPUBudget.AddRoot((ICPULoad) this);
    foreach (BrainScheduler.BrainGroup brainGroup in this.brainGroups)
      CPUBudget.AddChild((ICPULoad) this, (ICPULoad) brainGroup, brainGroup.LoadBalanceThreshold());
    CPUBudget.FinalizeChildren((ICPULoad) this);
  }

  private void OnAddBrain(Brain brain)
  {
    bool flag = false;
    foreach (BrainScheduler.BrainGroup brainGroup in this.brainGroups)
    {
      if (((Component) brain).HasTag(brainGroup.tag))
      {
        brainGroup.AddBrain(brain);
        flag = true;
      }
      Navigator component = ((Component) brain).GetComponent<Navigator>();
      if (Object.op_Inequality((Object) component, (Object) null))
        component.executePathProbeTaskAsync = this.isAsyncPathProbeEnabled;
    }
    DebugUtil.Assert(flag);
  }

  private void OnRemoveBrain(Brain brain)
  {
    bool flag = false;
    foreach (BrainScheduler.BrainGroup brainGroup in this.brainGroups)
    {
      if (((Component) brain).HasTag(brainGroup.tag))
      {
        flag = true;
        brainGroup.RemoveBrain(brain);
      }
      Navigator component = ((Component) brain).GetComponent<Navigator>();
      if (Object.op_Inequality((Object) component, (Object) null))
        component.executePathProbeTaskAsync = false;
    }
    DebugUtil.Assert(flag);
  }

  public float GetEstimatedFrameTime() => TuningData<BrainScheduler.Tuning>.Get().frameTime;

  public bool AdjustLoad(float currentFrameTime, float frameTimeDelta) => false;

  public void RenderEveryTick(float dt)
  {
    if (Game.IsQuitting() || KMonoBehaviour.isLoadingScene)
      return;
    foreach (BrainScheduler.BrainGroup brainGroup in this.brainGroups)
      brainGroup.RenderEveryTick(dt, this.isAsyncPathProbeEnabled);
  }

  protected virtual void OnForcedCleanUp()
  {
    CPUBudget.Remove((ICPULoad) this);
    base.OnForcedCleanUp();
  }

  private class Tuning : TuningData<BrainScheduler.Tuning>
  {
    public bool disableAsyncPathProbes;
    public float frameTime = 5f;
  }

  private abstract class BrainGroup : ICPULoad
  {
    private List<Brain> brains = new List<Brain>();
    private string increaseLoadLabel;
    private string decreaseLoadLabel;
    private WorkItemCollection<Navigator.PathProbeTask, object> pathProbeJob = new WorkItemCollection<Navigator.PathProbeTask, object>();
    private int nextUpdateBrain;
    private int nextPathProbeBrain;

    public Tag tag { get; private set; }

    protected BrainGroup(Tag tag)
    {
      this.tag = tag;
      this.probeSize = this.InitialProbeSize();
      this.probeCount = this.InitialProbeCount();
      string str = tag.ToString();
      this.increaseLoadLabel = "IncLoad" + str;
      this.decreaseLoadLabel = "DecLoad" + str;
    }

    public void AddBrain(Brain brain) => this.brains.Add(brain);

    public void RemoveBrain(Brain brain)
    {
      int num = this.brains.IndexOf(brain);
      if (num == -1)
        return;
      this.brains.RemoveAt(num);
      this.OnRemoveBrain(num, ref this.nextUpdateBrain);
      this.OnRemoveBrain(num, ref this.nextPathProbeBrain);
    }

    public int probeSize { get; private set; }

    public int probeCount { get; private set; }

    public bool AdjustLoad(float currentFrameTime, float frameTimeDelta)
    {
      bool flag = (double) frameTimeDelta > 0.0;
      int num1 = 0;
      int num2 = Math.Max(this.probeCount, Math.Min(this.brains.Count, CPUBudget.coreCount));
      int num3 = num1 + (num2 - this.probeCount);
      this.probeCount = num2;
      float num4 = Math.Min(1f, (float) this.probeCount / (float) CPUBudget.coreCount);
      float num5 = num4 * (float) this.probeSize;
      float num6 = num4 * (float) this.probeSize;
      float num7 = currentFrameTime / num6;
      float num8 = frameTimeDelta / num7;
      if (num3 == 0)
      {
        float num9 = num5 + num8 / (float) CPUBudget.coreCount;
        int num10 = MathUtil.Clamp(this.MinProbeSize(), this.IdealProbeSize(), (int) ((double) num9 / (double) num4));
        num3 += num10 - this.probeSize;
        this.probeSize = num10;
      }
      if (num3 == 0)
      {
        int num11 = Math.Max(1, (int) num4 + (flag ? 1 : -1));
        int num12 = MathUtil.Clamp(this.MinProbeSize(), this.IdealProbeSize(), (int) (((double) num6 + (double) num8) / (double) num11));
        int num13 = Math.Min(this.brains.Count, num11 * CPUBudget.coreCount);
        num3 += num13 - this.probeCount;
        this.probeCount = num13;
        this.probeSize = num12;
      }
      if (num3 == 0 & flag)
      {
        int num14 = this.probeSize + this.ProbeSizeStep();
        num3 += num14 - this.probeSize;
        this.probeSize = num14;
      }
      if (num3 >= 0 && num3 <= 0)
        Debug.LogWarning((object) "AdjustLoad() failed");
      return num3 != 0;
    }

    private void IncrementBrainIndex(ref int brainIndex)
    {
      ++brainIndex;
      if (brainIndex != this.brains.Count)
        return;
      brainIndex = 0;
    }

    private void ClampBrainIndex(ref int brainIndex) => brainIndex = MathUtil.Clamp(0, this.brains.Count - 1, brainIndex);

    private void OnRemoveBrain(int removedIndex, ref int brainIndex)
    {
      if (removedIndex < brainIndex)
      {
        --brainIndex;
      }
      else
      {
        if (brainIndex != this.brains.Count)
          return;
        brainIndex = 0;
      }
    }

    private void AsyncPathProbe()
    {
      int probeSize = this.probeSize;
      this.pathProbeJob.Reset((object) null);
      for (int index = 0; index != this.brains.Count; ++index)
      {
        this.ClampBrainIndex(ref this.nextPathProbeBrain);
        Brain brain = this.brains[this.nextPathProbeBrain];
        if (brain.IsRunning())
        {
          Navigator component = ((Component) brain).GetComponent<Navigator>();
          if (Object.op_Inequality((Object) component, (Object) null))
          {
            component.executePathProbeTaskAsync = true;
            component.PathProber.potentialCellsPerUpdate = this.probeSize;
            component.pathProbeTask.Update();
            this.pathProbeJob.Add(component.pathProbeTask);
            if (this.pathProbeJob.Count == this.probeCount)
              break;
          }
        }
        this.IncrementBrainIndex(ref this.nextPathProbeBrain);
      }
      CPUBudget.Start((ICPULoad) this);
      GlobalJobManager.Run((IWorkItemCollection) this.pathProbeJob);
      CPUBudget.End((ICPULoad) this);
    }

    public void RenderEveryTick(float dt, bool isAsyncPathProbeEnabled)
    {
      if (isAsyncPathProbeEnabled)
        this.AsyncPathProbe();
      int num = this.InitialProbeCount();
      for (int index = 0; index != this.brains.Count && num != 0; ++index)
      {
        this.ClampBrainIndex(ref this.nextUpdateBrain);
        Brain brain = this.brains[this.nextUpdateBrain];
        if (brain.IsRunning())
        {
          brain.UpdateBrain();
          --num;
        }
        this.IncrementBrainIndex(ref this.nextUpdateBrain);
      }
    }

    public void AccumulatePathProbeIterations(Dictionary<string, int> pathProbeIterations)
    {
      foreach (Brain brain in this.brains)
      {
        Navigator component = ((Component) brain).GetComponent<Navigator>();
        if (!Object.op_Equality((Object) component, (Object) null) && !pathProbeIterations.ContainsKey(((Object) brain).name))
          pathProbeIterations.Add(((Object) brain).name, component.PathProber.updateCount);
      }
    }

    protected abstract int InitialProbeCount();

    protected abstract int InitialProbeSize();

    protected abstract int MinProbeSize();

    protected abstract int IdealProbeSize();

    protected abstract int ProbeSizeStep();

    public abstract float GetEstimatedFrameTime();

    public abstract float LoadBalanceThreshold();
  }

  private class DupeBrainGroup : BrainScheduler.BrainGroup
  {
    public DupeBrainGroup()
      : base(GameTags.DupeBrain)
    {
    }

    protected override int InitialProbeCount() => TuningData<BrainScheduler.DupeBrainGroup.Tuning>.Get().initialProbeCount;

    protected override int InitialProbeSize() => TuningData<BrainScheduler.DupeBrainGroup.Tuning>.Get().initialProbeSize;

    protected override int MinProbeSize() => TuningData<BrainScheduler.DupeBrainGroup.Tuning>.Get().minProbeSize;

    protected override int IdealProbeSize() => TuningData<BrainScheduler.DupeBrainGroup.Tuning>.Get().idealProbeSize;

    protected override int ProbeSizeStep() => TuningData<BrainScheduler.DupeBrainGroup.Tuning>.Get().probeSizeStep;

    public override float GetEstimatedFrameTime() => TuningData<BrainScheduler.DupeBrainGroup.Tuning>.Get().estimatedFrameTime;

    public override float LoadBalanceThreshold() => TuningData<BrainScheduler.DupeBrainGroup.Tuning>.Get().loadBalanceThreshold;

    public class Tuning : TuningData<BrainScheduler.DupeBrainGroup.Tuning>
    {
      public int initialProbeCount = 1;
      public int initialProbeSize = 1000;
      public int minProbeSize = 100;
      public int idealProbeSize = 1000;
      public int probeSizeStep = 100;
      public float estimatedFrameTime = 2f;
      public float loadBalanceThreshold = 0.1f;
    }
  }

  private class CreatureBrainGroup : BrainScheduler.BrainGroup
  {
    public CreatureBrainGroup()
      : base(GameTags.CreatureBrain)
    {
    }

    protected override int InitialProbeCount() => TuningData<BrainScheduler.CreatureBrainGroup.Tuning>.Get().initialProbeCount;

    protected override int InitialProbeSize() => TuningData<BrainScheduler.CreatureBrainGroup.Tuning>.Get().initialProbeSize;

    protected override int MinProbeSize() => TuningData<BrainScheduler.CreatureBrainGroup.Tuning>.Get().minProbeSize;

    protected override int IdealProbeSize() => TuningData<BrainScheduler.CreatureBrainGroup.Tuning>.Get().idealProbeSize;

    protected override int ProbeSizeStep() => TuningData<BrainScheduler.CreatureBrainGroup.Tuning>.Get().probeSizeStep;

    public override float GetEstimatedFrameTime() => TuningData<BrainScheduler.CreatureBrainGroup.Tuning>.Get().estimatedFrameTime;

    public override float LoadBalanceThreshold() => TuningData<BrainScheduler.CreatureBrainGroup.Tuning>.Get().loadBalanceThreshold;

    public class Tuning : TuningData<BrainScheduler.CreatureBrainGroup.Tuning>
    {
      public int initialProbeCount = 1;
      public int initialProbeSize = 1000;
      public int minProbeSize = 100;
      public int idealProbeSize = 300;
      public int probeSizeStep = 100;
      public float estimatedFrameTime = 1f;
      public float loadBalanceThreshold = 0.1f;
    }
  }
}
