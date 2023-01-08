// Decompiled with JetBrains decompiler
// Type: ResearchModule
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ResearchModule")]
public class ResearchModule : KMonoBehaviour
{
  private static readonly EventSystem.IntraObjectHandler<ResearchModule> OnLaunchDelegate = new EventSystem.IntraObjectHandler<ResearchModule>((Action<ResearchModule, object>) ((component, data) => component.OnLaunch(data)));
  private static readonly EventSystem.IntraObjectHandler<ResearchModule> OnLandDelegate = new EventSystem.IntraObjectHandler<ResearchModule>((Action<ResearchModule, object>) ((component, data) => component.OnLand(data)));

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    ((Component) this).GetComponent<KBatchedAnimController>().Play(HashedString.op_Implicit("grounded"), (KAnim.PlayMode) 0);
    this.Subscribe<ResearchModule>(-1277991738, ResearchModule.OnLaunchDelegate);
    this.Subscribe<ResearchModule>(-887025858, ResearchModule.OnLandDelegate);
  }

  public void OnLaunch(object data)
  {
  }

  public void OnLand(object data)
  {
    if (!DlcManager.FeatureClusterSpaceEnabled())
    {
      SpaceDestination.ResearchOpportunity researchOpportunity = SpacecraftManager.instance.GetSpacecraftDestination(SpacecraftManager.instance.GetSpacecraftID(((Component) ((Component) this).GetComponent<RocketModule>().conditionManager).GetComponent<ILaunchableRocket>())).TryCompleteResearchOpportunity();
      if (researchOpportunity != null)
      {
        GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(Tag.op_Implicit("ResearchDatabank")), TransformExtensions.GetPosition(((Component) this).gameObject.transform), Grid.SceneLayer.Ore);
        gameObject.SetActive(true);
        gameObject.GetComponent<PrimaryElement>().Mass = (float) researchOpportunity.dataValue;
        if (!string.IsNullOrEmpty(researchOpportunity.discoveredRareItem))
        {
          GameObject prefab = Assets.GetPrefab(Tag.op_Implicit(researchOpportunity.discoveredRareItem));
          if (Object.op_Equality((Object) prefab, (Object) null))
            KCrashReporter.Assert(false, "Missing prefab: " + researchOpportunity.discoveredRareItem);
          else
            GameUtil.KInstantiate(prefab, TransformExtensions.GetPosition(((Component) this).gameObject.transform), Grid.SceneLayer.Ore).SetActive(true);
        }
      }
    }
    GameObject gameObject1 = GameUtil.KInstantiate(Assets.GetPrefab(Tag.op_Implicit("ResearchDatabank")), TransformExtensions.GetPosition(((Component) this).gameObject.transform), Grid.SceneLayer.Ore);
    gameObject1.SetActive(true);
    gameObject1.GetComponent<PrimaryElement>().Mass = (float) ROCKETRY.DESTINATION_RESEARCH.EVERGREEN;
  }
}
