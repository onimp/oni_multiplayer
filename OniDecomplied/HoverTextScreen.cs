// Decompiled with JetBrains decompiler
// Type: HoverTextScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class HoverTextScreen : KScreen
{
  [SerializeField]
  private HoverTextSkin skin;
  public Sprite[] HoverIcons;
  public HoverTextDrawer drawer;
  public static HoverTextScreen Instance;

  public static void DestroyInstance() => HoverTextScreen.Instance = (HoverTextScreen) null;

  protected virtual void OnActivate()
  {
    base.OnActivate();
    HoverTextScreen.Instance = this;
    this.drawer = new HoverTextDrawer(this.skin.skin, ((Component) this).GetComponent<RectTransform>());
  }

  public HoverTextDrawer BeginDrawing()
  {
    Vector2 zero = Vector2.zero;
    Vector2 vector2 = Vector2.op_Implicit(KInputManager.GetMousePos());
    RectTransform parent = ((KMonoBehaviour) this).transform.parent as RectTransform;
    RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, vector2, ((Component) ((KMonoBehaviour) this).transform.parent).GetComponent<Canvas>().worldCamera, ref zero);
    zero.x += parent.sizeDelta.x / 2f;
    zero.y -= parent.sizeDelta.y / 2f;
    this.drawer.BeginDrawing(zero);
    return this.drawer;
  }

  private void Update() => this.drawer.SetEnabled(PlayerController.Instance.ActiveTool.ShowHoverUI());

  public Sprite GetSprite(string byName)
  {
    foreach (Sprite hoverIcon in this.HoverIcons)
    {
      if (Object.op_Inequality((Object) hoverIcon, (Object) null) && ((Object) hoverIcon).name == byName)
        return hoverIcon;
    }
    Debug.LogWarning((object) ("No icon named " + byName + " was found on HoverTextScreen.prefab"));
    return (Sprite) null;
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    this.drawer.Cleanup();
  }
}
