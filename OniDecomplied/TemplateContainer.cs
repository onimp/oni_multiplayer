// Decompiled with JetBrains decompiler
// Type: TemplateContainer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using System;
using System.Collections.Generic;
using System.IO;
using TemplateClasses;
using UnityEngine;

[Serializable]
public class TemplateContainer
{
  public string name { get; set; }

  public int priority { get; set; }

  public TemplateContainer.Info info { get; set; }

  public List<TemplateClasses.Cell> cells { get; set; }

  public List<Prefab> buildings { get; set; }

  public List<Prefab> pickupables { get; set; }

  public List<Prefab> elementalOres { get; set; }

  public List<Prefab> otherEntities { get; set; }

  public void Init(
    List<TemplateClasses.Cell> _cells,
    List<Prefab> _buildings,
    List<Prefab> _pickupables,
    List<Prefab> _elementalOres,
    List<Prefab> _otherEntities)
  {
    if (_cells != null && _cells.Count > 0)
      this.cells = _cells;
    if (_buildings != null && _buildings.Count > 0)
      this.buildings = _buildings;
    if (_pickupables != null && _pickupables.Count > 0)
      this.pickupables = _pickupables;
    if (_elementalOres != null && _elementalOres.Count > 0)
      this.elementalOres = _elementalOres;
    if (_otherEntities != null && _otherEntities.Count > 0)
      this.otherEntities = _otherEntities;
    this.info = new TemplateContainer.Info();
    this.RefreshInfo();
  }

  public RectInt GetTemplateBounds(int padding = 0) => this.GetTemplateBounds(Vector2I.zero, padding);

  public RectInt GetTemplateBounds(Vector2 position, int padding = 0) => this.GetTemplateBounds(new Vector2I((int) position.x, (int) position.y), padding);

  public RectInt GetTemplateBounds(Vector2I position, int padding = 0)
  {
    Vector2f vector2f = Vector2f.op_Subtraction(this.info.min, new Vector2f(0, 0));
    if ((double) ((Vector2f) ref vector2f).sqrMagnitude <= 9.9999999747524271E-07)
      this.RefreshInfo();
    return this.info.GetBounds(position, padding);
  }

  public void RefreshInfo()
  {
    if (this.cells == null)
      return;
    int num1 = 1;
    int num2 = -1;
    int num3 = 1;
    int num4 = -1;
    foreach (TemplateClasses.Cell cell in this.cells)
    {
      if (cell.location_x < num1)
        num1 = cell.location_x;
      if (cell.location_x > num2)
        num2 = cell.location_x;
      if (cell.location_y < num3)
        num3 = cell.location_y;
      if (cell.location_y > num4)
        num4 = cell.location_y;
    }
    this.info.size = Vector2f.op_Implicit(new Vector2((float) (1 + (num2 - num1)), (float) (1 + (num4 - num3))));
    this.info.min = Vector2f.op_Implicit(new Vector2((float) num1, (float) num3));
    this.info.area = this.cells.Count;
  }

  public void SaveToYaml(string save_name)
  {
    string path = TemplateCache.RewriteTemplatePath(save_name);
    if (!Directory.Exists(System.IO.Path.GetDirectoryName(path)))
      Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
    YamlIO.Save<TemplateContainer>(this, path + ".yaml", (List<Tuple<string, System.Type>>) null);
  }

  [Serializable]
  public class Info
  {
    public Vector2f size { get; set; }

    public Vector2f min { get; set; }

    public int area { get; set; }

    public Tag[] tags { get; set; }

    public RectInt GetBounds(Vector2I position, int padding) => new RectInt(position.x + (int) this.min.x - padding, position.y + (int) this.min.y - padding, (int) this.size.x + padding * 2, (int) this.size.y + padding * 2);
  }
}
