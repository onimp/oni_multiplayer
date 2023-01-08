// Decompiled with JetBrains decompiler
// Type: SingleEntityReceptacle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/SingleEntityReceptacle")]
public class SingleEntityReceptacle : Workable, IRender1000ms
{
  [MyCmpGet]
  protected Operational operational;
  [MyCmpReq]
  protected Storage storage;
  [MyCmpGet]
  public Rotatable rotatable;
  protected FetchChore fetchChore;
  public ChoreType choreType = Db.Get().ChoreTypes.Fetch;
  [Serialize]
  public bool autoReplaceEntity;
  [Serialize]
  public Tag requestedEntityTag;
  [Serialize]
  public Tag requestedEntityAdditionalFilterTag;
  [Serialize]
  protected Ref<KSelectable> occupyObjectRef = new Ref<KSelectable>();
  [SerializeField]
  private List<Tag> possibleDepositTagsList = new List<Tag>();
  [SerializeField]
  private List<Func<GameObject, bool>> additionalCriteria = new List<Func<GameObject, bool>>();
  [SerializeField]
  protected bool destroyEntityOnDeposit;
  [SerializeField]
  protected SingleEntityReceptacle.ReceptacleDirection direction;
  public Vector3 occupyingObjectRelativePosition = new Vector3(0.0f, 1f, 3f);
  protected StatusItem statusItemAwaitingDelivery;
  protected StatusItem statusItemNeed;
  protected StatusItem statusItemNoneAvailable;
  private static readonly EventSystem.IntraObjectHandler<SingleEntityReceptacle> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<SingleEntityReceptacle>((Action<SingleEntityReceptacle, object>) ((component, data) => component.OnOperationalChanged(data)));

  public FetchChore GetActiveRequest => this.fetchChore;

  protected GameObject occupyingObject
  {
    get => Object.op_Inequality((Object) this.occupyObjectRef.Get(), (Object) null) ? ((Component) this.occupyObjectRef.Get()).gameObject : (GameObject) null;
    set
    {
      if (Object.op_Equality((Object) value, (Object) null))
        this.occupyObjectRef.Set((KSelectable) null);
      else
        this.occupyObjectRef.Set(value.GetComponent<KSelectable>());
    }
  }

  public GameObject Occupant => this.occupyingObject;

  public IReadOnlyList<Tag> possibleDepositObjectTags => (IReadOnlyList<Tag>) this.possibleDepositTagsList;

  public bool HasDepositTag(Tag tag) => this.possibleDepositTagsList.Contains(tag);

  public bool IsValidEntity(GameObject candidate)
  {
    IReceptacleDirection component = candidate.GetComponent<IReceptacleDirection>();
    bool flag = Object.op_Inequality((Object) this.rotatable, (Object) null) || component == null || component.Direction == this.Direction;
    for (int index = 0; flag && index < this.additionalCriteria.Count; ++index)
      flag = this.additionalCriteria[index](candidate);
    return flag;
  }

  public SingleEntityReceptacle.ReceptacleDirection Direction => this.direction;

  protected override void OnPrefabInit() => base.OnPrefabInit();

  protected override void OnSpawn()
  {
    base.OnSpawn();
    if (Object.op_Inequality((Object) this.occupyingObject, (Object) null))
    {
      this.PositionOccupyingObject();
      this.SubscribeToOccupant();
    }
    this.UpdateStatusItem();
    if (Object.op_Equality((Object) this.occupyingObject, (Object) null) && !((Tag) ref this.requestedEntityTag).IsValid)
      this.requestedEntityAdditionalFilterTag = Tag.op_Implicit((string) null);
    if (Object.op_Equality((Object) this.occupyingObject, (Object) null) && ((Tag) ref this.requestedEntityTag).IsValid)
      this.CreateOrder(this.requestedEntityTag, this.requestedEntityAdditionalFilterTag);
    this.Subscribe<SingleEntityReceptacle>(-592767678, SingleEntityReceptacle.OnOperationalChangedDelegate);
  }

  public void AddDepositTag(Tag t) => this.possibleDepositTagsList.Add(t);

  public void AddAdditionalCriteria(Func<GameObject, bool> criteria) => this.additionalCriteria.Add(criteria);

  public void SetReceptacleDirection(SingleEntityReceptacle.ReceptacleDirection d) => this.direction = d;

  public virtual void SetPreview(Tag entityTag, bool solid = false)
  {
  }

  public virtual void CreateOrder(Tag entityTag, Tag additionalFilterTag)
  {
    this.requestedEntityTag = entityTag;
    this.requestedEntityAdditionalFilterTag = additionalFilterTag;
    this.CreateFetchChore(this.requestedEntityTag, this.requestedEntityAdditionalFilterTag);
    this.SetPreview(entityTag, true);
    this.UpdateStatusItem();
  }

  public void Render1000ms(float dt) => this.UpdateStatusItem();

  protected void UpdateStatusItem()
  {
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    if (Object.op_Inequality((Object) this.Occupant, (Object) null))
      component.SetStatusItem(Db.Get().StatusItemCategories.EntityReceptacle, (StatusItem) null);
    else if (this.fetchChore != null)
    {
      bool flag = Object.op_Inequality((Object) this.fetchChore.fetcher, (Object) null);
      WorldContainer myWorld = this.GetMyWorld();
      if (!flag && Object.op_Inequality((Object) myWorld, (Object) null))
      {
        foreach (Tag tag in this.fetchChore.tags)
        {
          if ((double) myWorld.worldInventory.GetTotalAmount(tag, true) > 0.0)
          {
            if ((double) myWorld.worldInventory.GetTotalAmount(this.requestedEntityAdditionalFilterTag, true) <= 0.0)
            {
              if (!Tag.op_Equality(this.requestedEntityAdditionalFilterTag, Tag.Invalid))
                break;
            }
            flag = true;
            break;
          }
        }
      }
      if (flag)
        component.SetStatusItem(Db.Get().StatusItemCategories.EntityReceptacle, this.statusItemAwaitingDelivery);
      else
        component.SetStatusItem(Db.Get().StatusItemCategories.EntityReceptacle, this.statusItemNoneAvailable);
    }
    else
      component.SetStatusItem(Db.Get().StatusItemCategories.EntityReceptacle, this.statusItemNeed);
  }

  protected void CreateFetchChore(Tag entityTag, Tag additionalRequiredTag)
  {
    if (this.fetchChore != null || !((Tag) ref entityTag).IsValid || !Tag.op_Inequality(entityTag, GameTags.Empty))
      return;
    ChoreType choreType = this.choreType;
    Storage storage = this.storage;
    HashSet<Tag> tags = new HashSet<Tag>();
    tags.Add(entityTag);
    Tag required_tag = !((Tag) ref additionalRequiredTag).IsValid || !Tag.op_Inequality(additionalRequiredTag, GameTags.Empty) ? Tag.Invalid : additionalRequiredTag;
    Action<Chore> on_complete = new Action<Chore>(this.OnFetchComplete);
    Action<Chore> on_begin = (Action<Chore>) (chore => this.UpdateStatusItem());
    Action<Chore> on_end = (Action<Chore>) (chore => this.UpdateStatusItem());
    this.fetchChore = new FetchChore(choreType, storage, 1f, tags, FetchChore.MatchCriteria.MatchID, required_tag, on_complete: on_complete, on_begin: on_begin, on_end: on_end, operational_requirement: Operational.State.Functional);
    MaterialNeeds.UpdateNeed(this.requestedEntityTag, 1f, ((Component) this).gameObject.GetMyWorldId());
    this.UpdateStatusItem();
  }

  public virtual void OrderRemoveOccupant() => this.ClearOccupant();

  protected virtual void ClearOccupant()
  {
    if (Object.op_Implicit((Object) this.occupyingObject))
    {
      this.UnsubscribeFromOccupant();
      this.storage.DropAll(false, false, new Vector3(), true, (List<GameObject>) null);
    }
    this.occupyingObject = (GameObject) null;
    this.UpdateActive();
    this.UpdateStatusItem();
    this.Trigger(-731304873, (object) this.occupyingObject);
  }

  public void CancelActiveRequest()
  {
    if (this.fetchChore != null)
    {
      MaterialNeeds.UpdateNeed(this.requestedEntityTag, -1f, ((Component) this).gameObject.GetMyWorldId());
      this.fetchChore.Cancel("User canceled");
      this.fetchChore = (FetchChore) null;
    }
    this.requestedEntityTag = Tag.Invalid;
    this.requestedEntityAdditionalFilterTag = Tag.Invalid;
    this.UpdateStatusItem();
    this.SetPreview(Tag.Invalid);
  }

  private void OnOccupantDestroyed(object data)
  {
    this.occupyingObject = (GameObject) null;
    this.ClearOccupant();
    if (!this.autoReplaceEntity || !((Tag) ref this.requestedEntityTag).IsValid || !Tag.op_Inequality(this.requestedEntityTag, GameTags.Empty))
      return;
    this.CreateOrder(this.requestedEntityTag, this.requestedEntityAdditionalFilterTag);
  }

  protected virtual void SubscribeToOccupant()
  {
    if (!Object.op_Inequality((Object) this.occupyingObject, (Object) null))
      return;
    this.Subscribe(this.occupyingObject, 1969584890, new Action<object>(this.OnOccupantDestroyed));
  }

  protected virtual void UnsubscribeFromOccupant()
  {
    if (!Object.op_Inequality((Object) this.occupyingObject, (Object) null))
      return;
    this.Unsubscribe(this.occupyingObject, 1969584890, new Action<object>(this.OnOccupantDestroyed));
  }

  private void OnFetchComplete(Chore chore)
  {
    if (this.fetchChore == null)
      Debug.LogWarningFormat((Object) ((Component) this).gameObject, "{0} OnFetchComplete fetchChore null", new object[1]
      {
        (object) ((Component) this).gameObject
      });
    else if (Object.op_Equality((Object) this.fetchChore.fetchTarget, (Object) null))
      Debug.LogWarningFormat((Object) ((Component) this).gameObject, "{0} OnFetchComplete fetchChore.fetchTarget null", new object[1]
      {
        (object) ((Component) this).gameObject
      });
    else
      this.OnDepositObject(((Component) this.fetchChore.fetchTarget).gameObject);
  }

  public void ForceDeposit(GameObject depositedObject)
  {
    if (Object.op_Inequality((Object) this.occupyingObject, (Object) null))
      this.ClearOccupant();
    this.OnDepositObject(depositedObject);
  }

  private void OnDepositObject(GameObject depositedObject)
  {
    this.SetPreview(Tag.Invalid);
    MaterialNeeds.UpdateNeed(this.requestedEntityTag, -1f, ((Component) this).gameObject.GetMyWorldId());
    KBatchedAnimController component = depositedObject.GetComponent<KBatchedAnimController>();
    if (Object.op_Inequality((Object) component, (Object) null))
      component.GetBatchInstanceData().ClearOverrideTransformMatrix();
    this.occupyingObject = this.SpawnOccupyingObject(depositedObject);
    if (Object.op_Inequality((Object) this.occupyingObject, (Object) null))
    {
      this.ConfigureOccupyingObject(this.occupyingObject);
      this.occupyingObject.SetActive(true);
      this.PositionOccupyingObject();
      this.SubscribeToOccupant();
    }
    else
      Debug.LogWarning((object) (((Object) ((Component) this).gameObject).name + " EntityReceptacle did not spawn occupying entity."));
    if (this.fetchChore != null)
    {
      this.fetchChore.Cancel("receptacle filled");
      this.fetchChore = (FetchChore) null;
    }
    if (!this.autoReplaceEntity)
    {
      this.requestedEntityTag = Tag.Invalid;
      this.requestedEntityAdditionalFilterTag = Tag.Invalid;
    }
    this.UpdateActive();
    this.UpdateStatusItem();
    if (this.destroyEntityOnDeposit)
      Util.KDestroyGameObject(depositedObject);
    this.Trigger(-731304873, (object) this.occupyingObject);
  }

  protected virtual GameObject SpawnOccupyingObject(GameObject depositedEntity) => depositedEntity;

  protected virtual void ConfigureOccupyingObject(GameObject source)
  {
  }

  protected virtual void PositionOccupyingObject()
  {
    if (Object.op_Inequality((Object) this.rotatable, (Object) null))
      TransformExtensions.SetPosition(this.occupyingObject.transform, Vector3.op_Addition(TransformExtensions.GetPosition(((Component) this).gameObject.transform), this.rotatable.GetRotatedOffset(this.occupyingObjectRelativePosition)));
    else
      TransformExtensions.SetPosition(this.occupyingObject.transform, Vector3.op_Addition(TransformExtensions.GetPosition(((Component) this).gameObject.transform), this.occupyingObjectRelativePosition));
    KBatchedAnimController component = this.occupyingObject.GetComponent<KBatchedAnimController>();
    component.enabled = false;
    component.enabled = true;
  }

  private void UpdateActive()
  {
    if (((object) this).Equals((object) null) || Object.op_Equality((Object) this, (Object) null) || ((object) ((Component) this).gameObject).Equals((object) null) || Object.op_Equality((Object) ((Component) this).gameObject, (Object) null) || !Object.op_Inequality((Object) this.operational, (Object) null))
      return;
    this.operational.SetActive(this.operational.IsOperational && Object.op_Inequality((Object) this.occupyingObject, (Object) null));
  }

  protected override void OnCleanUp()
  {
    this.CancelActiveRequest();
    this.UnsubscribeFromOccupant();
    base.OnCleanUp();
  }

  private void OnOperationalChanged(object data)
  {
    this.UpdateActive();
    if (!Object.op_Implicit((Object) this.occupyingObject))
      return;
    EventExtensions.Trigger(this.occupyingObject, this.operational.IsOperational ? 1628751838 : 960378201, (object) null);
  }

  public enum ReceptacleDirection
  {
    Top,
    Side,
    Bottom,
  }
}
