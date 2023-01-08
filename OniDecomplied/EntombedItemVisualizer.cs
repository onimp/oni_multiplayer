// Decompiled with JetBrains decompiler
// Type: EntombedItemVisualizer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/EntombedItemVisualizer")]
public class EntombedItemVisualizer : KMonoBehaviour
{
  [SerializeField]
  private GameObject entombedItemPrefab;
  private static readonly string[] EntombedVisualizerAnims = new string[4]
  {
    "idle1",
    "idle2",
    "idle3",
    "idle4"
  };
  private GameObjectPool entombedItemPool;
  private Dictionary<int, EntombedItemVisualizer.Data> cellEntombedCounts = new Dictionary<int, EntombedItemVisualizer.Data>();

  public void Clear() => this.cellEntombedCounts.Clear();

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.entombedItemPool = new GameObjectPool(new Func<GameObject>(this.InstantiateEntombedObject), 32);
  }

  public bool AddItem(int cell)
  {
    bool flag = false;
    if (Object.op_Equality((Object) Grid.Objects[cell, 9], (Object) null))
    {
      flag = true;
      EntombedItemVisualizer.Data data;
      this.cellEntombedCounts.TryGetValue(cell, out data);
      if (data.refCount == 0)
      {
        GameObject instance = this.entombedItemPool.GetInstance();
        TransformExtensions.SetPosition(instance.transform, Grid.CellToPosCCC(cell, Grid.SceneLayer.FXFront));
        instance.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Random.value * 360f);
        KBatchedAnimController component = instance.GetComponent<KBatchedAnimController>();
        int index = Random.Range(0, EntombedItemVisualizer.EntombedVisualizerAnims.Length);
        string entombedVisualizerAnim = EntombedItemVisualizer.EntombedVisualizerAnims[index];
        component.initialAnim = entombedVisualizerAnim;
        instance.SetActive(true);
        component.Play(HashedString.op_Implicit(entombedVisualizerAnim));
        data.controller = component;
      }
      ++data.refCount;
      this.cellEntombedCounts[cell] = data;
    }
    return flag;
  }

  public void RemoveItem(int cell)
  {
    EntombedItemVisualizer.Data data;
    if (!this.cellEntombedCounts.TryGetValue(cell, out data))
      return;
    --data.refCount;
    if (data.refCount == 0)
      this.ReleaseVisualizer(cell, data);
    else
      this.cellEntombedCounts[cell] = data;
  }

  public void ForceClear(int cell)
  {
    EntombedItemVisualizer.Data data;
    if (!this.cellEntombedCounts.TryGetValue(cell, out data))
      return;
    this.ReleaseVisualizer(cell, data);
  }

  private void ReleaseVisualizer(int cell, EntombedItemVisualizer.Data data)
  {
    if (Object.op_Inequality((Object) data.controller, (Object) null))
    {
      ((Component) data.controller).gameObject.SetActive(false);
      this.entombedItemPool.ReleaseInstance(((Component) data.controller).gameObject);
    }
    this.cellEntombedCounts.Remove(cell);
  }

  public bool IsEntombedItem(int cell) => this.cellEntombedCounts.ContainsKey(cell) && this.cellEntombedCounts[cell].refCount > 0;

  private GameObject InstantiateEntombedObject()
  {
    GameObject gameObject = GameUtil.KInstantiate(this.entombedItemPrefab, Grid.SceneLayer.FXFront);
    gameObject.SetActive(false);
    return gameObject;
  }

  private struct Data
  {
    public int refCount;
    public KBatchedAnimController controller;
  }
}
