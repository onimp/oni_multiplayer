// Decompiled with JetBrains decompiler
// Type: NotificationHighlightController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationHighlightController : KMonoBehaviour
{
  public RectTransform highlightBoxPrefab;
  private RectTransform highlightBox;
  private List<NotificationHighlightTarget> targets = new List<NotificationHighlightTarget>();
  private ManagementMenuNotification activeTargetNotification;

  protected virtual void OnSpawn()
  {
    this.highlightBox = Util.KInstantiateUI<RectTransform>(((Component) this.highlightBoxPrefab).gameObject, ((Component) this).gameObject, false);
    this.HideBox();
  }

  [ContextMenu("Force Update")]
  protected void LateUpdate()
  {
    bool flag = false;
    if (this.activeTargetNotification != null)
    {
      foreach (NotificationHighlightTarget target in this.targets)
      {
        if (target.targetKey == this.activeTargetNotification.highlightTarget)
        {
          this.SnapBoxToTarget(target);
          flag = true;
          break;
        }
      }
    }
    if (flag)
      return;
    this.HideBox();
  }

  public void AddTarget(NotificationHighlightTarget target) => this.targets.Add(target);

  public void RemoveTarget(NotificationHighlightTarget target) => this.targets.Remove(target);

  public void SetActiveTarget(ManagementMenuNotification notification) => this.activeTargetNotification = notification;

  public void ClearActiveTarget(ManagementMenuNotification checkNotification)
  {
    if (checkNotification != this.activeTargetNotification)
      return;
    this.activeTargetNotification = (ManagementMenuNotification) null;
  }

  public void ClearActiveTarget() => this.activeTargetNotification = (ManagementMenuNotification) null;

  public void TargetViewed(NotificationHighlightTarget target)
  {
    if (this.activeTargetNotification == null || !(this.activeTargetNotification.highlightTarget == target.targetKey))
      return;
    this.activeTargetNotification.View();
  }

  private void SnapBoxToTarget(NotificationHighlightTarget target)
  {
    RectTransform rectTransform1 = Util.rectTransform((Component) target);
    Vector3 position = TransformExtensions.GetPosition((Transform) rectTransform1);
    RectTransform highlightBox1 = this.highlightBox;
    Rect rect1 = rectTransform1.rect;
    Vector2 size = ((Rect) ref rect1).size;
    highlightBox1.sizeDelta = size;
    RectTransform highlightBox2 = this.highlightBox;
    Vector3 vector3_1 = position;
    Rect rect2 = rectTransform1.rect;
    double x = (double) ((Rect) ref rect2).position.x;
    rect2 = rectTransform1.rect;
    double y = (double) ((Rect) ref rect2).position.y;
    Vector3 vector3_2 = new Vector3((float) x, (float) y, 0.0f);
    Vector3 vector3_3 = Vector3.op_Addition(vector3_1, vector3_2);
    TransformExtensions.SetPosition((Transform) highlightBox2, vector3_3);
    RectMask2D componentInParent = ((Component) rectTransform1).GetComponentInParent<RectMask2D>();
    if (Object.op_Inequality((Object) componentInParent, (Object) null))
    {
      RectTransform rectTransform2 = Util.rectTransform((Component) componentInParent);
      Rect rect3 = rectTransform2.rect;
      Vector3 vector3_4 = ((Transform) rectTransform2).TransformPoint(Vector2.op_Implicit(((Rect) ref rect3).min));
      rect3 = rectTransform2.rect;
      Vector3 vector3_5 = ((Transform) rectTransform2).TransformPoint(Vector2.op_Implicit(((Rect) ref rect3).max));
      RectTransform highlightBox3 = this.highlightBox;
      rect3 = this.highlightBox.rect;
      Vector3 vector3_6 = Vector2.op_Implicit(((Rect) ref rect3).min);
      Vector3 vector3_7 = ((Transform) highlightBox3).TransformPoint(vector3_6);
      RectTransform highlightBox4 = this.highlightBox;
      Rect rect4 = this.highlightBox.rect;
      Vector3 vector3_8 = Vector2.op_Implicit(((Rect) ref rect4).max);
      Vector3 vector3_9 = ((Transform) highlightBox4).TransformPoint(vector3_8);
      Vector3 vector3_10 = Vector3.op_Subtraction(vector3_4, vector3_7);
      Vector3 vector3_11 = vector3_9;
      Vector3 vector3_12 = Vector3.op_Subtraction(vector3_5, vector3_11);
      if ((double) vector3_10.x > 0.0)
      {
        this.highlightBox.anchoredPosition = Vector2.op_Addition(this.highlightBox.anchoredPosition, new Vector2(vector3_10.x, 0.0f));
        RectTransform highlightBox5 = this.highlightBox;
        highlightBox5.sizeDelta = Vector2.op_Subtraction(highlightBox5.sizeDelta, new Vector2(vector3_10.x, 0.0f));
      }
      else if ((double) vector3_10.y > 0.0)
      {
        this.highlightBox.anchoredPosition = Vector2.op_Addition(this.highlightBox.anchoredPosition, new Vector2(0.0f, vector3_10.y));
        RectTransform highlightBox6 = this.highlightBox;
        highlightBox6.sizeDelta = Vector2.op_Subtraction(highlightBox6.sizeDelta, new Vector2(0.0f, vector3_10.y));
      }
      if ((double) vector3_12.x < 0.0)
      {
        RectTransform highlightBox7 = this.highlightBox;
        highlightBox7.sizeDelta = Vector2.op_Addition(highlightBox7.sizeDelta, new Vector2(vector3_12.x, 0.0f));
      }
      if ((double) vector3_12.y < 0.0)
      {
        RectTransform highlightBox8 = this.highlightBox;
        highlightBox8.sizeDelta = Vector2.op_Addition(highlightBox8.sizeDelta, new Vector2(0.0f, vector3_12.y));
      }
    }
    ((Component) this.highlightBox).gameObject.SetActive((double) this.highlightBox.sizeDelta.x > 0.0 && (double) this.highlightBox.sizeDelta.y > 0.0);
  }

  private void HideBox() => ((Component) this.highlightBox).gameObject.SetActive(false);
}
