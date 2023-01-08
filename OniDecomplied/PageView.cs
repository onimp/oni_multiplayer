// Decompiled with JetBrains decompiler
// Type: PageView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using TMPro;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/PageView")]
public class PageView : KMonoBehaviour
{
  [SerializeField]
  private MultiToggle nextButton;
  [SerializeField]
  private MultiToggle prevButton;
  [SerializeField]
  private LocText pageLabel;
  [SerializeField]
  private int childrenPerPage = 8;
  private int currentPage;
  private int oldChildCount;
  public Action<int> OnChangePage;

  public int ChildrenPerPage => this.childrenPerPage;

  private void Update()
  {
    if (this.oldChildCount == this.transform.childCount)
      return;
    this.oldChildCount = this.transform.childCount;
    this.RefreshPage();
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.nextButton.onClick += (System.Action) (() =>
    {
      this.currentPage = (this.currentPage + 1) % this.pageCount;
      if (this.OnChangePage != null)
        this.OnChangePage(this.currentPage);
      this.RefreshPage();
    });
    this.prevButton.onClick += (System.Action) (() =>
    {
      --this.currentPage;
      if (this.currentPage < 0)
        this.currentPage += this.pageCount;
      if (this.OnChangePage != null)
        this.OnChangePage(this.currentPage);
      this.RefreshPage();
    });
  }

  private int pageCount
  {
    get
    {
      int pageCount = this.transform.childCount / this.childrenPerPage;
      if (this.transform.childCount % this.childrenPerPage != 0)
        ++pageCount;
      return pageCount;
    }
  }

  private void RefreshPage()
  {
    for (int index = 0; index < this.transform.childCount; ++index)
    {
      if (index < this.currentPage * this.childrenPerPage)
        ((Component) this.transform.GetChild(index)).gameObject.SetActive(false);
      else if (index >= this.currentPage * this.childrenPerPage + this.childrenPerPage)
        ((Component) this.transform.GetChild(index)).gameObject.SetActive(false);
      else
        ((Component) this.transform.GetChild(index)).gameObject.SetActive(true);
    }
    LocText pageLabel = this.pageLabel;
    int num = this.currentPage % this.pageCount + 1;
    string str1 = num.ToString();
    num = this.pageCount;
    string str2 = num.ToString();
    string str3 = str1 + "/" + str2;
    ((TMP_Text) pageLabel).SetText(str3);
  }
}
