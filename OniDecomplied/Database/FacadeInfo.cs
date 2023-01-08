// Decompiled with JetBrains decompiler
// Type: Database.FacadeInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

namespace Database
{
  public class FacadeInfo
  {
    public string prefabID { get; set; }

    public string id { get; set; }

    public string name { get; set; }

    public string description { get; set; }

    public string animFile { get; set; }

    public List<FacadeInfo.workable> workables { get; set; }

    public class workable
    {
      public string workableName { get; set; }

      public string workableAnim { get; set; }
    }
  }
}
