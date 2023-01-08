// Decompiled with JetBrains decompiler
// Type: LaunchableRocket
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class LaunchableRocket : 
  StateMachineComponent<LaunchableRocket.StatesInstance>,
  ILaunchableRocket
{
  public List<GameObject> parts = new List<GameObject>();
  [Serialize]
  private int takeOffLocation;
  [Serialize]
  private float flightAnimOffset;
  private GameObject soundSpeakerObject;

  public LaunchableRocketRegisterType registerType => LaunchableRocketRegisterType.Spacecraft;

  public GameObject LaunchableGameObject => ((Component) this).gameObject;

  public float rocketSpeed { get; private set; }

  public bool isLanding { get; private set; }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.master.parts = AttachableBuilding.GetAttachedNetwork(((Component) this.smi.master).GetComponent<AttachableBuilding>());
    if (SpacecraftManager.instance.GetSpacecraftID((ILaunchableRocket) this) == -1)
    {
      Spacecraft craft = new Spacecraft(((Component) this).GetComponent<LaunchConditionManager>());
      craft.GenerateName();
      SpacecraftManager.instance.RegisterSpacecraft(craft);
    }
    this.smi.StartSM();
  }

  public List<GameObject> GetEngines()
  {
    List<GameObject> engines = new List<GameObject>();
    foreach (GameObject part in this.parts)
    {
      if (Object.op_Implicit((Object) part.GetComponent<RocketEngine>()))
        engines.Add(part);
    }
    return engines;
  }

  protected override void OnCleanUp()
  {
    SpacecraftManager.instance.UnregisterSpacecraft(((Component) this).GetComponent<LaunchConditionManager>());
    base.OnCleanUp();
  }

  public class StatesInstance : 
    GameStateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.GameInstance
  {
    public StatesInstance(LaunchableRocket master)
      : base(master)
    {
    }

    public bool IsMissionState(Spacecraft.MissionState state) => SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(((Component) this.master).GetComponent<LaunchConditionManager>()).state == state;

    public void SetMissionState(Spacecraft.MissionState state) => SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(((Component) this.master).GetComponent<LaunchConditionManager>()).SetState(state);
  }

  public class States : 
    GameStateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket>
  {
    public GameStateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.State grounded;
    public LaunchableRocket.States.NotGroundedStates not_grounded;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.grounded;
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      this.grounded.ToggleTag(GameTags.RocketOnGround).Enter((StateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.State.Callback) (smi =>
      {
        foreach (GameObject part in smi.master.parts)
        {
          if (!Object.op_Equality((Object) part, (Object) null))
            part.AddTag(GameTags.RocketOnGround);
        }
      })).Exit((StateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.State.Callback) (smi =>
      {
        foreach (GameObject part in smi.master.parts)
        {
          if (!Object.op_Equality((Object) part, (Object) null))
            part.RemoveTag(GameTags.RocketOnGround);
        }
      })).EventTransition(GameHashes.DoLaunchRocket, this.not_grounded.launch_pre).Enter((StateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.State.Callback) (smi =>
      {
        smi.master.rocketSpeed = 0.0f;
        foreach (GameObject part in smi.master.parts)
        {
          if (!Object.op_Equality((Object) part, (Object) null))
            part.GetComponent<KBatchedAnimController>().Offset = Vector3.zero;
        }
        smi.SetMissionState(Spacecraft.MissionState.Grounded);
      }));
      this.not_grounded.ToggleTag(GameTags.RocketNotOnGround).Enter((StateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.State.Callback) (smi =>
      {
        foreach (GameObject part in smi.master.parts)
        {
          if (!Object.op_Equality((Object) part, (Object) null))
            part.AddTag(GameTags.RocketNotOnGround);
        }
      })).Exit((StateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.State.Callback) (smi =>
      {
        foreach (GameObject part in smi.master.parts)
        {
          if (!Object.op_Equality((Object) part, (Object) null))
            part.RemoveTag(GameTags.RocketNotOnGround);
        }
      }));
      this.not_grounded.launch_pre.Enter((StateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.State.Callback) (smi =>
      {
        smi.master.isLanding = false;
        smi.master.rocketSpeed = 0.0f;
        smi.master.parts = AttachableBuilding.GetAttachedNetwork(((Component) smi.master).GetComponent<AttachableBuilding>());
        if (Object.op_Equality((Object) smi.master.soundSpeakerObject, (Object) null))
        {
          smi.master.soundSpeakerObject = new GameObject("rocketSpeaker");
          smi.master.soundSpeakerObject.transform.SetParent(((Component) smi.master).gameObject.transform);
        }
        foreach (GameObject engine in smi.master.GetEngines())
          EventExtensions.Trigger(engine, -1358394196, (object) null);
        Game.Instance.Trigger(-1277991738, (object) smi.gameObject);
        foreach (GameObject part in smi.master.parts)
        {
          if (!Object.op_Equality((Object) part, (Object) null))
          {
            smi.master.takeOffLocation = Grid.PosToCell(((Component) smi.master).gameObject);
            EventExtensions.Trigger(part, -1277991738, (object) null);
          }
        }
        smi.SetMissionState(Spacecraft.MissionState.Launching);
      })).ScheduleGoTo(5f, (StateMachine.BaseState) this.not_grounded.launch_loop);
      this.not_grounded.launch_loop.EventTransition(GameHashes.DoReturnRocket, this.not_grounded.returning).Update((Action<LaunchableRocket.StatesInstance, float>) ((smi, dt) =>
      {
        smi.master.isLanding = false;
        bool flag = true;
        float num = Mathf.Clamp(Mathf.Pow(smi.timeinstate / 5f, 4f), 0.0f, 10f);
        smi.master.rocketSpeed = num;
        smi.master.flightAnimOffset += dt * num;
        foreach (GameObject part in smi.master.parts)
        {
          if (!Object.op_Equality((Object) part, (Object) null))
          {
            KBatchedAnimController component = part.GetComponent<KBatchedAnimController>();
            component.Offset = Vector3.op_Multiply(Vector3.up, smi.master.flightAnimOffset);
            Vector3 positionIncludingOffset = component.PositionIncludingOffset;
            if (Object.op_Equality((Object) smi.master.soundSpeakerObject, (Object) null))
            {
              smi.master.soundSpeakerObject = new GameObject("rocketSpeaker");
              smi.master.soundSpeakerObject.transform.SetParent(((Component) smi.master).gameObject.transform);
            }
            TransformExtensions.SetLocalPosition(smi.master.soundSpeakerObject.transform, Vector3.op_Multiply(smi.master.flightAnimOffset, Vector3.up));
            if (Grid.PosToXY(positionIncludingOffset).y > Singleton<KBatchedAnimUpdater>.Instance.GetVisibleSize().y)
            {
              part.GetComponent<KBatchedAnimController>().enabled = false;
            }
            else
            {
              flag = false;
              LaunchableRocket.States.DoWorldDamage(part, positionIncludingOffset);
            }
          }
        }
        if (!flag)
          return;
        smi.GoTo((StateMachine.BaseState) this.not_grounded.space);
      }), (UpdateRate) 4).Exit((StateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.State.Callback) (smi => smi.gameObject.GetMyWorld().RevealSurface()));
      this.not_grounded.space.Enter((StateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.State.Callback) (smi =>
      {
        smi.master.rocketSpeed = 0.0f;
        foreach (GameObject part in smi.master.parts)
        {
          if (!Object.op_Equality((Object) part, (Object) null))
          {
            part.GetComponent<KBatchedAnimController>().Offset = Vector3.op_Multiply(Vector3.up, smi.master.flightAnimOffset);
            part.GetComponent<KBatchedAnimController>().enabled = false;
          }
        }
        smi.SetMissionState(Spacecraft.MissionState.Underway);
      })).EventTransition(GameHashes.DoReturnRocket, this.not_grounded.returning, (StateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.Transition.ConditionCallback) (smi => smi.IsMissionState(Spacecraft.MissionState.WaitingToLand)));
      this.not_grounded.returning.Enter((StateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.State.Callback) (smi =>
      {
        smi.master.isLanding = true;
        smi.master.rocketSpeed = 0.0f;
        smi.SetMissionState(Spacecraft.MissionState.Landing);
      })).Update((Action<LaunchableRocket.StatesInstance, float>) ((smi, dt) =>
      {
        smi.master.isLanding = true;
        KBatchedAnimController component1 = ((Component) smi.master).gameObject.GetComponent<KBatchedAnimController>();
        component1.Offset = Vector3.op_Multiply(Vector3.up, smi.master.flightAnimOffset);
        float num = Mathf.Clamp(0.5f * Mathf.Abs(((Component) smi.master).gameObject.transform.position.y + component1.Offset.y - Vector3.op_Addition(Grid.CellToPos(smi.master.takeOffLocation), Vector3.op_Multiply(Vector3.down, Grid.CellSizeInMeters / 2f)).y), 0.0f, 10f) * dt;
        smi.master.rocketSpeed = num;
        smi.master.flightAnimOffset -= num;
        bool flag = true;
        if (Object.op_Equality((Object) smi.master.soundSpeakerObject, (Object) null))
        {
          smi.master.soundSpeakerObject = new GameObject("rocketSpeaker");
          smi.master.soundSpeakerObject.transform.SetParent(((Component) smi.master).gameObject.transform);
        }
        TransformExtensions.SetLocalPosition(smi.master.soundSpeakerObject.transform, Vector3.op_Multiply(smi.master.flightAnimOffset, Vector3.up));
        foreach (GameObject part in smi.master.parts)
        {
          if (!Object.op_Equality((Object) part, (Object) null))
          {
            KBatchedAnimController component2 = part.GetComponent<KBatchedAnimController>();
            component2.Offset = Vector3.op_Multiply(Vector3.up, smi.master.flightAnimOffset);
            Vector3 positionIncludingOffset = component2.PositionIncludingOffset;
            if (Grid.IsValidCell(Grid.PosToCell(part)))
              part.GetComponent<KBatchedAnimController>().enabled = true;
            else
              flag = false;
            LaunchableRocket.States.DoWorldDamage(part, positionIncludingOffset);
          }
        }
        if (!flag)
          return;
        smi.GoTo((StateMachine.BaseState) this.not_grounded.landing_loop);
      }), (UpdateRate) 4);
      this.not_grounded.landing_loop.Enter((StateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.State.Callback) (smi =>
      {
        smi.master.isLanding = true;
        int index1 = -1;
        for (int index2 = 0; index2 < smi.master.parts.Count; ++index2)
        {
          GameObject part = smi.master.parts[index2];
          if (!Object.op_Equality((Object) part, (Object) null) && Object.op_Inequality((Object) part, (Object) ((Component) smi.master).gameObject) && Object.op_Inequality((Object) part.GetComponent<RocketEngine>(), (Object) null))
            index1 = index2;
        }
        if (index1 == -1)
          return;
        EventExtensions.Trigger(smi.master.parts[index1], -1358394196, (object) null);
      })).Update((Action<LaunchableRocket.StatesInstance, float>) ((smi, dt) =>
      {
        ((Component) smi.master).gameObject.GetComponent<KBatchedAnimController>().Offset = Vector3.op_Multiply(Vector3.up, smi.master.flightAnimOffset);
        float flightAnimOffset = smi.master.flightAnimOffset;
        float num1 = 0.0f;
        float num2 = Mathf.Clamp(0.5f * flightAnimOffset, 0.0f, 10f);
        smi.master.rocketSpeed = num2;
        smi.master.flightAnimOffset -= num2 * dt;
        if (Object.op_Equality((Object) smi.master.soundSpeakerObject, (Object) null))
        {
          smi.master.soundSpeakerObject = new GameObject("rocketSpeaker");
          smi.master.soundSpeakerObject.transform.SetParent(((Component) smi.master).gameObject.transform);
        }
        TransformExtensions.SetLocalPosition(smi.master.soundSpeakerObject.transform, Vector3.op_Multiply(smi.master.flightAnimOffset, Vector3.up));
        if ((double) num2 <= 1.0 / 400.0 && (double) dt != 0.0)
        {
          ((Component) smi.master).GetComponent<KSelectable>().IsSelectable = true;
          Game.Instance.Trigger(-887025858, (object) smi.gameObject);
          num1 = 0.0f;
          foreach (GameObject part in smi.master.parts)
          {
            if (!Object.op_Equality((Object) part, (Object) null))
              EventExtensions.Trigger(part, -887025858, (object) null);
          }
          smi.GoTo((StateMachine.BaseState) this.grounded);
        }
        else
        {
          foreach (GameObject part in smi.master.parts)
          {
            if (!Object.op_Equality((Object) part, (Object) null))
            {
              KBatchedAnimController component = part.GetComponent<KBatchedAnimController>();
              component.Offset = Vector3.op_Multiply(Vector3.up, smi.master.flightAnimOffset);
              Vector3 positionIncludingOffset = component.PositionIncludingOffset;
              LaunchableRocket.States.DoWorldDamage(part, positionIncludingOffset);
            }
          }
        }
      }), (UpdateRate) 4);
    }

    private static void DoWorldDamage(GameObject part, Vector3 apparentPosition)
    {
      OccupyArea component1 = part.GetComponent<OccupyArea>();
      component1.UpdateOccupiedArea();
      foreach (CellOffset occupiedCellsOffset in component1.OccupiedCellsOffsets)
      {
        int num1 = Grid.OffsetCell(Grid.PosToCell(apparentPosition), occupiedCellsOffset);
        if (Grid.IsValidCell(num1))
        {
          if (Grid.Solid[num1])
          {
            double num2 = (double) WorldDamage.Instance.ApplyDamage(num1, 10000f, num1, (string) BUILDINGS.DAMAGESOURCES.ROCKET, (string) UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.ROCKET);
          }
          else if (Grid.FakeFloor[num1])
          {
            GameObject gameObject = Grid.Objects[num1, 39];
            if (Object.op_Inequality((Object) gameObject, (Object) null))
            {
              BuildingHP component2 = gameObject.GetComponent<BuildingHP>();
              if (Object.op_Inequality((Object) component2, (Object) null))
                EventExtensions.Trigger(gameObject, -794517298, (object) new BuildingHP.DamageSourceInfo()
                {
                  damage = component2.MaxHitPoints,
                  source = (string) BUILDINGS.DAMAGESOURCES.ROCKET,
                  popString = (string) UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.ROCKET
                });
            }
          }
        }
      }
    }

    public class NotGroundedStates : 
      GameStateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.State
    {
      public GameStateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.State launch_pre;
      public GameStateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.State space;
      public GameStateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.State launch_loop;
      public GameStateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.State returning;
      public GameStateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.State landing_loop;
    }
  }
}
