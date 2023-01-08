// Decompiled with JetBrains decompiler
// Type: Klei.TerrainCellLogged
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ProcGen.Map;
using ProcGenGame;
using System.Collections.Generic;
using VoronoiTree;

namespace Klei
{
  public class TerrainCellLogged : TerrainCell
  {
    public TerrainCellLogged()
    {
    }

    public TerrainCellLogged(Cell node, Diagram.Site site, Dictionary<Tag, int> distancesToTags)
      : base(node, site, distancesToTags)
    {
    }

    public override void LogInfo(string evt, string param, float value)
    {
    }
  }
}
