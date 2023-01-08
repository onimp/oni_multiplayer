// Decompiled with JetBrains decompiler
// Type: Unsealable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Unsealable")]
public class Unsealable : Workable
{
  [Serialize]
  public bool facingRight;
  [Serialize]
  public bool unsealed;

  private Unsealable()
  {
  }

  public override CellOffset[] GetOffsets(int cell) => this.facingRight ? OffsetGroups.RightOnly : OffsetGroups.LeftOnly;

  protected override void OnPrefabInit()
  {
    this.faceTargetWhenWorking = true;
    base.OnPrefabInit();
    this.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_door_poi_kanim"))
    };
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.SetWorkTime(3f);
    if (!this.unsealed)
      return;
    Deconstructable component = ((Component) this).GetComponent<Deconstructable>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.allowDeconstruction = true;
  }

  protected override void OnStartWork(Worker worker) => base.OnStartWork(worker);

  protected override void OnCompleteWork(Worker worker)
  {
    this.unsealed = true;
    base.OnCompleteWork(worker);
    Deconstructable component = ((Component) this).GetComponent<Deconstructable>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.allowDeconstruction = true;
    Game.Instance.Trigger(1980521255, (object) ((Component) this).gameObject);
  }
}
