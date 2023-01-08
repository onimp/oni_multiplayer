// Decompiled with JetBrains decompiler
// Type: BundledAssetsLoader
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class BundledAssetsLoader : KMonoBehaviour
{
  public static BundledAssetsLoader instance;

  public BundledAssets Expansion1Assets { get; private set; }

  protected virtual void OnPrefabInit()
  {
    BundledAssetsLoader.instance = this;
    Debug.Log((object) ("Expansion1: " + DlcManager.IsExpansion1Active().ToString()));
    if (!DlcManager.IsExpansion1Active())
      return;
    Debug.Log((object) "Loading Expansion1 assets from bundle");
    AssetBundle assetBundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(Application.streamingAssetsPath, DlcManager.GetContentBundleName("EXPANSION1_ID")));
    Debug.Assert(Object.op_Inequality((Object) assetBundle, (Object) null), (object) "Expansion1 is Active but its asset bundle failed to load");
    GameObject gameObject = assetBundle.LoadAsset<GameObject>("Expansion1Assets");
    Debug.Assert(Object.op_Inequality((Object) gameObject, (Object) null), (object) "Could not load the Expansion1Assets prefab");
    this.Expansion1Assets = Util.KInstantiate(gameObject, ((Component) this).gameObject, (string) null).GetComponent<BundledAssets>();
  }
}
