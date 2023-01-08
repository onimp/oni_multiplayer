// Decompiled with JetBrains decompiler
// Type: KMod.Mod
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;

namespace KMod
{
  [JsonObject]
  [DebuggerDisplay("{title}")]
  public class Mod
  {
    public const int MOD_API_VERSION_NONE = 0;
    public const int MOD_API_VERSION_HARMONY1 = 1;
    public const int MOD_API_VERSION_HARMONY2 = 2;
    public const int MOD_API_VERSION = 2;
    [JsonProperty]
    public Label label;
    [JsonProperty]
    public Mod.Status status;
    [JsonProperty]
    public bool enabled;
    [JsonProperty]
    public List<string> enabledForDlc;
    [JsonProperty]
    public int crash_count;
    [JsonProperty]
    public string reinstall_path;
    public bool foundInStackTrace;
    public string relative_root = "";
    public Mod.PackagedModInfo packagedModInfo;
    public LoadedModData loaded_mod_data;
    public IFileSource file_source;
    public IFileSource content_source;
    public bool is_subscribed;
    private const string VANILLA_ID = "vanilla_id";
    private const string ALL_ID = "all";
    private const string ARCHIVED_VERSIONS_FOLDER = "archived_versions";
    private const string MOD_INFO_FILENAME = "mod_info.yaml";
    public ModContentCompatability contentCompatability;
    public const int MAX_CRASH_COUNT = 3;
    private static readonly List<string> PREVIEW_FILENAMES = new List<string>()
    {
      "preview.png",
      "Preview.png",
      "PREVIEW.PNG"
    };

    public Content available_content { get; private set; }

    [JsonProperty]
    public string staticID { get; private set; }

    public LocString manage_tooltip { get; private set; }

    public System.Action on_managed { get; private set; }

    public bool is_managed => this.manage_tooltip != null;

    public string title => this.label.title;

    public string description { get; private set; }

    public Content loaded_content { get; private set; }

    public bool DevModCrashTriggered { get; private set; }

    [JsonConstructor]
    public Mod()
    {
    }

    public void CopyPersistentDataTo(Mod other_mod)
    {
      other_mod.status = this.status;
      other_mod.enabledForDlc = this.enabledForDlc != null ? new List<string>((IEnumerable<string>) this.enabledForDlc) : new List<string>();
      other_mod.crash_count = this.crash_count;
      other_mod.loaded_content = this.loaded_content;
      other_mod.loaded_mod_data = this.loaded_mod_data;
      other_mod.reinstall_path = this.reinstall_path;
    }

    public Mod(
      Label label,
      string staticID,
      string description,
      IFileSource file_source,
      LocString manage_tooltip,
      System.Action on_managed)
    {
      this.label = label;
      this.status = Mod.Status.NotInstalled;
      this.staticID = staticID;
      this.description = description;
      this.file_source = file_source;
      this.manage_tooltip = manage_tooltip;
      this.on_managed = on_managed;
      this.loaded_content = (Content) 0;
      this.available_content = (Content) 0;
      this.ScanContent();
    }

    public bool IsEnabledForActiveDlc() => this.IsEnabledForDlc(DlcManager.GetHighestActiveDlcId());

    public bool IsEnabledForDlc(string dlcId) => this.enabledForDlc != null && this.enabledForDlc.Contains(dlcId);

    public void SetEnabledForActiveDlc(bool enabled) => this.SetEnabledForDlc(DlcManager.GetHighestActiveDlcId(), enabled);

    public void SetEnabledForDlc(string dlcId, bool set_enabled)
    {
      if (this.enabledForDlc == null)
        this.enabledForDlc = new List<string>();
      bool flag = this.enabledForDlc.Contains(dlcId);
      if (set_enabled && !flag)
      {
        this.enabledForDlc.Add(dlcId);
      }
      else
      {
        if (!(!set_enabled & flag))
          return;
        this.enabledForDlc.Remove(dlcId);
      }
    }

    public void ScanContent()
    {
      this.ModDevLog(string.Format("{0} ({1}): Setting up mod.", (object) this.label, (object) this.label.id));
      this.available_content = (Content) 0;
      if (this.file_source == null)
      {
        if (this.label.id.EndsWith(".zip"))
        {
          DebugUtil.DevAssert(false, "Does this actually get used ever?", (Object) null);
          this.file_source = (IFileSource) new ZipFile(this.label.install_path);
        }
        else
          this.file_source = (IFileSource) new Directory(this.label.install_path);
      }
      if (!this.file_source.Exists())
      {
        Debug.LogWarning((object) string.Format("{0}: File source does not appear to be valid, skipping. ({1})", (object) this.label, (object) this.label.install_path));
      }
      else
      {
        KModHeader header = KModUtil.GetHeader(this.file_source, this.label.defaultStaticID, this.label.title, this.description, this.IsDev);
        if (this.label.title != header.title)
          Debug.Log((object) ("\t" + this.label.title + " has a mod.yaml with the title `" + header.title + "`, using that from now on."));
        if (this.label.defaultStaticID != header.staticID)
          Debug.Log((object) ("\t" + this.label.title + " has a mod.yaml with a staticID `" + header.staticID + "`, using that from now on."));
        this.label.title = header.title;
        this.staticID = header.staticID;
        this.description = header.description;
        Mod.ArchivedVersion mostSuitableArchive = this.GetMostSuitableArchive();
        if (mostSuitableArchive == null)
        {
          Debug.LogWarning((object) string.Format("{0}: No archive supports this game version, skipping content.", (object) this.label));
          this.contentCompatability = ModContentCompatability.DoesntSupportDLCConfig;
          this.available_content = (Content) 0;
          this.SetEnabledForActiveDlc(false);
        }
        else
        {
          this.packagedModInfo = mostSuitableArchive.info;
          Content available;
          this.ScanContentFromSource(mostSuitableArchive.relativePath, out available);
          if (available == (Content) 0)
          {
            Debug.LogWarning((object) string.Format("{0}: No supported content for mod, skipping content.", (object) this.label));
            this.contentCompatability = ModContentCompatability.NoContent;
            this.available_content = (Content) 0;
            this.SetEnabledForActiveDlc(false);
          }
          else
          {
            bool flag = mostSuitableArchive.info.APIVersion == 2;
            if ((available & Content.DLL) != (Content) 0 && !flag)
            {
              Debug.LogWarning((object) string.Format("{0}: DLLs found but not using the correct API version.", (object) this.label));
              this.contentCompatability = ModContentCompatability.OldAPI;
              this.available_content = (Content) 0;
              this.SetEnabledForActiveDlc(false);
            }
            else
            {
              this.contentCompatability = ModContentCompatability.OK;
              this.available_content = available;
              this.relative_root = mostSuitableArchive.relativePath;
              Debug.Assert(this.content_source == null);
              this.content_source = (IFileSource) new Directory(this.ContentPath);
              Debug.Log((object) string.Format("{0}: Successfully loaded from path '{1}' with content '{2}'.", (object) this.label, string.IsNullOrEmpty(this.relative_root) ? (object) "root" : (object) this.relative_root, (object) this.available_content.ToString()));
            }
          }
        }
      }
    }

    private Mod.ArchivedVersion GetMostSuitableArchive()
    {
      Mod.PackagedModInfo mod_info = this.GetModInfoForFolder("");
      if (mod_info == null)
      {
        mod_info = new Mod.PackagedModInfo()
        {
          supportedContent = "vanilla_id",
          minimumSupportedBuild = 0
        };
        if (this.ScanContentFromSourceForTranslationsOnly(""))
        {
          this.ModDevLogWarning(string.Format("{0}: No mod_info.yaml found, but since it contains a translation, default its supported content to 'ALL'", (object) this.label));
          mod_info.supportedContent = "all";
        }
        else
          this.ModDevLogWarning(string.Format("{0}: No mod_info.yaml found, default its supported content to 'VANILLA_ID'", (object) this.label));
      }
      Mod.ArchivedVersion archivedVersion = new Mod.ArchivedVersion()
      {
        relativePath = "",
        info = mod_info
      };
      if (!this.file_source.Exists("archived_versions"))
      {
        this.ModDevLog(string.Format("\t{0}: No archived_versions for this mod, using root version directly.", (object) this.label));
        return !this.DoesModSupportCurrentContent(mod_info) ? (Mod.ArchivedVersion) null : archivedVersion;
      }
      List<FileSystemItem> file_system_items = new List<FileSystemItem>();
      this.file_source.GetTopLevelItems(file_system_items, "archived_versions");
      if (file_system_items.Count == 0)
      {
        this.ModDevLog(string.Format("\t{0}: No archived_versions for this mod, using root version directly.", (object) this.label));
        return !this.DoesModSupportCurrentContent(mod_info) ? (Mod.ArchivedVersion) null : archivedVersion;
      }
      List<Mod.ArchivedVersion> source = new List<Mod.ArchivedVersion>();
      source.Add(archivedVersion);
      foreach (FileSystemItem fileSystemItem in file_system_items)
      {
        string relative_root = System.IO.Path.Combine("archived_versions", fileSystemItem.name);
        Mod.PackagedModInfo modInfoForFolder = this.GetModInfoForFolder(relative_root);
        if (modInfoForFolder != null)
          source.Add(new Mod.ArchivedVersion()
          {
            relativePath = relative_root,
            info = modInfoForFolder
          });
      }
      return source.Where<Mod.ArchivedVersion>((Func<Mod.ArchivedVersion, bool>) (v => this.DoesModSupportCurrentContent(v.info))).ToList<Mod.ArchivedVersion>().Where<Mod.ArchivedVersion>((Func<Mod.ArchivedVersion, bool>) (v => v.info.APIVersion == 2 || v.info.APIVersion == 0)).ToList<Mod.ArchivedVersion>().Where<Mod.ArchivedVersion>((Func<Mod.ArchivedVersion, bool>) (v => v.info.minimumSupportedBuild <= 535842)).OrderByDescending<Mod.ArchivedVersion, int>((Func<Mod.ArchivedVersion, int>) (v => v.info.minimumSupportedBuild)).FirstOrDefault<Mod.ArchivedVersion>() ?? (Mod.ArchivedVersion) null;
    }

    private Mod.PackagedModInfo GetModInfoForFolder(string relative_root)
    {
      List<FileSystemItem> file_system_items = new List<FileSystemItem>();
      this.file_source.GetTopLevelItems(file_system_items, relative_root);
      bool flag = false;
      foreach (FileSystemItem fileSystemItem in file_system_items)
      {
        if (fileSystemItem.type == FileSystemItem.ItemType.File && fileSystemItem.name.ToLower() == "mod_info.yaml")
        {
          flag = true;
          break;
        }
      }
      string str1 = string.IsNullOrEmpty(relative_root) ? "root" : relative_root;
      if (!flag)
      {
        this.ModDevLogWarning("\t" + this.title + ": has no mod_info.yaml in folder '" + str1 + "'");
        return (Mod.PackagedModInfo) null;
      }
      string str2 = this.file_source.Read(System.IO.Path.Combine(relative_root, "mod_info.yaml"));
      if (string.IsNullOrEmpty(str2))
      {
        this.ModDevLogError(string.Format("\t{0}: Failed to read {1} in folder '{2}', skipping", (object) this.label, (object) "mod_info.yaml", (object) str1));
        return (Mod.PackagedModInfo) null;
      }
      // ISSUE: method pointer
      YamlIO.ErrorHandler errorHandler = new YamlIO.ErrorHandler((object) this, __methodptr(\u003CGetModInfoForFolder\u003Eb__66_0));
      Mod.PackagedModInfo modInfoForFolder = YamlIO.Parse<Mod.PackagedModInfo>(str2, new FileHandle(), errorHandler, (List<Tuple<string, System.Type>>) null);
      if (modInfoForFolder == null)
      {
        this.ModDevLogError(string.Format("\t{0}: Failed to parse {1} in folder '{2}', text is {3}", (object) this.label, (object) "mod_info.yaml", (object) str1, (object) str2));
        return (Mod.PackagedModInfo) null;
      }
      if (modInfoForFolder.supportedContent == null)
      {
        this.ModDevLogError(string.Format("\t{0}: {1} in folder '{2}' does not specify supportedContent. Make sure you spelled it correctly in your mod_info!", (object) this.label, (object) "mod_info.yaml", (object) str1));
        return (Mod.PackagedModInfo) null;
      }
      if (modInfoForFolder.lastWorkingBuild != 0)
      {
        this.ModDevLogError(string.Format("\t{0}: {1} in folder '{2}' is using `{3}`, please upgrade this to `{4}`", (object) this.label, (object) "mod_info.yaml", (object) str1, (object) "lastWorkingBuild", (object) "minimumSupportedBuild"));
        if (modInfoForFolder.minimumSupportedBuild == 0)
          modInfoForFolder.minimumSupportedBuild = modInfoForFolder.lastWorkingBuild;
      }
      this.ModDevLog(string.Format("\t{0}: Found valid mod_info.yaml in folder '{1}': {2} at {3}", (object) this.label, (object) str1, (object) modInfoForFolder.supportedContent, (object) modInfoForFolder.minimumSupportedBuild));
      return modInfoForFolder;
    }

    private bool DoesModSupportCurrentContent(Mod.PackagedModInfo mod_info)
    {
      string str = DlcManager.GetHighestActiveDlcId();
      if (str == "")
        str = "vanilla_id";
      string lower1 = str.ToLower();
      string lower2 = mod_info.supportedContent.ToLower();
      return lower2.Contains(lower1) || lower2.Contains("all");
    }

    private bool ScanContentFromSourceForTranslationsOnly(string relativeRoot)
    {
      this.available_content = (Content) 0;
      List<FileSystemItem> file_system_items = new List<FileSystemItem>();
      this.file_source.GetTopLevelItems(file_system_items, relativeRoot);
      foreach (FileSystemItem fileSystemItem in file_system_items)
      {
        if (fileSystemItem.type == FileSystemItem.ItemType.File && fileSystemItem.name.ToLower().EndsWith(".po"))
          this.available_content |= Content.Translation;
      }
      return this.available_content != 0;
    }

    private bool ScanContentFromSource(string relativeRoot, out Content available)
    {
      available = (Content) 0;
      List<FileSystemItem> file_system_items = new List<FileSystemItem>();
      this.file_source.GetTopLevelItems(file_system_items, relativeRoot);
      foreach (FileSystemItem fileSystemItem in file_system_items)
      {
        if (fileSystemItem.type == FileSystemItem.ItemType.Directory)
        {
          string lower = fileSystemItem.name.ToLower();
          available |= this.AddDirectory(lower);
        }
        else
        {
          string lower = fileSystemItem.name.ToLower();
          available |= this.AddFile(lower);
        }
      }
      return available != 0;
    }

    public string ContentPath => System.IO.Path.Combine(this.label.install_path, this.relative_root);

    public bool IsEmpty() => this.available_content == (Content) 0;

    private Content AddDirectory(string directory)
    {
      Content content = (Content) 0;
      switch (directory.TrimEnd('/'))
      {
        case "anim":
          content |= Content.Animation;
          break;
        case "buildingfacades":
          content |= Content.Animation;
          break;
        case "codex":
          content |= Content.LayerableFiles;
          break;
        case "elements":
          content |= Content.LayerableFiles;
          break;
        case "strings":
          content |= Content.Strings;
          break;
        case "templates":
          content |= Content.LayerableFiles;
          break;
        case "worldgen":
          content |= Content.LayerableFiles;
          break;
      }
      return content;
    }

    private Content AddFile(string file)
    {
      Content content = (Content) 0;
      if (file.EndsWith(".dll"))
        content |= Content.DLL;
      if (file.EndsWith(".po"))
        content |= Content.Translation;
      return content;
    }

    private static void AccumulateExtensions(Content content, List<string> extensions)
    {
      if ((content & Content.DLL) != (Content) 0)
        extensions.Add(".dll");
      if ((content & (Content.Strings | Content.Translation)) == (Content) 0)
        return;
      extensions.Add(".po");
    }

    [Conditional("DEBUG")]
    private void Assert(bool condition, string failure_message)
    {
      if (string.IsNullOrEmpty(this.title))
        DebugUtil.Assert(condition, string.Format("{2}\n\t{0}\n\t{1}", (object) this.title, (object) this.label.ToString(), (object) failure_message));
      else
        DebugUtil.Assert(condition, string.Format("{1}\n\t{0}", (object) this.label.ToString(), (object) failure_message));
    }

    public void Install()
    {
      if (this.IsLocal)
      {
        this.status = Mod.Status.Installed;
      }
      else
      {
        this.status = Mod.Status.ReinstallPending;
        if (this.file_source == null || !FileUtil.DeleteDirectory(this.label.install_path, 0) || !FileUtil.CreateDirectory(this.label.install_path, 0))
          return;
        this.file_source.CopyTo(this.label.install_path);
        this.file_source = (IFileSource) new Directory(this.label.install_path);
        this.status = Mod.Status.Installed;
      }
    }

    public bool Uninstall()
    {
      this.SetEnabledForActiveDlc(false);
      if (this.loaded_content != (Content) 0)
      {
        Debug.Log((object) string.Format("Can't uninstall {0}: still has loaded content: {1}", (object) this.label.ToString(), (object) this.loaded_content.ToString()));
        this.status = Mod.Status.UninstallPending;
        return false;
      }
      if (!this.IsLocal && !FileUtil.DeleteDirectory(this.label.install_path, 0))
      {
        Debug.Log((object) string.Format("Can't uninstall {0}: directory deletion failed", (object) this.label.ToString()));
        this.status = Mod.Status.UninstallPending;
        return false;
      }
      this.status = Mod.Status.NotInstalled;
      return true;
    }

    private bool LoadStrings()
    {
      string path = FileSystem.Normalize(System.IO.Path.Combine(this.ContentPath, "strings"));
      if (!System.IO.Directory.Exists(path))
        return false;
      int num = 0;
      foreach (FileInfo file in new DirectoryInfo(path).GetFiles())
      {
        if (!(file.Extension.ToLower() != ".po"))
        {
          ++num;
          Localization.OverloadStrings(Localization.LoadStringsFile(file.FullName, false));
        }
      }
      return true;
    }

    private bool LoadTranslations() => false;

    private bool LoadAnimation()
    {
      string path = FileSystem.Normalize(System.IO.Path.Combine(this.ContentPath, "anim"));
      if (!System.IO.Directory.Exists(path))
        return false;
      int num = 0;
      foreach (DirectoryInfo directory1 in new DirectoryInfo(path).GetDirectories())
      {
        foreach (DirectoryInfo directory2 in directory1.GetDirectories())
        {
          KAnimFile.Mod anim_mod = new KAnimFile.Mod();
          foreach (FileInfo file in directory2.GetFiles())
          {
            if (file.Extension == ".png")
            {
              byte[] numArray = File.ReadAllBytes(file.FullName);
              Texture2D texture2D = new Texture2D(2, 2);
              ImageConversion.LoadImage(texture2D, numArray);
              anim_mod.textures.Add(texture2D);
            }
            else if (file.Extension == ".bytes")
            {
              string withoutExtension = System.IO.Path.GetFileNameWithoutExtension(file.Name);
              byte[] numArray = File.ReadAllBytes(file.FullName);
              if (withoutExtension.EndsWith("_anim"))
                anim_mod.anim = numArray;
              else if (withoutExtension.EndsWith("_build"))
                anim_mod.build = numArray;
              else
                DebugUtil.LogWarningArgs(new object[1]
                {
                  (object) string.Format("Unhandled TextAsset ({0})...ignoring", (object) file.FullName)
                });
            }
            else
              DebugUtil.LogWarningArgs(new object[1]
              {
                (object) string.Format("Unhandled asset ({0})...ignoring", (object) file.FullName)
              });
          }
          string name = directory2.Name + "_kanim";
          if (anim_mod.IsValid() && Object.op_Implicit((Object) ModUtil.AddKAnimMod(name, anim_mod)))
            ++num;
        }
      }
      return true;
    }

    public void Load(Content content)
    {
      content &= this.available_content & ~this.loaded_content;
      if (content > (Content) 0)
        Debug.Log((object) string.Format("Loading mod content {2} [{0}:{1}] (provides {3})", (object) this.title, (object) this.label.id, (object) content.ToString(), (object) this.available_content.ToString()));
      if ((content & Content.Strings) != (Content) 0 && this.LoadStrings())
        this.loaded_content |= Content.Strings;
      if ((content & Content.Translation) != (Content) 0 && this.LoadTranslations())
        this.loaded_content |= Content.Translation;
      if ((content & Content.DLL) != (Content) 0)
      {
        this.loaded_mod_data = DLLLoader.LoadDLLs(this, this.staticID, this.ContentPath, this.IsDev);
        if (this.loaded_mod_data != null)
          this.loaded_content |= Content.DLL;
      }
      if ((content & Content.LayerableFiles) != (Content) 0)
      {
        Debug.Assert(this.content_source != null, (object) "Attempting to Load layerable files with content_source not initialized");
        FileSystem.file_sources.Insert(0, this.content_source.GetFileSystem());
        this.loaded_content |= Content.LayerableFiles;
      }
      if ((content & Content.Animation) == (Content) 0 || !this.LoadAnimation())
        return;
      this.loaded_content |= Content.Animation;
    }

    public void PostLoad(IReadOnlyList<Mod> mods)
    {
      if ((this.loaded_content & Content.DLL) == (Content) 0 || this.loaded_mod_data == null)
        return;
      DLLLoader.PostLoadDLLs(this.staticID, this.loaded_mod_data, mods);
    }

    public void Unload(Content content)
    {
      content &= this.loaded_content;
      if ((content & Content.LayerableFiles) == (Content) 0)
        return;
      FileSystem.file_sources.Remove(this.content_source.GetFileSystem());
      this.loaded_content &= ~Content.LayerableFiles;
    }

    private void SetCrashCount(int new_crash_count) => this.crash_count = MathUtil.Clamp(0, 3, new_crash_count);

    public bool IsDev => this.label.distribution_platform == Label.DistributionPlatform.Dev;

    public bool IsLocal => this.label.distribution_platform == Label.DistributionPlatform.Dev || this.label.distribution_platform == Label.DistributionPlatform.Local;

    public void SetCrashed()
    {
      this.SetCrashCount(this.crash_count + 1);
      if (this.IsDev)
        return;
      this.SetEnabledForActiveDlc(false);
    }

    public void Uncrash() => this.SetCrashCount(this.IsDev ? this.crash_count - 1 : 0);

    public bool IsActive() => this.loaded_content != 0;

    public bool AllActive(Content content) => (this.loaded_content & content) == content;

    public bool AllActive() => (this.loaded_content & this.available_content) == this.available_content;

    public bool AnyActive(Content content) => (this.loaded_content & content) != 0;

    public bool HasContent() => this.available_content != 0;

    public bool HasAnyContent(Content content) => (this.available_content & content) != 0;

    public bool HasOnlyTranslationContent() => this.available_content == Content.Translation;

    public Texture2D GetPreviewImage()
    {
      string path2_1 = (string) null;
      foreach (string path2_2 in Mod.PREVIEW_FILENAMES)
      {
        if (System.IO.Directory.Exists(this.ContentPath) && File.Exists(System.IO.Path.Combine(this.ContentPath, path2_2)))
        {
          path2_1 = path2_2;
          break;
        }
      }
      if (path2_1 == null)
        return (Texture2D) null;
      try
      {
        byte[] numArray = File.ReadAllBytes(System.IO.Path.Combine(this.ContentPath, path2_1));
        Texture2D previewImage = new Texture2D(2, 2);
        ImageConversion.LoadImage(previewImage, numArray);
        return previewImage;
      }
      catch
      {
        Debug.LogWarning((object) string.Format("Mod {0} seems to have a preview.png but it didn't load correctly.", (object) this.label));
        return (Texture2D) null;
      }
    }

    public void ModDevLog(string msg)
    {
      if (!this.IsDev)
        return;
      Debug.Log((object) msg);
    }

    public void ModDevLogWarning(string msg)
    {
      if (!this.IsDev)
        return;
      Debug.LogWarning((object) msg);
    }

    public void ModDevLogError(string msg)
    {
      if (!this.IsDev)
        return;
      this.DevModCrashTriggered = true;
      Debug.LogError((object) msg);
    }

    public enum Status
    {
      NotInstalled,
      Installed,
      UninstallPending,
      ReinstallPending,
    }

    public class ArchivedVersion
    {
      public string relativePath;
      public Mod.PackagedModInfo info;
    }

    public class PackagedModInfo
    {
      public string supportedContent { get; set; }

      [Obsolete("Use minimumSupportedBuild instead!")]
      public int lastWorkingBuild { get; set; }

      public int minimumSupportedBuild { get; set; }

      public int APIVersion { get; set; }

      public string version { get; set; }
    }
  }
}
