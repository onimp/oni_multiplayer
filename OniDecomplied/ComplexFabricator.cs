// Decompiled with JetBrains decompiler
// Type: ComplexFabricator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/ComplexFabricator")]
public class ComplexFabricator : KMonoBehaviour, ISim200ms, ISim1000ms
{
  private const int MaxPrefetchCount = 2;
  public bool duplicantOperated = true;
  protected ComplexFabricatorWorkable workable;
  [SerializeField]
  public HashedString fetchChoreTypeIdHash = Db.Get().ChoreTypes.FabricateFetch.IdHash;
  [SerializeField]
  public float heatedTemperature;
  [SerializeField]
  public bool storeProduced;
  public ComplexFabricatorSideScreen.StyleSetting sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
  public bool labelByResult = true;
  public Vector3 outputOffset = Vector3.zero;
  public ChoreType choreType;
  public bool keepExcessLiquids;
  public Tag keepAdditionalTag = Tag.Invalid;
  public static int MAX_QUEUE_SIZE = 99;
  public static int QUEUE_INFINITE = -1;
  [Serialize]
  private Dictionary<string, int> recipeQueueCounts = new Dictionary<string, int>();
  private int nextOrderIdx;
  private bool nextOrderIsWorkable;
  private int workingOrderIdx = -1;
  [Serialize]
  private string lastWorkingRecipe;
  [Serialize]
  private float orderProgress;
  private List<int> openOrderCounts = new List<int>();
  [Serialize]
  private bool forbidMutantSeeds;
  private Tag[] forbiddenMutantTags = new Tag[1]
  {
    GameTags.MutatedSeed
  };
  private bool queueDirty = true;
  private bool hasOpenOrders;
  private List<FetchList2> fetchListList = new List<FetchList2>();
  private Chore chore;
  private bool cancelling;
  private ComplexRecipe[] recipe_list;
  private Dictionary<Tag, float> materialNeedCache = new Dictionary<Tag, float>();
  [SerializeField]
  public Storage inStorage;
  [SerializeField]
  public Storage buildStorage;
  [SerializeField]
  public Storage outStorage;
  [MyCmpAdd]
  private LoopingSounds loopingSounds;
  [MyCmpReq]
  protected Operational operational;
  [MyCmpAdd]
  private ComplexFabricatorSM fabricatorSM;
  private ProgressBar progressBar;
  private static readonly EventSystem.IntraObjectHandler<ComplexFabricator> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<ComplexFabricator>((Action<ComplexFabricator, object>) ((component, data) => component.OnStorageChange(data)));
  private static readonly EventSystem.IntraObjectHandler<ComplexFabricator> OnParticleStorageChangedDelegate = new EventSystem.IntraObjectHandler<ComplexFabricator>((Action<ComplexFabricator, object>) ((component, data) => component.OnStorageChange(data)));
  private static readonly EventSystem.IntraObjectHandler<ComplexFabricator> OnDroppedAllDelegate = new EventSystem.IntraObjectHandler<ComplexFabricator>((Action<ComplexFabricator, object>) ((component, data) => component.OnDroppedAll(data)));
  private static readonly EventSystem.IntraObjectHandler<ComplexFabricator> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<ComplexFabricator>((Action<ComplexFabricator, object>) ((component, data) => component.OnOperationalChanged(data)));
  private static readonly EventSystem.IntraObjectHandler<ComplexFabricator> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<ComplexFabricator>((Action<ComplexFabricator, object>) ((component, data) => component.OnCopySettings(data)));
  private static readonly EventSystem.IntraObjectHandler<ComplexFabricator> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<ComplexFabricator>((Action<ComplexFabricator, object>) ((component, data) => component.OnRefreshUserMenu(data)));

  public ComplexFabricatorWorkable Workable => this.workable;

  public bool ForbidMutantSeeds
  {
    get => this.forbidMutantSeeds;
    set
    {
      this.forbidMutantSeeds = value;
      this.ToggleMutantSeedFetches();
      this.UpdateMutantSeedStatusItem();
    }
  }

  public Tag[] ForbiddenTags => !this.forbidMutantSeeds ? (Tag[]) null : this.forbiddenMutantTags;

  public int CurrentOrderIdx => this.nextOrderIdx;

  public ComplexRecipe CurrentWorkingOrder => !this.HasWorkingOrder ? (ComplexRecipe) null : this.recipe_list[this.workingOrderIdx];

  public ComplexRecipe NextOrder => !this.nextOrderIsWorkable ? (ComplexRecipe) null : this.recipe_list[this.nextOrderIdx];

  public float OrderProgress
  {
    get => this.orderProgress;
    set => this.orderProgress = value;
  }

  public bool HasAnyOrder => this.HasWorkingOrder || this.hasOpenOrders;

  public bool HasWorker => !this.duplicantOperated || Object.op_Inequality((Object) this.workable.worker, (Object) null);

  public bool WaitingForWorker => this.HasWorkingOrder && !this.HasWorker;

  private bool HasWorkingOrder => this.workingOrderIdx > -1;

  public List<FetchList2> DebugFetchLists => this.fetchListList;

  [OnDeserialized]
  protected virtual void OnDeserializedMethod()
  {
    List<string> stringList = new List<string>();
    foreach (string key in this.recipeQueueCounts.Keys)
    {
      if (ComplexRecipeManager.Get().GetRecipe(key) == null)
        stringList.Add(key);
    }
    foreach (string key in stringList)
    {
      Debug.LogWarningFormat("{1} removing missing recipe from queue: {0}", new object[2]
      {
        (object) key,
        (object) ((Object) this).name
      });
      this.recipeQueueCounts.Remove(key);
    }
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.GetRecipes();
    this.simRenderLoadBalance = true;
    this.choreType = Db.Get().ChoreTypes.Fabricate;
    this.Subscribe<ComplexFabricator>(-1957399615, ComplexFabricator.OnDroppedAllDelegate);
    this.Subscribe<ComplexFabricator>(-592767678, ComplexFabricator.OnOperationalChangedDelegate);
    this.Subscribe<ComplexFabricator>(-905833192, ComplexFabricator.OnCopySettingsDelegate);
    this.Subscribe<ComplexFabricator>(-1697596308, ComplexFabricator.OnStorageChangeDelegate);
    this.Subscribe<ComplexFabricator>(-1837862626, ComplexFabricator.OnParticleStorageChangedDelegate);
    this.workable = ((Component) this).GetComponent<ComplexFabricatorWorkable>();
    Components.ComplexFabricators.Add(this);
    this.Subscribe<ComplexFabricator>(493375141, ComplexFabricator.OnRefreshUserMenuDelegate);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.InitRecipeQueueCount();
    foreach (string key in this.recipeQueueCounts.Keys)
    {
      if (this.recipeQueueCounts[key] == 100)
        this.recipeQueueCounts[key] = ComplexFabricator.QUEUE_INFINITE;
    }
    this.buildStorage.Transfer(this.inStorage, true, true);
    this.DropExcessIngredients(this.inStorage);
    int recipeIndex = this.FindRecipeIndex(this.lastWorkingRecipe);
    if (recipeIndex > -1)
      this.nextOrderIdx = recipeIndex;
    this.UpdateMutantSeedStatusItem();
  }

  protected virtual void OnCleanUp()
  {
    this.CancelAllOpenOrders();
    this.CancelChore();
    Components.ComplexFabricators.Remove(this);
    base.OnCleanUp();
  }

  private void OnRefreshUserMenu(object data)
  {
    if (!DlcManager.IsContentActive("EXPANSION1_ID") || !this.HasRecipiesWithSeeds())
      return;
    Game.Instance.userMenu.AddButton(((Component) this).gameObject, new KIconButtonMenu.ButtonInfo("action_switch_toggle", (string) (this.ForbidMutantSeeds ? UI.USERMENUACTIONS.ACCEPT_MUTANT_SEEDS.ACCEPT : UI.USERMENUACTIONS.ACCEPT_MUTANT_SEEDS.REJECT), (System.Action) (() =>
    {
      this.ForbidMutantSeeds = !this.ForbidMutantSeeds;
      this.OnRefreshUserMenu((object) null);
    }), tooltipText: ((string) UI.USERMENUACTIONS.ACCEPT_MUTANT_SEEDS.TOOLTIP)));
  }

  private bool HasRecipiesWithSeeds()
  {
    bool flag = false;
    foreach (ComplexRecipe recipe in this.recipe_list)
    {
      foreach (ComplexRecipe.RecipeElement ingredient in recipe.ingredients)
      {
        GameObject prefab = Assets.GetPrefab(ingredient.material);
        if (Object.op_Inequality((Object) prefab, (Object) null) && Object.op_Inequality((Object) prefab.GetComponent<PlantableSeed>(), (Object) null))
        {
          flag = true;
          break;
        }
      }
    }
    return flag;
  }

  private void UpdateMutantSeedStatusItem() => ((Component) this).gameObject.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.FabricatorAcceptsMutantSeeds, DlcManager.IsContentActive("EXPANSION1_ID") && this.HasRecipiesWithSeeds() && !this.forbidMutantSeeds);

  private void OnOperationalChanged(object data)
  {
    if ((bool) data)
      this.queueDirty = true;
    else
      this.CancelAllOpenOrders();
    this.UpdateChore();
  }

  public void Sim1000ms(float dt)
  {
    this.RefreshAndStartNextOrder();
    if (this.materialNeedCache.Count <= 0 || this.fetchListList.Count != 0)
      return;
    Debug.LogWarningFormat((Object) ((Component) this).gameObject, "{0} has material needs cached, but no open fetches. materialNeedCache={1}, fetchListList={2}", new object[3]
    {
      (object) ((Component) this).gameObject,
      (object) this.materialNeedCache.Count,
      (object) this.fetchListList.Count
    });
    this.queueDirty = true;
  }

  public void Sim200ms(float dt)
  {
    if (!this.operational.IsOperational)
      return;
    this.operational.SetActive(this.HasWorkingOrder && this.HasWorker);
    if (this.duplicantOperated || !this.HasWorkingOrder)
      return;
    ComplexRecipe recipe = this.recipe_list[this.workingOrderIdx];
    this.orderProgress += dt / recipe.time;
    if ((double) this.orderProgress < 1.0)
      return;
    this.CompleteWorkingOrder();
  }

  private void RefreshAndStartNextOrder()
  {
    if (!this.operational.IsOperational)
      return;
    if (this.queueDirty)
      this.RefreshQueue();
    if (this.HasWorkingOrder || !this.nextOrderIsWorkable)
      return;
    this.StartWorkingOrder(this.nextOrderIdx);
  }

  public void SetQueueDirty() => this.queueDirty = true;

  private void RefreshQueue()
  {
    this.queueDirty = false;
    this.ValidateWorkingOrder();
    this.ValidateNextOrder();
    this.UpdateOpenOrders();
    this.DropExcessIngredients(this.inStorage);
    this.Trigger(1721324763, (object) this);
  }

  private void StartWorkingOrder(int index)
  {
    Debug.Assert(!this.HasWorkingOrder, (object) "machineOrderIdx already set");
    this.workingOrderIdx = index;
    if (this.recipe_list[this.workingOrderIdx].id != this.lastWorkingRecipe)
    {
      this.orderProgress = 0.0f;
      this.lastWorkingRecipe = this.recipe_list[this.workingOrderIdx].id;
    }
    this.TransferCurrentRecipeIngredientsForBuild();
    Debug.Assert(this.openOrderCounts[this.workingOrderIdx] > 0, (object) "openOrderCount invalid");
    this.openOrderCounts[this.workingOrderIdx]--;
    this.UpdateChore();
    this.AdvanceNextOrder();
  }

  private void CancelWorkingOrder()
  {
    Debug.Assert(this.HasWorkingOrder, (object) "machineOrderIdx not set");
    this.buildStorage.Transfer(this.inStorage, true, true);
    this.workingOrderIdx = -1;
    this.orderProgress = 0.0f;
    this.UpdateChore();
  }

  public void CompleteWorkingOrder()
  {
    if (!this.HasWorkingOrder)
    {
      Debug.LogWarning((object) "CompleteWorkingOrder called with no working order.", (Object) ((Component) this).gameObject);
    }
    else
    {
      ComplexRecipe recipe = this.recipe_list[this.workingOrderIdx];
      this.SpawnOrderProduct(recipe);
      float num = this.buildStorage.MassStored();
      if ((double) num != 0.0)
      {
        Debug.LogWarningFormat((Object) ((Component) this).gameObject, "{0} build storage contains mass {1} after order completion.", new object[2]
        {
          (object) ((Component) this).gameObject,
          (object) num
        });
        this.buildStorage.Transfer(this.inStorage, true, true);
      }
      this.DecrementRecipeQueueCountInternal(recipe);
      this.workingOrderIdx = -1;
      this.orderProgress = 0.0f;
      this.CancelChore();
      if (this.cancelling)
        return;
      this.RefreshAndStartNextOrder();
    }
  }

  private void ValidateWorkingOrder()
  {
    if (!this.HasWorkingOrder || this.IsRecipeQueued(this.recipe_list[this.workingOrderIdx]))
      return;
    this.CancelWorkingOrder();
  }

  private void UpdateChore()
  {
    if (!this.duplicantOperated)
      return;
    bool flag = this.operational.IsOperational && this.HasWorkingOrder;
    if (flag && this.chore == null)
    {
      this.CreateChore();
    }
    else
    {
      if (flag || this.chore == null)
        return;
      this.CancelChore();
    }
  }

  private void AdvanceNextOrder()
  {
    for (int index = 0; index < this.recipe_list.Length; ++index)
    {
      this.nextOrderIdx = (this.nextOrderIdx + 1) % this.recipe_list.Length;
      ComplexRecipe recipe = this.recipe_list[this.nextOrderIdx];
      this.nextOrderIsWorkable = this.GetRemainingQueueCount(recipe) > 0 && this.HasIngredients(recipe, this.inStorage);
      if (this.nextOrderIsWorkable)
        break;
    }
  }

  private void ValidateNextOrder()
  {
    ComplexRecipe recipe = this.recipe_list[this.nextOrderIdx];
    this.nextOrderIsWorkable = this.GetRemainingQueueCount(recipe) > 0 && this.HasIngredients(recipe, this.inStorage);
    if (this.nextOrderIsWorkable)
      return;
    this.AdvanceNextOrder();
  }

  private void CancelAllOpenOrders()
  {
    for (int index = 0; index < this.openOrderCounts.Count; ++index)
      this.openOrderCounts[index] = 0;
    this.ClearMaterialNeeds();
    this.CancelFetches();
  }

  private void UpdateOpenOrders()
  {
    ComplexRecipe[] recipes = this.GetRecipes();
    if (recipes.Length != this.openOrderCounts.Count)
      Debug.LogErrorFormat((Object) ((Component) this).gameObject, "Recipe count {0} doesn't match open order count {1}", new object[2]
      {
        (object) recipes.Length,
        (object) this.openOrderCounts.Count
      });
    bool flag = false;
    this.hasOpenOrders = false;
    for (int index = 0; index < recipes.Length; ++index)
    {
      int recipePrefetchCount = this.GetRecipePrefetchCount(recipes[index]);
      if (recipePrefetchCount > 0)
        this.hasOpenOrders = true;
      int openOrderCount = this.openOrderCounts[index];
      if (openOrderCount != recipePrefetchCount)
      {
        if (recipePrefetchCount < openOrderCount)
          flag = true;
        this.openOrderCounts[index] = recipePrefetchCount;
      }
    }
    DictionaryPool<Tag, float, ComplexFabricator>.PooledDictionary pooledDictionary1 = DictionaryPool<Tag, float, ComplexFabricator>.Allocate();
    DictionaryPool<Tag, float, ComplexFabricator>.PooledDictionary missingAmounts = DictionaryPool<Tag, float, ComplexFabricator>.Allocate();
    for (int index = 0; index < this.openOrderCounts.Count; ++index)
    {
      if (this.openOrderCounts[index] > 0)
      {
        foreach (ComplexRecipe.RecipeElement ingredient in this.recipe_list[index].ingredients)
          ((Dictionary<Tag, float>) pooledDictionary1)[ingredient.material] = this.inStorage.GetAmountAvailable(ingredient.material);
      }
    }
    for (int index = 0; index < this.recipe_list.Length; ++index)
    {
      int openOrderCount = this.openOrderCounts[index];
      if (openOrderCount > 0)
      {
        foreach (ComplexRecipe.RecipeElement ingredient in this.recipe_list[index].ingredients)
        {
          float num1 = ingredient.amount * (float) openOrderCount;
          float num2 = num1 - ((Dictionary<Tag, float>) pooledDictionary1)[ingredient.material];
          if ((double) num2 > 0.0)
          {
            float num3;
            ((Dictionary<Tag, float>) missingAmounts).TryGetValue(ingredient.material, out num3);
            ((Dictionary<Tag, float>) missingAmounts)[ingredient.material] = num3 + num2;
            ((Dictionary<Tag, float>) pooledDictionary1)[ingredient.material] = 0.0f;
          }
          else
          {
            DictionaryPool<Tag, float, ComplexFabricator>.PooledDictionary pooledDictionary2 = pooledDictionary1;
            Tag material = ingredient.material;
            ((Dictionary<Tag, float>) pooledDictionary2)[material] = ((Dictionary<Tag, float>) pooledDictionary2)[material] - num1;
          }
        }
      }
    }
    if (flag)
      this.CancelFetches();
    if (((Dictionary<Tag, float>) missingAmounts).Count > 0)
      this.UpdateFetches(missingAmounts);
    this.UpdateMaterialNeeds((Dictionary<Tag, float>) missingAmounts);
    missingAmounts.Recycle();
    pooledDictionary1.Recycle();
  }

  private void UpdateMaterialNeeds(Dictionary<Tag, float> missingAmounts)
  {
    this.ClearMaterialNeeds();
    foreach (KeyValuePair<Tag, float> missingAmount in missingAmounts)
    {
      MaterialNeeds.UpdateNeed(missingAmount.Key, missingAmount.Value, ((Component) this).gameObject.GetMyWorldId());
      this.materialNeedCache.Add(missingAmount.Key, missingAmount.Value);
    }
  }

  private void ClearMaterialNeeds()
  {
    foreach (KeyValuePair<Tag, float> keyValuePair in this.materialNeedCache)
      MaterialNeeds.UpdateNeed(keyValuePair.Key, -keyValuePair.Value, ((Component) this).gameObject.GetMyWorldId());
    this.materialNeedCache.Clear();
  }

  public int HighestHEPQueued()
  {
    int val2 = 0;
    foreach (KeyValuePair<string, int> recipeQueueCount in this.recipeQueueCounts)
    {
      if (recipeQueueCount.Value > 0)
        val2 = Math.Max(this.recipe_list[this.FindRecipeIndex(recipeQueueCount.Key)].consumedHEP, val2);
    }
    return val2;
  }

  private void OnFetchComplete()
  {
    for (int index = this.fetchListList.Count - 1; index >= 0; --index)
    {
      if (this.fetchListList[index].IsComplete)
      {
        this.fetchListList.RemoveAt(index);
        this.queueDirty = true;
      }
    }
  }

  private void OnStorageChange(object data) => this.queueDirty = true;

  private void OnDroppedAll(object data)
  {
    if (this.HasWorkingOrder)
      this.CancelWorkingOrder();
    this.CancelAllOpenOrders();
    this.RefreshQueue();
  }

  private void DropExcessIngredients(Storage storage)
  {
    HashSet<Tag> tagSet = new HashSet<Tag>();
    if (Tag.op_Inequality(this.keepAdditionalTag, Tag.Invalid))
      tagSet.Add(this.keepAdditionalTag);
    for (int index = 0; index < this.recipe_list.Length; ++index)
    {
      ComplexRecipe recipe = this.recipe_list[index];
      if (this.IsRecipeQueued(recipe))
      {
        foreach (ComplexRecipe.RecipeElement ingredient in recipe.ingredients)
          tagSet.Add(ingredient.material);
      }
    }
    for (int index = storage.items.Count - 1; index >= 0; --index)
    {
      GameObject go = storage.items[index];
      if (!Object.op_Equality((Object) go, (Object) null))
      {
        PrimaryElement component1 = go.GetComponent<PrimaryElement>();
        if (!Object.op_Equality((Object) component1, (Object) null) && (!this.keepExcessLiquids || !component1.Element.IsLiquid))
        {
          KPrefabID component2 = go.GetComponent<KPrefabID>();
          if (Object.op_Implicit((Object) component2) && !tagSet.Contains(component2.PrefabID()))
            storage.Drop(go, true);
        }
      }
    }
  }

  private void OnCopySettings(object data)
  {
    GameObject gameObject = (GameObject) data;
    if (Object.op_Equality((Object) gameObject, (Object) null))
      return;
    ComplexFabricator component = gameObject.GetComponent<ComplexFabricator>();
    if (Object.op_Equality((Object) component, (Object) null))
      return;
    this.ForbidMutantSeeds = component.ForbidMutantSeeds;
    foreach (ComplexRecipe recipe in this.recipe_list)
    {
      int count;
      if (!component.recipeQueueCounts.TryGetValue(recipe.id, out count))
        count = 0;
      this.SetRecipeQueueCountInternal(recipe, count);
    }
    this.RefreshQueue();
  }

  private int CompareRecipe(ComplexRecipe a, ComplexRecipe b) => a.sortOrder != b.sortOrder ? a.sortOrder - b.sortOrder : StringComparer.InvariantCulture.Compare(a.id, b.id);

  public ComplexRecipe[] GetRecipes()
  {
    if (this.recipe_list == null)
    {
      Tag prefabTag = ((Component) this).GetComponent<KPrefabID>().PrefabTag;
      List<ComplexRecipe> recipes = ComplexRecipeManager.Get().recipes;
      List<ComplexRecipe> complexRecipeList = new List<ComplexRecipe>();
      foreach (ComplexRecipe complexRecipe in recipes)
      {
        foreach (Tag fabricator in complexRecipe.fabricators)
        {
          if (Tag.op_Equality(fabricator, prefabTag))
            complexRecipeList.Add(complexRecipe);
        }
      }
      this.recipe_list = complexRecipeList.ToArray();
      Array.Sort<ComplexRecipe>(this.recipe_list, new Comparison<ComplexRecipe>(this.CompareRecipe));
    }
    return this.recipe_list;
  }

  private void InitRecipeQueueCount()
  {
    foreach (ComplexRecipe recipe in this.GetRecipes())
    {
      bool flag = false;
      foreach (string key in this.recipeQueueCounts.Keys)
      {
        if (key == recipe.id)
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        this.recipeQueueCounts.Add(recipe.id, 0);
      this.openOrderCounts.Add(0);
    }
  }

  private int FindRecipeIndex(string id)
  {
    for (int recipeIndex = 0; recipeIndex < this.recipe_list.Length; ++recipeIndex)
    {
      if (this.recipe_list[recipeIndex].id == id)
        return recipeIndex;
    }
    return -1;
  }

  public int GetRecipeQueueCount(ComplexRecipe recipe) => this.recipeQueueCounts[recipe.id];

  public bool IsRecipeQueued(ComplexRecipe recipe)
  {
    int recipeQueueCount = this.recipeQueueCounts[recipe.id];
    Debug.Assert(recipeQueueCount >= 0 || recipeQueueCount == ComplexFabricator.QUEUE_INFINITE);
    return recipeQueueCount != 0;
  }

  public int GetRecipePrefetchCount(ComplexRecipe recipe)
  {
    int remainingQueueCount = this.GetRemainingQueueCount(recipe);
    Debug.Assert(remainingQueueCount >= 0);
    return Mathf.Min(2, remainingQueueCount);
  }

  private int GetRemainingQueueCount(ComplexRecipe recipe)
  {
    int recipeQueueCount = this.recipeQueueCounts[recipe.id];
    Debug.Assert(recipeQueueCount >= 0 || recipeQueueCount == ComplexFabricator.QUEUE_INFINITE);
    if (recipeQueueCount == ComplexFabricator.QUEUE_INFINITE)
      return ComplexFabricator.MAX_QUEUE_SIZE;
    if (recipeQueueCount <= 0)
      return 0;
    if (this.IsCurrentRecipe(recipe))
      --recipeQueueCount;
    return recipeQueueCount;
  }

  private bool IsCurrentRecipe(ComplexRecipe recipe) => this.workingOrderIdx >= 0 && this.recipe_list[this.workingOrderIdx].id == recipe.id;

  public void SetRecipeQueueCount(ComplexRecipe recipe, int count)
  {
    this.SetRecipeQueueCountInternal(recipe, count);
    this.RefreshQueue();
  }

  private void SetRecipeQueueCountInternal(ComplexRecipe recipe, int count) => this.recipeQueueCounts[recipe.id] = count;

  public void IncrementRecipeQueueCount(ComplexRecipe recipe)
  {
    if (this.recipeQueueCounts[recipe.id] == ComplexFabricator.QUEUE_INFINITE)
      this.recipeQueueCounts[recipe.id] = 0;
    else if (this.recipeQueueCounts[recipe.id] >= ComplexFabricator.MAX_QUEUE_SIZE)
      this.recipeQueueCounts[recipe.id] = ComplexFabricator.QUEUE_INFINITE;
    else
      this.recipeQueueCounts[recipe.id]++;
    this.RefreshQueue();
  }

  public void DecrementRecipeQueueCount(ComplexRecipe recipe, bool respectInfinite = true)
  {
    this.DecrementRecipeQueueCountInternal(recipe, respectInfinite);
    this.RefreshQueue();
  }

  private void DecrementRecipeQueueCountInternal(ComplexRecipe recipe, bool respectInfinite = true)
  {
    if (respectInfinite && this.recipeQueueCounts[recipe.id] == ComplexFabricator.QUEUE_INFINITE)
      return;
    if (this.recipeQueueCounts[recipe.id] == ComplexFabricator.QUEUE_INFINITE)
      this.recipeQueueCounts[recipe.id] = ComplexFabricator.MAX_QUEUE_SIZE;
    else if (this.recipeQueueCounts[recipe.id] == 0)
      this.recipeQueueCounts[recipe.id] = ComplexFabricator.QUEUE_INFINITE;
    else
      this.recipeQueueCounts[recipe.id]--;
  }

  private void CreateChore()
  {
    Debug.Assert(this.chore == null, (object) "chore should be null");
    this.chore = this.workable.CreateWorkChore(this.choreType, this.orderProgress);
  }

  private void CancelChore()
  {
    if (this.cancelling)
      return;
    this.cancelling = true;
    if (this.chore != null)
    {
      this.chore.Cancel("order cancelled");
      this.chore = (Chore) null;
    }
    this.cancelling = false;
  }

  private void UpdateFetches(
    DictionaryPool<Tag, float, ComplexFabricator>.PooledDictionary missingAmounts)
  {
    ChoreType byHash = Db.Get().ChoreTypes.GetByHash(this.fetchChoreTypeIdHash);
    foreach (KeyValuePair<Tag, float> missingAmount in (Dictionary<Tag, float>) missingAmounts)
    {
      if ((double) missingAmount.Value >= (double) PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT && !this.HasPendingFetch(missingAmount.Key))
      {
        FetchList2 fetchList2_1 = new FetchList2(this.inStorage, byHash);
        FetchList2 fetchList2_2 = fetchList2_1;
        Tag key = missingAmount.Key;
        float num = missingAmount.Value;
        Tag[] forbiddenTags = this.ForbiddenTags;
        double amount = (double) num;
        fetchList2_2.Add(key, forbiddenTags, (float) amount);
        fetchList2_1.ShowStatusItem = false;
        fetchList2_1.Submit(new System.Action(this.OnFetchComplete), false);
        this.fetchListList.Add(fetchList2_1);
      }
    }
  }

  private bool HasPendingFetch(Tag tag)
  {
    foreach (FetchList2 fetchList in this.fetchListList)
    {
      float num;
      fetchList.MinimumAmount.TryGetValue(tag, out num);
      if ((double) num > 0.0)
        return true;
    }
    return false;
  }

  private void CancelFetches()
  {
    foreach (FetchList2 fetchList in this.fetchListList)
      fetchList.Cancel("cancel all orders");
    this.fetchListList.Clear();
  }

  protected virtual void TransferCurrentRecipeIngredientsForBuild()
  {
    ComplexRecipe.RecipeElement[] ingredients = this.recipe_list[this.workingOrderIdx].ingredients;
label_7:
    for (int index = 0; index < ingredients.Length; ++index)
    {
      ComplexRecipe.RecipeElement recipeElement = ingredients[index];
      float amount;
      while (true)
      {
        amount = recipeElement.amount - this.buildStorage.GetAmountAvailable(recipeElement.material);
        if ((double) amount > 0.0)
        {
          if ((double) this.inStorage.GetAmountAvailable(recipeElement.material) > 0.0)
          {
            double num = (double) this.inStorage.Transfer(this.buildStorage, recipeElement.material, amount, hide_popups: true);
          }
          else
            break;
        }
        else
          goto label_7;
      }
      Debug.LogWarningFormat("TransferCurrentRecipeIngredientsForBuild ran out of {0} but still needed {1} more.", new object[2]
      {
        (object) recipeElement.material,
        (object) amount
      });
    }
  }

  protected virtual bool HasIngredients(ComplexRecipe recipe, Storage storage)
  {
    ComplexRecipe.RecipeElement[] ingredients = recipe.ingredients;
    if (recipe.consumedHEP > 0)
    {
      HighEnergyParticleStorage component = ((Component) this).GetComponent<HighEnergyParticleStorage>();
      if (Object.op_Equality((Object) component, (Object) null) || (double) component.Particles < (double) recipe.consumedHEP)
        return false;
    }
    foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
    {
      float amountAvailable = storage.GetAmountAvailable(recipeElement.material);
      if ((double) recipeElement.amount - (double) amountAvailable >= (double) PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT)
        return false;
    }
    return true;
  }

  private void ToggleMutantSeedFetches()
  {
    if (!this.HasAnyOrder)
      return;
    ChoreType byHash = Db.Get().ChoreTypes.GetByHash(this.fetchChoreTypeIdHash);
    List<FetchList2> fetchList2List = new List<FetchList2>();
    foreach (FetchList2 fetchList in this.fetchListList)
    {
      foreach (FetchOrder2 fetchOrder in fetchList.FetchOrders)
      {
        foreach (Tag tag in fetchOrder.Tags)
        {
          GameObject prefab = Assets.GetPrefab(tag);
          if (Object.op_Inequality((Object) prefab, (Object) null) && Object.op_Inequality((Object) prefab.GetComponent<PlantableSeed>(), (Object) null))
          {
            fetchList.Cancel("MutantSeedTagChanged");
            fetchList2List.Add(fetchList);
          }
        }
      }
    }
    foreach (FetchList2 fetchList2_1 in fetchList2List)
    {
      this.fetchListList.Remove(fetchList2_1);
      foreach (FetchOrder2 fetchOrder in fetchList2_1.FetchOrders)
      {
        foreach (Tag tag1 in fetchOrder.Tags)
        {
          FetchList2 fetchList2_2 = new FetchList2(this.inStorage, byHash);
          FetchList2 fetchList2_3 = fetchList2_2;
          Tag tag2 = tag1;
          float totalAmount = fetchOrder.TotalAmount;
          Tag[] forbiddenTags = this.ForbiddenTags;
          double amount = (double) totalAmount;
          fetchList2_3.Add(tag2, forbiddenTags, (float) amount);
          fetchList2_2.ShowStatusItem = false;
          fetchList2_2.Submit(new System.Action(this.OnFetchComplete), false);
          this.fetchListList.Add(fetchList2_2);
        }
      }
    }
  }

  protected virtual List<GameObject> SpawnOrderProduct(ComplexRecipe recipe)
  {
    List<GameObject> gameObjectList = new List<GameObject>();
    SimUtil.DiseaseInfo diseaseInfo;
    diseaseInfo.count = 0;
    diseaseInfo.idx = (byte) 0;
    float num1 = 0.0f;
    float num2 = 0.0f;
    string facadeID = (string) null;
    foreach (ComplexRecipe.RecipeElement ingredient in recipe.ingredients)
      num2 += ingredient.amount;
    Element element = (Element) null;
    foreach (ComplexRecipe.RecipeElement ingredient in recipe.ingredients)
    {
      float num3 = ingredient.amount / num2;
      if (recipe.ProductHasFacade && Util.IsNullOrWhiteSpace(facadeID))
      {
        RepairableEquipment component = this.buildStorage.FindFirst(ingredient.material).GetComponent<RepairableEquipment>();
        if (Object.op_Inequality((Object) component, (Object) null))
          facadeID = component.facadeID;
      }
      if (ingredient.inheritElement)
        element = this.buildStorage.FindFirst(ingredient.material).GetComponent<PrimaryElement>().Element;
      SimUtil.DiseaseInfo disease_info;
      float aggregate_temperature;
      this.buildStorage.ConsumeAndGetDisease(ingredient.material, ingredient.amount, out float _, out disease_info, out aggregate_temperature);
      if (disease_info.count > diseaseInfo.count)
        diseaseInfo = disease_info;
      num1 += aggregate_temperature * num3;
    }
    if (recipe.consumedHEP > 0)
    {
      double num4 = (double) ((Component) this).GetComponent<HighEnergyParticleStorage>().ConsumeAndGet((float) recipe.consumedHEP);
    }
    foreach (ComplexRecipe.RecipeElement result in recipe.results)
    {
      GameObject first = this.buildStorage.FindFirst(result.material);
      if (Object.op_Inequality((Object) first, (Object) null))
      {
        Edible component = first.GetComponent<Edible>();
        if (Object.op_Implicit((Object) component))
          ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, -component.Calories, StringFormatter.Replace((string) UI.ENDOFDAYREPORT.NOTES.CRAFTED_USED, "{0}", ((Component) component).GetProperName()), (string) UI.ENDOFDAYREPORT.NOTES.CRAFTED_CONTEXT);
      }
      switch (result.temperatureOperation)
      {
        case ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature:
        case ComplexRecipe.RecipeElement.TemperatureOperation.Heated:
          GameObject go = GameUtil.KInstantiate(Assets.GetPrefab(result.material), Grid.SceneLayer.Ore);
          int cell = Grid.PosToCell((KMonoBehaviour) this);
          TransformExtensions.SetPosition(go.transform, Vector3.op_Addition(Grid.CellToPosCCC(cell, Grid.SceneLayer.Ore), this.outputOffset));
          PrimaryElement component1 = go.GetComponent<PrimaryElement>();
          component1.Units = result.amount;
          component1.Temperature = result.temperatureOperation == ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature ? num1 : this.heatedTemperature;
          if (element != null)
            component1.SetElement(element.id, false);
          if (recipe.ProductHasFacade && !Util.IsNullOrWhiteSpace(facadeID))
          {
            Equippable component2 = go.GetComponent<Equippable>();
            if (Object.op_Inequality((Object) component2, (Object) null))
              EquippableFacade.AddFacadeToEquippable(component2, facadeID);
          }
          go.SetActive(true);
          float num5 = result.amount / recipe.TotalResultUnits();
          component1.AddDisease(diseaseInfo.idx, Mathf.RoundToInt((float) diseaseInfo.count * num5), "ComplexFabricator.CompleteOrder");
          if (!Util.IsNullOrWhiteSpace(result.facadeID))
          {
            Equippable component3 = go.GetComponent<Equippable>();
            if (Object.op_Inequality((Object) component3, (Object) null))
              EquippableFacade.AddFacadeToEquippable(component3, result.facadeID);
          }
          go.GetComponent<KMonoBehaviour>().Trigger(748399584, (object) null);
          gameObjectList.Add(go);
          if (this.storeProduced || result.storeElement)
          {
            this.outStorage.Store(go);
            break;
          }
          break;
        case ComplexRecipe.RecipeElement.TemperatureOperation.Melted:
          if (this.storeProduced || result.storeElement)
          {
            float temperature = ElementLoader.GetElement(result.material).defaultValues.temperature;
            this.outStorage.AddLiquid(ElementLoader.GetElementID(result.material), result.amount, temperature, (byte) 0, 0);
            break;
          }
          break;
      }
      if (gameObjectList.Count > 0)
      {
        SymbolOverrideController component4 = ((Component) this).GetComponent<SymbolOverrideController>();
        if (Object.op_Inequality((Object) component4, (Object) null))
        {
          KAnim.Build build = gameObjectList[0].GetComponent<KBatchedAnimController>().AnimFiles[0].GetData().build;
          KAnim.Build.Symbol symbol = build.GetSymbol(KAnimHashedString.op_Implicit(build.name));
          if (symbol != null)
          {
            component4.TryRemoveSymbolOverride(HashedString.op_Implicit("output_tracker"));
            component4.AddSymbolOverride(HashedString.op_Implicit("output_tracker"), symbol);
          }
          else
            Debug.LogWarning((object) (((Object) component4).name + " is missing symbol " + build.name));
        }
      }
    }
    if (recipe.producedHEP > 0)
    {
      double num6 = (double) ((Component) this).GetComponent<HighEnergyParticleStorage>().Store((float) recipe.producedHEP);
    }
    return gameObjectList;
  }

  public virtual List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    ComplexRecipe[] recipes = this.GetRecipes();
    if (recipes.Length != 0)
    {
      Descriptor descriptor = new Descriptor();
      ((Descriptor) ref descriptor).SetupDescriptor((string) UI.BUILDINGEFFECTS.PROCESSES, (string) UI.BUILDINGEFFECTS.TOOLTIPS.PROCESSES, (Descriptor.DescriptorType) 1);
      descriptors.Add(descriptor);
    }
    foreach (ComplexRecipe complexRecipe in recipes)
    {
      string str = "";
      string uiName = complexRecipe.GetUIName(false);
      foreach (ComplexRecipe.RecipeElement ingredient in complexRecipe.ingredients)
        str = str + "• " + string.Format((string) UI.BUILDINGEFFECTS.PROCESSEDITEM, (object) ingredient.material.ProperName(), (object) ingredient.amount) + "\n";
      Descriptor descriptor;
      // ISSUE: explicit constructor call
      ((Descriptor) ref descriptor).\u002Ector(uiName, string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.FABRICATOR_INGREDIENTS, (object) str), (Descriptor.DescriptorType) 1, false);
      ((Descriptor) ref descriptor).IncreaseIndent();
      descriptors.Add(descriptor);
    }
    return descriptors;
  }

  public virtual List<Descriptor> AdditionalEffectsForRecipe(ComplexRecipe recipe) => new List<Descriptor>();

  public string GetConversationTopic()
  {
    if (this.HasWorkingOrder)
    {
      ComplexRecipe recipe = this.recipe_list[this.workingOrderIdx];
      if (recipe != null)
        return ((Tag) ref recipe.results[0].material).Name;
    }
    return (string) null;
  }

  public bool NeedsMoreHEPForQueuedRecipe()
  {
    if (this.hasOpenOrders)
    {
      HighEnergyParticleStorage component = ((Component) this).GetComponent<HighEnergyParticleStorage>();
      foreach (KeyValuePair<string, int> recipeQueueCount in this.recipeQueueCounts)
      {
        if (recipeQueueCount.Value > 0)
        {
          foreach (ComplexRecipe recipe in this.GetRecipes())
          {
            if (recipe.id == recipeQueueCount.Key && (double) recipe.consumedHEP > (double) component.Particles)
              return true;
          }
        }
      }
    }
    return false;
  }
}
