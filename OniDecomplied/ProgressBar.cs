// Decompiled with JetBrains decompiler
// Type: ProgressBar
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/ProgressBar")]
public class ProgressBar : KMonoBehaviour
{
  public Image bar;
  private Func<float> updatePercentFull;
  private int overlayUpdateHandle = -1;
  public bool autoHide = true;

  public Color barColor
  {
    get => ((Graphic) this.bar).color;
    set => ((Graphic) this.bar).color = value;
  }

  public float PercentFull
  {
    get => this.bar.fillAmount;
    set => this.bar.fillAmount = value;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (this.autoHide)
    {
      this.overlayUpdateHandle = Game.Instance.Subscribe(1798162660, new Action<object>(this.OnOverlayChanged));
      if (Object.op_Inequality((Object) OverlayScreen.Instance, (Object) null) && HashedString.op_Inequality(OverlayScreen.Instance.GetMode(), OverlayModes.None.ID))
        ((Component) this).gameObject.SetActive(false);
    }
    Game.Instance.Subscribe(1983128072, new Action<object>(this.OnActiveWorldChanged));
    this.SetWorldActive(ClusterManager.Instance.activeWorldId);
    ((Behaviour) this).enabled = this.updatePercentFull != null;
  }

  private void OnActiveWorldChanged(object data) => this.SetWorldActive(((Tuple<int, int>) data).first);

  private void SetWorldActive(int worldId)
  {
    ((Component) this).gameObject.SetActive(this.GetMyWorldId() == worldId);
    if (this.updatePercentFull != null && !Util.IsNullOrDestroyed(this.updatePercentFull.Target))
      return;
    ((Component) this).gameObject.SetActive(false);
  }

  public void SetUpdateFunc(Func<float> func)
  {
    this.updatePercentFull = func;
    ((Behaviour) this).enabled = this.updatePercentFull != null;
  }

  public virtual void Update()
  {
    if (this.updatePercentFull == null || Util.IsNullOrDestroyed(this.updatePercentFull.Target))
      return;
    this.PercentFull = this.updatePercentFull();
  }

  public virtual void OnOverlayChanged(object data = null)
  {
    if (!this.autoHide)
      return;
    if (HashedString.op_Equality((HashedString) data, OverlayModes.None.ID))
    {
      if (((Component) this).gameObject.activeSelf)
        return;
      ((Component) this).gameObject.SetActive(true);
    }
    else
    {
      if (!((Component) this).gameObject.activeSelf)
        return;
      ((Component) this).gameObject.SetActive(false);
    }
  }

  public void Retarget(GameObject entity)
  {
    Vector3 vector3 = Vector3.op_Addition(TransformExtensions.GetPosition(entity.transform), Vector3.op_Multiply(Vector3.down, 0.5f));
    Building component = entity.GetComponent<Building>();
    TransformExtensions.SetPosition(this.transform, !Object.op_Inequality((Object) component, (Object) null) ? Vector3.op_Subtraction(vector3, Vector3.op_Multiply(Vector3.right, 0.5f)) : Vector3.op_Subtraction(vector3, Vector3.op_Multiply(Vector3.op_Multiply(Vector3.right, 0.5f), (float) (component.Def.WidthInCells % 2))));
  }

  protected virtual void OnCleanUp()
  {
    if (this.overlayUpdateHandle != -1)
      Game.Instance.Unsubscribe(this.overlayUpdateHandle);
    Game.Instance.Unsubscribe(1983128072, new Action<object>(this.OnActiveWorldChanged));
    base.OnCleanUp();
  }

  private void OnBecameInvisible() => ((Behaviour) this).enabled = false;

  private void OnBecameVisible() => ((Behaviour) this).enabled = true;

  public static ProgressBar CreateProgressBar(GameObject entity, Func<float> updateFunc)
  {
    ProgressBar progressBar = Util.KInstantiateUI<ProgressBar>(ProgressBarsConfig.Instance.progressBarPrefab, (GameObject) null, false);
    progressBar.SetUpdateFunc(updateFunc);
    progressBar.transform.SetParent(GameScreenManager.Instance.worldSpaceCanvas.transform);
    ((Object) progressBar).name = (Object.op_Inequality((Object) entity, (Object) null) ? ((Object) entity).name + "_" : "") + " ProgressBar";
    ((Graphic) ((Component) progressBar.transform.Find("Bar")).GetComponent<Image>()).color = ProgressBarsConfig.Instance.GetBarColor(nameof (ProgressBar));
    progressBar.Update();
    progressBar.Retarget(entity);
    return progressBar;
  }
}
