// Decompiled with JetBrains decompiler
// Type: SceneInitializer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneInitializer : MonoBehaviour
{
  public const int MAXDEPTH = -30000;
  public const int SCREENDEPTH = -1000;
  public GameObject prefab_NewSaveGame;
  public List<GameObject> preloadPrefabs = new List<GameObject>();
  public List<GameObject> prefabs = new List<GameObject>();

  public static SceneInitializer Instance { get; private set; }

  private void Awake()
  {
    Localization.SwapToLocalizedFont();
    string environmentVariable = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
    string str = Application.dataPath + System.IO.Path.DirectorySeparatorChar.ToString() + "Plugins";
    if (!environmentVariable.Contains(str))
      Environment.SetEnvironmentVariable("PATH", environmentVariable + System.IO.Path.PathSeparator.ToString() + str, EnvironmentVariableTarget.Process);
    SceneInitializer.Instance = this;
    this.PreLoadPrefabs();
  }

  private void OnDestroy() => SceneInitializer.Instance = (SceneInitializer) null;

  private void PreLoadPrefabs()
  {
    foreach (GameObject preloadPrefab in this.preloadPrefabs)
    {
      if (Object.op_Inequality((Object) preloadPrefab, (Object) null))
        Util.KInstantiate(preloadPrefab, TransformExtensions.GetPosition(preloadPrefab.transform), Quaternion.identity, ((Component) this).gameObject, (string) null, true, 0);
    }
  }

  public void NewSaveGamePrefab()
  {
    if (!Object.op_Inequality((Object) this.prefab_NewSaveGame, (Object) null) || !Object.op_Equality((Object) SaveGame.Instance, (Object) null))
      return;
    Util.KInstantiate(this.prefab_NewSaveGame, ((Component) this).gameObject, (string) null);
  }

  public void PostLoadPrefabs()
  {
    foreach (GameObject prefab in this.prefabs)
    {
      if (Object.op_Inequality((Object) prefab, (Object) null))
        Util.KInstantiate(prefab, ((Component) this).gameObject, (string) null);
    }
  }
}
