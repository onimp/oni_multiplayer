// Decompiled with JetBrains decompiler
// Type: DiseaseVisualization
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class DiseaseVisualization : ScriptableObject
{
  public Sprite overlaySprite;
  public List<DiseaseVisualization.Info> info = new List<DiseaseVisualization.Info>();

  public DiseaseVisualization.Info GetInfo(HashedString id)
  {
    foreach (DiseaseVisualization.Info info in this.info)
    {
      if (HashedString.op_Equality(id, HashedString.op_Implicit(info.name)))
        return info;
    }
    return new DiseaseVisualization.Info();
  }

  [Serializable]
  public struct Info
  {
    public string name;
    public string overlayColourName;

    public Info(string name)
    {
      this.name = name;
      this.overlayColourName = "germFoodPoisoning";
    }
  }
}
