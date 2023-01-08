// Decompiled with JetBrains decompiler
// Type: SteamUGCService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Ionic.Zip;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SteamUGCService : MonoBehaviour
{
  private UGCQueryHandle_t details_query = UGCQueryHandle_t.Invalid;
  private Callback<RemoteStoragePublishedFileSubscribed_t> on_subscribed;
  private Callback<RemoteStoragePublishedFileUpdated_t> on_updated;
  private Callback<RemoteStoragePublishedFileUnsubscribed_t> on_unsubscribed;
  private CallResult<SteamUGCQueryCompleted_t> on_query_completed;
  private HashSet<PublishedFileId_t> downloads = new HashSet<PublishedFileId_t>();
  private HashSet<PublishedFileId_t> queries = new HashSet<PublishedFileId_t>();
  private HashSet<PublishedFileId_t> proxies = new HashSet<PublishedFileId_t>();
  private HashSet<SteamUGCDetails_t> publishes = new HashSet<SteamUGCDetails_t>();
  private HashSet<PublishedFileId_t> removals = new HashSet<PublishedFileId_t>();
  private HashSet<SteamUGCDetails_t> previews = new HashSet<SteamUGCDetails_t>();
  private List<SteamUGCService.Mod> mods = new List<SteamUGCService.Mod>();
  private Dictionary<PublishedFileId_t, int> retry_counts = new Dictionary<PublishedFileId_t, int>();
  private static readonly string[] previewFileNames = new string[5]
  {
    "preview.png",
    "Preview.png",
    "PREVIEW.png",
    ".png",
    ".jpg"
  };
  private List<SteamUGCService.IClient> clients = new List<SteamUGCService.IClient>();
  private static SteamUGCService instance;
  private const EItemState DOWNLOADING_MASK = (EItemState) 48;
  private const int RETRY_THRESHOLD = 1000;

  public static SteamUGCService Instance => SteamUGCService.instance;

  private SteamUGCService()
  {
    // ISSUE: method pointer
    this.on_subscribed = Callback<RemoteStoragePublishedFileSubscribed_t>.Create(new Callback<RemoteStoragePublishedFileSubscribed_t>.DispatchDelegate((object) this, __methodptr(OnItemSubscribed)));
    // ISSUE: method pointer
    this.on_unsubscribed = Callback<RemoteStoragePublishedFileUnsubscribed_t>.Create(new Callback<RemoteStoragePublishedFileUnsubscribed_t>.DispatchDelegate((object) this, __methodptr(OnItemUnsubscribed)));
    // ISSUE: method pointer
    this.on_updated = Callback<RemoteStoragePublishedFileUpdated_t>.Create(new Callback<RemoteStoragePublishedFileUpdated_t>.DispatchDelegate((object) this, __methodptr(OnItemUpdated)));
    // ISSUE: method pointer
    this.on_query_completed = CallResult<SteamUGCQueryCompleted_t>.Create(new CallResult<SteamUGCQueryCompleted_t>.APIDispatchDelegate((object) this, __methodptr(OnSteamUGCQueryDetailsCompleted)));
    this.mods = new List<SteamUGCService.Mod>();
  }

  public static void Initialize()
  {
    if (Object.op_Inequality((Object) SteamUGCService.instance, (Object) null))
      return;
    GameObject gameObject = GameObject.Find("/SteamManager");
    SteamUGCService.instance = gameObject.GetComponent<SteamUGCService>();
    if (!Object.op_Equality((Object) SteamUGCService.instance, (Object) null))
      return;
    SteamUGCService.instance = gameObject.AddComponent<SteamUGCService>();
  }

  public void AddClient(SteamUGCService.IClient client)
  {
    this.clients.Add(client);
    ListPool<PublishedFileId_t, SteamUGCService>.PooledList added = ListPool<PublishedFileId_t, SteamUGCService>.Allocate();
    foreach (SteamUGCService.Mod mod in this.mods)
      ((List<PublishedFileId_t>) added).Add(mod.fileId);
    client.UpdateMods((IEnumerable<PublishedFileId_t>) added, Enumerable.Empty<PublishedFileId_t>(), Enumerable.Empty<PublishedFileId_t>(), Enumerable.Empty<SteamUGCService.Mod>());
    added.Recycle();
  }

  public void RemoveClient(SteamUGCService.IClient client) => this.clients.Remove(client);

  public void Awake()
  {
    Debug.Assert(Object.op_Equality((Object) SteamUGCService.instance, (Object) null));
    SteamUGCService.instance = this;
    uint numSubscribedItems = SteamUGC.GetNumSubscribedItems();
    if (numSubscribedItems == 0U)
      return;
    PublishedFileId_t[] other = new PublishedFileId_t[(int) numSubscribedItems];
    int subscribedItems = (int) SteamUGC.GetSubscribedItems(other, numSubscribedItems);
    this.downloads.UnionWith((IEnumerable<PublishedFileId_t>) other);
  }

  public bool IsSubscribed(PublishedFileId_t item) => this.downloads.Contains(item) || this.proxies.Contains(item) || this.queries.Contains(item) || ((IEnumerable<SteamUGCDetails_t>) this.publishes).Any<SteamUGCDetails_t>((Func<SteamUGCDetails_t, bool>) (candidate => PublishedFileId_t.op_Equality(candidate.m_nPublishedFileId, item))) || this.mods.Exists((Predicate<SteamUGCService.Mod>) (candidate => PublishedFileId_t.op_Equality(candidate.fileId, item)));

  public SteamUGCService.Mod FindMod(PublishedFileId_t item) => this.mods.Find((Predicate<SteamUGCService.Mod>) (candidate => PublishedFileId_t.op_Equality(candidate.fileId, item)));

  private void OnDestroy()
  {
    Debug.Assert(Object.op_Equality((Object) SteamUGCService.instance, (Object) this));
    SteamUGCService.instance = (SteamUGCService) null;
  }

  private Texture2D LoadPreviewImage(SteamUGCDetails_t details)
  {
    byte[] numArray = (byte[]) null;
    if (UGCHandle_t.op_Inequality(details.m_hPreviewFile, UGCHandle_t.Invalid))
    {
      SteamRemoteStorage.UGCDownload(details.m_hPreviewFile, 0U);
      numArray = new byte[details.m_nPreviewFileSize];
      if (SteamRemoteStorage.UGCRead(details.m_hPreviewFile, numArray, details.m_nPreviewFileSize, 0U, (EUGCReadAction) 0) != details.m_nPreviewFileSize)
      {
        if (this.retry_counts[details.m_nPublishedFileId] % 100 == 0)
          Debug.LogFormat("Steam: Preview image load failed", Array.Empty<object>());
        numArray = (byte[]) null;
      }
    }
    if (numArray == null)
      numArray = SteamUGCService.GetBytesFromZip(details.m_nPublishedFileId, SteamUGCService.previewFileNames, out System.DateTime _);
    Texture2D texture2D = (Texture2D) null;
    if (numArray != null)
    {
      texture2D = new Texture2D(2, 2);
      ImageConversion.LoadImage(texture2D, numArray);
    }
    else
      ++this.retry_counts[details.m_nPublishedFileId];
    return texture2D;
  }

  private void Update()
  {
    if (!SteamManager.Initialized || Object.op_Inequality((Object) Game.Instance, (Object) null))
      return;
    this.downloads.ExceptWith((IEnumerable<PublishedFileId_t>) this.removals);
    this.publishes.RemoveWhere((Predicate<SteamUGCDetails_t>) (publish => this.removals.Contains(publish.m_nPublishedFileId)));
    this.previews.RemoveWhere((Predicate<SteamUGCDetails_t>) (publish => this.removals.Contains(publish.m_nPublishedFileId)));
    this.proxies.ExceptWith((IEnumerable<PublishedFileId_t>) this.removals);
    HashSetPool<SteamUGCService.Mod, SteamUGCService>.PooledHashSet loaded_previews = HashSetPool<SteamUGCService.Mod, SteamUGCService>.Allocate();
    HashSetPool<PublishedFileId_t, SteamUGCService>.PooledHashSet cancelled_previews = HashSetPool<PublishedFileId_t, SteamUGCService>.Allocate();
    foreach (SteamUGCDetails_t preview in this.previews)
    {
      SteamUGCService.Mod mod = this.FindMod(preview.m_nPublishedFileId);
      DebugUtil.DevAssert(mod != null, "expect mod with pending preview to be published", (Object) null);
      mod.previewImage = this.LoadPreviewImage(preview);
      if (Object.op_Inequality((Object) mod.previewImage, (Object) null))
        ((HashSet<SteamUGCService.Mod>) loaded_previews).Add(mod);
      else if (1000 < this.retry_counts[preview.m_nPublishedFileId])
        ((HashSet<PublishedFileId_t>) cancelled_previews).Add(mod.fileId);
    }
    this.previews.RemoveWhere((Predicate<SteamUGCDetails_t>) (publish => ((IEnumerable<SteamUGCService.Mod>) loaded_previews).Any<SteamUGCService.Mod>((Func<SteamUGCService.Mod, bool>) (mod => PublishedFileId_t.op_Equality(mod.fileId, publish.m_nPublishedFileId))) || ((HashSet<PublishedFileId_t>) cancelled_previews).Contains(publish.m_nPublishedFileId)));
    cancelled_previews.Recycle();
    ListPool<SteamUGCService.Mod, SteamUGCService>.PooledList pooledList = ListPool<SteamUGCService.Mod, SteamUGCService>.Allocate();
    HashSetPool<PublishedFileId_t, SteamUGCService>.PooledHashSet published = HashSetPool<PublishedFileId_t, SteamUGCService>.Allocate();
    foreach (SteamUGCDetails_t publish in this.publishes)
    {
      if (((int) SteamUGC.GetItemState(publish.m_nPublishedFileId) & 48) == 0)
      {
        Debug.LogFormat("Steam: updating info for mod {0}", new object[1]
        {
          (object) ((SteamUGCDetails_t) ref publish).m_rgchTitle
        });
        SteamUGCService.Mod mod = new SteamUGCService.Mod(publish, this.LoadPreviewImage(publish));
        ((List<SteamUGCService.Mod>) pooledList).Add(mod);
        if (UGCHandle_t.op_Inequality(publish.m_hPreviewFile, UGCHandle_t.Invalid) && Object.op_Equality((Object) mod.previewImage, (Object) null))
          this.previews.Add(publish);
        ((HashSet<PublishedFileId_t>) published).Add(publish.m_nPublishedFileId);
      }
    }
    this.publishes.RemoveWhere((Predicate<SteamUGCDetails_t>) (publish => ((HashSet<PublishedFileId_t>) published).Contains(publish.m_nPublishedFileId)));
    published.Recycle();
    foreach (PublishedFileId_t proxy in this.proxies)
    {
      Debug.LogFormat("Steam: proxy mod {0}", new object[1]
      {
        (object) proxy
      });
      ((List<SteamUGCService.Mod>) pooledList).Add(new SteamUGCService.Mod(proxy));
    }
    this.proxies.Clear();
    ListPool<PublishedFileId_t, SteamUGCService>.PooledList added = ListPool<PublishedFileId_t, SteamUGCService>.Allocate();
    ListPool<PublishedFileId_t, SteamUGCService>.PooledList updated = ListPool<PublishedFileId_t, SteamUGCService>.Allocate();
    foreach (SteamUGCService.Mod mod1 in (List<SteamUGCService.Mod>) pooledList)
    {
      SteamUGCService.Mod mod = mod1;
      int index = this.mods.FindIndex((Predicate<SteamUGCService.Mod>) (candidate => PublishedFileId_t.op_Equality(candidate.fileId, mod.fileId)));
      if (index == -1)
      {
        this.mods.Add(mod);
        ((List<PublishedFileId_t>) added).Add(mod.fileId);
      }
      else
      {
        this.mods[index] = mod;
        ((List<PublishedFileId_t>) updated).Add(mod.fileId);
      }
    }
    pooledList.Recycle();
    bool flag = UGCQueryHandle_t.op_Equality(this.details_query, UGCQueryHandle_t.Invalid);
    if (((List<PublishedFileId_t>) added).Count != 0 || ((List<PublishedFileId_t>) updated).Count != 0 || flag && this.removals.Count != 0 || ((HashSet<SteamUGCService.Mod>) loaded_previews).Count != 0)
    {
      foreach (SteamUGCService.IClient client in this.clients)
        client.UpdateMods((IEnumerable<PublishedFileId_t>) added, (IEnumerable<PublishedFileId_t>) updated, flag ? (IEnumerable<PublishedFileId_t>) this.removals : Enumerable.Empty<PublishedFileId_t>(), (IEnumerable<SteamUGCService.Mod>) loaded_previews);
    }
    added.Recycle();
    updated.Recycle();
    loaded_previews.Recycle();
    if (flag)
    {
      foreach (PublishedFileId_t removal1 in this.removals)
      {
        PublishedFileId_t removal = removal1;
        this.mods.RemoveAll((Predicate<SteamUGCService.Mod>) (candidate => PublishedFileId_t.op_Equality(candidate.fileId, removal)));
      }
      this.removals.Clear();
    }
    foreach (PublishedFileId_t download in this.downloads)
    {
      EItemState itemState = (EItemState) (int) SteamUGC.GetItemState(download);
      if (((itemState & 4) == null || (itemState & 8) != null) && (itemState & 48) == null)
        SteamUGC.DownloadItem(download, false);
    }
    if (!UGCQueryHandle_t.op_Equality(this.details_query, UGCQueryHandle_t.Invalid))
      return;
    this.queries.UnionWith((IEnumerable<PublishedFileId_t>) this.downloads);
    this.downloads.Clear();
    if (this.queries.Count == 0)
      return;
    this.details_query = SteamUGC.CreateQueryUGCDetailsRequest(((IEnumerable<PublishedFileId_t>) this.queries).ToArray<PublishedFileId_t>(), (uint) this.queries.Count);
    this.on_query_completed.Set(SteamUGC.SendQueryUGCRequest(this.details_query), (CallResult<SteamUGCQueryCompleted_t>.APIDispatchDelegate) null);
  }

  private void OnSteamUGCQueryDetailsCompleted(SteamUGCQueryCompleted_t pCallback, bool bIOFailure)
  {
    EResult eResult = pCallback.m_eResult;
    if (eResult != 1)
    {
      if (eResult == 10)
      {
        Debug.Log((object) ("Steam: [OnSteamUGCQueryDetailsCompleted] - handle: " + pCallback.m_handle.ToString() + " -- Result: " + pCallback.m_eResult.ToString() + " Resending"));
      }
      else
      {
        Debug.Log((object) ("Steam: [OnSteamUGCQueryDetailsCompleted] - handle: " + pCallback.m_handle.ToString() + " -- Result: " + pCallback.m_eResult.ToString() + " -- NUm results: " + pCallback.m_unNumResultsReturned.ToString() + " --Total Matching: " + pCallback.m_unTotalMatchingResults.ToString() + " -- cached: " + pCallback.m_bCachedData.ToString()));
        HashSet<PublishedFileId_t> proxies = this.proxies;
        this.proxies = this.queries;
        this.queries = proxies;
      }
    }
    else
    {
      for (uint index = 0; index < pCallback.m_unNumResultsReturned; ++index)
      {
        SteamUGCDetails_t steamUgcDetailsT = new SteamUGCDetails_t();
        SteamUGC.GetQueryUGCResult(this.details_query, index, ref steamUgcDetailsT);
        if (!this.removals.Contains(steamUgcDetailsT.m_nPublishedFileId))
        {
          this.publishes.Add(steamUgcDetailsT);
          this.retry_counts[steamUgcDetailsT.m_nPublishedFileId] = 0;
        }
        this.queries.Remove(steamUgcDetailsT.m_nPublishedFileId);
      }
    }
    SteamUGC.ReleaseQueryUGCRequest(this.details_query);
    this.details_query = UGCQueryHandle_t.Invalid;
  }

  private void OnItemSubscribed(RemoteStoragePublishedFileSubscribed_t pCallback) => this.downloads.Add(pCallback.m_nPublishedFileId);

  private void OnItemUpdated(RemoteStoragePublishedFileUpdated_t pCallback) => this.downloads.Add(pCallback.m_nPublishedFileId);

  private void OnItemUnsubscribed(RemoteStoragePublishedFileUnsubscribed_t pCallback) => this.removals.Add(pCallback.m_nPublishedFileId);

  public static byte[] GetBytesFromZip(
    PublishedFileId_t item,
    string[] filesToExtract,
    out System.DateTime lastModified,
    bool getFirstMatch = false)
  {
    byte[] bytesFromZip = (byte[]) null;
    lastModified = System.DateTime.MinValue;
    ulong num1;
    string path;
    uint num2;
    SteamUGC.GetItemInstallInfo(item, ref num1, ref path, 1024U, ref num2);
    try
    {
      lastModified = File.GetLastWriteTimeUtc(path);
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (ZipFile zipFile = ZipFile.Read(path))
        {
          ZipEntry zipEntry = (ZipEntry) null;
          foreach (string str in filesToExtract)
          {
            if (str.Length > 4)
            {
              if (zipFile.ContainsEntry(str))
                zipEntry = zipFile[str];
            }
            else
            {
              foreach (ZipEntry entry in (IEnumerable<ZipEntry>) zipFile.Entries)
              {
                if (entry.FileName.EndsWith(str))
                {
                  zipEntry = entry;
                  break;
                }
              }
            }
            if (zipEntry != null)
              break;
          }
          if (zipEntry != null)
          {
            zipEntry.Extract((Stream) memoryStream);
            memoryStream.Flush();
            bytesFromZip = memoryStream.ToArray();
          }
        }
      }
    }
    catch (Exception ex)
    {
    }
    return bytesFromZip;
  }

  public class Mod
  {
    public Texture2D previewImage;

    public Mod(SteamUGCDetails_t item, Texture2D previewImage)
    {
      this.title = ((SteamUGCDetails_t) ref item).m_rgchTitle;
      this.description = ((SteamUGCDetails_t) ref item).m_rgchDescription;
      this.fileId = item.m_nPublishedFileId;
      this.lastUpdateTime = (ulong) item.m_rtimeUpdated;
      this.tags = new List<string>((IEnumerable<string>) ((SteamUGCDetails_t) ref item).m_rgchTags.Split(','));
      this.previewImage = previewImage;
    }

    public Mod(PublishedFileId_t id)
    {
      this.title = string.Empty;
      this.description = string.Empty;
      this.fileId = id;
      this.lastUpdateTime = 0UL;
      this.tags = new List<string>();
      this.previewImage = (Texture2D) null;
    }

    public string title { get; private set; }

    public string description { get; private set; }

    public PublishedFileId_t fileId { get; private set; }

    public ulong lastUpdateTime { get; private set; }

    public List<string> tags { get; private set; }
  }

  public interface IClient
  {
    void UpdateMods(
      IEnumerable<PublishedFileId_t> added,
      IEnumerable<PublishedFileId_t> updated,
      IEnumerable<PublishedFileId_t> removed,
      IEnumerable<SteamUGCService.Mod> loaded_previews);
  }
}
