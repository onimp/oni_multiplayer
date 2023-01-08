// Decompiled with JetBrains decompiler
// Type: CreatureFallMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class CreatureFallMonitor : 
  GameStateMachine<CreatureFallMonitor, CreatureFallMonitor.Instance, IStateMachineTarget, CreatureFallMonitor.Def>
{
  public static float FLOOR_DISTANCE = -0.065f;
  public GameStateMachine<CreatureFallMonitor, CreatureFallMonitor.Instance, IStateMachineTarget, CreatureFallMonitor.Def>.State grounded;
  public GameStateMachine<CreatureFallMonitor, CreatureFallMonitor.Instance, IStateMachineTarget, CreatureFallMonitor.Def>.State falling;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.grounded;
    this.grounded.ToggleBehaviour(GameTags.Creatures.Falling, (StateMachine<CreatureFallMonitor, CreatureFallMonitor.Instance, IStateMachineTarget, CreatureFallMonitor.Def>.Transition.ConditionCallback) (smi => smi.ShouldFall()));
  }

  public class Def : StateMachine.BaseDef
  {
    public bool canSwim;
    public bool checkHead = true;
  }

  public new class Instance : 
    GameStateMachine<CreatureFallMonitor, CreatureFallMonitor.Instance, IStateMachineTarget, CreatureFallMonitor.Def>.GameInstance
  {
    public string anim = "fall";
    private Navigator navigator;

    public Instance(IStateMachineTarget master, CreatureFallMonitor.Def def)
      : base(master, def)
    {
      this.navigator = master.GetComponent<Navigator>();
    }

    public void SnapToGround()
    {
      Vector3 position = TransformExtensions.GetPosition(this.smi.transform);
      Vector3 posCbc = Grid.CellToPosCBC(Grid.PosToCell(position), Grid.SceneLayer.Creatures);
      posCbc.x = position.x;
      TransformExtensions.SetPosition(this.smi.transform, posCbc);
      if (this.navigator.IsValidNavType(NavType.Floor))
      {
        this.navigator.SetCurrentNavType(NavType.Floor);
      }
      else
      {
        if (!this.navigator.IsValidNavType(NavType.Hover))
          return;
        this.navigator.SetCurrentNavType(NavType.Hover);
      }
    }

    public bool ShouldFall()
    {
      if (this.gameObject.HasTag(GameTags.Stored))
        return false;
      Vector3 position = TransformExtensions.GetPosition(this.smi.transform);
      int cell1 = Grid.PosToCell(position);
      if ((!Grid.IsValidCell(cell1) ? 0 : (Grid.Solid[cell1] ? 1 : 0)) != 0 || this.navigator.IsMoving() || this.CanSwimAtCurrentLocation())
        return false;
      if (this.navigator.CurrentNavType != NavType.Swim)
      {
        if (this.navigator.NavGrid.NavTable.IsValid(cell1, this.navigator.CurrentNavType))
          return false;
        if (this.navigator.CurrentNavType == NavType.Ceiling || this.navigator.CurrentNavType == NavType.LeftWall || this.navigator.CurrentNavType == NavType.RightWall)
          return true;
      }
      Vector3 pos = position;
      pos.y += CreatureFallMonitor.FLOOR_DISTANCE;
      int cell2 = Grid.PosToCell(pos);
      return (!Grid.IsValidCell(cell2) ? 0 : (Grid.Solid[cell2] ? 1 : 0)) == 0;
    }

    public bool CanSwimAtCurrentLocation()
    {
      if (this.def.canSwim)
      {
        Vector3 position = TransformExtensions.GetPosition(this.transform);
        float num = 1f;
        if (!this.def.checkHead)
          num = 0.5f;
        position.y += ((Component) this.transform).GetComponent<KBoxCollider2D>().size.y * num;
        if (Grid.IsSubstantialLiquid(Grid.PosToCell(position)) && (!((KComponentManager<GravityComponent>) GameComps.Gravities).Has((object) this.gameObject) || (double) ((Vector2) ref ((KCompactedVector<GravityComponent>) GameComps.Gravities).GetData(GameComps.Gravities.GetHandle(this.gameObject)).velocity).magnitude < 2.0))
          return true;
      }
      return false;
    }
  }
}
