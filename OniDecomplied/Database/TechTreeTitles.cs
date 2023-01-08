// Decompiled with JetBrains decompiler
// Type: Database.TechTreeTitles
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

namespace Database
{
  public class TechTreeTitles : ResourceSet<TechTreeTitle>
  {
    public TechTreeTitles(ResourceSet parent)
      : base("TreeTitles", parent)
    {
    }

    public void Load(TextAsset tree_file)
    {
      foreach (ResourceTreeNode node in (ResourceLoader<ResourceTreeNode>) new ResourceTreeLoader<ResourceTreeNode>(tree_file))
      {
        if (string.Equals(((Resource) node).Id.Substring(0, 1), "_"))
        {
          TechTreeTitle techTreeTitle = new TechTreeTitle(((Resource) node).Id, (ResourceSet) this, StringEntry.op_Implicit(Strings.Get("STRINGS.RESEARCH.TREES.TITLE" + ((Resource) node).Id.ToUpper())), node);
        }
      }
    }
  }
}
