// Decompiled with JetBrains decompiler
// Type: UpdateObjectCountParameter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

internal class UpdateObjectCountParameter : LoopingSoundParameterUpdater
{
  private List<UpdateObjectCountParameter.Entry> entries = new List<UpdateObjectCountParameter.Entry>();
  private static Dictionary<HashedString, UpdateObjectCountParameter.Settings> settings = new Dictionary<HashedString, UpdateObjectCountParameter.Settings>();
  private static readonly HashedString parameterHash = HashedString.op_Implicit("objectCount");

  public static UpdateObjectCountParameter.Settings GetSettings(
    HashedString path_hash,
    SoundDescription description)
  {
    UpdateObjectCountParameter.Settings settings = new UpdateObjectCountParameter.Settings();
    if (!UpdateObjectCountParameter.settings.TryGetValue(path_hash, out settings))
    {
      settings = new UpdateObjectCountParameter.Settings();
      EventDescription eventDescription = RuntimeManager.GetEventDescription(description.path);
      USER_PROPERTY userProperty1;
      settings.minObjects = ((EventDescription) ref eventDescription).getUserProperty("minObj", ref userProperty1) != null ? 1f : (float) (short) ((USER_PROPERTY) ref userProperty1).floatValue();
      USER_PROPERTY userProperty2;
      settings.maxObjects = ((EventDescription) ref eventDescription).getUserProperty("maxObj", ref userProperty2) != null ? 0.0f : ((USER_PROPERTY) ref userProperty2).floatValue();
      USER_PROPERTY userProperty3;
      if (((EventDescription) ref eventDescription).getUserProperty("curveType", ref userProperty3) == null && ((USER_PROPERTY) ref userProperty3).stringValue() == "exp")
        settings.useExponentialCurve = true;
      settings.parameterId = ((SoundDescription) ref description).GetParameterId(UpdateObjectCountParameter.parameterHash);
      settings.path = path_hash;
      UpdateObjectCountParameter.settings[path_hash] = settings;
    }
    return settings;
  }

  public static void ApplySettings(
    EventInstance ev,
    int count,
    UpdateObjectCountParameter.Settings settings)
  {
    float num = 0.0f;
    if ((double) settings.maxObjects != (double) settings.minObjects)
      num = Mathf.Clamp01((float) (((double) count - (double) settings.minObjects) / ((double) settings.maxObjects - (double) settings.minObjects)));
    if (settings.useExponentialCurve)
      num *= num;
    ((EventInstance) ref ev).setParameterByID(settings.parameterId, num, false);
  }

  public UpdateObjectCountParameter()
    : base(HashedString.op_Implicit("objectCount"))
  {
  }

  public override void Add(LoopingSoundParameterUpdater.Sound sound)
  {
    UpdateObjectCountParameter.Settings settings = UpdateObjectCountParameter.GetSettings(sound.path, sound.description);
    this.entries.Add(new UpdateObjectCountParameter.Entry()
    {
      ev = sound.ev,
      settings = settings
    });
  }

  public override void Update(float dt)
  {
    DictionaryPool<HashedString, int, LoopingSoundManager>.PooledDictionary pooledDictionary = DictionaryPool<HashedString, int, LoopingSoundManager>.Allocate();
    foreach (UpdateObjectCountParameter.Entry entry in this.entries)
    {
      int num = 0;
      ((Dictionary<HashedString, int>) pooledDictionary).TryGetValue(entry.settings.path, out num);
      ((Dictionary<HashedString, int>) pooledDictionary)[entry.settings.path] = ++num;
    }
    foreach (UpdateObjectCountParameter.Entry entry in this.entries)
    {
      int count = ((Dictionary<HashedString, int>) pooledDictionary)[entry.settings.path];
      UpdateObjectCountParameter.ApplySettings(entry.ev, count, entry.settings);
    }
    pooledDictionary.Recycle();
  }

  public override void Remove(LoopingSoundParameterUpdater.Sound sound)
  {
    for (int index = 0; index < this.entries.Count; ++index)
    {
      if (this.entries[index].ev.handle == sound.ev.handle)
      {
        this.entries.RemoveAt(index);
        break;
      }
    }
  }

  public static void Clear() => UpdateObjectCountParameter.settings.Clear();

  private struct Entry
  {
    public EventInstance ev;
    public UpdateObjectCountParameter.Settings settings;
  }

  public struct Settings
  {
    public HashedString path;
    public PARAMETER_ID parameterId;
    public float minObjects;
    public float maxObjects;
    public bool useExponentialCurve;
  }
}
