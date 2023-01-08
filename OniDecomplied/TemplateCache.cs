// Decompiled with JetBrains decompiler
// Type: TemplateCache
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using ProcGen;
using System.Collections.Generic;

public static class TemplateCache
{
  private const string defaultAssetFolder = "bases";
  private static Dictionary<string, TemplateContainer> templates;

  public static bool Initted { get; private set; }

  public static void Init()
  {
    if (TemplateCache.Initted)
      return;
    TemplateCache.templates = new Dictionary<string, TemplateContainer>();
    TemplateCache.Initted = true;
  }

  public static void Clear()
  {
    TemplateCache.templates = (Dictionary<string, TemplateContainer>) null;
    TemplateCache.Initted = false;
  }

  public static string RewriteTemplatePath(string scopePath)
  {
    string str1;
    string str2;
    SettingsCache.GetDlcIdAndPath(scopePath, ref str1, ref str2);
    return SettingsCache.GetAbsoluteContentPath(str1, "templates/" + str2);
  }

  public static string RewriteTemplateYaml(string scopePath) => TemplateCache.RewriteTemplatePath(scopePath) + ".yaml";

  public static TemplateContainer GetTemplate(string templatePath)
  {
    if (!TemplateCache.templates.ContainsKey(templatePath))
      TemplateCache.templates.Add(templatePath, (TemplateContainer) null);
    if (TemplateCache.templates[templatePath] == null)
    {
      string str = TemplateCache.RewriteTemplateYaml(templatePath);
      TemplateContainer templateContainer = YamlIO.LoadFile<TemplateContainer>(str, (YamlIO.ErrorHandler) null, (List<Tuple<string, System.Type>>) null);
      if (templateContainer == null)
        Debug.LogWarning((object) ("Missing template [" + str + "]"));
      templateContainer.name = templatePath;
      TemplateCache.templates[templatePath] = templateContainer;
    }
    return TemplateCache.templates[templatePath];
  }

  public static bool TemplateExists(string templatePath) => FileSystem.FileExists(TemplateCache.RewriteTemplateYaml(templatePath));
}
