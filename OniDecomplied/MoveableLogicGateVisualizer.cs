// Decompiled with JetBrains decompiler
// Type: MoveableLogicGateVisualizer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[SkipSaveFileSerialization]
public class MoveableLogicGateVisualizer : LogicGateBase
{
  private int cell;
  protected List<GameObject> visChildren = new List<GameObject>();
  private static readonly EventSystem.IntraObjectHandler<MoveableLogicGateVisualizer> OnRotatedDelegate = new EventSystem.IntraObjectHandler<MoveableLogicGateVisualizer>((Action<MoveableLogicGateVisualizer, object>) ((component, data) => component.OnRotated(data)));

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.cell = -1;
    OverlayScreen.Instance.OnOverlayChanged += new Action<HashedString>(this.OnOverlayChanged);
    this.OnOverlayChanged(OverlayScreen.Instance.mode);
    this.Subscribe<MoveableLogicGateVisualizer>(-1643076535, MoveableLogicGateVisualizer.OnRotatedDelegate);
  }

  protected virtual void OnCleanUp()
  {
    OverlayScreen.Instance.OnOverlayChanged -= new Action<HashedString>(this.OnOverlayChanged);
    this.Unregister();
    base.OnCleanUp();
  }

  private void OnOverlayChanged(HashedString mode)
  {
    if (HashedString.op_Equality(mode, OverlayModes.Logic.ID))
      this.Register();
    else
      this.Unregister();
  }

  private void OnRotated(object data)
  {
    this.Unregister();
    this.OnOverlayChanged(OverlayScreen.Instance.mode);
  }

  private void Update()
  {
    if (this.visChildren.Count <= 0)
      return;
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
    if (cell == this.cell)
      return;
    this.cell = cell;
    this.Unregister();
    this.Register();
  }

  private GameObject CreateUIElem(int cell, bool is_input)
  {
    GameObject uiElem = Util.KInstantiate(LogicGateBase.uiSrcData.prefab, Grid.CellToPosCCC(cell, Grid.SceneLayer.Front), Quaternion.identity, GameScreenManager.Instance.worldSpaceCanvas, (string) null, true, 0);
    Image component = uiElem.GetComponent<Image>();
    component.sprite = is_input ? LogicGateBase.uiSrcData.inputSprite : LogicGateBase.uiSrcData.outputSprite;
    ((Graphic) component).raycastTarget = false;
    return uiElem;
  }

  private void Register()
  {
    if (this.visChildren.Count > 0)
      return;
    ((Behaviour) this).enabled = true;
    this.visChildren.Add(this.CreateUIElem(this.OutputCellOne, false));
    if (this.RequiresFourOutputs)
    {
      this.visChildren.Add(this.CreateUIElem(this.OutputCellTwo, false));
      this.visChildren.Add(this.CreateUIElem(this.OutputCellThree, false));
      this.visChildren.Add(this.CreateUIElem(this.OutputCellFour, false));
    }
    this.visChildren.Add(this.CreateUIElem(this.InputCellOne, true));
    if (this.RequiresTwoInputs)
      this.visChildren.Add(this.CreateUIElem(this.InputCellTwo, true));
    else if (this.RequiresFourInputs)
    {
      this.visChildren.Add(this.CreateUIElem(this.InputCellTwo, true));
      this.visChildren.Add(this.CreateUIElem(this.InputCellThree, true));
      this.visChildren.Add(this.CreateUIElem(this.InputCellFour, true));
    }
    if (!this.RequiresControlInputs)
      return;
    this.visChildren.Add(this.CreateUIElem(this.ControlCellOne, true));
    this.visChildren.Add(this.CreateUIElem(this.ControlCellTwo, true));
  }

  private void Unregister()
  {
    if (this.visChildren.Count <= 0)
      return;
    ((Behaviour) this).enabled = false;
    this.cell = -1;
    foreach (GameObject visChild in this.visChildren)
      Util.KDestroyGameObject(visChild);
    this.visChildren.Clear();
  }
}
