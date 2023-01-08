// Decompiled with JetBrains decompiler
// Type: SideDetailsScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SideDetailsScreen : KScreen
{
  [SerializeField]
  private List<SideTargetScreen> screens;
  [SerializeField]
  private LocText title;
  [SerializeField]
  private KButton backButton;
  [SerializeField]
  private RectTransform body;
  private RectTransform rectTransform;
  private Dictionary<string, SideTargetScreen> screenMap;
  private SideTargetScreen activeScreen;
  public static SideDetailsScreen Instance;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    SideDetailsScreen.Instance = this;
    this.Initialize();
    ((Component) this).gameObject.SetActive(false);
  }

  protected virtual void OnForcedCleanUp()
  {
    SideDetailsScreen.Instance = (SideDetailsScreen) null;
    ((KMonoBehaviour) this).OnForcedCleanUp();
  }

  private void Initialize()
  {
    if (this.screens == null)
      return;
    this.rectTransform = ((Component) this).GetComponent<RectTransform>();
    this.screenMap = new Dictionary<string, SideTargetScreen>();
    List<SideTargetScreen> sideTargetScreenList = new List<SideTargetScreen>();
    foreach (Component screen in this.screens)
    {
      SideTargetScreen sideTargetScreen = Util.KInstantiateUI<SideTargetScreen>(screen.gameObject, ((Component) this.body).gameObject, false);
      ((Component) sideTargetScreen).gameObject.SetActive(false);
      sideTargetScreenList.Add(sideTargetScreen);
    }
    sideTargetScreenList.ForEach((Action<SideTargetScreen>) (s => this.screenMap.Add(((Object) s).name, s)));
    this.backButton.onClick += (System.Action) (() => this.Show(false));
  }

  public void SetTitle(string newTitle) => ((TMP_Text) this.title).text = newTitle;

  public void SetScreen(string screenName, object content, float x)
  {
    if (!this.screenMap.ContainsKey(screenName))
      Debug.LogError((object) "Tried to open a screen that does exist on the manager!");
    else if (content == null)
    {
      Debug.LogError((object) ("Tried to set " + screenName + " with null content!"));
    }
    else
    {
      if (!((Component) this).gameObject.activeInHierarchy)
        ((Component) this).gameObject.SetActive(true);
      Rect rect = this.rectTransform.rect;
      this.rectTransform.offsetMin = new Vector2(x, this.rectTransform.offsetMin.y);
      this.rectTransform.offsetMax = new Vector2(x + ((Rect) ref rect).width, this.rectTransform.offsetMax.y);
      if (Object.op_Inequality((Object) this.activeScreen, (Object) null))
        ((Component) this.activeScreen).gameObject.SetActive(false);
      this.activeScreen = this.screenMap[screenName];
      ((Component) this.activeScreen).gameObject.SetActive(true);
      this.SetTitle(this.activeScreen.displayName);
      this.activeScreen.SetTarget(content);
    }
  }
}
