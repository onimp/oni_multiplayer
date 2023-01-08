// Decompiled with JetBrains decompiler
// Type: SingleSliderSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class SingleSliderSideScreen : SideScreenContent
{
  private ISingleSliderControl target;
  public List<SliderSet> sliderSets;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    for (int index = 0; index < this.sliderSets.Count; ++index)
      this.sliderSets[index].SetupSlider(index);
  }

  public override bool IsValidForTarget(GameObject target)
  {
    KPrefabID component = target.GetComponent<KPrefabID>();
    return (target.GetComponent<ISingleSliderControl>() != null || target.GetSMI<ISingleSliderControl>() != null) && !component.IsPrefabID(TagExtensions.ToTag("HydrogenGenerator")) && !component.IsPrefabID(TagExtensions.ToTag("MethaneGenerator")) && !component.IsPrefabID(TagExtensions.ToTag("PetroleumGenerator")) && !component.IsPrefabID(TagExtensions.ToTag("DevGenerator")) && !component.HasTag(GameTags.DeadReactor);
  }

  public override void SetTarget(GameObject new_target)
  {
    if (Object.op_Equality((Object) new_target, (Object) null))
    {
      Debug.LogError((object) "Invalid gameObject received");
    }
    else
    {
      this.target = new_target.GetComponent<ISingleSliderControl>();
      if (this.target == null)
      {
        this.target = new_target.GetSMI<ISingleSliderControl>();
        if (this.target == null)
        {
          Debug.LogError((object) "The gameObject received does not contain a ISingleSliderControl implementation");
          return;
        }
      }
      this.titleKey = this.target.SliderTitleKey;
      for (int index = 0; index < this.sliderSets.Count; ++index)
        this.sliderSets[index].SetTarget((ISliderControl) this.target);
    }
  }
}
