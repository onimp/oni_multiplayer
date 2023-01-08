// Decompiled with JetBrains decompiler
// Type: StickerBomb
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using KSerialization;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class StickerBomb : StateMachineComponent<StickerBomb.StatesInstance>
{
  [Serialize]
  public string stickerType;
  [Serialize]
  public string stickerName;
  private HandleVector<int>.Handle partitionerEntry;
  private List<int> cellOffsets;

  protected virtual void OnSpawn()
  {
    if (Util.IsNullOrWhiteSpace(this.stickerName))
    {
      Debug.LogError((object) ("Missing sticker db entry for " + this.stickerType));
    }
    else
    {
      DbStickerBomb dbStickerBomb = Db.GetStickerBombs().Get(this.stickerName);
      ((Component) this).GetComponent<KBatchedAnimController>().SwapAnims(new KAnimFile[1]
      {
        dbStickerBomb.animFile
      });
    }
    this.cellOffsets = StickerBomb.BuildCellOffsets(TransformExtensions.GetPosition(this.transform));
    this.smi.destroyTime = GameClock.Instance.GetTime() + TRAITS.JOY_REACTIONS.STICKER_BOMBER.STICKER_DURATION;
    this.smi.StartSM();
    Extents extents = ((Component) this).GetComponent<OccupyArea>().GetExtents();
    this.partitionerEntry = GameScenePartitioner.Instance.Add("StickerBomb.OnSpawn", (object) ((Component) this).gameObject, new Extents(extents.x - 1, extents.y - 1, extents.width + 2, extents.height + 2), GameScenePartitioner.Instance.objectLayers[2], new Action<object>(this.OnFoundationCellChanged));
    base.OnSpawn();
  }

  [System.Runtime.Serialization.OnDeserialized]
  public void OnDeserialized()
  {
    if (!Util.IsNullOrWhiteSpace(this.stickerName) || Util.IsNullOrWhiteSpace(this.stickerType))
      return;
    string[] strArray = this.stickerType.Split('_');
    if (strArray.Length != 2)
      return;
    this.stickerName = strArray[1];
  }

  protected override void OnCleanUp()
  {
    GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
    base.OnCleanUp();
  }

  private void OnFoundationCellChanged(object data)
  {
    if (StickerBomb.CanPlaceSticker(this.cellOffsets))
      return;
    Util.KDestroyGameObject(((Component) this).gameObject);
  }

  public static List<int> BuildCellOffsets(Vector3 position)
  {
    List<int> intList = new List<int>();
    int num = (double) position.x % 1.0 < 0.5 ? 1 : 0;
    bool flag = (double) position.y % 1.0 > 0.5;
    int cell = Grid.PosToCell(position);
    intList.Add(cell);
    if (num != 0)
    {
      intList.Add(Grid.CellLeft(cell));
      if (flag)
      {
        intList.Add(Grid.CellAbove(cell));
        intList.Add(Grid.CellUpLeft(cell));
      }
      else
      {
        intList.Add(Grid.CellBelow(cell));
        intList.Add(Grid.CellDownLeft(cell));
      }
    }
    else
    {
      intList.Add(Grid.CellRight(cell));
      if (flag)
      {
        intList.Add(Grid.CellAbove(cell));
        intList.Add(Grid.CellUpRight(cell));
      }
      else
      {
        intList.Add(Grid.CellBelow(cell));
        intList.Add(Grid.CellDownRight(cell));
      }
    }
    return intList;
  }

  public static bool CanPlaceSticker(List<int> offsets)
  {
    foreach (int offset in offsets)
    {
      if (Grid.IsCellOpenToSpace(offset))
        return false;
    }
    return true;
  }

  public void SetStickerType(string newStickerType)
  {
    if (newStickerType == null)
      newStickerType = "sticker";
    DbStickerBomb randomSticker = Db.GetStickerBombs().GetRandomSticker();
    this.stickerName = randomSticker.Id;
    this.stickerType = string.Format("{0}_{1}", (object) newStickerType, (object) randomSticker.stickerName);
    ((Component) this).GetComponent<KBatchedAnimController>().SwapAnims(new KAnimFile[1]
    {
      randomSticker.animFile
    });
  }

  public class StatesInstance : 
    GameStateMachine<StickerBomb.States, StickerBomb.StatesInstance, StickerBomb, object>.GameInstance
  {
    [Serialize]
    public float destroyTime;

    public StatesInstance(StickerBomb master)
      : base(master)
    {
    }

    public string GetStickerAnim(string type) => string.Format("{0}_{1}", (object) type, (object) this.master.stickerType);
  }

  public class States : GameStateMachine<StickerBomb.States, StickerBomb.StatesInstance, StickerBomb>
  {
    public GameStateMachine<StickerBomb.States, StickerBomb.StatesInstance, StickerBomb, object>.State destroy;
    public GameStateMachine<StickerBomb.States, StickerBomb.StatesInstance, StickerBomb, object>.State sparkle;
    public GameStateMachine<StickerBomb.States, StickerBomb.StatesInstance, StickerBomb, object>.State idle;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.idle;
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      this.root.Transition(this.destroy, (StateMachine<StickerBomb.States, StickerBomb.StatesInstance, StickerBomb, object>.Transition.ConditionCallback) (smi => (double) GameClock.Instance.GetTime() >= (double) smi.destroyTime)).DefaultState(this.idle);
      this.idle.PlayAnim((Func<StickerBomb.StatesInstance, string>) (smi => smi.GetStickerAnim("idle"))).ScheduleGoTo((Func<StickerBomb.StatesInstance, float>) (smi => (float) Random.Range(20, 30)), (StateMachine.BaseState) this.sparkle);
      this.sparkle.PlayAnim((Func<StickerBomb.StatesInstance, string>) (smi => smi.GetStickerAnim("sparkle"))).OnAnimQueueComplete(this.idle);
      this.destroy.Enter((StateMachine<StickerBomb.States, StickerBomb.StatesInstance, StickerBomb, object>.State.Callback) (smi => Util.KDestroyGameObject((Component) smi.master)));
    }
  }
}
