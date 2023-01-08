// Decompiled with JetBrains decompiler
// Type: Diggable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/Workable/Diggable")]
public class Diggable : Workable
{
  private HandleVector<int>.Handle partitionerEntry;
  private HandleVector<int>.Handle unstableEntry;
  private MeshRenderer childRenderer;
  private bool isReachable;
  private Element originalDigElement;
  [MyCmpAdd]
  private Prioritizable prioritizable;
  [SerializeField]
  public HashedString choreTypeIdHash;
  [SerializeField]
  public Material[] materials;
  [SerializeField]
  public MeshRenderer materialDisplay;
  private bool isDigComplete;
  private static List<Tuple<string, Tag>> lasersForHardness;
  private int handle;
  private static readonly EventSystem.IntraObjectHandler<Diggable> OnReachableChangedDelegate;
  private static readonly EventSystem.IntraObjectHandler<Diggable> OnRefreshUserMenuDelegate;
  public Chore chore;

  public bool Reachable => this.isReachable;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.workerStatusItem = Db.Get().DuplicantStatusItems.Digging;
    this.readyForSkillWorkStatusItem = Db.Get().BuildingStatusItems.DigRequiresSkillPerk;
    this.faceTargetWhenWorking = true;
    this.Subscribe<Diggable>(-1432940121, Diggable.OnReachableChangedDelegate);
    this.attributeConverter = Db.Get().AttributeConverters.DiggingSpeed;
    this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
    this.skillExperienceSkillGroup = Db.Get().SkillGroups.Mining.Id;
    this.skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
    this.multitoolContext = HashedString.op_Implicit("dig");
    this.multitoolHitEffectTag = Tag.op_Implicit("fx_dig_splash");
    this.workingPstComplete = (HashedString[]) null;
    this.workingPstFailed = (HashedString[]) null;
    Prioritizable.AddRef(((Component) this).gameObject);
  }

  private Diggable() => this.SetOffsetTable(OffsetGroups.InvertedStandardTableWithCorners);

  protected override void OnSpawn()
  {
    base.OnSpawn();
    int cell = Grid.PosToCell((KMonoBehaviour) this);
    this.originalDigElement = Grid.Element[cell];
    if (this.originalDigElement.hardness == byte.MaxValue)
      this.OnCancel();
    ((Component) this).GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().MiscStatusItems.WaitingForDig);
    this.UpdateColor(this.isReachable);
    Grid.Objects[cell, 7] = ((Component) this).gameObject;
    ChoreType chore_type = Db.Get().ChoreTypes.Dig;
    if (((HashedString) ref this.choreTypeIdHash).IsValid)
      chore_type = Db.Get().ChoreTypes.GetByHash(this.choreTypeIdHash);
    this.chore = (Chore) new WorkChore<Diggable>(chore_type, (IStateMachineTarget) this, is_preemptable: true);
    this.SetWorkTime(float.PositiveInfinity);
    this.partitionerEntry = GameScenePartitioner.Instance.Add("Diggable.OnSpawn", (object) ((Component) this).gameObject, Grid.PosToCell((KMonoBehaviour) this), GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnSolidChanged));
    this.OnSolidChanged((object) null);
    new ReachabilityMonitor.Instance((Workable) this).StartSM();
    this.Subscribe<Diggable>(493375141, Diggable.OnRefreshUserMenuDelegate);
    this.handle = Game.Instance.Subscribe(-1523247426, new Action<object>(((Workable) this).UpdateStatusItem));
    Components.Diggables.Add(this);
  }

  public override Workable.AnimInfo GetAnim(Worker worker)
  {
    Workable.AnimInfo anim = new Workable.AnimInfo();
    if (this.overrideAnims != null && this.overrideAnims.Length != 0)
      anim.overrideAnims = this.overrideAnims;
    if (((HashedString) ref this.multitoolContext).IsValid && ((Tag) ref this.multitoolHitEffectTag).IsValid)
      anim.smi = (StateMachine.Instance) new MultitoolController.Instance((Workable) this, worker, this.multitoolContext, Assets.GetPrefab(this.multitoolHitEffectTag));
    return anim;
  }

  private static bool IsCellBuildable(int cell)
  {
    bool flag = false;
    GameObject gameObject = Grid.Objects[cell, 1];
    if (Object.op_Inequality((Object) gameObject, (Object) null) && Object.op_Inequality((Object) gameObject.GetComponent<Constructable>(), (Object) null))
      flag = true;
    return flag;
  }

  private IEnumerator PeriodicUnstableFallingRecheck()
  {
    yield return (object) SequenceUtil.WaitForSeconds(2f);
    this.OnSolidChanged((object) null);
  }

  private void OnSolidChanged(object data)
  {
    if (Object.op_Equality((Object) this, (Object) null) || Object.op_Equality((Object) ((Component) this).gameObject, (Object) null))
      return;
    GameScenePartitioner.Instance.Free(ref this.unstableEntry);
    int cell = Grid.PosToCell((KMonoBehaviour) this);
    int num = -1;
    this.UpdateColor(this.isReachable);
    if (Grid.Element[cell].hardness == byte.MaxValue)
    {
      this.UpdateColor(false);
      this.requiredSkillPerk = (string) null;
      this.chore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, (object) Db.Get().SkillPerks.CanDigUnobtanium);
    }
    else if (Grid.Element[cell].hardness >= (byte) 251)
    {
      bool flag = false;
      foreach (Chore.PreconditionInstance precondition in this.chore.GetPreconditions())
      {
        if (precondition.id == ChorePreconditions.instance.HasSkillPerk.id)
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        this.chore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, (object) Db.Get().SkillPerks.CanDigRadioactiveMaterials);
      this.requiredSkillPerk = Db.Get().SkillPerks.CanDigRadioactiveMaterials.Id;
      ((Renderer) this.materialDisplay).sharedMaterial = this.materials[3];
    }
    else if (Grid.Element[cell].hardness >= (byte) 200)
    {
      bool flag = false;
      foreach (Chore.PreconditionInstance precondition in this.chore.GetPreconditions())
      {
        if (precondition.id == ChorePreconditions.instance.HasSkillPerk.id)
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        this.chore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, (object) Db.Get().SkillPerks.CanDigSuperDuperHard);
      this.requiredSkillPerk = Db.Get().SkillPerks.CanDigSuperDuperHard.Id;
      ((Renderer) this.materialDisplay).sharedMaterial = this.materials[3];
    }
    else if (Grid.Element[cell].hardness >= (byte) 150)
    {
      bool flag = false;
      foreach (Chore.PreconditionInstance precondition in this.chore.GetPreconditions())
      {
        if (precondition.id == ChorePreconditions.instance.HasSkillPerk.id)
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        this.chore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, (object) Db.Get().SkillPerks.CanDigNearlyImpenetrable);
      this.requiredSkillPerk = Db.Get().SkillPerks.CanDigNearlyImpenetrable.Id;
      ((Renderer) this.materialDisplay).sharedMaterial = this.materials[2];
    }
    else if (Grid.Element[cell].hardness >= (byte) 50)
    {
      bool flag = false;
      foreach (Chore.PreconditionInstance precondition in this.chore.GetPreconditions())
      {
        if (precondition.id == ChorePreconditions.instance.HasSkillPerk.id)
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        this.chore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, (object) Db.Get().SkillPerks.CanDigVeryFirm);
      this.requiredSkillPerk = Db.Get().SkillPerks.CanDigVeryFirm.Id;
      ((Renderer) this.materialDisplay).sharedMaterial = this.materials[1];
    }
    else
    {
      this.requiredSkillPerk = (string) null;
      this.chore.GetPreconditions().Remove(this.chore.GetPreconditions().Find((Predicate<Chore.PreconditionInstance>) (o => o.id == ChorePreconditions.instance.HasSkillPerk.id)));
    }
    this.UpdateStatusItem();
    bool flag1 = false;
    if (!Grid.Solid[cell])
    {
      num = Diggable.GetUnstableCellAbove(cell);
      if (num == -1)
        flag1 = true;
      else
        ((MonoBehaviour) this).StartCoroutine("PeriodicUnstableFallingRecheck");
    }
    else if (Grid.Foundation[cell])
      flag1 = true;
    if (flag1)
    {
      this.isDigComplete = true;
      if (this.chore == null || !this.chore.InProgress())
        Util.KDestroyGameObject(((Component) this).gameObject);
      else
        ((Renderer) ((Component) this).GetComponentInChildren<MeshRenderer>()).enabled = false;
    }
    else
    {
      if (num == -1)
        return;
      Extents extents = new Extents();
      Grid.CellToXY(cell, out extents.x, out extents.y);
      extents.width = 1;
      extents.height = (num - cell + Grid.WidthInCells - 1) / Grid.WidthInCells + 1;
      this.unstableEntry = GameScenePartitioner.Instance.Add("Diggable.OnSolidChanged", (object) ((Component) this).gameObject, extents, GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnSolidChanged));
    }
  }

  public Element GetTargetElement()
  {
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
    return Grid.Element[cell];
  }

  public override string GetConversationTopic() => ((Tag) ref this.originalDigElement.tag).Name;

  protected override bool OnWorkTick(Worker worker, float dt)
  {
    Diggable.DoDigTick(Grid.PosToCell((KMonoBehaviour) this), dt);
    return this.isDigComplete;
  }

  protected override void OnStopWork(Worker worker)
  {
    if (!this.isDigComplete)
      return;
    Util.KDestroyGameObject(((Component) this).gameObject);
  }

  public override bool InstantlyFinish(Worker worker)
  {
    int cell = Grid.PosToCell((KMonoBehaviour) this);
    if (Grid.Element[cell].hardness == byte.MaxValue)
      return false;
    float approximateDigTime = Diggable.GetApproximateDigTime(cell);
    int num = (int) worker.Work(approximateDigTime);
    return true;
  }

  public static void DoDigTick(int cell, float dt)
  {
    float approximateDigTime = Diggable.GetApproximateDigTime(cell);
    float amount = dt / approximateDigTime;
    double num = (double) WorldDamage.Instance.ApplyDamage(cell, amount, -1);
  }

  public static float GetApproximateDigTime(int cell)
  {
    float hardness = (float) Grid.Element[cell].hardness;
    if ((double) hardness == (double) byte.MaxValue)
      return float.MaxValue;
    Element elementByHash = ElementLoader.FindElementByHash(SimHashes.Ice);
    float num1 = hardness / (float) elementByHash.hardness;
    float num2 = 4f * (Mathf.Min(Grid.Mass[cell], 400f) / 400f);
    return num2 + num1 * num2;
  }

  public static Diggable GetDiggable(int cell)
  {
    GameObject gameObject = Grid.Objects[cell, 7];
    return Object.op_Inequality((Object) gameObject, (Object) null) ? gameObject.GetComponent<Diggable>() : (Diggable) null;
  }

  public static bool IsDiggable(int cell) => Grid.Solid[cell] ? !Grid.Foundation[cell] : Diggable.GetUnstableCellAbove(cell) != Grid.InvalidCell;

  private static int GetUnstableCellAbove(int cell)
  {
    Vector2I xy = Grid.CellToXY(cell);
    List<int> containingFallingAbove = ((Component) World.Instance).GetComponent<UnstableGroundManager>().GetCellsContainingFallingAbove(xy);
    if (containingFallingAbove.Contains(cell))
      return cell;
    byte num = Grid.WorldIdx[cell];
    for (int unstableCellAbove = Grid.CellAbove(cell); Grid.IsValidCell(unstableCellAbove) && (int) Grid.WorldIdx[unstableCellAbove] == (int) num && !Grid.Foundation[unstableCellAbove]; unstableCellAbove = Grid.CellAbove(unstableCellAbove))
    {
      if (Grid.Solid[unstableCellAbove])
        return Grid.Element[unstableCellAbove].IsUnstable ? unstableCellAbove : Grid.InvalidCell;
      if (containingFallingAbove.Contains(unstableCellAbove))
        return unstableCellAbove;
    }
    return Grid.InvalidCell;
  }

  public static bool RequiresTool(Element e) => false;

  public static bool Undiggable(Element e) => e.id == SimHashes.Unobtanium;

  private void OnReachableChanged(object data)
  {
    if (Object.op_Equality((Object) this.childRenderer, (Object) null))
      this.childRenderer = ((Component) this).GetComponentInChildren<MeshRenderer>();
    Material material = ((Renderer) this.childRenderer).material;
    this.isReachable = (bool) data;
    if (Color.op_Equality(material.color, Game.Instance.uiColours.Dig.invalidLocation))
      return;
    this.UpdateColor(this.isReachable);
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    if (this.isReachable)
    {
      component.RemoveStatusItem(Db.Get().BuildingStatusItems.DigUnreachable);
    }
    else
    {
      component.AddStatusItem(Db.Get().BuildingStatusItems.DigUnreachable, (object) this);
      GameScheduler.Instance.Schedule("Locomotion Tutorial", 2f, (Action<object>) (obj => Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Locomotion)), (object) null, (SchedulerGroup) null);
    }
  }

  private void UpdateColor(bool reachable)
  {
    if (!Object.op_Inequality((Object) this.childRenderer, (Object) null))
      return;
    Material material = ((Renderer) this.childRenderer).material;
    if (Diggable.RequiresTool(Grid.Element[Grid.PosToCell(((Component) this).gameObject)]) || Diggable.Undiggable(Grid.Element[Grid.PosToCell(((Component) this).gameObject)]))
      material.color = Game.Instance.uiColours.Dig.invalidLocation;
    else if (Grid.Element[Grid.PosToCell(((Component) this).gameObject)].hardness >= (byte) 50)
    {
      material.color = !reachable ? Game.Instance.uiColours.Dig.unreachable : Game.Instance.uiColours.Dig.validLocation;
      this.multitoolContext = HashedString.op_Implicit(Diggable.lasersForHardness[1].first);
      this.multitoolHitEffectTag = Diggable.lasersForHardness[1].second;
    }
    else
    {
      material.color = !reachable ? Game.Instance.uiColours.Dig.unreachable : Game.Instance.uiColours.Dig.validLocation;
      this.multitoolContext = HashedString.op_Implicit(Diggable.lasersForHardness[0].first);
      this.multitoolHitEffectTag = Diggable.lasersForHardness[0].second;
    }
  }

  public override float GetPercentComplete() => Grid.Damage[Grid.PosToCell((KMonoBehaviour) this)];

  protected override void OnCleanUp()
  {
    base.OnCleanUp();
    GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
    GameScenePartitioner.Instance.Free(ref this.unstableEntry);
    Game.Instance.Unsubscribe(this.handle);
    GameScenePartitioner.Instance.TriggerEvent(Grid.PosToCell((KMonoBehaviour) this), GameScenePartitioner.Instance.digDestroyedLayer, (object) null);
    Components.Diggables.Remove(this);
  }

  private void OnCancel()
  {
    if (Object.op_Inequality((Object) DetailsScreen.Instance, (Object) null))
      ((KScreen) DetailsScreen.Instance).Show(false);
    EventExtensions.Trigger(((Component) this).gameObject, 2127324410, (object) null);
  }

  private void OnRefreshUserMenu(object data) => Game.Instance.userMenu.AddButton(((Component) this).gameObject, new KIconButtonMenu.ButtonInfo("icon_cancel", (string) UI.USERMENUACTIONS.CANCELDIG.NAME, new System.Action(this.OnCancel), tooltipText: ((string) UI.USERMENUACTIONS.CANCELDIG.TOOLTIP)));

  static Diggable()
  {
    List<Tuple<string, Tag>> tupleList = new List<Tuple<string, Tag>>();
    tupleList.Add(new Tuple<string, Tag>("dig", Tag.op_Implicit("fx_dig_splash")));
    tupleList.Add(new Tuple<string, Tag>("specialistdig", Tag.op_Implicit("fx_dig_splash")));
    Diggable.lasersForHardness = tupleList;
    Diggable.OnReachableChangedDelegate = new EventSystem.IntraObjectHandler<Diggable>((Action<Diggable, object>) ((component, data) => component.OnReachableChanged(data)));
    Diggable.OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Diggable>((Action<Diggable, object>) ((component, data) => component.OnRefreshUserMenu(data)));
  }
}
