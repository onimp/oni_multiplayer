// Decompiled with JetBrains decompiler
// Type: SicknessTrigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SicknessTrigger")]
public class SicknessTrigger : KMonoBehaviour, IGameObjectEffectDescriptor
{
  public List<SicknessTrigger.TriggerInfo> triggers = new List<SicknessTrigger.TriggerInfo>();

  public void AddTrigger(
    GameHashes src_event,
    string[] sickness_ids,
    SicknessTrigger.SourceCallback source_callback)
  {
    this.triggers.Add(new SicknessTrigger.TriggerInfo()
    {
      srcEvent = src_event,
      sickness_ids = sickness_ids,
      sourceCallback = source_callback
    });
  }

  protected virtual void OnSpawn()
  {
    for (int index = 0; index < this.triggers.Count; ++index)
    {
      SicknessTrigger.TriggerInfo trigger = this.triggers[index];
      this.Subscribe((int) trigger.srcEvent, (Action<object>) (data => this.OnSicknessTrigger((GameObject) data, trigger)));
    }
  }

  private void OnSicknessTrigger(GameObject target, SicknessTrigger.TriggerInfo trigger)
  {
    int index1 = Random.Range(0, trigger.sickness_ids.Length);
    string sicknessId = trigger.sickness_ids[index1];
    Sickness sickness = (Sickness) null;
    Database.Sicknesses sicknesses = Db.Get().Sicknesses;
    for (int index2 = 0; index2 < ((ResourceSet) sicknesses).Count; ++index2)
    {
      if (sicknesses[index2].Id == sicknessId)
      {
        sickness = sicknesses[index2];
        break;
      }
    }
    if (sickness != null)
    {
      string infection_source_info = trigger.sourceCallback(((Component) this).gameObject, target);
      SicknessExposureInfo exposure_info = new SicknessExposureInfo(sickness.Id, infection_source_info);
      target.GetComponent<MinionModifiers>().sicknesses.Infect(exposure_info);
    }
    else
      DebugUtil.DevLogErrorFormat((Object) ((Component) this).gameObject, "Couldn't find sickness with id [{0}]", new object[1]
      {
        (object) sicknessId
      });
  }

  public List<Descriptor> EffectDescriptors(GameObject go)
  {
    Dictionary<GameHashes, HashSet<string>> dictionary = new Dictionary<GameHashes, HashSet<string>>();
    foreach (SicknessTrigger.TriggerInfo trigger in this.triggers)
    {
      HashSet<string> stringSet = (HashSet<string>) null;
      if (!dictionary.TryGetValue(trigger.srcEvent, out stringSet))
      {
        stringSet = new HashSet<string>();
        dictionary[trigger.srcEvent] = stringSet;
      }
      foreach (string sicknessId in trigger.sickness_ids)
        stringSet.Add(sicknessId);
    }
    List<Descriptor> descriptorList = new List<Descriptor>();
    List<string> stringList = new List<string>();
    string properName = ((Component) this).GetComponent<KSelectable>().GetProperName();
    foreach (KeyValuePair<GameHashes, HashSet<string>> keyValuePair in dictionary)
    {
      HashSet<string> stringSet = keyValuePair.Value;
      stringList.Clear();
      foreach (string str in stringSet)
      {
        Sickness sickness = Db.Get().Sicknesses.TryGet(str);
        stringList.Add(sickness.Name);
      }
      string newValue = string.Join(", ", stringList.ToArray());
      string str1 = Strings.Get("STRINGS.DUPLICANTS.DISEASES.TRIGGERS." + Enum.GetName(typeof (GameHashes), (object) keyValuePair.Key).ToUpper()).String;
      string str2 = Strings.Get("STRINGS.DUPLICANTS.DISEASES.TRIGGERS.TOOLTIPS." + Enum.GetName(typeof (GameHashes), (object) keyValuePair.Key).ToUpper()).String;
      string str3 = str1.Replace("{ItemName}", properName).Replace("{Diseases}", newValue);
      string str4 = str2.Replace("{ItemName}", properName).Replace("{Diseases}", newValue);
      descriptorList.Add(new Descriptor(str3, str4, (Descriptor.DescriptorType) 1, false));
    }
    return descriptorList;
  }

  public List<Descriptor> GetDescriptors(GameObject go) => this.EffectDescriptors(go);

  public delegate string SourceCallback(GameObject source, GameObject target);

  [Serializable]
  public struct TriggerInfo
  {
    [HashedEnum]
    public GameHashes srcEvent;
    public string[] sickness_ids;
    public SicknessTrigger.SourceCallback sourceCallback;
  }
}
