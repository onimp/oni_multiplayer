// Decompiled with JetBrains decompiler
// Type: AmbienceManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD;
using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/AmbienceManager")]
public class AmbienceManager : KMonoBehaviour
{
  private float emitterZPosition;
  public AmbienceManager.QuadrantDef[] quadrantDefs;
  public AmbienceManager.Quadrant[] quadrants = new AmbienceManager.Quadrant[4];

  protected virtual void OnSpawn()
  {
    if (!RuntimeManager.IsInitialized)
    {
      ((Behaviour) this).enabled = false;
    }
    else
    {
      for (int index = 0; index < this.quadrants.Length; ++index)
        this.quadrants[index] = new AmbienceManager.Quadrant(this.quadrantDefs[index]);
    }
  }

  protected virtual void OnForcedCleanUp()
  {
    foreach (AmbienceManager.Quadrant quadrant in this.quadrants)
    {
      foreach (AmbienceManager.Layer allLayer in quadrant.GetAllLayers())
        allLayer.Stop();
    }
  }

  private void LateUpdate()
  {
    GridArea visibleArea = GridVisibleArea.GetVisibleArea();
    Vector2I min = visibleArea.Min;
    Vector2I max = visibleArea.Max;
    Vector2I vector2I = Vector2I.op_Addition(min, Vector2I.op_Division(Vector2I.op_Subtraction(max, min), 2));
    Vector3 worldPoint1 = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, TransformExtensions.GetPosition(((Component) Camera.main).transform).z));
    Vector3 worldPoint2 = Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, TransformExtensions.GetPosition(((Component) Camera.main).transform).z));
    Vector3 vector3_1 = Vector3.op_Addition(worldPoint2, Vector3.op_Division(Vector3.op_Subtraction(worldPoint1, worldPoint2), 2f));
    Vector3 vector3_2 = Vector3.op_Subtraction(worldPoint1, worldPoint2);
    if ((double) vector3_2.x > (double) vector3_2.y)
      vector3_2.y = vector3_2.x;
    else
      vector3_2.x = vector3_2.y;
    Vector3.op_Addition(vector3_1, Vector3.op_Division(vector3_2, 2f));
    Vector3 vector3_3 = Vector3.op_Subtraction(vector3_1, Vector3.op_Division(vector3_2, 2f));
    Vector3 vector3_4 = Vector3.op_Division(Vector3.op_Division(vector3_2, 2f), 2f);
    this.quadrants[0].Update(new Vector2I(min.x, min.y), new Vector2I(vector2I.x, vector2I.y), new Vector3(vector3_3.x + vector3_4.x, vector3_3.y + vector3_4.y, this.emitterZPosition));
    this.quadrants[1].Update(new Vector2I(vector2I.x, min.y), new Vector2I(max.x, vector2I.y), new Vector3(vector3_1.x + vector3_4.x, vector3_3.y + vector3_4.y, this.emitterZPosition));
    this.quadrants[2].Update(new Vector2I(min.x, vector2I.y), new Vector2I(vector2I.x, max.y), new Vector3(vector3_3.x + vector3_4.x, vector3_1.y + vector3_4.y, this.emitterZPosition));
    this.quadrants[3].Update(new Vector2I(vector2I.x, vector2I.y), new Vector2I(max.x, max.y), new Vector3(vector3_1.x + vector3_4.x, vector3_1.y + vector3_4.y, this.emitterZPosition));
    float num1 = 0.0f;
    float num2 = 0.0f;
    float num3 = 0.0f;
    for (int index = 0; index < this.quadrants.Length; ++index)
    {
      num1 += (float) this.quadrants[index].spaceLayer.tileCount;
      num2 += (float) this.quadrants[index].facilityLayer.tileCount;
      num3 += (float) this.quadrants[index].totalTileCount;
    }
    AudioMixer.instance.UpdateSpaceVisibleSnapshot(num1 / num3);
    AudioMixer.instance.UpdateFacilityVisibleSnapshot(num2 / num3);
  }

  public class Tuning : TuningData<AmbienceManager.Tuning>
  {
    public int backwallTileValue = 1;
    public int foundationTileValue = 2;
    public int buildingTileValue = 3;
  }

  public class Layer : IComparable<AmbienceManager.Layer>
  {
    private const string TILE_PERCENTAGE_ID = "tilePercentage";
    private const string AVERAGE_TEMPERATURE_ID = "averageTemperature";
    private const string AVERAGE_RADIATION_ID = "averageRadiation";
    public EventReference sound;
    public EventReference oneShotSound;
    public int tileCount;
    public float tilePercentage;
    public float volume;
    public bool isRunning;
    private EventInstance soundEvent;
    public float averageTemperature;
    public float averageRadiation;

    public Layer(EventReference sound, EventReference one_shot_sound = default (EventReference))
    {
      this.sound = sound;
      this.oneShotSound = one_shot_sound;
    }

    public void Reset()
    {
      this.tileCount = 0;
      this.averageTemperature = 0.0f;
      this.averageRadiation = 0.0f;
    }

    public void UpdatePercentage(int cell_count) => this.tilePercentage = (float) this.tileCount / (float) cell_count;

    public void UpdateAverageTemperature()
    {
      this.averageTemperature /= (float) this.tileCount;
      ((EventInstance) ref this.soundEvent).setParameterByName("averageTemperature", this.averageTemperature, false);
    }

    public void UpdateAverageRadiation()
    {
      this.averageRadiation = this.tileCount > 0 ? this.averageRadiation / (float) this.tileCount : 0.0f;
      ((EventInstance) ref this.soundEvent).setParameterByName("averageRadiation", this.averageRadiation, false);
    }

    public void UpdateParameters(Vector3 emitter_position)
    {
      if (!((EventInstance) ref this.soundEvent).isValid())
        return;
      Vector3 vector3;
      // ISSUE: explicit constructor call
      ((Vector3) ref vector3).\u002Ector(emitter_position.x, emitter_position.y, 0.0f);
      ((EventInstance) ref this.soundEvent).set3DAttributes(RuntimeUtils.To3DAttributes(vector3));
      ((EventInstance) ref this.soundEvent).setParameterByName("tilePercentage", this.tilePercentage, false);
    }

    public int CompareTo(AmbienceManager.Layer layer) => layer.tileCount - this.tileCount;

    public void Stop()
    {
      if (((EventInstance) ref this.soundEvent).isValid())
      {
        ((EventInstance) ref this.soundEvent).stop((STOP_MODE) 0);
        ((EventInstance) ref this.soundEvent).release();
      }
      this.isRunning = false;
    }

    public void Start(Vector3 emitter_position)
    {
      if (this.isRunning)
        return;
      if (!((EventReference) ref this.oneShotSound).IsNull)
      {
        EventInstance instance = KFMOD.CreateInstance(this.oneShotSound);
        if (!((EventInstance) ref instance).isValid())
        {
          Debug.LogWarning((object) ("Could not find event: " + this.oneShotSound.ToString()));
        }
        else
        {
          ATTRIBUTES_3D attributes3D = RuntimeUtils.To3DAttributes(new Vector3(emitter_position.x, emitter_position.y, 0.0f));
          ((EventInstance) ref instance).set3DAttributes(attributes3D);
          ((EventInstance) ref instance).setVolume(this.tilePercentage * 2f);
          ((EventInstance) ref instance).start();
          ((EventInstance) ref instance).release();
        }
      }
      else
      {
        this.soundEvent = KFMOD.CreateInstance(this.sound);
        if (((EventInstance) ref this.soundEvent).isValid())
          ((EventInstance) ref this.soundEvent).start();
        this.isRunning = true;
      }
    }
  }

  [Serializable]
  public class QuadrantDef
  {
    public string name;
    public EventReference[] liquidSounds;
    public EventReference[] gasSounds;
    public EventReference[] solidSounds;
    public EventReference fogSound;
    public EventReference spaceSound;
    public EventReference facilitySound;
    public EventReference radiationSound;
  }

  public class Quadrant
  {
    public string name;
    public Vector3 emitterPosition;
    public AmbienceManager.Layer[] gasLayers = new AmbienceManager.Layer[4];
    public AmbienceManager.Layer[] liquidLayers = new AmbienceManager.Layer[4];
    public AmbienceManager.Layer fogLayer;
    public AmbienceManager.Layer spaceLayer;
    public AmbienceManager.Layer facilityLayer;
    public AmbienceManager.Layer radiationLayer;
    public AmbienceManager.Layer[] solidLayers = new AmbienceManager.Layer[16];
    private List<AmbienceManager.Layer> allLayers = new List<AmbienceManager.Layer>();
    private List<AmbienceManager.Layer> loopingLayers = new List<AmbienceManager.Layer>();
    private List<AmbienceManager.Layer> oneShotLayers = new List<AmbienceManager.Layer>();
    private List<AmbienceManager.Layer> topLayers = new List<AmbienceManager.Layer>();
    public static int activeSolidLayerCount = 2;
    public int totalTileCount;
    private bool m_isRadiationEnabled;
    private AmbienceManager.Quadrant.SolidTimer[] solidTimers;

    public Quadrant(AmbienceManager.QuadrantDef def)
    {
      this.name = def.name;
      this.fogLayer = new AmbienceManager.Layer(def.fogSound, new EventReference());
      this.allLayers.Add(this.fogLayer);
      this.loopingLayers.Add(this.fogLayer);
      this.spaceLayer = new AmbienceManager.Layer(def.spaceSound, new EventReference());
      this.allLayers.Add(this.spaceLayer);
      this.loopingLayers.Add(this.spaceLayer);
      this.facilityLayer = new AmbienceManager.Layer(def.facilitySound, new EventReference());
      this.allLayers.Add(this.facilityLayer);
      this.loopingLayers.Add(this.facilityLayer);
      this.m_isRadiationEnabled = Sim.IsRadiationEnabled();
      if (this.m_isRadiationEnabled)
      {
        this.radiationLayer = new AmbienceManager.Layer(def.radiationSound, new EventReference());
        this.allLayers.Add(this.radiationLayer);
      }
      for (int index = 0; index < 4; ++index)
      {
        this.gasLayers[index] = new AmbienceManager.Layer(def.gasSounds[index], new EventReference());
        this.liquidLayers[index] = new AmbienceManager.Layer(def.liquidSounds[index], new EventReference());
        this.allLayers.Add(this.gasLayers[index]);
        this.allLayers.Add(this.liquidLayers[index]);
        this.loopingLayers.Add(this.gasLayers[index]);
        this.loopingLayers.Add(this.liquidLayers[index]);
      }
      for (int index = 0; index < this.solidLayers.Length; ++index)
      {
        if (index >= def.solidSounds.Length)
          Debug.LogError((object) ("Missing solid layer: " + ((SolidAmbienceType) index).ToString()));
        this.solidLayers[index] = new AmbienceManager.Layer(new EventReference(), def.solidSounds[index]);
        this.allLayers.Add(this.solidLayers[index]);
        this.oneShotLayers.Add(this.solidLayers[index]);
      }
      this.solidTimers = new AmbienceManager.Quadrant.SolidTimer[AmbienceManager.Quadrant.activeSolidLayerCount];
      for (int index = 0; index < AmbienceManager.Quadrant.activeSolidLayerCount; ++index)
        this.solidTimers[index] = new AmbienceManager.Quadrant.SolidTimer();
    }

    public void Update(Vector2I min, Vector2I max, Vector3 emitter_position)
    {
      this.emitterPosition = emitter_position;
      this.totalTileCount = 0;
      for (int index = 0; index < this.allLayers.Count; ++index)
        this.allLayers[index].Reset();
      for (int y = min.y; y < max.y; ++y)
      {
        if (y % 2 != 1)
        {
          for (int x = min.x; x < max.x; ++x)
          {
            if (x % 2 != 0)
            {
              int cell = Grid.XYToCell(x, y);
              if (Grid.IsValidCell(cell))
              {
                ++this.totalTileCount;
                if (Grid.IsVisible(cell))
                {
                  if (Grid.GravitasFacility[cell])
                  {
                    this.facilityLayer.tileCount += 8;
                  }
                  else
                  {
                    Element element = Grid.Element[cell];
                    if (element != null)
                    {
                      if (element.IsLiquid && Grid.IsSubstantialLiquid(cell))
                      {
                        AmbienceType ambience = element.substance.GetAmbience();
                        if (ambience != AmbienceType.None)
                        {
                          ++this.liquidLayers[(int) ambience].tileCount;
                          this.liquidLayers[(int) ambience].averageTemperature += Grid.Temperature[cell];
                        }
                      }
                      else if (element.IsGas)
                      {
                        AmbienceType ambience = element.substance.GetAmbience();
                        if (ambience != AmbienceType.None)
                        {
                          ++this.gasLayers[(int) ambience].tileCount;
                          this.gasLayers[(int) ambience].averageTemperature += Grid.Temperature[cell];
                        }
                      }
                      else if (element.IsSolid)
                      {
                        SolidAmbienceType solidAmbience = element.substance.GetSolidAmbience();
                        if (Grid.Foundation[cell])
                        {
                          this.solidLayers[3].tileCount += TuningData<AmbienceManager.Tuning>.Get().foundationTileValue;
                          this.spaceLayer.tileCount -= TuningData<AmbienceManager.Tuning>.Get().foundationTileValue;
                        }
                        else if (Object.op_Inequality((Object) Grid.Objects[cell, 2], (Object) null))
                        {
                          this.solidLayers[3].tileCount += TuningData<AmbienceManager.Tuning>.Get().backwallTileValue;
                          this.spaceLayer.tileCount -= TuningData<AmbienceManager.Tuning>.Get().backwallTileValue;
                        }
                        else if (solidAmbience != SolidAmbienceType.None)
                          ++this.solidLayers[(int) solidAmbience].tileCount;
                        else if (element.id == SimHashes.Regolith || element.id == SimHashes.MaficRock)
                          ++this.spaceLayer.tileCount;
                      }
                      else if (element.id == SimHashes.Vacuum && CellSelectionObject.IsExposedToSpace(cell))
                      {
                        if (Object.op_Inequality((Object) Grid.Objects[cell, 1], (Object) null))
                          this.spaceLayer.tileCount -= TuningData<AmbienceManager.Tuning>.Get().buildingTileValue;
                        ++this.spaceLayer.tileCount;
                      }
                    }
                  }
                  if ((double) Grid.Radiation[cell] > 0.0)
                  {
                    this.radiationLayer.averageRadiation += Grid.Radiation[cell];
                    ++this.radiationLayer.tileCount;
                  }
                }
                else
                  ++this.fogLayer.tileCount;
              }
            }
          }
        }
      }
      Vector2I vector2I = Vector2I.op_Subtraction(max, min);
      int cell_count = vector2I.x * vector2I.y;
      for (int index = 0; index < this.allLayers.Count; ++index)
        this.allLayers[index].UpdatePercentage(cell_count);
      this.loopingLayers.Sort();
      this.topLayers.Clear();
      for (int index = 0; index < this.loopingLayers.Count; ++index)
      {
        AmbienceManager.Layer loopingLayer = this.loopingLayers[index];
        if (index < 3 && (double) loopingLayer.tilePercentage > 0.0)
        {
          loopingLayer.Start(emitter_position);
          loopingLayer.UpdateAverageTemperature();
          loopingLayer.UpdateParameters(emitter_position);
          this.topLayers.Add(loopingLayer);
        }
        else
          loopingLayer.Stop();
      }
      if (this.m_isRadiationEnabled)
      {
        this.radiationLayer.Start(emitter_position);
        this.radiationLayer.UpdateAverageRadiation();
        this.radiationLayer.UpdateParameters(emitter_position);
      }
      this.oneShotLayers.Sort();
      for (int index = 0; index < AmbienceManager.Quadrant.activeSolidLayerCount; ++index)
      {
        if (this.solidTimers[index].ShouldPlay() && (double) this.oneShotLayers[index].tilePercentage > 0.0)
          this.oneShotLayers[index].Start(emitter_position);
      }
    }

    public List<AmbienceManager.Layer> GetAllLayers() => this.allLayers;

    public class SolidTimer
    {
      public static float solidMinTime = 9f;
      public static float solidMaxTime = 15f;
      public float solidTargetTime;

      public SolidTimer() => this.solidTargetTime = Time.unscaledTime + Random.value * AmbienceManager.Quadrant.SolidTimer.solidMinTime;

      public bool ShouldPlay()
      {
        if ((double) Time.unscaledTime <= (double) this.solidTargetTime)
          return false;
        this.solidTargetTime = (float) ((double) Time.unscaledTime + (double) AmbienceManager.Quadrant.SolidTimer.solidMinTime + (double) Random.value * ((double) AmbienceManager.Quadrant.SolidTimer.solidMaxTime - (double) AmbienceManager.Quadrant.SolidTimer.solidMinTime));
        return true;
      }
    }
  }
}
