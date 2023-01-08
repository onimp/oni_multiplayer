// Decompiled with JetBrains decompiler
// Type: SubstanceChunk
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/SubstanceChunk")]
public class SubstanceChunk : KMonoBehaviour, ISaveLoadable
{
  private static readonly KAnimHashedString symbolToTint = new KAnimHashedString("substance_tinter");

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    Color color = Color32.op_Implicit(((Component) this).GetComponent<PrimaryElement>().Element.substance.colour);
    color.a = 1f;
    ((Component) this).GetComponent<KBatchedAnimController>().SetSymbolTint(SubstanceChunk.symbolToTint, color);
  }

  private void OnRefreshUserMenu(object data) => Game.Instance.userMenu.AddButton(((Component) this).gameObject, new KIconButtonMenu.ButtonInfo("action_deconstruct", (string) UI.USERMENUACTIONS.RELEASEELEMENT.NAME, new System.Action(this.OnRelease), tooltipText: ((string) UI.USERMENUACTIONS.RELEASEELEMENT.TOOLTIP)));

  private void OnRelease()
  {
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
    PrimaryElement component = ((Component) this).GetComponent<PrimaryElement>();
    if ((double) component.Mass > 0.0)
      SimMessages.AddRemoveSubstance(cell, component.ElementID, CellEventLogger.Instance.ExhaustSimUpdate, component.Mass, component.Temperature, component.DiseaseIdx, component.DiseaseCount);
    TracesExtesions.DeleteObject(((Component) this).gameObject);
  }
}
