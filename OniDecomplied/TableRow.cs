// Decompiled with JetBrains decompiler
// Type: TableRow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/TableRow")]
public class TableRow : KMonoBehaviour
{
  public TableRow.RowType rowType;
  private IAssignableIdentity minion;
  private Dictionary<TableColumn, GameObject> widgets = new Dictionary<TableColumn, GameObject>();
  private Dictionary<string, GameObject> scrollers = new Dictionary<string, GameObject>();
  private Dictionary<string, GameObject> scrollerBorders = new Dictionary<string, GameObject>();
  public bool isDefault;
  public KButton selectMinionButton;
  [SerializeField]
  private ColorStyleSetting style_setting_default;
  [SerializeField]
  private ColorStyleSetting style_setting_minion;
  [SerializeField]
  private GameObject scrollerPrefab;
  [SerializeField]
  private GameObject scrollbarPrefab;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    if (!Object.op_Inequality((Object) this.selectMinionButton, (Object) null))
      return;
    this.selectMinionButton.onClick += new System.Action(this.SelectMinion);
    this.selectMinionButton.onDoubleClick += new System.Action(this.SelectAndFocusMinion);
  }

  public GameObject GetScroller(string scrollerID) => this.scrollers[scrollerID];

  public GameObject GetScrollerBorder(string scrolledID) => this.scrollerBorders[scrolledID];

  public void SelectMinion()
  {
    MinionIdentity minion = this.minion as MinionIdentity;
    if (Object.op_Equality((Object) minion, (Object) null))
      return;
    SelectTool.Instance.Select(((Component) minion).GetComponent<KSelectable>());
  }

  public void SelectAndFocusMinion()
  {
    MinionIdentity minion = this.minion as MinionIdentity;
    if (Object.op_Equality((Object) minion, (Object) null))
      return;
    SelectTool.Instance.SelectAndFocus(TransformExtensions.GetPosition(minion.transform), ((Component) minion).GetComponent<KSelectable>(), new Vector3(8f, 0.0f, 0.0f));
  }

  public void ConfigureAsWorldDivider(Dictionary<string, TableColumn> columns, TableScreen screen)
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    TableRow.\u003C\u003Ec__DisplayClass17_0 cDisplayClass170 = new TableRow.\u003C\u003Ec__DisplayClass17_0();
    // ISSUE: reference to a compiler-generated field
    cDisplayClass170.screen = screen;
    // ISSUE: reference to a compiler-generated field
    cDisplayClass170.scroll_rect = ((Component) this).gameObject.GetComponentInChildren<ScrollRect>();
    this.rowType = TableRow.RowType.WorldDivider;
    foreach (KeyValuePair<string, TableColumn> column in columns)
    {
      if (column.Value.scrollerID != "")
      {
        TableColumn tableColumn = column.Value;
        break;
      }
    }
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    ((UnityEvent<Vector2>) cDisplayClass170.scroll_rect.onValueChanged).AddListener(new UnityAction<Vector2>((object) cDisplayClass170, __methodptr(\u003CConfigureAsWorldDivider\u003Eb__0)));
  }

  public void ConfigureContent(
    IAssignableIdentity minion,
    Dictionary<string, TableColumn> columns,
    TableScreen screen)
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    TableRow.\u003C\u003Ec__DisplayClass18_0 cDisplayClass180 = new TableRow.\u003C\u003Ec__DisplayClass18_0();
    // ISSUE: reference to a compiler-generated field
    cDisplayClass180.screen = screen;
    this.minion = minion;
    KImage componentInChildren = ((Component) this).GetComponentInChildren<KImage>(true);
    componentInChildren.colorStyleSetting = minion == null ? this.style_setting_default : this.style_setting_minion;
    componentInChildren.ColorState = (KImage.ColorSelector) 1;
    CanvasGroup component = ((Component) this).GetComponent<CanvasGroup>();
    if (Object.op_Inequality((Object) component, (Object) null) && Object.op_Inequality((Object) (minion as StoredMinionIdentity), (Object) null))
      component.alpha = 0.6f;
    foreach (KeyValuePair<string, TableColumn> column in columns)
    {
      GameObject gameObject1 = minion != null ? column.Value.GetMinionWidget(((Component) this).gameObject) : (!this.isDefault ? column.Value.GetHeaderWidget(((Component) this).gameObject) : column.Value.GetDefaultWidget(((Component) this).gameObject));
      this.widgets.Add(column.Value, gameObject1);
      column.Value.widgets_by_row.Add(this, gameObject1);
      if (column.Value.scrollerID != "")
      {
        foreach (string columnScroller in column.Value.screen.column_scrollers)
        {
          if (!(columnScroller != column.Value.scrollerID))
          {
            if (!this.scrollers.ContainsKey(columnScroller))
            {
              // ISSUE: object of a compiler-generated type is created
              // ISSUE: variable of a compiler-generated type
              TableRow.\u003C\u003Ec__DisplayClass18_1 cDisplayClass181 = new TableRow.\u003C\u003Ec__DisplayClass18_1();
              // ISSUE: reference to a compiler-generated field
              cDisplayClass181.CS\u0024\u003C\u003E8__locals1 = cDisplayClass180;
              GameObject gameObject2 = Util.KInstantiateUI(this.scrollerPrefab, ((Component) this).gameObject, true);
              // ISSUE: reference to a compiler-generated field
              cDisplayClass181.scroll_rect = gameObject2.GetComponent<ScrollRect>();
              // ISSUE: reference to a compiler-generated field
              // ISSUE: method pointer
              ((UnityEvent<Vector2>) cDisplayClass181.scroll_rect.onValueChanged).AddListener(new UnityAction<Vector2>((object) cDisplayClass181, __methodptr(\u003CConfigureContent\u003Eb__0)));
              // ISSUE: reference to a compiler-generated field
              this.scrollers.Add(columnScroller, ((Component) cDisplayClass181.scroll_rect.content).gameObject);
              // ISSUE: reference to a compiler-generated field
              if (Object.op_Inequality((Object) ((Component) cDisplayClass181.scroll_rect.content).transform.parent.Find("Border"), (Object) null))
              {
                // ISSUE: reference to a compiler-generated field
                this.scrollerBorders.Add(columnScroller, ((Component) ((Component) cDisplayClass181.scroll_rect.content).transform.parent.Find("Border")).gameObject);
              }
            }
            gameObject1.transform.SetParent(this.scrollers[columnScroller].transform);
            ((Component) this.scrollers[columnScroller].transform.parent).GetComponent<ScrollRect>().horizontalNormalizedPosition = 0.0f;
          }
        }
      }
    }
    this.RefreshColumns(columns);
    if (minion != null)
      ((Object) ((Component) this).gameObject).name = minion.GetProperName();
    else if (this.isDefault)
      ((Object) ((Component) this).gameObject).name = "defaultRow";
    if (Object.op_Implicit((Object) this.selectMinionButton))
      ((KMonoBehaviour) this.selectMinionButton).transform.SetAsLastSibling();
    foreach (KeyValuePair<string, GameObject> scrollerBorder in this.scrollerBorders)
    {
      RectTransform rectTransform1 = Util.rectTransform(scrollerBorder.Value);
      Rect rect = rectTransform1.rect;
      float width = ((Rect) ref rect).width;
      scrollerBorder.Value.transform.SetParent(((Component) this).gameObject.transform);
      rectTransform1.anchorMin = rectTransform1.anchorMax = new Vector2(0.0f, 1f);
      rectTransform1.sizeDelta = new Vector2(width, rectTransform1.sizeDelta.y);
      RectTransform rectTransform2 = Util.rectTransform((Component) this.scrollers[scrollerBorder.Key].transform.parent);
      Vector3 vector3 = Vector3.op_Subtraction(TransformExtensions.GetLocalPosition((Transform) Util.rectTransform((Component) this.scrollers[scrollerBorder.Key].transform.parent)), new Vector3(rectTransform2.sizeDelta.x / 2f, (float) (-1.0 * ((double) rectTransform2.sizeDelta.y / 2.0)), 0.0f));
      vector3.y = 0.0f;
      rectTransform1.sizeDelta = new Vector2(rectTransform1.sizeDelta.x, 374f);
      TransformExtensions.SetLocalPosition((Transform) rectTransform1, Vector3.op_Addition(Vector3.op_Addition(vector3, Vector3.op_Multiply(Vector3.up, TransformExtensions.GetLocalPosition((Transform) rectTransform1).y)), Vector3.op_Multiply(Vector3.up, -rectTransform1.anchoredPosition.y)));
    }
  }

  public void RefreshColumns(Dictionary<string, TableColumn> columns)
  {
    foreach (KeyValuePair<string, TableColumn> column in columns)
    {
      if (column.Value.on_load_action != null)
        column.Value.on_load_action(this.minion, column.Value.widgets_by_row[this]);
    }
  }

  public void RefreshScrollers()
  {
    foreach (KeyValuePair<string, GameObject> scroller in this.scrollers)
    {
      ScrollRect component = ((Component) scroller.Value.transform.parent).GetComponent<ScrollRect>();
      ((Component) component).GetComponent<LayoutElement>().minWidth = Mathf.Min(768f, component.content.sizeDelta.x);
    }
    foreach (KeyValuePair<string, GameObject> scrollerBorder in this.scrollerBorders)
    {
      RectTransform rectTransform = Util.rectTransform(scrollerBorder.Value);
      rectTransform.sizeDelta = new Vector2(((Component) this.scrollers[scrollerBorder.Key].transform.parent).GetComponent<LayoutElement>().minWidth, rectTransform.sizeDelta.y);
    }
  }

  public GameObject GetWidget(TableColumn column)
  {
    if (this.widgets.ContainsKey(column) && Object.op_Inequality((Object) this.widgets[column], (Object) null))
      return this.widgets[column];
    Debug.LogWarning((object) ("Widget is null or row does not contain widget for column " + column?.ToString()));
    return (GameObject) null;
  }

  public IAssignableIdentity GetIdentity() => this.minion;

  public bool ContainsWidget(GameObject widget) => this.widgets.ContainsValue(widget);

  public void Clear()
  {
    foreach (KeyValuePair<TableColumn, GameObject> widget in this.widgets)
      widget.Key.widgets_by_row.Remove(this);
    Object.Destroy((Object) ((Component) this).gameObject);
  }

  public enum RowType
  {
    Header,
    Default,
    Minion,
    StoredMinon,
    WorldDivider,
  }
}
