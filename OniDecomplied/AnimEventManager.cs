// Decompiled with JetBrains decompiler
// Type: AnimEventManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AnimEventManager : Singleton<AnimEventManager>
{
  private static readonly List<AnimEvent> emptyEventList = new List<AnimEvent>();
  private const int INITIAL_VECTOR_SIZE = 256;
  private KCompactedVector<AnimEventManager.AnimData> animData = new KCompactedVector<AnimEventManager.AnimData>(256);
  private KCompactedVector<AnimEventManager.EventPlayerData> eventData = new KCompactedVector<AnimEventManager.EventPlayerData>(256);
  private KCompactedVector<AnimEventManager.AnimData> uiAnimData = new KCompactedVector<AnimEventManager.AnimData>(256);
  private KCompactedVector<AnimEventManager.EventPlayerData> uiEventData = new KCompactedVector<AnimEventManager.EventPlayerData>(256);
  private KCompactedVector<AnimEventManager.IndirectionData> indirectionData = new KCompactedVector<AnimEventManager.IndirectionData>(0);
  private List<KBatchedAnimController> finishedCalls = new List<KBatchedAnimController>();

  public void FreeResources()
  {
  }

  public HandleVector<int>.Handle PlayAnim(
    KAnimControllerBase controller,
    KAnim.Anim anim,
    KAnim.PlayMode mode,
    float time,
    bool use_unscaled_time)
  {
    AnimEventManager.AnimData animData = new AnimEventManager.AnimData();
    animData.frameRate = anim.frameRate;
    animData.totalTime = anim.totalTime;
    animData.numFrames = anim.numFrames;
    animData.useUnscaledTime = use_unscaled_time;
    AnimEventManager.EventPlayerData eventPlayerData = new AnimEventManager.EventPlayerData()
    {
      elapsedTime = time,
      mode = mode,
      controller = controller as KBatchedAnimController
    };
    eventPlayerData.currentFrame = eventPlayerData.controller.GetFrameIdx(eventPlayerData.elapsedTime, false);
    eventPlayerData.previousFrame = -1;
    eventPlayerData.events = (List<AnimEvent>) null;
    eventPlayerData.updatingEvents = (List<AnimEvent>) null;
    eventPlayerData.events = GameAudioSheets.Get().GetEvents(anim.id);
    if (eventPlayerData.events == null)
      eventPlayerData.events = AnimEventManager.emptyEventList;
    return !animData.useUnscaledTime ? this.indirectionData.Allocate(new AnimEventManager.IndirectionData(this.animData.Allocate(animData), this.eventData.Allocate(eventPlayerData), false)) : this.indirectionData.Allocate(new AnimEventManager.IndirectionData(this.uiAnimData.Allocate(animData), this.uiEventData.Allocate(eventPlayerData), true));
  }

  public void SetMode(HandleVector<int>.Handle handle, KAnim.PlayMode mode)
  {
    if (!handle.IsValid())
      return;
    AnimEventManager.IndirectionData data1 = this.indirectionData.GetData(handle);
    KCompactedVector<AnimEventManager.EventPlayerData> kcompactedVector = data1.isUIData ? this.uiEventData : this.eventData;
    AnimEventManager.EventPlayerData data2 = kcompactedVector.GetData(data1.eventDataHandle) with
    {
      mode = mode
    };
    kcompactedVector.SetData(data1.eventDataHandle, data2);
  }

  public void StopAnim(HandleVector<int>.Handle handle)
  {
    if (!handle.IsValid())
      return;
    AnimEventManager.IndirectionData data = this.indirectionData.GetData(handle);
    KCompactedVector<AnimEventManager.AnimData> kcompactedVector1 = data.isUIData ? this.uiAnimData : this.animData;
    KCompactedVector<AnimEventManager.EventPlayerData> kcompactedVector2 = data.isUIData ? this.uiEventData : this.eventData;
    this.StopEvents(kcompactedVector2.GetData(data.eventDataHandle));
    kcompactedVector1.Free(data.animDataHandle);
    kcompactedVector2.Free(data.eventDataHandle);
    this.indirectionData.Free(handle);
  }

  public float GetElapsedTime(HandleVector<int>.Handle handle)
  {
    AnimEventManager.IndirectionData data = this.indirectionData.GetData(handle);
    return (data.isUIData ? this.uiEventData : this.eventData).GetData(data.eventDataHandle).elapsedTime;
  }

  public void SetElapsedTime(HandleVector<int>.Handle handle, float elapsed_time)
  {
    AnimEventManager.IndirectionData data1 = this.indirectionData.GetData(handle);
    KCompactedVector<AnimEventManager.EventPlayerData> kcompactedVector = data1.isUIData ? this.uiEventData : this.eventData;
    AnimEventManager.EventPlayerData data2 = kcompactedVector.GetData(data1.eventDataHandle) with
    {
      elapsedTime = elapsed_time
    };
    kcompactedVector.SetData(data1.eventDataHandle, data2);
  }

  public void Update()
  {
    float deltaTime = Time.deltaTime;
    float unscaledDeltaTime = Time.unscaledDeltaTime;
    this.Update(deltaTime, this.animData.GetDataList(), this.eventData.GetDataList());
    this.Update(unscaledDeltaTime, this.uiAnimData.GetDataList(), this.uiEventData.GetDataList());
    for (int index = 0; index < this.finishedCalls.Count; ++index)
      this.finishedCalls[index].TriggerStop();
    this.finishedCalls.Clear();
  }

  private void Update(
    float dt,
    List<AnimEventManager.AnimData> anim_data,
    List<AnimEventManager.EventPlayerData> event_data)
  {
    if ((double) dt <= 0.0)
      return;
    for (int index1 = 0; index1 < event_data.Count; ++index1)
    {
      AnimEventManager.EventPlayerData eventPlayerData = event_data[index1];
      if (!Object.op_Equality((Object) eventPlayerData.controller, (Object) null) && eventPlayerData.mode != 2)
      {
        eventPlayerData.currentFrame = eventPlayerData.controller.GetFrameIdx(eventPlayerData.elapsedTime, false);
        event_data[index1] = eventPlayerData;
        this.PlayEvents(eventPlayerData);
        eventPlayerData.previousFrame = eventPlayerData.currentFrame;
        eventPlayerData.elapsedTime += dt * eventPlayerData.controller.GetPlaySpeed();
        event_data[index1] = eventPlayerData;
        if (eventPlayerData.updatingEvents != null)
        {
          for (int index2 = 0; index2 < eventPlayerData.updatingEvents.Count; ++index2)
            eventPlayerData.updatingEvents[index2].OnUpdate(eventPlayerData);
        }
        event_data[index1] = eventPlayerData;
        if (eventPlayerData.mode != null && eventPlayerData.currentFrame >= anim_data[index1].numFrames - 1)
        {
          this.StopEvents(eventPlayerData);
          this.finishedCalls.Add(eventPlayerData.controller);
        }
      }
    }
  }

  private void PlayEvents(AnimEventManager.EventPlayerData data)
  {
    for (int index = 0; index < data.events.Count; ++index)
      data.events[index].Play(data);
  }

  private void StopEvents(AnimEventManager.EventPlayerData data)
  {
    for (int index = 0; index < data.events.Count; ++index)
      data.events[index].Stop(data);
    if (data.updatingEvents == null)
      return;
    data.updatingEvents.Clear();
  }

  public AnimEventManager.DevTools_DebugInfo DevTools_GetDebugInfo() => new AnimEventManager.DevTools_DebugInfo(this, this.animData, this.eventData, this.uiAnimData, this.uiEventData);

  public struct AnimData
  {
    public float frameRate;
    public float totalTime;
    public int numFrames;
    public bool useUnscaledTime;
  }

  [DebuggerDisplay("{controller.name}, Anim={currentAnim}, Frame={currentFrame}, Mode={mode}")]
  public struct EventPlayerData
  {
    public float elapsedTime;
    public KAnim.PlayMode mode;
    public List<AnimEvent> events;
    public List<AnimEvent> updatingEvents;
    public KBatchedAnimController controller;

    public int currentFrame { get; set; }

    public int previousFrame { get; set; }

    public ComponentType GetComponent<ComponentType>() => ((Component) this.controller).GetComponent<ComponentType>();

    public string name => ((Object) this.controller).name;

    public float normalizedTime => this.elapsedTime / this.controller.CurrentAnim.totalTime;

    public Vector3 position => TransformExtensions.GetPosition(((Component) this.controller).transform);

    public void AddUpdatingEvent(AnimEvent ev)
    {
      if (this.updatingEvents == null)
        this.updatingEvents = new List<AnimEvent>();
      this.updatingEvents.Add(ev);
    }

    public void SetElapsedTime(float elapsedTime) => this.elapsedTime = elapsedTime;

    public void FreeResources()
    {
      this.elapsedTime = 0.0f;
      this.mode = (KAnim.PlayMode) 1;
      this.currentFrame = 0;
      this.previousFrame = 0;
      this.events = (List<AnimEvent>) null;
      this.updatingEvents = (List<AnimEvent>) null;
      this.controller = (KBatchedAnimController) null;
    }
  }

  private struct IndirectionData
  {
    public bool isUIData;
    public HandleVector<int>.Handle animDataHandle;
    public HandleVector<int>.Handle eventDataHandle;

    public IndirectionData(
      HandleVector<int>.Handle anim_data_handle,
      HandleVector<int>.Handle event_data_handle,
      bool is_ui_data)
    {
      this.isUIData = is_ui_data;
      this.animDataHandle = anim_data_handle;
      this.eventDataHandle = event_data_handle;
    }
  }

  public readonly struct DevTools_DebugInfo
  {
    public readonly AnimEventManager eventManager;
    public readonly KCompactedVector<AnimEventManager.AnimData> animData;
    public readonly KCompactedVector<AnimEventManager.EventPlayerData> eventData;
    public readonly KCompactedVector<AnimEventManager.AnimData> uiAnimData;
    public readonly KCompactedVector<AnimEventManager.EventPlayerData> uiEventData;

    public DevTools_DebugInfo(
      AnimEventManager eventManager,
      KCompactedVector<AnimEventManager.AnimData> animData,
      KCompactedVector<AnimEventManager.EventPlayerData> eventData,
      KCompactedVector<AnimEventManager.AnimData> uiAnimData,
      KCompactedVector<AnimEventManager.EventPlayerData> uiEventData)
    {
      this.eventManager = eventManager;
      this.animData = animData;
      this.eventData = eventData;
      this.uiAnimData = uiAnimData;
      this.uiEventData = uiEventData;
    }
  }
}
