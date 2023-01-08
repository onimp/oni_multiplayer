// Decompiled with JetBrains decompiler
// Type: ScheduledUIInstantiation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ScheduledUIInstantiation")]
public class ScheduledUIInstantiation : KMonoBehaviour
{
  public ScheduledUIInstantiation.Instantiation[] UIElements;
  public bool InstantiateOnAwake;
  public GameHashes InstantiationEvent = GameHashes.StartGameUser;
  private bool completed;
  private List<GameObject> instantiatedObjects = new List<GameObject>();

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    if (this.InstantiateOnAwake)
      this.InstantiateElements((object) null);
    else
      Game.Instance.Subscribe((int) this.InstantiationEvent, new Action<object>(this.InstantiateElements));
  }

  public void InstantiateElements(object data)
  {
    if (this.completed)
      return;
    this.completed = true;
    foreach (ScheduledUIInstantiation.Instantiation uiElement in this.UIElements)
    {
      foreach (GameObject prefab in uiElement.prefabs)
      {
        if (DlcManager.IsContentActive(uiElement.RequiredDlcId))
        {
          Vector3 vector3 = Vector2.op_Implicit(Util.rectTransform(prefab).anchoredPosition);
          GameObject gameObject = Util.KInstantiateUI(prefab, ((Component) uiElement.parent).gameObject, false);
          Util.rectTransform(gameObject).anchoredPosition = Vector2.op_Implicit(vector3);
          ((Transform) Util.rectTransform(gameObject)).localScale = Vector3.one;
          this.instantiatedObjects.Add(gameObject);
        }
      }
    }
    if (this.InstantiateOnAwake)
      return;
    this.Unsubscribe((int) this.InstantiationEvent, new Action<object>(this.InstantiateElements));
  }

  public T GetInstantiatedObject<T>() where T : Component
  {
    for (int index = 0; index < this.instantiatedObjects.Count; ++index)
    {
      if (Object.op_Inequality((Object) this.instantiatedObjects[index].GetComponent(typeof (T)), (Object) null))
        return this.instantiatedObjects[index].GetComponent(typeof (T)) as T;
    }
    return default (T);
  }

  [Serializable]
  public struct Instantiation
  {
    public string Name;
    public string Comment;
    public GameObject[] prefabs;
    public Transform parent;
    public string RequiredDlcId;
  }
}
