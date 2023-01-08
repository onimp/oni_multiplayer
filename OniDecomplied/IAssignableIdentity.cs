// Decompiled with JetBrains decompiler
// Type: IAssignableIdentity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

public interface IAssignableIdentity
{
  string GetProperName();

  List<Ownables> GetOwners();

  Ownables GetSoleOwner();

  bool IsNull();

  bool HasOwner(Assignables owner);

  int NumOwners();
}
