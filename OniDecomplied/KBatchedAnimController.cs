// Decompiled with JetBrains decompiler
// Type: KBatchedAnimController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

[DebuggerDisplay("{name} visible={isVisible} suspendUpdates={suspendUpdates} moving={moving}")]
public class KBatchedAnimController : KAnimControllerBase, KAnimConverter.IAnimConverter
{
  [NonSerialized]
  protected bool _forceRebuild;
  private Vector3 lastPos = Vector3.zero;
  private Vector2I lastChunkXY = KBatchedAnimUpdater.INVALID_CHUNK_ID;
  private KAnimBatch batch;
  public float animScale = 0.005f;
  private bool suspendUpdates;
  private bool visibilityListenerRegistered;
  private bool moving;
  private SymbolOverrideController symbolOverrideController;
  private int symbolOverrideControllerVersion;
  [NonSerialized]
  public KBatchedAnimUpdater.RegistrationState updateRegistrationState = KBatchedAnimUpdater.RegistrationState.Unregistered;
  public Grid.SceneLayer sceneLayer;
  private RectTransform rt;
  private Vector3 screenOffset = new Vector3(0.0f, 0.0f, 0.0f);
  public Matrix2x3 navMatrix = Matrix2x3.identity;
  private CanvasScaler scaler;
  public bool setScaleFromAnim = true;
  public Vector2 animOverrideSize = Vector2.one;
  private Canvas rootCanvas;
  public bool isMovable;

  public int GetCurrentFrameIndex() => this.curAnimFrameIdx;

  public KBatchedAnimInstanceData GetBatchInstanceData() => this.batchInstanceData;

  protected bool forceRebuild
  {
    get => this._forceRebuild;
    set => this._forceRebuild = value;
  }

  public KBatchedAnimController() => this.batchInstanceData = new KBatchedAnimInstanceData((KAnimConverter.IAnimConverter) this);

  public bool IsActive() => ((Behaviour) this).isActiveAndEnabled && this._enabled;

  public bool IsVisible() => this.isVisible;

  public void SetSymbolScale(KAnimHashedString symbol_name, float scale)
  {
    KAnim.Build.Symbol symbol = KAnimBatchManager.Instance().GetBatchGroupData(this.GetBatchGroupID()).GetSymbol(symbol_name);
    if (symbol == null)
      return;
    this.symbolInstanceGpuData.SetSymbolScale(symbol.symbolIndexInSourceBuild, scale);
    this.SuspendUpdates(false);
    this.SetDirty();
  }

  public void SetSymbolTint(KAnimHashedString symbol_name, Color color)
  {
    KAnim.Build.Symbol symbol = KAnimBatchManager.Instance().GetBatchGroupData(this.GetBatchGroupID()).GetSymbol(symbol_name);
    if (symbol == null)
      return;
    this.symbolInstanceGpuData.SetSymbolTint(symbol.symbolIndexInSourceBuild, color);
    this.SuspendUpdates(false);
    this.SetDirty();
  }

  public Vector2I GetCellXY()
  {
    Vector3 positionIncludingOffset = this.PositionIncludingOffset;
    return (double) Grid.CellSizeInMeters == 0.0 ? new Vector2I((int) positionIncludingOffset.x, (int) positionIncludingOffset.y) : Grid.PosToXY(positionIncludingOffset);
  }

  public float GetZ() => TransformExtensions.GetPosition(((Component) this).transform).z;

  public string GetName() => ((Object) this).name;

  public override KAnim.Anim GetAnim(int index)
  {
    HashedString batchGroupId = this.batchGroupID;
    if (!((HashedString) ref batchGroupId).IsValid || !HashedString.op_Inequality(this.batchGroupID, KAnimBatchManager.NO_BATCH))
      Debug.LogError((object) (((Object) this).name + " batch not ready"));
    KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(this.batchGroupID);
    Debug.Assert(batchGroupData != null);
    return batchGroupData.GetAnim(index);
  }

  private void Initialize()
  {
    HashedString batchGroupId = this.batchGroupID;
    if (!((HashedString) ref batchGroupId).IsValid || !HashedString.op_Inequality(this.batchGroupID, KAnimBatchManager.NO_BATCH))
      return;
    this.DeRegister();
    this.Register();
  }

  private void OnMovementStateChanged(bool is_moving)
  {
    if (is_moving == this.moving)
      return;
    this.moving = is_moving;
    this.SetDirty();
    this.ConfigureUpdateListener();
  }

  private static void OnMovementStateChanged(Transform transform, bool is_moving) => ((Component) transform).GetComponent<KBatchedAnimController>().OnMovementStateChanged(is_moving);

  private void SetBatchGroup(KAnimFileData kafd)
  {
    HashedString batchGroupId1 = this.batchGroupID;
    if (((HashedString) ref batchGroupId1).IsValid && kafd != null && HashedString.op_Equality(this.batchGroupID, kafd.batchTag))
      return;
    HashedString batchGroupId2 = this.batchGroupID;
    DebugUtil.Assert(!((HashedString) ref batchGroupId2).IsValid, "Should only be setting the batch group once.");
    DebugUtil.Assert(kafd != null, "Null anim data!! For", ((Object) this).name);
    this.curBuild = kafd.build;
    DebugUtil.Assert(this.curBuild != null, "Null build for anim!! ", ((Object) this).name, kafd.name);
    KAnimGroupFile.Group group = KAnimGroupFile.GetGroup(this.curBuild.batchTag);
    HashedString hashedString1 = kafd.build.batchTag;
    HashedString hashedString2;
    if (group.renderType == 3 || group.renderType == 4)
    {
      int num = ((HashedString) ref group.swapTarget).IsValid ? 1 : 0;
      hashedString2 = group.id;
      string str = "Invalid swap target fro group [" + hashedString2.ToString() + "]";
      Debug.Assert(num != 0, (object) str);
      hashedString1 = group.swapTarget;
    }
    this.batchGroupID = hashedString1;
    this.symbolInstanceGpuData = new SymbolInstanceGpuData(KAnimBatchManager.instance.GetBatchGroupData(this.batchGroupID).maxSymbolsPerBuild);
    this.symbolOverrideInfoGpuData = new SymbolOverrideInfoGpuData(KAnimBatchManager.instance.GetBatchGroupData(this.batchGroupID).symbolFrameInstances.Count);
    hashedString2 = this.batchGroupID;
    if (!((HashedString) ref hashedString2).IsValid || HashedString.op_Equality(this.batchGroupID, KAnimBatchManager.NO_BATCH))
      Debug.LogError((object) ("Batch is not ready: " + ((Object) this).name));
    if (this.materialType != null || !HashedString.op_Equality(this.batchGroupID, KAnimBatchManager.BATCH_HUMAN))
      return;
    this.materialType = (KAnimBatchGroup.MaterialType) 5;
  }

  public void LoadAnims()
  {
    if (!KAnimBatchManager.Instance().isReady)
      Debug.LogError((object) ("KAnimBatchManager is not ready when loading anim:" + ((Object) this).name));
    if (this.animFiles.Length == 0)
      DebugUtil.Assert(false, "KBatchedAnimController has no anim files:" + ((Object) this).name);
    if (!this.animFiles[0].IsBuildLoaded)
      DebugUtil.LogErrorArgs((Object) ((Component) this).gameObject, new object[1]
      {
        (object) string.Format("First anim file needs to be the build file but {0} doesn't have an associated build", (object) this.animFiles[0].GetData().name)
      });
    this.overrideAnims.Clear();
    this.anims.Clear();
    this.SetBatchGroup(this.animFiles[0].GetData());
    for (int index = 0; index < this.animFiles.Length; ++index)
      this.AddAnims(this.animFiles[index]);
    this.forceRebuild = true;
    if (this.layering != null)
      this.layering.HideSymbols();
    if (!this.usingNewSymbolOverrideSystem)
      return;
    DebugUtil.Assert(Object.op_Inequality((Object) ((Component) this).GetComponent<SymbolOverrideController>(), (Object) null));
  }

  public void SwapAnims(KAnimFile[] anims)
  {
    HashedString batchGroupId = this.batchGroupID;
    if (((HashedString) ref batchGroupId).IsValid)
    {
      this.DeRegister();
      this.batchGroupID = HashedString.Invalid;
    }
    this.AnimFiles = anims;
    this.LoadAnims();
    this.Register();
  }

  public void UpdateAnim(float dt)
  {
    if (this.batch != null && ((Component) this).transform.hasChanged)
    {
      ((Component) this).transform.hasChanged = false;
      if (this.batch != null && this.batch.group.maxGroupSize == 1 && (double) this.lastPos.z != (double) TransformExtensions.GetPosition(((Component) this).transform).z)
        this.batch.OverrideZ(TransformExtensions.GetPosition(((Component) this).transform).z);
      this.lastPos = this.PositionIncludingOffset;
      if (this.visibilityType != KAnimControllerBase.VisibilityType.Always && Vector2I.op_Inequality(KAnimBatchManager.ControllerToChunkXY((KAnimConverter.IAnimConverter) this), this.lastChunkXY) && Vector2I.op_Inequality(this.lastChunkXY, KBatchedAnimUpdater.INVALID_CHUNK_ID))
      {
        this.DeRegister();
        this.Register();
      }
      this.SetDirty();
    }
    if (HashedString.op_Equality(this.batchGroupID, KAnimBatchManager.NO_BATCH) || !this.IsActive())
      return;
    if (!this.forceRebuild && (this.mode == 2 || this.stopped || this.curAnim == null || this.mode == 1 && this.curAnim != null && ((double) this.elapsedTime > (double) this.curAnim.totalTime || (double) this.curAnim.totalTime <= 0.0) && this.animQueue.Count == 0))
      this.SuspendUpdates(true);
    if (!this.isVisible && !this.forceRebuild)
    {
      if (this.visibilityType != KAnimControllerBase.VisibilityType.OffscreenUpdate || this.stopped || this.mode == 2)
        return;
      this.SetElapsedTime(this.elapsedTime + dt * this.playSpeed);
    }
    else
    {
      this.curAnimFrameIdx = this.GetFrameIdx(this.elapsedTime, true);
      if (this.eventManagerHandle.IsValid() && this.aem != null && (int) (((double) this.elapsedTime - (double) this.aem.GetElapsedTime(this.eventManagerHandle)) * 100.0) != 0)
        this.UpdateAnimEventSequenceTime();
      this.UpdateFrame(this.elapsedTime);
      if (!this.stopped && this.mode != 2)
        this.SetElapsedTime(this.elapsedTime + dt * this.playSpeed);
      this.forceRebuild = false;
    }
  }

  protected override void UpdateFrame(float t)
  {
    this.previousFrame = this.currentFrame;
    if (!this.stopped || this.forceRebuild)
    {
      if (this.curAnim != null && (this.mode == null || (double) this.elapsedTime <= (double) this.GetDuration() || this.forceRebuild))
      {
        this.currentFrame = this.curAnim.GetFrameIdx(this.mode, this.elapsedTime);
        if (this.currentFrame != this.previousFrame || this.forceRebuild)
          this.SetDirty();
      }
      else
        this.TriggerStop();
      if (!this.stopped && this.mode == null && this.currentFrame == 0)
        this.AnimEnter(this.curAnim.hash);
    }
    if (this.synchronizer == null)
      return;
    this.synchronizer.SyncTime();
  }

  public override void TriggerStop()
  {
    if (this.animQueue.Count > 0)
    {
      this.StartQueuedAnim();
    }
    else
    {
      if (this.curAnim == null || this.mode != 1)
        return;
      this.currentFrame = this.curAnim.numFrames - 1;
      this.Stop();
      EventExtensions.Trigger(((Component) this).gameObject, -1061186183, (object) null);
      if (!this.destroyOnAnimComplete)
        return;
      this.DestroySelf();
    }
  }

  public override void UpdateHidden()
  {
    KBatchGroupData batchGroupData = KAnimBatchManager.instance.GetBatchGroupData(this.batchGroupID);
    for (int index = 0; index < batchGroupData.frameElementSymbols.Count; ++index)
    {
      bool flag = !this.hiddenSymbols.Contains(batchGroupData.frameElementSymbols[index].hash);
      this.symbolInstanceGpuData.SetVisible(index, flag);
    }
    this.SetDirty();
  }

  public int GetMaxVisible() => this.maxSymbols;

  public HashedString batchGroupID { get; private set; }

  public HashedString GetBatchGroupID(bool isEditorWindow = false)
  {
    int num;
    if (!isEditorWindow)
    {
      if (this.animFiles != null && this.animFiles.Length != 0)
      {
        HashedString batchGroupId = this.batchGroupID;
        num = !((HashedString) ref batchGroupId).IsValid ? 0 : (HashedString.op_Inequality(this.batchGroupID, KAnimBatchManager.NO_BATCH) ? 1 : 0);
      }
      else
        num = 1;
    }
    else
      num = 1;
    Debug.Assert(num != 0);
    return this.batchGroupID;
  }

  public int GetLayer() => ((Component) this).gameObject.layer;

  public KAnimBatch GetBatch() => this.batch;

  public void SetBatch(KAnimBatch new_batch)
  {
    this.batch = new_batch;
    if (this.materialType != 3)
      return;
    KBatchedAnimCanvasRenderer animCanvasRenderer = ((Component) this).GetComponent<KBatchedAnimCanvasRenderer>();
    if (Object.op_Equality((Object) animCanvasRenderer, (Object) null) && new_batch != null)
      animCanvasRenderer = ((Component) this).gameObject.AddComponent<KBatchedAnimCanvasRenderer>();
    if (!Object.op_Inequality((Object) animCanvasRenderer, (Object) null))
      return;
    animCanvasRenderer.SetBatch((KAnimConverter.IAnimConverter) this);
  }

  public int GetCurrentNumFrames() => this.curAnim == null ? 0 : this.curAnim.numFrames;

  public int GetFirstFrameIndex() => this.curAnim == null ? -1 : this.curAnim.firstFrameIdx;

  private Canvas GetRootCanvas()
  {
    if (Object.op_Equality((Object) this.rt, (Object) null))
      return (Canvas) null;
    for (RectTransform component1 = ((Component) ((Transform) this.rt).parent).GetComponent<RectTransform>(); Object.op_Inequality((Object) component1, (Object) null); component1 = ((Component) ((Transform) component1).parent).GetComponent<RectTransform>())
    {
      Canvas component2 = ((Component) component1).GetComponent<Canvas>();
      if (Object.op_Inequality((Object) component2, (Object) null) && component2.isRootCanvas)
        return component2;
    }
    return (Canvas) null;
  }

  public override Matrix2x3 GetTransformMatrix()
  {
    Vector3 vector3 = this.PositionIncludingOffset;
    vector3.z = 0.0f;
    Vector2 vector2;
    // ISSUE: explicit constructor call
    ((Vector2) ref vector2).\u002Ector(this.animScale * this.animWidth, -this.animScale * this.animHeight);
    if (this.materialType == 3)
    {
      this.rt = ((Component) this).GetComponent<RectTransform>();
      if (Object.op_Equality((Object) this.rootCanvas, (Object) null))
        this.rootCanvas = this.GetRootCanvas();
      if (Object.op_Equality((Object) this.scaler, (Object) null) && Object.op_Inequality((Object) this.rootCanvas, (Object) null))
        this.scaler = ((Component) this.rootCanvas).GetComponent<CanvasScaler>();
      Rect rect;
      if (Object.op_Equality((Object) this.rootCanvas, (Object) null))
      {
        this.screenOffset.x = (float) (Screen.width / 2);
        this.screenOffset.y = (float) (Screen.height / 2);
      }
      else
      {
        ref Vector3 local1 = ref this.screenOffset;
        double num1;
        if (this.rootCanvas.renderMode != 2)
        {
          rect = Util.rectTransform((Component) this.rootCanvas).rect;
          num1 = (double) ((Rect) ref rect).width / 2.0;
        }
        else
          num1 = 0.0;
        local1.x = (float) num1;
        ref Vector3 local2 = ref this.screenOffset;
        double num2;
        if (this.rootCanvas.renderMode != 2)
        {
          rect = Util.rectTransform((Component) this.rootCanvas).rect;
          num2 = (double) ((Rect) ref rect).height / 2.0;
        }
        else
          num2 = 0.0;
        local2.y = (float) num2;
      }
      float num3 = 1f;
      if (Object.op_Inequality((Object) this.scaler, (Object) null))
        num3 = 1f / this.scaler.scaleFactor;
      Matrix4x4 localToWorldMatrix = ((Transform) this.rt).localToWorldMatrix;
      vector3 = Vector3.op_Subtraction(Vector3.op_Multiply(Vector3.op_Addition(((Matrix4x4) ref localToWorldMatrix).MultiplyPoint(Vector2.op_Implicit(this.rt.pivot)), this.offset), num3), this.screenOffset);
      float num4 = this.animWidth * this.animScale;
      float num5 = this.animHeight * this.animScale;
      float num6;
      float num7;
      if (this.setScaleFromAnim && this.curAnim != null)
      {
        double num8 = (double) num4;
        rect = this.rt.rect;
        double num9 = (double) ((Rect) ref rect).size.x / (double) this.curAnim.unScaledSize.x;
        num6 = (float) (num8 * num9);
        double num10 = (double) num5;
        rect = this.rt.rect;
        double num11 = (double) ((Rect) ref rect).size.y / (double) this.curAnim.unScaledSize.y;
        num7 = (float) (num10 * num11);
      }
      else
      {
        double num12 = (double) num4;
        rect = this.rt.rect;
        double num13 = (double) ((Rect) ref rect).size.x / (double) this.animOverrideSize.x;
        num6 = (float) (num12 * num13);
        double num14 = (double) num5;
        rect = this.rt.rect;
        double num15 = (double) ((Rect) ref rect).size.y / (double) this.animOverrideSize.y;
        num7 = (float) (num14 * num15);
      }
      vector2 = Vector2.op_Implicit(new Vector3(((Transform) this.rt).lossyScale.x * num6 * num3, -((Transform) this.rt).lossyScale.y * num7 * num3, ((Transform) this.rt).lossyScale.z * num3));
      this.pivot = Vector2.op_Implicit(this.rt.pivot);
    }
    Matrix2x3 matrix2x3_1 = Matrix2x3.Scale(vector2);
    Matrix2x3 matrix2x3_2 = Matrix2x3.Scale(new Vector2(this.flipX ? -1f : 1f, this.flipY ? -1f : 1f));
    Matrix2x3 transformMatrix;
    if ((double) this.rotation != 0.0)
    {
      Matrix2x3 matrix2x3_3 = Matrix2x3.Translate(Vector2.op_Implicit(Vector3.op_UnaryNegation(this.pivot)));
      Matrix2x3 matrix2x3_4 = Matrix2x3.Rotate(this.rotation * ((float) Math.PI / 180f));
      Matrix2x3 matrix2x3_5 = Matrix2x3.op_Multiply(Matrix2x3.op_Multiply(Matrix2x3.Translate(Vector2.op_Implicit(this.pivot)), matrix2x3_4), matrix2x3_3);
      transformMatrix = Matrix2x3.op_Multiply(Matrix2x3.op_Multiply(Matrix2x3.op_Multiply(Matrix2x3.op_Multiply(Matrix2x3.TRS(Vector2.op_Implicit(vector3), ((Component) this).transform.rotation, Vector2.op_Implicit(((Component) this).transform.localScale)), matrix2x3_5), matrix2x3_1), this.navMatrix), matrix2x3_2);
    }
    else
      transformMatrix = Matrix2x3.op_Multiply(Matrix2x3.op_Multiply(Matrix2x3.op_Multiply(Matrix2x3.TRS(Vector2.op_Implicit(vector3), ((Component) this).transform.rotation, Vector2.op_Implicit(((Component) this).transform.localScale)), matrix2x3_1), this.navMatrix), matrix2x3_2);
    return transformMatrix;
  }

  public override Matrix4x4 GetSymbolTransform(HashedString symbol, out bool symbolVisible)
  {
    if (this.curAnimFrameIdx != -1 && this.batch != null)
    {
      Matrix2x3 symbolLocalTransform = this.GetSymbolLocalTransform(symbol, out symbolVisible);
      if (symbolVisible)
        return Matrix4x4.op_Multiply(Matrix2x3.op_Implicit(this.GetTransformMatrix()), Matrix2x3.op_Implicit(symbolLocalTransform));
    }
    symbolVisible = false;
    return new Matrix4x4();
  }

  public override Matrix2x3 GetSymbolLocalTransform(HashedString symbol, out bool symbolVisible)
  {
    if (this.curAnimFrameIdx != -1 && this.batch != null)
    {
      KAnim.Anim.Frame frame = this.batch.group.data.GetFrame(this.curAnimFrameIdx);
      if (KAnim.Anim.Frame.op_Inequality(frame, KAnim.Anim.Frame.InvalidFrame))
      {
        for (int index1 = 0; index1 < frame.numElements; ++index1)
        {
          int index2 = frame.firstElementIdx + index1;
          if (index2 < this.batch.group.data.frameElements.Count)
          {
            KAnim.Anim.FrameElement frameElement = this.batch.group.data.frameElements[index2];
            if (KAnimHashedString.op_Equality(frameElement.symbol, symbol))
            {
              symbolVisible = true;
              return frameElement.transform;
            }
          }
        }
      }
    }
    symbolVisible = false;
    return Matrix2x3.identity;
  }

  public override void SetLayer(int layer)
  {
    if (layer == ((Component) this).gameObject.layer)
      return;
    base.SetLayer(layer);
    this.DeRegister();
    ((Component) this).gameObject.layer = layer;
    this.Register();
  }

  public override void SetDirty()
  {
    if (this.batch == null)
      return;
    this.batch.SetDirty((KAnimConverter.IAnimConverter) this);
  }

  protected override void OnStartQueuedAnim() => this.SuspendUpdates(false);

  protected override void OnAwake()
  {
    this.LoadAnims();
    if (this.visibilityType == KAnimControllerBase.VisibilityType.Default)
      this.visibilityType = this.materialType == 3 ? KAnimControllerBase.VisibilityType.Always : this.visibilityType;
    if (this.materialType == null && HashedString.op_Equality(this.batchGroupID, KAnimBatchManager.BATCH_HUMAN))
      this.materialType = (KAnimBatchGroup.MaterialType) 5;
    this.symbolOverrideController = ((Component) this).GetComponent<SymbolOverrideController>();
    this.UpdateHidden();
    this.hasEnableRun = false;
  }

  protected override void OnStart()
  {
    if (this.batch == null)
      this.Initialize();
    if (this.visibilityType == KAnimControllerBase.VisibilityType.Always || this.visibilityType == KAnimControllerBase.VisibilityType.OffscreenUpdate)
      this.ConfigureUpdateListener();
    CellChangeMonitor instance = Singleton<CellChangeMonitor>.Instance;
    if (instance != null)
    {
      instance.RegisterMovementStateChanged(((Component) this).transform, new Action<Transform, bool>(KBatchedAnimController.OnMovementStateChanged));
      this.moving = instance.IsMoving(((Component) this).transform);
    }
    this.symbolOverrideController = ((Component) this).GetComponent<SymbolOverrideController>();
    this.SetDirty();
  }

  protected override void OnStop() => this.SetDirty();

  private void OnEnable()
  {
    if (!this._enabled)
      return;
    this.Enable();
  }

  protected override void Enable()
  {
    if (this.hasEnableRun)
      return;
    this.hasEnableRun = true;
    if (this.batch == null)
      this.Initialize();
    this.SetDirty();
    this.SuspendUpdates(false);
    this.ConfigureVisibilityListener(true);
    if (this.stopped || this.curAnim == null || this.mode == 2 || this.eventManagerHandle.IsValid())
      return;
    this.StartAnimEventSequence();
  }

  private void OnDisable() => this.Disable();

  protected override void Disable()
  {
    if (App.IsExiting || KMonoBehaviour.isLoadingScene || !this.hasEnableRun)
      return;
    this.hasEnableRun = false;
    this.SuspendUpdates(true);
    if (this.batch != null)
      this.DeRegister();
    this.ConfigureVisibilityListener(false);
    this.StopAnimEventSequence();
  }

  protected override void OnDestroy()
  {
    if (App.IsExiting)
      return;
    Singleton<CellChangeMonitor>.Instance?.UnregisterMovementStateChanged(((Component) this).transform, new Action<Transform, bool>(KBatchedAnimController.OnMovementStateChanged));
    Singleton<KBatchedAnimUpdater>.Instance?.UpdateUnregister(this);
    this.isVisible = false;
    this.DeRegister();
    this.stopped = true;
    this.StopAnimEventSequence();
    this.batchInstanceData = (KBatchedAnimInstanceData) null;
    this.batch = (KAnimBatch) null;
    base.OnDestroy();
  }

  public void SetBlendValue(float value)
  {
    this.batchInstanceData.SetBlend(value);
    this.SetDirty();
  }

  public bool ApplySymbolOverrides()
  {
    this.batch.atlases.Apply(this.batch.matProperties);
    if (!Object.op_Inequality((Object) this.symbolOverrideController, (Object) null))
      return false;
    if (this.symbolOverrideControllerVersion != this.symbolOverrideController.version || this.symbolOverrideController.applySymbolOverridesEveryFrame)
    {
      this.symbolOverrideControllerVersion = this.symbolOverrideController.version;
      this.symbolOverrideController.ApplyOverrides();
    }
    this.symbolOverrideController.ApplyAtlases();
    return true;
  }

  public void SetSymbolOverrides(
    int symbol_start_idx,
    int symbol_num_frames,
    int atlas_idx,
    KBatchGroupData source_data,
    int source_start_idx,
    int source_num_frames)
  {
    this.symbolOverrideInfoGpuData.SetSymbolOverrideInfo(symbol_start_idx, symbol_num_frames, atlas_idx, source_data, source_start_idx, source_num_frames);
  }

  public void SetSymbolOverride(
    int symbol_idx,
    ref KAnim.Build.SymbolFrameInstance symbol_frame_instance)
  {
    this.symbolOverrideInfoGpuData.SetSymbolOverrideInfo(symbol_idx, ref symbol_frame_instance);
  }

  protected override void Register()
  {
    if (!this.IsActive() || this.batch != null)
      return;
    HashedString batchGroupId = this.batchGroupID;
    if (!((HashedString) ref batchGroupId).IsValid || !HashedString.op_Inequality(this.batchGroupID, KAnimBatchManager.NO_BATCH))
      return;
    this.lastChunkXY = KAnimBatchManager.ControllerToChunkXY((KAnimConverter.IAnimConverter) this);
    KAnimBatchManager.Instance().Register((KAnimConverter.IAnimConverter) this);
    this.forceRebuild = true;
    this.SetDirty();
  }

  protected override void DeRegister()
  {
    if (this.batch == null)
      return;
    this.batch.Deregister((KAnimConverter.IAnimConverter) this);
  }

  private void ConfigureUpdateListener()
  {
    if ((this.IsActive() && !this.suspendUpdates && this.isVisible || this.moving || this.visibilityType == KAnimControllerBase.VisibilityType.OffscreenUpdate ? 1 : (this.visibilityType == KAnimControllerBase.VisibilityType.Always ? 1 : 0)) != 0)
      Singleton<KBatchedAnimUpdater>.Instance.UpdateRegister(this);
    else
      Singleton<KBatchedAnimUpdater>.Instance.UpdateUnregister(this);
  }

  protected override void SuspendUpdates(bool suspend)
  {
    this.suspendUpdates = suspend;
    this.ConfigureUpdateListener();
  }

  public void SetVisiblity(bool is_visible)
  {
    if (is_visible == this.isVisible)
      return;
    this.isVisible = is_visible;
    if (is_visible)
    {
      this.SuspendUpdates(false);
      this.SetDirty();
      this.UpdateAnimEventSequenceTime();
    }
    else
    {
      this.SuspendUpdates(true);
      this.SetDirty();
    }
  }

  private void ConfigureVisibilityListener(bool enabled)
  {
    if (this.visibilityType == KAnimControllerBase.VisibilityType.Always || this.visibilityType == KAnimControllerBase.VisibilityType.OffscreenUpdate)
      return;
    if (enabled)
      this.RegisterVisibilityListener();
    else
      this.UnregisterVisibilityListener();
  }

  protected override void RefreshVisibilityListener()
  {
    if (!this.visibilityListenerRegistered)
      return;
    this.ConfigureVisibilityListener(false);
    this.ConfigureVisibilityListener(true);
  }

  private void RegisterVisibilityListener()
  {
    DebugUtil.Assert(!this.visibilityListenerRegistered);
    Singleton<KBatchedAnimUpdater>.Instance.VisibilityRegister(this);
    this.visibilityListenerRegistered = true;
  }

  private void UnregisterVisibilityListener()
  {
    DebugUtil.Assert(this.visibilityListenerRegistered);
    Singleton<KBatchedAnimUpdater>.Instance.VisibilityUnregister(this);
    this.visibilityListenerRegistered = false;
  }

  public void SetSceneLayer(Grid.SceneLayer layer)
  {
    float layerZ = Grid.GetLayerZ(layer);
    this.sceneLayer = layer;
    Vector3 position = TransformExtensions.GetPosition(((Component) this).transform);
    position.z = layerZ;
    TransformExtensions.SetPosition(((Component) this).transform, position);
    this.DeRegister();
    this.Register();
  }
}
