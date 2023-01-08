// Decompiled with JetBrains decompiler
// Type: AutoMiner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMODUnity;
using KSerialization;
using System;
using UnityEngine;

[SerializationConfig]
public class AutoMiner : StateMachineComponent<AutoMiner.Instance>, ISim1000ms
{
  private static HashedString HASH_ROTATION = HashedString.op_Implicit("rotation");
  [MyCmpReq]
  private Operational operational;
  [MyCmpGet]
  private KSelectable selectable;
  [MyCmpAdd]
  private Storage storage;
  [MyCmpGet]
  private Rotatable rotatable;
  [MyCmpReq]
  private MiningSounds mining_sounds;
  public int x;
  public int y;
  public int width;
  public int height;
  public CellOffset vision_offset;
  private KBatchedAnimController arm_anim_ctrl;
  private GameObject arm_go;
  private LoopingSounds looping_sounds;
  private string rotateSoundName = "AutoMiner_rotate";
  private EventReference rotateSound;
  private KAnimLink link;
  private float arm_rot = 45f;
  private float turn_rate = 180f;
  private bool rotation_complete;
  private bool rotate_sound_playing;
  private GameObject hitEffectPrefab;
  private GameObject hitEffect;
  private int dig_cell = Grid.InvalidCell;
  private static readonly EventSystem.IntraObjectHandler<AutoMiner> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<AutoMiner>((Action<AutoMiner, object>) ((component, data) => component.OnOperationalChanged(data)));

  private bool HasDigCell => this.dig_cell != Grid.InvalidCell;

  private bool RotationComplete => this.HasDigCell && this.rotation_complete;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.simRenderLoadBalance = true;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.hitEffectPrefab = Assets.GetPrefab(Tag.op_Implicit("fx_dig_splash"));
    KBatchedAnimController component = ((Component) this).GetComponent<KBatchedAnimController>();
    string str = ((Object) component).name + ".gun";
    this.arm_go = new GameObject(str);
    this.arm_go.SetActive(false);
    this.arm_go.transform.parent = ((Component) component).transform;
    this.looping_sounds = this.arm_go.AddComponent<LoopingSounds>();
    this.rotateSound = RuntimeManager.PathToEventReference(GlobalAssets.GetSound(this.rotateSoundName));
    this.arm_go.AddComponent<KPrefabID>().PrefabTag = new Tag(str);
    this.arm_anim_ctrl = this.arm_go.AddComponent<KBatchedAnimController>();
    this.arm_anim_ctrl.AnimFiles = new KAnimFile[1]
    {
      component.AnimFiles[0]
    };
    this.arm_anim_ctrl.initialAnim = "gun";
    this.arm_anim_ctrl.isMovable = true;
    this.arm_anim_ctrl.sceneLayer = Grid.SceneLayer.TransferArm;
    component.SetSymbolVisiblity(KAnimHashedString.op_Implicit("gun_target"), false);
    Matrix4x4 symbolTransform = component.GetSymbolTransform(new HashedString("gun_target"), out bool _);
    Vector3 vector3 = Vector4.op_Implicit(((Matrix4x4) ref symbolTransform).GetColumn(3));
    vector3.z = Grid.GetLayerZ(Grid.SceneLayer.TransferArm);
    TransformExtensions.SetPosition(this.arm_go.transform, vector3);
    this.arm_go.SetActive(true);
    this.link = new KAnimLink((KAnimControllerBase) component, (KAnimControllerBase) this.arm_anim_ctrl);
    this.Subscribe<AutoMiner>(-592767678, AutoMiner.OnOperationalChangedDelegate);
    this.RotateArm(this.rotatable.GetRotatedOffset(Quaternion.op_Multiply(Quaternion.Euler(0.0f, 0.0f, -45f), Vector3.up)), true, 0.0f);
    this.StopDig();
    this.smi.StartSM();
  }

  protected override void OnCleanUp() => base.OnCleanUp();

  public void Sim1000ms(float dt)
  {
    if (!this.operational.IsOperational)
      return;
    this.RefreshDiggableCell();
    this.operational.SetActive(this.HasDigCell);
  }

  private void OnOperationalChanged(object data)
  {
    if ((bool) data)
      return;
    this.dig_cell = Grid.InvalidCell;
    this.rotation_complete = false;
  }

  public void UpdateRotation(float dt)
  {
    if (!this.HasDigCell)
      return;
    Vector3 posCcc = Grid.CellToPosCCC(this.dig_cell, Grid.SceneLayer.TileMain);
    posCcc.z = 0.0f;
    Vector3 position = TransformExtensions.GetPosition(this.arm_go.transform);
    position.z = 0.0f;
    this.RotateArm(Vector3.Normalize(Vector3.op_Subtraction(posCcc, position)), false, dt);
  }

  private Element GetTargetElement() => this.HasDigCell ? Grid.Element[this.dig_cell] : (Element) null;

  public void StartDig()
  {
    this.Trigger(-1762453998, (object) this.GetTargetElement());
    this.CreateHitEffect();
    this.arm_anim_ctrl.Play(HashedString.op_Implicit("gun_digging"), (KAnim.PlayMode) 0);
  }

  public void StopDig()
  {
    this.Trigger(939543986, (object) null);
    this.DestroyHitEffect();
    this.arm_anim_ctrl.Play(HashedString.op_Implicit("gun"), (KAnim.PlayMode) 0);
  }

  public void UpdateDig(float dt)
  {
    if (!this.HasDigCell || !this.rotation_complete)
      return;
    Diggable.DoDigTick(this.dig_cell, dt);
    this.mining_sounds.SetPercentComplete(Grid.Damage[this.dig_cell]);
    Vector3 posCcc = Grid.CellToPosCCC(this.dig_cell, Grid.SceneLayer.FXFront2);
    posCcc.z = 0.0f;
    Vector3 position = TransformExtensions.GetPosition(this.arm_go.transform);
    position.z = 0.0f;
    Vector3 vector3 = Vector3.op_Subtraction(posCcc, position);
    float sqrMagnitude = ((Vector3) ref vector3).sqrMagnitude;
    this.arm_anim_ctrl.GetBatchInstanceData().SetClipRadius(position.x, position.y, sqrMagnitude, true);
    if (AutoMiner.ValidDigCell(this.dig_cell))
      return;
    this.dig_cell = Grid.InvalidCell;
    this.rotation_complete = false;
  }

  private void CreateHitEffect()
  {
    if (Object.op_Equality((Object) this.hitEffectPrefab, (Object) null))
      return;
    if (Object.op_Inequality((Object) this.hitEffect, (Object) null))
      this.DestroyHitEffect();
    this.hitEffect = GameUtil.KInstantiate(this.hitEffectPrefab, Grid.CellToPosCCC(this.dig_cell, Grid.SceneLayer.FXFront2), Grid.SceneLayer.FXFront2);
    this.hitEffect.SetActive(true);
    KBatchedAnimController component = this.hitEffect.GetComponent<KBatchedAnimController>();
    component.sceneLayer = Grid.SceneLayer.FXFront2;
    component.initialMode = (KAnim.PlayMode) 0;
    component.enabled = false;
    component.enabled = true;
  }

  private void DestroyHitEffect()
  {
    if (Object.op_Equality((Object) this.hitEffectPrefab, (Object) null) || !Object.op_Inequality((Object) this.hitEffect, (Object) null))
      return;
    TracesExtesions.DeleteObject(this.hitEffect);
    this.hitEffect = (GameObject) null;
  }

  private void RefreshDiggableCell()
  {
    CellOffset offset = this.vision_offset;
    if (Object.op_Implicit((Object) this.rotatable))
      offset = this.rotatable.GetRotatedCellOffset(this.vision_offset);
    int cell1 = Grid.PosToCell(((Component) this.transform).gameObject);
    int cell2 = Grid.OffsetCell(cell1, offset);
    int x1;
    int y1;
    Grid.CellToXY(cell2, out x1, out y1);
    float num1 = float.MaxValue;
    int num2 = Grid.InvalidCell;
    Vector3 pos1 = Grid.CellToPos(cell2);
    bool flag = false;
    for (int index1 = 0; index1 < this.height; ++index1)
    {
      for (int index2 = 0; index2 < this.width; ++index2)
      {
        CellOffset rotatedCellOffset;
        // ISSUE: explicit constructor call
        ((CellOffset) ref rotatedCellOffset).\u002Ector(this.x + index2, this.y + index1);
        if (Object.op_Implicit((Object) this.rotatable))
          rotatedCellOffset = this.rotatable.GetRotatedCellOffset(rotatedCellOffset);
        int cell3 = Grid.OffsetCell(cell1, rotatedCellOffset);
        if (Grid.IsValidCell(cell3))
        {
          int x2;
          int y2;
          Grid.CellToXY(cell3, out x2, out y2);
          if (Grid.IsValidCell(cell3) && AutoMiner.ValidDigCell(cell3) && Grid.TestLineOfSight(x1, y1, x2, y2, new Func<int, bool>(AutoMiner.DigBlockingCB)))
          {
            if (cell3 == this.dig_cell)
              flag = true;
            Vector3 pos2 = Grid.CellToPos(cell3);
            float num3 = Vector3.Distance(pos1, pos2);
            if ((double) num3 < (double) num1)
            {
              num1 = num3;
              num2 = cell3;
            }
          }
        }
      }
    }
    if (flag || this.dig_cell == num2)
      return;
    this.dig_cell = num2;
    this.rotation_complete = false;
  }

  private static bool ValidDigCell(int cell) => Grid.Solid[cell] && !Grid.Foundation[cell] && Grid.Element[cell].hardness < (byte) 150;

  public static bool DigBlockingCB(int cell) => Grid.Foundation[cell] || Grid.Element[cell].hardness >= (byte) 150;

  private void RotateArm(Vector3 target_dir, bool warp, float dt)
  {
    if (this.rotation_complete)
      return;
    float num1 = MathUtil.Wrap(-180f, 180f, MathUtil.AngleSigned(Vector3.up, target_dir, Vector3.forward) - this.arm_rot);
    this.rotation_complete = Mathf.Approximately(num1, 0.0f);
    float num2 = num1;
    if (warp)
      this.rotation_complete = true;
    else
      num2 = Mathf.Clamp(num2, -this.turn_rate * dt, this.turn_rate * dt);
    this.arm_rot += num2;
    this.arm_rot = MathUtil.Wrap(-180f, 180f, this.arm_rot);
    this.arm_go.transform.rotation = Quaternion.Euler(0.0f, 0.0f, this.arm_rot);
    if (!this.rotation_complete)
    {
      this.StartRotateSound();
      this.looping_sounds.SetParameter(this.rotateSound, AutoMiner.HASH_ROTATION, this.arm_rot);
    }
    else
      this.StopRotateSound();
  }

  private void StartRotateSound()
  {
    if (this.rotate_sound_playing)
      return;
    this.looping_sounds.StartSound(this.rotateSound);
    this.rotate_sound_playing = true;
  }

  private void StopRotateSound()
  {
    if (!this.rotate_sound_playing)
      return;
    this.looping_sounds.StopSound(this.rotateSound);
    this.rotate_sound_playing = false;
  }

  public class Instance : 
    GameStateMachine<AutoMiner.States, AutoMiner.Instance, AutoMiner, object>.GameInstance
  {
    public Instance(AutoMiner master)
      : base(master)
    {
    }
  }

  public class States : GameStateMachine<AutoMiner.States, AutoMiner.Instance, AutoMiner>
  {
    public StateMachine<AutoMiner.States, AutoMiner.Instance, AutoMiner, object>.BoolParameter transferring;
    public GameStateMachine<AutoMiner.States, AutoMiner.Instance, AutoMiner, object>.State off;
    public AutoMiner.States.ReadyStates on;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.off;
      this.root.DoNothing();
      this.off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, (GameStateMachine<AutoMiner.States, AutoMiner.Instance, AutoMiner, object>.State) this.on, (StateMachine<AutoMiner.States, AutoMiner.Instance, AutoMiner, object>.Transition.ConditionCallback) (smi => smi.GetComponent<Operational>().IsOperational));
      this.on.DefaultState(this.on.idle).EventTransition(GameHashes.OperationalChanged, this.off, (StateMachine<AutoMiner.States, AutoMiner.Instance, AutoMiner, object>.Transition.ConditionCallback) (smi => !smi.GetComponent<Operational>().IsOperational));
      this.on.idle.PlayAnim("on").EventTransition(GameHashes.ActiveChanged, this.on.moving, (StateMachine<AutoMiner.States, AutoMiner.Instance, AutoMiner, object>.Transition.ConditionCallback) (smi => smi.GetComponent<Operational>().IsActive));
      this.on.moving.Exit((StateMachine<AutoMiner.States, AutoMiner.Instance, AutoMiner, object>.State.Callback) (smi => smi.master.StopRotateSound())).PlayAnim("working").EventTransition(GameHashes.ActiveChanged, this.on.idle, (StateMachine<AutoMiner.States, AutoMiner.Instance, AutoMiner, object>.Transition.ConditionCallback) (smi => !smi.GetComponent<Operational>().IsActive)).Update((Action<AutoMiner.Instance, float>) ((smi, dt) => smi.master.UpdateRotation(dt)), (UpdateRate) 4).Transition(this.on.digging, new StateMachine<AutoMiner.States, AutoMiner.Instance, AutoMiner, object>.Transition.ConditionCallback(AutoMiner.States.RotationComplete));
      this.on.digging.Enter((StateMachine<AutoMiner.States, AutoMiner.Instance, AutoMiner, object>.State.Callback) (smi => smi.master.StartDig())).Exit((StateMachine<AutoMiner.States, AutoMiner.Instance, AutoMiner, object>.State.Callback) (smi => smi.master.StopDig())).PlayAnim("working").EventTransition(GameHashes.ActiveChanged, this.on.idle, (StateMachine<AutoMiner.States, AutoMiner.Instance, AutoMiner, object>.Transition.ConditionCallback) (smi => !smi.GetComponent<Operational>().IsActive)).Update((Action<AutoMiner.Instance, float>) ((smi, dt) => smi.master.UpdateDig(dt))).Transition(this.on.moving, GameStateMachine<AutoMiner.States, AutoMiner.Instance, AutoMiner, object>.Not(new StateMachine<AutoMiner.States, AutoMiner.Instance, AutoMiner, object>.Transition.ConditionCallback(AutoMiner.States.RotationComplete)));
    }

    public static bool RotationComplete(AutoMiner.Instance smi) => smi.master.RotationComplete;

    public class ReadyStates : 
      GameStateMachine<AutoMiner.States, AutoMiner.Instance, AutoMiner, object>.State
    {
      public GameStateMachine<AutoMiner.States, AutoMiner.Instance, AutoMiner, object>.State idle;
      public GameStateMachine<AutoMiner.States, AutoMiner.Instance, AutoMiner, object>.State moving;
      public GameStateMachine<AutoMiner.States, AutoMiner.Instance, AutoMiner, object>.State digging;
    }
  }
}
