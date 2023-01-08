// Decompiled with JetBrains decompiler
// Type: Klei.SaveFileRoot
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

namespace Klei
{
  internal class SaveFileRoot
  {
    public int WidthInCells;
    public int HeightInCells;
    public Dictionary<string, byte[]> streamed;
    public string clusterID;
    public List<ModInfo> requiredMods;
    public List<KMod.Label> active_mods;

    public SaveFileRoot() => this.streamed = new Dictionary<string, byte[]>();
  }
}
