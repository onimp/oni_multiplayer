// Decompiled with JetBrains decompiler
// Type: KCrashReporter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class KCrashReporter : MonoBehaviour
{
  public static string MOST_RECENT_SAVEFILE = (string) null;
  public const string CRASH_REPORTER_SERVER = "http://crashes.klei.ca";
  public const uint MAX_LOGS = 10000000;
  public static bool ignoreAll = false;
  public static bool debugWasUsed = false;
  public static bool haveActiveMods = false;
  public static uint logCount = 0;
  public static string error_canvas_name = "ErrorCanvas";
  private static bool disableDeduping = false;
  public static bool hasCrash = false;
  private static readonly Regex failedToLoadModuleRegEx = new Regex("^Failed to load '(.*?)' with error (.*)", RegexOptions.Multiline);
  [SerializeField]
  private LoadScreen loadScreenPrefab;
  [SerializeField]
  private GameObject reportErrorPrefab;
  [SerializeField]
  private ConfirmDialogScreen confirmDialogPrefab;
  private GameObject errorScreen;
  public static bool terminateOnError = true;
  private static string dataRoot;
  private static readonly string[] IgnoreStrings = new string[5]
  {
    "Releasing render texture whose render buffer is set as Camera's target buffer with Camera.SetTargetBuffers!",
    "The profiler has run out of samples for this frame. This frame will be skipped. Increase the sample limit using Profiler.maxNumberOfSamplesPerFrame",
    "Trying to add Text (LocText) for graphic rebuild while we are already inside a graphic rebuild loop. This is not supported.",
    "Texture has out of range width / height",
    "<I> Failed to get cursor position:\r\nSuccess.\r\n"
  };
  private static HashSet<int> previouslyReportedDevNotifications;

  public static event Action<string> onCrashReported;

  public static bool hasReportedError { get; private set; }

  private void OnEnable()
  {
    KCrashReporter.dataRoot = Application.dataPath;
    // ISSUE: method pointer
    Application.logMessageReceived += new Application.LogCallback((object) this, __methodptr(HandleLog));
    KCrashReporter.ignoreAll = true;
    string path = System.IO.Path.Combine(KCrashReporter.dataRoot, "hashes.json");
    if (System.IO.File.Exists(path))
    {
      StringBuilder stringBuilder = new StringBuilder();
      MD5 md5 = MD5.Create();
      Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(System.IO.File.ReadAllText(path));
      if (dictionary.Count > 0)
      {
        bool flag = true;
        foreach (KeyValuePair<string, string> keyValuePair in dictionary)
        {
          string key = keyValuePair.Key;
          string str = keyValuePair.Value;
          stringBuilder.Length = 0;
          using (FileStream inputStream = new FileStream(System.IO.Path.Combine(KCrashReporter.dataRoot, key), FileMode.Open, FileAccess.Read))
          {
            foreach (byte num in md5.ComputeHash((Stream) inputStream))
              stringBuilder.AppendFormat("{0:x2}", (object) num);
            if (stringBuilder.ToString() != str)
            {
              flag = false;
              break;
            }
          }
        }
        if (flag)
          KCrashReporter.ignoreAll = false;
      }
      else
        KCrashReporter.ignoreAll = false;
    }
    else
      KCrashReporter.ignoreAll = false;
    if (KCrashReporter.ignoreAll)
      Debug.Log((object) "Ignoring crash due to mismatched hashes.json entries.");
    if (System.IO.File.Exists("ignorekcrashreporter.txt"))
    {
      KCrashReporter.ignoreAll = true;
      Debug.Log((object) "Ignoring crash due to ignorekcrashreporter.txt");
    }
    if (!Application.isEditor || GenericGameSettings.instance.enableEditorCrashReporting)
      return;
    KCrashReporter.terminateOnError = false;
  }

  private void OnDisable() => Application.logMessageReceived -= new Application.LogCallback((object) this, __methodptr(HandleLog));

  private void HandleLog(string msg, string stack_trace, LogType type)
  {
    if (++KCrashReporter.logCount == 10000000U)
    {
      DebugUtil.DevLogError("Turning off logging to avoid increasing the file to an unreasonable size, please review the logs as they probably contain spam");
      Debug.DisableLogging();
    }
    if (KCrashReporter.ignoreAll || Array.IndexOf<string>(KCrashReporter.IgnoreStrings, msg) != -1 || msg != null && msg.StartsWith("<RI.Hid>") || msg != null && msg.StartsWith("Failed to load cursor") || msg != null && msg.StartsWith("Failed to save a temporary cursor"))
      return;
    if (type == 4)
      RestartWarning.ShouldWarn = true;
    if (!Object.op_Equality((Object) this.errorScreen, (Object) null) || type != 4 && type != null || KCrashReporter.terminateOnError && KCrashReporter.hasCrash)
      return;
    if (Object.op_Inequality((Object) SpeedControlScreen.Instance, (Object) null))
      SpeedControlScreen.Instance.Pause(isCrashed: true);
    string error = msg;
    string stack_trace1 = stack_trace;
    if (string.IsNullOrEmpty(stack_trace1))
      stack_trace1 = new StackTrace(5, true).ToString();
    if (App.isLoading)
    {
      if (SceneInitializerLoader.deferred_error.IsValid)
        return;
      SceneInitializerLoader.deferred_error = new SceneInitializerLoader.DeferredError()
      {
        msg = error,
        stack_trace = stack_trace1
      };
    }
    else
      this.ShowDialog(error, stack_trace1);
  }

  public bool ShowDialog(string error, string stack_trace)
  {
    if (Object.op_Inequality((Object) this.errorScreen, (Object) null))
      return false;
    GameObject gameObject = GameObject.Find(KCrashReporter.error_canvas_name);
    if (Object.op_Equality((Object) gameObject, (Object) null))
    {
      gameObject = new GameObject();
      ((Object) gameObject).name = KCrashReporter.error_canvas_name;
      Canvas canvas = gameObject.AddComponent<Canvas>();
      canvas.renderMode = (RenderMode) 0;
      canvas.additionalShaderChannels = (AdditionalCanvasShaderChannels) 1;
      canvas.sortingOrder = (int) short.MaxValue;
      gameObject.AddComponent<GraphicRaycaster>();
    }
    this.errorScreen = Object.Instantiate<GameObject>(this.reportErrorPrefab, Vector3.zero, Quaternion.identity);
    this.errorScreen.transform.SetParent(gameObject.transform, false);
    ReportErrorDialog errorDialog = this.errorScreen.GetComponentInChildren<ReportErrorDialog>();
    string stackTrace = error + "\n\n" + stack_trace;
    KCrashReporter.hasCrash = true;
    if (Object.op_Inequality((Object) Global.Instance, (Object) null) && Global.Instance.modManager != null && Global.Instance.modManager.HasCrashableMods())
    {
      Exception e = DebugUtil.RetrieveLastExceptionLogged();
      Global.Instance.modManager.SearchForModsInStackTrace(e != null ? new StackTrace(e) : new StackTrace(5, true));
      Global.Instance.modManager.SearchForModsInStackTrace(stack_trace);
      errorDialog.PopupDisableModsDialog(stackTrace, new System.Action(this.OnQuitToDesktop), Global.Instance.modManager.IsInDevMode() || !KCrashReporter.terminateOnError ? new System.Action(this.OnCloseErrorDialog) : (System.Action) null);
    }
    else
      errorDialog.PopupSubmitErrorDialog(stackTrace, (System.Action) (() =>
      {
        string save_file_hash = (string) null;
        if (KCrashReporter.MOST_RECENT_SAVEFILE != null)
          save_file_hash = KCrashReporter.UploadSaveFile(KCrashReporter.MOST_RECENT_SAVEFILE, stack_trace);
        KCrashReporter.ReportError(error, stack_trace, save_file_hash, this.confirmDialogPrefab, this.errorScreen, errorDialog.UserMessage());
      }), new System.Action(this.OnQuitToDesktop), KCrashReporter.terminateOnError ? (System.Action) null : new System.Action(this.OnCloseErrorDialog));
    return true;
  }

  private void OnCloseErrorDialog()
  {
    Object.Destroy((Object) this.errorScreen);
    this.errorScreen = (GameObject) null;
    KCrashReporter.hasCrash = false;
    if (!Object.op_Inequality((Object) SpeedControlScreen.Instance, (Object) null))
      return;
    SpeedControlScreen.Instance.Unpause();
  }

  private void OnQuitToDesktop() => App.Quit();

  private static string UploadSaveFile(
    string save_file,
    string stack_trace,
    Dictionary<string, string> metadata = null)
  {
    Debug.Log((object) string.Format("Save_file: {0}", (object) save_file));
    if (KPrivacyPrefs.instance.disableDataCollection || save_file == null || !System.IO.File.Exists(save_file))
      return "";
    using (WebClient webClient = new WebClient())
    {
      Encoding utF8 = Encoding.UTF8;
      webClient.Encoding = utF8;
      byte[] numArray1 = System.IO.File.ReadAllBytes(save_file);
      string str1 = "----" + System.DateTime.Now.Ticks.ToString("x");
      webClient.Headers.Add("Content-Type", "multipart/form-data; boundary=" + str1);
      string str2 = "";
      string str3;
      using (SHA1CryptoServiceProvider cryptoServiceProvider = new SHA1CryptoServiceProvider())
        str3 = BitConverter.ToString(cryptoServiceProvider.ComputeHash(numArray1)).Replace("-", "");
      string str4 = str2 + string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n", (object) str1, (object) "hash", (object) str3);
      if (metadata != null)
      {
        string str5 = JsonConvert.SerializeObject((object) metadata);
        str4 += string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n", (object) str1, (object) nameof (metadata), (object) str5);
      }
      string s1 = str4 + string.Format("--{0}\r\nContent-Disposition: form-data; name=\"save\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n", new object[3]
      {
        (object) str1,
        (object) save_file,
        (object) "application/x-spss-sav"
      });
      byte[] bytes1 = utF8.GetBytes(s1);
      string s2 = string.Format("\r\n--{0}--\r\n", (object) str1);
      byte[] bytes2 = utF8.GetBytes(s2);
      byte[] numArray2 = new byte[bytes1.Length + numArray1.Length + bytes2.Length];
      Buffer.BlockCopy((Array) bytes1, 0, (Array) numArray2, 0, bytes1.Length);
      Buffer.BlockCopy((Array) numArray1, 0, (Array) numArray2, bytes1.Length, numArray1.Length);
      Buffer.BlockCopy((Array) bytes2, 0, (Array) numArray2, bytes1.Length + numArray1.Length, bytes2.Length);
      Uri address = new Uri("http://crashes.klei.ca/submitSave");
      try
      {
        webClient.UploadData(address, "POST", numArray2);
        return str3;
      }
      catch (Exception ex)
      {
        Debug.Log((object) ex);
        return "";
      }
    }
  }

  private static string GetUserID()
  {
    if (!DistributionPlatform.Initialized)
      return "LocalUser";
    return DistributionPlatform.Inst.Name + "ID_" + DistributionPlatform.Inst.LocalUser.Name + "_" + ((object) DistributionPlatform.Inst.LocalUser.Id)?.ToString();
  }

  private static string GetLogContents()
  {
    string path = Util.LogFilePath();
    if (!System.IO.File.Exists(path))
      return "";
    using (FileStream fileStream = System.IO.File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
    {
      using (StreamReader streamReader = new StreamReader((Stream) fileStream))
        return streamReader.ReadToEnd();
    }
  }

  public static void ReportErrorDevNotification(
    string notification_name,
    string stack_trace,
    string details = "")
  {
    if (KCrashReporter.previouslyReportedDevNotifications == null)
      KCrashReporter.previouslyReportedDevNotifications = new HashSet<int>();
    details = "DevNotification: " + notification_name + " - " + details;
    Debug.Log((object) details);
    HashedString hashedString = new HashedString(notification_name);
    int hashValue = ((HashedString) ref hashedString).HashValue;
    int num = KCrashReporter.hasReportedError ? 1 : 0;
    if (!KCrashReporter.previouslyReportedDevNotifications.Contains(hashValue))
    {
      KCrashReporter.previouslyReportedDevNotifications.Add(hashValue);
      KCrashReporter.ReportError("DevNotification: " + notification_name, stack_trace, (string) null, (ConfirmDialogScreen) null, (GameObject) null, details);
    }
    KCrashReporter.hasReportedError = num != 0;
  }

  public static void ReportError(
    string msg,
    string stack_trace,
    string save_file_hash,
    ConfirmDialogScreen confirm_prefab,
    GameObject confirm_parent,
    string userMessage = "")
  {
    if (KCrashReporter.ignoreAll)
      return;
    Debug.Log((object) "Reporting error.\n");
    if (msg != null)
      Debug.Log((object) msg);
    if (stack_trace != null)
      Debug.Log((object) stack_trace);
    KCrashReporter.hasReportedError = true;
    if (KPrivacyPrefs.instance.disableDataCollection)
      return;
    string str1;
    using (WebClient webClient = new WebClient())
    {
      webClient.Encoding = Encoding.UTF8;
      if (string.IsNullOrEmpty(msg))
        msg = "No message";
      Match match = KCrashReporter.failedToLoadModuleRegEx.Match(msg);
      if (match.Success)
      {
        string path = match.Groups[1].ToString();
        string str2 = match.Groups[2].ToString();
        msg = "Failed to load '" + System.IO.Path.GetFileName(path) + "' with error '" + str2 + "'.";
      }
      if (string.IsNullOrEmpty(stack_trace))
        stack_trace = string.Format("No stack trace {0}\n\n{1}", (object) BuildWatermark.GetBuildText(), (object) msg);
      List<string> stringList = new List<string>();
      if (KCrashReporter.debugWasUsed)
        stringList.Add("(Debug Used)");
      if (KCrashReporter.haveActiveMods)
        stringList.Add("(Mods Active)");
      stringList.Add(msg);
      string[] strArray = new string[8]
      {
        "Debug:LogError",
        "UnityEngine.Debug",
        "Output:LogError",
        "DebugUtil:Assert",
        "System.Array",
        "System.Collections",
        "KCrashReporter.Assert",
        "No stack trace."
      };
      string str3 = stack_trace;
      char[] chArray = new char[1]{ '\n' };
      foreach (string str4 in str3.Split(chArray))
      {
        if (stringList.Count < 5)
        {
          if (!string.IsNullOrEmpty(str4))
          {
            bool flag = false;
            foreach (string str5 in strArray)
            {
              if (str4.StartsWith(str5))
              {
                flag = true;
                break;
              }
            }
            if (!flag)
              stringList.Add(str4);
          }
        }
        else
          break;
      }
      if (userMessage == STRINGS.UI.CRASHSCREEN.BODY.text || Util.IsNullOrWhiteSpace(userMessage))
      {
        userMessage = "";
      }
      else
      {
        userMessage = "[" + BuildWatermark.GetBuildText() + "]" + userMessage;
        if (!string.IsNullOrEmpty(save_file_hash))
          userMessage = userMessage + "\nsave_hash: " + save_file_hash;
      }
      KCrashReporter.Error error = new KCrashReporter.Error();
      error.user = KCrashReporter.GetUserID();
      error.callstack = stack_trace;
      if (KCrashReporter.disableDeduping)
        error.callstack = error.callstack + "\n" + Guid.NewGuid().ToString();
      error.fullstack = string.Format("{0}\n\n{1}", (object) msg, (object) stack_trace);
      error.build = 535842;
      error.log = KCrashReporter.GetLogContents();
      error.summaryline = string.Join("\n", stringList.ToArray());
      error.user_message = userMessage;
      if (!string.IsNullOrEmpty(save_file_hash))
        error.save_hash = save_file_hash;
      if (DistributionPlatform.Initialized)
        error.steam64_verified = DistributionPlatform.Inst.LocalUser.Id.ToInt64();
      string data = JsonConvert.SerializeObject((object) error);
      string str6 = "";
      Uri address = new Uri("http://crashes.klei.ca/submitCrash");
      Debug.Log((object) "Submitting crash:");
      try
      {
        webClient.UploadStringAsync(address, data);
      }
      catch (Exception ex)
      {
        Debug.Log((object) ex);
      }
      if (Object.op_Inequality((Object) confirm_prefab, (Object) null) && Object.op_Inequality((Object) confirm_parent, (Object) null))
        ((ConfirmDialogScreen) KScreenManager.Instance.StartScreen(((Component) confirm_prefab).gameObject, confirm_parent)).PopupConfirmDialog((string) STRINGS.UI.CRASHSCREEN.REPORTEDERROR, (System.Action) null, (System.Action) null);
      str1 = str6;
    }
    if (KCrashReporter.onCrashReported == null)
      return;
    KCrashReporter.onCrashReported(str1);
  }

  public static void ReportBug(string msg, string save_file, GameObject confirmParent)
  {
    string stack_trace = "Bug Report From: " + KCrashReporter.GetUserID() + " at " + System.DateTime.Now.ToString();
    string save_file_hash = KCrashReporter.UploadSaveFile(save_file, stack_trace, new Dictionary<string, string>()
    {
      {
        "user",
        KCrashReporter.GetUserID()
      }
    });
    KCrashReporter.ReportError(msg, stack_trace, save_file_hash, ScreenPrefabs.Instance.ConfirmDialogScreen, confirmParent);
  }

  public static void Assert(bool condition, string message)
  {
    if (condition || KCrashReporter.hasReportedError)
      return;
    StackTrace stackTrace = new StackTrace(1, true);
    KCrashReporter.ReportError("ASSERT: " + message, stackTrace.ToString(), (string) null, (ConfirmDialogScreen) null, (GameObject) null);
  }

  public static void ReportSimDLLCrash(string msg, string stack_trace, string dmp_filename)
  {
    if (KCrashReporter.hasReportedError)
      return;
    string save_file_hash = (string) null;
    string str1 = (string) null;
    string str2 = (string) null;
    if (dmp_filename != null)
    {
      string withoutExtension = System.IO.Path.GetFileNameWithoutExtension(dmp_filename);
      str1 = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(KCrashReporter.dataRoot), dmp_filename);
      str2 = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(KCrashReporter.dataRoot), withoutExtension + ".sav");
      System.IO.File.Move(str1, str2);
      save_file_hash = KCrashReporter.UploadSaveFile(str2, stack_trace, new Dictionary<string, string>()
      {
        {
          "user",
          KCrashReporter.GetUserID()
        }
      });
    }
    KCrashReporter.ReportError(msg, stack_trace, save_file_hash, (ConfirmDialogScreen) null, (GameObject) null);
    if (dmp_filename == null)
      return;
    System.IO.File.Move(str2, str1);
  }

  private class Error
  {
    public string game = "simgame";
    public int build = -1;
    public string platform = Environment.OSVersion.ToString();
    public string user = "unknown";
    public ulong steam64_verified;
    public string callstack = "";
    public string fullstack = "";
    public string log = "";
    public string summaryline = "";
    public string user_message = "";
    public bool is_server;
    public bool is_dedicated;
    public string save_hash = "";
  }
}
