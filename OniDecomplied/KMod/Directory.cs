// Decompiled with JetBrains decompiler
// Type: KMod.Directory
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace KMod
{
  internal struct Directory : IFileSource
  {
    private AliasDirectory file_system;
    private string root;

    public Directory(string root)
    {
      this.root = root;
      this.file_system = new AliasDirectory(root, root, Application.streamingAssetsPath, true);
    }

    public string GetRoot() => this.root;

    public bool Exists() => System.IO.Directory.Exists(this.GetRoot());

    public bool Exists(string relative_path) => this.Exists() && new DirectoryInfo(FileSystem.Normalize(System.IO.Path.Combine(this.root, relative_path))).Exists;

    public void GetTopLevelItems(List<FileSystemItem> file_system_items, string relative_root)
    {
      relative_root = relative_root ?? "";
      string path = FileSystem.Normalize(System.IO.Path.Combine(this.root, relative_root));
      DirectoryInfo directoryInfo = new DirectoryInfo(path);
      if (!directoryInfo.Exists)
      {
        Debug.LogError((object) ("Cannot iterate over $" + path + ", this directory does not exist"));
      }
      else
      {
        foreach (FileSystemInfo fileSystemInfo in directoryInfo.GetFileSystemInfos())
          file_system_items.Add(new FileSystemItem()
          {
            name = fileSystemInfo.Name,
            type = fileSystemInfo is DirectoryInfo ? FileSystemItem.ItemType.Directory : FileSystemItem.ItemType.File
          });
      }
    }

    public IFileDirectory GetFileSystem() => (IFileDirectory) this.file_system;

    public void CopyTo(string path, List<string> extensions = null)
    {
      try
      {
        Directory.CopyDirectory(this.root, path, extensions);
      }
      catch (UnauthorizedAccessException ex)
      {
        FileUtil.ErrorDialog((FileUtil.ErrorType) 1, path, (string) null, (string) null);
      }
      catch (IOException ex)
      {
        FileUtil.ErrorDialog((FileUtil.ErrorType) 2, path, (string) null, (string) null);
      }
      catch
      {
        throw;
      }
    }

    public string Read(string relative_path)
    {
      try
      {
        using (FileStream fileStream = File.OpenRead(System.IO.Path.Combine(this.root, relative_path)))
        {
          byte[] numArray = new byte[fileStream.Length];
          fileStream.Read(numArray, 0, (int) fileStream.Length);
          return Encoding.UTF8.GetString(numArray);
        }
      }
      catch
      {
        return string.Empty;
      }
    }

    private static int CopyDirectory(
      string sourceDirName,
      string destDirName,
      List<string> extensions)
    {
      DirectoryInfo directoryInfo = new DirectoryInfo(sourceDirName);
      if (!directoryInfo.Exists || !FileUtil.CreateDirectory(destDirName, 0))
        return 0;
      FileInfo[] files = directoryInfo.GetFiles();
      int num = 0;
      foreach (FileInfo fileInfo in files)
      {
        bool flag = extensions == null || extensions.Count == 0;
        if (extensions != null)
        {
          foreach (string extension in extensions)
          {
            if (extension == System.IO.Path.GetExtension(fileInfo.Name).ToLower())
            {
              flag = true;
              break;
            }
          }
        }
        if (flag)
        {
          string destFileName = System.IO.Path.Combine(destDirName, fileInfo.Name);
          fileInfo.CopyTo(destFileName, false);
          ++num;
        }
      }
      foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
      {
        string destDirName1 = System.IO.Path.Combine(destDirName, directory.Name);
        num += Directory.CopyDirectory(directory.FullName, destDirName1, extensions);
      }
      if (num == 0)
        FileUtil.DeleteDirectory(destDirName, 0);
      return num;
    }
  }
}
