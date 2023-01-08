// Decompiled with JetBrains decompiler
// Type: MinionAssignablesProxy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/MinionAssignablesProxy")]
public class MinionAssignablesProxy : KMonoBehaviour, IAssignableIdentity
{
  public List<Ownables> ownables;
  [Serialize]
  private int target_instance_id = -1;
  private bool slotsConfigured;
  private static readonly EventSystem.IntraObjectHandler<MinionAssignablesProxy> OnAssignablesChangedDelegate = new EventSystem.IntraObjectHandler<MinionAssignablesProxy>((Action<MinionAssignablesProxy, object>) ((component, data) => component.OnAssignablesChanged(data)));
  private static readonly EventSystem.IntraObjectHandler<MinionAssignablesProxy> OnQueueDestroyObjectDelegate = new EventSystem.IntraObjectHandler<MinionAssignablesProxy>((Action<MinionAssignablesProxy, object>) ((component, data) => component.OnQueueDestroyObject(data)));

  public IAssignableIdentity target { get; private set; }

  public bool IsConfigured => this.slotsConfigured;

  public int TargetInstanceID => this.target_instance_id;

  public GameObject GetTargetGameObject()
  {
    if (this.target == null && this.target_instance_id != -1)
      this.RestoreTargetFromInstanceID();
    KMonoBehaviour target = (KMonoBehaviour) this.target;
    return Object.op_Inequality((Object) target, (Object) null) ? ((Component) target).gameObject : (GameObject) null;
  }

  public float GetArrivalTime()
  {
    if (Object.op_Inequality((Object) this.GetTargetGameObject().GetComponent<MinionIdentity>(), (Object) null))
      return this.GetTargetGameObject().GetComponent<MinionIdentity>().arrivalTime;
    if (Object.op_Inequality((Object) this.GetTargetGameObject().GetComponent<StoredMinionIdentity>(), (Object) null))
      return this.GetTargetGameObject().GetComponent<StoredMinionIdentity>().arrivalTime;
    Debug.LogError((object) "Could not get minion arrival time");
    return -1f;
  }

  public int GetTotalSkillpoints()
  {
    if (Object.op_Inequality((Object) this.GetTargetGameObject().GetComponent<MinionIdentity>(), (Object) null))
      return this.GetTargetGameObject().GetComponent<MinionResume>().TotalSkillPointsGained;
    if (Object.op_Inequality((Object) this.GetTargetGameObject().GetComponent<StoredMinionIdentity>(), (Object) null))
      return MinionResume.CalculateTotalSkillPointsGained(this.GetTargetGameObject().GetComponent<StoredMinionIdentity>().TotalExperienceGained);
    Debug.LogError((object) "Could not get minion skill points time");
    return -1;
  }

  public void SetTarget(IAssignableIdentity target, GameObject targetGO)
  {
    Debug.Assert(target != null, (object) "target was null");
    if (Object.op_Equality((Object) targetGO, (Object) null))
    {
      Debug.LogWarningFormat("{0} MinionAssignablesProxy.SetTarget {1}, {2}, {3}. DESTROYING", new object[4]
      {
        (object) ((Object) this).GetInstanceID(),
        (object) this.target_instance_id,
        (object) target,
        (object) targetGO
      });
      Util.KDestroyGameObject(((Component) this).gameObject);
    }
    this.target = target;
    this.target_instance_id = targetGO.GetComponent<KPrefabID>().InstanceID;
    ((Object) ((Component) this).gameObject).name = "Minion Assignables Proxy : " + ((Object) targetGO).name;
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.ownables = new List<Ownables>()
    {
      ((Component) this).gameObject.AddOrGet<Ownables>()
    };
    Components.MinionAssignablesProxy.Add(this);
    this.Subscribe<MinionAssignablesProxy>(1502190696, MinionAssignablesProxy.OnQueueDestroyObjectDelegate);
    this.ConfigureAssignableSlots();
  }

  [System.Runtime.Serialization.OnDeserialized]
  private void OnDeserialized()
  {
  }

  public void ConfigureAssignableSlots()
  {
    if (this.slotsConfigured)
      return;
    Ownables component1 = ((Component) this).GetComponent<Ownables>();
    Equipment component2 = ((Component) this).GetComponent<Equipment>();
    if (Object.op_Inequality((Object) component2, (Object) null))
    {
      foreach (AssignableSlot resource in Db.Get().AssignableSlots.resources)
      {
        if (resource is OwnableSlot)
        {
          OwnableSlotInstance slot_instance = new OwnableSlotInstance((Assignables) component1, (OwnableSlot) resource);
          component1.Add((AssignableSlotInstance) slot_instance);
        }
        else if (resource is EquipmentSlot)
        {
          EquipmentSlotInstance slot_instance = new EquipmentSlotInstance((Assignables) component2, (EquipmentSlot) resource);
          component2.Add((AssignableSlotInstance) slot_instance);
        }
      }
    }
    this.slotsConfigured = true;
  }

  public void RestoreTargetFromInstanceID()
  {
    if (this.target_instance_id == -1 || this.target != null)
      return;
    KPrefabID instance = KPrefabIDTracker.Get().GetInstance(this.target_instance_id);
    if (Object.op_Implicit((Object) instance))
    {
      IAssignableIdentity component = ((Component) instance).GetComponent<IAssignableIdentity>();
      if (component != null)
      {
        this.SetTarget(component, ((Component) instance).gameObject);
      }
      else
      {
        Debug.LogWarningFormat("RestoreTargetFromInstanceID target ID {0} was found but it wasn't an IAssignableIdentity, destroying proxy object.", new object[1]
        {
          (object) this.target_instance_id
        });
        Util.KDestroyGameObject(((Component) this).gameObject);
      }
    }
    else
    {
      Debug.LogWarningFormat("RestoreTargetFromInstanceID target ID {0} was not found, destroying proxy object.", new object[1]
      {
        (object) this.target_instance_id
      });
      Util.KDestroyGameObject(((Component) this).gameObject);
    }
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.RestoreTargetFromInstanceID();
    if (this.target == null)
      return;
    this.Subscribe<MinionAssignablesProxy>(-1585839766, MinionAssignablesProxy.OnAssignablesChangedDelegate);
    Game.Instance.assignmentManager.AddToAssignmentGroup("public", (IAssignableIdentity) this);
  }

  private void OnQueueDestroyObject(object data) => Components.MinionAssignablesProxy.Remove(this);

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    Game.Instance.assignmentManager.RemoveFromAllGroups((IAssignableIdentity) this);
    ((Component) this).GetComponent<Ownables>().UnassignAll();
    ((Component) this).GetComponent<Equipment>().UnequipAll();
  }

  private void OnAssignablesChanged(object data)
  {
    if (this.target.IsNull())
      return;
    ((KMonoBehaviour) this.target).Trigger(-1585839766, data);
  }

  private void CheckTarget()
  {
    if (this.target != null)
      return;
    KPrefabID instance = KPrefabIDTracker.Get().GetInstance(this.target_instance_id);
    if (!Object.op_Inequality((Object) instance, (Object) null))
      return;
    this.target = ((Component) instance).GetComponent<IAssignableIdentity>();
    if (this.target == null)
      return;
    MinionIdentity target1 = this.target as MinionIdentity;
    if (Object.op_Implicit((Object) target1))
    {
      target1.ValidateProxy();
    }
    else
    {
      StoredMinionIdentity target2 = this.target as StoredMinionIdentity;
      if (!Object.op_Implicit((Object) target2))
        return;
      target2.ValidateProxy();
    }
  }

  public List<Ownables> GetOwners()
  {
    this.CheckTarget();
    return this.target.GetOwners();
  }

  public string GetProperName()
  {
    this.CheckTarget();
    return this.target.GetProperName();
  }

  public Ownables GetSoleOwner()
  {
    this.CheckTarget();
    return this.target.GetSoleOwner();
  }

  public bool HasOwner(Assignables owner)
  {
    this.CheckTarget();
    return this.target.HasOwner(owner);
  }

  public int NumOwners()
  {
    this.CheckTarget();
    return this.target.NumOwners();
  }

  public bool IsNull()
  {
    this.CheckTarget();
    return this.target.IsNull();
  }

  public static Ref<MinionAssignablesProxy> InitAssignableProxy(
    Ref<MinionAssignablesProxy> assignableProxyRef,
    IAssignableIdentity source)
  {
    if (assignableProxyRef == null)
      assignableProxyRef = new Ref<MinionAssignablesProxy>();
    GameObject gameObject1 = ((Component) source).gameObject;
    MinionAssignablesProxy assignablesProxy = assignableProxyRef.Get();
    if (Object.op_Equality((Object) assignablesProxy, (Object) null))
    {
      GameObject gameObject2 = GameUtil.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(MinionAssignablesProxyConfig.ID)), Grid.SceneLayer.NoLayer);
      MinionAssignablesProxy component = gameObject2.GetComponent<MinionAssignablesProxy>();
      component.SetTarget(source, gameObject1);
      gameObject2.SetActive(true);
      assignableProxyRef.Set(component);
    }
    else
      assignablesProxy.SetTarget(source, gameObject1);
    return assignableProxyRef;
  }
}
