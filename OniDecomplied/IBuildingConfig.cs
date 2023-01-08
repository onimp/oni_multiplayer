// Decompiled with JetBrains decompiler
// Type: IBuildingConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public abstract class IBuildingConfig
{
  public abstract BuildingDef CreateBuildingDef();

  public virtual void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
  }

  public abstract void DoPostConfigureComplete(GameObject go);

  public virtual void DoPostConfigurePreview(BuildingDef def, GameObject go)
  {
  }

  public virtual void DoPostConfigureUnderConstruction(GameObject go)
  {
  }

  public virtual void ConfigurePost(BuildingDef def)
  {
  }

  public virtual string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public virtual bool ForbidFromLoading() => false;
}
