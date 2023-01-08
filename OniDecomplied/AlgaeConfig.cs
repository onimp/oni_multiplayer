// Decompiled with JetBrains decompiler
// Type: AlgaeConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class AlgaeConfig : IOreConfig
{
  public SimHashes ElementID => SimHashes.Algae;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    int elementId = (int) this.ElementID;
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.Life);
    return EntityTemplates.CreateSolidOreEntity((SimHashes) elementId, additionalTags);
  }
}
