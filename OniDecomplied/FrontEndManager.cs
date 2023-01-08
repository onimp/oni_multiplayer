// Decompiled with JetBrains decompiler
// Type: FrontEndManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/FrontEndManager")]
public class FrontEndManager : KMonoBehaviour
{
  public static FrontEndManager Instance;
  public static bool firstInit = true;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    FrontEndManager.Instance = this;
    GameObject gameObject1 = ((Component) this).gameObject;
    switch (DlcManager.GetHighestActiveDlcId())
    {
      case "EXPANSION1_ID":
        Util.KInstantiateUI(ScreenPrefabs.Instance.MainMenuForSpacedOut, gameObject1, true);
        break;
      default:
        Util.KInstantiateUI(ScreenPrefabs.Instance.MainMenuForVanilla, gameObject1, true);
        break;
    }
    if (!FrontEndManager.firstInit)
      return;
    FrontEndManager.firstInit = false;
    GameObject[] gameObjectArray = new GameObject[2]
    {
      ScreenPrefabs.Instance.MainMenuIntroShort,
      ScreenPrefabs.Instance.MainMenuHealthyGameMessage
    };
    foreach (GameObject gameObject2 in gameObjectArray)
      Util.KInstantiateUI(gameObject2, gameObject1, true);
    GameObject[] screensPrefabsToSpawn = new GameObject[3]
    {
      ScreenPrefabs.Instance.KleiItemDropScreen,
      ScreenPrefabs.Instance.LockerMenuScreen,
      ScreenPrefabs.Instance.LockerNavigator
    };
    List<GameObject> gameObjectsToDestroyOnNextCreate = new List<GameObject>();
    CoroutineRunner coroutineRunner = CoroutineRunner.Create();
    Object.DontDestroyOnLoad((Object) coroutineRunner);
    CreateCanvases();
    Singleton<KBatchedAnimUpdater>.Instance.OnClear += new System.Action(RecreateCanvases);

    void CreateCanvases()
    {
      int num = 30;
      foreach (GameObject gameObject1 in screensPrefabsToSpawn)
      {
        GameObject gameObject2 = this.MakeKleiCanvas(((Object) gameObject1).name + " Canvas");
        gameObject2.GetComponent<Canvas>().sortingOrder = num++;
        Util.KInstantiateUI(gameObject1, gameObject2, true);
        Object.DontDestroyOnLoad((Object) gameObject2);
        gameObjectsToDestroyOnNextCreate.Add(gameObject2);
      }
    }
    // ISSUE: variable of a compiler-generated type
    FrontEndManager.\u003C\u003Ec__DisplayClass2_0 cDisplayClass20;

    void RecreateCanvases()
    {
      if (Object.op_Equality((Object) coroutineRunner, (Object) null) || !Object.op_Implicit((Object) coroutineRunner))
        return;
      foreach (Object @object in gameObjectsToDestroyOnNextCreate)
        Object.Destroy(@object);
      gameObjectsToDestroyOnNextCreate.Clear();
      coroutineRunner.StopAllCoroutines();
      // ISSUE: method pointer
      coroutineRunner.Run((IEnumerator) Updater.Series(Updater.WaitOneFrame(), Updater.Do(new System.Action((object) cDisplayClass20, __methodptr(\u003COnPrefabInit\u003Eg__CreateCanvases\u007C0)))));
    }
  }

  protected virtual void OnForcedCleanUp()
  {
    FrontEndManager.Instance = (FrontEndManager) null;
    base.OnForcedCleanUp();
  }

  private void LateUpdate()
  {
    if (Debug.developerConsoleVisible)
      Debug.developerConsoleVisible = false;
    KAnimBatchManager.Instance().UpdateActiveArea(new Vector2I(0, 0), new Vector2I(9999, 9999));
    KAnimBatchManager.Instance().UpdateDirty(Time.frameCount);
    KAnimBatchManager.Instance().Render();
  }

  public GameObject MakeKleiCanvas(string gameObjectName = "Canvas")
  {
    GameObject gameObject = new GameObject(gameObjectName, new System.Type[1]
    {
      typeof (RectTransform)
    });
    this.ConfigureAsKleiCanvas(gameObject);
    return gameObject;
  }

  public void ConfigureAsKleiCanvas(GameObject gameObject)
  {
    Canvas canvas = gameObject.AddOrGet<Canvas>();
    canvas.renderMode = (RenderMode) 0;
    canvas.sortingOrder = 10;
    canvas.pixelPerfect = false;
    canvas.additionalShaderChannels = (AdditionalCanvasShaderChannels) 25;
    GraphicRaycaster graphicRaycaster = gameObject.AddOrGet<GraphicRaycaster>();
    graphicRaycaster.ignoreReversedGraphics = true;
    graphicRaycaster.blockingObjects = (GraphicRaycaster.BlockingObjects) 0;
    graphicRaycaster.blockingMask = LayerMask.op_Implicit(-1);
    CanvasScaler canvasScaler = gameObject.AddOrGet<CanvasScaler>();
    canvasScaler.uiScaleMode = (CanvasScaler.ScaleMode) 0;
    canvasScaler.referencePixelsPerUnit = 100f;
    gameObject.AddOrGet<KCanvasScaler>();
  }
}
