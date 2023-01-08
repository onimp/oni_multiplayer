// Decompiled with JetBrains decompiler
// Type: NotificationScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NotificationScreen : KScreen
{
  public float lifetime;
  public bool dirty;
  public GameObject LabelPrefab;
  public GameObject LabelsFolder;
  public GameObject MessagesPrefab;
  public GameObject MessagesFolder;
  private MessageDialogFrame messageDialog;
  private float initTime;
  [MyCmpAdd]
  private Notifier notifier;
  [SerializeField]
  private List<MessageDialog> dialogPrefabs = new List<MessageDialog>();
  [SerializeField]
  private Color badColorBG;
  [SerializeField]
  private Color badColor = Color.red;
  [SerializeField]
  private Color normalColorBG;
  [SerializeField]
  private Color normalColor = Color.white;
  [SerializeField]
  private Color warningColorBG;
  [SerializeField]
  private Color warningColor;
  [SerializeField]
  private Color messageColorBG;
  [SerializeField]
  private Color messageColor;
  [SerializeField]
  private Color eventColorBG;
  [SerializeField]
  private Color eventColor;
  public Sprite icon_normal;
  public Sprite icon_warning;
  public Sprite icon_bad;
  public Sprite icon_threatening;
  public Sprite icon_message;
  public Sprite icon_video;
  public Sprite icon_event;
  private List<Notification> pendingNotifications = new List<Notification>();
  private List<Notification> notifications = new List<Notification>();
  public TextStyleSetting TooltipTextStyle;
  private Dictionary<NotificationType, string> notificationSounds = new Dictionary<NotificationType, string>();
  private Dictionary<string, float> timeOfLastNotification = new Dictionary<string, float>();
  private float soundDecayTime = 10f;
  private List<NotificationScreen.Entry> entries = new List<NotificationScreen.Entry>();
  private Dictionary<string, NotificationScreen.Entry> entriesByMessage = new Dictionary<string, NotificationScreen.Entry>();

  public static NotificationScreen Instance { get; private set; }

  public static void DestroyInstance() => NotificationScreen.Instance = (NotificationScreen) null;

  private void OnAddNotifier(Notifier notifier)
  {
    notifier.OnAdd += new Action<Notification>(this.OnAddNotification);
    notifier.OnRemove += new Action<Notification>(this.OnRemoveNotification);
  }

  private void OnRemoveNotifier(Notifier notifier)
  {
    notifier.OnAdd -= new Action<Notification>(this.OnAddNotification);
    notifier.OnRemove -= new Action<Notification>(this.OnRemoveNotification);
  }

  private void OnAddNotification(Notification notification) => this.pendingNotifications.Add(notification);

  private void OnRemoveNotification(Notification notification)
  {
    this.dirty = true;
    this.pendingNotifications.Remove(notification);
    NotificationScreen.Entry entry = (NotificationScreen.Entry) null;
    this.entriesByMessage.TryGetValue(notification.titleText, out entry);
    if (entry == null)
      return;
    this.notifications.Remove(notification);
    entry.Remove(notification);
    if (entry.notifications.Count != 0)
      return;
    Object.Destroy((Object) entry.label);
    this.entriesByMessage[notification.titleText] = (NotificationScreen.Entry) null;
    this.entries.Remove(entry);
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    NotificationScreen.Instance = this;
    Components.Notifiers.OnAdd += new Action<Notifier>(this.OnAddNotifier);
    Components.Notifiers.OnRemove += new Action<Notifier>(this.OnRemoveNotifier);
    foreach (Notifier notifier in Components.Notifiers.Items)
      this.OnAddNotifier(notifier);
    this.MessagesPrefab.gameObject.SetActive(false);
    this.LabelPrefab.gameObject.SetActive(false);
    this.InitNotificationSounds();
  }

  private void OnNewMessage(object data) => this.notifier.Add((Notification) new MessageNotification((Message) data));

  private void ShowMessage(MessageNotification mn)
  {
    mn.message.OnClick();
    if (mn.message.ShowDialog())
    {
      for (int index = 0; index < this.dialogPrefabs.Count; ++index)
      {
        if (this.dialogPrefabs[index].CanDisplay(mn.message))
        {
          if (Object.op_Inequality((Object) this.messageDialog, (Object) null))
          {
            Object.Destroy((Object) ((Component) this.messageDialog).gameObject);
            this.messageDialog = (MessageDialogFrame) null;
          }
          this.messageDialog = Util.KInstantiateUI<MessageDialogFrame>(((Component) ScreenPrefabs.Instance.MessageDialogFrame).gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, false);
          this.messageDialog.SetMessage(Util.KInstantiateUI<MessageDialog>(((Component) this.dialogPrefabs[index]).gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, false), mn.message);
          this.messageDialog.Show(true);
          break;
        }
      }
    }
    Messenger.Instance.RemoveMessage(mn.message);
    mn.Clear();
  }

  public void OnClickNextMessage() => this.ShowMessage((MessageNotification) this.notifications.Find((Predicate<Notification>) (notification => notification.Type == NotificationType.Messages)));

  protected virtual void OnCleanUp()
  {
    Components.Notifiers.OnAdd -= new Action<Notifier>(this.OnAddNotifier);
    Components.Notifiers.OnRemove -= new Action<Notifier>(this.OnRemoveNotifier);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.initTime = KTime.Instance.UnscaledGameTime;
    foreach (Graphic componentsInChild in this.LabelPrefab.GetComponentsInChildren<LocText>())
      componentsInChild.color = this.normalColor;
    foreach (Graphic componentsInChild in this.MessagesPrefab.GetComponentsInChildren<LocText>())
      componentsInChild.color = this.normalColor;
    ((KMonoBehaviour) this).Subscribe(((Component) Messenger.Instance).gameObject, 1558809273, new Action<object>(this.OnNewMessage));
    foreach (Message message in Messenger.Instance.Messages)
    {
      Notification notification = (Notification) new MessageNotification(message);
      notification.playSound = false;
      this.notifier.Add(notification);
    }
  }

  protected virtual void OnActivate()
  {
    base.OnActivate();
    this.dirty = true;
  }

  private void AddNotification(Notification notification)
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    NotificationScreen.\u003C\u003Ec__DisplayClass52_0 cDisplayClass520 = new NotificationScreen.\u003C\u003Ec__DisplayClass52_0();
    // ISSUE: reference to a compiler-generated field
    cDisplayClass520.\u003C\u003E4__this = this;
    // ISSUE: reference to a compiler-generated field
    cDisplayClass520.notification = notification;
    if (DebugHandler.NotificationsDisabled)
      return;
    // ISSUE: reference to a compiler-generated field
    this.notifications.Add(cDisplayClass520.notification);
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this.entriesByMessage.TryGetValue(cDisplayClass520.notification.titleText, out cDisplayClass520.entry);
    // ISSUE: reference to a compiler-generated field
    if (cDisplayClass520.entry == null)
    {
      // ISSUE: reference to a compiler-generated field
      HierarchyReferences hierarchyReferences = cDisplayClass520.notification.Type != NotificationType.Messages ? Util.KInstantiateUI<HierarchyReferences>(this.LabelPrefab, this.LabelsFolder, false) : Util.KInstantiateUI<HierarchyReferences>(this.MessagesPrefab, this.MessagesFolder, false);
      Button reference1 = hierarchyReferences.GetReference<Button>("DismissButton");
      // ISSUE: reference to a compiler-generated field
      ((Component) reference1).gameObject.SetActive(cDisplayClass520.notification.showDismissButton);
      // ISSUE: reference to a compiler-generated field
      if (cDisplayClass520.notification.showDismissButton)
      {
        // ISSUE: method pointer
        ((UnityEvent) reference1.onClick).AddListener(new UnityAction((object) cDisplayClass520, __methodptr(\u003CAddNotification\u003Eb__0)));
      }
      hierarchyReferences.GetReference<NotificationAnimator>("Animator").Begin();
      ((Component) hierarchyReferences).gameObject.SetActive(true);
      // ISSUE: reference to a compiler-generated field
      if (cDisplayClass520.notification.ToolTip != null)
      {
        ToolTip tooltip = hierarchyReferences.GetReference<ToolTip>("ToolTip");
        tooltip.OnToolTip = (Func<string>) (() =>
        {
          tooltip.ClearMultiStringTooltip();
          tooltip.AddMultiStringTooltip(notification.ToolTip(entry.notifications, notification.tooltipData), this.TooltipTextStyle);
          return "";
        });
      }
      KImage reference2 = hierarchyReferences.GetReference<KImage>("Icon");
      LocText reference3 = hierarchyReferences.GetReference<LocText>("Text");
      Button reference4 = hierarchyReferences.GetReference<Button>("MainButton");
      ColorBlock colors = ((Selectable) reference4).colors;
      // ISSUE: reference to a compiler-generated field
      switch (cDisplayClass520.notification.Type)
      {
        case NotificationType.Bad:
        case NotificationType.DuplicantThreatening:
          ((ColorBlock) ref colors).normalColor = this.badColorBG;
          ((Graphic) reference3).color = this.badColor;
          ((Graphic) reference2).color = this.badColor;
          // ISSUE: reference to a compiler-generated field
          ((Image) reference2).sprite = cDisplayClass520.notification.Type == NotificationType.Bad ? this.icon_bad : this.icon_threatening;
          break;
        case NotificationType.Tutorial:
          ((ColorBlock) ref colors).normalColor = this.warningColorBG;
          ((Graphic) reference3).color = this.warningColor;
          ((Graphic) reference2).color = this.warningColor;
          ((Image) reference2).sprite = this.icon_warning;
          break;
        case NotificationType.Messages:
          ((ColorBlock) ref colors).normalColor = this.messageColorBG;
          ((Graphic) reference3).color = this.messageColor;
          ((Graphic) reference2).color = this.messageColor;
          ((Image) reference2).sprite = this.icon_message;
          // ISSUE: reference to a compiler-generated field
          if (cDisplayClass520.notification is MessageNotification notification1 && notification1.message is TutorialMessage message && !string.IsNullOrEmpty(message.videoClipId))
          {
            ((Image) reference2).sprite = this.icon_video;
            break;
          }
          break;
        case NotificationType.Event:
          ((ColorBlock) ref colors).normalColor = this.eventColorBG;
          ((Graphic) reference3).color = this.eventColor;
          ((Graphic) reference2).color = this.eventColor;
          ((Image) reference2).sprite = this.icon_event;
          break;
        default:
          ((ColorBlock) ref colors).normalColor = this.normalColorBG;
          ((Graphic) reference3).color = this.normalColor;
          ((Graphic) reference2).color = this.normalColor;
          ((Image) reference2).sprite = this.icon_normal;
          break;
      }
      ((Selectable) reference4).colors = colors;
      // ISSUE: method pointer
      ((UnityEvent) reference4.onClick).AddListener(new UnityAction((object) cDisplayClass520, __methodptr(\u003CAddNotification\u003Eb__1)));
      string str = "";
      // ISSUE: reference to a compiler-generated field
      if ((double) KTime.Instance.UnscaledGameTime - (double) this.initTime > 5.0 && cDisplayClass520.notification.playSound)
      {
        // ISSUE: reference to a compiler-generated field
        this.PlayDingSound(cDisplayClass520.notification, 0);
      }
      else
        str = "too early";
      if (AudioDebug.Get().debugNotificationSounds)
      {
        // ISSUE: reference to a compiler-generated field
        Debug.Log((object) ("Notification(" + cDisplayClass520.notification.titleText + "):" + str));
      }
      // ISSUE: reference to a compiler-generated field
      cDisplayClass520.entry = new NotificationScreen.Entry(((Component) hierarchyReferences).gameObject);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.entriesByMessage[cDisplayClass520.notification.titleText] = cDisplayClass520.entry;
      // ISSUE: reference to a compiler-generated field
      this.entries.Add(cDisplayClass520.entry);
    }
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    cDisplayClass520.entry.Add(cDisplayClass520.notification);
    this.dirty = true;
    this.SortNotifications();
  }

  private void SortNotifications()
  {
    this.notifications.Sort((Comparison<Notification>) ((n1, n2) => n1.Type == n2.Type ? n1.Idx - n2.Idx : n1.Type - n2.Type));
    foreach (Notification notification in this.notifications)
    {
      NotificationScreen.Entry entry = (NotificationScreen.Entry) null;
      this.entriesByMessage.TryGetValue(notification.titleText, out entry);
      ((Transform) entry?.label.GetComponent<RectTransform>()).SetAsLastSibling();
    }
  }

  private void PlayDingSound(Notification notification, int count)
  {
    string str1;
    if (!this.notificationSounds.TryGetValue(notification.Type, out str1))
      str1 = "Notification";
    float num1;
    if (!this.timeOfLastNotification.TryGetValue(str1, out num1))
      num1 = 0.0f;
    float num2 = notification.volume_attenuation ? (Time.time - num1) / this.soundDecayTime : 1f;
    this.timeOfLastNotification[str1] = Time.time;
    string str2 = count <= 1 ? GlobalAssets.GetSound(str1) : GlobalAssets.GetSound(str1 + "_AddCount", true) ?? GlobalAssets.GetSound(str1);
    if (!notification.playSound)
      return;
    EventInstance eventInstance = KFMOD.BeginOneShot(str2, Vector3.zero, 1f);
    ((EventInstance) ref eventInstance).setParameterByName("timeSinceLast", num2, false);
    KFMOD.EndOneShot(eventInstance);
  }

  private void Update()
  {
    int index1 = 0;
    while (index1 < this.pendingNotifications.Count)
    {
      if (this.pendingNotifications[index1].IsReady())
      {
        this.AddNotification(this.pendingNotifications[index1]);
        this.pendingNotifications.RemoveAt(index1);
      }
      else
        ++index1;
    }
    int num1 = 0;
    int num2 = 0;
    for (int index2 = 0; index2 < this.notifications.Count; ++index2)
    {
      Notification notification = this.notifications[index2];
      if (notification.Type == NotificationType.Messages)
        ++num2;
      else
        ++num1;
      if (notification.expires && (double) KTime.Instance.UnscaledGameTime - (double) notification.Time > (double) this.lifetime)
      {
        this.dirty = true;
        if (Object.op_Equality((Object) notification.Notifier, (Object) null))
          this.OnRemoveNotification(notification);
        else
          notification.Clear();
      }
    }
  }

  private void OnClick(NotificationScreen.Entry entry)
  {
    Notification clickedNotification = entry.NextClickedNotification;
    ((KMonoBehaviour) this).PlaySound3D(GlobalAssets.GetSound("HUD_Click_Open"));
    if (clickedNotification.customClickCallback != null)
    {
      clickedNotification.customClickCallback(clickedNotification.customClickData);
    }
    else
    {
      if (Object.op_Inequality((Object) clickedNotification.clickFocus, (Object) null))
      {
        Vector3 position = TransformExtensions.GetPosition(clickedNotification.clickFocus);
        position.z = -40f;
        ClusterGridEntity component1 = ((Component) clickedNotification.clickFocus).GetComponent<ClusterGridEntity>();
        KSelectable component2 = ((Component) clickedNotification.clickFocus).GetComponent<KSelectable>();
        int myWorldId = ((Component) clickedNotification.clickFocus).gameObject.GetMyWorldId();
        if (myWorldId != -1)
          CameraController.Instance.ActiveWorldStarWipe(myWorldId, position);
        else if (DlcManager.FeatureClusterSpaceEnabled() && Object.op_Inequality((Object) component1, (Object) null) && component1.IsVisible)
        {
          ManagementMenu.Instance.OpenClusterMap();
          ClusterMapScreen.Instance.SetTargetFocusPosition(component1.Location);
        }
        if (Object.op_Inequality((Object) component2, (Object) null))
        {
          if (DlcManager.FeatureClusterSpaceEnabled() && Object.op_Inequality((Object) component1, (Object) null) && component1.IsVisible)
            ClusterMapSelectTool.Instance.Select(component2);
          else
            SelectTool.Instance.Select(component2);
        }
      }
      else if (Object.op_Inequality((Object) clickedNotification.Notifier, (Object) null))
        SelectTool.Instance.Select(((Component) clickedNotification.Notifier).GetComponent<KSelectable>());
      if (clickedNotification.Type == NotificationType.Messages)
        this.ShowMessage((MessageNotification) clickedNotification);
    }
    if (!clickedNotification.clearOnClick)
      return;
    clickedNotification.Clear();
  }

  private void PositionLocatorIcon()
  {
  }

  private void InitNotificationSounds()
  {
    this.notificationSounds[NotificationType.Good] = "Notification";
    this.notificationSounds[NotificationType.BadMinor] = "Notification";
    this.notificationSounds[NotificationType.Bad] = "Warning";
    this.notificationSounds[NotificationType.Neutral] = "Notification";
    this.notificationSounds[NotificationType.Tutorial] = "Notification";
    this.notificationSounds[NotificationType.Messages] = "Message";
    this.notificationSounds[NotificationType.DuplicantThreatening] = "Warning_DupeThreatening";
    this.notificationSounds[NotificationType.Event] = "Message";
  }

  public Color32 BadColorBG => Color32.op_Implicit(this.badColorBG);

  public Sprite GetNotificationIcon(NotificationType type)
  {
    switch (type)
    {
      case NotificationType.Bad:
        return this.icon_bad;
      case NotificationType.Tutorial:
        return this.icon_warning;
      case NotificationType.Messages:
        return this.icon_message;
      case NotificationType.DuplicantThreatening:
        return this.icon_threatening;
      case NotificationType.Event:
        return this.icon_event;
      default:
        return this.icon_normal;
    }
  }

  public Color GetNotificationColour(NotificationType type)
  {
    switch (type)
    {
      case NotificationType.Bad:
        return this.badColor;
      case NotificationType.Tutorial:
        return this.warningColor;
      case NotificationType.Messages:
        return this.messageColor;
      case NotificationType.DuplicantThreatening:
        return this.badColor;
      case NotificationType.Event:
        return this.eventColor;
      default:
        return this.normalColor;
    }
  }

  public Color GetNotificationBGColour(NotificationType type)
  {
    switch (type)
    {
      case NotificationType.Bad:
        return this.badColorBG;
      case NotificationType.Tutorial:
        return this.warningColorBG;
      case NotificationType.Messages:
        return this.messageColorBG;
      case NotificationType.DuplicantThreatening:
        return this.badColorBG;
      case NotificationType.Event:
        return this.eventColorBG;
      default:
        return this.normalColorBG;
    }
  }

  public string GetNotificationSound(NotificationType type) => this.notificationSounds[type];

  private class Entry
  {
    public string message;
    public int clickIdx;
    public GameObject label;
    public List<Notification> notifications = new List<Notification>();

    public Entry(GameObject label) => this.label = label;

    public void Add(Notification notification)
    {
      this.notifications.Add(notification);
      this.UpdateMessage(notification);
    }

    public void Remove(Notification notification)
    {
      this.notifications.Remove(notification);
      this.UpdateMessage(notification, false);
    }

    public void UpdateMessage(Notification notification, bool playSound = true)
    {
      if (Game.IsQuitting())
        return;
      this.message = notification.titleText;
      if (this.notifications.Count > 1)
      {
        if (playSound && (notification.Type == NotificationType.Bad || notification.Type == NotificationType.DuplicantThreatening))
          NotificationScreen.Instance.PlayDingSound(notification, this.notifications.Count);
        this.message = this.message + " (" + this.notifications.Count.ToString() + ")";
      }
      if (!Object.op_Inequality((Object) this.label, (Object) null))
        return;
      ((TMP_Text) this.label.GetComponent<HierarchyReferences>().GetReference<LocText>("Text")).text = this.message;
    }

    public Notification NextClickedNotification => this.notifications[this.clickIdx++ % this.notifications.Count];
  }
}
