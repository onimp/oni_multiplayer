// Decompiled with JetBrains decompiler
// Type: KMod.KModUtil
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using System.Collections.Generic;

namespace KMod
{
  public class KModUtil
  {
    public static KModHeader GetHeader(
      IFileSource file_source,
      string defaultStaticID,
      string defaultTitle,
      string defaultDescription,
      bool devMod)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      KModUtil.\u003C\u003Ec__DisplayClass0_0 cDisplayClass00 = new KModUtil.\u003C\u003Ec__DisplayClass0_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass00.devMod = devMod;
      string str1 = "mod.yaml";
      string str2 = file_source.Read(str1);
      // ISSUE: method pointer
      YamlIO.ErrorHandler errorHandler = new YamlIO.ErrorHandler((object) cDisplayClass00, __methodptr(\u003CGetHeader\u003Eb__0));
      KModHeader kmodHeader;
      if (string.IsNullOrEmpty(str2))
        kmodHeader = (KModHeader) null;
      else
        kmodHeader = YamlIO.Parse<KModHeader>(str2, new FileHandle()
        {
          full_path = System.IO.Path.Combine(file_source.GetRoot(), str1)
        }, errorHandler, (List<Tuple<string, System.Type>>) null);
      KModHeader header = kmodHeader;
      if (header == null)
        header = new KModHeader()
        {
          title = defaultTitle,
          description = defaultDescription,
          staticID = defaultStaticID
        };
      if (string.IsNullOrEmpty(header.staticID))
        header.staticID = defaultStaticID;
      if (header.title == null)
        header.title = defaultTitle;
      if (header.description == null)
        header.description = defaultDescription;
      return header;
    }
  }
}
