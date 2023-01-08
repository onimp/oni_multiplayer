// Decompiled with JetBrains decompiler
// Type: SuitWearer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class SuitWearer : GameStateMachine<SuitWearer, SuitWearer.Instance>
{
  public GameStateMachine<SuitWearer, SuitWearer.Instance, IStateMachineTarget, object>.State suit;
  public GameStateMachine<SuitWearer, SuitWearer.Instance, IStateMachineTarget, object>.State nosuit;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.root;
    this.root.EventHandler(GameHashes.PathAdvanced, (GameStateMachine<SuitWearer, SuitWearer.Instance, IStateMachineTarget, object>.GameEvent.Callback) ((smi, data) => smi.OnPathAdvanced(data))).DoNothing();
    this.suit.DoNothing();
    this.nosuit.DoNothing();
  }

  public new class Instance : 
    GameStateMachine<SuitWearer, SuitWearer.Instance, IStateMachineTarget, object>.GameInstance
  {
    private List<int> suitReservations = new List<int>();
    private List<int> emptyLockerReservations = new List<int>();
    private Navigator navigator;
    private int prefabInstanceID;

    public Instance(IStateMachineTarget master)
      : base(master)
    {
      this.navigator = master.GetComponent<Navigator>();
      this.navigator.SetFlags(PathFinder.PotentialPath.Flags.PerformSuitChecks);
      this.prefabInstanceID = ((Component) this.navigator).GetComponent<KPrefabID>().InstanceID;
      master.GetComponent<KBatchedAnimController>().SetSymbolVisiblity(KAnimHashedString.op_Implicit("snapto_neck"), false);
    }

    public void OnPathAdvanced(object data)
    {
      if (this.navigator.CurrentNavType == NavType.Hover && (this.navigator.flags & PathFinder.PotentialPath.Flags.HasJetPack) == 0)
        this.navigator.SetCurrentNavType(NavType.Floor);
      this.UnreserveSuits();
      this.ReserveSuits();
    }

    public void ReserveSuits()
    {
      PathFinder.Path path = this.navigator.path;
      if (path.nodes == null)
        return;
      bool flag1 = (this.navigator.flags & PathFinder.PotentialPath.Flags.HasAtmoSuit) != 0;
      bool flag2 = (this.navigator.flags & PathFinder.PotentialPath.Flags.HasJetPack) != 0;
      bool flag3 = (this.navigator.flags & PathFinder.PotentialPath.Flags.HasOxygenMask) != 0;
      for (int index = 0; index < path.nodes.Count - 1; ++index)
      {
        int cell = path.nodes[index].cell;
        Grid.SuitMarker.Flags flags = (Grid.SuitMarker.Flags) 0;
        PathFinder.PotentialPath.Flags pathFlags = PathFinder.PotentialPath.Flags.None;
        if (Grid.TryGetSuitMarkerFlags(cell, out flags, out pathFlags))
        {
          bool flag4 = (pathFlags & PathFinder.PotentialPath.Flags.HasAtmoSuit) != 0;
          bool flag5 = (pathFlags & PathFinder.PotentialPath.Flags.HasJetPack) != 0;
          bool flag6 = (pathFlags & PathFinder.PotentialPath.Flags.HasOxygenMask) != 0;
          bool flag7 = flag2 | flag1 | flag3;
          bool flag8 = flag4 == flag1 && flag5 == flag2 && flag6 == flag3;
          bool flag9 = SuitMarker.DoesTraversalDirectionRequireSuit(cell, path.nodes[index + 1].cell, flags);
          if (flag9 && !flag7)
          {
            Grid.ReserveSuit(cell, this.prefabInstanceID, true);
            this.suitReservations.Add(cell);
            if (flag4)
              flag1 = true;
            if (flag5)
              flag2 = true;
            if (flag6)
              flag3 = true;
          }
          else if (!flag9 & flag8 && Grid.HasEmptyLocker(cell, this.prefabInstanceID))
          {
            Grid.ReserveEmptyLocker(cell, this.prefabInstanceID, true);
            this.emptyLockerReservations.Add(cell);
            if (flag4)
              flag1 = false;
            if (flag5)
              flag2 = false;
            if (flag6)
              flag3 = false;
          }
        }
      }
    }

    public void UnreserveSuits()
    {
      foreach (int suitReservation in this.suitReservations)
      {
        if (Grid.HasSuitMarker[suitReservation])
          Grid.ReserveSuit(suitReservation, this.prefabInstanceID, false);
      }
      this.suitReservations.Clear();
      foreach (int lockerReservation in this.emptyLockerReservations)
      {
        if (Grid.HasSuitMarker[lockerReservation])
          Grid.ReserveEmptyLocker(lockerReservation, this.prefabInstanceID, false);
      }
      this.emptyLockerReservations.Clear();
    }
  }
}
