// Decompiled with JetBrains decompiler
// Type: Sensor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class Sensor
{
  protected Sensors sensors;

  public string Name { get; private set; }

  public Sensor(Sensors sensors)
  {
    this.sensors = sensors;
    this.Name = this.GetType().Name;
  }

  public ComponentType GetComponent<ComponentType>() => ((Component) this.sensors).GetComponent<ComponentType>();

  public GameObject gameObject => ((Component) this.sensors).gameObject;

  public Transform transform => this.gameObject.transform;

  public void Trigger(int hash, object data = null) => this.sensors.Trigger(hash, data);

  public virtual void Update()
  {
  }

  public virtual void ShowEditor()
  {
  }
}
