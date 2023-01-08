// Decompiled with JetBrains decompiler
// Type: Brain
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Brain")]
public class Brain : KMonoBehaviour
{
  private bool running;
  private bool suspend;
  protected KPrefabID prefabId;
  protected ChoreConsumer choreConsumer;

  protected virtual void OnPrefabInit() => base.OnPrefabInit();

  protected virtual void OnSpawn()
  {
    this.prefabId = ((Component) this).GetComponent<KPrefabID>();
    this.choreConsumer = ((Component) this).GetComponent<ChoreConsumer>();
    this.running = true;
    Components.Brains.Add(this);
  }

  public event System.Action onPreUpdate;

  public virtual void UpdateBrain()
  {
    if (this.onPreUpdate != null)
      this.onPreUpdate();
    if (!this.IsRunning())
      return;
    this.UpdateChores();
  }

  private bool FindBetterChore(ref Chore.Precondition.Context context) => this.choreConsumer.FindNextChore(ref context);

  private void UpdateChores()
  {
    if (this.prefabId.HasTag(GameTags.PreventChoreInterruption))
      return;
    Chore.Precondition.Context context = new Chore.Precondition.Context();
    if (!this.FindBetterChore(ref context))
      return;
    if (this.prefabId.HasTag(GameTags.PerformingWorkRequest))
      this.Trigger(1485595942, (object) null);
    else
      this.choreConsumer.choreDriver.SetChore(context);
  }

  public bool IsRunning() => this.running && !this.suspend;

  public void Reset(string reason)
  {
    this.Stop(nameof (Reset));
    this.running = true;
  }

  public void Stop(string reason)
  {
    ((Component) this).GetComponent<ChoreDriver>().StopChore();
    this.running = false;
  }

  public void Resume(string caller) => this.suspend = false;

  public void Suspend(string caller) => this.suspend = true;

  protected virtual void OnCmpDisable()
  {
    base.OnCmpDisable();
    this.Stop(nameof (OnCmpDisable));
  }

  protected virtual void OnCleanUp()
  {
    this.Stop(nameof (OnCleanUp));
    Components.Brains.Remove(this);
  }
}
