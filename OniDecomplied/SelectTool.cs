// Decompiled with JetBrains decompiler
// Type: SelectTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using System;
using UnityEngine;

public class SelectTool : InterfaceTool
{
  public KSelectable selected;
  protected int cell_new;
  private int selectedCell;
  protected int defaultLayerMask;
  public static SelectTool Instance;
  private KSelectable delayedNextSelection;
  private bool delayedSkipSound;
  private KSelectable previousSelection;

  public static void DestroyInstance() => SelectTool.Instance = (SelectTool) null;

  protected virtual void OnPrefabInit()
  {
    this.defaultLayerMask = 1 | LayerMask.GetMask(new string[7]
    {
      "World",
      "Pickupable",
      "Place",
      "PlaceWithDepth",
      "BlockSelection",
      "Construction",
      "Selection"
    });
    this.layerMask = this.defaultLayerMask;
    this.selectMarker = Util.KInstantiateUI<SelectMarker>(EntityPrefabs.Instance.SelectMarker, GameScreenManager.Instance.worldSpaceCanvas, false);
    ((Component) this.selectMarker).gameObject.SetActive(false);
    this.populateHitsList = true;
    SelectTool.Instance = this;
  }

  public void Activate()
  {
    PlayerController.Instance.ActivateTool((InterfaceTool) this);
    ToolMenu.Instance.PriorityScreen.ResetPriority();
    this.Select((KSelectable) null);
  }

  public void SetLayerMask(int mask)
  {
    this.layerMask = mask;
    this.ClearHover();
    this.LateUpdate();
  }

  public void ClearLayerMask() => this.layerMask = this.defaultLayerMask;

  public int GetDefaultLayerMask() => this.defaultLayerMask;

  protected override void OnDeactivateTool(InterfaceTool new_tool)
  {
    base.OnDeactivateTool(new_tool);
    this.ClearHover();
    this.Select((KSelectable) null);
  }

  public void Focus(Vector3 pos, KSelectable selectable, Vector3 offset)
  {
    if (Object.op_Inequality((Object) selectable, (Object) null))
      pos = TransformExtensions.GetPosition(selectable.transform);
    pos.z = -40f;
    pos = Vector3.op_Addition(pos, offset);
    WorldContainer worldFromPosition = ClusterManager.Instance.GetWorldFromPosition(pos);
    if (Object.op_Inequality((Object) worldFromPosition, (Object) null))
      CameraController.Instance.ActiveWorldStarWipe(worldFromPosition.id, pos);
    else
      DebugUtil.DevLogError("DevError: specified camera focus position has null world - possible out of bounds location");
  }

  public void SelectAndFocus(Vector3 pos, KSelectable selectable, Vector3 offset)
  {
    this.Focus(pos, selectable, offset);
    this.Select(selectable);
  }

  public void SelectAndFocus(Vector3 pos, KSelectable selectable) => this.SelectAndFocus(pos, selectable, Vector3.zero);

  public void SelectNextFrame(KSelectable new_selected, bool skipSound = false)
  {
    this.delayedNextSelection = new_selected;
    this.delayedSkipSound = skipSound;
    UIScheduler.Instance.ScheduleNextFrame("DelayedSelect", new Action<object>(this.DoSelectNextFrame));
  }

  private void DoSelectNextFrame(object data)
  {
    this.Select(this.delayedNextSelection, this.delayedSkipSound);
    this.delayedNextSelection = (KSelectable) null;
  }

  public void Select(KSelectable new_selected, bool skipSound = false)
  {
    if (Object.op_Equality((Object) new_selected, (Object) this.previousSelection))
      return;
    this.previousSelection = new_selected;
    if (Object.op_Inequality((Object) this.selected, (Object) null))
      this.selected.Unselect();
    GameObject gameObject = (GameObject) null;
    if (Object.op_Inequality((Object) new_selected, (Object) null) && new_selected.GetMyWorldId() == ClusterManager.Instance.activeWorldId)
    {
      SelectToolHoverTextCard component = ((Component) this).GetComponent<SelectToolHoverTextCard>();
      if (Object.op_Inequality((Object) component, (Object) null))
      {
        int selectedSelectableIndex = component.currentSelectedSelectableIndex;
        int displayedSelectables = component.recentNumberOfDisplayedSelectables;
        if (displayedSelectables != 0)
        {
          int num = (selectedSelectableIndex + 1) % displayedSelectables;
          if (!skipSound)
          {
            if (displayedSelectables == 1)
            {
              KFMOD.PlayUISound(GlobalAssets.GetSound("Select_empty"));
            }
            else
            {
              EventInstance instance = KFMOD.BeginOneShot(GlobalAssets.GetSound("Select_full"), Vector3.zero, 1f);
              ((EventInstance) ref instance).setParameterByName("selection", (float) num, false);
              SoundEvent.EndOneShot(instance);
            }
            this.playedSoundThisFrame = true;
          }
        }
      }
      if (Object.op_Equality((Object) new_selected, (Object) this.hover))
        this.ClearHover();
      new_selected.Select();
      gameObject = ((Component) new_selected).gameObject;
      this.selectMarker.SetTargetTransform(gameObject.transform);
      ((Component) this.selectMarker).gameObject.SetActive(!new_selected.DisableSelectMarker);
    }
    else if (Object.op_Inequality((Object) this.selectMarker, (Object) null))
      ((Component) this.selectMarker).gameObject.SetActive(false);
    this.selected = Object.op_Equality((Object) gameObject, (Object) null) ? (KSelectable) null : new_selected;
    Game.Instance.Trigger(-1503271301, (object) gameObject);
  }

  public override void OnLeftClickDown(Vector3 cursor_pos)
  {
    KSelectable objectUnderCursor = this.GetObjectUnderCursor<KSelectable>(true, (Func<KSelectable, bool>) (s => ((Component) s).GetComponent<KSelectable>().IsSelectable), (Component) this.selected);
    this.selectedCell = Grid.PosToCell(cursor_pos);
    this.Select(objectUnderCursor);
    if (DevToolSimDebug.Instance != null)
      DevToolSimDebug.Instance.SetCell(this.selectedCell);
    if (DevToolNavGrid.Instance == null)
      return;
    DevToolNavGrid.Instance.SetCell(this.selectedCell);
  }

  public int GetSelectedCell() => this.selectedCell;
}
