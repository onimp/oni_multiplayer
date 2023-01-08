// Decompiled with JetBrains decompiler
// Type: KScrollbarVisibility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class KScrollbarVisibility : MonoBehaviour
{
  [SerializeField]
  private ScrollRect content;
  [SerializeField]
  private RectTransform parent;
  [SerializeField]
  private bool checkWidth = true;
  [SerializeField]
  private bool checkHeight = true;
  [SerializeField]
  private Scrollbar scrollbar;
  [SerializeField]
  private GameObject[] others;

  private void Start() => this.Update();

  private void Update()
  {
    if (Object.op_Equality((Object) this.content.content, (Object) null))
      return;
    bool flag = false;
    Vector2 vector2;
    ref Vector2 local = ref vector2;
    Rect rect1 = this.parent.rect;
    double width = (double) ((Rect) ref rect1).width;
    Rect rect2 = this.parent.rect;
    double height = (double) ((Rect) ref rect2).height;
    // ISSUE: explicit constructor call
    ((Vector2) ref local).\u002Ector((float) width, (float) height);
    Vector2 sizeDelta = ((Component) this.content.content).GetComponent<RectTransform>().sizeDelta;
    if ((double) sizeDelta.x >= (double) vector2.x && this.checkWidth || (double) sizeDelta.y >= (double) vector2.y && this.checkHeight)
      flag = true;
    if (((Component) this.scrollbar).gameObject.activeSelf == flag)
      return;
    ((Component) this.scrollbar).gameObject.SetActive(flag);
    if (this.others == null)
      return;
    foreach (GameObject other in this.others)
      other.SetActive(flag);
  }
}
