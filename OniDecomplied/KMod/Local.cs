// Decompiled with JetBrains decompiler
// Type: KMod.Local
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using STRINGS;
using System.IO;

namespace KMod
{
  public class Local : IDistributionPlatform
  {
    public string folder { get; private set; }

    public Label.DistributionPlatform distribution_platform { get; private set; }

    public string GetDirectory() => FileSystem.Normalize(System.IO.Path.Combine(Manager.GetDirectory(), this.folder));

    private void Subscribe(
      string directoryName,
      long timestamp,
      IFileSource file_source,
      bool isDevMod)
    {
      Label label = new Label()
      {
        id = directoryName,
        distribution_platform = this.distribution_platform,
        version = (long) directoryName.GetHashCode(),
        title = directoryName
      };
      KModHeader header = KModUtil.GetHeader(file_source, label.defaultStaticID, directoryName, directoryName, isDevMod);
      label.title = header.title;
      Mod mod = new Mod(label, header.staticID, header.description, file_source, UI.FRONTEND.MODS.TOOLTIPS.MANAGE_LOCAL_MOD, (System.Action) (() => App.OpenWebURL("file://" + file_source.GetRoot())));
      if (file_source.GetType() == typeof (Directory))
        mod.status = Mod.Status.Installed;
      Global.Instance.modManager.Subscribe(mod, (object) this);
    }

    public Local(string folder, Label.DistributionPlatform distribution_platform, bool isDevFolder)
    {
      this.folder = folder;
      this.distribution_platform = distribution_platform;
      DirectoryInfo directoryInfo = new DirectoryInfo(this.GetDirectory());
      if (!directoryInfo.Exists)
        return;
      foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
        this.Subscribe(directory.Name, directory.LastWriteTime.ToFileTime(), (IFileSource) new Directory(directory.FullName), isDevFolder);
    }
  }
}
