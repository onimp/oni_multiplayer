// Decompiled with JetBrains decompiler
// Type: FlatTagFilterSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlatTagFilterSideScreen : SideScreenContent
{
  private FlatTagFilterable tagFilterable;
  [SerializeField]
  private GameObject rowPrefab;
  [SerializeField]
  private GameObject listContainer;
  [SerializeField]
  private LocText headerLabel;
  private Dictionary<Tag, GameObject> rows = new Dictionary<Tag, GameObject>();

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<FlatTagFilterable>(), (Object) null);

  public override void SetTarget(GameObject target)
  {
    base.SetTarget(target);
    this.tagFilterable = target.GetComponent<FlatTagFilterable>();
    this.Build();
  }

  private void Build()
  {
    ((TMP_Text) this.headerLabel).SetText(this.tagFilterable.GetHeaderText());
    foreach (KeyValuePair<Tag, GameObject> row in this.rows)
      Util.KDestroyGameObject(row.Value);
    this.rows.Clear();
    foreach (Tag tagOption in this.tagFilterable.tagOptions)
    {
      GameObject gameObject = Util.KInstantiateUI(this.rowPrefab, this.listContainer, false);
      ((Object) gameObject.gameObject).name = tagOption.ProperName();
      this.rows.Add(tagOption, gameObject);
    }
    this.Refresh();
  }

  private void Refresh()
  {
    foreach (KeyValuePair<Tag, GameObject> row in this.rows)
    {
      KeyValuePair<Tag, GameObject> kvp = row;
      ((TMP_Text) kvp.Value.GetComponent<HierarchyReferences>().GetReference<LocText>("Label")).SetText(kvp.Key.ProperNameStripLink());
      kvp.Value.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").sprite = Def.GetUISprite((object) kvp.Key).first;
      ((Graphic) kvp.Value.GetComponent<HierarchyReferences>().GetReference<Image>("Icon")).color = Def.GetUISprite((object) kvp.Key).second;
      kvp.Value.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("Toggle").onClick = (System.Action) (() =>
      {
        this.tagFilterable.ToggleTag(kvp.Key);
        this.Refresh();
      });
      kvp.Value.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("Toggle").ChangeState(this.tagFilterable.selectedTags.Contains(kvp.Key) ? 1 : 0);
      kvp.Value.SetActive(DiscoveredResources.Instance.IsDiscovered(kvp.Key));
    }
  }
}
