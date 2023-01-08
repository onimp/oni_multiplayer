// Decompiled with JetBrains decompiler
// Type: GroupSelectorHeaderWidget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupSelectorHeaderWidget : MonoBehaviour
{
  public LocText label;
  [SerializeField]
  private GameObject itemTemplate;
  [SerializeField]
  private RectTransform itemsPanel;
  [SerializeField]
  private KButton addItemButton;
  [SerializeField]
  private KButton removeItemButton;
  [SerializeField]
  private KButton sortButton;
  [SerializeField]
  private int numExpectedPanelColumns = 3;
  private object widgetID;
  private GroupSelectorHeaderWidget.ItemCallbacks itemCallbacks;
  private IList<GroupSelectorWidget.ItemData> options;

  public void Initialize(
    object widget_id,
    IList<GroupSelectorWidget.ItemData> options,
    GroupSelectorHeaderWidget.ItemCallbacks item_callbacks)
  {
    this.widgetID = widget_id;
    this.options = options;
    this.itemCallbacks = item_callbacks;
    if (this.itemCallbacks.getTitleHoverText != null)
      ((Component) this.label).GetComponent<ToolTip>().OnToolTip = (Func<string>) (() => this.itemCallbacks.getTitleHoverText(widget_id));
    bool adding_item1 = true;
    this.addItemButton.onClick += (System.Action) (() => this.RebuildSubPanel(TransformExtensions.GetPosition(((KMonoBehaviour) this.addItemButton).transform), (Func<object, IList<int>>) (widget_go => this.itemCallbacks.getHeaderButtonOptions(widget_go, adding_item1)), this.itemCallbacks.onItemAdded, (Func<object, object, string>) ((widget_go, item_data) => this.itemCallbacks.getItemHoverText(widget_go, adding_item1, item_data))));
    bool adding_item2 = false;
    this.removeItemButton.onClick += (System.Action) (() => this.RebuildSubPanel(TransformExtensions.GetPosition(((KMonoBehaviour) this.removeItemButton).transform), (Func<object, IList<int>>) (widget_go => this.itemCallbacks.getHeaderButtonOptions(widget_go, adding_item2)), this.itemCallbacks.onItemRemoved, (Func<object, object, string>) ((widget_go, item_data) => this.itemCallbacks.getItemHoverText(widget_go, adding_item2, item_data))));
    this.sortButton.onClick += (System.Action) (() => this.RebuildSubPanel(TransformExtensions.GetPosition(((KMonoBehaviour) this.sortButton).transform), this.itemCallbacks.getValidSortOptionIndices, (Action<object>) (item_data => this.itemCallbacks.onSort(this.widgetID, item_data)), (Func<object, object, string>) ((widget_go, item_data) => this.itemCallbacks.getSortHoverText(item_data))));
    if (this.itemCallbacks.getTitleButtonHoverText == null)
      return;
    ((Component) this.addItemButton).GetComponent<ToolTip>().OnToolTip = (Func<string>) (() => this.itemCallbacks.getTitleButtonHoverText(widget_id, true));
    ((Component) this.removeItemButton).GetComponent<ToolTip>().OnToolTip = (Func<string>) (() => this.itemCallbacks.getTitleButtonHoverText(widget_id, false));
  }

  private void RebuildSubPanel(
    Vector3 pos,
    Func<object, IList<int>> display_list_query,
    Action<object> on_item_selected,
    Func<object, object, string> get_item_hover_text)
  {
    TransformExtensions.SetPosition(((Component) this.itemsPanel).gameObject.transform, Vector3.op_Addition(pos, new Vector3(2f, 2f, 0.0f)));
    IList<int> intList = display_list_query(this.widgetID);
    if (intList.Count > 0)
    {
      this.ClearSubPanelOptions();
      foreach (int num in (IEnumerable<int>) intList)
      {
        int idx = num;
        GroupSelectorWidget.ItemData option = this.options[idx];
        GameObject gameObject = Util.KInstantiateUI(this.itemTemplate, ((Component) this.itemsPanel).gameObject, true);
        KButton component = gameObject.GetComponent<KButton>();
        component.fgImage.sprite = this.options[idx].sprite;
        component.onClick += (System.Action) (() =>
        {
          on_item_selected(this.options[idx].userData);
          this.RebuildSubPanel(pos, display_list_query, on_item_selected, get_item_hover_text);
        });
        if (get_item_hover_text != null)
          gameObject.GetComponent<ToolTip>().OnToolTip = (Func<string>) (() => get_item_hover_text(this.widgetID, this.options[idx].userData));
      }
      ((Component) this.itemsPanel).GetComponent<GridLayoutGroup>().constraintCount = Mathf.Min(this.numExpectedPanelColumns, ((Transform) this.itemsPanel).childCount);
      ((Component) this.itemsPanel).gameObject.SetActive(true);
      ((Component) this.itemsPanel).GetComponent<Selectable>().Select();
    }
    else
      this.CloseSubPanel();
  }

  public void CloseSubPanel()
  {
    this.ClearSubPanelOptions();
    ((Component) this.itemsPanel).gameObject.SetActive(false);
  }

  private void ClearSubPanelOptions()
  {
    foreach (Component component in ((Component) this.itemsPanel).transform)
      Util.KDestroyGameObject(component.gameObject);
  }

  public struct ItemCallbacks
  {
    public Func<object, string> getTitleHoverText;
    public Func<object, bool, string> getTitleButtonHoverText;
    public Func<object, bool, IList<int>> getHeaderButtonOptions;
    public Action<object> onItemAdded;
    public Action<object> onItemRemoved;
    public Func<object, bool, object, string> getItemHoverText;
    public Func<object, IList<int>> getValidSortOptionIndices;
    public Func<object, string> getSortHoverText;
    public Action<object, object> onSort;
  }
}
