// Decompiled with JetBrains decompiler
// Type: TemplateClasses.Cell
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

namespace TemplateClasses
{
  [Serializable]
  public class Cell
  {
    public Cell()
    {
    }

    public Cell(
      int loc_x,
      int loc_y,
      SimHashes _element,
      float _temperature,
      float _mass,
      string _diseaseName,
      int _diseaseCount,
      bool _preventFoWReveal = false)
    {
      this.location_x = loc_x;
      this.location_y = loc_y;
      this.element = _element;
      this.temperature = _temperature;
      this.mass = _mass;
      this.diseaseName = _diseaseName;
      this.diseaseCount = _diseaseCount;
      this.preventFoWReveal = _preventFoWReveal;
    }

    public SimHashes element { get; set; }

    public float mass { get; set; }

    public float temperature { get; set; }

    public string diseaseName { get; set; }

    public int diseaseCount { get; set; }

    public int location_x { get; set; }

    public int location_y { get; set; }

    public bool preventFoWReveal { get; set; }
  }
}
