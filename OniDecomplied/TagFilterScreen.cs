// Decompiled with JetBrains decompiler
// Type: TagFilterScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class TagFilterScreen : SideScreenContent
{
  [SerializeField]
  private KTreeControl treeControl;
  private KTreeControl.UserItem rootItem;
  private TagFilterScreen.TagEntry rootTag = TagFilterScreen.defaultRootTag;
  private HashSet<Tag> acceptedTags = new HashSet<Tag>();
  private TreeFilterable targetFilterable;
  public static TagFilterScreen.TagEntry defaultRootTag = new TagFilterScreen.TagEntry()
  {
    name = "All",
    tag = new Tag(),
    children = new TagFilterScreen.TagEntry[0]
  };

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<TreeFilterable>(), (Object) null);

  public override void SetTarget(GameObject target)
  {
    if (Object.op_Equality((Object) target, (Object) null))
    {
      Debug.LogError((object) "The target object provided was null");
    }
    else
    {
      this.targetFilterable = target.GetComponent<TreeFilterable>();
      if (Object.op_Equality((Object) this.targetFilterable, (Object) null))
      {
        Debug.LogError((object) "The target provided does not have a Tree Filterable component");
      }
      else
      {
        if (!this.targetFilterable.showUserMenu)
          return;
        this.Filter(this.targetFilterable.AcceptedTags);
        this.Activate();
      }
    }
  }

  protected virtual void OnActivate()
  {
    this.rootItem = this.BuildDisplay(this.rootTag);
    this.treeControl.SetUserItemRoot(this.rootItem);
    this.treeControl.root.opened = true;
    this.Filter(this.treeControl.root, this.acceptedTags, false);
  }

  public static List<Tag> GetAllTags()
  {
    List<Tag> allTags = new List<Tag>();
    foreach (TagFilterScreen.TagEntry child in TagFilterScreen.defaultRootTag.children)
    {
      if (((Tag) ref child.tag).IsValid)
        allTags.Add(child.tag);
    }
    return allTags;
  }

  private KTreeControl.UserItem BuildDisplay(TagFilterScreen.TagEntry root)
  {
    KTreeControl.UserItem userItem = (KTreeControl.UserItem) null;
    if (root.name != null && root.name != "")
    {
      userItem = new KTreeControl.UserItem()
      {
        text = root.name,
        userData = (object) root.tag
      };
      List<KTreeControl.UserItem> userItemList = new List<KTreeControl.UserItem>();
      if (root.children != null)
      {
        foreach (TagFilterScreen.TagEntry child in root.children)
          userItemList.Add(this.BuildDisplay(child));
      }
      userItem.children = (IList<KTreeControl.UserItem>) userItemList;
    }
    return userItem;
  }

  private static KTreeControl.UserItem CreateTree(
    string tree_name,
    Tag tree_tag,
    IList<Element> items)
  {
    KTreeControl.UserItem tree = new KTreeControl.UserItem()
    {
      text = tree_name,
      userData = (object) tree_tag,
      children = (IList<KTreeControl.UserItem>) new List<KTreeControl.UserItem>()
    };
    foreach (Element element in (IEnumerable<Element>) items)
    {
      KTreeControl.UserItem userItem = new KTreeControl.UserItem()
      {
        text = element.name,
        userData = (object) GameTagExtensions.Create(element.id)
      };
      ((ICollection<KTreeControl.UserItem>) tree.children).Add(userItem);
    }
    return tree;
  }

  public void SetRootTag(TagFilterScreen.TagEntry root_tag) => this.rootTag = root_tag;

  public void Filter(HashSet<Tag> acceptedTags) => this.acceptedTags = acceptedTags;

  private void Filter(KTreeItem root, HashSet<Tag> acceptedTags, bool parentEnabled)
  {
    root.checkboxChecked = parentEnabled || root.userData != null && acceptedTags.Contains((Tag) root.userData);
    foreach (KTreeItem child in (IEnumerable<KTreeItem>) root.children)
      this.Filter(child, acceptedTags, root.checkboxChecked);
    if (root.checkboxChecked || ((ICollection<KTreeItem>) root.children).Count <= 0)
      return;
    bool flag = true;
    foreach (KTreeItem child in (IEnumerable<KTreeItem>) root.children)
    {
      if (!child.checkboxChecked)
      {
        flag = false;
        break;
      }
    }
    root.checkboxChecked = flag;
  }

  public class TagEntry
  {
    public string name;
    public Tag tag;
    public TagFilterScreen.TagEntry[] children;
  }
}
