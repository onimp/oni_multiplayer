// Decompiled with JetBrains decompiler
// Type: CreditsScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreditsScreen : KModalScreen
{
  public GameObject entryPrefab;
  public GameObject teamHeaderPrefab;
  private Dictionary<string, GameObject> teamContainers = new Dictionary<string, GameObject>();
  public Transform entryContainer;
  public KButton CloseButton;
  public TextAsset[] creditsFiles;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    foreach (TextAsset creditsFile in this.creditsFiles)
      this.AddCredits(creditsFile);
    this.CloseButton.onClick += new System.Action(this.Close);
  }

  public void Close() => this.Deactivate();

  private void AddCredits(TextAsset csv)
  {
    string[,] strArray = CSVReader.SplitCsvGrid(csv.text, ((Object) csv).name);
    List<string> stringList = new List<string>();
    for (int index = 1; index < strArray.GetLength(1); ++index)
    {
      string str = string.Format("{0} {1}", (object) strArray[0, index], (object) strArray[1, index]);
      if (!(str == " "))
        stringList.Add(str);
    }
    Util.Shuffle<string>((IList<string>) stringList);
    string key = strArray[0, 0];
    GameObject gameObject = Util.KInstantiateUI(this.teamHeaderPrefab, ((Component) this.entryContainer).gameObject, true);
    ((TMP_Text) gameObject.GetComponent<LocText>()).text = key;
    this.teamContainers.Add(key, gameObject);
    foreach (string str in stringList)
      ((TMP_Text) Util.KInstantiateUI(this.entryPrefab, this.teamContainers[key], true).GetComponent<LocText>()).text = str;
  }
}
