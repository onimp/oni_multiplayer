// Decompiled with JetBrains decompiler
// Type: ClothingOutfitTarget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public readonly struct ClothingOutfitTarget : IEquatable<ClothingOutfitTarget>
{
  public readonly ClothingOutfitTarget.Implementation impl;

  public string Id => this.impl.OutfitId;

  public string[] ReadItems() => ((IEnumerable<string>) this.impl.ReadItems()).Where<string>(new Func<string, bool>(ClothingOutfitTarget.DoesClothingItemExist)).ToArray<string>();

  public void WriteItems(string[] items) => this.impl.WriteItems(items);

  public bool CanWriteItems => this.impl.CanWriteItems;

  public string ReadName() => this.impl.ReadName();

  public void WriteName(string name) => this.impl.WriteName(name);

  public bool CanWriteName => this.impl.CanWriteName;

  public void Delete() => this.impl.Delete();

  public bool CanDelete => this.impl.CanDelete;

  public bool DoesExist() => this.impl.DoesExist();

  public ClothingOutfitTarget(ClothingOutfitTarget.Implementation impl) => this.impl = impl;

  public bool DoesContainNonOwnedItems() => ClothingOutfitTarget.DoesContainNonOwnedItems((IList<string>) this.ReadItems());

  public static bool DoesContainNonOwnedItems(IList<string> itemIds)
  {
    foreach (string itemId in (IEnumerable<string>) itemIds)
    {
      (bool hasValue, int num1) = PermitItems.GetOwnedCount(itemId);
      int num2 = hasValue ? 1 : 0;
      int num3 = num1;
      if (num2 != 0 && num3 <= 0)
        return true;
    }
    return false;
  }

  public IEnumerable<ClothingItemResource> ReadItemValues() => ((IEnumerable<string>) this.ReadItems()).Select<string, ClothingItemResource>((Func<string, ClothingItemResource>) (i => Db.Get().Permits.ClothingItems.Get(i)));

  public static bool DoesClothingItemExist(string clothingItemId) => !Util.IsNullOrDestroyed((object) Db.Get().Permits.ClothingItems.TryGet(clothingItemId));

  public bool Is<T>() where T : ClothingOutfitTarget.Implementation => this.impl is T;

  public bool Is<T>(out T value) where T : ClothingOutfitTarget.Implementation
  {
    if (this.impl is T impl)
    {
      value = impl;
      return true;
    }
    value = default (T);
    return false;
  }

  public bool IsTemplateOutfit() => this.Is<ClothingOutfitTarget.KleiAuthored>() || this.Is<ClothingOutfitTarget.UserAuthored>();

  public static ClothingOutfitTarget ForNewOutfit() => new ClothingOutfitTarget((ClothingOutfitTarget.Implementation) new ClothingOutfitTarget.UserAuthored(ClothingOutfitTarget.GetUniqueNameIdFrom((string) UI.OUTFIT_NAME.NEW)));

  public static ClothingOutfitTarget ForNewOutfit(string id) => !ClothingOutfitTarget.DoesExist(id) ? new ClothingOutfitTarget((ClothingOutfitTarget.Implementation) new ClothingOutfitTarget.UserAuthored(id)) : throw new ArgumentException("Can not create a new target with id " + id + ", an outfit with that id already exists");

  public static ClothingOutfitTarget ForCopyOf(ClothingOutfitTarget sourceTarget) => new ClothingOutfitTarget((ClothingOutfitTarget.Implementation) new ClothingOutfitTarget.UserAuthored(ClothingOutfitTarget.GetUniqueNameIdFrom(UI.OUTFIT_NAME.COPY_OF.Replace("{OutfitName}", sourceTarget.ReadName()))));

  public static ClothingOutfitTarget FromMinion(GameObject minionInstance) => new ClothingOutfitTarget((ClothingOutfitTarget.Implementation) new ClothingOutfitTarget.MinionInstance(minionInstance));

  public static ClothingOutfitTarget FromId(string outfitId) => ClothingOutfitTarget.TryFromId(outfitId).Value;

  public static Option<ClothingOutfitTarget> TryFromId(string outfitId)
  {
    if (outfitId == null)
      return (Option<ClothingOutfitTarget>) Option.None;
    if (CustomClothingOutfits.Instance.OutfitData.CustomOutfits.ContainsKey(outfitId))
      return (Option<ClothingOutfitTarget>) new ClothingOutfitTarget((ClothingOutfitTarget.Implementation) new ClothingOutfitTarget.UserAuthored(outfitId));
    return Db.Get().Permits.ClothingOutfits.TryGet(outfitId) != null ? (Option<ClothingOutfitTarget>) new ClothingOutfitTarget((ClothingOutfitTarget.Implementation) new ClothingOutfitTarget.KleiAuthored(outfitId)) : (Option<ClothingOutfitTarget>) Option.None;
  }

  public static bool DoesExist(string outfitId) => Db.Get().Permits.ClothingOutfits.TryGet(outfitId) != null || CustomClothingOutfits.Instance.OutfitData.CustomOutfits.ContainsKey(outfitId);

  public static IEnumerable<ClothingOutfitTarget> GetAll()
  {
    foreach (ClothingOutfitResource resource in Db.Get().Permits.ClothingOutfits.resources)
      yield return new ClothingOutfitTarget((ClothingOutfitTarget.Implementation) new ClothingOutfitTarget.KleiAuthored(resource));
    foreach (KeyValuePair<string, string[]> customOutfit in CustomClothingOutfits.Instance.OutfitData.CustomOutfits)
    {
      string outfitId;
      string[] strArray;
      Util.Deconstruct<string, string[]>(customOutfit, ref outfitId, ref strArray);
      yield return new ClothingOutfitTarget((ClothingOutfitTarget.Implementation) new ClothingOutfitTarget.UserAuthored(outfitId));
    }
  }

  public static ClothingOutfitTarget GetRandom() => Util.GetRandom<ClothingOutfitTarget>(ClothingOutfitTarget.GetAll());

  public static string GetUniqueNameIdFrom(string preferredName)
  {
    if (!ClothingOutfitTarget.DoesExist(preferredName))
      return preferredName;
    string replacement = "testOutfit";
    string str = !(UI.OUTFIT_NAME.RESOLVE_CONFLICT.Replace("{OutfitName}", replacement).Replace("{ConflictNumber}", 1.ToString()) != UI.OUTFIT_NAME.RESOLVE_CONFLICT.Replace("{OutfitName}", replacement).Replace("{ConflictNumber}", 2.ToString())) ? "{OutfitName} ({ConflictNumber})" : (string) UI.OUTFIT_NAME.RESOLVE_CONFLICT;
    for (int index = 1; index < 10000; ++index)
    {
      string outfitId = str.Replace("{OutfitName}", preferredName).Replace("{ConflictNumber}", index.ToString());
      if (!ClothingOutfitTarget.DoesExist(outfitId))
        return outfitId;
    }
    throw new Exception("Couldn't get a unique name for preferred name: " + preferredName);
  }

  public static bool operator ==(ClothingOutfitTarget a, ClothingOutfitTarget b) => a.Equals(b);

  public static bool operator !=(ClothingOutfitTarget a, ClothingOutfitTarget b) => !a.Equals(b);

  public override bool Equals(object obj) => obj is ClothingOutfitTarget other && this.Equals(other);

  public bool Equals(ClothingOutfitTarget other) => this.impl == null || other.impl == null ? this.impl == null == (other.impl == null) : this.Id == other.Id;

  public override int GetHashCode() => Hash.SDBMLower(this.impl.OutfitId);

  public interface Implementation
  {
    string OutfitId { get; }

    string[] ReadItems();

    void WriteItems(string[] items);

    bool CanWriteItems { get; }

    string ReadName();

    void WriteName(string name);

    bool CanWriteName { get; }

    void Delete();

    bool CanDelete { get; }

    bool DoesExist();
  }

  public readonly struct MinionInstance : ClothingOutfitTarget.Implementation
  {
    public readonly GameObject minionInstance;
    public readonly Accessorizer accessorizer;

    public bool CanWriteItems => true;

    public bool CanWriteName => false;

    public bool CanDelete => false;

    public bool DoesExist() => !Util.IsNullOrDestroyed((object) this.minionInstance);

    public string OutfitId => ((Object) this.minionInstance).GetInstanceID().ToString() + "_outfit";

    public MinionInstance(GameObject minionInstance)
    {
      this.minionInstance = minionInstance;
      this.accessorizer = minionInstance.GetComponent<Accessorizer>();
    }

    public string[] ReadItems() => this.accessorizer.GetClothingItemIds();

    public void WriteItems(string[] items) => this.accessorizer.ApplyClothingItems(((IEnumerable<string>) items).Select<string, ClothingItemResource>((Func<string, ClothingItemResource>) (i => Db.Get().Permits.ClothingItems.Get(i))));

    public string ReadName() => UI.OUTFIT_NAME.MINIONS_OUTFIT.Replace("{MinionName}", this.minionInstance.GetProperName());

    public void WriteName(string name) => throw new InvalidOperationException("Can not change change the outfit id for a minion instance");

    public void Delete() => throw new InvalidOperationException("Can not delete a minion instance outfit");
  }

  public readonly struct UserAuthored : ClothingOutfitTarget.Implementation
  {
    private readonly string[] m_outfitId;

    public bool CanWriteItems => true;

    public bool CanWriteName => true;

    public bool CanDelete => true;

    public bool DoesExist() => CustomClothingOutfits.Instance.OutfitData.CustomOutfits.ContainsKey(this.OutfitId);

    public string OutfitId => this.m_outfitId[0];

    public UserAuthored(string outfitId) => this.m_outfitId = new string[1]
    {
      outfitId
    };

    public string[] ReadItems()
    {
      string[] strArray;
      return CustomClothingOutfits.Instance.OutfitData.CustomOutfits.TryGetValue(this.OutfitId, out strArray) ? strArray : ClothingOutfitTargetExtensions.NO_ITEMS;
    }

    public void WriteItems(string[] items) => CustomClothingOutfits.Instance.EditOutfit(this.OutfitId, items);

    public string ReadName() => this.OutfitId;

    public void WriteName(string name)
    {
      if (this.OutfitId == name)
        return;
      if (ClothingOutfitTarget.DoesExist(name))
        throw new Exception("Can not change outfit name from \"" + this.OutfitId + "\" to \"" + name + "\", \"" + name + "\" already exists");
      if (CustomClothingOutfits.Instance.OutfitData.CustomOutfits.ContainsKey(this.OutfitId))
        CustomClothingOutfits.Instance.RenameOutfit(this.OutfitId, name);
      else
        CustomClothingOutfits.Instance.EditOutfit(name, ClothingOutfitTargetExtensions.NO_ITEMS);
      this.m_outfitId[0] = name;
    }

    public void Delete() => CustomClothingOutfits.Instance.RemoveOutfit(this.OutfitId);
  }

  public readonly struct KleiAuthored : ClothingOutfitTarget.Implementation
  {
    public readonly ClothingOutfitResource resource;
    private readonly string m_outfitId;

    public bool CanWriteItems => false;

    public bool CanWriteName => false;

    public bool CanDelete => false;

    public bool DoesExist() => true;

    public string OutfitId => this.m_outfitId;

    public KleiAuthored(string outfitId)
    {
      this.m_outfitId = outfitId;
      this.resource = Db.Get().Permits.ClothingOutfits.Get(outfitId);
    }

    public KleiAuthored(ClothingOutfitResource outfit)
    {
      this.m_outfitId = outfit.Id;
      this.resource = outfit;
    }

    public string[] ReadItems() => this.resource.itemsInOutfit;

    public void WriteItems(string[] items) => throw new InvalidOperationException("Can not set items on a Db authored outfit");

    public string ReadName() => this.resource.Name;

    public void WriteName(string name) => throw new InvalidOperationException("Can not set name on a Db authored outfit");

    public void Delete() => throw new InvalidOperationException("Can not delete a Db authored outfit");
  }
}
