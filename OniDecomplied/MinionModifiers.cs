// Decompiled with JetBrains decompiler
// Type: MinionModifiers
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using KSerialization;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[SerializationConfig]
public class MinionModifiers : Modifiers, ISaveLoadable
{
  public bool addBaseTraits = true;
  private static readonly EventSystem.IntraObjectHandler<MinionModifiers> OnDeathDelegate = new EventSystem.IntraObjectHandler<MinionModifiers>((Action<MinionModifiers, object>) ((component, data) => component.OnDeath(data)));
  private static readonly EventSystem.IntraObjectHandler<MinionModifiers> OnAttachFollowCamDelegate = new EventSystem.IntraObjectHandler<MinionModifiers>((Action<MinionModifiers, object>) ((component, data) => component.OnAttachFollowCam(data)));
  private static readonly EventSystem.IntraObjectHandler<MinionModifiers> OnDetachFollowCamDelegate = new EventSystem.IntraObjectHandler<MinionModifiers>((Action<MinionModifiers, object>) ((component, data) => component.OnDetachFollowCam(data)));
  private static readonly EventSystem.IntraObjectHandler<MinionModifiers> OnBeginChoreDelegate = new EventSystem.IntraObjectHandler<MinionModifiers>((Action<MinionModifiers, object>) ((component, data) => component.OnBeginChore(data)));

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    if (!this.addBaseTraits)
      return;
    foreach (Klei.AI.Attribute resource in Db.Get().Attributes.resources)
    {
      if (this.attributes.Get(resource) == null)
        this.attributes.Add(resource);
    }
    foreach (Klei.AI.Disease resource in Db.Get().Diseases.resources)
    {
      AmountInstance amountInstance = this.AddAmount(resource.amount);
      this.attributes.Add(resource.cureSpeedBase);
      double num = (double) amountInstance.SetValue(0.0f);
    }
    ChoreConsumer component = ((Component) this).GetComponent<ChoreConsumer>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.AddProvider((ChoreProvider) GlobalChoreProvider.Instance);
    ((Component) this).gameObject.AddComponent<QualityOfLifeNeed>();
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (!Object.op_Inequality((Object) ((Component) this).GetComponent<ChoreConsumer>(), (Object) null))
      return;
    this.Subscribe<MinionModifiers>(1623392196, MinionModifiers.OnDeathDelegate);
    this.Subscribe<MinionModifiers>(-1506069671, MinionModifiers.OnAttachFollowCamDelegate);
    this.Subscribe<MinionModifiers>(-485480405, MinionModifiers.OnDetachFollowCamDelegate);
    this.Subscribe<MinionModifiers>(-1988963660, MinionModifiers.OnBeginChoreDelegate);
    this.GetAmounts().Get("Calories").OnMaxValueReached += new System.Action(this.OnMaxCaloriesReached);
    Vector3 position = TransformExtensions.GetPosition(this.transform);
    position.z = Grid.GetLayerZ(Grid.SceneLayer.Move);
    TransformExtensions.SetPosition(this.transform, position);
    ((Component) this).gameObject.layer = LayerMask.NameToLayer("Default");
    this.SetupDependentAttribute(Db.Get().Attributes.CarryAmount, Db.Get().AttributeConverters.CarryAmountFromStrength);
  }

  private AmountInstance AddAmount(Amount amount) => this.amounts.Add(new AmountInstance(amount, ((Component) this).gameObject));

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

  private void OnDeath(object data)
  {
    Debug.LogFormat("OnDeath {0} -- {1} has died!", new object[2]
    {
      data,
      (object) ((Object) this).name
    });
    foreach (Component component in Components.LiveMinionIdentities.Items)
      component.GetComponent<Effects>().Add("Mourning", true);
  }

  private void OnMaxCaloriesReached() => ((Component) this).GetComponent<Effects>().Add("WellFed", true);

  private void OnBeginChore(object data)
  {
    Storage component = ((Component) this).GetComponent<Storage>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.DropAll(false, false, new Vector3(), true, (List<GameObject>) null);
  }

  public override void OnSerialize(BinaryWriter writer) => base.OnSerialize(writer);

  public override void OnDeserialize(IReader reader) => base.OnDeserialize(reader);

  private void OnAttachFollowCam(object data) => ((Component) this).GetComponent<Effects>().Add("CenterOfAttention", false);

  private void OnDetachFollowCam(object data) => ((Component) this).GetComponent<Effects>().Remove("CenterOfAttention");
}
