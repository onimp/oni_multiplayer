// Decompiled with JetBrains decompiler
// Type: SimpleNetworkCache
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public static class SimpleNetworkCache
{
  public static void LoadFromCacheOrDownload(
    string cache_id,
    string url,
    int version,
    UnityWebRequest data_wr,
    Action<UnityWebRequest> callback)
  {
    string cache_folder = Util.CacheFolder();
    string cache_prefix = System.IO.Path.Combine(cache_folder, cache_id);
    string version_filepath = cache_prefix + "_version";
    string data_filepath = cache_prefix + "_data";
    UnityWebRequest version_wr = new UnityWebRequest(new Uri(version_filepath, UriKind.Absolute), "GET", (DownloadHandler) new DownloadHandlerBuffer(), (UploadHandler) null);
    ((AsyncOperation) version_wr.SendWebRequest()).completed += (Action<AsyncOperation>) (op =>
    {
      if (SimpleNetworkCache.GetVersionFromWebRequest(version_wr) == version)
      {
        data_wr.uri = new Uri(data_filepath, UriKind.Absolute);
        ((AsyncOperation) data_wr.SendWebRequest()).completed += (Action<AsyncOperation>) (fileOp =>
        {
          if (!string.IsNullOrEmpty(data_wr.error))
          {
            Debug.LogWarning((object) ("Failure to read cached file: " + data_filepath));
            try
            {
              File.Delete(version_filepath);
              File.Delete(data_filepath);
            }
            catch
            {
              Debug.LogWarning((object) "Failed to delete cached files");
            }
          }
          callback(data_wr);
        });
      }
      else
      {
        data_wr.url = url;
        ((AsyncOperation) data_wr.SendWebRequest()).completed += (Action<AsyncOperation>) (webOp =>
        {
          if (string.IsNullOrEmpty(data_wr.error))
          {
            try
            {
              Directory.CreateDirectory(cache_folder);
              File.WriteAllBytes(data_filepath, data_wr.downloadHandler.data);
              File.WriteAllText(version_filepath, version.ToString());
            }
            catch
            {
              Debug.LogWarning((object) ("Failed to write cache files to: " + cache_prefix));
            }
          }
          callback(data_wr);
        });
      }
      version_wr.Dispose();
    });
  }

  private static int GetVersionFromWebRequest(UnityWebRequest version_wr)
  {
    int result;
    return !string.IsNullOrEmpty(version_wr.error) || version_wr.downloadHandler == null || !int.TryParse(version_wr.downloadHandler.text, out result) ? -1 : result;
  }
}
