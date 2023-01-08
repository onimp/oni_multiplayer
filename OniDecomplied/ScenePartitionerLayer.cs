// Decompiled with JetBrains decompiler
// Type: ScenePartitionerLayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

public class ScenePartitionerLayer
{
  public HashedString name;
  public int layer;
  public Action<int, object> OnEvent;

  public ScenePartitionerLayer(HashedString name, int layer)
  {
    this.name = name;
    this.layer = layer;
  }
}
