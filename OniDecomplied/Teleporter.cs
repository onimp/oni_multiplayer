// Decompiled with JetBrains decompiler
// Type: Teleporter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : KMonoBehaviour
{
  [MyCmpReq]
  private Operational operational;
  [Serialize]
  public Ref<Teleporter> teleportTarget = new Ref<Teleporter>();
  public int ID_LENGTH = 4;
  private static readonly EventSystem.IntraObjectHandler<Teleporter> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<Teleporter>((Action<Teleporter, object>) ((component, data) => component.OnLogicValueChanged(data)));

  [Serialize]
  public int teleporterID { get; private set; }

  protected virtual void OnPrefabInit() => base.OnPrefabInit();

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    Components.Teleporters.Add(this);
    this.SetTeleporterID(0);
    this.Subscribe<Teleporter>(-801688580, Teleporter.OnLogicValueChangedDelegate);
  }

  private void OnLogicValueChanged(object data)
  {
    LogicPorts component = ((Component) this).GetComponent<LogicPorts>();
    LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
    List<int> intList = new List<int>();
    int ID = 0;
    int num1 = Mathf.Min(this.ID_LENGTH, component.inputPorts.Count);
    for (int index = 0; index < num1; ++index)
    {
      int logicUiCell = component.inputPorts[index].GetLogicUICell();
      LogicCircuitNetwork networkForCell = logicCircuitManager.GetNetworkForCell(logicUiCell);
      int num2 = networkForCell != null ? networkForCell.OutputValue : 1;
      intList.Add(num2);
    }
    foreach (int num3 in intList)
      ID = ID << 1 | num3;
    this.SetTeleporterID(ID);
  }

  protected virtual void OnCleanUp()
  {
    Components.Teleporters.Remove(this);
    base.OnCleanUp();
  }

  public bool HasTeleporterTarget() => Object.op_Inequality((Object) this.FindTeleportTarget(), (Object) null);

  public bool IsValidTeleportTarget(Teleporter from_tele) => from_tele.teleporterID == this.teleporterID && this.operational.IsOperational;

  public Teleporter FindTeleportTarget()
  {
    List<Teleporter> teleporterList = new List<Teleporter>();
    foreach (Teleporter teleporter in Components.Teleporters)
    {
      if (teleporter.IsValidTeleportTarget(this) && Object.op_Inequality((Object) teleporter, (Object) this))
        teleporterList.Add(teleporter);
    }
    Teleporter teleportTarget = (Teleporter) null;
    if (teleporterList.Count > 0)
      teleportTarget = Util.GetRandom<Teleporter>(teleporterList);
    return teleportTarget;
  }

  public void SetTeleporterID(int ID)
  {
    this.teleporterID = ID;
    foreach (KMonoBehaviour teleporter in Components.Teleporters)
      teleporter.Trigger(-1266722732, (object) null);
  }

  public void SetTeleportTarget(Teleporter target) => this.teleportTarget.Set(target);

  public void TeleportObjects()
  {
    Teleporter cmp = this.teleportTarget.Get();
    int widthInCells = ((Component) this).GetComponent<Building>().Def.WidthInCells;
    int height = ((Component) this).GetComponent<Building>().Def.HeightInCells - 1;
    Vector3 position = TransformExtensions.GetPosition(this.transform);
    if (Object.op_Inequality((Object) cmp, (Object) null))
    {
      ListPool<ScenePartitionerEntry, Teleporter>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, Teleporter>.Allocate();
      GameScenePartitioner.Instance.GatherEntries((int) position.x - widthInCells / 2 + 1, (int) position.y - height / 2 + 1, widthInCells, height, GameScenePartitioner.Instance.pickupablesLayer, (List<ScenePartitionerEntry>) gathered_entries);
      int cell = Grid.PosToCell((KMonoBehaviour) cmp);
      foreach (ScenePartitionerEntry partitionerEntry in (List<ScenePartitionerEntry>) gathered_entries)
      {
        GameObject gameObject = ((Component) (partitionerEntry.obj as Pickupable)).gameObject;
        Vector3 vector3 = Vector3.op_Subtraction(TransformExtensions.GetPosition(gameObject.transform), position);
        MinionIdentity component = gameObject.GetComponent<MinionIdentity>();
        if (Object.op_Inequality((Object) component, (Object) null))
        {
          EmoteChore emoteChore = new EmoteChore((IStateMachineTarget) ((Component) component).GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, HashedString.op_Implicit("anim_interacts_portal_kanim"), Telepad.PortalBirthAnim);
        }
        else
          vector3 = Vector3.op_Addition(vector3, Vector3.up);
        TransformExtensions.SetLocalPosition(gameObject.transform, Vector3.op_Addition(Grid.CellToPosCBC(cell, Grid.SceneLayer.Move), vector3));
      }
      gathered_entries.Recycle();
    }
    TeleportalPad.StatesInstance smi = ((Component) this.teleportTarget.Get()).GetSMI<TeleportalPad.StatesInstance>();
    smi.sm.doTeleport.Trigger(smi);
    this.teleportTarget.Set((Teleporter) null);
  }
}
