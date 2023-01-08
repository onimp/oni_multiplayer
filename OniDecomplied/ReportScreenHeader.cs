// Decompiled with JetBrains decompiler
// Type: ReportScreenHeader
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ReportScreenHeader")]
public class ReportScreenHeader : KMonoBehaviour
{
  [SerializeField]
  private ReportScreenHeaderRow rowTemplate;
  private ReportScreenHeaderRow mainRow;

  public void SetMainEntry(ReportManager.ReportGroup reportGroup)
  {
    if (Object.op_Equality((Object) this.mainRow, (Object) null))
      this.mainRow = Util.KInstantiateUI(((Component) this.rowTemplate).gameObject, ((Component) this).gameObject, true).GetComponent<ReportScreenHeaderRow>();
    this.mainRow.SetLine(reportGroup);
  }
}
