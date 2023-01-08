// Decompiled with JetBrains decompiler
// Type: AutoPlumberSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class AutoPlumberSideScreen : SideScreenContent
{
  public KButton activateButton;
  public KButton powerButton;
  public KButton pipesButton;
  public KButton solidsButton;
  public KButton minionButton;
  public KButton applyTestFacade;
  private Building building;

  protected virtual void OnSpawn()
  {
    this.activateButton.onClick += (System.Action) (() => DevAutoPlumber.AutoPlumbBuilding(this.building));
    this.powerButton.onClick += (System.Action) (() => DevAutoPlumber.DoElectricalPlumbing(this.building));
    this.pipesButton.onClick += (System.Action) (() => DevAutoPlumber.DoLiquidAndGasPlumbing(this.building));
    this.solidsButton.onClick += (System.Action) (() => DevAutoPlumber.SetupSolidOreDelivery(this.building));
    this.minionButton.onClick += (System.Action) (() => this.SpawnMinion());
    this.applyTestFacade.onClick += (System.Action) (() => this.CycleAvailableFacades());
  }

  private void SpawnMinion()
  {
    GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(MinionConfig.ID)), (GameObject) null, (string) null);
    ((Object) gameObject).name = ((Object) Assets.GetPrefab(Tag.op_Implicit(MinionConfig.ID))).name;
    Immigration.Instance.ApplyDefaultPersonalPriorities(gameObject);
    Vector3 pos = Grid.CellToPos(Grid.PosToCell((KMonoBehaviour) this.building), (CellAlignment) 1, Grid.SceneLayer.Move);
    TransformExtensions.SetLocalPosition(gameObject.transform, pos);
    gameObject.SetActive(true);
    new MinionStartingStats(false, isDebugMinion: true).Apply(gameObject);
  }

  public override int GetSideScreenSortOrder() => -150;

  public override bool IsValidForTarget(GameObject target) => DebugHandler.InstantBuildMode && Object.op_Inequality((Object) target.GetComponent<Building>(), (Object) null);

  public override void SetTarget(GameObject target)
  {
    this.building = target.GetComponent<Building>();
    this.Refresh();
  }

  public override void ClearTarget()
  {
  }

  private void Refresh() => ((Component) this.applyTestFacade).gameObject.SetActive(Object.op_Inequality((Object) this.building, (Object) null) && this.building.Def.AvailableFacades.Count > 0);

  private void CycleAvailableFacades()
  {
    BuildingFacade component = ((Component) this.building).GetComponent<BuildingFacade>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    string nextFacade = component.GetNextFacade();
    component.ApplyBuildingFacade(Db.GetBuildingFacades().TryGet(nextFacade));
  }
}
