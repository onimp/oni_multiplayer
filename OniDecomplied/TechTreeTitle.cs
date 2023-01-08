// Decompiled with JetBrains decompiler
// Type: TechTreeTitle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class TechTreeTitle : Resource
{
  public string desc;
  private ResourceTreeNode node;

  public Vector2 center => this.node.center;

  public float width => this.node.width;

  public float height => this.node.height;

  public TechTreeTitle(string id, ResourceSet parent, string name, ResourceTreeNode node)
    : base(id, parent, name)
  {
    this.node = node;
  }
}
