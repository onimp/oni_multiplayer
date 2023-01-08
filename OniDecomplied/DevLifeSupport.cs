// Decompiled with JetBrains decompiler
// Type: DevLifeSupport
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class DevLifeSupport : KMonoBehaviour, ISim200ms
{
  [MyCmpReq]
  private ElementConsumer elementConsumer;
  public float targetTemperature = 303.15f;
  public int effectRadius = 7;
  private const float temperatureControlK = 0.2f;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (!Object.op_Inequality((Object) this.elementConsumer, (Object) null))
      return;
    this.elementConsumer.EnableConsumption(true);
  }

  public void Sim200ms(float dt)
  {
    Vector2I vector2I1;
    // ISSUE: explicit constructor call
    ((Vector2I) ref vector2I1).\u002Ector(-this.effectRadius, -this.effectRadius);
    Vector2I vector2I2;
    // ISSUE: explicit constructor call
    ((Vector2I) ref vector2I2).\u002Ector(this.effectRadius, this.effectRadius);
    int x1;
    int y1;
    Grid.PosToXY(TransformExtensions.GetPosition(this.transform), out x1, out y1);
    int cell1 = Grid.XYToCell(x1, y1);
    if (!Grid.IsValidCell(cell1))
      return;
    int world = (int) Grid.WorldIdx[cell1];
    for (int y2 = vector2I1.y; y2 <= vector2I2.y; ++y2)
    {
      for (int x2 = vector2I1.x; x2 <= vector2I2.x; ++x2)
      {
        int cell2 = Grid.XYToCell(x1 + x2, y1 + y2);
        if (Grid.IsValidCellInWorld(cell2, world))
        {
          float num = (this.targetTemperature - Grid.Temperature[cell2]) * Grid.Element[cell2].specificHeatCapacity * Grid.Mass[cell2];
          if (!Mathf.Approximately(0.0f, num))
            SimMessages.ModifyEnergy(cell2, num * 0.2f, 5000f, (double) num > 0.0 ? SimMessages.EnergySourceID.DebugHeat : SimMessages.EnergySourceID.DebugCool);
        }
      }
    }
  }
}
