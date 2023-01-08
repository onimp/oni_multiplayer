// Decompiled with JetBrains decompiler
// Type: BalloonStandConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using UnityEngine;

public class BalloonStandConfig : IEntityConfig
{
  public static readonly string ID = "BalloonStand";
  private Chore.Precondition HasNoBalloon = new Chore.Precondition()
  {
    id = nameof (HasNoBalloon),
    description = "Duplicant doesn't have a balloon already",
    fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) => !Object.op_Equality((Object) context.consumerState.consumer, (Object) null) && !context.consumerState.gameObject.GetComponent<Effects>().HasEffect("HasBalloon"))
  };

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject entity = EntityTemplates.CreateEntity(BalloonStandConfig.ID, BalloonStandConfig.ID, false);
    KAnimFile[] kanimFileArray = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_balloon_receiver_kanim"))
    };
    GetBalloonWorkable getBalloonWorkable = entity.AddOrGet<GetBalloonWorkable>();
    getBalloonWorkable.workTime = 2f;
    getBalloonWorkable.workLayer = Grid.SceneLayer.BuildingFront;
    getBalloonWorkable.overrideAnims = kanimFileArray;
    getBalloonWorkable.synchronizeAnims = false;
    return entity;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
    GetBalloonWorkable component = inst.GetComponent<GetBalloonWorkable>();
    WorkChore<GetBalloonWorkable> data = new WorkChore<GetBalloonWorkable>(Db.Get().ChoreTypes.JoyReaction, (IStateMachineTarget) component, on_complete: new Action<Chore>(this.MakeNewBalloonChore), schedule_block: Db.Get().ScheduleBlockTypes.Recreation, priority_class: PriorityScreen.PriorityClass.high, ignore_building_assignment: true);
    data.AddPrecondition(this.HasNoBalloon, (object) data);
    data.AddPrecondition(ChorePreconditions.instance.IsNotARobot, (object) data);
  }

  private void MakeNewBalloonChore(Chore chore)
  {
    GetBalloonWorkable component = chore.target.GetComponent<GetBalloonWorkable>();
    WorkChore<GetBalloonWorkable> data = new WorkChore<GetBalloonWorkable>(Db.Get().ChoreTypes.JoyReaction, (IStateMachineTarget) component, on_complete: new Action<Chore>(this.MakeNewBalloonChore), schedule_block: Db.Get().ScheduleBlockTypes.Recreation, priority_class: PriorityScreen.PriorityClass.high, ignore_building_assignment: true);
    data.AddPrecondition(this.HasNoBalloon, (object) data);
    data.AddPrecondition(ChorePreconditions.instance.IsNotARobot, (object) data);
  }
}
