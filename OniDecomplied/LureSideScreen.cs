// Decompiled with JetBrains decompiler
// Type: LureSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LureSideScreen : SideScreenContent
{
  protected CreatureLure target_lure;
  public GameObject prefab_toggle;
  public GameObject toggle_container;
  public Dictionary<Tag, MultiToggle> toggles_by_tag;
  private Dictionary<Tag, string> baitAttractionStrings;

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<CreatureLure>(), (Object) null);

  public override void SetTarget(GameObject target)
  {
    base.SetTarget(target);
    this.target_lure = target.GetComponent<CreatureLure>();
    foreach (Tag baitType in this.target_lure.baitTypes)
    {
      Tag bait = baitType;
      Tag key = bait;
      if (!this.toggles_by_tag.ContainsKey(bait))
      {
        GameObject gameObject = Util.KInstantiateUI(this.prefab_toggle, this.toggle_container, true);
        Image reference = gameObject.GetComponent<HierarchyReferences>().GetReference<Image>("FGImage");
        ((TMP_Text) gameObject.GetComponent<HierarchyReferences>().GetReference<LocText>("Label")).text = ElementLoader.GetElement(bait).name;
        reference.sprite = Def.GetUISpriteFromMultiObjectAnim(ElementLoader.GetElement(bait).substance.anim);
        MultiToggle component = gameObject.GetComponent<MultiToggle>();
        this.toggles_by_tag.Add(key, component);
      }
      this.toggles_by_tag[bait].onClick = (System.Action) (() => this.SelectToggle(bait));
    }
    this.RefreshToggles();
  }

  public void SelectToggle(Tag tag)
  {
    if (Tag.op_Inequality(this.target_lure.activeBaitSetting, tag))
      this.target_lure.ChangeBaitSetting(tag);
    else
      this.target_lure.ChangeBaitSetting(Tag.Invalid);
    this.RefreshToggles();
  }

  private void RefreshToggles()
  {
    foreach (KeyValuePair<Tag, MultiToggle> keyValuePair in this.toggles_by_tag)
    {
      if (Tag.op_Equality(this.target_lure.activeBaitSetting, keyValuePair.Key))
        keyValuePair.Value.ChangeState(2);
      else
        keyValuePair.Value.ChangeState(1);
      ((Component) keyValuePair.Value).GetComponent<ToolTip>().SetSimpleTooltip(string.Format((string) STRINGS.UI.UISIDESCREENS.LURE.ATTRACTS, (object) ElementLoader.GetElement(keyValuePair.Key).name, (object) this.baitAttractionStrings[keyValuePair.Key]));
    }
  }

  public LureSideScreen()
  {
    Dictionary<Tag, string> dictionary = new Dictionary<Tag, string>();
    dictionary.Add(GameTags.SlimeMold, (string) CREATURES.SPECIES.PUFT.NAME);
    dictionary.Add(GameTags.Phosphorite, (string) CREATURES.SPECIES.LIGHTBUG.NAME);
    this.baitAttractionStrings = dictionary;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }
}
