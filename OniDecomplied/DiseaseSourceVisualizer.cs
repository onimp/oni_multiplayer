// Decompiled with JetBrains decompiler
// Type: DiseaseSourceVisualizer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/DiseaseSourceVisualizer")]
public class DiseaseSourceVisualizer : KMonoBehaviour
{
  [SerializeField]
  private Vector3 offset;
  private GameObject visualizer;
  private bool visible;
  public string alwaysShowDisease;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.UpdateVisibility();
    Components.DiseaseSourceVisualizers.Add(this);
  }

  protected virtual void OnCleanUp()
  {
    OverlayScreen.Instance.OnOverlayChanged -= new Action<HashedString>(this.OnViewModeChanged);
    base.OnCleanUp();
    Components.DiseaseSourceVisualizers.Remove(this);
    if (!Object.op_Inequality((Object) this.visualizer, (Object) null))
      return;
    Object.Destroy((Object) this.visualizer);
    this.visualizer = (GameObject) null;
  }

  private void CreateVisualizer()
  {
    if (Object.op_Inequality((Object) this.visualizer, (Object) null) || Object.op_Equality((Object) GameScreenManager.Instance.worldSpaceCanvas, (Object) null))
      return;
    this.visualizer = Util.KInstantiate(Assets.UIPrefabs.ResourceVisualizer, GameScreenManager.Instance.worldSpaceCanvas, (string) null);
  }

  public void UpdateVisibility()
  {
    this.CreateVisualizer();
    if (string.IsNullOrEmpty(this.alwaysShowDisease))
    {
      this.visible = false;
    }
    else
    {
      Klei.AI.Disease disease = Db.Get().Diseases.Get(this.alwaysShowDisease);
      if (disease != null)
        this.SetVisibleDisease(disease);
    }
    if (!Object.op_Inequality((Object) OverlayScreen.Instance, (Object) null))
      return;
    this.Show(OverlayScreen.Instance.GetMode());
  }

  private void SetVisibleDisease(Klei.AI.Disease disease)
  {
    Sprite overlaySprite = Assets.instance.DiseaseVisualization.overlaySprite;
    Color32 colorByName = GlobalAssets.Instance.colorSet.GetColorByName(disease.overlayColourName);
    Image component = ((Component) this.visualizer.transform.GetChild(0)).GetComponent<Image>();
    component.sprite = overlaySprite;
    ((Graphic) component).color = Color32.op_Implicit(colorByName);
    this.visible = true;
  }

  private void Update()
  {
    if (Object.op_Equality((Object) this.visualizer, (Object) null))
      return;
    TransformExtensions.SetPosition(this.visualizer.transform, Vector3.op_Addition(TransformExtensions.GetPosition(this.transform), this.offset));
  }

  private void OnViewModeChanged(HashedString mode) => this.Show(mode);

  public void Show(HashedString mode)
  {
    ((Behaviour) this).enabled = this.visible && HashedString.op_Equality(mode, OverlayModes.Disease.ID);
    if (!Object.op_Inequality((Object) this.visualizer, (Object) null))
      return;
    this.visualizer.SetActive(((Behaviour) this).enabled);
  }
}
