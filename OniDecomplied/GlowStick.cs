// Decompiled with JetBrains decompiler
// Type: GlowStick
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using TUNING;
using UnityEngine;

[SkipSaveFileSerialization]
public class GlowStick : StateMachineComponent<GlowStick.StatesInstance>
{
  protected virtual void OnSpawn() => this.smi.StartSM();

  public class StatesInstance : 
    GameStateMachine<GlowStick.States, GlowStick.StatesInstance, GlowStick, object>.GameInstance
  {
    [MyCmpAdd]
    private RadiationEmitter _radiationEmitter;
    [MyCmpAdd]
    private Light2D _light2D;
    public AttributeModifier radiationResistance;

    public StatesInstance(GlowStick master)
      : base(master)
    {
      this._light2D.Color = Color.green;
      this._light2D.Range = 2f;
      this._light2D.Angle = 0.0f;
      this._light2D.Direction = LIGHT2D.DEFAULT_DIRECTION;
      this._light2D.Offset = new Vector2(0.05f, 0.5f);
      this._light2D.shape = LightShape.Circle;
      this._light2D.Lux = 500;
      this._radiationEmitter.emitRads = 100f;
      this._radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Constant;
      this._radiationEmitter.emitRate = 0.5f;
      this._radiationEmitter.emitRadiusX = (short) 3;
      this._radiationEmitter.emitRadiusY = (short) 3;
      this.radiationResistance = new AttributeModifier(Db.Get().Attributes.RadiationResistance.Id, TUNING.TRAITS.GLOWSTICK_RADIATION_RESISTANCE, (string) DUPLICANTS.TRAITS.GLOWSTICK.NAME);
    }
  }

  public class States : GameStateMachine<GlowStick.States, GlowStick.StatesInstance, GlowStick>
  {
    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.root;
      this.root.ToggleComponent<RadiationEmitter>().ToggleComponent<Light2D>().ToggleAttributeModifier("Radiation Resistance", (Func<GlowStick.StatesInstance, AttributeModifier>) (smi => smi.radiationResistance));
    }
  }
}
