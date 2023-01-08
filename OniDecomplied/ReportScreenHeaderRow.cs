// Decompiled with JetBrains decompiler
// Type: ReportScreenHeaderRow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TMPro;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/ReportScreenHeaderRow")]
public class ReportScreenHeaderRow : KMonoBehaviour
{
  [SerializeField]
  public LocText name;
  [SerializeField]
  private LayoutElement spacer;
  [SerializeField]
  private Image bgImage;
  public float groupSpacerWidth;
  private float nameWidth = 164f;
  [SerializeField]
  private Color oddRowColor;

  public void SetLine(ReportManager.ReportGroup reportGroup)
  {
    LayoutElement component = ((Component) this.name).GetComponent<LayoutElement>();
    double nameWidth;
    float num = (float) (nameWidth = (double) this.nameWidth);
    component.preferredWidth = (float) nameWidth;
    component.minWidth = num;
    this.spacer.minWidth = this.groupSpacerWidth;
    ((TMP_Text) this.name).text = reportGroup.stringKey;
  }
}
