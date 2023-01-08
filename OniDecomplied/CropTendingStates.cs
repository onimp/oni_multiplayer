// Decompiled with JetBrains decompiler
// Type: CropTendingStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CropTendingStates : 
  GameStateMachine<CropTendingStates, CropTendingStates.Instance, IStateMachineTarget, CropTendingStates.Def>
{
  private const int MAX_NAVIGATE_DISTANCE = 100;
  private const int MAX_SQR_EUCLIDEAN_DISTANCE = 625;
  private static CropTendingStates.AnimSet defaultAnimSet = new CropTendingStates.AnimSet()
  {
    crop_tending_pre = "crop_tending_pre",
    crop_tending = "crop_tending_loop",
    crop_tending_pst = "crop_tending_pst"
  };
  public StateMachine<CropTendingStates, CropTendingStates.Instance, IStateMachineTarget, CropTendingStates.Def>.TargetParameter targetCrop;
  private GameStateMachine<CropTendingStates, CropTendingStates.Instance, IStateMachineTarget, CropTendingStates.Def>.State findCrop;
  private GameStateMachine<CropTendingStates, CropTendingStates.Instance, IStateMachineTarget, CropTendingStates.Def>.State moveToCrop;
  private CropTendingStates.TendingStates tendCrop;
  private GameStateMachine<CropTendingStates, CropTendingStates.Instance, IStateMachineTarget, CropTendingStates.Def>.State behaviourcomplete;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.findCrop;
    this.root.Exit((StateMachine<CropTendingStates, CropTendingStates.Instance, IStateMachineTarget, CropTendingStates.Def>.State.Callback) (smi =>
    {
      this.UnreserveCrop(smi);
      if (smi.tendedSucceeded)
        return;
      this.RestoreSymbolsVisibility(smi);
    }));
    this.findCrop.Enter((StateMachine<CropTendingStates, CropTendingStates.Instance, IStateMachineTarget, CropTendingStates.Def>.State.Callback) (smi =>
    {
      this.FindCrop(smi);
      if (Object.op_Equality((Object) smi.sm.targetCrop.Get(smi), (Object) null))
      {
        smi.GoTo((StateMachine.BaseState) this.behaviourcomplete);
      }
      else
      {
        this.ReserverCrop(smi);
        smi.GoTo((StateMachine.BaseState) this.moveToCrop);
      }
    }));
    this.moveToCrop.ToggleStatusItem((string) CREATURES.STATUSITEMS.DIVERGENT_WILL_TEND.NAME, (string) CREATURES.STATUSITEMS.DIVERGENT_WILL_TEND.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main).MoveTo((Func<CropTendingStates.Instance, int>) (smi => smi.moveCell), (GameStateMachine<CropTendingStates, CropTendingStates.Instance, IStateMachineTarget, CropTendingStates.Def>.State) this.tendCrop, this.behaviourcomplete).ParamTransition<GameObject>((StateMachine<CropTendingStates, CropTendingStates.Instance, IStateMachineTarget, CropTendingStates.Def>.Parameter<GameObject>) this.targetCrop, this.behaviourcomplete, (StateMachine<CropTendingStates, CropTendingStates.Instance, IStateMachineTarget, CropTendingStates.Def>.Parameter<GameObject>.Callback) ((smi, p) => Object.op_Equality((Object) this.targetCrop.Get(smi), (Object) null)));
    this.tendCrop.DefaultState(this.tendCrop.pre).ToggleStatusItem((string) CREATURES.STATUSITEMS.DIVERGENT_TENDING.NAME, (string) CREATURES.STATUSITEMS.DIVERGENT_TENDING.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main).ParamTransition<GameObject>((StateMachine<CropTendingStates, CropTendingStates.Instance, IStateMachineTarget, CropTendingStates.Def>.Parameter<GameObject>) this.targetCrop, this.behaviourcomplete, (StateMachine<CropTendingStates, CropTendingStates.Instance, IStateMachineTarget, CropTendingStates.Def>.Parameter<GameObject>.Callback) ((smi, p) => Object.op_Equality((Object) this.targetCrop.Get(smi), (Object) null))).Enter((StateMachine<CropTendingStates, CropTendingStates.Instance, IStateMachineTarget, CropTendingStates.Def>.State.Callback) (smi =>
    {
      smi.animSet = this.GetCropTendingAnimSet(smi);
      this.StoreSymbolsVisibility(smi);
    }));
    this.tendCrop.pre.Face(this.targetCrop).PlayAnim((Func<CropTendingStates.Instance, string>) (smi => smi.animSet.crop_tending_pre)).OnAnimQueueComplete(this.tendCrop.tend);
    this.tendCrop.tend.Enter((StateMachine<CropTendingStates, CropTendingStates.Instance, IStateMachineTarget, CropTendingStates.Def>.State.Callback) (smi => this.SetSymbolsVisibility(smi, false))).QueueAnim((Func<CropTendingStates.Instance, string>) (smi => smi.animSet.crop_tending)).OnAnimQueueComplete(this.tendCrop.pst);
    this.tendCrop.pst.QueueAnim((Func<CropTendingStates.Instance, string>) (smi => smi.animSet.crop_tending_pst)).OnAnimQueueComplete(this.behaviourcomplete).Exit((StateMachine<CropTendingStates, CropTendingStates.Instance, IStateMachineTarget, CropTendingStates.Def>.State.Callback) (smi =>
    {
      GameObject gameObject = smi.sm.targetCrop.Get(smi);
      if (!Object.op_Inequality((Object) gameObject, (Object) null))
        return;
      if (smi.effect != null)
        gameObject.GetComponent<Effects>().Add(smi.effect, true);
      smi.tendedSucceeded = true;
      CropTendingStates.CropTendingEventData data = new CropTendingStates.CropTendingEventData()
      {
        source = smi.gameObject,
        cropId = smi.sm.targetCrop.Get(smi).PrefabID()
      };
      EventExtensions.Trigger(smi.sm.targetCrop.Get(smi), 90606262, (object) data);
      smi.Trigger(90606262, (object) data);
    }));
    this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsToTendCrops);
  }

  private CropTendingStates.AnimSet GetCropTendingAnimSet(CropTendingStates.Instance smi)
  {
    CropTendingStates.AnimSet animSet;
    return smi.def.animSetOverrides.TryGetValue(this.targetCrop.Get(smi).PrefabID(), out animSet) ? animSet : CropTendingStates.defaultAnimSet;
  }

  private void FindCrop(CropTendingStates.Instance smi)
  {
    Navigator component1 = smi.GetComponent<Navigator>();
    Crop crop = (Crop) null;
    int num1 = Grid.InvalidCell;
    int num2 = 100;
    int num3 = -1;
    foreach (Crop worldItem in Components.Crops.GetWorldItems(smi.gameObject.GetMyWorldId()))
    {
      if (smi.effect != null)
      {
        Effects component2 = ((Component) worldItem).GetComponent<Effects>();
        if (Object.op_Inequality((Object) component2, (Object) null))
        {
          bool flag = false;
          foreach (string effect_id in smi.def.ignoreEffectGroup)
          {
            if (component2.HasEffect(effect_id))
            {
              flag = true;
              break;
            }
          }
          if (flag)
            continue;
        }
      }
      Growing component3 = ((Component) worldItem).GetComponent<Growing>();
      if ((!Object.op_Inequality((Object) component3, (Object) null) || !component3.IsGrown()) && !((Component) worldItem).HasTag(GameTags.Creatures.ReservedByCreature) && (double) Vector2.SqrMagnitude(Vector2.op_Implicit(Vector3.op_Subtraction(worldItem.transform.position, smi.transform.position))) <= 625.0)
      {
        int num4;
        smi.def.interests.TryGetValue(((Component) worldItem).PrefabID(), out num4);
        if (num4 >= num3)
        {
          bool flag = num4 > num3;
          int cell = Grid.PosToCell((KMonoBehaviour) worldItem);
          int[] numArray = new int[2]
          {
            Grid.CellLeft(cell),
            Grid.CellRight(cell)
          };
          int num5 = 100;
          int invalidCell = Grid.InvalidCell;
          for (int index = 0; index < numArray.Length; ++index)
          {
            if (Grid.IsValidCell(numArray[index]))
            {
              int navigationCost = component1.GetNavigationCost(numArray[index]);
              if (navigationCost != -1 && navigationCost < num5)
              {
                num5 = navigationCost;
                invalidCell = numArray[index];
              }
            }
          }
          if (num5 != -1 && invalidCell != Grid.InvalidCell && (flag || num5 < num2))
          {
            num1 = invalidCell;
            num2 = num5;
            num3 = num4;
            crop = worldItem;
          }
        }
      }
    }
    GameObject gameObject = Object.op_Inequality((Object) crop, (Object) null) ? ((Component) crop).gameObject : (GameObject) null;
    smi.sm.targetCrop.Set(gameObject, smi);
    smi.moveCell = num1;
  }

  private void ReserverCrop(CropTendingStates.Instance smi)
  {
    GameObject go = smi.sm.targetCrop.Get(smi);
    if (!Object.op_Inequality((Object) go, (Object) null))
      return;
    DebugUtil.Assert(!go.HasTag(GameTags.Creatures.ReservedByCreature));
    go.AddTag(GameTags.Creatures.ReservedByCreature);
  }

  private void UnreserveCrop(CropTendingStates.Instance smi)
  {
    GameObject go = smi.sm.targetCrop.Get(smi);
    if (!Object.op_Inequality((Object) go, (Object) null))
      return;
    go.RemoveTag(GameTags.Creatures.ReservedByCreature);
  }

  private void SetSymbolsVisibility(CropTendingStates.Instance smi, bool isVisible)
  {
    if (!Object.op_Inequality((Object) this.targetCrop.Get(smi), (Object) null))
      return;
    string[] hideSymbolsAfterPre = smi.animSet.hide_symbols_after_pre;
    if (hideSymbolsAfterPre == null)
      return;
    KAnimControllerBase component = this.targetCrop.Get(smi).GetComponent<KAnimControllerBase>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    foreach (string str in hideSymbolsAfterPre)
      component.SetSymbolVisiblity(KAnimHashedString.op_Implicit(str), isVisible);
  }

  private void StoreSymbolsVisibility(CropTendingStates.Instance smi)
  {
    if (!Object.op_Inequality((Object) this.targetCrop.Get(smi), (Object) null))
      return;
    string[] hideSymbolsAfterPre = smi.animSet.hide_symbols_after_pre;
    if (hideSymbolsAfterPre == null)
      return;
    KAnimControllerBase component = this.targetCrop.Get(smi).GetComponent<KAnimControllerBase>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    smi.symbolStates = new bool[hideSymbolsAfterPre.Length];
    for (int index = 0; index < hideSymbolsAfterPre.Length; ++index)
      smi.symbolStates[index] = component.GetSymbolVisiblity(KAnimHashedString.op_Implicit(hideSymbolsAfterPre[index]));
  }

  private void RestoreSymbolsVisibility(CropTendingStates.Instance smi)
  {
    if (!Object.op_Inequality((Object) this.targetCrop.Get(smi), (Object) null) || smi.symbolStates == null)
      return;
    string[] hideSymbolsAfterPre = smi.animSet.hide_symbols_after_pre;
    if (hideSymbolsAfterPre == null)
      return;
    KAnimControllerBase component = this.targetCrop.Get(smi).GetComponent<KAnimControllerBase>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    for (int index = 0; index < hideSymbolsAfterPre.Length; ++index)
      component.SetSymbolVisiblity(KAnimHashedString.op_Implicit(hideSymbolsAfterPre[index]), smi.symbolStates[index]);
  }

  public class AnimSet
  {
    public string crop_tending_pre;
    public string crop_tending;
    public string crop_tending_pst;
    public string[] hide_symbols_after_pre;
  }

  public class CropTendingEventData
  {
    public GameObject source;
    public Tag cropId;
  }

  public class Def : StateMachine.BaseDef
  {
    public string effectId;
    public string[] ignoreEffectGroup;
    public Dictionary<Tag, int> interests = new Dictionary<Tag, int>();
    public Dictionary<Tag, CropTendingStates.AnimSet> animSetOverrides = new Dictionary<Tag, CropTendingStates.AnimSet>();
  }

  public new class Instance : 
    GameStateMachine<CropTendingStates, CropTendingStates.Instance, IStateMachineTarget, CropTendingStates.Def>.GameInstance
  {
    public Effect effect;
    public int moveCell;
    public CropTendingStates.AnimSet animSet;
    public bool tendedSucceeded;
    public bool[] symbolStates;

    public Instance(Chore<CropTendingStates.Instance> chore, CropTendingStates.Def def)
      : base((IStateMachineTarget) chore, def)
    {
      chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, (object) GameTags.Creatures.WantsToTendCrops);
      this.effect = Db.Get().effects.TryGet(this.smi.def.effectId);
    }
  }

  public class TendingStates : 
    GameStateMachine<CropTendingStates, CropTendingStates.Instance, IStateMachineTarget, CropTendingStates.Def>.State
  {
    public GameStateMachine<CropTendingStates, CropTendingStates.Instance, IStateMachineTarget, CropTendingStates.Def>.State pre;
    public GameStateMachine<CropTendingStates, CropTendingStates.Instance, IStateMachineTarget, CropTendingStates.Def>.State tend;
    public GameStateMachine<CropTendingStates, CropTendingStates.Instance, IStateMachineTarget, CropTendingStates.Def>.State pst;
  }
}
