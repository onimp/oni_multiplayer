// Decompiled with JetBrains decompiler
// Type: ResearchScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResearchScreen : KModalScreen
{
  private const float SCROLL_BUFFER = 250f;
  [SerializeField]
  private Image BG;
  public ResearchEntry entryPrefab;
  public ResearchTreeTitle researchTreeTitlePrefab;
  public GameObject foreground;
  public GameObject scrollContent;
  public GameObject treeTitles;
  public GameObject pointDisplayCountPrefab;
  public GameObject pointDisplayContainer;
  private Dictionary<string, LocText> pointDisplayMap;
  private Dictionary<Tech, ResearchEntry> entryMap;
  [SerializeField]
  private KButton zoomOutButton;
  [SerializeField]
  private KButton zoomInButton;
  [SerializeField]
  private ResearchScreenSideBar sideBar;
  private Tech currentResearch;
  public KButton CloseButton;
  private GraphicRaycaster m_Raycaster;
  private PointerEventData m_PointerEventData;
  private Vector3 currentScrollPosition;
  private bool panUp;
  private bool panDown;
  private bool panLeft;
  private bool panRight;
  [SerializeField]
  private KChildFitter scrollContentChildFitter;
  private bool isDragging;
  private Vector3 dragStartPosition;
  private Vector3 dragLastPosition;
  private Vector2 dragInteria;
  private Vector2 forceTargetPosition;
  private bool zoomingToTarget;
  private bool draggingJustEnded;
  private float targetZoom = 1f;
  private float currentZoom = 1f;
  private bool zoomCenterLock;
  private Vector2 keyPanDelta = Vector2.op_Implicit(Vector3.zero);
  [SerializeField]
  private float effectiveZoomSpeed = 5f;
  [SerializeField]
  private float zoomAmountPerScroll = 0.05f;
  [SerializeField]
  private float zoomAmountPerButton = 0.5f;
  [SerializeField]
  private float minZoom = 0.15f;
  [SerializeField]
  private float maxZoom = 1f;
  [SerializeField]
  private float keyboardScrollSpeed = 200f;
  [SerializeField]
  private float keyPanEasing = 1f;
  [SerializeField]
  private float edgeClampFactor = 0.5f;

  public bool IsBeingResearched(Tech tech) => Research.Instance.IsBeingResearched(tech);

  public override float GetSortKey() => this.isEditing ? 50f : 20f;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.ConsumeMouseScroll = true;
    Transform transform = ((KMonoBehaviour) this).transform;
    while (Object.op_Equality((Object) this.m_Raycaster, (Object) null))
    {
      this.m_Raycaster = ((Component) transform).GetComponent<GraphicRaycaster>();
      if (Object.op_Equality((Object) this.m_Raycaster, (Object) null))
        transform = transform.parent;
    }
  }

  private void ZoomOut()
  {
    this.targetZoom = Mathf.Clamp(this.targetZoom - this.zoomAmountPerButton, this.minZoom, this.maxZoom);
    this.zoomCenterLock = true;
  }

  private void ZoomIn()
  {
    this.targetZoom = Mathf.Clamp(this.targetZoom + this.zoomAmountPerButton, this.minZoom, this.maxZoom);
    this.zoomCenterLock = true;
  }

  public void ZoomToTech(string techID)
  {
    Vector2 vector2_1 = Vector2.op_Implicit(TransformExtensions.GetLocalPosition((Transform) Util.rectTransform((Component) this.entryMap[Db.Get().Techs.Get(techID)])));
    Rect rect = Util.rectTransform(this.foreground).rect;
    double num1 = -(double) ((Rect) ref rect).size.x / 2.0;
    rect = Util.rectTransform(this.foreground).rect;
    double num2 = (double) ((Rect) ref rect).size.y / 2.0;
    Vector2 vector2_2 = new Vector2((float) num1, (float) num2);
    this.forceTargetPosition = Vector2.op_UnaryNegation(Vector2.op_Addition(vector2_1, vector2_2));
    this.zoomingToTarget = true;
    this.targetZoom = this.maxZoom;
  }

  private void Update()
  {
    if (!((Behaviour) this.canvas).enabled)
      return;
    RectTransform component = this.scrollContent.GetComponent<RectTransform>();
    if (this.isDragging && !KInputManager.isFocused)
      this.AbortDragging();
    Vector2 anchoredPosition = component.anchoredPosition;
    this.currentZoom = Mathf.Lerp(this.currentZoom, this.targetZoom, Mathf.Min(this.effectiveZoomSpeed * Time.unscaledDeltaTime, 0.9f));
    Vector2 zero = Vector2.zero;
    Vector2 vector2_1 = Vector2.op_Implicit(KInputManager.GetMousePos());
    Vector2 vector2_2 = Vector2.op_Implicit(this.zoomCenterLock ? Vector3.op_Multiply(((Transform) component).InverseTransformPoint(Vector2.op_Implicit(new Vector2((float) (Screen.width / 2), (float) (Screen.height / 2)))), this.currentZoom) : Vector3.op_Multiply(((Transform) component).InverseTransformPoint(Vector2.op_Implicit(vector2_1)), this.currentZoom));
    ((Transform) component).localScale = new Vector3(this.currentZoom, this.currentZoom, 1f);
    Vector2 vector2_3 = Vector2.op_Subtraction(Vector2.op_Implicit(this.zoomCenterLock ? Vector3.op_Multiply(((Transform) component).InverseTransformPoint(Vector2.op_Implicit(new Vector2((float) (Screen.width / 2), (float) (Screen.height / 2)))), this.currentZoom) : Vector3.op_Multiply(((Transform) component).InverseTransformPoint(Vector2.op_Implicit(vector2_1)), this.currentZoom)), vector2_2);
    float keyboardScrollSpeed = this.keyboardScrollSpeed;
    if (this.panUp)
      this.keyPanDelta = Vector2.op_Subtraction(this.keyPanDelta, Vector2.op_Multiply(Vector2.op_Multiply(Vector2.up, Time.unscaledDeltaTime), keyboardScrollSpeed));
    else if (this.panDown)
      this.keyPanDelta = Vector2.op_Addition(this.keyPanDelta, Vector2.op_Multiply(Vector2.op_Multiply(Vector2.up, Time.unscaledDeltaTime), keyboardScrollSpeed));
    if (this.panLeft)
      this.keyPanDelta = Vector2.op_Addition(this.keyPanDelta, Vector2.op_Multiply(Vector2.op_Multiply(Vector2.right, Time.unscaledDeltaTime), keyboardScrollSpeed));
    else if (this.panRight)
      this.keyPanDelta = Vector2.op_Subtraction(this.keyPanDelta, Vector2.op_Multiply(Vector2.op_Multiply(Vector2.right, Time.unscaledDeltaTime), keyboardScrollSpeed));
    if (KInputManager.currentControllerIsGamepad)
      this.keyPanDelta = Vector2.op_Multiply(Vector2.op_Multiply(Vector2.op_Multiply(Vector2.op_Multiply(KInputManager.steamInputInterpreter.GetSteamCameraMovement(), -1f), Time.unscaledDeltaTime), keyboardScrollSpeed), 2f);
    Vector2 vector2_4;
    // ISSUE: explicit constructor call
    ((Vector2) ref vector2_4).\u002Ector(Mathf.Lerp(0.0f, this.keyPanDelta.x, Time.unscaledDeltaTime * this.keyPanEasing), Mathf.Lerp(0.0f, this.keyPanDelta.y, Time.unscaledDeltaTime * this.keyPanEasing));
    this.keyPanDelta = Vector2.op_Subtraction(this.keyPanDelta, vector2_4);
    Vector2 vector2_5 = Vector2.zero;
    if (this.isDragging)
    {
      Vector2 vector2_6 = Vector2.op_Implicit(Vector3.op_Subtraction(KInputManager.GetMousePos(), this.dragLastPosition));
      vector2_5 = Vector2.op_Addition(vector2_5, vector2_6);
      this.dragLastPosition = KInputManager.GetMousePos();
      this.dragInteria = Vector2.ClampMagnitude(Vector2.op_Addition(this.dragInteria, vector2_6), 400f);
    }
    this.dragInteria = Vector2.op_Multiply(this.dragInteria, Mathf.Max(0.0f, (float) (1.0 - (double) Time.unscaledDeltaTime * 4.0)));
    Vector2 vector2_7 = vector2_3;
    Vector2 vector2_8 = Vector2.op_Addition(Vector2.op_Addition(Vector2.op_Addition(anchoredPosition, vector2_7), this.keyPanDelta), vector2_5);
    if (!this.isDragging)
    {
      Rect rect = ((Component) this).GetComponent<RectTransform>().rect;
      Vector2 size = ((Rect) ref rect).size;
      Vector2 vector2_9;
      ref Vector2 local1 = ref vector2_9;
      rect = component.rect;
      double num1 = (-(double) ((Rect) ref rect).size.x / 2.0 - 250.0) * (double) this.currentZoom;
      double num2 = -250.0 * (double) this.currentZoom;
      // ISSUE: explicit constructor call
      ((Vector2) ref local1).\u002Ector((float) num1, (float) num2);
      Vector2 vector2_10;
      ref Vector2 local2 = ref vector2_10;
      double num3 = 250.0 * (double) this.currentZoom;
      rect = component.rect;
      double num4 = ((double) ((Rect) ref rect).size.y + 250.0) * (double) this.currentZoom - (double) size.y;
      // ISSUE: explicit constructor call
      ((Vector2) ref local2).\u002Ector((float) num3, (float) num4);
      Vector2 vector2_11 = new Vector2(Mathf.Clamp(vector2_8.x, vector2_9.x, vector2_10.x), Mathf.Clamp(vector2_8.y, vector2_9.y, vector2_10.y));
      this.forceTargetPosition = new Vector2(Mathf.Clamp(this.forceTargetPosition.x, vector2_9.x, vector2_10.x), Mathf.Clamp(this.forceTargetPosition.y, vector2_9.y, vector2_10.y));
      Vector2 dragInteria = this.dragInteria;
      Vector2 vector2_12 = Vector2.op_Subtraction(Vector2.op_Addition(vector2_11, dragInteria), vector2_8);
      if (!this.panLeft && !this.panRight && !this.panUp && !this.panDown)
      {
        vector2_8 = Vector2.op_Addition(vector2_8, Vector2.op_Multiply(Vector2.op_Multiply(vector2_12, this.edgeClampFactor), Time.unscaledDeltaTime));
      }
      else
      {
        vector2_8 = Vector2.op_Addition(vector2_8, vector2_12);
        if ((double) vector2_12.x < 0.0)
          this.keyPanDelta.x = Mathf.Min(0.0f, this.keyPanDelta.x);
        if ((double) vector2_12.x > 0.0)
          this.keyPanDelta.x = Mathf.Max(0.0f, this.keyPanDelta.x);
        if ((double) vector2_12.y < 0.0)
          this.keyPanDelta.y = Mathf.Min(0.0f, this.keyPanDelta.y);
        if ((double) vector2_12.y > 0.0)
          this.keyPanDelta.y = Mathf.Max(0.0f, this.keyPanDelta.y);
      }
    }
    if (this.zoomingToTarget)
    {
      vector2_8 = Vector2.Lerp(vector2_8, this.forceTargetPosition, Time.unscaledDeltaTime * 4f);
      if ((double) Vector3.Distance(Vector2.op_Implicit(vector2_8), Vector2.op_Implicit(this.forceTargetPosition)) < 1.0 || this.isDragging || this.panLeft || this.panRight || this.panUp || this.panDown)
        this.zoomingToTarget = false;
    }
    component.anchoredPosition = vector2_8;
  }

  protected virtual void OnSpawn()
  {
    ((KMonoBehaviour) this).Subscribe(((Component) Research.Instance).gameObject, -1914338957, new Action<object>(this.OnActiveResearchChanged));
    ((KMonoBehaviour) this).Subscribe(((Component) Game.Instance).gameObject, -107300940, new Action<object>(this.OnResearchComplete));
    ((KMonoBehaviour) this).Subscribe(((Component) Game.Instance).gameObject, -1974454597, (Action<object>) (o => base.Show(false)));
    this.pointDisplayMap = new Dictionary<string, LocText>();
    foreach (ResearchType type in Research.Instance.researchTypes.Types)
    {
      this.pointDisplayMap[type.id] = Util.KInstantiateUI(this.pointDisplayCountPrefab, this.pointDisplayContainer, true).GetComponentInChildren<LocText>();
      ((TMP_Text) this.pointDisplayMap[type.id]).text = Research.Instance.globalPointInventory.PointsByTypeID[type.id].ToString();
      ((Component) ((TMP_Text) this.pointDisplayMap[type.id]).transform.parent).GetComponent<ToolTip>().SetSimpleTooltip(type.description);
      ((Component) ((TMP_Text) this.pointDisplayMap[type.id]).transform.parent).GetComponentInChildren<Image>().sprite = type.sprite;
    }
    ((Component) this.pointDisplayContainer.transform.parent).gameObject.SetActive(Research.Instance.UseGlobalPointInventory);
    this.entryMap = new Dictionary<Tech, ResearchEntry>();
    List<Tech> resources1 = Db.Get().Techs.resources;
    resources1.Sort((Comparison<Tech>) ((x, y) => y.center.y.CompareTo(x.center.y)));
    List<TechTreeTitle> resources2 = Db.Get().TechTreeTitles.resources;
    resources2.Sort((Comparison<TechTreeTitle>) ((x, y) => y.center.y.CompareTo(x.center.y)));
    float num1 = 0.0f;
    float num2 = 125f;
    Vector2 vector2_1;
    // ISSUE: explicit constructor call
    ((Vector2) ref vector2_1).\u002Ector(num1, num2);
    for (int index = 0; index < resources2.Count; ++index)
    {
      ResearchTreeTitle researchTreeTitle = Util.KInstantiateUI<ResearchTreeTitle>(((Component) this.researchTreeTitlePrefab).gameObject, this.treeTitles, false);
      TechTreeTitle techTreeTitle1 = resources2[index];
      ((Object) researchTreeTitle).name = techTreeTitle1.Name + " Title";
      Vector3 vector3_1 = Vector2.op_Implicit(Vector2.op_Addition(techTreeTitle1.center, vector2_1));
      Util.rectTransform((Component) ((Component) researchTreeTitle).transform).anchoredPosition = Vector2.op_Implicit(vector3_1);
      float height = techTreeTitle1.height;
      float num3;
      if (index + 1 < resources2.Count)
      {
        TechTreeTitle techTreeTitle2 = resources2[index + 1];
        Vector3 vector3_2 = Vector2.op_Implicit(Vector2.op_Addition(techTreeTitle2.center, vector2_1));
        num3 = height + (vector3_1.y - (vector3_2.y + techTreeTitle2.height));
      }
      else
        num3 = height + 600f;
      Util.rectTransform((Component) ((Component) researchTreeTitle).transform).sizeDelta = new Vector2(techTreeTitle1.width, num3);
      researchTreeTitle.SetLabel(techTreeTitle1.Name);
      researchTreeTitle.SetColor(index);
    }
    List<Vector2> vector2List1 = new List<Vector2>();
    float num4 = 0.0f;
    float num5 = 0.0f;
    Vector2 vector2_2;
    // ISSUE: explicit constructor call
    ((Vector2) ref vector2_2).\u002Ector(num4, num5);
    for (int index1 = 0; index1 < resources1.Count; ++index1)
    {
      ResearchEntry researchEntry = Util.KInstantiateUI<ResearchEntry>(((Component) this.entryPrefab).gameObject, this.scrollContent, false);
      Tech key = resources1[index1];
      ((Object) researchEntry).name = key.Name + " Panel";
      Vector3 vector3 = Vector2.op_Implicit(Vector2.op_Addition(key.center, vector2_2));
      Util.rectTransform((Component) researchEntry.transform).anchoredPosition = Vector2.op_Implicit(vector3);
      Util.rectTransform((Component) researchEntry.transform).sizeDelta = new Vector2(key.width, key.height);
      this.entryMap.Add(key, researchEntry);
      if (key.edges.Count > 0)
      {
        for (int index2 = 0; index2 < key.edges.Count; ++index2)
        {
          ResourceTreeNode.Edge edge = key.edges[index2];
          if (edge.path == null)
          {
            vector2List1.AddRange((IEnumerable<Vector2>) edge.SrcTarget);
          }
          else
          {
            ResourceTreeNode.Edge.EdgeType edgeType = edge.edgeType;
            if (edgeType <= 1 || edgeType - 4 <= 1)
            {
              vector2List1.Add(edge.SrcTarget[0]);
              vector2List1.Add(edge.path[0]);
              for (int index3 = 1; index3 < edge.path.Count; ++index3)
              {
                vector2List1.Add(edge.path[index3 - 1]);
                vector2List1.Add(edge.path[index3]);
              }
              vector2List1.Add(edge.path[edge.path.Count - 1]);
              vector2List1.Add(edge.SrcTarget[1]);
            }
            else
              vector2List1.AddRange((IEnumerable<Vector2>) edge.path);
          }
        }
      }
    }
    for (int index4 = 0; index4 < vector2List1.Count; ++index4)
    {
      List<Vector2> vector2List2 = vector2List1;
      int index5 = index4;
      double x = (double) vector2List1[index4].x;
      double y = (double) vector2List1[index4].y;
      Rect rect = Util.rectTransform((Component) this.foreground.transform).rect;
      double height = (double) ((Rect) ref rect).height;
      double num6 = y + height;
      Vector2 vector2_3 = new Vector2((float) x, (float) num6);
      vector2List2[index5] = vector2_3;
    }
    foreach (KeyValuePair<Tech, ResearchEntry> entry in this.entryMap)
      entry.Value.SetTech(entry.Key);
    ((WidgetSoundPlayer) this.CloseButton.soundPlayer).Enabled = false;
    this.CloseButton.onClick += (System.Action) (() => ManagementMenu.Instance.CloseAll());
    ((MonoBehaviour) this).StartCoroutine(this.WaitAndSetActiveResearch());
    base.OnSpawn();
    this.scrollContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(250f, -250f);
    this.zoomOutButton.onClick += (System.Action) (() => this.ZoomOut());
    this.zoomInButton.onClick += (System.Action) (() => this.ZoomIn());
    ((Component) this).gameObject.SetActive(true);
    base.Show(false);
  }

  public virtual void OnBeginDrag(PointerEventData eventData)
  {
    base.OnBeginDrag(eventData);
    this.isDragging = true;
  }

  public virtual void OnEndDrag(PointerEventData eventData)
  {
    base.OnEndDrag(eventData);
    this.AbortDragging();
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    ((KMonoBehaviour) this).Unsubscribe(((Component) Game.Instance).gameObject, -1974454597, (Action<object>) (o => this.Deactivate()));
  }

  private IEnumerator WaitAndSetActiveResearch()
  {
    yield return (object) SequenceUtil.WaitForEndOfFrame;
    TechInstance targetResearch = Research.Instance.GetTargetResearch();
    if (targetResearch != null)
      this.SetActiveResearch(targetResearch.tech);
  }

  public Vector3 GetEntryPosition(Tech tech)
  {
    if (this.entryMap.ContainsKey(tech))
      return TransformExtensions.GetPosition(this.entryMap[tech].transform);
    Debug.LogError((object) "The Tech provided was not present in the dictionary");
    return Vector3.zero;
  }

  public ResearchEntry GetEntry(Tech tech)
  {
    if (this.entryMap == null)
      return (ResearchEntry) null;
    if (this.entryMap.ContainsKey(tech))
      return this.entryMap[tech];
    Debug.LogError((object) "The Tech provided was not present in the dictionary");
    return (ResearchEntry) null;
  }

  public void SetEntryPercentage(Tech tech, float percent)
  {
    ResearchEntry entry = this.GetEntry(tech);
    if (!Object.op_Inequality((Object) entry, (Object) null))
      return;
    entry.SetPercentage(percent);
  }

  public void TurnEverythingOff()
  {
    foreach (KeyValuePair<Tech, ResearchEntry> entry in this.entryMap)
      entry.Value.SetEverythingOff();
  }

  public void TurnEverythingOn()
  {
    foreach (KeyValuePair<Tech, ResearchEntry> entry in this.entryMap)
      entry.Value.SetEverythingOn();
  }

  private void SelectAllEntries(Tech tech, bool isSelected)
  {
    ResearchEntry entry = this.GetEntry(tech);
    if (Object.op_Inequality((Object) entry, (Object) null))
      entry.QueueStateChanged(isSelected);
    foreach (Tech tech1 in tech.requiredTech)
      this.SelectAllEntries(tech1, isSelected);
  }

  private void OnResearchComplete(object data)
  {
    ResearchEntry entry = this.GetEntry((Tech) data);
    if (Object.op_Inequality((Object) entry, (Object) null))
      entry.ResearchCompleted();
    this.UpdateProgressBars();
    this.UpdatePointDisplay();
  }

  private void UpdatePointDisplay()
  {
    foreach (ResearchType type in Research.Instance.researchTypes.Types)
      ((TMP_Text) this.pointDisplayMap[type.id]).text = string.Format("{0}: {1}", (object) Research.Instance.researchTypes.GetResearchType(type.id).name, (object) Research.Instance.globalPointInventory.PointsByTypeID[type.id].ToString());
  }

  private void OnActiveResearchChanged(object data)
  {
    List<TechInstance> techInstanceList = (List<TechInstance>) data;
    foreach (TechInstance techInstance in techInstanceList)
    {
      ResearchEntry entry = this.GetEntry(techInstance.tech);
      if (Object.op_Inequality((Object) entry, (Object) null))
        entry.QueueStateChanged(true);
    }
    this.UpdateProgressBars();
    this.UpdatePointDisplay();
    if (techInstanceList.Count <= 0)
      return;
    this.currentResearch = techInstanceList[techInstanceList.Count - 1].tech;
  }

  private void UpdateProgressBars()
  {
    foreach (KeyValuePair<Tech, ResearchEntry> entry in this.entryMap)
      entry.Value.UpdateProgressBars();
  }

  public void CancelResearch()
  {
    List<TechInstance> researchQueue = Research.Instance.GetResearchQueue();
    foreach (TechInstance techInstance in researchQueue)
    {
      ResearchEntry entry = this.GetEntry(techInstance.tech);
      if (Object.op_Inequality((Object) entry, (Object) null))
        entry.QueueStateChanged(false);
    }
    researchQueue.Clear();
  }

  private void SetActiveResearch(Tech newResearch)
  {
    if (newResearch != this.currentResearch && this.currentResearch != null)
      this.SelectAllEntries(this.currentResearch, false);
    this.currentResearch = newResearch;
    if (this.currentResearch == null)
      return;
    this.SelectAllEntries(this.currentResearch, true);
  }

  public virtual void Show(bool show = true)
  {
    this.mouseOver = false;
    ((Behaviour) this.scrollContentChildFitter).enabled = show;
    foreach (Canvas componentsInChild in ((Component) this).GetComponentsInChildren<Canvas>(true))
    {
      if (((Behaviour) componentsInChild).enabled != show)
        ((Behaviour) componentsInChild).enabled = show;
    }
    CanvasGroup component = ((Component) this).GetComponent<CanvasGroup>();
    if (Object.op_Inequality((Object) component, (Object) null))
    {
      component.interactable = show;
      component.blocksRaycasts = show;
      component.ignoreParentGroups = true;
    }
    base.OnShow(show);
  }

  protected override void OnShow(bool show)
  {
    base.OnShow(show);
    if (show)
    {
      if (Object.op_Inequality((Object) DetailsScreen.Instance, (Object) null))
        ((Component) DetailsScreen.Instance).gameObject.SetActive(false);
    }
    else if (Object.op_Inequality((Object) SelectTool.Instance.selected, (Object) null) && !((Component) DetailsScreen.Instance).gameObject.activeSelf)
    {
      ((Component) DetailsScreen.Instance).gameObject.SetActive(true);
      DetailsScreen.Instance.Refresh(((Component) SelectTool.Instance.selected).gameObject);
    }
    this.UpdateProgressBars();
    this.UpdatePointDisplay();
  }

  private void AbortDragging()
  {
    this.isDragging = false;
    this.draggingJustEnded = true;
  }

  private void LateUpdate() => this.draggingJustEnded = false;

  public override void OnKeyUp(KButtonEvent e)
  {
    if (!((Behaviour) this.canvas).enabled)
      return;
    if (!((KInputEvent) e).Consumed)
    {
      if (e.IsAction((Action) 5) && !this.isDragging && !this.draggingJustEnded)
        ManagementMenu.Instance.CloseAll();
      if (e.IsAction((Action) 5) || e.IsAction((Action) 3) || e.IsAction((Action) 6))
        this.AbortDragging();
      if (this.panUp && e.TryConsume((Action) 134))
      {
        this.panUp = false;
        return;
      }
      if (this.panDown && e.TryConsume((Action) 135))
      {
        this.panDown = false;
        return;
      }
      if (this.panRight && e.TryConsume((Action) 137))
      {
        this.panRight = false;
        return;
      }
      if (this.panLeft && e.TryConsume((Action) 136))
      {
        this.panLeft = false;
        return;
      }
    }
    base.OnKeyUp(e);
  }

  public override void OnKeyDown(KButtonEvent e)
  {
    if (!((Behaviour) this.canvas).enabled)
      return;
    if (!((KInputEvent) e).Consumed)
    {
      if (e.TryConsume((Action) 5))
      {
        this.dragStartPosition = KInputManager.GetMousePos();
        this.dragLastPosition = KInputManager.GetMousePos();
        return;
      }
      if (e.TryConsume((Action) 3))
      {
        this.dragStartPosition = KInputManager.GetMousePos();
        this.dragLastPosition = KInputManager.GetMousePos();
        return;
      }
      if ((double) KInputManager.GetMousePos().x > (double) Util.rectTransform((Component) this.sideBar).sizeDelta.x)
      {
        if (e.TryConsume((Action) 7))
        {
          this.targetZoom = Mathf.Clamp(this.targetZoom + this.zoomAmountPerScroll, this.minZoom, this.maxZoom);
          this.zoomCenterLock = false;
          return;
        }
        if (e.TryConsume((Action) 8))
        {
          this.targetZoom = Mathf.Clamp(this.targetZoom - this.zoomAmountPerScroll, this.minZoom, this.maxZoom);
          this.zoomCenterLock = false;
          return;
        }
      }
      if (e.TryConsume((Action) 1))
      {
        ManagementMenu.Instance.CloseAll();
        return;
      }
      if (e.TryConsume((Action) 136))
      {
        this.panLeft = true;
        return;
      }
      if (e.TryConsume((Action) 137))
      {
        this.panRight = true;
        return;
      }
      if (e.TryConsume((Action) 134))
      {
        this.panUp = true;
        return;
      }
      if (e.TryConsume((Action) 135))
      {
        this.panDown = true;
        return;
      }
    }
    base.OnKeyDown(e);
  }

  public static bool TechPassesSearchFilter(string techID, string filterString)
  {
    if (string.IsNullOrEmpty(filterString))
      return true;
    filterString = filterString.ToUpper();
    Tech tech = Db.Get().Techs.Get(techID);
    bool flag = STRINGS.UI.StripLinkFormatting(tech.Name).ToLower().ToUpper().Contains(filterString);
    if (!flag)
    {
      flag = tech.category.ToUpper().Contains(filterString);
      foreach (TechItem unlockedItem in tech.unlockedItems)
      {
        if (STRINGS.UI.StripLinkFormatting(unlockedItem.Name).ToLower().ToUpper().Contains(filterString))
        {
          flag = true;
          break;
        }
        if (STRINGS.UI.StripLinkFormatting(unlockedItem.description).ToLower().ToUpper().Contains(filterString))
        {
          flag = true;
          break;
        }
      }
    }
    return flag;
  }

  public static bool TechItemPassesSearchFilter(string techItemID, string filterString)
  {
    if (string.IsNullOrEmpty(filterString))
      return true;
    filterString = filterString.ToUpper();
    TechItem techItem = Db.Get().TechItems.Get(techItemID);
    bool flag = STRINGS.UI.StripLinkFormatting(techItem.Name).ToLower().ToUpper().Contains(filterString);
    if (!flag)
      flag = techItem.Name.ToUpper().Contains(filterString) && techItem.description.ToUpper().Contains(filterString);
    return flag;
  }

  public enum ResearchState
  {
    Available,
    ActiveResearch,
    ResearchComplete,
    MissingPrerequisites,
    StateCount,
  }
}
