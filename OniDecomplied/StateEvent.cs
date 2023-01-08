// Decompiled with JetBrains decompiler
// Type: StateEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public abstract class StateEvent
{
  protected string name;
  private string debugName;

  public StateEvent(string name)
  {
    this.name = name;
    this.debugName = "(Event)" + name;
  }

  public virtual StateEvent.Context Subscribe(StateMachine.Instance smi) => new StateEvent.Context(this);

  public virtual void Unsubscribe(StateMachine.Instance smi, StateEvent.Context context)
  {
  }

  public string GetName() => this.name;

  public string GetDebugName() => this.debugName;

  public struct Context
  {
    public StateEvent stateEvent;
    public int data;

    public Context(StateEvent state_event)
    {
      this.stateEvent = state_event;
      this.data = 0;
    }
  }
}
