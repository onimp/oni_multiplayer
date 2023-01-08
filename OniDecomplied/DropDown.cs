// Decompiled with JetBrains decompiler
// Type: DropDown
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/DropDown")]
public class DropDown : KMonoBehaviour
{
  public GameObject targetDropDownContainer;
  public LocText selectedLabel;
  public KButton openButton;
  public Transform contentContainer;
  public GameObject scrollRect;
  public RectTransform dropdownAlignmentTarget;
  public GameObject rowEntryPrefab;
  public bool addEmptyRow = true;
  private static Vector2 edgePadding = new Vector2(8f, 8f);
  public object targetData;
  private List<IListableOption> entries = new List<IListableOption>();
  private Action<IListableOption, object> onEntrySelectedAction;
  private Action<DropDownEntry, object> rowRefreshAction;
  public Dictionary<IListableOption, GameObject> rowLookup = new Dictionary<IListableOption, GameObject>();
  private Func<IListableOption, IListableOption, object, int> sortFunction;
  private GameObject emptyRow;
  private string emptyRowLabel;
  private Sprite emptyRowSprite;
  private bool built;
  private bool displaySelectedValueWhenClosed = true;
  private const int ROWS_BEFORE_SCROLL = 8;
  private KCanvasScaler canvasScaler;

  public bool open { get; private set; }

  public List<IListableOption> Entries => this.entries;

  public void Initialize(
    IEnumerable<IListableOption> contentKeys,
    Action<IListableOption, object> onEntrySelectedAction,
    Func<IListableOption, IListableOption, object, int> sortFunction = null,
    Action<DropDownEntry, object> refreshAction = null,
    bool displaySelectedValueWhenClosed = true,
    object targetData = null)
  {
    this.targetData = targetData;
    this.sortFunction = sortFunction;
    this.onEntrySelectedAction = onEntrySelectedAction;
    this.displaySelectedValueWhenClosed = displaySelectedValueWhenClosed;
    this.rowRefreshAction = refreshAction;
    this.ChangeContent(contentKeys);
    this.openButton.ClearOnClick();
    this.openButton.onClick += (System.Action) (() => this.OnClick());
    this.canvasScaler = GameScreenManager.Instance.ssOverlayCanvas.GetComponent<KCanvasScaler>();
  }

  public void CustomizeEmptyRow(string txt, Sprite icon)
  {
    this.emptyRowLabel = txt;
    this.emptyRowSprite = icon;
  }

  public void OnClick()
  {
    if (!this.open)
      this.Open();
    else
      this.Close();
  }

  public void ChangeContent(IEnumerable<IListableOption> contentKeys)
  {
    this.entries.Clear();
    foreach (IListableOption contentKey in contentKeys)
      this.entries.Add(contentKey);
    this.built = false;
  }

  private void Update()
  {
    if (!this.open || !Input.GetMouseButtonDown(0) && (double) Input.GetAxis("Mouse ScrollWheel") == 0.0 && !KInputManager.steamInputInterpreter.GetSteamInputActionIsDown((Action) 3))
      return;
    float canvasScale = this.canvasScaler.GetCanvasScale();
    if ((double) TransformExtensions.GetPosition((Transform) Util.rectTransform(this.scrollRect)).x + (double) Util.rectTransform(this.scrollRect).sizeDelta.x * (double) canvasScale >= (double) KInputManager.GetMousePos().x && (double) TransformExtensions.GetPosition((Transform) Util.rectTransform(this.scrollRect)).x <= (double) KInputManager.GetMousePos().x && (double) TransformExtensions.GetPosition((Transform) Util.rectTransform(this.scrollRect)).y - (double) Util.rectTransform(this.scrollRect).sizeDelta.y * (double) canvasScale <= (double) KInputManager.GetMousePos().y && (double) TransformExtensions.GetPosition((Transform) Util.rectTransform(this.scrollRect)).y >= (double) KInputManager.GetMousePos().y)
      return;
    this.Close();
  }

  private void Build(List<IListableOption> contentKeys)
  {
    this.built = true;
    for (int index = this.contentContainer.childCount - 1; index >= 0; --index)
      Util.KDestroyGameObject((Component) this.contentContainer.GetChild(index));
    this.rowLookup.Clear();
    if (this.addEmptyRow)
    {
      this.emptyRow = Util.KInstantiateUI(this.rowEntryPrefab, ((Component) this.contentContainer).gameObject, true);
      this.emptyRow.GetComponent<KButton>().onClick += (System.Action) (() =>
      {
        this.onEntrySelectedAction((IListableOption) null, this.targetData);
        if (this.displaySelectedValueWhenClosed)
          ((TMP_Text) this.selectedLabel).text = this.emptyRowLabel ?? (string) STRINGS.UI.DROPDOWN.NONE;
        this.Close();
      });
      string str = this.emptyRowLabel ?? (string) STRINGS.UI.DROPDOWN.NONE;
      ((TMP_Text) this.emptyRow.GetComponent<DropDownEntry>().label).text = str;
      if (Object.op_Inequality((Object) this.emptyRowSprite, (Object) null))
        ((Image) this.emptyRow.GetComponent<DropDownEntry>().image).sprite = this.emptyRowSprite;
    }
    for (int index = 0; index < contentKeys.Count; ++index)
    {
      GameObject gameObject = Util.KInstantiateUI(this.rowEntryPrefab, ((Component) this.contentContainer).gameObject, true);
      IListableOption id = contentKeys[index];
      gameObject.GetComponent<DropDownEntry>().entryData = (object) id;
      gameObject.GetComponent<KButton>().onClick += (System.Action) (() =>
      {
        this.onEntrySelectedAction(id, this.targetData);
        if (this.displaySelectedValueWhenClosed)
          ((TMP_Text) this.selectedLabel).text = id.GetProperName();
        this.Close();
      });
      this.rowLookup.Add(id, gameObject);
    }
    this.RefreshEntries();
    this.Close();
    this.scrollRect.gameObject.transform.SetParent(this.targetDropDownContainer.transform);
    this.scrollRect.gameObject.SetActive(false);
  }

  private void RefreshEntries()
  {
    foreach (KeyValuePair<IListableOption, GameObject> keyValuePair in this.rowLookup)
    {
      DropDownEntry component = keyValuePair.Value.GetComponent<DropDownEntry>();
      ((TMP_Text) component.label).text = keyValuePair.Key.GetProperName();
      if (Object.op_Inequality((Object) component.portrait, (Object) null) && keyValuePair.Key is IAssignableIdentity)
        component.portrait.SetIdentityObject(keyValuePair.Key as IAssignableIdentity);
    }
    if (this.sortFunction != null)
    {
      this.entries.Sort((Comparison<IListableOption>) ((a, b) => this.sortFunction(a, b, this.targetData)));
      for (int index = 0; index < this.entries.Count; ++index)
        this.rowLookup[this.entries[index]].transform.SetAsFirstSibling();
      if (Object.op_Inequality((Object) this.emptyRow, (Object) null))
        this.emptyRow.transform.SetAsFirstSibling();
    }
    foreach (KeyValuePair<IListableOption, GameObject> keyValuePair in this.rowLookup)
      this.rowRefreshAction(keyValuePair.Value.GetComponent<DropDownEntry>(), this.targetData);
    if (!Object.op_Inequality((Object) this.emptyRow, (Object) null))
      return;
    this.rowRefreshAction(this.emptyRow.GetComponent<DropDownEntry>(), this.targetData);
  }

  protected virtual void OnCleanUp()
  {
    Util.KDestroyGameObject(this.scrollRect);
    base.OnCleanUp();
  }

  public void Open()
  {
    if (this.open)
      return;
    if (!this.built)
      this.Build(this.entries);
    else
      this.RefreshEntries();
    this.open = true;
    this.scrollRect.gameObject.SetActive(true);
    ((Transform) Util.rectTransform(this.scrollRect)).localScale = Vector3.one;
    foreach (KeyValuePair<IListableOption, GameObject> keyValuePair in this.rowLookup)
      keyValuePair.Value.SetActive(true);
    Util.rectTransform(this.scrollRect).sizeDelta = new Vector2(Util.rectTransform(this.scrollRect).sizeDelta.x, 32f * (float) Mathf.Min(this.contentContainer.childCount, 8));
    RectTransform dropdownAlignmentTarget = this.dropdownAlignmentTarget;
    Rect rect1 = this.dropdownAlignmentTarget.rect;
    double x = (double) ((Rect) ref rect1).x;
    Rect rect2 = this.dropdownAlignmentTarget.rect;
    double y = (double) ((Rect) ref rect2).y;
    Vector3 vector3_1 = ((Transform) dropdownAlignmentTarget).TransformPoint((float) x, (float) y, 0.0f);
    Vector2 vector2;
    // ISSUE: explicit constructor call
    ((Vector2) ref vector2).\u002Ector(Mathf.Min(0.0f, (float) Screen.width - (vector3_1.x + (this.rowEntryPrefab.GetComponent<LayoutElement>().minWidth * this.canvasScaler.GetCanvasScale() + DropDown.edgePadding.x))), -Mathf.Min(0.0f, vector3_1.y - (Util.rectTransform(this.scrollRect).sizeDelta.y * this.canvasScaler.GetCanvasScale() + DropDown.edgePadding.y)));
    Vector3 vector3_2 = Vector3.op_Addition(vector3_1, Vector2.op_Implicit(vector2));
    TransformExtensions.SetPosition((Transform) Util.rectTransform(this.scrollRect), vector3_2);
  }

  public void Close()
  {
    if (!this.open)
      return;
    this.open = false;
    foreach (KeyValuePair<IListableOption, GameObject> keyValuePair in this.rowLookup)
      keyValuePair.Value.SetActive(false);
    this.scrollRect.SetActive(false);
  }
}
