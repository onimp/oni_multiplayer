// Decompiled with JetBrains decompiler
// Type: BuildingGroupScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class BuildingGroupScreen : KScreen
{
  public static BuildingGroupScreen Instance;

  protected virtual void OnPrefabInit()
  {
    BuildingGroupScreen.Instance = this;
    base.OnPrefabInit();
    this.ConsumeMouseScroll = true;
  }

  protected virtual void OnActivate()
  {
    base.OnActivate();
    this.ConsumeMouseScroll = true;
  }
}
