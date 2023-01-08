// Decompiled with JetBrains decompiler
// Type: Database.StateMachineCategories
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace Database
{
  public class StateMachineCategories : ResourceSet<StateMachine.Category>
  {
    public StateMachine.Category Ai;
    public StateMachine.Category Monitor;
    public StateMachine.Category Chore;
    public StateMachine.Category Misc;

    public StateMachineCategories()
    {
      this.Ai = this.Add(new StateMachine.Category(nameof (Ai)));
      this.Monitor = this.Add(new StateMachine.Category(nameof (Monitor)));
      this.Chore = this.Add(new StateMachine.Category(nameof (Chore)));
      this.Misc = this.Add(new StateMachine.Category(nameof (Misc)));
    }
  }
}
