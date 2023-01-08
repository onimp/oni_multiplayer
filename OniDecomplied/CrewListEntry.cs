// Decompiled with JetBrains decompiler
// Type: CrewListEntry
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/CrewListEntry")]
public class CrewListEntry : 
  KMonoBehaviour,
  IPointerEnterHandler,
  IEventSystemHandler,
  IPointerExitHandler,
  IPointerClickHandler
{
  protected MinionIdentity identity;
  protected CrewPortrait portrait;
  public CrewPortrait PortraitPrefab;
  public GameObject crewPortraitParent;
  protected bool mouseOver;
  public Image BorderHighlight;
  public Image BGImage;
  public float lastClickTime;

  public MinionIdentity Identity => this.identity;

  public void OnPointerEnter(PointerEventData eventData)
  {
    this.mouseOver = true;
    ((Behaviour) this.BGImage).enabled = true;
    ((Graphic) this.BorderHighlight).color = new Color(0.65882355f, 0.2901961f, 0.4745098f);
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    this.mouseOver = false;
    ((Behaviour) this.BGImage).enabled = false;
    ((Graphic) this.BorderHighlight).color = new Color(0.8f, 0.8f, 0.8f);
  }

  public void OnPointerClick(PointerEventData eventData)
  {
    this.SelectCrewMember((double) Time.unscaledTime - (double) this.lastClickTime < 0.30000001192092896);
    this.lastClickTime = Time.unscaledTime;
  }

  public virtual void Populate(MinionIdentity _identity)
  {
    this.identity = _identity;
    if (Object.op_Equality((Object) this.portrait, (Object) null))
    {
      this.portrait = Util.KInstantiateUI<CrewPortrait>(((Component) this.PortraitPrefab).gameObject, Object.op_Inequality((Object) this.crewPortraitParent, (Object) null) ? this.crewPortraitParent : ((Component) this).gameObject, false);
      if (Object.op_Equality((Object) this.crewPortraitParent, (Object) null))
        this.portrait.transform.SetSiblingIndex(2);
    }
    this.portrait.SetIdentityObject((IAssignableIdentity) _identity);
  }

  public virtual void Refresh()
  {
  }

  public void RefreshCrewPortraitContent()
  {
    if (!Object.op_Inequality((Object) this.portrait, (Object) null))
      return;
    this.portrait.ForceRefresh();
  }

  private string seniorityString() => this.identity.GetAttributes().GetProfessionString();

  public void SelectCrewMember(bool focus)
  {
    if (focus)
      SelectTool.Instance.SelectAndFocus(TransformExtensions.GetPosition(this.identity.transform), ((Component) this.identity).GetComponent<KSelectable>(), new Vector3(8f, 0.0f, 0.0f));
    else
      SelectTool.Instance.Select(((Component) this.identity).GetComponent<KSelectable>());
  }
}
