// Decompiled with JetBrains decompiler
// Type: HighEnergyParticlePort
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class HighEnergyParticlePort : KMonoBehaviour, IGameObjectEffectDescriptor
{
  [MyCmpGet]
  private Building m_building;
  public HighEnergyParticlePort.OnParticleCapture onParticleCapture;
  public HighEnergyParticlePort.OnParticleCaptureAllowed onParticleCaptureAllowed;
  public HighEnergyParticlePort.OnParticleCapture onParticleUncapture;
  public HighEnergyParticle currentParticle;
  public bool requireOperational = true;
  public bool particleInputEnabled;
  public bool particleOutputEnabled;
  public CellOffset particleInputOffset;
  public CellOffset particleOutputOffset;

  public int GetHighEnergyParticleInputPortPosition() => this.m_building.GetHighEnergyParticleInputCell();

  public int GetHighEnergyParticleOutputPortPosition() => this.m_building.GetHighEnergyParticleOutputCell();

  protected virtual void OnPrefabInit() => base.OnPrefabInit();

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    Components.HighEnergyParticlePorts.Add(this);
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    Components.HighEnergyParticlePorts.Remove(this);
  }

  public bool InputActive()
  {
    Operational component = ((Component) this).GetComponent<Operational>();
    if (!this.particleInputEnabled || !Object.op_Inequality((Object) component, (Object) null) || !component.IsFunctional)
      return false;
    return !this.requireOperational || component.IsOperational;
  }

  public bool AllowCapture(HighEnergyParticle particle) => this.onParticleCaptureAllowed == null || this.onParticleCaptureAllowed(particle);

  public void Capture(HighEnergyParticle particle)
  {
    this.currentParticle = particle;
    if (this.onParticleCapture == null)
      return;
    this.onParticleCapture(particle);
  }

  public void Uncapture(HighEnergyParticle particle)
  {
    if (this.onParticleUncapture != null)
      this.onParticleUncapture(particle);
    this.currentParticle = (HighEnergyParticle) null;
  }

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    if (this.particleInputEnabled)
      descriptors.Add(new Descriptor((string) UI.BUILDINGEFFECTS.PARTICLE_PORT_INPUT, (string) UI.BUILDINGEFFECTS.TOOLTIPS.PARTICLE_PORT_INPUT, (Descriptor.DescriptorType) 0, false));
    if (this.particleOutputEnabled)
      descriptors.Add(new Descriptor((string) UI.BUILDINGEFFECTS.PARTICLE_PORT_OUTPUT, (string) UI.BUILDINGEFFECTS.TOOLTIPS.PARTICLE_PORT_OUTPUT, (Descriptor.DescriptorType) 1, false));
    return descriptors;
  }

  public delegate void OnParticleCapture(HighEnergyParticle particle);

  public delegate bool OnParticleCaptureAllowed(HighEnergyParticle particle);
}
