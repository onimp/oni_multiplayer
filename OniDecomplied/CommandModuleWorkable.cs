// Decompiled with JetBrains decompiler
// Type: CommandModuleWorkable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/CommandModuleWorkable")]
public class CommandModuleWorkable : Workable
{
  private static CellOffset[] entryOffsets = new CellOffset[5]
  {
    new CellOffset(0, 0),
    new CellOffset(0, 1),
    new CellOffset(0, 2),
    new CellOffset(0, 3),
    new CellOffset(0, 4)
  };

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.SetOffsets(CommandModuleWorkable.entryOffsets);
    this.synchronizeAnims = false;
    this.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_incubator_kanim"))
    };
    this.SetWorkTime(float.PositiveInfinity);
    this.showProgressBar = false;
  }

  protected override void OnStartWork(Worker worker) => base.OnStartWork(worker);

  protected override bool OnWorkTick(Worker worker, float dt)
  {
    if (!Object.op_Inequality((Object) worker, (Object) null))
      return base.OnWorkTick(worker, dt);
    if (DlcManager.IsExpansion1Active())
    {
      GameObject gameObject = ((Component) worker).gameObject;
      this.CompleteWork(worker);
      ((Component) this).GetComponent<ClustercraftExteriorDoor>().FerryMinion(gameObject);
      return true;
    }
    GameObject gameObject1 = ((Component) worker).gameObject;
    this.CompleteWork(worker);
    ((Component) this).GetComponent<MinionStorage>().SerializeMinion(gameObject1);
    return true;
  }

  protected override void OnStopWork(Worker worker) => base.OnStopWork(worker);

  protected override void OnCompleteWork(Worker worker)
  {
  }
}
