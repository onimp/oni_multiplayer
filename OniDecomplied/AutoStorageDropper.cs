// Decompiled with JetBrains decompiler
// Type: AutoStorageDropper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class AutoStorageDropper : 
  GameStateMachine<AutoStorageDropper, AutoStorageDropper.Instance, IStateMachineTarget, AutoStorageDropper.Def>
{
  private GameStateMachine<AutoStorageDropper, AutoStorageDropper.Instance, IStateMachineTarget, AutoStorageDropper.Def>.State idle;
  private GameStateMachine<AutoStorageDropper, AutoStorageDropper.Instance, IStateMachineTarget, AutoStorageDropper.Def>.State pre_drop;
  private GameStateMachine<AutoStorageDropper, AutoStorageDropper.Instance, IStateMachineTarget, AutoStorageDropper.Def>.State dropping;
  private GameStateMachine<AutoStorageDropper, AutoStorageDropper.Instance, IStateMachineTarget, AutoStorageDropper.Def>.State blocked;
  private StateMachine<AutoStorageDropper, AutoStorageDropper.Instance, IStateMachineTarget, AutoStorageDropper.Def>.BoolParameter isBlocked;
  public StateMachine<AutoStorageDropper, AutoStorageDropper.Instance, IStateMachineTarget, AutoStorageDropper.Def>.Signal checkCanDrop;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.idle;
    this.root.Update((System.Action<AutoStorageDropper.Instance, float>) ((smi, dt) => smi.UpdateBlockedStatus()), load_balance: true);
    this.idle.EventTransition(GameHashes.OnStorageChange, this.pre_drop).OnSignal(this.checkCanDrop, this.pre_drop, (Func<AutoStorageDropper.Instance, bool>) (smi => !smi.GetComponent<Storage>().IsEmpty())).ParamTransition<bool>((StateMachine<AutoStorageDropper, AutoStorageDropper.Instance, IStateMachineTarget, AutoStorageDropper.Def>.Parameter<bool>) this.isBlocked, this.blocked, GameStateMachine<AutoStorageDropper, AutoStorageDropper.Instance, IStateMachineTarget, AutoStorageDropper.Def>.IsTrue);
    this.pre_drop.ScheduleGoTo((Func<AutoStorageDropper.Instance, float>) (smi => smi.def.delay), (StateMachine.BaseState) this.dropping);
    this.dropping.Enter((StateMachine<AutoStorageDropper, AutoStorageDropper.Instance, IStateMachineTarget, AutoStorageDropper.Def>.State.Callback) (smi => smi.Drop())).GoTo(this.idle);
    this.blocked.ParamTransition<bool>((StateMachine<AutoStorageDropper, AutoStorageDropper.Instance, IStateMachineTarget, AutoStorageDropper.Def>.Parameter<bool>) this.isBlocked, this.pre_drop, GameStateMachine<AutoStorageDropper, AutoStorageDropper.Instance, IStateMachineTarget, AutoStorageDropper.Def>.IsFalse).ToggleStatusItem(Db.Get().BuildingStatusItems.OutputTileBlocked);
  }

  public class DropperFxConfig
  {
    public string animFile;
    public string animName;
    public Grid.SceneLayer layer = Grid.SceneLayer.FXFront;
    public bool useElementTint = true;
    public bool flipX;
    public bool flipY;
  }

  public class Def : StateMachine.BaseDef
  {
    public CellOffset dropOffset;
    public bool asOre;
    public SimHashes[] elementFilter;
    public bool invertElementFilter;
    public bool blockedBySubstantialLiquid;
    public AutoStorageDropper.DropperFxConfig neutralFx;
    public AutoStorageDropper.DropperFxConfig leftFx;
    public AutoStorageDropper.DropperFxConfig rightFx;
    public AutoStorageDropper.DropperFxConfig upFx;
    public AutoStorageDropper.DropperFxConfig downFx;
    public Vector3 fxOffset = Vector3.zero;
    public float cooldown = 2f;
    public float delay;
  }

  public new class Instance : 
    GameStateMachine<AutoStorageDropper, AutoStorageDropper.Instance, IStateMachineTarget, AutoStorageDropper.Def>.GameInstance
  {
    [MyCmpGet]
    private Storage m_storage;
    [MyCmpGet]
    private Rotatable m_rotatable;
    private float m_timeSinceLastDrop;

    public Instance(IStateMachineTarget master, AutoStorageDropper.Def def)
      : base(master, def)
    {
    }

    public void SetInvertElementFilter(bool value)
    {
      this.def.invertElementFilter = value;
      this.smi.sm.checkCanDrop.Trigger(this.smi);
    }

    public void UpdateBlockedStatus()
    {
      int cell = Grid.PosToCell(this.smi.GetDropPosition());
      this.sm.isBlocked.Set(Grid.IsSolidCell(cell) || this.def.blockedBySubstantialLiquid && Grid.IsSubstantialLiquid(cell), this.smi);
    }

    private bool IsFilteredElement(SimHashes element)
    {
      for (int index = 0; index != this.def.elementFilter.Length; ++index)
      {
        if (this.def.elementFilter[index] == element)
          return true;
      }
      return false;
    }

    private bool AllowedToDrop(SimHashes element)
    {
      if (this.def.elementFilter == null || this.def.elementFilter.Length == 0 || !this.def.invertElementFilter && this.IsFilteredElement(element))
        return true;
      return this.def.invertElementFilter && !this.IsFilteredElement(element);
    }

    public void Drop()
    {
      bool flag = false;
      Element element = (Element) null;
      for (int index = this.m_storage.Count - 1; index >= 0; --index)
      {
        GameObject go = this.m_storage.items[index];
        PrimaryElement component1 = go.GetComponent<PrimaryElement>();
        if (this.AllowedToDrop(component1.ElementID))
        {
          if (this.def.asOre)
          {
            this.m_storage.Drop(go, true);
            TransformExtensions.SetPosition(go.transform, this.GetDropPosition());
            element = component1.Element;
            flag = true;
          }
          else
          {
            Dumpable component2 = go.GetComponent<Dumpable>();
            if (!Util.IsNullOrDestroyed((object) component2))
            {
              component2.Dump(this.GetDropPosition());
              element = component1.Element;
              flag = true;
            }
          }
        }
      }
      AutoStorageDropper.DropperFxConfig dropperAnim = this.GetDropperAnim();
      if (!flag || dropperAnim == null || (double) GameClock.Instance.GetTime() <= (double) this.m_timeSinceLastDrop + (double) this.def.cooldown)
        return;
      this.m_timeSinceLastDrop = GameClock.Instance.GetTime();
      Vector3 position = Vector3.op_Addition(Grid.CellToPosCCC(Grid.PosToCell(this.GetDropPosition()), dropperAnim.layer), Object.op_Inequality((Object) this.m_rotatable, (Object) null) ? this.m_rotatable.GetRotatedOffset(this.def.fxOffset) : this.def.fxOffset);
      KBatchedAnimController effect = FXHelpers.CreateEffect(dropperAnim.animFile, position, layer: dropperAnim.layer);
      effect.destroyOnAnimComplete = false;
      effect.FlipX = dropperAnim.flipX;
      effect.FlipY = dropperAnim.flipY;
      if (dropperAnim.useElementTint)
        effect.TintColour = element.substance.colour;
      effect.Play(HashedString.op_Implicit(dropperAnim.animName));
    }

    public AutoStorageDropper.DropperFxConfig GetDropperAnim()
    {
      CellOffset cellOffset = Object.op_Inequality((Object) this.m_rotatable, (Object) null) ? this.m_rotatable.GetRotatedCellOffset(this.def.dropOffset) : this.def.dropOffset;
      if (cellOffset.x < 0)
        return this.def.leftFx;
      if (cellOffset.x > 0)
        return this.def.rightFx;
      if (cellOffset.y < 0)
        return this.def.downFx;
      return cellOffset.y > 0 ? this.def.upFx : this.def.neutralFx;
    }

    public Vector3 GetDropPosition()
    {
      if (!Object.op_Inequality((Object) this.m_rotatable, (Object) null))
        return Vector3.op_Addition(TransformExtensions.GetPosition(this.transform), ((CellOffset) ref this.def.dropOffset).ToVector3());
      Vector3 position = TransformExtensions.GetPosition(this.transform);
      CellOffset rotatedCellOffset = this.m_rotatable.GetRotatedCellOffset(this.def.dropOffset);
      Vector3 vector3 = ((CellOffset) ref rotatedCellOffset).ToVector3();
      return Vector3.op_Addition(position, vector3);
    }
  }
}
