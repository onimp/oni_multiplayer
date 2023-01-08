// Decompiled with JetBrains decompiler
// Type: TechItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class TechItem : Resource
{
  public string description;
  public Func<string, bool, Sprite> getUISprite;
  public string parentTechId;
  public string[] dlcIds;

  public TechItem(
    string id,
    ResourceSet parent,
    string name,
    string description,
    Func<string, bool, Sprite> getUISprite,
    string parentTechId,
    string[] dlcIds)
    : base(id, parent, name)
  {
    this.description = description;
    this.getUISprite = getUISprite;
    this.parentTechId = parentTechId;
    this.dlcIds = dlcIds;
  }

  public Tech ParentTech => Db.Get().Techs.Get(this.parentTechId);

  public Sprite UISprite() => this.getUISprite("ui", false);

  public bool IsComplete() => this.ParentTech.IsComplete();
}
