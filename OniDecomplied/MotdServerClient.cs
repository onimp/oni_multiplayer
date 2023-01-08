// Decompiled with JetBrains decompiler
// Type: MotdServerClient
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using STRINGS;
using System;
using UnityEngine;
using UnityEngine.Networking;

public class MotdServerClient
{
  private Action<MotdServerClient.MotdResponse, string> m_callback;
  private MotdServerClient.MotdResponse m_localMotd;

  private static string MotdServerUrl => "https://klei-motd.s3.amazonaws.com/oni/" + MotdServerClient.GetLocalePathSuffix();

  private static string MotdLocalPath => "motd_local/" + MotdServerClient.GetLocalePathSuffix();

  private static string MotdLocalImagePath(int imageVersion) => MotdServerClient.MotdLocalImagePath(imageVersion, Localization.GetLocale());

  private static string FallbackMotdLocalImagePath(int imageVersion) => MotdServerClient.MotdLocalImagePath(imageVersion, (Localization.Locale) null);

  private static string MotdLocalImagePath(int imageVersion, Localization.Locale locale) => "motd_local/" + MotdServerClient.GetLocalePathModifier(locale) + "image_" + imageVersion.ToString();

  private static string GetLocalePathModifier() => MotdServerClient.GetLocalePathModifier(Localization.GetLocale());

  private static string GetLocalePathModifier(Localization.Locale locale)
  {
    string localePathModifier = "";
    if (locale != null)
    {
      switch (locale.Lang)
      {
        case Localization.Language.Chinese:
        case Localization.Language.Korean:
        case Localization.Language.Russian:
          localePathModifier = locale.Code + "/";
          break;
      }
    }
    return localePathModifier;
  }

  private static string GetLocalePathSuffix() => MotdServerClient.GetLocalePathModifier() + "motd.json";

  public void GetMotd(Action<MotdServerClient.MotdResponse, string> cb)
  {
    this.m_callback = cb;
    MotdServerClient.MotdResponse localResponse = this.GetLocalMotd(MotdServerClient.MotdLocalPath);
    this.GetWebMotd(MotdServerClient.MotdServerUrl, localResponse, (Action<MotdServerClient.MotdResponse, string>) ((response, err) =>
    {
      MotdServerClient.MotdResponse response1;
      if (err == null)
      {
        Debug.Assert(Object.op_Inequality((Object) response.image_texture, (Object) null), (object) "Attempting to return response with no image texture");
        response1 = response;
      }
      else
      {
        Debug.LogWarning((object) ("Could not retrieve web motd from " + MotdServerClient.MotdServerUrl + ", falling back to local - err: " + err));
        response1 = localResponse;
      }
      if (Localization.GetSelectedLanguageType() == Localization.SelectedLanguageType.UGC)
      {
        Debug.Log((object) "Language Mod detected, MOTD strings falling back to local file");
        response1.image_header_text = (string) UI.FRONTEND.MOTD.IMAGE_HEADER;
        response1.news_header_text = (string) UI.FRONTEND.MOTD.NEWS_HEADER;
        response1.news_body_text = (string) UI.FRONTEND.MOTD.NEWS_BODY;
        response1.patch_notes_summary = (string) UI.FRONTEND.MOTD.PATCH_NOTES_SUMMARY;
        response1.vanilla_update_data.update_text_override = (string) UI.FRONTEND.MOTD.UPDATE_TEXT;
        response1.expansion1_update_data.update_text_override = (string) UI.FRONTEND.MOTD.UPDATE_TEXT_EXPANSION1;
      }
      this.doCallback(response1, (string) null);
    }));
  }

  private MotdServerClient.MotdResponse GetLocalMotd(string filePath)
  {
    this.m_localMotd = JsonConvert.DeserializeObject<MotdServerClient.MotdResponse>(((object) Resources.Load<TextAsset>(filePath.Replace(".json", ""))).ToString());
    string str1 = MotdServerClient.MotdLocalImagePath(this.m_localMotd.image_version);
    this.m_localMotd.image_texture = Resources.Load<Texture2D>(str1);
    if (Object.op_Equality((Object) this.m_localMotd.image_texture, (Object) null))
    {
      string str2 = MotdServerClient.FallbackMotdLocalImagePath(this.m_localMotd.image_version);
      if (str2 != str1)
      {
        Debug.Log((object) ("Could not load " + str1 + ", falling back to " + str2));
        str1 = str2;
        this.m_localMotd.image_texture = Resources.Load<Texture2D>(str1);
      }
    }
    Debug.Assert(Object.op_Inequality((Object) this.m_localMotd.image_texture, (Object) null), (object) ("Failed to load " + str1));
    return this.m_localMotd;
  }

  private void GetWebMotd(
    string url,
    MotdServerClient.MotdResponse localMotd,
    Action<MotdServerClient.MotdResponse, string> cb)
  {
    Action<string, string> cb1 = (Action<string, string>) ((response, err) =>
    {
      DebugUtil.DevAssert(Object.op_Inequality((Object) localMotd.image_texture, (Object) null), "Local MOTD image_texture is no longer loaded", (Object) null);
      if (Object.op_Equality((Object) localMotd.image_texture, (Object) null))
        cb((MotdServerClient.MotdResponse) null, "Local image_texture has been unloaded since we requested the MOTD");
      else if (err != null)
      {
        cb((MotdServerClient.MotdResponse) null, err);
      }
      else
      {
        MotdServerClient.MotdResponse responseStruct = JsonConvert.DeserializeObject<MotdServerClient.MotdResponse>(response, new JsonSerializerSettings()
        {
          Error = (EventHandler<ErrorEventArgs>) ((sender, args) => args.ErrorContext.Handled = true)
        });
        if (responseStruct == null)
          cb((MotdServerClient.MotdResponse) null, "Invalid json from server:" + response);
        else if (responseStruct.version <= localMotd.version)
        {
          int version = localMotd.version;
          string str4 = version.ToString();
          version = responseStruct.version;
          string str5 = version.ToString();
          Debug.Log((object) ("Using local MOTD at version: " + str4 + ", web version at " + str5));
          cb(localMotd, (string) null);
        }
        else
          SimpleNetworkCache.LoadFromCacheOrDownload("motd_image", responseStruct.image_url, responseStruct.image_version, new UnityWebRequest()
          {
            downloadHandler = (DownloadHandler) new DownloadHandlerTexture()
          }, (Action<UnityWebRequest>) (wr =>
          {
            string str6 = (string) null;
            if (string.IsNullOrEmpty(wr.error))
            {
              int version = responseStruct.version;
              string str7 = version.ToString();
              version = localMotd.version;
              string str8 = version.ToString();
              Debug.Log((object) ("Using web MOTD at version: " + str7 + ", local version at " + str8));
              responseStruct.image_texture = DownloadHandlerTexture.GetContent(wr);
            }
            else
              str6 = "Failed to load image: " + responseStruct.image_url + " SimpleNetworkCache - " + wr.error;
            cb(responseStruct, str6);
            wr.Dispose();
          }));
      }
    });
    this.getAsyncRequest(url, cb1);
  }

  private void getAsyncRequest(string url, Action<string, string> cb)
  {
    UnityWebRequest motdRequest = UnityWebRequest.Get(url);
    motdRequest.SetRequestHeader("Content-Type", "application/json");
    ((AsyncOperation) motdRequest.SendWebRequest()).completed += (Action<AsyncOperation>) (operation =>
    {
      cb(motdRequest.downloadHandler.text, motdRequest.error);
      motdRequest.Dispose();
    });
  }

  public void UnregisterCallback() => this.m_callback = (Action<MotdServerClient.MotdResponse, string>) null;

  private void doCallback(MotdServerClient.MotdResponse response, string error)
  {
    if (this.m_callback != null)
      this.m_callback(response, error);
    else
      Debug.Log((object) "Motd Response receieved, but callback was unregistered");
  }

  public class MotdUpdateData
  {
    public string last_update_time { get; set; }

    public string next_update_time { get; set; }

    public string update_text_override { get; set; }
  }

  public class MotdResponse
  {
    public int version { get; set; }

    public string image_header_text { get; set; }

    public int image_version { get; set; }

    public string image_url { get; set; }

    public string image_link_url { get; set; }

    public string image_rail_link_url { get; set; }

    public string news_header_text { get; set; }

    public string news_body_text { get; set; }

    public string patch_notes_summary { get; set; }

    public string patch_notes_link_url { get; set; }

    public string patch_notes_rail_link_url { get; set; }

    public MotdServerClient.MotdUpdateData vanilla_update_data { get; set; }

    public MotdServerClient.MotdUpdateData expansion1_update_data { get; set; }

    public string latest_update_build { get; set; }

    [JsonIgnore]
    public Texture2D image_texture { get; set; }
  }
}
