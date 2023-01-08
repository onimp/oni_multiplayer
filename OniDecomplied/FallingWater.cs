// Decompiled with JetBrains decompiler
// Type: FallingWater
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using FMOD.Studio;
using FMODUnity;
using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/FallingWater")]
public class FallingWater : KMonoBehaviour, ISim200ms
{
  private const float STATE_TRANSITION_TEMPERATURE_BUFER = 3f;
  private const byte FORCED_ALPHA = 191;
  private int simUpdateDelay = 2;
  [SerializeField]
  private Vector2 particleSize;
  [SerializeField]
  private Vector2 initialOffset;
  [SerializeField]
  private float jitterStep;
  [SerializeField]
  private Vector3 renderOffset;
  [SerializeField]
  private float minSpawnDelay;
  [SerializeField]
  private float gravityScale = 0.05f;
  [SerializeField]
  private float particleMassToSplit = 75f;
  [SerializeField]
  private float particleMassVariation = 15f;
  [SerializeField]
  private Vector2 multipleOffsetRange;
  [SerializeField]
  private GameObject mistEffect;
  [SerializeField]
  private float mistEffectMinAliveTime = 2f;
  [SerializeField]
  private Material material;
  [SerializeField]
  private Texture2D texture;
  [SerializeField]
  private int numFrames;
  [SerializeField]
  private FallingWater.DecorInfo liquid_splash;
  [SerializeField]
  private EventReference liquid_top_loop;
  [SerializeField]
  private EventReference liquid_splash_initial;
  [SerializeField]
  private EventReference liquid_splash_loop;
  [SerializeField]
  private float stopTopLoopDelay = 0.2f;
  [SerializeField]
  private float stopSplashLoopDelay = 1f;
  [SerializeField]
  private int splashCountLoopThreshold = 10;
  [Serialize]
  private List<FallingWater.ParticlePhysics> physics = new List<FallingWater.ParticlePhysics>();
  private List<FallingWater.ParticleProperties> particleProperties = new List<FallingWater.ParticleProperties>();
  [Serialize]
  private List<FallingWater.SerializedParticleProperties> serializedParticleProperties;
  [Serialize]
  private List<FallingWater.ParticleProperties> properties = new List<FallingWater.ParticleProperties>();
  private Dictionary<int, FallingWater.SoundInfo> topSounds = new Dictionary<int, FallingWater.SoundInfo>();
  private Dictionary<int, FallingWater.SoundInfo> splashSounds = new Dictionary<int, FallingWater.SoundInfo>();
  private GameObjectPool mistPool;
  private Mesh mesh;
  private float offset;
  private float[] lastSpawnTime;
  private Dictionary<Pair<int, bool>, FallingWater.MistInfo> mistAlive = new Dictionary<Pair<int, bool>, FallingWater.MistInfo>();
  private Vector2 uvFrameSize;
  private MaterialPropertyBlock propertyBlock;
  private static FallingWater _instance;
  private List<int> clearList = new List<int>();
  private List<Pair<int, bool>> mistClearList = new List<Pair<int, bool>>();
  private static HashedString HASH_LIQUIDDEPTH = HashedString.op_Implicit("liquidDepth");
  private static HashedString HASH_LIQUIDVOLUME = HashedString.op_Implicit("liquidVolume");

  public static FallingWater instance
  {
    get => FallingWater._instance;
    private set
    {
    }
  }

  public static void DestroyInstance() => FallingWater._instance = (FallingWater) null;

  protected virtual void OnPrefabInit()
  {
    FallingWater._instance = this;
    base.OnPrefabInit();
    this.mistEffect.SetActive(false);
    this.mistPool = new GameObjectPool(new Func<GameObject>(this.InstantiateMist), 16);
  }

  protected virtual void OnSpawn()
  {
    this.mesh = new Mesh();
    this.mesh.MarkDynamic();
    ((Object) this.mesh).name = nameof (FallingWater);
    this.lastSpawnTime = new float[Grid.WidthInCells * Grid.HeightInCells];
    for (int index = 0; index < this.lastSpawnTime.Length; ++index)
      this.lastSpawnTime[index] = 0.0f;
    this.propertyBlock = new MaterialPropertyBlock();
    this.propertyBlock.SetTexture("_MainTex", (Texture) this.texture);
    this.uvFrameSize = new Vector2(1f / (float) this.numFrames, 1f);
  }

  protected virtual void OnCleanUp()
  {
    FallingWater.instance = (FallingWater) null;
    base.OnCleanUp();
  }

  private float GetTime() => Time.time % 360f;

  public void AddParticle(
    int cell,
    ushort elementIdx,
    float base_mass,
    float temperature,
    byte disease_idx,
    int base_disease_count,
    bool skip_sound = false,
    bool skip_decor = false,
    bool debug_track = false,
    bool disable_randomness = false)
  {
    this.AddParticle(Vector2.op_Implicit(Grid.CellToPos2D(cell)), elementIdx, base_mass, temperature, disease_idx, base_disease_count, skip_sound, skip_decor, debug_track, disable_randomness);
  }

  public void AddParticle(
    Vector2 root_pos,
    ushort elementIdx,
    float base_mass,
    float temperature,
    byte disease_idx,
    int base_disease_count,
    bool skip_sound = false,
    bool skip_decor = false,
    bool debug_track = false,
    bool disable_randomness = false)
  {
    int cell1 = Grid.PosToCell(root_pos);
    if (!Grid.IsValidCell(cell1))
    {
      KCrashReporter.Assert(false, "Trying to add falling water outside of the scene");
    }
    else
    {
      if ((double) temperature <= 0.0 || (double) base_mass <= 0.0)
        Debug.LogError((object) string.Format("Unexpected water mass/temperature values added to the falling water manager T({0}) M({1})", (object) temperature, (object) base_mass));
      float time = this.GetTime();
      if (!skip_sound)
      {
        FallingWater.SoundInfo soundInfo;
        if (!this.topSounds.TryGetValue(cell1, out soundInfo))
        {
          soundInfo = new FallingWater.SoundInfo();
          soundInfo.handle = LoopingSoundManager.StartSound(this.liquid_top_loop, Vector2.op_Implicit(root_pos));
        }
        soundInfo.startTime = time;
        LoopingSoundManager.Get().UpdateSecondParameter(soundInfo.handle, FallingWater.HASH_LIQUIDVOLUME, SoundUtil.GetLiquidVolume(base_mass));
        this.topSounds[cell1] = soundInfo;
      }
      int num1 = base_disease_count;
      while ((double) base_mass > 0.0)
      {
        float num2 = Random.value * 2f * this.particleMassVariation - this.particleMassVariation;
        float mass = Mathf.Max(0.0f, Mathf.Min(base_mass, this.particleMassToSplit + num2));
        double num3 = (double) mass / (double) base_mass;
        base_mass -= mass;
        double num4 = (double) base_disease_count;
        int num5 = (int) (num3 * num4);
        int disease_count = Mathf.Min(num1, num5);
        num1 = Mathf.Max(0, num1 - disease_count);
        int frame = Random.Range(0, this.numFrames);
        Vector2 vector2_1 = disable_randomness ? Vector2.zero : new Vector2(this.jitterStep * Mathf.Sin(this.offset), this.jitterStep * Mathf.Sin(this.offset + 17f));
        Vector2 vector2_2 = disable_randomness ? Vector2.zero : new Vector2(Random.Range(-this.multipleOffsetRange.x, this.multipleOffsetRange.x), Random.Range(-this.multipleOffsetRange.y, this.multipleOffsetRange.y));
        Element element = ElementLoader.elements[(int) elementIdx];
        Vector2 vector2_3 = root_pos;
        bool flag1 = !skip_decor && this.SpawnLiquidTopDecor(time, Grid.CellLeft(cell1), false, element);
        bool flag2 = !skip_decor && this.SpawnLiquidTopDecor(time, Grid.CellRight(cell1), true, element);
        Vector2 vector2_4 = Vector2.ClampMagnitude(Vector2.op_Addition(Vector2.op_Addition(this.initialOffset, vector2_1), vector2_2), 1f);
        if (flag1 | flag2)
        {
          if (flag1 & flag2)
          {
            vector2_3 = Vector2.op_Addition(vector2_3, vector2_4);
            vector2_3.x += 0.5f;
          }
          else if (flag1)
          {
            vector2_3 = Vector2.op_Addition(vector2_3, vector2_4);
          }
          else
          {
            vector2_3.x += 1f - vector2_4.x;
            vector2_3.y += vector2_4.y;
          }
        }
        else
        {
          vector2_3 = Vector2.op_Addition(vector2_3, vector2_4);
          vector2_3.x += 0.5f;
        }
        int cell2 = Grid.PosToCell(vector2_3);
        if ((Grid.Element[cell2].state & Element.State.Solid) == Element.State.Solid || ((int) Grid.Properties[cell2] & 2) != 0)
          vector2_3.y = Mathf.Floor(vector2_3.y + 1f);
        this.physics.Add(new FallingWater.ParticlePhysics(vector2_3, Vector2.zero, frame, elementIdx, (int) Grid.WorldIdx[cell1]));
        this.particleProperties.Add(new FallingWater.ParticleProperties(elementIdx, mass, temperature, disease_idx, disease_count, debug_track));
      }
    }
  }

  private bool SpawnLiquidTopDecor(float time, int cell, bool flip, Element element)
  {
    if (Grid.IsValidCell(cell) && Grid.Element[cell] == element)
    {
      Vector3 posCbc = Grid.CellToPosCBC(cell, Grid.SceneLayer.TileMain);
      if (CameraController.Instance.IsVisiblePos(posCbc))
      {
        Pair<int, bool> key = new Pair<int, bool>(cell, flip);
        FallingWater.MistInfo mistInfo;
        if (!this.mistAlive.TryGetValue(key, out mistInfo))
        {
          mistInfo = new FallingWater.MistInfo();
          mistInfo.fx = this.SpawnMist();
          mistInfo.fx.TintColour = element.substance.colour;
          Vector3 vector3 = Vector3.op_Addition(posCbc, Vector3.op_Multiply(flip ? Vector3.op_UnaryNegation(Vector3.right) : Vector3.right, 0.5f));
          TransformExtensions.SetPosition(((Component) mistInfo.fx).transform, vector3);
          mistInfo.fx.FlipX = flip;
        }
        mistInfo.deathTime = Time.time + this.mistEffectMinAliveTime;
        this.mistAlive[key] = mistInfo;
        return true;
      }
    }
    return false;
  }

  public void SpawnLiquidSplash(float x, int cell, bool forceSplash = false)
  {
    float time = this.GetTime();
    float num = this.lastSpawnTime[cell];
    if ((double) time - (double) num < (double) this.minSpawnDelay && !forceSplash)
      return;
    this.lastSpawnTime[cell] = time;
    Vector2 vector2_1 = Vector2.op_Implicit(Grid.CellToPos2D(cell));
    vector2_1.x = x - 0.5f;
    int index = Random.Range(0, this.liquid_splash.names.Length);
    Vector2 vector2_2 = Vector2.op_Addition(vector2_1, new Vector2(this.liquid_splash.offset.x, this.liquid_splash.offset.y));
    SpriteSheetAnimManager.instance.Play(this.liquid_splash.names[index], new Vector3(vector2_2.x, vector2_2.y, this.renderOffset.z), new Vector2(this.liquid_splash.size.x, this.liquid_splash.size.y), Color32.op_Implicit(Color.white));
  }

  public void UpdateParticles(float dt)
  {
    if ((double) dt <= 0.0 || this.simUpdateDelay >= 0)
      return;
    this.offset = (float) (((double) this.offset + (double) dt) % 360.0);
    int count = this.physics.Count;
    Vector2 vector2 = Vector2.op_Implicit(Vector3.op_Multiply(Vector3.op_Multiply(Physics.gravity, dt), this.gravityScale));
    for (int index1 = 0; index1 < count; ++index1)
    {
      FallingWater.ParticlePhysics physic = this.physics[index1];
      Vector3 pos = Vector2.op_Implicit(physic.position);
      int x;
      int y1;
      Grid.PosToXY(pos, out x, out y1);
      ref Vector2 local = ref physic.velocity;
      local = Vector2.op_Addition(local, vector2);
      Vector3 vector3 = Vector3.op_Addition(pos, Vector2.op_Implicit(Vector2.op_Multiply(physic.velocity, dt)));
      physic.position = Vector2.op_Implicit(vector3);
      this.physics[index1] = physic;
      int y2;
      Grid.PosToXY(Vector2.op_Implicit(physic.position), out int _, out y2);
      int num1 = y1 > y2 ? y1 : y2;
      int num2 = y1 > y2 ? y2 : y1;
      for (int index2 = num1; index2 >= num2; --index2)
      {
        int index3 = index2 * Grid.WidthInCells + x;
        int cell = (index2 + 1) * Grid.WidthInCells + x;
        if (Grid.IsValidCell(index3) && (int) Grid.WorldIdx[index3] != physic.worldIdx)
        {
          this.RemoveParticle(index1, ref count);
          break;
        }
        if (Grid.IsValidCell(index3))
        {
          Element element1 = Grid.Element[index3];
          Element.State state = element1.state & Element.State.Solid;
          bool flag = false;
          if (state == Element.State.Solid || ((int) Grid.Properties[index3] & 2) != 0)
          {
            this.AddToSim(cell, index1, ref count);
          }
          else
          {
            switch (state)
            {
              case Element.State.Vacuum:
                if (element1.id == SimHashes.Vacuum)
                {
                  flag = true;
                  break;
                }
                this.RemoveParticle(index1, ref count);
                break;
              case Element.State.Gas:
                flag = true;
                break;
              case Element.State.Liquid:
                FallingWater.ParticleProperties particleProperty = this.particleProperties[index1];
                Element element2 = ElementLoader.elements[(int) particleProperty.elementIdx];
                if (element2.id == element1.id)
                {
                  if ((double) Grid.Mass[index3] <= (double) element1.defaultValues.mass)
                  {
                    flag = true;
                    break;
                  }
                  this.SpawnLiquidSplash(physic.position.x, cell);
                  this.AddToSim(index3, index1, ref count);
                  break;
                }
                if ((double) element2.molarMass > (double) element1.molarMass)
                {
                  flag = true;
                  break;
                }
                this.SpawnLiquidSplash(physic.position.x, cell);
                this.AddToSim(cell, index1, ref count);
                break;
            }
          }
          if (!flag)
            break;
        }
        else
        {
          if (Grid.IsValidCell(cell))
          {
            this.SpawnLiquidSplash(physic.position.x, cell);
            this.AddToSim(cell, index1, ref count);
            break;
          }
          this.RemoveParticle(index1, ref count);
          break;
        }
      }
    }
    this.UpdateSounds(this.GetTime());
    this.UpdateMistFX(Time.time);
  }

  private void UpdateMistFX(float t)
  {
    this.mistClearList.Clear();
    foreach (KeyValuePair<Pair<int, bool>, FallingWater.MistInfo> keyValuePair in this.mistAlive)
    {
      if ((double) t > (double) keyValuePair.Value.deathTime)
      {
        keyValuePair.Value.fx.Play(HashedString.op_Implicit("end"));
        this.mistClearList.Add(keyValuePair.Key);
      }
    }
    foreach (Pair<int, bool> mistClear in this.mistClearList)
      this.mistAlive.Remove(mistClear);
    this.mistClearList.Clear();
  }

  private void UpdateSounds(float t)
  {
    this.clearList.Clear();
    foreach (KeyValuePair<int, FallingWater.SoundInfo> topSound in this.topSounds)
    {
      FallingWater.SoundInfo soundInfo = topSound.Value;
      if ((double) t - (double) soundInfo.startTime >= (double) this.stopTopLoopDelay)
      {
        if (HandleVector<int>.Handle.op_Inequality(soundInfo.handle, HandleVector<int>.InvalidHandle))
          LoopingSoundManager.StopSound(soundInfo.handle);
        this.clearList.Add(topSound.Key);
      }
    }
    foreach (int clear in this.clearList)
      this.topSounds.Remove(clear);
    this.clearList.Clear();
    foreach (KeyValuePair<int, FallingWater.SoundInfo> splashSound in this.splashSounds)
    {
      FallingWater.SoundInfo soundInfo = splashSound.Value;
      if ((double) t - (double) soundInfo.startTime >= (double) this.stopSplashLoopDelay)
      {
        if (HandleVector<int>.Handle.op_Inequality(soundInfo.handle, HandleVector<int>.InvalidHandle))
          LoopingSoundManager.StopSound(soundInfo.handle);
        this.clearList.Add(splashSound.Key);
      }
    }
    foreach (int clear in this.clearList)
      this.splashSounds.Remove(clear);
    this.clearList.Clear();
  }

  public Dictionary<int, float> GetInfo(int cell)
  {
    Dictionary<int, float> info = new Dictionary<int, float>();
    int count = this.physics.Count;
    for (int index = 0; index < count; ++index)
    {
      if (Grid.PosToCell(this.physics[index].position) == cell)
      {
        FallingWater.ParticleProperties particleProperty = this.particleProperties[index];
        float num1 = 0.0f;
        info.TryGetValue((int) particleProperty.elementIdx, out num1);
        float num2 = num1 + particleProperty.mass;
        info[(int) particleProperty.elementIdx] = num2;
      }
    }
    return info;
  }

  private float GetParticleVolume(float mass) => Mathf.Clamp01((float) (((double) mass - ((double) this.particleMassToSplit - (double) this.particleMassVariation)) / (2.0 * (double) this.particleMassVariation)));

  private void AddToSim(int cell, int particleIdx, ref int num_particles)
  {
    bool flag1 = false;
    do
    {
      if ((Grid.Element[cell].state & Element.State.Solid) == Element.State.Solid || ((int) Grid.Properties[cell] & 2) != 0)
      {
        cell += Grid.WidthInCells;
        if (!Grid.IsValidCell(cell))
          return;
      }
      else
        flag1 = true;
    }
    while (!flag1);
    FallingWater.ParticleProperties particleProperty = this.particleProperties[particleIdx];
    SimMessages.AddRemoveSubstance(cell, particleProperty.elementIdx, CellEventLogger.Instance.FallingWaterAddToSim, particleProperty.mass, particleProperty.temperature, particleProperty.diseaseIdx, particleProperty.diseaseCount);
    this.RemoveParticle(particleIdx, ref num_particles);
    float time = this.GetTime();
    float num = this.lastSpawnTime[cell];
    if ((double) time - (double) num < (double) this.minSpawnDelay)
      return;
    this.lastSpawnTime[cell] = time;
    Vector3 posCcc = Grid.CellToPosCCC(cell, Grid.SceneLayer.TileMain);
    posCcc.z = 0.0f;
    if (!CameraController.Instance.IsAudibleSound(Vector2.op_Implicit(posCcc)))
      return;
    bool flag2 = true;
    FallingWater.SoundInfo soundInfo;
    if (this.splashSounds.TryGetValue(cell, out soundInfo))
    {
      ++soundInfo.splashCount;
      if (soundInfo.splashCount > this.splashCountLoopThreshold)
      {
        if (HandleVector<int>.Handle.op_Equality(soundInfo.handle, HandleVector<int>.InvalidHandle))
          soundInfo.handle = LoopingSoundManager.StartSound(this.liquid_splash_loop, posCcc);
        LoopingSoundManager.Get().UpdateFirstParameter(soundInfo.handle, FallingWater.HASH_LIQUIDDEPTH, SoundUtil.GetLiquidDepth(cell));
        LoopingSoundManager.Get().UpdateSecondParameter(soundInfo.handle, FallingWater.HASH_LIQUIDVOLUME, this.GetParticleVolume(particleProperty.mass));
        flag2 = false;
      }
    }
    else
    {
      soundInfo = new FallingWater.SoundInfo();
      soundInfo.handle = HandleVector<int>.InvalidHandle;
    }
    soundInfo.startTime = time;
    this.splashSounds[cell] = soundInfo;
    if (!flag2)
      return;
    EventInstance instance = SoundEvent.BeginOneShot(this.liquid_splash_initial, posCcc);
    ((EventInstance) ref instance).setParameterByName("liquidDepth", SoundUtil.GetLiquidDepth(cell), false);
    ((EventInstance) ref instance).setParameterByName("liquidVolume", this.GetParticleVolume(particleProperty.mass), false);
    SoundEvent.EndOneShot(instance);
  }

  private void RemoveParticle(int particleIdx, ref int num_particles)
  {
    --num_particles;
    this.physics[particleIdx] = this.physics[num_particles];
    this.particleProperties[particleIdx] = this.particleProperties[num_particles];
    this.physics.RemoveAt(num_particles);
    this.particleProperties.RemoveAt(num_particles);
  }

  public void Render()
  {
    List<Vector3> vertices = MeshUtil.vertices;
    List<Color32> colours32 = MeshUtil.colours32;
    List<Vector2> uvs = MeshUtil.uvs;
    List<int> indices = MeshUtil.indices;
    uvs.Clear();
    vertices.Clear();
    indices.Clear();
    colours32.Clear();
    float num1 = this.particleSize.x * 0.5f;
    float num2 = this.particleSize.y * 0.5f;
    Vector2 vector2_1;
    // ISSUE: explicit constructor call
    ((Vector2) ref vector2_1).\u002Ector(-num1, -num2);
    Vector2 vector2_2;
    // ISSUE: explicit constructor call
    ((Vector2) ref vector2_2).\u002Ector(num1, -num2);
    Vector2 vector2_3;
    // ISSUE: explicit constructor call
    ((Vector2) ref vector2_3).\u002Ector(num1, num2);
    Vector2 vector2_4;
    // ISSUE: explicit constructor call
    ((Vector2) ref vector2_4).\u002Ector(-num1, num2);
    float num3 = 1f;
    float num4 = 0.0f;
    int num5 = Mathf.Min(this.physics.Count, 16249);
    if (num5 < this.physics.Count)
      DebugUtil.LogWarningArgs(new object[3]
      {
        (object) "Too many water particles to render. Wanted",
        (object) this.physics.Count,
        (object) "but truncating to limit"
      });
    for (int index = 0; index < num5; ++index)
    {
      Vector2 position = this.physics[index].position;
      float num6 = Mathf.Lerp(0.25f, 1f, Mathf.Clamp01(this.particleProperties[index].mass / this.particleMassToSplit));
      vertices.Add(Vector2.op_Implicit(Vector2.op_Addition(position, Vector2.op_Multiply(vector2_1, num6))));
      vertices.Add(Vector2.op_Implicit(Vector2.op_Addition(position, Vector2.op_Multiply(vector2_2, num6))));
      vertices.Add(Vector2.op_Implicit(Vector2.op_Addition(position, Vector2.op_Multiply(vector2_3, num6))));
      vertices.Add(Vector2.op_Implicit(Vector2.op_Addition(position, Vector2.op_Multiply(vector2_4, num6))));
      int frame;
      float num7 = (float) (frame = this.physics[index].frame) * this.uvFrameSize.x;
      float num8 = (float) (frame + 1) * this.uvFrameSize.x;
      uvs.Add(new Vector2(num7, num4));
      uvs.Add(new Vector2(num8, num4));
      uvs.Add(new Vector2(num8, num3));
      uvs.Add(new Vector2(num7, num3));
      Color32 colour = this.physics[index].colour;
      colours32.Add(colour);
      colours32.Add(colour);
      colours32.Add(colour);
      colours32.Add(colour);
      int num9 = index * 4;
      indices.Add(num9);
      indices.Add(num9 + 1);
      indices.Add(num9 + 2);
      indices.Add(num9);
      indices.Add(num9 + 2);
      indices.Add(num9 + 3);
    }
    this.mesh.Clear();
    this.mesh.SetVertices(vertices);
    this.mesh.SetUVs(0, uvs);
    this.mesh.SetColors(colours32);
    this.mesh.SetTriangles(indices, 0);
    Graphics.DrawMesh(this.mesh, this.renderOffset, Quaternion.identity, this.material, LayerMask.NameToLayer("Water"), (Camera) null, 0, this.propertyBlock);
  }

  private KBatchedAnimController SpawnMist()
  {
    GameObject instance = this.mistPool.GetInstance();
    instance.SetActive(true);
    KBatchedAnimController component = instance.GetComponent<KBatchedAnimController>();
    component.Play(HashedString.op_Implicit("loop"), (KAnim.PlayMode) 0);
    return component;
  }

  private GameObject InstantiateMist()
  {
    GameObject gameObject = GameUtil.KInstantiate(this.mistEffect, Grid.SceneLayer.BuildingBack);
    gameObject.SetActive(false);
    gameObject.GetComponent<KBatchedAnimController>().onDestroySelf = new Action<GameObject>(this.ReleaseMist);
    return gameObject;
  }

  private void ReleaseMist(GameObject go)
  {
    go.SetActive(false);
    this.mistPool.ReleaseInstance(go);
  }

  public void Sim200ms(float dt)
  {
    if (this.simUpdateDelay >= 0)
      --this.simUpdateDelay;
    else
      SimAndRenderScheduler.instance.Remove((object) this);
  }

  [System.Runtime.Serialization.OnSerializing]
  private void OnSerializing()
  {
    List<Element> elements = ElementLoader.elements;
    Diseases diseases = Db.Get().Diseases;
    this.serializedParticleProperties = new List<FallingWater.SerializedParticleProperties>();
    foreach (FallingWater.ParticleProperties particleProperty in this.particleProperties)
      this.serializedParticleProperties.Add(new FallingWater.SerializedParticleProperties()
      {
        elementID = elements[(int) particleProperty.elementIdx].id,
        diseaseID = particleProperty.diseaseIdx != byte.MaxValue ? diseases[(int) particleProperty.diseaseIdx].IdHash : HashedString.Invalid,
        mass = particleProperty.mass,
        temperature = particleProperty.temperature,
        diseaseCount = particleProperty.diseaseCount
      });
  }

  [System.Runtime.Serialization.OnSerialized]
  private void OnSerialized() => this.serializedParticleProperties = (List<FallingWater.SerializedParticleProperties>) null;

  [System.Runtime.Serialization.OnDeserialized]
  private void OnDeserialized()
  {
    if (!SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 26))
    {
      for (int index = 0; index < this.physics.Count; ++index)
      {
        int cell = Grid.PosToCell(this.physics[index].position);
        if (Grid.IsValidCell(cell))
        {
          FallingWater.ParticlePhysics physic = this.physics[index] with
          {
            worldIdx = (int) Grid.WorldIdx[cell]
          };
          this.physics[index] = physic;
        }
      }
    }
    if (this.serializedParticleProperties != null)
    {
      Diseases diseases = Db.Get().Diseases;
      this.particleProperties.Clear();
      foreach (FallingWater.SerializedParticleProperties particleProperty in this.serializedParticleProperties)
        this.particleProperties.Add(new FallingWater.ParticleProperties()
        {
          elementIdx = ElementLoader.GetElementIndex(particleProperty.elementID),
          diseaseIdx = HashedString.op_Inequality(particleProperty.diseaseID, HashedString.Invalid) ? diseases.GetIndex(particleProperty.diseaseID) : byte.MaxValue,
          mass = particleProperty.mass,
          temperature = particleProperty.temperature,
          diseaseCount = particleProperty.diseaseCount
        });
    }
    else
      this.particleProperties = this.properties;
    this.properties = (List<FallingWater.ParticleProperties>) null;
  }

  [Serializable]
  private struct DecorInfo
  {
    public string[] names;
    public Vector2 offset;
    public Vector2 size;
  }

  private struct SoundInfo
  {
    public float startTime;
    public int splashCount;
    public HandleVector<int>.Handle handle;
  }

  private struct MistInfo
  {
    public KBatchedAnimController fx;
    public float deathTime;
  }

  private struct ParticlePhysics
  {
    public Vector2 position;
    public Vector2 velocity;
    public int frame;
    public Color32 colour;
    public int worldIdx;

    public ParticlePhysics(
      Vector2 position,
      Vector2 velocity,
      int frame,
      ushort elementIdx,
      int worldIdx)
    {
      this.position = position;
      this.velocity = velocity;
      this.frame = frame;
      this.colour = ElementLoader.elements[(int) elementIdx].substance.colour;
      this.colour.a = (byte) 191;
      this.worldIdx = worldIdx;
    }
  }

  private struct SerializedParticleProperties
  {
    public SimHashes elementID;
    public HashedString diseaseID;
    public float mass;
    public float temperature;
    public int diseaseCount;
  }

  private struct ParticleProperties
  {
    public ushort elementIdx;
    public byte diseaseIdx;
    public float mass;
    public float temperature;
    public int diseaseCount;

    public ParticleProperties(
      ushort elementIdx,
      float mass,
      float temperature,
      byte disease_idx,
      int disease_count,
      bool debug_track)
    {
      this.elementIdx = elementIdx;
      this.diseaseIdx = disease_idx;
      this.mass = mass;
      this.temperature = temperature;
      this.diseaseCount = disease_count;
    }
  }
}
