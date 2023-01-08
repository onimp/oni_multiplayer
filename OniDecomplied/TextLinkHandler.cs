// Decompiled with JetBrains decompiler
// Type: TextLinkHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextLinkHandler : 
  MonoBehaviour,
  IPointerClickHandler,
  IEventSystemHandler,
  IPointerEnterHandler,
  IPointerExitHandler
{
  private static TextLinkHandler hoveredText;
  [MyCmpGet]
  private LocText text;
  private bool hoverLink;

  public void OnPointerClick(PointerEventData eventData)
  {
    if (eventData.button != null || !this.text.AllowLinks)
      return;
    int intersectingLink = TMP_TextUtilities.FindIntersectingLink((TMP_Text) this.text, KInputManager.GetMousePos(), (Camera) null);
    if (intersectingLink == -1)
      return;
    string str = CodexCache.FormatLinkID(((TMP_LinkInfo) ref ((TMP_Text) this.text).textInfo.linkInfo[intersectingLink]).GetLinkID());
    if (!CodexCache.entries.ContainsKey(str))
    {
      SubEntry subEntry = CodexCache.FindSubEntry(str);
      if (subEntry == null || subEntry.disabled)
        str = "PAGENOTFOUND";
    }
    else if (CodexCache.entries[str].disabled)
      str = "PAGENOTFOUND";
    if (!((Component) ManagementMenu.Instance.codexScreen).gameObject.activeInHierarchy)
      ManagementMenu.Instance.ToggleCodex();
    ManagementMenu.Instance.codexScreen.ChangeArticle(str, true, new Vector3());
  }

  private void Update()
  {
    this.CheckMouseOver();
    if (!Object.op_Equality((Object) TextLinkHandler.hoveredText, (Object) this) || !this.text.AllowLinks)
      return;
    PlayerController.Instance.ActiveTool.SetLinkCursor(this.hoverLink);
  }

  private void OnEnable() => this.CheckMouseOver();

  private void OnDisable() => this.ClearState();

  private void Awake()
  {
    this.text = ((Component) this).GetComponent<LocText>();
    if (!this.text.AllowLinks || ((Graphic) this.text).raycastTarget)
      return;
    ((Graphic) this.text).raycastTarget = true;
  }

  public void OnPointerEnter(PointerEventData eventData) => this.SetMouseOver();

  public void OnPointerExit(PointerEventData eventData) => this.ClearState();

  private void ClearState()
  {
    if (Object.op_Equality((Object) this, (Object) null) || ((object) this).Equals((object) null) || !Object.op_Equality((Object) TextLinkHandler.hoveredText, (Object) this))
      return;
    if (this.hoverLink && Object.op_Inequality((Object) PlayerController.Instance, (Object) null) && Object.op_Inequality((Object) PlayerController.Instance.ActiveTool, (Object) null))
      PlayerController.Instance.ActiveTool.SetLinkCursor(false);
    TextLinkHandler.hoveredText = (TextLinkHandler) null;
    this.hoverLink = false;
  }

  public void CheckMouseOver()
  {
    if (Object.op_Equality((Object) this.text, (Object) null))
      return;
    if (TMP_TextUtilities.FindIntersectingLink((TMP_Text) this.text, KInputManager.GetMousePos(), (Camera) null) != -1)
    {
      this.SetMouseOver();
      this.hoverLink = true;
    }
    else
    {
      if (!Object.op_Equality((Object) TextLinkHandler.hoveredText, (Object) this))
        return;
      this.hoverLink = false;
    }
  }

  private void SetMouseOver()
  {
    if (Object.op_Inequality((Object) TextLinkHandler.hoveredText, (Object) null) && Object.op_Inequality((Object) TextLinkHandler.hoveredText, (Object) this))
      TextLinkHandler.hoveredText.hoverLink = false;
    TextLinkHandler.hoveredText = this;
  }
}
