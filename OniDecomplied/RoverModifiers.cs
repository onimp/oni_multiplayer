// Decompiled with JetBrains decompiler
// Type: RoverModifiers
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class RoverModifiers : Modifiers, ISaveLoadable
{
  private static readonly EventSystem.IntraObjectHandler<RoverModifiers> OnBeginChoreDelegate = new EventSystem.IntraObjectHandler<RoverModifiers>((Action<RoverModifiers, object>) ((component, data) => component.OnBeginChore(data)));

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.attributes.Add(Db.Get().Attributes.Construction);
    this.attributes.Add(Db.Get().Attributes.Digging);
    this.attributes.Add(Db.Get().Attributes.Strength);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (!Object.op_Inequality((Object) ((Component) this).GetComponent<ChoreConsumer>(), (Object) null))
      return;
    this.Subscribe<RoverModifiers>(-1988963660, RoverModifiers.OnBeginChoreDelegate);
    Vector3 position = TransformExtensions.GetPosition(this.transform);
    position.z = Grid.GetLayerZ(Grid.SceneLayer.Move);
    TransformExtensions.SetPosition(this.transform, position);
    ((Component) this).gameObject.layer = LayerMask.NameToLayer("Default");
    this.SetupDependentAttribute(Db.Get().Attributes.CarryAmount, Db.Get().AttributeConverters.CarryAmountFromStrength);
  }

  private void SetupDependentAttribute(
    Klei.AI.Attribute targetAttribute,
    AttributeConverter attributeConverter)
  {
    Klei.AI.Attribute attribute = attributeConverter.attribute;
    AttributeInstance attributeInstance = attribute.Lookup((Component) this);
    AttributeModifier target_modifier = new AttributeModifier(targetAttribute.Id, attributeConverter.Lookup((Component) this).Evaluate(), attribute.Name, is_readonly: false);
    this.GetAttributes().Add(target_modifier);
    attributeInstance.OnDirty += (System.Action) (() => target_modifier.SetValue(attributeConverter.Lookup((Component) this).Evaluate()));
  }

  private void OnBeginChore(object data)
  {
    Storage component = ((Component) this).GetComponent<Storage>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.DropAll(false, false, new Vector3(), true, (List<GameObject>) null);
  }
}
