// Decompiled with JetBrains decompiler
// Type: AssignmentGroupController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using UnityEngine;

public class AssignmentGroupController : KMonoBehaviour
{
  public bool generateGroupOnStart;
  [Serialize]
  private string _assignmentGroupID;
  [Serialize]
  private Ref<MinionAssignablesProxy>[] minionsInGroupAtLoad;

  public string AssignmentGroupID
  {
    get => this._assignmentGroupID;
    private set => this._assignmentGroupID = value;
  }

  protected virtual void OnPrefabInit() => base.OnPrefabInit();

  [OnDeserialized]
  protected void CreateOrRestoreGroupID()
  {
    if (string.IsNullOrEmpty(this.AssignmentGroupID))
      this.GenerateGroupID();
    else
      Game.Instance.assignmentManager.TryCreateAssignmentGroup(this.AssignmentGroupID, new IAssignableIdentity[0], ((Component) this).gameObject.GetProperName());
  }

  public void SetGroupID(string id)
  {
    DebugUtil.DevAssert(!string.IsNullOrEmpty(id), "Trying to set Assignment group on " + ((Object) ((Component) this).gameObject).name + " to null or empty.", (Object) null);
    if (string.IsNullOrEmpty(id))
      return;
    this.AssignmentGroupID = id;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.RestoreGroupAssignees();
  }

  private void GenerateGroupID()
  {
    if (!this.generateGroupOnStart || !string.IsNullOrEmpty(this.AssignmentGroupID))
      return;
    this.SetGroupID(((Component) this).GetComponent<KPrefabID>().PrefabID().ToString() + "_" + ((Component) this).GetComponent<KPrefabID>().InstanceID.ToString() + "_assignmentGroup");
    Game.Instance.assignmentManager.TryCreateAssignmentGroup(this.AssignmentGroupID, new IAssignableIdentity[0], ((Component) this).gameObject.GetProperName());
  }

  private void RestoreGroupAssignees()
  {
    if (!this.generateGroupOnStart)
      return;
    this.CreateOrRestoreGroupID();
    if (this.minionsInGroupAtLoad == null)
      this.minionsInGroupAtLoad = new Ref<MinionAssignablesProxy>[0];
    for (int index = 0; index < this.minionsInGroupAtLoad.Length; ++index)
      Game.Instance.assignmentManager.AddToAssignmentGroup(this.AssignmentGroupID, (IAssignableIdentity) this.minionsInGroupAtLoad[index].Get());
    Ownable component = ((Component) this).GetComponent<Ownable>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.Assign((IAssignableIdentity) Game.Instance.assignmentManager.assignment_groups[this.AssignmentGroupID]);
    component.SetCanBeAssigned(false);
  }

  public bool CheckMinionIsMember(MinionAssignablesProxy minion)
  {
    if (string.IsNullOrEmpty(this.AssignmentGroupID))
      this.GenerateGroupID();
    return Game.Instance.assignmentManager.assignment_groups[this.AssignmentGroupID].HasMember((IAssignableIdentity) minion);
  }

  public void SetMember(MinionAssignablesProxy minion, bool isAllowed)
  {
    Debug.Assert(DlcManager.IsExpansion1Active());
    if (!isAllowed)
    {
      Game.Instance.assignmentManager.RemoveFromAssignmentGroup(this.AssignmentGroupID, (IAssignableIdentity) minion);
    }
    else
    {
      if (this.CheckMinionIsMember(minion))
        return;
      Game.Instance.assignmentManager.AddToAssignmentGroup(this.AssignmentGroupID, (IAssignableIdentity) minion);
    }
  }

  protected virtual void OnCleanUp()
  {
    if (this.generateGroupOnStart)
      Game.Instance.assignmentManager.RemoveAssignmentGroup(this.AssignmentGroupID);
    base.OnCleanUp();
  }

  [OnSerializing]
  private void OnSerialize()
  {
    Debug.Assert(!string.IsNullOrEmpty(this.AssignmentGroupID), (object) ("Assignment group on " + ((Object) ((Component) this).gameObject).name + " has null or empty ID"));
    ReadOnlyCollection<IAssignableIdentity> members = Game.Instance.assignmentManager.assignment_groups[this.AssignmentGroupID].GetMembers();
    this.minionsInGroupAtLoad = new Ref<MinionAssignablesProxy>[members.Count];
    for (int index = 0; index < members.Count; ++index)
      this.minionsInGroupAtLoad[index] = new Ref<MinionAssignablesProxy>((MinionAssignablesProxy) members[index]);
  }

  public ReadOnlyCollection<IAssignableIdentity> GetMembers() => Game.Instance.assignmentManager.assignment_groups[this.AssignmentGroupID].GetMembers();
}
