// Decompiled with JetBrains decompiler
// Type: ComicViewer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComicViewer : KScreen
{
  public GameObject panelPrefab;
  public GameObject contentContainer;
  public List<GameObject> activePanels = new List<GameObject>();
  public KButton closeButton;
  public System.Action OnStop;

  public void ShowComic(ComicData comic, bool isVictoryComic)
  {
    for (int index = 0; index < Mathf.Max(comic.images.Length, comic.stringKeys.Length); ++index)
    {
      GameObject gameObject = Util.KInstantiateUI(this.panelPrefab, this.contentContainer, true);
      this.activePanels.Add(gameObject);
      gameObject.GetComponentInChildren<Image>().sprite = comic.images[index];
      ((TMP_Text) gameObject.GetComponentInChildren<LocText>()).SetText(comic.stringKeys[index]);
    }
    this.closeButton.ClearOnClick();
    if (isVictoryComic)
      this.closeButton.onClick += (System.Action) (() =>
      {
        this.Stop();
        this.Show(false);
      });
    else
      this.closeButton.onClick += (System.Action) (() => this.Stop());
  }

  public void Stop()
  {
    this.OnStop();
    this.Show(false);
    ((Component) this).gameObject.SetActive(false);
  }
}
