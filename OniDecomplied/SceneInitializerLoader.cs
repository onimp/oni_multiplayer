// Decompiled with JetBrains decompiler
// Type: SceneInitializerLoader
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class SceneInitializerLoader : MonoBehaviour
{
  public SceneInitializer sceneInitializer;
  public static SceneInitializerLoader.DeferredError deferred_error;
  public static SceneInitializerLoader.DeferredErrorDelegate ReportDeferredError;

  private void Awake()
  {
    foreach (Behaviour behaviour in Object.FindObjectsOfType<Camera>())
      behaviour.enabled = false;
    KMonoBehaviour.isLoadingScene = false;
    Singleton<StateMachineManager>.Instance.Clear();
    Util.KInstantiate((Component) this.sceneInitializer, (GameObject) null, (string) null);
    if (SceneInitializerLoader.ReportDeferredError == null || !SceneInitializerLoader.deferred_error.IsValid)
      return;
    SceneInitializerLoader.ReportDeferredError(SceneInitializerLoader.deferred_error);
    SceneInitializerLoader.deferred_error = new SceneInitializerLoader.DeferredError();
  }

  public struct DeferredError
  {
    public string msg;
    public string stack_trace;

    public bool IsValid => !string.IsNullOrEmpty(this.msg);
  }

  public delegate void DeferredErrorDelegate(
    SceneInitializerLoader.DeferredError deferred_error);
}
