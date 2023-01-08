// Decompiled with JetBrains decompiler
// Type: ResearchPointObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ResearchPointObject")]
public class ResearchPointObject : KMonoBehaviour, IGameObjectEffectDescriptor
{
  public string TypeID = "";

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    Research.Instance.AddResearchPoints(this.TypeID, 1f);
    ResearchType researchType = Research.Instance.GetResearchType(this.TypeID);
    PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Research, researchType.name, this.transform);
    Util.KDestroyGameObject(((Component) this).gameObject);
  }

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    ResearchType researchType = Research.Instance.GetResearchType(this.TypeID);
    descriptors.Add(new Descriptor(string.Format((string) UI.UISIDESCREENS.FABRICATORSIDESCREEN.EFFECTS.RESEARCHPOINT, (object) researchType.name), string.Format((string) UI.UISIDESCREENS.FABRICATORSIDESCREEN.EFFECTS.RESEARCHPOINT, (object) researchType.description), (Descriptor.DescriptorType) 1, false));
    return descriptors;
  }
}
