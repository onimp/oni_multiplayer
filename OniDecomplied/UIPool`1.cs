// Decompiled with JetBrains decompiler
// Type: UIPool`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class UIPool<T> where T : MonoBehaviour
{
  private T prefab;
  private List<T> freeElements = new List<T>();
  private List<T> activeElements = new List<T>();
  public Transform disabledElementParent;

  public int ActiveElementsCount => this.activeElements.Count;

  public int FreeElementsCount => this.freeElements.Count;

  public int TotalElementsCount => this.ActiveElementsCount + this.FreeElementsCount;

  public UIPool(T prefab)
  {
    this.prefab = prefab;
    this.freeElements = new List<T>();
    this.activeElements = new List<T>();
  }

  public T GetFreeElement(GameObject instantiateParent = null, bool forceActive = false)
  {
    if (this.freeElements.Count == 0)
    {
      this.activeElements.Add(Util.KInstantiateUI<T>(((Component) (object) this.prefab).gameObject, instantiateParent, false));
    }
    else
    {
      T freeElement = this.freeElements[0];
      this.activeElements.Add(freeElement);
      if (Object.op_Inequality((Object) ((Component) (object) freeElement).transform.parent, (Object) instantiateParent))
        ((Component) (object) freeElement).transform.SetParent(instantiateParent.transform);
      this.freeElements.RemoveAt(0);
    }
    T activeElement = this.activeElements[this.activeElements.Count - 1];
    if (((Component) (object) activeElement).gameObject.activeInHierarchy != forceActive)
      ((Component) (object) activeElement).gameObject.SetActive(forceActive);
    return activeElement;
  }

  public void ClearElement(T element)
  {
    if (!this.activeElements.Contains(element))
    {
      Debug.LogError(this.freeElements.Contains(element) ? (object) "The element provided is already inactive" : (object) "The element provided does not belong to this pool");
    }
    else
    {
      if (Object.op_Inequality((Object) this.disabledElementParent, (Object) null))
        ((Component) (object) element).gameObject.transform.SetParent(this.disabledElementParent);
      ((Component) (object) element).gameObject.SetActive(false);
      this.freeElements.Add(element);
      this.activeElements.Remove(element);
    }
  }

  public void ClearAll()
  {
    while (this.activeElements.Count > 0)
    {
      if (Object.op_Inequality((Object) this.disabledElementParent, (Object) null))
        ((Component) (object) this.activeElements[0]).gameObject.transform.SetParent(this.disabledElementParent);
      ((Component) (object) this.activeElements[0]).gameObject.SetActive(false);
      this.freeElements.Add(this.activeElements[0]);
      this.activeElements.RemoveAt(0);
    }
  }

  public void DestroyAll()
  {
    this.DestroyAllActive();
    this.DestroyAllFree();
  }

  public void DestroyAllActive()
  {
    this.activeElements.ForEach((Action<T>) (ae => Object.Destroy((Object) ((Component) (object) ae).gameObject)));
    this.activeElements.Clear();
  }

  public void DestroyAllFree()
  {
    this.freeElements.ForEach((Action<T>) (ae => Object.Destroy((Object) ((Component) (object) ae).gameObject)));
    this.freeElements.Clear();
  }

  public void ForEachActiveElement(Action<T> predicate) => this.activeElements.ForEach(predicate);

  public void ForEachFreeElement(Action<T> predicate) => this.freeElements.ForEach(predicate);
}
