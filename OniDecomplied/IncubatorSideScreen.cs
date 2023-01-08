// Decompiled with JetBrains decompiler
// Type: IncubatorSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TMPro;
using UnityEngine;

public class IncubatorSideScreen : ReceptacleSideScreen
{
  public DescriptorPanel RequirementsDescriptorPanel;
  public DescriptorPanel HarvestDescriptorPanel;
  public DescriptorPanel EffectsDescriptorPanel;
  public MultiToggle continuousToggle;

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<EggIncubator>(), (Object) null);

  protected override void SetResultDescriptions(GameObject go)
  {
    string str = "";
    InfoDescription component = go.GetComponent<InfoDescription>();
    if (Object.op_Implicit((Object) component))
      str += component.description;
    ((TMP_Text) this.descriptionLabel).SetText(str);
  }

  protected override bool RequiresAvailableAmountToDeposit() => false;

  protected override Sprite GetEntityIcon(Tag prefabTag) => Def.GetUISprite((object) Assets.GetPrefab(prefabTag)).first;

  public override void SetTarget(GameObject target)
  {
    base.SetTarget(target);
    EggIncubator incubator = target.GetComponent<EggIncubator>();
    this.continuousToggle.ChangeState(incubator.autoReplaceEntity ? 0 : 1);
    this.continuousToggle.onClick = (System.Action) (() =>
    {
      incubator.autoReplaceEntity = !incubator.autoReplaceEntity;
      this.continuousToggle.ChangeState(incubator.autoReplaceEntity ? 0 : 1);
    });
  }
}
