// Decompiled with JetBrains decompiler
// Type: AttackableBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/AttackableBase")]
public class AttackableBase : Workable, IApproachable
{
  private HandleVector<int>.Handle scenePartitionerEntry;
  private static readonly EventSystem.IntraObjectHandler<AttackableBase> OnDeadTagAddedDelegate = GameUtil.CreateHasTagHandler<AttackableBase>(GameTags.Dead, (Action<AttackableBase, object>) ((component, data) => component.OnDefeated(data)));
  private static readonly EventSystem.IntraObjectHandler<AttackableBase> OnDefeatedDelegate = new EventSystem.IntraObjectHandler<AttackableBase>((Action<AttackableBase, object>) ((component, data) => component.OnDefeated(data)));
  private static readonly EventSystem.IntraObjectHandler<AttackableBase> SetupScenePartitionerDelegate = new EventSystem.IntraObjectHandler<AttackableBase>((Action<AttackableBase, object>) ((component, data) => component.SetupScenePartitioner(data)));
  private static readonly EventSystem.IntraObjectHandler<AttackableBase> OnCellChangedDelegate = new EventSystem.IntraObjectHandler<AttackableBase>((Action<AttackableBase, object>) ((component, data) => GameScenePartitioner.Instance.UpdatePosition(component.scenePartitionerEntry, Grid.PosToCell(((Component) component).gameObject))));

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.attributeConverter = Db.Get().AttributeConverters.AttackDamage;
    this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.BARELY_EVER_EXPERIENCE;
    this.skillExperienceSkillGroup = Db.Get().SkillGroups.Mining.Id;
    this.skillExperienceMultiplier = SKILLS.BARELY_EVER_EXPERIENCE;
    this.SetupScenePartitioner();
    this.Subscribe<AttackableBase>(1088554450, AttackableBase.OnCellChangedDelegate);
    GameUtil.SubscribeToTags<AttackableBase>(this, AttackableBase.OnDeadTagAddedDelegate, true);
    this.Subscribe<AttackableBase>(-1506500077, AttackableBase.OnDefeatedDelegate);
    this.Subscribe<AttackableBase>(-1256572400, AttackableBase.SetupScenePartitionerDelegate);
  }

  public float GetDamageMultiplier() => this.attributeConverter != null && Object.op_Inequality((Object) this.worker, (Object) null) ? Mathf.Max(1f + ((Component) this.worker).GetComponent<AttributeConverters>().GetConverter(this.attributeConverter.Id).Evaluate(), 0.1f) : 1f;

  private void SetupScenePartitioner(object data = null)
  {
    Extents extents = new Extents(Grid.PosToXY(TransformExtensions.GetPosition(this.transform)).x, Grid.PosToXY(TransformExtensions.GetPosition(this.transform)).y, 1, 1);
    this.scenePartitionerEntry = GameScenePartitioner.Instance.Add(((Object) ((Component) this).gameObject).name, (object) ((Component) this).GetComponent<FactionAlignment>(), extents, GameScenePartitioner.Instance.attackableEntitiesLayer, (Action<object>) null);
  }

  private void OnDefeated(object data = null) => GameScenePartitioner.Instance.Free(ref this.scenePartitionerEntry);

  public override float GetEfficiencyMultiplier(Worker worker) => 1f;

  protected override void OnCleanUp()
  {
    this.Unsubscribe<AttackableBase>(1088554450, AttackableBase.OnCellChangedDelegate, false);
    GameUtil.UnsubscribeToTags<AttackableBase>(this, AttackableBase.OnDeadTagAddedDelegate);
    this.Unsubscribe<AttackableBase>(-1506500077, AttackableBase.OnDefeatedDelegate, false);
    this.Unsubscribe<AttackableBase>(-1256572400, AttackableBase.SetupScenePartitionerDelegate, false);
    GameScenePartitioner.Instance.Free(ref this.scenePartitionerEntry);
    base.OnCleanUp();
  }
}
