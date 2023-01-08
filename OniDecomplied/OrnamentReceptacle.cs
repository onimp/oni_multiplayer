// Decompiled with JetBrains decompiler
// Type: OrnamentReceptacle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class OrnamentReceptacle : SingleEntityReceptacle
{
  [MyCmpReq]
  private SnapOn snapOn;
  private KBatchedAnimTracker occupyingTracker;
  private KAnimLink animLink;

  protected override void OnPrefabInit() => base.OnPrefabInit();

  protected override void OnSpawn()
  {
    base.OnSpawn();
    ((Component) this).GetComponent<KBatchedAnimController>().SetSymbolVisiblity(KAnimHashedString.op_Implicit("snapTo_ornament"), false);
  }

  protected override void PositionOccupyingObject()
  {
    KBatchedAnimController component = this.occupyingObject.GetComponent<KBatchedAnimController>();
    TransformExtensions.SetLocalPosition(((Component) component).transform, new Vector3(0.0f, 0.0f, -0.1f));
    this.occupyingTracker = this.occupyingObject.AddComponent<KBatchedAnimTracker>();
    this.occupyingTracker.symbol = new HashedString("snapTo_ornament");
    this.occupyingTracker.forceAlwaysVisible = true;
    this.animLink = new KAnimLink((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), (KAnimControllerBase) component);
  }

  protected override void ClearOccupant()
  {
    if (Object.op_Inequality((Object) this.occupyingTracker, (Object) null))
    {
      Object.Destroy((Object) this.occupyingTracker);
      this.occupyingTracker = (KBatchedAnimTracker) null;
    }
    if (this.animLink != null)
    {
      this.animLink.Unregister();
      this.animLink = (KAnimLink) null;
    }
    base.ClearOccupant();
  }
}
