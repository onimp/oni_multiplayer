// Decompiled with JetBrains decompiler
// Type: UIPrefabLocalPool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIPrefabLocalPool
{
  public readonly GameObject sourcePrefab;
  public readonly GameObject parent;
  private Dictionary<int, GameObject> checkedOutInstances = new Dictionary<int, GameObject>();
  private Dictionary<int, GameObject> availableInstances = new Dictionary<int, GameObject>();

  public UIPrefabLocalPool(GameObject sourcePrefab, GameObject parent)
  {
    this.sourcePrefab = sourcePrefab;
    this.parent = parent;
  }

  public GameObject Borrow()
  {
    GameObject gameObject;
    if (this.availableInstances.Count == 0)
    {
      gameObject = Util.KInstantiateUI(this.sourcePrefab, this.parent, true);
    }
    else
    {
      gameObject = ((IEnumerable<KeyValuePair<int, GameObject>>) this.availableInstances).First<KeyValuePair<int, GameObject>>().Value;
      this.availableInstances.Remove(((Object) gameObject).GetInstanceID());
    }
    this.checkedOutInstances.Add(((Object) gameObject).GetInstanceID(), gameObject);
    gameObject.SetActive(true);
    gameObject.transform.SetAsLastSibling();
    return gameObject;
  }

  public void Return(GameObject instance)
  {
    this.checkedOutInstances.Remove(((Object) instance).GetInstanceID());
    this.availableInstances.Add(((Object) instance).GetInstanceID(), instance);
    instance.SetActive(false);
  }

  public void ReturnAll()
  {
    foreach (KeyValuePair<int, GameObject> checkedOutInstance in this.checkedOutInstances)
    {
      int num;
      GameObject gameObject1;
      Util.Deconstruct<int, GameObject>(checkedOutInstance, ref num, ref gameObject1);
      int key = num;
      GameObject gameObject2 = gameObject1;
      this.availableInstances.Add(key, gameObject2);
      gameObject2.SetActive(false);
    }
    this.checkedOutInstances.Clear();
  }

  public IEnumerable<GameObject> GetBorrowedObjects() => (IEnumerable<GameObject>) this.checkedOutInstances.Values;
}
