// Decompiled with JetBrains decompiler
// Type: KUISelectable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/KUISelectable")]
public class KUISelectable : 
  KMonoBehaviour,
  IPointerEnterHandler,
  IEventSystemHandler,
  IPointerExitHandler
{
  private GameObject target;

  protected virtual void OnPrefabInit()
  {
  }

  protected virtual void OnSpawn() => ((UnityEvent) ((Component) this).GetComponent<Button>().onClick).AddListener(new UnityAction((object) this, __methodptr(OnClick)));

  public void SetTarget(GameObject target) => this.target = target;

  public void OnPointerEnter(PointerEventData eventData)
  {
    if (!Object.op_Inequality((Object) this.target, (Object) null))
      return;
    SelectTool.Instance.SetHoverOverride(this.target.GetComponent<KSelectable>());
  }

  public void OnPointerExit(PointerEventData eventData) => SelectTool.Instance.SetHoverOverride((KSelectable) null);

  private void OnClick()
  {
    if (!Object.op_Inequality((Object) this.target, (Object) null))
      return;
    SelectTool.Instance.Select(this.target.GetComponent<KSelectable>());
  }

  protected virtual void OnCmpDisable()
  {
    if (!Object.op_Inequality((Object) SelectTool.Instance, (Object) null))
      return;
    SelectTool.Instance.SetHoverOverride((KSelectable) null);
  }
}
