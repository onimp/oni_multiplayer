// Decompiled with JetBrains decompiler
// Type: DivisibleTask`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

internal abstract class DivisibleTask<SharedData> : IWorkItem<SharedData>
{
  public string name;
  public int start;
  public int end;

  public void Run(SharedData sharedData) => this.RunDivision(sharedData);

  protected DivisibleTask(string name) => this.name = name;

  protected abstract void RunDivision(SharedData sharedData);
}
