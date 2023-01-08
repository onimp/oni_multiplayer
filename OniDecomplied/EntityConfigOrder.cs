// Decompiled with JetBrains decompiler
// Type: EntityConfigOrder
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

internal class EntityConfigOrder : Attribute
{
  public int sortOrder;

  public EntityConfigOrder(int sort_order) => this.sortOrder = sort_order;
}
