// Decompiled with JetBrains decompiler
// Type: PasteBaseTemplateScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using ProcGen;
using STRINGS;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class PasteBaseTemplateScreen : KScreen
{
  public static PasteBaseTemplateScreen Instance;
  public GameObject button_list_container;
  public GameObject prefab_paste_button;
  public GameObject prefab_directory_button;
  public KButton button_directory_up;
  public LocText directory_path_text;
  private List<GameObject> m_template_buttons = new List<GameObject>();
  private static readonly string NO_DIRECTORY = "NONE";
  private string m_CurrentDirectory = PasteBaseTemplateScreen.NO_DIRECTORY;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    PasteBaseTemplateScreen.Instance = this;
    TemplateCache.Init();
    this.button_directory_up.onClick += new System.Action(this.UpDirectory);
    this.ConsumeMouseScroll = true;
    this.RefreshStampButtons();
  }

  protected virtual void OnForcedCleanUp()
  {
    PasteBaseTemplateScreen.Instance = (PasteBaseTemplateScreen) null;
    ((KMonoBehaviour) this).OnForcedCleanUp();
  }

  [ContextMenu("Refresh")]
  public void RefreshStampButtons()
  {
    ((TMP_Text) this.directory_path_text).text = this.m_CurrentDirectory;
    this.button_directory_up.isInteractable = this.m_CurrentDirectory != PasteBaseTemplateScreen.NO_DIRECTORY;
    foreach (Object templateButton in this.m_template_buttons)
      Object.Destroy(templateButton);
    this.m_template_buttons.Clear();
    Debug.Log((object) ("Changing directory to " + this.m_CurrentDirectory));
    if (this.m_CurrentDirectory == PasteBaseTemplateScreen.NO_DIRECTORY)
    {
      ((TMP_Text) this.directory_path_text).text = "";
      foreach (string str in DlcManager.RELEASE_ORDER)
      {
        string dlcId = str;
        if (DlcManager.IsContentActive(dlcId))
        {
          GameObject gameObject = Util.KInstantiateUI(this.prefab_directory_button, this.button_list_container, true);
          gameObject.GetComponent<KButton>().onClick += (System.Action) (() => this.UpdateDirectory(SettingsCache.GetScope(dlcId)));
          ((TMP_Text) gameObject.GetComponentInChildren<LocText>()).text = dlcId == "" ? UI.DEBUG_TOOLS.SAVE_BASE_TEMPLATE.BASE_GAME_FOLDER_NAME.text : SettingsCache.GetScope(dlcId);
          this.m_template_buttons.Add(gameObject);
        }
      }
    }
    else
    {
      foreach (string directory in Directory.GetDirectories(TemplateCache.RewriteTemplatePath(this.m_CurrentDirectory)))
      {
        string directory_name = System.IO.Path.GetFileNameWithoutExtension(directory);
        GameObject gameObject = Util.KInstantiateUI(this.prefab_directory_button, this.button_list_container, true);
        gameObject.GetComponent<KButton>().onClick += (System.Action) (() => this.UpdateDirectory(directory_name));
        ((TMP_Text) gameObject.GetComponentInChildren<LocText>()).text = directory_name;
        this.m_template_buttons.Add(gameObject);
      }
      ListPool<FileHandle, PasteBaseTemplateScreen>.PooledList pooledList = ListPool<FileHandle, PasteBaseTemplateScreen>.Allocate();
      FileSystem.GetFiles(TemplateCache.RewriteTemplatePath(this.m_CurrentDirectory), "*.yaml", (ICollection<FileHandle>) pooledList);
      foreach (FileHandle fileHandle in (List<FileHandle>) pooledList)
      {
        string file_path_no_extension = System.IO.Path.GetFileNameWithoutExtension(fileHandle.full_path);
        GameObject gameObject = Util.KInstantiateUI(this.prefab_paste_button, this.button_list_container, true);
        gameObject.GetComponent<KButton>().onClick += (System.Action) (() => this.OnClickPasteButton(file_path_no_extension));
        ((TMP_Text) gameObject.GetComponentInChildren<LocText>()).text = file_path_no_extension;
        this.m_template_buttons.Add(gameObject);
      }
    }
  }

  private void UpdateDirectory(string relativePath)
  {
    if (this.m_CurrentDirectory == PasteBaseTemplateScreen.NO_DIRECTORY)
      this.m_CurrentDirectory = "";
    this.m_CurrentDirectory = FileSystem.CombineAndNormalize(new string[2]
    {
      this.m_CurrentDirectory,
      relativePath
    });
    this.RefreshStampButtons();
  }

  private void UpDirectory()
  {
    int length = this.m_CurrentDirectory.LastIndexOf("/");
    if (length > 0)
    {
      this.m_CurrentDirectory = this.m_CurrentDirectory.Substring(0, length);
    }
    else
    {
      string str1;
      string str2;
      SettingsCache.GetDlcIdAndPath(this.m_CurrentDirectory, ref str1, ref str2);
      this.m_CurrentDirectory = !Util.IsNullOrWhiteSpace(str2) ? SettingsCache.GetScope(str1) : PasteBaseTemplateScreen.NO_DIRECTORY;
    }
    this.RefreshStampButtons();
  }

  private void OnClickPasteButton(string template_name)
  {
    if (template_name == null)
      return;
    string templatePath = FileSystem.CombineAndNormalize(new string[2]
    {
      this.m_CurrentDirectory,
      template_name
    });
    DebugTool.Instance.DeactivateTool();
    DebugBaseTemplateButton.Instance.ClearSelection();
    ((TMP_InputField) DebugBaseTemplateButton.Instance.nameField).text = templatePath;
    TemplateContainer template = TemplateCache.GetTemplate(templatePath);
    StampTool.Instance.Activate(template, true);
  }
}
