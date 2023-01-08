// Decompiled with JetBrains decompiler
// Type: PrimaryElement
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using Klei.AI;
using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/PrimaryElement")]
public class PrimaryElement : KMonoBehaviour, ISaveLoadable
{
  public static float MAX_MASS = 100000f;
  public PrimaryElement.GetTemperatureCallback getTemperatureCallback = new PrimaryElement.GetTemperatureCallback(PrimaryElement.OnGetTemperature);
  public PrimaryElement.SetTemperatureCallback setTemperatureCallback = new PrimaryElement.SetTemperatureCallback(PrimaryElement.OnSetTemperature);
  private PrimaryElement diseaseRedirectTarget;
  private bool useSimDiseaseInfo;
  public const float DefaultChunkMass = 400f;
  private static readonly Tag[] metalTags = new Tag[2]
  {
    GameTags.Metal,
    GameTags.RefinedMetal
  };
  [Serialize]
  [HashedEnum]
  public SimHashes ElementID;
  private float _units = 1f;
  [Serialize]
  [SerializeField]
  private float _Temperature;
  [Serialize]
  [NonSerialized]
  public bool KeepZeroMassObject;
  [Serialize]
  private HashedString diseaseID;
  [Serialize]
  private int diseaseCount;
  private HandleVector<int>.Handle diseaseHandle = HandleVector<int>.InvalidHandle;
  public float MassPerUnit = 1f;
  [NonSerialized]
  private Element _Element;
  [NonSerialized]
  public Action<PrimaryElement> onDataChanged;
  [NonSerialized]
  private bool forcePermanentDiseaseContainer;
  private static readonly EventSystem.IntraObjectHandler<PrimaryElement> OnSplitFromChunkDelegate = new EventSystem.IntraObjectHandler<PrimaryElement>((Action<PrimaryElement, object>) ((component, data) => component.OnSplitFromChunk(data)));
  private static readonly EventSystem.IntraObjectHandler<PrimaryElement> OnAbsorbDelegate = new EventSystem.IntraObjectHandler<PrimaryElement>((Action<PrimaryElement, object>) ((component, data) => component.OnAbsorb(data)));

  public void SetUseSimDiseaseInfo(bool use) => this.useSimDiseaseInfo = use;

  [Serialize]
  public float Units
  {
    get => this._units;
    set
    {
      if (float.IsInfinity(value) || float.IsNaN(value))
      {
        DebugUtil.DevLogError("Invalid units value for element, setting Units to 0");
        this._units = 0.0f;
      }
      else
        this._units = value;
      if (this.onDataChanged == null)
        return;
      this.onDataChanged(this);
    }
  }

  public float Temperature
  {
    get => this.getTemperatureCallback(this);
    set => this.SetTemperature(value);
  }

  public float InternalTemperature
  {
    get => this._Temperature;
    set => this._Temperature = value;
  }

  [System.Runtime.Serialization.OnSerializing]
  private void OnSerializing()
  {
    this._Temperature = this.Temperature;
    this.SanitizeMassAndTemperature();
    ((HashedString) ref this.diseaseID).HashValue = 0;
    this.diseaseCount = 0;
    if (this.useSimDiseaseInfo)
    {
      int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
      if (Grid.DiseaseIdx[cell] == byte.MaxValue)
        return;
      this.diseaseID = Db.Get().Diseases[(int) Grid.DiseaseIdx[cell]].id;
      this.diseaseCount = Grid.DiseaseCount[cell];
    }
    else
    {
      if (!this.diseaseHandle.IsValid())
        return;
      DiseaseHeader header = ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) GameComps.DiseaseContainers).GetHeader(this.diseaseHandle);
      if (header.diseaseIdx == byte.MaxValue)
        return;
      this.diseaseID = Db.Get().Diseases[(int) header.diseaseIdx].id;
      this.diseaseCount = header.diseaseCount;
    }
  }

  [System.Runtime.Serialization.OnDeserialized]
  private void OnDeserialized()
  {
    if (this.ElementID == (SimHashes) 351109216)
      this.ElementID = SimHashes.Creature;
    this.SanitizeMassAndTemperature();
    float temperature = this._Temperature;
    if (float.IsNaN(temperature) || float.IsInfinity(temperature) || (double) temperature < 0.0 || 10000.0 < (double) temperature)
    {
      DeserializeWarnings.Instance.PrimaryElementTemperatureIsNan.Warn(string.Format("{0} has invalid temperature of {1}. Resetting temperature.", (object) ((Object) this).name, (object) this.Temperature));
      temperature = this.Element.defaultValues.temperature;
    }
    this._Temperature = temperature;
    this.Temperature = temperature;
    if (this.Element == null)
      DeserializeWarnings.Instance.PrimaryElementHasNoElement.Warn(((Object) this).name + "Primary element has no element.");
    if ((double) this.Mass < 0.0)
    {
      DebugUtil.DevLogError((Object) ((Component) this).gameObject, "deserialized ore with less than 0 mass. Error! Destroying");
      Util.KDestroyGameObject(((Component) this).gameObject);
    }
    else if ((double) this.Mass == 0.0 && !this.KeepZeroMassObject)
    {
      DebugUtil.DevLogError((Object) ((Component) this).gameObject, "deserialized element with 0 mass. Destroying");
      Util.KDestroyGameObject(((Component) this).gameObject);
    }
    else
    {
      if (this.onDataChanged != null)
        this.onDataChanged(this);
      byte index = Db.Get().Diseases.GetIndex(this.diseaseID);
      if (index == byte.MaxValue || this.diseaseCount <= 0)
      {
        if (!this.diseaseHandle.IsValid())
          return;
        GameComps.DiseaseContainers.Remove(((Component) this).gameObject);
        this.diseaseHandle.Clear();
      }
      else if (this.diseaseHandle.IsValid())
      {
        DiseaseHeader header = ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) GameComps.DiseaseContainers).GetHeader(this.diseaseHandle) with
        {
          diseaseIdx = index,
          diseaseCount = this.diseaseCount
        };
        ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) GameComps.DiseaseContainers).SetHeader(this.diseaseHandle, header);
      }
      else
        this.diseaseHandle = GameComps.DiseaseContainers.Add(((Component) this).gameObject, index, this.diseaseCount);
    }
  }

  protected virtual void OnLoadLevel() => base.OnLoadLevel();

  private void SanitizeMassAndTemperature()
  {
    if ((double) this._Temperature <= 0.0)
    {
      DebugUtil.DevLogError(((Object) ((Component) this).gameObject).name + " is attempting to serialize a temperature of <= 0K. Resetting to default. world=" + ((Component) this).gameObject.DebugGetMyWorldName());
      this._Temperature = this.Element.defaultValues.temperature;
    }
    if ((double) this.Mass <= (double) PrimaryElement.MAX_MASS)
      return;
    DebugUtil.DevLogError(string.Format("{0} is attempting to serialize very large mass {1}. Resetting to default. world={2}", (object) ((Object) ((Component) this).gameObject).name, (object) this.Mass, (object) ((Component) this).gameObject.DebugGetMyWorldName()));
    this.Mass = this.Element.defaultValues.mass;
  }

  public float Mass
  {
    get => this.Units * this.MassPerUnit;
    set
    {
      this.SetMass(value);
      if (this.onDataChanged == null)
        return;
      this.onDataChanged(this);
    }
  }

  private void SetMass(float mass)
  {
    if (((double) mass > (double) PrimaryElement.MAX_MASS || (double) mass < 0.0) && this.ElementID != SimHashes.Regolith)
      DebugUtil.DevLogErrorFormat((Object) ((Component) this).gameObject, "{0} is getting an abnormal mass set {1}.", new object[2]
      {
        (object) ((Object) ((Component) this).gameObject).name,
        (object) mass
      });
    mass = Mathf.Clamp(mass, 0.0f, PrimaryElement.MAX_MASS);
    this.Units = mass / this.MassPerUnit;
    if ((double) this.Units > 0.0 || this.KeepZeroMassObject)
      return;
    Util.KDestroyGameObject(((Component) this).gameObject);
  }

  private void SetTemperature(float temperature)
  {
    if (float.IsNaN(temperature) || float.IsInfinity(temperature))
    {
      DebugUtil.LogErrorArgs((Object) ((Component) this).gameObject, new object[1]
      {
        (object) ("Invalid temperature [" + temperature.ToString() + "]")
      });
    }
    else
    {
      if ((double) temperature <= 0.0)
        KCrashReporter.Assert(false, "Tried to set PrimaryElement.Temperature to a value <= 0");
      this.setTemperatureCallback(this, temperature);
    }
  }

  public void SetMassTemperature(float mass, float temperature)
  {
    this.SetMass(mass);
    this.SetTemperature(temperature);
  }

  public Element Element
  {
    get
    {
      if (this._Element == null)
        this._Element = ElementLoader.FindElementByHash(this.ElementID);
      return this._Element;
    }
  }

  public byte DiseaseIdx
  {
    get
    {
      if (Object.op_Implicit((Object) this.diseaseRedirectTarget))
        return this.diseaseRedirectTarget.DiseaseIdx;
      byte diseaseIdx = byte.MaxValue;
      if (this.useSimDiseaseInfo)
      {
        int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
        diseaseIdx = Grid.DiseaseIdx[cell];
      }
      else if (this.diseaseHandle.IsValid())
        diseaseIdx = ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) GameComps.DiseaseContainers).GetHeader(this.diseaseHandle).diseaseIdx;
      return diseaseIdx;
    }
  }

  public int DiseaseCount
  {
    get
    {
      if (Object.op_Implicit((Object) this.diseaseRedirectTarget))
        return this.diseaseRedirectTarget.DiseaseCount;
      int diseaseCount = 0;
      if (this.useSimDiseaseInfo)
      {
        int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
        diseaseCount = Grid.DiseaseCount[cell];
      }
      else if (this.diseaseHandle.IsValid())
        diseaseCount = ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) GameComps.DiseaseContainers).GetHeader(this.diseaseHandle).diseaseCount;
      return diseaseCount;
    }
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    GameComps.InfraredVisualizers.Add(((Component) this).gameObject);
    this.Subscribe<PrimaryElement>(1335436905, PrimaryElement.OnSplitFromChunkDelegate);
    this.Subscribe<PrimaryElement>(-2064133523, PrimaryElement.OnAbsorbDelegate);
  }

  protected virtual void OnSpawn()
  {
    Attributes attributes = this.GetAttributes();
    if (attributes == null)
      return;
    foreach (AttributeModifier attributeModifier in this.Element.attributeModifiers)
      attributes.Add(attributeModifier);
  }

  public void ForcePermanentDiseaseContainer(bool force_on)
  {
    if (force_on)
    {
      if (!this.diseaseHandle.IsValid())
        this.diseaseHandle = GameComps.DiseaseContainers.Add(((Component) this).gameObject, byte.MaxValue, 0);
    }
    else if (this.diseaseHandle.IsValid() && this.DiseaseIdx == byte.MaxValue)
    {
      GameComps.DiseaseContainers.Remove(((Component) this).gameObject);
      this.diseaseHandle.Clear();
    }
    this.forcePermanentDiseaseContainer = force_on;
  }

  protected virtual void OnCleanUp()
  {
    GameComps.InfraredVisualizers.Remove(((Component) this).gameObject);
    if (this.diseaseHandle.IsValid())
    {
      GameComps.DiseaseContainers.Remove(((Component) this).gameObject);
      this.diseaseHandle.Clear();
    }
    base.OnCleanUp();
  }

  public void SetElement(SimHashes element_id, bool addTags = true)
  {
    this.ElementID = element_id;
    if (!addTags)
      return;
    this.UpdateTags();
  }

  public void UpdateTags()
  {
    if (this.ElementID == (SimHashes) 0)
    {
      Debug.Log((object) "UpdateTags() Primary element 0", (Object) ((Component) this).gameObject);
    }
    else
    {
      KPrefabID component = ((Component) this).GetComponent<KPrefabID>();
      if (!Object.op_Inequality((Object) component, (Object) null))
        return;
      List<Tag> tagList = new List<Tag>();
      foreach (Tag oreTag in this.Element.oreTags)
        tagList.Add(oreTag);
      if (component.HasAnyTags(PrimaryElement.metalTags))
        tagList.Add(GameTags.StoredMetal);
      foreach (Tag tag in tagList)
        component.AddTag(tag, false);
    }
  }

  public void ModifyDiseaseCount(int delta, string reason)
  {
    if (Object.op_Implicit((Object) this.diseaseRedirectTarget))
      this.diseaseRedirectTarget.ModifyDiseaseCount(delta, reason);
    else if (this.useSimDiseaseInfo)
    {
      SimMessages.ModifyDiseaseOnCell(Grid.PosToCell((KMonoBehaviour) this), byte.MaxValue, delta);
    }
    else
    {
      if (delta == 0 || !this.diseaseHandle.IsValid() || GameComps.DiseaseContainers.ModifyDiseaseCount(this.diseaseHandle, delta) > 0 || this.forcePermanentDiseaseContainer)
        return;
      this.Trigger(-1689370368, (object) false);
      GameComps.DiseaseContainers.Remove(((Component) this).gameObject);
      this.diseaseHandle.Clear();
    }
  }

  public void AddDisease(byte disease_idx, int delta, string reason)
  {
    if (delta == 0)
      return;
    if (Object.op_Implicit((Object) this.diseaseRedirectTarget))
      this.diseaseRedirectTarget.AddDisease(disease_idx, delta, reason);
    else if (this.useSimDiseaseInfo)
      SimMessages.ModifyDiseaseOnCell(Grid.PosToCell((KMonoBehaviour) this), disease_idx, delta);
    else if (this.diseaseHandle.IsValid())
    {
      if (GameComps.DiseaseContainers.AddDisease(this.diseaseHandle, disease_idx, delta) > 0)
        return;
      GameComps.DiseaseContainers.Remove(((Component) this).gameObject);
      this.diseaseHandle.Clear();
    }
    else
    {
      if (delta <= 0)
        return;
      this.diseaseHandle = GameComps.DiseaseContainers.Add(((Component) this).gameObject, disease_idx, delta);
      this.Trigger(-1689370368, (object) true);
      this.Trigger(-283306403, (object) null);
    }
  }

  private static float OnGetTemperature(PrimaryElement primary_element) => primary_element._Temperature;

  private static void OnSetTemperature(PrimaryElement primary_element, float temperature)
  {
    Debug.Assert(!float.IsNaN(temperature));
    if ((double) temperature <= 0.0)
      DebugUtil.LogErrorArgs((Object) ((Component) primary_element).gameObject, new object[1]
      {
        (object) (((Object) ((Component) primary_element).gameObject).name + " has a temperature of zero which has always been an error in my experience.")
      });
    primary_element._Temperature = temperature;
  }

  private void OnSplitFromChunk(object data)
  {
    Pickupable pickupable = (Pickupable) data;
    if (Object.op_Equality((Object) pickupable, (Object) null))
      return;
    float percent = this.Units / (this.Units + pickupable.PrimaryElement.Units);
    SimUtil.DiseaseInfo percentOfDisease = SimUtil.GetPercentOfDisease(pickupable.PrimaryElement, percent);
    this.AddDisease(percentOfDisease.idx, percentOfDisease.count, "PrimaryElement.SplitFromChunk");
    pickupable.PrimaryElement.ModifyDiseaseCount(-percentOfDisease.count, "PrimaryElement.SplitFromChunk");
  }

  private void OnAbsorb(object data)
  {
    Pickupable pickupable = (Pickupable) data;
    if (Object.op_Equality((Object) pickupable, (Object) null))
      return;
    this.AddDisease(pickupable.PrimaryElement.DiseaseIdx, pickupable.PrimaryElement.DiseaseCount, "PrimaryElement.OnAbsorb");
  }

  private void SetDiseaseVisualProvider(GameObject visualizer)
  {
    HandleVector<int>.Handle handle = GameComps.DiseaseContainers.GetHandle(((Component) this).gameObject);
    if (!HandleVector<int>.Handle.op_Inequality(handle, HandleVector<int>.InvalidHandle))
      return;
    DiseaseContainer payload = ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) GameComps.DiseaseContainers).GetPayload(handle) with
    {
      visualDiseaseProvider = visualizer
    };
    ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) GameComps.DiseaseContainers).SetPayload(handle, ref payload);
  }

  public void RedirectDisease(GameObject target)
  {
    this.SetDiseaseVisualProvider(target);
    this.diseaseRedirectTarget = Object.op_Implicit((Object) target) ? target.GetComponent<PrimaryElement>() : (PrimaryElement) null;
    Debug.Assert(Object.op_Inequality((Object) this.diseaseRedirectTarget, (Object) this), (object) "Disease redirect target set to myself");
  }

  public delegate float GetTemperatureCallback(PrimaryElement primary_element);

  public delegate void SetTemperatureCallback(PrimaryElement primary_element, float temperature);
}
