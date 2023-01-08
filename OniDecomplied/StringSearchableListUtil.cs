// Decompiled with JetBrains decompiler
// Type: StringSearchableListUtil
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;

public static class StringSearchableListUtil
{
  public static bool DoAnyTagsMatchFilter(string[] lowercaseTags, in string filter)
  {
    string filter1 = filter.Trim().ToLowerInvariant();
    string[] source = filter1.Split(' ');
    foreach (string lowercaseTag in lowercaseTags)
    {
      string tag = lowercaseTag;
      if (StringSearchableListUtil.DoesTagMatchFilter(tag, in filter1) || ((IEnumerable<string>) source).Select<string, bool>((Func<string, bool>) (f => StringSearchableListUtil.DoesTagMatchFilter(tag, in f))).All<bool>((Func<bool, bool>) (result => result)))
        return true;
    }
    return false;
  }

  public static bool DoesTagMatchFilter(string lowercaseTag, in string filter) => string.IsNullOrWhiteSpace(filter) || lowercaseTag.Contains(filter);

  public static bool ShouldUseFilter(string filter) => !string.IsNullOrWhiteSpace(filter);
}
