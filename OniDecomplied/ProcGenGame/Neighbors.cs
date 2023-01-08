// Decompiled with JetBrains decompiler
// Type: ProcGenGame.Neighbors
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;

namespace ProcGenGame
{
  [SerializationConfig]
  public struct Neighbors
  {
    public TerrainCell n0;
    public TerrainCell n1;

    public Neighbors(TerrainCell a, TerrainCell b)
    {
      Debug.Assert(a != null && b != null, (object) "NULL Neighbor");
      this.n0 = a;
      this.n1 = b;
    }
  }
}
