// Decompiled with JetBrains decompiler
// Type: GetBalloonWorkable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/GetBalloonWorkable")]
public class GetBalloonWorkable : Workable
{
  private static readonly HashedString[] GET_BALLOON_ANIMS = new HashedString[2]
  {
    HashedString.op_Implicit("working_pre"),
    HashedString.op_Implicit("working_loop")
  };
  private static readonly HashedString PST_ANIM = new HashedString("working_pst");
  private BalloonArtistChore.StatesInstance balloonArtist;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.faceTargetWhenWorking = true;
    this.workerStatusItem = (StatusItem) null;
    this.workingStatusItem = (StatusItem) null;
    this.workAnims = GetBalloonWorkable.GET_BALLOON_ANIMS;
    this.workingPstComplete = new HashedString[1]
    {
      GetBalloonWorkable.PST_ANIM
    };
    this.workingPstFailed = new HashedString[1]
    {
      GetBalloonWorkable.PST_ANIM
    };
  }

  protected override void OnCompleteWork(Worker worker)
  {
    this.balloonArtist.GiveBalloon();
    GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit("EquippableBalloon")), TransformExtensions.GetPosition(worker.transform));
    gameObject.GetComponent<Equippable>().Assign((IAssignableIdentity) ((Component) worker).GetComponent<MinionIdentity>());
    gameObject.GetComponent<Equippable>().isEquipped = true;
    gameObject.SetActive(true);
    base.OnCompleteWork(worker);
  }

  public override Vector3 GetFacingTarget() => TransformExtensions.GetPosition(this.balloonArtist.master.transform);

  public void SetBalloonArtist(BalloonArtistChore.StatesInstance chore) => this.balloonArtist = chore;
}
