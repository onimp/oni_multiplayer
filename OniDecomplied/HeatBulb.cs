// Decompiled with JetBrains decompiler
// Type: HeatBulb
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/HeatBulb")]
public class HeatBulb : KMonoBehaviour, ISim200ms
{
  [SerializeField]
  private float minTemperature;
  [SerializeField]
  private float kjConsumptionRate;
  [SerializeField]
  private float lightKJConsumptionRate;
  [SerializeField]
  private Vector2I minCheckOffset;
  [SerializeField]
  private Vector2I maxCheckOffset;
  [MyCmpGet]
  private Light2D lightSource;
  [MyCmpGet]
  private KBatchedAnimController kanim;
  [Serialize]
  private float kjConsumed;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.kanim.Play(HashedString.op_Implicit("off"));
  }

  public void Sim200ms(float dt)
  {
    double num1 = (double) this.kjConsumptionRate * (double) dt;
    Vector2I vector2I = Vector2I.op_Addition(Vector2I.op_Subtraction(this.maxCheckOffset, this.minCheckOffset), 1);
    double num2 = (double) (vector2I.x * vector2I.y);
    float num3 = (float) (num1 / num2);
    int x1;
    int y1;
    Grid.PosToXY(TransformExtensions.GetPosition(this.transform), out x1, out y1);
    for (int y2 = this.minCheckOffset.y; y2 <= this.maxCheckOffset.y; ++y2)
    {
      for (int x2 = this.minCheckOffset.x; x2 <= this.maxCheckOffset.x; ++x2)
      {
        int cell = Grid.XYToCell(x1 + x2, y1 + y2);
        if (Grid.IsValidCell(cell) && (double) Grid.Temperature[cell] > (double) this.minTemperature)
        {
          this.kjConsumed += num3;
          SimMessages.ModifyEnergy(cell, -num3, 5000f, SimMessages.EnergySourceID.HeatBulb);
        }
      }
    }
    float num4 = this.lightKJConsumptionRate * dt;
    if ((double) this.kjConsumed > (double) num4)
    {
      if (!((Behaviour) this.lightSource).enabled)
      {
        this.kanim.Play(HashedString.op_Implicit("open"));
        this.kanim.Queue(HashedString.op_Implicit("on"));
        ((Behaviour) this.lightSource).enabled = true;
      }
      this.kjConsumed -= num4;
    }
    else
    {
      if (((Behaviour) this.lightSource).enabled)
      {
        this.kanim.Play(HashedString.op_Implicit("close"));
        this.kanim.Queue(HashedString.op_Implicit("off"));
      }
      ((Behaviour) this.lightSource).enabled = false;
    }
  }
}
