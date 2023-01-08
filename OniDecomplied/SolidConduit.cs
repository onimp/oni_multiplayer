// Decompiled with JetBrains decompiler
// Type: SolidConduit
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/SolidConduit")]
public class SolidConduit : KMonoBehaviour, IFirstFrameCallback, IHaveUtilityNetworkMgr
{
  [MyCmpReq]
  private KAnimGraphTileVisualizer graphTileDependency;
  private System.Action firstFrameCallback;

  public void SetFirstFrameCallback(System.Action ffCb)
  {
    this.firstFrameCallback = ffCb;
    ((MonoBehaviour) this).StartCoroutine(this.RunCallback());
  }

  private IEnumerator RunCallback()
  {
    yield return (object) null;
    if (this.firstFrameCallback != null)
    {
      this.firstFrameCallback();
      this.firstFrameCallback = (System.Action) null;
    }
    yield return (object) null;
  }

  public IUtilityNetworkMgr GetNetworkManager() => (IUtilityNetworkMgr) Game.Instance.solidConduitSystem;

  public UtilityNetwork GetNetwork() => this.GetNetworkManager().GetNetworkForCell(Grid.PosToCell((KMonoBehaviour) this));

  public static SolidConduitFlow GetFlowManager() => Game.Instance.solidConduitFlow;

  public Vector3 Position => TransformExtensions.GetPosition(this.transform);

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    ((Component) this).GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Conveyor, (object) this);
  }

  protected virtual void OnCleanUp()
  {
    int cell = Grid.PosToCell((KMonoBehaviour) this);
    BuildingComplete component = ((Component) this).GetComponent<BuildingComplete>();
    if (component.Def.ReplacementLayer == ObjectLayer.NumLayers || Object.op_Equality((Object) Grid.Objects[cell, (int) component.Def.ReplacementLayer], (Object) null))
    {
      this.GetNetworkManager().RemoveFromNetworks(cell, (object) this, false);
      SolidConduit.GetFlowManager().EmptyConduit(cell);
    }
    base.OnCleanUp();
  }
}
