// Decompiled with JetBrains decompiler
// Type: SymbolOverrideController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SymbolOverrideController")]
public class SymbolOverrideController : KMonoBehaviour
{
  public bool applySymbolOverridesEveryFrame;
  [SerializeField]
  private List<SymbolOverrideController.SymbolEntry> symbolOverrides = new List<SymbolOverrideController.SymbolEntry>();
  private KAnimBatch.AtlasList atlases;
  private KBatchedAnimController animController;
  private FaceGraph faceGraph;
  private bool requiresSorting;

  public SymbolOverrideController.SymbolEntry[] GetSymbolOverrides => this.symbolOverrides.ToArray();

  public int version { get; private set; }

  protected virtual void OnPrefabInit()
  {
    this.animController = ((Component) this).GetComponent<KBatchedAnimController>();
    DebugUtil.Assert(Object.op_Inequality((Object) ((Component) this).GetComponent<KBatchedAnimController>(), (Object) null), "SymbolOverrideController requires KBatchedAnimController");
    DebugUtil.Assert(((Component) this).GetComponent<KBatchedAnimController>().usingNewSymbolOverrideSystem, "SymbolOverrideController requires usingNewSymbolOverrideSystem to be set to true. Try adding the component by calling: SymbolOverrideControllerUtil.AddToPrefab");
    for (int index = 0; index < this.symbolOverrides.Count; ++index)
    {
      SymbolOverrideController.SymbolEntry symbolOverride = this.symbolOverrides[index];
      symbolOverride.sourceSymbol = KAnimBatchManager.Instance().GetBatchGroupData(symbolOverride.sourceSymbolBatchTag).GetSymbol(KAnimHashedString.op_Implicit(symbolOverride.sourceSymbolId));
      this.symbolOverrides[index] = symbolOverride;
    }
    this.atlases = new KAnimBatch.AtlasList(0, KAnimBatchManager.MaxAtlasesByMaterialType[this.animController.materialType]);
    this.faceGraph = ((Component) this).GetComponent<FaceGraph>();
  }

  public int AddSymbolOverride(
    HashedString target_symbol,
    KAnim.Build.Symbol source_symbol,
    int priority = 0)
  {
    if (source_symbol == null)
      throw new Exception("NULL source symbol when overriding: " + target_symbol.ToString());
    SymbolOverrideController.SymbolEntry symbolEntry = new SymbolOverrideController.SymbolEntry()
    {
      targetSymbol = target_symbol,
      sourceSymbol = source_symbol,
      sourceSymbolId = new HashedString(((KAnimHashedString) ref source_symbol.hash).HashValue),
      sourceSymbolBatchTag = source_symbol.build.batchTag,
      priority = priority
    };
    int index = this.GetSymbolOverrideIdx(target_symbol, priority);
    if (index >= 0)
    {
      this.symbolOverrides[index] = symbolEntry;
    }
    else
    {
      index = this.symbolOverrides.Count;
      this.symbolOverrides.Add(symbolEntry);
    }
    this.MarkDirty();
    return index;
  }

  public bool RemoveSymbolOverride(HashedString target_symbol, int priority = 0)
  {
    for (int index = 0; index < this.symbolOverrides.Count; ++index)
    {
      SymbolOverrideController.SymbolEntry symbolOverride = this.symbolOverrides[index];
      if (HashedString.op_Equality(symbolOverride.targetSymbol, target_symbol) && symbolOverride.priority == priority)
      {
        this.symbolOverrides.RemoveAt(index);
        return true;
      }
    }
    this.MarkDirty();
    return false;
  }

  public void RemoveAllSymbolOverrides(int priority = 0)
  {
    this.symbolOverrides.RemoveAll((Predicate<SymbolOverrideController.SymbolEntry>) (x => x.priority >= priority));
    this.MarkDirty();
  }

  public int GetSymbolOverrideIdx(HashedString target_symbol, int priority = 0)
  {
    for (int index = 0; index < this.symbolOverrides.Count; ++index)
    {
      SymbolOverrideController.SymbolEntry symbolOverride = this.symbolOverrides[index];
      if (HashedString.op_Equality(symbolOverride.targetSymbol, target_symbol) && symbolOverride.priority == priority)
        return index;
    }
    return -1;
  }

  public int GetAtlasIdx(Texture2D atlas) => this.atlases.GetAtlasIdx(atlas);

  public void ApplyOverrides()
  {
    if (this.requiresSorting)
    {
      this.symbolOverrides.Sort((Comparison<SymbolOverrideController.SymbolEntry>) ((x, y) => x.priority - y.priority));
      this.requiresSorting = false;
    }
    KAnimBatch batch = this.animController.GetBatch();
    DebugUtil.Assert(batch != null);
    KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(this.animController.batchGroupID);
    this.atlases.Clear(batch.atlases.Count);
    DictionaryPool<HashedString, Pair<int, int>, SymbolOverrideController>.PooledDictionary pooledDictionary1 = DictionaryPool<HashedString, Pair<int, int>, SymbolOverrideController>.Allocate();
    ListPool<SymbolOverrideController.SymbolEntry, SymbolOverrideController>.PooledList pooledList = ListPool<SymbolOverrideController.SymbolEntry, SymbolOverrideController>.Allocate();
    for (int index = 0; index < this.symbolOverrides.Count; ++index)
    {
      SymbolOverrideController.SymbolEntry symbolOverride = this.symbolOverrides[index];
      Pair<int, int> pair;
      if (((Dictionary<HashedString, Pair<int, int>>) pooledDictionary1).TryGetValue(symbolOverride.targetSymbol, out pair))
      {
        int first = pair.first;
        if (symbolOverride.priority > first)
        {
          int second = pair.second;
          ((Dictionary<HashedString, Pair<int, int>>) pooledDictionary1)[symbolOverride.targetSymbol] = new Pair<int, int>(symbolOverride.priority, second);
          ((List<SymbolOverrideController.SymbolEntry>) pooledList)[second] = symbolOverride;
        }
      }
      else
      {
        ((Dictionary<HashedString, Pair<int, int>>) pooledDictionary1)[symbolOverride.targetSymbol] = new Pair<int, int>(symbolOverride.priority, ((List<SymbolOverrideController.SymbolEntry>) pooledList).Count);
        ((List<SymbolOverrideController.SymbolEntry>) pooledList).Add(symbolOverride);
      }
    }
    DictionaryPool<KAnim.Build, SymbolOverrideController.BatchGroupInfo, SymbolOverrideController>.PooledDictionary pooledDictionary2 = DictionaryPool<KAnim.Build, SymbolOverrideController.BatchGroupInfo, SymbolOverrideController>.Allocate();
    for (int index = 0; index < ((List<SymbolOverrideController.SymbolEntry>) pooledList).Count; ++index)
    {
      SymbolOverrideController.SymbolEntry symbolEntry = ((List<SymbolOverrideController.SymbolEntry>) pooledList)[index];
      SymbolOverrideController.BatchGroupInfo batchGroupInfo;
      if (!((Dictionary<KAnim.Build, SymbolOverrideController.BatchGroupInfo>) pooledDictionary2).TryGetValue(symbolEntry.sourceSymbol.build, out batchGroupInfo))
      {
        batchGroupInfo = new SymbolOverrideController.BatchGroupInfo()
        {
          build = symbolEntry.sourceSymbol.build,
          data = KAnimBatchManager.Instance().GetBatchGroupData(symbolEntry.sourceSymbol.build.batchTag)
        };
        Texture2D texture = symbolEntry.sourceSymbol.build.GetTexture(0);
        int num = batch.atlases.GetAtlasIdx(texture);
        if (num < 0)
          num = this.atlases.Add(texture);
        batchGroupInfo.atlasIdx = num;
        ((Dictionary<KAnim.Build, SymbolOverrideController.BatchGroupInfo>) pooledDictionary2)[batchGroupInfo.build] = batchGroupInfo;
      }
      KAnim.Build.Symbol symbol = batchGroupData.GetSymbol(KAnimHashedString.op_Implicit(symbolEntry.targetSymbol));
      if (symbol != null)
        this.animController.SetSymbolOverrides(symbol.firstFrameIdx, symbol.numFrames, batchGroupInfo.atlasIdx, batchGroupInfo.data, symbolEntry.sourceSymbol.firstFrameIdx, symbolEntry.sourceSymbol.numFrames);
    }
    pooledDictionary2.Recycle();
    pooledList.Recycle();
    pooledDictionary1.Recycle();
    if (!Object.op_Inequality((Object) this.faceGraph, (Object) null))
      return;
    this.faceGraph.ApplyShape();
  }

  public void ApplyAtlases() => this.atlases.Apply(this.animController.GetBatch().matProperties);

  public KAnimBatch.AtlasList GetAtlasList() => this.atlases;

  public void MarkDirty()
  {
    if (Object.op_Inequality((Object) this.animController, (Object) null))
      this.animController.SetDirty();
    ++this.version;
    this.requiresSorting = true;
  }

  [Serializable]
  public struct SymbolEntry
  {
    public HashedString targetSymbol;
    [NonSerialized]
    public KAnim.Build.Symbol sourceSymbol;
    public HashedString sourceSymbolId;
    public HashedString sourceSymbolBatchTag;
    public int priority;
  }

  private struct SymbolToOverride
  {
    public KAnim.Build.Symbol sourceSymbol;
    public HashedString targetSymbol;
    public KBatchGroupData data;
    public int atlasIdx;
  }

  private class BatchGroupInfo
  {
    public KAnim.Build build;
    public int atlasIdx;
    public KBatchGroupData data;
  }
}
