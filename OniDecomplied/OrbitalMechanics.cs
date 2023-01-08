// Decompiled with JetBrains decompiler
// Type: OrbitalMechanics
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/OrbitalMechanics")]
public class OrbitalMechanics : KMonoBehaviour
{
  [Serialize]
  private List<Ref<OrbitalObject>> orbitingObjects = new List<Ref<OrbitalObject>>();
  private EventSystem.IntraObjectHandler<OrbitalMechanics> OnClusterLocationChangedDelegate = new EventSystem.IntraObjectHandler<OrbitalMechanics>((Action<OrbitalMechanics, object>) ((cmp, data) => cmp.OnClusterLocationChanged(data)));

  protected virtual void OnPrefabInit() => this.Subscribe<OrbitalMechanics>(-1298331547, this.OnClusterLocationChangedDelegate);

  private void OnClusterLocationChanged(object data) => this.UpdateLocation(((ClusterLocationChangedEvent) data).newLocation);

  protected virtual void OnCleanUp()
  {
    if (this.orbitingObjects == null)
      return;
    foreach (Ref<OrbitalObject> orbitingObject in this.orbitingObjects)
    {
      if (!Util.IsNullOrDestroyed((object) orbitingObject.Get()))
        Util.KDestroyGameObject((Component) orbitingObject.Get());
    }
  }

  [ContextMenu("Rebuild")]
  private void Rebuild()
  {
    List<string> stringList = new List<string>();
    if (this.orbitingObjects != null)
    {
      foreach (Ref<OrbitalObject> orbitingObject in this.orbitingObjects)
      {
        if (!Util.IsNullOrDestroyed((object) orbitingObject.Get()))
        {
          stringList.Add(orbitingObject.Get().orbitalDBId);
          Util.KDestroyGameObject((Component) orbitingObject.Get());
        }
      }
      this.orbitingObjects = new List<Ref<OrbitalObject>>();
    }
    if (stringList.Count <= 0)
      return;
    for (int index = 0; index < stringList.Count; ++index)
      this.CreateOrbitalObject(stringList[index]);
  }

  private void UpdateLocation(AxialI location)
  {
    if (this.orbitingObjects.Count > 0)
    {
      foreach (Ref<OrbitalObject> orbitingObject in this.orbitingObjects)
      {
        if (!Util.IsNullOrDestroyed((object) orbitingObject.Get()))
          Util.KDestroyGameObject((Component) orbitingObject.Get());
      }
      this.orbitingObjects = new List<Ref<OrbitalObject>>();
    }
    ClusterGridEntity entityOfLayerAtCell = ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(location, EntityLayer.POI);
    if (Object.op_Inequality((Object) entityOfLayerAtCell, (Object) null))
    {
      ArtifactPOIClusterGridEntity component1 = ((Component) entityOfLayerAtCell).GetComponent<ArtifactPOIClusterGridEntity>();
      if (Object.op_Inequality((Object) component1, (Object) null))
      {
        ArtifactPOIStates.Instance smi = ((Component) component1).GetSMI<ArtifactPOIStates.Instance>();
        if (smi != null && smi.configuration.poiType.orbitalObject != null)
        {
          foreach (string orbit_db_name in smi.configuration.poiType.orbitalObject)
            this.CreateOrbitalObject(orbit_db_name);
        }
      }
      HarvestablePOIClusterGridEntity component2 = ((Component) entityOfLayerAtCell).GetComponent<HarvestablePOIClusterGridEntity>();
      if (!Object.op_Inequality((Object) component2, (Object) null))
        return;
      HarvestablePOIStates.Instance smi1 = ((Component) component2).GetSMI<HarvestablePOIStates.Instance>();
      if (smi1 == null || smi1.configuration.poiType.orbitalObject == null)
        return;
      List<string> orbitalObject = smi1.configuration.poiType.orbitalObject;
      KRandom krandom = new KRandom();
      float num = smi1.poiCapacity / smi1.configuration.GetMaxCapacity() * (float) smi1.configuration.poiType.maxNumOrbitingObjects;
      for (int index1 = 0; (double) index1 < (double) num; ++index1)
      {
        int index2 = krandom.Next(orbitalObject.Count);
        this.CreateOrbitalObject(orbitalObject[index2]);
      }
    }
    else
    {
      Clustercraft component = ((Component) this).GetComponent<Clustercraft>();
      if (!Object.op_Inequality((Object) component, (Object) null))
        return;
      if (Object.op_Inequality((Object) component.GetOrbitAsteroid(), (Object) null) || component.Status == Clustercraft.CraftStatus.Launching)
      {
        this.CreateOrbitalObject(Db.Get().OrbitalTypeCategories.orbit.Id);
      }
      else
      {
        if (component.Status != Clustercraft.CraftStatus.Landing)
          return;
        this.CreateOrbitalObject(Db.Get().OrbitalTypeCategories.landed.Id);
      }
    }
  }

  public void CreateOrbitalObject(string orbit_db_name)
  {
    WorldContainer component1 = ((Component) this).GetComponent<WorldContainer>();
    GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(OrbitalBGConfig.ID)), ((Component) this).gameObject, (string) null);
    OrbitalObject component2 = gameObject.GetComponent<OrbitalObject>();
    component2.Init(orbit_db_name, component1, this.orbitingObjects);
    gameObject.SetActive(true);
    this.orbitingObjects.Add(new Ref<OrbitalObject>(component2));
  }
}
