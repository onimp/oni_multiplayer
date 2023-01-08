// Decompiled with JetBrains decompiler
// Type: IKleiPermitDioramaVisTarget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using UnityEngine;

public interface IKleiPermitDioramaVisTarget
{
  GameObject GetGameObject();

  void ConfigureSetup();

  void ConfigureWith(PermitResource permit, PermitPresentationInfo permitPresInfo);
}
