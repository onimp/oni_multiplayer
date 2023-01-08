// Decompiled with JetBrains decompiler
// Type: ClusterMapVisualizer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ClusterMapVisualizer : KMonoBehaviour
{
  public KBatchedAnimController animControllerPrefab;
  public KBatchedAnimController peekControllerPrefab;
  public Transform nameTarget;
  public AlertVignette alertVignette;
  public bool doesTransitionAnimation;
  [HideInInspector]
  public Transform animContainer;
  private ClusterGridEntity entity;
  private ClusterMapPathDrawer pathDrawer;
  private ClusterMapPath mapPath;
  private List<KBatchedAnimController> animControllers;
  private bool isSelected;
  private ClusterRevealLevel lastRevealLevel;

  public void Init(ClusterGridEntity entity, ClusterMapPathDrawer pathDrawer)
  {
    this.entity = entity;
    this.pathDrawer = pathDrawer;
    this.animControllers = new List<KBatchedAnimController>();
    if (Object.op_Equality((Object) this.animContainer, (Object) null))
    {
      GameObject gameObject = new GameObject("AnimContainer", new System.Type[1]
      {
        typeof (RectTransform)
      });
      RectTransform component1 = ((Component) this).GetComponent<RectTransform>();
      RectTransform component2 = gameObject.GetComponent<RectTransform>();
      ((Transform) component2).SetParent((Transform) component1, false);
      TransformExtensions.SetLocalPosition((Transform) component2, new Vector3(0.0f, 0.0f, 0.0f));
      component2.sizeDelta = component1.sizeDelta;
      ((Transform) component2).localScale = Vector3.one;
      this.animContainer = (Transform) component2;
    }
    Vector3 position = ClusterGrid.Instance.GetPosition(entity);
    TransformExtensions.SetLocalPosition((Transform) Util.rectTransform((Component) this), position);
    this.RefreshPathDrawing();
    entity.Subscribe(543433792, new Action<object>(this.OnClusterDestinationChanged));
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    if (!this.doesTransitionAnimation)
      return;
    new ClusterMapTravelAnimator.StatesInstance(this, this.entity).StartSM();
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (!Object.op_Inequality((Object) this.entity, (Object) null))
      return;
    if (this.entity is Clustercraft)
      new ClusterMapRocketAnimator.StatesInstance(this, this.entity).StartSM();
    else if (this.entity is BallisticClusterGridEntity)
    {
      new ClusterMapBallisticAnimator.StatesInstance(this, this.entity).StartSM();
    }
    else
    {
      if (this.entity.Layer != EntityLayer.FX)
        return;
      new ClusterMapFXAnimator.StatesInstance(this, this.entity).StartSM();
    }
  }

  protected virtual void OnCleanUp()
  {
    if (Object.op_Inequality((Object) this.entity, (Object) null))
      this.entity.Unsubscribe(543433792, new Action<object>(this.OnClusterDestinationChanged));
    base.OnCleanUp();
  }

  private void OnClusterDestinationChanged(object data) => this.RefreshPathDrawing();

  public void Select(bool selected)
  {
    if (this.animControllers == null || this.animControllers.Count == 0)
      return;
    if (!selected == this.isSelected)
    {
      this.isSelected = selected;
      this.RefreshPathDrawing();
    }
    this.GetFirstAnimController().SetSymbolVisiblity(KAnimHashedString.op_Implicit(nameof (selected)), selected);
  }

  public void PlayAnim(string animName, KAnim.PlayMode playMode) => this.GetFirstAnimController().Play(HashedString.op_Implicit(animName), playMode);

  public KBatchedAnimController GetFirstAnimController() => this.animControllers[0];

  public void Show(ClusterRevealLevel level)
  {
    if (!this.entity.IsVisible)
      level = ClusterRevealLevel.Hidden;
    if (level == this.lastRevealLevel)
      return;
    this.lastRevealLevel = level;
    switch (level)
    {
      case ClusterRevealLevel.Hidden:
        ((Component) this).gameObject.SetActive(false);
        break;
      case ClusterRevealLevel.Peeked:
        this.ClearAnimControllers();
        KBatchedAnimController kbatchedAnimController1 = Object.Instantiate<KBatchedAnimController>(this.peekControllerPrefab, this.animContainer);
        ((Component) kbatchedAnimController1).gameObject.SetActive(true);
        this.animControllers.Add(kbatchedAnimController1);
        ((Component) this).gameObject.SetActive(true);
        break;
      case ClusterRevealLevel.Visible:
        this.ClearAnimControllers();
        if (Object.op_Inequality((Object) this.animControllerPrefab, (Object) null) && this.entity.AnimConfigs != null)
        {
          foreach (ClusterGridEntity.AnimConfig animConfig in this.entity.AnimConfigs)
          {
            KBatchedAnimController kbatchedAnimController2 = Object.Instantiate<KBatchedAnimController>(this.animControllerPrefab, this.animContainer);
            kbatchedAnimController2.AnimFiles = new KAnimFile[1]
            {
              animConfig.animFile
            };
            kbatchedAnimController2.initialMode = animConfig.playMode;
            kbatchedAnimController2.initialAnim = animConfig.initialAnim;
            kbatchedAnimController2.Offset = animConfig.animOffset;
            ((Component) kbatchedAnimController2).gameObject.AddComponent<LoopingSounds>();
            if (!string.IsNullOrEmpty(animConfig.symbolSwapTarget) && !string.IsNullOrEmpty(animConfig.symbolSwapSymbol))
            {
              SymbolOverrideController component = ((Component) kbatchedAnimController2).GetComponent<SymbolOverrideController>();
              KAnim.Build.Symbol symbol = kbatchedAnimController2.AnimFiles[0].GetData().build.GetSymbol(KAnimHashedString.op_Implicit(animConfig.symbolSwapSymbol));
              HashedString target_symbol = HashedString.op_Implicit(animConfig.symbolSwapTarget);
              KAnim.Build.Symbol source_symbol = symbol;
              component.AddSymbolOverride(target_symbol, source_symbol);
            }
            ((Component) kbatchedAnimController2).gameObject.SetActive(true);
            this.animControllers.Add(kbatchedAnimController2);
          }
        }
        ((Component) this).gameObject.SetActive(true);
        break;
    }
  }

  public void RefreshPathDrawing()
  {
    if (Object.op_Equality((Object) this.entity, (Object) null))
      return;
    ClusterTraveler component = ((Component) this.entity).GetComponent<ClusterTraveler>();
    if (Object.op_Equality((Object) component, (Object) null))
      return;
    List<AxialI> pathPoints = !this.entity.IsVisible || !component.IsTraveling() ? (List<AxialI>) null : component.CurrentPath;
    if (pathPoints != null && pathPoints.Count > 0)
    {
      if (Object.op_Equality((Object) this.mapPath, (Object) null))
        this.mapPath = this.pathDrawer.AddPath();
      this.mapPath.SetPoints(ClusterMapPathDrawer.GetDrawPathList(Vector2.op_Implicit(TransformExtensions.GetLocalPosition(this.transform)), pathPoints));
      Color color;
      if (this.isSelected)
        color = ClusterMapScreen.Instance.rocketSelectedPathColor;
      else if (this.entity.ShowPath())
      {
        color = ClusterMapScreen.Instance.rocketPathColor;
      }
      else
      {
        // ISSUE: explicit constructor call
        ((Color) ref color).\u002Ector(0.0f, 0.0f, 0.0f, 0.0f);
      }
      this.mapPath.SetColor(color);
    }
    else
    {
      if (!Object.op_Inequality((Object) this.mapPath, (Object) null))
        return;
      Util.KDestroyGameObject((Component) this.mapPath);
      this.mapPath = (ClusterMapPath) null;
    }
  }

  public void SetAnimRotation(float rotation) => this.animContainer.localRotation = Quaternion.Euler(0.0f, 0.0f, rotation);

  public float GetPathAngle() => Object.op_Equality((Object) this.mapPath, (Object) null) ? 0.0f : this.mapPath.GetRotationForNextSegment();

  private void ClearAnimControllers()
  {
    if (this.animControllers == null)
      return;
    foreach (Component animController in this.animControllers)
      Util.KDestroyGameObject(animController.gameObject);
    this.animControllers.Clear();
  }

  private class UpdateXPositionParameter : LoopingSoundParameterUpdater
  {
    private List<ClusterMapVisualizer.UpdateXPositionParameter.Entry> entries = new List<ClusterMapVisualizer.UpdateXPositionParameter.Entry>();

    public UpdateXPositionParameter()
      : base(HashedString.op_Implicit("Starmap_Position_X"))
    {
    }

    public override void Add(LoopingSoundParameterUpdater.Sound sound) => this.entries.Add(new ClusterMapVisualizer.UpdateXPositionParameter.Entry()
    {
      transform = sound.transform,
      ev = sound.ev,
      parameterId = ((SoundDescription) ref sound.description).GetParameterId(this.parameter)
    });

    public override void Update(float dt)
    {
      foreach (ClusterMapVisualizer.UpdateXPositionParameter.Entry entry in this.entries)
      {
        if (!Object.op_Equality((Object) entry.transform, (Object) null))
        {
          EventInstance ev = entry.ev;
          ((EventInstance) ref ev).setParameterByID(entry.parameterId, TransformExtensions.GetPosition(entry.transform).x / (float) Screen.width, false);
        }
      }
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

    private struct Entry
    {
      public Transform transform;
      public EventInstance ev;
      public PARAMETER_ID parameterId;
    }
  }

  private class UpdateYPositionParameter : LoopingSoundParameterUpdater
  {
    private List<ClusterMapVisualizer.UpdateYPositionParameter.Entry> entries = new List<ClusterMapVisualizer.UpdateYPositionParameter.Entry>();

    public UpdateYPositionParameter()
      : base(HashedString.op_Implicit("Starmap_Position_Y"))
    {
    }

    public override void Add(LoopingSoundParameterUpdater.Sound sound) => this.entries.Add(new ClusterMapVisualizer.UpdateYPositionParameter.Entry()
    {
      transform = sound.transform,
      ev = sound.ev,
      parameterId = ((SoundDescription) ref sound.description).GetParameterId(this.parameter)
    });

    public override void Update(float dt)
    {
      foreach (ClusterMapVisualizer.UpdateYPositionParameter.Entry entry in this.entries)
      {
        if (!Object.op_Equality((Object) entry.transform, (Object) null))
        {
          EventInstance ev = entry.ev;
          ((EventInstance) ref ev).setParameterByID(entry.parameterId, TransformExtensions.GetPosition(entry.transform).y / (float) Screen.height, false);
        }
      }
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

    private struct Entry
    {
      public Transform transform;
      public EventInstance ev;
      public PARAMETER_ID parameterId;
    }
  }

  private class UpdateZoomPercentageParameter : LoopingSoundParameterUpdater
  {
    private List<ClusterMapVisualizer.UpdateZoomPercentageParameter.Entry> entries = new List<ClusterMapVisualizer.UpdateZoomPercentageParameter.Entry>();

    public UpdateZoomPercentageParameter()
      : base(HashedString.op_Implicit("Starmap_Zoom_Percentage"))
    {
    }

    public override void Add(LoopingSoundParameterUpdater.Sound sound) => this.entries.Add(new ClusterMapVisualizer.UpdateZoomPercentageParameter.Entry()
    {
      ev = sound.ev,
      parameterId = ((SoundDescription) ref sound.description).GetParameterId(this.parameter)
    });

    public override void Update(float dt)
    {
      foreach (ClusterMapVisualizer.UpdateZoomPercentageParameter.Entry entry in this.entries)
      {
        EventInstance ev = entry.ev;
        ((EventInstance) ref ev).setParameterByID(entry.parameterId, ClusterMapScreen.Instance.CurrentZoomPercentage(), false);
      }
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

    private struct Entry
    {
      public Transform transform;
      public EventInstance ev;
      public PARAMETER_ID parameterId;
    }
  }
}
