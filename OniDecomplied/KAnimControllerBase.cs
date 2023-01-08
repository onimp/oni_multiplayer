// Decompiled with JetBrains decompiler
// Type: KAnimControllerBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class KAnimControllerBase : MonoBehaviour
{
  [NonSerialized]
  public GameObject showWhenMissing;
  [SerializeField]
  public KAnimBatchGroup.MaterialType materialType;
  [SerializeField]
  public string initialAnim;
  [SerializeField]
  public KAnim.PlayMode initialMode = (KAnim.PlayMode) 1;
  [SerializeField]
  protected KAnimFile[] animFiles = new KAnimFile[0];
  [SerializeField]
  protected Vector3 offset;
  [SerializeField]
  protected Vector3 pivot;
  [SerializeField]
  protected float rotation;
  [SerializeField]
  public bool destroyOnAnimComplete;
  [SerializeField]
  public bool inactiveDisable;
  [SerializeField]
  protected bool flipX;
  [SerializeField]
  protected bool flipY;
  [SerializeField]
  public bool forceUseGameTime;
  public string defaultAnim;
  protected KAnim.Anim curAnim;
  protected int curAnimFrameIdx = KAnim.Anim.Frame.InvalidFrame.idx;
  protected int prevAnimFrame = KAnim.Anim.Frame.InvalidFrame.idx;
  public bool usingNewSymbolOverrideSystem;
  protected HandleVector<int>.Handle eventManagerHandle = HandleVector<int>.InvalidHandle;
  protected List<KAnimControllerBase.OverrideAnimFileData> overrideAnimFiles = new List<KAnimControllerBase.OverrideAnimFileData>();
  protected DeepProfiler DeepProfiler = new DeepProfiler(false);
  public bool randomiseLoopedOffset;
  protected float elapsedTime;
  protected float playSpeed = 1f;
  protected KAnim.PlayMode mode = (KAnim.PlayMode) 1;
  protected bool stopped = true;
  public float animHeight = 1f;
  public float animWidth = 1f;
  protected bool isVisible;
  protected Bounds bounds;
  public Action<Bounds> OnUpdateBounds;
  public Action<Color> OnTintChanged;
  public Action<Color> OnHighlightChanged;
  protected KAnimSynchronizer synchronizer;
  protected KAnimLayering layering;
  [SerializeField]
  protected bool _enabled = true;
  protected bool hasEnableRun;
  protected bool hasAwakeRun;
  protected KBatchedAnimInstanceData batchInstanceData;
  public KAnimControllerBase.VisibilityType visibilityType;
  public Action<GameObject> onDestroySelf;
  [SerializeField]
  protected List<KAnimHashedString> hiddenSymbols = new List<KAnimHashedString>();
  protected Dictionary<HashedString, KAnimControllerBase.AnimLookupData> anims = new Dictionary<HashedString, KAnimControllerBase.AnimLookupData>();
  protected Dictionary<HashedString, KAnimControllerBase.AnimLookupData> overrideAnims = new Dictionary<HashedString, KAnimControllerBase.AnimLookupData>();
  protected System.Collections.Generic.Queue<KAnimControllerBase.AnimData> animQueue = new System.Collections.Generic.Queue<KAnimControllerBase.AnimData>();
  protected int maxSymbols;
  public Grid.SceneLayer fgLayer = Grid.SceneLayer.NoLayer;
  protected AnimEventManager aem;
  private static HashedString snaptoPivot = new HashedString("snapTo_pivot");

  protected KAnimControllerBase()
  {
    this.previousFrame = -1;
    this.currentFrame = -1;
    this.PlaySpeedMultiplier = 1f;
    this.synchronizer = new KAnimSynchronizer(this);
    this.layering = new KAnimLayering(this, this.fgLayer);
    this.isVisible = true;
  }

  public abstract KAnim.Anim GetAnim(int index);

  public string debugName { get; private set; }

  public KAnim.Build curBuild { get; protected set; }

  public event Action<Color32> OnOverlayColourChanged;

  public bool enabled
  {
    get => this._enabled;
    set
    {
      this._enabled = value;
      if (!this.hasAwakeRun)
        return;
      if (this._enabled)
        this.Enable();
      else
        this.Disable();
    }
  }

  public bool HasBatchInstanceData => this.batchInstanceData != null;

  public SymbolInstanceGpuData symbolInstanceGpuData { get; protected set; }

  public SymbolOverrideInfoGpuData symbolOverrideInfoGpuData { get; protected set; }

  public Color32 TintColour
  {
    get => Color32.op_Implicit(this.batchInstanceData.GetTintColour());
    set
    {
      if (this.batchInstanceData == null || !this.batchInstanceData.SetTintColour(Color32.op_Implicit(value)))
        return;
      this.SetDirty();
      this.SuspendUpdates(false);
      if (this.OnTintChanged == null)
        return;
      this.OnTintChanged(Color32.op_Implicit(value));
    }
  }

  public Color32 HighlightColour
  {
    get => Color32.op_Implicit(this.batchInstanceData.GetHighlightcolour());
    set
    {
      if (!this.batchInstanceData.SetHighlightColour(Color32.op_Implicit(value)))
        return;
      this.SetDirty();
      this.SuspendUpdates(false);
      if (this.OnHighlightChanged == null)
        return;
      this.OnHighlightChanged(Color32.op_Implicit(value));
    }
  }

  public Color OverlayColour
  {
    get => this.batchInstanceData.GetOverlayColour();
    set
    {
      if (!this.batchInstanceData.SetOverlayColour(value))
        return;
      this.SetDirty();
      this.SuspendUpdates(false);
      if (this.OnOverlayColourChanged == null)
        return;
      this.OnOverlayColourChanged(Color32.op_Implicit(value));
    }
  }

  public event KAnimControllerBase.KAnimEvent onAnimEnter;

  public event KAnimControllerBase.KAnimEvent onAnimComplete;

  public event Action<int> onLayerChanged;

  public int previousFrame { get; protected set; }

  public int currentFrame { get; protected set; }

  public HashedString currentAnim => this.curAnim == null ? new HashedString() : this.curAnim.hash;

  public float PlaySpeedMultiplier { set; get; }

  public void SetFGLayer(Grid.SceneLayer layer)
  {
    this.fgLayer = layer;
    this.GetLayering();
    if (this.layering == null)
      return;
    this.layering.SetLayer(this.fgLayer);
  }

  public KAnim.PlayMode PlayMode
  {
    get => this.mode;
    set => this.mode = value;
  }

  public bool FlipX
  {
    get => this.flipX;
    set
    {
      this.flipX = value;
      if (this.layering != null)
        this.layering.Dirty();
      this.SetDirty();
    }
  }

  public bool FlipY
  {
    get => this.flipY;
    set
    {
      this.flipY = value;
      if (this.layering != null)
        this.layering.Dirty();
      this.SetDirty();
    }
  }

  public Vector3 Offset
  {
    get => this.offset;
    set
    {
      this.offset = value;
      if (this.layering != null)
        this.layering.Dirty();
      this.DeRegister();
      this.Register();
      this.RefreshVisibilityListener();
      this.SetDirty();
    }
  }

  public float Rotation
  {
    get => this.rotation;
    set
    {
      this.rotation = value;
      if (this.layering != null)
        this.layering.Dirty();
      this.SetDirty();
    }
  }

  public Vector3 Pivot
  {
    get => this.pivot;
    set
    {
      this.pivot = value;
      if (this.layering != null)
        this.layering.Dirty();
      this.SetDirty();
    }
  }

  public Vector3 PositionIncludingOffset => Vector3.op_Addition(TransformExtensions.GetPosition(((Component) this).transform), this.Offset);

  public KAnimBatchGroup.MaterialType GetMaterialType() => this.materialType;

  public Vector3 GetWorldPivot()
  {
    Vector3 position = TransformExtensions.GetPosition(((Component) this).transform);
    KBoxCollider2D component = ((Component) this).GetComponent<KBoxCollider2D>();
    if (Object.op_Inequality((Object) component, (Object) null))
    {
      position.x += component.offset.x;
      position.y += component.offset.y - component.size.y / 2f;
    }
    return position;
  }

  public KAnim.Anim GetCurrentAnim() => this.curAnim;

  public KAnimHashedString GetBuildHash() => this.curBuild == null ? KAnimHashedString.op_Implicit(KAnimBatchManager.NO_BATCH) : this.curBuild.fileHash;

  protected float GetDuration() => this.curAnim != null ? (float) this.curAnim.numFrames / this.curAnim.frameRate : 0.0f;

  protected int GetFrameIdxFromOffset(int offset)
  {
    int frameIdxFromOffset = -1;
    if (this.curAnim != null)
      frameIdxFromOffset = offset + this.curAnim.firstFrameIdx;
    return frameIdxFromOffset;
  }

  public int GetFrameIdx(float time, bool absolute)
  {
    int frameIdx = -1;
    if (this.curAnim != null)
      frameIdx = this.curAnim.GetFrameIdx(this.mode, time) + (absolute ? this.curAnim.firstFrameIdx : 0);
    return frameIdx;
  }

  public bool IsStopped() => this.stopped;

  public KAnim.Anim CurrentAnim => this.curAnim;

  public KAnimSynchronizer GetSynchronizer() => this.synchronizer;

  public KAnimLayering GetLayering()
  {
    if (this.layering == null && this.fgLayer != Grid.SceneLayer.NoLayer)
      this.layering = new KAnimLayering(this, this.fgLayer);
    return this.layering;
  }

  public KAnim.PlayMode GetMode() => this.mode;

  public static string GetModeString(KAnim.PlayMode mode)
  {
    switch ((int) mode)
    {
      case 0:
        return "Loop";
      case 1:
        return "Once";
      case 2:
        return "Paused";
      default:
        return "Unknown";
    }
  }

  public float GetPlaySpeed() => this.playSpeed;

  public void SetElapsedTime(float value) => this.elapsedTime = value;

  public float GetElapsedTime() => this.elapsedTime;

  protected abstract void SuspendUpdates(bool suspend);

  protected abstract void OnStartQueuedAnim();

  public abstract void SetDirty();

  protected abstract void RefreshVisibilityListener();

  protected abstract void DeRegister();

  protected abstract void Register();

  protected abstract void OnAwake();

  protected abstract void OnStart();

  protected abstract void OnStop();

  protected abstract void Enable();

  protected abstract void Disable();

  protected abstract void UpdateFrame(float t);

  public abstract Matrix2x3 GetTransformMatrix();

  public abstract Matrix2x3 GetSymbolLocalTransform(HashedString symbol, out bool symbolVisible);

  public abstract void UpdateHidden();

  public abstract void TriggerStop();

  public virtual void SetLayer(int layer)
  {
    if (this.onLayerChanged == null)
      return;
    this.onLayerChanged(layer);
  }

  public Vector3 GetPivotSymbolPosition()
  {
    bool symbolVisible = false;
    Matrix4x4 symbolTransform = this.GetSymbolTransform(KAnimControllerBase.snaptoPivot, out symbolVisible);
    Vector3 position = TransformExtensions.GetPosition(((Component) this).transform);
    if (symbolVisible)
    {
      // ISSUE: explicit constructor call
      ((Vector3) ref position).\u002Ector(((Matrix4x4) ref symbolTransform)[0, 3], ((Matrix4x4) ref symbolTransform)[1, 3], ((Matrix4x4) ref symbolTransform)[2, 3]);
    }
    return position;
  }

  public virtual Matrix4x4 GetSymbolTransform(HashedString symbol, out bool symbolVisible)
  {
    symbolVisible = false;
    return Matrix4x4.identity;
  }

  private void Awake()
  {
    this.aem = Singleton<AnimEventManager>.Instance;
    this.debugName = ((Object) this).name;
    this.SetFGLayer(this.fgLayer);
    this.OnAwake();
    if (!string.IsNullOrEmpty(this.initialAnim))
    {
      this.SetDirty();
      this.Play(HashedString.op_Implicit(this.initialAnim), this.initialMode);
    }
    this.hasAwakeRun = true;
  }

  private void Start() => this.OnStart();

  protected virtual void OnDestroy()
  {
    this.animFiles = (KAnimFile[]) null;
    this.curAnim = (KAnim.Anim) null;
    this.curBuild = (KAnim.Build) null;
    this.synchronizer = (KAnimSynchronizer) null;
    this.layering = (KAnimLayering) null;
    this.animQueue = (System.Collections.Generic.Queue<KAnimControllerBase.AnimData>) null;
    this.overrideAnims = (Dictionary<HashedString, KAnimControllerBase.AnimLookupData>) null;
    this.anims = (Dictionary<HashedString, KAnimControllerBase.AnimLookupData>) null;
    this.synchronizer = (KAnimSynchronizer) null;
    this.layering = (KAnimLayering) null;
    this.overrideAnimFiles = (List<KAnimControllerBase.OverrideAnimFileData>) null;
  }

  protected void AnimEnter(HashedString hashed_name)
  {
    if (this.onAnimEnter == null)
      return;
    this.onAnimEnter(hashed_name);
  }

  public void Play(HashedString anim_name, KAnim.PlayMode mode = 1, float speed = 1f, float time_offset = 0.0f)
  {
    if (!this.stopped)
      this.Stop();
    this.Queue(anim_name, mode, speed, time_offset);
  }

  public void Play(HashedString[] anim_names, KAnim.PlayMode mode = 1)
  {
    if (!this.stopped)
      this.Stop();
    for (int index = 0; index < anim_names.Length - 1; ++index)
      this.Queue(anim_names[index]);
    Debug.Assert(anim_names.Length != 0, (object) "Play was called with an empty anim array");
    this.Queue(anim_names[anim_names.Length - 1], mode);
  }

  public void Queue(HashedString anim_name, KAnim.PlayMode mode = 1, float speed = 1f, float time_offset = 0.0f)
  {
    this.animQueue.Enqueue(new KAnimControllerBase.AnimData()
    {
      anim = anim_name,
      mode = mode,
      speed = speed,
      timeOffset = time_offset
    });
    this.mode = mode == 2 ? (KAnim.PlayMode) 2 : (KAnim.PlayMode) 1;
    if (this.aem != null)
      this.aem.SetMode(this.eventManagerHandle, this.mode);
    if (this.animQueue.Count != 1 || !this.stopped)
      return;
    this.StartQueuedAnim();
  }

  public void QueueAndSyncTransition(
    HashedString anim_name,
    KAnim.PlayMode mode = 1,
    float speed = 1f,
    float time_offset = 0.0f)
  {
    this.SyncTransition();
    this.Queue(anim_name, mode, speed, time_offset);
  }

  public void SyncTransition() => this.elapsedTime %= Mathf.Max(float.Epsilon, this.GetDuration());

  public void ClearQueue() => this.animQueue.Clear();

  private void Restart(
    HashedString anim_name,
    KAnim.PlayMode mode = 1,
    float speed = 1f,
    float time_offset = 0.0f)
  {
    if (this.curBuild == null)
    {
      Debug.LogWarning((object) ("[" + ((Object) ((Component) this).gameObject).name + "] Missing build while trying to play anim [" + anim_name.ToString() + "]"), (Object) ((Component) this).gameObject);
    }
    else
    {
      System.Collections.Generic.Queue<KAnimControllerBase.AnimData> animDataQueue = new System.Collections.Generic.Queue<KAnimControllerBase.AnimData>();
      animDataQueue.Enqueue(new KAnimControllerBase.AnimData()
      {
        anim = anim_name,
        mode = mode,
        speed = speed,
        timeOffset = time_offset
      });
      while (this.animQueue.Count > 0)
        animDataQueue.Enqueue(this.animQueue.Dequeue());
      this.animQueue = animDataQueue;
      if (this.animQueue.Count != 1 || !this.stopped)
        return;
      this.StartQueuedAnim();
    }
  }

  protected void StartQueuedAnim()
  {
    this.StopAnimEventSequence();
    this.previousFrame = -1;
    this.currentFrame = -1;
    this.SuspendUpdates(false);
    this.stopped = false;
    this.OnStartQueuedAnim();
    KAnimControllerBase.AnimData animData = this.animQueue.Dequeue();
    while (animData.mode == null && this.animQueue.Count > 0)
      animData = this.animQueue.Dequeue();
    KAnimControllerBase.AnimLookupData animLookupData;
    if (this.overrideAnims == null || !this.overrideAnims.TryGetValue(animData.anim, out animLookupData))
    {
      if (!this.anims.TryGetValue(animData.anim, out animLookupData))
      {
        if (Object.op_Inequality((Object) this.showWhenMissing, (Object) null))
          this.showWhenMissing.SetActive(true);
        if (true)
        {
          this.TriggerStop();
          return;
        }
      }
      else if (Object.op_Inequality((Object) this.showWhenMissing, (Object) null))
        this.showWhenMissing.SetActive(false);
    }
    this.curAnim = this.GetAnim(animLookupData.animIndex);
    int offset = 0;
    if (animData.mode == null && this.randomiseLoopedOffset)
      offset = Random.Range(0, this.curAnim.numFrames - 1);
    this.prevAnimFrame = -1;
    this.curAnimFrameIdx = this.GetFrameIdxFromOffset(offset);
    this.currentFrame = this.curAnimFrameIdx;
    this.mode = animData.mode;
    this.playSpeed = animData.speed * this.PlaySpeedMultiplier;
    this.SetElapsedTime((float) offset / this.curAnim.frameRate + animData.timeOffset);
    this.synchronizer.Sync();
    this.StartAnimEventSequence();
    this.AnimEnter(animData.anim);
  }

  public bool GetSymbolVisiblity(KAnimHashedString symbol) => !this.hiddenSymbols.Contains(symbol);

  public void SetSymbolVisiblity(KAnimHashedString symbol, bool is_visible)
  {
    if (is_visible)
      this.hiddenSymbols.Remove(symbol);
    else if (!this.hiddenSymbols.Contains(symbol))
      this.hiddenSymbols.Add(symbol);
    if (this.curBuild == null)
      return;
    this.UpdateHidden();
  }

  public void AddAnimOverrides(KAnimFile kanim_file, float priority = 0.0f)
  {
    Debug.Assert(Object.op_Inequality((Object) kanim_file, (Object) null));
    if (kanim_file.GetData().build != null && kanim_file.GetData().build.symbols.Length != 0)
    {
      SymbolOverrideController component = ((Component) this).GetComponent<SymbolOverrideController>();
      DebugUtil.Assert(Object.op_Inequality((Object) component, (Object) null), "Anim overrides containing additional symbols require a symbol override controller.");
      component.AddBuildOverride(kanim_file.GetData());
    }
    this.overrideAnimFiles.Add(new KAnimControllerBase.OverrideAnimFileData()
    {
      priority = priority,
      file = kanim_file
    });
    this.overrideAnimFiles.Sort((Comparison<KAnimControllerBase.OverrideAnimFileData>) ((a, b) => b.priority.CompareTo(a.priority)));
    this.RebuildOverrides(kanim_file);
  }

  public void RemoveAnimOverrides(KAnimFile kanim_file)
  {
    Debug.Assert(Object.op_Inequality((Object) kanim_file, (Object) null));
    if (kanim_file.GetData().build != null && kanim_file.GetData().build.symbols.Length != 0)
    {
      SymbolOverrideController component = ((Component) this).GetComponent<SymbolOverrideController>();
      DebugUtil.Assert(Object.op_Inequality((Object) component, (Object) null), "Anim overrides containing additional symbols require a symbol override controller.");
      component.TryRemoveBuildOverride(kanim_file.GetData());
    }
    for (int index = 0; index < this.overrideAnimFiles.Count; ++index)
    {
      if (Object.op_Equality((Object) this.overrideAnimFiles[index].file, (Object) kanim_file))
      {
        this.overrideAnimFiles.RemoveAt(index);
        break;
      }
    }
    this.RebuildOverrides(kanim_file);
  }

  private void RebuildOverrides(KAnimFile kanim_file)
  {
    bool flag = false;
    this.overrideAnims.Clear();
    for (int index1 = 0; index1 < this.overrideAnimFiles.Count; ++index1)
    {
      KAnimControllerBase.OverrideAnimFileData overrideAnimFile = this.overrideAnimFiles[index1];
      KAnimFileData data = overrideAnimFile.file.GetData();
      for (int index2 = 0; index2 < data.animCount; ++index2)
      {
        KAnim.Anim anim = data.GetAnim(index2);
        if (KAnimHashedString.op_Inequality(anim.animFile.hashName, data.hashName))
          Debug.LogError((object) string.Format("How did we get an anim from another file? [{0}] != [{1}] for anim [{2}]", (object) data.name, (object) anim.animFile.name, (object) index2));
        KAnimControllerBase.AnimLookupData animLookupData = new KAnimControllerBase.AnimLookupData();
        animLookupData.animIndex = anim.index;
        HashedString key;
        // ISSUE: explicit constructor call
        ((HashedString) ref key).\u002Ector(anim.name);
        if (!this.overrideAnims.ContainsKey(key))
          this.overrideAnims[key] = animLookupData;
        if (this.curAnim != null && HashedString.op_Equality(this.curAnim.hash, key) && Object.op_Equality((Object) overrideAnimFile.file, (Object) kanim_file))
          flag = true;
      }
    }
    if (!flag)
      return;
    this.Restart(HashedString.op_Implicit(this.curAnim.name), this.mode, this.playSpeed);
  }

  public bool HasAnimation(HashedString anim_name)
  {
    bool flag = ((HashedString) ref anim_name).IsValid;
    if (flag)
    {
      int num = this.anims.ContainsKey(anim_name) ? 1 : 0;
      flag = (num | (num != 0 ? (false ? 1 : 0) : (this.overrideAnims.ContainsKey(anim_name) ? 1 : 0))) != 0;
    }
    return flag;
  }

  public bool HasAnimationFile(KAnimHashedString anim_file_name)
  {
    KAnimFile match = (KAnimFile) null;
    return this.TryGetAnimationFile(anim_file_name, out match);
  }

  public bool TryGetAnimationFile(KAnimHashedString anim_file_name, out KAnimFile match)
  {
    match = (KAnimFile) null;
    if (!((KAnimHashedString) ref anim_file_name).IsValid())
      return false;
    KAnimFileData kanimFileData1 = (KAnimFileData) null;
    int index1 = 0;
    int index2 = this.overrideAnimFiles.Count - 1;
    int num1 = (int) ((double) this.overrideAnimFiles.Count * 0.5);
    while (num1 > 0 && Object.op_Equality((Object) match, (Object) null) && index1 < num1)
    {
      if (Object.op_Inequality((Object) this.overrideAnimFiles[index1].file, (Object) null))
        kanimFileData1 = this.overrideAnimFiles[index1].file.GetData();
      if (kanimFileData1 != null)
      {
        KAnimHashedString hashName = kanimFileData1.hashName;
        if (((KAnimHashedString) ref hashName).HashValue == ((KAnimHashedString) ref anim_file_name).HashValue)
        {
          match = this.overrideAnimFiles[index1].file;
          break;
        }
      }
      if (Object.op_Inequality((Object) this.overrideAnimFiles[index2].file, (Object) null))
        kanimFileData1 = this.overrideAnimFiles[index2].file.GetData();
      if (kanimFileData1 != null)
      {
        KAnimHashedString hashName = kanimFileData1.hashName;
        if (((KAnimHashedString) ref hashName).HashValue == ((KAnimHashedString) ref anim_file_name).HashValue)
          match = this.overrideAnimFiles[index2].file;
      }
      ++index1;
      --index2;
    }
    if (Object.op_Equality((Object) match, (Object) null) && this.overrideAnimFiles.Count % 2 != 0)
    {
      if (Object.op_Inequality((Object) this.overrideAnimFiles[index1].file, (Object) null))
        kanimFileData1 = this.overrideAnimFiles[index1].file.GetData();
      if (kanimFileData1 != null)
      {
        KAnimHashedString hashName = kanimFileData1.hashName;
        if (((KAnimHashedString) ref hashName).HashValue == ((KAnimHashedString) ref anim_file_name).HashValue)
          match = this.overrideAnimFiles[index1].file;
      }
    }
    KAnimFileData kanimFileData2 = (KAnimFileData) null;
    if (Object.op_Equality((Object) match, (Object) null) && this.animFiles != null)
    {
      int index3 = 0;
      int index4 = this.animFiles.Length - 1;
      int num2 = (int) ((double) this.animFiles.Length * 0.5);
      while (num2 > 0 && Object.op_Equality((Object) match, (Object) null) && index3 < num2)
      {
        if (Object.op_Inequality((Object) this.animFiles[index3], (Object) null))
          kanimFileData2 = this.animFiles[index3].GetData();
        if (kanimFileData2 != null)
        {
          KAnimHashedString hashName = kanimFileData2.hashName;
          if (((KAnimHashedString) ref hashName).HashValue == ((KAnimHashedString) ref anim_file_name).HashValue)
          {
            match = this.animFiles[index3];
            break;
          }
        }
        if (Object.op_Inequality((Object) this.animFiles[index4], (Object) null))
          kanimFileData2 = this.animFiles[index4].GetData();
        if (kanimFileData2 != null)
        {
          KAnimHashedString hashName = kanimFileData2.hashName;
          if (((KAnimHashedString) ref hashName).HashValue == ((KAnimHashedString) ref anim_file_name).HashValue)
            match = this.animFiles[index4];
        }
        ++index3;
        --index4;
      }
      if (Object.op_Equality((Object) match, (Object) null) && this.animFiles.Length % 2 != 0)
      {
        if (Object.op_Inequality((Object) this.animFiles[index3], (Object) null))
          kanimFileData2 = this.animFiles[index3].GetData();
        if (kanimFileData2 != null)
        {
          KAnimHashedString hashName = kanimFileData2.hashName;
          if (((KAnimHashedString) ref hashName).HashValue == ((KAnimHashedString) ref anim_file_name).HashValue)
            match = this.animFiles[index3];
        }
      }
    }
    return Object.op_Inequality((Object) match, (Object) null);
  }

  public void AddAnims(KAnimFile anim_file)
  {
    KAnimFileData data = anim_file.GetData();
    if (data == null)
    {
      Debug.LogError((object) "AddAnims() Null animfile data");
    }
    else
    {
      this.maxSymbols = Mathf.Max(this.maxSymbols, data.maxVisSymbolFrames);
      for (int index = 0; index < data.animCount; ++index)
      {
        KAnim.Anim anim = data.GetAnim(index);
        if (KAnimHashedString.op_Inequality(anim.animFile.hashName, data.hashName))
          Debug.LogErrorFormat("How did we get an anim from another file? [{0}] != [{1}] for anim [{2}]", new object[3]
          {
            (object) data.name,
            (object) anim.animFile.name,
            (object) index
          });
        this.anims[anim.hash] = new KAnimControllerBase.AnimLookupData()
        {
          animIndex = anim.index
        };
      }
      if (!this.usingNewSymbolOverrideSystem || data.buildIndex == -1 || data.build.symbols == null || data.build.symbols.Length == 0)
        return;
      ((Component) this).GetComponent<SymbolOverrideController>().AddBuildOverride(anim_file.GetData(), -1);
    }
  }

  public KAnimFile[] AnimFiles
  {
    get => this.animFiles;
    set
    {
      DebugUtil.AssertArgs((value.Length != 0 ? 1 : 0) != 0, new object[2]
      {
        (object) "Controller has no anim files.",
        (object) ((Component) this).gameObject
      });
      DebugUtil.AssertArgs((Object.op_Inequality((Object) value[0], (Object) null) ? 1 : 0) != 0, new object[2]
      {
        (object) "First anim file needs to be non-null.",
        (object) ((Component) this).gameObject
      });
      DebugUtil.AssertArgs((value[0].IsBuildLoaded ? 1 : 0) != 0, new object[2]
      {
        (object) "First anim file needs to be the build file.",
        (object) ((Component) this).gameObject
      });
      for (int index = 0; index < value.Length; ++index)
        DebugUtil.AssertArgs((Object.op_Inequality((Object) value[index], (Object) null) ? 1 : 0) != 0, new object[2]
        {
          (object) "Anim file is null",
          (object) ((Component) this).gameObject
        });
      this.animFiles = new KAnimFile[value.Length];
      for (int index = 0; index < value.Length; ++index)
        this.animFiles[index] = value[index];
    }
  }

  public IReadOnlyList<KAnimControllerBase.OverrideAnimFileData> OverrideAnimFiles => (IReadOnlyList<KAnimControllerBase.OverrideAnimFileData>) this.overrideAnimFiles;

  public void Stop()
  {
    if (this.curAnim != null)
      this.StopAnimEventSequence();
    this.animQueue.Clear();
    this.stopped = true;
    if (this.onAnimComplete != null)
      this.onAnimComplete(this.curAnim == null ? HashedString.Invalid : this.curAnim.hash);
    this.OnStop();
  }

  public void StopAndClear()
  {
    if (!this.stopped)
      this.Stop();
    ((Bounds) ref this.bounds).center = Vector3.zero;
    ((Bounds) ref this.bounds).extents = Vector3.zero;
    if (this.OnUpdateBounds == null)
      return;
    this.OnUpdateBounds(this.bounds);
  }

  public float GetPositionPercent() => this.GetElapsedTime() / this.GetDuration();

  public void SetPositionPercent(float percent)
  {
    if (this.curAnim == null)
      return;
    this.SetElapsedTime((float) this.curAnim.numFrames / this.curAnim.frameRate * percent);
    if (this.currentFrame == this.curAnim.GetFrameIdx(this.mode, this.elapsedTime))
      return;
    this.SetDirty();
    this.UpdateAnimEventSequenceTime();
    this.SuspendUpdates(false);
  }

  protected void StartAnimEventSequence()
  {
    if (this.layering.GetIsForeground() || this.aem == null)
      return;
    this.eventManagerHandle = this.aem.PlayAnim(this, this.curAnim, this.mode, this.elapsedTime, this.visibilityType == KAnimControllerBase.VisibilityType.Always);
  }

  protected void UpdateAnimEventSequenceTime()
  {
    if (!this.eventManagerHandle.IsValid() || this.aem == null)
      return;
    this.aem.SetElapsedTime(this.eventManagerHandle, this.elapsedTime);
  }

  protected void StopAnimEventSequence()
  {
    if (!this.eventManagerHandle.IsValid() || this.aem == null)
      return;
    if (!this.stopped && this.mode != 2)
      this.SetElapsedTime(this.aem.GetElapsedTime(this.eventManagerHandle));
    this.aem.StopAnim(this.eventManagerHandle);
    this.eventManagerHandle = HandleVector<int>.InvalidHandle;
  }

  protected void DestroySelf()
  {
    if (this.onDestroySelf != null)
      this.onDestroySelf(((Component) this).gameObject);
    else
      Util.KDestroyGameObject(((Component) this).gameObject);
  }

  public struct OverrideAnimFileData
  {
    public float priority;
    public KAnimFile file;
  }

  public struct AnimLookupData
  {
    public int animIndex;
  }

  public struct AnimData
  {
    public HashedString anim;
    public KAnim.PlayMode mode;
    public float speed;
    public float timeOffset;
  }

  public enum VisibilityType
  {
    Default,
    OffscreenUpdate,
    Always,
  }

  public delegate void KAnimEvent(HashedString name);
}
