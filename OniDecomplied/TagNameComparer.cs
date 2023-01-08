// Decompiled with JetBrains decompiler
// Type: TagNameComparer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

public class TagNameComparer : IComparer<Tag>
{
  private Tag firstTag;

  public TagNameComparer()
  {
  }

  public TagNameComparer(Tag firstTag) => this.firstTag = firstTag;

  public int Compare(Tag x, Tag y)
  {
    if (Tag.op_Equality(x, y))
      return 0;
    if (((Tag) ref this.firstTag).IsValid)
    {
      if (Tag.op_Equality(x, this.firstTag) && Tag.op_Inequality(y, this.firstTag))
        return 1;
      if (Tag.op_Inequality(x, this.firstTag) && Tag.op_Equality(y, this.firstTag))
        return -1;
    }
    return x.ProperNameStripLink().CompareTo(y.ProperNameStripLink());
  }
}
