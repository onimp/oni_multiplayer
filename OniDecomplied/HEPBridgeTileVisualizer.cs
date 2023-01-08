// Decompiled with JetBrains decompiler
// Type: HEPBridgeTileVisualizer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class HEPBridgeTileVisualizer : KMonoBehaviour, IHighEnergyParticleDirection
{
  private static readonly EventSystem.IntraObjectHandler<HEPBridgeTileVisualizer> OnRotateDelegate = new EventSystem.IntraObjectHandler<HEPBridgeTileVisualizer>((Action<HEPBridgeTileVisualizer, object>) ((component, data) => component.OnRotate()));

  protected virtual void OnSpawn()
  {
    this.Subscribe<HEPBridgeTileVisualizer>(-1643076535, HEPBridgeTileVisualizer.OnRotateDelegate);
    this.OnRotate();
  }

  public void OnRotate() => Game.Instance.ForceOverlayUpdate(true);

  public EightDirection Direction
  {
    get
    {
      EightDirection direction = EightDirection.Right;
      Rotatable component = ((Component) this).GetComponent<Rotatable>();
      if (Object.op_Inequality((Object) component, (Object) null))
      {
        switch (component.Orientation)
        {
          case Orientation.Neutral:
            direction = EightDirection.Left;
            break;
          case Orientation.R90:
            direction = EightDirection.Up;
            break;
          case Orientation.R180:
            direction = EightDirection.Right;
            break;
          case Orientation.R270:
            direction = EightDirection.Down;
            break;
        }
      }
      return direction;
    }
    set
    {
    }
  }
}
