// Decompiled with JetBrains decompiler
// Type: Insulator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/Insulator")]
public class Insulator : KMonoBehaviour
{
  [MyCmpReq]
  private Building building;
  [SerializeField]
  public CellOffset offset = CellOffset.none;

  protected virtual void OnSpawn() => SimMessages.SetInsulation(Grid.OffsetCell(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)), this.offset), this.building.Def.ThermalConductivity);

  protected virtual void OnCleanUp() => SimMessages.SetInsulation(Grid.OffsetCell(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)), this.offset), 1f);
}
