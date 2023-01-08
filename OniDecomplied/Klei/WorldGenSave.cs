// Decompiled with JetBrains decompiler
// Type: Klei.WorldGenSave
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

namespace Klei
{
  public class WorldGenSave
  {
    public Vector2I version;
    public Dictionary<string, object> stats;
    public Data data;
    public string worldID;
    public List<string> traitIDs;
    public List<string> storyTraitIDs;

    public WorldGenSave()
    {
      this.data = new Data();
      this.stats = new Dictionary<string, object>();
    }
  }
}
