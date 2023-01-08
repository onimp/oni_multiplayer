// Decompiled with JetBrains decompiler
// Type: InstantiateUIPrefabChild
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/InstantiateUIPrefabChild")]
public class InstantiateUIPrefabChild : KMonoBehaviour
{
  public GameObject[] prefabs;
  public bool InstantiateOnAwake = true;
  private bool alreadyInstantiated;
  public bool setAsFirstSibling;

  protected virtual void OnPrefabInit()
  {
    if (!this.InstantiateOnAwake)
      return;
    this.Instantiate();
  }

  public void Instantiate()
  {
    if (this.alreadyInstantiated)
    {
      Debug.LogWarning((object) (((Object) ((Component) this).gameObject).name + "trying to instantiate UI prefabs multiple times."));
    }
    else
    {
      this.alreadyInstantiated = true;
      foreach (GameObject prefab in this.prefabs)
      {
        if (!Object.op_Equality((Object) prefab, (Object) null))
        {
          Vector3 vector3 = Vector2.op_Implicit(Util.rectTransform(prefab).anchoredPosition);
          GameObject gameObject = Object.Instantiate<GameObject>(prefab);
          gameObject.transform.SetParent(this.transform);
          Util.rectTransform(gameObject).anchoredPosition = Vector2.op_Implicit(vector3);
          ((Transform) Util.rectTransform(gameObject)).localScale = Vector3.one;
          if (this.setAsFirstSibling)
            gameObject.transform.SetAsFirstSibling();
        }
      }
    }
  }
}
