// Decompiled with JetBrains decompiler
// Type: RobotExhaustPipe
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/RobotExhaustPipe")]
public class RobotExhaustPipe : KMonoBehaviour, ISim4000ms
{
  private float CO2_RATE = 1f / 1000f;

  public void Sim4000ms(float dt)
  {
    Facing component = ((Component) this).GetComponent<Facing>();
    bool flip = false;
    if (Object.op_Implicit((Object) component))
      flip = component.GetFacing();
    CO2Manager.instance.SpawnBreath(Grid.CellToPos(Grid.PosToCell(((Component) this).gameObject)), dt * this.CO2_RATE, 303.15f, flip);
  }
}
