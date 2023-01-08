// Decompiled with JetBrains decompiler
// Type: LoadingOverlay
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using TMPro;
using UnityEngine;

public class LoadingOverlay : KModalScreen
{
  private bool loadNextFrame;
  private bool showLoad;
  private System.Action loadCb;
  private static LoadingOverlay instance;

  protected override void OnPrefabInit()
  {
    this.pause = false;
    this.fadeIn = false;
    base.OnPrefabInit();
  }

  private void Update()
  {
    if (!this.loadNextFrame && this.showLoad)
    {
      this.loadNextFrame = true;
      this.showLoad = false;
    }
    else
    {
      if (!this.loadNextFrame)
        return;
      this.loadNextFrame = false;
      this.loadCb();
    }
  }

  public static void DestroyInstance() => LoadingOverlay.instance = (LoadingOverlay) null;

  public static void Load(System.Action cb)
  {
    GameObject gameObject = GameObject.Find("/SceneInitializerFE/FrontEndManager");
    if (Object.op_Equality((Object) LoadingOverlay.instance, (Object) null))
    {
      LoadingOverlay.instance = Util.KInstantiateUI<LoadingOverlay>(((Component) ScreenPrefabs.Instance.loadingOverlay).gameObject, Object.op_Equality((Object) GameScreenManager.Instance, (Object) null) ? gameObject : GameScreenManager.Instance.ssOverlayCanvas, false);
      ((TMP_Text) ((Component) LoadingOverlay.instance).GetComponentInChildren<LocText>()).SetText((string) UI.FRONTEND.LOADING);
    }
    if (Object.op_Inequality((Object) GameScreenManager.Instance, (Object) null))
    {
      ((KMonoBehaviour) LoadingOverlay.instance).transform.SetParent(GameScreenManager.Instance.ssOverlayCanvas.transform);
      ((KMonoBehaviour) LoadingOverlay.instance).transform.SetSiblingIndex(GameScreenManager.Instance.ssOverlayCanvas.transform.childCount - 1);
    }
    else
    {
      ((KMonoBehaviour) LoadingOverlay.instance).transform.SetParent(gameObject.transform);
      ((KMonoBehaviour) LoadingOverlay.instance).transform.SetSiblingIndex(gameObject.transform.childCount - 1);
      if (Object.op_Inequality((Object) MainMenu.Instance, (Object) null))
        MainMenu.Instance.StopAmbience();
    }
    LoadingOverlay.instance.loadCb = cb;
    LoadingOverlay.instance.showLoad = true;
    LoadingOverlay.instance.Activate();
  }

  public static void Clear()
  {
    if (!Object.op_Inequality((Object) LoadingOverlay.instance, (Object) null))
      return;
    LoadingOverlay.instance.Deactivate();
  }
}
