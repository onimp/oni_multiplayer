// Decompiled with JetBrains decompiler
// Type: DragMe
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragMe : 
  MonoBehaviour,
  IBeginDragHandler,
  IEventSystemHandler,
  IDragHandler,
  IEndDragHandler
{
  public bool dragOnSurfaces = true;
  private GameObject m_DraggingIcon;
  private RectTransform m_DraggingPlane;
  public DragMe.IDragListener listener;

  public void OnBeginDrag(PointerEventData eventData)
  {
    Canvas inParents = DragMe.FindInParents<Canvas>(((Component) this).gameObject);
    if (Object.op_Equality((Object) inParents, (Object) null))
      return;
    this.m_DraggingIcon = Object.Instantiate<GameObject>(((Component) this).gameObject);
    GraphicRaycaster component = this.m_DraggingIcon.GetComponent<GraphicRaycaster>();
    if (Object.op_Inequality((Object) component, (Object) null))
      ((Behaviour) component).enabled = false;
    ((Object) this.m_DraggingIcon).name = "dragObj";
    this.m_DraggingIcon.transform.SetParent(((Component) inParents).transform, false);
    this.m_DraggingIcon.transform.SetAsLastSibling();
    this.m_DraggingIcon.GetComponent<RectTransform>().pivot = Vector2.zero;
    this.m_DraggingPlane = !this.dragOnSurfaces ? ((Component) inParents).transform as RectTransform : ((Component) this).transform as RectTransform;
    this.SetDraggedPosition(eventData);
    this.listener.OnBeginDrag(eventData.position);
  }

  public void OnDrag(PointerEventData data)
  {
    if (!Object.op_Inequality((Object) this.m_DraggingIcon, (Object) null))
      return;
    this.SetDraggedPosition(data);
  }

  private void SetDraggedPosition(PointerEventData data)
  {
    if (this.dragOnSurfaces && Object.op_Inequality((Object) data.pointerEnter, (Object) null) && Object.op_Inequality((Object) (data.pointerEnter.transform as RectTransform), (Object) null))
      this.m_DraggingPlane = data.pointerEnter.transform as RectTransform;
    RectTransform component = this.m_DraggingIcon.GetComponent<RectTransform>();
    Vector3 vector3;
    if (!RectTransformUtility.ScreenPointToWorldPointInRectangle(this.m_DraggingPlane, data.position, data.pressEventCamera, ref vector3))
      return;
    ((Transform) component).position = vector3;
    ((Transform) component).rotation = ((Transform) this.m_DraggingPlane).rotation;
  }

  public void OnEndDrag(PointerEventData eventData)
  {
    this.listener.OnEndDrag(eventData.position);
    if (!Object.op_Inequality((Object) this.m_DraggingIcon, (Object) null))
      return;
    Object.Destroy((Object) this.m_DraggingIcon);
  }

  public static T FindInParents<T>(GameObject go) where T : Component
  {
    if (Object.op_Equality((Object) go, (Object) null))
      return default (T);
    T inParents = default (T);
    for (Transform parent = go.transform.parent; Object.op_Inequality((Object) parent, (Object) null) && Object.op_Equality((Object) (object) inParents, (Object) null); parent = parent.parent)
      inParents = ((Component) parent).gameObject.GetComponent<T>();
    return inParents;
  }

  public interface IDragListener
  {
    void OnBeginDrag(Vector2 position);

    void OnEndDrag(Vector2 position);
  }
}
