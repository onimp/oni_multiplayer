// Decompiled with JetBrains decompiler
// Type: OpenURLButtons
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/OpenURLButtons")]
public class OpenURLButtons : KMonoBehaviour
{
  public GameObject buttonPrefab;
  public List<OpenURLButtons.URLButtonData> buttonData;
  [SerializeField]
  private GameObject patchNotesScreenPrefab;
  [SerializeField]
  private FeedbackScreen feedbackScreenPrefab;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    for (int index = 0; index < this.buttonData.Count; ++index)
    {
      OpenURLButtons.URLButtonData data = this.buttonData[index];
      GameObject gameObject = Util.KInstantiateUI(this.buttonPrefab, ((Component) this).gameObject, true);
      string str = StringEntry.op_Implicit(Strings.Get(data.stringKey));
      ((TMP_Text) gameObject.GetComponentInChildren<LocText>()).SetText(str);
      switch (data.urlType)
      {
        case OpenURLButtons.URLButtonType.url:
          gameObject.GetComponent<KButton>().onClick += (System.Action) (() => this.OpenURL(data.url));
          break;
        case OpenURLButtons.URLButtonType.platformUrl:
          gameObject.GetComponent<KButton>().onClick += (System.Action) (() => this.OpenPlatformURL(data.url));
          break;
        case OpenURLButtons.URLButtonType.patchNotes:
          gameObject.GetComponent<KButton>().onClick += (System.Action) (() => this.OpenPatchNotes());
          break;
        case OpenURLButtons.URLButtonType.feedbackScreen:
          gameObject.GetComponent<KButton>().onClick += (System.Action) (() => this.OpenFeedbackScreen());
          break;
      }
    }
  }

  public void OpenPatchNotes() => Util.KInstantiateUI(this.patchNotesScreenPrefab, ((Component) FrontEndManager.Instance).gameObject, true);

  public void OpenFeedbackScreen() => Util.KInstantiateUI(((Component) this.feedbackScreenPrefab).gameObject, ((Component) FrontEndManager.Instance).gameObject, true);

  public void OpenURL(string URL) => App.OpenWebURL(URL);

  public void OpenPlatformURL(string URL)
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    OpenURLButtons.\u003C\u003Ec__DisplayClass10_0 cDisplayClass100 = new OpenURLButtons.\u003C\u003Ec__DisplayClass10_0();
    // ISSUE: reference to a compiler-generated field
    cDisplayClass100.URL = URL;
    if (DistributionPlatform.Inst.Platform == "Steam" && DistributionPlatform.Inst.Initialized)
    {
      // ISSUE: method pointer
      DistributionPlatform.Inst.GetAuthTicket(new DistributionPlatform.AuthTicketHandler((object) cDisplayClass100, __methodptr(\u003COpenPlatformURL\u003Eb__0)));
    }
    else
    {
      // ISSUE: reference to a compiler-generated field
      App.OpenWebURL("https://accounts.klei.com/login?goto={gotoUrl}".Replace("{gotoUrl}", WebUtility.HtmlEncode(cDisplayClass100.URL.Replace("{SteamID}", "").Replace("{SteamTicket}", ""))));
    }
  }

  public enum URLButtonType
  {
    url,
    platformUrl,
    patchNotes,
    feedbackScreen,
  }

  [Serializable]
  public class URLButtonData
  {
    public string stringKey;
    public OpenURLButtons.URLButtonType urlType;
    public string url;
  }
}
