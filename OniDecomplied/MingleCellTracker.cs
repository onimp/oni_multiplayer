// Decompiled with JetBrains decompiler
// Type: MingleCellTracker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/MingleCellTracker")]
public class MingleCellTracker : KMonoBehaviour, ISim1000ms
{
  public List<int> mingleCells = new List<int>();

  public void Sim1000ms(float dt)
  {
    this.mingleCells.Clear();
    RoomProber roomProber = Game.Instance.roomProber;
    MinionGroupProber minionGroupProber = MinionGroupProber.Get();
    foreach (Room room in roomProber.rooms)
    {
      if (room.roomType == Db.Get().RoomTypes.RecRoom)
      {
        for (int minY = room.cavity.minY; minY <= room.cavity.maxY; ++minY)
        {
          for (int minX = room.cavity.minX; minX <= room.cavity.maxX; ++minX)
          {
            int cell = Grid.XYToCell(minX, minY);
            if (roomProber.GetCavityForCell(cell) == room.cavity && minionGroupProber.IsReachable(cell) && !Grid.HasLadder[cell] && !Grid.HasTube[cell] && !Grid.IsLiquid(cell) && Grid.Element[cell].id == SimHashes.Oxygen)
              this.mingleCells.Add(cell);
          }
        }
      }
    }
  }
}
