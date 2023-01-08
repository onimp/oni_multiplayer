// Decompiled with JetBrains decompiler
// Type: KleiPermitDioramaVis
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class KleiPermitDioramaVis : KMonoBehaviour
{
  [SerializeField]
  private KleiPermitDioramaVis_Fallback fallbackVis;
  [SerializeField]
  private KleiPermitDioramaVis_DupeEquipment equipmentVis;
  [SerializeField]
  private KleiPermitDioramaVis_BuildingOnFloor buildingOnFloorVis;
  [SerializeField]
  private KleiPermitDioramaVis_PedestalAndItem pedestalAndItemVis;
  [SerializeField]
  private KleiPermitDioramaVis_HangingPlanter hangingPlanterVis;
  [SerializeField]
  private KleiPermitDioramaVis_ArtablePainting artablePaintingVis;
  [SerializeField]
  private KleiPermitDioramaVis_ArtableSculpture artableSculptureVis;
  private IReadOnlyList<IKleiPermitDioramaVisTarget> allVisList;
  public static PermitResource lastRenderedPermit;

  protected virtual void OnPrefabInit()
  {
    this.allVisList = (IReadOnlyList<IKleiPermitDioramaVisTarget>) ReflectionUtil.For<KleiPermitDioramaVis>(this).CollectValuesForFieldsThatInheritOrImplement<IKleiPermitDioramaVisTarget>(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
    foreach (IKleiPermitDioramaVisTarget allVis in (IEnumerable<IKleiPermitDioramaVisTarget>) this.allVisList)
      allVis.ConfigureSetup();
  }

  public void ConfigureWith(PermitResource permit, PermitPresentationInfo permitPresInfo)
  {
    foreach (IKleiPermitDioramaVisTarget allVis in (IEnumerable<IKleiPermitDioramaVisTarget>) this.allVisList)
      allVis.GetGameObject().SetActive(false);
    IKleiPermitDioramaVisTarget permitVisTarget = this.GetPermitVisTarget(permit, permitPresInfo);
    permitVisTarget.GetGameObject().SetActive(true);
    permitVisTarget.ConfigureWith(permit, permitPresInfo);
  }

  public IKleiPermitDioramaVisTarget GetPermitVisTarget(
    PermitResource permit,
    PermitPresentationInfo permitPresInfo)
  {
    KleiPermitDioramaVis.lastRenderedPermit = permit;
    if (permit == null)
      return (IKleiPermitDioramaVisTarget) this.fallbackVis.WithError(string.Format("Given invalid permit: {0}", (object) permit));
    if (permit.PermitCategory == PermitCategory.Equipment || permit.PermitCategory == PermitCategory.DupeTops || permit.PermitCategory == PermitCategory.DupeBottoms || permit.PermitCategory == PermitCategory.DupeGloves || permit.PermitCategory == PermitCategory.DupeShoes || permit.PermitCategory == PermitCategory.DupeHats || permit.PermitCategory == PermitCategory.DupeAccessories)
      return (IKleiPermitDioramaVisTarget) this.equipmentVis;
    if (permit.PermitCategory == PermitCategory.Building)
    {
      (bool hasValue, BuildLocationRule buildLocationRule1) = KleiPermitVisUtil.GetBuildLocationRule(permit);
      int num = hasValue ? 1 : 0;
      BuildLocationRule buildLocationRule2 = buildLocationRule1;
      if (num == 0)
        return (IKleiPermitDioramaVisTarget) this.fallbackVis.WithError("Couldn't get BuildLocationRule on permit with id \"" + permit.Id + "\"");
      switch (buildLocationRule2)
      {
        case BuildLocationRule.OnFloor:
          return (IKleiPermitDioramaVisTarget) this.buildingOnFloorVis;
        case BuildLocationRule.OnCeiling:
          string prefabId = KleiPermitVisUtil.GetBuildingDef(permit).Value.PrefabID;
          return prefabId == "FlowerVaseHanging" || prefabId == "FlowerVaseHangingFancy" ? (IKleiPermitDioramaVisTarget) this.hangingPlanterVis : (IKleiPermitDioramaVisTarget) this.pedestalAndItemVis;
        case BuildLocationRule.OnWall:
        case BuildLocationRule.InCorner:
        case BuildLocationRule.NotInTiles:
          return (IKleiPermitDioramaVisTarget) this.pedestalAndItemVis;
        default:
          return (IKleiPermitDioramaVisTarget) this.fallbackVis.WithError(string.Format("No visualization available for building with BuildLocationRule of {0}", (object) buildLocationRule2));
      }
    }
    else
    {
      if (permit.PermitCategory != PermitCategory.Artwork)
        return (IKleiPermitDioramaVisTarget) this.fallbackVis.WithError("No visualization has been defined for permit with id \"" + permit.Id + "\"");
      (bool hasValue, BuildingDef buildingDef1) = KleiPermitVisUtil.GetBuildingDef(permit);
      int num = hasValue ? 1 : 0;
      BuildingDef buildingDef2 = buildingDef1;
      if (num == 0)
        return (IKleiPermitDioramaVisTarget) this.fallbackVis.WithError("Couldn't find building def for Artable " + permit.Id);
      if (Has<Sculpture>(buildingDef2))
        return (IKleiPermitDioramaVisTarget) this.artableSculptureVis;
      return Has<Painting>(buildingDef2) ? (IKleiPermitDioramaVisTarget) this.artablePaintingVis : (IKleiPermitDioramaVisTarget) this.fallbackVis.WithError("No visualization available for Artable " + permit.Id);
    }

    static bool Has<T>(BuildingDef buildingDef) where T : Component => !Util.IsNullOrDestroyed((object) buildingDef.BuildingComplete.GetComponent<T>());
  }
}
