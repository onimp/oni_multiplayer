// Decompiled with JetBrains decompiler
// Type: Radiator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Radiator")]
public class Radiator : KMonoBehaviour, IGameObjectEffectDescriptor
{
  public RadiationGridEmitter emitter;
  public int intensity;
  public int projectionCount;
  public int direction;
  public int angle = 360;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.emitter = new RadiationGridEmitter(Grid.PosToCell(((Component) this).gameObject), this.intensity);
    this.emitter.projectionCount = this.projectionCount;
    this.emitter.direction = this.direction;
    this.emitter.angle = this.angle;
    if (Object.op_Equality((Object) ((Component) this).GetComponent<Operational>(), (Object) null))
      this.emitter.enabled = true;
    else
      this.Subscribe(824508782, new Action<object>(this.OnOperationalChanged));
    RadiationGridManager.emitters.Add(this.emitter);
  }

  protected virtual void OnCleanUp()
  {
    RadiationGridManager.emitters.Remove(this.emitter);
    base.OnCleanUp();
  }

  private void OnOperationalChanged(object data) => this.emitter.enabled = ((Component) this).GetComponent<Operational>().IsActive;

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    descriptors.Add(new Descriptor(string.Format((string) UI.GAMEOBJECTEFFECTS.EMITS_LIGHT, (object) this.intensity), (string) UI.GAMEOBJECTEFFECTS.TOOLTIPS.EMITS_LIGHT, (Descriptor.DescriptorType) 1, false));
    return descriptors;
  }

  private void Update() => this.emitter.originCell = Grid.PosToCell(((Component) this).gameObject);
}
