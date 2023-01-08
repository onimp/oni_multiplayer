// Decompiled with JetBrains decompiler
// Type: OffscreenIndicator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/OffscreenIndicator")]
public class OffscreenIndicator : KMonoBehaviour
{
  public GameObject IndicatorPrefab;
  public GameObject IndicatorContainer;
  private Dictionary<GameObject, GameObject> targets = new Dictionary<GameObject, GameObject>();
  public static OffscreenIndicator Instance;
  [SerializeField]
  private float edgeInset = 25f;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    OffscreenIndicator.Instance = this;
  }

  protected virtual void OnForcedCleanUp()
  {
    OffscreenIndicator.Instance = (OffscreenIndicator) null;
    base.OnForcedCleanUp();
  }

  private void Update()
  {
    foreach (KeyValuePair<GameObject, GameObject> target in this.targets)
      this.UpdateArrow(target.Value, target.Key);
  }

  public void ActivateIndicator(GameObject target)
  {
    if (this.targets.ContainsKey(target))
      return;
    Tuple<Sprite, Color> uiSprite = Def.GetUISprite((object) target);
    if (uiSprite == null)
      return;
    this.ActivateIndicator(target, uiSprite);
  }

  public void ActivateIndicator(GameObject target, GameObject iconSource)
  {
    if (this.targets.ContainsKey(target))
      return;
    MinionIdentity component = iconSource.GetComponent<MinionIdentity>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    GameObject gameObject = Util.KInstantiateUI(this.IndicatorPrefab, this.IndicatorContainer, true);
    ((Component) gameObject.GetComponent<HierarchyReferences>().GetReference<Image>("icon")).gameObject.SetActive(false);
    CrewPortrait reference = gameObject.GetComponent<HierarchyReferences>().GetReference<CrewPortrait>("Portrait");
    ((Component) reference).gameObject.SetActive(true);
    reference.SetIdentityObject((IAssignableIdentity) component);
    this.targets.Add(target, gameObject);
  }

  public void ActivateIndicator(GameObject target, Tuple<Sprite, Color> icon)
  {
    if (this.targets.ContainsKey(target))
      return;
    GameObject gameObject = Util.KInstantiateUI(this.IndicatorPrefab, this.IndicatorContainer, true);
    Image reference = gameObject.GetComponent<HierarchyReferences>().GetReference<Image>(nameof (icon));
    if (icon == null)
      return;
    reference.sprite = icon.first;
    ((Graphic) reference).color = icon.second;
    this.targets.Add(target, gameObject);
  }

  public void DeactivateIndicator(GameObject target)
  {
    if (!this.targets.ContainsKey(target))
      return;
    Object.Destroy((Object) this.targets[target]);
    this.targets.Remove(target);
  }

  private void UpdateArrow(GameObject arrow, GameObject target)
  {
    if (Object.op_Equality((Object) target, (Object) null))
    {
      Object.Destroy((Object) arrow);
      this.targets.Remove(target);
    }
    else
    {
      Vector3 viewportPoint = Camera.main.WorldToViewportPoint(target.transform.position);
      if ((double) viewportPoint.x > 0.3 && (double) viewportPoint.x < 0.7 && (double) viewportPoint.y > 0.3 && (double) viewportPoint.y < 0.7)
      {
        arrow.GetComponent<HierarchyReferences>().GetReference<CrewPortrait>("Portrait").SetIdentityObject((IAssignableIdentity) null);
        arrow.SetActive(false);
      }
      else
      {
        arrow.SetActive(true);
        TransformExtensions.SetLocalPosition((Transform) Util.rectTransform(arrow), Vector3.zero);
        Vector3 worldPoint = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        worldPoint.z = target.transform.position.z;
        Vector3 vector3 = Vector3.op_Subtraction(target.transform.position, worldPoint);
        Vector3 normalized = ((Vector3) ref vector3).normalized;
        arrow.transform.up = normalized;
        this.UpdateTargetIconPosition(target, arrow);
      }
    }
  }

  private void UpdateTargetIconPosition(GameObject goTarget, GameObject indicator)
  {
    Vector3 vector = Camera.main.WorldToViewportPoint(goTarget.transform.position);
    if ((double) vector.z < 0.0)
    {
      vector.x = 1f - vector.x;
      vector.y = 1f - vector.y;
      vector.z = 0.0f;
      vector = this.Vector3Maxamize(vector);
    }
    Vector3 screenPoint = Camera.main.ViewportToScreenPoint(vector);
    screenPoint.x = Mathf.Clamp(screenPoint.x, this.edgeInset, (float) Screen.width - this.edgeInset);
    screenPoint.y = Mathf.Clamp(screenPoint.y, this.edgeInset, (float) Screen.height - this.edgeInset);
    indicator.transform.position = screenPoint;
    ((Transform) ((Graphic) indicator.GetComponent<HierarchyReferences>().GetReference<Image>("icon")).rectTransform).up = Vector3.up;
    indicator.GetComponent<HierarchyReferences>().GetReference<CrewPortrait>("Portrait").transform.up = Vector3.up;
  }

  public Vector3 Vector3Maxamize(Vector3 vector)
  {
    Vector3 vector3 = vector;
    float num1 = 0.0f;
    float num2 = (double) vector.x > (double) num1 ? vector.x : num1;
    float num3 = (double) vector.y > (double) num2 ? vector.y : num2;
    double num4 = (double) vector.z > (double) num3 ? (double) vector.z : (double) num3;
    return Vector3.op_Division(vector3, (float) num4);
  }
}
