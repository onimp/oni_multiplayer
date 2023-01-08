// Decompiled with JetBrains decompiler
// Type: ProcGenGame.SymbolicMapElement
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace ProcGenGame
{
  public interface SymbolicMapElement
  {
    void ConvertToMap(
      Chunk world,
      TerrainCell.SetValuesFunction SetValues,
      float temperatureMin,
      float temperatureRange,
      SeededRandom rnd);
  }
}
