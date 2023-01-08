// Decompiled with JetBrains decompiler
// Type: DescriptorPanel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TMPro;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/DescriptorPanel")]
public class DescriptorPanel : KMonoBehaviour
{
  [SerializeField]
  private GameObject customLabelPrefab;
  private List<GameObject> labels = new List<GameObject>();

  public bool HasDescriptors() => this.labels.Count > 0;

  public void SetDescriptors(IList<Descriptor> descriptors)
  {
    int index;
    for (index = 0; index < ((ICollection<Descriptor>) descriptors).Count; ++index)
    {
      GameObject gameObject;
      if (index >= this.labels.Count)
      {
        gameObject = Util.KInstantiate(Object.op_Inequality((Object) this.customLabelPrefab, (Object) null) ? this.customLabelPrefab : ScreenPrefabs.Instance.DescriptionLabel, ((Component) this).gameObject, (string) null);
        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        this.labels.Add(gameObject);
      }
      else
        gameObject = this.labels[index];
      LocText component = gameObject.GetComponent<LocText>();
      Descriptor descriptor = descriptors[index];
      string str = ((Descriptor) ref descriptor).IndentedText();
      ((TMP_Text) component).text = str;
      gameObject.GetComponent<ToolTip>().toolTip = descriptors[index].tooltipText;
      gameObject.SetActive(true);
    }
    for (; index < this.labels.Count; ++index)
      this.labels[index].SetActive(false);
  }
}
