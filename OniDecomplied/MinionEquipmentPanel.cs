// Decompiled with JetBrains decompiler
// Type: MinionEquipmentPanel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/MinionEquipmentPanel")]
public class MinionEquipmentPanel : KMonoBehaviour
{
  public GameObject SelectedMinion;
  public GameObject labelTemplate;
  private GameObject roomPanel;
  private GameObject ownablePanel;
  private Storage storage;
  private Dictionary<string, GameObject> labels = new Dictionary<string, GameObject>();
  private Action<object> refreshDelegate;

  public MinionEquipmentPanel() => this.refreshDelegate = new Action<object>(this.Refresh);

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.roomPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, ((Component) this).gameObject, false);
    ((TMP_Text) this.roomPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel).text = (string) UI.DETAILTABS.PERSONALITY.EQUIPMENT.GROUPNAME_ROOMS;
    this.roomPanel.SetActive(true);
    this.ownablePanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, ((Component) this).gameObject, false);
    ((TMP_Text) this.ownablePanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel).text = (string) UI.DETAILTABS.PERSONALITY.EQUIPMENT.GROUPNAME_OWNABLE;
    this.ownablePanel.SetActive(true);
  }

  public void SetSelectedMinion(GameObject minion)
  {
    if (Object.op_Inequality((Object) this.SelectedMinion, (Object) null))
    {
      KMonoBehaviourExtensions.Unsubscribe(this.SelectedMinion, -448952673, this.refreshDelegate);
      KMonoBehaviourExtensions.Unsubscribe(this.SelectedMinion, -1285462312, this.refreshDelegate);
      KMonoBehaviourExtensions.Unsubscribe(this.SelectedMinion, -1585839766, this.refreshDelegate);
    }
    this.SelectedMinion = minion;
    KMonoBehaviourExtensions.Subscribe(this.SelectedMinion, -448952673, this.refreshDelegate);
    KMonoBehaviourExtensions.Subscribe(this.SelectedMinion, -1285462312, this.refreshDelegate);
    KMonoBehaviourExtensions.Subscribe(this.SelectedMinion, -1585839766, this.refreshDelegate);
    this.Refresh();
  }

  public void Refresh(object data = null)
  {
    if (Object.op_Equality((Object) this.SelectedMinion, (Object) null))
      return;
    this.Build();
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    if (!Object.op_Inequality((Object) this.SelectedMinion, (Object) null))
      return;
    KMonoBehaviourExtensions.Unsubscribe(this.SelectedMinion, -448952673, this.refreshDelegate);
    KMonoBehaviourExtensions.Unsubscribe(this.SelectedMinion, -1285462312, this.refreshDelegate);
    KMonoBehaviourExtensions.Unsubscribe(this.SelectedMinion, -1585839766, this.refreshDelegate);
  }

  private GameObject AddOrGetLabel(
    Dictionary<string, GameObject> labels,
    GameObject panel,
    string id)
  {
    GameObject label;
    if (labels.ContainsKey(id))
    {
      label = labels[id];
    }
    else
    {
      label = Util.KInstantiate(this.labelTemplate, ((Component) panel.GetComponent<CollapsibleDetailContentPanel>().Content).gameObject, (string) null);
      label.transform.localScale = new Vector3(1f, 1f, 1f);
      labels[id] = label;
    }
    label.SetActive(true);
    return label;
  }

  private void Build()
  {
    this.ShowAssignables((Assignables) this.SelectedMinion.GetComponent<MinionIdentity>().GetSoleOwner(), this.roomPanel);
    this.ShowAssignables((Assignables) this.SelectedMinion.GetComponent<MinionIdentity>().GetEquipment(), this.ownablePanel);
  }

  private void ShowAssignables(Assignables assignables, GameObject panel)
  {
    bool flag = false;
    foreach (AssignableSlotInstance slot in assignables.Slots)
    {
      if (slot.slot.showInUI)
      {
        GameObject label = this.AddOrGetLabel(this.labels, panel, slot.slot.Name);
        if (slot.IsAssigned())
        {
          label.SetActive(true);
          flag = true;
          string str = slot.IsAssigned() ? ((Component) slot.assignable).GetComponent<KSelectable>().GetName() : UI.DETAILTABS.PERSONALITY.EQUIPMENT.NO_ASSIGNABLES.text;
          ((TMP_Text) label.GetComponent<LocText>()).text = string.Format("{0}: {1}", (object) slot.slot.Name, (object) str);
          label.GetComponent<ToolTip>().toolTip = string.Format((string) UI.DETAILTABS.PERSONALITY.EQUIPMENT.ASSIGNED_TOOLTIP, (object) str, (object) this.GetAssignedEffectsString(slot), (object) ((Object) this.SelectedMinion).name);
        }
        else
        {
          label.SetActive(false);
          ((TMP_Text) label.GetComponent<LocText>()).text = (string) UI.DETAILTABS.PERSONALITY.EQUIPMENT.NO_ASSIGNABLES;
          label.GetComponent<ToolTip>().toolTip = (string) UI.DETAILTABS.PERSONALITY.EQUIPMENT.NO_ASSIGNABLES_TOOLTIP;
        }
      }
    }
    if (assignables is Ownables)
    {
      if (!flag)
      {
        GameObject label = this.AddOrGetLabel(this.labels, panel, "NothingAssigned");
        this.labels["NothingAssigned"].SetActive(true);
        ((TMP_Text) label.GetComponent<LocText>()).text = (string) UI.DETAILTABS.PERSONALITY.EQUIPMENT.NO_ASSIGNABLES;
        label.GetComponent<ToolTip>().toolTip = string.Format((string) UI.DETAILTABS.PERSONALITY.EQUIPMENT.NO_ASSIGNABLES_TOOLTIP, (object) ((Object) this.SelectedMinion).name);
      }
      else if (this.labels.ContainsKey("NothingAssigned"))
        this.labels["NothingAssigned"].SetActive(false);
    }
    if (!(assignables is Equipment))
      return;
    if (!flag)
    {
      GameObject label = this.AddOrGetLabel(this.labels, panel, "NoSuitAssigned");
      this.labels["NoSuitAssigned"].SetActive(true);
      ((TMP_Text) label.GetComponent<LocText>()).text = (string) UI.DETAILTABS.PERSONALITY.EQUIPMENT.NOEQUIPMENT;
      label.GetComponent<ToolTip>().toolTip = string.Format((string) UI.DETAILTABS.PERSONALITY.EQUIPMENT.NOEQUIPMENT_TOOLTIP, (object) ((Object) this.SelectedMinion).name);
    }
    else
    {
      if (!this.labels.ContainsKey("NoSuitAssigned"))
        return;
      this.labels["NoSuitAssigned"].SetActive(false);
    }
  }

  private string GetAssignedEffectsString(AssignableSlotInstance slot)
  {
    string assignedEffectsString = "";
    List<Descriptor> descriptorList = new List<Descriptor>();
    descriptorList.AddRange((IEnumerable<Descriptor>) GameUtil.GetGameObjectEffects(((Component) slot.assignable).gameObject));
    if (descriptorList.Count > 0)
    {
      assignedEffectsString += "\n";
      foreach (Descriptor descriptor in descriptorList)
        assignedEffectsString = assignedEffectsString + "  • " + ((Descriptor) ref descriptor).IndentedText() + "\n";
    }
    return assignedEffectsString;
  }
}
