// Decompiled with JetBrains decompiler
// Type: KMod.IFileSource
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using System.Collections.Generic;

namespace KMod
{
  public interface IFileSource
  {
    string GetRoot();

    bool Exists();

    bool Exists(string relative_path);

    void GetTopLevelItems(List<FileSystemItem> file_system_items, string relative_root = "");

    IFileDirectory GetFileSystem();

    void CopyTo(string path, List<string> extensions = null);

    string Read(string relative_path);
  }
}
