// Decompiled with JetBrains decompiler
// Type: StoryMessageScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMODUnity;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryMessageScreen : KScreen
{
  private const float ALPHA_SPEED = 0.01f;
  [SerializeField]
  private Image bg;
  [SerializeField]
  private GameObject dialog;
  [SerializeField]
  private KButton button;
  [SerializeField]
  private EventReference dialogSound;
  [SerializeField]
  private LocText titleLabel;
  [SerializeField]
  private LocText bodyLabel;
  private const float expandedHeight = 300f;
  [SerializeField]
  private GameObject content;
  public bool restoreInterfaceOnClose = true;
  public System.Action OnClose;
  private bool startFade;

  public string title
  {
    set => ((TMP_Text) this.titleLabel).SetText(value);
  }

  public string body
  {
    set => ((TMP_Text) this.bodyLabel).SetText(value);
  }

  public virtual float GetSortKey() => 8f;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    StoryMessageScreen.HideInterface(true);
    CameraController.Instance.FadeOut(0.5f);
  }

  private IEnumerator ExpandPanel()
  {
    this.content.gameObject.SetActive(true);
    yield return (object) SequenceUtil.WaitForSecondsRealtime(0.25f);
    float height = 0.0f;
    while ((double) height < 299.0)
    {
      height = Mathf.Lerp(Util.rectTransform(this.dialog).sizeDelta.y, 300f, Time.unscaledDeltaTime * 15f);
      Util.rectTransform(this.dialog).sizeDelta = new Vector2(Util.rectTransform(this.dialog).sizeDelta.x, height);
      yield return (object) 0;
    }
    CameraController.Instance.FadeOut(0.5f);
    yield return (object) null;
  }

  private IEnumerator CollapsePanel()
  {
    StoryMessageScreen storyMessageScreen = this;
    float height = 300f;
    while ((double) height > 0.0)
    {
      height = Mathf.Lerp(Util.rectTransform(storyMessageScreen.dialog).sizeDelta.y, -1f, Time.unscaledDeltaTime * 15f);
      Util.rectTransform(storyMessageScreen.dialog).sizeDelta = new Vector2(Util.rectTransform(storyMessageScreen.dialog).sizeDelta.x, height);
      yield return (object) 0;
    }
    storyMessageScreen.content.gameObject.SetActive(false);
    if (storyMessageScreen.OnClose != null)
    {
      storyMessageScreen.OnClose();
      storyMessageScreen.OnClose = (System.Action) null;
    }
    storyMessageScreen.Deactivate();
    yield return (object) null;
  }

  public static void HideInterface(bool hide)
  {
    SelectTool.Instance.Select((KSelectable) null, true);
    NotificationScreen.Instance.Show(!hide);
    OverlayMenu.Instance.Show(!hide);
    if (Object.op_Inequality((Object) PlanScreen.Instance, (Object) null))
      PlanScreen.Instance.Show(!hide);
    if (Object.op_Inequality((Object) BuildMenu.Instance, (Object) null))
      BuildMenu.Instance.Show(!hide);
    ManagementMenu.Instance.Show(!hide);
    ToolMenu.Instance.Show(!hide);
    ToolMenu.Instance.PriorityScreen.Show(!hide);
    ColonyDiagnosticScreen.Instance.Show(!hide);
    PinnedResourcesPanel.Instance.Show(!hide);
    TopLeftControlScreen.Instance.Show(!hide);
    if (Object.op_Inequality((Object) WorldSelector.Instance, (Object) null))
      WorldSelector.Instance.Show(!hide);
    DateTime.Instance.Show(!hide);
    if (Object.op_Inequality((Object) BuildWatermark.Instance, (Object) null))
      BuildWatermark.Instance.Show(!hide);
    PopFXManager.Instance.Show(!hide);
  }

  public void Update()
  {
    if (!this.startFade)
      return;
    Color color = ((Graphic) this.bg).color;
    color.a -= 0.01f;
    if ((double) color.a <= 0.0)
      color.a = 0.0f;
    ((Graphic) this.bg).color = color;
  }

  protected virtual void OnActivate()
  {
    base.OnActivate();
    SelectTool.Instance.Select((KSelectable) null);
    this.button.onClick += (System.Action) (() => ((MonoBehaviour) this).StartCoroutine(this.CollapsePanel()));
    this.dialog.GetComponent<KScreen>().Show(false);
    this.startFade = false;
    CameraController.Instance.DisableUserCameraControl = true;
    KFMOD.PlayUISound(this.dialogSound);
    this.dialog.GetComponent<KScreen>().Activate();
    this.dialog.GetComponent<KScreen>().SetShouldFadeIn(true);
    this.dialog.GetComponent<KScreen>().Show(true);
    MusicManager.instance.PlaySong("Music_Victory_01_Message");
    ((MonoBehaviour) this).StartCoroutine(this.ExpandPanel());
  }

  protected virtual void OnDeactivate()
  {
    this.IsActive();
    base.OnDeactivate();
    MusicManager.instance.StopSong("Music_Victory_01_Message");
    if (!this.restoreInterfaceOnClose)
      return;
    CameraController.Instance.DisableUserCameraControl = false;
    CameraController.Instance.FadeIn();
    StoryMessageScreen.HideInterface(false);
  }

  public virtual void OnKeyDown(KButtonEvent e)
  {
    if (e.TryConsume((Action) 1))
      ((MonoBehaviour) this).StartCoroutine(this.CollapsePanel());
    ((KInputEvent) e).Consumed = true;
  }

  public virtual void OnKeyUp(KButtonEvent e) => ((KInputEvent) e).Consumed = true;
}
