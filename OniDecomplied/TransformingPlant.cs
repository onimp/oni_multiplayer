// Decompiled with JetBrains decompiler
// Type: TransformingPlant
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using UnityEngine;

public class TransformingPlant : KMonoBehaviour
{
  public string transformPlantId;
  public Func<object, bool> eventDataCondition;
  public bool useGrowthTimeRatio;
  public bool keepPlantablePlotStorage = true;
  public string fxKAnim;
  public string fxAnim;
  private static readonly EventSystem.IntraObjectHandler<TransformingPlant> OnTransformationEventDelegate = new EventSystem.IntraObjectHandler<TransformingPlant>((Action<TransformingPlant, object>) ((component, data) => component.DoPlantTransform(data)));

  public void SubscribeToTransformEvent(GameHashes eventHash) => this.Subscribe<TransformingPlant>((int) eventHash, TransformingPlant.OnTransformationEventDelegate);

  public void UnsubscribeToTransformEvent(GameHashes eventHash) => this.Unsubscribe<TransformingPlant>((int) eventHash, TransformingPlant.OnTransformationEventDelegate, false);

  private void DoPlantTransform(object data)
  {
    if (this.eventDataCondition != null && !this.eventDataCondition(data))
      return;
    GameObject plant = GameUtil.KInstantiate(Assets.GetPrefab(TagExtensions.ToTag(this.transformPlantId)), Grid.SceneLayer.BuildingBack);
    TransformExtensions.SetPosition(plant.transform, TransformExtensions.GetPosition(this.transform));
    MutantPlant component1 = ((Component) this).GetComponent<MutantPlant>();
    MutantPlant component2 = plant.GetComponent<MutantPlant>();
    if (Object.op_Inequality((Object) component1, (Object) null) && Object.op_Inequality((Object) plant, (Object) null))
    {
      component1.CopyMutationsTo(component2);
      PlantSubSpeciesCatalog.Instance.IdentifySubSpecies(component2.SubSpeciesID);
    }
    plant.SetActive(true);
    Growing component3 = ((Component) this).GetComponent<Growing>();
    Growing component4 = plant.GetComponent<Growing>();
    if (Object.op_Inequality((Object) component3, (Object) null) && Object.op_Inequality((Object) component4, (Object) null))
    {
      float percent = component3.PercentGrown();
      if (this.useGrowthTimeRatio)
      {
        AmountInstance amountInstance1 = component3.GetAmounts().Get(Db.Get().Amounts.Maturity);
        AmountInstance amountInstance2 = component4.GetAmounts().Get(Db.Get().Amounts.Maturity);
        float num = amountInstance1.GetMax() / amountInstance2.GetMax();
        percent = Mathf.Clamp01(percent * num);
      }
      component4.OverrideMaturityLevel(percent);
    }
    PrimaryElement component5 = plant.GetComponent<PrimaryElement>();
    PrimaryElement component6 = ((Component) this).GetComponent<PrimaryElement>();
    component5.Temperature = component6.Temperature;
    component5.AddDisease(component6.DiseaseIdx, component6.DiseaseCount, "TransformedPlant");
    plant.GetComponent<Effects>().CopyEffects(((Component) this).GetComponent<Effects>());
    HarvestDesignatable component7 = ((Component) this).GetComponent<HarvestDesignatable>();
    HarvestDesignatable component8 = plant.GetComponent<HarvestDesignatable>();
    if (Object.op_Inequality((Object) component7, (Object) null) && Object.op_Inequality((Object) component8, (Object) null))
      component8.SetHarvestWhenReady(component7.HarvestWhenReady);
    Prioritizable component9 = ((Component) this).GetComponent<Prioritizable>();
    Prioritizable component10 = plant.GetComponent<Prioritizable>();
    if (Object.op_Inequality((Object) component9, (Object) null) && Object.op_Inequality((Object) component10, (Object) null))
      component10.SetMasterPriority(component9.GetMasterPriority());
    PlantablePlot receptacle = ((Component) this).GetComponent<ReceptacleMonitor>().GetReceptacle();
    if (Object.op_Inequality((Object) receptacle, (Object) null))
      receptacle.ReplacePlant(plant, this.keepPlantablePlotStorage);
    Util.KDestroyGameObject(((Component) this).gameObject);
    if (this.fxKAnim == null)
      return;
    KBatchedAnimController effect = FXHelpers.CreateEffect(this.fxKAnim, plant.transform.position, layer: Grid.SceneLayer.FXFront);
    effect.Play(HashedString.op_Implicit(this.fxAnim));
    effect.destroyOnAnimComplete = true;
  }
}
