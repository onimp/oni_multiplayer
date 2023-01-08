// Decompiled with JetBrains decompiler
// Type: Bee
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class Bee : KMonoBehaviour
{
  public float radiationOutputAmount;
  private Dictionary<HashedString, float> radiationModifiers = new Dictionary<HashedString, float>();
  private float unhappyRadiationMod = 0.1f;
  private float awakeRadiationMod;
  private HashedString unhappyRadiationModKey = HashedString.op_Implicit("UNHAPPY");
  private HashedString awakeRadiationModKey = HashedString.op_Implicit("AWAKE");
  private static readonly EventSystem.IntraObjectHandler<Bee> OnAttackDelegate = new EventSystem.IntraObjectHandler<Bee>((Action<Bee, object>) ((component, data) => component.OnAttack(data)));
  private static readonly EventSystem.IntraObjectHandler<Bee> OnSleepDelegate = new EventSystem.IntraObjectHandler<Bee>((Action<Bee, object>) ((component, data) => component.StartSleep()));
  private static readonly EventSystem.IntraObjectHandler<Bee> OnWakeUpDelegate = new EventSystem.IntraObjectHandler<Bee>((Action<Bee, object>) ((component, data) => component.StopSleep()));
  private static readonly EventSystem.IntraObjectHandler<Bee> OnDeathDelegate = new EventSystem.IntraObjectHandler<Bee>((Action<Bee, object>) ((component, data) => component.OnDeath(data)));
  private static readonly EventSystem.IntraObjectHandler<Bee> OnHappyDelegate = new EventSystem.IntraObjectHandler<Bee>((Action<Bee, object>) ((component, data) => component.RemoveRadiationMod(component.unhappyRadiationModKey)));
  private static readonly EventSystem.IntraObjectHandler<Bee> OnUnhappyDelegate = new EventSystem.IntraObjectHandler<Bee>((Action<Bee, object>) ((component, data) => component.AddRadiationModifier(component.unhappyRadiationModKey, component.unhappyRadiationMod)));

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<Bee>(-739654666, Bee.OnAttackDelegate);
    this.Subscribe<Bee>(-1283701846, Bee.OnSleepDelegate);
    this.Subscribe<Bee>(-2090444759, Bee.OnWakeUpDelegate);
    this.Subscribe<Bee>(1623392196, Bee.OnDeathDelegate);
    this.Subscribe<Bee>(1890751808, Bee.OnHappyDelegate);
    this.Subscribe<Bee>(-647798969, Bee.OnUnhappyDelegate);
    ((Component) this).GetComponent<KBatchedAnimController>().SetSymbolVisiblity(KAnimHashedString.op_Implicit("tag"), false);
    ((Component) this).GetComponent<KBatchedAnimController>().SetSymbolVisiblity(KAnimHashedString.op_Implicit("snapto_tag"), false);
    this.StopSleep();
  }

  private void OnDeath(object data)
  {
    PrimaryElement component1 = ((Component) this).GetComponent<PrimaryElement>();
    Storage component2 = ((Component) this).GetComponent<Storage>();
    byte index = Db.Get().Diseases.GetIndex(Db.Get().Diseases.RadiationPoisoning.id);
    component2.AddOre(SimHashes.NuclearWaste, BeeTuning.WASTE_DROPPED_ON_DEATH, component1.Temperature, index, BeeTuning.GERMS_DROPPED_ON_DEATH);
    component2.DropAll(this.transform.position, true, true, new Vector3());
  }

  private void StartSleep()
  {
    this.RemoveRadiationMod(this.awakeRadiationModKey);
    ((Component) this).GetComponent<ElementConsumer>().EnableConsumption(true);
  }

  private void StopSleep()
  {
    this.AddRadiationModifier(this.awakeRadiationModKey, this.awakeRadiationMod);
    ((Component) this).GetComponent<ElementConsumer>().EnableConsumption(false);
  }

  private void AddRadiationModifier(HashedString name, float mod)
  {
    this.radiationModifiers.Add(name, mod);
    this.RefreshRadiationOutput();
  }

  private void RemoveRadiationMod(HashedString name)
  {
    this.radiationModifiers.Remove(name);
    this.RefreshRadiationOutput();
  }

  public void RefreshRadiationOutput()
  {
    float radiationOutputAmount = this.radiationOutputAmount;
    foreach (KeyValuePair<HashedString, float> radiationModifier in this.radiationModifiers)
      radiationOutputAmount *= radiationModifier.Value;
    RadiationEmitter component = ((Component) this).GetComponent<RadiationEmitter>();
    component.SetEmitting(true);
    component.emitRads = radiationOutputAmount;
    component.Refresh();
  }

  private void OnAttack(object data)
  {
    if (!Tag.op_Equality((Tag) data, GameTags.Creatures.Attack))
      return;
    ((Component) this).GetComponent<Health>().Damage(((Component) this).GetComponent<Health>().hitPoints);
  }

  public KPrefabID FindHiveInRoom()
  {
    List<BeeHive.StatesInstance> statesInstanceList = new List<BeeHive.StatesInstance>();
    Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(((Component) this).gameObject);
    foreach (BeeHive.StatesInstance statesInstance in Components.BeeHives.Items)
    {
      if (Game.Instance.roomProber.GetRoomOfGameObject(statesInstance.gameObject) == roomOfGameObject)
        statesInstanceList.Add(statesInstance);
    }
    int num = int.MaxValue;
    KPrefabID hiveInRoom = (KPrefabID) null;
    foreach (BeeHive.StatesInstance statesInstance in statesInstanceList)
    {
      int navigationCost = ((Component) this).gameObject.GetComponent<Navigator>().GetNavigationCost(Grid.PosToCell(TransformExtensions.GetLocalPosition(statesInstance.transform)));
      if (navigationCost < num)
      {
        num = navigationCost;
        hiveInRoom = statesInstance.GetComponent<KPrefabID>();
      }
    }
    return hiveInRoom;
  }
}
