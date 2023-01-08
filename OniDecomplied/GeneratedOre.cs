// Decompiled with JetBrains decompiler
// Type: GeneratedOre
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedOre
{
  public static void LoadGeneratedOre(List<System.Type> types)
  {
    System.Type type1 = typeof (IOreConfig);
    HashSet<SimHashes> simHashesSet = new HashSet<SimHashes>();
    foreach (System.Type type2 in types)
    {
      if (type1.IsAssignableFrom(type2) && !type2.IsAbstract && !type2.IsInterface)
      {
        IOreConfig instance = Activator.CreateInstance(type2) as IOreConfig;
        SimHashes elementId = instance.ElementID;
        Element elementByHash = ElementLoader.FindElementByHash(elementId);
        if (elementByHash != null && DlcManager.IsContentActive(elementByHash.dlcId))
        {
          if (elementId != SimHashes.Void)
            simHashesSet.Add(elementId);
          Assets.AddPrefab(instance.CreatePrefab().GetComponent<KPrefabID>());
        }
      }
    }
    foreach (Element element in ElementLoader.elements)
    {
      if (element != null && !simHashesSet.Contains(element.id) && DlcManager.IsContentActive(element.dlcId) && element.substance != null && Object.op_Inequality((Object) element.substance.anim, (Object) null))
      {
        GameObject gameObject = (GameObject) null;
        if (element.IsSolid)
          gameObject = EntityTemplates.CreateSolidOreEntity(element.id);
        else if (element.IsLiquid)
          gameObject = EntityTemplates.CreateLiquidOreEntity(element.id);
        else if (element.IsGas)
          gameObject = EntityTemplates.CreateGasOreEntity(element.id);
        if (Object.op_Inequality((Object) gameObject, (Object) null))
          Assets.AddPrefab(gameObject.GetComponent<KPrefabID>());
      }
    }
  }

  public static SubstanceChunk CreateChunk(
    Element element,
    float mass,
    float temperature,
    byte diseaseIdx,
    int diseaseCount,
    Vector3 position)
  {
    if ((double) temperature <= 0.0)
      DebugUtil.LogWarningArgs(new object[1]
      {
        (object) "GeneratedOre.CreateChunk tried to create a chunk with a temperature <= 0"
      });
    GameObject prefab = Assets.GetPrefab(element.tag);
    if (Object.op_Equality((Object) prefab, (Object) null))
      Debug.LogError((object) ("Could not find prefab for element " + element.id.ToString()));
    SubstanceChunk component1 = GameUtil.KInstantiate(prefab, Grid.SceneLayer.Ore).GetComponent<SubstanceChunk>();
    TransformExtensions.SetPosition(component1.transform, position);
    ((Component) component1).gameObject.SetActive(true);
    PrimaryElement component2 = ((Component) component1).GetComponent<PrimaryElement>();
    component2.Mass = mass;
    component2.Temperature = temperature;
    component2.AddDisease(diseaseIdx, diseaseCount, "GeneratedOre.CreateChunk");
    ((Component) component1).GetComponent<KPrefabID>().InitializeTags(false);
    return component1;
  }
}
