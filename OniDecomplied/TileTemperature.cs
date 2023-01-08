// Decompiled with JetBrains decompiler
// Type: TileTemperature
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/TileTemperature")]
public class TileTemperature : KMonoBehaviour
{
  [MyCmpReq]
  private PrimaryElement primaryElement;
  [MyCmpReq]
  private KSelectable selectable;

  protected virtual void OnPrefabInit()
  {
    this.primaryElement.getTemperatureCallback = new PrimaryElement.GetTemperatureCallback(TileTemperature.OnGetTemperature);
    this.primaryElement.setTemperatureCallback = new PrimaryElement.SetTemperatureCallback(TileTemperature.OnSetTemperature);
    base.OnPrefabInit();
  }

  protected virtual void OnSpawn() => base.OnSpawn();

  private static float OnGetTemperature(PrimaryElement primary_element)
  {
    SimCellOccupier component = ((Component) primary_element).GetComponent<SimCellOccupier>();
    if (!Object.op_Inequality((Object) component, (Object) null) || !component.IsReady())
      return primary_element.InternalTemperature;
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(primary_element.transform));
    return Grid.Temperature[cell];
  }

  private static void OnSetTemperature(PrimaryElement primary_element, float temperature)
  {
    SimCellOccupier component = ((Component) primary_element).GetComponent<SimCellOccupier>();
    if (Object.op_Inequality((Object) component, (Object) null) && component.IsReady())
      Debug.LogWarning((object) "Only set a tile's temperature during initialization. Otherwise you should be modifying the cell via the sim!");
    else
      primary_element.InternalTemperature = temperature;
  }
}
