// Decompiled with JetBrains decompiler
// Type: BuildingFacade
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class BuildingFacade : KMonoBehaviour
{
  [Serialize]
  private string currentFacade;
  public KAnimFile[] animFiles;
  public Dictionary<string, KAnimFile[]> interactAnims = new Dictionary<string, KAnimFile[]>();

  public string CurrentFacade => this.currentFacade;

  public bool IsOriginal => Util.IsNullOrWhiteSpace(this.currentFacade);

  protected virtual void OnPrefabInit()
  {
  }

  protected virtual void OnSpawn()
  {
    if (this.IsOriginal)
      return;
    this.ApplyBuildingFacade(Db.GetBuildingFacades().TryGet(this.currentFacade));
  }

  public void ApplyBuildingFacade(BuildingFacadeResource facade)
  {
    if (facade == null)
    {
      this.ClearFacade();
    }
    else
    {
      this.currentFacade = facade.Id;
      this.ChangeBuilding(new KAnimFile[1]
      {
        Assets.GetAnim(HashedString.op_Implicit(facade.AnimFile))
      }, facade.Name, facade.Description, facade.InteractFile);
    }
  }

  private void ClearFacade()
  {
    Building component = ((Component) this).GetComponent<Building>();
    this.ChangeBuilding(component.Def.AnimFiles, component.Def.Name, component.Def.Desc);
  }

  private void ChangeBuilding(
    KAnimFile[] animFiles,
    string displayName,
    string desc,
    Dictionary<string, string> interactAnimsNames = null)
  {
    this.interactAnims.Clear();
    if (interactAnimsNames != null && interactAnimsNames.Count > 0)
    {
      this.interactAnims = new Dictionary<string, KAnimFile[]>();
      foreach (KeyValuePair<string, string> interactAnimsName in interactAnimsNames)
        this.interactAnims.Add(interactAnimsName.Key, new KAnimFile[1]
        {
          Assets.GetAnim(HashedString.op_Implicit(interactAnimsName.Value))
        });
    }
    Building[] components = ((Component) this).GetComponents<Building>();
    foreach (Building building in components)
    {
      building.SetDescription(desc);
      ((Component) building).GetComponent<KBatchedAnimController>().SwapAnims(animFiles);
    }
    ((Component) this).GetComponent<KSelectable>().SetName(displayName);
    if (!Object.op_Inequality((Object) ((Component) this).GetComponent<AnimTileable>(), (Object) null) || components.Length == 0)
      return;
    GameScenePartitioner.Instance.TriggerEvent(components[0].GetExtents(), GameScenePartitioner.Instance.objectLayers[1], (object) null);
  }

  public string GetNextFacade()
  {
    BuildingDef def = ((Component) this).GetComponent<Building>().Def;
    int index = def.AvailableFacades.FindIndex((Predicate<string>) (s => s == this.currentFacade)) + 1;
    if (index >= def.AvailableFacades.Count)
      index = 0;
    return def.AvailableFacades[index];
  }
}
