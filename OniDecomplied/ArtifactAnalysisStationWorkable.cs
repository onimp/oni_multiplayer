// Decompiled with JetBrains decompiler
// Type: ArtifactAnalysisStationWorkable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using TUNING;
using UnityEngine;

public class ArtifactAnalysisStationWorkable : Workable
{
  [MyCmpAdd]
  public Notifier notifier;
  [MyCmpReq]
  public Storage storage;
  [SerializeField]
  public Vector3 finishedArtifactDropOffset;
  private Notification notification;
  public ArtifactAnalysisStation.StatesInstance statesInstance;
  private KBatchedAnimController animController;
  [Serialize]
  private float nextYeildRoll = -1f;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.requiredSkillPerk = Db.Get().SkillPerks.CanStudyArtifact.Id;
    this.workerStatusItem = Db.Get().DuplicantStatusItems.AnalyzingArtifact;
    this.attributeConverter = Db.Get().AttributeConverters.ArtSpeed;
    this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
    this.skillExperienceSkillGroup = Db.Get().SkillGroups.Research.Id;
    this.skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
    this.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_artifact_analysis_kanim"))
    };
    this.SetWorkTime(150f);
    this.showProgressBar = true;
    this.lightEfficiencyBonus = true;
    Components.ArtifactAnalysisStations.Add(this);
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.animController = ((Component) this).GetComponent<KBatchedAnimController>();
    this.animController.SetSymbolVisiblity(KAnimHashedString.op_Implicit("snapTo_artifact"), false);
  }

  protected override void OnCleanUp()
  {
    base.OnCleanUp();
    Components.ArtifactAnalysisStations.Remove(this);
  }

  protected override void OnStartWork(Worker worker)
  {
    base.OnStartWork(worker);
    this.InitialDisplayStoredArtifact();
  }

  protected override bool OnWorkTick(Worker worker, float dt)
  {
    this.PositionArtifact();
    return base.OnWorkTick(worker, dt);
  }

  private void InitialDisplayStoredArtifact()
  {
    GameObject gameObject = ((Component) this).GetComponent<Storage>().GetItems()[0];
    KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
    if (Object.op_Inequality((Object) component, (Object) null))
      component.GetBatchInstanceData().ClearOverrideTransformMatrix();
    TransformExtensions.SetPosition(gameObject.transform, new Vector3(this.transform.position.x, this.transform.position.y, Grid.GetLayerZ(Grid.SceneLayer.BuildingBack)));
    gameObject.SetActive(true);
    component.enabled = false;
    component.enabled = true;
    this.PositionArtifact();
  }

  private void ReleaseStoredArtifact()
  {
    Storage component1 = ((Component) this).GetComponent<Storage>();
    GameObject go = component1.GetItems()[0];
    KBatchedAnimController component2 = go.GetComponent<KBatchedAnimController>();
    TransformExtensions.SetPosition(go.transform, new Vector3(go.transform.position.x, go.transform.position.y, Grid.GetLayerZ(Grid.SceneLayer.Ore)));
    component2.enabled = false;
    component2.enabled = true;
    component1.Drop(go, true);
  }

  private void PositionArtifact()
  {
    GameObject gameObject = ((Component) this).GetComponent<Storage>().GetItems()[0];
    Matrix4x4 symbolTransform = this.animController.GetSymbolTransform(HashedString.op_Implicit("snapTo_artifact"), out bool _);
    Vector3 vector3 = Vector4.op_Implicit(((Matrix4x4) ref symbolTransform).GetColumn(3));
    vector3.z = Grid.GetLayerZ(Grid.SceneLayer.BuildingBack);
    TransformExtensions.SetPosition(gameObject.transform, vector3);
  }

  protected override void OnCompleteWork(Worker worker)
  {
    base.OnCompleteWork(worker);
    this.ConsumeCharm();
    this.ReleaseStoredArtifact();
  }

  private void ConsumeCharm()
  {
    GameObject first = this.storage.FindFirst(GameTags.CharmedArtifact);
    DebugUtil.DevAssertArgs((Object.op_Inequality((Object) first, (Object) null) ? 1 : 0) != 0, new object[1]
    {
      (object) "ArtifactAnalysisStation finished studying a charmed artifact but there is not one in its storage"
    });
    if (Object.op_Inequality((Object) first, (Object) null))
    {
      this.YieldPayload(first.GetComponent<SpaceArtifact>());
      first.GetComponent<SpaceArtifact>().RemoveCharm();
    }
    if (!ArtifactSelector.Instance.RecordArtifactAnalyzed(first.GetComponent<KPrefabID>().PrefabID().ToString()))
      return;
    if (first.HasTag(GameTags.TerrestrialArtifact))
      ArtifactSelector.Instance.IncrementAnalyzedTerrestrialArtifacts();
    else
      ArtifactSelector.Instance.IncrementAnalyzedSpaceArtifacts();
  }

  private void YieldPayload(SpaceArtifact artifact)
  {
    if ((double) this.nextYeildRoll == -1.0)
      this.nextYeildRoll = Random.Range(0.0f, 1f);
    if ((double) this.nextYeildRoll <= (double) artifact.GetArtifactTier().payloadDropChance)
      GameUtil.KInstantiate(Assets.GetPrefab(Tag.op_Implicit("GeneShufflerRecharge")), Vector3.op_Addition(this.statesInstance.master.transform.position, this.finishedArtifactDropOffset), Grid.SceneLayer.Ore).SetActive(true);
    int num = Mathf.FloorToInt(artifact.GetArtifactTier().payloadDropChance * 20f);
    for (int index = 0; index < num; ++index)
      GameUtil.KInstantiate(Assets.GetPrefab(Tag.op_Implicit("OrbitalResearchDatabank")), Vector3.op_Addition(this.statesInstance.master.transform.position, this.finishedArtifactDropOffset), Grid.SceneLayer.Ore).SetActive(true);
    this.nextYeildRoll = Random.Range(0.0f, 1f);
  }
}
